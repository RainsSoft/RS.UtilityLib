namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Group expand button elements supported by the BetterListViewPainter.
	/// </summary>
	internal enum BetterListViewPainterElementGroupExpandButton
	{
		/// <summary>
		///   collapsed expand button in a normal state
		/// </summary>
		CollapsedNormal = 0,
		/// <summary>
		///   collapsed expand button in a hot state
		/// </summary>
		CollapsedHot = 1,
		/// <summary>
		///   collapsed expand button in a pressed state
		/// </summary>
		CollapsedPressed = 2,
		/// <summary>
		///   expanded expand button in a normal state
		/// </summary>
		ExpandedNormal = 3,
		/// <summary>
		///   expanded expand button in a hot state
		/// </summary>
		ExpandedHot = 4,
		/// <summary>
		///   expanded expand button in a pressed state
		/// </summary>
		ExpandedPressed = 5,
		/// <summary>
		///   collapsed expand button in a normal state; selected group
		/// </summary>
		SelectedCollapsedNormal = 6,
		/// <summary>
		///   collapsed expand button in a hot state; selected group
		/// </summary>
		SelectedCollapsedHot = 7,
		/// <summary>
		///   collapsed expand button in a pressed state; selected group
		/// </summary>
		SelectedCollapsedPressed = 8,
		/// <summary>
		///   undefined element
		/// </summary>
		Undefined = 9
	}
}