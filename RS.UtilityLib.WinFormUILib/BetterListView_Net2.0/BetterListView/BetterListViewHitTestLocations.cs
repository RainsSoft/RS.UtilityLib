using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Locations of mouse pointer on a single position.
	/// </summary>
	[Flags]
	public enum BetterListViewHitTestLocations
	{
		/// <summary>
		///   custom pointer location
		/// </summary>
		Custom = 0,
		/// <summary>
		///   pointer located on column area (area for column headers)
		/// </summary>
		ColumnArea = 1,
		/// <summary>
		///   pointer located on content area (area for items and groups)
		/// </summary>
		ContentArea = 2,
		/// <summary>
		///   pointer located on column header
		/// </summary>
		ColumnHeader = 4,
		/// <summary>
		///   pointer located on column header image
		/// </summary>
		ColumnHeaderImage = 8,
		/// <summary>
		///   pointer located on column header text
		/// </summary>
		ColumnHeaderText = 0x10,
		/// <summary>
		///   pointer located on column header border
		/// </summary>
		ColumnHeaderBorder = 0x20,
		/// <summary>
		///   pointer located on column header sort glyph
		/// </summary>
		ColumnHeaderSortGlyph = 0x40,
		/// <summary>
		///   pointer located on an item
		/// </summary>
		Item = 0x80,
		/// <summary>
		///   pointer located on an item selection area
		/// </summary>
		ItemSelection = 0x100,
		ItemCheckBox = 0x400,
		/// <summary>
		///   pointer located on an item image
		/// </summary>
		ItemImage = 0x800,
		/// <summary>
		///   pointer located on a sub-item text
		/// </summary>
		ItemText = 0x1000,
		/// <summary>
		///   pointer located on a sub-item
		/// </summary>
		SubItem = 0x2000,
		/// <summary>
		///   pointer located on a sub-item image
		/// </summary>
		SubItemImage = 0x4000,
		/// <summary>
		///   pointer located on a sub-item text
		/// </summary>
		SubItemText = 0x8000,
		/// <summary>
		///   pointer located on a group
		/// </summary>
		Group = 0x10000,
		/// <summary>
		///   pointer located on a group image
		/// </summary>
		GroupImage = 0x40000,
		/// <summary>
		///   pointer located on a group text
		/// </summary>
		GroupText = 0x80000
	}
}