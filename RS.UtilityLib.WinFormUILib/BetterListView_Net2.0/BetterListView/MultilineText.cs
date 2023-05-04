using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a multi-line text.
	/// </summary>
	internal sealed class MultilineText : IDisposable
	{
		private const int WidthUndefined = -1;

		private readonly Font font;

		private readonly TextTrimming textTrimming;

		private readonly int maximumTextLines;

		private readonly TextOptions textOptions;

		private readonly TextLine[] textLines;

		private readonly bool verticalEllipsis;

		private CachedLineCount cachedLineCount = CachedLineCount.Empty;

		private CachedAdjustedText cachedAdjustedText = CachedAdjustedText.Empty;

		private int cachedWidth = -1;

		private TextBreak[] cachedTextBreaks;

		private bool isDisposed;

		/// <summary>
		///   Font to use for measurement and rendering of the text.
		/// </summary>
		public Font Font => this.font;

		/// <summary>
		///   Trimming method to use for measurement and rendering of the text.
		/// </summary>
		public TextTrimming TextTrimming => this.textTrimming;

		/// <summary>
		///   Maximum allowed number of text lines.
		/// </summary>
		public int MaximumTextLines => this.maximumTextLines;

		/// <summary>
		///   Text measurement and rendering options.
		/// </summary>
		public TextOptions TextOptions => this.textOptions;

		/// <summary>
		///   Individual lines of the multi-line text.
		/// </summary>
		internal TextLine[] TextLines => this.textLines;

		/// <summary>
		///   Original text contains more lines of text than allowed (see MaximumTextLines property) so the ellipsis should be displayed.
		/// </summary>
		internal bool VerticalEllipsis => this.verticalEllipsis;

		/// <summary>
		///   Cached adjusted text to speed up repeated rendering.
		/// </summary>
		internal CachedAdjustedText CachedAdjustedText {
			get {
				return this.cachedAdjustedText;
			}
			set {
				this.cachedAdjustedText = value;
			}
		}

		/// <summary>
		///   Cached text line count to speed up repeated measurement.
		/// </summary>
		internal CachedLineCount CachedLineCount {
			get {
				return this.cachedLineCount;
			}
			set {
				this.cachedLineCount = value;
			}
		}

		/// <summary>
		///   Cached width of the longest line of the text.
		/// </summary>
		internal int CachedWidth {
			get {
				return this.cachedWidth;
			}
			set {
				this.cachedWidth = value;
			}
		}

		internal TextBreak[] CachedTextBreaks {
			get {
				return this.cachedTextBreaks;
			}
			set {
				Checks.CheckCollection(value, "value");
				this.cachedTextBreaks = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.MultilineText" /> class.
		/// </summary>
		/// <param name="originalText">Original text possibly containing newline characters.</param>
		/// <param name="font">Font to use for measurement and rendering of the text.</param>
		/// <param name="textTrimming">Trimming method to use for measurement and rendering of the text.</param>
		/// <param name="maximumTextLines">Maximum allowed number of text lines.</param>
		/// <param name="textOptions">Text measurement and rendering options.</param>
		public MultilineText(string originalText, Font font, TextTrimming textTrimming, int maximumTextLines, TextOptions textOptions) {
			Checks.CheckNotNull(originalText, "originalText");
			Checks.CheckNotNull(font, "font");
			Checks.CheckNotEqual(textTrimming, TextTrimming.Undefined, "textTrimming", "TextTrimming.Undefined");
			Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
			this.font = font;
			this.textTrimming = textTrimming;
			this.maximumTextLines = maximumTextLines;
			this.textOptions = textOptions;
			List<TextLine> list = new List<TextLine>();
			StringBuilder stringBuilder = new StringBuilder(originalText.Length);
			this.verticalEllipsis = false;
			for (int i = 0; i < originalText.Length; i++) {
				char c = originalText[i];
				switch (c) {
					case '\n':
						if (list.Count != maximumTextLines) {
							list.Add(new TextLine(stringBuilder.ToString().Trim()));
							stringBuilder.Length = 0;
							continue;
						}
						break;
					default:
						stringBuilder.Append(c);
						continue;
					case '\r':
						continue;
				}
				this.verticalEllipsis = i < originalText.Length - 1;
				break;
			}
			if (stringBuilder.Length != 0) {
				if (list.Count < maximumTextLines) {
					list.Add(new TextLine(stringBuilder.ToString().Trim()));
				}
				else {
					this.verticalEllipsis = true;
				}
			}
			this.textLines = list.ToArray();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.MultilineText" /> class.
		/// </summary>
		/// <param name="font">Font to use for measurement and rendering of the text.</param>
		/// <param name="textTrimming">Trimming method to use for measurement and rendering of the text.</param>
		/// <param name="maximumTextLines">Maximum allowed number of text lines.</param>
		/// <param name="textOptions">Text measurement and rendering options.</param>
		/// <param name="textLines">Individual lines of the multi-line text.</param>
		/// <param name="verticalEllipsis">Original text contains more lines of text than allowed (see MaximumTextLines property) so the ellipsis should be displayed.</param>
		/// <param name="cachedWidth">Cached width of the longest line of the text.</param>
		internal MultilineText(Font font, TextTrimming textTrimming, int maximumTextLines, TextOptions textOptions, TextLine[] textLines, bool verticalEllipsis, int cachedWidth) {
			this.font = font;
			this.textTrimming = textTrimming;
			this.maximumTextLines = maximumTextLines;
			this.textOptions = textOptions;
			this.textLines = textLines;
			this.verticalEllipsis = verticalEllipsis;
			this.cachedWidth = cachedWidth;
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (this.isDisposed) {
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			this.font.Dispose();
			this.isDisposed = true;
		}
	}
}