using System.Windows.Forms.VisualStyles;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item state information.
	/// </summary>
	public struct BetterListViewItemStateInfo
	{
		/// <summary>
		///   represents an empty BetterListViewItemStateInfo structure
		/// </summary>
		public static readonly BetterListViewItemStateInfo Empty = new BetterListViewItemStateInfo(BetterListViewItemState.Normal, BetterListViewItemExpandButtonState.Expanded, CheckBoxState.UncheckedNormal);

		private readonly BetterListViewItemState itemState;

		private readonly BetterListViewItemExpandButtonState expandButtonState;

		private readonly CheckBoxState checkBoxState;

		/// <summary>
		///   item state
		/// </summary>
		public BetterListViewItemState ItemState => this.itemState;

		/// <summary>
		///   item expand button state
		/// </summary>
		public BetterListViewItemExpandButtonState ExpandButtonState => this.expandButtonState;

		public CheckBoxState CheckBoxState => this.checkBoxState;

		/// <summary>
		///   this BetterListViewItemStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewItemStateInfo.Empty);

		public BetterListViewItemStateInfo(BetterListViewItemState itemState, BetterListViewItemExpandButtonState expandButtonState, CheckBoxState checkBoxState) {
			this.itemState = itemState;
			this.expandButtonState = expandButtonState;
			this.checkBoxState = checkBoxState;
		}

		/// <summary>
		///   Tests whether the two BetterListViewItemStateInfo objects are identical.
		/// </summary>
		/// <param name="itemStateInfoA">first BetterListViewItemStateInfo object</param>
		/// <param name="itemStateInfoB">second BetterListViewItemStateInfo object</param>
		/// <returns>
		///   the two BetterListViewItemStateInfo objects are identical
		/// </returns>
		public static bool operator ==(BetterListViewItemStateInfo itemStateInfoA, BetterListViewItemStateInfo itemStateInfoB) {
			return itemStateInfoA.Equals(itemStateInfoB);
		}

		/// <summary>
		///   Tests whether the two BetterListViewItemStateInfo objects are different.
		/// </summary>
		/// <param name="itemStateInfoA">first BetterListViewItemStateInfo object</param>
		/// <param name="itemStateInfoB">second BetterListViewItemStateInfo object</param>
		/// <returns>
		///   the two BetterListViewItemStateInfo objects are different
		/// </returns>
		public static bool operator !=(BetterListViewItemStateInfo itemStateInfoA, BetterListViewItemStateInfo itemStateInfoB) {
			return !itemStateInfoA.Equals(itemStateInfoB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewItemStateInfo betterListViewItemStateInfo)) {
				return false;
			}
			if (this.itemState == betterListViewItemStateInfo.itemState && this.expandButtonState == betterListViewItemStateInfo.expandButtonState) {
				return this.checkBoxState == betterListViewItemStateInfo.checkBoxState;
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.itemState.GetHashCode() ^ this.expandButtonState.GetHashCode() ^ this.checkBoxState.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return $"{base.GetType().Name}: {{ItemState = '{this.itemState}', ExpandButtonState = '{this.expandButtonState}', CheckBoxState = '{this.checkBoxState}'}}";
		}
	}
}