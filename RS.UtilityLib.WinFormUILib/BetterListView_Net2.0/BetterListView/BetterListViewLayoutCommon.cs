using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common layout of BetterListView elements.
	/// </summary>
	public abstract class BetterListViewLayoutCommon<TElement> : BetterListViewLayoutBase where TElement : BetterListViewElementBase
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutCommon`1" /> class.
		/// </summary>
		/// <param name="itemsView">BetterListView instance.</param>
		protected internal BetterListViewLayoutCommon(BetterListViewBase itemsView)
			: base(itemsView) {
		}

		internal TElement GetElement(ReadOnlyCollection<TElement> elements, Point location, BetterListViewLayoutVisibleRange visibleRange) {
			Checks.CheckNotNull(elements, "elements");
			if (visibleRange.IsUndefined) {
				return null;
			}
			int num = visibleRange.IndexElementFirst;
			int num2 = visibleRange.IndexElementLast;
			int num3 = visibleRange.IndexElementFirst;
			while (num != num2) {
				num3 = num + num2 >> 1;
				int num4 = this.CompareLocations(((IBetterListViewLayoutElementDisplayable)elements[num3]).LayoutBounds.BoundsOuterExtended, location);
				if (num4 > 0) {
					num2 = num3;
					continue;
				}
				if (num4 >= 0) {
					break;
				}
				if (num3 == num) {
					num3++;
					break;
				}
				num = num3;
			}
			TElement val = elements[num3];
			if (((IBetterListViewLayoutElementDisplayable)val).LayoutBounds.BoundsOuterExtended.Contains(location)) {
				return val;
			}
			return null;
		}

		internal TElement GetElementNearest(ReadOnlyCollection<TElement> elements, Point location) {
			Checks.CheckNotNull(elements, "elements");
			int num = 0;
			int num2 = elements.Count - 1;
			int num3 = 0;
			while (num != num2) {
				num3 = num + num2 >> 1;
				int num4 = this.CompareLocations(((IBetterListViewLayoutElementDisplayable)elements[num3]).LayoutBounds.BoundsOuterExtended, location);
				if (num4 > 0) {
					num2 = num3;
					continue;
				}
				if (num4 >= 0) {
					break;
				}
				if (num3 == num) {
					num3++;
					break;
				}
				num = num3;
			}
			return elements[num3];
		}

		/// <summary>
		///   Get layout element adjacent to the specified layout element.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElement">index of a layout element to get adjacent layout element for</param>
		/// <param name="elementsPerRow">number of layout elements per row</param>
		/// <param name="elementsPerPage">number of elements per page</param>
		/// <param name="minimumAllowedIndex">minimum allowed index of the adjacent layout element</param>
		/// <param name="maximumAllowedIndex">maximum allowed index of the adjacent layout element</param>
		/// <param name="targetElement">adjacent layout element search direction</param>
		/// <returns>layout element adjacent to the specified layout element</returns>
		internal TElement GetAdjacentElement(ReadOnlyCollection<TElement> elements, int indexElement, int elementsPerRow, int elementsPerPage, int minimumAllowedIndex, int maximumAllowedIndex, BetterListViewTargetElement targetElement) {
			Checks.CheckCollection(elements, "elements");
			Checks.CheckBounds(indexElement, 0, elements.Count - 1, "indexElement");
			Checks.CheckBounds(elementsPerRow, 1, elements.Count, "elementsPerRow");
			Checks.CheckBounds(elementsPerPage, 1, elements.Count, "elementsPerPage");
			if (elements.Count != 0) {
				Checks.CheckBounds(minimumAllowedIndex, 0, elements.Count - 1, "minimumAllowedIndex");
				Checks.CheckBounds(maximumAllowedIndex, minimumAllowedIndex, elements.Count - 1, "maximumAllowedIndex");
			}
			else {
				Checks.CheckEqual(minimumAllowedIndex, 0, "minimumAllowedIndex", "0");
				Checks.CheckEqual(maximumAllowedIndex, 0, "maximumAllowedIndex", "0");
			}
			int num;
			switch (targetElement) {
				case BetterListViewTargetElement.Up:
					num = ((!this.OrientationVertical) ? (indexElement - 1) : (indexElement - elementsPerRow));
					break;
				case BetterListViewTargetElement.Down:
					if (this.OrientationVertical) {
						num = indexElement + elementsPerRow;
						int num5 = indexElement / elementsPerRow;
						int num6 = num / elementsPerRow;
						int num7 = (maximumAllowedIndex - minimumAllowedIndex) / elementsPerRow;
						if (num6 != num5 && num6 == num7) {
							num = Math.Min(num, maximumAllowedIndex);
						}
					}
					else {
						num = indexElement + 1;
					}
					break;
				case BetterListViewTargetElement.Left:
					num = ((!this.OrientationVertical) ? (indexElement - elementsPerRow) : (indexElement - 1));
					break;
				case BetterListViewTargetElement.Right: {
						if (this.OrientationVertical) {
							num = indexElement + 1;
							break;
						}
						num = indexElement + elementsPerRow;
						int num2 = indexElement / elementsPerRow;
						int num3 = num / elementsPerRow;
						int num4 = (maximumAllowedIndex - minimumAllowedIndex) / elementsPerRow;
						if (num3 != num2 && num3 == num4) {
							num = Math.Min(num, maximumAllowedIndex);
						}
						break;
					}
				case BetterListViewTargetElement.PageUp:
					num = indexElement - elementsPerPage;
					break;
				case BetterListViewTargetElement.PageDown:
					num = indexElement + elementsPerPage;
					break;
				case BetterListViewTargetElement.Home:
					num = 0;
					break;
				case BetterListViewTargetElement.End:
					num = elements.Count - 1;
					break;
				default:
					throw new ApplicationException($"Unknown layout search direction: '{targetElement}'.");
			}
			if (num < minimumAllowedIndex || num > maximumAllowedIndex) {
				return null;
			}
			return elements[num];
		}

		/// <summary>
		///   Check whether the layout overflows the specified content area.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to check</param>
		/// <param name="indexElementLast">index of the last layout element to check</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="vertical">check for vertical overflow</param>
		/// <returns>the layout overflows the specified content area</returns>
		protected internal virtual bool CheckOverflows(ReadOnlyCollection<TElement> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool vertical) {
			if (elements.Count == 0) {
				return false;
			}
			int contentSizeColumn = this.GetContentSizeColumn(contentSize, customPadding: false);
			int columnSize = this.GetColumnSize(elements[0], customPadding: false);
			bool orientationVertical = this.OrientationVertical;
			if (vertical == orientationVertical) {
				int elementCount = indexElementLast - indexElementFirst + 1;
				int elementsPerRow = this.GetElementsPerRow(elementCount, contentSizeColumn, columnSize);
				int num = 0;
				int num2 = indexElementFirst;
				do {
					int num3 = 0;
					for (int i = 0; i < elementsPerRow && num2 + i <= indexElementLast; i++) {
						BetterListViewElementBoundsBase layoutBounds = ((IBetterListViewLayoutElementDisplayable)elements[num2 + i]).LayoutBounds;
						num3 = Math.Max(num3, orientationVertical ? layoutBounds.BoundsOuter.Height : layoutBounds.BoundsOuter.Width);
					}
					num3 += (orientationVertical ? base.ElementOuterPadding.Height : base.ElementOuterPadding.Width);
					num += num3;
					if (num > this.GetContentSizeRow(contentSize, customPadding: false)) {
						return true;
					}
					num2 += elementsPerRow;
				}
				while (num2 <= indexElementLast);
			}
			else if (columnSize > contentSizeColumn) {
				return true;
			}
			return false;
		}

		private int CompareLocations(Rectangle rectBoundsSpacing, Point location) {
			if (this.OrientationVertical) {
				if (rectBoundsSpacing.Bottom <= location.Y) {
					return -1;
				}
				if (rectBoundsSpacing.Top > location.Y) {
					return 1;
				}
				if (rectBoundsSpacing.Right <= location.X) {
					return -1;
				}
				if (rectBoundsSpacing.Left > location.X) {
					return 1;
				}
			}
			else {
				if (rectBoundsSpacing.Right <= location.X) {
					return -1;
				}
				if (rectBoundsSpacing.Left > location.X) {
					return 1;
				}
				if (rectBoundsSpacing.Bottom <= location.Y) {
					return -1;
				}
				if (rectBoundsSpacing.Top > location.Y) {
					return 1;
				}
			}
			return 0;
		}

		/// <summary>
		///   Measure the layout.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="indexElementFirst">index of the first layout element to measure</param>
		/// <param name="indexElementLast">index of the last layout element to measure</param>
		/// <param name="contentSize">content area size</param>
		/// <returns>layout measurement</returns>
		internal virtual BetterListViewLayoutMeasurement MeasureLayout(IList<TElement> elements, int indexElementFirst, int indexElementLast, Size contentSize) {
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
		internal virtual BetterListViewLayoutMeasurement MeasureLayout(IList<TElement> elements, int indexElementFirst, int indexElementLast, Size contentSize, bool customPadding) {
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
			int contentSizeColumn = this.GetContentSizeColumn(contentSize, customPadding);
			if (contentSizeColumn < 0) {
				return BetterListViewLayoutMeasurement.Empty;
			}
			int columnSize = this.GetColumnSize(elements[indexElementFirst], customPadding);
			int elementCount = indexElementLast - indexElementFirst + 1;
			int elementsPerRow = this.GetElementsPerRow(elementCount, contentSizeColumn, columnSize);
			int num;
			int num2;
			if (this.OrientationVertical) {
				num = elementsPerRow * columnSize;
				num2 = 0;
				for (int i = indexElementFirst; i <= indexElementLast; i += elementsPerRow) {
					int num3 = Math.Min(i + elementsPerRow - 1, indexElementLast);
					int num4 = 0;
					for (int j = i; j <= num3; j++) {
						num4 = Math.Max(num4, ((IBetterListViewLayoutElementDisplayable)elements[j]).LayoutBounds.BoundsOuterExtended.Height);
					}
					num2 += num4 + base.ElementOuterPadding.Height;
				}
			}
			else {
				num = 0;
				for (int k = indexElementFirst; k <= indexElementLast; k += elementsPerRow) {
					int num5 = Math.Min(k + elementsPerRow - 1, indexElementLast);
					int num6 = 0;
					for (int l = k; l <= num5; l++) {
						num6 = Math.Max(num6, ((IBetterListViewLayoutElementDisplayable)elements[l]).LayoutBounds.BoundsOuterExtended.Width);
					}
					num += num6 + base.ElementOuterPadding.Width;
				}
				num2 = elementsPerRow * columnSize;
			}
			if (!customPadding) {
				num += base.LayoutPadding.Horizontal;
				num2 += base.LayoutPadding.Vertical;
			}
			return new BetterListViewLayoutMeasurement(elementsPerRow, num, num2);
		}

		/// <summary>
		///   Get content size in a column direction.
		/// </summary>
		/// <param name="contentSize">size of a content area</param>
		/// <param name="customPadding">custom layout padding is used</param>
		/// <returns>content size in a column direction</returns>
		internal int GetContentSizeColumn(Size contentSize, bool customPadding) {
			Checks.CheckSize(contentSize, "contentSize");
			if (customPadding) {
				if (!this.OrientationVertical) {
					return contentSize.Height;
				}
				return contentSize.Width;
			}
			return Math.Max(this.OrientationVertical ? (contentSize.Width - base.LayoutPadding.Horizontal) : (contentSize.Height - base.LayoutPadding.Vertical), 0);
		}

		/// <summary>
		///   Get content size in a row direction.
		/// </summary>
		/// <param name="contentSize">size of a content area</param>
		/// <param name="customPadding">custom layout padding is used</param>
		/// <returns>content size in a row direction</returns>
		internal int GetContentSizeRow(Size contentSize, bool customPadding) {
			Checks.CheckSize(contentSize, "contentSize");
			if (customPadding) {
				if (!this.OrientationVertical) {
					return contentSize.Width;
				}
				return contentSize.Height;
			}
			if (!this.OrientationVertical) {
				return contentSize.Width - base.LayoutPadding.Horizontal;
			}
			return contentSize.Height - base.LayoutPadding.Vertical;
		}

		/// <summary>
		///   Get column size.
		/// </summary>
		/// <param name="element">reference layout element</param>
		/// <param name="customPadding">custom layout padding is used</param>
		/// <returns></returns>
		internal int GetColumnSize(TElement element, bool customPadding) {
			Checks.CheckNotNull(element, "element");
			int val = (customPadding ? (this.OrientationVertical ? ((IBetterListViewLayoutElementDisplayable)element).LayoutBounds.BoundsOuterExtended.Width : ((IBetterListViewLayoutElementDisplayable)element).LayoutBounds.BoundsOuterExtended.Height) : (this.OrientationVertical ? (((IBetterListViewLayoutElementDisplayable)element).LayoutBounds.BoundsOuterExtended.Width + base.ElementOuterPadding.Width) : (((IBetterListViewLayoutElementDisplayable)element).LayoutBounds.BoundsOuterExtended.Height + base.ElementOuterPadding.Height)));
			return Math.Max(val, 1);
		}

		/// <summary>
		///   Get number of layout elements per row.
		/// </summary>
		/// <param name="elementCount">total number of layout elements</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="columnSize">column size</param>
		/// <returns>number of layout elements per row</returns>
		internal int GetElementsPerRow(int elementCount, int contentSize, int columnSize) {
			Checks.CheckTrue(elementCount > 0, "elementCount > 0");
			Checks.CheckTrue(contentSize >= 0, "contentSize >= 0");
			Checks.CheckTrue(columnSize > 0, "columnSize > 0");
			if (contentSize == 0 || this.SingleItemPerRow) {
				return 1;
			}
			return Math.Min(Math.Max(contentSize / columnSize, 1), elementCount);
		}

		/// <summary>
		///   Set positions of the layout elements.
		/// </summary>
		/// <param name="elements">layout elements to position</param>
		/// <param name="indexElementFirst">index of the first layout element to position</param>
		/// <param name="indexElementLast">index of the last layout element to position</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="extraPadding">extra padding added to the layout</param>
		protected internal void PositionElements(ReadOnlyCollection<TElement> elements, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding) {
			this.PositionElements(elements, indexElementFirst, indexElementLast, contentSize, extraPadding, customPadding: false);
		}

		/// <summary>
		///   Set positions of the layout elements.
		/// </summary>
		/// <param name="elements">layout elements to position</param>
		/// <param name="indexElementFirst">index of the first layout element to position</param>
		/// <param name="indexElementLast">index of the last layout element to position</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="extraPadding">extra padding added to the layout</param>
		/// <param name="customPadding">do not automatically govern padding</param>
		protected internal void PositionElements(ReadOnlyCollection<TElement> elements, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding, bool customPadding) {
			if (elements.Count == 0 || contentSize.Width <= 0 || contentSize.Height <= 0) {
				return;
			}
			bool flag = (this.PositioningOptions & BetterListViewLayoutPositioningOptions.StretchSpacing) == BetterListViewLayoutPositioningOptions.StretchSpacing;
			bool flag2 = (this.PositioningOptions & BetterListViewLayoutPositioningOptions.CenterRow) == BetterListViewLayoutPositioningOptions.CenterRow;
			int contentSizeColumn = this.GetContentSizeColumn(contentSize, customPadding);
			if (contentSizeColumn <= 0) {
				return;
			}
			int num = this.GetColumnSize(elements[0], customPadding);
			int num5 = indexElementLast - indexElementFirst + 1;
			int elementsPerRow = this.GetElementsPerRow(num5, contentSizeColumn, num);
			int num6 = elementsPerRow * num;
			if (flag && num5 > elementsPerRow) {
				num += (contentSizeColumn - num6) / elementsPerRow;
				num6 = elementsPerRow * num;
			}
			int num7 = indexElementFirst;
			if (this.OrientationVertical) {
				int num8 = ((!customPadding) ? (base.LayoutPadding.Left + extraPadding.Width) : 0);
				if (flag2) {
					num8 += contentSizeColumn - num6 >> 1;
				}
				int num9 = num8;
				int num10 = ((!customPadding) ? (base.LayoutPadding.Top + extraPadding.Height) : 0);
				do {
					int num11 = 0;
					for (int i = 0; i < elementsPerRow && num7 + i <= indexElementLast; i++) {
						BetterListViewElementBoundsBase layoutBounds = ((IBetterListViewLayoutElementDisplayable)elements[num7 + i]).LayoutBounds;
						layoutBounds.Relocate(new Point(num9, num10));
						num9 += num;
						num11 = Math.Max(num11, layoutBounds.BoundsOuter.Height);
					}
					if (num7 + elementsPerRow <= indexElementLast) {
						num11 += base.ElementOuterPadding.Height;
					}
					for (int j = 0; j < elementsPerRow && num7 + j <= indexElementLast; j++) {
						BetterListViewElementBoundsBase layoutBounds2 = ((IBetterListViewLayoutElementDisplayable)elements[num7 + j]).LayoutBounds;
						layoutBounds2.BoundsSpacing = new Rectangle(layoutBounds2.BoundsOuter.Left, layoutBounds2.BoundsOuter.Top, num, num11);
					}
					num7 += elementsPerRow;
					num9 = num8;
					num10 += num11;
				}
				while (num7 <= indexElementLast);
				return;
			}
			int num12 = ((!customPadding) ? (base.LayoutPadding.Top + extraPadding.Height) : 0);
			if (flag2) {
				num12 += contentSizeColumn - num6 >> 1;
			}
			int num2 = ((!customPadding) ? (base.LayoutPadding.Left + extraPadding.Width) : 0);
			int num3 = num12;
			do {
				int num4 = 0;
				for (int k = 0; k < elementsPerRow && num7 + k <= indexElementLast; k++) {
					BetterListViewElementBoundsBase layoutBounds3 = ((IBetterListViewLayoutElementDisplayable)elements[num7 + k]).LayoutBounds;
					layoutBounds3.Relocate(new Point(num2, num3));
					num3 += num;
					num4 = Math.Max(num4, layoutBounds3.BoundsOuter.Width);
				}
				if (num7 + elementsPerRow <= indexElementLast) {
					num4 += base.ElementOuterPadding.Width;
				}
				for (int l = 0; l < elementsPerRow && num7 + l <= indexElementLast; l++) {
					BetterListViewElementBoundsBase layoutBounds4 = ((IBetterListViewLayoutElementDisplayable)elements[num7 + l]).LayoutBounds;
					layoutBounds4.BoundsSpacing = new Rectangle(layoutBounds4.BoundsOuter.Left, layoutBounds4.BoundsOuter.Top, num4, num);
				}
				num7 += elementsPerRow;
				num2 += num4;
				num3 = num12;
			}
			while (num7 <= indexElementLast);
		}

		/// <summary>
		///   Get indices of the first and the last visible layout elements.
		/// </summary>
		/// <param name="elements">layout elements</param>
		/// <param name="boundsContent">content area (relative coordinates)</param>
		/// <param name="offset">offset from absolute coordinates</param>
		/// <param name="indexElementFirst">index of the first currently visible layout element</param>
		/// <param name="indexElementLast">index of the last currently visible layout element</param>
		internal BetterListViewLayoutVisibleRange GetVisibleRange(ReadOnlyCollection<TElement> elements, Rectangle boundsContent, Point offset, int indexElementFirst, int indexElementLast) {
			Checks.CheckNotNull(elements, "elements");
			if (elements.Count != 0) {
				Checks.CheckBounds(indexElementFirst, 0, elements.Count - 1, "indexElementFirst");
				Checks.CheckBounds(indexElementLast, 0, elements.Count - 1, "indexElementLast");
				Checks.CheckTrue(indexElementFirst <= indexElementLast, "indexElementFirst <= indexElementLast");
			}
			else {
				Checks.CheckEqual(indexElementFirst, 0, "indexElementFirst", "0");
				Checks.CheckEqual(indexElementLast, 0, "indexElementLast", "0");
			}
			if (elements.Count == 0 || (boundsContent.Width <= 0 && boundsContent.Height <= 0)) {
				return BetterListViewLayoutVisibleRange.Undefined;
			}
			BetterListViewElementBoundsBase betterListViewElementBoundsBase = (BetterListViewElementBoundsBase)((IBetterListViewLayoutElementDisplayable)elements[indexElementFirst]).LayoutBounds.Clone();
			betterListViewElementBoundsBase.Offset(offset);
			if (this.OrientationVertical) {
				if (betterListViewElementBoundsBase.BoundsSpacing.Bottom <= boundsContent.Top) {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: false, directionIncremental: true, transitionToVisible: true, ref indexElementFirst);
				}
				else if (betterListViewElementBoundsBase.BoundsSpacing.Top >= boundsContent.Bottom) {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: false, directionIncremental: false, transitionToVisible: false, ref indexElementFirst);
				}
				else {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: true, directionIncremental: false, transitionToVisible: false, ref indexElementFirst);
				}
			}
			else if (betterListViewElementBoundsBase.BoundsSpacing.Right <= boundsContent.Left) {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: false, directionIncremental: true, transitionToVisible: true, ref indexElementFirst);
			}
			else if (betterListViewElementBoundsBase.BoundsSpacing.Left >= boundsContent.Right) {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: false, directionIncremental: false, transitionToVisible: false, ref indexElementFirst);
			}
			else {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: true, directionIncremental: false, transitionToVisible: false, ref indexElementFirst);
			}
			betterListViewElementBoundsBase = (BetterListViewElementBoundsBase)((IBetterListViewLayoutElementDisplayable)elements[indexElementLast]).LayoutBounds.Clone();
			betterListViewElementBoundsBase.Offset(offset);
			if (this.OrientationVertical) {
				if (betterListViewElementBoundsBase.BoundsSpacing.Bottom <= boundsContent.Top) {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: false, directionIncremental: true, transitionToVisible: false, ref indexElementLast);
				}
				else if (betterListViewElementBoundsBase.BoundsSpacing.Top >= boundsContent.Bottom) {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: false, directionIncremental: false, transitionToVisible: true, ref indexElementLast);
				}
				else {
					BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: false, visibleLast: true, directionIncremental: true, transitionToVisible: false, ref indexElementLast);
				}
			}
			else if (betterListViewElementBoundsBase.BoundsSpacing.Right <= boundsContent.Left) {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: false, directionIncremental: true, transitionToVisible: false, ref indexElementLast);
			}
			else if (betterListViewElementBoundsBase.BoundsSpacing.Left >= boundsContent.Right) {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: false, directionIncremental: false, transitionToVisible: true, ref indexElementLast);
			}
			else {
				BetterListViewLayoutCommon<TElement>.GetVisibleIndex(elements, boundsContent, offset, orientationHorizontal: true, visibleLast: true, directionIncremental: true, transitionToVisible: false, ref indexElementLast);
			}
			return new BetterListViewLayoutVisibleRange(indexElementFirst, indexElementLast);
		}

		private static void GetVisibleIndex(ReadOnlyCollection<TElement> elements, Rectangle boundsContent, Point offset, bool orientationHorizontal, bool visibleLast, bool directionIncremental, bool transitionToVisible, ref int index) {
			if (directionIncremental) {
				for (int i = index + 1; i < elements.Count; i++) {
					Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)elements[i]).LayoutBounds.BoundsSpacing;
					boundsSpacing.Offset(offset);
					bool flag = (orientationHorizontal ? (boundsSpacing.Right >= boundsContent.Left && boundsSpacing.Left <= boundsContent.Right) : (boundsSpacing.Bottom >= boundsContent.Top && boundsSpacing.Top <= boundsContent.Bottom));
					if (flag != visibleLast && flag == transitionToVisible) {
						index = i - 1;
						return;
					}
					visibleLast = flag;
				}
				index = elements.Count - 1;
				return;
			}
			for (int num = index - 1; num >= 0; num--) {
				Rectangle boundsSpacing2 = ((IBetterListViewLayoutElementDisplayable)elements[num]).LayoutBounds.BoundsSpacing;
				boundsSpacing2.Offset(offset);
				bool flag2 = boundsContent.IntersectsWith(boundsSpacing2);
				if (flag2 != visibleLast && flag2 == transitionToVisible) {
					index = num + 1;
					return;
				}
				visibleLast = flag2;
			}
			index = 0;
		}
	}
}