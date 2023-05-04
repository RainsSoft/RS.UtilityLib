using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a modification done to an element collection.
	/// </summary>
	public struct BetterListViewElementCollectionChangeInfo
	{
		private readonly BetterListViewElementCollectionChangeType changeType;

		private readonly ReadOnlyDictionary<BetterListViewElementBase, int> elements;

		private readonly bool isSync;

		/// <summary>
		///   type of collection modification
		/// </summary>
		public BetterListViewElementCollectionChangeType ChangeType => this.changeType;

		/// <summary>
		///   elements that are subject to collection modification with corresponding indices before modification
		/// </summary>
		public ReadOnlyDictionary<BetterListViewElementBase, int> Elements => this.elements;

		/// <summary>
		///   the collection modification is done for synchronization with other data
		/// </summary>
		public bool IsSync => this.isSync;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementCollectionChangeInfo" /> struct.
		/// </summary>
		/// <param name="changeType">type of collection modification</param>
		/// <param name="elements">elements that are subject to collection modification with corresponding indices before modification</param>
		/// <param name="isSync">the collection modification is done from internal code (not from user code)</param>
		public BetterListViewElementCollectionChangeInfo(BetterListViewElementCollectionChangeType changeType, ReadOnlyDictionary<BetterListViewElementBase, int> elements, bool isSync) {
			Checks.CheckNotNull(elements, "elements");
			Checks.CheckTrue(elements.Count != 0, "elements.Count != 0");
			this.changeType = changeType;
			this.elements = elements;
			this.isSync = isSync;
		}

		/// <summary>
		///   Test whether the two BetterListViewAddress objects are identical.
		/// </summary>
		/// <param name="changeInfoA">first BetterListViewElementCollectionChangeInfo object</param>
		/// <param name="changeInfoB">second BetterListViewElementCollectionChangeInfo object</param>
		/// <returns>the two BetterListViewElementCollectionChangeInfo objects are identical</returns>
		public static bool operator ==(BetterListViewElementCollectionChangeInfo changeInfoA, BetterListViewElementCollectionChangeInfo changeInfoB) {
			return changeInfoA.Equals(changeInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewAddress objects are different.
		/// </summary>
		/// <param name="changeInfoA">first BetterListViewElementCollectionChangeInfo object</param>
		/// <param name="changeInfoB">second BetterListViewElementCollectionChangeInfo object</param>
		/// <returns>the two BetterListViewElementCollectionChangeInfo objects are different</returns>
		public static bool operator !=(BetterListViewElementCollectionChangeInfo changeInfoA, BetterListViewElementCollectionChangeInfo changeInfoB) {
			return !changeInfoA.Equals(changeInfoB);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		///   true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">Another object to compare to. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewElementCollectionChangeInfo betterListViewElementCollectionChangeInfo)) {
				return false;
			}
			if (this.changeType == betterListViewElementCollectionChangeInfo.changeType && this.elements.EqualsContent(betterListViewElementCollectionChangeInfo.elements)) {
				return this.isSync == betterListViewElementCollectionChangeInfo.isSync;
			}
			return false;
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		///   A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return this.changeType.GetHashCode() ^ this.elements.Count.GetHashCode() ^ this.isSync.GetHashCode();
		}
	}
}