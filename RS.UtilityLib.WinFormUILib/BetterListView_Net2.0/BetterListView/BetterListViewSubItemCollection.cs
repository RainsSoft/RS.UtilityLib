using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView sub-items.
	/// </summary>
	[Serializable]
	[Editor(typeof(BetterListViewSubItemCollectionEditor), typeof(UITypeEditor))]
	[TypeConverter(typeof(BetterListViewSubItemCollectionConverter))]
	public sealed class BetterListViewSubItemCollection : BetterListViewElementCollection<BetterListViewSubItem>
	{
		[NonSerialized]
		private BetterListViewItem ownerItem;

		/// <summary>
		///   control that owns this collection
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		internal override BetterListViewBase OwnerControl {
			get {
				if (this.OwnerItem == null || this.OwnerItem.OwnerCollection == null) {
					return null;
				}
				return this.OwnerItem.OwnerCollection.OwnerControl;
			}
			set {
				throw new NotSupportedException("Setting owner control on sub-item collection not supported.");
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
				this.ownerItem = value;
			}
		}

		/// <summary>
		///   name of the collection element
		/// </summary>
		protected override string ElementName => "sub-item";

		/// <summary>
		///   Initialize a new BetterListViewSubItemCollection instance.
		/// </summary>
		public BetterListViewSubItemCollection()
			: this(isInternal: false) {
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItemCollection instance.
		/// </summary>
		/// <param name="enumerable">enumerable to create this collection from</param>
		public BetterListViewSubItemCollection(IEnumerable<BetterListViewSubItem> enumerable)
			: this(isInternal: false, enumerable) {
		}

		internal BetterListViewSubItemCollection(bool isInternal)
			: this(isInternal, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItemCollection instance.
		/// </summary>
		/// <param name="isInternal">this collection is for internal use in BetterListView</param>
		/// <param name="enumerable">enumerable to create this collection from</param>
		internal BetterListViewSubItemCollection(bool isInternal, IEnumerable<BetterListViewSubItem> enumerable)
			: base(isInternal, enumerable) {
			if (this.Count == 0 && isInternal) {
				this.Add(new BetterListViewSubItem());
			}
		}

		private BetterListViewSubItemCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		/// <summary>
		///   Add a new sub-item to the collection.
		/// </summary>
		/// <param name="image">sub-item image</param>
		/// <returns>newly created BetterListViewSubItem instance</returns>
		public BetterListViewSubItem Add(Image image) {
			BetterListViewSubItem betterListViewSubItem = new BetterListViewSubItem(image);
			this.Add(betterListViewSubItem);
			return betterListViewSubItem;
		}

		/// <summary>
		///   Add a new sub-item to the collection.
		/// </summary>
		/// <param name="text">sub-item text</param>
		/// <returns>newly created BetterListViewSubItem instance</returns>
		public BetterListViewSubItem Add(string text) {
			BetterListViewSubItem betterListViewSubItem = new BetterListViewSubItem(text);
			this.Add(betterListViewSubItem);
			return betterListViewSubItem;
		}

		/// <summary>
		///   Add a new sub-item to the collection.
		/// </summary>
		/// <param name="image">sub-item image</param>
		/// <param name="text">sub-item text</param>
		/// <returns>newly created BetterListViewSubItem instance</returns>
		public BetterListViewSubItem Add(Image image, string text) {
			BetterListViewSubItem betterListViewSubItem = new BetterListViewSubItem(image, text);
			this.Add(betterListViewSubItem);
			return betterListViewSubItem;
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
		/// </exception>
		public override void Clear() {
			if (base.IsInternal) {
				if (this.Count > 1) {
					base.RemoveRange(1, this.Count - 1);
				}
			}
			else {
				base.Clear();
			}
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewSubItemCollection betterListViewSubItemCollection = new BetterListViewSubItemCollection();
			foreach (BetterListViewSubItem current in this) {
				betterListViewSubItemCollection.Add((BetterListViewSubItem)current.Clone());
			}
			return betterListViewSubItemCollection;
		}

		/// <summary>
		///   Make this collection internal.
		/// </summary>
		internal void SetInternal() {
			Checks.CheckFalse(base.IsInternal, "IsInternal");
			base.IsInternal = true;
			for (int i = 0; i < base.InnerList.Count; i++) {
				BetterListViewSubItem betterListViewSubItem = base.InnerList[i];
				betterListViewSubItem.Index = i;
				betterListViewSubItem.OwnerCollection = this;
			}
		}

		/// <summary>
		///   Creeate a new element from its text.
		/// </summary>
		/// <param name="text">element text</param>
		/// <returns>element instance</returns>
		protected override BetterListViewSubItem CreateElement(string text) {
			return new BetterListViewSubItem(text);
		}
	}
}