using System;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Comparer of BetterListViewItem instances by their indices.
	/// </summary>
	internal sealed class BetterListViewItemIndexComparer : IComparer<BetterListViewItem>
	{
		private static readonly BetterListViewItemIndexComparer instance = new BetterListViewItemIndexComparer();

		/// <summary>
		///   get instance of BetterListViewItemComparer singleton
		/// </summary>
		public static BetterListViewItemIndexComparer Instance => BetterListViewItemIndexComparer.instance;

		private BetterListViewItemIndexComparer() {
		}

		public int Compare(BetterListViewItem itemA, BetterListViewItem itemB) {
			if (itemA.ParentItem == null && itemB.ParentItem == null) {
				return itemA.Index.CompareTo(itemB.Index);
			}
			int[] numericAddress = BetterListViewItemIndexComparer.GetNumericAddress(itemA);
			int[] numericAddress2 = BetterListViewItemIndexComparer.GetNumericAddress(itemB);
			int num = Math.Min(numericAddress.Length, numericAddress2.Length);
			for (int i = 0; i < num; i++) {
				int num2 = numericAddress[i].CompareTo(numericAddress2[i]);
				if (num2 != 0) {
					return num2;
				}
			}
			return numericAddress.Length.CompareTo(numericAddress2.Length);
		}

		private static int[] GetNumericAddress(BetterListViewItem item) {
			int level = item.Level;
			int[] array = new int[level + 1];
			for (int num = level; num >= 0; num--) {
				array[num] = item.Index;
				item = item.ParentItem;
			}
			return array;
		}
	}
}