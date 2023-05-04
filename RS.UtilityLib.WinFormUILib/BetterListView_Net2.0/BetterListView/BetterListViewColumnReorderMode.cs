namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Column reordering mode.
	/// </summary>
	public enum BetterListViewColumnReorderMode
	{
		/// <summary>
		///   column reordering is disabled
		/// </summary>
		Disabled = 0,
		/// <summary>
		///   column reordering is visually enabled, but handled by the user
		/// </summary>
		Custom = 1,
		/// <summary>
		///   only column headers are reordered
		/// </summary>
		ColumnsOnly = 2,
		/// <summary>
		///   both column headers and sub-items are reordered
		/// </summary>
		Enabled = 3
	}
}