using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Reasons why item reorder has been invalidated.
	/// </summary>
	[Flags]
	public enum BetterListViewItemReorderInvalidationReasons
	{
		/// <summary>
		/// Item reorder is valid.
		/// </summary>
		None = 0,
		/// <summary>
		/// Item reorder is invalid in case of data binding when other than top-level item is being reordered.
		/// </summary>
		NotTopLevel = 1,
		/// <summary>
		/// Item is being reordered under different parent when SameParentOnly item reordering option is turned on.
		/// </summary>
		NotSameParent = 2,
		/// <summary>
		/// Item is being reordered under itself.
		/// </summary>
		PutInItself = 4,
		/// <summary>
		/// Item is being reordered in different group while changing groups is not allowed.
		/// </summary>
		AnotherGroup = 8
	}
}