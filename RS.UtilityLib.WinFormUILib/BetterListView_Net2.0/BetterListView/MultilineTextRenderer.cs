using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Measures and renders multi-line text using GDI and GDI+.
	/// </summary>
	internal static class MultilineTextRenderer
	{
		public const string Ellipsis = "…";

		private const TextFormatFlags DefaultTextFormatFlags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;

		private const TextAlignmentHorizontal DefaultTextAlignmentHorizontal = TextAlignmentHorizontal.Left;

		private const TextAlignmentVertical DefaultTextAlignmentVertical = TextAlignmentVertical.Top;

		private const int LineCountUndefined = -1;

		private static readonly Size maximumSize = new Size(int.MaxValue, int.MaxValue);

		private static void AdjustText(Graphics graphics, MultilineText multilineText, int width, out AdjustedText adjustedText, out int lineCount, out bool isTextShrunk) {
			if (width == 0 || multilineText.TextLines.Length == 0) {
				adjustedText = AdjustedText.Empty;
				lineCount = 1;
				isTextShrunk = false;
				return;
			}
			CachedAdjustedText cachedAdjustedText = multilineText.CachedAdjustedText;
			if (cachedAdjustedText.Width == width) {
				adjustedText = cachedAdjustedText.AdjustedText;
				lineCount = cachedAdjustedText.LineCount;
				isTextShrunk = cachedAdjustedText.IsTextShrunk;
				return;
			}
			switch (multilineText.TextTrimming) {
				case TextTrimming.None:
					MultilineTextRenderer.AdjustTextNone(multilineText, out adjustedText, out isTextShrunk, out lineCount);
					break;
				case TextTrimming.TrimCharacter:
					MultilineTextRenderer.AdjustTextCharacter(graphics, multilineText, string.Empty, width, out adjustedText, out isTextShrunk, out lineCount);
					break;
				case TextTrimming.TrimWord:
					MultilineTextRenderer.AdjustTextWord(graphics, multilineText, string.Empty, width, out adjustedText, out isTextShrunk, out lineCount);
					break;
				case TextTrimming.EllipsisCharacter:
					MultilineTextRenderer.AdjustTextCharacter(graphics, multilineText, "…", width, out adjustedText, out isTextShrunk, out lineCount);
					break;
				case TextTrimming.EllipsisWord:
					MultilineTextRenderer.AdjustTextWord(graphics, multilineText, "…", width, out adjustedText, out isTextShrunk, out lineCount);
					break;
				case TextTrimming.EllipsisPath:
					MultilineTextRenderer.AdjustTextPath(graphics, multilineText, width, out adjustedText, out isTextShrunk, out lineCount);
					break;
				default:
					throw new ApplicationException($"Unknown text trimming: '{multilineText.TextTrimming}'.");
			}
			multilineText.CachedAdjustedText = new CachedAdjustedText(width, adjustedText, lineCount, isTextShrunk);
		}

		private static void AdjustTextNone(MultilineText multilineText, out AdjustedText adjustedText, out bool isTextShrunk, out int lineCount) {
			if (multilineText.TextLines.Length == 1) {
				adjustedText = new AdjustedText(new string[1] { multilineText.TextLines[0].Text });
				lineCount = 1;
			}
			else {
				lineCount = multilineText.TextLines.Length;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < lineCount - 1; i++) {
					stringBuilder.AppendLine(multilineText.TextLines[i].Text);
				}
				stringBuilder.Append(multilineText.TextLines[lineCount - 1].Text);
				adjustedText = new AdjustedText(new string[1] { stringBuilder.ToString() });
			}
			isTextShrunk = false;
		}

		private static void AdjustTextPath(Graphics graphics, MultilineText multilineText, int width, out AdjustedText adjustedText, out bool isTextShrunk, out int lineCount) {
			int num = multilineText.MaximumTextLines;
			int num2 = multilineText.TextLines.Length;
			List<string> list = new List<string>();
			isTextShrunk = false;
			lineCount = 0;
			for (int i = 0; i < num2; i++) {
				TextLine textLine = multilineText.TextLines[i];
				string text = textLine.Text;
				MultilineText multilineText2 = new MultilineText(multilineText.Font, multilineText.TextTrimming, num, multilineText.TextOptions, new TextLine[1] { textLine }, verticalEllipsis: false, textLine.Width);
				MultilineTextRenderer.AdjustTextCharacter(graphics, multilineText2, string.Empty, width, out var adjustedText2, out var isTextShrunk2, out var lineCount2);
				if (isTextShrunk2) {
					int num3 = text.LastIndexOf(Path.DirectorySeparatorChar);
					string str;
					if (num3 == -1 || num3 == 0) {
						str = string.Empty;
					}
					else {
						str = "…" + text.Substring(num3, text.Length - num3);
						text = text.Substring(0, num3 + 1);
					}
					int num4 = 0;
					int num5 = text.Length - 1;
					int num6;
					while (true) {
						num6 = num5 + num4 >> 1;
						if ((num4 & 1) != (num5 & 1)) {
							num6++;
						}
						TextLine textLine2 = new TextLine(text.Substring(0, num6 + 1) + str);
						multilineText2 = new MultilineText(multilineText.Font, multilineText.TextTrimming, num, multilineText.TextOptions, new TextLine[1] { textLine2 }, verticalEllipsis: false, -1);
						MultilineTextRenderer.AdjustTextCharacter(graphics, multilineText2, string.Empty, width, out adjustedText2, out isTextShrunk2, out lineCount2);
						if (num6 == num5) {
							break;
						}
						if (!isTextShrunk2) {
							num4 = num6;
						}
						else {
							num5 = num6;
						}
					}
					if (isTextShrunk2 && num6 > 0) {
						num6--;
						TextLine textLine3 = new TextLine(text.Substring(0, num6 + 1) + str);
						multilineText2 = new MultilineText(multilineText.Font, multilineText.TextTrimming, num, multilineText.TextOptions, new TextLine[1] { textLine3 }, verticalEllipsis: false, -1);
						MultilineTextRenderer.AdjustTextCharacter(graphics, multilineText2, string.Empty, width, out adjustedText2, out isTextShrunk2, out lineCount2);
					}
					isTextShrunk = true;
				}
				else {
					isTextShrunk = false;
				}
				num -= lineCount2;
				lineCount += lineCount2;
				if (i == num2 - 1 || num == 0) {
					list.AddRange(adjustedText2.TextLines);
					break;
				}
			}
			adjustedText = new AdjustedText(list.ToArray());
		}

		private static void AdjustTextCharacter(Graphics graphics, MultilineText multilineText, string ellipsis, int width, out AdjustedText adjustedText, out bool isTextShrunk, out int lineCount) {
			bool flag = (multilineText.TextOptions & TextOptions.AllowWrap) == TextOptions.AllowWrap;
			List<string> list = new List<string>();
			isTextShrunk = false;
			string textRight = string.Empty;
			string textLeft;
			if (flag) {
				int num2 = 0;
				int num3 = 0;
				string text = multilineText.TextLines[0].Text;
				bool flag2 = false;
				while (num3 < multilineText.MaximumTextLines) {
					bool flag3 = num2 == multilineText.TextLines.Length - 1;
					flag2 = num3 == multilineText.MaximumTextLines - 1;
					MultilineTextRenderer.AdjustTextLineCharacter(graphics, multilineText.Font, (flag2 && (!flag3 || multilineText.VerticalEllipsis)) ? (text + ellipsis) : text, flag2 ? ellipsis : string.Empty, width, multilineText.TextOptions, out textLeft, out textRight);
					list.Add(textLeft);
					if ((flag3 && textRight.Length == 0) || flag2) {
						break;
					}
					num3++;
					if (textRight.Length != 0) {
						text = textRight;
						continue;
					}
					num2++;
					text = multilineText.TextLines[num2].Text;
				}
				isTextShrunk = (flag2 && multilineText.VerticalEllipsis) || textRight.Length != 0;
				lineCount = num3 + 1;
			}
			else {
				int num4 = Math.Min(multilineText.MaximumTextLines, multilineText.TextLines.Length);
				for (int num = 0; num < num4; num++) {
					bool flag4 = num == num4 - 1;
					TextLine textLine = multilineText.TextLines[num];
					if (textLine.Width != -1 && textLine.Width <= width) {
						textLeft = ((flag4 && multilineText.VerticalEllipsis) ? (textLine.Text + ellipsis) : textLine.Text);
					}
					else {
						MultilineTextRenderer.AdjustTextLineCharacter(graphics, multilineText.Font, (flag4 && multilineText.VerticalEllipsis) ? (textLine.Text + "…") : textLine.Text, ellipsis, width, multilineText.TextOptions, out textLeft, out textRight);
						isTextShrunk |= textRight.Length != 0;
					}
					list.Add(textLeft);
				}
				lineCount = num4;
			}
			adjustedText = new AdjustedText(list.ToArray());
		}

		private static void AdjustTextWord(Graphics graphics, MultilineText multilineText, string ellipsis, int width, out AdjustedText adjustedText, out bool isTextShrunk, out int lineCount) {
			bool flag = (multilineText.TextOptions & TextOptions.AllowWrap) == TextOptions.AllowWrap;
			List<string> list = new List<string>();
			isTextShrunk = false;
			string textRight = string.Empty;
			string textLeft;
			if (flag) {
				int num2 = 0;
				int num3 = 0;
				string text = multilineText.TextLines[0].Text;
				bool flag2 = false;
				while (num3 < multilineText.MaximumTextLines) {
					bool flag3 = num2 == multilineText.TextLines.Length - 1;
					flag2 = num3 == multilineText.MaximumTextLines - 1;
					MultilineTextRenderer.AdjustTextLineWord(graphics, multilineText.Font, (flag2 && (!flag3 || multilineText.VerticalEllipsis)) ? (text + ellipsis) : text, flag2 ? ellipsis : string.Empty, width, multilineText.TextOptions, out textLeft, out textRight);
					list.Add(textLeft);
					if ((flag3 && textRight.Length == 0) || flag2) {
						break;
					}
					num3++;
					if (textRight.Length != 0) {
						text = textRight;
						continue;
					}
					num2++;
					text = multilineText.TextLines[num2].Text;
				}
				isTextShrunk = (flag2 && multilineText.VerticalEllipsis) || textRight.Length != 0;
				lineCount = num3 + 1;
			}
			else {
				int num4 = Math.Min(multilineText.MaximumTextLines, multilineText.TextLines.Length);
				for (int num = 0; num < num4; num++) {
					bool flag4 = num == num4 - 1;
					TextLine textLine = multilineText.TextLines[num];
					if (textLine.Width != -1 && textLine.Width <= width) {
						textLeft = ((flag4 && multilineText.VerticalEllipsis) ? (textLine.Text + ellipsis) : textLine.Text);
					}
					else {
						MultilineTextRenderer.AdjustTextLineWord(graphics, multilineText.Font, (flag4 && multilineText.VerticalEllipsis) ? (textLine.Text + ellipsis) : textLine.Text, ellipsis, width, multilineText.TextOptions, out textLeft, out textRight);
						isTextShrunk |= textRight.Length != 0;
					}
					list.Add(textLeft);
				}
				lineCount = num4;
			}
			adjustedText = new AdjustedText(list.ToArray());
		}

		private static void AdjustTextLineCharacter(Graphics graphics, Font font, string text, string ellipsis, int width, TextOptions textOptions, out string textLeft, out string textRight) {
			if (MultilineTextRenderer.MeasureTextInternal(graphics, text, font, textOptions).Width <= width) {
				textLeft = text;
				textRight = string.Empty;
			}
			else {
				int trimmingPosition = MultilineTextRenderer.GetTrimmingPosition(graphics, font, text, ellipsis, width, textOptions);
				textLeft = text.Substring(0, trimmingPosition + 1) + ellipsis;
				textRight = text.Substring(trimmingPosition + 1, text.Length - trimmingPosition - 1).TrimStart();
			}
		}

		private static void AdjustTextLineWord(Graphics graphics, Font font, string text, string ellipsis, int width, TextOptions textOptions, out string textLeft, out string textRight) {
			if (MultilineTextRenderer.MeasureTextInternal(graphics, text, font, textOptions).Width <= width) {
				textLeft = text;
				textRight = string.Empty;
				return;
			}
			int num = MultilineTextRenderer.GetTrimmingPosition(graphics, font, text, ellipsis, width, textOptions);
			if (num != text.Length - 1) {
				for (int num2 = num; num2 >= 0; num2--) {
					if (!char.IsWhiteSpace(text[num2]) && char.IsWhiteSpace(text[num2 + 1])) {
						num = num2;
						break;
					}
				}
			}
			textLeft = text.Substring(0, num + 1) + ellipsis;
			textRight = text.Substring(num + 1, text.Length - num - 1).TrimStart();
		}

		private static int GetTrimmingPosition(Graphics graphics, Font font, string text, string ellipsis, int width, TextOptions textOptions) {
			int num = 0;
			int num2 = text.Length - 1;
			int num3;
			int width2;
			while (true) {
				num3 = num2 + num >> 1;
				if ((num & 1) != (num2 & 1)) {
					num3++;
				}
				string text2 = text.Substring(0, num3 + 1).Trim() + ellipsis;
				width2 = MultilineTextRenderer.MeasureTextInternal(graphics, text2, font, textOptions).Width;
				if (num3 == num2) {
					break;
				}
				if (width2 < width) {
					num = num3;
					continue;
				}
				if (width2 <= width) {
					break;
				}
				num2 = num3;
			}
			if (width2 > width && num3 > 0) {
				num3--;
			}
			return num3;
		}

		private static TextFormatFlags GetTextFormatFlags(bool hotkeyPrefix) {
			TextFormatFlags textFormatFlags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;
			if (!hotkeyPrefix) {
				textFormatFlags |= TextFormatFlags.NoPrefix;
			}
			return textFormatFlags;
		}

		private static StringFormat GetStringFormat(bool hotkeyPrefix) {
			StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			stringFormat.HotkeyPrefix = (hotkeyPrefix ? HotkeyPrefix.Show : HotkeyPrefix.None);
			return stringFormat;
		}

		/// <summary>
		/// Render multi-line text.
		/// </summary>
		/// <param name="graphics">Graphics object used for rendering.</param>
		/// <param name="color">Color of the text.</param>
		/// <param name="multilineText">Text to render.</param>
		/// <param name="bounds">Layout rectangle.</param>
		/// <param name="alignmentHorizontal">Horizontal text alignment.</param>
		/// <param name="alignmentVertical">Vertical text alignment.</param>
		public static void DrawText(Graphics graphics, Color color, MultilineText multilineText, Rectangle bounds, TextAlignmentHorizontal alignmentHorizontal, TextAlignmentVertical alignmentVertical) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(multilineText, "multilineText");
			Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "bounds.Width >= 0 && bounds.Height >= 0");
			bool hotkeyPrefix = (multilineText.TextOptions & TextOptions.HotkeyPrefix) == TextOptions.HotkeyPrefix;
			bool flag = (multilineText.TextOptions & TextOptions.UseGdiPlus) == TextOptions.UseGdiPlus;
			if (bounds.Width != 0) {
				if (alignmentHorizontal == TextAlignmentHorizontal.Default) {
					alignmentHorizontal = TextAlignmentHorizontal.Left;
				}
				if (alignmentVertical == TextAlignmentVertical.Default) {
					alignmentVertical = TextAlignmentVertical.Top;
				}
				MultilineTextRenderer.AdjustText(graphics, multilineText, bounds.Width, out var adjustedText, out var _, out var _);
				Size size = ((!flag) ? TextRenderer.MeasureText(graphics, adjustedText.ToString(), multilineText.Font, MultilineTextRenderer.maximumSize, TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform) : Size.Ceiling(graphics.MeasureString(adjustedText.ToString(), multilineText.Font, MultilineTextRenderer.maximumSize, StringFormat.GenericTypographic)));
				size = new Size(Math.Min(size.Width, bounds.Width), Math.Min(size.Height, bounds.Height));
				//int num = alignmentHorizontal switch {
				//	TextAlignmentHorizontal.Center => bounds.Left + (bounds.Width - size.Width >> 1),
				//	TextAlignmentHorizontal.Right => bounds.Right - size.Width,
				//	_ => bounds.Left,
				//};
				int num = 0;
				switch (alignmentHorizontal) {
					case TextAlignmentHorizontal.Center:
						num = bounds.Left + (bounds.Width - size.Width >> 1);
						break; 
					case TextAlignmentHorizontal.Left:
						num = bounds.Right - size.Width;
                        break;
					default:
						num = bounds.Left;
						break;
				}
				//int num2 = alignmentVertical switch {
				//	TextAlignmentVertical.Middle => bounds.Top + (bounds.Height - size.Height >> 1),
				//	TextAlignmentVertical.Bottom => bounds.Bottom - size.Height,
				//	_ => bounds.Top,
				//};
				int num2 = 0;
				switch (alignmentVertical) {
					case TextAlignmentVertical.Middle:
						num2 = bounds.Top + (bounds.Height - size.Height >> 1);
						break;
					case TextAlignmentVertical.Bottom:
						num2 = bounds.Bottom - size.Height;
                        break;
					default:
						num2 = bounds.Top;
                        break;
				}
				RectangleF clipBounds = graphics.ClipBounds;
				graphics.IntersectClip(bounds);
				if (flag) {
					StringFormat stringFormat = MultilineTextRenderer.GetStringFormat(hotkeyPrefix);
					Brush brush = new SolidBrush(color);
					graphics.DrawString(adjustedText.ToString(), multilineText.Font, brush, new RectangleF(num, num2, size.Width, 2.14748365E+09f), stringFormat);
					brush.Dispose();
					stringFormat.Dispose();
				}
				else {
					TextRenderer.DrawText(graphics, adjustedText.ToString(), multilineText.Font, new Rectangle(num, num2, size.Width, int.MaxValue), color, Color.Transparent, MultilineTextRenderer.GetTextFormatFlags(hotkeyPrefix));
				}
				graphics.SetClip(clipBounds);
			}
		}

		public static int GetLineCount(Graphics graphics, int width, MultilineText multilineText) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckNotNull(multilineText, "multilineText");
			if (!multilineText.CachedLineCount.IsEmpty && multilineText.CachedLineCount.Width == width) {
				return multilineText.CachedLineCount.LineCount;
			}
			int num = -1;
			if (width == 0 || multilineText.TextLines.Length == 0) {
				num = 1;
			}
			else if (multilineText.TextLines.Length == multilineText.MaximumTextLines) {
				num = multilineText.MaximumTextLines;
			}
			else if ((multilineText.TextOptions & TextOptions.AllowWrap) != TextOptions.AllowWrap) {
				num = multilineText.TextLines.Length;
			}
			else if (multilineText.CachedTextBreaks != null) {
				TextBreak[] cachedTextBreaks = multilineText.CachedTextBreaks;
				TextBreak[] array2 = cachedTextBreaks;
				foreach (TextBreak textBreak in array2) {
					if (width <= textBreak.Width) {
						num = textBreak.TextSize.LineCount;
						break;
					}
				}
			}
			if (num == -1 && multilineText.TextLines.Length == multilineText.MaximumTextLines - 1) {
				int num2 = multilineText.TextLines.Length - 1;
				TextLine[] array;
				if (num2 == 0) {
					array = multilineText.TextLines;
				}
				else {
					array = new TextLine[num2];
					Array.Copy(multilineText.TextLines, array, num2);
				}
				MultilineText multilineText2 = new MultilineText(multilineText.Font, multilineText.TextTrimming, multilineText.MaximumTextLines - 1, multilineText.TextOptions, array, multilineText.VerticalEllipsis, multilineText.CachedWidth);
				num = (MultilineTextRenderer.MeasureText(graphics, width, multilineText2).IsTextShrunk ? multilineText.MaximumTextLines : multilineText.TextLines.Length);
			}
			if (num == -1) {
				num = MultilineTextRenderer.MeasureText(graphics, width, multilineText).LineCount;
			}
			multilineText.CachedLineCount = new CachedLineCount(width, num);
			return num;
		}

		/// <summary>
		///   Measure a multi-line text.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="width">Maximum allowed text width.</param>
		/// <param name="multilineText">Multi-line text to measure.</param>
		/// <returns>Text size.</returns>
		public static TextSize MeasureText(Graphics graphics, int width, MultilineText multilineText) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckNotNull(multilineText, "multilineText");
			if (width == 0) {
				return new TextSize(0, TextSize.HeightFromLineCount(graphics, multilineText.Font, 1), 1, isTextShrunk: true);
			}
			bool hotkeyPrefix = (multilineText.TextOptions & TextOptions.HotkeyPrefix) == TextOptions.HotkeyPrefix;
			bool flag = (multilineText.TextOptions & TextOptions.UseGdiPlus) == TextOptions.UseGdiPlus;
			StringFormat stringFormat;
			TextFormatFlags flags;
			if (flag) {
				stringFormat = MultilineTextRenderer.GetStringFormat(hotkeyPrefix);
				flags = TextFormatFlags.Default;
			}
			else {
				stringFormat = null;
				flags = MultilineTextRenderer.GetTextFormatFlags(hotkeyPrefix);
			}
			if (multilineText.CachedWidth == -1) {
				multilineText.CachedWidth = 0;
				bool flag2 = (multilineText.TextTrimming == TextTrimming.EllipsisCharacter || multilineText.TextTrimming == TextTrimming.EllipsisWord || multilineText.TextTrimming == TextTrimming.EllipsisPath) && multilineText.VerticalEllipsis;
				TextLine[] textLines = multilineText.TextLines;
				TextLine[] array = textLines;
				foreach (TextLine textLine in array) {
					if (textLine.Text.Length == 0) {
						textLine.Width = 0;
						continue;
					}
					if (flag) {
						textLine.Width = (int)Math.Ceiling(graphics.MeasureString(flag2 ? (textLine.Text + "…") : textLine.Text, multilineText.Font, int.MaxValue, stringFormat).Width);
					}
					else {
						textLine.Width = TextRenderer.MeasureText(graphics, flag2 ? (textLine.Text + "…") : textLine.Text, multilineText.Font, MultilineTextRenderer.maximumSize, flags).Width;
					}
					multilineText.CachedWidth = Math.Max(multilineText.CachedWidth, textLine.Width);
				}
			}
			TextSize result;
			if (width >= multilineText.CachedWidth) {
				result = new TextSize(multilineText.CachedWidth, TextSize.HeightFromLineCount(graphics, multilineText.Font, multilineText.TextLines.Length), multilineText.TextLines.Length, isTextShrunk: false);
			}
			else {
				MultilineTextRenderer.AdjustText(graphics, multilineText, width, out var adjustedText, out var lineCount, out var isTextShrunk);
				int val = ((!flag) ? TextRenderer.MeasureText(graphics, adjustedText.ToString(), multilineText.Font, new Size(int.MaxValue, int.MaxValue), flags).Width : ((int)Math.Ceiling(graphics.MeasureString(adjustedText.ToString(), multilineText.Font, int.MaxValue, stringFormat).Width)));
				result = new TextSize(Math.Min(val, width), TextSize.HeightFromLineCount(graphics, multilineText.Font, lineCount), lineCount, isTextShrunk);
			}
			if (flag) {
				stringFormat.Dispose();
			}
			return result;
		}

		public static void MeasureTextBreaks(Graphics graphics, MultilineText multilineText) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(multilineText, "multilineText");
			List<TextBreak> list = new List<TextBreak>();
			bool flag = true;
			if (multilineText.TextLines.Length != 0) {
				TextLine[] textLines = multilineText.TextLines;
				TextLine[] array = textLines;
				foreach (TextLine textLine in array) {
					if (textLine.Text.Length != 0) {
						flag = false;
						break;
					}
				}
			}
			if (!flag) {
				TextSize textSize = MultilineTextRenderer.MeasureText(graphics, int.MaxValue, multilineText);
				if (multilineText.MaximumTextLines == 1) {
					list.Add(new TextBreak(textSize.Width, textSize, null));
				}
				else {
					TextBreak textBreak = new TextBreak(textSize.Width, textSize, null);
					TextBreak textBreak2 = new TextBreak(0, new TextSize(0, TextSize.HeightFromLineCount(graphics, multilineText.Font, multilineText.MaximumTextLines), multilineText.MaximumTextLines, isTextShrunk: true), textBreak);
					MultilineTextRenderer.MeasureTextBreaksInternal(graphics, multilineText, textBreak2, textBreak);
					TextBreak textBreak3 = textBreak2;
					TextBreak nextBreak = textBreak2.NextBreak;
					while (nextBreak != null) {
						textBreak3 = nextBreak;
						nextBreak = nextBreak.NextBreak;
						if (nextBreak == null || textBreak3.TextSize.Height != nextBreak.TextSize.Height || textBreak3.TextSize.IsTextShrunk != nextBreak.TextSize.IsTextShrunk) {
							list.Add(textBreak3);
						}
					}
				}
			}
			else {
				TextSize textSize2 = new TextSize(0, TextSize.HeightFromLineCount(graphics, multilineText.Font, 1), 1, isTextShrunk: false);
				list.Add(new TextBreak(0, textSize2, null));
			}
			multilineText.CachedTextBreaks = list.ToArray();
		}

		/// <summary>
		/// Check whether the specified location lays within the specified multiline text.
		/// </summary>
		/// <param name="graphics">Graphics object used for measurement.</param>
		/// <param name="multilineText">Multiline text.</param>
		/// <param name="bounds">Multiline text boundaries.</param>
		/// <param name="point">Location to check intersection for.</param>
		/// <returns>The specified location lays within the specified multiline text.</returns>
		public static bool CheckIntersection(Graphics graphics, MultilineText multilineText, Rectangle bounds, Point point) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(multilineText, "multilineText");
			Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "bounds.Width >= 0 && bounds.Height >= 0");
			if (!bounds.Contains(point)) {
				return false;
			}
			int num = TextSize.HeightFromLineCount(graphics, multilineText.Font, 1);
			int num2 = (point.Y - bounds.Top) / num;
			if (num2 < 0 || num2 >= multilineText.CachedAdjustedText.LineCount) {
				return false;
			}
			int width = MultilineTextRenderer.MeasureTextInternal(graphics, multilineText.CachedAdjustedText.AdjustedText.TextLines[num2], multilineText.Font, multilineText.TextOptions).Width;
			return point.X - bounds.Left < width;
		}

		private static Size MeasureTextInternal(Graphics graphics, string text, Font font, TextOptions textOptions) {
			bool flag = (textOptions & TextOptions.UseGdiPlus) == TextOptions.UseGdiPlus;
			bool hotkeyPrefix = (textOptions & TextOptions.HotkeyPrefix) == TextOptions.HotkeyPrefix;
			Size result;
			if (flag) {
				StringFormat stringFormat = MultilineTextRenderer.GetStringFormat(hotkeyPrefix);
				result = Size.Ceiling(graphics.MeasureString(text, font, int.MaxValue, stringFormat));
				stringFormat.Dispose();
			}
			else {
				result = TextRenderer.MeasureText(graphics, text, font, MultilineTextRenderer.maximumSize, MultilineTextRenderer.GetTextFormatFlags(hotkeyPrefix));
			}
			return result;
		}

		private static void MeasureTextBreaksInternal(Graphics graphics, MultilineText multilineText, TextBreak textBreakStart, TextBreak textBreakEnd) {
			int num = textBreakStart.Width + textBreakEnd.Width >> 1;
			if (num != textBreakStart.Width && (textBreakStart.TextSize.LineCount != textBreakEnd.TextSize.LineCount || textBreakStart.TextSize.IsTextShrunk != textBreakEnd.TextSize.IsTextShrunk)) {
				TextSize textSize = MultilineTextRenderer.MeasureText(graphics, num, multilineText);
				TextBreak textBreak4 = (textBreakStart.NextBreak = new TextBreak(num, textSize, textBreakEnd));
				TextBreak textBreak2 = textBreak4;
				if (textSize.Height != textBreakStart.TextSize.Height || !textSize.IsTextShrunk) {
					MultilineTextRenderer.MeasureTextBreaksInternal(graphics, multilineText, textBreakStart, textBreak2);
				}
				if (textSize.Height != textBreakEnd.TextSize.Height && !textBreakEnd.TextSize.IsTextShrunk) {
					MultilineTextRenderer.MeasureTextBreaksInternal(graphics, multilineText, textBreak2, textBreakEnd);
				}
			}
		}
	}
}