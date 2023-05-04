using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.CheckedItemsChanged event.
	/// </summary>
	public class BetterListViewCheckedItemsChangedEventArgs : EventArgs
	{
		private BetterListViewItem item;

		private BetterListViewItemCollection items;

		private ReadOnlyCollection<CheckState> oldCheckStates;

		private CheckState newCheckState;

		private BetterListViewCheckStateChangeMode checkStateChangeMode;

		/// <summary>
		///   Gets or sets the checked item.
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
		///   Gets or sets the items whose check state has been changed.
		/// </summary>
		/// <value>
		///   The items whose check state has been changed.
		/// </value>
		public BetterListViewItemCollection Items {
			get {
				return this.items;
			}
			set {
				Checks.CheckCollection(value, "value");
				this.items = value;
			}
		}

		/// <summary>
		///   Gets or sets the previous item check state values.
		/// </summary>
		/// <value>
		///   The previous item check state values.
		/// </value>
		public ReadOnlyCollection<CheckState> OldCheckStates {
			get {
				return this.oldCheckStates;
			}
			set {
				Checks.CheckCollection(value, "value");
				this.oldCheckStates = value;
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

		public BetterListViewCheckedItemsChangedEventArgs(BetterListViewItem item, BetterListViewItemCollection items, ReadOnlyCollection<CheckState> oldCheckStates, CheckState newCheckState, BetterListViewCheckStateChangeMode checkStateChangeMode) {
			this.Item = item;
			this.Items = items;
			this.OldCheckStates = oldCheckStates;
			this.NewCheckState = newCheckState;
			this.CheckStateChangeMode = checkStateChangeMode;
		}
	}
}