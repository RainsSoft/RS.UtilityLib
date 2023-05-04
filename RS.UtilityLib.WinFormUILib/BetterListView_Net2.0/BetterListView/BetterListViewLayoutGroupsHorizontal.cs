using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Horizontal group layout.
    /// </summary>
    public sealed class BetterListViewLayoutGroupsHorizontal : BetterListViewLayoutGroups
    {
        /// <summary>
        ///   layout elements are oriented vertically
        /// </summary>
        public override bool OrientationVertical => false;

        /// <summary>
        ///   layout allows expand buttons on groups
        /// </summary>
        public override bool AllowsExpandableGroups => false;

        internal override void AdjustElements(ReadOnlyCollection<BetterListViewGroup> elementsGroups, Size contentSize, Graphics graphics, ImageList imageList, bool enableExpandButtons, BetterListViewGroup invisibleHeaderGroup) {
            Checks.CheckNotNull(elementsGroups, "elementsGroups");
            Checks.CheckNotNull(graphics, "graphics");
            Rectangle rectContent = this.GetContentBounds(contentSize);
            if (rectContent.Width > 0 && rectContent.Height > 0) {
                for (int i = 0; i < elementsGroups.Count; i++) {
                    BetterListViewGroup betterListViewGroup = elementsGroups[i];
                    BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds;
                    betterListViewGroupBounds.BoundsSelection = new Rectangle(rectContent.Left, rectContent.Top, betterListViewGroup.LayoutMeasurementItems.Width, betterListViewGroupBounds.BoundsSelection.Height);
                    betterListViewGroupBounds.BoundsOuter = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: false);
                    betterListViewGroupBounds.BoundsOuterExtended = betterListViewGroupBounds.BoundsOuter;
                    betterListViewGroupBounds.BoundsSpacing = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: true);
                    this.MeasureElement(betterListViewGroup, graphics, imageList, betterListViewGroup != invisibleHeaderGroup);
                    rectContent = new Rectangle(rectContent.Left + betterListViewGroupBounds.BoundsSpacing.Width, rectContent.Top, rectContent.Width, rectContent.Height);
                }
            }
        }

        /// <summary>
        ///   Initialize a new BetterListViewLayoutGroupsHorizontal instance.
        /// </summary>
        /// <param name="listView">BetterListView using this layout</param>
        internal BetterListViewLayoutGroupsHorizontal(BetterListView listView)
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
            int num = contentSize.Width - base.LayoutPadding.Horizontal;
            if (num > 0) {
                Size contentSize2 = new Size(contentSize.Width, contentSize.Height - base.LayoutPadding.Vertical - ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)elementsGroups[0]).LayoutBounds).BoundsSelection.Height);
                if (contentSize2.Width > 0 && contentSize2.Height > 0) {
                    int num2 = 0;
                    overflowsHorizontal = false;
                    for (int i = 0; i < elementsGroups.Count; i++) {
                        BetterListViewGroup betterListViewGroup = elementsGroups[i];
                        num2 = ((betterListViewGroup.LayoutIndexItemDisplayFirst == -1) ? (num2 + base.MinimumSize.Width) : (num2 + Math.Max(layoutItems.MeasureLayout(elementsItems, betterListViewGroup.LayoutIndexItemDisplayFirst, betterListViewGroup.LayoutIndexItemDisplayLast, contentSize2).Width, base.MinimumSize.Width)));
                        if (i < elementsGroups.Count - 1) {
                            num2 += base.ElementOuterPadding.Width;
                        }
                        if (num2 > num) {
                            overflowsHorizontal = true;
                            break;
                        }
                    }
                }
                else {
                    overflowsHorizontal = false;
                }
            }
            else {
                overflowsHorizontal = false;
            }
            if (elementsItems.Count != 0) {
                int num3 = contentSize.Height - base.LayoutPadding.Vertical - layoutItems.LayoutPadding.Vertical;
                if (num3 > 0) {
                    overflowsVertical = ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)elementsGroups[0]).LayoutBounds).BoundsSelection.Height + layoutItems.GetColumnSize(elementsItems[0], customPadding: false) > num3;
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
            return new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.MeasureContent, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw, Rectangle.Empty);
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
            int height = base.LayoutPadding.Vertical;
            for (int i = indexElementFirst; i <= indexElementLast; i++) {
                Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)elements[i]).LayoutBounds.BoundsSpacing;
                num += boundsSpacing.Width;
                height = Math.Max(num, boundsSpacing.Height);
            }
            return new BetterListViewLayoutMeasurement(1, num, height);
        }

        internal override BetterListViewCommonMeasurementGroups MeasureElements(ICollection<BetterListViewGroup> elementsGroups, Graphics graphics, Size contentSize, bool enableExpandButtons) {
            Checks.CheckNotNull(elementsGroups, "elementsGroups");
            Checks.CheckNotNull(graphics, "graphics");
            if (elementsGroups.Count == 0 || contentSize.Width <= 0 || contentSize.Height <= 0) {
                return BetterListViewCommonMeasurementGroups.Empty;
            }
            int num = base.MinimumSize.Height;
            BetterListViewImageSize imageSize = base.ImageSize;
            if (!imageSize.IsEmpty) {
                num = Math.Max(num, imageSize.MaximumSize.Height + base.ImagePadding.Vertical);
            }
            if (base.MinimumTextHeight > 0) {
                num = Math.Max(num, base.MinimumTextHeight + base.TextPadding.Vertical);
            }
            num += base.ElementInnerPadding.Vertical;
            Size contentSizeItems = new Size(contentSize.Width, Math.Max(contentSize.Height - base.LayoutPadding.Horizontal - num, 0));
            return new BetterListViewCommonMeasurementGroups(num, contentSizeItems);
        }

        internal override void MeasureElement(BetterListViewGroup group, Graphics graphics, BetterListViewCommonMeasurementGroups commonMeasurement, ImageList imageList, bool showHeader) {
            Checks.CheckNotNull(group, "group");
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckFalse(commonMeasurement.IsEmpty, "commonMeasurement.IsEmpty");
            ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds).BoundsSelection = new Rectangle(0, 0, 0, commonMeasurement.HeightOuter);
        }

        private void MeasureElement(BetterListViewGroup group, Graphics graphics, ImageList imageList, bool showHeader) {
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
                    int num3 = sizeImageFrame.Width + base.ImagePadding.Horizontal;
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
                                y2 =betterListViewGroupBounds.BoundsInner.Top + (betterListViewGroupBounds.BoundsInner.Height - textSize.Height - base.TextPadding.Vertical >> 1) + base.TextPadding.Top;
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
                betterListViewGroupBounds.BoundsInner = new Rectangle(betterListViewGroupBounds.BoundsSelection.Left + base.ElementInnerPadding.Left, betterListViewGroupBounds.BoundsSelection.Top + base.ElementInnerPadding.Top, betterListViewGroupBounds.BoundsSelection.Width - base.ElementInnerPadding.Horizontal, betterListViewGroupBounds.BoundsSelection.Height - base.ElementInnerPadding.Vertical);
                betterListViewGroupBounds.BoundsImageFrame = Rectangle.Empty;
                betterListViewGroupBounds.BoundsImage = Rectangle.Empty;
                betterListViewGroupBounds.BoundsText = Rectangle.Empty;
                betterListViewGroupBounds.IsTextShrunk = false;
            }
        }

        private Rectangle GetContentBounds(Size contentSize) {
            return new Rectangle(base.LayoutPadding.Left, base.LayoutPadding.Top, contentSize.Width, contentSize.Height - base.LayoutPadding.Vertical);
        }

        private Rectangle GetGroupBounds(BetterListViewGroup group, int groupCount, Rectangle rectContent, bool spacingBounds) {
            BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
            int num = group.LayoutMeasurementItems.Width;
            if (spacingBounds && ((IBetterListViewLayoutElementDisplayable)group).LayoutIndexDisplay < groupCount - 1) {
                num += base.ElementOuterPadding.Width;
            }
            return new Rectangle(rectContent.Left, rectContent.Top, num, betterListViewGroupBounds.BoundsSelection.Height + group.LayoutMeasurementItems.Height + base.ElementOuterPadding.Height);
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
                if (betterListViewGroup.LayoutIndexItemDisplayFirst != -1) {
                    int layoutIndexDisplay = ((IBetterListViewLayoutElementDisplayable)elementsItems[betterListViewGroup.LayoutIndexItemDisplayFirst]).LayoutIndexDisplay;
                    int layoutIndexDisplay2 = ((IBetterListViewLayoutElementDisplayable)elementsItems[betterListViewGroup.LayoutIndexItemDisplayLast]).LayoutIndexDisplay;
                    Size contentSize2 = new Size(rectContent.Width, Math.Max(rectContent.Height - betterListViewGroupBounds.BoundsSelection.Height, 0));
                    layoutItems.PositionElements(elementsItems, columnHeaderBoundsFirst, layoutIndexDisplay, layoutIndexDisplay2, contentSize2, new Size(rectContent.Left, rectContent.Top + betterListViewGroupBounds.BoundsSelection.Height));
                    BetterListViewLayoutMeasurement betterListViewLayoutMeasurement = layoutItems.MeasureLayout(elementsItems, layoutIndexDisplay, layoutIndexDisplay2, contentSize2);
                    betterListViewGroup.LayoutMeasurementItems = new BetterListViewLayoutMeasurement(betterListViewLayoutMeasurement.ElementsPerRow, Math.Max(betterListViewLayoutMeasurement.Width, base.MinimumSize.Width), betterListViewLayoutMeasurement.Height);
                }
                else {
                    betterListViewGroup.LayoutMeasurementItems = new BetterListViewLayoutMeasurement(1, base.MinimumSize.Width, 0);
                }
                Rectangle groupBounds = this.GetGroupBounds(betterListViewGroup, elementsGroups.Count, rectContent, spacingBounds: true);
                rectContent = new Rectangle(rectContent.Left + groupBounds.Width, rectContent.Top, rectContent.Width, rectContent.Height);
            }
        }

        internal override int GetGroupContentOffset(BetterListViewGroup group) {
            Checks.CheckNotNull(group, "group");
            BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds;
            int num = ((!betterListViewGroupBounds.BoundsImage.Size.IsEmpty) ? (betterListViewGroupBounds.BoundsImage.Left - base.ImagePadding.Left) : (betterListViewGroupBounds.BoundsText.Size.IsEmpty ? betterListViewGroupBounds.BoundsInner.Left : (betterListViewGroupBounds.BoundsText.Left - base.TextPadding.Left)));
            return num - betterListViewGroupBounds.BoundsOuter.Left;
        }
    }
}