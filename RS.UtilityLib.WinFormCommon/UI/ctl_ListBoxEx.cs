using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public partial class ctl_ListBoxEx : ScrollableControl {
        MouseMessageFilter m_f;
        public ctl_ListBoxEx() {
            InitializeComponent();
            this.DoubleBuffered = true;
            m_f = new MouseMessageFilter(this.Handle);
            m_f.MouseMove += new MouseMessageFilter.CancelMouseEventHandler(m_f_MouseMove);
            m_f.StartFiltering();
        //    this.VerticalScroll.Visible = true;
         //   this.VerticalScroll.Enabled = true;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x;

            public int y;

            public POINT() {
            }

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        //public static HandleRef NullHandleRef;
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //public static extern int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, [In] [Out] POINT pt, int cPoints);
        //private Point PointToScreen(IntPtr handle, Point p) {
        //    POINT pOINT = new POINT(p.X, p.Y);
        //    MapWindowPoints(new HandleRef(this, handle), NullHandleRef, pOINT, 1);
        //    return new Point(pOINT.x, pOINT.y);
        //}
        void m_f_MouseMove(object source, Message e) {
            if(this.HasDestroyed) {
                return;
            }
            //int x = e.LParam.ToInt32() & 0x0000FFFF;
            //int y = e.LParam.ToInt32() >> 16;
            //Debug.WriteLine(x + "  " + y);
            
            //if (this.ClientRectangle.Contains(e.Location) == false) {
            //    return;
            //}

            foreach (var v in m_list) {
               
                //var rect = v.RectangleToScreen(v.ClientRectangle);
                var rect = new Rectangle(v.Location, v.Size);
                var rect_screen = v.RectangleToScreen(v.ClientRectangle);

                var p1 = Form.MousePosition;

                if (rect_screen.Contains(p1)) {
                    v.m_MouseState = ctl_ListBoxExItem.MouseState.Hover;
                }
                else {
                    v.m_MouseState = ctl_ListBoxExItem.MouseState.None;
                }
            }
            this.Refresh();
        }
        private List<ctl_ListBoxExItem> m_list = new List<ctl_ListBoxExItem>();
        public List<ctl_ListBoxExItem> Items {
            get {
                return m_list;
            }
        }
        public void ClearItem() {
            foreach (var v in m_list) {
                v.Dispose();
            }
            m_list.Clear();
        }
        public event Action<object> OnSelect;
        public event Action<object> OnDelete;
        public void AddItem(object item) {
            var ctl = new ctl_ListBoxExItem { ItemObj = item };

            //
            //ctl.Width = this.ClientSize.Width;
            //ctl.Top = m_list.Count * ctl.Height;
            m_list.Add(ctl);
       
            this.Controls.Add(ctl);
            ctl.Dock = DockStyle.Top;
            ctl.OnDelButtonClick += new EventHandler(ctl_Disposed);
            ctl.MouseClick += new MouseEventHandler(ctl_MouseClick);

        }

        void ctl_MouseClick(object sender, MouseEventArgs e) {
            var obj = (sender as ctl_ListBoxExItem).ItemObj;

            if (OnSelect != null) {
                OnSelect(obj);
            }
        }

        void ctl_Disposed(object sender, EventArgs e) {
            var ctl = (sender as ctl_ListBoxExItem);
            m_list.Remove(ctl);
            if (OnDelete != null) {
                OnDelete(ctl.ItemObj);
            }
            ctl.Dispose();

        }


        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if (this.Items.Count > 0) {
                // this.Items[0].Focus();

            }
        }
        protected override Point ScrollToControl(Control activeControl) {
            return this.AutoScrollPosition;// base.ScrollToControl(activeControl);
        }
        protected override void OnLostFocus(EventArgs e) {
            foreach (var v in m_list) {
                if (v.Focused) {
                    return;
                }
            }
            base.OnLostFocus(e);
            // if(
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            foreach (var v in m_list) {
                if (v.ClientRectangle.Contains(e.Location)) {
                    v.m_MouseState = ctl_ListBoxExItem.MouseState.Hover;
                }
                else {
                    v.m_MouseState = ctl_ListBoxExItem.MouseState.None;
                }
            }
            this.Refresh();
        }
        public bool HasDestroyed {
            get;
            private set;
        }
        protected override void OnHandleDestroyed(EventArgs e) {
            HasDestroyed = true;
            if(m_f != null) {
                m_f.Dispose();
            }
            base.OnHandleDestroyed(e);
        }

    }

    class MouseMessageFilter : IMessageFilter, IDisposable {
        private IntPtr m_Hwnd;
        public MouseMessageFilter(IntPtr ctl) {
            m_Hwnd = ctl;
        }

        public void Dispose() {
            StopFiltering();
        }

        #region IMessageFilter Members
        private int WM_MOUSEMOVE = 0x0200;
        public bool PreFilterMessage(ref Message m) {
            // Call the appropriate event
            //if (m.HWnd != m_Hwnd) {
            //    return false;
            //}
            //if (m.Msg == WM_MOUSEMOVE) {
            //    if (MouseMove != null) {
            //        int x = m.LParam.ToInt32() & 0x0000FFFF;
            //        int y = m.LParam.ToInt32() >> 16;
            //        MouseMove(this, new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
            //    }
            //}
            if (MouseMove != null) {
                MouseMove(this, m);
            }
            return false;
        }

        #endregion






        public delegate void CancelMouseEventHandler(object source, Message e);
        public event CancelMouseEventHandler MouseMove;
        public event CancelMouseEventHandler MouseDown;
        public event CancelMouseEventHandler MouseUp;

        public void StartFiltering() {
            StopFiltering();
            Application.AddMessageFilter(this);
        }

        public void StopFiltering() {
            Application.RemoveMessageFilter(this);
        }
    }

}
