using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// 支持分组的自定义ListView.
    /// 目前只只吃Table方式的布局.RibbonImageLayout=Table
    /// </summary>
    public class RibbonListView : UserControl, RibbonUISkin
    {
        #region
        /// <summary>
        /// 所有项
        /// </summary>
        private List<RibbonListViewItem> m_AllItmes = new List<RibbonListViewItem>();
        /// <summary>
        /// 经过过滤的项
        /// </summary>
        private List<RibbonListViewItem> m_Items = new List<RibbonListViewItem>();
        /// <summary>
        /// 按组分的所有项
        /// </summary>
        private Dictionary<int, List<RibbonListViewItem>> m_GroupedItems = new Dictionary<int, List<RibbonListViewItem>>();

        //有内容的情况下的实际宽度
        private int m_RealWidth;
        //有内容的情况下的实际高度
        private int m_RealHeight;
        //左上角在全内容实际大小区域中的相对位置,
        private Point m_CurPos;
        private List<RibbonListViewItem> m_VisibleItem = new List<RibbonListViewItem>();

        private RibbonScrollbar m_HScroll;

        internal Graphics m_Graphics;
        public RibbonListView() : this("RibbonListView") {

        }
        public RibbonListView(string name) {
            this.Name = name;
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            IsDrawBoard = false;
            RibbonImageLayout = RibbonImageLayout.Table;

            m_VScroll = new RibbonScrollbar();
            m_VScroll.TopImage = global::RS.UtilityLib.WinFormCommon.Properties.Resources.滚动条;//UI_Res.滚动条;
            try {
                Image img = global::RS.UtilityLib.WinFormCommon.Properties.Resources.ListScrollBar;//RibbonUIRes.GetImage("Skin\\Res\\ListScrollBar.bmp");
                if (img != null) {
                    m_VScroll.TopImage = img;
                }
            }
            catch {
            }
            m_VScroll.Scroll += new ScrollEventHandler(m_Scroll_Scroll);
            m_VScroll.Visible = false;
            this.Controls.Add(m_VScroll);
            m_VScroll.Dock = DockStyle.Right;
            m_VScroll.SmallChange = this.RowHeight;
            m_VScroll.LargeChange = this.RowHeight * 3;
            m_ToolTip = new ToolTip();
            m_ToolTip.InitialDelay = 1000;
            m_ToolTip.AutomaticDelay = 1000;
            m_ToolTip.AutoPopDelay = 5000;
            //m_HScroll = new RibbonScrollbar();
            //m_HScroll.Scroll += new ScrollEventHandler(m_Scroll_Scroll);
            //this.Controls.Add(m_HScroll);
            //m_HScroll.Dock = DockStyle.Bottom;

            m_Graphics = CreateGraphics();

            IsToLagerHoverItem = true;
            //
            m_ItemRender = new RibbonListViewItemRender(this);

        }

        void m_Scroll_Scroll(object sender, ScrollEventArgs e) {
            //System.Diagnostics.Debug.WriteLine(e.NewValue);
            RibbonScrollbar sb = sender as RibbonScrollbar;
            if (sb.Orientation == ScrollOrientation.VerticalScroll) {
                m_CurPos.Y = e.NewValue;
            }
            else {
                m_CurPos.X = e.NewValue;
            }
            //IRobotQ.CallTime.Enable = true;
            //IRobotQ.CallTime.Start(".....");
            m_VisibleItem = GetVisibleItems();
            //IRobotQ.CallTime.End();
            //IRobotQ.CallTime.Start(".....1");
            this.Invalidate();
            //IRobotQ.CallTime.End();
        }


        /// <summary>
        /// ImageList的方式组织项目图像。适用于重复项目中图标重复性高的场合。
        /// </summary>
        public ImageList ImageList {
            get;
            set;
        }
        /// <summary>
        /// 图标显示布局
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public RibbonImageLayout RibbonImageLayout {
            get;
            set;
        }


        public int DockFlag {
            get;
            set;
        }

        /// <summary>
        /// 图像是否拉伸.暂时不用
        /// </summary>
        public ImageLayout ImageShowFormat {
            get;
            set;
        }

        /// <summary>
        /// ImageShowFormat.Scretch模式下图标尺寸,暂时不用
        /// </summary>
        public Point ImageSize {
            get;
            set;
        }
        private bool m_ItemAutoSize = false;
        /// <summary>
        /// Item文本如果超出显示区域，是否自动换行,暂时只支持2行，默认false
        /// </summary>
        public bool ItemAutoSize {
            get {
                return m_ItemAutoSize;
            }
            set {
                m_ItemAutoSize = value;
            }
        }

        //private int m_ItemTextMaxShowCount = int.MaxValue;
        ///// <summary>
        ///// Item文本最多显示几个字，如果超出，如果ItemAutoSize为True，则自动换行，默认为int.MaxValue，表示无限制
        ///// </summary>
        //public int ItemTextMaxShowCount {
        //    get {
        //        return m_ItemTextMaxShowCount;
        //    }
        //    set {
        //        m_ItemTextMaxShowCount = value;
        //    }
        //}

        private int m_ColumnCount = 1;
        /// <summary>
        /// RibbonImageLayout.Table模式下,显示列数
        /// </summary>
        [Browsable(true)]
        [Description("RibbonImageLayout.Table模式下,显示列数")]
        public int ColumnCount {
            get {
                return m_ColumnCount;
            }

            set {
                m_ColumnCount = value;
            }
        }
        private int m_RowHeight = 32;
        /// <summary>
        /// 指定行高度
        /// </summary>
        [Browsable(true)]
        [Description("指定行高度")]
        public int RowHeight {
            get {
                return m_RowHeight;
            }
            set {
                m_RowHeight = value;
                m_VScroll.SmallChange = value;
                m_VScroll.LargeChange = value * 6;
                ReComputerItemPosAndSize(this, m_Items, m_CurPos);
            }
        }
        private int m_RowSp = 3;
        private int m_ColumnSp = 3;
        [Browsable(true)]
        [Description("指定行间隔")]
        public int RowSp {
            get {
                return m_RowSp;
            }
            set {
                m_RowSp = value;
                ReComputerItemPosAndSize(this, m_Items, m_CurPos);
            }
        }
        [Browsable(true)]
        [Description("指定列间隔")]
        public int ColumnSp {
            get {
                return m_ColumnSp;
            }
            set {
                m_ColumnSp = value;
                ReComputerItemPosAndSize(this, m_Items, m_CurPos);
            }
        }
        /// <summary>
        /// 左上角在全内容实际大小区域中的相对位置,
        /// </summary>
        public Point CurPosition {
            get {
                return m_CurPos;
            }
            set {
                m_CurPos = value;
                m_VisibleItem.Clear();
                m_VisibleItem = GetVisibleItems();
            }
        }


        [Browsable(true)]
        [Description("允许多选")]
        public bool MulitSelect {
            get;
            set;
        }
        [Browsable(true)]
        [Description("是否绘制边框")]
        public bool IsDrawBoard {
            get;
            set;
        }
        private Image m_SelectedMask;
        [Browsable(true)]
        [Description("选中Item后的效果图")]
        public Image SelectedMask {
            get {
                return m_SelectedMask;
            }
            set {
                m_SelectedMask = value;
            }
        }

        ///// <summary>
        ///// 是否显示Hint
        ///// </summary>
        //[Browsable(true)]
        //[Description("是否显示Hint")]
        //public bool IsShowHint {
        //    get;
        //    set;
        //}
        /// <summary>
        /// 显示Hint的方式.
        /// </summary>
        public enum ShowHintType
        {
            /// <summary>
            /// 不显示
            /// </summary>
            None = 0,
            OnlyEnableItem = 2,
            OnlyUnenableItem = 4,
            All = 6,
        }
        private ShowHintType m_ShowHintType = ShowHintType.None;
        /// <summary>
        /// 显示Hint的方式
        /// </summary>
        [Browsable(true)]
        [Description("显示Hint的方式")]
        public ShowHintType @HintType {
            get {
                return m_ShowHintType;
            }
            set {
                m_ShowHintType = value;
            }
        }
        //private bool m_ShowHintWhenNotEnable = false;

        ///// <summary>
        ///// 当Item的Enable为false时是否显示Hint
        ///// </summary>
        //[Browsable(true)]
        //[Description("当Item的Enable为false时是否显示Hint")]
        //public bool IsShowHintWhenNotEnable {
        //    get {
        //        return m_ShowHintWhenNotEnable;
        //    }
        //    set {
        //        m_ShowHintWhenNotEnable = value;
        //    }
        //}
        private bool m_IsShowToolTip = false;
        /// <summary>
        /// 是否为Item显示ToolTip
        /// </summary>
        [Browsable(true)]
        [Description("是否为Item显示ToolTip")]
        public bool IsShowToolTip {
            get {
                return m_IsShowToolTip;
            }
            set {
                m_IsShowToolTip = value;
            }
        }
        private bool m_IsShowItemText = false;
        /// <summary>
        /// 是否显示Item文本
        /// </summary>
        public bool IsShowItemText {
            get {
                return m_IsShowItemText;
            }
            set {
                if (m_IsShowItemText != value) {
                    m_IsShowItemText = value;
                    foreach (var v in m_Items) {
                        v.UpdateSize();
                    }
                    UpdateItems();
                }

            }
        }

        private ToolTip m_ToolTip;
        public ToolTip ItemToolTip {
            get {
                return m_ToolTip;
            }
        }
        private void SetToolTip(Control control, string tip) {
            m_ToolTip.SetToolTip(
                control,
                 tip);
        }
        private void ResetToolTip() {
            m_ToolTip.Active = false;
            //m_ToolTip.Opacity = 1D;
            //m_ToolTip.ToolTipImageSize = new Size(16, 16);
            //m_ToolTip.ToolTipImage = null;
            m_ToolTip.ToolTipTitle = "";
        }

        private bool m_IsShowHover;
        /// <summary>
        /// 鼠标覆盖Item时,是否突出显示
        /// </summary>
        [Browsable(true)]
        [Description("鼠标覆盖时,是否突出显示")]
        public bool IsShowHover {
            get {
                return m_IsShowHover;
            }
            set {
                m_IsShowHover = value;
            }
        }
        private bool m_FixHinPosition = true;
        [Browsable(true)]
        [Description("是否固定Hint的显示位置")]
        public bool FixHintPostion {
            get {
                return m_FixHinPosition;
            }
            set {
                m_FixHinPosition = value;
            }
        }
        public RibbonListViewItemBoard ItemBoard {
            get;
            set;
        }

        private int m_LevelTab = 12;
        [Browsable(true)]
        [Description("树状形式（Group模式）时，Item的缩进量，单位是像素")]
        public int LevelTab {
            get {
                return m_LevelTab;
            }
            set {
                m_LevelTab = value;
            }
        }

        #endregion
        #region
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            if (this.m_hintWnd != null) {
                this.m_hintWnd.Hide();
            }

        }
        private RibbonScrollbar m_VScroll;// = new RibbonScrollbar();
        /// <summary>
        /// 设置滚动条效果图
        /// </summary>
        /// <param name="imgpath"></param>
        public void SetScrollBarImg(string imgpath) {
            if (m_VScroll != null) {
                m_VScroll.ImagePath = imgpath;
            }
        }
        internal bool m_InUpdate = false;
        private int m_InUpdateGroup = 0;
        /// <summary>
        /// 必须和EndUpdate配合使用
        /// </summary>
        public void BeginUpdate() {
            if (this.m_hintWnd != null) {
                this.m_hintWnd.Hide();
            }
            m_InUpdate = true;
            m_InUpdateGroup = 0;
        }
        /// <summary>
        /// 开始更新指定的组
        /// </summary>
        /// <param name="groupIndex"></param>
        public void BeginUpdate(int groupIndex) {
            if (Groups.Count == 0 || groupIndex >= Groups.Count) {
                throw new Exception("不存在组");
            }
            if (this.m_hintWnd != null) {
                this.m_hintWnd.Hide();
            }
            m_InUpdate = true;
            m_InUpdateGroup = groupIndex;
        }
        /// <summary>
        /// 执行完这个方法，外部必须更新UpdateScrollMaxinum( scrollbar)
        /// </summary>
        public void EndUpdate() {
            m_InUpdate = false;

            UpdateItems();
            UpdateScrollMaxinum();
            UpdateGroupRect();
            //ReComputerItemPosAndSize(this, m_Items, new Point(0, 0));
            m_VisibleItem = GetVisibleItems();
            this.Invalidate();
        }

        private void UpdateGroupRect() {
            foreach (var g in this.Groups) {
                g.Rect = new Rectangle(0, g.Rect.Y, this.GetClientRect().Width, g.Height);
            }
        }

        private void UpdateItems() {
            if (Groups.Count > 0) {
                //
                // int groupIndex = m_InUpdateGroup > 0 ? m_InUpdateGroup - 1 : m_InUpdateGroup;
                UpdateGroupRect(m_InUpdateGroup);
            }
            else {
                ReComputerItemPosAndSize(this, m_Items, new Point(0, 0));// m_CurPos);
            }
        }

        public Rectangle GetClientRect() {
            int x = this.Left;
            int y = this.Top;
            int w = m_VScroll.Visible ? this.Width - m_VScroll.Width : this.Width;
            int h;
            if (m_HScroll != null) {
                h = m_HScroll.Visible ? this.Height - m_HScroll.Height : this.Height;
            }
            else {
                h = this.Height;
            }
            Rectangle ret = new Rectangle(x, y, w, h);
            return ret;
        }

        /// <summary>
        /// 手动更新RibbonScrollbar。Maxinum
        /// </summary>
        /// <param name="m_VScroll"></param>
        private void UpdateScrollMaxinum() {
            if (m_VScroll == null) {
                return;
            }
            m_VScroll.Visible = false;
            if (RibbonImageLayout == RibbonImageLayout.Table) {
                int max = 0;
                if (Groups.Count > 0) {
                    //如果是分组方式,则使用Group的GetAllRect来算就ok了.  
                    foreach (var v in Groups) {
                        max += v.GetAllRect().Height;
                    }
                    //最后一屏就不用算在里面了。
                    max -= (this.Height - m_VScroll.LargeChange);
                    if (max > 0) {
                        m_VScroll.Visible = true;
                        m_VScroll.Maximum = max;
                        //foreach (var item in m_Items) {
                        //    item.UpdateSize();
                        //}
                        UpdateItems();
                    }
                    else {
                        m_VScroll.Maximum = 0;
                        //m_VScroll.Visible = false;
                        foreach (var item in m_Items) {
                            item.UpdateSize();
                        }
                        UpdateItems();
                    }
                }
                else {
                    int k = m_Items.Count % m_ColumnCount;

                    int rows = (int)((float)m_Items.Count / m_ColumnCount);
                    if (k > 0) {
                        rows++;
                    }
                    max = rows * (m_RowHeight + m_RowSp);
                    //最后一屏不用算在里面?
                    if (max > this.Height) {
                        max -= (this.Height - m_VScroll.LargeChange);
                        if (max > 0) {
                            m_VScroll.Visible = true;
                            m_VScroll.Maximum = max;// -this.Height;
                        }
                    }
                    else {
                        m_VScroll.Maximum = 1;
                        m_VScroll.Value = 0;
                        m_VScroll.Visible = false;

                    }
                }


            }
        }

        public void Sort(Comparison<RibbonListViewItem> compare) {
            BeginUpdate();
            if (Groups.Count == 0) {
                m_Items.Sort(compare);
            }
            else {
                foreach (var g in Groups) {
                    g.Items.Sort(compare);
                }
            }
            EndUpdate();
        }
        /// <summary>
        /// 增加一个item,如果当前存在Group,则将添加到第一个Group.
        /// 如果需要循环添加多个,请务必使用BeginUpdate
        /// </summary>
        /// <param name="item"></param>
        public void InsertItem(RibbonListViewItem item, int index) {
            if (m_InUpdate == false) {
                throw new Exception("请在AddItem前使用BeginUpdate,AddItem后使用EndUpdate");
            }
            m_Items.Insert(index, item);
            m_AllItmes.Insert(index, item);
            item.ListView = this;
            if (!m_InUpdate) {
                if (m_Items.Count > 1) {
                    RibbonListViewItem previtem = m_Items[m_Items.Count - 2];
                    ComputPosAndSize(previtem, item);
                    m_VisibleItem = GetVisibleItems();
                }
            }
        }
        /// <summary>
        /// 增加一个item,如果当前存在Group,则将添加到第一个Group.
        /// 如果需要循环添加多个,请务必使用BeginUpdate
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(RibbonListViewItem item) {
            if (m_InUpdate == false) {
                throw new Exception("请在AddItem前使用BeginUpdate,AddItem后使用EndUpdate");
            }
            m_Items.Add(item);
            m_AllItmes.Add(item);
            item.ListView = this;
            if (!m_InUpdate) {
                if (m_Items.Count > 1) {
                    RibbonListViewItem previtem = m_Items[m_Items.Count - 2];
                    ComputPosAndSize(previtem, item);
                    m_VisibleItem = GetVisibleItems();
                }
            }
        }
        /// <summary>
        /// 添加新项到指定的Group.如果需要循环添加多个,请务必使用BeginUpdate
        /// </summary>
        /// <param name="group"></param>
        /// <param name="item"></param>
        public void AddItem(RibbonListViewGroup group, RibbonListViewItem item) {
            m_Items.Add(item);
            m_AllItmes.Add(item);
            item.ListView = this;
            group.AddItem(item);
            m_GroupedItems[group.GetHashCode()].Add(item);

            if (!m_InUpdate) {
                if (group.Items.Count > 1) {
                    RibbonListViewItem previtem = m_Items[m_Items.Count - 2];
                    ComputPosAndSize(previtem, item);
                    m_VisibleItem = GetVisibleItems();
                }
            }
        }
        /// <summary>
        /// 移除一个item,如果需要循环移除多个,请首先使用BeginUpdate
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(RibbonListViewItem item) {
            int groupIndex = 0;
            if (item.Group != null) {
                groupIndex = item.Group.GroupIndex;
                m_GroupedItems[item.Group.GetHashCode()].Remove(item);
                item.Group.RemoveItem(item);

            }
            m_Items.Remove(item);
            m_AllItmes.Remove(item);

            item.Level = 0;
            if (!m_InUpdate) {
                ReComputerItemPosAndSize(this, m_Items, m_CurPos);
            }
            if (m_InUpdate == false) {
                UpdateGroupRect(groupIndex);
                UpdateScrollMaxinum();
                m_VisibleItem = GetVisibleItems();
                this.Invalidate();
            }
            item.ListView = null;
        }
        public void Clear() {
            Clear(true);
        }
        public void Clear(bool disposeGroup) {
            this.BeginUpdate();
            m_VisibleItem.Clear();
            int count = this.m_AllItmes.Count;
            for (int i = count - 1; i > 0; i--) {
                m_AllItmes[i].Dispose();
            }
            if (disposeGroup) {
                foreach (var g in Groups) {
                    g.Dispose();
                }
                Groups.Clear();
            }
            m_Items.Clear();
            m_SelectedItems.Clear();
            m_AllItmes.Clear();
            m_GroupedItems.Clear();
            m_CurPos.X = 0;
            m_CurPos.Y = 0;

            this.EndUpdate();
            m_VScroll.Value = 0;
        }

        public RibbonListViewGroup GetGroupHeaderAt(int x, int y) {
            int curX = m_CurPos.X + x;
            int curY = m_CurPos.Y + y;
            foreach (var g in Groups) {
                if (g.Rect.Contains(curX, curY)) {
                    return g;
                }
            }
            return null;
        }

        public RibbonListViewItem GetItemAt(int x, int y) {
            int curX = m_CurPos.X + x;
            int curY = m_CurPos.Y + y;
            foreach (var item in m_VisibleItem) {
                if (item.Visble == true && curX > item.X && curX < item.X + item.Width && curY >= item.Y && curY <= item.Y + item.Height) {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 已知item位置获取到该item对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public RibbonListViewItem GetItemByItemPos(int x, int y) {
            foreach (var item in this.m_Items) {
                if (new Rectangle(item.X, item.Y, item.Width, item.Height).Contains(x, y)) {
                    return item;
                }
            }
            return null;
        }
        private List<RibbonListViewItem> m_SelectedItems = new List<RibbonListViewItem>();
        public int SelectedItemCount {
            get {
                return m_SelectedItems.Count;
            }
        }
        public IEnumerable<RibbonListViewItem> GetSelectedItems() {
            foreach (var p in m_SelectedItems) {
                yield return p;
            }
        }

        //GotoSelectedItem操作时,将选中项的第一个定位到上数下第N个
        private const int GOTOSELECTITEM_CONST = 6;
        /// <summary>
        /// 定位到第一个选中的Item
        /// </summary>
        public void GotoSelectedItem() {
            if (this.SelectedItemCount <= 0) {
                return;
            }

            RibbonListViewItem item = this.m_SelectedItems[0];
            //item.Group.Expand();
            int index = this.m_Items.IndexOf(item);

            if (index < GOTOSELECTITEM_CONST) {
                m_CurPos.Y = 0;
            }
            else {
                m_CurPos.Y = this.m_Items[index - GOTOSELECTITEM_CONST].Top;
            }
            int m_curnet = this.m_VScroll.Value;
            // this.m_VScroll.Value = m_CurPos.Y;
            int d = m_CurPos.Y - m_curnet;
            this.m_VScroll.Move(ScrollEventType.ThumbPosition, Math.Sign(d), Math.Abs(d));
        }

        public void ReflashSelectItems() {
            m_SelectedItems.Clear();
            foreach (RibbonListViewItem r in this.m_Items) {
                if (r.Selected) {
                    m_SelectedItems.Add(r);
                }
            }
        }
        public RibbonListViewItem GetSelectedItem(int index) {
            if (index < 0 || index >= m_SelectedItems.Count) {
                return null;
            }
            return m_SelectedItems[index];
        }
        public IEnumerable<RibbonListViewItem> GetItems() {
            foreach (var p in m_Items) {
                yield return p;
            }
        }

        /// <summary>
        /// 只显示符合条件的item。
        /// itemText开头的显示在前。
        /// 包括itemText的，按照string的序号排在后面
        /// </summary>
        /// <param name="itemText"></param>
        public void Filter(string itemText) {
            this.BeginUpdate();
            m_Items.Clear();

            m_CurPos = new Point();
            //滚动条清零。
            m_VScroll.Value = 0;
            if (string.IsNullOrEmpty(itemText)) {
                m_Items.AddRange(m_AllItmes);
            }
            else {

                List<RibbonListViewItem> list1 = new List<RibbonListViewItem>();
                List<RibbonListViewItem> list2 = new List<RibbonListViewItem>();
                foreach (var v in m_AllItmes) {
                    int index = v.Text.IndexOf(itemText, StringComparison.OrdinalIgnoreCase);
                    if (index == 0) {
                        //开头
                        list1.Add(v);
                    }
                    else if (index > 0) {
                        //包含
                        list2.Add(v);
                    }
                    else if (index < 0) {
                        //试试汉字
                        var s = PinyinHelper.Pinyin.GetInitials(v.Text);
                        if (s.IndexOf(itemText, StringComparison.OrdinalIgnoreCase) >= 0) {
                            list2.Add(v);
                        }
                    }
                }
                list1.Sort((o1, o2) => o1.Text.CompareTo(o2.Text));
                list2.Sort((o1, o2) => o1.Text.CompareTo(o2.Text));
                m_Items.AddRange(list1);
                m_Items.AddRange(list2);
            }
            if (Groups.Count > 0) {
                foreach (var g in Groups) {
                    g.Items.Clear();
                }
                foreach (var v in m_Items) {
                    v.Group.Items.Add(v);
                }
            }
            this.EndUpdate();
        }

        internal void AddSelectItem(RibbonListViewItem item) {
            //item.Selected = true;
            if (!m_SelectedItems.Contains(item)) {
                m_SelectedItems.Add(item);
            }
        }
        internal void RemoveSelectItem(RibbonListViewItem item) {
            // if (!m_SelectedItems.Contains(item)) {
            m_SelectedItems.Remove(item);
            //  }
        }
        private RibbonHintWindow m_hintWnd;
        /// <summary>
        /// 提示窗口
        /// </summary>
        public RibbonHintWindow Hint {
            get {
                return m_hintWnd;
            }
            set {
                if (value != null) {

                }
                m_hintWnd = value;
            }
        }
        //protected override void OnMouseWheel(MouseEventArgs e) {

        //    int k = Win32Native.SendMessage(this.m_VScroll.Handle, 522, (int)Win32Native.MakeUInt32(0, (short)e.Delta), (int)Win32Native.MakeUInt32((short)e.X, (short)e.Y));
        //    base.OnMouseWheel(e);
        //}
        private RibbonListViewItem m_PreItemHit;
        private RibbonListViewItem m_HoverItem;
        [Description("鼠标所覆盖的Item")]
        public RibbonListViewItem HoverItem {
            get {
                return m_HoverItem;
            }
            set {
                m_HoverItem = value;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if (this.Enabled) {
                if (m_IsShowHover) {
                    if (m_HoverItem != null) {
                        m_HoverItem.Hovered = false;
                    }
                }
                m_HoverItem = GetItemAt(e.X, e.Y);
                ShowCustomHintWindow(e);

                if (m_HoverItem != null && m_HoverItem.Enabled == false) {
                    return;
                }
                if (m_IsShowToolTip) {
                    if (m_HoverItem != null) {
                        if (m_HoverItem != m_PreItemHit) {

                            m_ToolTip.Show(m_HoverItem.Memo, this, e.X + 16, e.Y + 16);
                        }
                    }
                    else {
                        m_ToolTip.Hide(this);
                    }
                }

                //ShowCustomHintWindow(e);

                if (m_PreItemHit != null) {
                    Rectangle prevRect = new Rectangle(m_PreItemHit.X - m_CurPos.X, m_PreItemHit.Y - m_CurPos.Y, m_PreItemHit.Width, m_PreItemHit.Height);
                    this.Invalidate(prevRect, false);
                }
                m_PreItemHit = m_HoverItem;
                if (m_IsShowHover && m_HoverItem != null) {
                    m_HoverItem.Hovered = true;
                    this.Invalidate(new Rectangle(m_HoverItem.X - m_CurPos.X, m_HoverItem.Y - m_CurPos.Y, m_HoverItem.Width, m_HoverItem.Height), false);
                }
                m_MousetXHover = e.X;
                m_MouseYHover = e.Y;
            }
            base.OnMouseMove(e);
        }

        private void ShowCustomHintWindow(MouseEventArgs e) {
            if (this.m_hintWnd != null && this.m_hintWnd.IsDisposed == false) {

                if (m_HoverItem != null) {
                    bool showHint = false;
                    if (m_HoverItem.Enabled && (m_ShowHintType & ShowHintType.OnlyEnableItem) == ShowHintType.OnlyEnableItem) {
                        showHint = true;
                    }
                    else if (m_HoverItem.Enabled == false && (m_ShowHintType & ShowHintType.OnlyUnenableItem) == ShowHintType.OnlyUnenableItem) {
                        showHint = true;
                    }
                    if (showHint == false) {
                        RibbonHintWindow.HideHint(this.m_hintWnd);
                        return;
                    }
                    if (m_HoverItem != m_PreItemHit) {
                        Point p;
                        if (m_FixHinPosition) {
                            p = new Point(this.Width, e.Y);
                            if (this.HitWndLoactionType == FixHitWndTypeLocation.Left) {
                                p = new Point(0 - this.Hint.Width, e.Y);
                            }
                            else if (this.HitWndLoactionType == FixHitWndTypeLocation.Right) {
                                p = new Point(this.Width, e.Y);
                            }
                            else if (this.HitWndLoactionType == FixHitWndTypeLocation.Top) {
                                p = new Point(e.X, 0 - this.Hint.Height);
                            }
                            else if (this.HitWndLoactionType == FixHitWndTypeLocation.Bottom) {
                                p = new Point(e.X, this.Height);
                            }
                        }
                        else {
                            p = new Point(e.X + m_HoverItem.Width, e.Y);
                        }
                        p = this.PointToScreen(p);
                        int screenHeight = Screen.FromHandle(this.Handle).WorkingArea.Height;
                        if (this.m_hintWnd.Size.Height + p.Y > screenHeight) {
                            p.Y = screenHeight - this.m_hintWnd.Size.Height;
                        }
                        this.m_hintWnd.Owner = null;
                        //this.Parent.Focus();
                        this.m_hintWnd.Owner = this.FindForm();
                        this.m_hintWnd.TopLevel = true;
                        RibbonHintWindow.ShowHint(this.m_hintWnd, p.X, p.Y);
                    }
                }
                else {
                    RibbonHintWindow.HideHint(this.m_hintWnd);
                }
            }
        }


        private int m_MousetXHover = 0;
        private int m_MouseYHover = 0;
        //protected override void OnMouseHover(EventArgs e) { 
        //    if (this.Enabled) {
        //        if (this.IsShowHint && this.m_hintWnd != null) {
        //            RibbonListViewItem item = GetItemAt(m_MousetXHover, m_MouseYHover);
        //            m_MousetXHover = 0;
        //            m_MouseYHover = 0;
        //            if (item != null) {
        //                Point p=new Point(item.X+item.Width,item.Y+item.Height);
        //                p=this.PointToScreen(p);
        //                RibbonHintWindow.ShowHint(this.m_hintWnd,p.X,p.Y);
        //                //(this.Hint as RibbonHintWindowIRQModelMemo).SetHint(item.LargeImageStr, item.Text, item.Memo, "");
        //            }
        //            else {
        //                RibbonHintWindow.HideHint(this.m_hintWnd);
        //            }
        //        }
        //    }
        //    base.OnMouseHover(e);
        //}
        protected override void OnMouseWheel(MouseEventArgs e) {
            int d = (int)Win32Native.MakeUInt32(0, (short)e.Delta);
            Win32Native.SendMessage(m_VScroll.Handle,
                (int)Win32Native.WindowMessage.MouseWheel, d, 0);

            base.OnMouseWheel(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            if (m_hintWnd != null && m_hintWnd.IsDisposed == false) {
                RibbonHintWindow.HideHint(this.m_hintWnd);
            }
            if (m_ToolTip != null) {
                m_ToolTip.Hide(this);
            }
            //鼠标离开的时候，覆盖的取消掉
            if (m_HoverItem != null) {
                m_HoverItem.Hovered = false;
                m_HoverItem = null;
                this.Invalidate();
            }
            base.OnMouseLeave(e);
        }
        public enum FixHitWndTypeLocation : byte
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3
        }
        public FixHitWndTypeLocation HitWndLoactionType = FixHitWndTypeLocation.Right;
        protected override void OnMouseDown(MouseEventArgs e) {
            //选中项目,如果已经选中,则取消选中.
            if (this.Enabled) {
                RibbonListViewItem item = GetItemAt(e.X, e.Y);
                if (item != null) {
                    //if (item.Selected) {
                    if (this.m_hintWnd != null) {
                        bool showHint = false;
                        if (item.Enabled && (m_ShowHintType & ShowHintType.OnlyEnableItem) == ShowHintType.OnlyEnableItem) {
                            showHint = true;
                        }
                        else if (item.Enabled == false && (m_ShowHintType & ShowHintType.OnlyUnenableItem) == ShowHintType.OnlyUnenableItem) {
                            showHint = true;
                        }

                        //这里也需要触发 提示窗口信息
                        if (this.m_hintWnd.Visible == false) {
                            m_PreItemHit = item;
                            Point p;
                            if (m_FixHinPosition) {
                                p = new Point(this.Width, e.Y);
                                if (this.HitWndLoactionType == FixHitWndTypeLocation.Left) {
                                    p = new Point(0 - this.Hint.Width, e.Y);
                                }
                                else if (this.HitWndLoactionType == FixHitWndTypeLocation.Right) {
                                    p = new Point(this.Width, e.Y);
                                }
                                else if (this.HitWndLoactionType == FixHitWndTypeLocation.Top) {
                                    p = new Point(e.X, 0 - this.Hint.Height);
                                }
                                else if (this.HitWndLoactionType == FixHitWndTypeLocation.Bottom) {
                                    p = new Point(e.X, this.Height);
                                }
                            }
                            else {
                                p = new Point(e.X + item.Width, e.Y);
                            }
                            p = this.PointToScreen(p);
                            int screenHeight = Screen.FromHandle(this.Handle).WorkingArea.Height;
                            if (this.m_hintWnd.Size.Height + p.Y > screenHeight) {
                                p.Y = screenHeight - this.m_hintWnd.Size.Height;
                            }
                            if (showHint) {
                                RibbonHintWindow.ShowHint(this.m_hintWnd, p.X, p.Y);
                            }
                        }
                    }
                    //}
                    //else {
                    if (!this.MulitSelect) {
                        ClearSelectedItem();
                    }
                    if (item.Enabled) {
                        if (item.Selected && m_SelectedItems.Contains(item)) {
                            item.Selected = false;
                            m_SelectedItems.Remove(item);
                        }
                        else {
                            item.Selected = true;
                            if (!m_SelectedItems.Contains(item)) {
                                m_SelectedItems.Add(item);
                            }
                        }
                    }
                    //ShowItemHit(item);
                    // }

                }
                else {
                    ClearSelectedItem();
                    //看看有没有选中Group,如果有,则进行展开/收起操作
                    RibbonListViewGroup group = GetGroupHeaderAt(e.X, e.Y);
                    if (group != null) {
                        if (group.IsCollapse) {
                            group.Expand();
                        }
                        else {
                            group.Collapse();
                        }
                        UpdateGroupRect(group.GroupIndex);
                        UpdateScrollMaxinum();
                        m_VisibleItem = GetVisibleItems();
                    }
                }
            }

            this.Invalidate();
            base.OnMouseDown(e);
        }

        //public string StatusString {
        //    get;
        //    set;
        //}
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            foreach (var v in Groups) {
                if (v.m_CanVisible) {
                    v.OnPaint(e);
                }
            }
            foreach (var item in m_VisibleItem) {
                Rectangle rect = new Rectangle(item.X, item.Y - m_CurPos.Y, item.Width, m_RowHeight);
                if (e.ClipRectangle.IntersectsWith(rect)) {
                    DrawItem(e.Graphics, item);
                }
            }
            //this.GotoSelectedItem();
            //if(string.IsNullOrEmpty(StatusString) == false) {
            //    e.Graphics.DrawString(this.StatusString,)
            //}
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            if (this.FindForm() != null && this.FindForm().WindowState == FormWindowState.Minimized) {
                return;
            }
            //this.m_VScroll.ResetScroll();
            UpdateScrollMaxinum();
            m_VisibleItem = GetVisibleItems();
            this.Invalidate();
        }
        private void ComputPosAndSize(RibbonListViewItem prevItem, RibbonListViewItem newItem) {
            if (this.RibbonImageLayout == RibbonImageLayout.Table) {

                if (m_Items.Count % m_ColumnCount != 0) {  //如果还未满一行,则后面加上就是了.
                    newItem.X = prevItem.X + prevItem.Width + m_ColumnCount;
                    newItem.Y = prevItem.Y;
                }
                else {                                     //否则新开一行
                    newItem.X = 0;
                    //newItem.Y = prevItem.Y + m_RowHeight + m_RowSp;
                    newItem.Y = prevItem.Y + prevItem.Height + m_RowSp;
                }
            }
            else {
                //如果是平铺,则按照ListView的宽度来
                //TODO:
            }

        }
        /// <summary>
        /// 更新指定的组Rect
        /// 在更新指定的组时,该组及以后的都将得到更新
        /// </summary>
        /// <param name="groupIndex"></param>
        internal void UpdateGroupRect(int groupIndex) {
            int c = Groups.Count;
            if (groupIndex >= c) {
                return;
            }
            RibbonListViewGroup group;
            if (groupIndex == 0) {
                group = Groups[groupIndex];
                ReComputerItemPosAndSize(this, group.Items, new Point(group.Rect.Left, group.Rect.Bottom));
                group.UpdateGroupDataRect();
            }
            else {
                groupIndex--;
            }
            //按次序,逐项更新Group头的位置
            //后面的Group的位置就按照前一个Group的位置来更新就ok了.
            //当前的Group,由于头是不会变的.所以只需要更新其内容Rect就可以了
            int top = 0;
            for (int i = groupIndex; i < c; i++) {

                if (i > groupIndex) {
                    group = Groups[i];
                    group.Rect.Y = Groups[i - 1].GetAllRect().Bottom;
                    ReComputerItemPosAndSize(this, group.Items, new Point(group.Rect.Left, group.Rect.Bottom));
                }
                Groups[i].UpdateGroupDataRect();
            }


        }
        /// <summary>
        /// 控件尺寸变化时,重新计算item的位置
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="startX">基准位置X</param>
        /// <param name="startY">基准位置Y</param>
        internal static void ReComputerItemPosAndSize(RibbonListView lv, List<RibbonListViewItem> items, Point refPoint) {

            if (lv.RibbonImageLayout == RibbonImageLayout.Table) {
                int count = items.Count;
                if (count < 1) {
                    return;
                }
                int x = refPoint.X;
                int y = refPoint.Y;
                int k = 0;
                int cur_y = y;
                if (lv.m_ColumnCount == 1) {
                    //int vc = 0;
                    for (int i = 0; i < count; i++) {
                        if (items[i].Visble) {
                            items[i].X = x;
                            items[i].Y = cur_y;// y + k * (lv.m_RowHeight + lv.m_RowSp);
                            cur_y = cur_y + items[i].Height + lv.m_RowSp;
                            k++;
                        }
                    }
                }
                else {
                    RibbonListViewItem prevItem = items[0];
                    for (int i = 0; i < count; i += lv.m_ColumnCount) {

                        x = lv.m_ColumnSp;
                        for (int j = 0; j < lv.m_ColumnCount; j++) {
                            if (k >= count) {
                                break;
                            }

                            RibbonListViewItem item = items[k];
                            //if (!item.Visble) {
                            //    continue;
                            //}
                            item.X = x;
                            item.Y = y;
                            x += item.Width + lv.m_ColumnSp;
                            k++;
                            if (prevItem.Visble == false) {
                                item.X = prevItem.X;
                                item.Y = prevItem.Y;
                            }
                            prevItem = item;
                        }
                        y += lv.m_RowHeight + lv.m_RowSp;
                    }
                }
            }
            else {
                //如果是平铺,则按照ListView的宽度来
                int count = items.Count;
                int x = refPoint.X;
                int y = refPoint.Y;
                int i = 0;
                while (i < count) {
                    x = lv.m_ColumnSp;
                    y += lv.m_RowHeight + lv.m_RowSp;
                    while (true) {
                        RibbonListViewItem item = items[i];
                        int w = item.Width;
                        if (x + w > lv.Width) {
                            break;
                        }
                        item.X = x;
                        item.Y = y;
                        x += (w + lv.m_ColumnSp);
                        i++;
                    }
                }
            }
        }

        //获取控件当前可显示逻辑区域中的所有项目
        private List<RibbonListViewItem> GetVisibleItems() {
            //可视区域反正就这么点大.
            Rectangle visibleRect = new Rectangle(0, m_CurPos.Y, this.Width, this.Height);
            List<RibbonListViewItem> ret = new List<RibbonListViewItem>();
            if (Groups.Count == 0) {
                //如果没有组,那只好全部遍历了.
                foreach (var item in m_Items) {
                    if (item.Visble == false) {
                        continue;
                    }
                    //TODO:可以使用可视区域的大小配合行数列数进行优化,减少遍历的数量
                    if (item.X + item.Width < m_CurPos.X || item.X > m_CurPos.X + this.Width) {
                        continue;
                    }
                    if (item.Y + item.Height < m_CurPos.Y || item.Y > m_CurPos.Y + this.Height) {
                        continue;
                    }
                    ret.Add(item);
                }
            }
            else {
                foreach (var group in Groups) {
                    //如果Group和可视区域不相交,那Item就不需要判断了.
                    if (group.GroupRect.IntersectsWith(visibleRect)) {
                        group.m_CanVisible = true;
                        if (group.IsCollapse == false) {
                            foreach (var item in group.Items) {
                                if (item.X + item.Width < m_CurPos.X || item.X > m_CurPos.X + this.Width) {
                                    continue;
                                }
                                if (item.Y + item.Height < m_CurPos.Y || item.Y > m_CurPos.Y + this.Height) {
                                    continue;
                                }
                                ret.Add(item);
                            }
                        }
                    }
                    else {
                        group.m_CanVisible = false;
                    }
                }
            }
            return ret;
        }
        public void SetItemRender(IRibbonListViewItemRender itemRender) {
            m_ItemRender = itemRender;
        }
        private IRibbonListViewItemRender m_ItemRender;
        private void DrawItem(Graphics g, RibbonListViewItem item) {
            if (!item.Visble) {
                return;
            }

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bool drawstr = false;

            if (!drawstr) {
                int k = 0;
            }
            //边框先不要了
            //if (IsDrawBoard) {
            // ItemBoard.Draw(g, item);
            //}
            // g.DrawRectangle(Pens.Red, new Rectangle(item.Left, item.Top, item.Width, item.Height));

            if (item.Selected) {
                //画选中的情况
                m_ItemRender.DrawSelectedItem(g, item);
            }
            else if (item.Hovered && item.Enabled) {
                //画覆盖的情况
                m_ItemRender.DrawHoverItem(g, item);
            }
            else {
                //正常状态
                m_ItemRender.DrawNormalItem(g, item);
            }

        }

        public bool IsToLagerHoverItem {
            get;
            set;
        }

        public void ClearSelectedItem() {
            foreach (var item in m_SelectedItems) {
                item.m_Selected = false;
            }
            m_SelectedItems.Clear();
            //
            this.Invalidate();
        }



        protected override void OnVisibleChanged(EventArgs e) {
            if (this.Visible == false && m_ToolTip != null) {
                m_ToolTip.Hide(this);
            }
            base.OnVisibleChanged(e);
        }

        protected override void Dispose(bool disposing) {
            if (this.IsDisposed)
                return;
            m_Graphics.Dispose();
            ClearSelectedItem();
            m_VisibleItem.Clear();
            //
            int count = m_AllItmes.Count;
            for (int i = count - 1; i > 0; i--) {
                m_AllItmes[i].Dispose();
            }
            foreach (var g in Groups) {
                g.Dispose();
            }
            Groups.Clear();
            m_Items.Clear();
            m_AllItmes.Clear();
            if (m_ToolTip != null) {
                m_ToolTip.Dispose();
            }
            if (m_hintWnd != null && !m_hintWnd.IsDisposed) {
                m_hintWnd.Dispose();
                m_hintWnd = null;
            }
            //if (m_VScroll != null && !m_VScroll.IsDisposed) {
            //    m_VScroll.Dispose();
            //    m_VScroll = null;
            //}

            if (this.ImageList != null) {
                this.ImageList.Dispose();
            }
            if (m_ItemRender != null) {
                m_ItemRender.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Group
        public List<RibbonListViewGroup> Groups = new List<RibbonListViewGroup>();
        public RibbonListViewGroup AddGroup(string name) {
            RibbonListViewGroup group = new RibbonListViewGroup();
            group.Name = name;
            group.ListView = this;
            group.GroupIndex = Groups.Count;
            group.Rect = new Rectangle(0, group.Height * group.GroupIndex, this.GetClientRect().Width, group.Height);
            Groups.Add(group);
            m_GroupedItems.Add(group.GetHashCode(), new List<RibbonListViewItem>());
            return group;
        }
        public void RemoveGroup(string name) {
            RibbonListViewGroup g = null;
            foreach (var v in Groups) {
                if (v.Name == name) {
                    g = v;
                }
            }
            if (g == null) {
                return;
            }
            foreach (var item in g.m_AllItems) {
                m_Items.Remove(item);
                m_AllItmes.Remove(item);
                item.Dispose();
            }
            g.Items.Clear();
            g.m_AllItems.Clear();
            Groups.Remove(g);
            m_GroupedItems.Remove(g.GetHashCode());
            g.Dispose();
        }
        #endregion


        #region UISKin 成员
        private Rectangle m_NormalRect = Rectangle.Empty;
        private Rectangle m_HoverRect = Rectangle.Empty;
        private Rectangle m_DownRect = Rectangle.Empty;
        private Rectangle m_DisableRect = Rectangle.Empty;
        public Rectangle RectNormal {
            get {
                return m_NormalRect;
            }
            set {
                m_NormalRect = value;
            }
        }
        public Rectangle RectHover {
            get {
                return m_HoverRect;
            }
            set {
                m_HoverRect = value;
            }
        }

        public Rectangle RectDown {
            get {
                return m_DownRect;
            }
            set {
                m_DownRect = value;
            }
        }

        public Rectangle RectDisable {
            get {
                return m_DisableRect;
            }
            set {
                m_DisableRect = value;
            }
        }
        public int ImageLayout {
            get;
            set;
        }

        public int ZOrder {
            get;
            set;
        }

        public string ImagePath {
            get;
            set;
        }

        //private Image TopImage {
        //    get;
        //    set;
        //}
        public void UpdateSet() {
            //更新方法
            if (!string.IsNullOrEmpty(ImagePath)) {
                this.BackgroundImage = RibbonUISkinHelper.GetPathImage(ImagePath);
            }
            if (m_VScroll != null) {
                m_VScroll.UpdateSet();
            }
        }
        //对颜色的支持
        protected int m_SetFlag = 0;//颜色
        [Description("设置标志，0默认1:图片,2:颜色,3:颜色+图片")]
        public virtual int SetFlag {
            get {
                return m_SetFlag;
            }
            set {
                m_SetFlag = value;
            }
        }

        protected float m_ColorFactor = 0.4f;
        /// <summary>
        /// 从BACKCOLOR取渐变ENDCOLOR系数
        /// </summary>
        [Description("从BACKCOLOR取渐变ENDCOLOR系数")]
        public virtual float ColorFactor {
            get {
                return m_ColorFactor;
            }
            set {
                m_ColorFactor = value;
            }
        }
        /// <summary>
        /// 是否自己设置颜色 默认为false
        /// </summary>
        [Description("是否自己设置颜色 默认为false")]
        public virtual bool ColorUserSet {
            get;
            set;
        }
        protected Color m_ColorStart = RibbonThemeManager.BackColor;
        /// <summary>
        /// 自定义颜色起始色
        /// </summary>
        [Description("自定义颜色起始色")]
        public Color ColorStart {
            get {
                return m_ColorStart;
            }
            set {
                m_ColorStart = value;
            }
        }
        protected Color m_ColorEnd = Color.Transparent;
        /// <summary>
        /// 自定义颜色结束色
        /// </summary>
        [Description("自定义颜色结束色")]
        public Color ColorEnd {
            get {
                return m_ColorEnd;
            }
            set {
                m_ColorEnd = value;
            }
        }
        protected float m_ColorLinearAngle = 90;
        [Description("渐变角度")]
        public float ColorLinearAngle {
            get {
                return m_ColorLinearAngle;
            }
            set {
                m_ColorLinearAngle = value;
            }
        }
        #endregion


    }
   
}
