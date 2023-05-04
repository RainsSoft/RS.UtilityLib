namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Group elements supported by the BetterListViewPainter.
	/// </summary>
	internal enum BetterListViewPainterElementGroup
	{
		/// <summary>
		///   focused group
		/// </summary>
		Focused = 0,
		/// <summary>
		///   collapsed group in a hot state
		/// </summary>
		CollapsedHot = 1,
		/// <summary>
		///   collapsed focused group in a hot state
		/// </summary>
		CollapsedHotFocused = 2,
		/// <summary>
		///   collapsed selected group within inactive control
		/// </summary>
		CollapsedSelectedInactive = 3,
		/// <summary>
		///   collapsed selected group in a hot state within inactive control
		/// </summary>
		CollapsedSelectedHotInactive = 4,
		/// <summary>
		///   collapsed selected group within active control
		/// </summary>
		CollapsedSelectedActive = 5,
		/// <summary>
		///   collapsed selected group in a hot state within active control
		/// </summary>
		CollapsedSelectedHotActive = 6,
		/// <summary>
		///   expanded group in a hot state
		/// </summary>
		ExpandedHot = 7,
		/// <summary>
		///   expanded focused group in a hot state
		/// </summary>
		ExpandedHotFocused = 8,
		/// <summary>
		///   expanded selected group within inactive control
		/// </summary>
		ExpandedSelectedInactive = 9,
		/// <summary>
		///   expanded selected group in a hot state within inactive control
		/// </summary>
		ExpandedSelectedHotInactive = 10,
		/// <summary>
		///   expanded selected group within active control
		/// </summary>
		ExpandedSelectedActive = 11,
		/// <summary>
		///   expanded selected focused group within active control
		/// </summary>
		ExpandedSelectedFocusedActive = 12,
		/// <summary>
		///   expanded selected group in a hot state within active control
		/// </summary>
		ExpandedSelectedHotActive = 13,
		/// <summary>
		///   expanded selected focused group in a hot state within active control
		/// </summary>
		ExpandedSelectedHotFocusedActive = 14,
		/// <summary>
		///   undefined element
		/// </summary>
		Undefined = 15
	}
}