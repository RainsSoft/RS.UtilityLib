using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.SelectedItemsChanged event.
	/// </summary>
	public class BetterListViewSelectedItemsChangedEventArgs : EventArgs
	{
		private BetterListViewSelectionChangeMode selectionChangeMode;

		private BetterListViewItemCollection selectedItemsNew;

		private BetterListViewItemCollection selectedItemsOld;

		/// <summary>
		///   Gets or sets the type of selection change.
		/// </summary>
		/// <value>
		///   The type of selection change.
		/// </value>
		public BetterListViewSelectionChangeMode SelectionChangeMode {
			get {
				return this.selectionChangeMode;
			}
			set {
				Checks.CheckNotEqual(value, BetterListViewSelectionChangeMode.Undefined, "value", "BetterListViewSelectionChangeMode.Undefined");
				this.selectionChangeMode = value;
			}
		}

		/// <summary>
		///   Gets or sets the items selected after change in selection.
		/// </summary>
		/// <value>
		///   The items selected after change in selection.
		/// </value>
		public BetterListViewItemCollection SelectedItemsNew {
			get {
				return this.selectedItemsNew;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.selectedItemsNew = value;
			}
		}

		/// <summary>
		///   Gets or sets the items selected before change in selection.
		/// </summary>
		/// <value>
		///   The items selected before change in selection.
		/// </value>
		public BetterListViewItemCollection SelectedItemsOld {
			get {
				return this.selectedItemsOld;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.selectedItemsOld = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSelectedItemsChangedEventArgs" /> class.
		/// </summary>
		/// <param name="selectionChangeMode">Type of selection change.</param>
		/// <param name="selectedItemsNew">Items selected after change in selection.</param>
		/// <param name="selectedItemsOld">Items selected before change in selection.</param>
		public BetterListViewSelectedItemsChangedEventArgs(BetterListViewSelectionChangeMode selectionChangeMode, BetterListViewItemCollection selectedItemsNew, BetterListViewItemCollection selectedItemsOld) {
			this.SelectionChangeMode = selectionChangeMode;
			this.SelectedItemsNew = selectedItemsNew;
			this.SelectedItemsOld = selectedItemsOld;
		}
	}
}