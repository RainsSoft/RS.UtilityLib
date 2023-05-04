using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Text measurement and rendering options.
	/// </summary>
	[Flags]
	internal enum TextOptions
	{
		/// <summary>
		///   No options active.
		/// </summary>
		None = 0,
		/// <summary>
		///   Allow text to wrap in multiple lines.
		/// </summary>
		AllowWrap = 1,
		/// <summary>
		///   Display hotkey prefix (marked by &amp;).
		/// </summary>
		HotkeyPrefix = 2,
		/// <summary>
		///   Use GDI+ for text measurement and rendering.
		/// </summary>
		UseGdiPlus = 4
	}
}