namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Text wrapping behavior of items and sub-items.
	/// </summary>
	internal enum BetterListViewTextWrapping
	{
		/// <summary>
		///   Wrap text to maximum number of lines specified by the current layout (MaximumTextLines property).
		/// </summary>
		Layout = 0,
		/// <summary>
		///   Wrap text to available space.
		/// </summary>
		Space = 1,
		/// <summary>
		///   Do not wrap text.
		/// </summary>
		None = 2
	}
}