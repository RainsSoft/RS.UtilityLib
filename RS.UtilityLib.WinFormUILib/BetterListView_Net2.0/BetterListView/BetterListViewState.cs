namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type of a BetterListView state.
	/// </summary>
	public enum BetterListViewState
	{
		/// <summary>
		///   column header is being selected
		/// </summary>
		ColumnSelection = 0,
		/// <summary>
		///   column header is in state before being resized
		/// </summary>
		ColumnBeforeResize = 1,
		/// <summary>
		///   column header is being resized
		/// </summary>
		ColumnResize = 2,
		/// <summary>
		///   column headers are being reordered
		/// </summary>
		ColumnReorder = 3,
		/// <summary>
		///   group is in state before being selected
		/// </summary>
		GroupBeforeSelection = 4,
		/// <summary>
		///   normal (default) state
		/// </summary>
		Normal = 5,
		/// <summary>
		///   items are in state before their check state being changed using keyboard
		/// </summary>
		ItemBeforeCheckKeyboard = 6,
		/// <summary>
		///   items are in state before their check state being changed using mouse
		/// </summary>
		ItemBeforeCheckMouse = 7,
		/// <summary>
		///   items are in state before being dragged
		/// </summary>
		ItemBeforeDrag = 8,
		/// <summary>
		///   item is in state before label editing
		/// </summary>
		ItemBeforeLabelEdit = 9,
		/// <summary>
		///   items are in state before being selected
		/// </summary>
		ItemBeforeSelection = 10,
		/// <summary>
		///   items are being dragged
		/// </summary>
		ItemDrag = 11,
		/// <summary>
		///   items are being selected
		/// </summary>
		ItemSelection = 12,
		/// <summary>
		///   item/sub-item label is about to be edited
		/// </summary>
		LabelEditInit = 13,
		/// <summary>
		///   item/sub-item label is being edited
		/// </summary>
		LabelEdit = 14
	}
}