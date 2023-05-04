using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item selection rendering options.
	/// </summary>
	[Flags]
	internal enum BetterListViewSelectionRenderingOptions
	{
		/// <summary>
		///   no options
		/// </summary>
		None = 0,
		/// <summary>
		///   allow displaying classic-style focus rectangle
		/// </summary>
		AllowClassicFocusRectangle = 1,
		/// <summary>
		///   extended selection size vertically for overlapping
		/// </summary>
		ExtendVertical = 2
	}
}