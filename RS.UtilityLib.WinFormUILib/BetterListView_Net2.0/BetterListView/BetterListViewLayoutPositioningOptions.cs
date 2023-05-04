using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Layout positioning options.
	/// </summary>
	[Flags]
	internal enum BetterListViewLayoutPositioningOptions
	{
		/// <summary>
		///   no extra options
		/// </summary>
		None = 0,
		/// <summary>
		///   stretch spacing between elements in a row to fill content area
		/// </summary>
		StretchSpacing = 1,
		/// <summary>
		///   center row of elements in the content area
		/// </summary>
		CenterRow = 2
	}
}