namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Reason for changing column width.
	/// </summary>
	public enum BetterListViewColumnWidthChangeSource
	{
		/// <summary>
		///   Column width has been changed by auto-sizing with mouse.
		/// </summary>
		AutoSizeMouse = 0,
		/// <summary>
		///   Column width has been changed by auto-sizing from user code.
		/// </summary>
		AutoSizeUserCode = 1,
		/// <summary>
		///   Column width has been changed directly by mouse.
		/// </summary>
		ResizeMouse = 2,
		/// <summary>
		///   Column width has been changed directly from user code.
		/// </summary>
		ResizeUserCode = 3
	}
}