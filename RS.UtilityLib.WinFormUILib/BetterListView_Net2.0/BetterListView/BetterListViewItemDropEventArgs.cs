using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemDrop event.
	/// </summary>
	public class BetterListViewItemDropEventArgs : DragEventArgs
	{
		private BetterListViewItem item;

		private BetterListViewDropPart itemDropPart;

		/// <summary>
		///   Gets or sets the item on which the data has been dropped.
		/// </summary>
		/// <value>
		///   The item on which the data has been dropped.
		/// </value>
		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				this.item = value;
			}
		}

		/// <summary>
		///   Gets or sets the part of the item on which the data has been dropped.
		/// </summary>
		/// <value>
		///   The part of the item on which the data has been dropped.
		/// </value>
		public BetterListViewDropPart ItemDropPart {
			get {
				return this.itemDropPart;
			}
			set {
				this.itemDropPart = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemDropEventArgs" /> class.
		/// </summary>
		/// <param name="data">Data being dropped on the control.</param>
		/// <param name="keyState">Current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.</param>
		/// <param name="x">X-coordinate of the mouse pointer, in screen coordinates.</param>
		/// <param name="y">Y-coordinate of the mouse pointer, in screen coordinates.</param>
		/// <param name="allowedEffect">Which drag-and-drop operations are allowed by the originator (or source) of the drag event.</param>
		/// <param name="effect">Target drop effect in a drag-and-drop operation.</param>
		/// <param name="item">Item on which the data has been dropped.</param>
		/// <param name="itemDropPart">Part of the item on which the data has been dropped.</param>
		public BetterListViewItemDropEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect, BetterListViewItem item, BetterListViewDropPart itemDropPart)
			: base(data, keyState, x, y, allowedEffect, effect) {
			this.Item = item;
			this.ItemDropPart = itemDropPart;
		}
	}
}