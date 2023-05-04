using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Column header reordering options.
	/// </summary>
	[Flags]
	public enum BetterListViewColumnReorderOptions
	{
		/// <summary>
		///   No options active.
		/// </summary>
		None = 0,
		/// <summary>
		///   Reorder sub-items under the corresponding column headers.
		/// </summary>
		ReorderItems = 1,
		/// <summary>
		///   The provided indices corresponds to DisplayIndex (visible position) rather than Index (position in Columns collection).
		/// </summary>
		VisibleIndices = 2
	}
}