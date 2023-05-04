using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DragDropException event.
	/// </summary>
	public class BetterListViewDragDropExceptionEventArgs : EventArgs
	{
		private Exception exception;

		private bool showExceptionDialog = true;

		/// <summary>
		///   Gets or sets the exception thrown during Drag and Drop operation.
		/// </summary>
		/// <value>
		///   The exception thrown during Drag and Drop operation.
		/// </value>
		public Exception Exception {
			get {
				return this.exception;
			}
			set {
				this.exception = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether show error dialog.
		/// </summary>
		/// <value>
		///   <c>true</c> if show the error dialog; otherwise, <c>false</c>.
		/// </value>
		public bool ShowExceptionDialog {
			get {
				return this.showExceptionDialog;
			}
			set {
				this.showExceptionDialog = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDragDropExceptionEventArgs" /> class.
		/// </summary>
		/// <param name="exception">Exception thrown during Drag and Drop operation.</param>
		public BetterListViewDragDropExceptionEventArgs(Exception exception) {
			this.Exception = exception;
		}
	}
}