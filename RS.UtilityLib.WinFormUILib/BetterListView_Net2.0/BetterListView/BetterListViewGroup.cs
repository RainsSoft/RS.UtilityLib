using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a BetterListView group.
	/// </summary>
	[Serializable]
	[Designer(typeof(BetterListViewElementDesigner))]
	[ToolboxItem(false)]
	[TypeConverter(typeof(BetterListViewGroupConverter))]
	[DefaultProperty("Header")]
	public class BetterListViewGroup : BetterListViewElement, IEnumerable<BetterListViewItem>, IEnumerable, IBetterListViewLayoutElementDisplayable, IBetterListViewLayoutElementSelectable, IBetterListViewStateElement
	{
		private const TextTrimming DefaultTextTrimming = TextTrimming.EllipsisCharacter;

		private const float DefaultFontScale = 1.33333337f;

		private const bool DefaultIsExpanded = true;

		private const bool DefaultAllowShowExpandButton = true;

		private const string FieldAllowShowExpandButton = "allowShowExpandButton";

		private const string FieldFont = "font";

		private const string FieldForeColor = "foreColor";

		private const string FieldHeader = "header";

		private const string FieldHeaderAlignmentHorizontal = "headerAlignmentHorizontal";

		private const string FieldHeaderAlignmentHorizontalImage = "headerAlignmentHorizontalImage";

		private const string FieldHeaderAlignmentVertical = "headerAlignmentVertical";

		private const string FieldHeaderAlignmentVerticalImage = "headerAlignmentVerticalImage";

		private const string FieldIsExpanded = "isExpanded";

		private const string FieldTextTrimming = "textTrimming";

		private const string FieldToolTips = "toolTips";

		private MultilineText cachedMultilineText;

		private readonly BetterListView listView;

		private BetterListView cachedQueryListView;

		private ReadOnlyCollection<BetterListViewItem> cachedQueryItems;

		private int layoutIndexItemDisplayFirst;

		private int layoutIndexItemDisplayLast;

		private int layoutIndexItemSelectionFirst;

		private int layoutIndexItemSelectionLast;

		private BetterListViewLayoutMeasurement layoutMeasurementItems = BetterListViewLayoutMeasurement.Empty;

		private BetterListViewGroupBounds layoutBounds;

		private int layoutIndexDisplay = -1;

		private int layoutIndexSelection = -1;

		private static Font cachedDefaultFont;

		private Color backColor = Color.Empty;

		private Font font;

		private Color foreColor = Color.Empty;

		private string header = string.Empty;

		private TextAlignmentHorizontal headerAlignmentHorizontal = TextAlignmentHorizontal.Default;

		private BetterListViewImageAlignmentHorizontal headerAlignmentHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;

		private TextAlignmentVertical headerAlignmentVertical = TextAlignmentVertical.Default;

		private BetterListViewImageAlignmentVertical headerAlignmentVerticalImage = BetterListViewImageAlignmentVertical.Default;

		private bool isExpanded = true;

		private TextTrimming textTrimming = TextTrimming.EllipsisCharacter;

		private BetterListViewToolTipInfoCollection toolTips = new BetterListViewToolTipInfoCollection();

		private bool allowShowExpandButton = true;

		[NonSerialized]
		private readonly BetterListViewGroupItemCollection items;

		private readonly BetterListViewElementStateBox elementStateBox = new BetterListViewElementStateBox();

		/// <summary>
		///   this group is a default group
		/// </summary>
		internal bool IsDefault => this.listView != null;

		/// <summary>
		///   index of the first item layout element in the group
		/// </summary>
		internal int LayoutIndexItemDisplayFirst {
			get {
				return this.layoutIndexItemDisplayFirst;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexItemDisplayFirst = value;
			}
		}

		/// <summary>
		///   index of the last item layout element in the group
		/// </summary>
		internal int LayoutIndexItemDisplayLast {
			get {
				return this.layoutIndexItemDisplayLast;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexItemDisplayLast = value;
			}
		}

		/// <summary>
		///   index of the first selectable item layout element in the group
		/// </summary>
		internal int LayoutIndexItemSelectionFirst {
			get {
				return this.layoutIndexItemSelectionFirst;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexItemSelectionFirst = value;
			}
		}

		/// <summary>
		///   index of the last selectable item layout element in the group
		/// </summary>
		internal int LayoutIndexItemSelectionLast {
			get {
				return this.layoutIndexItemSelectionLast;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexItemSelectionLast = value;
			}
		}

		/// <summary>
		///   item layout measurement
		/// </summary>
		internal BetterListViewLayoutMeasurement LayoutMeasurementItems {
			get {
				return this.layoutMeasurementItems;
			}
			set {
				Checks.CheckTrue(value.ElementsPerRow >= 1, "value.ElementsPerRow >= 1");
				Checks.CheckTrue(value.Width >= 0, "value.Width >= 0");
				Checks.CheckTrue(value.Height >= 0, "value.Height >= 0");
				this.layoutMeasurementItems = value;
			}
		}

		BetterListViewElementBoundsBase IBetterListViewLayoutElementDisplayable.LayoutBounds {
			get {
				return this.layoutBounds;
			}
			set {
				Checks.CheckType(value, typeof(BetterListViewGroupBounds), "value", allowNull: true);
				this.layoutBounds = (BetterListViewGroupBounds)value;
			}
		}

		int IBetterListViewLayoutElementDisplayable.LayoutIndexDisplay {
			get {
				return this.layoutIndexDisplay;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexDisplay = value;
			}
		}

		int IBetterListViewLayoutElementSelectable.LayoutIndexSelection {
			get {
				return this.layoutIndexSelection;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				this.layoutIndexSelection = value;
			}
		}

		/// <summary>
		///   background color of the group
		/// </summary>
		[Description("Background color of the group")]
		[Category("Appearance")]
		public Color BackColor {
			get {
				return this.backColor;
			}
			set {
				if (!(this.backColor == value)) {
					this.backColor = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
				}
			}
		}

		/// <summary>
		///   font of the group text
		/// </summary>
		[Category("Appearance")]
		[Description("Font of the group text")]
		public override Font Font {
			get {
				if (this.font != null) {
					return this.font;
				}
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
					if (betterListView.FontGroups != null) {
						return betterListView.FontGroups;
					}
				}
				return BetterListViewGroup.DefaultFont;
			}
			set {
				Checks.CheckNotNull(value, "value");
				if (this.font != value) {
					this.font = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   foreground color the group text
		/// </summary>
		[Category("Appearance")]
		[Description("Foreground color the group text")]
		public override Color ForeColor {
			get {
				if (!this.foreColor.IsEmpty) {
					return this.foreColor;
				}
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
					if (!betterListView.ForeColorGroups.IsEmpty) {
						return betterListView.ForeColorGroups;
					}
				}
				return BetterListViewGroup.DefaultForeColor;
			}
			set {
				Checks.CheckFalse(value.IsEmpty, "value.IsEmpty");
				if (!(this.foreColor == value)) {
					this.foreColor = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
				}
			}
		}

		/// <summary>
		///   text of the group
		/// </summary>
		[DefaultValue("")]
		[Category("Appearance")]
		[Description("Text of the group")]
		public string Header {
			get {
				return this.header;
			}
			set {
				value = value ?? string.Empty;
				if (!this.header.Equals(value, StringComparison.Ordinal)) {
					this.header = value;
					this.ClearCache();
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.LayoutText);
				}
			}
		}

		/// <summary>
		///   horizontal alignment of the group text
		/// </summary>
		[Description("Horizontal alignment of the group text")]
		[Category("Appearance")]
		public TextAlignmentHorizontal HeaderAlignmentHorizontal {
			get {
				return this.headerAlignmentHorizontal;
			}
			set {
				if (this.headerAlignmentHorizontal != value) {
					this.headerAlignmentHorizontal = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   horizontal alignment of the group image
		/// </summary>
		[Category("Appearance")]
		[Description("Horizontal alignment of the group image")]
		public BetterListViewImageAlignmentHorizontal HeaderAlignmentHorizontalImage {
			get {
				return this.headerAlignmentHorizontalImage;
			}
			set {
				if (this.headerAlignmentHorizontalImage != value) {
					this.headerAlignmentHorizontalImage = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   vertical alignment of the group text
		/// </summary>
		[Description("Vertical alignment of the group text")]
		[Category("Appearance")]
		public TextAlignmentVertical HeaderAlignmentVertical {
			get {
				return this.headerAlignmentVertical;
			}
			set {
				if (this.headerAlignmentVertical != value) {
					this.headerAlignmentVertical = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   vertical alignment of the group image
		/// </summary>
		[Description("Vertical alignment of the group image")]
		[Category("Appearance")]
		public BetterListViewImageAlignmentVertical HeaderAlignmentVerticalImage {
			get {
				return this.headerAlignmentVerticalImage;
			}
			set {
				if (this.headerAlignmentVerticalImage != value) {
					this.headerAlignmentVerticalImage = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   the group is expanded to display its items
		/// </summary>
		internal bool IsExpanded => this.isExpanded;

		/// <summary>
		///   group text trimming
		/// </summary>
		internal override TextTrimming TextTrimming => this.textTrimming;

		/// <summary>
		///   information about ToolTips shown on this group
		/// </summary>
		internal BetterListViewToolTipInfoCollection ToolTips => this.toolTips;

		internal static Font DefaultFont {
			get {
				if (BetterListViewGroup.cachedDefaultFont == null) {
					float size = Control.DefaultFont.SizeInPoints * 1.33333337f;
					Font font = (BetterListViewGroup.cachedDefaultFont = WinFormsUtils.CreateFont(size));
				}
				return BetterListViewGroup.cachedDefaultFont;
			}
		}

		internal static Color DefaultForeColor {
			get {
				if (SystemColors.Window.GetBrightness() > 0.5f) {
					return Color.MidnightBlue;
				}
				return SystemColors.ControlText;
			}
		}

		/// <summary>
		///   allow displaying expand button on group
		/// </summary>
		internal bool AllowShowExpandButton {
			get {
				return this.allowShowExpandButton;
			}
			set {
				if (this.allowShowExpandButton != value) {
					this.allowShowExpandButton = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   address of this group
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewAddress Address => new BetterListViewAddress(base.Index, null, isGroup: true);

		/// <summary>
		///   Gets group boundaries.
		/// </summary>
		/// <returns><see cref="T:ComponentOwl.BetterListView.BetterListViewGroupBounds" /> instance if the element is active, null otherwise.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListViewGroupBounds Bounds => this.ListView?.GetGroupBounds(this);

		/// <summary>
		///   items contained within the group
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewItemCollection Items {
			get {
				if (this.IsDefault) {
					throw new InvalidOperationException("Cannot access items in default group directly. Use GetItems().");
				}
				return this.items;
			}
		}

		/// <summary>
		///   BetterListView instance in which this group is contained
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListView ListView {
			get {
				if (this.IsDefault) {
					return this.listView;
				}
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					return (BetterListView)this.OwnerCollection.OwnerControl;
				}
				return null;
			}
		}

		/// <summary>
		///   next group in the owner collection
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewGroup NextGroup {
			get {
				if (this.OwnerCollection == null) {
					return null;
				}
				BetterListViewGroupCollection betterListViewGroupCollection = (BetterListViewGroupCollection)this.OwnerCollection;
				if (base.Index == betterListViewGroupCollection.Count - 1) {
					return null;
				}
				return betterListViewGroupCollection[base.Index + 1];
			}
		}

		/// <summary>
		///   next non-empty group in the owner collection
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListViewGroup NextNonEmptyGroup {
			get {
				if (this.OwnerCollection == null) {
					return null;
				}
				BetterListViewGroupCollection betterListViewGroupCollection = (BetterListViewGroupCollection)this.OwnerCollection;
				for (int i = base.Index + 1; i < betterListViewGroupCollection.Count; i++) {
					BetterListViewGroup betterListViewGroup = betterListViewGroupCollection[i];
					if (betterListViewGroup.Items.Count != 0) {
						return betterListViewGroup;
					}
				}
				return null;
			}
		}

		/// <summary>
		///   previous group in the owner collection
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewGroup PreviousGroup {
			get {
				if (this.OwnerCollection == null || base.Index == 0) {
					return null;
				}
				return ((BetterListViewGroupCollection)this.OwnerCollection)[base.Index - 1];
			}
		}

		/// <summary>
		///   previous non-empty group in the owner collection
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListViewGroup PreviousNonEmptyGroup {
			get {
				if (this.OwnerCollection == null) {
					return null;
				}
				BetterListViewGroupCollection betterListViewGroupCollection = (BetterListViewGroupCollection)this.OwnerCollection;
				for (int num = base.Index - 1; num >= 0; num--) {
					BetterListViewGroup betterListViewGroup = betterListViewGroupCollection[num];
					if (betterListViewGroup.Items.Count != 0) {
						return betterListViewGroup;
					}
				}
				return null;
			}
		}

		/// <summary>
		///   Gets a value indicating whether this element is in active state.
		/// </summary>
		/// <value>
		///   <c>true</c> if this element is in active state; otherwise, <c>false</c>.
		/// </value>
		bool IBetterListViewStateElement.IsActive {
			get {
				BetterListViewElementState state = this.elementStateBox.State;
				if (state != BetterListViewElementState.Active && state != BetterListViewElementState.ActiveCoarse && state != BetterListViewElementState.ActiveFine) {
					return state == BetterListViewElementState.ActiveVisible;
				}
				return true;
			}
		}

		/// <summary>
		///   Current state of the element.
		/// </summary>
		BetterListViewElementState IBetterListViewStateElement.State => this.elementStateBox.State;

		internal void ClearCache() {
			if (this.cachedMultilineText != null) {
				this.cachedMultilineText.Dispose();
				this.cachedMultilineText = null;
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		public BetterListViewGroup()
			: this(null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		/// <param name="header">group text</param>
		public BetterListViewGroup(string header)
			: this(null, header) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		/// <param name="key">group name</param>
		/// <param name="header">group text</param>
		public BetterListViewGroup(string key, string header)
			: this(key, header, TextAlignmentHorizontal.Default) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		/// <param name="header">group text</param>
		/// <param name="headerAlignment">horizontal alignment of the group text</param>
		public BetterListViewGroup(string header, TextAlignmentHorizontal headerAlignment)
			: this(null, header, headerAlignment) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		/// <param name="key">group name</param>
		/// <param name="header">group text</param>
		/// <param name="headerAlignmentHorizontal">horizontal alignment of the group text</param>
		public BetterListViewGroup(string key, string header, TextAlignmentHorizontal headerAlignmentHorizontal)
			: this(key, header, headerAlignmentHorizontal, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewGroup instance.
		/// </summary>
		/// <param name="key">group name</param>
		/// <param name="header">group text</param>
		/// <param name="headerAlignmentHorizontal">horizontal alignment of the group text</param>
		/// <param name="listView">owner BetterListView control, specified on default groups only</param>
		internal BetterListViewGroup(string key, string header, TextAlignmentHorizontal headerAlignmentHorizontal, BetterListView listView)
			: base(null, new BetterListViewGroupBounds()) {
			if (key != null) {
				base.Name = key;
			}
			this.header = header ?? string.Empty;
			this.headerAlignmentHorizontal = headerAlignmentHorizontal;
			if (!this.IsDefault) {
				this.items = new BetterListViewGroupItemCollection(this);
			}
			this.listView = listView;
		}

		/// <summary>
		///   Invalidate this group for redrawing.
		/// </summary>
		public void Invalidate() {
			BetterListView betterListView = this.ListView;
			if (betterListView != null && ((IBetterListViewStateElement)this).State == BetterListViewElementState.ActiveVisible) {
				Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)this).LayoutBounds.BoundsSpacing;
				boundsSpacing.Offset(betterListView.OffsetContentFromAbsolute);
				betterListView.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, boundsSpacing);
			}
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewGroup betterListViewGroup = new BetterListViewGroup();
			this.CopyTo(betterListViewGroup);
			return betterListViewGroup;
		}

		/// <summary>
		///   Remove element from its owner collection.
		/// </summary>
		/// <returns>success</returns>
		public override bool Remove() {
			if (this.OwnerCollection == null) {
				return false;
			}
			return ((BetterListViewGroupCollection)this.OwnerCollection).Remove(this);
		}

		/// <summary>
		///   Check whether properties of this element equals properties of the specified element.
		/// </summary>
		/// <param name="other">Element to check.</param>
		/// <returns>
		///   Properties of this element equals properties of the specified element.
		/// </returns>
		public override bool EqualsContent(BetterListViewElementBase other) {
			if (this == other) {
				return true;
			}
			if (!(other is BetterListViewGroup betterListViewGroup) || !base.EqualsContent(other)) {
				return false;
			}
			if (this.items.Count == 0 && betterListViewGroup.items.Count == 0 && object.Equals(this.font, betterListViewGroup.font) && this.foreColor.Equals(betterListViewGroup.foreColor) && this.header.Equals(betterListViewGroup.header, StringComparison.Ordinal) && this.headerAlignmentHorizontal == betterListViewGroup.headerAlignmentHorizontal && this.headerAlignmentHorizontalImage == betterListViewGroup.headerAlignmentHorizontalImage && this.headerAlignmentVertical == betterListViewGroup.headerAlignmentVertical && this.headerAlignmentVerticalImage == betterListViewGroup.headerAlignmentVerticalImage && this.textTrimming == betterListViewGroup.textTrimming) {
				return this.toolTips.EqualsContent(betterListViewGroup.toolTips);
			}
			return false;
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			if (this.OwnerCollection == null) {
				return base.GetType().Name + ": {Header = '" + this.header + "'}";
			}
			return base.GetType().Name + ": {Index = '" + base.Index.ToString(CultureInfo.InvariantCulture) + "', Header = '" + this.header + "'}";
		}

		/// <summary>
		///   Report that property of this element has changed.
		/// </summary>
		/// <param name="elementPropertyType">type of the changed property</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		/// <param name="eventArgs">event data (in case of property change event should be fired)</param>
		internal override void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, object oldValue, EventArgs eventArgs) {
			this.ClearCache();
			this.ListView?.OnElementPropertyChanged(this.OwnerCollection, elementPropertyType, this, oldValue, eventArgs);
		}

		internal void ClearCachedItems() {
			this.cachedQueryListView = null;
			this.cachedQueryItems = null;
		}

		/// <summary>
		///   Called when underlying collection has been modified.
		/// </summary>
		internal void OnCollectionChanged() {
			this.cachedQueryListView = null;
		}

		/// <summary>
		///   Get items contained in the specified BetterListView.
		/// </summary>
		/// <param name="listView">BetterListView to get grouped items for</param>
		/// <returns>subset of items from the group</returns>
		internal ReadOnlyCollection<BetterListViewItem> GetItems(BetterListView listView) {
			Checks.CheckNotNull(listView, "listView");
			if (listView != this.cachedQueryListView) {
				this.cachedQueryListView = listView;
				List<BetterListViewItem> list = new List<BetterListViewItem>();
				if (this.IsDefault) {
					foreach (BetterListViewItem item in listView.Items) {
						if (item.Group == null) {
							list.Add(item);
						}
					}
				}
				else {
					foreach (BetterListViewItem item2 in this.Items) {
						if (item2.ListView != listView) {
							continue;
						}
						BetterListViewItem betterListViewItem = item2;
						bool flag = false;
						while (betterListViewItem.ParentItem != null) {
							betterListViewItem = betterListViewItem.ParentItem;
							if (this.Items.Contains(betterListViewItem)) {
								flag = true;
								break;
							}
						}
						if (!flag) {
							list.Add(item2);
						}
					}
				}
				list.Sort(BetterListViewElementBaseIndexComparer<BetterListViewItem>.Instance);
				this.cachedQueryItems = list.AsReadOnly();
			}
			return this.cachedQueryItems;
		}

		/// <summary>
		///   Copy content of this instance to the specified BetterListViewElementBase instance.
		/// </summary>
		/// <param name="element">BetterListViewElementBase to copy the content to</param>
		protected override void CopyTo(BetterListViewElementBase element) {
			BetterListViewGroup betterListViewGroup = (BetterListViewGroup)element;
			betterListViewGroup.Name = base.Name;
			betterListViewGroup.Header = this.Header;
			betterListViewGroup.HeaderAlignmentHorizontal = this.HeaderAlignmentHorizontal;
			betterListViewGroup.HeaderAlignmentVerticalImage = this.HeaderAlignmentVerticalImage;
			betterListViewGroup.HeaderAlignmentVertical = this.HeaderAlignmentVertical;
			betterListViewGroup.HeaderAlignmentVerticalImage = this.HeaderAlignmentVerticalImage;
			base.CopyTo(betterListViewGroup);
		}

		/// <summary>
		///   Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
		///   Value 
		///   Meaning 
		///   Less than zero 
		///   This object is less than the <paramref name="other" /> parameter.
		///   Zero 
		///   This object is equal to <paramref name="other" />. 
		///   Greater than zero 
		///   This object is greater than <paramref name="other" />. 
		/// </returns>
		/// <param name="other">An object to compare with this object.
		/// </param>
		public override int CompareTo(BetterListViewElementBase other) {
			if (other == null) {
				return 0;
			}
			if (!(other is BetterListViewGroup betterListViewGroup)) {
				return 0;
			}
			return string.Compare(this.Header, betterListViewGroup.Header, StringComparison.CurrentCulture);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<BetterListViewItem> GetEnumerator() {
			if (this.ListView == null) {
				return this.items.GetEnumerator();
			}
			return new BetterListViewGroupItemEnumerator(this);
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		internal void DrawingDrawText(Graphics graphics, Color color, Rectangle bounds) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "bounds.Width >= 0 && bounds.Height >= 0");
			this.UpdateCachedMultilineText();
			MultilineTextRenderer.DrawText(graphics, color, this.cachedMultilineText, bounds, this.HeaderAlignmentHorizontal, this.HeaderAlignmentVertical);
		}

		internal TextSize LayoutGetTextSize(Graphics graphics, int width) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(width >= 0, "width >= 0");
			this.UpdateCachedMultilineText();
			return MultilineTextRenderer.MeasureText(graphics, width, this.cachedMultilineText);
		}

		private void UpdateCachedMultilineText() {
			if (this.cachedMultilineText == null) {
				this.cachedMultilineText = new MultilineText(this.Header, (Font)this.Font.Clone(), this.TextTrimming, int.MaxValue, TextOptions.None);
			}
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetBackColor() {
			this.BackColor = Color.Empty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetFont() {
			this.font = null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetForeColor() {
			this.foreColor = Color.Empty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetHeaderAlignmentHorizontal() {
			this.HeaderAlignmentHorizontal = TextAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetHeaderAlignmentHorizontalImage() {
			this.HeaderAlignmentHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetHeaderAlignmentVertical() {
			this.HeaderAlignmentVertical = TextAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetHeaderAlignmentVerticalImage() {
			this.HeaderAlignmentVerticalImage = BetterListViewImageAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeBackColor() {
			return !this.BackColor.IsEmpty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeFont() {
			return this.font != BetterListViewGroup.DefaultFont;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeForeColor() {
			return !this.foreColor.IsEmpty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeHeaderAlignmentHorizontal() {
			return this.HeaderAlignmentHorizontal != TextAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeHeaderAlignmentHorizontalImage() {
			return this.HeaderAlignmentHorizontalImage != BetterListViewImageAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeHeaderAlignmentVertical() {
			return this.HeaderAlignmentVertical != TextAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeHeaderAlignmentVerticalImage() {
			return this.HeaderAlignmentVerticalImage != BetterListViewImageAlignmentVertical.Default;
		}

		private BetterListViewGroup(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.items = new BetterListViewGroupItemCollection(this);
			this.allowShowExpandButton = info.GetBoolean("allowShowExpandButton");
			string @string = info.GetString("font");
			this.font = ((@string.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, @string)) : null);
			this.foreColor = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("foreColor"));
			this.header = info.GetString("header");
			this.headerAlignmentHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), info.GetString("headerAlignmentHorizontal"));
			this.headerAlignmentHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), info.GetString("headerAlignmentHorizontalImage"));
			this.headerAlignmentVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), info.GetString("headerAlignmentVertical"));
			this.headerAlignmentVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), info.GetString("headerAlignmentVerticalImage"));
			this.isExpanded = info.GetBoolean("isExpanded");
			this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), info.GetString("textTrimming"));
			this.toolTips = (BetterListViewToolTipInfoCollection)info.GetValue("toolTips", typeof(BetterListViewToolTipInfoCollection));
			((IBetterListViewLayoutElementDisplayable)this).LayoutBounds = new BetterListViewGroupBounds();
		}

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		///   The caller does not have the required permission.
		/// </exception>
		protected override void GetObjectDataInternal(SerializationInfo info, StreamingContext context) {
			info.AddValue("allowShowExpandButton", this.allowShowExpandButton);
			info.AddValue("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
			info.AddValue("foreColor", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
			info.AddValue("header", this.header);
			info.AddValue("headerAlignmentHorizontal", this.headerAlignmentHorizontal.ToString());
			info.AddValue("headerAlignmentHorizontalImage", this.headerAlignmentHorizontalImage.ToString());
			info.AddValue("headerAlignmentVertical", this.headerAlignmentVertical.ToString());
			info.AddValue("headerAlignmentVerticalImage", this.headerAlignmentVerticalImage.ToString());
			info.AddValue("isExpanded", this.isExpanded);
			info.AddValue("textTrimming", this.textTrimming.ToString());
			info.AddValue("toolTips", this.toolTips, typeof(BetterListViewToolTipInfoCollection));
			base.GetObjectDataInternal(info, context);
		}

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		protected override void ReadXmlInternal(XmlReader reader) {
			reader.MoveToContent();
			reader.ReadStartElement();
			this.allowShowExpandButton = bool.Parse(reader.ReadElementString("allowShowExpandButton"));
			string text = reader.ReadElementString("font");
			this.font = ((text.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, text)) : null);
			this.foreColor = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("foreColor"));
			this.header = reader.ReadElementString("header");
			this.headerAlignmentHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), reader.ReadElementString("headerAlignmentHorizontal"));
			this.headerAlignmentHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), reader.ReadElementString("headerAlignmentHorizontalImage"));
			this.headerAlignmentVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), reader.ReadElementString("headerAlignmentVertical"));
			this.headerAlignmentVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), reader.ReadElementString("headerAlignmentVerticalImage"));
			this.isExpanded = bool.Parse(reader.ReadElementString("isExpanded"));
			this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), reader.ReadElementString("textTrimming"));
			reader.ReadStartElement("toolTips");
			this.toolTips = (BetterListViewToolTipInfoCollection)new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Deserialize(reader);
			reader.ReadEndElement();
			base.ReadXmlInternal(reader);
			reader.ReadEndElement();
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		protected override void WriteXmlInternal(XmlWriter writer) {
			writer.WriteElementString("allowShowExpandButton", this.allowShowExpandButton.ToString());
			writer.WriteElementString("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
			writer.WriteElementString("foreColor", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
			writer.WriteElementString("header", this.header);
			writer.WriteElementString("headerAlignmentHorizontal", this.headerAlignmentHorizontal.ToString());
			writer.WriteElementString("headerAlignmentHorizontalImage", this.headerAlignmentHorizontalImage.ToString());
			writer.WriteElementString("headerAlignmentVertical", this.headerAlignmentVertical.ToString());
			writer.WriteElementString("headerAlignmentVerticalImage", this.headerAlignmentVerticalImage.ToString());
			writer.WriteElementString("isExpanded", this.isExpanded.ToString());
			writer.WriteElementString("textTrimming", this.textTrimming.ToString());
			writer.WriteStartElement("toolTips");
			new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Serialize(writer, this.toolTips);
			writer.WriteEndElement();
			base.WriteXmlInternal(writer);
		}

		/// <summary>
		///   Change state of the element with the specified state transition.
		/// </summary>
		/// <param name="stateChange">State transition.</param>
		void IBetterListViewStateElement.ChangeState(BetterListViewElementStateChange stateChange) {
			this.elementStateBox.ChangeState(stateChange);
		}
	}
}