using System.Collections.Generic;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents information to make visible using EnsureVisible method.
	/// </summary>
	internal struct BetterListViewEnsureVisibleData
	{
		/// <summary>
		///   Empty BetterListViewEnsureVisibleData structure.
		/// </summary>
		public static readonly BetterListViewEnsureVisibleData Empty = new BetterListViewEnsureVisibleData(null, new BetterListViewItem[0], Rectangle.Empty);

		private readonly BetterListViewElementBase element;

		private readonly ICollection<BetterListViewItem> items;

		private Rectangle area;

		/// <summary>
		///   Element to scroll at.
		/// </summary>
		public BetterListViewElementBase Element => this.element;

		/// <summary>
		///   Items to scroll at.
		/// </summary>
		public ICollection<BetterListViewItem> Items => this.items;

		/// <summary>
		///   Area to scroll at.
		/// </summary>
		public Rectangle Area => this.area;

		/// <summary>
		///   This BetterListViewEnsureVisibleData structure is empty.
		/// </summary>
		public bool IsEmpty {
			get {
				if (this.element == null && this.items.Count == 0) {
					return this.area.IsEmpty;
				}
				return false;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewEnsureVisibleData" /> struct.
		/// </summary>
		/// <param name="element">Element to scroll at.</param>
		public BetterListViewEnsureVisibleData(BetterListViewElementBase element)
			: this(element, new BetterListViewItem[0], Rectangle.Empty) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewEnsureVisibleData" /> struct.
		/// </summary>
		/// <param name="items">Items to scroll at.</param>
		public BetterListViewEnsureVisibleData(ICollection<BetterListViewItem> items)
			: this(null, items, Rectangle.Empty) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewEnsureVisibleData" /> struct.
		/// </summary>
		/// <param name="area">Area to scroll at.</param>
		public BetterListViewEnsureVisibleData(Rectangle area)
			: this(null, new BetterListViewItemCollection(), area) {
		}

		private BetterListViewEnsureVisibleData(BetterListViewElementBase element, ICollection<BetterListViewItem> items, Rectangle area) {
			Checks.CheckNotNull(items, "items");
			if (element != null || items.Count != 0 || !area.IsEmpty) {
				if (element != null) {
					Checks.CheckTrue(items.Count == 0, "items.Count == 0");
					Checks.CheckTrue(area.IsEmpty, "area.IsEmpty");
				}
				else if (items.Count != 0) {
					Checks.CheckTrue(area.IsEmpty, "area.IsEmpty");
				}
			}
			this.element = element;
			this.items = items;
			this.area = area;
		}
	}
}