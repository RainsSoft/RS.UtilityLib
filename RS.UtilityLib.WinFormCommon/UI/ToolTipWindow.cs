using System;
using System.Drawing;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    //
    /// <summary>
    /// 可移动窗口
    /// </summary>
    public class ToolTipWindow : ContextMenuForm
    {

        public ToolTipWindow()
            : base() {
            this.TopLevel = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            m_LockSize = this.Size;

        }
        /// <summary>
        /// 设置移动窗体的控件
        /// </summary>
        /// <param name="moveformctl"></param>
        public void BingMoveEvent(Control moveformctl) {
            if (moveformctl == null) return;
            UnBingMoveEvent(moveformctl);
            moveformctl.MouseDown += gly_Caption_MouseDown;
            moveformctl.MouseUp += gly_Caption_MouseUp;
            moveformctl.MouseMove += gly_Caption_MouseMove;
            moveformctl.MouseLeave += gly_Caption_MouseLeave;
        }
        public void UnBingMoveEvent(Control moveformctl) {
            moveformctl.MouseDown -= gly_Caption_MouseDown;
            moveformctl.MouseUp -= gly_Caption_MouseUp;
            moveformctl.MouseMove -= gly_Caption_MouseMove;
            moveformctl.MouseLeave -= gly_Caption_MouseLeave;
        }
        /// <summary>
        /// 开始位置(父容器相对位置)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="startLocation"></param>
        public void Show(Control parent, Point startLocation) {
            _parentControl = parent;
            Point location = parent.PointToScreen(startLocation);
            this.Location = location;
            this.Show();
        }
        public void Show(Control parent, Point startLocation, int width, int height) {
            Show(parent, startLocation);
            this.Width = width;
            this.Height = height;
        }
        public void Show(Control parent, Point startLocation, int width, int height, Size lockSize, bool islock) {
            this.m_LockSize = lockSize;
            this.m_IsLockSize = islock;
            this.Width = width;
            this.Height = height;
            if (islock) {
                this.Width = lockSize.Width;
                this.Height = lockSize.Height;
            }
            Show(parent, startLocation);
        }
        #region 位置移动
        protected Size m_LockSize;
        /// <summary>
        /// 是否锁定大小
        /// </summary>
        public Size LockSize { get { return m_LockSize; } }
        protected bool m_IsLockSize;
        public bool IsLockSize { get { return m_IsLockSize; } }
        protected bool m_CanSize;
        /// <summary>
        /// 是否起用拉伸窗口模式
        /// </summary>
        public bool CanSize { get { return m_CanSize; } }
        protected int MinWidth = 1;
        protected int MinHeight = 1;

        protected const int WM_NCHITTEST = 0x0084;
        protected const int WM_SIZING = 532;

        private int HTCLIENT = 1;
        private int HTCAPTION = 2;
        private int HTLEFT = 10;
        private int HTRIGHT = 11;
        private int HTTOP = 12;
        private int HTTOPLEFT = 13;
        private int HTTOPRIGHT = 14;
        private int HTBOTTOM = 15;
        private int HTBOTTOMLEFT = 10;
        private int HTBOTTOMRIGHT = 17;


        private bool m_OnMove = false;
        private Point m_MoveDelta = Point.Empty;
        protected Size m_oldSize = Size.Empty;

        private IntPtr m_Captured = IntPtr.Zero;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCapture")]
        public static extern System.IntPtr SetCapture([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);
        protected unsafe override void WndProc(ref Message m) {
            switch (m.Msg) {
                case 532: //尺寸变化
                    int* rect = (int*)m.LParam;
                    int left = *rect;
                    int top = *(rect + 1);
                    int right = *(rect + 2);
                    int bottom = *(rect + 3);
                    if (!IsLockSize) {
                        if (right - left < this.MinWidth) {
                            *(rect + 2) = left + this.MinWidth;
                        }
                        if (bottom - top < this.MinHeight) {
                            *(rect + 3) = top + this.MinHeight;
                        }
                    }
                    else {
                        if (right - left < this.LockSize.Width) {
                            *(rect + 2) = left + this.LockSize.Width;
                        }
                        if (bottom - top < this.LockSize.Height) {
                            *(rect + 3) = top + this.LockSize.Height;
                        }
                    }
                    base.WndProc(ref m);
                    break;
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if (!CanSize) return;
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);

                    if (vPoint.X <= 5) {
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else m.Result = (IntPtr)HTLEFT;
                    }
                    else if (vPoint.X >= ClientSize.Width - 5) {
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)HTRIGHT;
                    }
                    else if (vPoint.Y <= 5) {
                        m.Result = (IntPtr)HTTOP;
                    }
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOM;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected void gly_Caption_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                m_OnMove = true;
                m_MoveDelta = e.Location;
                //this.m_Captured=SetCapture(this.Handle);

            }
        }

        protected void gly_Caption_MouseUp(object sender, MouseEventArgs e) {
            m_OnMove = false;
        }

        protected void gly_Caption_MouseMove(object sender, MouseEventArgs e) {
            //System.Diagnostics.Debug.WriteLine(e.Button);
            if (m_OnMove && e.Button == MouseButtons.Left) {
                //Point p = gly_Caption.PointToScreen(new Point(e.X, e.Y));
                this.Left += (e.X - m_MoveDelta.X);
                this.Top += (e.Y - m_MoveDelta.Y);
                // m_MoveDelta = new Point(e.X, e.Y);
                // System.Diagnostics.Debug.WriteLine(p.ToString());
            }
        }

        protected void gly_Caption_MouseLeave(object sender, EventArgs e) {
            m_OnMove = false;
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.F4 | Keys.Alt:
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
