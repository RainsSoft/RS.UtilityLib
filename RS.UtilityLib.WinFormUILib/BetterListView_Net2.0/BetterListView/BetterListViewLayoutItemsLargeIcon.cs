using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Layout for LargeIcon view.
	/// </summary>
	internal sealed class BetterListViewLayoutItemsLargeIcon : BetterListViewLayoutItemsLargeIconBase
	{
		internal override BetterListViewLayoutPositioningOptions PositioningOptions {
			get {
				if (!base.Center) {
					return BetterListViewLayoutPositioningOptions.StretchSpacing;
				}
				return BetterListViewLayoutPositioningOptions.StretchSpacing | BetterListViewLayoutPositioningOptions.CenterRow;
			}
		}

		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(Size.Empty, new Size(256, 256));

		protected override Padding DefaultImagePadding => new Padding(4);

		protected override ImageBorderType DefaultImageBorderType => ImageBorderType.None;

		protected override int DefaultImageBorderThickness => 4;

		protected override bool DefaultCenter => false;

		protected override int DefaultItemWidth => 48;

		protected override int DefaultItemHeight => 0;

		protected override int DefaultMaximumTextLines => 4;

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsLargeIcon instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		public BetterListViewLayoutItemsLargeIcon(BetterListView listView)
			: base(listView) {
		}

		internal override void MeasureElementCoarse(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets, IDictionary<int, int> usedFontHeights, int resizedColumnIndex) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(((IBetterListViewLayoutElementDisplayable)item).LayoutBounds, "((IBetterListViewLayoutElementDisplayable)item).LayoutBounds");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			Checks.CheckTrue(resizedColumnIndex == -1 || resizedColumnIndex >= 0, "resizedColumnIndex == BetterListViewElementBase.IndexUndefined || resizedColumnIndex >= 0");
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
			betterListViewItemBounds.Reset();
			int widthBase = commonMeasurement.WidthBase;
			int num = 0;
			int num2 = (enableCheckBox ? commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true).Height : 0);
			BetterListViewImageSize imageSize = base.ImageSize;
			if (!imageSize.IsEmpty) {
				Size sizeImageFrame;
				if (imageSize.IsFixed) {
					sizeImageFrame = imageSize.MinimumSize;
				}
				else {
					Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
					imageSize.GetImageSize(elementImage, out sizeImageFrame);
				}
				num2 = Math.Max(num2, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			num += num2;
			if (widthBase > base.TextPadding.Horizontal) {
				if (!string.IsNullOrEmpty(item.Text)) {
					Font font = betterListViewSubItem.Font;
					int maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
					int lineCount = betterListViewSubItem.LayoutGetLineCount(graphics, widthBase - base.TextPadding.Horizontal, maximumTextLines, allowUseTextBreaks: false);
					int hashCode = font.GetHashCode();
					if (!usedFontHeights.TryGetValue(hashCode, out var value)) {
						value = TextSize.HeightFromLineCount(graphics, font, 1);
						usedFontHeights.Add(hashCode, value);
					}
					num2 = Math.Max(TextSize.HeightFromLineCount(value, lineCount), base.EmptyTextSize.Height);
				}
				else {
					num2 = base.EmptyTextSize.Height;
				}
			}
			else {
				num2 = 0;
			}
			num = Math.Max(num + num2 + base.TextPadding.Vertical, base.ItemHeight);
			betterListViewItemBounds.BoundsOuter = new Rectangle(0, 0, widthBase + base.ElementInnerPadding.Horizontal, num + base.ElementInnerPadding.Vertical);
			betterListViewItemBounds.BoundsOuterExtended = betterListViewItemBounds.BoundsOuter;
		}

		internal override void MeasureElementFine(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(((IBetterListViewLayoutElementDisplayable)item).LayoutBounds, "((IBetterListViewLayoutElementDisplayable)item).LayoutBounds");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			Rectangle boundsOuter = betterListViewItemBounds.BoundsOuter;
			int num = boundsOuter.Top + base.ElementInnerPadding.Top;
			int num2 = 0;
			int widthBase = commonMeasurement.WidthBase;
			int num3;
			if (enableCheckBox) {
				Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
				if (showCheckBox) {
					betterListViewItemBounds.BoundsCheckBox = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top, checkBoxSize.Width, checkBoxSize.Height);
				}
				else {
					betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
				}
				num3 = checkBoxSize.Height + base.CheckBoxPadding.Vertical;
			}
			else {
				betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
				num3 = 0;
			}
			BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
			BetterListViewSubItemBounds betterListViewSubItemBounds = betterListViewItemBounds.SubItemBounds[0];
			Image elementImage = BetterListViewBasePainter.GetElementImage(betterListViewSubItem, imageList);
			BetterListViewImageSize imageSize = base.ImageSize;
			if (!imageSize.IsEmpty && (!imageSize.MinimumSize.IsEmpty || elementImage != null) && widthBase > base.ImagePadding.Horizontal) {
				BetterListViewImageAlignmentHorizontal alignmentHorizontal = ((betterListViewSubItem.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : betterListViewSubItem.AlignHorizontalImage);
				BetterListViewImageAlignmentVertical alignmentVertical = ((betterListViewSubItem.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : betterListViewSubItem.AlignVerticalImage);
				imageSize.GetImageBounds(elementImage, alignmentHorizontal, alignmentVertical, out var sizeImageFrame, out var boundsImage);
				Rectangle boundsImageFrame = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left + (widthBase - sizeImageFrame.Width - base.ImagePadding.Horizontal >> 1) + base.ImagePadding.Left, num + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
				if (!boundsImage.IsEmpty) {
					boundsImage.Offset(boundsImageFrame.Location);
				}
				betterListViewSubItemBounds.BoundsImageFrame = boundsImageFrame;
				betterListViewSubItemBounds.BoundsImage = boundsImage;
				num3 = Math.Max(num3, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			else {
				betterListViewSubItemBounds.BoundsImageFrame = Rectangle.Empty;
				betterListViewSubItemBounds.BoundsImage = Rectangle.Empty;
			}
			num += num3;
			num2 += num3;
			if (widthBase > base.TextPadding.Horizontal) {
				int maximumTextLines;
				TextSize textSize;
				if (!string.IsNullOrEmpty(item.Text)) {
					maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
					textSize = betterListViewSubItem.LayoutGetTextSize(graphics, widthBase - base.TextPadding.Horizontal, maximumTextLines);
				}
				else {
					maximumTextLines = 1;
					textSize = new TextSize(widthBase - base.TextPadding.Horizontal, base.EmptyTextSize.Height, 1, isTextShrunk: false);
				}
				int num4;
				switch ((betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : betterListViewSubItem.AlignHorizontal) {
					case TextAlignmentHorizontal.Center:
						num4 = base.ElementInnerPadding.Left + (widthBase - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
						break;
					case TextAlignmentHorizontal.Right:
						num4 = Math.Max(base.ElementInnerPadding.Left + widthBase - textSize.Width - base.TextPadding.Horizontal, base.ElementInnerPadding.Left);
						num4 += base.TextPadding.Left;
						break;
					default:
						num4 = base.ElementInnerPadding.Left + base.TextPadding.Left;
						break;
				}
				betterListViewSubItemBounds.BoundsText = new Rectangle(boundsOuter.Left + num4, num + base.TextPadding.Top, textSize.Width, textSize.Height);
				betterListViewSubItemBounds.BoundsCell = betterListViewSubItemBounds.BoundsText;
				betterListViewSubItemBounds.MaximumTextLines = maximumTextLines;
				betterListViewSubItemBounds.IsTextShrunk = textSize.IsTextShrunk;
				num3 = betterListViewSubItemBounds.BoundsText.Height + base.TextPadding.Vertical;
				num += num3;
				num2 += num3;
			}
			else {
				betterListViewSubItemBounds.BoundsText = Rectangle.Empty;
				betterListViewSubItemBounds.BoundsCell = Rectangle.Empty;
				betterListViewSubItemBounds.MaximumTextLines = 1;
				betterListViewSubItemBounds.IsTextShrunk = false;
				num3 = base.TextPadding.Vertical;
				num += num3;
				num2 += num3;
			}
			num2 = Math.Max(num2, base.ItemHeight);
			betterListViewSubItemBounds.BoundsInner = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top, widthBase, num2);
			betterListViewItemBounds.BoundsInner = betterListViewSubItemBounds.BoundsInner;
			betterListViewSubItemBounds.BoundsOuter = boundsOuter;
			betterListViewSubItemBounds.BoundsOuterExtended = boundsOuter;
			betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuter;
		}
	}
}