using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    ///支持变换图片
    /// </summary>
    public class PictureBoxEx : System.Windows.Forms.PictureBox
    {
        public PictureBoxEx() : base() {
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.FromArgb(100, 100, 100, 100);
            this.MinimumSize = new Size(2, 2);
        }
        #region Designer      
        private bool IsDesignerHosted {
            get {
                if (DesignMode)
                    return DesignMode;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
        #endregion
        //[Description("兄弟控件相交部分半透明支持")]
        //public bool ImgTranslucentBrotherSupport {
        //    get;
        //    set;
        //} = true;
        public Image ImgNormal {
            get;
            set;
        }
        public Image ImgHover {
            get;
            set;
        }
        public Image ImgDown {
            get;
            set;
        }
        private Image m_ImgRegion;
        public Image ImgRegion {
            get {
                return m_ImgRegion;
            }
            set {
                m_ImgRegion = value;
                setRegion();
            }
        }
        //private float m_ImgRegionScale = 1f;
        //public float ImgRegionScale {
        //    get {
        //        return m_ImgRegionScale;
        //    }
        //    set {
        //        var v = Math.Max(value, 0.01f);
        //        m_ImgRegionScale = Math.Min(v,1f);
        //        setRegion();
        //    }
        //}

        private void setRegion() {
            if (this.m_ImgRegion != null) {
                this.Region = Bitmap2Region((Bitmap)this.m_ImgRegion);
            }
            else {
                this.Region = null;
            }
        }
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            //
            setRegion();
        }
        private bool m_IsMouseHover;
        private bool m_IsMouseDown;
        protected override void OnMouseHover(EventArgs e) {
            base.OnMouseHover(e);
            m_IsMouseHover = true;
            updateDisplayImg();
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            m_IsMouseHover = true;
            updateDisplayImg();
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_IsMouseHover = false;
            updateDisplayImg();
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            m_IsMouseDown = true;
            updateDisplayImg();
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            m_IsMouseDown = false;
            updateDisplayImg();
        }
        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            updateDisplayImg();
        }
        private void updateDisplayImg() {
            if (this.Enabled == false) {
                if (this.ImgNormal != null) {
                    this.Image = this.ImgNormal;
                }
            }
            else {
                if (this.m_IsMouseDown) {
                    if (this.ImgDown != null) {
                        this.Image = this.ImgDown;
                    }

                }
                else if (this.m_IsMouseHover) {
                    if (this.ImgHover != null) {
                        this.Image = this.ImgHover;
                    }
                }
                else {
                    if (this.ImgNormal != null) {
                        this.Image = this.ImgNormal;
                    }
                }
            }
        }
        //
        //protected override void OnPaint(PaintEventArgs pe) {
        //    base.OnPaint(pe);
        //    if(this.ImgTranslucentBrotherSupport&&
        //        this.Parent!=null) {
        //        //重叠控件最上层使用paint把和他相交的所有下层控件的相交部分全部重汇一遍，达到透明效果。
        //        int index = this.Parent.Controls.GetChildIndex(this);
        //        //Z排序必须和添加顺序一致
        //        var g = pe.Graphics;
        //        for(int i = this.Parent.Controls.Count - 1; i > index; i--) {
        //            var c = this.Parent.Controls[i];
        //            if(c.Bounds.IntersectsWith(this.Bounds) && c.Visible) {
        //                using(Bitmap bmp = new Bitmap(c.Width, c.Height)) {
        //                    c.DrawToBitmap(bmp, c.ClientRectangle);
        //                    g.TranslateTransform(c.Left - this.Left, c.Top - this.Top);
        //                    g.DrawImageUnscaled(bmp, Point.Empty);
        //                    g.TranslateTransform(this.Left - c.Left, this.Top - c.Top);
        //                }
        //            }
        //        }
        //    }
        //}
        private Region Bitmap2Region(Bitmap bitmapSource) {
            try {
                //float scaleW = this.Width / (float)bitmapSource.Width;
                //float scaleH = this.Height / (float)bitmapSource.Height;
                //int width = (int)(bitmapSource.Width * scaleW);
                //int height = (int)(bitmapSource.Height * scaleH);
                //if(width < 2) {
                //    width = 2;
                //}
                //if(height < 2) {
                //    height = 2;
                //}
                int width = this.Width;
                int height = this.Height;
                var transparencyColor = bitmapSource.GetPixel(0, 0);
                using (Bitmap bitmap = new Bitmap(width, height)) {

                    using (Graphics g = Graphics.FromImage(bitmap)) {
                        g.DrawImage(bitmapSource, new Rectangle(0, 0, width, height));
                    }
                    //var bitmap = bitmapSource;
                    ////创建region
                    //BitmapData bd = bitmap.LockBits(
                    //    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    //    ImageLockMode.ReadOnly,
                    //    PixelFormat.Format32bppArgb);
                    //Region region = new Region(Rectangle.Empty);

                    //unsafe
                    //{
                    //    uint* pixel = (uint*)bd.Scan0.ToInt32();

                    //    for(int y = 0; y < height; y++) {
                    //        for(int x = 0; x < width; x++) {
                    //            if((*pixel & 0xFF000000) > 0) {
                    //                region.Union(new Rectangle(x, y, 1, 1));
                    //            }

                    //            pixel++;
                    //        }
                    //    }
                    //}

                    //bitmap.UnlockBits(bd);
                    //bitmap.Save("test.png");
                    //return region;
                    return BitmapToRegion.Convert(bitmap, transparencyColor, BitmapToRegion.TransparencyMode.ColorKeyTransparent);
                }
            }
            catch {
                Debug.WriteLine("Bitmap2Region error:");
            }
            return null;
        }
        /*
        /// <summary>
        /// determines the meaning of the transparencyKey argument to the Convert method
        /// </summary>
        enum TransparencyMode
        {
            /// <summary>
            /// the color key is used to define the transparent region of the bitmap
            /// </summary>
            ColorKeyTransparent,
            /// <summary>
            /// the color key is used to define the area that should _not_ be transparent
            /// </summary>
            ColorKeyOpaque
        }

        /// <summary>
        /// a class to convert a color-keyed bitmap into a region
        /// </summary>
        class BitmapToRegion
        {
            /// <summary>
            /// ctor made private to avoid instantiation
            /// </summary>
            private BitmapToRegion() {
            }
            //private static bool colorsMatch(Color color1, Color color2, int tolerance) {
            //    if(tolerance < 0)
            //        tolerance = 0;
            //    return Math.Abs(color1.R - color2.R) <= tolerance &&
            //           Math.Abs(color1.G - color2.G) <= tolerance &&
            //           Math.Abs(color1.B - color2.B) <= tolerance;
            //}

            private unsafe static bool colorsMatch(uint* pixelPtr, Color color1, int tolerance) {
                if (tolerance < 0)
                    tolerance = 0;
                byte a = (byte)(*pixelPtr >> 24);
                byte r = (byte)(*pixelPtr >> 16);
                byte g = (byte)(*pixelPtr >> 8);
                byte b = (byte)(*pixelPtr >> 0);
                //Color pointer = Color.FromArgb(a, r, g, b);
                //return Math.Abs(color1.A - pointer.A) <= tolerance &&
                //       Math.Abs(color1.R - pointer.R) <= tolerance &&
                //       Math.Abs(color1.G - pointer.G) <= tolerance &&
                //       Math.Abs(color1.B - pointer.B) <= tolerance;
                return Math.Abs(color1.A - a) <= tolerance &&
                       Math.Abs(color1.R - r) <= tolerance &&
                       Math.Abs(color1.G - g) <= tolerance &&
                       Math.Abs(color1.B - b) <= tolerance;
            }

            /// <summary>
            /// the meat of this class
            /// converts the bitmap to a region by scanning each line one by one
            /// this method will not affect the original bitmap in any way
            /// </summary>
            /// <param name="bitmap">The bitmap to convert</param>
            /// <param name="transparencyKey">The color which will indicate either transparency or opacity</param>
            /// <param name="mode">Whether the transparency key should indicate the transparent or the opaque region</param>
            public unsafe static Region Convert(Bitmap bitmap, Color transparencyKey,
                TransparencyMode mode, int tolerance) {
                //sanity check
                if (bitmap == null)
                    throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

                //flag = true means the color key represents the opaque color
                bool modeFlag = (mode == TransparencyMode.ColorKeyOpaque);

                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF boundsF = bitmap.GetBounds(ref unit);
                Rectangle bounds = new Rectangle((int)boundsF.Left, (int)boundsF.Top,
                    (int)boundsF.Width, (int)boundsF.Height);

                uint key = (uint)((transparencyKey.A << 24) | (transparencyKey.R << 16) |
                    (transparencyKey.G << 8) | (transparencyKey.B << 0));


                //get access to the raw bits of the image
                BitmapData bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);
                uint* pixelPtr = (uint*)bitmapData.Scan0.ToPointer();

                //avoid property accessors in the for
                int yMax = (int)boundsF.Height;
                int xMax = (int)boundsF.Width;

                //to store all the little rectangles in
                GraphicsPath path = new GraphicsPath();

                for (int y = 0; y < yMax; y++) {
                    //store the pointer so we can offset the stride directly from it later
                    //to get to the next line
                    byte* basePos = (byte*)pixelPtr;

                    for (int x = 0; x < xMax; x++, pixelPtr++) {
                        //is this transparent? if yes, just go on with the loop
                        //if(modeFlag ^ (*pixelPtr == key))
                        //    continue;
                        if (modeFlag ^ colorsMatch(pixelPtr, transparencyKey, tolerance))
                            continue;
                        //store where the scan starts
                        int x0 = x;

                        //not transparent - scan until we find the next transparent byte
                        //while(x < xMax && !(modeFlag ^ (*pixelPtr == key))) {
                        while (x < xMax && !(modeFlag ^ (colorsMatch(pixelPtr, transparencyKey, tolerance)))) {
                            ++x;
                            pixelPtr++;
                        }

                        //add the rectangle we have found to the path
                        path.AddRectangle(new Rectangle(x0, y, x - x0, 1));
                    }
                    //jump to the next line
                    pixelPtr = (uint*)(basePos + bitmapData.Stride);
                }

                //now create the region from all the rectangles
                Region region = new Region(path);

                //clean up
                path.Dispose();
                bitmap.UnlockBits(bitmapData);

                return region;
            }

        }
        */
    }
}
