namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item display mode.
	/// </summary>
	public enum BetterListViewView
	{
		/// <summary>
		///   large icons with item text
		/// </summary>
		LargeIcon = 0,
		/// <summary>
		///   detailed view of items and sub-items ordered in columns
		/// </summary>
		Details = 1,
		/// <summary>
		///   small icons with item text
		/// </summary>
		SmallIcon = 2,
		/// <summary>
		///   small icons with item text viewed in top-down fashion
		/// </summary>
		List = 3,
		/// <summary>
		///   tiles with custom-sized icons, item texts and sub-item texts
		/// </summary>
		Tile = 4
	}
}