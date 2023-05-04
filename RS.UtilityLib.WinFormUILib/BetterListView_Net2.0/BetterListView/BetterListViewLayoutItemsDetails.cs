using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Layout for Details view.
	/// </summary>
	internal sealed class BetterListViewLayoutItemsDetails : BetterListViewLayoutItemsDetailsBase
	{
		internal override BetterListViewInvalidationInfo ResizeInvalidationInfo {
			get {
				if (!((BetterListView)base.ItemsView).AutoSizeItemsInDetailsView) {
					return new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.MeasureContent, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw, Rectangle.Empty);
				}
				return new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw, Rectangle.Empty);
			}
		}

		internal override bool CustomPadding => false;

		protected override Padding DefaultLayoutPadding => new Padding(2);

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsDetails instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		public BetterListViewLayoutItemsDetails(BetterListView listView)
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
			bool autoSizeItemsInDetailsView = ((BetterListView)base.ItemsView).AutoSizeItemsInDetailsView;
			if (elementsItems.Count == 0 || (autoSizeItemsInDetailsView && (contentSize.Width <= 0 || contentSize.Height <= 0))) {
				return BetterListViewCommonMeasurementItems.Empty;
			}
			Size sizeExpandButton = (enableExpandButtons ? BetterListViewBasePainter.GetNodeGlyphSize(graphics) : Size.Empty);
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
			int widthBase = (autoSizeItemsInDetailsView ? contentSize.Width : base.ItemWidth);
			int num = base.ItemHeight;
			if (enableExpandButtons) {
				num = Math.Max(num, sizeExpandButton.Height + base.ExpandButtonPadding.Vertical);
			}
			if (enableCheckBoxes) {
				num = Math.Max(num, Math.Max(sizeCheckBoxCheck.Height, sizeCheckBoxRadio.Height) + base.CheckBoxPadding.Vertical);
			}
			int num2 = 1;
			foreach (BetterListViewItem elementsItem in elementsItems) {
				num2 = Math.Max(num2, elementsItem.Level + 1);
			}
			int[] array = new int[num2];
			int levelOffset = base.GetLevelOffset(graphics, indent);
			for (int i = 0; i < array.Length; i++) {
				array[i] = base.GetChildItemOffset(i, levelOffset);
			}
			return new BetterListViewCommonMeasurementItems(sizeExpandButton, sizeCheckBoxCheck, sizeCheckBoxRadio, widthBase, num);
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
			int num = itemOffsets[item.Level];
			this.GetBaseTextHeight(graphics, imageList, item, num, enableExpandButton, enableCheckBox, extraPadding, commonMeasurement, usedFontHeights, out var heightBase, out var _);
			BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(0);
			if (!subItemImageSize.IsEmpty) {
				Size sizeImageFrame;
				if (subItemImageSize.IsFixed) {
					sizeImageFrame = subItemImageSize.MinimumSize;
				}
				else {
					Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
					subItemImageSize.GetImageSize(elementImage, out sizeImageFrame);
				}
				heightBase = Math.Max(heightBase, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			if (commonMeasurement.WidthBase > base.LayoutPadding.Horizontal + num + extraPadding.Width) {
				betterListViewItemBounds.BoundsOuter = new Rectangle(base.LayoutPadding.Left + num + extraPadding.Width, 0, commonMeasurement.WidthBase - base.LayoutPadding.Horizontal - num - extraPadding.Width, Math.Max(heightBase, item.CustomHeight) + base.ElementInnerPadding.Vertical);
			}
			else {
				betterListViewItemBounds.BoundsOuter = new Rectangle(base.LayoutPadding.Left + num + extraPadding.Width, 0, 0, Math.Max(heightBase, item.CustomHeight) + heightBase + base.ElementInnerPadding.Vertical);
			}
			betterListViewItemBounds.BoundsOuterExtended = betterListViewItemBounds.BoundsOuter;
		}

		internal override void MeasureElementFine(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(((IBetterListViewLayoutElementDisplayable)item).LayoutBounds, "((IBetterListViewLayoutElementDisplayable)item).LayoutBounds");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			if (betterListViewItemBounds.BoundsOuter.Width == 0) {
				return;
			}
			int offsetItem = itemOffsets[item.Level];
			this.GetBaseTextHeight(graphics, imageList, item, offsetItem, enableExpandButton, enableCheckBox, extraPadding, commonMeasurement, null, out var heightBase, out var widthText);
			Image elementImage = BetterListViewBasePainter.GetElementImage(item, imageList);
			BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(0);
			if (!subItemImageSize.IsEmpty) {
				Size sizeImageFrame;
				if (subItemImageSize.IsFixed) {
					sizeImageFrame = subItemImageSize.MinimumSize;
				}
				else {
					subItemImageSize.GetImageSize(elementImage, out sizeImageFrame);
				}
				heightBase = Math.Max(heightBase, sizeImageFrame.Height + base.ImagePadding.Vertical);
			}
			if (betterListViewItemBounds.BoundsOuter.Width <= base.ElementInnerPadding.Horizontal || betterListViewItemBounds.BoundsOuter.Height <= base.ElementInnerPadding.Vertical) {
				return;
			}
			betterListViewItemBounds.BoundsInner = new Rectangle(betterListViewItemBounds.BoundsOuter.Left + base.ElementInnerPadding.Left, betterListViewItemBounds.BoundsOuter.Top + base.ElementInnerPadding.Top, betterListViewItemBounds.BoundsOuter.Width - base.ElementInnerPadding.Horizontal, Math.Max(heightBase, item.CustomHeight));
			int num = betterListViewItemBounds.BoundsInner.Left;
			int num2 = betterListViewItemBounds.BoundsInner.Width;
			if (enableCheckBox) {
				Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
				if (showCheckBox && num2 > base.CheckBoxPadding.Horizontal) {
					betterListViewItemBounds.BoundsCheckBox = new Rectangle(num + base.CheckBoxPadding.Left, betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - checkBoxSize.Height - base.CheckBoxPadding.Vertical >> 1) + base.CheckBoxPadding.Top, checkBoxSize.Width, checkBoxSize.Height);
				}
				else {
					betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
				}
				int num3 = checkBoxSize.Width + base.CheckBoxPadding.Horizontal;
				num += num3;
				num2 -= num3;
			}
			else {
				betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
			}
			BetterListViewItem betterListViewItem = item.SelectableItem ?? item;
			int left = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuter.Left;
			if (item.AllowSelectChildItems) {
				betterListViewItemBounds.BoundsOuterExtended = new Rectangle(left, betterListViewItemBounds.BoundsOuter.Top, betterListViewItemBounds.BoundsOuter.Right - left, betterListViewItemBounds.BoundsOuter.Height);
				betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuter;
			}
			else {
				BetterListViewItem lastAvailableChildItem = item.LastAvailableChildItem;
				if (lastAvailableChildItem != null) {
					BetterListViewElementBoundsBase layoutBounds = ((IBetterListViewLayoutElementDisplayable)lastAvailableChildItem).LayoutBounds;
					int left2 = betterListViewItemBounds.BoundsSpacing.Left;
					int top = betterListViewItemBounds.BoundsSpacing.Top;
					int right = layoutBounds.BoundsSpacing.Right;
					int bottom = layoutBounds.BoundsSpacing.Bottom;
					betterListViewItemBounds.BoundsSpacing = new Rectangle(left2, top, right - left2, bottom - top);
					int top2 = betterListViewItemBounds.BoundsOuter.Top;
					int right2 = layoutBounds.BoundsOuter.Right;
					int bottom2 = layoutBounds.BoundsOuter.Bottom;
					betterListViewItemBounds.BoundsOuterExtended = new Rectangle(left, top2, right2 - left, bottom2 - top2);
					betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuterExtended;
				}
				else {
					betterListViewItemBounds.BoundsOuterExtended = betterListViewItemBounds.BoundsOuter;
					betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuter;
				}
			}
			BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
			BetterListViewSubItemBounds betterListViewSubItemBounds = betterListViewItemBounds.SubItemBounds[0];
			betterListViewSubItemBounds.BoundsOuter = betterListViewItemBounds.BoundsOuter;
			betterListViewSubItemBounds.BoundsOuterExtended = betterListViewItemBounds.BoundsOuter;
			betterListViewSubItemBounds.BoundsInner = betterListViewItemBounds.BoundsInner;
			bool flag;
			if (!subItemImageSize.IsEmpty && (!subItemImageSize.MinimumSize.IsEmpty || elementImage != null) && num2 > base.ImagePadding.Horizontal) {
				BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : betterListViewSubItem.AlignHorizontalImage);
				BetterListViewImageAlignmentVertical alignmentVertical = ((betterListViewSubItem.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : betterListViewSubItem.AlignVerticalImage);
				subItemImageSize.GetImageBounds(elementImage, betterListViewImageAlignmentHorizontal, alignmentVertical, out var sizeImageFrame2, out var boundsImage);
				flag = betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextRight;
				int x = ((!flag) ? (num + base.ImagePadding.Left) : (num + base.ImagePadding.Left + Math.Max(num2 - sizeImageFrame2.Width - base.ImagePadding.Horizontal, 0)));
				Rectangle boundsImageFrame = new Rectangle(x, betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - sizeImageFrame2.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame2.Width, sizeImageFrame2.Height);
				if (!boundsImage.IsEmpty) {
					boundsImage.Offset(boundsImageFrame.Location);
				}
				betterListViewSubItemBounds.BoundsImageFrame = boundsImageFrame;
				betterListViewSubItemBounds.BoundsImage = boundsImage;
				int num4 = sizeImageFrame2.Width + base.ImagePadding.Horizontal;
				if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
					num += num4;
				}
				num2 -= num4;
			}
			else {
				flag = false;
				betterListViewSubItemBounds.BoundsImageFrame = Rectangle.Empty;
				betterListViewSubItemBounds.BoundsImage = Rectangle.Empty;
			}
			if (num2 > 0) {
				betterListViewSubItemBounds.BoundsCell = new Rectangle(num, betterListViewSubItemBounds.BoundsInner.Top, widthText, betterListViewSubItemBounds.BoundsInner.Height);
			}
			else {
				betterListViewSubItemBounds.BoundsCell = Rectangle.Empty;
			}
			if (num2 > base.TextPadding.Horizontal) {
				int maximumTextLines;
				TextSize textSize;
				if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
					maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
					textSize = betterListViewSubItem.LayoutGetTextSize(graphics, widthText, maximumTextLines);
				}
				else {
					maximumTextLines = 1;
					textSize = new TextSize(Math.Min(base.EmptyTextSize.Width, widthText), base.EmptyTextSize.Height, 1, isTextShrunk: false);
				}
				TextAlignmentHorizontal textAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : betterListViewSubItem.AlignHorizontal);
				TextAlignmentVertical textAlignmentVertical = ((betterListViewSubItem.AlignVertical == TextAlignmentVertical.Default) ? base.DefaultTextAlignmentVertical : betterListViewSubItem.AlignVertical);
				int x2;
				switch (textAlignmentHorizontal) {
					case TextAlignmentHorizontal.Center:
						x2 = num + (num2 - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
						break;
					case TextAlignmentHorizontal.Right:
						x2 = betterListViewItemBounds.BoundsInner.Right - textSize.Width - base.TextPadding.Horizontal;
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
				//	TextAlignmentVertical.Middle => betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top,
				//	TextAlignmentVertical.Bottom => betterListViewItemBounds.BoundsInner.Bottom - base.TextPadding.Bottom - textSize.Height,
				//	_ => betterListViewItemBounds.BoundsInner.Top + base.TextPadding.Top,
				//}, textSize.Width, textSize.Height);
				{
					int y2;
					switch (textAlignmentVertical) {
						case TextAlignmentVertical.Middle:
							y2 = betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top;
                            break;
							case TextAlignmentVertical.Bottom:
							y2 = betterListViewItemBounds.BoundsInner.Bottom - base.TextPadding.Bottom - textSize.Height;
                            break;
						default:
							y2 =betterListViewItemBounds.BoundsInner.Top + base.TextPadding.Top;
							break;
					}
                    betterListViewSubItemBounds.BoundsText = new Rectangle(x2, y2, textSize.Width, textSize.Height);
                }

				betterListViewSubItemBounds.MaximumTextLines = maximumTextLines;
				betterListViewSubItemBounds.IsTextShrunk = textSize.IsTextShrunk;
			}
			else {
				betterListViewSubItemBounds.BoundsText = Rectangle.Empty;
				betterListViewSubItemBounds.MaximumTextLines = 1;
				betterListViewSubItemBounds.IsTextShrunk = false;
			}
		}

		private void GetBaseTextHeight(Graphics graphics, ImageList imageList, BetterListViewItem item, int offsetItem, bool enableExpandButton, bool enableCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, IDictionary<int, int> usedFontHeights, out int heightBase, out int widthText) {
			heightBase = commonMeasurement.HeightBase;
			BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
			Font font = betterListViewSubItem.Font;
			int hashCode = font.GetHashCode();
			int value;
			if (usedFontHeights != null) {
				if (!usedFontHeights.TryGetValue(hashCode, out value)) {
					value = TextSize.HeightFromLineCount(graphics, betterListViewSubItem.Font, 1);
					usedFontHeights.Add(hashCode, value);
				}
			}
			else {
				value = TextSize.HeightFromLineCount(graphics, betterListViewSubItem.Font, 1);
			}
			widthText = commonMeasurement.WidthBase - base.LayoutPadding.Horizontal - offsetItem - extraPadding.Width - base.ElementInnerPadding.Horizontal;
			if (enableExpandButton) {
				widthText -= commonMeasurement.SizeExpandButton.Width + base.ExpandButtonPadding.Horizontal;
			}
			if (enableCheckBox) {
				widthText -= commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true).Width + base.CheckBoxPadding.Horizontal;
			}
			BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(0);
			if (!subItemImageSize.IsEmpty) {
				if (subItemImageSize.IsFixed) {
					widthText -= subItemImageSize.MinimumSize.Width + base.ImagePadding.Horizontal;
				}
				else {
					Image elementImage = BetterListViewBasePainter.GetElementImage(betterListViewSubItem, imageList);
					subItemImageSize.GetImageSize(elementImage, out var sizeImageFrame);
					widthText -= sizeImageFrame.Width + base.ImagePadding.Horizontal;
				}
			}
			widthText = Math.Max(widthText - base.TextPadding.Horizontal, 0);
			if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
				int lineCount = betterListViewSubItem.LayoutGetLineCount(graphics, widthText, base.GetMaximumTextLines(betterListViewSubItem), allowUseTextBreaks: true);
				heightBase = Math.Max(heightBase, TextSize.HeightFromLineCount(value, lineCount) + base.TextPadding.Vertical);
			}
			heightBase = Math.Max(heightBase, base.EmptyTextSize.Height + base.TextPadding.Vertical);
		}
	}
}