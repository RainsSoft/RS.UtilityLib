using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemMouseHover event.
	/// </summary>
	public class BetterListViewItemMouseHoverEventArgs : EventArgs
	{
		private BetterListViewItem item;

		/// <summary>
		///   Gets or sets the item on which mouse hovers.
		/// </summary>
		/// <value>
		///   The item on which mouse hovers.
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
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemMouseHoverEventArgs" /> class.
		/// </summary>
		/// <param name="item">Item on which mouse hovers.</param>
		public BetterListViewItemMouseHoverEventArgs(BetterListViewItem item) {
			this.item = item;
		}
	}
}