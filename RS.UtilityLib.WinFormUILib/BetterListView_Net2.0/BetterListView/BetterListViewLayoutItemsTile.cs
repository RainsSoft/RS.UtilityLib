using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Layout for Details view.
	/// </summary>
	internal sealed class BetterListViewLayoutItemsTile : BetterListViewLayoutItems
	{
		internal const int DefaultTileWidth = 216;

		internal const int DefaultTileHeight = 48;

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

		protected override Padding DefaultLayoutPadding => new Padding(4);

		protected override Size DefaultElementOuterPadding => new Size(4, 4);

		protected override Padding DefaultElementInnerPadding => new Padding(2);

		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(Size.Empty, new Size(256, 256));

		protected override Padding DefaultImagePadding => new Padding(4);

		protected override ImageBorderType DefaultImageBorderType => ImageBorderType.None;

		protected override int DefaultImageBorderThickness => 4;

		protected override Size DefaultEmptyTextSize => new Size(48, 16);

		protected override Padding DefaultTextPadding => new Padding(2);

		protected override bool DefaultCenter => false;

		protected override int DefaultItemWidth => 216;

		protected override int DefaultItemHeight => 48;

		protected override int DefaultMaximumTextLines => 1;

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsTile instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		public BetterListViewLayoutItemsTile(BetterListView listView)
			: base(listView) {
		}

		internal override int GetSubItemCount(BetterListViewItem item, int columnCount) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(columnCount >= 0, "columnCount >= 0");
			return item.SubItems.Count;
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
			return new BetterListViewCommonMeasurementItems(Size.Empty, sizeCheckBoxCheck, sizeCheckBoxRadio, 1, 1);
		}

		internal override void MeasureElementCoarse(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets, IDictionary<int, int> usedFontHeights, int resizedColumnIndex) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(((IBetterListViewLayoutElementDisplayable)item).LayoutBounds, "((IBetterListViewLayoutElementDisplayable)item).LayoutBounds");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			Checks.CheckTrue(resizedColumnIndex == -1 || resizedColumnIndex >= 0, "resizedColumnIndex == BetterListViewElementBase.IndexUndefined || resizedColumnIndex >= 0");
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			betterListViewItemBounds.Reset();
			Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
			int num = 0;
			int num2 = 0;
			if (enableCheckBox) {
				num = checkBoxSize.Width + base.CheckBoxPadding.Horizontal;
			}
			BetterListViewImageSize imageSize = base.ImageSize;
			Size sizeImageFrame;
			if (!imageSize.IsEmpty) {
				Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
				imageSize.GetImageSize(elementImage, out sizeImageFrame);
			}
			else {
				sizeImageFrame = Size.Empty;
			}
			int num3 = 0;
			int num4 = Math.Min(betterListViewItemBounds.SubItemBounds.Count, item.SubItems.Count);
			for (int i = 0; i < num4; i++) {
				BetterListViewSubItem betterListViewSubItem = item.SubItems[i];
				if (i == 0) {
					if (!imageSize.IsEmpty) {
						num = Math.Max(num, sizeImageFrame.Width + base.ImagePadding.Horizontal);
					}
					num3 = base.ItemWidth - num;
				}
				int num5;
				if (num3 > base.TextPadding.Horizontal) {
					Font font = betterListViewSubItem.Font;
					int hashCode = font.GetHashCode();
					if (!usedFontHeights.TryGetValue(hashCode, out var value)) {
						value = TextSize.HeightFromLineCount(graphics, font, 1);
						usedFontHeights.Add(hashCode, value);
					}
					int lineCount = betterListViewSubItem.LayoutGetLineCount(graphics, num3 - base.TextPadding.Horizontal, base.GetMaximumTextLines(betterListViewSubItem), allowUseTextBreaks: false);
					num5 = Math.Max(TextSize.HeightFromLineCount(value, lineCount), base.EmptyTextSize.Height);
					num5 += base.TextPadding.Vertical;
				}
				else {
					num5 = 0;
				}
				num2 += num5;
			}
			int val = 0;
			if (showCheckBox) {
				val = Math.Max(val, checkBoxSize.Height + base.CheckBoxPadding.Vertical);
			}
			if (!imageSize.IsEmpty) {
				val = Math.Max(val, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			val = Math.Max(val, num2);
			val = Math.Max(val, base.ItemHeight);
			betterListViewItemBounds.BoundsOuter = new Rectangle(0, 0, base.ItemWidth + base.ElementInnerPadding.Horizontal, val + base.ElementInnerPadding.Vertical);
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
			Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
			int num = boundsOuter.Left + base.ElementInnerPadding.Left;
			int num2 = boundsOuter.Top + base.ElementInnerPadding.Top;
			int num3 = 0;
			int num4 = 0;
			int x = 0;
			if (enableCheckBox) {
				if (showCheckBox) {
					betterListViewItemBounds.BoundsCheckBox = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left + base.CheckBoxPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top + base.CheckBoxPadding.Top, checkBoxSize.Width, checkBoxSize.Height);
				}
				else {
					betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
				}
				int num5 = checkBoxSize.Width + base.CheckBoxPadding.Horizontal;
				num3 += num5;
				num += num5;
			}
			else {
				betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
			}
			BetterListViewImageSize imageSize = base.ImageSize;
			BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((item.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : item.AlignHorizontalImage);
			BetterListViewImageAlignmentVertical betterListViewImageAlignmentVertical = ((item.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : item.AlignVerticalImage);
			int num7 = 0;
			Size sizeImageFrame = Size.Empty;
			Rectangle boundsImage = Rectangle.Empty;
			int num8 = Math.Min(betterListViewItemBounds.SubItemBounds.Count, item.SubItems.Count);
			for (int i = 0; i < num8; i++) {
				BetterListViewSubItem betterListViewSubItem = item.SubItems[i];
				BetterListViewSubItemBounds betterListViewSubItemBounds = betterListViewItemBounds.SubItemBounds[i];
				betterListViewSubItemBounds.BoundsImage = Rectangle.Empty;
				if (i == 0) {
					Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
					if (!imageSize.IsEmpty && (!imageSize.MinimumSize.IsEmpty || elementImage != null) && base.ItemWidth > base.ImagePadding.Horizontal) {
						imageSize.GetImageBounds(elementImage, betterListViewImageAlignmentHorizontal, betterListViewImageAlignmentVertical, out sizeImageFrame, out boundsImage);
						int num6 = sizeImageFrame.Width + base.ImagePadding.Horizontal;
						if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
							x = num + base.ImagePadding.Left;
							num += num6;
						}
						else {
							x = boundsOuter.Right - base.ElementInnerPadding.Right - num6;
						}
						num3 += num6;
					}
					num7 = base.ItemWidth - num3;
				}
				int num9;
				if (num7 > base.TextPadding.Horizontal) {
					int maximumTextLines;
					TextSize textSize;
					if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
						maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
						textSize = betterListViewSubItem.LayoutGetTextSize(graphics, num7 - base.TextPadding.Horizontal, maximumTextLines);
					}
					else {
						maximumTextLines = 1;
						textSize = new TextSize(Math.Min(base.EmptyTextSize.Width, num7 - base.TextPadding.Horizontal), base.EmptyTextSize.Height, 1, isTextShrunk: false);
					}
					//int x2 = ((betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : betterListViewSubItem.AlignHorizontal) switch {
					//	TextAlignmentHorizontal.Center => num + (num7 - textSize.Width >> 1),
					//	TextAlignmentHorizontal.Right => Math.Max(num + num7 - textSize.Width - base.TextPadding.Right, num),
					//	_ => num + base.TextPadding.Left,
					//};
					int x2;
					if (betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) {
						switch (base.DefaultTextAlignmentHorizontal) { 
						  case TextAlignmentHorizontal.Center:
								x2 = num + (num7 - textSize.Width >> 1);
								break;
							case TextAlignmentHorizontal.Right:
								x2 = Math.Max(num + num7 - textSize.Width - base.TextPadding.Right, num);
                                break;
								default: 
								x2 = num + base.TextPadding.Left; 
								break;
						}

                    }
					else {
                        switch (betterListViewSubItem.AlignHorizontal) {
                            case TextAlignmentHorizontal.Center:
                                x2 = num + (num7 - textSize.Width >> 1);
                                break;
                            case TextAlignmentHorizontal.Right:
                                x2 = Math.Max(num + num7 - textSize.Width - base.TextPadding.Right, num);
                                break;
                            default:
                                x2 = num + base.TextPadding.Left;
                                break;
                        }
                    }

                    betterListViewSubItemBounds.BoundsText = new Rectangle(x2, num2 + base.TextPadding.Top, textSize.Width, textSize.Height);
					betterListViewSubItemBounds.BoundsCell = new Rectangle(x2, num2 + base.TextPadding.Top, num7, textSize.Height);
					betterListViewSubItemBounds.MaximumTextLines = maximumTextLines;
					betterListViewSubItemBounds.IsTextShrunk = textSize.IsTextShrunk;
					num9 = textSize.Height + base.TextPadding.Vertical;
				}
				else {
					betterListViewSubItemBounds.BoundsText = Rectangle.Empty;
					betterListViewSubItemBounds.BoundsCell = Rectangle.Empty;
					betterListViewSubItemBounds.MaximumTextLines = 1;
					betterListViewSubItemBounds.IsTextShrunk = false;
					num9 = 0;
				}
				num2 += num9;
				num4 += num9;
			}
			int val = 0;
			if (showCheckBox) {
				val = Math.Max(val, checkBoxSize.Height + base.CheckBoxPadding.Vertical);
			}
			if (!imageSize.IsEmpty) {
				val = Math.Max(val, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			val = Math.Max(val, num4);
			val = Math.Max(val, base.ItemHeight);
			if (showCheckBox) {
				betterListViewItemBounds.BoundsCheckBox = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left + base.CheckBoxPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top + (val - checkBoxSize.Height >> 1), checkBoxSize.Width, checkBoxSize.Height);
			}
			BetterListViewSubItemBounds betterListViewSubItemBounds2 = betterListViewItemBounds.SubItemBounds[0];
			if (!imageSize.IsEmpty) {
				//Rectangle boundsImageFrame = new Rectangle(x, betterListViewImageAlignmentVertical switch {
				//	BetterListViewImageAlignmentVertical.Top => boundsOuter.Top + base.ElementInnerPadding.Top + base.ImagePadding.Top,
				//	BetterListViewImageAlignmentVertical.Bottom => boundsOuter.Bottom - base.ElementInnerPadding.Bottom - sizeImageFrame.Height - base.ImagePadding.Bottom,
				//	_ => boundsOuter.Top + base.ElementInnerPadding.Top + (val - sizeImageFrame.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top,
				//}, sizeImageFrame.Width, sizeImageFrame.Height);
				Rectangle boundsImageFrame;
				switch (betterListViewImageAlignmentVertical){
					case BetterListViewImageAlignmentVertical.Top:
						boundsImageFrame = new Rectangle(x, boundsOuter.Top + base.ElementInnerPadding.Top + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
						break;
					case BetterListViewImageAlignmentVertical.Bottom:
                        boundsImageFrame = new Rectangle(x, boundsOuter.Bottom - base.ElementInnerPadding.Bottom - sizeImageFrame.Height - base.ImagePadding.Bottom, sizeImageFrame.Width, sizeImageFrame.Height);
                        break;
					default:
                        boundsImageFrame = new Rectangle(x, boundsOuter.Top + base.ElementInnerPadding.Top + (val - sizeImageFrame.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
                        break;
				}

                if (!boundsImage.IsEmpty) {
					boundsImage.Offset(boundsImageFrame.Location);
				}
				betterListViewSubItemBounds2.BoundsImageFrame = boundsImageFrame;
				betterListViewSubItemBounds2.BoundsImage = boundsImage;
			}
			else {
				betterListViewSubItemBounds2.BoundsImageFrame = Rectangle.Empty;
				betterListViewSubItemBounds2.BoundsImage = Rectangle.Empty;
			}
			int num10 = val - num4 >> 1;
			for (int j = 0; j < betterListViewItemBounds.SubItemBounds.Count; j++) {
				BetterListViewSubItemBounds betterListViewSubItemBounds3 = betterListViewItemBounds.SubItemBounds[j];
				int left = betterListViewSubItemBounds3.BoundsText.Left;
				int y = betterListViewSubItemBounds3.BoundsText.Top + num10;
				int width = betterListViewSubItemBounds3.BoundsText.Width;
				Rectangle rectangle3 = betterListViewSubItemBounds3.BoundsText;
				betterListViewSubItemBounds3.BoundsText = new Rectangle(left, y, width, rectangle3.Height);
				rectangle3 = (betterListViewSubItemBounds3.BoundsCell = new Rectangle(betterListViewSubItemBounds3.BoundsCell.Left, betterListViewSubItemBounds3.BoundsCell.Top + num10, betterListViewSubItemBounds3.BoundsCell.Width, betterListViewSubItemBounds3.BoundsCell.Height));
				Rectangle rectangle2 = rectangle3;
				if (j != 0) {
					betterListViewSubItemBounds3.BoundsInner = rectangle2;
					betterListViewSubItemBounds3.BoundsOuter = rectangle2;
					betterListViewSubItemBounds3.BoundsOuterExtended = rectangle2;
				}
			}
			betterListViewItemBounds.BoundsInner = new Rectangle(boundsOuter.Left + base.ElementInnerPadding.Left, boundsOuter.Top + base.ElementInnerPadding.Top, base.ItemWidth, val);
			betterListViewItemBounds.SubItemBounds[0].BoundsInner = betterListViewItemBounds.BoundsInner;
			betterListViewItemBounds.SubItemBounds[0].BoundsOuter = boundsOuter;
			betterListViewItemBounds.SubItemBounds[0].BoundsOuterExtended = boundsOuter;
			betterListViewItemBounds.BoundsSelection = boundsOuter;
		}
	}
}