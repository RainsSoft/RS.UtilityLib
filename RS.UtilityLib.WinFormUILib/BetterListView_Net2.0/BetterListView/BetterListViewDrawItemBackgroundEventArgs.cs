using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DrawItemBackground events.
	/// </summary>
	public class BetterListViewDrawItemBackgroundEventArgs : EventArgs
	{
		private Graphics graphics;

		private BetterListViewItem item;

		private BetterListViewItemBounds itemBounds;

		/// <summary>
		///   Gets or sets the Graphics object used for drawing.
		/// </summary>
		/// <value>
		///   The Graphics object used for drawing.
		/// </value>
		public Graphics Graphics {
			get {
				return this.graphics;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.graphics = value;
			}
		}

		/// <summary>
		///   Gets or sets the item to draw.
		/// </summary>
		/// <value>
		///   The item to draw.
		/// </value>
		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.item = value;
			}
		}

		/// <summary>
		///   Gets or sets the item boundaries.
		/// </summary>
		/// <value>
		///   The item boundaries.
		/// </value>
		public BetterListViewItemBounds ItemBounds {
			get {
				return this.itemBounds;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.itemBounds = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawItemBackgroundEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="item">Item to draw.</param>
		/// <param name="itemBounds">Item boundaries.</param>
		public BetterListViewDrawItemBackgroundEventArgs(Graphics graphics, BetterListViewItem item, BetterListViewItemBounds itemBounds) {
			this.Graphics = graphics;
			this.Item = item;
			this.ItemBounds = itemBounds;
		}
	}
}