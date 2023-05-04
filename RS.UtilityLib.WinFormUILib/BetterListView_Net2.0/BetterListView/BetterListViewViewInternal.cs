namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item display mode.
	/// </summary>
	public enum BetterListViewViewInternal
	{
		/// <summary>
		///   detailed view of items without columns
		/// </summary>
		Details = 0,
		/// <summary>
		///   detailed view of items and sub-items ordered in columns
		/// </summary>
		DetailsColumns = 1,
		/// <summary>
		///   small icons with item text
		/// </summary>
		SmallIcon = 2,
		/// <summary>
		///   large icons with item text
		/// </summary>
		LargeIcon = 3,
		/// <summary>
		///   small icons with item text viewed in top-down fashion
		/// </summary>
		List = 4,
		/// <summary>
		///   tiles with custom-sized icons, item texts and sub-item texts
		/// </summary>
		Tile = 5
	}
}