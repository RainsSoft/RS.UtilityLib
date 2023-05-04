namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Column header state information.
	/// </summary>
	public struct BetterListViewColumnHeaderStateInfo
	{
		/// <summary>
		///   represents an empty BetterListViewColumnHeaderStateInfo structure
		/// </summary>
		public static BetterListViewColumnHeaderStateInfo Empty = new BetterListViewColumnHeaderStateInfo(BetterListViewColumnHeaderState.Normal, BetterListViewSortOrder.None);

		private readonly BetterListViewColumnHeaderState columnHeaderState;

		private readonly BetterListViewSortOrder sortOrder;

		/// <summary>
		///   column header state
		/// </summary>
		public BetterListViewColumnHeaderState ColumnHeaderState => this.columnHeaderState;

		/// <summary>
		///   column sort order
		/// </summary>
		public BetterListViewSortOrder SortOrder => this.sortOrder;

		/// <summary>
		///   this BetterListViewColumnHeaderStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewColumnHeaderStateInfo.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnHeaderStateInfo" /> struct.
		/// </summary>
		/// <param name="columnHeaderState">column header state</param>
		/// <param name="sortOrder">column sort order</param>
		public BetterListViewColumnHeaderStateInfo(BetterListViewColumnHeaderState columnHeaderState, BetterListViewSortOrder sortOrder) {
			this.columnHeaderState = columnHeaderState;
			this.sortOrder = sortOrder;
		}

		/// <summary>
		///   Test whether the two BetterListViewColumnHeaderStateInfo objects are identical.
		/// </summary>
		/// <param name="columnHeaderStateInfoA">first BetterListViewColumnHeaderStateInfo object</param>
		/// <param name="columnHeaderStateInfoB">second BetterListViewColumnHeaderStateInfo object</param>
		/// <returns>the two BetterListViewColumnHeaderStateInfo objects are identical</returns>
		public static bool operator ==(BetterListViewColumnHeaderStateInfo columnHeaderStateInfoA, BetterListViewColumnHeaderStateInfo columnHeaderStateInfoB) {
			return columnHeaderStateInfoA.Equals(columnHeaderStateInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewColumnHeaderStateInfo objects are different.
		/// </summary>
		/// <param name="columnHeaderStateInfoA">first BetterListViewColumnHeaderStateInfo object</param>
		/// <param name="columnHeaderStateInfoB">second BetterListViewColumnHeaderStateInfo object</param>
		/// <returns>the two BetterListViewColumnHeaderStateInfo objects are different</returns>
		public static bool operator !=(BetterListViewColumnHeaderStateInfo columnHeaderStateInfoA, BetterListViewColumnHeaderStateInfo columnHeaderStateInfoB) {
			return !columnHeaderStateInfoA.Equals(columnHeaderStateInfoB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewColumnHeaderStateInfo betterListViewColumnHeaderStateInfo)) {
				return false;
			}
			if (this.columnHeaderState == betterListViewColumnHeaderStateInfo.columnHeaderState) {
				return this.sortOrder == betterListViewColumnHeaderStateInfo.sortOrder;
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
			return this.columnHeaderState.GetHashCode() ^ this.sortOrder.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return $"{base.GetType().Name}: {{ColumnHeaderState = '{this.columnHeaderState}', SortOrder = '{this.sortOrder}'}}";
		}
	}
}