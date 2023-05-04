using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Enumerator for stepping through items of a group regardless of item hierarchy.
	/// </summary>
	internal sealed class BetterListViewGroupItemEnumerator : IEnumerator<BetterListViewItem>, IDisposable, IEnumerator
	{
		private readonly BetterListViewGroup group;

		private readonly ReadOnlyCollection<BetterListViewItem> items;

		private int indexItemStart;

		private BetterListViewItem itemCurrent;

		private bool isDisposed;

		public BetterListViewItem Current {
			get {
				this.CheckDisposed();
				return this.itemCurrent;
			}
		}

		object IEnumerator.Current => this.Current;

		/// <summary>
		///   Initialize a new BetterListViewGroupItemEnumerator instance.
		/// </summary>
		/// <param name="group">group that contains the specified items</param>
		public BetterListViewGroupItemEnumerator(BetterListViewGroup group) {
			Checks.CheckNotNull(group, "group");
			Checks.CheckNotNull(group.ListView, "group.ListView");
			this.group = group;
			this.items = group.GetItems(group.ListView);
		}

		private void CheckDisposed() {
			if (this.isDisposed) {
				throw new ObjectDisposedException("BetterListViewGroupItemEnumerator");
			}
		}

		bool IEnumerator.MoveNext() {
			this.CheckDisposed();
			if (this.itemCurrent == null) {
				this.indexItemStart = 0;
				this.itemCurrent = ((this.items.Count != 0) ? this.items[0] : null);
			}
			else {
				do {
					BetterListViewItem betterListViewItem = ((this.itemCurrent.ChildItems.Count == 0) ? this.itemCurrent.NextItem : this.itemCurrent.ChildItems[0]);
					if (betterListViewItem == null && this.itemCurrent.Level > this.items[this.indexItemStart].Level && this.itemCurrent.ParentItem != null) {
						betterListViewItem = this.itemCurrent.ParentItem.NextItem;
					}
					if (betterListViewItem == null) {
						this.indexItemStart++;
						if (this.indexItemStart < this.items.Count) {
							this.itemCurrent = this.items[this.indexItemStart];
						}
					}
					this.itemCurrent = betterListViewItem;
				}
				while (this.itemCurrent != null && this.itemCurrent.Group != this.group);
			}
			return this.itemCurrent != null;
		}

		void IEnumerator.Reset() {
			this.CheckDisposed();
			this.itemCurrent = null;
		}

		public void Dispose() {
			this.isDisposed = true;
		}
	}
}