using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents layout of column header elements.
	/// </summary>
	public abstract class BetterListViewLayoutItems : BetterListViewLayout<BetterListViewItem>
	{
		private const int DefaultExpandButtonPadding = 2;

		private const int DefaultCheckBoxPadding = 2;

		private bool center;

		private int itemWidth;

		private int itemHeight;

		private Padding expandButtonPadding;

		private Padding checkBoxPadding;

		private ImageBorderType imageBorderType;

		private int imageBorderThickness;

		private Color imageBorderColor;

		private Size emptyTextSize;

		private int maximumTextLines;

		private BetterListViewImageSize[] subItemImageSizes;

		/// <summary>
		///   layout allows expand buttons on items
		/// </summary>
		public abstract bool AllowsExpandableItems { get; }

		/// <summary>
		///   items are directed vertically
		/// </summary>
		public abstract bool DirectionVertical { get; }

		/// <summary>
		///   there should be only single boundary between neighboring item selections
		/// </summary>
		public abstract bool SingleBoundary { get; }

		/// <summary>
		///   Center items to the client area.
		/// </summary>
		/// <remarks>
		///   Applies to some layouts only.
		/// </remarks>
		public bool Center {
			get {
				return this.center;
			}
			set {
				this.center = value;
				base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
			}
		}

		/// <summary>
		///   minimum item inner area width
		/// </summary>
		public int ItemWidth {
			get {
				return this.itemWidth;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				if (this.itemWidth != value) {
					this.itemWidth = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   minimum item inner area height
		/// </summary>
		public int ItemHeight {
			get {
				return this.itemHeight;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				if (this.itemHeight != value) {
					this.itemHeight = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item expand button padding
		/// </summary>
		public Padding ExpandButtonPadding {
			get {
				return this.expandButtonPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.expandButtonPadding == value)) {
					this.expandButtonPadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item check box padding
		/// </summary>
		public Padding CheckBoxPadding {
			get {
				return this.checkBoxPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.checkBoxPadding == value)) {
					this.checkBoxPadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item image border style
		/// </summary>
		public ImageBorderType ImageBorderType {
			get {
				return this.imageBorderType;
			}
			set {
				Checks.CheckNotEqual(value, ImageBorderType.Undefined, "value", "ImageBorderType.Undefined");
				if (this.imageBorderType != value) {
					this.imageBorderType = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item image border thickness
		/// </summary>
		public int ImageBorderThickness {
			get {
				return this.imageBorderThickness;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				if (this.imageBorderThickness != value) {
					this.imageBorderThickness = Math.Min(Math.Max(value, 0), 16);
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item image border color
		/// </summary>
		public Color ImageBorderColor {
			get {
				return this.imageBorderColor;
			}
			set {
				Checks.CheckFalse(value.IsEmpty, "value.IsEmpty");
				if (!(this.imageBorderColor == value)) {
					this.imageBorderColor = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   item text size (when the text is empty)
		/// </summary>
		public Size EmptyTextSize {
			get {
				return this.emptyTextSize;
			}
			set {
				Checks.CheckSize(value, "value");
				if (!(this.emptyTextSize == value)) {
					this.emptyTextSize = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   maximum allowed number of item text lines
		/// </summary>
		public int MaximumTextLines {
			get {
				return this.maximumTextLines;
			}
			set {
				Checks.CheckTrue(value >= 1, "value >= 1");
				if (this.maximumTextLines != value) {
					this.maximumTextLines = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Text);
				}
			}
		}

		/// <summary>
		///   Item image size.
		/// </summary>
		public BetterListViewImageSize ImageSize {
			get {
				return this.subItemImageSizes[0];
			}
			set {
				this.SubItemImageSizes = new BetterListViewImageSize[1] { value };
			}
		}

		/// <summary>
		///   Sub-item image sizes.
		/// </summary>
		public BetterListViewImageSize[] SubItemImageSizes {
			get {
				return this.subItemImageSizes;
			}
			set {
				Checks.CheckNotNull(value, "value");
				Checks.CheckTrue(value.Length != 0, "value.Length != 0");
				if (this.subItemImageSizes.Length == value.Length) {
					bool flag = false;
					for (int i = 0; i < this.subItemImageSizes.Length; i++) {
						if (this.subItemImageSizes[i] != value[i]) {
							flag = true;
							break;
						}
					}
					if (!flag) {
						return;
					}
				}
				this.subItemImageSizes = value;
				base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
			}
		}

		/// <summary>
		///   Level of invalidation caused by resizing of the control.
		/// </summary>
		internal abstract BetterListViewInvalidationInfo ResizeInvalidationInfo { get; }

		/// <summary>
		///   The layout uses custom padding (LayoutPadding property does not apply).
		/// </summary>
		internal abstract bool CustomPadding { get; }

		/// <summary>
		///   default centering of the layout
		/// </summary>
		protected abstract bool DefaultCenter { get; }

		/// <summary>
		///   default item width
		/// </summary>
		protected abstract int DefaultItemWidth { get; }

		/// <summary>
		///   default item height
		/// </summary>
		protected abstract int DefaultItemHeight { get; }

		/// <summary>
		///   default item image border type
		/// </summary>
		protected abstract ImageBorderType DefaultImageBorderType { get; }

		/// <summary>
		///   default item image border thickness
		/// </summary>
		protected abstract int DefaultImageBorderThickness { get; }

		/// <summary>
		///   default item text size
		/// </summary>
		protected abstract Size DefaultEmptyTextSize { get; }

		/// <summary>
		///   default maximum text lines of an item
		/// </summary>
		protected abstract int DefaultMaximumTextLines { get; }

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutItems" /> class.
		/// </summary>
		/// <param name="listView">control containing the layout</param>
		protected internal BetterListViewLayoutItems(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		/// Get indentation (in pixels) of a child item.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="level">Child item level.</param>
		/// <returns>Indentation (in pixels) of a child item.</returns>
		public int GetChildItemOffset(Graphics graphics, int level) {
			return this.GetChildItemOffset(graphics, -1, level);
		}

		/// <summary>
		/// Get indentation (in pixels) of a child item.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="indent">Custom indentation (in pixels).</param>
		/// <param name="level">Child item level.</param>
		/// <returns>Indentation (in pixels) of a child item.</returns>
		public int GetChildItemOffset(Graphics graphics, int indent, int level) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(indent == -1 || indent >= 0, "indent == BetterListView.DefaultIndent || indent >= 0");
			Checks.CheckTrue(level >= 0, "level >= 0");
			return this.GetChildItemOffset(level, this.GetLevelOffset(graphics, indent));
		}

		internal abstract int GetSubItemCount(BetterListViewItem item, int columnCount);

		internal void CheckOverflows(ReadOnlyCollection<BetterListViewItem> elementsItems, int indexElementFirst, int indexElementLast, Size contentSize, ref bool overflowsHorizontal, ref bool overflowsVertical) {
			Checks.CheckNotNull(elementsItems, "elementsItems");
			if (elementsItems.Count != 0) {
				Checks.CheckBounds(indexElementFirst, 0, elementsItems.Count - 1, "indexElementFirst");
				Checks.CheckBounds(indexElementLast, indexElementFirst, elementsItems.Count - 1, "indexElementLast");
			}
			else {
				Checks.CheckEqual(indexElementFirst, 0, "indexElementFirst", "0");
				Checks.CheckEqual(indexElementLast, 0, "indexElementLast", "0");
			}
			Checks.CheckTrue(contentSize.Width >= 0 && contentSize.Height >= 0, "contentSize.Width >= 0 && sizeContent.Height >= 0");
			if (elementsItems.Count == 0) {
				overflowsHorizontal = false;
				overflowsVertical = false;
			}
			else {
				overflowsHorizontal = this.CheckOverflows(elementsItems, indexElementFirst, indexElementLast, contentSize, vertical: false);
				overflowsVertical = this.CheckOverflows(elementsItems, indexElementFirst, indexElementLast, contentSize, vertical: true);
			}
		}

		/// <summary>
		///   Get size of a sub-item image.
		/// </summary>
		/// <param name="index">sub-item index</param>
		/// <returns>sub-item image size</returns>
		internal BetterListViewImageSize GetSubItemImageSize(int index) {
			if (index < this.subItemImageSizes.Length) {
				return this.subItemImageSizes[index];
			}
			return BetterListViewImageSize.Empty;
		}

		/// <summary>
		///   Get single level offset (in pixels) for the specified item indentation.
		/// </summary>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <param name="indent">item indentatin</param>
		/// <returns>single level offset (in pixels)</returns>
		internal int GetLevelOffset(Graphics graphics, int indent) {
			if (indent == -1) {
				return base.ElementInnerPadding.Left + this.ExpandButtonPadding.Left + BetterListViewBasePainter.GetNodeGlyphSize(graphics).Width;
			}
			return indent;
		}

		/// <summary>
		///   Get offset of a child item (in pixels) for the specified level and level offset.
		/// </summary>
		/// <param name="level">child item level</param>
		/// <param name="levelOffset">single level offset (in pixels)</param>
		/// <returns>child item offset (in pixels)</returns>
		internal int GetChildItemOffset(int level, int levelOffset) {
			return level * levelOffset;
		}

		/// <summary>
		///   Get maximum allowed number of text lines for the specified sub-item.
		/// </summary>
		/// <param name="subItem">Sub-item to get maximum allowed number of text lines for.</param>
		/// <returns>Maximum allowed number of text lines.</returns>
		protected internal int GetMaximumTextLines(BetterListViewSubItem subItem) {
			int num = subItem.MaximumTextLines;
			if (num == 0) {
				return this.MaximumTextLines;
			}
			return num;
		}

		/// <summary>
		///   Measure the layout.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to measure</param>
		/// <param name="indexElementLast">index of the last layout element to measure</param>
		/// <param name="contentSize">content area size</param>
		/// <returns>layout measurement</returns>
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewItem> elements, int indexElementFirst, int indexElementLast, Size contentSize) {
			return this.MeasureLayout(elements, indexElementFirst, indexElementLast, contentSize, customPadding: false);
		}

		/// <summary>
		///   Measure the layout.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to measure</param>
		/// <param name="indexElementLast">index of the last layout element to measure</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="customPadding">custom layout padding is used</param>
		/// <returns>layout measurement</returns>
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewItem> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool customPadding) {
			BetterListViewLayoutMeasurement result = base.MeasureLayout(elements, indexElementFirst, indexElementLast, contentSize, customPadding);
			if (this.SingleBoundary) {
				result = new BetterListViewLayoutMeasurement(result.ElementsPerRow, result.Width, result.Height + 1);
			}
			return result;
		}

		internal abstract BetterListViewCommonMeasurementItems MeasureElements(ICollection<BetterListViewItem> elementsItems, Graphics graphics, Size contentSize, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool enableExpandButtons, bool enableCheckBoxes, int indent);

		internal abstract void MeasureElementCoarse(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets, IDictionary<int, int> usedFontHeights, int resizedColumnIndex);

		/// <summary>
		///   Measure all item boundaries.
		/// </summary>
		/// <param name="item">item to measure</param>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <param name="imageList">ImageList for items</param>
		/// <param name="elementsColumns">column headers layout elements</param>
		/// <param name="fullRowSelect">selections go over all columns</param>
		/// <param name="enableExpandButton">enable showing expand button</param>
		/// <param name="showExpandButton">show expand button</param>
		/// <param name="enableCheckBox">enable check box</param>
		/// <param name="showCheckBox">show check box</param>
		/// <param name="extraPadding">layout extra padding</param>
		/// <param name="commonMeasurement">common item layout element measurement information</param>
		/// <param name="itemOffsets">item offsets in item hierarchy (in pixels)</param>
		internal abstract void MeasureElementFine(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets);

		internal virtual void PositionElements(ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding) {
			this.PositionElements(elementsItems, columnHeaderBoundsFirst, indexElementFirst, indexElementLast, contentSize, extraPadding, customPadding: false);
		}

		internal virtual void PositionElements(ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding, bool customPadding) {
			Checks.CheckNotNull(elementsItems, "elementsItems");
			if (elementsItems.Count != 0) {
				Checks.CheckBounds(indexElementFirst, 0, elementsItems.Count - 1, "indexElementFirst");
				Checks.CheckTrue(indexElementLast >= indexElementFirst, "indexElementLast >= indexElementFirst");
			}
			else {
				Checks.CheckEqual(indexElementFirst, 0, "indexElementFirst", "0");
				Checks.CheckEqual(indexElementLast, 0, "indexElementLast", "0");
			}
			Checks.CheckSize(extraPadding, "extraPadding");
			base.PositionElements(elementsItems, indexElementFirst, indexElementLast, contentSize, extraPadding, customPadding);
		}

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected override void SetDefaultsInternal() {
			this.center = this.DefaultCenter;
			this.itemWidth = this.DefaultItemWidth;
			this.itemHeight = this.DefaultItemHeight;
			this.expandButtonPadding = new Padding(2);
			this.checkBoxPadding = new Padding(2);
			this.imageBorderType = this.DefaultImageBorderType;
			this.imageBorderThickness = this.DefaultImageBorderThickness;
			this.imageBorderColor = Painter.DefaultBorderColor;
			this.emptyTextSize = this.DefaultEmptyTextSize;
			this.maximumTextLines = this.DefaultMaximumTextLines;
			this.subItemImageSizes = new BetterListViewImageSize[1] { this.DefaultImageSize };
			base.SetDefaultsInternal();
		}
	}
}