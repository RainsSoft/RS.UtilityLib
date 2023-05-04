namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Style of the BetterListView column header.
	/// </summary>
	public enum BetterListViewHeaderStyle
	{
		/// <summary>
		///   column headers are present, but not displayed
		/// </summary>
		None = 0,
		/// <summary>
		///   column headers do not register clicking on them
		/// </summary>
		Nonclickable = 1,
		/// <summary>
		///   column headers register clicking on them with effect
		/// </summary>
		Clickable = 2,
		/// <summary>
		///   column headers register clicking on them and initiate sorting
		/// </summary>
		Sortable = 3,
		/// <summary>
		///   column headers allow removing sort state
		/// </summary>
		Unsortable = 4
	}
}