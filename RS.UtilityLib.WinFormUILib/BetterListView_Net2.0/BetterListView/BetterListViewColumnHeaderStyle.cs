namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Style of the BetterListView column header.
	/// </summary>
	public enum BetterListViewColumnHeaderStyle
	{
		/// <summary>
		///   column header is present, but not displayed
		/// </summary>
		None = 0,
		/// <summary>
		///   column header does not register clicking on it
		/// </summary>
		Nonclickable = 1,
		/// <summary>
		///   column header registers clicking on it with effect
		/// </summary>
		Clickable = 2,
		/// <summary>
		///   column header registers clicking on it and initiates sorting
		/// </summary>
		Sortable = 3,
		/// <summary>
		///   column header allows removing sort state
		/// </summary>
		Unsortable = 4,
		/// <summary>
		///   column header style is determined by BetterListView.HeaderStyle property
		/// </summary>
		Default = 6
	}
}