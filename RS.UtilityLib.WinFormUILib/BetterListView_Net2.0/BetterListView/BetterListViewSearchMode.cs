namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item search mode.
	/// </summary>
	public enum BetterListViewSearchMode
	{
		/// <summary>
		///   search by text prefix
		/// </summary>
		Prefix = 0,
		/// <summary>
		///   search by text substring
		/// </summary>
		Substring = 1,
		/// <summary>
		///   search by text substring, if nothing found by prefix
		/// </summary>
		PrefixOrSubstring = 2,
		/// <summary>
		///   search is disabled
		/// </summary>
		Disabled = 3
	}
}
