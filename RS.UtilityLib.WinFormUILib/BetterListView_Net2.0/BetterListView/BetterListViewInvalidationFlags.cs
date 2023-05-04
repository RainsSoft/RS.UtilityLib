using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Control invalidation options.
	/// </summary>
	[Flags]
	public enum BetterListViewInvalidationFlags
	{
		/// <summary>
		///   no extra invalidation options
		/// </summary>
		None = 0,
		/// <summary>
		///   position elements during refresh
		/// </summary>
		Position = 1,
		/// <summary>
		///   redraw control during refresh
		/// </summary>
		Draw = 2
	}
}