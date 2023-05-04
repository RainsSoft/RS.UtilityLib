using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Additional item sorting options.
	/// </summary>
	[Flags]
	public enum BetterListViewSortOptions
	{
		/// <summary>
		///   no additional sorting options selected
		/// </summary>
		None = 0,
		/// <summary>
		///   add a new column into the list of sorted columns
		/// </summary>
		AddColumn = 1,
		/// <summary>
		///   remove column from the list of sorted columns
		/// </summary>
		RemoveColumn = 2,
		/// <summary>
		///   sorting is invoked by clicking a column
		/// </summary>
		ColumnClicked = 4
	}
}