using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Label editing activation methods.
	/// </summary>
	[Flags]
	public enum BetterListViewLabelEditActivation
	{
		/// <summary>
		///   default label editing activation behavior
		/// </summary>
		Default = 0,
		/// <summary>
		///   perform label editing by pressing F2 key
		/// </summary>
		Keyboard = 1,
		/// <summary>
		///   perform label editing after just single click on the item/sub-item text
		/// </summary>
		SingleClick = 2,
		/// <summary>
		///   perform label editing immediately after clicking in the item/sub-item text
		/// </summary>
		Immediate = 4,
		/// <summary>
		///   perform label editing even when user clicks on unfocused control
		/// </summary>
		ClickThrough = 8
	}
}