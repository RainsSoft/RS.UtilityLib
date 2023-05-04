using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Options for the BetterListView.MakeSelection method.
	/// </summary>
	[Flags]
	internal enum BetterListViewSelectionOptions
	{
		None = 0,
		TakeFocus = 1,
		TakeSelection = 2,
		ExtendSelection = 4,
		AddSelection = 8,
		RemoveSelection = 0x10,
		TakeAnchor = 0x20,
		/// <summary>
		///   clear keyboard search string
		/// </summary>
		ClearSearchQuery = 0x40
	}
}