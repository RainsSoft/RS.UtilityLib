using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Provides rendering of BetterListView elements.
    /// </summary>
    internal static class BetterListViewPainter
    {
        private const int DefaultSortGlyphSize = 16;

        private const int DefaultSortGlyphPadding = 2;

        private const byte ColumnHeaderBitmapOpacity = 192;

        private const int SpacingSeparator = 8;

        private static readonly Dictionary<BetterListViewPainterElementColumnHeader, VisualStyleRenderer> cachedRenderersColumnHeader;

        private static readonly Dictionary<BetterListViewPainterElementSortGlyph, VisualStyleRenderer> cachedRenderersSortGlyph;

        private static readonly Dictionary<BetterListViewPainterElementGroup, VisualStyleRenderer> cachedRenderersGroup;

        private static readonly Dictionary<BetterListViewPainterElementGroupExpandButton, VisualStyleRenderer> cachedRenderersGroupExpandButton;

        private static bool cachedIsRendererSupported;

        /// <summary>
        ///   always consider the sort glyph is visible in column header
        /// </summary>
        public static bool AlwaysConsiderSortGlyph {
            get {
                if (VisualStyleRenderer.IsSupported) {
                    return VisualStyleRenderer.IsElementDefined(VisualStyleElement.Header.SortArrow.SortedUp);
                }
                return false;
            }
        }

        /// <summary>
        ///   alignment of a column header sort glyph
        /// </summary>
        public static BetterListViewPainterGlyphAlignment ColumnHeaderSortGlyphAlignment {
            get {
                if (BetterListViewPainter.CanGetRenderer(BetterListViewPainterElementSortGlyph.SortGlyphAscending) && BetterListViewPainter.CanGetRenderer(BetterListViewPainterElementSortGlyph.SortGlyphDescending)) {
                    return BetterListViewPainterGlyphAlignment.Top;
                }
                return BetterListViewPainterGlyphAlignment.Right;
            }
        }

        /// <summary>
        ///   Get size of a column header sort glyph.
        /// </summary>
        /// <param name="graphics">Graphics object for measurement</param>
        /// <returns>column header sort glyph size</returns>
        public static Size GetSortGlyphSize(Graphics graphics) {
            Checks.CheckNotNull(graphics, "graphics");
            if (BetterListViewPainter.TryGetRenderer(BetterListViewPainterElementSortGlyph.SortGlyphAscending, out var renderer)) {
                return renderer.GetPartSize(graphics, ThemeSizeType.True);
            }
            return new Size(16, 16);
        }

        public static Padding GetSortGlyphPadding(Padding minimumPadding) {
            int val = ((!BetterListViewPainter.CanGetRenderer(BetterListViewPainterElementSortGlyph.SortGlyphAscending)) ? 2 : 0);
            return new Padding(Math.Max(minimumPadding.Left, val), Math.Max(minimumPadding.Top, val), Math.Max(minimumPadding.Right, val), Math.Max(minimumPadding.Bottom, val));
        }

        /// <summary>
        ///   Draw background of a column header.
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        /// <param name="bounds">column header background boundaries</param>
        /// <param name="painterElementColumnHeader">column header element to draw</param>
        public static void DrawColumnHeaderBackground(Graphics graphics, Rectangle bounds, BetterListViewPainterElementColumnHeader painterElementColumnHeader) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckNotEqual(painterElementColumnHeader, BetterListViewPainterElementColumnHeader.Undefined, "painterElementColumnHeader", "BetterListViewPainterElementColumnHeader.Undefined");
            if (bounds.Width <= 0 || bounds.Height <= 0) {
                return;
            }
            RectangleF clipBounds = graphics.ClipBounds;
            graphics.IntersectClip(bounds);
            if (BetterListViewPainter.TryGetRenderer(painterElementColumnHeader, out var renderer)) {
                renderer.DrawBackground(graphics, bounds);
            }
            else {
                PushButtonState state;
                switch (painterElementColumnHeader) {
                    case BetterListViewPainterElementColumnHeader.LeftNormal:
                    case BetterListViewPainterElementColumnHeader.MiddleNormal:
                    case BetterListViewPainterElementColumnHeader.RightNormal:
                        state = PushButtonState.Normal;
                        break;
                    case BetterListViewPainterElementColumnHeader.LeftHot:
                    case BetterListViewPainterElementColumnHeader.MiddleHot:
                    case BetterListViewPainterElementColumnHeader.RightHot:
                        state = PushButtonState.Hot;
                        break;
                    case BetterListViewPainterElementColumnHeader.LeftPressed:
                    case BetterListViewPainterElementColumnHeader.MiddlePressed:
                    case BetterListViewPainterElementColumnHeader.RightPressed:
                        state = PushButtonState.Pressed;
                        break;
                    case BetterListViewPainterElementColumnHeader.LeftSorted:
                    case BetterListViewPainterElementColumnHeader.MiddleSorted:
                    case BetterListViewPainterElementColumnHeader.RightSorted:
                        state = PushButtonState.Normal;
                        break;
                    default:
                        throw new ApplicationException($"Unknown column header element: '{painterElementColumnHeader}'.");
                }
                ButtonRenderer.DrawButton(graphics, bounds, state);
            }
            graphics.SetClip(clipBounds);
        }

        /// <summary>
        ///   Draw foreground of a column header.
        /// </summary>
        /// <param name="eventArgs">column header drawing event data</param>
        /// <param name="imageList">images to be displayed on column headers</param>
        /// <param name="sortOrder">column header sort order for sort glyph display</param>
        /// <param name="enabled">draw column header in enabled state</param>
        /// <param name="maximumTextLines">maximum allowed number of text lines</param>
        public static void DrawColumnHeaderForeground(BetterListViewDrawColumnHeaderEventArgs eventArgs, ImageList imageList, BetterListViewSortOrder sortOrder, bool enabled, int maximumTextLines) {
            Checks.CheckNotNull(eventArgs, "eventArgs");
            Checks.CheckTrue(maximumTextLines >= 1, "maximumTextLines >= 1");
            BetterListViewPainter.DrawColumnHeaderForegroundInternal(eventArgs, imageList, sortOrder, enabled, maximumTextLines, useGdiPlus: false);
        }

        /// <summary>
        ///   Create semi-transparent bitmap of a column header.
        /// </summary>
        /// <param name="imageList">images to be displayed on column headers</param>
        /// <param name="columnHeader">column header to create a bitmap from</param>
        /// <param name="columnHeaderBounds">column header boundaries</param>
        /// <param name="sortOrder">column header sort order for sort glyph display</param>
        /// <param name="maximumTextLines">maximum allowed lines of text</param>
        /// <param name="columnHeadersCount">number of column headers</param>
        /// <returns>bitmap of a column header</returns>
        public unsafe static Bitmap CreateColumnHeaderBitmap(ImageList imageList, BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderBounds columnHeaderBounds, BetterListViewSortOrder sortOrder, int maximumTextLines, int columnHeadersCount) {
            Checks.CheckNotNull(columnHeader, "columnHeader");
            Checks.CheckNotNull(columnHeaderBounds, "columnHeaderBounds");
            Checks.CheckTrue(maximumTextLines >= 1, "maximumTextLines >= 1");
            Checks.CheckTrue(columnHeadersCount > 0, "columnHeadersCount > 0");
            Point location = columnHeaderBounds.BoundsSpacing.Location;
            columnHeaderBounds.Offset(new Point(-location.X, -location.Y));
            Bitmap bitmap = new Bitmap(columnHeaderBounds.BoundsOuter.Width, columnHeaderBounds.BoundsOuter.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            BetterListViewColumnHeaderStyle style = columnHeader.GetStyle();
            BetterListViewPainterElementColumnHeader painterElementColumnHeader = ((columnHeader.Index != 0) ? ((columnHeader.Index != columnHeadersCount - 1) ? ((style != BetterListViewColumnHeaderStyle.Nonclickable) ? BetterListViewPainterElementColumnHeader.MiddlePressed : ((sortOrder != 0) ? BetterListViewPainterElementColumnHeader.MiddleSorted : BetterListViewPainterElementColumnHeader.MiddleNormal)) : ((style != BetterListViewColumnHeaderStyle.Nonclickable) ? BetterListViewPainterElementColumnHeader.RightPressed : ((sortOrder != 0) ? BetterListViewPainterElementColumnHeader.RightSorted : BetterListViewPainterElementColumnHeader.RightNormal))) : ((style != BetterListViewColumnHeaderStyle.Nonclickable) ? BetterListViewPainterElementColumnHeader.LeftPressed : ((sortOrder != 0) ? BetterListViewPainterElementColumnHeader.LeftSorted : BetterListViewPainterElementColumnHeader.LeftNormal)));
            BetterListViewPainter.DrawColumnHeaderBackground(graphics, columnHeaderBounds.BoundsOuter, painterElementColumnHeader);
            BetterListViewPainter.DrawColumnHeaderForegroundInternal(new BetterListViewDrawColumnHeaderEventArgs(graphics, columnHeader, columnHeaderBounds, new BetterListViewColumnHeaderStateInfo(BetterListViewColumnHeaderState.Pressed, sortOrder)), imageList, sortOrder, enabled: true, maximumTextLines, useGdiPlus: true);
            graphics.Dispose();
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)(void*)IntPtr.Zero;
            for (int i = 0; i < bitmapData.Height; i++) {
                ptr = (byte*)(void*)bitmapData.Scan0 + (long)i * (long)bitmapData.Stride;
                for (int j = 0; j < bitmapData.Width; j++) {
                    ptr[3] = 192;
                    ptr += 4;
                }
            }
            bitmap.UnlockBits(bitmapData);
            columnHeaderBounds.Offset(location);
            return bitmap;
        }

        private static void DrawColumnHeaderForegroundInternal(BetterListViewDrawColumnHeaderEventArgs eventArgs, ImageList imageList, BetterListViewSortOrder sortOrder, bool enabled, int maximumTextLines, bool useGdiPlus) {
            Graphics graphics = eventArgs.Graphics;
            BetterListViewColumnHeader columnHeader = eventArgs.ColumnHeader;
            BetterListViewColumnHeaderBounds columnHeaderBounds = eventArgs.ColumnHeaderBounds;
            if (columnHeaderBounds.BoundsOuter.Width == 0 || columnHeaderBounds.BoundsOuter.Height == 0) {
                return;
            }
            RectangleF clipBounds = graphics.ClipBounds;
            graphics.IntersectClip(columnHeaderBounds.BoundsOuter);
            if (eventArgs.DrawSortGlyph && !columnHeaderBounds.BoundsSortGlyph.IsEmpty && sortOrder != 0) {
                BetterListViewPainterElementSortGlyph painterElementSortGlyph = ((sortOrder != BetterListViewSortOrder.Ascending) ? BetterListViewPainterElementSortGlyph.SortGlyphDescending : BetterListViewPainterElementSortGlyph.SortGlyphAscending);
                if (BetterListViewPainter.TryGetRenderer(painterElementSortGlyph, out var renderer)) {
                    Size partSize = renderer.GetPartSize(graphics, ThemeSizeType.True);
                    renderer.DrawBackground(graphics, new Rectangle(columnHeaderBounds.BoundsOuter.Left + (columnHeaderBounds.BoundsOuter.Width - partSize.Width >> 1), columnHeaderBounds.BoundsOuter.Top, partSize.Width, partSize.Height));
                }
                else {
                    bool flag = sortOrder == BetterListViewSortOrder.Ascending;
                    float num = (float)columnHeaderBounds.BoundsSortGlyph.Left + (float)columnHeaderBounds.BoundsSortGlyph.Width / 2f;
                    float num2 = (float)columnHeaderBounds.BoundsSortGlyph.Top + (float)columnHeaderBounds.BoundsSortGlyph.Height / 2f;
                    PointF[] array = new PointF[3];
                    if (flag) {
                        array[0] = new PointF(num - 5.5f, num2 + 2.5f);
                        array[1] = new PointF(num + 5.5f, num2 + 2.5f);
                        array[2] = new PointF(num, num2 - 2.5f);
                    }
                    else {
                        array[0] = new PointF(num - 5.5f, num2 - 2.5f);
                        array[1] = new PointF(num + 5.5f, num2 - 2.5f);
                        array[2] = new PointF(num, num2 + 2.5f);
                    }
                    graphics.FillPolygon(SystemBrushes.ControlDark, array);
                }
            }
            graphics.IntersectClip(columnHeaderBounds.BoundsInner);
            if (eventArgs.DrawImage && !columnHeaderBounds.BoundsImage.Size.IsEmpty) {
                Image elementImage = BetterListViewBasePainter.GetElementImage(columnHeader, imageList);
                if (elementImage != null) {
                    BetterListViewBasePainter.DrawImage(graphics, elementImage, columnHeaderBounds.BoundsImage, byte.MaxValue, enabled);
                }
            }
            if (eventArgs.DrawText && columnHeaderBounds.BoundsText.Width != 0) {
                Color updatedColor = BetterListViewBasePainter.GetUpdatedColor(columnHeader.ForeColor, Color.Transparent, isCut: false, enabled);
                columnHeader.DrawingDrawText(graphics, updatedColor, columnHeaderBounds.BoundsText, maximumTextLines, useGdiPlus);
            }
            graphics.SetClip(clipBounds);
        }

        /// <summary>
        ///   Initialize a new BetterListViewPainter instance.
        /// </summary>
        static BetterListViewPainter() {
            BetterListViewPainter.cachedRenderersColumnHeader = new Dictionary<BetterListViewPainterElementColumnHeader, VisualStyleRenderer>();
            BetterListViewPainter.cachedRenderersSortGlyph = new Dictionary<BetterListViewPainterElementSortGlyph, VisualStyleRenderer>();
            BetterListViewPainter.cachedRenderersGroup = new Dictionary<BetterListViewPainterElementGroup, VisualStyleRenderer>();
            BetterListViewPainter.cachedRenderersGroupExpandButton = new Dictionary<BetterListViewPainterElementGroupExpandButton, VisualStyleRenderer>();
            BetterListViewPainter.ReloadRenderers();
        }

        /// <summary>
        ///   Reload visual style renderers in case of theme change.
        /// </summary>
        public static bool ReloadRenderers() {
            bool isSupported = VisualStyleRenderer.IsSupported;
            if (isSupported == BetterListViewPainter.cachedIsRendererSupported) {
                return false;
            }
            BetterListViewPainter.cachedIsRendererSupported = isSupported;
            BetterListViewPainter.cachedRenderersColumnHeader.Clear();
            BetterListViewPainter.cachedRenderersSortGlyph.Clear();
            if (!isSupported) {
                return true;
            }
            VisualStyleElement element = VisualStyleElement.CreateElement("Header", 2, 2);
            VisualStyleElement element5 = VisualStyleElement.CreateElement("Header", 2, 1);
            VisualStyleElement element6 = VisualStyleElement.CreateElement("Header", 2, 3);
            VisualStyleElement element7 = VisualStyleElement.CreateElement("Header", 2, 4);
            VisualStyleElement element8 = VisualStyleElement.CreateElement("Header", 1, 2);
            VisualStyleElement element9 = VisualStyleElement.CreateElement("Header", 1, 1);
            VisualStyleElement element10 = VisualStyleElement.CreateElement("Header", 1, 3);
            VisualStyleElement element11 = VisualStyleElement.CreateElement("Header", 1, 4);
            VisualStyleElement element12 = VisualStyleElement.CreateElement("Header", 3, 2);
            VisualStyleElement element2 = VisualStyleElement.CreateElement("Header", 3, 1);
            VisualStyleElement element3 = VisualStyleElement.CreateElement("Header", 3, 3);
            VisualStyleElement element4 = VisualStyleElement.CreateElement("Header", 3, 4);
            if (!BetterListViewPainter.TryAddRenderer(element, BetterListViewPainterElementColumnHeader.LeftHot) && !BetterListViewPainter.TryAddRenderer(element8, BetterListViewPainterElementColumnHeader.LeftHot) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemLeft.Hot, BetterListViewPainterElementColumnHeader.LeftHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Hot, BetterListViewPainterElementColumnHeader.LeftHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(element5, BetterListViewPainterElementColumnHeader.LeftNormal) && !BetterListViewPainter.TryAddRenderer(element9, BetterListViewPainterElementColumnHeader.LeftNormal) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemLeft.Normal, BetterListViewPainterElementColumnHeader.LeftNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.LeftNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(element6, BetterListViewPainterElementColumnHeader.LeftPressed) && !BetterListViewPainter.TryAddRenderer(element10, BetterListViewPainterElementColumnHeader.LeftPressed) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemLeft.Pressed, BetterListViewPainterElementColumnHeader.LeftPressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Pressed, BetterListViewPainterElementColumnHeader.LeftPressed);
            }
            if (!BetterListViewPainter.TryAddRenderer(element7, BetterListViewPainterElementColumnHeader.LeftSorted) && !BetterListViewPainter.TryAddRenderer(element11, BetterListViewPainterElementColumnHeader.LeftSorted) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemLeft.Normal, BetterListViewPainterElementColumnHeader.LeftSorted)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.LeftSorted);
            }
            if (!BetterListViewPainter.TryAddRenderer(element8, BetterListViewPainterElementColumnHeader.MiddleHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Hot, BetterListViewPainterElementColumnHeader.MiddleHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(element9, BetterListViewPainterElementColumnHeader.MiddleNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.MiddleNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(element10, BetterListViewPainterElementColumnHeader.MiddlePressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Pressed, BetterListViewPainterElementColumnHeader.MiddlePressed);
            }
            if (!BetterListViewPainter.TryAddRenderer(element11, BetterListViewPainterElementColumnHeader.MiddleSorted)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.MiddleSorted);
            }
            if (!BetterListViewPainter.TryAddRenderer(element12, BetterListViewPainterElementColumnHeader.RightHot) && !BetterListViewPainter.TryAddRenderer(element8, BetterListViewPainterElementColumnHeader.RightHot) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemRight.Hot, BetterListViewPainterElementColumnHeader.RightHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Hot, BetterListViewPainterElementColumnHeader.RightHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(element2, BetterListViewPainterElementColumnHeader.RightNormal) && !BetterListViewPainter.TryAddRenderer(element9, BetterListViewPainterElementColumnHeader.RightNormal) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemRight.Normal, BetterListViewPainterElementColumnHeader.RightNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.RightNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(element3, BetterListViewPainterElementColumnHeader.RightPressed) && !BetterListViewPainter.TryAddRenderer(element10, BetterListViewPainterElementColumnHeader.RightPressed) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemRight.Pressed, BetterListViewPainterElementColumnHeader.RightPressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Pressed, BetterListViewPainterElementColumnHeader.RightPressed);
            }
            if (!BetterListViewPainter.TryAddRenderer(element4, BetterListViewPainterElementColumnHeader.RightSorted) && !BetterListViewPainter.TryAddRenderer(element11, BetterListViewPainterElementColumnHeader.RightSorted) && !BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.ItemRight.Normal, BetterListViewPainterElementColumnHeader.RightSorted)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.Item.Normal, BetterListViewPainterElementColumnHeader.RightSorted);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("Header", 4, 1), BetterListViewPainterElementSortGlyph.SortGlyphAscending)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.SortArrow.SortedUp, BetterListViewPainterElementSortGlyph.SortGlyphAscending);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("Header", 4, 2), BetterListViewPainterElementSortGlyph.SortGlyphDescending)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.Header.SortArrow.SortedDown, BetterListViewPainterElementSortGlyph.SortGlyphDescending);
            }
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 1), BetterListViewPainterElementGroup.Focused);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 10), BetterListViewPainterElementGroup.CollapsedHot);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 2), BetterListViewPainterElementGroup.CollapsedHotFocused);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 13), BetterListViewPainterElementGroup.CollapsedSelectedInactive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 14), BetterListViewPainterElementGroup.CollapsedSelectedHotInactive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 11), BetterListViewPainterElementGroup.CollapsedSelectedActive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 12), BetterListViewPainterElementGroup.CollapsedSelectedHotActive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 2), BetterListViewPainterElementGroup.ExpandedHot);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 2), BetterListViewPainterElementGroup.ExpandedHotFocused);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 5), BetterListViewPainterElementGroup.ExpandedSelectedInactive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 6), BetterListViewPainterElementGroup.ExpandedSelectedHotInactive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 3), BetterListViewPainterElementGroup.ExpandedSelectedActive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 1), BetterListViewPainterElementGroup.ExpandedSelectedFocusedActive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 6, 4), BetterListViewPainterElementGroup.ExpandedSelectedHotActive);
            BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView", 3, 2), BetterListViewPainterElementGroup.ExpandedSelectedHotFocusedActive);
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 1), BetterListViewPainterElementGroupExpandButton.CollapsedNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupExpand.Normal, BetterListViewPainterElementGroupExpandButton.CollapsedNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 2), BetterListViewPainterElementGroupExpandButton.CollapsedHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupExpand.Hot, BetterListViewPainterElementGroupExpandButton.CollapsedHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 3), BetterListViewPainterElementGroupExpandButton.CollapsedPressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupExpand.Pressed, BetterListViewPainterElementGroupExpandButton.CollapsedPressed);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 9, 1), BetterListViewPainterElementGroupExpandButton.ExpandedNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal, BetterListViewPainterElementGroupExpandButton.ExpandedNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 9, 2), BetterListViewPainterElementGroupExpandButton.ExpandedHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupCollapse.Hot, BetterListViewPainterElementGroupExpandButton.ExpandedHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 9, 3), BetterListViewPainterElementGroupExpandButton.ExpandedPressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.NormalGroupCollapse.Pressed, BetterListViewPainterElementGroupExpandButton.ExpandedPressed);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 1), BetterListViewPainterElementGroupExpandButton.SelectedCollapsedNormal)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.SpecialGroupExpand.Normal, BetterListViewPainterElementGroupExpandButton.SelectedCollapsedNormal);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 2), BetterListViewPainterElementGroupExpandButton.SelectedCollapsedHot)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.SpecialGroupExpand.Hot, BetterListViewPainterElementGroupExpandButton.SelectedCollapsedHot);
            }
            if (!BetterListViewPainter.TryAddRenderer(VisualStyleElement.CreateElement("ItemsView::ListView", 8, 3), BetterListViewPainterElementGroupExpandButton.SelectedCollapsedPressed)) {
                BetterListViewPainter.TryAddRenderer(VisualStyleElement.ExplorerBar.SpecialGroupExpand.Pressed, BetterListViewPainterElementGroupExpandButton.SelectedCollapsedPressed);
            }
            return true;
        }

        private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementColumnHeader painterElementColumnHeader) {
            if (VisualStyleRenderer.IsElementDefined(element)) {
                if (BetterListViewPainter.cachedRenderersColumnHeader.ContainsKey(painterElementColumnHeader)) {
                    return true;
                }
                BetterListViewPainter.cachedRenderersColumnHeader.Add(painterElementColumnHeader, new VisualStyleRenderer(element));
                return true;
            }
            return false;
        }

        private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementSortGlyph painterElementSortGlyph) {
            if (VisualStyleRenderer.IsElementDefined(element)) {
                if (BetterListViewPainter.cachedRenderersSortGlyph.ContainsKey(painterElementSortGlyph)) {
                    return true;
                }
                BetterListViewPainter.cachedRenderersSortGlyph.Add(painterElementSortGlyph, new VisualStyleRenderer(element));
                return true;
            }
            return false;
        }

        private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementGroup painterElementGroup) {
            if (VisualStyleRenderer.IsElementDefined(element)) {
                if (BetterListViewPainter.cachedRenderersGroup.ContainsKey(painterElementGroup)) {
                    return true;
                }
                BetterListViewPainter.cachedRenderersGroup.Add(painterElementGroup, new VisualStyleRenderer(element));
                return true;
            }
            return false;
        }

        private static bool TryAddRenderer(VisualStyleElement element, BetterListViewPainterElementGroupExpandButton painterElementGroupExpandButton) {
            if (VisualStyleRenderer.IsElementDefined(element)) {
                if (BetterListViewPainter.cachedRenderersGroupExpandButton.ContainsKey(painterElementGroupExpandButton)) {
                    return true;
                }
                BetterListViewPainter.cachedRenderersGroupExpandButton.Add(painterElementGroupExpandButton, new VisualStyleRenderer(element));
                return true;
            }
            return false;
        }

        private static bool TryGetRenderer(BetterListViewPainterElementColumnHeader painterElementColumnHeader, out VisualStyleRenderer renderer) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersColumnHeader.TryGetValue(painterElementColumnHeader, out renderer);
            }
            renderer = null;
            return false;
        }

        private static bool TryGetRenderer(BetterListViewPainterElementSortGlyph painterElementSortGlyph, out VisualStyleRenderer renderer) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersSortGlyph.TryGetValue(painterElementSortGlyph, out renderer);
            }
            renderer = null;
            return false;
        }

        private static bool TryGetRenderer(BetterListViewPainterElementGroup painterElementGroup, out VisualStyleRenderer renderer) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersGroup.TryGetValue(painterElementGroup, out renderer);
            }
            renderer = null;
            return false;
        }

        private static bool TryGetRenderer(BetterListViewPainterElementGroupExpandButton painterElementGroupExpandButton, out VisualStyleRenderer renderer) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersGroupExpandButton.TryGetValue(painterElementGroupExpandButton, out renderer);
            }
            renderer = null;
            return false;
        }

        private static bool CanGetRenderer(BetterListViewPainterElementSortGlyph painterElementSortGlyph) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersSortGlyph.ContainsKey(painterElementSortGlyph);
            }
            return false;
        }

        private static bool CanGetRenderer(BetterListViewPainterElementGroup painterElementGroup) {
            if (VisualStyleRenderer.IsSupported) {
                return BetterListViewPainter.cachedRenderersGroup.ContainsKey(painterElementGroup);
            }
            return false;
        }

        /// <summary>
        ///   Get size of a group expand button.
        /// </summary>
        /// <param name="graphics">Graphics object for measurement</param>
        /// <returns>group expand button size</returns>
        public static Size GetExpandButtonSize(Graphics graphics) {
            Checks.CheckNotNull(graphics, "graphics");
            if (BetterListViewPainter.TryGetRenderer(BetterListViewPainterElementGroupExpandButton.CollapsedNormal, out var renderer)) {
                return renderer.GetPartSize(graphics, ThemeSizeType.True);
            }
            return BetterListViewBasePainter.GetNodeGlyphSize(graphics);
        }

        public static void DrawGroupBackground(Graphics graphics, BetterListViewGroup group, BetterListViewGroupBounds groupBounds) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckNotNull(group, "group");
            Checks.CheckNotNull(groupBounds, "groupBounds");
            if (groupBounds.BoundsSelection.Width != 0 && groupBounds.BoundsSelection.Height != 0) {
                RectangleF clipBounds = graphics.ClipBounds;
                graphics.IntersectClip(groupBounds.BoundsOuter);
                if (!group.BackColor.IsEmpty && group.BackColor != Color.Transparent) {
                    Brush brush = new SolidBrush(group.BackColor);
                    graphics.FillRectangle(brush, groupBounds.BoundsSelection);
                    brush.Dispose();
                }
                graphics.SetClip(clipBounds);
            }
        }

        public static void DrawGroupForeground(BetterListViewDrawGroupEventArgs eventArgs, ImageList imageList, TextAlignmentHorizontal layoutTextAlignmentHorizontal, BetterListViewImageAlignmentHorizontal layoutImageAlignmentHorizontal, BetterListViewPainterElementGroup painterElementGroup, BetterListViewPainterElementGroupExpandButton painterElementGroupExpandButton, Color controlBackColor, bool enabled) {
            Checks.CheckNotNull(eventArgs, "eventArgs");
            Checks.CheckNotEqual(painterElementGroupExpandButton, BetterListViewPainterElementGroupExpandButton.Undefined, "painterElementGroupExpandButton", "BetterListViewPainterElementGroupExpandButton.Undefined");
            Graphics graphics = eventArgs.Graphics;
            BetterListViewGroup group = eventArgs.Group;
            BetterListViewGroupBounds groupBounds = eventArgs.GroupBounds;
            if (groupBounds.BoundsInner.Width == 0 || groupBounds.BoundsInner.Height == 0) {
                return;
            }
            RectangleF clipBounds = graphics.ClipBounds;
            graphics.IntersectClip(groupBounds.BoundsSelection);
            VisualStyleRenderer renderer;
            if (eventArgs.DrawFace) {
                if (painterElementGroup != BetterListViewPainterElementGroup.Undefined && BetterListViewPainter.TryGetRenderer(painterElementGroup, out renderer)) {
                    renderer.DrawBackground(graphics, groupBounds.BoundsSelection);
                }
                else {
                    switch (painterElementGroup) {
                        case BetterListViewPainterElementGroup.CollapsedSelectedActive:
                        case BetterListViewPainterElementGroup.CollapsedSelectedHotActive:
                            graphics.FillRectangle(SystemBrushes.Highlight, groupBounds.BoundsSelection);
                            break;
                        case BetterListViewPainterElementGroup.CollapsedSelectedInactive:
                        case BetterListViewPainterElementGroup.CollapsedSelectedHotInactive:
                            graphics.FillRectangle(SystemBrushes.Control, groupBounds.BoundsSelection);
                            break;
                    }
                    if (painterElementGroup == BetterListViewPainterElementGroup.CollapsedHotFocused || painterElementGroup == BetterListViewPainterElementGroup.ExpandedHotFocused || painterElementGroup == BetterListViewPainterElementGroup.ExpandedSelectedFocusedActive || painterElementGroup == BetterListViewPainterElementGroup.ExpandedSelectedHotFocusedActive || painterElementGroup == BetterListViewPainterElementGroup.Focused) {
                        BetterListViewBasePainter.DrawFocusRectangle(graphics, groupBounds.BoundsSelection, controlBackColor);
                    }
                }
            }
            graphics.IntersectClip(groupBounds.BoundsInner);
            int num = groupBounds.BoundsInner.Left;
            int num2 = groupBounds.BoundsInner.Right - 8 - 1;
            if (!groupBounds.BoundsExpandButton.Size.IsEmpty) {
                if (eventArgs.DrawExpandButton) {
                    Size size = ((!BetterListViewPainter.TryGetRenderer(painterElementGroupExpandButton, out renderer)) ? BetterListViewBasePainter.GetNodeGlyphSize(graphics) : renderer.GetPartSize(graphics, ThemeSizeType.True));
                    Rectangle bounds = new Rectangle(groupBounds.BoundsExpandButton.Left + (groupBounds.BoundsExpandButton.Width - size.Width >> 1), groupBounds.BoundsExpandButton.Top + (groupBounds.BoundsExpandButton.Height - size.Height >> 1), size.Width, size.Height);
                    if (renderer != null) {
                        renderer.DrawBackground(graphics, bounds);
                    }
                    else {
                        BetterListViewPainterElementNode painterElementNode;
                        switch (painterElementGroupExpandButton) {
                            case BetterListViewPainterElementGroupExpandButton.CollapsedNormal:
                            case BetterListViewPainterElementGroupExpandButton.SelectedCollapsedNormal:
                                painterElementNode = BetterListViewPainterElementNode.Closed;
                                break;
                            case BetterListViewPainterElementGroupExpandButton.CollapsedHot:
                            case BetterListViewPainterElementGroupExpandButton.CollapsedPressed:
                            case BetterListViewPainterElementGroupExpandButton.SelectedCollapsedHot:
                            case BetterListViewPainterElementGroupExpandButton.SelectedCollapsedPressed:
                                painterElementNode = BetterListViewPainterElementNode.ClosedHot;
                                break;
                            case BetterListViewPainterElementGroupExpandButton.ExpandedNormal:
                                painterElementNode = BetterListViewPainterElementNode.Opened;
                                break;
                            case BetterListViewPainterElementGroupExpandButton.ExpandedHot:
                            case BetterListViewPainterElementGroupExpandButton.ExpandedPressed:
                                painterElementNode = BetterListViewPainterElementNode.OpenedHot;
                                break;
                            default:
                                throw new ApplicationException($"Unknown painter element: '{painterElementGroupExpandButton}'");
                        }
                        BetterListViewBasePainter.DrawNodeGlyph(graphics, bounds, painterElementNode);
                    }
                }
                num = groupBounds.BoundsExpandButton.Right + 8;
            }
            if (!groupBounds.BoundsImage.Size.IsEmpty) {
                if (eventArgs.DrawImage) {
                    Image elementImage = BetterListViewBasePainter.GetElementImage(group, imageList);
                    if (elementImage != null) {
                        BetterListViewBasePainter.DrawImage(graphics, elementImage, groupBounds.BoundsImage, byte.MaxValue, enabled);
                    }
                }
                BetterListViewImageAlignmentHorizontal betterListViewImageAlignmentHorizontal = ((group.HeaderAlignmentHorizontalImage == BetterListViewImageAlignmentHorizontal.Default) ? layoutImageAlignmentHorizontal : group.HeaderAlignmentHorizontalImage);
                if (betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextLeft || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextCenter || betterListViewImageAlignmentHorizontal == BetterListViewImageAlignmentHorizontal.BeforeTextRight) {
                    num = groupBounds.BoundsImageFrame.Right + 8;
                }
                else {
                    num2 = groupBounds.BoundsImageFrame.Left - 8 - 1;
                }
            }
            if (groupBounds.BoundsText.Width != 0 && eventArgs.DrawText) {
                Color colorForeground = ((painterElementGroup == BetterListViewPainterElementGroup.Undefined || BetterListViewPainter.CanGetRenderer(painterElementGroup)) ? group.ForeColor : ((painterElementGroup != BetterListViewPainterElementGroup.CollapsedSelectedActive && painterElementGroup != BetterListViewPainterElementGroup.CollapsedSelectedHotActive) ? group.ForeColor : SystemColors.HighlightText));
                colorForeground = BetterListViewBasePainter.GetUpdatedColor(colorForeground, group.BackColor, isCut: false, enabled);
                group.DrawingDrawText(graphics, colorForeground, groupBounds.BoundsText);
            }
            if (eventArgs.DrawSeparator) {
                TextAlignmentHorizontal textAlignmentHorizontal = ((group.HeaderAlignmentHorizontal == TextAlignmentHorizontal.Default) ? layoutTextAlignmentHorizontal : group.HeaderAlignmentHorizontal);
                Pen pen = new Pen(Color.FromArgb(64, SystemColors.ControlText));
                int num3 = groupBounds.BoundsInner.Top + (groupBounds.BoundsInner.Height >> 1);
                if (textAlignmentHorizontal == TextAlignmentHorizontal.Left || textAlignmentHorizontal == TextAlignmentHorizontal.Center) {
                    int num5 = groupBounds.BoundsText.Right + 8;
                    int num7 = num2;
                    if (num5 < num7) {
                        graphics.DrawLine(pen, num5, num3, num7, num3);
                    }
                }
                if (textAlignmentHorizontal == TextAlignmentHorizontal.Right || textAlignmentHorizontal == TextAlignmentHorizontal.Center) {
                    int num4 = num;
                    int num6 = groupBounds.BoundsText.Left - 8 - 1;
                    if (num4 < num6) {
                        graphics.DrawLine(pen, num4, num3, num6, num3);
                    }
                }
            }
            graphics.SetClip(clipBounds);
        }

        /// <summary>
        ///   Draw selection over an item.
        /// </summary>
        /// <param name="graphics"> Graphics object for drawing </param>
        /// <param name="itemBounds"> item boundaries </param>
        /// <param name="painterElementItem"> item element to draw </param>
        /// <param name="controlBackColor"> background color of the control </param>
        /// <param name="selectionRenderingOptions"> selection rendering options </param>
        /// <param name="focusedSubItemIndex"> index of a focused sub-item </param>
        public static void DrawItemSelection(Graphics graphics, BetterListViewItemBounds itemBounds, BetterListViewPainterElementItem painterElementItem, Color controlBackColor, BetterListViewSelectionRenderingOptions selectionRenderingOptions, int focusedSubItemIndex) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckNotEqual(painterElementItem, BetterListViewPainterElementItem.Undefined, "painterElementItem", "BetterListViewPainterElementItem.Undefined");
            Checks.CheckNotNull(itemBounds, "bounds");
            if (focusedSubItemIndex != -1) {
                if (painterElementItem == BetterListViewPainterElementItem.Focused || painterElementItem == BetterListViewPainterElementItem.HotFocused || painterElementItem == BetterListViewPainterElementItem.SelectedFocused || painterElementItem == BetterListViewPainterElementItem.SelectedHotFocused) {
                    Checks.CheckBounds(focusedSubItemIndex, 0, itemBounds.SubItemBounds.Count - 1, "focusedSubItemIndex");
                }
                else {
                    Checks.CheckTrue(focusedSubItemIndex >= 0, "focusedSubItemIndex >= 0");
                }
            }
            if (focusedSubItemIndex == -1 || focusedSubItemIndex == 0) {
                BetterListViewBasePainter.DrawSelection(graphics, itemBounds.BoundsSelection, painterElementItem, controlBackColor, selectionRenderingOptions);
                return;
            }
            //BetterListViewPainterElementItem betterListViewPainterElementItem = painterElementItem switch {
            //    BetterListViewPainterElementItem.Focused => BetterListViewPainterElementItem.Undefined,
            //    BetterListViewPainterElementItem.HotFocused => BetterListViewPainterElementItem.Hot,
            //    BetterListViewPainterElementItem.SelectedFocused => BetterListViewPainterElementItem.Selected,
            //    BetterListViewPainterElementItem.SelectedHotFocused => BetterListViewPainterElementItem.SelectedHot,
            //    _ => throw new ApplicationException($"Not a painter element for focused item: '{painterElementItem}'."),
            //};
            BetterListViewPainterElementItem betterListViewPainterElementItem;
            switch (painterElementItem) {
                case BetterListViewPainterElementItem.Focused:
                    betterListViewPainterElementItem = BetterListViewPainterElementItem.Undefined;
                    break;
                case BetterListViewPainterElementItem.HotFocused:
                    betterListViewPainterElementItem = BetterListViewPainterElementItem.Hot;
                    break;
                case BetterListViewPainterElementItem.SelectedFocused:
                    betterListViewPainterElementItem = BetterListViewPainterElementItem.Selected;
                    break;
                case BetterListViewPainterElementItem.SelectedHotFocused:
                    betterListViewPainterElementItem = BetterListViewPainterElementItem.SelectedHot;
                    break;
                default:
                    throw new ApplicationException($"Not a painter element for focused item: '{painterElementItem}'.");
                    break;
            }

            if (betterListViewPainterElementItem != BetterListViewPainterElementItem.Undefined) {
                BetterListViewBasePainter.DrawSelection(graphics, itemBounds.BoundsSelection, betterListViewPainterElementItem, controlBackColor, selectionRenderingOptions);
            }
            BetterListViewBasePainter.DrawSelection(graphics, itemBounds.SubItemBounds[focusedSubItemIndex].BoundsOuter, BetterListViewPainterElementItem.Focused, controlBackColor, selectionRenderingOptions);
        }

        /// <summary>
        ///   Draw background of an item.
        /// </summary>
        /// <param name="graphics"> Graphics object for drawing </param>
        /// <param name="item"> item to draw </param>
        /// <param name="itemBounds"> item boundaries </param>
        /// <param name="columnCount"> number of visible column headers </param>
        public static void DrawItemBackground(Graphics graphics, BetterListViewItem item, BetterListViewItemBounds itemBounds, int columnCount) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckNotNull(item, "item");
            Checks.CheckNotNull(itemBounds, "itemBounds");
            Checks.CheckTrue(columnCount >= 0, "columnCount >= 0");
            if (itemBounds.BoundsOuter.Width == 0 || itemBounds.BoundsOuter.Height == 0) {
                return;
            }
            RectangleF clipBounds = graphics.ClipBounds;
            graphics.IntersectClip(itemBounds.BoundsOuterExtended);
            if (!item.BackColor.IsEmpty && item.BackColor != Color.Transparent) {
                Rectangle rect = ((!item.UseItemStyleForSubItems && columnCount != 0) ? itemBounds.SubItemBounds[0].BoundsOuter : itemBounds.BoundsOuterExtended);
                Brush brush = new SolidBrush(item.BackColor);
                graphics.FillRectangle(brush, rect);
                brush.Dispose();
            }
            if (!item.UseItemStyleForSubItems) {
                for (int i = 1; i < Math.Min(itemBounds.SubItemBounds.Count, columnCount); i++) {
                    BetterListViewSubItem betterListViewSubItem = item.SubItems[i];
                    BetterListViewSubItemBounds betterListViewSubItemBounds = itemBounds.SubItemBounds[i];
                    if (!betterListViewSubItem.BackColor.IsEmpty && betterListViewSubItem.BackColor != Color.Transparent) {
                        Brush brush2 = new SolidBrush(betterListViewSubItem.BackColor);
                        graphics.FillRectangle(brush2, betterListViewSubItemBounds.BoundsOuter);
                        brush2.Dispose();
                    }
                }
            }
            graphics.SetClip(clipBounds);
        }

        public static void DrawItemForeground(BetterListViewDrawItemEventArgs eventArgs, ImageList imageList, ImageBorderType imageBorderType, int imageBorderThickness, Color imageBorderColor, BetterListViewPainterElementItem painterElementItem, BetterListViewPainterElementNode painterElementNode, CheckBoxState itemStateCheckBox, int maximumSubItemCount, bool enabled, bool highlightSubItems, int labelEditIndex, bool cacheImages) {
            if (eventArgs == null) {
                throw new ArgumentNullException("bad boy: eventArgs");
            }
            Graphics graphics = eventArgs.Graphics;
            BetterListViewItem item = eventArgs.Item;
            if (item == null) {
                throw new ArgumentNullException("bad boy: item");
            }
            if (item.SubItems.Count == 0) {
                throw new ArgumentNullException("bad boy: sub item count");
            }
            BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
            BetterListViewItemBounds itemBounds = eventArgs.ItemBounds;
            if (graphics == null) {
                throw new ArgumentNullException("bad boy: graphics");
            }
            if (betterListViewSubItem == null) {
                throw new ArgumentNullException("bad boy: subItemFirst");
            }
            if (itemBounds == null || itemBounds.BoundsInner.Width == 0 || itemBounds.BoundsInner.Height == 0) {
                return;
            }
            RectangleF clipBounds = graphics.ClipBounds;
            graphics.IntersectClip(itemBounds.SubItemBounds[0].BoundsInner);
            if (eventArgs.DrawExpandButton && !itemBounds.BoundsExpandButton.Size.IsEmpty && painterElementNode != BetterListViewPainterElementNode.Undefined) {
                BetterListViewBasePainter.DrawNodeGlyph(graphics, itemBounds.BoundsExpandButton, painterElementNode);
            }
            Rectangle boundsImage = itemBounds.SubItemBounds[0].BoundsImage;
            byte opacity = (byte)(item.IsCut ? 127u : 255u);
            if (eventArgs.DrawImage && !boundsImage.Size.IsEmpty) {
                Image elementImage = BetterListViewBasePainter.GetElementImage(betterListViewSubItem, imageList);
                if (elementImage != null) {
                    if (eventArgs.DrawImageBorder) {
                        betterListViewSubItem.DrawingDrawImage(graphics, elementImage, boundsImage, opacity, enabled, imageBorderType, imageBorderThickness, imageBorderColor, cacheImages);
                    }
                    else {
                        betterListViewSubItem.DrawingDrawImage(graphics, elementImage, boundsImage, opacity, enabled, ImageBorderType.None, 0, Painter.DefaultBorderColor, cacheImages);
                    }
                }
            }
            if (eventArgs.DrawCheckBox && !itemBounds.BoundsCheckBox.Size.IsEmpty) {
                Bitmap bitmap;
                Graphics graphics2;
                Point glyphLocation;
                if (item.IsCut) {
                    bitmap = new Bitmap(itemBounds.BoundsCheckBox.Width, itemBounds.BoundsCheckBox.Height);
                    graphics2 = Graphics.FromImage(bitmap);
                    glyphLocation = new Point(0, 0);
                }
                else {
                    bitmap = null;
                    graphics2 = graphics;
                    glyphLocation = itemBounds.BoundsCheckBox.Location;
                }
                switch (item.CheckBoxAppearance) {
                    case BetterListViewCheckBoxAppearance.CheckBox:
                        CheckBoxRenderer.DrawCheckBox(graphics2, glyphLocation, itemStateCheckBox);
                        break;
                    case BetterListViewCheckBoxAppearance.RadioButton:
                        //RadioButtonRenderer.DrawRadioButton(graphics2, glyphLocation, itemStateCheckBox switch {
                        //	CheckBoxState.CheckedDisabled => RadioButtonState.CheckedDisabled,
                        //	CheckBoxState.CheckedHot => RadioButtonState.CheckedHot,
                        //	CheckBoxState.CheckedNormal => RadioButtonState.CheckedNormal,
                        //	CheckBoxState.CheckedPressed => RadioButtonState.CheckedPressed,
                        //	CheckBoxState.MixedDisabled => RadioButtonState.CheckedDisabled,
                        //	CheckBoxState.MixedHot => RadioButtonState.CheckedHot,
                        //	CheckBoxState.MixedNormal => RadioButtonState.CheckedNormal,
                        //	CheckBoxState.MixedPressed => RadioButtonState.CheckedPressed,
                        //	CheckBoxState.UncheckedDisabled => RadioButtonState.UncheckedDisabled,
                        //	CheckBoxState.UncheckedHot => RadioButtonState.UncheckedHot,
                        //	CheckBoxState.UncheckedNormal => RadioButtonState.UncheckedNormal,
                        //	CheckBoxState.UncheckedPressed => RadioButtonState.UncheckedPressed,
                        //	_ => throw new ApplicationException($"Unknown check box state: {itemStateCheckBox}"),
                        //});
                        var rbs = RadioButtonState.UncheckedNormal;
                        switch (itemStateCheckBox) {
                            case CheckBoxState.CheckedDisabled:
                                rbs = RadioButtonState.CheckedDisabled;
                                break;
                            case CheckBoxState.CheckedHot:
                                rbs = RadioButtonState.CheckedHot;
                                break;
                            case CheckBoxState.CheckedNormal:
                                rbs = RadioButtonState.CheckedNormal;
                                break;
                            case CheckBoxState.CheckedPressed:
                                rbs = RadioButtonState.CheckedPressed;
                                break;
                            case CheckBoxState.MixedDisabled:
                                rbs = RadioButtonState.CheckedDisabled;
                                break;
                            case CheckBoxState.MixedHot:
                                rbs = RadioButtonState.CheckedHot;
                                break;
                            case CheckBoxState.MixedNormal:
                                rbs = RadioButtonState.CheckedNormal;
                                break;
                            case CheckBoxState.MixedPressed:
                                rbs = RadioButtonState.CheckedPressed;
                                break;
                            case CheckBoxState.UncheckedDisabled:
                                rbs = RadioButtonState.UncheckedDisabled;
                                break;
                            case CheckBoxState.UncheckedHot:
                                rbs = RadioButtonState.UncheckedHot;
                                break;
                            case CheckBoxState.UncheckedNormal:
                                rbs = RadioButtonState.UncheckedNormal;
                                break;
                            case CheckBoxState.UncheckedPressed:
                                rbs = RadioButtonState.UncheckedPressed;
                                break;
                            default:
                                throw new ApplicationException($"Unknown check box state: {itemStateCheckBox}");
                                break;
                        }
                        break;
                }
                if (item.IsCut) {
                    BetterListViewBasePainter.DrawImage(graphics, bitmap, itemBounds.BoundsCheckBox, 128, enabled: true);
                    bitmap.Dispose();
                    graphics2.Dispose();
                }
            }
            BetterListViewSubItemBounds betterListViewSubItemBounds = itemBounds.SubItemBounds[0];
            if (betterListViewSubItemBounds == null) {
                throw new ArgumentNullException("bad boy: subItemBoundsFirst");
            }
            if (eventArgs.DrawText && betterListViewSubItemBounds.BoundsText.Width != 0 && labelEditIndex != 0) {
                Color colorForeground;
                if (painterElementItem != BetterListViewPainterElementItem.Undefined && !BetterListViewBasePainter.CanGetRenderer(painterElementItem)) {
                    switch (painterElementItem) {
                        case BetterListViewPainterElementItem.Selected:
                        case BetterListViewPainterElementItem.SelectedFocused:
                        case BetterListViewPainterElementItem.SelectedHot:
                        case BetterListViewPainterElementItem.SelectedHotFocused:
                            colorForeground = SystemColors.HighlightText;
                            break;
                        case BetterListViewPainterElementItem.Focused:
                        case BetterListViewPainterElementItem.Hot:
                        case BetterListViewPainterElementItem.HotFocused:
                            colorForeground = item.ForeColor;
                            break;
                        default:
                            colorForeground = SystemColors.ControlText;
                            break;
                    }
                }
                else {
                    colorForeground = item.ForeColor;
                }
                colorForeground = BetterListViewBasePainter.GetUpdatedColor(colorForeground, item.BackColor.IsEmpty ? item.OwnerCollection.OwnerControl.BackColor : item.BackColor, item.IsCut, enabled);
                betterListViewSubItem.DrawingDrawText(graphics, colorForeground, betterListViewSubItemBounds.BoundsText, betterListViewSubItemBounds.MaximumTextLines);
            }
            int val = Math.Min(itemBounds.SubItemBounds.Count, maximumSubItemCount);
            val = Math.Min(val, item.SubItems.Count);
            for (int i = 1; i < val; i++) {
                graphics.SetClip(clipBounds);
                graphics.IntersectClip(itemBounds.SubItemBounds[i].BoundsInner);
                BetterListViewSubItem betterListViewSubItem2 = item.SubItems[i];
                BetterListViewSubItemBounds betterListViewSubItemBounds2 = itemBounds.SubItemBounds[i];
                if (betterListViewSubItem2 == null) {
                    throw new ArgumentNullException($"bad boy: subItem (column {i})");
                }
                if (betterListViewSubItemBounds2 == null) {
                    throw new ArgumentNullException($"bad boy: subItemBounds (column {i})");
                }
                if ((i >= eventArgs.DrawSubItemImages.Length || eventArgs.DrawSubItemImages[i]) && !betterListViewSubItemBounds2.BoundsImage.Size.IsEmpty) {
                    Image elementImage2 = BetterListViewBasePainter.GetElementImage(betterListViewSubItem2, imageList);
                    if (elementImage2 != null) {
                        if (i >= eventArgs.DrawSubItemImageBorders.Length || eventArgs.DrawSubItemImageBorders[i]) {
                            betterListViewSubItem2.DrawingDrawImage(graphics, elementImage2, betterListViewSubItemBounds2.BoundsImage, opacity, enabled, imageBorderType, imageBorderThickness, imageBorderColor, cacheImages);
                        }
                        else {
                            betterListViewSubItem2.DrawingDrawImage(graphics, elementImage2, betterListViewSubItemBounds2.BoundsImage, opacity, enabled, ImageBorderType.None, 0, Painter.DefaultBorderColor, cacheImages);
                        }
                    }
                }
                if ((i >= eventArgs.DrawSubItemTexts.Length || eventArgs.DrawSubItemTexts[i]) && betterListViewSubItemBounds2.BoundsText.Width != 0 && labelEditIndex != i) {
                    Color colorForeground2 = ((painterElementItem != BetterListViewPainterElementItem.Undefined && !BetterListViewBasePainter.CanGetRenderer(painterElementItem)) ? ((highlightSubItems && (painterElementItem == BetterListViewPainterElementItem.Selected || painterElementItem == BetterListViewPainterElementItem.SelectedFocused || painterElementItem == BetterListViewPainterElementItem.SelectedHot || painterElementItem == BetterListViewPainterElementItem.SelectedHotFocused)) ? SystemColors.HighlightText : ((painterElementItem != BetterListViewPainterElementItem.Focused && painterElementItem != BetterListViewPainterElementItem.Hot && painterElementItem != BetterListViewPainterElementItem.HotFocused && painterElementItem != BetterListViewPainterElementItem.Selected && painterElementItem != BetterListViewPainterElementItem.SelectedFocused && painterElementItem != BetterListViewPainterElementItem.SelectedHot && painterElementItem != BetterListViewPainterElementItem.SelectedHotFocused) ? SystemColors.ControlText : (item.UseItemStyleForSubItems ? item.ForeColor : betterListViewSubItem2.ForeColor))) : (item.UseItemStyleForSubItems ? item.ForeColor : betterListViewSubItem2.ForeColor));
                    Color color = (item.UseItemStyleForSubItems ? item.BackColor : betterListViewSubItem2.BackColor);
                    colorForeground2 = BetterListViewBasePainter.GetUpdatedColor(colorForeground2, (color.IsEmpty && item.OwnerCollection != null) ? item.OwnerCollection.OwnerControl.BackColor : color, item.IsCut, enabled);
                    betterListViewSubItem2.DrawingDrawText(graphics, colorForeground2, betterListViewSubItemBounds2.BoundsText, betterListViewSubItemBounds2.MaximumTextLines);
                }
            }
            graphics.SetClip(clipBounds);
        }

        /// <summary>
        ///   Draw selection rectangle.
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        /// <param name="bounds">selection rectangle boundaries</param>
        public static void DrawSelectionRectangle(Graphics graphics, Rectangle bounds) {
            Checks.CheckNotNull(graphics, "graphics");
            if (bounds.Width != 0 && bounds.Height != 0) {
                Brush brush = new SolidBrush(Color.FromArgb(85, SystemColors.HotTrack));
                graphics.FillRectangle(brush, new Rectangle(bounds.Left + 1, bounds.Top + 1, bounds.Width - 1, bounds.Height - 1));
                graphics.DrawRectangle(SystemPens.Highlight, bounds);
                brush.Dispose();
            }
        }

        /// <summary>
        ///   Draw control background.
        /// </summary>
        /// <param name="eventArgs">DrawBackground drawing event data</param>
        /// <param name="backgroundImage">background image</param>
        /// <param name="backgroundImageAlignment">background image alignment</param>
        /// <param name="backgroundImageLayout">background image layout</param>
        /// <param name="backgroundImageOpacity">background image opacity</param>
        /// <param name="colorBackground">background color of the control</param>
        /// <param name="colorSortedColumn">color of the sorted column background</param>
        /// <param name="enabled">draw background in enabled state</param>
        public static void DrawBackground(BetterListViewDrawBackgroundEventArgs eventArgs, Image backgroundImage, System.Drawing.ContentAlignment backgroundImageAlignment, ImageLayout backgroundImageLayout, byte backgroundImageOpacity, Color colorBackground, Color colorSortedColumn, bool enabled) {
            Checks.CheckNotNull(eventArgs, "eventArgs");
            if (eventArgs.BackgroundBounds.Size.Width == 0 || eventArgs.BackgroundBounds.Size.Height == 0) {
                return;
            }
            Graphics graphics = eventArgs.Graphics;
            if (enabled) {
                if (eventArgs.DrawBackground) {
                    Brush brush = new SolidBrush(colorBackground);
                    graphics.FillRectangle(brush, eventArgs.BackgroundBounds);
                    brush.Dispose();
                }
                if (eventArgs.DrawImage) {
                    BetterListViewPainter.DrawBackgroundImage(graphics, eventArgs.BackgroundBounds, backgroundImage, backgroundImageAlignment, backgroundImageLayout, backgroundImageOpacity, enabled: true);
                }
                if (eventArgs.DrawSortedColumn && eventArgs.SortedColumnHeaderBounds != null && eventArgs.SortedColumnHeaderBounds.BoundsOuter.Width > 0 && eventArgs.SortedColumnHeaderBounds.BoundsOuter.Height > 0) {
                    Brush brush2 = new SolidBrush(colorSortedColumn);
                    Rectangle rect = new Rectangle(eventArgs.SortedColumnHeaderBounds.BoundsOuter.Left, eventArgs.BackgroundBounds.Top, eventArgs.SortedColumnHeaderBounds.BoundsOuter.Width, eventArgs.BackgroundBounds.Height);
                    graphics.FillRectangle(brush2, rect);
                    brush2.Dispose();
                }
            }
            else {
                if (eventArgs.DrawBackground) {
                    graphics.FillRectangle(SystemBrushes.Control, eventArgs.BackgroundBounds);
                }
                if (eventArgs.DrawImage) {
                    BetterListViewPainter.DrawBackgroundImage(graphics, eventArgs.BackgroundBounds, backgroundImage, backgroundImageAlignment, backgroundImageLayout, backgroundImageOpacity, enabled: false);
                }
            }
        }

        /// <summary>
        ///   Draw grid lines.
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        /// <param name="gridLines">type of grid lines to draw</param>
        /// <param name="layoutElementsColumns">column header layout elements</param>
        /// <param name="visibleRangeColumns">range of currently visible column header indices</param>
        /// <param name="layoutElementsItems">item layout elements</param>
        /// <param name="visibleRangeItems">range of currently visible item indices</param>
        /// <param name="contentBounds">content area boundaries</param>
        /// <param name="offsetContentFromAbsolute">offset from absolute coordinates of column header layout elements</param>
        /// <param name="color">grid line color</param>
        public static void DrawGridLines(Graphics graphics, BetterListViewGridLines gridLines, ReadOnlyCollection<BetterListViewColumnHeader> layoutElementsColumns, BetterListViewLayoutVisibleRange visibleRangeColumns, ReadOnlyCollection<BetterListViewItem> layoutElementsItems, BetterListViewLayoutVisibleRange visibleRangeItems, Rectangle contentBounds, Point offsetContentFromAbsolute, Color color) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckNotEqual(gridLines, BetterListViewGridLines.None, "gridLines", "BetterListViewGridLines.None");
            Checks.CheckNotNull(layoutElementsColumns, "layoutElementsColumns");
            Checks.CheckTrue(visibleRangeColumns.IsUndefined == (layoutElementsColumns.Count == 0), "visibleRangeColumns.IsUndefined == (layoutElementsColumns.Count == 0)");
            Checks.CheckNotNull(layoutElementsItems, "layoutElementsItems");
            Checks.CheckTrue(visibleRangeItems.IsUndefined == (layoutElementsItems.Count == 0), "visibleRangeItems.IsUndefined == (layoutElementsItems.Count == 0)");
            Checks.CheckFalse(color.IsEmpty, "color.IsEmpty");
            Checks.CheckRectangle(contentBounds, "contentBounds");
            if (contentBounds.Width == 0 || contentBounds.Height == 0) {
                return;
            }
            Pen pen = new Pen(color, 1f);
            if ((gridLines == BetterListViewGridLines.Vertical || gridLines == BetterListViewGridLines.Grid) && layoutElementsColumns.Count != 0) {
                for (int i = visibleRangeColumns.IndexElementFirst; i <= visibleRangeColumns.IndexElementLast; i++) {
                    int num = ((IBetterListViewLayoutElementDisplayable)layoutElementsColumns[i]).LayoutBounds.BoundsOuter.Right - 1 + offsetContentFromAbsolute.X;
                    graphics.DrawLine(pen, num, contentBounds.Top, num, contentBounds.Bottom - 1);
                }
            }
            if ((gridLines == BetterListViewGridLines.Horizontal || gridLines == BetterListViewGridLines.Grid) && layoutElementsItems.Count != 0) {
                for (int j = visibleRangeItems.IndexElementFirst; j <= visibleRangeItems.IndexElementLast; j++) {
                    int num2 = ((IBetterListViewLayoutElementDisplayable)layoutElementsItems[j]).LayoutBounds.BoundsOuterExtended.Bottom - 1 + offsetContentFromAbsolute.Y;
                    graphics.DrawLine(pen, contentBounds.Left, num2, contentBounds.Right - 1, num2);
                }
            }
        }

        private static void DrawBackgroundImage(Graphics graphics, Rectangle bounds, Image backgroundImage, System.Drawing.ContentAlignment backgroundImageAlignment, ImageLayout backgroundImageLayout, byte backgroundImageOpacity, bool enabled) {
            if (backgroundImage == null) {
                return;
            }
            if (backgroundImageLayout == ImageLayout.Tile) {
                Rectangle rectangle = Rectangle.Ceiling(graphics.ClipBounds);
                graphics.SetClip(rectangle);
                int num = Math.Max(bounds.Width / backgroundImage.Width, 0) * backgroundImage.Width;
                int num2 = Math.Max(bounds.Height / backgroundImage.Height, 0) * backgroundImage.Height;
                for (int num3 = num2; num3 >= 0; num3 -= backgroundImage.Height) {
                    for (int num4 = num; num4 >= 0; num4 -= backgroundImage.Width) {
                        Rectangle bounds2 = new Rectangle(num4, num3, backgroundImage.Width, backgroundImage.Height);
                        if (bounds2.IntersectsWith(rectangle)) {
                            BetterListViewBasePainter.DrawImage(graphics, backgroundImage, bounds2, backgroundImageOpacity, enabled);
                        }
                    }
                }
                return;
            }
            Rectangle bounds3 = Rectangle.Empty;
            switch (backgroundImageLayout) {
                case ImageLayout.None: {
                        int x = 0;
                        int y = 0;
                        switch (backgroundImageAlignment) {
                            case System.Drawing.ContentAlignment.TopLeft:
                            case System.Drawing.ContentAlignment.MiddleLeft:
                            case System.Drawing.ContentAlignment.BottomLeft:
                                x = bounds.Left;
                                break;
                            case System.Drawing.ContentAlignment.TopCenter:
                            case System.Drawing.ContentAlignment.MiddleCenter:
                            case System.Drawing.ContentAlignment.BottomCenter:
                                x = bounds.Left + (bounds.Width - backgroundImage.Width >> 1);
                                break;
                            case System.Drawing.ContentAlignment.TopRight:
                            case System.Drawing.ContentAlignment.MiddleRight:
                            case System.Drawing.ContentAlignment.BottomRight:
                                x = bounds.Right - backgroundImage.Width;
                                break;
                        }
                        switch (backgroundImageAlignment) {
                            case System.Drawing.ContentAlignment.TopLeft:
                            case System.Drawing.ContentAlignment.TopCenter:
                            case System.Drawing.ContentAlignment.TopRight:
                                y = bounds.Top;
                                break;
                            case System.Drawing.ContentAlignment.MiddleLeft:
                            case System.Drawing.ContentAlignment.MiddleCenter:
                            case System.Drawing.ContentAlignment.MiddleRight:
                                y = bounds.Top + (bounds.Height - backgroundImage.Height >> 1);
                                break;
                            case System.Drawing.ContentAlignment.BottomLeft:
                            case System.Drawing.ContentAlignment.BottomCenter:
                            case System.Drawing.ContentAlignment.BottomRight:
                                y = bounds.Bottom - backgroundImage.Height;
                                break;
                        }
                        bounds3 = new Rectangle(x, y, backgroundImage.Width, backgroundImage.Height);
                        break;
                    }
                case ImageLayout.Center:
                    bounds3 = new Rectangle(bounds.Left + (bounds.Width - backgroundImage.Width >> 1), bounds.Top + (bounds.Height - backgroundImage.Height >> 1), backgroundImage.Width, backgroundImage.Height);
                    break;
                case ImageLayout.Stretch:
                    bounds3 = bounds;
                    break;
                case ImageLayout.Zoom: {
                        float num5 = Math.Min((float)bounds.Width / (float)backgroundImage.Width, (float)bounds.Height / (float)backgroundImage.Height);
                        Size size = new Size((int)Math.Ceiling((float)backgroundImage.Width * num5), (int)Math.Ceiling((float)backgroundImage.Height * num5));
                        bounds3 = new Rectangle(bounds.Left + (bounds.Width - size.Width >> 1), bounds.Top + (bounds.Height - size.Height >> 1), size.Width, size.Height);
                        break;
                    }
            }
            BetterListViewBasePainter.DrawImage(graphics, backgroundImage, bounds3, backgroundImageOpacity, enabled);
        }
    }
}