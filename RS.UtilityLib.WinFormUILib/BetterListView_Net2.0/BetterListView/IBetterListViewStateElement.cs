namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   An element with state assigned.
	/// </summary>
	internal interface IBetterListViewStateElement
	{
		/// <summary>
		/// Gets a value indicating whether this element is in active state.
		/// </summary>
		/// <value>
		///   <c>true</c> if this element is in active state; otherwise, <c>false</c>.
		/// </value>
		bool IsActive { get; }

		/// <summary>
		///   Current state of the element.
		/// </summary>
		BetterListViewElementState State { get; }

		/// <summary>
		///   Change state of the element with the specified state transition.
		/// </summary>
		/// <param name="stateChange">State transition.</param>
		void ChangeState(BetterListViewElementStateChange stateChange);
	}
}