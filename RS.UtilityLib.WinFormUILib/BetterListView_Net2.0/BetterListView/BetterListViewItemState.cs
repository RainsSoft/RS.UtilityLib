using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   State of a BetterListViewItem.
	/// </summary>
	[Flags]
	public enum BetterListViewItemState
	{
		/// <summary>
		///   item is in a default state
		/// </summary>
		Normal = 0,
		/// <summary>
		///   item is focused
		/// </summary>
		Focused = 1,
		/// <summary>
		///   item is hot
		/// </summary>
		Hot = 2,
		/// <summary>
		///   item is selected
		/// </summary>
		Selected = 4,
		/// <summary>
		///   item is drop-highlighted
		/// </summary>
		DropHighlight = 8
	}
}