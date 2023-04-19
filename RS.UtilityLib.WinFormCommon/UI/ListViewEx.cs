using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// Event Handler for SubItem events
    /// </summary>
    public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);
    /// <summary>
    /// Event Handler for SubItemEndEditing events
    /// </summary>
    public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditingEventArgs e);

    /// <summary>
    /// Event Args for SubItemClicked event
    /// </summary>
    public class SubItemEventArgs : EventArgs
    {
        public SubItemEventArgs(ListViewItem item, int subItem) {
            _subItemIndex = subItem;
            _item = item;
        }
        private int _subItemIndex = -1;
        private ListViewItem _item = null;
        public int SubItem {
            get {
                return _subItemIndex;
            }
        }
        public ListViewItem Item {
            get {
                return _item;
            }
        }
    }


    /// <summary>
    /// Event Args for SubItemEndEditingClicked event
    /// </summary>
    public class SubItemEndEditingEventArgs : SubItemEventArgs
    {
        private string _text = string.Empty;
        private bool _cancel = true;

        public SubItemEndEditingEventArgs(ListViewItem item, int subItem, string display, bool cancel) :
            base(item, subItem) {
            _text = display;
            _cancel = cancel;
        }
        public string DisplayText {
            get {
                return _text;
            }
            set {
                _text = value;
            }
        }
        public bool Cancel {
            get {
                return _cancel;
            }
            set {
                _cancel = value;
            }
        }
    }


    ///	<summary>
    ///	Inherited ListView to allow in-place editing of subitems
    ///	</summary>
    public class ListViewEx : System.Windows.Forms.ListView
    {
        #region Interop structs, imports and constants
        /// <summary>
        /// MessageHeader for WM_NOTIFY
        /// </summary>
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public Int32 idFrom;
            public Int32 code;
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, ref int[] order);

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // Windows Messages that will abort editing
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_SIZE = 0x05;
        private const int WM_NOTIFY = 0x4E;

        private const int HDN_FIRST = -300;
        private const int HDN_BEGINDRAG = (HDN_FIRST - 10);
        private const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
        private const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);
        #endregion

        ///	<summary>
        ///	Required designer variable.
        ///	</summary>
        private System.ComponentModel.Container components = null;

        public event SubItemEventHandler SubItemClicked;
        public event SubItemEventHandler SubItemBeginEditing;
        public event SubItemEndEditingEventHandler SubItemEndEditing;

        public ListViewEx() {
            // This	call is	required by	the	Windows.Forms Form Designer.
            InitializeComponent();

            base.FullRowSelect = true;
            base.View = View.Details;
            base.AllowColumnReorder = true;
        }

        ///	<summary>
        ///	Clean up any resources being used.
        ///	</summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component	Designer generated code
        ///	<summary>
        ///	Required method	for	Designer support - do not modify 
        ///	the	contents of	this method	with the code editor.
        ///	</summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
        }
        #endregion

        private bool _doubleClickActivation = false;
        /// <summary>
        /// Is a double click required to start editing a cell?
        /// </summary>
        public bool DoubleClickActivation {
            get {
                return _doubleClickActivation;
            }
            set {
                _doubleClickActivation = value;
            }
        }


        /// <summary>
        /// Retrieve the order in which columns appear
        /// </summary>
        /// <returns>Current display order of column indices</returns>
        public int[] GetColumnOrder() {
            IntPtr lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0)  // Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            int[] order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }


        /// <summary>
        /// Find ListViewItem and SubItem Index at position (x,y)
        /// </summary>
        /// <param name="x">relative to ListView</param>
        /// <param name="y">relative to ListView</param>
        /// <param name="item">Item at position (x,y)</param>
        /// <returns>SubItem index</returns>
        public int GetSubItemAt(int x, int y, out ListViewItem item) {
            item = this.GetItemAt(x, y);

            if (item != null) {
                int[] order = GetColumnOrder();
                Rectangle lviBounds;
                int subItemX;

                lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
                subItemX = lviBounds.Left;
                for (int i = 0; i < order.Length; i++) {
                    ColumnHeader h = this.Columns[order[i]];
                    if (x < subItemX + h.Width) {
                        return h.Index;
                    }
                    subItemX += h.Width;
                }
            }

            return -1;
        }


        /// <summary>
        /// Get bounds for a SubItem
        /// </summary>
        /// <param name="Item">Target ListViewItem</param>
        /// <param name="SubItem">Target SubItem index</param>
        /// <returns>Bounds of SubItem (relative to ListView)</returns>
        public Rectangle GetSubItemBounds(ListViewItem Item, int SubItem) {
            int[] order = GetColumnOrder();

            Rectangle subItemRect = Rectangle.Empty;
            if (SubItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");

            if (Item == null)
                throw new ArgumentNullException("Item");

            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;

            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++) {
                col = this.Columns[order[i]];
                if (col.Index == SubItem)
                    break;
                subItemX += col.Width;
            }
            subItemRect = new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);
            return subItemRect;
        }


        protected override void WndProc(ref Message msg) {
            switch (msg.Msg) {
                // Look	for	WM_VSCROLL,WM_HSCROLL or WM_SIZE messages.
                case WM_VSCROLL:
                case WM_HSCROLL:
                case WM_SIZE:
                    EndEditing(false);
                    break;
                case WM_NOTIFY:
                    // Look for WM_NOTIFY of events that might also change the
                    // editor's position/size: Column reordering or resizing
                    NMHDR h = (NMHDR)Marshal.PtrToStructure(msg.LParam, typeof(NMHDR));
                    if (h.code == HDN_BEGINDRAG ||
                        h.code == HDN_ITEMCHANGINGA ||
                        h.code == HDN_ITEMCHANGINGW)
                        EndEditing(false);
                    break;
            }

            base.WndProc(ref msg);
        }


        #region Initialize editing depending of DoubleClickActivation property
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
            base.OnMouseUp(e);

            if (DoubleClickActivation) {
                return;
            }

            EditSubitemAt(new Point(e.X, e.Y));
        }

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);

            if (!DoubleClickActivation) {
                return;
            }

            Point pt = this.PointToClient(Cursor.Position);

            EditSubitemAt(pt);
        }

        ///<summary>
        /// Fire SubItemClicked
        ///</summary>
        ///<param name="p">Point of click/doubleclick</param>
        private void EditSubitemAt(Point p) {
            ListViewItem item;
            int idx = GetSubItemAt(p.X, p.Y, out item);
            if (idx >= 0) {
                OnSubItemClicked(new SubItemEventArgs(item, idx));
            }
        }

        #endregion

        #region In-place editing functions
        // The control performing the actual editing
        private Control _editingControl;
        // The LVI being edited
        private ListViewItem _editItem;
        // The SubItem being edited
        private int _editSubItem;

        protected void OnSubItemBeginEditing(SubItemEventArgs e) {
            if (SubItemBeginEditing != null)
                SubItemBeginEditing(this, e);
        }
        protected void OnSubItemEndEditing(SubItemEndEditingEventArgs e) {
            if (SubItemEndEditing != null)
                SubItemEndEditing(this, e);
        }
        protected void OnSubItemClicked(SubItemEventArgs e) {
            if (SubItemClicked != null)
                SubItemClicked(this, e);
        }


        /// <summary>
        /// Begin in-place editing of given cell
        /// </summary>
        /// <param name="c">Control used as cell editor</param>
        /// <param name="Item">ListViewItem to edit</param>
        /// <param name="SubItem">SubItem index to edit</param>
        public void StartEditing(Control c, ListViewItem Item, int SubItem) {
            OnSubItemBeginEditing(new SubItemEventArgs(Item, SubItem));

            Rectangle rcSubItem = GetSubItemBounds(Item, SubItem);

            if (rcSubItem.X < 0) {
                // Left edge of SubItem not visible - adjust rectangle position and width
                rcSubItem.Width += rcSubItem.X;
                rcSubItem.X = 0;
            }
            if (rcSubItem.X + rcSubItem.Width > this.Width) {
                // Right edge of SubItem not visible - adjust rectangle width
                rcSubItem.Width = this.Width - rcSubItem.Left;
            }

            // Subitem bounds are relative to the location of the ListView!
            rcSubItem.Offset(Left, Top);

            // In case the editing control and the listview are on different parents,
            // account for different origins
            Point origin = new Point(0, 0);
            Point lvOrigin = this.Parent.PointToScreen(origin);
            Point ctlOrigin = c.Parent.PointToScreen(origin);

            rcSubItem.Offset(lvOrigin.X - ctlOrigin.X, lvOrigin.Y - ctlOrigin.Y);

            // Position and show editor
            c.Bounds = rcSubItem;
            c.Text = Item.SubItems[SubItem].Text;
            c.Visible = true;
            c.BringToFront();
            c.Focus();

            _editingControl = c;
            _editingControl.Leave += new EventHandler(_editControl_Leave);
            _editingControl.KeyPress += new KeyPressEventHandler(_editControl_KeyPress);

            _editItem = Item;
            _editSubItem = SubItem;
        }


        private void _editControl_Leave(object sender, EventArgs e) {
            // cell editor losing focus
            EndEditing(true);
        }

        private void _editControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char)(int)Keys.Escape: {
                        EndEditing(false);
                        break;
                    }

                case (char)(int)Keys.Enter: {
                        EndEditing(true);
                        break;
                    }
            }
        }

        /// <summary>
        /// Accept or discard current value of cell editor control
        /// </summary>
        /// <param name="AcceptChanges">Use the _editingControl's Text as new SubItem text or discard changes?</param>
        public void EndEditing(bool AcceptChanges) {
            if (_editingControl == null)
                return;

            SubItemEndEditingEventArgs e = new SubItemEndEditingEventArgs(
                _editItem,		// The item being edited
                _editSubItem,	// The subitem index being edited
                AcceptChanges ?
                    _editingControl.Text :	// Use editControl text if changes are accepted
                    _editItem.SubItems[_editSubItem].Text,	// or the original subitem's text, if changes are discarded
                !AcceptChanges	// Cancel?
            );

            OnSubItemEndEditing(e);

            _editItem.SubItems[_editSubItem].Text = e.DisplayText;

            _editingControl.Leave -= new EventHandler(_editControl_Leave);
            _editingControl.KeyPress -= new KeyPressEventHandler(_editControl_KeyPress);

            _editingControl.Visible = false;

            _editingControl = null;
            _editItem = null;
            _editSubItem = -1;
        }
        #endregion
    }
    /// <summary>
    /// 进度条/链接
    /// </summary>
    public class ListViewExp : ListViewEx
    {
        private Color _rowBackColor1 = Color.White;
        private Color _rowBackColor2 = Color.FromArgb(254, 216, 249);
        private Color _selectedColor = Color.FromArgb(166, 222, 255);
        private Color _headColor = Color.FromArgb(166, 222, 255);

        public ListViewExp()
            : base() {
            base.OwnerDraw = true;
            //base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [DefaultValue(typeof(Color), "White")]
        public Color RowBackColor1 {
            get {
                return _rowBackColor1;
            }
            set {
                _rowBackColor1 = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "254, 216, 249")]
        public Color RowBackColor2 {
            get {
                return _rowBackColor2;
            }
            set {
                _rowBackColor2 = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "166, 222, 255")]
        public Color SelectedColor {
            get {
                return _selectedColor;
            }
            set {
                _selectedColor = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "166, 222, 255")]
        public Color HeadColor {
            get {
                return _headColor;
            }
            set {
                _headColor = value;
                base.Invalidate();
            }
        }

        protected override void OnDrawColumnHeader(
            DrawListViewColumnHeaderEventArgs e) {
            base.OnDrawColumnHeader(e);

            Graphics g = e.Graphics;
            Rectangle bounds = e.Bounds;

            //Color baseColor = _headColor;
            //Color borderColor = _headColor;
            //Color innerBorderColor = Color.FromArgb(200, 255, 255);

            //RenderBackgroundInternal(
            //    g,
            //    bounds,
            //    baseColor,
            //    borderColor,
            //    innerBorderColor,
            //    0.35f,
            //    true,
            //    LinearGradientMode.Vertical);

            TextFormatFlags flags = GetFormatFlags(e.Header.TextAlign);
            Rectangle textRect = new Rectangle(
                       bounds.X + 3,
                       bounds.Y,
                       bounds.Width - 6,
                       bounds.Height);
            ;

            //if (e.Header.ImageList != null) {
            //    Image image = e.Header.ImageIndex == -1 ?
            //        null : e.Header.ImageList.Images[e.Header.ImageIndex];
            //    if (image != null) {
            //        Rectangle imageRect = new Rectangle(
            //            bounds.X + 3,
            //            bounds.Y + 2,
            //            bounds.Height - 4,
            //            bounds.Height - 4);
            //        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            //        g.DrawImage(image, imageRect);

            //        textRect.X = imageRect.Right + 3;
            //        textRect.Width -= imageRect.Width;
            //    }
            //}
            //
            e.DrawBackground();
            System.Windows.Forms.GroupBoxRenderer.DrawGroupBox(g, bounds, System.Windows.Forms.VisualStyles.GroupBoxState.Normal);
            //System.Windows.Forms.ButtonRenderer.DrawButton(g, bounds, System.Windows.Forms.VisualStyles.PushButtonState.Default);
            //
            TextRenderer.DrawText(
                   g,
                   e.Header.Text,
                   e.Font,
                   textRect,
                   e.ForeColor,
                   flags);
        }

        protected override void OnPaint(PaintEventArgs e) {
            //e.Graphics.Clear(this.BackColor);//清除显示类容
            //System.Windows.Forms.GroupBoxRenderer.DrawGroupBox(e.Graphics, e.ClipRectangle, System.Windows.Forms.VisualStyles.GroupBoxState.Normal);
            base.OnPaint(e);
        }
        protected override void OnDrawItem(DrawListViewItemEventArgs e) {
            //int x=  Cursor.Current.HotSpot.X;
            //int y = Cursor.Current.HotSpot.Y;
            //Point p= this.PointToClient(new Point(x, y));
            // if (e.Bounds.Contains(p)) {
            //     int i = 0;
            // }

            base.OnDrawItem(e);
            if (View != View.Details) {
                e.DrawDefault = true;
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e) {
            base.OnDrawSubItem(e);
            if (View != View.Details) {
                return;
            }
            if (e.ItemIndex == -1) {
                return;
            }
            Rectangle bounds = e.Bounds;
            ListViewItemStates itemState = e.ItemState;
            Graphics g = e.Graphics;
            //if ((itemState & ListViewItemStates.Selected)
            //    == ListViewItemStates.Selected) {
            if (e.Item.Selected) {
                //e.DrawFocusRectangle(e.Bounds);
                //e.DrawBackground();
                //e.Graphics.FillRectangle(SystemBrushes.WindowFrame, e.Bounds);
                //bounds.Height--;
                Color baseColor = _selectedColor;
                Color borderColor = _selectedColor;
                Color innerBorderColor = Color.FromArgb(200, 255, 255);

                RenderBackgroundInternal(
                    g,
                    bounds,
                    baseColor,
                    borderColor,
                    innerBorderColor,
                    0.35f,
                    true,
                    LinearGradientMode.Vertical);
                //bounds.Height++;
            }
            else {
                Color backColor = e.ItemIndex % 2 == 0 ?
                _rowBackColor1 : _rowBackColor2;

                using (SolidBrush brush = new SolidBrush(backColor)) {
                    g.FillRectangle(brush, bounds);
                }
            }
            TextFormatFlags flags = GetFormatFlags(e.Header.TextAlign);

            if (e.ColumnIndex == 0) {
                //g.FillRectangle(SystemBrushes.Info, bounds);
                if (e.Item.ImageList == null) {
                    e.DrawText(flags);
                    return;
                }
                Image image = e.Item.ImageIndex == -1 ?
                    null : e.Item.ImageList.Images[e.Item.ImageIndex];
                if (image == null) {
                    e.DrawText(flags);
                    return;
                }
                Rectangle imageRect = new Rectangle(
                    bounds.X + 4,
                    bounds.Y + 2,
                    bounds.Height - 4,
                    bounds.Height - 4);
                g.DrawImage(image, imageRect);

                Rectangle textRect = new Rectangle(
                    imageRect.Right + 3,
                    bounds.Y,
                    bounds.Width - imageRect.Right - 3,
                    bounds.Height);
                TextRenderer.DrawText(
                    g,
                    e.Item.Text,
                    e.Item.Font,
                    textRect,
                    e.Item.ForeColor,
                    flags);
                return;
            }
            //绘制进度条
            if (e.Header.Text == mProgressColumnText) {
                if (CheckIsFloat(e.SubItem.Text)) {
                    float tmpf = float.Parse(e.SubItem.Text);
                    if (tmpf >= 1.0f) {
                        tmpf = tmpf / 100.0f;
                    }
                    DrawProgress(
                        new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 6),
                        tmpf, e.Graphics, e.Item.Selected);
                }
            }
            else if (e.Header.Text == mLinkColumnText) {
                //绘制可链接文本
                e.DrawText(flags);
                Rectangle rect = e.Bounds;
                Size size = rect.Size;
                //SizeF sf= e.Graphics.MeasureString("复制下载地址",this.Font);
                g.DrawLine(System.Drawing.Pens.Blue, rect.X + 5, rect.Y + size.Height * 3 / 4, rect.X + 90, rect.Y + size.Height * 3 / 4);
            }
            else {
                bool isimage = false;
                System.Drawing.Image btnImg = null;
                if (mBtnImgs != null && mBtnImgs.ContainsKey(e.Header.Text)) {
                    isimage = true;
                    btnImg = mBtnImgs[e.Header.Text];
                }
                Rectangle tmpRect = e.Bounds;
                if (isimage && btnImg != null) {
                    bool over = tmpRect.Contains(mMouseX, mMouseY);
                    DrawButtonImg(tmpRect, btnImg, e.Header.TextAlign, e.Graphics,
                    e.Item.Selected, over);
                }
                else {
                    e.DrawText(flags);
                }
            }
        }

        protected TextFormatFlags GetFormatFlags(HorizontalAlignment align) {
            TextFormatFlags flags =
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.VerticalCenter;

            switch (align) {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            return flags;
        }

        internal void RenderBackgroundInternal(
          Graphics g,
          Rectangle rect,
          Color baseColor,
          Color borderColor,
          Color innerBorderColor,
          float basePosition,
          bool drawBorder,
          LinearGradientMode mode) {
            if (drawBorder) {
                rect.Width--;
                rect.Height--;
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(
               rect, Color.Transparent, Color.Transparent, mode)) {
                Color[] colors = new Color[4];
                colors[0] = GetColor(baseColor, 0, 35, 24, 9);
                colors[1] = GetColor(baseColor, 0, 13, 8, 3);
                colors[2] = baseColor;
                colors[3] = GetColor(baseColor, 0, 68, 69, 54);

                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0.0f, basePosition, basePosition + 0.05f, 1.0f };
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            if (baseColor.A > 80) {
                Rectangle rectTop = rect;
                if (mode == LinearGradientMode.Vertical) {
                    rectTop.Height = (int)(rectTop.Height * basePosition);
                }
                else {
                    rectTop.Width = (int)(rect.Width * basePosition);
                }

                using (SolidBrush brushAlpha =
                    new SolidBrush(Color.FromArgb(80, 255, 255, 255))) {
                    g.FillRectangle(brushAlpha, rectTop);
                }
            }

            if (drawBorder) {
                using (Pen pen = new Pen(borderColor)) {
                    g.DrawRectangle(pen, rect);
                }

                rect.Inflate(-1, -1);
                using (Pen pen = new Pen(innerBorderColor)) {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private Color GetColor(Color colorBase, int a, int r, int g, int b) {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;

            if (a + a0 > 255) {
                a = 255;
            }
            else {
                a = a + a0;
            }
            if (r + r0 > 255) {
                r = 255;
            }
            else {
                r = r + r0;
            }
            if (g + g0 > 255) {
                g = 255;
            }
            else {
                g = g + g0;
            }
            if (b + b0 > 255) {
                b = 255;
            }
            else {
                b = b + b0;
            }

            return Color.FromArgb(a, r, g, b);
        }
        #region 组件设计器生成的代码
        //const int  WM_NCPAINT = 0x0085;
        //const int WM_WINDOWPOSCHANGED = 0x0047;
        //protected override void WndProc(ref Message m) {
        //    switch (m.Msg) {
        //        case (int)WM_NCPAINT:
        //            //WmNcPaint(ref m);
        //            break;
        //        case (int)WM_WINDOWPOSCHANGED:
        //            base.WndProc(ref m);
        //            //IntPtr result = m.Result;
        //            //WmNcPaint(ref m);
        //            //m.Result = result;
        //            break;
        //        default:
        //            base.WndProc(ref m);
        //            break;
        //    }
        //}
        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e) {
            base.OnColumnWidthChanging(e);
            return;
            //
            e.Cancel = true;
            if (e.NewWidth < 60) {
                e.NewWidth = 60;
            }
            e.NewWidth = this.Columns[e.ColumnIndex].Width;
        }
        //protected override void OnColumnReordered(ColumnReorderedEventArgs e) {
        //    e.Cancel = true;
        //}

        #endregion

        #region 进度条
        private String mProgressColumnText = "progress";//String.Empty;

        /// <summary>
        /// 需要设置为进度条的列标头文字,根据这些文字判断所在列是否为进度条显示列
        /// </summary>
        internal void SetProgressColumnText(string ProgressColumn) {
            mProgressColumnText = ProgressColumn;
        }
        private Color mProgressColor = Color.Red;
        public Color ProgressColor {
            get {
                return this.mProgressColor;
            }
            set {
                this.mProgressColor = value;
            }
        }
        private Color mProgressTextColor = Color.Black;
        public Color ProgressTextColor {
            get {
                return mProgressTextColor;
            }
            set {
                mProgressTextColor = value;
            }
        }

        const string numberstring = "0123456789.";
        private bool CheckIsFloat(String s) {
            foreach (char c in s) {
                if (numberstring.IndexOf(c) > -1) {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }
        #endregion
        int mMouseX, mMouseY;
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            mMouseX = e.X;
            mMouseY = e.Y;
            //return;
            //ListViewItem item=this.GetItemAt(mMouseX, mMouseY);
            ListViewHitTestInfo hinfo = this.HitTest(e.X, e.Y);
            if (hinfo.SubItem != null) {
                //this.Cursor = Cursors.Hand;
                //Rectangle rec = hinfo.Item.SubItems[0].Bounds;
                //Rectangle reca=hinfo.Item.Bounds;
                //rec = new Rectangle(rec.X + rec.Width, rec.Y, reca.Width - rec.Width, rec.Height);
                this.Invalidate(hinfo.Item.Bounds);
            }
            else {
                //this.Cursor = Cursors.Arrow;
            }
        }


        Dictionary<string, System.Drawing.Image> mBtnImgs;
        /// <summary>
        /// 列名称 背景图
        /// </summary>
        /// <param name="btnImgs"></param>
        internal void SetButtonImgs(Dictionary<string, System.Drawing.Image> btnImgs) {
            mBtnImgs = btnImgs;

        }

        string mLinkColumnText="link";
        internal void SetLinkColumnText(string LinkColumnColumn) {
            mLinkColumnText = LinkColumnColumn;
        }
        private void DrawButtonImg(Rectangle rect, System.Drawing.Image s, HorizontalAlignment aglin, Graphics g, bool isSelect, bool mouseover) {
            if (rect.Height < 2 || rect.Width < 2) {
                return;
            }
            //StringFormat sf = new StringFormat();
            //switch (aglin) {
            //    case HorizontalAlignment.Center:
            //        sf.Alignment = StringAlignment.Center;
            //        break;
            //    case HorizontalAlignment.Left:
            //        sf.Alignment = StringAlignment.Near;
            //        break;
            //    case HorizontalAlignment.Right:
            //        sf.Alignment = StringAlignment.Far;
            //        break;
            //}
            //sf.LineAlignment = StringAlignment.Center;
            //sf.Trimming = StringTrimming.EllipsisCharacter;
            int mImgWidth = s.Width / 3 > rect.Width ? rect.Width : s.Width / 3;
            int mImgHeight = s.Height > rect.Height ? rect.Height : s.Height;
            if (!isSelect) {
                //g.DrawString(s, this.Font, Brushes.Black, rect, sf);
                if (mouseover) {
                    this.Cursor = Cursors.Hand;
                    Rectangle imgRec = new Rectangle(s.Width * 1 / 3, 0, s.Width / 3, s.Height);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                }
                else {
                    Rectangle imgRec = new Rectangle(s.Width * 0 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                }
            }
            else {
                //g.FillRectangle(Brushes.RoyalBlue, rect);
                //g.DrawString(s, this.Font, Brushes.White, rect, sf);
                if (mouseover) {
                    this.Cursor = Cursors.Hand;
                    Rectangle imgRec = new Rectangle(s.Width * 1 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);

                }
                else {

                    Rectangle imgRec = new Rectangle(s.Width * 2 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                }
            }

        }
        //System.Drawing.Color mLinkColor = System.Drawing.Color.Blue;
        //System.Drawing.Color mLinkColorMouseOver= System.Drawing.Color.Red;

        private void DrawLinkString(Rectangle rect, string s, HorizontalAlignment aglin, Graphics g, bool isSelect, bool mouseover) {
            StringFormat sf = new StringFormat();
            switch (aglin) {
                case HorizontalAlignment.Center:
                    sf.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Right:
                    sf.Alignment = StringAlignment.Far;
                    break;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            if (!isSelect) {
                g.DrawString(s, this.Font, Brushes.Blue, rect, sf);
                if (mouseover) {
                    this.Cursor = Cursors.Hand;
                    SizeF size = g.MeasureString(s, this.Font);
                    g.DrawLine(System.Drawing.Pens.Blue, rect.X, rect.Y + size.Height, rect.X + size.Width, rect.Y + size.Height);
                }
                else {
                    //this.Cursor = Cursors.Arrow;
                }
            }
            else {
                g.FillRectangle(Brushes.RoyalBlue, rect);
                g.DrawString(s, this.Font, Brushes.White, rect, sf);
                if (mouseover) {
                    SizeF size = g.MeasureString(s, this.Font);
                    g.DrawLine(System.Drawing.Pens.White, rect.X, rect.Y + size.Height, rect.Right, rect.Y + size.Height);

                }
                else {
                    //this.Cursor = Cursors.Arrow;
                }
            }

        }
        //绘制非进度条列的subitem
        private void DrawString(Rectangle rect, String s, HorizontalAlignment aglin, Graphics g, bool isSelect) {
            StringFormat sf = new StringFormat();
            switch (aglin) {
                case HorizontalAlignment.Center:
                    sf.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Right:
                    sf.Alignment = StringAlignment.Far;
                    break;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            if (!isSelect)
                g.DrawString(s, this.Font, Brushes.Black, rect, sf);
            else {
                g.FillRectangle(Brushes.RoyalBlue, rect);
                g.DrawString(s, this.Font, Brushes.White, rect, sf);
            }
        }

        ///绘制进度条列的subitem
        private void DrawProgress(Rectangle rect, float percent, Graphics g, bool isSelect) {
            if (rect.Height > 2 && rect.Width > 2) {
                if (rect.Top > 0 && rect.Top < this.Height &&
                 (rect.Left > this.Left && rect.Left < this.Width)) {
                    //绘制进度
                    int width = (int)(rect.Width * percent);
                    Rectangle newRect = new Rectangle(rect.Left + 1, rect.Top + 1, width - 2, rect.Height - 2);
                    using (Brush tmpb = new SolidBrush(this.mProgressColor)) {
                        g.FillRectangle(tmpb, newRect);
                    }
                    if (!isSelect)
                        g.DrawRectangle(Pens.Yellow, rect);
                    else {
                        newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                        g.DrawRectangle(Pens.RoyalBlue, newRect);
                    }
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                    using (Brush b = new SolidBrush(mProgressTextColor)) {
                        g.DrawString(percent.ToString("p1"), this.Font, b, newRect, sf);
                    }
                }
            }
            else
                return;
        }



        //private void ListViewEx_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
        //    e.Graphics.Clear(this.BackColor);//清除显示类容

        //    int progressIndex = -1;
        //    //检索被设置为进度条的列
        //    if (progressIndex == -1 && !string.IsNullOrEmpty(mProgressColumnText)) {
        //        for (int i = 0; i < this.Columns.Count; i++) {
        //            if (this.Columns[i].Text == mProgressColumnText) {
        //                progressIndex = i; break;
        //            }
        //        }
        //    }
        //    int linIndex = -1;
        //    if (linIndex == -1 && !string.IsNullOrEmpty(mLinkColumnText)) {
        //        for (int i = 0; i < this.Columns.Count; i++) {
        //            if (this.Columns[i].Text == mLinkColumnText) {
        //                linIndex = i; break;
        //            }
        //        }
        //    }
        //    //依次绘制每一行
        //    for (int i = 0; i < this.Items.Count; i++) {
        //        Rectangle rect = this.GetItemRect(i);//获取当前要绘制行的Rect
        //        if (rect.Top < 0 || rect.Top >= this.ClientRectangle.Height)
        //            continue; //不在显示范围内,跳过

        //        if (rect != Rectangle.Empty) {
        //            int totalwidth = 0; //列宽记数,实际上就是当前要绘制列的X坐标
        //            for (int j = 0; j < this.Columns.Count; j++) {
        //                int currwidth = this.Columns[j].Width;//获取当前列宽度
        //                Rectangle tmpRect = new Rectangle(totalwidth, rect.Top, currwidth, rect.Height);//生成当前subitem的外接矩形
        //                if (j == linIndex) {
        //                    //非进度条列
        //                    bool over = tmpRect.Contains(mMouseX, mMouseY);
        //                    DrawLinkString(tmpRect, this.Items[i].SubItems[j].Text, this.Columns[j].TextAlign, e.Graphics,
        //                    this.Items[i].Selected, over);
        //                }
        //                else if (j == progressIndex) {
        //                    //进度条列
        //                    //判断当前subitem文本是否可以转为浮点数
        //                    if (CheckIsFloat(this.Items[i].SubItems[j].Text)) {
        //                        float tmpf = float.Parse(this.Items[i].SubItems[j].Text);
        //                        if (tmpf >= 1.0f) {
        //                            tmpf = tmpf / 100.0f;
        //                        }
        //                        DrawProgress(tmpRect, tmpf, e.Graphics,
        //                         this.Items[i].Selected);
        //                    }
        //                }
        //                else {
        //                    bool isimage = false;
        //                    System.Drawing.Image btnImg = null;
        //                    if (mBtnImgs != null && mBtnImgs.ContainsKey(this.Columns[j].Text)) {
        //                        isimage = true;
        //                        btnImg = mBtnImgs[this.Columns[j].Text];
        //                    }
        //                    if (isimage && btnImg != null) {
        //                        bool over = tmpRect.Contains(mMouseX, mMouseY);
        //                        DrawButtonImg(tmpRect, btnImg, this.Columns[j].TextAlign, e.Graphics,
        //                        this.Items[i].Selected, over);
        //                    }
        //                    else {
        //                        DrawString(tmpRect, this.Items[i].SubItems[j].Text, this.Columns[j].TextAlign, e.Graphics,
        //                        this.Items[i].Selected);
        //                    }
        //                }
        //                totalwidth += this.Columns[j].Width;

        //            }

        //        }

        //    }

        //}
    }
}
