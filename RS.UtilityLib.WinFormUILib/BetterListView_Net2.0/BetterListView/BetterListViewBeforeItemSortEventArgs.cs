using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BeforeItemSort event.
	/// </summary>
	public class BetterListViewBeforeItemSortEventArgs : CancelEventArgs
	{
		private BetterListViewSortList sortList;

		private bool columnClicked;

		/// <summary>
		///   Gets or sets column sort information.
		/// </summary>
		/// <value>
		///   Column sort information.
		/// </value>
		public BetterListViewSortList SortList {
			get {
				return this.sortList;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.sortList = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether sorting was invoked from UI by clicking on a column.
		/// </summary>
		/// <value>
		///   <c>true</c> if sorting was invoked from UI by clicking on a column; otherwise, <c>false</c>.
		/// </value>
		public bool ColumnClicked {
			get {
				return this.columnClicked;
			}
			set {
				this.columnClicked = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewBeforeItemSortEventArgs" /> class.
		/// </summary>
		/// <param name="sortList">Column sort information.</param>
		/// <param name="columnClicked">Sorting was invoked from UI by clicking on a column.</param>
		public BetterListViewBeforeItemSortEventArgs(BetterListViewSortList sortList, bool columnClicked) {
			this.SortList = sortList;
			this.ColumnClicked = columnClicked;
		}
	}
}