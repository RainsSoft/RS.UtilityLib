using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Comparer of BetterListViewElementBase instances by their indices.
	/// </summary>
	internal sealed class BetterListViewElementBaseIndexComparer<T> : IComparer<T> where T : BetterListViewElementBase
	{
		private static readonly BetterListViewElementBaseIndexComparer<T> instance = new BetterListViewElementBaseIndexComparer<T>();

		/// <summary>
		///   get instance of BetterListViewElementBaseIndexComparer singleton
		/// </summary>
		public static BetterListViewElementBaseIndexComparer<T> Instance => BetterListViewElementBaseIndexComparer<T>.instance;

		/// <summary>
		/// Prevents a default instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementBaseIndexComparer`1" /> class from being created.
		/// </summary>
		private BetterListViewElementBaseIndexComparer() {
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <returns>
		/// Value 
		///                     Condition 
		///                     Less than zero
		///                 <paramref name="x" /> is less than <paramref name="y" />.
		///                     Zero
		///                 <paramref name="x" /> equals <paramref name="y" />.
		///                     Greater than zero
		///                 <paramref name="x" /> is greater than <paramref name="y" />.
		/// </returns>
		/// <param name="x">The first object to compare.
		///                 </param><param name="y">The second object to compare.
		///                 </param>
		public int Compare(T x, T y) {
			return x.Index.CompareTo(y.Index);
		}
	}
}