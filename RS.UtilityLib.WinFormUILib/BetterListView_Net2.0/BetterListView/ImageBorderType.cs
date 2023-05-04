namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Image border style.
	/// </summary>
	public enum ImageBorderType
	{
		/// <summary>
		///   no border
		/// </summary>
		None = 0,
		/// <summary>
		///   single line border
		/// </summary>
		Single = 1,
		/// <summary>
		///   single line border with offset from center
		/// </summary>
		SingleOffset = 2,
		/// <summary>
		///   shadow shown on all edges
		/// </summary>
		SymmetricShadow = 3,
		/// <summary>
		///   shadow shown on right and bottom edges
		/// </summary>
		DropShadow = 4,
		/// <summary>
		///   border style not defined
		/// </summary>
		Undefined = 5
	}
}