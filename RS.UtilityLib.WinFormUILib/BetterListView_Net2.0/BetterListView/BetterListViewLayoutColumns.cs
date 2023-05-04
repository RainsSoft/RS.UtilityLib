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
	public sealed class BetterListViewLayoutColumns : BetterListViewLayout<BetterListViewColumnHeader>
	{
		private const int DefaultBorderSize = 16;

		private const int DefaultSortGlyphPadding = 0;

		private const int DefaultMinimumTextHeight = 16;

		private const int DefaultMaximumTextLines = 1;

		private int borderSize;

		private BetterListViewImageSize imageSize;

		private Padding sortGlyphPadding;

		private int minimumTextHeight;

		private int maximumTextLines;

		/// <summary>
		///   layout elements are oriented vertically
		/// </summary>
		public override bool OrientationVertical => false;

		/// <summary>
		///   column header border extent
		/// </summary>
		public int BorderSize {
			get {
				return this.borderSize;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				if (this.borderSize != value) {
					this.borderSize = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   column header image size
		/// </summary>
		public BetterListViewImageSize ImageSize {
			get {
				return this.imageSize;
			}
			set {
				if (!(this.imageSize == value)) {
					this.imageSize = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   column header sort glyph padding
		/// </summary>
		public Padding SortGlyphPadding {
			get {
				return this.sortGlyphPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.sortGlyphPadding == value)) {
					this.sortGlyphPadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   minimum column header text area height
		/// </summary>
		public int MinimumTextHeight {
			get {
				return this.minimumTextHeight;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				if (this.minimumTextHeight != value) {
					this.minimumTextHeight = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   maximum allowed number of text lines
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
		///   layout positioning options
		/// </summary>
		internal override BetterListViewLayoutPositioningOptions PositioningOptions => BetterListViewLayoutPositioningOptions.None;

		/// <summary>
		///   each row contains just a single item, even if there is enough space on the row for more items
		/// </summary>
		protected override bool SingleItemPerRow => true;

		/// <summary>
		///   default layout padding
		/// </summary>
		protected override Padding DefaultLayoutPadding => Padding.Empty;

		/// <summary>
		///   default outer padding of a layout element
		/// </summary>
		protected override Size DefaultElementOuterPadding => Size.Empty;

		/// <summary>
		///   default inner padding of a layout element
		/// </summary>
		protected override Padding DefaultElementInnerPadding => new Padding(2);

		/// <summary>
		///   default image size of a layout element
		/// </summary>
		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(new Size(16, 16), new Size(64, 64));

		/// <summary>
		///   default image padding of a layout element
		/// </summary>
		protected override Padding DefaultImagePadding => new Padding(4);

		/// <summary>
		///   default text padding of a layout element
		/// </summary>
		protected override Padding DefaultTextPadding => new Padding(2);

		/// <summary>
		///   default horizontal element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentHorizontal DefaultDefaultImageAlignmentHorizontal => BetterListViewImageAlignmentHorizontal.BeforeTextCenter;

		/// <summary>
		///   default vertical element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentVertical DefaultDefaultImageAlignmentVertical => BetterListViewImageAlignmentVertical.Middle;

		/// <summary>
		///   default horizontal element text alignment
		/// </summary>
		protected override TextAlignmentHorizontal DefaultDefaultTextAlignmentHorizontal => TextAlignmentHorizontal.Left;

		/// <summary>
		///   default vertical element text alignment
		/// </summary>
		protected override TextAlignmentVertical DefaultDefaultTextAlignmentVertical => TextAlignmentVertical.Middle;

		/// <summary>
		///   default item/sub-item text trimming
		/// </summary>
		protected override TextTrimming DefaultDefaultTextTrimming => TextTrimming.EllipsisCharacter;

		/// <summary>
		///   Initialize a new BetterListViewLayoutColumns instance.
		/// </summary>
		/// <param name="listView">BetterListView using this layout</param>
		internal BetterListViewLayoutColumns(BetterListView listView)
			: base(listView) {
		}

		internal void CheckOverflows(ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, Size contentSize, ref bool overflowsHorizontal, ref bool overflowsVertical) {
			Checks.CheckNotNull(elementsColumns, "elements");
			Checks.CheckTrue(contentSize.Width >= 0 && contentSize.Height >= 0, "contentSize.Width >= 0 && sizeContent.Height >= 0");
			if (elementsColumns.Count == 0) {
				overflowsHorizontal = false;
				overflowsVertical = false;
				return;
			}
			int num = 0;
			foreach (BetterListViewColumnHeader elementsColumn in elementsColumns) {
				num += ((IBetterListViewLayoutElementDisplayable)elementsColumn).LayoutBounds.BoundsOuter.Width;
				if (num > contentSize.Width) {
					overflowsHorizontal = true;
					break;
				}
			}
			overflowsVertical = ((IBetterListViewLayoutElementDisplayable)elementsColumns[0]).LayoutBounds.BoundsOuter.Height > contentSize.Height;
		}

		/// <summary>
		///   Measure the layout.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to measure</param>
		/// <param name="indexElementLast">index of the last layout element to measure</param>
		/// <param name="contentSize">content area size</param>
		/// <returns>layout measurement</returns>
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewColumnHeader> elements, int indexElementFirst, int indexElementLast, Size contentSize) {
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
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewColumnHeader> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool customPadding) {
			Checks.CheckNotNull(elements, "elements");
			if (elements.Count != 0) {
				Checks.CheckBounds(indexElementFirst, 0, elements.Count - 1, "indexElementFirst");
				Checks.CheckBounds(indexElementLast, indexElementFirst, elements.Count - 1, "indexElementLast");
			}
			else {
				Checks.CheckEqual(indexElementFirst, 0, "indexElementFirst", "0");
				Checks.CheckEqual(indexElementLast, 0, "indexElementLast", "0");
			}
			Checks.CheckSize(contentSize, "contentSize");
			int num = 0;
			for (int i = indexElementFirst; i <= indexElementLast; i++) {
				BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)elements[i]).LayoutBounds;
				num += betterListViewColumnHeaderBounds.BoundsOuter.Width;
			}
			int height = ((IBetterListViewLayoutElementDisplayable)elements[indexElementFirst]).LayoutBounds.BoundsOuter.Height;
			return new BetterListViewLayoutMeasurement(1, num, height);
		}

		internal BetterListViewCommonMeasurementColumns MeasureElements(ICollection<BetterListViewColumnHeader> elementsColumns, Graphics graphics, BetterListViewSortList sortList, ImageList imageList, bool zeroHeight) {
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(sortList, "sortList");
			if (elementsColumns.Count == 0) {
				return BetterListViewCommonMeasurementColumns.Empty;
			}
			Size sortGlyphSize = BetterListViewPainter.GetSortGlyphSize(graphics);
			Padding paddingSortGlyph = BetterListViewPainter.GetSortGlyphPadding(this.SortGlyphPadding);
			BetterListViewImageSize betterListViewImageSize = this.ImageSize;
			int num = betterListViewImageSize.MinimumSize.Height;
			int num2 = this.MinimumTextHeight;
			Dictionary<int, int> dictionary = new Dictionary<int, int>(elementsColumns.Count);
			foreach (BetterListViewColumnHeader elementsColumn in elementsColumns) {
				if (!elementsColumn.Visible) {
					continue;
				}
				int width = elementsColumn.Width;
				width -= base.ElementInnerPadding.Horizontal;
				if (!betterListViewImageSize.IsEmpty) {
					if (betterListViewImageSize.IsFixed) {
						width -= betterListViewImageSize.MinimumSize.Width + base.ImagePadding.Horizontal;
					}
					else {
						Image elementImage = BetterListViewBasePainter.GetElementImage(elementsColumn, imageList);
						betterListViewImageSize.GetImageSize(elementImage, out var sizeImageFrame);
						width -= sizeImageFrame.Width + base.ImagePadding.Horizontal;
						num = Math.Max(num, sizeImageFrame.Height);
					}
				}
				width -= base.TextPadding.Horizontal;
				if (sortList.Contains(elementsColumn.Index) && BetterListViewPainter.ColumnHeaderSortGlyphAlignment == BetterListViewPainterGlyphAlignment.Right) {
					width -= sortGlyphSize.Width + paddingSortGlyph.Horizontal;
				}
				width = Math.Max(width, 0);
				int lineCount = elementsColumn.LayoutGetLineCount(graphics, width, this.MaximumTextLines);
				num2 = Math.Max(num2, TextSize.HeightFromLineCount(graphics, elementsColumn.Font, lineCount));
				dictionary.Add(((IBetterListViewLayoutElementDisplayable)elementsColumn).LayoutIndexDisplay, width);
			}
			int heightOuterMinimum;
			int heightOuter;
			if (zeroHeight) {
				heightOuter = 0;
				heightOuterMinimum = 0;
			}
			else {
				heightOuter = 0;
				if (num > 0) {
					heightOuter = Math.Max(heightOuter, num + base.ImagePadding.Vertical);
				}
				if (num2 > 0) {
					heightOuter = Math.Max(heightOuter, num2 + base.TextPadding.Vertical);
				}
				heightOuter = ((BetterListViewPainter.ColumnHeaderSortGlyphAlignment != 0) ? Math.Max(heightOuter, sortGlyphSize.Height + paddingSortGlyph.Vertical) : (heightOuter + (sortGlyphSize.Height + paddingSortGlyph.Vertical)));
				heightOuter += base.ElementInnerPadding.Vertical;
				heightOuterMinimum = ((BetterListViewPainter.ColumnHeaderSortGlyphAlignment == BetterListViewPainterGlyphAlignment.Top) ? (base.ElementInnerPadding.Vertical + sortGlyphSize.Height) : base.ElementInnerPadding.Vertical);
			}
			return new BetterListViewCommonMeasurementColumns(sortGlyphSize, paddingSortGlyph, heightOuter, heightOuterMinimum, dictionary);
		}

		/// <summary>
		///   Measure column header layout elements.
		/// </summary>
		/// <param name="columnHeader">column header layout element</param>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <param name="imageList">ImageList for column headers</param>
		/// <param name="sortList">sort state</param>
		/// <param name="commonMeasurement">column header layout elements common measurement information</param>
		internal void MeasureElement(BetterListViewColumnHeader columnHeader, Graphics graphics, ImageList imageList, BetterListViewSortList sortList, BetterListViewCommonMeasurementColumns commonMeasurement) {
			Checks.CheckNotNull(columnHeader, "columnHeader");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(sortList, "sortList");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds;
			bool flag = sortList.Contains(columnHeader.Index);
			betterListViewColumnHeaderBounds.Reset();
			if (!columnHeader.Visible) {
				betterListViewColumnHeaderBounds.BoundsOuter = new Rectangle(0, 0, 0, commonMeasurement.HeightOuter);
				betterListViewColumnHeaderBounds.BoundsOuterExtended = betterListViewColumnHeaderBounds.BoundsOuter;
				betterListViewColumnHeaderBounds.BoundsInner = new Rectangle(0, 0, 0, commonMeasurement.HeightOuter);
				return;
			}
			betterListViewColumnHeaderBounds.BoundsOuter = new Rectangle(0, 0, columnHeader.Width, commonMeasurement.HeightOuter);
			betterListViewColumnHeaderBounds.BoundsOuterExtended = betterListViewColumnHeaderBounds.BoundsOuter;
			int num = this.BorderSize >> 1;
			int num2 = Math.Max(columnHeader.Width - num, 0);
			int num3 = columnHeader.Width - 1 + num;
			betterListViewColumnHeaderBounds.BoundsBorder = new Rectangle(num2, 0, num3 - num2 + 1, commonMeasurement.HeightOuter);
			if (betterListViewColumnHeaderBounds.BoundsOuter.Width > base.ElementInnerPadding.Horizontal && betterListViewColumnHeaderBounds.BoundsOuter.Height > commonMeasurement.HeightOuterMinimum) {
				if (flag) {
					switch (BetterListViewPainter.ColumnHeaderSortGlyphAlignment) {
						case BetterListViewPainterGlyphAlignment.Top:
							betterListViewColumnHeaderBounds.BoundsInner = new Rectangle(base.ElementInnerPadding.Left, commonMeasurement.SizeSortGlyph.Height + commonMeasurement.PaddingSortGlyph.Vertical + base.ElementInnerPadding.Top, betterListViewColumnHeaderBounds.BoundsOuter.Width - base.ElementInnerPadding.Horizontal, betterListViewColumnHeaderBounds.BoundsOuter.Height - commonMeasurement.SizeSortGlyph.Height - commonMeasurement.PaddingSortGlyph.Vertical - base.ElementInnerPadding.Vertical);
							break;
						case BetterListViewPainterGlyphAlignment.Right:
							betterListViewColumnHeaderBounds.BoundsInner = new Rectangle(base.ElementInnerPadding.Left, base.ElementInnerPadding.Top, betterListViewColumnHeaderBounds.BoundsOuter.Width - base.ElementInnerPadding.Horizontal - commonMeasurement.SizeSortGlyph.Width - commonMeasurement.PaddingSortGlyph.Horizontal, betterListViewColumnHeaderBounds.BoundsOuter.Height - base.ElementInnerPadding.Vertical);
							break;
						default:
							throw new ApplicationException($"Unknown column header sort glyph alignment: '{BetterListViewPainter.ColumnHeaderSortGlyphAlignment}'.");
					}
				}
				else {
					betterListViewColumnHeaderBounds.BoundsInner = new Rectangle(base.ElementInnerPadding.Left, base.ElementInnerPadding.Top, betterListViewColumnHeaderBounds.BoundsOuter.Width - base.ElementInnerPadding.Horizontal, betterListViewColumnHeaderBounds.BoundsOuter.Height - base.ElementInnerPadding.Vertical);
				}
				int num4 = betterListViewColumnHeaderBounds.BoundsInner.Left;
				int num5 = betterListViewColumnHeaderBounds.BoundsInner.Width;
				Image elementImage = BetterListViewBasePainter.GetElementImage(columnHeader, imageList);
				bool flag2;
				if (!this.ImageSize.IsEmpty && (!this.imageSize.MinimumSize.IsEmpty || elementImage != null) && num5 > base.ImagePadding.Horizontal) {
					BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((columnHeader.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : columnHeader.AlignHorizontalImage);
					BetterListViewImageAlignmentVertical alignmentVertical = ((columnHeader.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : columnHeader.AlignVerticalImage);
					this.ImageSize.GetImageBounds(elementImage, betterListViewImageAlignmentHorizontal, alignmentVertical, out var sizeImageFrame, out var boundsImage);
					flag2 = betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextRight;
					int x = ((!flag2) ? (num4 + base.ImagePadding.Left) : (num4 + base.ImagePadding.Left + Math.Max(num5 - sizeImageFrame.Width - base.ImagePadding.Horizontal, 0)));
					Rectangle boundsImageFrame = new Rectangle(x, betterListViewColumnHeaderBounds.BoundsInner.Top + (betterListViewColumnHeaderBounds.BoundsInner.Height - sizeImageFrame.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
					if (!boundsImage.IsEmpty) {
						boundsImage.Offset(boundsImageFrame.Location);
					}
					betterListViewColumnHeaderBounds.BoundsImageFrame = boundsImageFrame;
					betterListViewColumnHeaderBounds.BoundsImage = boundsImage;
					int num6 = boundsImageFrame.Width + base.ImagePadding.Horizontal;
					if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
						num4 += num6;
					}
					num5 -= num6;
				}
				else {
					flag2 = false;
					betterListViewColumnHeaderBounds.BoundsImageFrame = Rectangle.Empty;
					betterListViewColumnHeaderBounds.BoundsImage = Rectangle.Empty;
				}
				if (num5 > base.TextPadding.Horizontal && !string.IsNullOrEmpty(columnHeader.Text)) {
					int num7 = commonMeasurement.WidthsText[((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutIndexDisplay];
					TextSize textSize = columnHeader.LayoutGetTextSize(graphics, num7, this.MaximumTextLines);
					TextAlignmentHorizontal textAlignmentHorizontal = ((columnHeader.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : columnHeader.AlignHorizontal);
					TextAlignmentVertical textAlignmentVertical = ((columnHeader.AlignVertical == TextAlignmentVertical.Default) ? base.DefaultTextAlignmentVertical : columnHeader.AlignVertical);
					switch (textAlignmentHorizontal) {
						case TextAlignmentHorizontal.Center:
							num4 += (num7 - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
							break;
						case TextAlignmentHorizontal.Right:
							num4 = betterListViewColumnHeaderBounds.BoundsInner.Right - textSize.Width - base.TextPadding.Horizontal;
							if (flag2) {
								num4 -= betterListViewColumnHeaderBounds.BoundsImageFrame.Width + base.ImagePadding.Horizontal;
							}
							break;
					}
					switch (textAlignmentVertical) {
						case TextAlignmentVertical.Middle:
							betterListViewColumnHeaderBounds.BoundsText = new Rectangle(num4 + base.TextPadding.Left, betterListViewColumnHeaderBounds.BoundsInner.Top + (betterListViewColumnHeaderBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top, textSize.Width, textSize.Height);
							break;
						case TextAlignmentVertical.Bottom:
							betterListViewColumnHeaderBounds.BoundsText = new Rectangle(num4 + base.TextPadding.Left, betterListViewColumnHeaderBounds.BoundsInner.Bottom - base.TextPadding.Bottom - textSize.Height, textSize.Width, textSize.Height);
							break;
						default:
							betterListViewColumnHeaderBounds.BoundsText = new Rectangle(num4 + base.TextPadding.Left, betterListViewColumnHeaderBounds.BoundsInner.Top + base.TextPadding.Top, textSize.Width, textSize.Height);
							break;
					}
					betterListViewColumnHeaderBounds.IsTextShrunk = textSize.IsTextShrunk;
				}
				else {
					betterListViewColumnHeaderBounds.BoundsText = Rectangle.Empty;
					betterListViewColumnHeaderBounds.IsTextShrunk = false;
				}
			}
			if (flag) {
				switch (BetterListViewPainter.ColumnHeaderSortGlyphAlignment) {
					case BetterListViewPainterGlyphAlignment.Top:
						betterListViewColumnHeaderBounds.BoundsSortGlyph = new Rectangle((betterListViewColumnHeaderBounds.BoundsOuter.Width - commonMeasurement.SizeSortGlyph.Width - commonMeasurement.PaddingSortGlyph.Horizontal >> 1) + commonMeasurement.PaddingSortGlyph.Left, commonMeasurement.PaddingSortGlyph.Top, commonMeasurement.SizeSortGlyph.Width, commonMeasurement.SizeSortGlyph.Height);
						break;
					case BetterListViewPainterGlyphAlignment.Right:
						betterListViewColumnHeaderBounds.BoundsSortGlyph = new Rectangle(betterListViewColumnHeaderBounds.BoundsOuter.Width - commonMeasurement.SizeSortGlyph.Width - commonMeasurement.PaddingSortGlyph.Horizontal, (betterListViewColumnHeaderBounds.BoundsOuter.Height - commonMeasurement.SizeSortGlyph.Height - commonMeasurement.PaddingSortGlyph.Vertical >> 1) + commonMeasurement.PaddingSortGlyph.Top, commonMeasurement.SizeSortGlyph.Width, commonMeasurement.SizeSortGlyph.Height);
						break;
					default:
						throw new ApplicationException($"Unknown column header sort glyph alignment: '{BetterListViewPainter.ColumnHeaderSortGlyphAlignment}'.");
				}
			}
			else {
				betterListViewColumnHeaderBounds.BoundsSortGlyph = Rectangle.Empty;
			}
		}

		/// <summary>
		///   Get automatic width for the specified column header.
		/// </summary>
		/// <param name="columnHeader">column header to measure</param>
		/// <param name="graphics">graphics object used for measurement</param>
		/// <param name="imageList">images to be displayed on column headers</param>
		/// <param name="sortGlyphVisible">sort glyph is visible on the column header</param>
		/// <returns>column header automatic width</returns>
		internal int GetColumnHeaderAutoWidth(BetterListViewColumnHeader columnHeader, Graphics graphics, ImageList imageList, bool sortGlyphVisible) {
			Checks.CheckNotNull(columnHeader, "columnHeader");
			Checks.CheckNotNull(graphics, "graphics");
			int num = base.ElementInnerPadding.Horizontal;
			BetterListViewImageSize betterListViewImageSize = this.ImageSize;
			if (!betterListViewImageSize.IsEmpty) {
				Image elementImage = BetterListViewBasePainter.GetElementImage(columnHeader, imageList);
				betterListViewImageSize.GetImageSize(elementImage, out var sizeImageFrame);
				num += sizeImageFrame.Width + base.ImagePadding.Horizontal;
			}
			if (!string.IsNullOrEmpty(columnHeader.Text)) {
				num += columnHeader.LayoutGetTextSize(graphics, int.MaxValue, 1).Width;
				num += base.TextPadding.Horizontal;
			}
			if (sortGlyphVisible && BetterListViewPainter.ColumnHeaderSortGlyphAlignment == BetterListViewPainterGlyphAlignment.Right) {
				num += BetterListViewPainter.GetSortGlyphSize(graphics).Width;
				num += BetterListViewPainter.GetSortGlyphPadding(this.SortGlyphPadding).Horizontal;
			}
			return num;
		}

		internal void PositionElements(ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, int indexElementFirst, int indexElementLast) {
			Checks.CheckNotNull(elementsColumns, "elements");
			if (elementsColumns.Count != 0) {
				Checks.CheckBounds(indexElementFirst, 0, elementsColumns.Count - 1, "indexElementFirst");
				Checks.CheckTrue(indexElementLast >= indexElementFirst, "indexElementLast >= indexElementFirst");
			}
			else {
				Checks.CheckEqual(indexElementFirst, 0, "indexElementFirst", "0");
				Checks.CheckEqual(indexElementLast, 0, "indexElementLast", "0");
			}
			int num = base.LayoutPadding.Left;
			int top = base.LayoutPadding.Top;
			for (int i = indexElementFirst; i <= indexElementLast; i++) {
				BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)elementsColumns[i]).LayoutBounds;
				betterListViewColumnHeaderBounds.BoundsSpacing = betterListViewColumnHeaderBounds.BoundsOuter;
				betterListViewColumnHeaderBounds.Relocate(new Point(num, top));
				num += betterListViewColumnHeaderBounds.BoundsOuter.Width;
			}
		}

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected override void SetDefaultsInternal() {
			this.borderSize = 16;
			this.imageSize = this.DefaultImageSize;
			this.sortGlyphPadding = new Padding(0);
			this.minimumTextHeight = 16;
			this.maximumTextLines = 1;
			base.SetDefaultsInternal();
		}
	}
}