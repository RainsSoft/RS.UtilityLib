namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Location of a ToolTip on BetterListView.
	/// </summary>
	public enum BetterListViewToolTipLocation
	{
		/// <summary>
		///   custom ToolTip location
		/// </summary>
		Custom = 0,
		/// <summary>
		///   ToolTip located on element client area
		/// </summary>
		Client = 1,
		/// <summary>
		///   ToolTip located on element expand button
		/// </summary>
		ExpandButton = 2,
		/// <summary>
		///   ToolTip located on element image
		/// </summary>
		Image = 3,
		/// <summary>
		///   ToolTip located on element text
		/// </summary>
		Text = 4,
		/// <summary>
		///   ToolTip located on element border (applies to column headers)
		/// </summary>
		Border = 5,
		/// <summary>
		///   ToolTip located on element sort glyph (applies to column headers)
		/// </summary>
		SortGlyph = 6,
		/// <summary>
		///   ToolTip located on an element check box
		/// </summary>
		CheckBox = 7
	}
}