using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	/// Common Windows Forms utilities.
	/// </summary>
	public static class WinFormsUtils
	{
		private const float DefaultFontSize = 9f;

		private const string FontNameSegoe = "Segoe UI";

		private const string FontNameTrebuchet = "Trebuchet MS";

		private const string FontNameTahoma = "Tahoma";

		private const string FontNameVerdana = "Verdana";

		private static bool fontSegoeExists = true;

		private static bool fontTrebuchetExists = true;

		private static bool fontTahomaExists = true;

		private static bool fontVerdanaExists = true;

		/// <summary>
		/// Create a new Font instance with modern and supported font.
		/// </summary>
		public static Font CreateFont() {
			return WinFormsUtils.CreateFont(9f, FontStyle.Regular);
		}

		/// <summary>
		/// Create a new Font instance with modern and supported font.
		/// </summary>
		/// <param name="size">Font size.</param>
		/// <returns>Font instance.</returns>
		public static Font CreateFont(float size) {
			return WinFormsUtils.CreateFont(size, FontStyle.Regular);
		}

		/// <summary>
		/// Create a new Font instance with modern and supported font.
		/// </summary>
		/// <param name="style">Font style.</param>
		/// <returns>Font instance.</returns>
		public static Font CreateFont(FontStyle style) {
			return WinFormsUtils.CreateFont(9f, style);
		}

		/// <summary>
		/// Create a new Font instance with modern and supported font.
		/// </summary>
		/// <param name="size">Font size.</param>
		/// <param name="style">Font style.</param>
		/// <returns>Font instance.</returns>
		public static Font CreateFont(float size, FontStyle style) {
			if ((WinFormsUtils.fontSegoeExists && WinFormsUtils.TryCreateFont("Segoe UI", size, style, out var font, out WinFormsUtils.fontSegoeExists)) || (WinFormsUtils.fontTrebuchetExists && WinFormsUtils.TryCreateFont("Trebuchet MS", size, style, out font, out WinFormsUtils.fontTrebuchetExists)) || (WinFormsUtils.fontTahomaExists && WinFormsUtils.TryCreateFont("Tahoma", size, style, out font, out WinFormsUtils.fontTahomaExists)) || (WinFormsUtils.fontVerdanaExists && WinFormsUtils.TryCreateFont("Verdana", size, style, out font, out WinFormsUtils.fontVerdanaExists))) {
				return font;
			}
			return new Font(Control.DefaultFont.Name, 9f, style);
		}

		private static bool TryCreateFont(string fontName, float size, FontStyle style, out Font font, out bool fontExists) {
			try {
				font = new Font(fontName, size, style);
			}
			catch (ArgumentException) {
				font = null;
			}
			if (font == null) {
				fontExists = false;
				return false;
			}
			if (!font.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase)) {
				font.Dispose();
				fontExists = false;
				return false;
			}
			fontExists = true;
			return true;
		}
	}
}