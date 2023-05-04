using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.AfterLabelEdit event.
	/// </summary>
	public class BetterListViewLabelEditEventArgs : EventArgs
	{
		private string label;

		private BetterListViewSubItem subItem;

		/// <summary>
		///   Gets or sets the new text assigned to the sub-item.
		/// </summary>
		/// <value>
		///   The new text assigned to the sub-item.
		/// </value>
		public string Label {
			get {
				return this.label;
			}
			set {
				this.label = value;
			}
		}

		/// <summary>
		///   Gets or sets the sub-item being edited.
		/// </summary>
		/// <value>
		///   The sub-item being edited.
		/// </value>
		public BetterListViewSubItem SubItem {
			get {
				return this.subItem;
			}
			set {
				this.subItem = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item being edited.</param>
		public BetterListViewLabelEditEventArgs(BetterListViewSubItem subItem)
			: this(string.Empty, subItem) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> class.
		/// </summary>
		/// <param name="label">New text assigned to the sub-item.</param>
		/// <param name="subItem">Sub-item being edited.</param>
		public BetterListViewLabelEditEventArgs(string label, BetterListViewSubItem subItem) {
			this.label = label;
			this.subItem = subItem;
		}
	}
}