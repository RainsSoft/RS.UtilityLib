namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a selectable layout element.
	/// </summary>
	internal interface IBetterListViewLayoutElementSelectable
	{
		/// <summary>
		///   Index of the element within some layout.
		/// </summary>
		int LayoutIndexSelection { get; set; }
	}
}