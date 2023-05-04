using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DrawGroup events.
	/// </summary>
	public class BetterListViewDrawInsertionMarkEventArgs : EventArgs
	{
		private Graphics graphics;

		private BetterListViewInsertionLocation insertionLocation;

		private Point location;

		private int length;

		private bool isHorizontal;

		private Color color;

		private bool isEnabled;

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
		/// Logical location of the insertion mark.
		/// </summary>
		public BetterListViewInsertionLocation InsertionLocation {
			get {
				return this.insertionLocation;
			}
			set {
				this.insertionLocation = value;
			}
		}

		/// <summary>
		/// Exact location of the insertion mark.
		/// </summary>
		public Point Location {
			get {
				return this.location;
			}
			set {
				this.location = value;
			}
		}

		/// <summary>
		/// Exact length of the insertion mark.
		/// </summary>
		public int Length {
			get {
				return this.length;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.length = value;
			}
		}

		/// <summary>
		/// The insertion mark is drawn horizontally.
		/// </summary>
		public bool IsHorizontal {
			get {
				return this.isHorizontal;
			}
			set {
				this.isHorizontal = value;
			}
		}

		/// <summary>
		/// Actual color of the insertion mark used.
		/// </summary>
		public Color Color {
			get {
				return this.color;
			}
			set {
				Checks.CheckFalse(value.IsEmpty, "value.IsEmpty");
				this.color = value;
			}
		}

		/// <summary>
		/// Draw insertion mark in enabled state.
		/// </summary>
		public bool IsEnabled {
			get {
				return this.isEnabled;
			}
			set {
				this.isEnabled = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawInsertionMarkEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="insertionLocation">Logical locatin of the insertion mark.</param>
		/// <param name="location">Exact location of the insertion mark.</param>
		/// <param name="length">Exact length of the insertion mark.</param>
		/// <param name="isHorizontal">The insertion mark is drawn horizontally.</param>
		/// <param name="color">Actual color of the insertion mark used.</param>
		/// <param name="isEnabled">Draw insertion mark in enabled state.</param>
		public BetterListViewDrawInsertionMarkEventArgs(Graphics graphics, BetterListViewInsertionLocation insertionLocation, Point location, int length, bool isHorizontal, Color color, bool isEnabled) {
			this.Graphics = graphics;
			this.InsertionLocation = insertionLocation;
			this.Location = location;
			this.Length = length;
			this.IsHorizontal = isHorizontal;
			this.Color = color;
			this.IsEnabled = isEnabled;
		}
	}
}