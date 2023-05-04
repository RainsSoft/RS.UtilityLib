using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common layout for LargeIcon-like views.
	/// </summary>
	internal abstract class BetterListViewLayoutItemsLargeIconBase : BetterListViewLayoutItems
	{
		public override bool OrientationVertical => true;

		public override bool AllowsExpandableItems => false;

		public override bool DirectionVertical => false;

		public override bool SingleBoundary => false;

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

		protected override TextAlignmentHorizontal DefaultDefaultTextAlignmentHorizontal => TextAlignmentHorizontal.Center;

		protected override TextAlignmentVertical DefaultDefaultTextAlignmentVertical => TextAlignmentVertical.Middle;

		protected override TextTrimming DefaultDefaultTextTrimming => TextTrimming.EllipsisCharacter;

		protected override Padding DefaultLayoutPadding => new Padding(2);

		protected override Size DefaultElementOuterPadding => new Size(2, 2);

		protected override Padding DefaultElementInnerPadding => new Padding(2);

		protected override Size DefaultEmptyTextSize => new Size(48, 16);

		protected override Padding DefaultTextPadding => new Padding(2);

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsLargeIconBase instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		protected BetterListViewLayoutItemsLargeIconBase(BetterListView listView)
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
			int num = base.ItemWidth;
			BetterListViewImageSize imageSize = base.ImageSize;
			if (!imageSize.IsEmpty) {
				num = Math.Max(num, imageSize.MaximumSize.Width + base.ImagePadding.Horizontal);
			}
			return new BetterListViewCommonMeasurementItems(Size.Empty, sizeCheckBoxCheck, sizeCheckBoxRadio, num, 0);
		}
	}
}