namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type of collection modification.
	/// </summary>
	public enum BetterListViewElementCollectionChangeType
	{
		/// <summary>
		///   item(s) have been added in the collection
		/// </summary>
		Add = 0,
		/// <summary>
		///   item(s) have been removed from the collection
		/// </summary>
		Remove = 1,
		/// <summary>
		///   an item was changed
		/// </summary>
		Set = 2,
		/// <summary>
		///   item(s) have been sorted within the collection
		/// </summary>
		Sort = 3
	}
}