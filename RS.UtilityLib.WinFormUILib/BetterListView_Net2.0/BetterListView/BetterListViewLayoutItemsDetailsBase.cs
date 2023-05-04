using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Layout for Details view.
	/// </summary>
	internal abstract class BetterListViewLayoutItemsDetailsBase : BetterListViewLayoutItems
	{
		public override bool OrientationVertical => true;

		public override bool AllowsExpandableItems => true;

		public override bool DirectionVertical => true;

		public override bool SingleBoundary => base.ElementOuterPadding.Height == 0;

		/// <summary>
		///   layout positioning options
		/// </summary>
		internal override BetterListViewLayoutPositioningOptions PositioningOptions => BetterListViewLayoutPositioningOptions.None;

		/// <summary>
		///   each row contains just a single item, even if there is enough space on the row for more items
		/// </summary>
		protected override bool SingleItemPerRow => true;

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

		protected override Size DefaultElementOuterPadding => Size.Empty;

		protected override Padding DefaultElementInnerPadding => new Padding(2, 2, 2, 1);

		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(Size.Empty, new Size(64, 64));

		protected override Padding DefaultImagePadding => new Padding(4);

		protected override ImageBorderType DefaultImageBorderType => ImageBorderType.None;

		protected override int DefaultImageBorderThickness => 4;

		protected override Size DefaultEmptyTextSize => new Size(48, 16);

		protected override Padding DefaultTextPadding => new Padding(2);

		protected override bool DefaultCenter => false;

		protected override int DefaultItemWidth => 512;

		protected override int DefaultItemHeight => 0;

		protected override int DefaultMaximumTextLines => 1;

		/// <summary>
		///   Initialize a new BetterListViewLayoutItemsDetailsBase instance.
		/// </summary>
		/// <param name="listView">Better ListView using this layout</param>
		protected BetterListViewLayoutItemsDetailsBase(BetterListView listView)
			: base(listView) {
		}

		internal override void PositionElements(ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding) {
			this.PositionElements(elementsItems, columnHeaderBoundsFirst, indexElementFirst, indexElementLast, contentSize, extraPadding, customPadding: false);
		}

		/// <summary>
		///   Positions the elements.
		/// </summary>
		/// <param name="layoutElementsItems">The layout elements items.</param>
		/// <param name="columnHeaderBoundsFirst">The column header bounds first.</param>
		/// <param name="indexElementFirst">The index element first.</param>
		/// <param name="indexElementLast">The index element last.</param>
		/// <param name="contentSize">Size of the content.</param>
		/// <param name="extraPadding">The extra padding.</param>
		/// <param name="customPadding">if set to <c>true</c> [custom padding].</param>
		internal override void PositionElements(ReadOnlyCollection<BetterListViewItem> layoutElementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, int indexElementFirst, int indexElementLast, Size contentSize, Size extraPadding, bool customPadding) {
			int num = base.LayoutPadding.Top + extraPadding.Height;
			for (int i = indexElementFirst; i <= indexElementLast; i++) {
				BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)layoutElementsItems[i]).LayoutBounds;
				if (i < indexElementLast) {
					betterListViewItemBounds.BoundsSpacing = new Rectangle(betterListViewItemBounds.BoundsOuter.Left, betterListViewItemBounds.BoundsOuter.Top, betterListViewItemBounds.BoundsOuter.Width + base.ElementOuterPadding.Width, betterListViewItemBounds.BoundsOuter.Height + base.ElementOuterPadding.Height);
				}
				else {
					betterListViewItemBounds.BoundsSpacing = new Rectangle(betterListViewItemBounds.BoundsOuter.Left, betterListViewItemBounds.BoundsOuter.Top, betterListViewItemBounds.BoundsOuter.Width + base.ElementOuterPadding.Width, betterListViewItemBounds.BoundsOuter.Height);
				}
				betterListViewItemBounds.Relocate(new Point(betterListViewItemBounds.BoundsOuter.Left, num));
				num += betterListViewItemBounds.BoundsSpacing.Height;
			}
		}
	}
}