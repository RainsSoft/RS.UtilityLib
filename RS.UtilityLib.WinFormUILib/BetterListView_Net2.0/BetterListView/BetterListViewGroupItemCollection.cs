using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides view on the BetterListView group items.
	/// </summary>
	internal sealed class BetterListViewGroupItemCollection : BetterListViewItemCollection
	{
		private readonly BetterListViewGroup ownerGroup;

		private bool autoAddItems = true;

		private bool autoRemoveItems = true;

		/// <summary>
		///   Gets or sets a value indicating whether to automatically add items without owner control to the control of the group.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatically add items without owner control to the control of the group; otherwise, <c>false</c>.
		/// </value>
		public bool AutoAddItems {
			get {
				return this.autoAddItems;
			}
			set {
				this.autoAddItems = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to automatically remove items from the control of the group.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatically remove items from the control of the group; otherwise, <c>false</c>.
		/// </value>
		public bool AutoRemoveItems {
			get {
				return this.autoRemoveItems;
			}
			set {
				this.autoRemoveItems = value;
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewGroupItemCollection instance.
		/// </summary>
		/// <param name="ownerGroup">group that owns this collection</param>
		public BetterListViewGroupItemCollection(BetterListViewGroup ownerGroup) {
			Checks.CheckNotNull(ownerGroup, "ownerGroup");
			this.ownerGroup = ownerGroup;
		}

		internal override void OnCollectionChanged(BetterListViewElementCollectionChangeInfo changeInfo) {
			BetterListView listView = this.ownerGroup.ListView;
			try {
				if (listView != null) {
					listView.BeginUpdate();
					if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add && this.AutoAddItems) {
						foreach (BetterListViewItem key in changeInfo.Elements.Keys) {
							if (key.ListView == null) {
								listView.Items.Add(key);
							}
						}
					}
					if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove && this.AutoRemoveItems) {
						foreach (BetterListViewItem key2 in changeInfo.Elements.Keys) {
							key2.Remove();
						}
					}
				}
				if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add || changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove || changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Set) {
					this.ownerGroup.OnCollectionChanged();
				}
				if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add) {
					foreach (KeyValuePair<BetterListViewElementBase, int> element in changeInfo.Elements) {
						BetterListViewItem betterListViewItem3 = (BetterListViewItem)element.Key;
						if (betterListViewItem3.Group != null) {
							((BetterListViewGroupItemCollection)betterListViewItem3.Group.Items).RemoveInternal(betterListViewItem3);
						}
						betterListViewItem3.SetGroup(this.ownerGroup, quiet: true);
					}
				}
				if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove) {
					foreach (KeyValuePair<BetterListViewElementBase, int> element2 in changeInfo.Elements) {
						BetterListViewItem betterListViewItem4 = (BetterListViewItem)element2.Key;
						betterListViewItem4.SetGroup(null, quiet: true);
					}
				}
				base.OnCollectionChanged(changeInfo);
				this.ownerGroup.OnElementPropertyChanged(BetterListViewElementPropertyType.Grouping);
			}
			finally {
				listView?.EndUpdate();
			}
		}

		private void RemoveInternal(BetterListViewItem item) {
			base.InnerList.Remove(item);
		}
	}
}