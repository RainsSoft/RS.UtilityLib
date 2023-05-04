using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Defined extra behavior of group headers when interacting with keyboard and mouse.
	/// </summary>
	[Flags]
	public enum BetterListViewGroupHeaderBehavior
	{
		/// <summary>
		///   Just display the group header; the group header is not interactive.
		/// </summary>
		None = 0,
		/// <summary>
		///   Allow focusing the group header with keyboard.
		/// </summary>
		KeyboardFocus = 1,
		/// <summary>
		///   Allow selecting items within group by selecting the group header with keyboard.
		/// </summary>
		KeyboardSelectAndFocus = 3,
		/// <summary>
		///   Allow focusing the group header with mouse.
		/// </summary>
		MouseFocus = 4,
		/// <summary>
		///   Allow highlighting the group header with mouse.
		/// </summary>
		MouseHighlight = 8,
		/// <summary>
		///   Allow selecting items within group by selecting the group header with mouse.
		/// </summary>
		MouseSelect = 0x10,
		/// <summary>
		///   All the extra behaviors of group headers are active; the group header is fully interactive.
		/// </summary>
		All = 0x1F
	}
}