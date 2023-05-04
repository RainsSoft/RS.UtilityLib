using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Collection with extensions for certain collection operations.
	/// </summary>
	/// <typeparam name="TItem">collection item type</typeparam>
	public interface IExtendedCollection<TItem> : ICollection<TItem>, IEnumerable<TItem>, IEnumerable, ICollection
	{
		/// <summary>
		///   Add specified items to the collection.
		/// </summary>
		/// <param name="items">items to add</param>
		void AddRange(IEnumerable items);

		/// <summary>
		///   Remove specified items from the collection.
		/// </summary>
		/// <param name="items">items to remove</param>
		void RemoveRange(IEnumerable items);
	}
}