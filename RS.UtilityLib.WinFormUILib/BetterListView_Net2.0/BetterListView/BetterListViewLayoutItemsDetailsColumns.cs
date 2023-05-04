using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Layout for Details view.
	/// </summary>
	internal sealed class BetterListViewLayoutItemsDetailsColumns : BetterListViewLayoutItemsDetailsBase
	{
		internal override BetterListViewInvalidationInfo ResizeInvalidationInfo => new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, Rectangle.Empty);

		internal override bool CustomPadding => true;

		protected override Padding DefaultLayoutPadding => new Padding(2, 2, 0, 2);

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsDetailsColumns instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		public BetterListViewLayoutItemsDetailsColumns(BetterListView listView)
			: base(listView) {
		}

		internal override int GetSubItemCount(BetterListViewItem item, int columnCount) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(columnCount >= 0, "columnCount >= 0");
			return Math.Min(item.SubItems.Count, columnCount);
		}

		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewItem> elements, int indexElementFirst, int indexElementLast, Size contentSize) {
			return this.MeasureLayout(elements, indexElementFirst, indexElementLast, contentSize, customPadding: false);
		}

		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewItem> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool customPadding) {
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
			if (elements.Count == 0) {
				return BetterListViewLayoutMeasurement.Empty;
			}
			int width = ((IBetterListViewLayoutElementDisplayable)elements[0]).LayoutBounds.BoundsOuter.Width;
			int num = 0;
			for (int i = indexElementFirst; i <= indexElementLast; i++) {
				num += ((IBetterListViewLayoutElementDisplayable)elements[i]).LayoutBounds.BoundsOuter.Height;
			}
			width += base.LayoutPadding.Left;
			num += base.LayoutPadding.Vertical + 1;
			return new BetterListViewLayoutMeasurement(1, width, num);
		}

		internal override BetterListViewCommonMeasurementItems MeasureElements(ICollection<BetterListViewItem> elementsItems, Graphics graphics, Size contentSize, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool enableExpandButtons, bool enableCheckBoxes, int indent) {
			Checks.CheckNotNull(elementsItems, "elementsItems");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			if (elementsItems.Count == 0) {
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
			return new BetterListViewCommonMeasurementItems(sizeExpandButton, sizeCheckBoxCheck, sizeCheckBoxRadio, 0, num);
		}

		internal override void MeasureElementCoarse(BetterListViewItem item, Graphics graphics, ImageList imageList, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool fullRowSelect, bool enableExpandButton, bool showExpandButton, bool enableCheckBox, bool showCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, ReadOnlyCollection<int> itemOffsets, IDictionary<int, int> usedFontHeights, int resizedColumnIndex) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(((IBetterListViewLayoutElementDisplayable)item).LayoutBounds, "((IBetterListViewLayoutElementDisplayable)item).LayoutBounds");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(elementsColumns, "elementsColumns");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			Checks.CheckNotNull(usedFontHeights, "usedFontHeights");
			Checks.CheckTrue(resizedColumnIndex == -1 || resizedColumnIndex >= 0, "resizedColumnIndex == BetterListViewElementBase.IndexUndefined || resizedColumnIndex >= 0");
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			betterListViewItemBounds.Reset();
			int num = itemOffsets[item.Level];
			this.GetBaseTextHeight(graphics, imageList, item, num, elementsColumns, enableExpandButton, enableCheckBox, extraPadding, commonMeasurement, usedFontHeights, resizedColumnIndex, out var heightBase, out var _);
			for (int i = 0; i < Math.Min(elementsColumns.Count, item.SubItems.Count); i++) {
				BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(i);
				if (!subItemImageSize.IsEmpty) {
					Size sizeImageFrame;
					if (subItemImageSize.IsFixed) {
						sizeImageFrame = subItemImageSize.MinimumSize;
					}
					else {
						Image elementImage = BetterListViewBasePainter.GetElementImage(item.SubItems[i], imageList);
						subItemImageSize.GetImageSize(elementImage, out sizeImageFrame);
					}
					heightBase = Math.Max(heightBase, sizeImageFrame.Height + base.ImagePadding.Vertical);
				}
			}
			int num2 = 0;
			foreach (BetterListViewColumnHeader elementsColumn in elementsColumns) {
				if (elementsColumn.Visible) {
					num2 += ((IBetterListViewLayoutElementDisplayable)elementsColumn).LayoutBounds.BoundsOuter.Width;
				}
			}
			int num3 = Math.Min(base.LayoutPadding.Left + num + extraPadding.Width, ((IBetterListViewLayoutElementDisplayable)elementsColumns[0]).LayoutBounds.BoundsOuter.Width - 1);
			if (num2 > num3) {
				betterListViewItemBounds.BoundsOuter = new Rectangle(num3, 0, num2 - num3, Math.Max(heightBase, item.CustomHeight) + base.ElementInnerPadding.Vertical);
			}
			else {
				betterListViewItemBounds.BoundsOuter = new Rectangle(num3, 0, 0, Math.Max(heightBase, item.CustomHeight) + base.ElementInnerPadding.Vertical);
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
			int num = itemOffsets[item.Level];
			this.GetBaseTextHeight(graphics, imageList, item, num, elementsColumns, enableExpandButton, enableCheckBox, extraPadding, commonMeasurement, null, -1, out var heightBase, out var widthsText);
			for (int i = 0; i < Math.Min(elementsColumns.Count, item.SubItems.Count); i++) {
				BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(i);
				if (!subItemImageSize.IsEmpty) {
					Size sizeImageFrame;
					if (subItemImageSize.IsFixed) {
						sizeImageFrame = subItemImageSize.MinimumSize;
					}
					else {
						Image elementImage = BetterListViewBasePainter.GetElementImage(item.SubItems[i], imageList);
						subItemImageSize.GetImageSize(elementImage, out sizeImageFrame);
					}
					heightBase = Math.Max(heightBase, sizeImageFrame.Height + base.ImagePadding.Vertical);
				}
			}
			Padding elementInnerPadding = base.ElementInnerPadding;
			int num2 = Math.Min(base.LayoutPadding.Left + num + extraPadding.Width, ((IBetterListViewLayoutElementDisplayable)elementsColumns[0]).LayoutBounds.BoundsOuter.Width - 1);
			if (betterListViewItemBounds.BoundsOuter.Width <= elementInnerPadding.Horizontal || betterListViewItemBounds.BoundsOuter.Height <= elementInnerPadding.Vertical) {
				return;
			}
			betterListViewItemBounds.BoundsInner = new Rectangle(betterListViewItemBounds.BoundsOuter.Left + elementInnerPadding.Left, betterListViewItemBounds.BoundsOuter.Top + elementInnerPadding.Top, betterListViewItemBounds.BoundsOuter.Width - elementInnerPadding.Horizontal, Math.Max(heightBase, item.CustomHeight));
			int num3 = 0;
			int val = Math.Min(betterListViewItemBounds.SubItemBounds.Count, elementsColumns.Count);
			val = Math.Min(val, item.SubItems.Count);
			for (int j = 0; j < val; j++) {
				BetterListViewSubItem betterListViewSubItem = item.SubItems[j];
				BetterListViewSubItemBounds betterListViewSubItemBounds = betterListViewItemBounds.SubItemBounds[j];
				if (!elementsColumns[j].Visible) {
					betterListViewSubItemBounds.Reset();
					continue;
				}
				int width = ((IBetterListViewLayoutElementDisplayable)elementsColumns[j]).LayoutBounds.BoundsOuter.Width;
				if (j == 0) {
					if (width <= num2) {
						betterListViewSubItemBounds.Reset();
						num3 += width;
						continue;
					}
					betterListViewSubItemBounds.BoundsOuter = new Rectangle(num2, betterListViewItemBounds.BoundsOuter.Top, width - num2, betterListViewItemBounds.BoundsOuter.Height);
					betterListViewSubItemBounds.BoundsOuterExtended = betterListViewSubItemBounds.BoundsOuter;
				}
				else {
					if (width <= 0) {
						betterListViewSubItemBounds.Reset();
						continue;
					}
					betterListViewSubItemBounds.BoundsOuter = new Rectangle(num3, betterListViewItemBounds.BoundsOuter.Top, width, betterListViewItemBounds.BoundsOuter.Height);
					betterListViewSubItemBounds.BoundsOuterExtended = betterListViewSubItemBounds.BoundsOuter;
				}
				num3 += width;
				if (betterListViewSubItemBounds.BoundsOuter.Width > elementInnerPadding.Horizontal) {
					betterListViewSubItemBounds.BoundsInner = new Rectangle(betterListViewSubItemBounds.BoundsOuter.Left + elementInnerPadding.Left, betterListViewSubItemBounds.BoundsOuter.Top + elementInnerPadding.Top, betterListViewSubItemBounds.BoundsOuter.Width - elementInnerPadding.Horizontal, betterListViewSubItemBounds.BoundsOuter.Height - elementInnerPadding.Vertical);
					int num4 = betterListViewSubItemBounds.BoundsInner.Left;
					int num5 = betterListViewSubItemBounds.BoundsInner.Width;
					if (j == 0) {
						if (enableCheckBox) {
							Size checkBoxSize = commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true);
							if (showCheckBox && num5 > base.CheckBoxPadding.Horizontal) {
								betterListViewItemBounds.BoundsCheckBox = new Rectangle(num4 + base.CheckBoxPadding.Left, betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - checkBoxSize.Height - base.CheckBoxPadding.Vertical >> 1) + base.CheckBoxPadding.Top, checkBoxSize.Width, checkBoxSize.Height);
							}
							else {
								betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
							}
							int num7 = checkBoxSize.Width + base.CheckBoxPadding.Horizontal;
							num4 += num7;
							num5 -= num7;
						}
						else {
							betterListViewItemBounds.BoundsCheckBox = Rectangle.Empty;
						}
					}
					Image elementImage2 = BetterListViewBasePainter.GetElementImage(betterListViewSubItem, imageList);
					BetterListViewImageSize subItemImageSize2 = base.GetSubItemImageSize(j);
					bool flag;
					if (!subItemImageSize2.IsEmpty && (!subItemImageSize2.MinimumSize.IsEmpty || elementImage2 != null) && num5 > base.ImagePadding.Horizontal) {
						BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : betterListViewSubItem.AlignHorizontalImage);
						BetterListViewImageAlignmentVertical alignmentVertical = ((betterListViewSubItem.AlignVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : betterListViewSubItem.AlignVerticalImage);
						subItemImageSize2.GetImageBounds(elementImage2, betterListViewImageAlignmentHorizontal, alignmentVertical, out var sizeImageFrame2, out var boundsImage);
						flag = betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextRight;
						int x = ((!flag) ? (num4 + base.ImagePadding.Left) : (num4 + base.ImagePadding.Left + Math.Max(num5 - sizeImageFrame2.Width - base.ImagePadding.Horizontal, 0)));
						Rectangle boundsImageFrame = new Rectangle(x, betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - sizeImageFrame2.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame2.Width, sizeImageFrame2.Height);
						if (!boundsImage.IsEmpty) {
							boundsImage.Offset(boundsImageFrame.Location);
						}
						betterListViewSubItemBounds.BoundsImageFrame = boundsImageFrame;
						betterListViewSubItemBounds.BoundsImage = boundsImage;
						int num6 = sizeImageFrame2.Width + base.ImagePadding.Horizontal;
						if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
							num4 += num6;
						}
						num5 -= num6;
					}
					else {
						flag = false;
						betterListViewSubItemBounds.BoundsImageFrame = Rectangle.Empty;
						betterListViewSubItemBounds.BoundsImage = Rectangle.Empty;
					}
					if (num5 > 0) {
						betterListViewSubItemBounds.BoundsCell = new Rectangle(num4, betterListViewSubItemBounds.BoundsInner.Top, widthsText[j], betterListViewSubItemBounds.BoundsInner.Height);
					}
					else {
						betterListViewSubItemBounds.BoundsCell = Rectangle.Empty;
					}
					if (num5 > base.TextPadding.Horizontal) {
						int maximumTextLines;
						TextSize textSize;
						if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
							switch (betterListViewSubItem.TextWrapping) {
								case BetterListViewTextWrapping.Layout:
									maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
									textSize = betterListViewSubItem.LayoutGetTextSize(graphics, widthsText[j], maximumTextLines);
									break;
								case BetterListViewTextWrapping.Space:
									maximumTextLines = TextSize.HeightToLineCount(graphics, betterListViewSubItem.Font, betterListViewSubItemBounds.BoundsInner.Height);
									textSize = betterListViewSubItem.LayoutGetTextSize(graphics, widthsText[j], maximumTextLines);
									break;
								case BetterListViewTextWrapping.None:
									maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
									textSize = betterListViewSubItem.LayoutGetTextSize(graphics, widthsText[j], maximumTextLines);
									break;
								default:
									throw new ApplicationException($"Unknown text wrapping: '{betterListViewSubItem.TextWrapping}'.");
							}
						}
						else {
							maximumTextLines = 1;
							textSize = new TextSize(Math.Min(base.EmptyTextSize.Width, widthsText[j]), base.EmptyTextSize.Height, 1, isTextShrunk: false);
						}
						TextAlignmentHorizontal textAlignmentHorizontal = ((betterListViewSubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : betterListViewSubItem.AlignHorizontal);
						TextAlignmentVertical textAlignmentVertical = ((betterListViewSubItem.AlignVertical == TextAlignmentVertical.Default) ? base.DefaultTextAlignmentVertical : betterListViewSubItem.AlignVertical);
						int x2;
						switch (textAlignmentHorizontal) {
							case TextAlignmentHorizontal.Center:
								x2 = num4 + (num5 - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
								break;
							case TextAlignmentHorizontal.Right:
								x2 = betterListViewSubItemBounds.BoundsInner.Right - textSize.Width - base.TextPadding.Horizontal;
								if (flag) {
									x2 -= betterListViewSubItemBounds.BoundsImageFrame.Width + base.ImagePadding.Horizontal;
								}
								x2 = Math.Max(x2, num4);
								x2 += base.TextPadding.Left;
								break;
							default:
								x2 = num4 + base.TextPadding.Left;
								break;
						}
						//betterListViewSubItemBounds.BoundsText = new Rectangle(x2, textAlignmentVertical switch {
						//	TextAlignmentVertical.Middle => betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top,
						//	TextAlignmentVertical.Bottom => betterListViewItemBounds.BoundsInner.Bottom - base.TextPadding.Bottom - textSize.Height,
						//	_ => betterListViewItemBounds.BoundsInner.Top + base.TextPadding.Top,
						//}, textSize.Width, textSize.Height);
						{ 
						int y2 = 0;
							switch (textAlignmentVertical) {
								case TextAlignmentVertical.Middle:
									y2 = betterListViewItemBounds.BoundsInner.Top + (betterListViewItemBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top;
                                    break;
								case TextAlignmentVertical.Bottom:
									y2 = betterListViewItemBounds.BoundsInner.Bottom - base.TextPadding.Bottom - textSize.Height;
									break;
								default:
									y2 = betterListViewItemBounds.BoundsInner.Top + base.TextPadding.Top;
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
				else {
					betterListViewSubItemBounds.Reset();
				}
			}
			BetterListViewItem betterListViewItem = item.SelectableItem ?? item;
			int left = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuter.Left;
			if (item.AllowSelectChildItems) {
				betterListViewItemBounds.BoundsOuterExtended = new Rectangle(left, betterListViewItemBounds.BoundsOuter.Top, betterListViewItemBounds.BoundsOuter.Right - left, betterListViewItemBounds.BoundsOuter.Height);
				int num8 = (fullRowSelect ? betterListViewItemBounds.BoundsOuter.Right : betterListViewItemBounds.SubItemBounds[0].BoundsOuter.Right);
				betterListViewItemBounds.BoundsSelection = new Rectangle(left, betterListViewItemBounds.BoundsOuter.Top, num8 - left, betterListViewItemBounds.BoundsOuter.Height);
				return;
			}
			BetterListViewItem lastAvailableChildItem = item.LastAvailableChildItem;
			if (lastAvailableChildItem != null) {
				BetterListViewItemBounds betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)lastAvailableChildItem).LayoutBounds;
				int left2 = betterListViewItemBounds.BoundsSpacing.Left;
				int top = betterListViewItemBounds.BoundsSpacing.Top;
				int right = betterListViewItemBounds2.BoundsSpacing.Right;
				int bottom = betterListViewItemBounds2.BoundsSpacing.Bottom;
				betterListViewItemBounds.BoundsSpacing = new Rectangle(left2, top, right - left2, bottom - top);
				int top2 = betterListViewItemBounds.BoundsOuter.Top;
				int num9 = (fullRowSelect ? betterListViewItemBounds2.BoundsOuter.Right : betterListViewItemBounds2.SubItemBounds[0].BoundsOuter.Right);
				int bottom2 = betterListViewItemBounds2.BoundsOuter.Bottom;
				betterListViewItemBounds.BoundsOuterExtended = new Rectangle(left, top2, num9 - left, bottom2 - top2);
				betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuterExtended;
			}
			else {
				betterListViewItemBounds.BoundsOuterExtended = betterListViewItemBounds.BoundsOuter;
				betterListViewItemBounds.BoundsSelection = betterListViewItemBounds.BoundsOuter;
			}
		}

		/// <summary>
		///   Get sub-item width for automatic column resizing.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement</param>
		/// <param name="item">item to be measured</param>
		/// <param name="imageList">ImageList for items</param>
		/// <param name="columnIndex">index of a column to be resized</param>
		/// <param name="showExpandButton">expand button is visible on the item</param>
		/// <param name="showCheckBox">check box is visible on the item</param>
		/// <param name="maximumWidth">maximum allowed width</param>
		/// <returns>item width for automatic column resizing</returns>
		internal int GetSubItemAutoWidth(Graphics graphics, BetterListViewItem item, ImageList imageList, int columnIndex, bool showExpandButton, bool showCheckBox, int maximumWidth) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(item, "item");
			Checks.CheckTrue(columnIndex >= 0, "columnIndex >= 0");
			Checks.CheckTrue(maximumWidth >= 0, "maximumWidth >= 0");
			if (columnIndex >= item.SubItems.Count) {
				return 0;
			}
			int num = 0;
			num += base.ElementInnerPadding.Horizontal;
			if (columnIndex == 0) {
				if (showExpandButton) {
					num += BetterListViewBasePainter.GetNodeGlyphSize(graphics).Width + base.ExpandButtonPadding.Horizontal;
				}
				if (showCheckBox) {
					BetterListViewCheckBoxAppearance checkBoxAppearance = ((item.CheckBoxAppearance == BetterListViewCheckBoxAppearance.Hide) ? BetterListViewCheckBoxAppearance.CheckBox : item.CheckBoxAppearance);
					num += BetterListViewBasePainter.GetCheckBoxSize(graphics, checkBoxAppearance).Width + base.CheckBoxPadding.Horizontal;
				}
			}
			BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(columnIndex);
			if (!subItemImageSize.IsEmpty) {
				Size sizeImageFrame;
				if (subItemImageSize.IsFixed) {
					sizeImageFrame = subItemImageSize.MinimumSize;
				}
				else {
					Image elementImage = BetterListViewBasePainter.GetElementImage(item.SubItems[columnIndex], imageList);
					subItemImageSize.GetImageSize(elementImage, out sizeImageFrame);
				}
				num += sizeImageFrame.Width + base.ImagePadding.Horizontal;
			}
			BetterListViewSubItem betterListViewSubItem = item.SubItems[columnIndex];
			if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
				int maximumTextLines = base.GetMaximumTextLines(betterListViewSubItem);
				int width = betterListViewSubItem.LayoutGetTextSize(graphics, int.MaxValue, maximumTextLines).Width;
				num += width + base.TextPadding.Horizontal;
			}
			return Math.Min(num, maximumWidth);
		}

		private void GetBaseTextHeight(Graphics graphics, ImageList imageList, BetterListViewItem item, int itemOffset, ReadOnlyCollection<BetterListViewColumnHeader> elementsColumns, bool enableExpandButton, bool enableCheckBox, Size extraPadding, BetterListViewCommonMeasurementItems commonMeasurement, IDictionary<int, int> usedFontHeights, int resizedColumnIndex, out int heightBase, out int[] widthsText) {
			heightBase = commonMeasurement.HeightBase;
			BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
			widthsText = new int[betterListViewItemBounds.SubItemBounds.Count];
			int num = Math.Min(betterListViewItemBounds.SubItemBounds.Count, item.SubItems.Count);
			List<BetterListViewSubItem> list = new List<BetterListViewSubItem>();
			for (int i = 0; i < num; i++) {
				BetterListViewColumnHeader betterListViewColumnHeader = elementsColumns[i];
				if (!betterListViewColumnHeader.Visible) {
					continue;
				}
				BetterListViewSubItem betterListViewSubItem = item.SubItems[i];
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
				int num2 = ((IBetterListViewLayoutElementDisplayable)betterListViewColumnHeader).LayoutBounds.BoundsOuter.Width - base.ElementInnerPadding.Horizontal;
				if (i == 0) {
					num2 -= base.LayoutPadding.Left + itemOffset + extraPadding.Width;
					if (enableExpandButton) {
						num2 -= commonMeasurement.SizeExpandButton.Width + base.ExpandButtonPadding.Horizontal;
					}
					if (enableCheckBox) {
						num2 -= commonMeasurement.GetCheckBoxSize(item.CheckBoxAppearance, nonEmpty: true).Width + base.CheckBoxPadding.Horizontal;
					}
				}
				BetterListViewImageSize subItemImageSize = base.GetSubItemImageSize(betterListViewSubItem.Index);
				if (!subItemImageSize.IsEmpty) {
					if (subItemImageSize.IsFixed) {
						num2 -= subItemImageSize.MinimumSize.Width + base.ImagePadding.Horizontal;
					}
					else {
						Image elementImage = BetterListViewBasePainter.GetElementImage(betterListViewSubItem, imageList);
						subItemImageSize.GetImageSize(elementImage, out var sizeImageFrame);
						num2 -= sizeImageFrame.Width + base.ImagePadding.Horizontal;
					}
				}
				num2 = Math.Max(num2 - base.TextPadding.Horizontal, 0);
				if (!string.IsNullOrEmpty(betterListViewSubItem.Text)) {
					heightBase = Math.Max(heightBase, TextSize.HeightFromLineCount(value, 1) + base.TextPadding.Vertical);
				}
				widthsText[i] = num2;
			}
			if (usedFontHeights != null) {
				foreach (BetterListViewSubItem item2 in list) {
					int fontHeight = usedFontHeights[item2.Font.GetHashCode()];
					heightBase = Math.Max(heightBase, TextSize.HeightFromLineCount(fontHeight, 1) + base.TextPadding.Vertical);
				}
			}
			else {
				foreach (BetterListViewSubItem item3 in list) {
					heightBase = Math.Max(heightBase, TextSize.HeightFromLineCount(graphics, item3.Font, 1) + base.TextPadding.Vertical);
				}
			}
			heightBase = Math.Max(heightBase, base.EmptyTextSize.Height + base.TextPadding.Vertical);
		}
	}
}