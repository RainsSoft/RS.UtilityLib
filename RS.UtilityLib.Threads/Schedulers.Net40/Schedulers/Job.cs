//#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Schedulers.Deque;

namespace Schedulers
{
	/// <summary>
	///     The <see cref="T:Schedulers.Job" /> struct
	///     represents the Job itself with all its most important data. 
	/// </summary>
	internal class Job
	{
		private static readonly XorshiftRandom _random = new XorshiftRandom();

		private readonly JobScheduler _scheduler;

		private int _version = 0;

		private IJob _work;

		private IJobParallelFor _parallelWork;

		private JobHandle? _masterJob;

		private int _parallelJobID;

		private volatile int _parallelSubscribers;

		private readonly RangeWorkStealingDeque[] _workerDeques;

		private readonly ManualResetEvent _waitHandle;

		private readonly List<Job> _dependents;

		private int _dependencyCount = 0;

		private int _waitHandleSubscriptionCount = 0;

		private bool _isComplete = false;

		private readonly object _jobLock = new object();

		/// <summary>
		///     An unchanging ID that goes with the Job instance. Used primarily by <see cref="T:Schedulers.JobHandle" />
		///     for looking up jobs.
		/// </summary>
		internal long InstanceId { get; }

		/// <summary>
		/// Create a new <see cref="T:Schedulers.Job" /> with dependent capacity <paramref name="dependentCapacity" />, ready for scheduling.
		/// Automatically adds itself to the <see cref="T:Schedulers.JobScheduler" />'s pool, meaning you should acquire it only from the pool
		/// and never use a <see cref="T:Schedulers.Job" /> straight from construction.
		/// </summary>
		/// <param name="dependentCapacity"></param>
		/// <param name="threadCapacity"></param>
		/// <param name="scheduler">The scheduler this <see cref="T:Schedulers.Job" /> was created with.</param>
		public Job(int dependentCapacity, int threadCapacity, JobScheduler scheduler, int instanceId)
		{
			_waitHandle = new ManualResetEvent(initialState: false);
			_scheduler = scheduler;
			_dependents = new List<Job>(dependentCapacity);
			_workerDeques = new RangeWorkStealingDeque[threadCapacity];
			InstanceId = instanceId;
			for (int i = 0; i < threadCapacity; i++)
			{
				_workerDeques[i] = new RangeWorkStealingDeque();
			}
			PoolSelf();
		}

		/// <summary>
		/// Schedule a new instance of this job. It must be fresh out of the pool.
		/// </summary>
		/// <param name="work"></param>
		/// <param name="dependencies"></param>
		/// <param name="ready"></param>
		/// <param name="parallelWork"></param>
		/// <param name="masterJob"></param>
		/// <param name="amount"></param>
		/// <param name="totalThreads"></param>
		/// <param name="thisThread"></param>
		/// <returns></returns>
		public JobHandle Schedule(IJob work, List<JobHandle> dependencies, out bool ready, IJobParallelFor parallelWork = null, JobHandle? masterJob = null, int amount = 0, int totalThreads = 0, int thisThread = 0)
		{
			lock (_jobLock)
			{
				if (parallelWork != null)
				{
					if (work != null)
					{
						throw new ArgumentOutOfRangeException("amount");
					}
					if (amount <= 0)
					{
						throw new ArgumentOutOfRangeException("amount");
					}
					_masterJob = masterJob;
					_parallelWork = parallelWork;
					_parallelJobID = thisThread;
					if (!masterJob.HasValue)
					{
						Debug.Assert(thisThread == 0);
						int baseAmount = amount / totalThreads;
						int remainder = amount % totalThreads;
						int start = 0;
						_workerDeques[0].Set(start, baseAmount + remainder, _parallelWork.BatchSize);
						start += baseAmount + remainder;
						for (int i = 1; i < totalThreads; i++)
						{
							int end = start + baseAmount;
							_workerDeques[i].Set(start, baseAmount, _parallelWork.BatchSize);
							start = end;
						}
					}
				}
				Debug.Assert(_dependencyCount == 0);
				foreach (JobHandle handle in dependencies)
				{
					Job otherJob = handle.Job;
					lock (otherJob._jobLock)
					{
						if (handle.Version == otherJob._version && !otherJob._isComplete)
						{
							otherJob._dependents.Add(this);
							_dependencyCount++;
						}
					}
				}
				ready = _dependencyCount == 0;
				_work = work;
				JobHandle thisHandle = new JobHandle(_scheduler.InstanceId, _version, InstanceId);
				if (_parallelWork != null)
				{
					JobHandle? masterJob2 = _masterJob;
					if (!masterJob2.HasValue)
					{
						_masterJob = thisHandle;
					}
				}
				return thisHandle;
			}
		}

		/// <summary>
		/// Execute the job. Fills <paramref name="readyDependents" /> with any dependents who are newly ready because of us.
		/// </summary>
		/// <param name="readyDependents"></param>
		public void Execute(List<Job> readyDependents)
		{
			_work?.Execute();
			if (_parallelWork != null)
			{
				JobHandle? masterJob = _masterJob;
				Debug.Assert(masterJob.HasValue);
				Debug.Assert(_work == null);
				if (_masterJob.Value.Job.TrySubscribeToParallel(_masterJob.Value.Version))
				{
					RangeWorkStealingDeque[] workerDeques = _masterJob.Value.Job._workerDeques;
					Range range;
					while (workerDeques[_parallelJobID].TryPopBottom(out range) == RangeWorkStealingDeque.Status.Success)
					{
						for (int j = range.Start.Value; j < range.End.Value; j++)
						{
							_parallelWork.Execute(j);
						}
					}
					while (true)
					{
						RangeWorkStealingDeque victim = null;
						int start = _random.Next(0, workerDeques.Length);
						int i = start;
						do
						{
							RangeWorkStealingDeque vic = workerDeques[i];
							if (!vic.IsEmpty)
							{
								victim = vic;
								break;
							}
							i++;
							if (i >= workerDeques.Length)
							{
								i = 0;
							}
						}
						while (i != start);
						if (victim == null)
						{
							break;
						}
						Range range2;
						while (victim.TrySteal(out range2) != 0)
						{
							for (int r = range2.Start.Value; r < range2.End.Value; r++)
							{
								_parallelWork.Execute(r);
							}
						}
					}
					_masterJob.Value.Job.UnsubscribeFromParallel(_masterJob.Value.Version);
					RangeWorkStealingDeque[] array = workerDeques;
					foreach (RangeWorkStealingDeque deque2 in array)
					{
						Debug.Assert(deque2.IsEmpty);
					}
				}
				if (_masterJob.Value.Job == this && _parallelSubscribers > 0)
				{
					SpinWait spin = default(SpinWait);
					RangeWorkStealingDeque[] workerDeques2 = _workerDeques;
					foreach (RangeWorkStealingDeque deque in workerDeques2)
					{
						Debug.Assert(deque.IsEmpty);
					}
					while (_parallelSubscribers > 0)
					{
						spin.SpinOnce();
					}
				}
				if (_masterJob.Value.Job == this)
				{
					_parallelWork.Finish();
				}
			}
			lock (_jobLock)
			{
				_isComplete = true;
				foreach (Job dependent in _dependents)
				{
					lock (dependent._jobLock)
					{
						dependent._dependencyCount--;
						if (dependent._dependencyCount == 0)
						{
							readyDependents.Add(dependent);
						}
					}
				}
				if (_waitHandleSubscriptionCount != 0)
				{
					_waitHandle.Set();
				}
				else
				{
					PoolSelf();
				}
			}
		}

		private void UnsubscribeFromParallel(int version)
		{
			lock (_jobLock)
			{
				Debug.Assert(version == _version);
				_parallelSubscribers--;
			}
		}

		private bool TrySubscribeToParallel(int version)
		{
			lock (_jobLock)
			{
				if (_version != version || _isComplete)
				{
					return false;
				}
				bool foundDeque = false;
				RangeWorkStealingDeque[] workerDeques = _workerDeques;
				foreach (RangeWorkStealingDeque deque in workerDeques)
				{
					if (!deque.IsEmpty)
					{
						foundDeque = true;
					}
				}
				if (!foundDeque)
				{
					return false;
				}
				_parallelSubscribers++;
				return true;
			}
		}

		/// <summary>
		/// Prepare for a subscribe to our <see cref="T:System.Threading.ManualResetEvent" />.
		/// Returns true if the handle is available for subscription (i.e. the job is still incomplete).
		/// If this returns true, the caller must call <see cref="M:Schedulers.Job.Unsubscribe(System.Int32)" />, and may not wait on the handle
		/// after <see cref="M:Schedulers.Job.Unsubscribe(System.Int32)" /> is called.
		/// </summary>
		/// <param name="version"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		public bool TrySubscribe(int version, out ManualResetEvent handle)
		{
			handle = null;
			lock (_jobLock)
			{
				if (_version != version || _isComplete)
				{
					return false;
				}
				_waitHandleSubscriptionCount++;
				handle = _waitHandle;
				return true;
			}
		}

		/// <summary>
		///     Unsubscribe from a particular wait. Call only after <see cref="M:Schedulers.Job.TrySubscribe(System.Int32,System.Threading.ManualResetEvent@)" /> has returned true, and all
		///     the handle-waiting is done.
		/// </summary>
		/// <param name="version"></param>
		public void Unsubscribe(int version)
		{
			lock (_jobLock)
			{
				Debug.Assert(version == _version);
				_waitHandleSubscriptionCount--;
				if (_waitHandleSubscriptionCount == 0)
				{
					PoolSelf();
				}
			}
		}

		private void PoolSelf()
		{
			_version++;
			_parallelWork = null;
			_parallelSubscribers = 0;
			RangeWorkStealingDeque[] workerDeques = _workerDeques;
			foreach (RangeWorkStealingDeque deque in workerDeques)
			{
				Debug.Assert(deque.IsEmpty);
			}
			_dependents.Clear();
			_dependencyCount = 0;
			_waitHandleSubscriptionCount = 0;
			_isComplete = false;
			_waitHandle.Reset();
			_work = null;
			_scheduler.PoolJob(this);
		}
	}
}
