namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Way of checking collection element.
	/// </summary>
	internal enum ElementCheckType
	{
		/// <summary>
		///   element should be contained in this collection
		/// </summary>
		ContainedInThis = 0,
		/// <summary>
		///   element should not be contained in this collection
		/// </summary>
		NotContainedInThis = 1,
		/// <summary>
		///   element should not be contained in other collection
		/// </summary>
		NotContainedInOther = 2,
		/// <summary>
		///   element should not be contained in any collection
		/// </summary>
		NotContainedInAny = 3
	}
}