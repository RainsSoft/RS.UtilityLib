using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView items.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(BetterListViewItemCollectionConverter))]
	[Editor(typeof(BetterListViewItemCollectionEditor), typeof(UITypeEditor))]
	public class BetterListViewItemCollection : BetterListViewElementCollection<BetterListViewItem>
	{
		private BetterListView ownerControl;

		[NonSerialized]
		private BetterListViewItem ownerItem;

		/// <summary>
		///   control that owns this collection
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal override BetterListViewBase OwnerControl {
			get {
				if (this.ownerControl != null) {
					return this.ownerControl;
				}
				if (this.ownerItem != null) {
					return this.ownerItem.ListView;
				}
				return null;
			}
			set {
				Checks.CheckType(value, typeof(BetterListView), "value");
				Checks.CheckTrue(this.ownerItem == null, "this.ownerItem == null", "Cannot set owner control to a collection already owned by an item.");
				this.ownerControl = (BetterListView)value;
			}
		}

		/// <summary>
		///   item that owns this collection
		/// </summary>
		internal BetterListViewItem OwnerItem {
			get {
				return this.ownerItem;
			}
			set {
				Checks.CheckTrue(this.ownerControl == null, "this.ownerControl == null", "Cannot set owner item to a collection already owned by an item.");
				this.ownerItem = value;
			}
		}

		/// <summary>
		///   name of the collection element
		/// </summary>
		protected override string ElementName => "item";

		/// <summary>
		///   Initialize a new BetterListViewItemCollection instance.
		/// </summary>
		public BetterListViewItemCollection()
			: this(isInternal: false) {
		}

		/// <summary>
		///   Initialize a new BetterListViewItemCollection instance.
		/// </summary>
		/// <param name="enumerable">elements of items to create this collection from</param>
		public BetterListViewItemCollection(IEnumerable<BetterListViewItem> enumerable)
			: this(isInternal: false, enumerable) {
		}

		internal BetterListViewItemCollection(bool isInternal)
			: this(isInternal, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewItemCollection instance.
		/// </summary>
		/// <param name="isInternal">this collection is for internal use in BetterListView</param>
		/// <param name="enumerable">elements of items to create this collection from</param>
		internal BetterListViewItemCollection(bool isInternal, IEnumerable<BetterListViewItem> enumerable)
			: base(isInternal, enumerable) {
		}

		private BetterListViewItemCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		/// <summary>
		///   Add new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <returns>added item</returns>
		public BetterListViewItem Add(Image image) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(image);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Add new item to the collection.
		/// </summary>
		/// <param name="text">item text</param>
		/// <returns>added item</returns>
		public BetterListViewItem Add(string text) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(text);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Add new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <param name="text">item text</param>
		/// <returns>added item</returns>
		public BetterListViewItem Add(Image image, string text) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(image, text);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Add new item to the collection.
		/// </summary>
		/// <param name="items">sub-item texts</param>
		/// <returns>added item</returns>
		public BetterListViewItem Add(string[] items) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(items);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Add new item to the collection.
		/// </summary>
		/// <param name="key">Name of the item.</param>
		/// <param name="text">Label of the item.</param>
		/// <param name="imageIndex">Index specifying item image in the ImageList.</param>
		/// <returns>Added item.</returns>
		public BetterListViewItem Add(string key, string text, int imageIndex) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(text, imageIndex);
			betterListViewItem.Name = key;
			BetterListViewItem betterListViewItem2 = betterListViewItem;
			this.Add(betterListViewItem2);
			return betterListViewItem2;
		}

		/// <summary>
		///   Adds the specified key.
		/// </summary>
		/// <param name="key">Name of the item.</param>
		/// <param name="text">Label of the item.</param>
		/// <param name="imageKey">Key specifying item image in the ImageList.</param>
		/// <returns>Added item.</returns>
		public BetterListViewItem Add(string key, string text, string imageKey) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(text, imageKey);
			betterListViewItem.Name = key;
			BetterListViewItem betterListViewItem2 = betterListViewItem;
			this.Add(betterListViewItem2);
			return betterListViewItem2;
		}

		/// <summary>
		///   Adds the specified key.
		/// </summary>
		/// <param name="text">Label of the item.</param>
		/// <param name="imageKey">Key specifying item image in the ImageList.</param>
		/// <returns>Added item.</returns>
		public BetterListViewItem Add(string text, string imageKey) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(text, imageKey);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Adds the specified key.
		/// </summary>
		/// <param name="text">Label of the item.</param>
		/// <param name="imageIndex">Index specifying item image in the ImageList.</param>
		/// <returns>Added item.</returns>
		public BetterListViewItem Add(string text, int imageIndex) {
			BetterListViewItem betterListViewItem = new BetterListViewItem(text, imageIndex);
			this.Add(betterListViewItem);
			return betterListViewItem;
		}

		/// <summary>
		///   Find all items with the name specified by the search key.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <param name="searchAllSubItems">Search in all sub-items of each item.</param>
		/// <returns>Array of items matching the specified search key.</returns>
		public BetterListViewItem[] Find(string key, bool searchAllSubItems) {
			List<BetterListViewItem> list = new List<BetterListViewItem>();
			if (!string.IsNullOrEmpty(key)) {
				if (searchAllSubItems) {
					foreach (BetterListViewItem inner in base.InnerList) {
						foreach (BetterListViewSubItem subItem in inner.SubItems) {
							string name = subItem.Name;
							if (!string.IsNullOrEmpty(name) && key.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
								list.Add(inner);
								break;
							}
						}
					}
				}
				else {
					foreach (BetterListViewItem inner2 in base.InnerList) {
						string name2 = inner2.Name;
						if (!string.IsNullOrEmpty(name2) && key.Equals(name2, StringComparison.InvariantCultureIgnoreCase)) {
							list.Add(inner2);
						}
					}
				}
			}
			return list.ToArray();
		}

		/// <summary>
		///   Make this collection internal.
		/// </summary>
		internal void SetInternal() {
			Checks.CheckFalse(base.IsInternal, "IsInternal");
			base.IsInternal = true;
			for (int i = 0; i < base.InnerList.Count; i++) {
				BetterListViewItem betterListViewItem = base.InnerList[i];
				betterListViewItem.Index = i;
				betterListViewItem.OwnerCollection = this;
			}
		}

		/// <summary>
		///   Creeate a new element from its text.
		/// </summary>
		/// <param name="text">element text</param>
		/// <returns>element instance</returns>
		protected override BetterListViewItem CreateElement(string text) {
			return new BetterListViewItem(text);
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection();
			foreach (BetterListViewItem current in this) {
				betterListViewItemCollection.Add((BetterListViewItem)current.Clone());
			}
			return betterListViewItemCollection;
		}
	}
}