using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace RS.UtilityLib.StateMachine.PDN
{
    [Flags]
    public enum ThreadBackgroundFlags
    {
        None = 0,
        Cpu = 1,
        IO = 2,

        All = Cpu | IO
    }
    /// <summary>
    /// This exception is thrown by a foreground thread when a background worker thread
    /// had an exception. This allows all exceptions to be handled by the foreground thread.
    /// </summary>
    public class WorkerThreadException
        : Exception
    {
        private const string defaultMessage = "Worker thread threw an exception";

        public WorkerThreadException(Exception innerException)
            : this(defaultMessage, innerException) {
        }

        public WorkerThreadException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
    // Current implementation deficiency: The interface is such that an implied push/pop of
    // flags is presented. However, once you 'push' a background mode, it is not 'popped'
    // until all ThreadBackground objects have been disposed on the current thread.

    public sealed class ThreadBackground
        : IDisposable
    {
        [ThreadStatic]
        private static int count = 0;

        [ThreadStatic]
        private ThreadBackgroundFlags activeFlags = ThreadBackgroundFlags.None;

        private Thread currentThread;
        private ThreadPriority oldThreadPriority;
        private ThreadBackgroundFlags flags;

        public ThreadBackground(ThreadBackgroundFlags flags) {
            this.flags = flags;
            this.currentThread = Thread.CurrentThread;
            this.oldThreadPriority = this.currentThread.Priority;

            if ((flags & ThreadBackgroundFlags.Cpu) == ThreadBackgroundFlags.Cpu &&
                (activeFlags & ThreadBackgroundFlags.Cpu) != ThreadBackgroundFlags.Cpu) {
                this.currentThread.Priority = ThreadPriority.BelowNormal;
                activeFlags |= ThreadBackgroundFlags.Cpu;
            }

            activeFlags |= flags;

            ++count;
        }

        ~ThreadBackground() {
            Debug.Assert(false, "ThreadBackgroundMode() object must be manually Disposed()");
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            --count;

            if (Thread.CurrentThread.ManagedThreadId != this.currentThread.ManagedThreadId) {
                throw new InvalidOperationException("Dispose() was called on a thread other than the one that this object was created on");
            }

            if (count == 0) {
                if ((activeFlags & ThreadBackgroundFlags.Cpu) == ThreadBackgroundFlags.Cpu) {
                    this.currentThread.Priority = this.oldThreadPriority;
                    activeFlags &= ~ThreadBackgroundFlags.Cpu;
                }
            }
        }
    }
}
