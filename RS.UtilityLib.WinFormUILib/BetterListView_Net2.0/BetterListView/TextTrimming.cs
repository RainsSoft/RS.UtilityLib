namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Text trimming method.
	/// </summary>
	public enum TextTrimming
	{
		/// <summary>
		///   No trimming.
		/// </summary>
		None = 0,
		/// <summary>
		///   Trim text on whole characters.
		/// </summary>
		TrimCharacter = 1,
		/// <summary>
		///   Trim text on whole words.
		/// </summary>
		TrimWord = 2,
		/// <summary>
		///   Trim text on whole characters; adding ellipsis character at the end of the text.
		/// </summary>
		EllipsisCharacter = 3,
		/// <summary>
		///   Trim text on whole words; adding ellipsis character at the end of the text.
		/// </summary>
		EllipsisWord = 4,
		/// <summary>
		///   Trim text with respect to path representation.
		/// </summary>
		EllipsisPath = 5,
		/// <summary>
		///   Trimming not defined.
		/// </summary>
		Undefined = 6
	}
}