namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Cause of item activation.
	/// </summary>
	public enum BetterListViewItemActivationSource
	{
		/// <summary>
		///   Item has been activated using assistive technology.
		/// </summary>
		Accessibility = 0,
		/// <summary>
		///   Item has been activated using keyboard.
		/// </summary>
		Keyboard = 1,
		/// <summary>
		///   Item has been activated using mouse.
		/// </summary>
		Mouse = 2,
		/// <summary>
		///   Item has been activated from user code.
		/// </summary>
		User = 3
	}
}
