using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView groups.
	/// </summary>
	[Serializable]
	[Editor(typeof(BetterListViewGroupCollectionEditor), typeof(UITypeEditor))]
	[TypeConverter(typeof(BetterListViewGroupCollectionConverter))]
	public sealed class BetterListViewGroupCollection : BetterListViewElementCollection<BetterListViewGroup>
	{
		private const string ElementBase = "base";

		private const string FieldDefaultGroup = "defaultGroup";

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
		///   name of the collection element
		/// </summary>
		protected override string ElementName => "group";

		/// <summary>
		///   Initialize a new BetterListViewGroupCollection instance.
		/// </summary>
		public BetterListViewGroupCollection()
			: this(isInternal: false) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroupCollection instance.
		/// </summary>
		/// <param name="enumerable">enumerable of items to create this collection from</param>
		public BetterListViewGroupCollection(IEnumerable<BetterListViewGroup> enumerable)
			: this(isInternal: false, enumerable) {
		}

		internal BetterListViewGroupCollection(bool isInternal)
			: this(isInternal, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroupCollection instance.
		/// </summary>
		/// <param name="isInternal">this collection is for internal use in BetterListView</param>
		/// <param name="enumerable">enumerable of items to create this collection from</param>
		internal BetterListViewGroupCollection(bool isInternal, IEnumerable<BetterListViewGroup> enumerable)
			: base(isInternal, enumerable) {
		}

		private BetterListViewGroupCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		/// <summary>
		///   Add a new group to the collection.
		/// </summary>
		/// <param name="header">group text</param>
		/// <returns>newly created BetterListViewGroup instance</returns>
		public BetterListViewGroup Add(string header) {
			BetterListViewGroup betterListViewGroup = new BetterListViewGroup(header);
			this.Add(betterListViewGroup);
			return betterListViewGroup;
		}

		/// <summary>
		///   Add a new group to the collection.
		/// </summary>
		/// <param name="key">group name</param>
		/// <param name="header">group text</param>
		/// <returns>newly created BetterListViewGroup instance</returns>
		public BetterListViewGroup Add(string key, string header) {
			BetterListViewGroup betterListViewGroup = new BetterListViewGroup(key, header);
			this.Add(betterListViewGroup);
			return betterListViewGroup;
		}

		/// <summary>
		///   Add a new group to the collection.
		/// </summary>
		/// <param name="header">group text</param>
		/// <param name="headerAlignment">group text alignment</param>
		/// <returns>newly created BetterListViewGroup instance</returns>
		public BetterListViewGroup Add(string header, TextAlignmentHorizontal headerAlignment) {
			BetterListViewGroup betterListViewGroup = new BetterListViewGroup(header, headerAlignment);
			this.Add(betterListViewGroup);
			return betterListViewGroup;
		}

		/// <summary>
		///   Add a new group to the collection.
		/// </summary>
		/// <param name="key">group name</param>
		/// <param name="header">group text</param>
		/// <param name="headerAlignment">group text alignment</param>
		/// <returns>newly created BetterListViewGroup instance</returns>
		public BetterListViewGroup Add(string key, string header, TextAlignmentHorizontal headerAlignment) {
			BetterListViewGroup betterListViewGroup = new BetterListViewGroup(key, header, headerAlignment);
			this.Add(betterListViewGroup);
			return betterListViewGroup;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewGroupCollection betterListViewGroupCollection = new BetterListViewGroupCollection();
			foreach (BetterListViewGroup current in this) {
				betterListViewGroupCollection.Add((BetterListViewGroup)current.Clone());
			}
			return betterListViewGroupCollection;
		}

		internal override void OnCollectionChanged(BetterListViewElementCollectionChangeInfo changeInfo) {
			if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add) {
				BetterListView betterListView = this.ownerControl;
				if (betterListView != null) {
					try {
						betterListView.BeginUpdate();
						foreach (BetterListViewGroup key in changeInfo.Elements.Keys) {
							foreach (BetterListViewItem item in key.Items) {
								if (item.ListView == null) {
									betterListView.Items.Add(item);
								}
							}
						}
					}
					finally {
						betterListView.EndUpdate(suppressRefresh: true);
					}
				}
			}
			base.OnCollectionChanged(changeInfo);
		}

		/// <summary>
		///   Creeate a new element from its text.
		/// </summary>
		/// <param name="text">element text</param>
		/// <returns>element instance</returns>
		protected override BetterListViewGroup CreateElement(string text) {
			return new BetterListViewGroup(text);
		}
	}
}