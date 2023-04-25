using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
{

    /// <summary>
    /// 控件上 炫一下（多张图片在控件上闪烁） 
    /// </summary> 
    public class ShineControlHelper
    {
        static ShineWnd m_Form;
        static int m_Seconds;
        static int m_Current;
        static System.Windows.Forms.Timer m_Timer;
        public static void GetCtlCenterPos(Control ctl, out int x, out int y) {
            Point p = Point.Empty;//ctl.Location;
            p = ctl.PointToScreen(p);
            p.Offset((int)(ctl.Width * 0.5), (int)(ctl.Height * 0.5));
            //
            x = p.X;
            y = p.Y;
        }
        /*
        internal static void Show(string ctlName, int secs) {
            Show(ctlName, secs, 0, 0);
        }
        internal static void Show(string ctlName, string imgFile, int secs) {
            m_Form.SetImage(imgFile);
            Show(ctlName, secs, 0, 0);
        }
        internal static void Show(string ctlName, string imgFile, int secs, int w, int h) {
            m_Form.SetImage(imgFile);
            Show(ctlName, secs, w, h);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctlName"></param>
        /// <param name="w">炫窗口宽</param>
        /// <param name="h">炫窗口高</param>
        internal static void Show(string ctlName, int secs, int w, int h) {
            int x, y;
            if (UIMgr.GetCtlCenterPos(ctlName, out x, out y) == false) {
                return;
            }
            if (m_Form.IsDisposed) {
                InitShineWnd();
            }
            object o = UIMgr.Find(ctlName);

            m_Current = 0;
            if (secs < 1) {
                secs = 1;
            }
            m_Seconds = secs * 1000;
            m_Timer.Start();

            Form owner;
            if (o is Control) {
                owner = (o as Control).FindForm();
                if (w == 0 || h == 0) {
                    m_Form.Size = (o as Control).Size;
                }
                else {
                    m_Form.Size = new Size(w, h);
                }
            }
            else {
                owner = (o as WndlessCtl).ContainControl.FindForm();
                if (w == 0 || h == 0) {
                    m_Form.Size = (o as WndlessCtl).Rect.Size;
                }
                else {
                    m_Form.Size = new Size(w, h);
                }
            }
            if (owner != null) {
                m_Form.Owner = owner;
                m_Form.TopMost = false;
                m_Form.TopLevel = true;
            }
            else {
                m_Form.Owner = null;
                m_Form.TopMost = true;
            }
            m_Form.Location = new System.Drawing.Point(x - (int)(m_Form.Width * 0.5), y - (int)(m_Form.Height * 0.5));
            Debug.WriteLine(m_Form.Owner.Name);
            Debug.WriteLine(m_Form.Location);
            m_Form.Show();

        }
        */
        public static void Show(Control ctl, int secs) {
            int x, y; 
            GetCtlCenterPos(ctl, out x, out y);
            Show(ctl, secs, x, y, 0, 0);
        }
        public static void Show(Control ctl, int secs, int x, int y, int w, int h) {
            //初始化
            if (m_Form.IsDisposed) {
                InitShineWnd();
            }
            m_Current = 0;
            if (secs < 1) {
                secs = 1;
            }
            m_Seconds = secs * 1000;
            m_Timer.Start();

            Form owner = ctl.FindForm();
            if (w == 0 || h == 0) {
                m_Form.Size = ctl.Size;
            }
            else {
                m_Form.Size = new Size(w, h);
            }
            if (owner != null) {
                m_Form.Owner = owner;
                m_Form.TopMost = false;
                m_Form.TopLevel = true;
            }
            else {
                m_Form.Owner = null;
                m_Form.TopMost = true;
            }
            m_Form.Location = new System.Drawing.Point(x - (int)(m_Form.Width * 0.5), y - (int)(m_Form.Height * 0.5));
            Debug.WriteLine(m_Form.Owner.Name);
            Debug.WriteLine(m_Form.Location);
            m_Form.Show();

        }
        /// <summary>
        /// 设置炫动的窗口尺寸
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public static void SetShineSize(int w, int h) {
            m_Form.Size = new Size(w, h);
        }
        public static void Hide() {
            m_Form.Owner = null;
            m_Form.Hide();
        }
        static ShineControlHelper() {
            InitShineWnd();
            m_Timer = new System.Windows.Forms.Timer();
            m_Timer.Interval = 50;
            m_Timer.Tick += new EventHandler(t_Tick);
        }

        private static void InitShineWnd() {
            m_Form = new ShineWnd();
            m_Form.SetImage(RS.UtilityLib.WinFormCommon.Properties.Resources.shine);
            //m_Form.SetImage("skin\\shine.png");
            m_Form.SetParam(10, 64, 64);
            m_Form.SetInternal(50);
            m_Form.StartPosition = FormStartPosition.Manual;
        }

        static void t_Tick(object sender, EventArgs e) {
            m_Current += 50;
            //m_Form.Refresh();
            if (m_Current >= m_Seconds) {
                Hide();
                m_Timer.Stop();
            }
        }
    }

    internal static class Win32
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

    internal class ShineWnd : Form
    {
        protected Image m_Image;
        bool haveHandle = false;
        Timer timerSpeed = new Timer();
        protected int frameCount = 20;
        int frame = 0;
        int frameWidth = 200;
        int frameHeight = 200;

        public ShineWnd() {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.TopLevel = true;
            //
            this.Size = new Size(64, 32);
            timerSpeed.Interval = 50;
            timerSpeed.Enabled = true;
            timerSpeed.Tick += new EventHandler(timerSpeed_Tick);

            this.MouseEnter += new EventHandler(ShineWnd_MouseEnter);
        }
        public void SetInternal(int interval) {
            if (timerSpeed != null) {
                timerSpeed.Interval = interval;
            }
        }
        public void SetParam(int framecount, int frameW, int frameH) {
            frameCount = framecount;
            frameWidth = frameW;
            frameHeight = frameH;
        }
        public virtual void SetImage(Image img) {
            if (m_Image != null) {
                m_Image.Dispose();
            }
            m_Image = new Bitmap(img);
        }
        public virtual void SetImage(string imgFile) {
            if (m_Image != null) {
                m_Image.Dispose();
            }
            using (Stream stream = UIRes.GetResourceStream(imgFile)) {
                m_Image = new Bitmap(stream);
            }
        }
        void ShineWnd_MouseEnter(object sender, EventArgs e) {

        }

        #region Override
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            timerSpeed.Enabled = false;
            this.Hide();
        }
        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);
            timerSpeed.Enabled = this.Visible;
        }
        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            base.OnClosing(e);
            haveHandle = false;
        }
        protected override void Dispose(bool disposing) {
            haveHandle = false;
            if (timerSpeed != null) {
                timerSpeed.Dispose();
            }
            if (m_Image != null) {
                m_Image.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnHandleCreated(EventArgs e) {
            InitializeStyles();
            base.OnHandleCreated(e);
            haveHandle = true;
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }

        #endregion

        private void InitializeStyles() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private void timerSpeed_Tick(object sender, EventArgs e) {

            frame++;
            if (frame >= frameCount) frame = 0;
            SetBits(FrameImage);
        }

        private Bitmap FrameImage {
            get {
                Bitmap bitmap = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.DrawImage(m_Image,
                    new Rectangle(0, 0, this.Width, this.Height),
                    new Rectangle(frameWidth * frame, 0, frameWidth, frameHeight),
                    GraphicsUnit.Pixel);

                return bitmap;
            }
        }

        private void SetBits(Bitmap bitmap) {
            if (!haveHandle) return;

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                throw new ApplicationException("The picture must be 32bit picture with alpha channel.");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try {
                Win32.Point topLoc = new Win32.Point(Left, Top);
                Win32.Size bitMapSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
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

    }
}
