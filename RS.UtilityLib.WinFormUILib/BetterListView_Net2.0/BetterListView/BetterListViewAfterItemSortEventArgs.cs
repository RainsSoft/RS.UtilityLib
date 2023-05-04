using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.AfterItemSort event.
	/// </summary>
	public class BetterListViewAfterItemSortEventArgs : EventArgs
	{
		private bool columnClicked;

		private BetterListViewSortList sortList;

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
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAfterItemSortEventArgs" /> class.
		/// </summary>
		/// <param name="columnClicked">Sorting was invoked from UI by clicking on a column.</param>
		/// <param name="sortList">Column sort information.</param>
		public BetterListViewAfterItemSortEventArgs(bool columnClicked, BetterListViewSortList sortList) {
			this.ColumnClicked = columnClicked;
			this.SortList = sortList;
		}
	}
}