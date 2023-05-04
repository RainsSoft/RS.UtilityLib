namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Update type of a layout property.
	/// </summary>
	internal enum BetterListViewLayoutPropertyType
	{
		/// <summary>
		///   property change invokes layout setup
		/// </summary>
		Setup = 0,
		/// <summary>
		///   property change clears text caches
		/// </summary>
		Text = 1,
		/// <summary>
		///   property change has other consequences
		/// </summary>
		Other = 2
	}
}