using System.Globalization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents information about currently focused element.
	/// </summary>
	internal struct BetterListViewFocusInfo
	{
		/// <summary>
		///   empty BetterListViewFocusInfo instance
		/// </summary>
		public static readonly BetterListViewFocusInfo Empty = new BetterListViewFocusInfo(null, -1);

		private readonly BetterListViewElementBase element;

		private readonly int columnIndex;

		/// <summary>
		///   focused element
		/// </summary>
		public BetterListViewElementBase Element => this.element;

		/// <summary>
		///   focused sub-item corresponding column index; in case of item/sub-item is focused
		/// </summary>
		public int ColumnIndex => this.columnIndex;

		/// <summary>
		///   this BetterListViewFocusInfo instance is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewFocusInfo.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewFocusInfo" /> struct.
		/// </summary>
		/// <param name="group">focused group</param>
		public BetterListViewFocusInfo(BetterListViewGroup group) {
			Checks.CheckNotNull(group, "group");
			this.element = group;
			this.columnIndex = -1;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewFocusInfo" /> struct.
		/// </summary>
		/// <param name="item">focused item</param>
		/// <param name="columnIndex">focused sub-item corresponding column index</param>
		public BetterListViewFocusInfo(BetterListViewItem item, int columnIndex) {
			if (item != null) {
				Checks.CheckTrue(columnIndex == -1 || columnIndex >= 0, "(columnIndex == BetterListViewElementBase.IndexUndefined) || (columnIndex >= 0)");
			}
			else {
				Checks.CheckEqual(columnIndex, -1, "columnIndex", "BetterListViewElementBase.IndexUndefined");
			}
			this.element = item;
			this.columnIndex = columnIndex;
		}

		/// <summary>
		///   Test whether the two BetterListViewFocusInfo objects are identical.
		/// </summary>
		/// <param name="focusInfoA">first BetterListViewFocusInfo object</param>
		/// <param name="focusInfoB">second BetterListViewFocusInfo object</param>
		/// <returns>the two BetterListViewFocusInfo objects are identical</returns>
		public static bool operator ==(BetterListViewFocusInfo focusInfoA, BetterListViewFocusInfo focusInfoB) {
			return focusInfoA.Equals(focusInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewFocusInfo objects are different.
		/// </summary>
		/// <param name="focusInfoA">first BetterListViewFocusInfo object</param>
		/// <param name="focusInfoB">second BetterListViewFocusInfo object</param>
		/// <returns>the two BetterListViewFocusInfo objects are different</returns>
		public static bool operator !=(BetterListViewFocusInfo focusInfoA, BetterListViewFocusInfo focusInfoB) {
			return !focusInfoA.Equals(focusInfoB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewFocusInfo betterListViewFocusInfo)) {
				return false;
			}
			if (this.element == betterListViewFocusInfo.element) {
				return this.columnIndex == betterListViewFocusInfo.columnIndex;
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
			return this.element.GetHashCode() ^ this.columnIndex.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{Element = '{this.element}', ColumnIndex = '{this.columnIndex.ToString(CultureInfo.InvariantCulture)}'}}";
		}
	}
}