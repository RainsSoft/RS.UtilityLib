using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView column headers.
	/// </summary>
	[Serializable]
	[Editor(typeof(BetterListViewColumnHeaderCollectionEditor), typeof(UITypeEditor))]
	[TypeConverter(typeof(BetterListViewColumnHeaderCollectionConverter))]
	public sealed class BetterListViewColumnHeaderCollection : BetterListViewElementCollection<BetterListViewColumnHeader>
	{
		private BetterListView ownerControl;

		/// <summary>
		///   control that owns this collection
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		internal override BetterListViewBase OwnerControl {
			get {
				return this.ownerControl;
			}
			set {
				Checks.CheckType(value, typeof(BetterListView), "value");
				this.ownerControl = (BetterListView)value;
			}
		}

		/// <summary>
		///   any of the column headers is sortable
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		internal bool ConsiderSorting {
			get {
				foreach (BetterListViewColumnHeader current in this) {
					BetterListViewColumnHeaderStyle style = current.GetStyle(this.ownerControl);
					if (style == BetterListViewColumnHeaderStyle.Sortable || style == BetterListViewColumnHeaderStyle.Unsortable) {
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		///   name of the collection element
		/// </summary>
		protected override string ElementName => "column header";

		/// <summary>
		///   Initialize a new BetterListViewColumnHeaderCollection instance.
		/// </summary>
		public BetterListViewColumnHeaderCollection()
			: this(null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewColumnHeaderCollection instance.
		/// </summary>
		/// <param name="enumerable">enumerable of column headers to create this collection from</param>
		public BetterListViewColumnHeaderCollection(IEnumerable<BetterListViewColumnHeader> enumerable)
			: this(isInternal: false, enumerable) {
		}

		/// <summary>
		///   Initialize a new BetterListViewColumnHeaderCollection instance.
		/// </summary>
		/// <param name="isInternal">this collection is for internal use in BetterListView</param>
		internal BetterListViewColumnHeaderCollection(bool isInternal)
			: this(isInternal, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewColumnHeaderCollection instance.
		/// </summary>
		/// <param name="isInternal">this collection is for internal use in BetterListView</param>
		/// <param name="enumerable">enumerable of column headers to create this collection from</param>
		internal BetterListViewColumnHeaderCollection(bool isInternal, IEnumerable<BetterListViewColumnHeader> enumerable)
			: base(isInternal, enumerable) {
		}

		private BetterListViewColumnHeaderCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(Image image) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(image);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="text">item text</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(string text) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(text);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="width">item width</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(int width) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(width);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <param name="text">item text</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(Image image, string text) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(image, text);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <param name="width">item width</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(Image image, int width) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(image, width);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="text">item text</param>
		/// <param name="width">item width</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(string text, int width) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(text, width);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Add a new item to the collection.
		/// </summary>
		/// <param name="image">item image</param>
		/// <param name="text">item text</param>
		/// <param name="width">item width</param>
		/// <returns>newly created BetterListViewColumnHeader instance</returns>
		public BetterListViewColumnHeader Add(Image image, string text, int width) {
			BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(image, text, width);
			this.Add(betterListViewColumnHeader);
			return betterListViewColumnHeader;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewColumnHeaderCollection betterListViewColumnHeaderCollection = new BetterListViewColumnHeaderCollection();
			foreach (BetterListViewColumnHeader current in this) {
				betterListViewColumnHeaderCollection.Add((BetterListViewColumnHeader)current.Clone());
			}
			return betterListViewColumnHeaderCollection;
		}

		/// <summary>
		///   Creeate a new element from its text.
		/// </summary>
		/// <param name="text">element text</param>
		/// <returns>element instance</returns>
		protected override BetterListViewColumnHeader CreateElement(string text) {
			return new BetterListViewColumnHeader(text);
		}
	}
}