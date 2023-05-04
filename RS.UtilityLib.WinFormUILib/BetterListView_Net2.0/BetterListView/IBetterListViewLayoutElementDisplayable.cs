namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a displayable layout element.
	/// </summary>
	internal interface IBetterListViewLayoutElementDisplayable
	{
		/// <summary>
		///   Element boundaries.
		/// </summary>
		BetterListViewElementBoundsBase LayoutBounds { get; set; }

		/// <summary>
		///   Index of the element within some layout.
		/// </summary>
		int LayoutIndexDisplay { get; set; }
	}
}