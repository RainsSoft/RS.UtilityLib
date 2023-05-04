using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ColumnClicked event.
	/// </summary>
	public class BetterListViewColumnClickedEventArgs : EventArgs
	{
		private BetterListViewColumnHeader columnHeader;

		/// <summary>
		///   Gets or sets the column header being clicked.
		/// </summary>
		/// <value>
		///   The column header being clicked.
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
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnClickedEventArgs" /> class.
		/// </summary>
		/// <param name="columnHeader">Column header that was clicked.</param>
		public BetterListViewColumnClickedEventArgs(BetterListViewColumnHeader columnHeader) {
			this.ColumnHeader = columnHeader;
		}
	}
}