using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents checked items within Better ListView control.
	/// </summary>
	public sealed class BetterListViewSelectedIndexCollection : BetterListViewCachedCollection<int>
	{
		/// <summary>
		///   Gets or sets the selected item at the specified index.
		/// </summary>
		/// <returns>The selected item at the specified index.</returns>
		public override int this[int index] {
			get {
				List<int> cachedItems = base.CachedItems;
				Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				return cachedItems[index];
			}
			set {
				List<int> cachedItems = base.CachedItems;
				if (value != -1) {
					Checks.CheckBounds(value, 0, base.ListView.Items.Count - 1, "value");
				}
				Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
				betterListViewItemSet.Remove(base.ListView.Items[cachedItems[index]]);
				if (value != -1) {
					betterListViewItemSet.Add(base.ListView.Items[value]);
				}
				base.ListView.SelectedItemsSet = betterListViewItemSet;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSelectedIndexCollection" /> class.
		/// </summary>
		/// <param name="listView">BetterListView that owns this collection.</param>
		internal BetterListViewSelectedIndexCollection(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		///   Select the specified items by adding them in this collection.
		/// </summary>
		/// <param name="items">Items to select.</param>
		public override void AddRange(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			foreach (int item2 in items) {
				betterListViewItemSet.Add(base.ListView.Items[item2]);
			}
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Deselect the specified items by removing them this collection.
		/// </summary>
		/// <param name="items">Items to deselect.</param>
		public override void RemoveRange(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			foreach (int item2 in items) {
				betterListViewItemSet.Remove(base.ListView.Items[item2]);
			}
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Select just the specified items.
		/// </summary>
		/// <param name="items">Items to be selected.</param>
		public override void Set(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet();
			foreach (int item2 in items) {
				betterListViewItemSet.Add(base.ListView.Items[item2]);
			}
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Recreated cached view by collecting items this collection should represent.
		/// </summary>
		/// <param name="cachedItems">Items viewed by this collection.</param>
		protected override void CollectCachedItems(List<int> cachedItems) {
			ICollection<BetterListViewItem> selectedItemsSet = base.ListView.SelectedItemsSet;
			if (selectedItemsSet.Count == 0) {
				return;
			}
			foreach (BetterListViewItem item in selectedItemsSet) {
				if (item.ParentItem == null) {
					cachedItems.Add(item.Index);
				}
			}
			cachedItems.Sort();
		}

		/// <summary>
		///   Determines the index of a selected item in the list.
		/// </summary>
		/// <param name="item">The item to locate in the list.</param>
		/// <returns>
		///   The index of selected item if found in the list; otherwise, -1.
		/// </returns>
		public override int IndexOf(int item) {
			if (item < 0 || item >= base.ListView.Items.Count) {
				return -1;
			}
			return base.CachedItems.IndexOf(item);
		}

		/// <summary>
		///   Select the specified item by inserting it to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The item to insert into the list.</param>
		public override void Insert(int index, int item) {
			List<int> cachedItems = base.CachedItems;
			Checks.CheckBounds(index, 0, cachedItems.Count, "index");
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			if (item != -1) {
				betterListViewItemSet.Add(base.ListView.Items[item]);
			}
			else {
				betterListViewItemSet.Remove(base.ListView.Items[cachedItems[index]]);
			}
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Deselect the specified item by removing in from the specified position in the list.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public override void RemoveAt(int index) {
			List<int> cachedItems = base.CachedItems;
			Checks.CheckTrue(cachedItems.Count != 0, "selectedItems.Count != 0");
			Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			betterListViewItemSet.Remove(base.ListView.Items[cachedItems[index]]);
			base.ListView.SelectedItemsSet = betterListViewItemSet;
		}

		/// <summary>
		///   Select the specified item by adding it to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public override void Add(int item) {
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItems);
			betterListViewItemSet.Add(base.ListView.Items[item]);
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
		public override bool Contains(int item) {
			return base.CachedItems.Contains(item);
		}

		/// <summary>
		///   Deselect the specified item by removing it from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>
		///   true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.
		/// </returns>
		public override bool Remove(int item) {
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(base.ListView.SelectedItemsSet);
			if (betterListViewItemSet.Remove(base.ListView.Items[item])) {
				base.ListView.SelectedItemsSet = betterListViewItemSet;
				return true;
			}
			return false;
		}
	}
}