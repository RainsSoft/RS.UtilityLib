namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Specifies whether individual sub-items can be focused.
	/// </summary>
	public enum BetterListViewSubItemFocusBehavior
	{
		/// <summary>
		///   Sub-items cannot be focused.
		/// </summary>
		None = 0,
		/// <summary>
		///   Sub-items can be focused, including sub-items of combined items.
		/// </summary>
		All = 1,
		/// <summary>
		///   Only sub-items of combined items cannot be focused.
		/// </summary>
		Auto = 2
	}
}