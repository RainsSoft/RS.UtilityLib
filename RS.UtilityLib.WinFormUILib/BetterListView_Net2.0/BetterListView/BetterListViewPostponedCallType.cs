namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Type of a call to make after EndUpdate.
	/// </summary>
	internal enum BetterListViewPostponedCallType
	{
		AutoResizeColumn = 0,
		/// <summary>
		/// Scroll element(s) or custom area into view.
		/// </summary>
		EnsureVisible = 1
	}
}