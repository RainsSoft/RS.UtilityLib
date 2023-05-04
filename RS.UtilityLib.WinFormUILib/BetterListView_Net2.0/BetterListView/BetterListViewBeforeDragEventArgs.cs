using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BeforeDrag event.
	/// </summary>
	public class BetterListViewBeforeDragEventArgs : DragEventArgs
	{
		private bool cancel;

		/// <summary>
		///   Gets or sets a value indicating whether to cancel the Drag and Drop operation.
		/// </summary>
		/// <value>
		///   <c>true</c> if cancel the Drag and Drop operation; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel {
			get {
				return this.cancel;
			}
			set {
				this.cancel = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewBeforeDragEventArgs" /> class.
		/// </summary>
		/// <param name="data">The data associated with this event.</param>
		/// <param name="keyState">The current state of the SHIFT, CTRL, and ALT keys.</param>
		/// <param name="x">The x-coordinate of the mouse cursor in pixels.</param>
		/// <param name="y">The y-coordinate of the mouse cursor in pixels.</param>
		/// <param name="allowedEffect">Allowed System.Windows.Forms.DragDropEffects values.</param>
		/// <param name="effect">One of the System.Windows.Forms.DragDropEffects values.</param>
		public BetterListViewBeforeDragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
			: base(data, keyState, x, y, allowedEffect, effect) {
		}
	}
}