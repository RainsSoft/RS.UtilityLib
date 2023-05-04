using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DragDropEffectSetting event.
	/// </summary>
	public class BetterListViewDragDropEffectSettingEventArgs : DragEventArgs
	{
		private bool isItemReorder;

		private bool updateInsertionMark;

		/// <summary>
		///   Gets or sets a value indicating whether item reordering is in progress.
		/// </summary>
		/// <value>
		///   <c>true</c> if item reordering is in progress; otherwise, <c>false</c>.
		/// </value>
		public bool IsItemReorder {
			get {
				return this.isItemReorder;
			}
			set {
				this.isItemReorder = value;
			}
		}

		/// <summary>
		///   Set InsertionMark property automatically when setting the drop effect.
		/// </summary>
		public bool UpdateInsertionMark {
			get {
				return this.updateInsertionMark;
			}
			set {
				this.updateInsertionMark = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDragDropEffectSettingEventArgs" /> class.
		/// </summary>
		/// <param name="data">The data associated with this event.</param>
		/// <param name="keyState">The current state of the SHIFT, CTRL, and ALT keys.</param>
		/// <param name="x">The x-coordinate of the mouse cursor in pixels.</param>
		/// <param name="y">The y-coordinate of the mouse cursor in pixels.</param>
		/// <param name="allowedEffect">Allowed System.Windows.Forms.DragDropEffects values.</param>
		/// <param name="effect">One of the System.Windows.Forms.DragDropEffects values.</param>
		/// <param name="isItemReorder">Item reordering is in progress.</param>
		/// <param name="updateInsertionMark">Set InsertionMark property automatically when setting the drop effect.</param>
		public BetterListViewDragDropEffectSettingEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect, bool isItemReorder, bool updateInsertionMark)
			: base(data, keyState, x, y, allowedEffect, effect) {
			this.IsItemReorder = isItemReorder;
			this.UpdateInsertionMark = updateInsertionMark;
		}
	}
}