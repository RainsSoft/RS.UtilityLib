using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Read-only hash set.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public class ReadOnlySet<TItem> : SetBase<TItem>
	{
		/// <summary>
		///   Initialize a new ReadOnlySet{T} instance.
		/// </summary>
		public ReadOnlySet() {
		}

		/// <summary>
		///   Initialize a new ReadOnlySet{T} instance.
		/// </summary>
		/// <param name="enumerable">IEnumerable to create this collection from</param>
		public ReadOnlySet(IEnumerable<TItem> enumerable)
			: base(enumerable) {
		}

		/// <summary>
		///   Initialize a new ReadOnlySet{T} instance.
		/// </summary>
		/// <param name="comparer">item comparer</param>
		public ReadOnlySet(IEqualityComparer<TItem> comparer)
			: base(comparer) {
		}

		/// <summary>
		///   Initialize a new ReadOnlySet{T} instance.
		/// </summary>
		/// <param name="enumerable">IEnumerable to create this collection from</param>
		/// <param name="comparer">item comparer</param>
		public ReadOnlySet(IEnumerable<TItem> enumerable, IEqualityComparer<TItem> comparer)
			: base(enumerable, comparer) {
		}
	}
}