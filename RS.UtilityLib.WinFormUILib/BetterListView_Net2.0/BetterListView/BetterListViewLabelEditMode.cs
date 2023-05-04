namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Label editing mode.
	/// </summary>
	public enum BetterListViewLabelEditMode
	{
		/// <summary>
		///   no label editing
		/// </summary>
		None = 0,
		/// <summary>
		///   textual editing with internal editor
		/// </summary>
		Text = 1,
		/// <summary>
		///   editing with custom control
		/// </summary>
		CustomControl = 2
	}
}