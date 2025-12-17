using System;
using System.Threading;

namespace Schedulers.Deque
{
	/// <summary>
	///     A <see cref="T:Schedulers.Deque.WorkStealingDeque`1" /> is an implementation of the Chase &amp; Lev Dynamic Circular Work-Stealing Deque [1]
	///     It is thread safe, lock-free, and concurrent, but with a caveat: It must have an owner process that exclusively calls
	///     <see cref="M:Schedulers.Deque.WorkStealingDeque`1.TryPopBottom(`0@)" /> and <see cref="M:Schedulers.Deque.WorkStealingDeque`1.PushBottom(`0)" />. Any number of child stealers can call
	///     <see cref="M:Schedulers.Deque.WorkStealingDeque`1.TrySteal(`0@)" /> concurrently.
	/// </summary>
	/// <remarks>
	///     While Chase &amp; Lev provide several options for memory management, we choose to let resizes discard of the additional
	///     memory through GC. This is because we don't expect to frequently grow, or to shrink at all, given our API.
	///     [1] Chase, D., &amp; Lev, Y. (2005). Dynamic circular work-stealing deque. Proceedings of the Seventeenth Annual ACM
	///         Symposium on Parallelism in Algorithms and Architectures. ⟨10.1145/1073970.1073974⟩.
	///         Retrieved October 17, 2023, from https://www.dre.vanderbilt.edu/~schmidt/PDF/work-stealing-dequeue.pdf.
	///     [2] Nhat Minh Lê, Antoniu Pop, Albert Cohen, Francesco Zappa Nardelli. Correct and Efficient Work-Stealing for Weak Memory
	///         Models. PPoPP '13 - Proceedings of the 18th ACM SIGPLAN symposium on Principles and practice of parallel programming,
	///         Feb 2013, Shenzhen, China. pp.69-80, ⟨10.1145/2442516.2442524⟩. ⟨hal-00802885⟩. Retrieved October 17, 2023 from
	///         https://hal.science/hal-00786679/.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	internal class WorkStealingDeque<T>
	{
		private long _bottom = 0L;

		private long _top = 0L;

		private long _lastTopValue = 0L;

		private volatile CircularArray<T> _activeArray;

		/// <summary>
		/// Create a new <see cref="T:Schedulers.Deque.WorkStealingDeque`1" /> with capacity of at least <paramref name="capacity" />.
		/// </summary>
		/// <param name="capacity"></param>
		public WorkStealingDeque(int capacity)
		{
			_activeArray = new CircularArray<T>((int)Math.Ceiling(Math.Log(capacity, 2.0)));
		}

		private bool CASTop(long oldVal, long newVal)
		{
			return Interlocked.CompareExchange(ref _top, newVal, oldVal) == oldVal;
		}

		/// <summary>
		///     Push an item to the bottom of the <see cref="T:Schedulers.Deque.WorkStealingDeque`1" />.
		/// </summary>
		/// <remarks>
		///     This method must ONLY be called by the deque's owning process, ever!
		///     It is not concurrent with itself, only with <see cref="M:Schedulers.Deque.WorkStealingDeque`1.TrySteal(`0@)" />
		/// </remarks>
		/// <param name="item">The item to add.</param>
		public void PushBottom(T item)
		{
			long b = Volatile.Read(ref _bottom);
			CircularArray<T> a = _activeArray;
			long sizeUpperBound = b - _lastTopValue;
			if (sizeUpperBound >= a.Capacity - 1)
			{
				long t = (_lastTopValue = Interlocked.Read(ref _top));
				long actualSize = b - t;
				if (actualSize >= a.Capacity - 1)
				{
					a = (_activeArray = a.EnsureCapacity(b, t));
				}
			}
			a[b] = item;
			Volatile.Write(ref _bottom, b + 1);
		}

		/// <summary>
		///     Attempt to pop an item from the bottom of the <see cref="T:Schedulers.Deque.WorkStealingDeque`1" />.
		/// </summary>
		/// <remarks>
		///     This method must ONLY be called by the deque's owning process, ever!
		///     It is not concurrent with itself, only with <see cref="M:Schedulers.Deque.WorkStealingDeque`1.TrySteal(`0@)" />
		/// </remarks>
		/// <param name="item">Set to the popped item if success. If no success, undefined.</param>
		/// <returns>True if we popped successfully and therefore <paramref name="item" /> contains useful data.</returns>
		public bool TryPopBottom(out T item)
		{
			item = default(T);
			long b = Volatile.Read(ref _bottom);
			CircularArray<T> a = _activeArray;
			b--;
            Interlocked.Exchange(ref _bottom, b);
			long t = Interlocked.Read(ref _top);
			long size = b - t;
			if (size < 0)
			{
				Volatile.Write(ref _bottom, t);
				return false;
			}
			T popped = a[b];
			if (size > 0)
			{
				item = popped;
				return true;
			}
			if (!CASTop(t, t + 1))
			{
                Interlocked.Exchange(ref _bottom, t + 1);
				return false;
			}
            Interlocked.Exchange(ref _bottom, t + 1);
			item = popped;
			return true;
		}

		/// <summary>
		///     Attempt to steal an item from the top of the <see cref="T:Schedulers.Deque.WorkStealingDeque`1" />.
		/// </summary>
		/// <remarks>
		///     Unlike <see cref="M:Schedulers.Deque.WorkStealingDeque`1.PushBottom(`0)" /> and <see cref="M:Schedulers.Deque.WorkStealingDeque`1.TryPopBottom(`0@)" />, this method can be called from any thread
		///     at any time, and it is guaranteed to be concurrently compatible with all other methods including itself.
		/// </remarks>
		/// <param name="item">Set to the stolen item if success. If no success, undefined.</param>
		/// <returns>True if we stole successfully and therefore <paramref name="item" /> contains useful data.</returns>
		public bool TrySteal(out T item)
		{
			item = default(T);
			long t = Interlocked.Read(ref _top);
			long b = Volatile.Read(ref _bottom);
			CircularArray<T> a = _activeArray;
			long size = b - t;
			if (size <= 0)
			{
				return false;
			}
			T stolen = a[t];
			if (!CASTop(t, t + 1))
			{
				return false;
			}
			item = stolen;
			return true;
		}
	}
}
