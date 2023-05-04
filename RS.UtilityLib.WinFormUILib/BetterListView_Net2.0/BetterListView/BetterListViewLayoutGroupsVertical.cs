using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Vertical groups layout.
	/// </summary>
	public sealed class BetterListViewLayoutGroupsVertical : BetterListViewLayoutGroups
	{
		private const int DefaultExpandButtonPadding = 2;

		private Padding expandButtonPadding;

		/// <summary>
		///   layout elements are oriented vertically
		/// </summary>
		public override bool OrientationVertical => true;

		/// <summary>
		///   layout allows expand buttons on groups
		/// </summary>
		public override bool AllowsExpandableGroups => true;

		/// <summary>
		///   group expand button padding
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

		internal override void AdjustElements(ReadOnlyCollection<BetterListViewGroup> elementsGroups, Size contentSize, Graphics graphics, ImageList imageList, bool enableExpandButtons, BetterListViewGroup invisibleHeaderGroup) {
			Checks.CheckNotNull(elementsGroups, "elementsGroups");
			Checks.CheckNotNull(graphics, "graphics");
			Rectangle rectContent = this.GetContentBounds(contentSize);
			if (rectContent.Width > 0 && rectContent.Height > 0) {
				for (int i = 0; i < elementsGroups.Count; i++) {
					BetterListViewGroup betterListViewGroup = elementsGroups[i];
					BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds;
					betterListViewGroupBounds.BoundsSelection = new Rectangle(rectContent.Left, rectContent.Top, rectContent.Width, betterListViewGroupBounds.BoundsSelection.Height);
					betterListViewGroupBounds.BoundsOuter = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: false);
					betterListViewGroupBounds.BoundsOuterExtended = betterListViewGroupBounds.BoundsOuter;
					betterListViewGroupBounds.BoundsSpacing = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: true);
					this.MeasureElement(betterListViewGroup, graphics, enableExpandButtons, betterListViewGroup.AllowShowExpandButton && betterListViewGroup.GetItems((BetterListView)base.ItemsView).Count != 0, imageList, betterListViewGroup != invisibleHeaderGroup);
					rectContent = new Rectangle(rectContent.Left, rectContent.Top + betterListViewGroupBounds.BoundsSpacing.Height, rectContent.Width, rectContent.Height);
				}
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewLayoutGroupsVertical instance.
		/// </summary>
		/// <param name="listView">BetterListView using this layout</param>
		internal BetterListViewLayoutGroupsVertical(BetterListView listView)
			: base(listView) {
		}

		internal override void CheckOverflows(ReadOnlyCollection<BetterListViewGroup> elementsGroups, ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewLayoutItems layoutItems, Size contentSize, ref bool overflowsHorizontal, ref bool overflowsVertical) {
			Checks.CheckNotNull(elementsGroups, "elementsGroups");
			Checks.CheckNotNull(elementsItems, "elementsItems");
			Checks.CheckNotNull(layoutItems, "layoutItems");
			Checks.CheckTrue(contentSize.Width >= 0 && contentSize.Height >= 0, "contentSize.Width >= 0 && sizeContent.Height >= 0");
			if (elementsGroups.Count == 0 && elementsItems.Count == 0) {
				overflowsHorizontal = false;
				overflowsVertical = false;
				return;
			}
			if (elementsItems.Count != 0) {
				int num = (layoutItems.CustomPadding ? (contentSize.Width - base.LayoutPadding.Horizontal) : (contentSize.Width - base.LayoutPadding.Horizontal - layoutItems.LayoutPadding.Horizontal));
				if (num > 0) {
					overflowsHorizontal = layoutItems.GetColumnSize(elementsItems[0], customPadding: false) > num;
				}
				else {
					overflowsHorizontal = false;
				}
			}
			else {
				overflowsHorizontal = false;
			}
			int num2 = contentSize.Height - base.LayoutPadding.Vertical;
			if (num2 > 0) {
				Size contentSize2 = new Size(Math.Max(contentSize.Width - base.LayoutPadding.Horizontal, 0), contentSize.Height);
				if (contentSize2.Width > 0 && contentSize2.Height > 0) {
					int num3 = 0;
					overflowsVertical = false;
					int num4 = 0;
					while (true) {
						if (num4 < elementsGroups.Count) {
							BetterListViewGroup betterListViewGroup = elementsGroups[num4];
							num3 += ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds).BoundsSelection.Height;
							if (betterListViewGroup.IsExpanded && betterListViewGroup.LayoutIndexItemDisplayFirst != -1) {
								num3 += layoutItems.MeasureLayout(elementsItems, betterListViewGroup.LayoutIndexItemDisplayFirst, betterListViewGroup.LayoutIndexItemDisplayLast, contentSize2).Height;
							}
							if (num4 < elementsGroups.Count - 1) {
								num3 += base.ElementOuterPadding.Height;
							}
							if (num3 > num2) {
								break;
							}
							num4++;
							continue;
						}
						return;
					}
					overflowsVertical = true;
				}
				else {
					overflowsVertical = false;
				}
			}
			else {
				overflowsVertical = false;
			}
		}

		internal override BetterListViewInvalidationInfo GetResizeInvalidationInfo(BetterListViewInvalidationInfo itemResizeInvalidationInfo) {
			return new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.Adjust, BetterListViewInvalidationFlags.Draw, Rectangle.Empty).UnionWith(itemResizeInvalidationInfo);
		}

		/// <summary>
		///   Measure the layout.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to measure</param>
		/// <param name="indexElementLast">index of the last layout element to measure</param>
		/// <param name="contentSize">content area size</param>
		/// <returns>layout measurement</returns>
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewGroup> elements, int indexElementFirst, int indexElementLast, Size contentSize) {
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
		internal override BetterListViewLayoutMeasurement MeasureLayout(IList<BetterListViewGroup> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool customPadding) {
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
			int num = base.LayoutPadding.Horizontal;
			int num2 = base.LayoutPadding.Vertical;
			for (int i = indexElementFirst; i <= indexElementLast; i++) {
				BetterListViewGroup betterListViewGroup = elements[i];
				Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds.BoundsSpacing;
				num = Math.Max(num, Math.Max(boundsSpacing.Width, betterListViewGroup.LayoutMeasurementItems.Width));
				num2 += boundsSpacing.Height;
			}
			return new BetterListViewLayoutMeasurement(1, num, num2);
		}

		internal override BetterListViewCommonMeasurementGroups MeasureElements(ICollection<BetterListViewGroup> elementsGroups, Graphics graphics, Size contentSize, bool enableExpandButtons) {
			Checks.CheckNotNull(elementsGroups, "elements");
			Checks.CheckNotNull(graphics, "graphics");
			if (elementsGroups.Count == 0 || contentSize.Width <= 0 || contentSize.Height <= 0) {
				return BetterListViewCommonMeasurementGroups.Empty;
			}
			Size expandButtonSize = BetterListViewPainter.GetExpandButtonSize(graphics);
			int num = base.MinimumSize.Height;
			if (enableExpandButtons) {
				num = Math.Max(num, expandButtonSize.Height + this.ExpandButtonPadding.Vertical);
			}
			Size contentSizeItems = new Size(Math.Max(contentSize.Width - base.LayoutPadding.Horizontal, 0), contentSize.Height);
			return new BetterListViewCommonMeasurementGroups(num, contentSizeItems);
		}

		internal override void MeasureElement(BetterListViewGroup group, Graphics graphics, BetterListViewCommonMeasurementGroups commonMeasurement, ImageList imageList, bool showHeader) {
			Checks.CheckNotNull(group, "group");
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
			BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
			if (showHeader) {
				int val = commonMeasurement.HeightOuter;
				BetterListViewImageSize imageSize = base.ImageSize;
				if (!imageSize.IsEmpty) {
					Size sizeImageFrame;
					if (imageSize.IsFixed) {
						sizeImageFrame = imageSize.MinimumSize;
					}
					else {
						Image elementImage = BetterListViewBasePainter.GetElementImage(group, imageList);
						imageSize.GetImageSize(elementImage, out sizeImageFrame);
					}
					val = Math.Max(val, sizeImageFrame.Height + base.ImagePadding.Vertical);
				}
				val = Math.Max(val, group.LayoutGetTextSize(graphics, int.MaxValue).Height + base.TextPadding.Vertical);
				val += base.ElementInnerPadding.Vertical;
				betterListViewGroupBounds.BoundsSelection = new Rectangle(0, 0, 0, val);
			}
			else {
				betterListViewGroupBounds.BoundsSelection = Rectangle.Empty;
			}
		}

		private void MeasureElement(BetterListViewGroup group, Graphics graphics, bool enableExpandButtons, bool showExpandButton, ImageList imageList, bool showHeader) {
			BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
			if (showHeader) {
				betterListViewGroupBounds.BoundsInner = new Rectangle(betterListViewGroupBounds.BoundsSelection.Left + base.ElementInnerPadding.Left, betterListViewGroupBounds.BoundsSelection.Top + base.ElementInnerPadding.Top, betterListViewGroupBounds.BoundsSelection.Width - base.ElementInnerPadding.Horizontal, betterListViewGroupBounds.BoundsSelection.Height - base.ElementInnerPadding.Vertical);
				int num = betterListViewGroupBounds.BoundsInner.Left;
				int num2 = betterListViewGroupBounds.BoundsInner.Width;
				BetterListViewImageSize imageSize = base.ImageSize;
				Image elementImage = BetterListViewBasePainter.GetElementImage(group, imageList);
				bool flag;
				if (!imageSize.IsEmpty && (!imageSize.MinimumSize.IsEmpty || elementImage != null) && num2 > base.ImagePadding.Horizontal) {
					BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((group.HeaderAlignmentHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? base.DefaultImageAlignmentHorizontal : group.HeaderAlignmentHorizontalImage);
					BetterListViewImageAlignmentVertical alignmentVertical = ((group.HeaderAlignmentVerticalImage == BetterListViewImageAlignmentVertical.Default) ? base.DefaultImageAlignmentVertical : group.HeaderAlignmentVerticalImage);
					imageSize.GetImageBounds(elementImage, betterListViewImageAlignmentHorizontal, alignmentVertical, out var sizeImageFrame, out var boundsImage);
					flag = betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.AfterTextRight;
					int x = ((!flag) ? (num + base.ImagePadding.Left) : (num + base.ImagePadding.Left + Math.Max(num2 - sizeImageFrame.Width - base.ImagePadding.Horizontal, 0)));
					Rectangle boundsImageFrame = new Rectangle(x, betterListViewGroupBounds.BoundsInner.Top + (betterListViewGroupBounds.BoundsInner.Height - sizeImageFrame.Height - base.ImagePadding.Vertical >> 1) + base.ImagePadding.Top, sizeImageFrame.Width, sizeImageFrame.Height);
					if (!boundsImage.IsEmpty) {
						boundsImage.Offset(boundsImageFrame.Location);
					}
					betterListViewGroupBounds.BoundsImageFrame = boundsImageFrame;
					betterListViewGroupBounds.BoundsImage = boundsImage;
					int num3 = boundsImageFrame.Width + base.ImagePadding.Horizontal;
					if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
						num += num3;
					}
					num2 -= num3;
				}
				else {
					flag = false;
					betterListViewGroupBounds.BoundsImageFrame = Rectangle.Empty;
					betterListViewGroupBounds.BoundsImage = Rectangle.Empty;
				}
				if (num2 > base.TextPadding.Horizontal && !string.IsNullOrEmpty(group.Header)) {
					TextSize textSize = group.LayoutGetTextSize(graphics, num2 - base.TextPadding.Horizontal);
					TextAlignmentHorizontal textAlignmentHorizontal = ((group.HeaderAlignmentHorizontal == TextAlignmentHorizontal.Default) ? base.DefaultTextAlignmentHorizontal : group.HeaderAlignmentHorizontal);
					TextAlignmentVertical textAlignmentVertical = ((group.HeaderAlignmentVertical == TextAlignmentVertical.Default) ? base.DefaultTextAlignmentVertical : group.HeaderAlignmentVertical);
					int x2;
					switch (textAlignmentHorizontal) {
						case TextAlignmentHorizontal.Center:
							x2 = num + (num2 - textSize.Width - base.TextPadding.Horizontal >> 1) + base.TextPadding.Left;
							break;
						case TextAlignmentHorizontal.Right:
							x2 = betterListViewGroupBounds.BoundsInner.Right - textSize.Width - base.TextPadding.Horizontal;
							if (flag) {
								x2 -= betterListViewGroupBounds.BoundsImageFrame.Width + base.ImagePadding.Horizontal;
							}
							x2 = Math.Max(x2, num);
							x2 += base.TextPadding.Left;
							break;
						default:
							x2 = num + base.TextPadding.Left;
							break;
					}
					//betterListViewGroupBounds.BoundsText = new Rectangle(x2, textAlignmentVertical switch {
					//	TextAlignmentVertical.Middle => betterListViewGroupBounds.BoundsInner.Top + (betterListViewGroupBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top,
					//	TextAlignmentVertical.Bottom => betterListViewGroupBounds.BoundsInner.Bottom - textSize.Height - base.TextPadding.Vertical,
					//	_ => betterListViewGroupBounds.BoundsInner.Top + base.TextPadding.Top,
					//}, textSize.Width, textSize.Height);
					{
                        int y2;
                        switch (textAlignmentVertical) {
                            case TextAlignmentVertical.Middle:
                                y2 = betterListViewGroupBounds.BoundsInner.Top + (betterListViewGroupBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top;
                                break;
                            case TextAlignmentVertical.Bottom:
                                y2 =betterListViewGroupBounds.BoundsInner.Bottom - textSize.Height - base.TextPadding.Vertical;
                                break;
                            default:
                                y2 =betterListViewGroupBounds.BoundsInner.Top + base.TextPadding.Top;
                                break;
                        }
						betterListViewGroupBounds.BoundsText = new Rectangle(x2, y2, textSize.Width, textSize.Height);
					}
					betterListViewGroupBounds.IsTextShrunk = textSize.IsTextShrunk;
				}
				else {
					betterListViewGroupBounds.BoundsText = Rectangle.Empty;
					betterListViewGroupBounds.IsTextShrunk = false;
				}
			}
			else {
				betterListViewGroupBounds.BoundsInner = Rectangle.Empty;
				betterListViewGroupBounds.BoundsImageFrame = Rectangle.Empty;
				betterListViewGroupBounds.BoundsImage = Rectangle.Empty;
				betterListViewGroupBounds.BoundsText = Rectangle.Empty;
				betterListViewGroupBounds.IsTextShrunk = false;
			}
		}

		private Rectangle GetContentBounds(Size contentSize) {
			return new Rectangle(base.LayoutPadding.Left, base.LayoutPadding.Top, Math.Max(contentSize.Width - base.LayoutPadding.Horizontal, 0), contentSize.Height);
		}

		private Rectangle GetGroupBounds(BetterListViewGroup group, int groupCount, Rectangle rectContent, bool spacingBounds) {
			BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
			int num = (group.IsExpanded ? group.LayoutMeasurementItems.Height : 0);
			int num2 = betterListViewGroupBounds.BoundsSelection.Height + num;
			if (spacingBounds && ((IBetterListViewLayoutElementDisplayable)group).LayoutIndexDisplay < groupCount - 1) {
				num2 += base.ElementOuterPadding.Height;
			}
			return new Rectangle(rectContent.Left, rectContent.Top, rectContent.Width, num2);
		}

		internal override void PositionElements(ReadOnlyCollection<BetterListViewGroup> elementsGroups, ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, Size contentSize, BetterListViewLayoutItems layoutItems) {
			Checks.CheckNotNull(elementsGroups, "elementsGroups");
			Checks.CheckNotNull(elementsItems, "elementsItems");
			Checks.CheckNotNull(layoutItems, "layoutItems");
			Rectangle rectContent = this.GetContentBounds(contentSize);
			if (rectContent.Width <= 0 || rectContent.Height <= 0) {
				return;
			}
			for (int i = 0; i < elementsGroups.Count; i++) {
				BetterListViewGroup betterListViewGroup = elementsGroups[i];
				BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds;
				if (betterListViewGroup.IsExpanded && betterListViewGroup.LayoutIndexItemDisplayFirst != -1) {
					int layoutIndexDisplay = ((IBetterListViewLayoutElementDisplayable)elementsItems[betterListViewGroup.LayoutIndexItemDisplayFirst]).LayoutIndexDisplay;
					int layoutIndexDisplay2 = ((IBetterListViewLayoutElementDisplayable)elementsItems[betterListViewGroup.LayoutIndexItemDisplayLast]).LayoutIndexDisplay;
					layoutItems.PositionElements(elementsItems, columnHeaderBoundsFirst, layoutIndexDisplay, layoutIndexDisplay2, rectContent.Size, new Size(rectContent.Left, rectContent.Top + betterListViewGroupBounds.BoundsSelection.Height));
					betterListViewGroup.LayoutMeasurementItems = layoutItems.MeasureLayout(elementsItems, layoutIndexDisplay, layoutIndexDisplay2, rectContent.Size);
				}
				else {
					betterListViewGroup.LayoutMeasurementItems = BetterListViewLayoutMeasurement.Empty;
				}
				Rectangle groupBounds = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: true);
				rectContent = new Rectangle(rectContent.Left, rectContent.Top + groupBounds.Height, rectContent.Width, rectContent.Height);
			}
		}

		internal override int GetGroupContentOffset(BetterListViewGroup group) {
			Checks.CheckNotNull(group, "group");
			BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
			int num = ((!betterListViewGroupBounds.BoundsImage.Size.IsEmpty) ? (betterListViewGroupBounds.BoundsImage.Left - base.ImagePadding.Left) : (betterListViewGroupBounds.BoundsText.Size.IsEmpty ? (betterListViewGroupBounds.BoundsExpandButton.Right + this.ExpandButtonPadding.Right) : (betterListViewGroupBounds.BoundsText.Left - base.TextPadding.Left)));
			return num - betterListViewGroupBounds.BoundsOuter.Left;
		}

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected override void SetDefaultsInternal() {
			this.expandButtonPadding = new Padding(2);
			base.SetDefaultsInternal();
		}
	}
}