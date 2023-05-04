namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Type of child item unselection behavior when parent item is collapsed.
	/// </summary>
	public enum BetterListViewUnselectionBehavior
	{
		/// <summary>
		/// No action is performed when items are unselected.
		/// </summary>
		DoNothing = 0,
		/// <summary>
		/// Unselect items only.
		/// </summary>
		UnselectOnly = 1,
		/// <summary>
		/// Unselect child items and select parent item.
		/// </summary>
		SelectParent = 2
	}
}