using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemSelectionChanged event.
	/// </summary>
	public class BetterListViewItemSelectionChangedEventArgs : EventArgs
	{
		private bool isSelected;

		private BetterListViewItem item;

		private BetterListViewAddress itemAddress;

		private int itemIndex;

		/// <summary>
		///   Gets or sets a value indicating whether the items is selected.
		/// </summary>
		/// <value>
		///   <c>true</c> if the item is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected {
			get {
				return this.isSelected;
			}
			set {
				this.isSelected = value;
			}
		}

		/// <summary>
		///   Gets or sets the item whose selection state has changed.
		/// </summary>
		/// <value>
		///   The item whose selection state has changed.
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
		///   Gets or sets address of the item whose selection state has changed.
		/// </summary>
		/// <value>
		///   Address of the item whose selection state has changed.
		/// </value>
		public BetterListViewAddress ItemAddress {
			get {
				return this.itemAddress;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.itemAddress = value;
			}
		}

		/// <summary>
		///   Gets or sets index of the item whose selection state has changed.
		/// </summary>
		/// <value>
		///   The index of the item whose selection state has changed.
		/// </value>
		public int ItemIndex {
			get {
				return this.itemIndex;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.itemIndex = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemSelectionChangedEventArgs" /> class.
		/// </summary>
		/// <param name="isSelected">The item is selected.</param>
		/// <param name="item">The item whose selection state has changed.</param>
		/// <param name="itemAddress">Address of the item whose selection state has changed.</param>
		/// <param name="itemIndex">Index of the item whose selection state has changed.</param>
		public BetterListViewItemSelectionChangedEventArgs(bool isSelected, BetterListViewItem item, BetterListViewAddress itemAddress, int itemIndex) {
			this.IsSelected = isSelected;
			this.Item = item;
			this.ItemAddress = itemAddress;
			this.ItemIndex = itemIndex;
		}
	}
}