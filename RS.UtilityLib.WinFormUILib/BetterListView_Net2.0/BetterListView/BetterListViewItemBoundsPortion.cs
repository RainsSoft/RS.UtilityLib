namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents portion of item boundaries.
	/// </summary>
	public enum BetterListViewItemBoundsPortion
	{
		/// <summary>
		///   The bounding rectangle of the entire item (including sub-items).
		/// </summary>
		Entire = 0,
		/// <summary>
		///   The bounding rectangle of the item image.
		/// </summary>
		Icon = 1,
		/// <summary>
		///   The bounding rectangle of the item text.
		/// </summary>
		Label = 2,
		/// <summary>
		///   The bounding rectangle of the item (without sub-items).
		/// </summary>
		ItemOnly = 3,
		/// <summary>
		///   The bounding rectangle of the item selection.
		/// </summary>
		Selection = 4,
		/// <summary>
		///   The bounding rectangle of the item expand button.
		/// </summary>
		ExpandButton = 5,
		/// <summary>
		///   The bounding rectangle of the item check box.
		/// </summary>
		CheckBox = 6
	}
}