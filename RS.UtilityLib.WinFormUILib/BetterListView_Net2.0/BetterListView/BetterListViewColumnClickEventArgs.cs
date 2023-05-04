using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ColumnClick event.
	/// </summary>
	public class BetterListViewColumnClickEventArgs : CancelEventArgs
	{
		private BetterListViewColumnHeader columnHeader;

		/// <summary>
		///   Gets or sets the column header that was clicked.
		/// </summary>
		/// <value>
		///   The column header that was clicked.
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
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnClickEventArgs" /> class.
		/// </summary>
		/// <param name="columnHeader">Column header that was clicked.</param>
		public BetterListViewColumnClickEventArgs(BetterListViewColumnHeader columnHeader) {
			this.ColumnHeader = columnHeader;
		}
	}
}