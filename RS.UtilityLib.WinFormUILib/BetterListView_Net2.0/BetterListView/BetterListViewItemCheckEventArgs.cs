using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemCheck event.
	/// </summary>
	public class BetterListViewItemCheckEventArgs : CancelEventArgs
	{
		private BetterListViewItem item;

		private BetterListViewItemCollection items;

		private ReadOnlyCollection<CheckState> currentCheckStates;

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
		///   Gets or sets the items whose check state is selected to be changed.
		/// </summary>
		/// <value>
		///   The items whose check state is selected to be changed.
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
		///   Gets or sets the current item check state values.
		/// </summary>
		/// <value>
		///   The current item check state values.
		/// </value>
		public ReadOnlyCollection<CheckState> CurrentCheckStates {
			get {
				return this.currentCheckStates;
			}
			set {
				Checks.CheckCollection(value, "value");
				this.currentCheckStates = value;
			}
		}

		/// <summary>
		///   Gets or sets the new new item check state value.
		/// </summary>
		/// <value>
		///   The new new item check state value.
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

		public BetterListViewItemCheckEventArgs(BetterListViewItem item, BetterListViewItemCollection items, ReadOnlyCollection<CheckState> currentValues, CheckState newValue, BetterListViewCheckStateChangeMode checkStateChangeMode) {
			this.Item = item;
			this.Items = items;
			this.CurrentCheckStates = currentValues;
			this.NewCheckState = newValue;
			this.CheckStateChangeMode = checkStateChangeMode;
		}
	}
}