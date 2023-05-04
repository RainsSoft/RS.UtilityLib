namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   State of a BetterListViewColumnHeader.
	/// </summary>
	public enum BetterListViewColumnHeaderState
	{
		/// <summary>
		///   column header is in a default state
		/// </summary>
		Normal = 0,
		/// <summary>
		///   column header is hot (mouse hovers over the column header)
		/// </summary>
		Hot = 1,
		/// <summary>
		///   column header is pressed
		/// </summary>
		Pressed = 2,
		/// <summary>
		///   column corresponding to the column header is sorted
		/// </summary>
		Sorted = 3
	}
}
