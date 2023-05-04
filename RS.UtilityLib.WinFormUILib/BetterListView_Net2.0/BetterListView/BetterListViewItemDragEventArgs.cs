using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemDrag event.
	/// </summary>
	public class BetterListViewItemDragEventArgs : EventArgs
	{
		private BetterListViewItemDragData itemDragData;

		private Point mousePosition;

		/// <summary>
		///   Gets or sets the data being dragged.
		/// </summary>
		/// <value>
		///   The data being dragged.
		/// </value>
		public BetterListViewItemDragData ItemDragData {
			get {
				return this.itemDragData;
			}
			set {
				this.itemDragData = value;
			}
		}

		/// <summary>
		///   Gets or sets the current mouse position (in client coordinates).
		/// </summary>
		/// <value>
		///   The current mouse position (in client coordinates).
		/// </value>
		public Point MousePosition {
			get {
				return this.mousePosition;
			}
			set {
				this.mousePosition = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemDragEventArgs" /> class.
		/// </summary>
		/// <param name="itemDragData">Data being dragged.</param>
		/// <param name="mousePosition">Current mouse position (in client coordinates).</param>
		public BetterListViewItemDragEventArgs(BetterListViewItemDragData itemDragData, Point mousePosition) {
			this.ItemDragData = itemDragData;
			this.MousePosition = mousePosition;
		}
	}
}