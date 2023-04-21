using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public class RibbonTreeView : TreeView
    {
        private const int TV_FIRST = 0x1100;
        private const int TVM_EXPAND = TV_FIRST + 2;
        private const int TVM_INSERTITEMW = TV_FIRST + 50;
        private const int TVM_INSERTITEMA = TV_FIRST + 0;
        private const int TVM_DELITEM = TV_FIRST + 1;
        private const int TVM_GETITEMHEIGHT = TV_FIRST + 28;
        private int testCount = 0;
        protected override void WndProc(ref Message m) {


            //当BeginUpdate..EndUpdate时,此消息不会触发.
            if (m.Msg == TVM_INSERTITEMA
                || m.Msg == TVM_INSERTITEMW
                || m.Msg == TVM_DELITEM
                ) { //WM_VSCROLL

                //short t=Win32Native.LoWord((uint)m.WParam.ToInt32());
                //if (t == 5 || t==4) { //SB_THUMBTRACK \\ SB_THUMBPOSITION
                //    Win32Native.SCROLLINFO result;
                //    bool ret = Win32Native.GetScrollPos(this.Handle, 0, out result);
                //    if (ret) {
                //        //this.TopNode = this.Nodes[ret];
                //        //(0x110b, 5, lparam);

                //        //Win32Native.SendMessage(this.Handle, 0x110b, 5, this.Handle);
                //    }
                //}
                Win32Native.SCROLLINFO result;
                if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                    if (result.nMax != m_VScroll.Maximum) {
                        int heightPixel = GetItemHeight();
                        m_VScroll.PixelLogicFactor = heightPixel;
                        m_VScroll.Maximum = result.nMax;
                    }
                }
                if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                    if (result.nMax != m_HScroll.Maximum) {
                        //int heightPixel = GetItemHeight();
                        m_HScroll.PixelLogicFactor = 1;
                        m_HScroll.Maximum = result.nMax;
                    }
                }
                testCount++;
                System.Diagnostics.Debug.WriteLine(testCount);
                //m_VScroll.Maximum=
            }
            if (m.Msg == 276) {
                System.Diagnostics.Debug.WriteLine(Win32Native.HiWord((uint)m.WParam.ToInt32()));
            }
            base.WndProc(ref m);

        }
        public int GetItemHeight() {
            int ret = Win32Native.SendMessage(this.Handle, TVM_GETITEMHEIGHT, 0, 0);
            return ret;
        }
        private RibbonScrollbar m_VScroll;
        private RibbonScrollbar m_HScroll;
        public RibbonTreeView() {

            m_VScroll = new RibbonScrollbar();
            m_VScroll.Visible = true;
            m_VScroll.Scroll += new ScrollEventHandler(OnScroll);

            m_HScroll = new RibbonScrollbar();
            m_HScroll.Orientation = ScrollOrientation.HorizontalScroll;
            m_HScroll.Visible = true;
            m_HScroll.Scroll += new ScrollEventHandler(OnScroll);

            NativeAPI.ShowScrollBar(this.Handle, 0, false);
            NativeAPI.ShowScrollBar(this.Handle, 1, false);

        }

        /// <summary>
        /// 覆盖了基类的EndUpdate
        /// </summary>
        public new void EndUpdate() {
            //为了能够获得最新的滚动最大值.

            base.EndUpdate();

            Win32Native.SCROLLINFO result;
            if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                if (result.nMax != m_VScroll.Maximum) {
                    int heightPixel = GetItemHeight();
                    m_VScroll.PixelLogicFactor = heightPixel;
                    m_VScroll.Maximum = result.nMax;
                }
            }
            if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                if (result.nMax != m_HScroll.Maximum) {
                    //int heightPixel = GetItemHeight();
                    m_HScroll.PixelLogicFactor = 1;
                    m_HScroll.Maximum = result.nMax;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            int d = (int)Win32Native.MakeUInt32(0, (short)e.Delta);
            Win32Native.SendMessage(m_VScroll.Handle, 522, d, 0);
            base.OnMouseWheel(e);
        }
        protected override void OnSizeChanged(EventArgs e) {
            m_HScroll.Height = 16;
            m_VScroll.Height = this.Height - m_HScroll.Height;
            m_HScroll.Width = this.Width - m_VScroll.Width;
            //需要动态获取此类控件的LargeChange信息,通常是一整个ClientRect的量.
            Win32Native.SCROLLINFO lsi;
            if (Win32Native.GetScrollPos(this.Handle, 1, out lsi)) {
                m_VScroll.LargeChange = lsi.nPage;
            }
            if (Win32Native.GetScrollPos(this.Handle, 0, out lsi)) {
                m_HScroll.LargeChange = lsi.nPage;
            }
            NativeAPI.ShowScrollBar(this.Handle, 0, false);
            NativeAPI.ShowScrollBar(this.Handle, 1, false);
            base.OnSizeChanged(e);
        }
        protected override void OnLocationChanged(EventArgs e) {
            m_VScroll.Location = new Point(this.Left + this.Width - m_VScroll.Width - 1, this.Top);
            m_HScroll.Location = new Point(this.Left, this.Top + this.Height - m_HScroll.Height - 1);
            base.OnLocationChanged(e);
        }
        protected override void OnParentChanged(EventArgs e) {
            m_VScroll.Parent = this.Parent;
            m_VScroll.BringToFront();

            m_HScroll.Parent = this.Parent;
            m_HScroll.BringToFront();
            base.OnParentChanged(e);
        }
        protected override void OnAfterExpand(TreeViewEventArgs e) {
            Win32Native.SCROLLINFO result;
            if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                if (result.nMax != m_VScroll.Maximum) {
                    int heightPixel = GetItemHeight();
                    m_VScroll.PixelLogicFactor = heightPixel;
                    m_VScroll.Maximum = result.nMax;


                }
            }
            if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                if (result.nMax != m_HScroll.Maximum) {
                    //int heightPixel = GetItemHeight();
                    m_HScroll.PixelLogicFactor = 1;
                    m_HScroll.Maximum = result.nMax;
                }
            }
            NativeAPI.ShowScrollBar(this.Handle, 0, false);
            NativeAPI.ShowScrollBar(this.Handle, 1, false);
            base.OnAfterExpand(e);
        }
        protected override void OnAfterCollapse(TreeViewEventArgs e) {
            Win32Native.SCROLLINFO result;
            if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                if (result.nMax != m_VScroll.Maximum) {
                    int heightPixel = GetItemHeight();
                    m_VScroll.PixelLogicFactor = heightPixel;
                    m_VScroll.Maximum = result.nMax;

                }
            }
            if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                if (result.nMax != m_HScroll.Maximum) {
                    //int heightPixel = GetItemHeight();
                    m_HScroll.PixelLogicFactor = 1;
                    m_HScroll.Maximum = result.nMax;
                }
            }
            NativeAPI.ShowScrollBar(this.Handle, 0, false);
            NativeAPI.ShowScrollBar(this.Handle, 1, false);

            base.OnAfterCollapse(e);
        }

        protected override void Dispose(bool disposing) {
            if (m_VScroll != null && !m_VScroll.IsDisposed) {
                m_VScroll.Scroll = null;
                m_VScroll.Dispose();
                m_VScroll = null;
            }
            if (m_HScroll != null && !m_HScroll.IsDisposed) {
                m_HScroll.Scroll = null;
                m_HScroll.Dispose();
                m_HScroll = null;
            }
            base.Dispose(disposing);
        }

        private const int WM_VSCROLL = 277;
        private const int WM_HSCROLL = 276;
        private void OnScroll(object sender, ScrollEventArgs e) {
            RibbonScrollbar rs = sender as RibbonScrollbar;
            switch (e.Type) {
                case ScrollEventType.LargeIncrement:
                    Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 3, 0);
                    break;
                case ScrollEventType.LargeDecrement:
                    Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 2, 0);
                    break;
                case ScrollEventType.SmallDecrement:
                    Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 0, 0);
                    break;
                case ScrollEventType.SmallIncrement:
                    //e.NewValue 
                    Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 1, 0);
                    break;
                case ScrollEventType.ThumbPosition:
                    if (e.NewValue < short.MaxValue) {
                        int row = e.NewValue;
                        uint k = Win32Native.MakeUInt32(4, (short)row);
                        Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, (int)k, 0);
                    }
                    else {
                        Win32Native.SCROLLINFO lsc = new Win32Native.SCROLLINFO(
                            16, 0, rs.Maximum, 0, e.NewValue);

                        int nBar = (rs.Orientation == ScrollOrientation.VerticalScroll ? 1 : 0);

                        Win32Native.SetScrollInfo(this.Handle, nBar, ref lsc, true);
                        Win32Native.SendMessage(this.Handle, rs.Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 4, 0);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
