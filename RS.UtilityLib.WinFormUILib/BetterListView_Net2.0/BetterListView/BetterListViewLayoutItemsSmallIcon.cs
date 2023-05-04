using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Layout for SmallIcon view.
	/// </summary>
	internal sealed class BetterListViewLayoutItemsSmallIcon : BetterListViewLayoutItems
	{
		public override bool OrientationVertical => true;

		public override bool AllowsExpandableItems => false;

		public override bool DirectionVertical => false;

		public override bool SingleBoundary => false;

		internal override BetterListViewLayoutPositioningOptions PositioningOptions => BetterListViewLayoutPositioningOptions.None;

		internal override BetterListViewInvalidationInfo ResizeInvalidationInfo => new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.MeasureContent, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw, Rectangle.Empty);

		internal override bool CustomPadding => false;

		/// <summary>
		///   each row contains just a single item, even if there is enough space on the row for more items
		/// </summary>
		protected override bool SingleItemPerRow => false;

		/// <summary>
		///   default horizontal element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentHorizontal DefaultDefaultImageAlignmentHorizontal => BetterListViewImageAlignmentHorizontal.BeforeTextCenter;

		/// <summary>
		///   default vertical element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentVertical DefaultDefaultImageAlignmentVertical => BetterListViewImageAlignmentVertical.Middle;

		protected override TextAlignmentHorizontal DefaultDefaultTextAlignmentHorizontal => TextAlignmentHorizontal.Left;

		protected override TextAlignmentVertical DefaultDefaultTextAlignmentVertical => TextAlignmentVertical.Middle;

		protected override TextTrimming DefaultDefaultTextTrimming => TextTrimming.EllipsisCharacter;

		protected override Padding DefaultLayoutPadding => new Padding(2);

		protected override Size DefaultElementOuterPadding => new Size(1, 1);

		protected override Padding DefaultElementInnerPadding => new Padding(2);

		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(Size.Empty, new Size(16, 16));

		protected override Padding DefaultImagePadding => new Padding(4);

		protected override ImageBorderType DefaultImageBorderType => ImageBorderType.None;

		protected override int DefaultImageBorderThickness => 4;

		protected override Size DefaultEmptyTextSize => new Size(48, 16);

		protected override Padding DefaultTextPadding => new Padding(2);

		protected override bool DefaultCenter => false;

		protected override int DefaultItemWidth => 216;

		protected override int DefaultItemHeight => 0;

		protected override int DefaultMaximumTextLines => 1;

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsSmallIcon instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		internal BetterListViewLayoutItemsSmallIcon(BetterListView listView)
			: base(listView) {
		}

		internal override int GetSubItemCount(BetterListViewItem item, int columnCount) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(columnCount >= 0, "columnCount >= 0");
			return 1;
		}

		internal override BetterListViewCommonMeasurementItems MeasureElements(ICollection<BetterListViewItem> elementsItems, Graphics graphics, Size contentSize, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool enableExpandButtons, bool enableCheckBoxes, int indent) {
			Checks.CheckNotNull(elementsItems, "elementsItems");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			if (elementsItems.Count == 0 || contentSize.Width <= 0 || contentSize.Height <= 0) {
				return BetterListViewCommonMeasurementItems.Empty;
			}
			Size sizeCheckBoxCheck;
			Size sizeCheckBoxRadio;
			if (enableCheckBoxes) {
				sizeCheckBoxCheck = BetterListViewBasePainter.GetCheckBoxSize(graphics, BetterListViewCheckBoxAppearance.CheckBox);
				sizeCheckBoxRadio = BetterListViewBasePainter.GetCheckBoxSize(graphics, BetterListViewCheckBoxAppearance.RadioButton);
			}
			else {
				sizeCheckBoxCheck = Size.Empty;
				sizeCheckBoxRadio = Size.Empty;
			}
			int val = base.ItemHeight;
			if (enableCheckBoxes) {
				val = Math.Max(val, Math.Max(sizeCheckBoxCheck.Height, sizeCheckBoxRadio.Height) + base.CheckBoxPadding.Vertical);
			}
			BetterListViewImageSize imageSize = base.ImageSize;
			if (!imageSize.IsEmpty) {
				val = Math.Max(val, imageSize.MaximumSize.Height + base.ImagePadding.Vertical);
			}
			val = Math.Max(val, base.EmptyTextSize.Height + base.TextPadding.Vertical);
			return new BetterListViewCommonMeasurementItems(Size.Empty, sizeCheckBoxCheck, sizeCheckBoxRadio, 0, val);
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
			int itemWidth = base.ItemWidth;
			int num = commonMeasurement.HeightBase;
			if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
				int maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
				int value;
				if (maximumTextLines == 1) {
					Font font = betterListViewSubItem.Font;
					int hashCode = font.GetHashCode();
					if (!usedFontHeights.TryGetValue(hashCode, out value)) {
						value = TextSize.HeightFromLineCount(graphics, font, 1);
						usedFontHeights.Add(hashCode, value);
					}
				}
				else {
					int num2 = itemWidth;
					if (enableCheckBox) {
						num2 -= commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true).Width - base.CheckBoxPadding.Horizontal;
					}
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
						num2 -= sizeImageFrame.Width + base.ImagePadding.Horizontal;
					}
					value = betterListViewSubItem.LayoutGetTextSize(graphics, num2, maximumTextLines).Height;
				}
				num = Math.Max(num, value + base.TextPadding.Vertical);
			}
			betterListViewItemBounds.BoundsOuter = new Rectangle(0, 0, itemWidth + base.ElementInnerPadding.Horizontal, num + base.ElementInnerPadding.Vertical);
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
			BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
			BetterListViewSubItemBounds betterListViewSubItemBounds = betterListViewItemBounds.SubItemBounds[0];
			int num = boundsOuter.Left + base.ElementInnerPadding.Left;
			int num2 = base.ItemWidth;
			BetterListViewImageSize imageSize = base.ImageSize;
			BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : betterListViewSubItem.AlignHorizontalImage);
			BetterListViewImageAlignmentVertical alignmentVertical = ((betterListViewSubItem.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : betterListViewSubItem.AlignVerticalImage);
			Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
			Size sizeImageFrame;
			Rectangle boundsImage;
			if (!imageSize.IsEmpty && (!imageSize.MinimumSize.IsEmpty || elementImage != null)) {
				imageSize.GetImageBounds(elementImage, betterListViewImageAlignmentHorizontal, alignmentVertical, out sizeImageFrame, out boundsImage);
			}
			else {
				sizeImageFrame = Size.Empty;
				boundsImage = Rectangle.Empty;
			}
			int num3 = commonMeasurement.HeightBase;
			if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
				int num4 = num2;
				if (enableCheckBox) {
					num4 -= commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true).Width - base.CheckBoxPadding.Horizontal;
				}
				if (!imageSize.IsEmpty) {
					num4 -= sizeImageFrame.Width + base.ImagePadding.Horizontal;
				}
				num3 = Math.Max(num3, betterListViewSubItem.LayoutGetTextSize(graphics, num4, base.GetMaximumTextLines(betterListViewSubItem)).Height + base.TextPadding.Vertical);
			}
			if (enableCheckBox) {
				Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
				if (showCheckBox && num2 > base.CheckBoxPadding.Horizontal) {
					betterListViewItemBounds.BoundsCheckBox = new Rectangle(num + base.CheckBoxPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top + (num3 - checkBoxSize.Height - base.CheckBoxPadding.Vertical >> 1) + base.CheckBoxPadding.Top, checkBoxSize.Width, checkBoxSize.Height);
				}
				else {
					betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
				}
				int num5 = checkBoxSize.Width + base.CheckBoxPadding.Horizontal;
				num += num5;
				num2 -= num5;
			}
			else {
				betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
			}
			bool flag;
			if (!sizeImageFrame.IsEmpty && num2 > base.ImagePadding.Horizontal) {
				flag = betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextRight;
				int x = ((!flag) ? (num + base.ImagePadding.Left) : (num + num2 - sizeImageFrame.Width - base.ImagePadding.Horizontal));
				Rectangle boundsImageFrame = new Rectangle(x, boundsOuter.Top + base.ElementInnerPadding.Top + (num3 - sizeImageFrame.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
				if (!boundsImage.IsEmpty) {
					boundsImage.Offset(boundsImageFrame.Location);
				}
				betterListViewSubItemBounds.BoundsImageFrame = boundsImageFrame;
				betterListViewSubItemBounds.BoundsImage = boundsImage;
				int num6 = sizeImageFrame.Width + base.ImagePadding.Horizontal;
				if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
					num += num6;
				}
				num2 -= num6;
			}
			else {
				flag = false;
				betterListViewSubItemBounds.BoundsImageFrame = Rectangle.Empty;
				betterListViewSubItemBounds.BoundsImage = Rectangle.Empty;
			}
			if (num2 > base.TextPadding.Horizontal) {
				int maximumTextLines;
				TextSize textSize;
				if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
					maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
					textSize = betterListViewSubItem.LayoutGetTextSize(graphics, num2 - base.TextPadding.Horizontal, maximumTextLines);
				}
				else {
					maximumTextLines = 1;
					textSize = new TextSize(Math.Min(base.EmptyTextSize.Width, num2 - base.TextPadding.Horizontal), base.EmptyTextSize.Height, 1, isTextShrunk: false);
				}
				TextAlignmentHorizontal textAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : betterListViewSubItem.AlignHorizontal);
				TextAlignmentVertical textAlignmentVertical = ((betterListViewSubItem.AlignVertical == TextAlignmentVertical.Default) ? base.DefaultTextAlignmentVertical : betterListViewSubItem.AlignVertical);
				int x2;
				switch (textAlignmentHorizontal) {
					case TextAlignmentHorizontal.Center:
						x2 = num + (num2 - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
						break;
					case TextAlignmentHorizontal.Right:
						x2 = boundsOuter.Left + base.ElementInnerPadding.Left + base.ItemWidth - textSize.Width - base.TextPadding.Horizontal;
						if (flag) {
							x2 -= betterListViewSubItemBounds.BoundsImageFrame.Width + base.ImagePadding.Horizontal;
						}
						x2 = Math.Max(x2, num);
						x2 += base.TextPadding.Left;
						break;
					default:
						x2 = num + base.TextPadding.Left;
						break;
				}
				//betterListViewSubItemBounds.BoundsText = new Rectangle(x2, textAlignmentVertical switch {
				//	TextAlignmentVertical.Middle => boundsOuter.Top + base.ElementInnerPadding.Top + (num3 - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top,
				//	TextAlignmentVertical.Bottom => boundsOuter.Top + base.ElementInnerPadding.Top + num3 - base.TextPadding.Bottom - textSize.Height,
				//	_ => boundsOuter.Top + base.ElementInnerPadding.Top + base.TextPadding.Top,
				//}, textSize.Width, textSize.Height);
				{
					int y2;
					switch (textAlignmentVertical) { 
						case TextAlignmentVertical.Middle:
							y2 = boundsOuter.Top + base.ElementInnerPadding.Top + (num3 - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top;
                            break;
							case TextAlignmentVertical.Bottom:
							y2=boundsOuter.Top + base.ElementInnerPadding.Top + num3 - base.TextPadding.Bottom - textSize.Height;
                            break;
						default:
							y2 = boundsOuter.Top + base.ElementInnerPadding.Top + base.TextPadding.Top;
                            break;
					}
                    betterListViewSubItemBounds.BoundsText = new Rectangle(x2, y2, textSize.Width, textSize.Height);
                }

				betterListViewSubItemBounds.BoundsCell = new Rectangle(num, boundsOuter.Top + base.ElementInnerPadding.Top, num2, num3);
				betterListViewSubItemBounds.MaximumTextLines = maximumTextLines;
				betterListViewSubItemBounds.IsTextShrunk = textSize.IsTextShrunk;
			}
			else {
				betterListViewSubItemBounds.BoundsText = Rectangle.Empty;
				betterListViewSubItemBounds.BoundsCell = Rectangle.Empty;
				betterListViewSubItemBounds.MaximumTextLines = 1;
				betterListViewSubItemBounds.IsTextShrunk = false;
			}
			betterListViewSubItemBounds.BoundsInner = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top, base.ItemWidth, num3);
			betterListViewItemBounds.BoundsInner = betterListViewSubItemBounds.BoundsInner;
			betterListViewSubItemBounds.BoundsOuter = boundsOuter;
			betterListViewSubItemBounds.BoundsOuterExtended = boundsOuter;
			betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuter;
		}
	}
}