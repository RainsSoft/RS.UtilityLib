namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Type of check box state change.
	/// </summary>
	public enum BetterListViewCheckStateChangeMode
	{
		/// <summary>
		///   check state changed by keyboard
		/// </summary>
		Keyboard = 0,
		/// <summary>
		///   check state changed by mouse
		/// </summary>
		Mouse = 1,
		/// <summary>
		///   check state changed from user code
		/// </summary>
		UserCode = 2,
		/// <summary>
		///   cehck state change mode not defined
		/// </summary>
		Undefined = 3
	}
}