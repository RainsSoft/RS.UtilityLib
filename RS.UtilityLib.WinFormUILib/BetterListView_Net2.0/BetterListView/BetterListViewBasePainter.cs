using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ComponentOwl.BetterListView {

	/// <summary>
	///   Provides rendering of BetterListView elements.
	/// </summary>
	internal static class BetterListViewBasePainter
	{
		/// <summary>
		///   cut item colors opacity value
		/// </summary>
		public const byte OpacityCut = 127;

		/// <summary>
		///   full opacity value
		/// </summary>
		public const byte OpacityFull = byte.MaxValue;

		private const float GrayCoeffR = 0.299f;

		private const float GrayCoeffG = 0.587f;

		private const float GrayCoeffB = 0.114f;

		private const int DefaultNodeGlyphSize = 9;

		private const int DisabledInsertionMarkAlpha = 32;

		private const string TrialVersionFontName = "Arial";

		private const FontStyle TrialVersionFontStyle = FontStyle.Bold;

		private const float TrialVersionFontSizeInitial = 72f;

		private const string TrialVersionText = "TRIAL VERSION";

		private const float TrialVersionTextScaling = 0.8f;

		private const int TrialVersionTextIterations = 4;

		private static bool cachedIsRendererSupported;

		private static readonly Dictionary<BetterListViewPainterElementItem, VisualStyleRenderer> cachedRenderersItem;

		private static readonly Dictionary<BetterListViewPainterElementNode, VisualStyleRenderer> cachedRenderersNode;

		private static readonly StringFormat stringFormatOneLineNoTrim;

		private static readonly Color TrialVersionFontColor;

		/// <summary>
		///   format of strings rendered on a single line without trimming
		/// </summary>
		public static StringFormat StringFormatOneLineNoTrim => BetterListViewBasePainter.stringFormatOneLineNoTrim;

		/// <summary>
		///   Initialize a new BetterListViewBasePainter instance.
		/// </summary>
		static BetterListViewBasePainter() {
			BetterListViewBasePainter.cachedRenderersItem = new Dictionary<BetterListViewPainterElementItem, VisualStyleRenderer>();
			BetterListViewBasePainter.cachedRenderersNode = new Dictionary<BetterListViewPainterElementNode, VisualStyleRenderer>();
			BetterListViewBasePainter.TrialVersionFontColor = Color.FromArgb(96, Color.LightCoral);
			BetterListViewBasePainter.stringFormatOneLineNoTrim = new StringFormat {
				Trimming = StringTrimming.None
			};
			BetterListViewBasePainter.ReloadRenderers();
		}

		/// <summary>
		///   Reload visual style renderers in case of theme change.
		/// </summary>
		public static bool ReloadRenderers() {
			bool isSupported = VisualStyleRenderer.IsSupported;
			if (isSupported == BetterListViewBasePainter.cachedIsRendererSupported) {
				return false;
			}
			BetterListViewBasePainter.cachedIsRendererSupported = isSupported;
			BetterListViewBasePainter.cachedRenderersItem.Clear();
			if (!isSupported) {
				return true;
			}
			BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 1), BetterListViewPainterElementItem.Focused);
			VisualStyleElement element = VisualStyleElement.CreateElement("Explorer::ListView", 1, 2);
			BetterListViewBasePainter.TryAddRenderer(element, BetterListViewPainterElementItem.Hot);
			if (!BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 2), BetterListViewPainterElementItem.HotFocused)) {
				BetterListViewBasePainter.TryAddRenderer(element, BetterListViewPainterElementItem.HotFocused);
			}
			VisualStyleElement element2 = VisualStyleElement.CreateElement("Explorer::ListView", 1, 6);
			BetterListViewBasePainter.TryAddRenderer(element2, BetterListViewPainterElementItem.SelectedFocused);
			BetterListViewBasePainter.TryAddRenderer(element2, BetterListViewPainterElementItem.SelectedHot);
			BetterListViewBasePainter.TryAddRenderer(element2, BetterListViewPainterElementItem.SelectedHotFocused);
			BetterListViewBasePainter.TryAddRenderer(element2, BetterListViewPainterElementItem.SelectedHotUnfocused);
			VisualStyleElement element3 = VisualStyleElement.CreateElement("Explorer::ListView", 1, 3);
			BetterListViewBasePainter.TryAddRenderer(element3, BetterListViewPainterElementItem.Selected);
			VisualStyleElement element4 = VisualStyleElement.CreateElement("Explorer::ListView", 1, 5);
			BetterListViewBasePainter.TryAddRenderer(element4, BetterListViewPainterElementItem.SelectedUnfocused);
			BetterListViewBasePainter.TryAddRenderer(element4, BetterListViewPainterElementItem.Disabled);
			if (!BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("Explorer::TreeView", 2, 1), BetterListViewPainterElementNode.Closed)) {
				BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.TreeView.Glyph.Closed, BetterListViewPainterElementNode.Closed);
			}
			if (!BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("Explorer::TreeView", 2, 2), BetterListViewPainterElementNode.Opened)) {
				BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.TreeView.Glyph.Opened, BetterListViewPainterElementNode.Opened);
			}
			if (!BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("Explorer::TreeView", 4, 1), BetterListViewPainterElementNode.ClosedHot)) {
				BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.TreeView.Glyph.Closed, BetterListViewPainterElementNode.ClosedHot);
			}
			if (!BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.CreateElement("Explorer::TreeView", 4, 2), BetterListViewPainterElementNode.OpenedHot)) {
				BetterListViewBasePainter.TryAddRenderer(VisualStyleElement.TreeView.Glyph.Opened, BetterListViewPainterElementNode.OpenedHot);
			}
			return true;
		}

		/// <summary>
		///   Check whether visual style renderer can be obtained for the specified element.
		/// </summary>
		/// <param name="painterElementItem">element to check</param>
		/// <returns>visual style renderer can be obtained for the specified element</returns>
		public static bool CanGetRenderer(BetterListViewPainterElementItem painterElementItem) {
			if (VisualStyleRenderer.IsSupported) {
				return BetterListViewBasePainter.cachedRenderersItem.ContainsKey(painterElementItem);
			}
			return false;
		}

		private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementItem painterElementItem) {
			if (VisualStyleRenderer.IsElementDefined(element)) {
				if (BetterListViewBasePainter.cachedRenderersItem.ContainsKey(painterElementItem)) {
					return true;
				}
				BetterListViewBasePainter.cachedRenderersItem.Add(painterElementItem, new VisualStyleRenderer(element));
				return true;
			}
			return false;
		}

		private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementNode painterElementNode) {
			if (VisualStyleRenderer.IsElementDefined(element)) {
				if (BetterListViewBasePainter.cachedRenderersNode.ContainsKey(painterElementNode)) {
					return true;
				}
				BetterListViewBasePainter.cachedRenderersNode.Add(painterElementNode, new VisualStyleRenderer(element));
				return true;
			}
			return false;
		}

		private static bool TryGetRenderer(BetterListViewPainterElementItem painterElementItem, out VisualStyleRenderer renderer) {
			if (VisualStyleRenderer.IsSupported) {
				return BetterListViewBasePainter.cachedRenderersItem.TryGetValue(painterElementItem, out renderer);
			}
			renderer = null;
			return false;
		}

		private static bool TryGetRenderer(BetterListViewPainterElementNode painterElementNode, out VisualStyleRenderer renderer) {
			if (VisualStyleRenderer.IsSupported) {
				return BetterListViewBasePainter.cachedRenderersNode.TryGetValue(painterElementNode, out renderer);
			}
			renderer = null;
			return false;
		}

		/// <summary>
		///   Get element image with respect to its Image, ImageIndex and ImageKey properties.
		/// </summary>
		/// <param name="element">element to get image for</param>
		/// <param name="imageList">ImageList providing element images</param>
		/// <returns>Image instance, or null</returns>
		public static Image GetElementImage(BetterListViewElementBase element, ImageList imageList) {
			Checks.CheckNotNull(element, "element");
			return BetterListViewBasePainter.GetElementImage(element.Image, element.ImageIndex, element.ImageKey, imageList);
		}

		/// <summary>
		///   Get element image with respect to its Image, ImageIndex and ImageKey properties.
		/// </summary>
		/// <param name="image">image of the element</param>
		/// <param name="imageIndex">index specifying image of the element in ImageList</param>
		/// <param name="imageKey">key specifying image of the element in ImageList</param>
		/// <param name="imageList">ImageList providing element images</param>
		/// <returns>Image instance, or null</returns>
		public static Image GetElementImage(Image image, int imageIndex, string imageKey, ImageList imageList) {
			Checks.CheckTrue(imageIndex == -1 || imageIndex >= 0, "imageIndex == BetterListViewElementBase.IndexUndefined || imageIndex >= 0");
			Checks.CheckNotNull(imageKey, "imageKey");
			if (image != null) {
				return image;
			}
			if (imageList != null) {
				if (imageIndex != -1 && imageIndex < imageList.Images.Count) {
					return imageList.Images[imageIndex];
				}
				if (!string.IsNullOrEmpty(imageKey) && imageList.Images.ContainsKey(imageKey)) {
					return imageList.Images[imageKey];
				}
			}
			return null;
		}

		/// <summary>
		///   Draw selection over an item.
		/// </summary>
		/// <param name="graphics">Graphics object for drawing</param>
		/// <param name="bounds">item boundaries</param>
		/// <param name="painterElementItem">item element to draw</param>
		/// <param name="controlBackColor">background color of the control</param>
		/// <param name="selectionRenderingOptions">selection rendering options</param>
		public static void DrawSelection(Graphics graphics, Rectangle bounds, BetterListViewPainterElementItem painterElementItem, Color controlBackColor, BetterListViewSelectionRenderingOptions selectionRenderingOptions) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotEqual(painterElementItem, BetterListViewPainterElementItem.Undefined, "painterElementItem", "BetterListViewPainterElementItem.Undefined");
			if (bounds.Width == 0 || bounds.Height == 0) {
				return;
			}
			bool flag = (selectionRenderingOptions & BetterListViewSelectionRenderingOptions.AllowClassicFocusRectangle) == BetterListViewSelectionRenderingOptions.AllowClassicFocusRectangle;
			bool flag2 = (selectionRenderingOptions & BetterListViewSelectionRenderingOptions.ExtendVertical) == BetterListViewSelectionRenderingOptions.ExtendVertical;
			RectangleF clipBounds = graphics.ClipBounds;
			if (BetterListViewBasePainter.TryGetRenderer(painterElementItem, out var renderer)) {
				Rectangle rect = bounds;
				Rectangle bounds2 = bounds;
				if (flag2) {
					rect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height + 1);
					bounds2 = new Rectangle(bounds2.Left, bounds2.Top, bounds2.Width, bounds2.Height + 1);
				}
				graphics.IntersectClip(rect);
				renderer.DrawBackground(graphics, bounds2);
			}
			else {
				graphics.IntersectClip(bounds);
				switch (painterElementItem) {
					case BetterListViewPainterElementItem.Selected:
					case BetterListViewPainterElementItem.SelectedFocused:
					case BetterListViewPainterElementItem.SelectedHot:
					case BetterListViewPainterElementItem.SelectedHotFocused:
						graphics.FillRectangle(SystemBrushes.Highlight, bounds);
						break;
					case BetterListViewPainterElementItem.SelectedUnfocused:
					case BetterListViewPainterElementItem.SelectedHotUnfocused:
						graphics.FillRectangle(SystemBrushes.Control, bounds);
						break;
				}
				if (flag && (painterElementItem == BetterListViewPainterElementItem.Focused || painterElementItem == BetterListViewPainterElementItem.HotFocused || painterElementItem == BetterListViewPainterElementItem.SelectedFocused || painterElementItem == BetterListViewPainterElementItem.SelectedHotFocused)) {
					BetterListViewBasePainter.DrawFocusRectangle(graphics, bounds, controlBackColor);
				}
			}
			graphics.SetClip(clipBounds);
		}

		/// <summary>
		///   Draw focus rectangle.
		/// </summary>
		/// <param name="graphics">Graphics object for drawing</param>
		/// <param name="bounds">focus rectangle boundaries</param>
		/// <param name="controlBackColor">background color of the control</param>
		public static void DrawFocusRectangle(Graphics graphics, Rectangle bounds, Color controlBackColor) {
			Checks.CheckNotNull(graphics, "graphics");
			Pen pen = new Pen(controlBackColor, 1f);
			graphics.DrawRectangle(pen, new Rectangle(bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1));
			pen.Dispose();
			ControlPaint.DrawFocusRectangle(graphics, bounds);
		}

		/// <summary>
		///   Get size of a tree node glyph.
		/// </summary>
		/// <param name="graphics">Graphics object for measurement</param>
		/// <returns>tree node glyph size</returns>
		public static Size GetNodeGlyphSize(Graphics graphics) {
			Checks.CheckNotNull(graphics, "graphics");
			if (BetterListViewBasePainter.TryGetRenderer(BetterListViewPainterElementNode.Closed, out var renderer)) {
				return renderer.GetPartSize(graphics, ThemeSizeType.True);
			}
			return new Size(9, 9);
		}

		/// <summary>
		///   Draw tree node glyph.
		/// </summary>
		/// <param name="graphics">Graphics object for drawing</param>
		/// <param name="bounds">tree node glyph boundaries</param>
		/// <param name="painterElementNode">tree node glyph element</param>
		public static void DrawNodeGlyph(Graphics graphics, Rectangle bounds, BetterListViewPainterElementNode painterElementNode) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotEqual(painterElementNode, BetterListViewPainterElementNode.Undefined, "painterElementNode", "BetterListViewPainterElementNode.Undefined");
			if (bounds.Width == 0 || bounds.Height == 0) {
				return;
			}
			if (BetterListViewBasePainter.TryGetRenderer(painterElementNode, out var renderer)) {
				Size partSize = renderer.GetPartSize(graphics, ThemeSizeType.Draw);
				renderer.DrawBackground(graphics, new Rectangle(bounds.Left + (bounds.Width - partSize.Width >> 1), bounds.Top + (bounds.Height - partSize.Height >> 1), partSize.Width, partSize.Height));
				return;
			}
			Rectangle rect = new Rectangle(bounds.Left + (bounds.Width - 9 >> 1), bounds.Top + (bounds.Height - 9 >> 1), 9, 9);
			graphics.FillRectangle(Brushes.White, rect);
			graphics.DrawRectangle(Pens.Gray, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
			int num = rect.Left + (rect.Width >> 1);
			int num2 = rect.Top + (rect.Height >> 1);
			if (painterElementNode == BetterListViewPainterElementNode.Closed || painterElementNode == BetterListViewPainterElementNode.ClosedHot) {
				graphics.DrawLine(Pens.Black, rect.Left + 2, num2, rect.Right - 3, num2);
				graphics.DrawLine(Pens.Black, num, rect.Top + 2, num, rect.Bottom - 3);
			}
			else {
				graphics.DrawLine(Pens.Black, rect.Left + 2, num2, rect.Right - 3, num2);
			}
		}

		/// <summary>
		///   Get item check box size.
		/// </summary>
		/// <param name="graphics">Graphics object for measurement.</param>
		/// <param name="checkBoxAppearance">Item check box appearance.</param>
		/// <returns>Item check box size.</returns>
		public static Size GetCheckBoxSize(Graphics graphics, BetterListViewCheckBoxAppearance checkBoxAppearance) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotEqual(checkBoxAppearance, BetterListViewCheckBoxAppearance.Hide, "checkBoxAppearance", "BetterListViewCheckBoxAppearance.Hide");
			//return checkBoxAppearance switch {
			//	BetterListViewCheckBoxAppearance.CheckBox => CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal),
			//	BetterListViewCheckBoxAppearance.RadioButton => RadioButtonRenderer.GetGlyphSize(graphics, RadioButtonState.UncheckedNormal),
			//	_ => throw new ApplicationException($"Unknown check box appearance: {checkBoxAppearance}."),
			//};
			switch (checkBoxAppearance) {
				case BetterListViewCheckBoxAppearance.CheckBox:
					return CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
                    break;
					case  BetterListViewCheckBoxAppearance.RadioButton:
					return RadioButtonRenderer.GetGlyphSize(graphics, RadioButtonState.UncheckedNormal);
                    break;
				default:
					throw new ApplicationException($"Unknown check box appearance: {checkBoxAppearance}.");
                    break;
			};
		}

		/// <summary>
		///   Draw image to the specified rectangle with specified alpha transparency and state.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing</param>
		/// <param name="image">image to draw</param>
		/// <param name="bounds">area in which to draw the image</param>
		/// <param name="opacity">image opacity</param>
		/// <param name="enabled">draw image in enabled state</param>
		public static void DrawImage(Graphics graphics, Image image, Rectangle bounds, byte opacity, bool enabled) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(image, "image");
			InterpolationMode interpolationMode = graphics.InterpolationMode;
			PixelOffsetMode pixelOffsetMode = graphics.PixelOffsetMode;
			ImageAttributes imageAttributes = new ImageAttributes();
			if (image.Size == bounds.Size) {
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.PixelOffsetMode = PixelOffsetMode.Half;
			}
			else {
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
			}
			if (opacity != byte.MaxValue || !enabled) {
				ColorMatrix colorMatrix = ((!enabled) ? new ColorMatrix(new float[5][]
				{
				new float[5] { 0.299f, 0.299f, 0.299f, 0f, 0f },
				new float[5] { 0.587f, 0.587f, 0.587f, 0f, 0f },
				new float[5] { 0.114f, 0.114f, 0.114f, 0f, 0f },
				new float[5]
				{
					0f,
					0f,
					0f,
					(float)(int)opacity / 255f,
					0f
				},
				new float[5] { 0f, 0f, 0f, 0f, 1f }
				}) : new ColorMatrix(new float[5][]
				{
				new float[5] { 1f, 0f, 0f, 0f, 0f },
				new float[5] { 0f, 1f, 0f, 0f, 0f },
				new float[5] { 0f, 0f, 1f, 0f, 0f },
				new float[5]
				{
					0f,
					0f,
					0f,
					(float)(int)opacity / 255f,
					0f
				},
				new float[5] { 0f, 0f, 0f, 0f, 1f }
				}));
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			graphics.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			imageAttributes.Dispose();
			graphics.InterpolationMode = interpolationMode;
			graphics.PixelOffsetMode = pixelOffsetMode;
		}

		/// <summary>
		///   Draw insertion mark.
		/// </summary>
		/// <param name="graphics">Graphics object for drawing</param>
		/// <param name="location">location of the insertion mark</param>
		/// <param name="length">length of the insertion mark</param>
		/// <param name="horizontal">insertion mark is oriented horizontally</param>
		/// <param name="color">color of the insertion mark</param>
		/// <param name="enabled">draw the insertion mark in enabled state</param>
		public static void DrawInsertionMark(Graphics graphics, Point location, int length, bool horizontal, Color color, bool enabled) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(length >= 0, "length >= 0");
			Checks.CheckFalse(color.IsEmpty, "color.IsEmpty");
			if (length != 0) {
				if (!enabled) {
					color = Color.FromArgb(32, color);
				}
				Pen pen = new Pen(color, 2f);
				Brush brush = new SolidBrush(color);
				graphics.SmoothingMode = SmoothingMode.None;
				if (horizontal) {
					graphics.DrawLine(pen, (float)location.X + 4f, (float)location.Y + 0.5f, (float)(location.X + length) - 4f, (float)location.Y + 0.5f);
					graphics.FillPolygon(brush, new PointF[3]
					{
					new PointF(location.X, (float)location.Y - 3.5f),
					new PointF(location.X, (float)location.Y + 4.5f),
					new PointF((float)location.X + 4.5f, (float)location.Y + 0.5f)
					});
					graphics.FillPolygon(brush, new PointF[3]
					{
					new PointF(location.X + length, (float)location.Y - 4f),
					new PointF(location.X + length, (float)location.Y + 5f),
					new PointF((float)(location.X + length) - 4.5f, (float)location.Y + 0.5f)
					});
				}
				else {
					graphics.DrawLine(pen, (float)location.X + 0.5f, (float)location.Y + 4f, (float)location.X + 0.5f, (float)(location.Y + length) - 4f);
					graphics.FillPolygon(brush, new PointF[3]
					{
					new PointF((float)location.X - 3.5f, location.Y),
					new PointF((float)location.X + 4.5f, location.Y),
					new PointF((float)location.X + 0.5f, (float)location.Y + 4.5f)
					});
					graphics.FillPolygon(brush, new PointF[3]
					{
					new PointF((float)location.X - 4f, location.Y + length),
					new PointF((float)location.X + 5f, location.Y + length),
					new PointF((float)location.X + 0.5f, (float)(location.Y + length) - 5f)
					});
				}
				graphics.SmoothingMode = SmoothingMode.Default;
				pen.Dispose();
				brush.Dispose();
			}
		}

		/// <summary>
		///   Draw 'Trial Version' text in the specified boundaries.
		/// </summary>
		/// <param name="graphics">Graphics object for drawing</param>
		/// <param name="bounds">text boundaries</param>
		public static void DrawTrialVersionText(Graphics graphics, Rectangle bounds) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "boundsContent.Width >= 0 && boundsContent.Height >= 0");
			if (bounds.Width != 0 && bounds.Height != 0) {
				float num = 72f;
				SizeF sizeF = SizeF.Empty;
				Font font = new Font("Arial", num, FontStyle.Bold);
				for (int i = 0; i < 4; i++) {
					sizeF = graphics.MeasureString("TRIAL VERSION", font);
					float num2 = Math.Min((float)bounds.Width * 0.8f / sizeF.Width, (float)bounds.Height * 0.8f / sizeF.Height);
					num *= num2;
					font.Dispose();
					font = new Font("Arial", num, FontStyle.Bold);
				}
				Brush brush = new SolidBrush(BetterListViewBasePainter.TrialVersionFontColor);
				graphics.DrawString("TRIAL VERSION", font, brush, new PointF((float)bounds.Left + ((float)bounds.Width - sizeF.Width) * 0.5f, (float)bounds.Top + ((float)bounds.Height - sizeF.Height) * 0.5f));
				brush.Dispose();
				font.Dispose();
			}
		}

		/// <summary>
		///   Get color updated for enabled or disabled control state.
		/// </summary>
		/// <param name="colorForeground">foreground color</param>
		/// <param name="colorBackground">background color</param>
		/// <param name="isCut">cut state</param>
		/// <param name="enabled">control state</param>
		/// <returns>updated color</returns>
		public static Color GetUpdatedColor(Color colorForeground, Color colorBackground, bool isCut, bool enabled) {
			Color result = ((!isCut) ? colorForeground : Color.FromArgb(colorForeground.A, colorForeground.R + colorBackground.R >> 1, colorForeground.G + colorBackground.G >> 1, colorForeground.B + colorBackground.B >> 1));
			if (!enabled) {
				int num = (int)Math.Round(0.299f * (float)(int)result.R + 0.587f * (float)(int)result.G + 0.114f * (float)(int)result.B);
				result = Color.FromArgb(result.A, num, num, num);
			}
			return result;
		}
	}
}