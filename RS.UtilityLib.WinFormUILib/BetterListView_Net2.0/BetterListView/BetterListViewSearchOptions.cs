using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item search options.
	/// </summary>
	[Flags]
	public enum BetterListViewSearchOptions
	{
		/// <summary>
		///   no options are active
		/// </summary>
		None = 0,
		/// <summary>
		///   case-sensitive searching
		/// </summary>
		CaseSensitive = 1,
		/// <summary>
		///   consider only first word of the searched text
		/// </summary>
		FirstWordOnly = 2,
		/// <summary>
		///   play sound, when no item is found
		/// </summary>
		PlaySound = 4,
		/// <summary>
		///   if a query string is found in the item text prefix, the item is prefered among others
		/// </summary>
		PrefixPreference = 8,
		/// <summary>
		///   search through seletable items only
		/// </summary>
		SelectableItemsOnly = 0x10,
		/// <summary>
		///   search by words, instead of whole strings
		/// </summary>
		WordSearch = 0x20
	}
}