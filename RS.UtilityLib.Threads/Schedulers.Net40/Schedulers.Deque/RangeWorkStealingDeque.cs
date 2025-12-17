using System;
using System.Threading;

namespace Schedulers.Deque
{
	/// <summary>
	///     A <see cref="T:Schedulers.Deque.RangeWorkStealingDeque" /> is an implementation of the Chase &amp; Lev Dynamic Circular Work-Stealing Deque [1]
	///     specifically for the case of an array of deque of contiguous integers from [0, n). It is implemented specifically for the
	///     <see cref="T:Schedulers.IJobParallelFor" /> case. It is a much lighter version than the full implementation of <see cref="T:Schedulers.Deque.WorkStealingDeque`1" />.
	///     It is thread safe, lock-free, and concurrent, but with a caveat: It must have an owner process that exclusively calls
	///     <see cref="M:Schedulers.Deque.RangeWorkStealingDeque.TryPopBottom(System.Range@)" />. Any number of child stealers can call <see cref="M:Schedulers.Deque.RangeWorkStealingDeque.TrySteal(System.Range@)" /> concurrently.
	/// </summary>
	/// <remarks>
	///     See <see cref="T:Schedulers.Deque.WorkStealingDeque`1" /> for the canonical implementation.
	/// </remarks>
	internal class RangeWorkStealingDeque
	{
		/// <summary>
		/// Unlike <see cref="T:Schedulers.Deque.WorkStealingDeque`1" />, we actually return a status depending on whether it's empty, because
		/// we're not using this with a Lin et al. algorithm, rather a much simpler algorithm that does need to know if it's
		/// finished or aborted due to contention.
		/// </summary>
		public enum Status
		{
			Empty,
			Abort,
			Success
		}

		private long _top;

		private long _bottom;

		private int _start;

		private int _end;

		private int _batchSize;

		private bool _empty = true;

		/// <summary>
		/// Returns whether this <see cref="T:Schedulers.Deque.RangeWorkStealingDeque" /> is "empty," i.e. completed.
		/// </summary>
		public bool IsEmpty => _empty;

		/// <summary>
		/// Initializes an empty <see cref="T:Schedulers.Deque.RangeWorkStealingDeque" />.
		/// </summary>
		public RangeWorkStealingDeque()
		{
			_top = 0L;
			_bottom = 0L;
		}

		/// <summary>
		/// Reset the state to the the initial range. Do not call while the work-stealing operation is in progress.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <param name="batchSize"></param>
		public void Set(int start, int count, int batchSize)
		{
			int batches = count / batchSize;
			if (count % batchSize != 0)
			{
				batches++;
			}
			_top = 0L;
			_bottom = batches;
			_start = start;
			_end = start + count;
			_batchSize = batchSize;
			_empty = false;
		}

		private bool CASTop(long oldVal, long newVal)
		{
			return Interlocked.CompareExchange(ref _top, newVal, oldVal) == oldVal;
		}

		/// <summary>
		/// Attempt a pop of some range from the bottom.
		/// </summary>
		/// <param name="range">The output range, valid only if the return value is equal to <see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Success" />.</param>
		/// <returns><see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Empty" /> if the range is empty and we failed to pop. <see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Success" /> if we succeeded in popping.</returns>
		public Status TryPopBottom(out Range range)
		{
			range = default(Range);
			long b = Volatile.Read(ref _bottom);
			b--;
			Volatile.Write(ref _bottom, b);
			long t = Interlocked.Read(ref _top);
			long size = b - t;
			if (size < 0)
			{
				Volatile.Write(ref _bottom, t);
				return Status.Empty;
			}
			Range popped = MakeRange(b);
			if (size > 0)
			{
				range = popped;
				return Status.Success;
			}
			if (!CASTop(t, t + 1))
			{
				Volatile.Write(ref _bottom, t + 1);
				_empty = true;
				return Status.Empty;
			}
			Volatile.Write(ref _bottom, t + 1);
			range = popped;
			_empty = true;
			return Status.Success;
		}

		/// <summary>
		/// Attempt a steal of some range from the top.
		/// </summary>
		/// <param name="range">The output range, valid only if the return value is equal to <see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Success" />.</param>
		/// <returns><see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Empty" /> if the range is empty and we failed to steal. <see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Success" /> if we succeeded in stealing.
		/// <see cref="F:Schedulers.Deque.RangeWorkStealingDeque.Status.Abort" /> if contention occurred and we should try again.</returns>
		public Status TrySteal(out Range range)
		{
			range = default(Range);
			long t = Interlocked.Read(ref _top);
			long b = Volatile.Read(ref _bottom);
			long size = b - t;
			if (size <= 0)
			{
				_empty = true;
				return Status.Empty;
			}
			Range stolen = MakeRange(t);
			if (!CASTop(t, t + 1))
			{
				return Status.Abort;
			}
			range = stolen;
			if (b - (t + 1) <= 0)
			{
				_empty = true;
			}
			return Status.Success;
		}

		private Range MakeRange(long index)
		{
			int start = (int)index * _batchSize + _start;
			int end = start + _batchSize;
			end = Math.Min(end, _end);
			return new Range( start,end);
		}
	}
}
