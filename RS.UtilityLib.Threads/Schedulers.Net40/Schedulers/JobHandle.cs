using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Schedulers
{
	/// <summary>
	///     The <see cref="T:Schedulers.JobHandle" /> struct
	///     is used to control and await a scheduled <see cref="T:Schedulers.IJob" />.
	/// </summary>
	public readonly struct JobHandle : IEquatable<JobHandle>
	{
        /// <summary>
        ///     Assigns schedulers an ID, and a cache of tracked jobs.
        ///     This way, we can store a Scheduler and a Job on a JobHandle by integer ID,
        ///     so that stackalloc JobHandle[] can work. Otherwise the managed types would prevent it.
        /// </summary>
        //private static readonly Dictionary<int, (JobScheduler Scheduler, Job[] JobIds)> _schedulerCache = new Dictionary<int, (JobScheduler, Job[])>();
        private static readonly Dictionary<int, Tuple<JobScheduler , Job[] >> _schedulerCache =new Dictionary<int, Tuple<JobScheduler, Job[]>>()  ;
        /// <summary>
        ///     The <see cref="T:Schedulers.JobScheduler" /> used by this scheduled job, as tracked by the ID system.
        /// </summary>
        internal int SchedulerId { get; }

		/// <summary>
		///     The <see cref="P:Schedulers.JobHandle.Job" /> that was associated with the handle on creation, as tracked by the
		///     ID system.
		///     May not be the current job, if the version is expired.
		/// </summary>
		internal long JobId { get; }

		/// <summary>
		///     The job version used by this scheduled job. If this doesn't match <see cref="P:Schedulers.JobHandle.Job" />, it means
		///     the job is completed and the original object was recycled.
		/// </summary>
		internal int Version { get; }

		internal JobScheduler Scheduler
		{
			get
			{
				lock (_schedulerCache)
				{
					if (!_schedulerCache.TryGetValue(SchedulerId, out Tuple<JobScheduler, Job[]> foundScheduler))
					{
						throw new InvalidOperationException("Cannot process a job handle from a disposed scheduler!");
					}
					return foundScheduler.Item1;
				}
			}
		}

		internal Job Job
		{
			get
			{
				lock (_schedulerCache)
				{
					if (!_schedulerCache.TryGetValue(SchedulerId, out Tuple<JobScheduler, Job[]> foundScheduler))
					{
						throw new InvalidOperationException("Cannot process a job handle from a disposed scheduler!");
					}
					Job[] jobIds = foundScheduler.Item2;
					if (JobId >= jobIds.Length)
					{
						throw new InvalidOperationException("Job ID not tracked!");
					}
					return jobIds[JobId];
				}
			}
		}

		/// <summary>
		///     Initialize a new Scheduler with the handle-recycling system. Will spontaneously allocate.
		/// </summary>
		/// <param name="schedulerId">
		///     The ID of the scheduler. Must be unique per scheduler instance, and must never
		///     be recycled.
		/// </param>
		/// <param name="scheduler">The scheduler object.</param>
		/// <param name="jobsCount">The number of jobs to </param>
		internal static void InitializeScheduler(int schedulerId, JobScheduler scheduler, int jobsCount)
		{
			lock (_schedulerCache)
			{
                _schedulerCache[schedulerId] = new Tuple<JobScheduler, Job[]>(scheduler,new Job[jobsCount]);//Tuple<scheduler, new Job[jobsCount]>;
			}
		}

		/// <summary>
		///     Track a newly-created job with the handle-recycling system. Will spontaneously allocate.
		/// </summary>
		/// <param name="schedulerId">The ID of the scheduler to track jobs for.</param>
		/// <param name="job">The job object to track.</param>
		internal static void TrackJob(int schedulerId, Job job)
		{
			lock (_schedulerCache)
			{
                Tuple < JobScheduler, Job[]> cache = _schedulerCache[schedulerId];
				if (job.InstanceId >= cache.Item2.Length)
				{
                    var cache_Item2= cache.Item2;
					Array.Resize(ref cache_Item2, cache.Item2.Length * 2);
					_schedulerCache[schedulerId] = cache;
				}
				cache.Item2[job.InstanceId] = job;
			}
		}

		/// <summary>
		///     Remove a scheduler, and all tracked job IDs.
		///     This will invalidate all existing handles; any methods on them will be invalid.
		/// </summary>
		/// <param name="id"></param>
		internal static void DisposeScheduler(int id)
		{
			lock (_schedulerCache)
			{
				_schedulerCache.Remove(id);
			}
		}

		/// <summary>
		///     Creates a new <see cref="T:Schedulers.JobHandle" /> instance.
		/// </summary>
		/// <param name="schedulerId">The <see cref="T:Schedulers.JobScheduler" /> instance ID.</param>
		/// <param name="version">The current version of the job.</param>
		/// <param name="jobId">The job to assciate with this handle.</param>
		internal JobHandle(int schedulerId, int version, long jobId)
		{
			Version = version;
			SchedulerId = schedulerId;
			JobId = jobId;
		}

		/// <summary>
		///     Waits for the <see cref="T:Schedulers.JobHandle" /> to complete.
		/// </summary>
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		public void Complete()
		{
			Scheduler.Complete(in this);
		}

		/// <summary>
		///     Waits and blocks the calling thread until all <see cref="T:Schedulers.JobHandle" />s are completed.
		/// </summary>
		/// <remarks>
		///     This is equivalent to calling <see cref="M:Schedulers.JobHandle.Complete" /> on each <see cref="T:Schedulers.JobHandle" /> individually.
		/// </remarks>
		/// <param name="handles">The handles to complete.</param>
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		public static void CompleteAll(ReadOnlySpan<JobHandle> handles)
		{
			ReadOnlySpan<JobHandle> readOnlySpan = handles;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				JobHandle handle = readOnlySpan[i];
				handle.Complete();
			}
		}

		/// <inheritdoc cref="M:Schedulers.JobHandle.CompleteAll(System.ReadOnlySpan{Schedulers.JobHandle})" />
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		public static void CompleteAll(IList<JobHandle> handles)
		{
			for (int i = 0; i < handles.Count; i++)
			{              
				handles[i].Complete();
			}
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is JobHandle handle && Equals(handle);
		}

		/// <inheritdoc />
		public bool Equals(JobHandle other)
		{
			return SchedulerId == other.SchedulerId && JobId == other.JobId && Version == other.Version;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine(SchedulerId, JobId, Version);
		}

		/// <inheritdoc />
		public static bool operator ==(JobHandle left, JobHandle right)
		{
			return left.Equals(right);
		}

		/// <inheritdoc />
		public static bool operator !=(JobHandle left, JobHandle right)
		{
			return !(left == right);
		}
	}
}
