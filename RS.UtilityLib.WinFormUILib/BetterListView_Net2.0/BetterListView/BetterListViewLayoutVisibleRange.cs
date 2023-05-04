using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents range of visible layout elements indices.
	/// </summary>
	internal struct BetterListViewLayoutVisibleRange : IEquatable<BetterListViewLayoutVisibleRange>
	{
		/// <summary>
		///   Represents undefined BetterListViewLayoutVisibleRange structure.
		/// </summary>
		public static readonly BetterListViewLayoutVisibleRange Undefined = new BetterListViewLayoutVisibleRange(-1, -1);

		private readonly int indexElementFirst;

		private readonly int indexElementLast;

		/// <summary>
		///   Index of the first visible element.
		/// </summary>
		public int IndexElementFirst => this.indexElementFirst;

		/// <summary>
		///   Index of the last visible element.
		/// </summary>
		public int IndexElementLast => this.indexElementLast;

		/// <summary>
		///   Number of visible elements.
		/// </summary>
		public int ElementCount {
			get {
				if (this.IsUndefined) {
					return 0;
				}
				return this.indexElementLast - this.indexElementFirst + 1;
			}
		}

		/// <summary>
		///   This BetterListViewLayoutVisibleRange structure is undefined.
		/// </summary>
		public bool IsUndefined => ((IEquatable<BetterListViewLayoutVisibleRange>)this).Equals(BetterListViewLayoutVisibleRange.Undefined);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutVisibleRange" /> struct.
		/// </summary>
		/// <param name="indexElementFirst">Index of the first visible element.</param>
		/// <param name="indexElementLast">Index of the last visible element.</param>
		public BetterListViewLayoutVisibleRange(int indexElementFirst, int indexElementLast) {
			if (indexElementFirst != -1 || indexElementLast != -1) {
				Checks.CheckTrue(indexElementFirst >= 0 && indexElementLast >= 0, "indexElementFirst >= 0 && indexElementLast >= 0");
				Checks.CheckTrue(indexElementFirst <= indexElementLast, "indexElementFirst <= indexElementLast");
			}
			this.indexElementFirst = indexElementFirst;
			this.indexElementLast = indexElementLast;
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewLayoutVisibleRange)) {
				return false;
			}
			return ((IEquatable<BetterListViewLayoutVisibleRange>)this).Equals((BetterListViewLayoutVisibleRange)obj);
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.indexElementFirst.GetHashCode() ^ this.indexElementLast.GetHashCode();
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   true if the current object is equal to the other parameter; otherwise, false.
		/// </returns>
		bool IEquatable<BetterListViewLayoutVisibleRange>.Equals(BetterListViewLayoutVisibleRange other) {
			if (this.indexElementFirst == other.indexElementFirst) {
				return this.indexElementLast == other.indexElementLast;
			}
			return false;
		}
	}
}