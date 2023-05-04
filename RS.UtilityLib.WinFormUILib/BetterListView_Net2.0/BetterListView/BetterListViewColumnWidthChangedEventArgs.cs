using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ColumnWidthChanged event.
	/// </summary>
	public class BetterListViewColumnWidthChangedEventArgs : EventArgs
	{
		private BetterListViewColumnHeader columnHeader;

		private BetterListViewColumnWidthChangeSource columnWidthChangeSource;

		/// <summary>
		///   Gets or sets the column header being resized.
		/// </summary>
		/// <value>
		///   The column header being resized.
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
		///   Gets or sets the reason for column width change.
		/// </summary>
		/// <value>
		///   The reason for column width change.
		/// </value>
		public BetterListViewColumnWidthChangeSource ColumnWidthChangeSource {
			get {
				return this.columnWidthChangeSource;
			}
			set {
				this.columnWidthChangeSource = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnWidthChangedEventArgs" /> class.
		/// </summary>
		/// <param name="columnHeader">Column header being resized.</param>
		/// <param name="columnWidthChangeSource">Reason for column width change.</param>
		public BetterListViewColumnWidthChangedEventArgs(BetterListViewColumnHeader columnHeader, BetterListViewColumnWidthChangeSource columnWidthChangeSource) {
			this.ColumnHeader = columnHeader;
			this.ColumnWidthChangeSource = columnWidthChangeSource;
		}
	}
}