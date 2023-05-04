namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Specifies how a column header should be automatically resized.
	/// </summary>
	public enum BetterListViewColumnHeaderAutoResizeStyle
	{
		/// <summary>
		///   Specifies no resizing should occur.
		/// </summary>
		None = 0,
		/// <summary>
		///   Specifies the column should be resized based on the length of the column header content.
		/// </summary>
		HeaderSize = 1,
		/// <summary>
		///   Specifies the column should be resized based on the length of the column content.
		/// </summary>
		ColumnContent = 2
	}
}