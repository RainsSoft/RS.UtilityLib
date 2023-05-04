using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents single line of a multi-line text.
	/// </summary>
	internal sealed class TextLine
	{
		/// <summary>
		///   Width of text which is not yet measured.
		/// </summary>
		public const int WidthUndefined = -1;

		private readonly string text;

		private int width = -1;

		/// <summary>
		///   String containing single line of a multi-line text.
		/// </summary>
		public string Text => this.text;

		/// <summary>
		///   Width of the text line represented by this instance.
		/// </summary>
		public int Width {
			get {
				return this.width;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.width = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.TextLine" /> class.
		/// </summary>
		/// <param name="text">String containing single line of a multi-line text.</param>
		public TextLine(string text) {
			Checks.CheckNotNull(text, "text");
			foreach (char c in text) {
				if (c == '\r' || c == '\n') {
					throw new ArgumentException("TextLine instance should not contain text with CR or LF characters.");
				}
			}
			this.text = text;
		}
	}
}