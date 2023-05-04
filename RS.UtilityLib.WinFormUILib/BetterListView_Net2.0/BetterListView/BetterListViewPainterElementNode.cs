namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Tree node glyph elements supported by the BetterListViewPainter.
	/// </summary>
	internal enum BetterListViewPainterElementNode
	{
		/// <summary>
		///   closed tree node glyph
		/// </summary>
		Closed = 0,
		/// <summary>
		///   closed tree node glyph in a hot state
		/// </summary>
		ClosedHot = 1,
		/// <summary>
		///   opened tree node glyph
		/// </summary>
		Opened = 2,
		/// <summary>
		///   opened tree node glyph in a hot state
		/// </summary>
		OpenedHot = 3,
		/// <summary>
		///   undefined element
		/// </summary>
		Undefined = 4
	}
}