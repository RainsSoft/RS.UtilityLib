using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents checked items within Better ListView control.
	/// </summary>
	public sealed class BetterListViewCheckedItemCollection : BetterListViewCachedItemCollection
	{
		/// <summary>
		///   Gets or sets the checked item at the specified index.
		/// </summary>
		/// <returns>The checked item at the specified index.</returns>
		public override BetterListViewItem this[int index] {
			get {
				List<BetterListViewItem> cachedItems = base.CachedItems;
				Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				return cachedItems[index];
			}
			set {
				List<BetterListViewItem> cachedItems = base.CachedItems;
				if (value != null) {
					Checks.CheckTrue(value.ListView == base.ListView, "ReferenceEquals(value.ListView, ListView)");
				}
				Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				BetterListViewItem betterListViewItem = cachedItems[index];
				if (betterListViewItem == value) {
					return;
				}
				try {
					base.ListView.BeginUpdate();
					betterListViewItem.Checked = false;
					if (value != null) {
						value.Checked = true;
					}
				}
				finally {
					base.ListView.EndUpdate();
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewCheckedItemCollection" /> class.
		/// </summary>
		/// <param name="listView">BetterListView that owns this collection.</param>
		internal BetterListViewCheckedItemCollection(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		///   Check the specified items by adding them in this collection.
		/// </summary>
		/// <param name="items">Items to check.</param>
		public override void AddRange(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item2 in items) {
					item2.Checked = true;
				}
			}
			finally {
				base.ListView.EndUpdate();
			}
		}

		/// <summary>
		///   Uncheck the specified items by removing them this collection.
		/// </summary>
		/// <param name="items">Items to uncheck.</param>
		public override void RemoveRange(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item2 in items) {
					item2.Checked = false;
				}
			}
			finally {
				base.ListView.EndUpdate();
			}
		}

		/// <summary>
		///   Check just the specified items.
		/// </summary>
		/// <param name="items">Items to be checked.</param>
		public override void Set(IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(items, "items");
			foreach (BetterListViewItem item in items) {
				Checks.CheckNotNull(item, "item");
				Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			}
			BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(items);
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item2 in base.ListView) {
					item2.Checked = betterListViewItemSet.Contains(item2);
				}
			}
			finally {
				base.ListView.EndUpdate();
			}
		}

		/// <summary>
		///   Recreated cached view by collecting items this collection should represent.
		/// </summary>
		/// <param name="cachedItems">Items viewed by this collection.</param>
		protected override void CollectCachedItems(List<BetterListViewItem> cachedItems) {
			foreach (BetterListViewItem item in base.ListView) {
				if (item.Checked) {
					cachedItems.Add(item);
				}
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
		///   Check the specified item by inserting it to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The item to insert into the list.</param>
		public override void Insert(int index, BetterListViewItem item) {
			List<BetterListViewItem> cachedItems = base.CachedItems;
			Checks.CheckBounds(index, 0, cachedItems.Count, "index");
			item.Checked = true;
		}

		/// <summary>
		///   Uncheck the specified item by removing in from the specified position in the list.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public override void RemoveAt(int index) {
			List<BetterListViewItem> cachedItems = base.CachedItems;
			Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
			Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
			cachedItems[index].Checked = false;
		}

		/// <summary>
		///   Check the specified item by adding it to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public override void Add(BetterListViewItem item) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			item.Checked = true;
		}

		/// <summary>
		///   Uncheck all items by clearing the collection.
		/// </summary>
		public override void Clear() {
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item in base.ListView) {
					item.Checked = false;
				}
			}
			finally {
				base.ListView.EndUpdate();
			}
		}

		/// <summary>
		///   Check whether the specified item is contained within the collection (is checked).
		/// </summary>
		/// <param name="item">The item to locate in the collection.</param>
		/// <returns>
		///   true if item is found in the collection; otherwise, false.
		/// </returns>
		public override bool Contains(BetterListViewItem item) {
			if (item == null || item.ListView != base.ListView) {
				return false;
			}
			return base.CachedItems.Contains(item);
		}

		/// <summary>
		///   Uncheck the specified item by removing it from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>
		///   true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.
		/// </returns>
		public override bool Remove(BetterListViewItem item) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(item.ListView == base.ListView, "ReferenceEquals(item.ListView, ListView)");
			if (item.Checked) {
				item.Checked = false;
				return true;
			}
			return false;
		}
	}
}