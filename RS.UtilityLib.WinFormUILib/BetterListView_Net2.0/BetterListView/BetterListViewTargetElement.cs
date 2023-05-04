namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Specifies target adjacent element.
	/// </summary>
	public enum BetterListViewTargetElement
	{
		/// <summary>
		///   search layout element that is above a certain element
		/// </summary>
		Up = 0,
		/// <summary>
		///   search layout element that is below a certain element
		/// </summary>
		Down = 1,
		/// <summary>
		///   search layout element that is on the left side of a certain element
		/// </summary>
		Left = 2,
		/// <summary>
		///   search layout element that is on the right side of a certain element
		/// </summary>
		Right = 3,
		/// <summary>
		///   search layout element that is previous for a certain element (by the number of elements-per-page)
		/// </summary>
		PageUp = 4,
		/// <summary>
		///   search layout element that is next for a certain element (by the number of elements-per-page)
		/// </summary>
		PageDown = 5,
		/// <summary>
		///   search layout element that is the first one in its context
		/// </summary>
		Home = 6,
		/// <summary>
		///   search layout element that is the last one in its context
		/// </summary>
		End = 7
	}
}