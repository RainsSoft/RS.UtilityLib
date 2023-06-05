using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RS.UtilityLib.Unity_WebRTC
{
    public delegate void Action();
    static class MethodImplOptionsEx
    {
        public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;

        public const MethodImplOptions ForwardRef = (MethodImplOptions)16;

        public const MethodImplOptions InternalCall = (MethodImplOptions)4096;

        public const MethodImplOptions NoInlining = (MethodImplOptions)8;

        public const MethodImplOptions NoOptimization = (MethodImplOptions)64;

        public const MethodImplOptions PreserveSig = (MethodImplOptions)128;

        public const MethodImplOptions SecurityMitigations = (MethodImplOptions)1024;

        public const MethodImplOptions Synchronized = (MethodImplOptions)32;

        public const MethodImplOptions Unmanaged = (MethodImplOptions)4;
    }
    /// <summary>
    /// 
    /// SynchronizationContext提供了一种编写可以在许多不同框架中工作的组件的方法。BackgroundWorker和WebClient是Windows Forms，WPF，Silverlight，控制台和ASP.NET应用程序中同样常见的两个示例。
    /// 注意适应范围,Unity主线程UnitySynchronizationContext/System.Window.Form界面线程WindowsFormsSynchronizationContext/ WPF界面DispatcherSynchronizationContext
    ///基于UI的SynchronizationContext实现确实满足这些条件，但是ASP.NET SynchronizationContext仅提供同步。默认的SynchronizationContext不保证执行顺序或同步顺序。
    ///SynchronizationContext实例与线程之间没有1：1的对应关系。
    ///WindowsFormsSynchronizationContext确实具有到线程的1：1映射（只要不调用SynchronizationContext.CreateCopy即可），但这在任何其他实现中均不成立。通常，最好不要假定任何上下文实例都可以在任何特定线程上运行。
    ///SynchronizationContext.Post方法不一定是异步的。
    ///大多数实现都异步实现它，但是AspNetSynchronizationContext是一个明显的例外。这可能会导致意外的重新进入问题。
    ///AspNetSynchronizationContext???
    /// 一个可执行文件（xref:System.Threading.SynchronizationContext），设计用于包装当前主线程（xref:System.Thredding.Synchronzationcontext）
    /// An executable (xref: System.Threading.SynchronizationContext) designed to wrap the current main thread
    /// (xref: System.Threading.SynchronizationContext).
    /// </summary>
    /// <remarks>
    /// 函数类似于UnitySynchronizationContext，只是它允许手动调用主线程上的任务执行。
    /// Functions similarly to the UnitySynchronizationContext except it allows task execution on the main thread to be
    /// invoked manually.
    /// </remarks>
    public class ExecutableUnitySynchronizationContext : SynchronizationContext
    {
        //同步上下文详情：https://blog.csdn.net/kalvin_y_liu/article/details/117787437
        /*
            默认SynchronizationContext是默认构造的SynchronizationContext对象。根据惯例，如果线程的当前SynchronizationContext为null，则它隐式具有默认的SynchronizationContext。
            默认的同步上下文将异步委托排队到ThreadPool，但直接在调用线程上执行其同步委托。
            因此，默认同步上下文涵盖所有ThreadPool线程以及任何调用Send的线程。默认SynchronizationContext“借用”线程调用Send，将它们带入上下文，直到委托完成。从这个意义上讲，默认上下文可以包括进程中的任何线程。
            除非代码由ASP.NET托管，否则默认的SynchronizationContext将应用于ThreadPool线程。除非子线程设置自己的SynchronizationContext，否则默认的SynchronizationContext也会隐式应用于显式子线程（Thread类的实例）。
            因此，UI应用程序通常具有两个同步上下文：覆盖UI线程的UI SynchronizationContext和覆盖ThreadPool线程的默认SynchronizationContext。
         */
        /*
         默认情况下，控制台应用程序和Windows Services中的所有线程仅具有默认的SynchronizationContext。这会导致某些基于事件的异步组件失败。
两种解决方案：

创建一个显式子线程并在该线程上安装SynchronizationContext，然后可以为这些组件提供上下文。
实现SynchronizationContext，可参考Nito.Async库（nitoasync.codeplex.com或https://github.com/StephenClearyArchive/Nito.Asynchronous）的ActionThread类可以用作通用的SynchronizationContext实现。
         */

        /*
         AsyncOperationManager 和 AsyncOperation
NET Framework中的AsyncOperationManager和AsyncOperation类是ynchronizationContext抽象的轻量级封装。本人经常使用Unity，很多时候使用AsyncOperationManager和AsyncOperation实现异步操作。
实现：
AsyncOperationManager首次创建AsyncOperation时会捕获当前的SynchronizationContext，如果当前的SynchronizationContext为null，则将其替换为默认的SynchronizationContext。
AsyncOperation将委托异步发布到捕获的SynchronizationContext。
大多数基于事件的异步组件在其实现中都使用AsyncOperationManager和AsyncOperation。这些方法对于具有定义的完成点的异步操作非常有效，也就是说，异步操作在一个点开始，在另一个点结束。其他异步通知可能没有定义的完成点。这些可能是一种订阅，从一个点开始，然后无限期地继续。对于这些类型的操作，可以直接捕获并使用SynchronizationContext。
        新组件不应使用基于事件的异步模式，应采用基于任务的异步模式。组件返回Task和Task对象，而不是通过SynchronizationContext引发事件。基于任务的API是.NET中异步编程的未来，不过就目前来说，TAP已经是异步普遍采用的模式了。
         */
        /*
         任务并行库(TPL)
Task Parallel Library (TPL): TaskScheduler.FromCurrentSynchronizationContext 和 CancellationToken.Register
TPL 使用任务(Task)对象作为其工作单元并通过任务调度( TaskScheduler )执行。
默认TaskScheduler的作用类似于默认同步上下文，将任务排队到ThreadPool。
TPL队列提供了另一个TaskScheduler，它将任务排队到同步上下文。
UI更新的进度报告可以使用嵌套任务来完成
         */
        /*
          通过BackgroundWorker取代Thread执行异步操作
BackgroundWorker组件是.Net推出的一个组件，用来方便地进行跨线程的界面更新操作。以下引自MSDN：
BackgroundWorker 类允许您在单独的专用线程上运行操作。
耗时的操作（如下载和数据库事务）在长时间运行时可能会导致用户界面 (UI) 似乎处于停止响应状态。如果您需要能进行响应的用户界面，而且面临与这类操作相关的长时间延迟，则可以使用 BackgroundWorker 类方便地解决问题。
         */
        const int k_AwqInitialCapacity = 20;

        static SynchronizationContext s_MainThreadContext;

        readonly List<WorkRequest> m_AsyncWorkQueue;
        readonly List<WorkRequest> m_CurrentFrameWork = new List<WorkRequest>(k_AwqInitialCapacity);
        readonly int m_MainThreadID;
        int m_TrackedCount;
        /// <summary>
        /// UI线程 SynchronizationContext.Current的值是在第一个Control控件创建的时候被初始化的
        /// </summary>
        /// <param name="context">当前上下文线程，注意，并不是每个线程都有上下文线程的</param>
        public ExecutableUnitySynchronizationContext(SynchronizationContext context) {
            if (s_MainThreadContext == null) {
                s_MainThreadContext = context;
            }
            
            if (s_MainThreadContext == null || s_MainThreadContext != context) {
                throw new InvalidOperationException("Unable to create executable synchronization context without a valid synchronization context.");
            }

            // Set the thread ID to the current thread sync context.  It is assumed to be the main thread sync context.
            m_MainThreadID = Thread.CurrentThread.ManagedThreadId;
            m_AsyncWorkQueue = new List<WorkRequest>();
            //使用同步上下文排队并执行工作请求。
            // Queue up and Execute work request with the synchronization context.
            s_MainThreadContext.Post(SendOrPostCallback, new CallbackObject(ExecuteAndAppendNextExecute));
        }

        ExecutableUnitySynchronizationContext(List<WorkRequest> queue, int mainThreadID) {
            m_AsyncWorkQueue = queue;
            m_MainThreadID = mainThreadID;
        }

        static void SendOrPostCallback(object state) {
            var obj = state as CallbackObject;
            obj?.callback();
        }

        public override void Send(SendOrPostCallback callback, object state) {
            //Send将同步处理呼叫。如果调用是在主线程上处理的，我们将调用它
            //直接在这里。如果调用在另一个线程上处理，它将像POST一样排队等待执行
            //在主线程上，它将等待。一旦主线程处理了工作，我们就可以继续
            // Send will process the call synchronously. If the call is processed on the main thread, we'll invoke it
            // directly here. If the call is processed on another thread it will be queued up like POST to be executed
            // on the main thread and it will wait. Once the main thread processes the work we can continue
            if (m_MainThreadID == Thread.CurrentThread.ManagedThreadId) {
                callback(state);
            }
            else {
                using (var waitHandle = new ManualResetEvent(false)) {
                    lock (m_AsyncWorkQueue) {
                        m_AsyncWorkQueue.Add(new WorkRequest(callback, state, waitHandle));
                    }
                    waitHandle.WaitOne();
                }
            }
        }

        public override void OperationStarted() { Interlocked.Increment(ref m_TrackedCount); }

        public override void OperationCompleted() { Interlocked.Decrement(ref m_TrackedCount); }
        //Post会将调用添加到稍后在主线程上执行的任务列表中，然后工作将异步继续
        // Post will add the call to a task list to be executed later on the main thread then work will continue asynchronously
        public override void Post(SendOrPostCallback callback, object state) {
            lock (m_AsyncWorkQueue) {
                m_AsyncWorkQueue.Add(new WorkRequest(callback, state));
            }
        }

        // CreateCopy returns a new ExecutableUnitySynchronizationContext object, but the queue is still shared with the original
        public override SynchronizationContext CreateCopy() {
            lock (m_AsyncWorkQueue) {
                return new ExecutableUnitySynchronizationContext(m_AsyncWorkQueue, m_MainThreadID);
            }
        }

        /// <summary>
        /// 执行当前一组挂起的任务，超时会阻止执行继续添加和执行工作。如果当前设置完成，并且还有时间，则进行下一批。
        /// Executes the current set of pending tasks with a timeout that prevents the execution from continuously adding
        /// and executing work. If the current set finishes and there is time left then take the next batch.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The timeout in milliseconds that the execute can take.
        /// </param>
        /// <returns>
        /// <c>true</c> if all tasks have been executed and completed and <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The timeout is there to guard against tasks that spawn other tasks and threads adding jobs during processing.
        /// It will not cease execution on the current set of tasks, just prevent execution of another set of tasks.
        /// </remarks>
        public bool ExecutePendingTasks(long millisecondsTimeout) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (HasPendingTasks()) {
                // To prevent work from posting more work indefinitely, stop processing after timeout.
                // Does not prevent a given work queue from going over the time allotment
                if (stopwatch.ElapsedMilliseconds > millisecondsTimeout) {
                    break;
                }

                Execute();
                Thread.Sleep(1);
            }
            stopwatch.Stop();

            return !HasPendingTasks();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool HasPendingTasks() {
            lock (m_AsyncWorkQueue) {
                return m_AsyncWorkQueue.Count != 0 || m_TrackedCount != 0;
            }
        }

        /// <summary>
        /// Executes the current set of pending tasks.
        /// </summary>
        /// <remarks>
        /// This will take the complete set of pending tasks in the work queue.
        /// </remarks>
        void Execute() {
            // Enforce all job execution completion on the main thread.
            if (m_MainThreadID == Thread.CurrentThread.ManagedThreadId) {
                // The following is the same behavior as UnitySynchronizationContext
                lock (m_AsyncWorkQueue) {
                    m_CurrentFrameWork.AddRange(m_AsyncWorkQueue);
                    m_AsyncWorkQueue.Clear();
                }

                while (m_CurrentFrameWork.Count > 0) {
                    var work = m_CurrentFrameWork[0];
                    m_CurrentFrameWork.RemoveAt(0);
                    work.Invoke();
                }
            }
        }

        /// <summary>
        /// Executes the current set of pending tasks and then (xref: System.Threading.SynchronizationContext.Post)
        /// another <see cref='CallbackObject'> with another ExecuteAndAppendNextExecute action to the (xref: UnityEngine.UnitySyncrhonizationContext)
        /// </summary>
        /// <remarks>
        /// This method is intended to only be used to hook into the (xref: UnityEngine.UnitySyncrhonizationContext) pending task processing callback.
        /// </remarks>
        void ExecuteAndAppendNextExecute() {
#if UNITY3D
            // Stop infinity loop when application is stopped.
            if (!UnityEngine.Application.isPlaying)
                return;
#endif
            Execute();

            // UnitySynchronizationContext works by performing work in batches so as not to stall the main thread
            // forever. Therefore it is safe to re-add ourselves to the delayed work queue. This is how we hook into
            // the main thread delayed tasks.
            s_MainThreadContext.Post(SendOrPostCallback, new CallbackObject(ExecuteAndAppendNextExecute));
        }

        class CallbackObject
        {
            public readonly Action callback;

            public CallbackObject(Action callback) {
                this.callback = callback;
            }
        }

        readonly struct WorkRequest
        {
            readonly SendOrPostCallback m_DelegateCallback;
            readonly object m_DelegateState;
            readonly ManualResetEvent m_WaitHandle;

            public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null) {
                m_DelegateCallback = callback;
                m_DelegateState = state;
                m_WaitHandle = waitHandle;
            }

            public void Invoke() {
                try {
                    m_DelegateCallback(m_DelegateState);
                }
                finally {
                    m_WaitHandle?.Set();
                }
            }
        }
    }
}