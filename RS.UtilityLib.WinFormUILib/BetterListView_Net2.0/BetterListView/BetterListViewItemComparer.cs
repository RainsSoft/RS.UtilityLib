using System;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Comparer of BetterListView items.
	/// </summary>
	public class BetterListViewItemComparer : IComparer<BetterListViewItem>
	{
		private BetterListViewSortList sortList = new BetterListViewSortList();

		private BetterListViewColumnHeaderCollection columnHeaders = new BetterListViewColumnHeaderCollection();

		private bool sortAlways;

		/// <summary>
		///   column sorting ordering
		/// </summary>
		internal BetterListViewSortList SortList => this.sortList;

		/// <summary>
		///   sort items event when sort list is empty
		/// </summary>
		internal bool SortAlways => this.sortAlways;

		/// <summary>
		///   Set sort list with corresponding column headers.
		/// </summary>
		/// <param name="sortList">sort list to set</param>
		/// <param name="columnHeaders">column headers to set</param>
		/// <param name="sortAlways">sort items event when sort list is empty</param>
		public void SetSortList(BetterListViewSortList sortList, BetterListViewColumnHeaderCollection columnHeaders, bool sortAlways) {
			Checks.CheckNotNull(sortList, "sortList");
			Checks.CheckNotNull(columnHeaders, "columnHeaders");
			foreach (BetterListViewSortInfo sort in sortList) {
				Checks.CheckBounds(sort.ColumnIndex, 0, columnHeaders.Count - 1, "sortInfo.ColumnIndex");
			}
			this.sortList = sortList;
			this.columnHeaders = columnHeaders;
			this.sortAlways = sortAlways;
		}

		/// <summary>
		///   Compare two items.
		/// </summary>
		/// <param name="itemA">first item to compare</param>
		/// <param name="itemB">second item to compare</param>
		/// <returns>comparison result</returns>
		public virtual int Compare(BetterListViewItem itemA, BetterListViewItem itemB) {
			if (this.sortList.Count != 0) {
				foreach (BetterListViewSortInfo sort in this.sortList) {
					BetterListViewColumnHeader betterListViewColumnHeader = this.columnHeaders[sort.ColumnIndex];
					int num = (sort.OrderAscending ? 1 : (-1));
					int num2 = ((betterListViewColumnHeader.Index < itemA.SubItems.Count && itemA.SubItems[betterListViewColumnHeader.Index] != null) ? 1 : 0);
					int num3 = ((betterListViewColumnHeader.Index < itemB.SubItems.Count && itemB.SubItems[betterListViewColumnHeader.Index] != null) ? 1 : 0);
					if (num2 != num3) {
						return num * num2.CompareTo(num3);
					}
					if (num2 != 0) {
						int num4 = this.CompareSubItems(itemA.SubItems[betterListViewColumnHeader.Index], itemB.SubItems[betterListViewColumnHeader.Index], betterListViewColumnHeader.SortMethod, num);
						if (num4 != 0) {
							return num4;
						}
					}
				}
			}
			else if (this.sortAlways) {
				IComparable valueA = itemA.Key ?? itemA.Text;
				IComparable valueB = itemB.Key ?? itemB.Text;
				int num5 = this.CompareValues(valueA, valueB, 1);
				if (num5 != 0) {
					return num5;
				}
			}
			return this.CompareEqualItems(itemA, itemB);
		}

		/// <summary>
		///   Compare two sub-items.
		/// </summary>
		/// <param name="subItemA">First sub-item to compare.</param>
		/// <param name="subItemB">Second sub-item to compare.</param>
		/// <param name="sortMethod">Item comparison method.</param>
		/// <param name="order">Sort order.</param>
		/// <returns>Comparison result.</returns>
		protected virtual int CompareSubItems(BetterListViewSubItem subItemA, BetterListViewSubItem subItemB, BetterListViewSortMethod sortMethod, int order) {
			IComparable valueA = null;
			IComparable valueB = null;
			object value = subItemA.Value;
			object value2 = subItemB.Value;
			IComparable comparable = ((value != null) ? ((value as IComparable) ?? value.ToString()) : null);
			IComparable comparable2 = ((value2 != null) ? ((value2 as IComparable) ?? value2.ToString()) : null);
			IComparable key = subItemA.Key;
			IComparable key2 = subItemB.Key;
			IComparable text = subItemA.Text;
			IComparable text2 = subItemB.Text;
			switch (sortMethod) {
				case BetterListViewSortMethod.Auto:
					valueA = key ?? comparable ?? text;
					valueB = key2 ?? comparable2 ?? text2;
					break;
				case BetterListViewSortMethod.Text:
					valueA = text;
					valueB = text2;
					break;
				case BetterListViewSortMethod.Key:
					valueA = subItemA.Key;
					valueB = subItemB.Key;
					break;
			}
			return this.CompareValues(valueA, valueB, order);
		}

		/// <summary>
		///   Compare two values considering specific order.
		/// </summary>
		/// <param name="valueA">first value to compare</param>
		/// <param name="valueB">second value to compare</param>
		/// <param name="order">ordering of the values (1 for ascending, -1 for descending)</param>
		/// <returns>comparison result</returns>
		protected virtual int CompareValues(IComparable valueA, IComparable valueB, int order) {
			int num = ((valueA != null) ? 1 : 0);
			int num2 = ((valueB != null) ? 1 : 0);
			if (num != num2) {
				return order * num.CompareTo(num2);
			}
			if (num == 0) {
				return 0;
			}
			return order * valueA.CompareTo(valueB);
		}

		/// <summary>
		///   Compare items that would be otherwise equal using additional criteria.
		/// </summary>
		/// <param name="itemA">first item to compare</param>
		/// <param name="itemB">second item to compare</param>
		/// <returns>comparison result</returns>
		protected virtual int CompareEqualItems(BetterListViewItem itemA, BetterListViewItem itemB) {
			return itemA.Index.CompareTo(itemB.Index);
		}
	}
}