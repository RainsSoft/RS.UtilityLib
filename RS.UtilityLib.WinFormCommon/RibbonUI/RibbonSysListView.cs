using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// listview
    /// </summary>
    public class RibbonSysListView : ListView
    {
        private bool IsDesignerHosted {
            get {
                if (DesignMode)
                    return DesignMode;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }

        private int testCount = 0;
        protected override void WndProc(ref Message m) {




            if (m.Msg == NativeAPI.LVM_INSERTITEMA
                || m.Msg == NativeAPI.LVM_INSERTITEMW
                || m.Msg == NativeAPI.LVM_DELETEITEM
                ) { //WM_VSCROLL

                Win32Native.SCROLLINFO result;
                if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                    if (result.nMax != m_VScroll.Maximum) {
                        //int heightPixel = GetItemHeight();
                        //m_VScroll.PixelLogicFactor = heightPixel;
                        m_VScroll.Maximum = result.nMax;
                    }
                }
                if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                    if (result.nMax != m_HScroll.Maximum) {
                        //int heightPixel = GetItemHeight();
                        //m_HScroll.PixelLogicFactor = 1;
                        m_HScroll.Maximum = result.nMax;
                    }
                }
                testCount++;
                //System.Diagnostics.Debug.WriteLine(testCount);
                //m_VScroll.Maximum=
            }

            base.WndProc(ref m);

        }
        public int GetItemHeight() {
            if (this.Items.Count > 0) {
                Rectangle rect = this.GetItemRect(0, ItemBoundsPortion.Entire);
                return rect.Height;
            }
            else {
                return 14;
            }

        }
        public new void EndUpdate() {
            base.EndUpdate();
            Win32Native.SCROLLINFO result;
            if (Win32Native.GetScrollPos(this.Handle, 1, out result)) {
                if (result.nMax != m_VScroll.Maximum) {
                    //int heightPixel = GetItemHeight();
                    //m_VScroll.PixelLogicFactor = heightPixel;
                    m_VScroll.Maximum = result.nMax * result.nPage;
                }
            }
            if (Win32Native.GetScrollPos(this.Handle, 0, out result)) {
                if (result.nMax != m_HScroll.Maximum) {
                    //int heightPixel = GetItemHeight();
                    //m_HScroll.PixelLogicFactor = 1;
                    m_HScroll.Maximum = result.nMax;
                }
            }

        }
        private RibbonScrollbar m_VScroll;
        private RibbonScrollbar m_HScroll;
        public RibbonSysListView() : base() {
            this.SuspendLayout();
            //this.Scrollable = false;
            m_VScroll = new RibbonScrollbar();
            m_VScroll.Visible = true;
            m_VScroll.Scroll += new ScrollEventHandler(OnScroll);

            m_HScroll = new RibbonScrollbar();
            m_HScroll.Orientation = ScrollOrientation.HorizontalScroll;
            m_HScroll.Visible = true;
            m_HScroll.Scroll += new ScrollEventHandler(OnScroll);

            NativeAPI.ShowScrollBar(this.Handle, 0, false);
            NativeAPI.ShowScrollBar(this.Handle, 1, false);
            View = View.Details;
            //
            this.Controls.Add(m_VScroll);
            this.Controls.Add(m_HScroll);
            //
            this.ResumeLayout();
        }
        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams t = base.CreateParams;

        //        t.Style -= 0x2000;
        //        return t;// base.CreateParams;
        //    }
        //}
        protected override void OnSizeChanged(EventArgs e) {
            m_HScroll.Height = 16;
            m_VScroll.Height = this.Height - m_HScroll.Height;
            m_HScroll.Width = this.Width - m_VScroll.Width;
            if (this.IsDesignerHosted) {
                m_VScroll.Location = new Point(this.Width - m_VScroll.Width - 1, 0);
                m_HScroll.Location = new Point(0, this.Height - m_HScroll.Height - 1);
            }
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
            if (this.IsDesignerHosted) {
                m_VScroll.Location = new Point(this.Width - m_VScroll.Width - 1, 0);
                m_HScroll.Location = new Point(0, this.Height - m_HScroll.Height - 1);
            }
            else {
                m_VScroll.Location = new Point(this.Left + this.Width - m_VScroll.Width - 1, this.Top);
                m_HScroll.Location = new Point(this.Left, this.Top + this.Height - m_HScroll.Height - 1);
            }
            base.OnLocationChanged(e);
        }
        protected override void OnParentChanged(EventArgs e) {
            if (this.IsDesignerHosted == false) {
                m_VScroll.Parent = this.Parent;
                m_VScroll.BringToFront();

                m_HScroll.Parent = this.Parent;
                m_HScroll.BringToFront();
                //修改位置
                m_VScroll.Location = new Point(this.Left + this.Width - m_VScroll.Width - 1, this.Top);
                m_HScroll.Location = new Point(this.Left, this.Top + this.Height - m_HScroll.Height - 1);
            }
            base.OnParentChanged(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            int d = (int)Win32Native.MakeUInt32(0, (short)e.Delta);
            Win32Native.SendMessage(m_VScroll.Handle, 522, d, 0);
            base.OnMouseWheel(e);
        }

        protected override void Dispose(bool disposing) {
            try {
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
            }
            catch {
            }
            base.Dispose(disposing);
        }

        private const int WM_VSCROLL = 277;
        private const int WM_HSCROLL = 276;

        private int m_PrevX, m_PrevY;
        private void OnScroll(object sender, ScrollEventArgs e) {
            RibbonScrollbar rs = sender as RibbonScrollbar;
            int x, y;
            switch (e.Type) {
                case ScrollEventType.LargeIncrement:
                    x = (rs.Orientation == ScrollOrientation.HorizontalScroll ? rs.LargeChange : 0);
                    y = (rs.Orientation == ScrollOrientation.VerticalScroll ? rs.LargeChange : 0);
                    Win32Native.SendMessage(this.Handle, NativeAPI.LVM_SCROLL, x, y);
                    break;
                case ScrollEventType.LargeDecrement:
                    x = (rs.Orientation == ScrollOrientation.HorizontalScroll ? -rs.LargeChange : 0);
                    y = (rs.Orientation == ScrollOrientation.VerticalScroll ? -rs.LargeChange : 0);
                    Win32Native.SendMessage(this.Handle, NativeAPI.LVM_SCROLL, x, y);
                    break;
                case ScrollEventType.SmallDecrement:
                    x = (rs.Orientation == ScrollOrientation.HorizontalScroll ? -rs.SmallChange : 0);
                    y = (rs.Orientation == ScrollOrientation.VerticalScroll ? -rs.SmallChange : 0);

                    Win32Native.SendMessage(this.Handle, NativeAPI.LVM_SCROLL, x, y);
                    break;
                case ScrollEventType.SmallIncrement:
                    x = (rs.Orientation == ScrollOrientation.HorizontalScroll ? rs.SmallChange : 0);
                    y = (rs.Orientation == ScrollOrientation.VerticalScroll ? rs.SmallChange : 0);

                    //e.NewValue 
                    Win32Native.SendMessage(this.Handle, NativeAPI.LVM_SCROLL, x, y);
                    break;
                case ScrollEventType.ThumbPosition:
                    x = (rs.Orientation == ScrollOrientation.HorizontalScroll ? rs.Value : 0);
                    y = (rs.Orientation == ScrollOrientation.VerticalScroll ? rs.Value : 0);
                    int dx = x - m_PrevX;
                    int dy = y - m_PrevY;
                    m_PrevX = x;
                    m_PrevY = y;
                    if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
                        NativeAPI.PostMessage(this.Handle, (uint)WM_HSCROLL, 4, e.NewValue);
                    }
                    else {
                        NativeAPI.PostMessage(this.Handle, (uint)WM_VSCROLL, 4, e.NewValue);
                    }
                    //Win32Native.SendMessage(this.Handle, NativeAPI.LVM_SCROLL, dx, dy);
                    break;
                default:
                    break;
            }
        }

    }
}
