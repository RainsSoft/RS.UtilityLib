using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item reordering options.
	/// </summary>
	[Flags]
	public enum BetterListViewItemReorderOptions
	{
		/// <summary>
		///   no options active
		/// </summary>
		None = 0,
		/// <summary>
		///   do not change item groups when reordering
		/// </summary>
		KeepGroups = 2
	}
}