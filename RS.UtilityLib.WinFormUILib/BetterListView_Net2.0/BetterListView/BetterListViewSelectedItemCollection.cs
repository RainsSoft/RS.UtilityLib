using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents checked items within Better ListView control.
	/// </summary>
	public sealed class BetterListViewSelectedItemCollection : BetterListViewCachedItemCollection
	{
		/// <summary>
		///   Gets or sets the selected item at the specified index.
		/// </summary>
		/// <returns>The selected item at the specified index.</returns>
		public override BetterListViewItem this[int index] {
			get {
				List<BetterListViewItem> cachedItems = base.CachedItems;
				Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				return cachedItems[index];
			}
			set {
				List<BetterListViewItem> cachedItems = base.CachedItems;
				if (value != null) {
					Checks.CheckTrue(value.ListView == base.ListView, "ReferenceEquals(value.ListView, ListView)");
				}
				Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
				betterListViewItemSet.Remove(cachedItems[index]);
				if (value != null) {
					betterListViewItemSet.Add(value);
				}
				base.ListView.SelectedItemsSet = betterListViewItemSet;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSelectedItemCollection" /> class.
		/// </summary>
		/// <param name="listView">BetterListView that owns this collection.</param>
		internal BetterListViewSelectedItemCollection(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		///   Select the specified items by adding them in this collection.
		/// </summary>
		/// <param name="items">Items to select.</param>
		public override void AddRange(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			betterListViewItemSet.AddRange(items);
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Deselect the specified items by removing them from this collection.
		/// </summary>
		/// <param name="items">Items to deselect.</param>
		public override void RemoveRange(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			betterListViewItemSet.RemoveRange(items);
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Select just the specified items.
		/// </summary>
		/// <param name="items">Items to be selected.</param>
		public override void Set(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			base.ListView.SelectedItemsSet = new List<BetterListViewItem>(items);
		}

		/// <summary>
		///   Recreated cached view by collecting items this collection should represent.
		/// </summary>
		/// <param name="cachedItems">Items viewed by this collection.</param>
		protected override void CollectCachedItems(List<BetterListViewItem> cachedItems) {
			ICollection<BetterListViewItem> selectedItemsSet = base.ListView.SelectedItemsSet;
			if (selectedItemsSet.Count != 0) {
				cachedItems.AddRange(selectedItemsSet);
				cachedItems.Sort(BetterListViewItemIndexComparer.Instance);
			}
		}

		/// <summary>
		///   Determines the index of the specified item in the list.
		/// </summary>
		/// <param name="item">The object to locate in the list.</param>
		/// <returns>
		///   The index of item if found in the list; otherwise, -1.
		/// </returns>
		public override int IndexOf(BetterListViewItem item) {
			if (item == null || item.ListView != base.ListView) {
				return -1;
			}
			return base.CachedItems.IndexOf(item);
		}

		/// <summary>
		///   Select the specified item by inserting it to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The item to insert into the list.</param>
		public override void Insert(int index, BetterListViewItem item) {
			List<BetterListViewItem> cachedItems = base.CachedItems;
			Checks.CheckBounds(index, 0, cachedItems.Count, "index");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			if (item != null) {
				betterListViewItemSet.Add(item);
			}
			else {
				betterListViewItemSet.Remove(cachedItems[index]);
			}
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Deselect the specified item by removing in from the specified position in the list.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public override void RemoveAt(int index) {
			List<BetterListViewItem> cachedItems = base.CachedItems;
			Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
			Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			betterListViewItemSet.Remove(cachedItems[index]);
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Select the specified item by adding it to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public override void Add(BetterListViewItem item) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItems);
			betterListViewItemSet.Add(item);
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Deselect all items by clearing the collection.
		/// </summary>
		public override void Clear() {
			base.ListView.SelectedItemsSet = new BetterListViewItem[0];
		}

		/// <summary>
		///   Check whether the specified item is contained within the collection (is selected).
		/// </summary>
		/// <param name="item">The item to locate in the collection.</param>
		/// <returns>
		///   true if item is found in the collection; otherwise, false.
		/// </returns>
		public override bool Contains(BetterListViewItem item) {
			if (item == null || item.ListView != base.ListView) {
				return false;
			}
			return base.ListView.SelectedItemsSet.Contains(item);
		}

		/// <summary>
		///   Deselect the specified item by removing it from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>
		///   true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.
		/// </returns>
		public override bool Remove(BetterListViewItem item) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			if (betterListViewItemSet.Remove(item)) {
				base.ListView.SelectedItemsSet = betterListViewItemSet;
				return true;
			}
			return false;
		}
	}
}