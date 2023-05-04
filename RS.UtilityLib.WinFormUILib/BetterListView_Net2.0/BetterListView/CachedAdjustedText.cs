namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents adjusted text information for some specific width.
	/// </summary>
	internal struct CachedAdjustedText
	{
		/// <summary>
		///   Represents an empty CachedAdjustedText structure.
		/// </summary>
		public static readonly CachedAdjustedText Empty = new CachedAdjustedText(0, AdjustedText.Empty, 1, isTextShrunk: false);

		private readonly int width;

		private readonly AdjustedText adjustedText;

		private readonly int lineCount;

		private readonly bool isTextShrunk;

		/// <summary>
		///   Width for which the text has been adjusted.
		/// </summary>
		public int Width => this.width;

		/// <summary>
		///   Text adjusted for rendering.
		/// </summary>
		public AdjustedText AdjustedText => this.adjustedText;

		/// <summary>
		///   Number of text lines.
		/// </summary>
		public int LineCount => this.lineCount;

		/// <summary>
		///   The text is displayed with ellipsis.
		/// </summary>
		public bool IsTextShrunk => this.isTextShrunk;

		/// <summary>
		///   This CachedAdjustedText structure is empty.
		/// </summary>
		public bool IsEmpty => this.Equals(CachedAdjustedText.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.CachedAdjustedText" /> struct.
		/// </summary>
		/// <param name="width">Width for which the text has been adjusted.</param>
		/// <param name="adjustedText">Text adjusted for rendering.</param>
		/// <param name="lineCount">Number of text lines.</param>
		/// <param name="isTextShrunk">Represents an empty CachedAdjustedText structure.</param>
		public CachedAdjustedText(int width, AdjustedText adjustedText, int lineCount, bool isTextShrunk) {
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckNotNull(adjustedText, "text");
			Checks.CheckTrue(lineCount > 0, "lineCount > 0");
			this.width = width;
			this.adjustedText = adjustedText;
			this.lineCount = lineCount;
			this.isTextShrunk = isTextShrunk;
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is CachedAdjustedText cachedAdjustedText)) {
				return false;
			}
			if (this.width == cachedAdjustedText.width) {
				AdjustedText adjustedText = this.adjustedText;
				if (adjustedText.Equals(cachedAdjustedText.adjustedText) && this.lineCount == cachedAdjustedText.lineCount) {
					return this.isTextShrunk == cachedAdjustedText.isTextShrunk;
				}
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			int hashCode = this.width.GetHashCode();
			AdjustedText adjustedText = this.adjustedText;
			return hashCode ^ adjustedText.GetHashCode() ^ this.lineCount.GetHashCode() ^ this.isTextShrunk.GetHashCode();
		}
	}
}