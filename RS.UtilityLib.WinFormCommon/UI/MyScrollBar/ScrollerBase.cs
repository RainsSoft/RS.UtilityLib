using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.MyScrollBar
{
    #region Scrollers

    public class ScrollerBase :  IDisposable {//,IEventHandler, {
        protected Control control;
        protected ScrollBarEx vScrollBar;
        protected ScrollBarEx hScrollBar;
        protected Control scrollerCorner;
        private bool disposed = false;

        /// <summary>
        /// Initialize ScrollerBase
        /// </summary>
        public ScrollerBase(Control control) {
            this.control = control;
            ScaleHelper.SetScaleForm(control.FindForm());
        }

        ///// <summary>
        ///// Handle the incoming theme events
        ///// </summary>
        //public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority) {
        //    if(e.Type == EventType.ApplyTheme) {
        //        Boolean enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
        //        if(control.Parent == null)
        //            return;
        //        else if(enabled) {
        //            if(!control.Parent.Controls.Contains(vScrollBar))
        //                AddScrollBars();
        //            UpdateScrollBarTheme();
        //        }
        //        else if(!enabled && control.Parent.Controls.Contains(vScrollBar)) {
        //            RemoveScrollBars();
        //        }
        //    }
        //}

        /// <summary>
        /// Dispose the controls (public interface)
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                RemoveScrollBars();
                control = null;
                vScrollBar.Dispose();
                hScrollBar.Dispose();
                scrollerCorner.Dispose();
            }
            disposed = true;
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        protected virtual void InitScrollBars() {
            vScrollBar = new ScrollBarEx();
            vScrollBar.Width = SystemInformation.VerticalScrollBarWidth+2; // Already scaled.
            // Should be odd for nice and crisp arrow points.
            if(vScrollBar.Width % 2 == 0)
                ++vScrollBar.Width;
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            hScrollBar = new ScrollBarEx();
            hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight+2; // Already scaled.
            // Should be odd for nice and crisp arrow points.
            if(hScrollBar.Height % 2 == 0)
                ++hScrollBar.Width;
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            scrollerCorner = new Control();
            scrollerCorner.Width = vScrollBar.Width;
            scrollerCorner.Height = hScrollBar.Height;
            //if(PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) {
                AddScrollBars();
                UpdateScrollBarTheme();
            //}
            //EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        /// <summary>
        /// Add controls to container
        /// </summary>
        protected virtual void AddScrollBars() {
            control.Parent.Controls.Add(hScrollBar);
            control.Parent.Controls.Add(vScrollBar);
            control.Parent.Controls.Add(scrollerCorner);
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Scroll += OnScroll;
            vScrollBar.VisibleChanged += OnResize;
            hScrollBar.VisibleChanged += OnResize;
            control.Parent.Resize += OnResize;
            control.Resize += OnResize;
            control.Paint += OnPaint;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        protected virtual void RemoveScrollBars() {
            control.Parent.Controls.Remove(hScrollBar);
            control.Parent.Controls.Remove(vScrollBar);
            control.Parent.Controls.Remove(scrollerCorner);
            vScrollBar.Scroll -= OnScroll;
            hScrollBar.Scroll -= OnScroll;
            vScrollBar.VisibleChanged -= OnResize;
            hScrollBar.VisibleChanged -= OnResize;
            control.Parent.Resize -= OnResize;
            control.Resize -= OnResize;
            control.Paint -= OnPaint;
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        protected virtual void UpdateScrollBarTheme() {
            //PluginBase.MainForm.ThemeControls(vScrollBar);
            //PluginBase.MainForm.ThemeControls(hScrollBar);
            //// Apply settings so that old defaults work...
            //vScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", vScrollBar.ForeColor);
            //vScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", vScrollBar.ForeColor);
            //vScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", vScrollBar.ActiveForeColor);
            //vScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", vScrollBar.ForeColor);
            //hScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", hScrollBar.ForeColor);
            //hScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", hScrollBar.ForeColor);
            //hScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", hScrollBar.ActiveForeColor);
            //hScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", hScrollBar.ForeColor);
            //scrollerCorner.BackColor = PluginBase.MainForm.GetThemeColor("ScrollBar.BackColor", vScrollBar.BackColor);
#if DEBUG
            scrollerCorner.BackColor = Color.Red;
#endif
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected virtual void UpdateScrollState() {
            Win32.SCROLLINFO vScroll = Win32.GetFullScrollInfo(control, false);
            Win32.SCROLLINFO hScroll = Win32.GetFullScrollInfo(control, true);
            if(vScroll != null && hScroll != null) {
                vScrollBar.Visible = vScroll.nMax > (vScroll.nPage - 1) && vScroll.nPage > 0;
                hScrollBar.Visible = hScroll.nMax > (hScroll.nPage - 1) && hScroll.nPage > 0;
                vScrollBar.Scroll -= OnScroll;
                vScrollBar.Minimum = vScroll.nMin;
                vScrollBar.Maximum = vScroll.nMax - (vScroll.nPage - 1);
                vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScroll.nPage - 1);
                vScrollBar.Value = vScroll.nPos;
                vScrollBar.Scroll += OnScroll;
                hScrollBar.Scroll -= OnScroll;
                hScrollBar.Minimum = hScroll.nMin;
                hScrollBar.Maximum = hScroll.nMax - (hScroll.nPage - 1);
                hScrollBar.ViewPortSize = hScrollBar.LargeChange = (hScroll.nPage - 1);
                hScrollBar.Value = hScroll.nPos;
                hScrollBar.Scroll += OnScroll;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnResize(Object sender, EventArgs e) {
            int borderWidth = 0;
            if(control is PropertyGrid && borderWidth == 0)
                borderWidth = 1;
            int vScrollBarHeight = (control.Height - (borderWidth * 2)) - (hScrollBar.Visible ? hScrollBar.Height : 0);
            if(control is PropertyGrid) {
                foreach(Control ctrl in control.Controls) {
                    if(ctrl.Text == "PropertyGridView") {
                        Type type = ctrl.GetType();
                        FieldInfo field = type.GetField("scrollBar", BindingFlags.Instance | BindingFlags.NonPublic);
                        var scrollBar = field.GetValue(ctrl) as ScrollBar;
                        vScrollBarHeight = scrollBar.Height;
                    }
                }
            }
            // Sets size, location and visibility
            vScrollBar.SetBounds(control.Location.X + control.Width - vScrollBar.Width - borderWidth, control.Location.Y + borderWidth, vScrollBar.Width, vScrollBarHeight);
            hScrollBar.SetBounds(control.Location.X + borderWidth, control.Location.Y + control.Height - hScrollBar.Height - borderWidth, (control.Width - (borderWidth * 2)) - (vScrollBar.Visible ? vScrollBar.Width : 0), hScrollBar.Height);
            scrollerCorner.Visible = vScrollBar.Visible && hScrollBar.Visible;
            if(scrollerCorner.Visible) {
                scrollerCorner.Location = new Point(vScrollBar.Location.X, hScrollBar.Location.Y);
                scrollerCorner.Refresh();
                scrollerCorner.BringToFront();
            }
            vScrollBar.BringToFront();
            hScrollBar.BringToFront();
            control.Invalidate();
        }

        /// <summary>
        /// Updates the scroll state on control paint
        /// </summary>
        protected virtual void OnPaint(Object sender, PaintEventArgs e) => UpdateScrollState();

        /// <summary>
        /// Updates the control on scrollbar scroll
        /// </summary>
        protected virtual void OnScroll(Object sender, ScrollEventArgs e) {
        }
    }

    public class TextBoxScroller : ScrollerBase {
        private bool disposed = false;
        private TextBox textBox;

        /// <summary>
        /// Initialize TextBoxScroller
        /// </summary>
        public TextBoxScroller(TextBox view) : base(view) {
            textBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                textBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the current line index
        /// </summary>
        public Int32 GetLineIndex(int index) {
            return Win32.SendMessage(textBox.Handle, Win32.EM_LINEINDEX, index, 0);
        }

        /// <summary>
        /// Gets the amount of lines, also with wrapping
        /// </summary>
        public Int32 GetLineCount() {
            return Win32.SendMessage(textBox.Handle, Win32.EM_GETLINECOUNT, 0, 0);
        }

        /// <summary>
        /// Gets the first visible line on screen
        /// </summary>
        public Int32 GetFirstVisibleLine() {
            return (Int32)Win32.SendMessage(textBox.Handle, Win32.EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        /// <summary>
        /// Gets the amount of visible lines
        /// </summary>
        public Int32 GetVisibleLines() {
            var rect = new Win32.RECT();
            Win32.SendMessage(textBox.Handle, Win32.EM_GETRECT, IntPtr.Zero, ref rect);
            var count = (rect.Bottom - rect.Top) / textBox.Font.Height;
            return count;
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState() {
            Int32 vScrollMax = GetLineCount();
            Int32 vScrollPos = GetFirstVisibleLine();
            Int32 vScrollPage = GetVisibleLines();
            if(this.textBox.ScrollBars != ScrollBars.Vertical) {
                // Force scrollbar so that content is displayed correctly...
                this.textBox.ScrollBars = ScrollBars.Vertical;
            }
            vScrollBar.Visible = vScrollMax > (vScrollPage - 1) && vScrollMax != vScrollPage;
            vScrollBar.Scroll -= OnScroll;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = vScrollMax - (vScrollPage);
            vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScrollPage - 1);
            vScrollBar.Value = vScrollPos;
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Visible = false;
        }

        /// <summary>
        /// Updates the textBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || textBox.Lines.Length == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
        }
    }

    public class RichTextBoxScroller : ScrollerBase {
        private bool disposed = false;
        private RichTextBox textBox;

        /// <summary>
        /// Initialize RichTextBoxScroller
        /// </summary>
        public RichTextBoxScroller(RichTextBox view) : base(view) {
            textBox = view;
            InitScrollBars();
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e) {
            this.UpdateScrollState();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the textBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || textBox.Lines.Length == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
            else {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
        }
    }

    public class PropertyGridScroller : ScrollerBase {
        private bool disposed = false;
        private PropertyGrid propertyGrid;

        /// <summary>
        /// Initialize PropertyGridScroller
        /// </summary>
        public PropertyGridScroller(PropertyGrid view) : base(view) {
            propertyGrid = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                propertyGrid = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the amount of visible rows
        /// </summary>
        private Int32 GetVisibleRows() {
            foreach(Control ctrl in propertyGrid.Controls) {
                if(ctrl.Text == "PropertyGridView") {
                    Type type = ctrl.GetType();
                    FieldInfo field = type.GetField("visibleRows", BindingFlags.Instance | BindingFlags.NonPublic);
                    return (Int32)field.GetValue(ctrl) - 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Updates the scroll position of the scrollbar
        /// </summary>
        private void SetScrollOffset(Int32 value) {
            foreach(Control ctrl in propertyGrid.Controls) {
                if(ctrl.Text == "PropertyGridView") {
                    Type type = ctrl.GetType();
                    MethodInfo info = type.GetMethod("SetScrollOffset");
                    object[] parameters = { value };
                    info.Invoke(ctrl, parameters);
                }
            }
        }

        /// <summary>
        /// Gets the scrollbar reference
        /// </summary>
        private ScrollBar GetScrollBar() {
            foreach(Control ctrl in propertyGrid.Controls) {
                if(ctrl.Text == "PropertyGridView") {
                    Type type = ctrl.GetType();
                    FieldInfo field = type.GetField("scrollBar", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(ctrl) as ScrollBar;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState() {
            var scrollBar = GetScrollBar();
            if(scrollBar != null) {
                vScrollBar.Scroll -= OnScroll;
                vScrollBar.Visible = scrollBar.Visible;
                vScrollBar.Minimum = scrollBar.Minimum;
                vScrollBar.Maximum = scrollBar.Maximum - (GetVisibleRows() - 1);
                vScrollBar.SmallChange = scrollBar.SmallChange;
                vScrollBar.LargeChange = scrollBar.LargeChange;
                vScrollBar.ViewPortSize = scrollBar.LargeChange = (GetVisibleRows() - 1);
                vScrollBar.Value = scrollBar.Value;
                vScrollBar.Scroll += OnScroll;
            }
            else
                vScrollBar.Visible = false;
            hScrollBar.Visible = false;
        }

        /// <summary>
        /// Updates the propertyGrid on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || propertyGrid.SelectedObjects.Length == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                SetScrollOffset(e.NewValue);
            }
        }
    }

    public class TreeViewScroller : ScrollerBase {
        private bool disposed = false;
        private TreeView treeView;

        /// <summary>
        /// Initialize TreeViewScroller
        /// </summary>
        public TreeViewScroller(TreeView view) : base(view) {
            treeView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                treeView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the treeView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || treeView.Nodes.Count == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                if(!PlatformHelper.isRunningOnWine()) {
                    treeView.BeginUpdate();
                    Win32.SetScrollPos(treeView.Handle, Win32.SB_VERT, e.NewValue, true);
                    treeView.EndUpdate();
                }
                else {
                    int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                    Win32.SendMessage(treeView.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
                }

            }
            else {
                if(!PlatformHelper.isRunningOnWine()) {
                    treeView.BeginUpdate();
                    Win32.SetScrollPos(treeView.Handle, Win32.SB_HORZ, e.NewValue, true);
                    treeView.EndUpdate();
                }
                else {
                    int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                    Win32.SendMessage(treeView.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
                }
            }
        }
    }

    public class ListViewScroller : ScrollerBase {
        private bool disposed = false;
        protected ListView listView;

        /// <summary>
        /// Initialize ListViewScroller
        /// </summary>
        public ListViewScroller(ListView view) : base(view) {
            listView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                listView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the listView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || listView.Items.Count == 0)
                return;
            Int32 height = listView.GetItemRect(0).Height; // Item height in pixels
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                Int32 vScroll;
                if(listView.ShowGroups && !PlatformHelper.isRunningOnWine()) {
                    Int32 prevPos = Win32.GetScrollPos(listView.Handle, Win32.SB_VERT);
                    vScroll = -(prevPos - e.NewValue);
                }
                else
                    vScroll = -(e.OldValue - e.NewValue) * height;
                Win32.SendMessage(listView.Handle, (Int32)Win32.LVM_SCROLL, IntPtr.Zero, (IntPtr)vScroll);
            }
            else {
                Int32 hScroll = -(e.OldValue - e.NewValue);
                Win32.SendMessage(listView.Handle, (Int32)Win32.LVM_SCROLL, (IntPtr)hScroll, IntPtr.Zero);
            }
        }
    }

    public class ListBoxScroller : ScrollerBase {
        private bool disposed = false;
        private ListBox listBox;

        /// <summary>
        /// Initialize ListBoxScroller
        /// </summary>
        public ListBoxScroller(ListBox view) : base(view) {
            listBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                listBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the listBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || listBox.Items.Count == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                Win32.PostMessage((IntPtr)listBox.Handle, Win32.WM_VSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
            else {
                Win32.PostMessage((IntPtr)listBox.Handle, Win32.WM_HSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
        }
    }

    public class DataGridViewScroller : ScrollerBase {
        private bool disposed = false;
        private DataGridView dataGridView;

        /// <summary>
        /// Initialize DataGridViewScroller
        /// </summary>
        public DataGridViewScroller(DataGridView view) : base(view) {
            dataGridView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                dataGridView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState() {
            Int32 vScrollMax = dataGridView.RowCount;
            Int32 vScrollPos = dataGridView.FirstDisplayedScrollingRowIndex;
            Int32 vScrollPage = dataGridView.DisplayedRowCount(false);
            Int32 hScrollMax = dataGridView.ColumnCount;
            Int32 hScrollPos = dataGridView.FirstDisplayedScrollingColumnIndex;
            Int32 hScrollPage = dataGridView.DisplayedColumnCount(false);
            vScrollBar.Visible = vScrollMax > (vScrollPage - 1) && vScrollMax != vScrollPage;
            hScrollBar.Visible = hScrollMax > (hScrollPage - 1) && hScrollMax != hScrollPage;
            vScrollBar.Scroll -= OnScroll;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = vScrollMax - (vScrollPage);
            vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScrollPage - 1);
            vScrollBar.Value = vScrollPos;
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Scroll -= OnScroll;
            hScrollBar.Minimum = 0;
            hScrollBar.Maximum = hScrollMax - (hScrollPage - 1);
            hScrollBar.ViewPortSize = hScrollBar.LargeChange = (hScrollPage - 1);
            hScrollBar.Value = hScrollPos;
            hScrollBar.Scroll += OnScroll;
        }

        /// <summary>
        /// Updates the dataGridView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(Object sender, ScrollEventArgs e) {
            if(e.OldValue == -1 || dataGridView.RowCount == 0)
                return;
            if(e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                dataGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
            else
                dataGridView.FirstDisplayedScrollingColumnIndex = e.NewValue;
        }

    }

    #endregion


    #region Enums


    /// <summary>
    /// Enum for the scrollbar orientation.
    /// </summary>
    public enum ScrollBarOrientation
    {
        /// <summary>
        /// Indicates a horizontal scrollbar.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Indicates a vertical scrollbar.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// The scrollbar states.
    /// </summary>
    internal enum ScrollBarState
    {
        /// <summary>
        /// Indicates a normal scrollbar state.
        /// </summary>
        Normal,

        /// <summary>
        /// Indicates a hot scrollbar state.
        /// </summary>
        Hot,

        /// <summary>
        /// Indicates an active scrollbar state.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates a pressed scrollbar state.
        /// </summary>
        Pressed,

        /// <summary>
        /// Indicates a disabled scrollbar state.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// The scrollbar arrow button states.
    /// </summary>
    internal enum ScrollBarArrowButtonState
    {
        /// <summary>
        /// Indicates the up arrow is in normal state.
        /// </summary>
        UpNormal,

        /// <summary>
        /// Indicates the up arrow is in hot state.
        /// </summary>
        UpHot,

        /// <summary>
        /// Indicates the up arrow is in active state.
        /// </summary>
        UpActive,

        /// <summary>
        /// Indicates the up arrow is in pressed state.
        /// </summary>
        UpPressed,

        /// <summary>
        /// Indicates the up arrow is in disabled state.
        /// </summary>
        UpDisabled,

        /// <summary>
        /// Indicates the down arrow is in normal state.
        /// </summary>
        DownNormal,

        /// <summary>
        /// Indicates the down arrow is in hot state.
        /// </summary>
        DownHot,

        /// <summary>
        /// Indicates the down arrow is in active state.
        /// </summary>
        DownActive,

        /// <summary>
        /// Indicates the down arrow is in pressed state.
        /// </summary>
        DownPressed,

        /// <summary>
        /// Indicates the down arrow is in disabled state.
        /// </summary>
        DownDisabled
    }

    #endregion
}
