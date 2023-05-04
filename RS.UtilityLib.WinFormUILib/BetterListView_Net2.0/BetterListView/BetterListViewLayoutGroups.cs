using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common groups layout.
	/// </summary>
	public abstract class BetterListViewLayoutGroups : BetterListViewLayout<BetterListViewGroup>
	{
		private const int DefaultMinimumTextHeight = 24;

		private static readonly Size DefaultMinimumSize = new Size(128, 32);

		private Size minimumSize;

		private BetterListViewImageSize imageSize;

		private int minimumTextHeight;

		/// <summary>
		///   layout allows expand buttons on groups
		/// </summary>
		public abstract bool AllowsExpandableGroups { get; }

		/// <summary>
		///   minimum group size
		/// </summary>
		public Size MinimumSize {
			get {
				return this.minimumSize;
			}
			set {
				Checks.CheckSize(value, "value");
				if (!(this.minimumSize == value)) {
					this.minimumSize = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   group image area size
		/// </summary>
		public BetterListViewImageSize ImageSize {
			get {
				return this.imageSize;
			}
			set {
				if (!(this.imageSize == value)) {
					this.imageSize = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   minimum group text area height
		/// </summary>
		public int MinimumTextHeight {
			get {
				return this.minimumTextHeight;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				if (this.minimumTextHeight != value) {
					this.minimumTextHeight = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   layout positioning options
		/// </summary>
		internal override BetterListViewLayoutPositioningOptions PositioningOptions => BetterListViewLayoutPositioningOptions.None;

		/// <summary>
		///   each row contains just a single item, even if there is enough space on the row for more items
		/// </summary>
		protected override bool SingleItemPerRow => true;

		/// <summary>
		///   default layout padding
		/// </summary>
		protected override Padding DefaultLayoutPadding => new Padding(2);

		/// <summary>
		///   default outer padding of a layout element
		/// </summary>
		protected override Size DefaultElementOuterPadding => new Size(3, 3);

		/// <summary>
		///   default layout element inner padding
		/// </summary>
		protected override Padding DefaultElementInnerPadding => new Padding(2);

		/// <summary>
		///   default layout element image size
		/// </summary>
		protected override BetterListViewImageSize DefaultImageSize => new BetterListViewImageSize(new Size(24, 24), new Size(64, 64));

		/// <summary>
		///   default layout element image padding
		/// </summary>
		protected override Padding DefaultImagePadding => new Padding(4);

		/// <summary>
		///   default layout element text padding
		/// </summary>
		protected override Padding DefaultTextPadding => new Padding(2);

		/// <summary>
		///   default horizontal element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentHorizontal DefaultDefaultImageAlignmentHorizontal => BetterListViewImageAlignmentHorizontal.BeforeTextCenter;

		/// <summary>
		///   default vertical element image alignment
		/// </summary>
		protected override BetterListViewImageAlignmentVertical DefaultDefaultImageAlignmentVertical => BetterListViewImageAlignmentVertical.Middle;

		/// <summary>
		///   default horizontal element text alignment
		/// </summary>
		protected override TextAlignmentHorizontal DefaultDefaultTextAlignmentHorizontal => TextAlignmentHorizontal.Left;

		/// <summary>
		///   default vertical element text alignment
		/// </summary>
		protected override TextAlignmentVertical DefaultDefaultTextAlignmentVertical => TextAlignmentVertical.Middle;

		/// <summary>
		///   default item/sub-item text trimming
		/// </summary>
		protected override TextTrimming DefaultDefaultTextTrimming => TextTrimming.EllipsisCharacter;

		internal abstract void AdjustElements(ReadOnlyCollection<BetterListViewGroup> elementsGroups, Size contentSize, Graphics graphics, ImageList imageList, bool enableExpandButtons, BetterListViewGroup invisibleHeaderGroup);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutGroups" /> class.
		/// </summary>
		/// <param name="listView">control containing the layout</param>
		protected internal BetterListViewLayoutGroups(BetterListView listView)
			: base(listView) {
		}

		internal abstract void CheckOverflows(ReadOnlyCollection<BetterListViewGroup> elementsGroups, ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewLayoutItems layoutItems, Size contentSize, ref bool overflowsHorizontal, ref bool overflowsVertical);

		internal abstract BetterListViewInvalidationInfo GetResizeInvalidationInfo(BetterListViewInvalidationInfo itemResizeInvalidationInfo);

		/// <summary>
		///   Get common measurement information for group layout elements.
		/// </summary>
		/// <param name="elementsGroups">layout elements to measure</param>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <param name="contentSize">content area size</param>
		/// <param name="enableExpandButtons">enable showing expand buttons</param>
		/// <returns>common measurement information for group layout elements</returns>
		internal abstract BetterListViewCommonMeasurementGroups MeasureElements(ICollection<BetterListViewGroup> elementsGroups, Graphics graphics, Size contentSize, bool enableExpandButtons);

		/// <summary>
		///   Measure group layout elements.
		/// </summary>
		/// <param name="group">group layout element</param>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <param name="commonMeasurement">common group layout element measurement information</param>
		/// <param name="imageList">ImageList for groups</param>
		/// <param name="showHeader">group header should be visible</param>
		internal abstract void MeasureElement(BetterListViewGroup group, Graphics graphics, BetterListViewCommonMeasurementGroups commonMeasurement, ImageList imageList, bool showHeader);

		internal abstract void PositionElements(ReadOnlyCollection<BetterListViewGroup> elementsGroups, ReadOnlyCollection<BetterListViewItem> elementsItems, BetterListViewColumnHeaderBounds columnHeaderBoundsFirst, Size contentSize, BetterListViewLayoutItems layoutItems);

		internal abstract int GetGroupContentOffset(BetterListViewGroup group);

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected override void SetDefaultsInternal() {
			this.minimumSize = BetterListViewLayoutGroups.DefaultMinimumSize;
			this.imageSize = this.DefaultImageSize;
			this.minimumTextHeight = 24;
			base.SetDefaultsInternal();
		}
	}
}