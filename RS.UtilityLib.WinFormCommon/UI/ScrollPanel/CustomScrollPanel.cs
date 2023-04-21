using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.ScrollPanel
{
    public class CustomScrollPanel : UserControl
    {
        public CustomScrollPanel()
            : base() {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        #region API
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int iBar, int bShow);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        //
        public enum SB
        {
            /// <summary>
            /// 水平滚动条。
            /// </summary>
            SB_HORZ = 0,
            /// <summary>
            /// 垂直滚动条。
            /// </summary>
            SB_VERT = 1,
        }

        private const int GWL_STYLE = -16;

        private const int WS_HSCROLL = 0x00100000;
        private const int WS_VSCROLL = 0x00200000;

        protected override void WndProc(ref Message m) {
            HideScrollBar(ref m);
            base.WndProc(ref m);
        }
        private void HideScrollBar(ref Message m) {
            int dwStyle = GetWindowLong(this.Handle, GWL_STYLE);
            if ((dwStyle & WS_HSCROLL) == WS_HSCROLL) {
                ShowScrollBar(this.Handle, (int)SB.SB_HORZ, 0);
            }
            if ((dwStyle & WS_VSCROLL) == WS_VSCROLL) {
                ShowScrollBar(this.Handle, (int)SB.SB_VERT, 0);
            }
        }

        #endregion
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //public struct SCROLLINFO {
        //    public uint cbSize;
        //    public uint fMask;
        //    public int nMin;
        //    public int nMax;
        //    public uint nPage;
        //    public int nPos;
        //    public int nTrackPos;
        //}
        //public enum ScrollBarInfoFlags {
        //    SIF_RANGE = 0x0001,
        //    SIF_PAGE = 0x0002,
        //    SIF_POS = 0x0004,
        //    SIF_DISABLENOSCROLL = 0x0008,
        //    SIF_TRACKPOS = 0x0010,
        //    SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
        //}
        //public enum ScrollBarRequests {
        //    SB_LINEUP = 0,
        //    SB_LINELEFT = 0,
        //    SB_LINEDOWN = 1,
        //    SB_LINERIGHT = 1,
        //    SB_PAGEUP = 2,
        //    SB_PAGELEFT = 2,
        //    SB_PAGEDOWN = 3,
        //    SB_PAGERIGHT = 3,
        //    SB_THUMBPOSITION = 4,
        //    SB_THUMBTRACK = 5,
        //    SB_TOP = 6,
        //    SB_LEFT = 6,
        //    SB_BOTTOM = 7,
        //    SB_RIGHT = 7,
        //    SB_ENDSCROLL = 8
        //}
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int GetScrollInfo(IntPtr hwnd, int bar, ref SCROLLINFO si);
        //[DllImport("user32")]
        //public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool Rush);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        ///// <summary>
        ///// 获取滚动条数据
        ///// </summary>
        ///// <param name="MyControl"></param>
        ///// <param name="ScrollSize"></param>
        ///// <returns></returns>
        //private static Point GetScrollPoint(Control MyControl) {
        //    Point MaxScroll = new Point();
        //    SCROLLINFO ScrollInfo = new SCROLLINFO();
        //    ScrollInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(ScrollInfo);
        //    ScrollInfo.fMask = (uint)ScrollBarInfoFlags.SIF_ALL;
        //    GetScrollInfo(MyControl.Handle, 1, ref ScrollInfo);
        //    MaxScroll.Y = ScrollInfo.nMax - (int)ScrollInfo.nPage;
        //    if ((int)ScrollInfo.nPage == 0) MaxScroll.Y = 0;
        //    GetScrollInfo(MyControl.Handle, 0, ref ScrollInfo);
        //    MaxScroll.X = ScrollInfo.nMax - (int)ScrollInfo.nPage;
        //    if ((int)ScrollInfo.nPage == 0) MaxScroll.X = 0;
        //    return MaxScroll;
        //}
        ///// <summary>
        ///// 移动控件滚动条位置
        ///// </summary>
        ///// <param name="Bar"></param>
        ///// <param name="Point"></param>
        ///// <param name="MyControl"></param>
        //public static void MoveBar(int Bar, int Point, Control MyControl) {
        //    if (Bar == 0) {
        //        SetScrollPos(MyControl.Handle, 0, Point, true);
        //        SendMessage(MyControl.Handle, (int)0x0114, (int)ScrollBarRequests.SB_THUMBPOSITION, 0);
        //    }
        //    else {
        //        SetScrollPos(MyControl.Handle, 1, Point, true);
        //        SendMessage(MyControl.Handle, (int)0x0115, (int)ScrollBarRequests.SB_THUMBPOSITION, 0);
        //    }
        //}
        //public void MovaBar(int Bar, int Point) {
        //    MoveBar(Bar, Point, this);
        //}
        //public static Bitmap GetPanel(Panel p_Panel) {
        //    bool _PanelAotu = p_Panel.AutoScroll;
        //    Size _PanelSize = p_Panel.Size;
        //    p_Panel.Visible = false;
        //    p_Panel.AutoScroll = true;
        //    MoveBar(0, 0, p_Panel);
        //    MoveBar(1, 0, p_Panel);
        //    Point _Point = GetScrollPoint(p_Panel);
        //    p_Panel.Width += _Point.X + 5;
        //    p_Panel.Height += _Point.Y + 5;
        //    Bitmap _BitMap = new Bitmap(p_Panel.Width, p_Panel.Height);
        //    p_Panel.DrawToBitmap(_BitMap, new Rectangle(0, 0, _BitMap.Width, _BitMap.Height));
        //    p_Panel.AutoScroll = _PanelAotu;
        //    p_Panel.Size = _PanelSize;
        //    p_Panel.Visible = true;

        //    return _BitMap;
        //}
        protected override void OnMouseWheel(MouseEventArgs e) {
            //base.OnMouseWheel(e);            
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            //base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            //base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            //base.OnMouseDown(e);
        }
        protected override void OnGotFocus(EventArgs e) {
            //base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e) {
            //base.OnLostFocus(e);
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // CustomScrollPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "CustomScrollPanel";
            this.ResumeLayout(false);

        }
    }
}
