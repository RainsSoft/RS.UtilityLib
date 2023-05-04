namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Part of an element on which data is dropped.
	/// </summary>
	public enum BetterListViewDropPart
	{
		/// <summary>
		///   data dropped before the element
		/// </summary>
		Before = 0,
		/// <summary>
		///   data dropped after the element
		/// </summary>
		After = 1,
		/// <summary>
		///   data dropped inside the element
		/// </summary>
		Inside = 2,
		/// <summary>
		///   data dropped on the element
		/// </summary>
		On = 3,
		/// <summary>
		///   undefined drop location
		/// </summary>
		Undefined = 4
	}
}