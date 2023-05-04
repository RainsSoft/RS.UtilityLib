using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BeforeDrawBackground and BetterListView.DrawBackground events.
	/// </summary>
	public class BetterListViewDrawBackgroundEventArgs : EventArgs
	{
		private Graphics graphics;

		private Rectangle backgroundBounds;

		private BetterListViewColumnHeaderBounds sortedColumnHeaderBounds;

		private bool drawBackground = true;

		private bool drawSortedColumn = true;

		private bool drawImage = true;

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
		///   Gets or sets the background boundaries.
		/// </summary>
		/// <value>
		///   The background background boundaries.
		/// </value>
		public Rectangle BackgroundBounds {
			get {
				return this.backgroundBounds;
			}
			set {
				Checks.CheckTrue(value.Width >= 0 && value.Height >= 0, "value.Width >= 0 && value.Height >= 0");
				this.backgroundBounds = value;
			}
		}

		/// <summary>
		///   Gets or sets the sorted column header boundaries.
		/// </summary>
		/// <value>
		///   The sorted column header boundaries.
		/// </value>
		public BetterListViewColumnHeaderBounds SortedColumnHeaderBounds {
			get {
				return this.sortedColumnHeaderBounds;
			}
			set {
				this.sortedColumnHeaderBounds = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw background area.
		/// </summary>
		/// <value>
		///   <c>true</c> if background area should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawBackground {
			get {
				return this.drawBackground;
			}
			set {
				this.drawBackground = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw sorted column.
		/// </summary>
		/// <value>
		///   <c>true</c> if sorted column should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawSortedColumn {
			get {
				return this.drawSortedColumn;
			}
			set {
				this.drawSortedColumn = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw background image.
		/// </summary>
		/// <value>
		///   <c>true</c> if background image should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawImage {
			get {
				return this.drawImage;
			}
			set {
				this.drawImage = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawBackgroundEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="backgroundBounds">Background boundaries.</param>
		/// <param name="sortedColumnHeaderBounds">Sorted column header boundaries.</param>
		public BetterListViewDrawBackgroundEventArgs(Graphics graphics, Rectangle backgroundBounds, BetterListViewColumnHeaderBounds sortedColumnHeaderBounds) {
			this.Graphics = graphics;
			this.BackgroundBounds = backgroundBounds;
			this.SortedColumnHeaderBounds = sortedColumnHeaderBounds;
		}
	}
}