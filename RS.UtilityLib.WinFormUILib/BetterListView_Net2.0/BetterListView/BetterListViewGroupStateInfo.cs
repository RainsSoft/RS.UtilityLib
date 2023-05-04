namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Group state information.
	/// </summary>
	public struct BetterListViewGroupStateInfo
	{
		/// <summary>
		///   represents an empty BetterListViewGroupStateInfo structure
		/// </summary>
		public static readonly BetterListViewGroupStateInfo Empty = new BetterListViewGroupStateInfo(BetterListViewGroupState.Normal, BetterListViewGroupExpandButtonState.ExpandedNormal);

		private readonly BetterListViewGroupState groupState;

		private readonly BetterListViewGroupExpandButtonState expandButtonState;

		/// <summary>
		///   group state
		/// </summary>
		public BetterListViewGroupState GroupState => this.groupState;

		/// <summary>
		///   group expand button state
		/// </summary>
		public BetterListViewGroupExpandButtonState ExpandButtonState => this.expandButtonState;

		/// <summary>
		///   this BetterListViewGroupStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewGroupStateInfo.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewGroupStateInfo" /> struct.
		/// </summary>
		/// <param name="groupState">group state</param>
		/// <param name="expandButtonState">group expand button state</param>
		public BetterListViewGroupStateInfo(BetterListViewGroupState groupState, BetterListViewGroupExpandButtonState expandButtonState) {
			this.groupState = groupState;
			this.expandButtonState = expandButtonState;
		}

		/// <summary>
		///   Test whether the two BetterListViewGroupStateInfo objects are identical.
		/// </summary>
		/// <param name="groupStateInfoA">first BetterListViewGroupStateInfo object</param>
		/// <param name="groupStateInfoB">second BetterListViewGroupStateInfo object</param>
		/// <returns>the two BetterListViewGroupStateInfo objects are identical</returns>
		public static bool operator ==(BetterListViewGroupStateInfo groupStateInfoA, BetterListViewGroupStateInfo groupStateInfoB) {
			return groupStateInfoA.Equals(groupStateInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewGroupStateInfo objects are different.
		/// </summary>
		/// <param name="groupStateInfoA">first BetterListViewGroupStateInfo object</param>
		/// <param name="groupStateInfoB">second BetterListViewGroupStateInfo object</param>
		/// <returns>the two BetterListViewGroupStateInfo objects are different</returns>
		public static bool operator !=(BetterListViewGroupStateInfo groupStateInfoA, BetterListViewGroupStateInfo groupStateInfoB) {
			return !groupStateInfoA.Equals(groupStateInfoB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewGroupStateInfo betterListViewGroupStateInfo)) {
				return false;
			}
			if (this.groupState == betterListViewGroupStateInfo.groupState) {
				return this.expandButtonState == betterListViewGroupStateInfo.expandButtonState;
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
			return this.groupState.GetHashCode() ^ this.expandButtonState.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return $"{base.GetType().Name}: {{GroupState = '{this.groupState}', ExpandButtonState = '{this.expandButtonState}'}}";
		}
	}
}