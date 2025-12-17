#define DEBUG
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Schedulers.Deque;

namespace Schedulers
{
    /// <summary>
    ///     A <see cref="T:Schedulers.JobScheduler" /> schedules and processes <see cref="T:Schedulers.IJob" />s asynchronously. Better-suited for larger jobs due to its underlying events. 
    /// </summary>
    public class JobScheduler : IDisposable
    {
        /// <summary>
        /// Contains configuration settings for <see cref="T:Schedulers.JobScheduler" />.
        /// </summary>
        public struct Config
        {
            /// <summary>
            /// Defines the maximum expected number of concurrent jobs. Increasing this number will allow more jobs to be scheduled
            /// without spontaneous allocation, but will increase total memory consumption and decrease performance.
            /// If unset, the default is <c>32</c>
            /// </summary>
            public int MaxExpectedConcurrentJobs { get; set; }

            /// <summary>
            /// Whether to use Strict Allocation Mode for this <see cref="T:Schedulers.JobScheduler" />. If an allocation might occur, the JobScheduler
            /// will throw a <see cref="T:Schedulers.JobScheduler.MaximumConcurrentJobCountExceededException" />.
            /// Not recommended for production environments (spontaneous allocation is probably usually better than crashing the program).
            /// </summary>
            public bool StrictAllocationMode { get; set; }

            /// <summary>
            /// The process name to use for spawned child threads. By default, set to the current domain's <see cref="P:System.AppDomain.FriendlyName" />.
            /// Thread will be named "prefix0" for the first thread, "prefix1" for the second thread, etc.
            /// </summary>
            public string ThreadPrefixName { get; set; }

            /// <summary>
            /// The amount of worker threads to use. By default, set to <see cref="P:System.Environment.ProcessorCount" />, the amount of hardware processors 
            /// available on the system.
            /// </summary>
            public int ThreadCount { get; set; }

            /// <summary>
            /// Create a new <see cref="T:Schedulers.JobScheduler.Config" /> for a <see cref="T:Schedulers.JobScheduler" /> with all default settings.
            /// </summary>
            //public Config()
            //{
            //	MaxExpectedConcurrentJobs = 32;
            //	StrictAllocationMode = false;
            //	ThreadPrefixName = AppDomain.CurrentDomain.FriendlyName;
            //	ThreadCount = Environment.ProcessorCount;
            //}
            static Config _defaultConfig = new Config()
            {
                MaxExpectedConcurrentJobs = 32,
                StrictAllocationMode = false,
                ThreadPrefixName = AppDomain.CurrentDomain.FriendlyName,
                ThreadCount = Environment.ProcessorCount,
            };
            public static Config Default { get { return _defaultConfig; } }
        }

        /// <summary>
        /// Thrown when <see cref="P:Schedulers.JobScheduler.Config.StrictAllocationMode" /> is enabled and the <see cref="T:Schedulers.JobScheduler" /> goes over its <see cref="P:Schedulers.JobScheduler.Config.MaxExpectedConcurrentJobs" />.
        /// </summary>
        public class MaximumConcurrentJobCountExceededException : Exception
        {
            internal MaximumConcurrentJobCountExceededException()
                : base("JobScheduler has gone over its MaxExpectedConcurrentJobs value! Increase that value or disable StrictAllocationMode to allow spontaneous allocations.")
            {
            }
        }

        private class Notifier
        {
            private readonly AutoResetEvent _singleNotifier = new AutoResetEvent(initialState: false);

            private readonly ManualResetEvent _multipleNotifier = new ManualResetEvent(initialState: false);

            private readonly WaitHandle[] _both;

            private readonly object _disposeLock = new object();

            public bool IsDisposed { get; private set; } = false;


            public Notifier()
            {
                _both = new WaitHandle[2];
                _both[0] = _singleNotifier;
                _both[1] = _multipleNotifier;
            }

            public void Wait()
            {
                WaitHandle.WaitAny(_both);
            }

            public void NotifyOne()
            {
                _singleNotifier.Set();
            }

            public void NotifyAll()
            {
                lock (_disposeLock)
                {
                    if (!IsDisposed)
                    {
                        _multipleNotifier.Set();
                    }
                }
            }

            public void Dispose()
            {
                lock (_disposeLock)
                {
                    if (!IsDisposed)
                    {
                        IsDisposed = true;
                        _singleNotifier.Dispose();
                        _multipleNotifier.Dispose();
                    }
                }
            }
        }

        private class WorkerData
        {
            public int Id { get; }

            public Job Cache { get; set; } = null;


            public WorkStealingDeque<Job> Deque { get; }

            public List<Job> ReadyDependencyCache { get; }

            public WorkerData(int id, int maxJobs)
            {
                Id = id;
                ReadyDependencyCache = new List<Job>(maxJobs - 1);
                Deque = new WorkStealingDeque<Job>(maxJobs);
            }
        }

        private int _threadsAlive = 0;

        private readonly bool _strictAllocationMode;

        private readonly int _maxConcurrentJobs;

        private readonly List<JobHandle> _dependencyCache;

        private readonly ConcurrentQueue<Job> _jobPool;

        private static int _lastInstanceId = -1;

        private int _lastJobId = -1;

        private int _stealBound = 0;

        private readonly int _yieldBound = 100;

        private int _numActives = 0;

        private int _numThieves = 0;

        private readonly Notifier _notifier = new Notifier();

        private readonly Random _random = new Random();

        private WorkerData[] _workers = null;

        private CancellationToken _token;

        internal int ThreadsAlive => _threadsAlive;

        /// <summary>
        /// The actual number of threads this <see cref="T:Schedulers.JobScheduler" /> was spawned with.
        /// </summary>
        public int ThreadCount { get; }

        /// <summary>
        ///     A unique ID for this particular Scheduler
        /// </summary>
        internal int InstanceId { get; }

        /// <summary>
        /// Tracks which thread the JobScheduler was constructed on
        /// </summary>
        private int MainThreadID { get; }

        /// <summary>
        /// Tracks the overall state of all threads; when canceled in Dispose, all child threads are exited
        /// </summary>
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();


        /// <summary>
        /// Jobs scheduled by the scheduler (NOT other jobs), but not yet flushed to the threads
        /// </summary>
        private List<Job> QueuedJobs { get; }

        /// <summary>
        /// Returns true if this is the main thread the scheduler was created on; false otherwise
        /// </summary>
        public bool IsMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadID;

        /// <summary>
        /// Jobs flushed and waiting to be picked up by worker threads
        /// </summary>
        private ConcurrentQueue<Job> MasterQueue { get; }

        /// <summary>
        /// Creates an instance of the <see cref="T:Schedulers.JobScheduler" />
        /// </summary>
        /// <param name="settings">The <see cref="T:Schedulers.JobScheduler.Config" /> to use for this instance of <see cref="T:Schedulers.JobScheduler" /></param>
        public JobScheduler(in Config settings)
        {
            InstanceId = Interlocked.Increment(ref _lastInstanceId);
            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            ThreadCount = settings.ThreadCount;
            if (ThreadCount <= 0)
            {
                ThreadCount = Environment.ProcessorCount;
            }
            _strictAllocationMode = settings.StrictAllocationMode;
            _maxConcurrentJobs = settings.MaxExpectedConcurrentJobs;
            _dependencyCache = new List<JobHandle>(settings.MaxExpectedConcurrentJobs - 1);
            JobHandle.InitializeScheduler(InstanceId, this, _maxConcurrentJobs);
            QueuedJobs = new List<Job>(settings.MaxExpectedConcurrentJobs);
            for (int k = 0; k < settings.MaxExpectedConcurrentJobs; k++)
            {
                QueuedJobs.Add(null);
            }
            MasterQueue = new ConcurrentQueue<Job>(QueuedJobs);
            _jobPool = new ConcurrentQueue<Job>(QueuedJobs);
            Job result;
            while (!MasterQueue.IsEmpty)
            {
                MasterQueue.TryDequeue(out result);
            }
            while (!_jobPool.IsEmpty)
            {
                _jobPool.TryDequeue(out result);
            }
            QueuedJobs.Clear();
            for (int j = 0; j < settings.MaxExpectedConcurrentJobs; j++)
            {
                Job l = new Job(settings.MaxExpectedConcurrentJobs - 1, ThreadCount, this, Interlocked.Increment(ref _lastJobId));
                JobHandle.TrackJob(InstanceId, l);
            }
            InitAlgorithm(ThreadCount, _maxConcurrentJobs, CancellationTokenSource.Token);
            _threadsAlive = ThreadCount;
            for (int i = 0; i < ThreadCount; i++)
            {
                int c = i;
                Thread thread = new Thread(WorkerLoop)
                {
                    Name = $"{settings.ThreadPrefixName}{i}"
                };
                thread.Start(i);
            }
        }

        private Job GetPooledJob()
        {
            Job job;
            while (!_jobPool.TryDequeue(out job))
            {
                if (_strictAllocationMode)
                {
                    throw new MaximumConcurrentJobCountExceededException();
                }
                Job i = new Job(0, ThreadCount, this, Interlocked.Increment(ref _lastJobId));
                JobHandle.TrackJob(InstanceId, i);
            }
            return job;
        }

        /// <summary>
        ///     Schedules a <see cref="T:Schedulers.IJob" /> and returns its <see cref="T:Schedulers.JobHandle" />.
        /// </summary>
        /// <param name="work">The <see cref="T:Schedulers.IJob" />.</param>
        /// <param name="dependency">The <see cref="T:Schedulers.JobHandle" />-Dependency.</param>
        /// <param name="dependencies">A list of additional <see cref="T:Schedulers.JobHandle" />-Dependencies.</param>
        /// <param name="parallelWork">A parallel job, if we want to schedule one.</param>
        /// <param name="amount">The amount of times to run the parallel job.</param>
        /// <returns>A <see cref="T:Schedulers.JobHandle" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">If called on a different thread than the <see cref="T:Schedulers.JobScheduler" /> was constructed on</exception>
        /// <exception cref="T:Schedulers.JobScheduler.MaximumConcurrentJobCountExceededException">If the maximum amount of concurrent jobs is at maximum, and strict mode is enabled.</exception>
        private JobHandle Schedule(IJob work, JobHandle? dependency = null, ReadOnlySpan<JobHandle> dependencies = default(ReadOnlySpan<JobHandle>), IJobParallelFor parallelWork = null, int amount = 0)
        {
            if (!IsMainThread)
            {
                throw new InvalidOperationException("Can only call Schedule from the thread that spawned the JobScheduler!");
            }
            _dependencyCache.Clear();
            ReadOnlySpan<JobHandle> readOnlySpan = dependencies;
            for (int j = 0; j < readOnlySpan.Length; j++)
            {
                JobHandle d = readOnlySpan[j];
                _dependencyCache.Add(d);
            }
            if (dependency.HasValue)
            {
                _dependencyCache.Add(dependency.Value);
            }
            JobHandle? handle = null;
            if (parallelWork == null)
            {
                Job pooledJob = GetPooledJob();
                handle = pooledJob.Schedule(work, _dependencyCache, out var ready2);
                if (ready2)
                {
                    QueuedJobs.Add(pooledJob);
                }
            }
            else
            {
                if (parallelWork.BatchSize <= 0)
                {
                    throw new ArgumentOutOfRangeException("BatchSize");
                }
                int threads = ((parallelWork.ThreadCount <= 0) ? ThreadCount : Math.Min(parallelWork.ThreadCount, ThreadCount));
                int batches = (int)Math.Ceiling((float)amount / (float)parallelWork.BatchSize);
                threads = Math.Min(threads, batches);
                for (int i = 0; i < threads; i++)
                {
                    Job pooledJob2 = GetPooledJob();
                    bool ready;
                    if (i == 0)
                    {
                        handle = pooledJob2.Schedule(work, _dependencyCache, out ready, parallelWork, null, amount, threads, i);
                    }
                    else
                    {
                        pooledJob2.Schedule(work, _dependencyCache, out ready, parallelWork, handle, amount, threads, i);
                    }
                    if (ready)
                    {
                        QueuedJobs.Add(pooledJob2);
                    }
                }
            }
            Debug.Assert(handle.HasValue);
            return handle.Value;
        }

        /// <summary>
        ///     Schedules an <see cref="T:Schedulers.IJob" />. It is only queued up, and will only begin processing when the user calls <see cref="M:Schedulers.JobScheduler.Flush" /> or when any in-progress dependencies complete.
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <param name="dependency">A job that must complete before this job can be run</param>
        /// <returns>Its <see cref="T:Schedulers.JobHandle" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">If called on a different thread than the <see cref="T:Schedulers.JobScheduler" /> was constructed on</exception>
        /// <exception cref="T:Schedulers.JobScheduler.MaximumConcurrentJobCountExceededException">If the maximum amount of concurrent jobs is at maximum, and strict mode is enabled.</exception>
        public JobHandle Schedule(IJob job, JobHandle? dependency = null)
        {
            if (dependency.HasValue)
            {
                CheckForSchedulerEquality(dependency.Value);
            }
            return Schedule(job, dependency, null);
        }

        /// <summary>
        ///     Schedules an <see cref="T:Schedulers.IJobParallelFor" />. It is only queued up, and will only begin processing when the user calls
        /// <see cref="M:Schedulers.JobScheduler.Flush" /> or when any in-progress dependencies complete.
        /// </summary>
        /// <remarks>
        ///     Note that this will schedule as many jobs as specified in <see cref="T:Schedulers.IJobParallelFor" /> or the maximum thread count, whichever is less
        ///     (or the maximum thread count if the threads provided are 0). See <see cref="T:Schedulers.IJobParallelFor" /> for details.
        /// </remarks>
        /// <param name="job">The <see cref="T:Schedulers.IJobParallelFor" /> to schedule.</param>
        /// <param name="amount">
        ///     The amount of indices to execute.
        ///     <see cref="M:Schedulers.IJobParallelFor.Execute(System.Int32)" /> will be called for each value in <c>[0, <paramref name="amount" />)</c>.
        /// </param>
        /// <param name="dependency">A <see cref="T:Schedulers.JobHandle" /> dependency to require completion of first.</param>
        /// <returns>The <see cref="T:Schedulers.JobHandle" /> of a job representing the full task.</returns>
        /// <exception cref="T:System.InvalidOperationException">If called on a different thread than the <see cref="T:Schedulers.JobScheduler" /> was constructed on</exception>
        /// <exception cref="T:Schedulers.JobScheduler.MaximumConcurrentJobCountExceededException">If the maximum amount of concurrent jobs is at maximum, and strict mode is enabled.</exception>
        public JobHandle Schedule(IJobParallelFor job, int amount, JobHandle? dependency = null)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }
            if (dependency.HasValue)
            {
                CheckForSchedulerEquality(dependency.Value);
            }
            return Schedule(null, dependency, null, job, amount);
        }

        /// <summary>
        ///     Combine multiple dependencies into a single <see cref="T:Schedulers.JobHandle" /> which is scheduled.
        /// </summary>
        /// <param name="dependencies">A list of handles to depend on. Assumed to not contain duplicates.</param>
        /// <returns>The combined <see cref="T:Schedulers.JobHandle" /></returns>
        /// <exception cref="T:System.InvalidOperationException">If called on a different thread than the <see cref="T:Schedulers.JobScheduler" /> was constructed on</exception>
        /// <exception cref="T:Schedulers.JobScheduler.MaximumConcurrentJobCountExceededException">If the maximum amount of concurrent jobs is at maximum, and strict mode is enabled.</exception>
        public JobHandle CombineDependencies(ReadOnlySpan<JobHandle> dependencies)
        {
            ReadOnlySpan<JobHandle> readOnlySpan = dependencies;
            for (int i = 0; i < readOnlySpan.Length; i++)
            {
                JobHandle dependency = readOnlySpan[i];
                CheckForSchedulerEquality(dependency);
            }
            return Schedule(null, null, dependencies);
        }

        /// <summary>
        ///     Checks if the passed <see cref="T:Schedulers.JobHandle" /> equals this <see cref="T:Schedulers.JobScheduler" />.
        /// </summary>
        /// <param name="dependency">The <see cref="T:Schedulers.JobHandle" />.</param>
        /// <exception cref="T:System.InvalidOperationException">Is thrown when the passed handle has a different scheduler.</exception>
        private void CheckForSchedulerEquality(JobHandle dependency)
        {
            if (dependency.Scheduler != this)
            {
                throw new InvalidOperationException("Job dependency was scheduled with a different JobScheduler!");
            }
        }

        /// <summary>
        /// Flushes all queued <see cref="T:Schedulers.IJob" />'s to the worker threads. 
        /// </summary>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Flush()
        {
            if (!IsMainThread)
            {
                throw new InvalidOperationException("Can only call Flush from the thread that spawned the JobScheduler!");
            }
            if (QueuedJobs.Count == 0)
            {
                return;
            }
            foreach (Job job in QueuedJobs)
            {
                MasterQueue.Enqueue(job);
            }
            QueuedJobs.Clear();
            _notifier.NotifyOne();
        }

        /// <summary>
        /// Blocks the thread until the given job ID has been completed. Can be called from Jobs.
        /// </summary>
        /// <param name="handle"></param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        internal void Complete(in JobHandle handle)
        {
            Job job = handle.Job;
            if (job.TrySubscribe(handle.Version, out ManualResetEvent waitHandle))
            {
                waitHandle.WaitOne();
                job.Unsubscribe(handle.Version);
            }
        }

        /// <summary>
        /// Called exclusively from <see cref="T:Schedulers.Job" /> when it wants to pool itself.
        /// </summary>
        /// <param name="job"></param>
        internal void PoolJob(Job job)
        {
            _jobPool.Enqueue(job);
        }

        /// <summary>
        /// Disposes all internals and notifies all threads to cancel.
        /// </summary>
        public void Dispose()
        {
            if (!IsMainThread)
            {
                throw new InvalidOperationException("Can only call Dispose from the thread that spawned the JobScheduler!");
            }
            CancellationTokenSource.Cancel(throwOnFirstException: false);
            QueuedJobs.Clear();
            Clear(MasterQueue);
            _notifier.NotifyAll();
            JobHandle.DisposeScheduler(InstanceId);
        }
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear<T>(ConcurrentQueue<T> concurrentQueue)
        {
            if (concurrentQueue == null)
            {
                throw new NullReferenceException();
            }
            T result;
            while (concurrentQueue.TryDequeue(out result))
            {
                // Empty
            }
        }
        private void InitAlgorithm(int threadCount, int maxJobs, CancellationToken token)
        {
            _stealBound = 2 * (threadCount - 1);
            _workers = new WorkerData[threadCount];
            for (int i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new WorkerData(i, maxJobs);
            }
            _token = token;
        }

        private void WorkerLoop(object data)
        {
            int worker = (int)data;
            Job task = null;
            WorkerData workerData = _workers[worker];
            do
            {
                ExploitTask(ref task, workerData);
            }
            while (WaitForTask(ref task, workerData));
            if (Interlocked.Decrement(ref _threadsAlive) == 0)
            {
                _notifier.Dispose();
            }
        }

        private void Execute(in Job task, WorkerData workerData)
        {
            List<Job> readyDependencies = workerData.ReadyDependencyCache;
            readyDependencies.Clear();
            task.Execute(readyDependencies);
            if (readyDependencies.Count > 0)
            {
                workerData.Cache = readyDependencies[0];
                for (int i = 1; i < readyDependencies.Count; i++)
                {
                    Job task2 = readyDependencies[i];
                    Push(in task2, workerData);
                }
            }
            else
            {
                workerData.Cache = null;
            }
        }

        private void Pop(out Job task, WorkerData workerData)
        {
            task = (workerData.Deque.TryPopBottom(out Job popped) ? popped : null);
        }

        private void Push(in Job task, WorkerData workerData)
        {
            workerData.Deque.PushBottom(task);
        }

        private void StealFrom(out Job task, WorkerData workerData)
        {
            task = (workerData.Deque.TrySteal(out Job stolen) ? stolen : null);
        }

        /// <summary>
        /// Resolves this thread's entire deque and cache; returns when empty;
        /// </summary>
        /// <remarks>
        /// Based on Algorithm 3 of Lin et al. [1]
        /// </remarks>
        /// <param name="task"></param>
        /// <param name="workerData"></param>
        private void ExploitTask(ref Job task, WorkerData workerData)
        {
            if (Interlocked.Increment(ref _numActives) == 1 && _numThieves == 0)
            {
                _notifier.NotifyOne();
            }
            do
            {
                if (task != null)
                {
                    Execute(in task, workerData);
                }
                if (workerData.Cache != null)
                {
                    task = workerData.Cache;
                }
                else
                {
                    Pop(out task, workerData);
                }
            }
            while (task != null);
            Interlocked.Decrement(ref _numActives);
        }

        /// <summary>
        /// Steals or waits for a task.
        /// </summary>
        /// <remarks>
        /// Based on Algorithm 5 of Lin et al. [1]
        /// </remarks>
        /// <param name="task"></param>
        /// <param name="workerData"></param>
        /// <returns></returns>
        private bool WaitForTask(ref Job task, WorkerData workerData)
        {
            do
            {
                Interlocked.Increment(ref _numThieves);
                ExploreTask(ref task, workerData);
                if (task != null)
                {
                    if (Interlocked.Decrement(ref _numThieves) == 0)
                    {
                        _notifier.NotifyOne();
                    }
                    return true;
                }
                if (MasterQueue.TryDequeue(out Job stolenTask))
                {
                    task = stolenTask;
                    if (Interlocked.Decrement(ref _numThieves) == 0)
                    {
                        _notifier.NotifyOne();
                    }
                    return true;
                }
                if (_token.IsCancellationRequested)
                {
                    _notifier.NotifyAll();
                    Interlocked.Decrement(ref _numThieves);
                    return false;
                }
            }
            while (Interlocked.Decrement(ref _numThieves) == 0 && _numActives > 0);
            _notifier.Wait();
            return true;
        }

        /// <summary>
        /// Runs the stealing algorithm, the key insight of Lin et al.
        /// It steals some number of times, then begins yielding between steals, and then after a number of failed yields,
        /// returns. If at any point it finds a task, it sets <paramref name="task" /> to the found task and returns.
        /// </summary>
        /// <remarks>
        /// Based on Algorithm 4 of Lin et al. [1]
        /// </remarks>
        /// <param name="task"></param>
        /// <param name="workerData"></param>
        private void ExploreTask(ref Job task, WorkerData workerData)
        {
            int numFailedSteals = 0;
            int numYields = 0;
            while (!_token.IsCancellationRequested)
            {
                WorkerData victim = GetRandomThread();
                if (victim.Id == workerData.Id)
                {
                    if (MasterQueue.TryDequeue(out Job stolen))
                    {
                        task = stolen;
                    }
                }
                else
                {
                    StealFrom(out task, victim);
                }
                if (task != null)
                {
                    break;
                }
                numFailedSteals++;
                if (numFailedSteals >= _stealBound)
                {
                    Thread.Yield();
                    numYields++;
                    if (numYields == _yieldBound)
                    {
                        break;
                    }
                }
            }
        }

        private WorkerData GetRandomThread()
        {
            int worker = _random.Next(0, _workers.Length);
            return _workers[worker];
        }
    }
}
