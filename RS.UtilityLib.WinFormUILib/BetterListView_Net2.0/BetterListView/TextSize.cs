using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents size of a text described by width and number of lines.
	/// </summary>
	internal struct TextSize : IEquatable<TextSize>
	{
		/// <summary>
		///   Represents an empty TextSize structure.
		/// </summary>
		public static readonly TextSize Empty = new TextSize(0, 0, 0, isTextShrunk: false);

		private readonly int width;

		private readonly int height;

		private readonly int lineCount;

		private readonly bool isTextShrunk;

		/// <summary>
		///   Text width (in pixels).
		/// </summary>
		public int Width => this.width;

		/// <summary>
		/// Text height (in pixels).
		/// </summary>
		public int Height => this.height;

		/// <summary>
		///   Number of text lines.
		/// </summary>
		public int LineCount => this.lineCount;

		/// <summary>
		/// The text is too long to be displayed whole.
		/// </summary>
		public bool IsTextShrunk => this.isTextShrunk;

		/// <summary>
		///   This TextSize structure is empty.
		/// </summary>
		public bool IsEmpty => this.Equals(TextSize.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.TextSize" /> struct.
		/// </summary>
		/// <param name="width">Text width (in pixels).</param>
		/// <param name="height">Text height (in pixels).</param>
		/// <param name="lineCount">Number of text lines.</param>
		/// <param name="isTextShrunk">The text is too long to be displayed whole.</param>
		public TextSize(int width, int height, int lineCount, bool isTextShrunk) {
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckTrue(height >= 0, "height >= 0");
			Checks.CheckTrue(lineCount >= 0, "lineCount >= 0");
			this.width = width;
			this.height = height;
			this.lineCount = lineCount;
			this.isTextShrunk = isTextShrunk;
		}

		/// <summary>
		/// Test whether the two TextSize instances are identical.
		/// </summary>
		/// <param name="textSizeA">The first TextSize instance.</param>
		/// <param name="textSizeB">The second TextSize instance.</param>
		/// <returns>The two TextSize instances are identical.</returns>
		public static bool operator ==(TextSize textSizeA, TextSize textSizeB) {
			return ((IEquatable<TextSize>)textSizeA).Equals(textSizeB);
		}

		/// <summary>
		/// Test whether the two TextSize instances are different.
		/// </summary>
		/// <param name="textSizeA">The first TextSize instance.</param>
		/// <param name="textSizeB">The second TextSize instance.</param>
		/// <returns>The two TextSize instances are different.</returns>
		public static bool operator !=(TextSize textSizeA, TextSize textSizeB) {
			return !((IEquatable<TextSize>)textSizeA).Equals(textSizeB);
		}

		/// <summary>
		/// Get text height from known line count and the specified font.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="font">Font of the text.</param>
		/// <param name="lineCount">Number of text lines.</param>
		/// <returns>Height of the text (in pixels).</returns>
		public static int HeightFromLineCount(Graphics graphics, Font font, int lineCount) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(font, "font");
			Checks.CheckTrue(lineCount >= 0, "lineCount >= 0");
			return lineCount * TextRenderer.MeasureText(graphics, "Ay", font).Height;
		}

		/// <summary>
		/// Get text height from known line count and the specified font height.
		/// </summary>
		/// <param name="fontHeight">Used font height (in pixel).</param>
		/// <returns>Height of the text (in pixels).</returns>
		/// <param name="lineCount">Number of text lines.</param>
		public static int HeightFromLineCount(int fontHeight, int lineCount) {
			Checks.CheckTrue(fontHeight > 0, "fontHeight > 0");
			Checks.CheckTrue(lineCount >= 0, "lineCount >= 0");
			return lineCount * fontHeight;
		}

		/// <summary>
		/// Get text height from known line count and the specified font.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="font">Font of the text.</param>
		/// <param name="height">Text height (in pixels).</param>
		/// <returns>Number of text lines.</returns>
		public static int HeightToLineCount(Graphics graphics, Font font, int height) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(font, "font");
			Checks.CheckTrue(height >= 0, "height >= 0");
			return Math.Max(height / TextRenderer.MeasureText(graphics, "Ay", font).Height, 1);
		}

		/// <summary>
		/// Get text height from known line count and the specified font height.
		/// </summary>
		/// <param name="fontHeight">Used font height (in pixel).</param>
		/// <param name="height">Text height (in pixels).</param>
		/// <returns>Number of text lines.</returns>
		public static int HeightToLineCount(int fontHeight, int height) {
			Checks.CheckTrue(fontHeight > 0, "fontHeight > 0");
			Checks.CheckTrue(height >= 0, "height >= 0");
			return Math.Max(height / fontHeight, 1);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is TextSize)) {
				return false;
			}
			return ((IEquatable<TextSize>)this).Equals((TextSize)obj);
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.width.GetHashCode() ^ this.height.GetHashCode() ^ this.lineCount.GetHashCode() ^ this.isTextShrunk.GetHashCode();
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   true if the current object is equal to the other parameter; otherwise, false.
		/// </returns>
		bool IEquatable<TextSize>.Equals(TextSize other) {
			if (this.width == other.width && this.height == other.height && this.lineCount == other.lineCount) {
				return this.isTextShrunk == other.isTextShrunk;
			}
			return false;
		}
	}
}