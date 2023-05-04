using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents text line count for some specific width.
	/// </summary>
	internal struct CachedLineCount : IEquatable<CachedLineCount>
	{
		/// <summary>
		///   Represents an empt CachedLineCount instance.
		/// </summary>
		public static readonly CachedLineCount Empty = new CachedLineCount(0, 1);

		private readonly int width;

		private readonly int lineCount;

		/// <summary>
		///   Text width.
		/// </summary>
		public int Width => this.width;

		/// <summary>
		///   Line count corresponding to the text width.
		/// </summary>
		public int LineCount => this.lineCount;

		/// <summary>
		///   This CachedLineCount instance is empty.
		/// </summary>
		public bool IsEmpty => this.Equals(CachedLineCount.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.CachedLineCount" /> struct.
		/// </summary>
		/// <param name="width">Text width.</param>
		/// <param name="lineCount">Line count corresponding to the text width.</param>
		public CachedLineCount(int width, int lineCount) {
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckTrue(lineCount > 0, "lineCount > 0");
			this.width = width;
			this.lineCount = lineCount;
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is CachedLineCount)) {
				return false;
			}
			return ((IEquatable<CachedLineCount>)this).Equals((CachedLineCount)obj);
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.width.GetHashCode() ^ this.lineCount.GetHashCode();
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   true if the current object is equal to the other parameter; otherwise, false.
		/// </returns>
		bool IEquatable<CachedLineCount>.Equals(CachedLineCount other) {
			if (this.width == other.width) {
				return this.lineCount == other.lineCount;
			}
			return false;
		}
	}
}