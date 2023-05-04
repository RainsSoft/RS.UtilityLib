namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Method of item comparison in sorting by column.
	/// </summary>
	public enum BetterListViewSortMethod
	{
		/// <summary>
		///   use Key property for comparison; fall back to Value property, if Key not available; fall back to Text property, if Value not available
		/// </summary>
		Auto = 0,
		/// <summary>
		///   use Text property for comparison
		/// </summary>
		Text = 1,
		/// <summary>
		///   use Key property for comparison
		/// </summary>
		Key = 2
	}
}
