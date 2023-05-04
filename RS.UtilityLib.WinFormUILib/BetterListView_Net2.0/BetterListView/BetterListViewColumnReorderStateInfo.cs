using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the ColumnReorder state.
	/// </summary>
	internal struct BetterListViewColumnReorderStateInfo : IDisposable
	{
		/// <summary>
		///   empty BetterListViewColumnReorderStateInfo structure
		/// </summary>
		public static readonly BetterListViewColumnReorderStateInfo Empty = new BetterListViewColumnReorderStateInfo(Point.Empty, 0, null, null);

		private Point startPoint;

		private int scrollPositionHorizontal;

		private BetterListViewColumnHeader column;

		private Bitmap columnBitmap;

		/// <summary>
		///   this BetterListViewColumnReorderStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewColumnReorderStateInfo.Empty);

		/// <summary>
		///   mouse cursor position
		/// </summary>
		public Point StartPoint {
			get {
				return this.startPoint;
			}
			set {
				Checks.CheckTrue(value.X >= 0 && value.Y >= 0, "value.X >= 0 && value.Y >= 0");
				this.startPoint = value;
			}
		}

		/// <summary>
		///   position of the horizontal scroll bar
		/// </summary>
		public int ScrollPositionHorizontal {
			get {
				return this.scrollPositionHorizontal;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.scrollPositionHorizontal = value;
			}
		}

		/// <summary>
		///   column header being reordered
		/// </summary>
		public BetterListViewColumnHeader Column {
			get {
				return this.column;
			}
			set {
				this.column = value;
			}
		}

		/// <summary>
		///   image of the column header being reordered
		/// </summary>
		public Bitmap ColumnBitmap {
			get {
				return this.columnBitmap;
			}
			set {
				this.columnBitmap = value;
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewColumnReoderStateInfo instance.
		/// </summary>
		/// <param name="startPoint">mouse cursor position</param>
		/// <param name="scrollPositionHorizontal">position of the horizontal scroll bar</param>
		/// <param name="column">column header being reordered</param>
		/// <param name="columnBitmap">image of the column header being reordered</param>
		public BetterListViewColumnReorderStateInfo(Point startPoint, int scrollPositionHorizontal, BetterListViewColumnHeader column, Bitmap columnBitmap) {
			Checks.CheckTrue(startPoint.X >= 0 && startPoint.Y >= 0, "startPoint.X >= 0 && startPoint.Y >= 0");
			Checks.CheckTrue(scrollPositionHorizontal >= 0, "scrollPositionHorizontal >= 0");
			this.startPoint = startPoint;
			this.scrollPositionHorizontal = scrollPositionHorizontal;
			this.column = column;
			this.columnBitmap = columnBitmap;
		}

		public static bool operator ==(BetterListViewColumnReorderStateInfo columnReorderStateInfoA, BetterListViewColumnReorderStateInfo columnReorderStateInfoB) {
			return columnReorderStateInfoA.Equals(columnReorderStateInfoB);
		}

		public static bool operator !=(BetterListViewColumnReorderStateInfo columnReorderStateInfoA, BetterListViewColumnReorderStateInfo columnReorderStateInfoB) {
			return !columnReorderStateInfoA.Equals(columnReorderStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewColumnReorderStateInfo betterListViewColumnReorderStateInfo)) {
				return false;
			}
			if (this.startPoint == betterListViewColumnReorderStateInfo.startPoint && this.scrollPositionHorizontal == betterListViewColumnReorderStateInfo.scrollPositionHorizontal && this.column == betterListViewColumnReorderStateInfo.column) {
				return this.columnBitmap == betterListViewColumnReorderStateInfo.columnBitmap;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.startPoint.GetHashCode() ^ this.scrollPositionHorizontal.GetHashCode() ^ ((this.column != null) ? this.column.GetHashCode() : 0) ^ ((this.columnBitmap != null) ? this.columnBitmap.GetHashCode() : 0);
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{StartPoint = '{this.startPoint}', ScrollPositionHorizontal = '{this.scrollPositionHorizontal}', Column = '{this.column}'}}";
		}

		public void Dispose() {
			if (this.columnBitmap != null) {
				this.columnBitmap.Dispose();
			}
		}
	}
}