using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BeforeDrawColumnHeaderBackground and BetterListView.DrawColumnHeaderBackground events.
	/// </summary>
	public class BetterListViewDrawColumnHeaderBackgroundEventArgs : EventArgs
	{
		private Graphics graphics;

		private BetterListViewColumnHeader columnHeader;

		private BetterListViewColumnHeaderBounds columnHeaderBounds;

		private BetterListViewColumnHeaderStateInfo columnHeaderStateInfo;

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
		///   Gets or sets the column header to draw.
		/// </summary>
		/// <value>
		///   The column header to draw.
		/// </value>
		public BetterListViewColumnHeader ColumnHeader {
			get {
				return this.columnHeader;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.columnHeader = value;
			}
		}

		/// <summary>
		///   Gets or sets the column header boundaries.
		/// </summary>
		/// <value>
		///   The column header boundaries.
		/// </value>
		public BetterListViewColumnHeaderBounds ColumnHeaderBounds {
			get {
				return this.columnHeaderBounds;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.columnHeaderBounds = value;
			}
		}

		/// <summary>
		///   Gets or sets the column header state information.
		/// </summary>
		/// <value>
		///   The column header state information.
		/// </value>
		public BetterListViewColumnHeaderStateInfo ColumnHeaderStateInfo {
			get {
				return this.columnHeaderStateInfo;
			}
			set {
				this.columnHeaderStateInfo = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawColumnHeaderBackgroundEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="columnHeader">Column header to draw.</param>
		/// <param name="columnHeaderBounds">Column header boundaries.</param>
		/// <param name="columnHeaderStateInfo">Column header state information.</param>
		public BetterListViewDrawColumnHeaderBackgroundEventArgs(Graphics graphics, BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderBounds columnHeaderBounds, BetterListViewColumnHeaderStateInfo columnHeaderStateInfo) {
			this.Graphics = graphics;
			this.ColumnHeader = columnHeader;
			this.ColumnHeaderBounds = columnHeaderBounds;
			this.ColumnHeaderStateInfo = columnHeaderStateInfo;
		}
	}
}