namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item elements supported by the BetterListViewPainter.
	/// </summary>
	internal enum BetterListViewPainterElementItem
	{
		/// <summary>
		///   disabled item (theme compatibility element)
		/// </summary>
		Disabled = 0,
		/// <summary>
		///   focused item
		/// </summary>
		Focused = 1,
		/// <summary>
		///   item in a hot state
		/// </summary>
		Hot = 2,
		/// <summary>
		///   focused item in a hot state
		/// </summary>
		HotFocused = 3,
		/// <summary>
		///   item in a selected state
		/// </summary>
		Selected = 4,
		/// <summary>
		///   focused item in selected state (theme compatibility element)
		/// </summary>
		SelectedFocused = 5,
		/// <summary>
		///   unfocused item in selected state
		/// </summary>
		SelectedUnfocused = 6,
		/// <summary>
		///   item in selected and hot state
		/// </summary>
		SelectedHot = 7,
		/// <summary>
		///   focused item in selected and hot state (theme compatibility element)
		/// </summary>
		SelectedHotFocused = 8,
		/// <summary>
		///   focused item in selected and hot state (theme compatibility element)
		/// </summary>
		SelectedHotUnfocused = 9,
		/// <summary>
		///   undefined element
		/// </summary>
		Undefined = 10
	}
}