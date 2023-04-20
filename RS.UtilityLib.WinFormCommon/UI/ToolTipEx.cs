using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace RS.UtilityLib.WinFormCommon.UI
{

    [ToolboxBitmap(typeof(ToolTip))]
    public partial class ToolTipEx : ToolTip
    {
        #region Events
        public event Action<ToolTipEx> OnAfterHideEvent;

        public event Action<ToolTipWidget> OnMouseDownEvent;
        public event Action<ToolTipWidget> OnMouseUpEvent;
        public event Action<ToolTipWidget> OnMouseMoveEvent;

        public bool ActionMouseMove() {
            if (IsMouseInTip) {
                if (this.Visible) {
                    ToolTipWidget widget = null;
                    foreach (var v in this._widgets) {
                        if (v.ClientRect.Contains(Cursor.Position)) {
                            widget = v;
                            //ToolTipImport.UpdateWindow(this.Handle);                           
                            break;
                        }
                    }
                    if (this.OnMouseMoveEvent != null) {
                        this.OnMouseMoveEvent(widget);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool ActionMouseDown() {
            if (IsMouseInTip) {
                if (this.Visible) {
                    ToolTipWidget widget = null;
                    foreach (var v in this._widgets) {
                        if (v.ClientRect.Contains(Cursor.Position)) {
                            widget = v;
                            break;
                        }
                    }
                    if (this.OnMouseDownEvent != null) {
                        this.OnMouseDownEvent(widget);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool ActionMouseUp() {
            if (IsMouseInTip) {
                if (this.Visible) {
                    ToolTipWidget widget = null;
                    foreach (var v in this._widgets) {
                        if (v.ClientRect.Contains(Cursor.Position)) {
                            widget = v;
                            break;
                        }
                    }
                    if (this.OnMouseUpEvent != null) {
                        this.OnMouseUpEvent(widget);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsMouseInTip {
            get {
                Rectangle tRec = new Rectangle(this.Position, this.Size);
                return tRec.Contains(Cursor.Position);
            }
        }
        public bool IsMouseInClient(Rectangle clientRect) {
            return clientRect.Contains(Cursor.Position);
        }
        public void AddWidget(ToolTipWidget widget) {
            widget.Owner = this;
            this._widgets.Add(widget);
        }
        public ToolTipWidget[] GetWidgets() {
            return this._widgets.ToArray();
        }
        public ToolTipWidget GetWidget(int widgetindex) {
            return this._widgets[widgetindex];
        }
        #endregion
        public bool IsShowWidget { get; set; }
        #region Fields
        private List<ToolTipWidget> _widgets = new List<ToolTipWidget>();

        private ImageDc _backDc;
        private Image _image;
        private Image _backImage;
        private Region _backRegion;
        private double _opacity = 1d;
        private ToolTipColorTable _colorTable;
        private Font _titleFont = new Font("宋体", 9, FontStyle.Bold);
        private Size _imageSize = SystemInformation.SmallIconSize;
        private Size _Size = new Size(32, 32);
        #endregion

        #region Constructors

        public ToolTipEx()
            : base() {
            InitOwnerDraw();
        }

        public ToolTipEx(IContainer cont)
            : base(cont) {
            InitOwnerDraw();
        }
        public new void Hide(IWin32Window window) {
            base.Hide(window);
            if (this.OnAfterHideEvent != null) {
                this.OnAfterHideEvent(this);
            }
        }
        public new bool IsBalloon {
            get { return base.IsBalloon; }
            set {
                base.IsBalloon = false;
            }
        }
        public bool Visible {
            get {
                return ToolTipImport.IsWindowVisible(this.Handle) == 1;
            }
        }
        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams cp = base.CreateParams;
        //        cp.Parent = ToolTipImport.GetDesktopWindow();
        //        cp.ExStyle = 0x00000080 | 0x00000008;//WS_EX_TOOLWINDOW | WS_EX_TOPMOST
        //        return cp;
        //    }
        //}
        //public void SetWindowTransparent(byte bAlpha) {
        //    try {
        //        ToolTipImport.SetWindowLong(this.Handle, (int)ToolTipImport.WindowStyle.GWL_EXSTYLE,
        //        ToolTipImport.GetWindowLong(this.Handle, (int)ToolTipImport.WindowStyle.GWL_EXSTYLE) | (uint)ToolTipImport.ExWindowStyle.WS_EX_LAYERED);
        //        ToolTipImport.SetLayeredWindowAttributes(this.Handle, 0, bAlpha, ToolTipImport.LWA_COLORKEY | ToolTipImport.LWA_ALPHA);
        //    }
        //    catch {
        //    }
        //}
        #endregion

        #region Properties

        [Browsable(false)]
        public ToolTipColorTable ColorTable {
            get {
                if (_colorTable == null) {
                    _colorTable = new ToolTipColorTable();
                }
                return _colorTable;
            }
        }

        [DefaultValue(typeof(Font), "宋体, 9pt, style=Bold")]
        public Font TitleFont {
            get { return _titleFont; }
            set {
                if (_titleFont == null) {
                    throw new ArgumentNullException("TitleFont");
                }

                if (!_titleFont.IsSystemFont) {
                    _titleFont.Dispose();
                }

                _titleFont = value;
            }
        }

        public new ToolTipIcon ToolTipIcon {
            get { return base.ToolTipIcon; }
            set {
                if (_image != null) {
                    base.ToolTipIcon = ToolTipIcon.Info;
                }
                else {
                    base.ToolTipIcon = value;
                }
            }
        }
        /// <summary>
        /// 绘制背景图
        /// </summary>
        public Image BackImage {
            get {
                return _backImage;
            }
            set {
                _backImage = value;
                if (value == null) {
                    if (_backRegion != null) {
                        _backRegion.Dispose();
                        _backRegion = null;
                    }
                }
                else {
                    //计算边缘
                    _backRegion = CreateImgRegion((Bitmap)_backImage, 0, 0);
                }
            }
        }
        public Region Region {
            get {
                return _backRegion;
            }
        }
        /// <summary>
        /// 提示框尺寸
        /// </summary>
        public Size Size {
            get {
                return this._Size;
            }
            set {
                this._Size = value;
            }
        }
        public Point Position {
            get;
            private set;
        }
        /// <summary>
        /// 标题文本 绘制偏移位置
        /// </summary>
        public Point TitleTxtOffset {
            get;
            set;
        }
        /// <summary>
        /// 提示文本 绘制偏移位置
        /// </summary>
        public Point TipTxtOffset {
            get;
            set;
        }
        [DefaultValue(null)]
        public Image ToolTipImage {
            get { return _image; }
            set {
                _image = value;
                if (_image == null) {
                    base.ToolTipIcon = ToolTipIcon.None;
                }
                else {
                    base.ToolTipIcon = ToolTipIcon.Info;
                }
            }
        }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size ToolTipImageSize {
            get { return _imageSize; }
            set {
                if (_imageSize != value) {
                    _imageSize = value;

                    if (_imageSize.Width > 32) {
                        _imageSize.Width = 32;
                    }

                    if (_imageSize.Height > 32) {
                        _imageSize.Height = 32;
                    }

                }
            }
        }

        [DefaultValue(1d)]
        [TypeConverter(typeof(OpacityConverter))]
        public double Opacity {
            get { return _opacity; }
            set {
                if (value < 0 && value > 1) {
                    throw new ArgumentOutOfRangeException("Opacity");
                }
                //if (base.IsBalloon) { 
                _opacity = value;
                //}
            }
        }
        protected IntPtr Handle {
            get {
                if (!DesignMode) {
                    Type t = typeof(ToolTip);
                    PropertyInfo pi = t.GetProperty(
                        "Handle",
                         BindingFlags.NonPublic | BindingFlags.Instance);

                    IntPtr handle = (IntPtr)pi.GetValue(this, null);
                    return handle;
                }

                return IntPtr.Zero;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_backDc != null) {
                    _backDc.Dispose();
                    _backDc = null;
                }
                if (_backRegion != null) {
                    _backRegion.Dispose();
                    _backRegion = null;
                }
                if (_backImage != null) {
                    _backImage.Dispose();
                    _backImage = null;
                }
                if (_titleFont != null && !_titleFont.IsSystemFont) {
                    _titleFont.Dispose();
                }
                _titleFont = null;

                _image = null;
                _colorTable = null;
            }
        }

        #endregion

        #region Helper Methods

        private void InitOwnerDraw() {
            base.UseAnimation = true;
            base.UseFading = true;
            base.OwnerDraw = true;
            //
            this.Position = new Point(0, 0);
            //
            base.ReshowDelay = 800;
            base.InitialDelay = 500;
            base.AutoPopDelay = 5000;

            base.Draw += new DrawToolTipEventHandler(ToolTipExDraw);
            base.Popup += new PopupEventHandler(ToolTipExPopup);
        }

        private void ToolTipExPopup(
            object sender, PopupEventArgs e) {
            if (this.Handle == IntPtr.Zero) return;
            //设置尺寸
            Size size = e.ToolTipSize;
            size.Height = size.Height > _Size.Height ? size.Height : _Size.Height;
            size.Width = size.Width > _Size.Width ? size.Width : _Size.Width;
            e.ToolTipSize = size;
            //    
            ToolTipImport.RECT rect = new ToolTipImport.RECT();
            ToolTipImport.GetWindowRect(this.Handle, ref rect);
            this.Position = new Point(rect.Left, rect.Top);
            this.Size = size;
            //
            //是否带透明提示框
            if (Handle != IntPtr.Zero && _opacity < 1D) {
                //如果使用背景透明，获取背景图。
                TipCapture(size.Width, size.Height);
            }

        }

        private void ToolTipExDraw(
            object sender, DrawToolTipEventArgs e) {
            Graphics g = e.Graphics;
            Rectangle bounds = e.Bounds;
            int alpha = 255;
            //if (base.IsBalloon) {
            alpha = (int)(_opacity * 255);
            //}
            int defaultXOffset = 12 + this.TitleTxtOffset.X;
            int defaultTopHeight = 36 + this.TitleTxtOffset.Y;

            int tipTextXOffset = 4 + this.TipTxtOffset.X;
            int tipTextYOffset = 4 + this.TipTxtOffset.Y;
            //
            g.Clear(System.Drawing.Color.Transparent);
            if (Handle != IntPtr.Zero && _opacity < 1D && _backDc != null) {
                IntPtr hDC = g.GetHdc();
                ToolTipImport.BitBlt(
                    hDC, 0, 0, bounds.Width, bounds.Height,
                    _backDc.Hdc, 0, 0, 0xCC0020);
                g.ReleaseHdc(hDC);
            }
            //if (this._backDc != null) { 
            //    功能同上 不同的写法
            //    Image img=Image.FromHbitmap(this._backDc.HBmp);
            //    g.DrawImage(img,0,0);
            //    g.ReleaseHdc(this._backDc.Hdc);
            //    this._backDc.Dispose();
            //    this._backDc = null;
            //}
            if (this._backRegion != null) {
                g.SetClip(this._backRegion, CombineMode.Replace);
                IntPtr rPtr = this._backRegion.GetHrgn(g);
                ToolTipImport.SetWindowRgn(this.Handle, rPtr, true);
                this._backRegion.ReleaseHrgn(rPtr);
            }
            //else {
            Color backNormalColor = Color.FromArgb(
               alpha, ColorTable.BackNormal);
            Color baseColor = Color.FromArgb(
                alpha, ColorTable.BackHover);
            Color borderColor = Color.FromArgb(
                alpha, ColorTable.Border);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                bounds,
                backNormalColor,
                baseColor,
                LinearGradientMode.Vertical)) {
                g.FillRectangle(brush, bounds);
            }

            ControlPaint.DrawBorder(
                g,
                bounds,
                borderColor,
                ButtonBorderStyle.Solid);
            //}
            //绘制背景
            if (this._backImage != null) {
                RenderAlphaImage(g, this._backImage, bounds, (float)this._opacity);
            }

            Rectangle imageRect = Rectangle.Empty;
            Rectangle titleRect;
            Rectangle tipRect;

            //
            if (base.ToolTipIcon != ToolTipIcon.None) {
                tipTextXOffset = defaultXOffset;
                tipTextYOffset = defaultTopHeight;

                imageRect = new Rectangle(
                    bounds.X + defaultXOffset - (ToolTipImageSize.Width - 16) / 2,
                    bounds.Y + (defaultTopHeight - _imageSize.Height) / 2,
                    _imageSize.Width,
                    _imageSize.Height);

                Image image = _image;
                bool bDispose = false;

                if (image == null) {
                    Icon icon = GetIcon();
                    if (icon != null) {
                        image = icon.ToBitmap();
                        bDispose = true;
                    }
                }

                if (image != null) {
                    using (InterpolationModeGraphics ig =
                        new InterpolationModeGraphics(g)) {
                        if (_opacity < 1D) {
                            RenderAlphaImage(
                                g,
                                image,
                                imageRect,
                                (float)_opacity);
                        }
                        else {
                            g.DrawImage(
                                image,
                                imageRect,
                                0,
                                0,
                                image.Width,
                                image.Height,
                                GraphicsUnit.Pixel);
                        }
                    }

                    if (bDispose) {
                        image.Dispose();
                    }
                }
            }

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;

            if (!string.IsNullOrEmpty(base.ToolTipTitle)) {
                tipTextXOffset = defaultXOffset;
                tipTextYOffset = defaultTopHeight;

                int x = imageRect.IsEmpty ?
                    defaultXOffset : imageRect.Right + 3;

                titleRect = new Rectangle(
                    x,
                    bounds.Y,
                    bounds.Width - x,
                    defaultTopHeight);

                Color foreColor = Color.FromArgb(
                    alpha, ColorTable.TitleFore);

                using (Brush brush = new SolidBrush(foreColor)) {
                    g.DrawString(
                        base.ToolTipTitle,
                        _titleFont,
                        brush,
                        titleRect,
                        sf);
                }
            }

            if (!string.IsNullOrEmpty(e.ToolTipText)) {
                tipRect = new Rectangle(
                    bounds.X + tipTextXOffset,
                    bounds.Y + tipTextYOffset,
                    bounds.Width - tipTextXOffset,
                    bounds.Height - tipTextYOffset);

                sf = StringFormat.GenericTypographic;

                Color foreColor = Color.FromArgb(
                   alpha, ColorTable.TipFore);

                using (Brush brush = new SolidBrush(foreColor)) {
                    g.DrawString(
                        e.ToolTipText,
                        e.Font,
                        brush,
                        tipRect,
                        sf);
                }
            }
            if (this.IsShowWidget) {
                sf.Alignment = StringAlignment.Near;
                foreach (var w in this._widgets) {
                    //绘制自己定义的元素
                    if (w.Width > 0 && w.Height > 0 && w.Visible) {
                        if (w.Image != null) {
                            g.DrawImage(w.Image, w.Rect, 0, 0, w.Image.Width, w.Image.Height, GraphicsUnit.Pixel);
                        }
                        if (!string.IsNullOrEmpty(w.Text)) {
                            using (Brush brush = new SolidBrush(w.TextColor)) {
                                g.DrawString(w.Text, w.TextFont, brush, new RectangleF(w.Left, w.Top, w.Width, w.Height), sf);
                            }
                            //if (this.IsMouseInClient(w.ClientRect)) {
                            //    g.DrawRectangle(System.Drawing.Pens.Red,new Rectangle(w.Left,w.Top,w.Height,w.Height));
                            //}
                        }
                    }
                }

            }
            //end    
            sf.Dispose();
        }

        internal static void RenderAlphaImage(
          Graphics g,
          Image image,
          Rectangle imageRect,
          float alpha) {
            using (ImageAttributes imageAttributes = new ImageAttributes()) {
                ColorMap colorMap = new ColorMap();

                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);

                ColorMap[] remapTable = { colorMap };

                imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                float[][] colorMatrixElements = {
                    new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                    new float[] {0.0f,  0.0f,  0.0f,  alpha, 0.0f},
                    new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

                imageAttributes.SetColorMatrix(
                    wmColorMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);

                g.DrawImage(
                    image,
                    imageRect,
                    0,
                    0,
                    image.Width,
                    image.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }
        }


        private void TipCapture(int width, int height) {
            IntPtr handle = Handle;
            if (handle == IntPtr.Zero) {
                return;
            }

            ToolTipImport.RECT rect = new ToolTipImport.RECT();

            ToolTipImport.GetWindowRect(handle, ref rect);

            //Size size = new Size(
            //    rect.Right - rect.Left,
            //    rect.Bottom - rect.Top);
            Size size = new Size(width, height);

            _backDc = new ImageDc(size.Width, size.Height);

            IntPtr pD = ToolTipImport.GetDesktopWindow();
            IntPtr pH = ToolTipImport.GetDC(pD);

            ToolTipImport.BitBlt(
                _backDc.Hdc,
                0, 0, size.Width, size.Height,
                pH, rect.Left, rect.Top, 0xCC0020);
            ToolTipImport.ReleaseDC(pD, pH);
        }

        private Icon GetIcon() {
            switch (base.ToolTipIcon) {
                case ToolTipIcon.Info:
                    return SystemIcons.Information;

                case ToolTipIcon.Warning:
                    return SystemIcons.Warning;
                case ToolTipIcon.Error:
                    return SystemIcons.Error;
                default:
                    return null;
            }
        }

        #endregion
        public static Region CreateImgRegion(Bitmap bitmap, int x, int y) {
            Color tancolor = bitmap.GetPixel(x, y);
            return CalculateRegion(bitmap, tancolor);
        }

        private unsafe static Region CalculateRegion(Bitmap bitmap, Color transColor) {
            GraphicsPath graphicsPath = new GraphicsPath();

            //uint colorTransparent = (uint)((transColor.A << 24) | (transColor.R << 16) | (transColor.G << 8) | (transColor.B));
            int colorTransparent = transColor.ToArgb();
            int colOpaquePixel = 0;
            int w = bitmap.Width;
            int h = bitmap.Height;

            System.Drawing.Imaging.BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte* lineStart = (byte*)bd.Scan0.ToPointer();

            for (int row = 0; row < h; row++) {
                colOpaquePixel = 0;
                int* currentPtr = (int*)lineStart;

                for (int col = 0; col < w; col++) {
                    if (*currentPtr != colorTransparent) {
                        colOpaquePixel = col;
                        int colNext = 0;
                        for (colNext = colOpaquePixel + 1; colNext < w; colNext++) {
                            currentPtr++;
                            if (*currentPtr == colorTransparent)
                                break;

                        }
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                        col = colNext;
                    }
                    else {
                        currentPtr++;
                    }
                }

                lineStart += bd.Stride;
            }
            bitmap.UnlockBits(bd);
            Region ret = new Region(graphicsPath);
            graphicsPath.Dispose();
            // Return calculated graphics path
            return ret;
        }

    }

    partial class ToolTipEx
    {

        public class ToolTipWidget
        {
            public ToolTipWidget(int left, int top, int width, int height) {
                this.Guid = System.Guid.NewGuid().ToString("N");
                this.m_Rec = new Rectangle(left, top, width, height);
                //this.Left = left;
                //this.Top = top;
                //this.Width = width;
                //this.Height = height;
            }
            private Rectangle m_Rec;
            /// <summary>
            /// 相对
            /// </summary>
            public int Left {
                get { return m_Rec.X; }
                set { m_Rec.X = value; }
            }
            /// <summary>
            /// 相对
            /// </summary>
            public int Top {
                get {
                    return m_Rec.Y;
                }
                set { m_Rec.Y = value; }
            }
            public int Width {
                get { return m_Rec.Width; }
                set { m_Rec.Width = value; }
            }
            public int Height {
                get { return m_Rec.Height; }
                set { m_Rec.Width = value; }
            }
            public System.Drawing.Image Image;
            public string Text;
            public string TextLinkUrl;
            public System.Drawing.Font TextFont = System.Drawing.SystemFonts.DefaultFont;
            public System.Drawing.Color TextColor = System.Drawing.SystemColors.ControlText;
            public object Tag;
            public bool Visible = true;
            public string Name;
            public int ZLayout;
            public string Guid {
                get;
                private set;
            }
            public ToolTipEx Owner { get; internal set; }
            public Rectangle Rect {
                get {
                    return m_Rec;
                }
            }
            /// <summary>
            /// 屏幕范围
            /// </summary>
            public Rectangle ClientRect {
                get {
                    Rectangle rec = new Rectangle(m_Rec.Location, m_Rec.Size);
                    if (Owner != null) {
                        rec.X = this.Left + this.Owner.Position.X;
                        rec.Y = this.Top + this.Owner.Position.Y;
                    }
                    return rec;
                }
            }
        }
        /// <summary>
        /// 屏幕图
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal class ImageDc : IDisposable
        {
            private int _height = 0;
            private int _width = 0;
            private IntPtr _pHdc = IntPtr.Zero;
            private IntPtr _pBmp = IntPtr.Zero;
            private IntPtr _pBmpOld = IntPtr.Zero;

            public ImageDc(int width, int height, IntPtr hBmp) {
                CreateImageDc(width, height, hBmp);
            }

            public ImageDc(int width, int height) {
                CreateImageDc(width, height, IntPtr.Zero);
            }

            public IntPtr Hdc {
                get { return _pHdc; }
            }

            public IntPtr HBmp {
                get { return _pBmp; }
            }

            private void CreateImageDc(int width, int height, IntPtr hBmp) {
                IntPtr pHdc = IntPtr.Zero;

                pHdc = ToolTipImport.CreateDCA("DISPLAY", "", "", 0);
                _pHdc = ToolTipImport.CreateCompatibleDC(pHdc);
                if (hBmp != IntPtr.Zero) {
                    _pBmp = hBmp;
                }
                else {
                    _pBmp = ToolTipImport.CreateCompatibleBitmap(pHdc, width, height);
                }
                _pBmpOld = ToolTipImport.SelectObject(_pHdc, _pBmp);
                if (_pBmpOld == IntPtr.Zero) {
                    ImageDestroy();
                }
                else {
                    _width = width;
                    _height = height;
                }
                ToolTipImport.DeleteDC(pHdc);
                pHdc = IntPtr.Zero;
            }

            private void ImageDestroy() {
                if (_pBmpOld != IntPtr.Zero) {
                    ToolTipImport.SelectObject(_pHdc, _pBmpOld);
                    _pBmpOld = IntPtr.Zero;
                }
                if (_pBmp != IntPtr.Zero) {
                    ToolTipImport.DeleteObject(_pBmp);
                    _pBmp = IntPtr.Zero;
                }
                if (_pHdc != IntPtr.Zero) {
                    ToolTipImport.DeleteDC(_pHdc);
                    _pHdc = IntPtr.Zero;
                }
            }

            public void Dispose() {
                ImageDestroy();
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// tooltip 颜色表
        /// </summary>
        public class ToolTipColorTable
        {
            private static readonly Color _base = Color.FromArgb(105, 200, 254);
            private static readonly Color _border = Color.FromArgb(204, 153, 51);
            private static readonly Color _backNormal = Color.FromArgb(250, 250, 250);
            private static readonly Color _backHover = Color.FromArgb(255, 180, 105);
            private static readonly Color _backPressed = Color.FromArgb(226, 176, 0);
            private static readonly Color _titleFore = Color.Brown;
            private static readonly Color _tipFore = Color.FromArgb(0, 0, 0);

            public ToolTipColorTable() { }

            public virtual Color Base {
                get { return _base; }
            }

            public virtual Color Border {
                get { return _border; }
            }

            public virtual Color BackNormal {
                get { return _backNormal; }
            }

            public virtual Color BackHover {
                get { return _backHover; }
            }

            public virtual Color BackPressed {
                get { return _backPressed; }
            }

            public virtual Color TitleFore {
                get { return _titleFore; }
            }

            public virtual Color TipFore {
                get { return _tipFore; }
            }
        }

        internal class ToolTipImport
        {
            private ToolTipImport() {
            }

            #region USER32.DLL
            [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowRgn")]
            public static extern int SetWindowRgn(System.IntPtr hWnd, System.IntPtr hRgn, bool bRedraw);
            [DllImport("user32")]
            public static extern int IsWindowVisible(IntPtr hWnd);
            /// <summary>
            /// 通过发送重绘消息 WM_PAINT 给目标窗体来更新目标窗体客户区的无效区域。
            /// </summary>
            [DllImport("user32")]
            public static extern bool UpdateWindow(IntPtr hWnd);
            [DllImport("user32.dll", EntryPoint = "RedrawWindow")]
            public static extern int RedrawWindow(IntPtr hwnd, ref RECT lprcUpdate, int hrgnUpdate, int fuRedraw);
            //[DllImport("user32.dll")]
            //public static extern IntPtr BeginPaint(
            //    IntPtr hWnd, ref PAINTSTRUCT ps);

            //[DllImport("user32.dll")]
            //public static extern bool EndPaint(
            //    IntPtr hWnd, ref PAINTSTRUCT ps);

            //[DllImport("user32.dll", SetLastError = true,
            //    CharSet = CharSet.Unicode, BestFitMapping = false)]
            //public static extern IntPtr CreateWindowEx(
            //    int exstyle,
            //    string lpClassName,
            //    string lpWindowName,
            //    int dwStyle,
            //    int x,
            //    int y,
            //    int nWidth,
            //    int nHeight,
            //    IntPtr hwndParent,
            //    IntPtr Menu,
            //    IntPtr hInstance,
            //    IntPtr lpParam);

            //[DllImport("user32.dll")]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool DestroyWindow(IntPtr hWnd);

            //[DllImport("user32.dll")]
            //public static extern IntPtr LoadIcon(
            //    IntPtr hInstance, int lpIconName);

            //[DllImport("user32.dll")]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(
                IntPtr hWnd,
                IntPtr hWndAfter,
                int x,
                int y,
                int cx,
                int cy,
                uint flags);

            //[DllImport("user32.dll")]
            //public static extern bool GetClientRect(
            //    IntPtr hWnd, ref RECT r);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(
                IntPtr hWnd, ref RECT lpRect);

            //[DllImport("user32.dll")]
            //public static extern int GetWindowLong(
            //    IntPtr hwnd, int nIndex);

            //[DllImport("user32.dll")]
            //public static extern int SetWindowLong(
            //    IntPtr hwnd, int nIndex, int dwNewLong);

            //[DllImport("user32.dll")]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool GetCursorPos(ref Point lpPoint);

            //[DllImport("user32.dll")]
            //public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr handle);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr handle, IntPtr hdc);

            [DllImport("user32.dll", SetLastError = false)]
            public static extern IntPtr GetDesktopWindow();

            //[DllImport("user32.dll")]
            //public static extern bool TrackMouseEvent(
            //    ref TRACKMOUSEEVENT lpEventTrack);

            //[DllImport("user32.dll")]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool PtInRect(ref RECT lprc, Point pt);

            //[DllImport("user32.dll", ExactSpelling = true)]
            //public static extern IntPtr SetTimer(
            //    IntPtr hWnd,
            //    int nIDEvent,
            //    uint uElapse,
            //    IntPtr lpTimerFunc);

            //[DllImport("user32.dll", ExactSpelling = true)]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool KillTimer(
            //    IntPtr hWnd, uint uIDEvent);

            //[DllImport("user32.dll")]
            //public static extern int SetFocus(IntPtr hWnd);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, int wParam, int lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, int wParam, ref TOOLINFO lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, int wParam, IntPtr lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, int wParam, ref RECT lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd,
            //    int msg,
            //    IntPtr wParam,
            //    [MarshalAs(UnmanagedType.LPTStr)]string lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, IntPtr wParam, ref NMHDR lParam);

            //[DllImport("user32.dll")]
            //public extern static int SendMessage(
            //    IntPtr hWnd, int msg, IntPtr wParam, int lParam);

            #endregion
            #region

            [DllImport("user32.dll")]
            public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
            public static uint LWA_COLORKEY = 0x00000001;
            public static uint LWA_ALPHA = 0x00000002;
            [DllImport("user32.dll")]
            public extern static uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
            [DllImport("user32.dll")]
            public extern static uint GetWindowLong(IntPtr hwnd, int nIndex);
            public enum WindowStyle : int
            {
                GWL_EXSTYLE = -20
            }
            public enum ExWindowStyle : uint
            {
                WS_EX_LAYERED = 0x00080000
            }
            #endregion

            #region GDI32.DLL

            [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
            public static extern bool AlphaBlend(
                IntPtr hdcDest,
                int nXOriginDest,
                int nYOriginDest,
                int nWidthDest,
                int nHeightDest,
                IntPtr hdcSrc,
                int nXOriginSrc,
                int nYOriginSrc,
                int nWidthSrc,
                int nHeightSrc,
                BLENDFUNCTION blendFunction);

            //[DllImport("gdi32.dll")]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool StretchBlt(
            //    IntPtr hDest,
            //    int X,
            //    int Y,
            //    int nWidth,
            //    int nHeight,
            //    IntPtr hdcSrc,
            //    int sX,
            //    int sY,
            //    int nWidthSrc,
            //    int nHeightSrc,
            //    int dwRop);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BitBlt(
                IntPtr hdc,
                int nXDest,
                int nYDest,
                int nWidth,
                int nHeight,
                IntPtr hdcSrc,
                int nXSrc,
                int nYSrc,
                int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDCA(
                [MarshalAs(UnmanagedType.LPStr)] string lpszDriver,
                [MarshalAs(UnmanagedType.LPStr)] string lpszDevice,
                [MarshalAs(UnmanagedType.LPStr)] string lpszOutput,
                int lpInitData);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDCW(
                [MarshalAs(UnmanagedType.LPWStr)] string lpszDriver,
                [MarshalAs(UnmanagedType.LPWStr)] string lpszDevice,
                [MarshalAs(UnmanagedType.LPWStr)] string lpszOutput,
                int lpInitData);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDC(
                string lpszDriver,
                string lpszDevice,
                string lpszOutput,
                int lpInitData);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(
                IntPtr hdc, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteDC(IntPtr hdc);

            [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true)]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject(IntPtr hObject);

            #endregion

            #region comctl32.dll

            [DllImport("comctl32.dll",
                CallingConvention = CallingConvention.StdCall)]
            public static extern bool InitCommonControlsEx(
                ref INITCOMMONCONTROLSEX iccex);

            #endregion

            #region kernel32.dll

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    ref NMHDR destination, IntPtr source, int length);

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    ref NMTTDISPINFO destination, IntPtr source, int length);

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    IntPtr destination, ref NMTTDISPINFO Source, int length);

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    ref POINT destination, ref RECT Source, int length);

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    ref NMTTCUSTOMDRAW destination, IntPtr Source, int length);

            //[DllImport("kernel32.dll")]
            //public extern static int RtlMoveMemory(
            //    ref NMCUSTOMDRAW destination, IntPtr Source, int length);

            #endregion


            #region Struct
            [StructLayout(LayoutKind.Sequential)]
            internal struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;

                public RECT(int left, int top, int right, int bottom) {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public RECT(Rectangle rect) {
                    Left = rect.Left;
                    Top = rect.Top;
                    Right = rect.Right;
                    Bottom = rect.Bottom;
                }

                public Rectangle Rect {
                    get {
                        return new Rectangle(
                            Left,
                            Top,
                            Right - Left,
                            Bottom - Top);
                    }
                }

                public Size Size {
                    get {
                        return new Size(Right - Left, Bottom - Top);
                    }
                }

                public static RECT FromXYWH(
                    int x, int y, int width, int height) {
                    return new RECT(x,
                                    y,
                                    x + width,
                                    y + height);
                }

                public static RECT FromRectangle(Rectangle rect) {
                    return new RECT(rect.Left,
                                     rect.Top,
                                     rect.Right,
                                     rect.Bottom);
                }
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct PAINTSTRUCT
            {
                public IntPtr hdc;
                public int fErase;
                public RECT rcPaint;
                public int fRestore;
                public int fIncUpdate;
                public int Reserved1;
                public int Reserved2;
                public int Reserved3;
                public int Reserved4;
                public int Reserved5;
                public int Reserved6;
                public int Reserved7;
                public int Reserved8;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct TOOLINFO
            {
                internal TOOLINFO(int flags) {
                    this.cbSize = Marshal.SizeOf(typeof(TOOLINFO));
                    this.uFlags = flags;
                    this.hwnd = IntPtr.Zero;
                    this.uId = IntPtr.Zero;
                    this.rect = new RECT(0, 0, 0, 0);
                    this.hinst = IntPtr.Zero;
                    this.lpszText = IntPtr.Zero;
                    this.lParam = IntPtr.Zero;
                }

                public int cbSize;
                public int uFlags;
                public IntPtr hwnd;
                public IntPtr uId;
                public RECT rect;
                public IntPtr hinst;
                public IntPtr lpszText;
                public IntPtr lParam;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct NMHDR
            {
                internal NMHDR(int flag) {
                    this.hwndFrom = IntPtr.Zero;
                    this.idFrom = 0;
                    this.code = 0;
                }

                internal IntPtr hwndFrom;
                internal int idFrom;
                internal int code;
            }
            //[StructLayout(LayoutKind.Sequential)]
            //internal struct TRACKMOUSEEVENT {
            //    internal uint cbSize;
            //    internal TRACKMOUSEEVENT_FLAGS dwFlags;
            //    internal IntPtr hwndTrack;
            //    internal uint dwHoverTime;
            //}
            [StructLayout(LayoutKind.Sequential)]
            internal struct BLENDFUNCTION
            {
                internal byte BlendOp;
                internal byte BlendFlags;
                internal byte SourceConstantAlpha;
                internal byte AlphaFormat;

                internal BLENDFUNCTION(
                    byte op, byte flags, byte alpha, byte format) {
                    BlendOp = op;
                    BlendFlags = flags;
                    SourceConstantAlpha = alpha;
                    AlphaFormat = format;
                }
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct NMTTDISPINFO
            {
                internal NMTTDISPINFO(int flags) {
                    this.hdr = new NMHDR(0);
                    this.lpszText = IntPtr.Zero;
                    this.szText = IntPtr.Zero;
                    this.hinst = IntPtr.Zero;
                    this.uFlags = 0;
                    this.lParam = IntPtr.Zero;
                }

                internal NMHDR hdr;
                internal IntPtr lpszText;
                internal IntPtr szText;
                internal IntPtr hinst;
                internal int uFlags;
                internal IntPtr lParam;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct INITCOMMONCONTROLSEX
            {
                internal INITCOMMONCONTROLSEX(int flags) {
                    this.dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
                    this.dwICC = flags;
                }

                internal int dwSize;
                internal int dwICC;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y) {
                    X = x;
                    Y = y;
                }
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct NMTTCUSTOMDRAW
            {
                internal NMCUSTOMDRAW nmcd;
                internal uint uDrawFlags;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct NMCUSTOMDRAW
            {
                internal NMHDR hdr;
                internal uint dwDrawStage;
                internal IntPtr hdc;
                internal RECT rc;
                internal IntPtr dwItemSpec;
                internal uint uItemState;
                internal IntPtr lItemlParam;
            }
            #endregion
        }
        /// <summary>
        /// 插值模式图形
        /// </summary>
        internal class InterpolationModeGraphics : IDisposable
        {
            private InterpolationMode _oldMode;
            private Graphics _graphics;

            public InterpolationModeGraphics(Graphics graphics)
                : this(graphics, InterpolationMode.HighQualityBicubic) {
            }

            public InterpolationModeGraphics(
                Graphics graphics, InterpolationMode newMode) {
                _graphics = graphics;
                _oldMode = graphics.InterpolationMode;
                graphics.InterpolationMode = newMode;
            }

            #region IDisposable 成员

            public void Dispose() {
                _graphics.InterpolationMode = _oldMode;
            }

            #endregion
        }
    }
}