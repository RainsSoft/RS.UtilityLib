using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Immutable set of BetterListViewItem instances ensuring reference equality comparison.
	/// </summary>
	public sealed class BetterListViewReadOnlyItemSet : ReadOnlySet<BetterListViewItem>
	{
		/// <summary>
		///   Initialize a new BetterListViewReadOnlyItemSet instance.
		/// </summary>
		public BetterListViewReadOnlyItemSet() {
		}

		/// <summary>
		///   Initialize a new BetterListViewReadOnlyItemSet instance.
		/// </summary>
		/// <param name="items">enumerable of items to fill this collection with</param>
		public BetterListViewReadOnlyItemSet(IEnumerable<BetterListViewItem> items)
			: base(items) {
		}
	}
}