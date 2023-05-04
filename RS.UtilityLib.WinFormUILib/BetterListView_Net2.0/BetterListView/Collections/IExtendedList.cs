using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   List with extensions for certain list operations.
	/// </summary>
	/// <typeparam name="TItem">list item type</typeparam>
	public interface IExtendedList<TItem> : IExtendedCollection<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable, ICollection, IList<TItem>, IList
	{
		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="index">start index of the range</param>
		/// <param name="count">length of the range</param>
		/// <returns>range of items from the list</returns>
		List<TItem> GetRange(int index, int count);

		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="indices">indices of items to obtain</param>
		/// <returns>range of items from the list</returns>
		List<TItem> GetRange(IEnumerable<int> indices);

		/// <summary>
		///   Insert specified items to the specified position in the list.
		/// </summary>
		/// <param name="index">insertion position</param>
		/// <param name="items">items to insert</param>
		void InsertRange(int index, IEnumerable items);

		/// <summary>
		///   Remove items at the specified positions from the list.
		/// </summary>
		/// <param name="index">index of the first item to be removed</param>
		/// <param name="count">number of item to be removed</param>
		void RemoveRange(int index, int count);

		/// <summary>
		///   Remove items with the specified items from the list.
		/// </summary>
		/// <param name="indices">indices of items to remove</param>
		void RemoveRange(IEnumerable<int> indices);

		/// <summary>
		///   Sort this list.
		/// </summary>
		void Sort();

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="comparer">item comparer</param>
		void Sort(IComparer<TItem> comparer);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="index">start index of item interval to sort</param>
		/// <param name="count">number of item within interval to sort</param>
		void SortRange(int index, int count);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="index">start index of item interval to sort</param>
		/// <param name="count">number of item within interval to sort</param>
		/// <param name="comparer">item comparer</param>
		void SortRange(int index, int count, IComparer<TItem> comparer);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="indices">indices of items to sort</param>
		void SortRange(IEnumerable<int> indices);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="indices">indices of items to sort</param>
		/// <param name="comparer">item comparer</param>
		void SortRange(IEnumerable<int> indices, IComparer<TItem> comparer);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="items">items to sort</param>
		void SortRange(IEnumerable items);

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="items">items to sort</param>
		/// <param name="comparer">item comparer</param>
		void SortRange(IEnumerable items, IComparer<TItem> comparer);

		/// <summary>
		///   Try to get item at the specified index.
		/// </summary>
		/// <param name="index">index of the item</param>
		/// <param name="item">item to get</param>
		/// <returns>success</returns>
		bool TryGetItem(int index, out TItem item);
	}
}