using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents information about BetterListView item selection.
	/// </summary>
	internal struct BetterListViewSelectionInfo
	{
		/// <summary>
		///   empty BetterListViewSelection structure
		/// </summary>
		public static readonly BetterListViewSelectionInfo Empty = new BetterListViewSelectionInfo(new BetterListViewGroup[0], new BetterListViewItem[0], null);

		private BetterListViewReadOnlyGroupSet selectedGroups;

		private ReadOnlyDictionary<BetterListViewItem, BetterListViewAddress> selectedItems;

		private BetterListViewElementBase lastSelectedElement;

		/// <summary>
		///   currently selected groups
		/// </summary>
		public BetterListViewReadOnlyGroupSet SelectedGroups => this.selectedGroups;

		/// <summary>
		///   currently selected items
		/// </summary>
		public ReadOnlyDictionary<BetterListViewItem, BetterListViewAddress> SelectedItems => this.selectedItems;

		/// <summary>
		///   element selected for the last time
		/// </summary>
		public BetterListViewElementBase LastSelectedElement {
			get {
				return this.lastSelectedElement;
			}
			set {
				this.lastSelectedElement = value;
			}
		}

		/// <summary>
		///   this BetterListViewSelectionInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewSelectionInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewSelectionInfo instance.
		/// </summary>
		/// <param name="selectedGroups">currently selected groups</param>
		/// <param name="selectedItems">currently selected items</param>
		/// <param name="lastSelectedElement">element selected for the last time</param>
		public BetterListViewSelectionInfo(IEnumerable<BetterListViewGroup> selectedGroups, IEnumerable<BetterListViewItem> selectedItems, BetterListViewElementBase lastSelectedElement) {
			Checks.CheckNotNull(selectedGroups, "selectedGroups");
			Checks.CheckNotNull(selectedItems, "selectedItems");
			this.selectedGroups = new BetterListViewReadOnlyGroupSet(selectedGroups);
			this.selectedItems = BetterListViewSelectionInfo.GetSelectedItemsDictionary(selectedItems);
			this.lastSelectedElement = lastSelectedElement;
		}

		/// <summary>
		///   Test whether the two BetterListViewSelectionInfo objects are identical.
		/// </summary>
		/// <param name="selectionInfoA">first BetterListViewSelectionInfo object</param>
		/// <param name="selectionInfoB">second BetterListViewSelectionInfo object</param>
		/// <returns>the two BetterListViewSelectionInfo objects are identical</returns>
		public static bool operator ==(BetterListViewSelectionInfo selectionInfoA, BetterListViewSelectionInfo selectionInfoB) {
			return selectionInfoA.Equals(selectionInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewSelectionInfo objects are different.
		/// </summary>
		/// <param name="selectionInfoA">first BetterListViewSelectionInfo object</param>
		/// <param name="selectionInfoB">second BetterListViewSelectionInfo object</param>
		/// <returns>the two BetterListViewSelectionInfo objects are different</returns>
		public static bool operator !=(BetterListViewSelectionInfo selectionInfoA, BetterListViewSelectionInfo selectionInfoB) {
			return !selectionInfoA.Equals(selectionInfoB);
		}

		/// <summary>
		///   Set currently selected items.
		/// </summary>
		/// <param name="groups">currently selected groups</param>
		/// <param name="items">currently selected items</param>
		public void SetSelectedItems(IEnumerable<BetterListViewGroup> groups, IEnumerable<BetterListViewItem> items) {
			Checks.CheckNotNull(groups, "groups");
			Checks.CheckNotNull(items, "items");
			this.selectedGroups = new BetterListViewReadOnlyGroupSet(groups);
			this.selectedItems = BetterListViewSelectionInfo.GetSelectedItemsDictionary(items);
			if (this.selectedItems.Count == 0) {
				this.lastSelectedElement = null;
				return;
			}
			BetterListViewGroup betterListViewGroup = this.lastSelectedElement as BetterListViewGroup;
			BetterListViewItem betterListViewItem = this.lastSelectedElement as BetterListViewItem;
			bool flag = betterListViewGroup == null || !this.selectedGroups.Contains(betterListViewGroup);
			bool flag2 = betterListViewItem == null || !this.selectedItems.Keys.Contains(betterListViewItem);
			if (!(flag && flag2) || this.selectedItems.Count == 0) {
				return;
			}
			BetterListViewItem betterListViewItem2 = null;
			foreach (BetterListViewItem key in this.selectedItems.Keys) {
				if (betterListViewItem2 == null) {
					betterListViewItem2 = key;
				}
				else if (key.Address < betterListViewItem2.Address) {
					betterListViewItem2 = key;
				}
			}
			this.lastSelectedElement = betterListViewItem2;
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewSelectionInfo betterListViewSelectionInfo)) {
				return false;
			}
			if (this.selectedGroups.EqualsContent(betterListViewSelectionInfo.selectedGroups) && this.selectedItems.EqualsContent(betterListViewSelectionInfo.selectedItems)) {
				return this.lastSelectedElement == betterListViewSelectionInfo.lastSelectedElement;
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.selectedGroups.Count.GetHashCode() ^ this.selectedItems.Count.GetHashCode() ^ ((this.lastSelectedElement != null) ? this.lastSelectedElement.GetHashCode() : 0);
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{SelectedGroups = '{this.selectedGroups.ToString(writeContent: true)}', SelectedItems = '{new BetterListViewItemCollection(this.selectedItems.Keys).ToString(writeContent: true)}', LastSelectedElement = '{this.lastSelectedElement}'}}";
		}

		private static ReadOnlyDictionary<BetterListViewItem, BetterListViewAddress> GetSelectedItemsDictionary(IEnumerable<BetterListViewItem> selectedItems) {
			Dictionary<BetterListViewItem, BetterListViewAddress> dictionary = new Dictionary<BetterListViewItem, BetterListViewAddress>();
			foreach (BetterListViewItem selectedItem in selectedItems) {
				dictionary.Add(selectedItem, selectedItem.Address);
			}
			return new ReadOnlyDictionary<BetterListViewItem, BetterListViewAddress>(dictionary);
		}
	}
}