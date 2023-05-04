using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ColumnWidthChanging event.
	/// </summary>
	public class BetterListViewColumnWidthChangingEventArgs : CancelEventArgs
	{
		private BetterListViewColumnHeader columnHeader;

		private BetterListViewColumnWidthChangeSource columnWidthChangeSource;

		private int newWidth;

		/// <summary>
		///   Gets or sets the column header being resized..
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
		///   Gets or sets the new width of the column.
		/// </summary>
		/// <value>
		///   The new width of the column.
		/// </value>
		public int NewWidth {
			get {
				return this.newWidth;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.newWidth = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnWidthChangingEventArgs" /> class.
		/// </summary>
		/// <param name="columnHeader">Column header being resized.</param>
		/// <param name="columnWidthChangeSource">Reason for column width change.</param>
		/// <param name="newWidth">New width of the column header.</param>
		public BetterListViewColumnWidthChangingEventArgs(BetterListViewColumnHeader columnHeader, BetterListViewColumnWidthChangeSource columnWidthChangeSource, int newWidth) {
			this.ColumnHeader = columnHeader;
			this.ColumnWidthChangeSource = columnWidthChangeSource;
			this.NewWidth = newWidth;
		}
	}
}