using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Part of an element mouse pointer is located on.
	/// </summary>
	[Flags]
	public enum BetterListViewHitPart
	{
		/// <summary>
		///   complement to the specified specified parts
		/// </summary>
		Other = 0,
		/// <summary>
		///   bottom part of an element
		/// </summary>
		Bottom = 1,
		/// <summary>
		///   right part of an element
		/// </summary>
		Right = 2,
		/// <summary>
		///   center part of an element (vertical orientation)
		/// </summary>
		VCenter = 4,
		/// <summary>
		///   center part of an element (horizontal orientation)
		/// </summary>
		HCenter = 8,
		/// <summary>
		///   part is not defined
		/// </summary>
		Undefined = 0x10
	}
}