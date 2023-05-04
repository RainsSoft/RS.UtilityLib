namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Display mode of a sorted column.
	/// </summary>
	public enum BetterListViewSortedColumnsRowsHighlight
	{
		/// <summary>
		///   hide the sorted column highlight
		/// </summary>
		Hide = 0,
		/// <summary>
		///   show sorted column highlight always
		/// </summary>
		ShowAlways = 1,
		/// <summary>
		///   show sorted column highlight only when multiple columns are sorted
		/// </summary>
		ShowMultiColumnOnly = 2
	}
}