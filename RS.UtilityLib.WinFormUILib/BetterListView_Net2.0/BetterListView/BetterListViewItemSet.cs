using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Set of BetterListViewItem instances.
	/// </summary>
	public sealed class BetterListViewItemSet : Set<BetterListViewItem>
	{
		/// <summary>
		///   Initialize a new BetterListViewItemSet instance.
		/// </summary>
		public BetterListViewItemSet() {
		}

		/// <summary>
		///   Initialize a new BetterListViewItemSet instance.
		/// </summary>
		/// <param name="items">enumerable of items to fill this collection with</param>
		public BetterListViewItemSet(IEnumerable<BetterListViewItem> items)
			: base(items) {
		}
	}
}