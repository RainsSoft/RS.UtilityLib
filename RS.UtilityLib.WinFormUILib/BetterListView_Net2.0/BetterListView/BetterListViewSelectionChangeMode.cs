namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type of selection change.
	/// </summary>
	public enum BetterListViewSelectionChangeMode
	{
		/// <summary>
		///   selection changed by means of accessibility aids
		/// </summary>
		Accessiblity = 0,
		/// <summary>
		///   selection changed by collapsing item with children
		/// </summary>
		Collapse = 1,
		/// <summary>
		///   selection changed by data binding logic
		/// </summary>
		DataBinding = 2,
		/// <summary>
		///   selection changed during Drag and Drop operation
		/// </summary>
		DragDrop = 3,
		/// <summary>
		///   selection changed by keyboard
		/// </summary>
		Keyboard = 4,
		/// <summary>
		///   selection changed by mouse button
		/// </summary>
		MouseButton = 5,
		/// <summary>
		///   selection changed by mouse drag selecting
		/// </summary>
		MouseDragSelection = 6,
		/// <summary>
		///   selection changed after mouse drag selection
		/// </summary>
		MouseDragSelectionEnd = 7,
		/// <summary>
		///   selection changed due to property change
		/// </summary>
		PropertyChanged = 8,
		/// <summary>
		///   selection changed from user code
		/// </summary>
		UserCode = 9,
		/// <summary>
		///   selection change mode not defined
		/// </summary>
		Undefined = 10
	}
}