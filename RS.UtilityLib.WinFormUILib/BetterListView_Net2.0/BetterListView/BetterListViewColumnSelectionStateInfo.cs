using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the ColumnSelection state.
	/// </summary>
	internal struct BetterListViewColumnSelectionStateInfo
	{
		/// <summary>
		///   empty BetterListViewColumnSelectionStateInfo structure
		/// </summary>
		public static readonly BetterListViewColumnSelectionStateInfo Empty = new BetterListViewColumnSelectionStateInfo(Point.Empty, null);

		private Point startPoint;

		private BetterListViewColumnHeader column;

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
		///   selected column header
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
		///   this BetterListViewColumnSelectionStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewColumnSelectionStateInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewColumnSelectionStateInfo instance.
		/// </summary>
		/// <param name="startPoint">mouse cursor position</param>
		/// <param name="column">selected column header</param>
		public BetterListViewColumnSelectionStateInfo(Point startPoint, BetterListViewColumnHeader column) {
			Checks.CheckTrue(startPoint.X >= 0 && startPoint.Y >= 0, "startPoint.X >= 0 && startPoint.Y >= 0");
			this.startPoint = startPoint;
			this.column = column;
		}

		public static bool operator ==(BetterListViewColumnSelectionStateInfo columnSelectionStateInfoA, BetterListViewColumnSelectionStateInfo columnSelectionStateInfoB) {
			return columnSelectionStateInfoA.Equals(columnSelectionStateInfoB);
		}

		public static bool operator !=(BetterListViewColumnSelectionStateInfo columnSelectionStateInfoA, BetterListViewColumnSelectionStateInfo columnSelectionStateInfoB) {
			return !columnSelectionStateInfoA.Equals(columnSelectionStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewColumnSelectionStateInfo betterListViewColumnSelectionStateInfo)) {
				return false;
			}
			if (this.startPoint == betterListViewColumnSelectionStateInfo.startPoint) {
				return this.column == betterListViewColumnSelectionStateInfo.column;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.startPoint.GetHashCode() ^ ((this.column != null) ? this.column.GetHashCode() : 0);
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{StartPoint = '{this.startPoint}', Column = '{this.column}'}}";
		}
	}
}