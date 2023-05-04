using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   State of a BetterListViewGroup.
	/// </summary>
	[Flags]
	public enum BetterListViewGroupState
	{
		/// <summary>
		///   group is in a default state
		/// </summary>
		Normal = 0,
		/// <summary>
		///   group is focused
		/// </summary>
		Focused = 1,
		/// <summary>
		///   group is hot
		/// </summary>
		Hot = 2
	}
}