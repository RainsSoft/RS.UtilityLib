using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;

namespace RS.UtilityLib.WinFormCommon.UINotifier
{
    public partial class UIStartForm : Form
    {
        static UIStartForm m_frm;
        public static void ShowForm() {
            m_frm = new UIStartForm();
            m_frm.Show();
        }
        public static void CloseForm() {
            if (m_frm != null) {
                //m_frm.Close(); 
                m_frm.SafeAction(() => {
                    m_frm.Close();
                });
                m_frm = null;

            }
        }
#if DEBUG
        static void test() {
            UIStartForm splashForm=null;
            //show splash
            Thread splashThread = new Thread(new ThreadStart(
                delegate {
                    splashForm= new UIStartForm();
                    Application.Run(splashForm);
                }
                ));

            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            //run form - time taking operation
            Form mainForm = new Form();
            mainForm.Load += (sender,e)=> {
                //close splash
                if (splashForm == null) {
                    return;
                }
                splashForm.Invoke(new Action(splashForm.Close));
                splashForm.Dispose();
                splashForm = null;
            };
        }
#endif
        public UIStartForm() {

            InitializeComponent();
            this.TopMost = true;
            //UseRegion();
            this.ShowInTaskbar = false;
            this.Visible = false;


        }
        #region
        private bool haveHandle = false;
        private void UseRegion() {
            Region loginRegion;

            loginRegion = BitmapRegion.CreateImgRegion((Bitmap)this.BackgroundImage, 0, 0);
            byte[] buf = loginRegion.GetRegionData().Data;
            loginRegion.GetRegionData().Data = buf;

            this.Size = this.BackgroundImage.Size;
            this.Region = loginRegion;
        }

        protected override void OnHandleCreated(EventArgs e) {
            InitializeStyles();
            base.OnHandleCreated(e);
            haveHandle = true;

            //
            SetBits("正在加载...");

        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }

        private void InitializeStyles() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private void SetBits(string msg) {
            Bitmap bmp = Properties.Resources.loading_txt;
            if (string.IsNullOrEmpty(msg) == false) {
                using (Graphics g = Graphics.FromImage(bmp)) {
                    using (Font f = new Font(SystemFonts.DefaultFont.FontFamily, 12)) {
                        using (Brush b = new SolidBrush(Color.FromArgb(150, 150, 150))) {
                            g.FillRectangle(b, new Rectangle(100, 5, 100, 14));
                            g.DrawString(msg, f, SystemBrushes.ControlText, new PointF(100f, 6f));
                        }
                    }
                }
            }
            if (!haveHandle)
                return;

            if (!Bitmap.IsCanonicalPixelFormat(bmp.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bmp.PixelFormat))
                throw new ApplicationException("The picture must be 32bit picture with alpha channel.");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try {
                Win32.Point topLoc = new Win32.Point(Left, Top);
                Win32.Size bitMapSize = new Win32.Size(bmp.Width, bmp.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBits = Win32.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
            }
            finally {
                if (hBitmap != IntPtr.Zero) {
                    Win32.SelectObject(memDc, oldBits);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.ReleaseDC(IntPtr.Zero, screenDC);
                Win32.DeleteDC(memDc);
            }
        }
        internal static class BitmapRegion
        {
            /// <summary>
            /// 创建支持位图区域的控件（目前有button和form）
            /// </summary>
            /// <param name="control"></param>
            /// <param name="bitmap"></param>
            internal static void CreateControlRegion(Control control, Bitmap bitmap) {
                if (control == null || bitmap == null)
                    return;
                control.Width = bitmap.Width;
                control.Height = bitmap.Height;
                if (control is System.Windows.Forms.Form) {
                    // Cast to a Form object
                    Form form = (Form)control;
                    form.Width = control.Width;
                    form.Height = control.Height;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.BackgroundImage = bitmap;
                    form.Region = CalculateRegion(bitmap, bitmap.GetPixel(1, 1));
                }
                else if (control is System.Windows.Forms.Button) {
                    Button button = (Button)control;
                    button.Text = "";
                    button.Cursor = Cursors.Hand;
                    button.BackgroundImage = bitmap;
                    button.Region = CalculateRegion(bitmap, bitmap.GetPixel(1, 1));

                }
            }
            internal static Region CreateImgRegion(Bitmap bitmap, int x, int y) {
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
        #endregion
        protected override void OnShown(EventArgs e) {

            base.OnShown(e);
            //         


            //     SetBits("正在加载...");

        }
        #region designer
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // UIStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(512, 128);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UIStartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion
        #endregion

        #region win32
        internal class Win32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Size
            {
                public Int32 cx;
                public Int32 cy;

                public Size(Int32 x, Int32 y) {
                    cx = x;
                    cy = y;
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct BLENDFUNCTION
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public Int32 x;
                public Int32 y;

                public Point(Int32 x, Int32 y) {
                    this.x = x;
                    this.y = y;
                }
            }

            public const byte AC_SRC_OVER = 0;
            public const Int32 ULW_ALPHA = 2;
            public const byte AC_SRC_ALPHA = 1;

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("gdi32.dll", ExactSpelling = true)]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

            [DllImport("user32.dll", ExactSpelling = true)]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern int DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern int DeleteObject(IntPtr hObj);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr ExtCreateRegion(IntPtr lpXform, uint nCount, IntPtr rgnData);
        }
        #endregion
    }

}
