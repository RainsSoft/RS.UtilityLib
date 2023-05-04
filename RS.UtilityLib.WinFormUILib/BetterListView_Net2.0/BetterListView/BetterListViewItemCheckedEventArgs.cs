using System;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemChecked event.
	/// </summary>
	public class BetterListViewItemCheckedEventArgs : EventArgs
	{
		private BetterListViewItem item;

		private CheckState oldCheckState;

		private CheckState newCheckState;

		private BetterListViewCheckStateChangeMode checkStateChangeMode;

		/// <summary>
		///   Gets or sets the item whose check state has changed.
		/// </summary>
		/// <value>
		///   The checked item.
		/// </value>
		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.item = value;
			}
		}

		/// <summary>
		///   Gets or sets the previous item check state value.
		/// </summary>
		/// <value>
		///   The previous item check state value.
		/// </value>
		public CheckState OldCheckState {
			get {
				return this.oldCheckState;
			}
			set {
				this.oldCheckState = value;
			}
		}

		/// <summary>
		///   Gets or sets the new item check state value.
		/// </summary>
		/// <value>
		///   The new item check state value.
		/// </value>
		public CheckState NewCheckState {
			get {
				return this.newCheckState;
			}
			set {
				this.newCheckState = value;
			}
		}

		/// <summary>
		///   Source of the check state change.
		/// </summary>
		public BetterListViewCheckStateChangeMode CheckStateChangeMode {
			get {
				return this.checkStateChangeMode;
			}
			set {
				Checks.CheckNotEqual(value, BetterListViewCheckStateChangeMode.Undefined, "value", "BetterListViewCheckStateChangeMode.Undefined");
				this.checkStateChangeMode = value;
			}
		}

		public BetterListViewItemCheckedEventArgs(BetterListViewItem item, CheckState oldCheckState, CheckState newCheckState, BetterListViewCheckStateChangeMode checkStateChangeMode) {
			this.Item = item;
			this.OldCheckState = oldCheckState;
			this.NewCheckState = newCheckState;
			this.CheckStateChangeMode = checkStateChangeMode;
		}
	}
}