namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Level of control invalidation.
	/// </summary>
	public enum BetterListViewInvalidationLevel
	{
		/// <summary>
		///   no action
		/// </summary>
		None = 0,
		/// <summary>
		///   set visible layout elements
		/// </summary>
		Scroll = 1,
		/// <summary>
		///   adjust layout elements
		/// </summary>
		Adjust = 2,
		/// <summary>
		///   measure content area
		/// </summary>
		MeasureContent = 3,
		/// <summary>
		///   measure layout elements
		/// </summary>
		MeasureElements = 4,
		/// <summary>
		///   setup layout elements
		/// </summary>
		Setup = 5
	}
}