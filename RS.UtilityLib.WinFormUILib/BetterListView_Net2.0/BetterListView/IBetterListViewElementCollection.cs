using System.Collections;
using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView elements.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public interface IBetterListViewElementCollection<TItem> : IExtendedList<TItem>, IExtendedCollection<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable, ICollection, IList<TItem>, IList where TItem : BetterListViewElementBase
	{
		/// <summary>
		///   BetterListView element
		/// </summary>
		/// <param name="key">key of the element within underlying list</param>
		/// <returns>element instance</returns>
		TItem this[string key] { get; }

		/// <summary>
		///   Create array of cloned elements.
		///   Use this method when needing elements without owner collection.
		/// </summary>
		/// <returns>array of cloned elements</returns>
		TItem[] CloneToArray();
	}
}