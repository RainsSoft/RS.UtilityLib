using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents checked items within Better ListView control.
	/// </summary>
	public sealed class BetterListViewCheckedIndexCollection : BetterListViewCachedCollection<int>
	{
		/// <summary>
		///   Gets or sets the checked item at the specified index.
		/// </summary>
		/// <returns>The checked item at the specified index.</returns>
		public override int this[int index] {
			get {
				List<int> cachedItems = base.CachedItems;
				Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				return cachedItems[index];
			}
			set {
				List<int> cachedItems = base.CachedItems;
				if (value != -1) {
					Checks.CheckBounds(value, 0, base.ListView.Items.Count - 1, "value");
				}
				Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
				Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
				int num = cachedItems[index];
				if (num == value) {
					return;
				}
				try {
					base.ListView.BeginUpdate();
					base.ListView.Items[num].Checked = false;
					if (value != -1) {
						base.ListView.Items[value].Checked = true;
					}
				}
				finally {
					base.ListView.EndUpdate();
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewCheckedIndexCollection" /> class.
		/// </summary>
		/// <param name="listView">BetterListView that owns this collection.</param>
		internal BetterListViewCheckedIndexCollection(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		///   Check the specified items by adding them in this collection.
		/// </summary>
		/// <param name="items">Items to check.</param>
		public override void AddRange(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			try {
				base.ListView.BeginUpdate();
				foreach (int item2 in items) {
					base.ListView.Items[item2].Checked = true;
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
		public override void RemoveRange(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			try {
				base.ListView.BeginUpdate();
				foreach (int item2 in items) {
					base.ListView.Items[item2].Checked = false;
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
		public override void Set(IEnumerable<int> items) {
			Checks.CheckNotNull(items, "items");
			int maxValue = base.ListView.Items.Count - 1;
			foreach (int item in items) {
				Checks.CheckBounds(item, 0, maxValue, "item");
			}
			Set<int> set = new Set<int>(items);
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item2 in base.ListView.Items) {
					item2.Checked = set.Contains(item2.Index);
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
		protected override void CollectCachedItems(List<int> cachedItems) {
			foreach (BetterListViewItem item in base.ListView.Items) {
				if (item.Checked) {
					cachedItems.Add(item.Index);
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
		public override int IndexOf(int item) {
			if (item < 0 || item >= base.ListView.Items.Count) {
				return -1;
			}
			return base.CachedItems.IndexOf(item);
		}

		/// <summary>
		///   Check the specified item by inserting it to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The item to insert into the list.</param>
		public override void Insert(int index, int item) {
			List<int> cachedItems = base.CachedItems;
			Checks.CheckBounds(index, 0, cachedItems.Count, "index");
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			base.ListView.Items[item].Checked = true;
		}

		/// <summary>
		///   Deselect the specified item by removing in from the specified position in the list.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public override void RemoveAt(int index) {
			List<int> cachedItems = base.CachedItems;
			Checks.CheckTrue(cachedItems.Count != 0, "checkedItems.Count != 0");
			Checks.CheckBounds(index, 0, cachedItems.Count - 1, "index");
			base.ListView.Items[cachedItems[index]].Checked = false;
		}

		/// <summary>
		///   Check the specified item by adding it to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public override void Add(int item) {
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			base.ListView.Items[item].Checked = true;
		}

		/// <summary>
		///   Uncheck all items by clearing the collection.
		/// </summary>
		public override void Clear() {
			try {
				base.ListView.BeginUpdate();
				foreach (BetterListViewItem item in base.ListView.Items) {
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
		public override bool Contains(int item) {
			return base.CachedItems.Contains(item);
		}

		/// <summary>
		///   Uncheck the specified item by removing it from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>
		///   true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.
		/// </returns>
		public override bool Remove(int item) {
			Checks.CheckBounds(item, 0, base.ListView.Items.Count - 1, "item");
			BetterListViewItem betterListViewItem = base.ListView.Items[item];
			if (betterListViewItem.Checked) {
				betterListViewItem.Checked = false;
				return true;
			}
			return false;
		}
	}
}