using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Common control drawing routines.
	/// </summary>
	public static class Painter
	{
		/// <summary>
		///   default image border thickness (determined by image border style)
		/// </summary>
		public const int DefaultBorderThickness = 0;

		/// <summary>
		///   minimum image border thickness
		/// </summary>
		public const int MinimumBorderThickness = 0;

		/// <summary>
		///   maximum image border thickness
		/// </summary>
		public const int MaximumBorderThickness = 16;

		/// <summary>
		///   default image border color
		/// </summary>
		public static readonly Color DefaultBorderColor = SystemColors.ControlText;

		/// <summary>
		///   Get image border padding for the specified border thickness.
		/// </summary>
		/// <param name="borderType">image border style</param>
		/// <param name="thickness">image border thickness</param>
		/// <returns>image border size</returns>
		public static Padding GetBorderPadding(ImageBorderType borderType, int thickness) {
			Checks.CheckNotEqual(borderType, ImageBorderType.Undefined, "borderType", "ImageBorderType.Undefined");
			Checks.CheckBounds(thickness, 0, 16, "thickness");
			if (thickness == 0) {
				thickness = Painter.GetDefaultBorderThickness(borderType);
			}
			//return borderType switch {
			//	ImageBorderType.None => new Padding(0),
			//	ImageBorderType.Single => new Padding(thickness),
			//	ImageBorderType.SingleOffset => new Padding(thickness << 1),
			//	ImageBorderType.SymmetricShadow => new Padding(thickness),
			//	ImageBorderType.DropShadow => new Padding(0, 0, thickness, thickness),
			//	_ => throw new ApplicationException($"Unknown image border type: '{borderType}'."),
			//};
			switch (borderType) {
				case ImageBorderType.None:
					return new Padding(0);
					break;
				case ImageBorderType.Single:
				case ImageBorderType.SymmetricShadow:
					return new Padding(thickness);
					break;
				case ImageBorderType.SingleOffset:
					return new Padding(thickness<<1);
					break;
				case ImageBorderType.DropShadow:
					return new Padding(0, 0, thickness, thickness);
					break;
				default:
					throw new ApplicationException($"Unknown image border type: '{borderType}'.");
					break;
			}
		}

		/// <summary>
		///   Get default image border thickness for the specified border style.
		/// </summary>
		/// <param name="borderType">image border style</param>
		/// <returns>border thickness</returns>
		public static int GetDefaultBorderThickness(ImageBorderType borderType) {
			Checks.CheckNotEqual(borderType, ImageBorderType.Undefined, "borderType", "ImageBorderType.Undefined");
			//return borderType switch {
			//	ImageBorderType.None => 0,
			//	ImageBorderType.Single => 1,
			//	ImageBorderType.SingleOffset => 1,
			//	ImageBorderType.SymmetricShadow => 4,
			//	ImageBorderType.DropShadow => 4,
			//	_ => throw new ApplicationException($"Unknown image border type: '{borderType}'."),
			//};
			switch (borderType) {
				case ImageBorderType.None:
					return 0;
					break;
				case ImageBorderType.Single:
				case ImageBorderType.SingleOffset:
					return 1;
					break;
				case ImageBorderType.SymmetricShadow:
				case ImageBorderType.DropShadow:
					return 4;
					break;
				default:
					throw new ApplicationException($"Unknown image border type: '{borderType}'.");
					break;

			}
		}

		/// <summary>
		///   Draw a border around specified rectangle.
		/// </summary>
		/// <param name="graphics">Graphics object</param>
		/// <param name="bounds">rectangle around which to draw the border</param>
		/// <param name="borderType">image border style</param>
		public static void DrawBorder(Graphics graphics, Rectangle bounds, ImageBorderType borderType) {
			Painter.DrawBorder(graphics, bounds, borderType, 0, Painter.DefaultBorderColor);
		}

		/// <summary>
		///   Draw a border around specified rectangle.
		/// </summary>
		/// <param name="graphics">Graphics object</param>
		/// <param name="bounds">rectangle around which to draw the border</param>
		/// <param name="borderType">image border style</param>
		/// <param name="thickness">image border thickness; 0 for default thickness</param>
		public static void DrawBorder(Graphics graphics, Rectangle bounds, ImageBorderType borderType, int thickness) {
			Painter.DrawBorder(graphics, bounds, borderType, thickness, Painter.DefaultBorderColor);
		}

		/// <summary>
		///   Draw a border around specified rectangle.
		/// </summary>
		/// <param name="graphics">Graphics object</param>
		/// <param name="bounds">rectangle around which to draw the border</param>
		/// <param name="borderType">image border style</param>
		/// <param name="thickness">image border thickness; 0 for default thickness</param>
		/// <param name="color">image border color</param>
		public static void DrawBorder(Graphics graphics, Rectangle bounds, ImageBorderType borderType, int thickness, Color color) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckFalse(bounds.IsEmpty, "bounds.IsEmpty");
			Checks.CheckNotEqual(borderType, ImageBorderType.Undefined, "borderType", "ImageBorderType.Undefined");
			Checks.CheckBounds(thickness, 0, 16, "thickness");
			Checks.CheckFalse(color.IsEmpty, "color.IsEmpty");
			if (borderType == ImageBorderType.None) {
				return;
			}
			if (thickness == 0) {
				thickness = Painter.GetDefaultBorderThickness(borderType);
			}
			float num = (float)bounds.Left - 0.5f;
			float num2 = (float)bounds.Top - 0.5f;
			float num3 = (float)bounds.Right - 0.5f;
			float num4 = (float)bounds.Bottom - 0.5f;
			switch (borderType) {
				case ImageBorderType.Single:
				case ImageBorderType.SingleOffset: {
						Pen pen = new Pen(color, thickness);
						float num5 = ((borderType != ImageBorderType.Single) ? ((float)thickness * 1.5f) : ((float)thickness * 0.5f));
						num -= num5;
						num2 -= num5;
						num3 += num5;
						num4 += num5;
						graphics.DrawRectangle(pen, num, num2, num3 - num, num4 - num2);
						pen.Dispose();
						break;
					}
				case ImageBorderType.SymmetricShadow:
				case ImageBorderType.DropShadow: {
						SmoothingMode smoothingMode = graphics.SmoothingMode;
						graphics.SmoothingMode = SmoothingMode.HighQuality;
						Brush brush = new SolidBrush(Color.FromArgb(64 / Math.Max(thickness - 1, 1), color));
						if (borderType == ImageBorderType.SymmetricShadow) {
							num -= (float)thickness;
							num2 -= (float)thickness;
							num3 += (float)thickness;
							num4 += (float)thickness;
						}
						else {
							num3 += (float)thickness;
							num4 += (float)thickness;
						}
						for (int i = 0; i < thickness; i++) {
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddLine(num + (float)thickness, num2, num3 - (float)(thickness * 2), num2);
							graphicsPath.AddArc(num3 - (float)(thickness * 2), num2, thickness * 2, thickness * 2, 270f, 90f);
							graphicsPath.AddLine(num3, num2 + (float)thickness, num3, num4 - (float)(thickness * 2));
							graphicsPath.AddArc(num3 - (float)(thickness * 2), num4 - (float)(thickness * 2), thickness * 2, thickness * 2, 0f, 90f);
							graphicsPath.AddLine(num3 - (float)(thickness * 2), num4, num + (float)thickness, num4);
							graphicsPath.AddArc(num, num4 - (float)(thickness * 2), thickness * 2, thickness * 2, 90f, 90f);
							graphicsPath.AddLine(num, num4 - (float)(thickness * 2), num, num2 + (float)thickness);
							graphicsPath.AddArc(num, num2, thickness * 2, thickness * 2, 180f, 90f);
							graphics.FillPath(brush, graphicsPath);
							graphicsPath.Dispose();
							num += 1f;
							num2 += 1f;
							num3 -= 1f;
							num4 -= 1f;
						}
						brush.Dispose();
						graphics.SmoothingMode = smoothingMode;
						break;
					}
				default:
					throw new ApplicationException($"Unknown image border style: '{borderType}'.");
			}
		}

		/// <summary>
		///   Get size which best fits targetSize while preserving aspect ratio of sourceSize.
		/// </summary>
		/// <param name="sourceSize">source size</param>
		/// <param name="targetSize">target size</param>
		/// <param name="upscale">allow result size to be greater than source size</param>
		/// <param name="fromOutside">allow result size to be greater that target size</param>
		/// <returns>resulting size</returns>
		public static Size ToIsotropicSize(Size sourceSize, Size targetSize, bool upscale, bool fromOutside) {
			if (sourceSize.IsEmpty || targetSize.IsEmpty) {
				return Size.Empty;
			}
			if (!upscale && sourceSize.Width <= targetSize.Width && sourceSize.Height <= targetSize.Height) {
				return sourceSize;
			}
			Size size = new Size(sourceSize.Width, sourceSize.Height);
			float val = (float)targetSize.Width / (float)size.Width;
			float val2 = (float)targetSize.Height / (float)size.Height;
			float num = (fromOutside ? Math.Max(val, val2) : Math.Min(val, val2));
			return new Size((int)Math.Round((float)size.Width * num), (int)Math.Round((float)size.Height * num));
		}

		/// <summary>
		///   Offset rectangle by the specified amount.
		/// </summary>
		/// <param name="rectangle">rectangle to offset</param>
		/// <param name="offset">amount of shift</param>
		/// <returns>adjusted rectangle</returns>
		public static Rectangle OffsetRectangle(Rectangle rectangle, Point offset) {
			return new Rectangle(rectangle.Left + offset.X, rectangle.Top + offset.Y, rectangle.Width, rectangle.Height);
		}
	}
}