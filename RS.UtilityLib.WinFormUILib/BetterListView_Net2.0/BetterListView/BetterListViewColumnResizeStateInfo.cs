using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for ColumnBeforeResize and ColumnResize states.
	/// </summary>
	internal struct BetterListViewColumnResizeStateInfo
	{
		/// <summary>
		///   empty BetterListViewColumnResizeStateInfo structure
		/// </summary>
		public static readonly BetterListViewColumnResizeStateInfo Empty = new BetterListViewColumnResizeStateInfo(Point.Empty, null, 0, 0, isSmooth: false);

		private Point startPoint;

		private BetterListViewColumnHeader column;

		private int columnWidthOriginal;

		private int columnWidthNew;

		private bool isSmooth;

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
		///   original column header width
		/// </summary>
		public int ColumnWidthOriginal {
			get {
				return this.columnWidthOriginal;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.columnWidthOriginal = value;
			}
		}

		/// <summary>
		///   new column header width
		/// </summary>
		public int ColumnWidthNew {
			get {
				return this.columnWidthNew;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.columnWidthNew = value;
			}
		}

		/// <summary>
		///   column header resizing is done dynamically
		/// </summary>
		public bool IsSmooth {
			get {
				return this.isSmooth;
			}
			set {
				this.isSmooth = value;
			}
		}

		/// <summary>
		///   this BetterListViewColumnResizeStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewColumnResizeStateInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewColumnResizeStateInfo instance.
		/// </summary>
		/// <param name="startPoint">mouse cursor position</param>
		/// <param name="column">column header being reordered</param>
		/// <param name="columnWidthOriginal">original column header width</param>
		/// <param name="columnWidthNew">new column header width</param>
		/// <param name="isSmooth">column header resizing is done dynamically</param>
		public BetterListViewColumnResizeStateInfo(Point startPoint, BetterListViewColumnHeader column, int columnWidthOriginal, int columnWidthNew, bool isSmooth) {
			Checks.CheckTrue(startPoint.X >= 0 && startPoint.Y >= 0, "startPoint.X >= 0 && startPoint.Y >= 0");
			Checks.CheckTrue(columnWidthOriginal >= 0, "ColumnWidthOriginal >= 0");
			Checks.CheckTrue(columnWidthNew >= 0, "columnWidthNew >= 0");
			this.startPoint = startPoint;
			this.column = column;
			this.columnWidthOriginal = columnWidthOriginal;
			this.columnWidthNew = columnWidthNew;
			this.isSmooth = isSmooth;
		}

		public static bool operator ==(BetterListViewColumnResizeStateInfo columnResizeStateInfoA, BetterListViewColumnResizeStateInfo columnResizeStateInfoB) {
			return columnResizeStateInfoA.Equals(columnResizeStateInfoB);
		}

		public static bool operator !=(BetterListViewColumnResizeStateInfo columnResizeStateInfoA, BetterListViewColumnResizeStateInfo columnResizeStateInfoB) {
			return !columnResizeStateInfoA.Equals(columnResizeStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewColumnResizeStateInfo betterListViewColumnResizeStateInfo)) {
				return false;
			}
			if (this.startPoint == betterListViewColumnResizeStateInfo.startPoint && this.column == betterListViewColumnResizeStateInfo.column && this.columnWidthOriginal == betterListViewColumnResizeStateInfo.columnWidthOriginal && this.columnWidthNew == betterListViewColumnResizeStateInfo.columnWidthNew) {
				return this.isSmooth == betterListViewColumnResizeStateInfo.isSmooth;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.startPoint.GetHashCode() ^ ((this.column != null) ? this.column.GetHashCode() : 0) ^ this.columnWidthOriginal.GetHashCode() ^ this.columnWidthNew.GetHashCode() ^ this.isSmooth.GetHashCode();
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{StartPoint = '{this.startPoint}', Column = '{this.column}', ColumnWidthOriginal = '{this.columnWidthOriginal}', ColumnWidthNew = '{this.columnWidthNew}', IsSmooth = '{this.isSmooth}'}}";
		}
	}
}