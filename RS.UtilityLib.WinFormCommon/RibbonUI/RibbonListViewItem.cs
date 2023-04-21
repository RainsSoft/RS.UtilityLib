using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// RibbonListView Item
    /// </summary>
    public class RibbonListViewItem : IDisposable
    {
        //TODO:把ListViewItem做成TreeNode的形式

        private RibbonListView m_View;
        public RibbonListView ListView {
            get {
                return m_View;
            }
            set {
                //如果ListView发生变化,则先从当前移除,再加入到其他里.
                if (value != m_View) {
                    if (m_View != null && value != null) {
                        throw new Exception("先从" + m_View.Name + "中移除");
                    }
                    else {
                        m_View = value;
                        //如果设置了ImageIndex，则需要获取一下Image
                        if (IsUseImageList()) {
                            if (m_View.ImageList.Images.Count > 0) {
                                m_Small = m_View.ImageList.Images[this.ImageListIndex];
                            }
                            //UpdateSize();
                        }
                        UpdateSize();
                    }
                }
                else {
                    //if (value != null) {
                    //    value.BeginUpdate();
                    //    value.AddItem(this);
                    //    value.EndUpdate();
                    //    UpdateSize();
                    //}
                    //m_View = value;
                }
            }
        }
        /// <summary>
        /// 所处的级别
        /// </summary>
        public int Level;
        private bool m_Visble = true;
        public bool Visble {
            get {
                return m_Visble;
            }
            set {
                if (m_Visble != value) {
                    m_Visble = value;

                }
            }
        }
        private Image m_Small;
        /// <summary>
        /// 图标。
        /// 赋值时采用拷贝方式。外部数据需要自行释放。
        /// </summary>
        public Image SmallImage {
            get {
                return m_Small;
            }
            set {
                if (value != null) {
                    m_Small = new Bitmap(value); //防止dispose的时候把外部资源给释放了
                    UpdateSize();
                }
                else {
                    if (m_Small != null && string.IsNullOrEmpty(m_ImageListIndex)) {
                        m_Small.Dispose();
                    }
                    m_Small = null;
                    UpdateSize();
                }

            }
        }

        private string m_ImageListIndex = string.Empty;
        /// <summary>
        /// 使用ImageList方式组织图标时当前项目的图标索引。
        /// 优先级比SmallImage低，即如果同时指定了SmallImage和ImageListIndex，将优先使用SmallImage
        /// </summary>
        public string ImageListIndex {
            get {
                return m_ImageListIndex;
            }
            set {
                m_ImageListIndex = value;
                if (ListView != null && ListView.ImageList != null) {
                    if (ListView.ImageList.Images.Count > 0) {
                        m_Small = ListView.ImageList.Images[this.ImageListIndex];
                    }
                    UpdateSize();
                }
            }
        }

        private bool IsUseImageList() {
            return ListView != null && ListView.ImageList != null && string.IsNullOrEmpty(m_ImageListIndex) == false;
        }
        //
        private string m_LargeImageStr;
        ///<summary>
        ///用于显示说明图片,大图路径 主要考虑到是减少内存的使用
        ///</summary>
        public string LargeImageStr {
            get {
                return m_LargeImageStr;
            }
            set {
                m_LargeImageStr = value;
            }
        }
        internal bool m_Selected;
        /// <summary>
        /// 是否选中。
        /// </summary>
        public bool Selected {
            get {
                return m_Selected;
            }
            set {
                m_Selected = value;
                if (m_Selected) {
                    Hovered = false;
                }
                if (ListView != null) {
                    if (m_Selected) {
                        ListView.AddSelectItem(this);
                    }
                    else {
                        ListView.RemoveSelectItem(this);
                    }
                }
            }
        }
        private string m_Text;
        public string Text {
            get {
                return m_Text;
            }
            set {
                if (m_Text != value) {
                    m_Text = value;
                    UpdateSize();
                }
            }
        }
        public string Memo;
        public object Tag;
        private bool m_Enabled = true;
        public bool Enabled {
            get {
                return m_Enabled;
            }
            set {
                //if (m_Enabled != value) {
                m_Enabled = value;
                if (TextBrush != null) {
                    if (TextBrush is SolidBrush) {
                        (TextBrush as SolidBrush).Color = m_Enabled ? m_ForeColor : Color.Gray;
                    }
                }
                if (ListView != null && ListView.m_InUpdate == false) {
                    ListView.Invalidate(new Rectangle(ListView.CurPosition.X + this.X, ListView.CurPosition.Y + this.Y, this.Width, Height));
                }
                // }
            }
        }
        private Font m_Font;
        private bool m_UserFont = false;
        public Font GetFont() {
            return m_Font;
        }
        public void SetFont(Font value) {
            if (m_UserFont && m_Font != null && !RibbonThemeManager.IsCacheFont(m_Font)) {
                m_Font.Dispose();
                m_Font = null;
            }
            m_Font = value;
            m_UserFont = true;
            if (m_Font != null) {
                UpdateSize();
            }
            else {
                throw new ArgumentNullException("font");
            }
        }
        private Color m_ForeColor;
        public Color ForeColor {
            get {
                return m_ForeColor;
            }
            set {
                if (m_ForeColor != value) {
                    m_ForeColor = value;
                    if (TextBrush != null) {
                        if (TextBrush is SolidBrush) {
                            (TextBrush as SolidBrush).Color = m_ForeColor;
                        }
                    }
                }
            }
        }

        public Brush TextBrush;
        internal int ImageWidth;
        internal int ImageHeight;
        internal int Width;
        internal int Height;
        internal int X;
        internal int Y;
        internal bool Hovered;
        public int Left {
            get {
                return X;
            }
        }
        public int Top {
            get {
                return Y;
            }
        }
        public RibbonListViewItem() {
            m_ForeColor = RibbonThemeManager.TextColor;
            TextBrush = new SolidBrush(m_ForeColor);
            m_Font = new Font(RibbonThemeManager.NormalFont, RibbonThemeManager.NormalFont.Style);
            m_UserFont = true;
        }
        public RibbonListViewItem(string text)
            : this() {
            this.Text = text;
        }
        private bool m_UpdateSize = true;
        public SizeF TextSize;
        [System.Runtime.InteropServices.DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        /// <summary>
        /// 更新尺寸。是否显示文本等会对尺寸造成影响
        /// </summary>
        /// <devdoc>
        /// ListView在修改IsShowItemText属性时，将对所有的Item进行遍历，修改其尺寸
        /// </devdoc>
        internal void UpdateSize() {

            //MeasureString会出错？？？？？
            try {
                if (m_View == null) {
                    return;
                }
                if (m_Small != null && string.IsNullOrEmpty(m_Text) == false) {
                    if (ListView.IsShowItemText) {
                        //2012/9/26
                        //by czj
                        //下面这个measureString崩溃过几次，一直找不出原因。可能是m_Font为null造成的。
                        CheckFont();
                        //m_Font = new Font("宋体", 9);
                        try {
                            TextSize = m_View.m_Graphics.MeasureString(string.IsNullOrEmpty(m_Text) ? "_" : m_Text, m_Font);
                        }
                        catch {
                            TextSize = new SizeF(64f, 16f);
                        }

                    }
                    else {
                        TextSize = SizeF.Empty;
                    }
                    ImageWidth = m_Small.Width;
                    ImageHeight = m_Small.Height;
                    float w = ImageWidth + TextSize.Width + 4;

                    //如果只有一列，就用ListView的宽度
                    //如果ListView的宽度比图+字的宽度小，那么，需要换行
                    //暂时只支持换两行
                    if (ListView.ColumnCount == 1) {
                        Width = ListView.GetClientRect().Width;
                        if (ListView.ItemAutoSize && ImageWidth + TextSize.Width > ListView.GetClientRect().Width - 4) {
                            TextSize = new SizeF(TextSize.Width, TextSize.Height * 2);
                            int itemheight = (int)Math.Ceiling(TextSize.Height);
                            Height = Math.Max(itemheight, ListView.RowHeight);
                            Height = Math.Max(ImageHeight, Height);
                        }
                        else {
                            Height = (int)Math.Ceiling(ImageHeight > TextSize.Height ? ImageHeight : TextSize.Height);
                            Height = Math.Max(Height, ListView.RowHeight);
                        }
                    }
                    else {
                        Width = (int)Math.Ceiling(w);
                        Height = (int)Math.Ceiling(ImageHeight > TextSize.Height ? ImageHeight : TextSize.Height);
                        Height = Math.Max(Height, ListView.RowHeight);
                    }

                }
                else if (m_Small == null) {
                    CheckFont();
                    try {
                        TextSize = ListView.m_Graphics.MeasureString(string.IsNullOrEmpty(m_Text) ? "_" : m_Text, m_Font);
                    }
                    catch {
                        TextSize = new SizeF(64f, 16f);
                    }
                    float w = TextSize.Width + 4;
                    //如果只有一列，就用ListView的宽度
                    //Width = ListView.ColumnCount == 1 ? ListView.GetClientRect().Width - 4 : (int)Math.Ceiling(w);
                    if (ListView.ColumnCount == 1) {

                        Width = ListView.GetClientRect().Width - 4;
                        //如果实际内容宽，那就换行，暂时支持最多2行
                        if (ListView.ItemAutoSize && TextSize.Width > Width) {
                            TextSize = new SizeF(TextSize.Width, TextSize.Height * 2);
                            int itemheight = (int)Math.Ceiling(TextSize.Height);
                            Height = Math.Max(itemheight, ListView.RowHeight);
                        }
                        else {
                            Height = (int)Math.Ceiling(Math.Max(ListView.RowHeight, TextSize.Height));
                        }
                    }
                    else {
                        Width = (int)Math.Ceiling(w);
                        Height = (int)Math.Ceiling(Math.Max(ListView.RowHeight, TextSize.Height));
                        Height = Math.Max(Height, ListView.RowHeight);
                    }

                    ImageHeight = 0;
                    ImageWidth = 0;

                }
            }
            catch {
                System.Reflection.FieldInfo fd = m_Font.GetType().GetField("nativeFont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fd != null) {
                    object o = fd.GetValue(m_Font);
                    IntPtr p = (IntPtr)o;
                    Debug.WriteLine("RibbonListViewItem.UpdateSize()错误:" + this.Text + " Font:" + p.ToInt32().ToString());

                }
                else {
                    Debug.WriteLine("RibbonListViewItem.UpdateSize()错误:" + this.Text + " Font:未取得nativeFont数据");
                }
                throw;
            }
        }

        private void CheckFont() {
            bool isDiposed = false;
            try {
                IntPtr pFont = m_Font.ToHfont();
                if (pFont == IntPtr.Zero) {
                    isDiposed = true;
                }
                else {
                    DeleteObject(pFont);
                }
            }
            catch {
                isDiposed = true;
            }
            if (m_Font == null) {
                isDiposed = true;
            }
            if (isDiposed) {
                m_UserFont = true;
                try {
                    m_Font = new Font("宋体", 9.0f);
                }
                catch {
                    m_Font = new Font("Arial", 9.0f);
                }
            }
        }

        private RibbonListViewGroup m_Group;
        /// <summary>
        /// 如果需要在组之间切换.需手动调用Group.AddItem,RemoveItem
        /// </summary>
        public RibbonListViewGroup Group {
            get {
                return m_Group;
            }
            internal set {

                m_Group = value;

            }
        }
        #region IDisposable 成员
        ~RibbonListViewItem() {
            Dispose(false);
        }
        private bool m_IsDisposed = false;
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            if (m_IsDisposed) {
                return;
            }
            this.Tag = null;
            ListView = null;
            //如果是用ImageList中的，则不能释放
            if (SmallImage != null && IsUseImageList() == false) {
                SmallImage.Dispose();
                SmallImage = null;
            }
            //if (LargeImage != null) {
            //    LargeImage.Dispose();
            //    LargeImage = null;
            //}
            if (TextBrush != null) {
                TextBrush.Dispose();
                TextBrush = null;
            }
            if (m_UserFont && m_Font != null && !RibbonThemeManager.IsCacheFont(m_Font)) {
                m_Font.Dispose();
                m_Font = null;
            }
            if (Group != null) {
                Group.RemoveItem(this);
            }
            Group = null;
            m_IsDisposed = true;
        }

        #endregion
    }
    public interface IRibbonListViewItemRender : IDisposable
    {
        void DrawNormalItem(Graphics g, RibbonListViewItem item);
        void DrawHoverItem(Graphics g, RibbonListViewItem item);
        void DrawSelectedItem(Graphics g, RibbonListViewItem item);
    }
    public class RibbonListViewItemRender : IRibbonListViewItemRender
    {
        int m_LevelTab;
        //bool m_IsShowHover, m_IsShowItemText, IsToLagerHoverItem;
        //Point m_CurPos;
        System.Drawing.Imaging.ImageAttributes disabledImageAttr;
        RibbonListView m_ListView;
        public RibbonListViewItemRender(RibbonListView listview) {
            float[][] newColorMatrix = new float[5][];
            //newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
            //newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
            //newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
            //newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
            //newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

            newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
            newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
            newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
            newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
            newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            disabledImageAttr = new ImageAttributes();
            disabledImageAttr.ClearColorKey();
            disabledImageAttr.SetColorMatrix(matrix);
            //     
            m_ListView = listview;
            m_LevelTab = m_ListView.LevelTab;
            //m_CurPos = m_ListView.CurPosition;
            //m_IsShowHover = m_ListView.IsShowHover;
            //m_IsShowItemText = m_ListView.IsShowItemText;
            //IsToLagerHoverItem = m_ListView.IsToLagerHoverItem;
        }
        #region IRibbonListViewItemRender 成员
        public void DrawNormalItem(Graphics g, RibbonListViewItem item) {

            int itemX = item.X + item.Level * m_LevelTab;
            Point curpos = m_ListView.CurPosition;
            if (item.SmallImage != null && (item.Hovered == false || m_ListView.IsShowHover == false)) {
                g.DrawImage(item.SmallImage,
                    new Rectangle(itemX, item.Y - curpos.Y, item.ImageWidth, item.ImageHeight),
                    0, 0, item.ImageWidth, item.ImageHeight,
                    GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);

                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    //  g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y + 1 - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                    RectangleF rect = new RectangleF((float)(item.ImageWidth + itemX), item.Y + 1 - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f, item.Width - item.ImageWidth, item.Height);
                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, rect);
                    // drawstr = true;
                }
            }
            else if (string.IsNullOrEmpty(item.Text) == false && (item.Hovered == false || m_ListView.IsShowHover == false) && m_ListView.IsShowItemText) {
                //g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y + 1 - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                //using(StringFormat sf = new StringFormat()) {
                //    sf.Trimming= StringTrimming.
                //}
                RectangleF rect = new RectangleF((float)(item.ImageWidth + itemX), item.Y + 1 - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f, item.Width, item.Height);
                g.DrawString(item.Text, item.GetFont(), item.TextBrush, rect);
                //drawstr = true;
                //g.DrawString(item.Text,item.GetFont(),item.TextBrush,item.)
            }
            //return drawstr;
        }
        public void DrawHoverItem(Graphics g, RibbonListViewItem item) {
            //需要减去当前的偏移量.
            if (!item.Visble) {
                return;
            }
            Point curpos = m_ListView.CurPosition;
            int itemX = item.X + item.Level * m_LevelTab;

            //g.DrawImage(item.SmallImage, new Rectangle(item.X, item.Y - m_CurPos.Y, item.Width, item.Height));
            //if (item.SmallImage != null) {
            //    g.DrawImage(item.SmallImage, new Rectangle(itemX - 2, item.Y - 2 - m_CurPos.Y, item.ImageWidth + 4, item.ImageHeight + 4));
            //    if (string.IsNullOrEmpty(item.Text) == false && m_IsShowItemText) {
            //        g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
            //    }
            //}
            //else if (string.IsNullOrEmpty(item.Text) == false && m_IsShowItemText) {
            //    g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
            //}
            //构造圆角矩形
            GraphicsPath path = new GraphicsPath();
            Point p1 = new Point(itemX - curpos.X + 2, item.Y - curpos.Y);
            Point p2 = new Point(itemX + item.Width - curpos.X - 2, item.Y - curpos.Y);
            Point p3 = new Point(itemX + item.Width - curpos.X, item.Y - curpos.Y + 2);
            Point p4 = new Point(itemX + item.Width - curpos.X, item.Y - curpos.Y + item.Height - 3);
            Point p5 = new Point(itemX + item.Width - curpos.X - 2, item.Y - curpos.Y + item.Height - 1);
            Point p6 = new Point(itemX - curpos.X + 2, item.Y - curpos.Y + item.Height - 1);
            Point p7 = new Point(itemX - curpos.X, item.Y + item.Height - curpos.Y - 3);
            Point p8 = new Point(itemX - curpos.X, item.Y - curpos.Y + 2);
            Point[] points = new Point[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            path.AddLines(points);
            //画选中的背景
            //Color c = Color.FromArgb(80, 21, 168, 59);
            Color edge = Color.FromArgb(255, 118, 158, 206);
            Color c = Color.FromArgb(64, 220, 235, 252);
            //Color c1 = Color.FromArgb(200, 21, 168, 59);
            Color c1 = Color.FromArgb(80, 0, 255, 0);
            Color c2 = Color.FromArgb(64, 220, 235, 252);
            //Pen p = new Pen(edge, 1);
            Region r = new Region(path);
            LinearGradientBrush lb = null;
            try {
                Rectangle rect = new Rectangle(itemX - curpos.X, item.Y - curpos.Y, item.Width, item.Height);
                lb = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical);
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { c, c1, c1, c2 };//, c };
                cb.Positions = new float[] { 0f, 0.4f, 0.6f, 1.0f };
                lb.InterpolationColors = cb;
                lb.WrapMode = WrapMode.Tile;
                //g.DrawPath(p, path);

                g.FillRegion(lb, r);

                //画Item
                if (item.SmallImage != null) {
                    if (m_ListView.IsToLagerHoverItem) {
                        g.DrawImage(item.SmallImage,
                           new Rectangle(itemX, item.Y - curpos.Y, item.ImageWidth, item.ImageHeight),
                           4, 4, item.ImageWidth - 4, item.ImageHeight - 4,
                           GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                    }
                    else
                        g.DrawImage(item.SmallImage,
                            new Rectangle(itemX, item.Y - curpos.Y, item.ImageWidth, item.ImageHeight),
                            0, 0, item.ImageWidth, item.ImageHeight,
                            GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                }
                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    //给Item的文字改一下颜色
                    SolidBrush txtB = item.TextBrush as SolidBrush;
                    Color cc = txtB.Color;
                    //txtB.Color = Color.White;
                    // g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y - curpos.Y + (item.Height - 1 - item.TextSize.Height) * 0.5f);

                    RectangleF rectstr;
                    if (item.SmallImage != null) {
                        rectstr = new RectangleF((float)(item.ImageWidth + itemX), item.Y - curpos.Y + (item.Height - 1 - item.TextSize.Height) * 0.5f, item.Width - item.ImageWidth, item.Height);
                    }
                    else {
                        rectstr = new RectangleF((float)(item.ImageWidth + itemX), item.Y - curpos.Y + (item.Height - 1 - item.TextSize.Height) * 0.5f, item.Width, item.Height);
                    }

                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, rectstr);

                    txtB.Color = cc;
                }
                //画选中的勾
                //using (Font f = new Font(RibbonThemeManager.NormalFont.FontFamily, 10, FontStyle.Bold)) {
                //    using (SolidBrush sb = new SolidBrush(SystemColors.ButtonHighlight/*Color.FromArgb(243, 145, 32)*/)) {
                //        g.DrawString("√", f, sb, p2.X - 12, p2.Y);
                //    }
                //}
            }
            finally {
                if (path != null) {
                    path.Dispose();
                }
                if (r != null) {
                    r.Dispose();
                }
                //if (p != null) {
                //    p.Dispose();
                //}
                if (lb != null) {
                    lb.Dispose();
                }
            }

        }
        public void DrawSelectedItem(Graphics g, RibbonListViewItem item) {
            if (!item.Visble) {
                return;
            }
            int itemX = item.X + item.Level * m_LevelTab;

            if (g == null) {
                throw new ArgumentNullException("graphics");
            }
            Point curpos = m_ListView.CurPosition;
            int left = 0;
            int width = 0; //由于偏移的存在，left＋Item的宽可能大于控件的大小，所以需要判断一下。
            //构造圆角矩形
            //GraphicsPath path = new GraphicsPath();
            //Point p1 = new Point(itemX - m_CurPos.X + 1, item.Y - m_CurPos.Y);

            //Point p2 = new Point(itemX + item.Width - m_CurPos.X - 1, item.Y - m_CurPos.Y);
            //Point p3 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + 1);
            //Point p4 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + item.Height - 3);
            //Point p5 = new Point(itemX + item.Width - m_CurPos.X - 1, item.Y - m_CurPos.Y + item.Height-2);
            //Point p6 = new Point(itemX - m_CurPos.X + 1, item.Y - m_CurPos.Y + item.Height-2);
            //Point p7 = new Point(itemX - m_CurPos.X, item.Y + item.Height - m_CurPos.Y - 3);
            //Point p8 = new Point(itemX - m_CurPos.X, item.Y - m_CurPos.Y + 1);
            //Point[] points = new Point[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            //path.AddLines(points);
            //画选中的背景
            //Color c = Color.FromArgb(80, 21, 168, 59);
            Color edge = Color.FromArgb(255, 118, 158, 255);
            Color c = Color.FromArgb(128, 220, 235, 255);
            //Color c1 = Color.FromArgb(200, 21, 168, 59);
            Color c1 = Color.FromArgb(164, 0, 255, 0);
            Color c2 = Color.FromArgb(128, 220, 235, 255);
            Pen p = new Pen(edge, 2);
            //Region r = new Region(path);
            LinearGradientBrush lb = null;
            try {
                Rectangle rect = new Rectangle(itemX - curpos.X, item.Y - curpos.Y - 1, item.Width, item.Height + 2);
                lb = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical);
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { c, c1, c1, c2 };//, c };
                cb.Positions = new float[] { 0f, 0.4f, 0.6f, 1.0f };
                lb.InterpolationColors = cb;
                lb.WrapMode = WrapMode.Tile;
                //g.DrawPath(p, path);
                //g.DrawRectangle(p, rect);
                //g.FillRegion(lb, r);
                g.FillRectangle(lb, rect);
                //画Item
                if (item.SmallImage != null) {
                    g.DrawImage(item.SmallImage,
                        new Rectangle(itemX + 2 + 2, item.Y - curpos.Y + 2, item.ImageWidth - 4, item.ImageHeight - 4),
                        0, 0, item.ImageWidth, item.ImageHeight,
                        GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                }
                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    //给Item的文字改一下颜色
                    SolidBrush txtB = item.TextBrush as SolidBrush;
                    Color cc = txtB.Color;
                    //txtB.Color = Color.White;
                    //g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                    RectangleF rectstr;
                    if (item.SmallImage != null) {
                        rectstr = new RectangleF((float)(item.ImageWidth + itemX + 2), item.Y - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f, item.Width - item.ImageWidth, item.Height);
                    }
                    else {
                        rectstr = new RectangleF((float)(item.ImageWidth + itemX + 2), item.Y - curpos.Y + (item.Height - item.TextSize.Height) * 0.5f, item.Width, item.Height);
                    }

                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, rectstr);
                    txtB.Color = cc;
                }
                //画选中的勾
                //using (Font f = new Font(RibbonThemeManager.NormalFont.FontFamily, 10, FontStyle.Bold)) {
                //    using (SolidBrush sb = new SolidBrush(SystemColors.ButtonHighlight/*Color.FromArgb(243, 145, 32)*/)) {
                //        g.DrawString("√", f, sb, p2.X - 12, p2.Y);
                //    }
                //}
            }
            finally {
                if (p != null) {
                    p.Dispose();
                }
                if (lb != null) {
                    lb.Dispose();
                }
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (disabledImageAttr != null) {
                disabledImageAttr.Dispose();
            }
            m_ListView = null;
        }
        #endregion
        ~RibbonListViewItemRender() {
            Dispose(false);
        }
    }

    public class RibbonListViewItemRenderWin7 : IRibbonListViewItemRender
    {

        int m_LevelTab;
        //bool m_IsShowHover, m_IsShowItemText, IsToLagerHoverItem;
        //Point m_CurPos;
        System.Drawing.Imaging.ImageAttributes disabledImageAttr;
        RibbonListView m_ListView;
        public RibbonListViewItemRenderWin7(RibbonListView listview) {
            float[][] newColorMatrix = new float[5][];
            //newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
            //newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
            //newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
            //newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
            //newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

            newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
            newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
            newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
            newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
            newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            disabledImageAttr = new ImageAttributes();
            disabledImageAttr.ClearColorKey();
            disabledImageAttr.SetColorMatrix(matrix);

            //     
            m_ListView = listview;
            m_LevelTab = m_ListView.LevelTab;
            //m_CurPos = m_ListView.CurPosition;
            //m_IsShowHover = m_ListView.IsShowHover;
            //m_IsShowItemText = m_ListView.IsShowItemText;
            //IsToLagerHoverItem = m_ListView.IsToLagerHoverItem;
        }
        #region IRibbonListViewItemRender 成员
        public void DrawNormalItem(Graphics g, RibbonListViewItem item) {
            Point m_CurPos = m_ListView.CurPosition;
            int itemX = item.X + item.Level * m_LevelTab;

            if (item.SmallImage != null && (item.Hovered == false || m_ListView.IsShowHover == false)) {
                g.DrawImage(item.SmallImage,
                    new Rectangle(itemX, item.Y - m_CurPos.Y, item.ImageWidth, item.ImageHeight),
                    0, 0, item.ImageWidth, item.ImageHeight,
                    GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);

                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y + 1 - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                    // drawstr = true;
                }
            }
            else if (string.IsNullOrEmpty(item.Text) == false && (item.Hovered == false || m_ListView.IsShowHover == false) && m_ListView.IsShowItemText) {
                g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y + 1 - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                //drawstr = true;
            }
            //return drawstr;
        }
        public void DrawHoverItem(Graphics g, RibbonListViewItem item) {
            //需要减去当前的偏移量.
            if (!item.Visble) {
                return;
            }
            Point m_CurPos = m_ListView.CurPosition;
            int itemX = item.X + item.Level * m_LevelTab;

            //g.DrawImage(item.SmallImage, new Rectangle(item.X, item.Y - m_CurPos.Y, item.Width, item.Height));
            //if (item.SmallImage != null) {
            //    g.DrawImage(item.SmallImage, new Rectangle(itemX - 2, item.Y - 2 - m_CurPos.Y, item.ImageWidth + 4, item.ImageHeight + 4));
            //    if (string.IsNullOrEmpty(item.Text) == false && m_IsShowItemText) {
            //        g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
            //    }
            //}
            //else if (string.IsNullOrEmpty(item.Text) == false && m_IsShowItemText) {
            //    g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
            //}
            //构造圆角矩形
            GraphicsPath path = new GraphicsPath();
            Point p1 = new Point(itemX - m_CurPos.X + 2, item.Y - m_CurPos.Y);
            Point p2 = new Point(itemX + item.Width - m_CurPos.X - 2, item.Y - m_CurPos.Y);
            Point p3 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + 2);
            Point p4 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + item.Height - 3);
            Point p5 = new Point(itemX + item.Width - m_CurPos.X - 2, item.Y - m_CurPos.Y + item.Height - 1);
            Point p6 = new Point(itemX - m_CurPos.X + 2, item.Y - m_CurPos.Y + item.Height - 1);
            Point p7 = new Point(itemX - m_CurPos.X, item.Y + item.Height - m_CurPos.Y - 3);
            Point p8 = new Point(itemX - m_CurPos.X, item.Y - m_CurPos.Y + 2);
            Point[] points = new Point[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            path.AddLines(points);
            //画选中的背景
            //Color c = Color.FromArgb(80, 21, 168, 59);
            Color edge = Color.FromArgb(255, 118, 158, 206);
            Color c = Color.FromArgb(64, 220, 235, 252);
            //Color c1 = Color.FromArgb(200, 21, 168, 59);
            Color c1 = Color.FromArgb(80, 0, 255, 0);
            Color c2 = Color.FromArgb(64, 220, 235, 252);
            //Pen p = new Pen(edge, 1);
            Region r = new Region(path);
            LinearGradientBrush lb = null;
            try {
                Rectangle rect = new Rectangle(itemX - m_CurPos.X, item.Y - m_CurPos.Y, item.Width, item.Height);
                lb = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical);
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { c, c1, c1, c2 };//, c };
                cb.Positions = new float[] { 0f, 0.4f, 0.6f, 1.0f };
                lb.InterpolationColors = cb;
                lb.WrapMode = WrapMode.Tile;
                //g.DrawPath(p, path);

                g.FillRegion(lb, r);

                //画Item
                if (item.SmallImage != null) {
                    if (m_ListView.IsToLagerHoverItem) {
                        g.DrawImage(item.SmallImage,
                           new Rectangle(itemX, item.Y - m_CurPos.Y, item.ImageWidth, item.ImageHeight),
                           4, 4, item.ImageWidth - 4, item.ImageHeight - 4,
                           GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                    }
                    else
                        g.DrawImage(item.SmallImage,
                            new Rectangle(itemX, item.Y - m_CurPos.Y, item.ImageWidth, item.ImageHeight),
                            0, 0, item.ImageWidth, item.ImageHeight,
                            GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                }
                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    //给Item的文字改一下颜色
                    SolidBrush txtB = item.TextBrush as SolidBrush;
                    Color cc = txtB.Color;
                    //txtB.Color = Color.White;
                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX), item.Y - m_CurPos.Y + (item.Height - 1 - item.TextSize.Height) * 0.5f);
                    txtB.Color = cc;
                }
                //画选中的勾
                //using (Font f = new Font(RibbonThemeManager.NormalFont.FontFamily, 10, FontStyle.Bold)) {
                //    using (SolidBrush sb = new SolidBrush(SystemColors.ButtonHighlight/*Color.FromArgb(243, 145, 32)*/)) {
                //        g.DrawString("√", f, sb, p2.X - 12, p2.Y);
                //    }
                //}
            }
            finally {
                if (path != null) {
                    path.Dispose();
                }
                if (r != null) {
                    r.Dispose();
                }
                //if (p != null) {
                //    p.Dispose();
                //}
                if (lb != null) {
                    lb.Dispose();
                }
            }

        }
        public void DrawSelectedItem(Graphics g, RibbonListViewItem item) {
            if (!item.Visble) {
                return;
            }
            int itemX = item.X + item.Level * m_LevelTab;

            if (g == null) {
                throw new ArgumentNullException("graphics");
            }
            Point m_CurPos = m_ListView.CurPosition;
            int left = 0;
            int width = 0; //由于偏移的存在，left＋Item的宽可能大于控件的大小，所以需要判断一下。
            //构造圆角矩形
            //GraphicsPath path = new GraphicsPath();
            //Point p1 = new Point(itemX - m_CurPos.X + 1, item.Y - m_CurPos.Y);

            //Point p2 = new Point(itemX + item.Width - m_CurPos.X - 1, item.Y - m_CurPos.Y);
            //Point p3 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + 1);
            //Point p4 = new Point(itemX + item.Width - m_CurPos.X, item.Y - m_CurPos.Y + item.Height - 3);
            //Point p5 = new Point(itemX + item.Width - m_CurPos.X - 1, item.Y - m_CurPos.Y + item.Height-2);
            //Point p6 = new Point(itemX - m_CurPos.X + 1, item.Y - m_CurPos.Y + item.Height-2);
            //Point p7 = new Point(itemX - m_CurPos.X, item.Y + item.Height - m_CurPos.Y - 3);
            //Point p8 = new Point(itemX - m_CurPos.X, item.Y - m_CurPos.Y + 1);
            //Point[] points = new Point[] { p1, p2, p3, p4, p5, p6, p7, p8, p1 };
            //path.AddLines(points);
            //画选中的背景
            //Color c = Color.FromArgb(80, 21, 168, 59);
            Color edge = Color.FromArgb(255, 118, 158, 255);
            Color c = Color.FromArgb(128, 220, 235, 255);
            //Color c1 = Color.FromArgb(200, 21, 168, 59);
            Color c1 = Color.FromArgb(164, 0, 255, 0);
            Color c2 = Color.FromArgb(128, 220, 235, 255);
            Pen p = new Pen(edge, 2);
            //Region r = new Region(path);
            LinearGradientBrush lb = null;
            try {
                Rectangle rect = new Rectangle(itemX - m_CurPos.X, item.Y - m_CurPos.Y - 1, item.Width, item.Height + 2);
                //lb = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical);
                //ColorBlend cb = new ColorBlend();
                //cb.Colors = new Color[] { c, c1, c1, c2 };//, c };
                //cb.Positions = new float[] { 0f, 0.4f, 0.6f, 1.0f };
                //lb.InterpolationColors = cb;
                //lb.WrapMode = WrapMode.Tile;
                ////g.DrawPath(p, path);
                ////g.DrawRectangle(p, rect);
                ////g.FillRegion(lb, r);
                //g.FillRectangle(lb, rect);
                //画Item
                if (item.SmallImage != null) {
                    g.DrawImage(item.SmallImage,
                        new Rectangle(itemX + 2 + 2, item.Y - m_CurPos.Y + 2, item.ImageWidth - 4, item.ImageHeight - 4),
                        0, 0, item.ImageWidth, item.ImageHeight,
                        GraphicsUnit.Pixel, item.Enabled ? null : disabledImageAttr);
                }
                if (string.IsNullOrEmpty(item.Text) == false && m_ListView.IsShowItemText) {
                    //给Item的文字改一下颜色
                    SolidBrush txtB = item.TextBrush as SolidBrush;
                    Color cc = txtB.Color;
                    //txtB.Color = Color.White;
                    g.DrawString(item.Text, item.GetFont(), item.TextBrush, (float)(item.ImageWidth + itemX + 2), item.Y - m_CurPos.Y + (item.Height - item.TextSize.Height) * 0.5f);
                    txtB.Color = cc;
                }
                RibbonTabControl.RenderSelection(g, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2), 2, true);
                //画选中的勾
                //using (Font f = new Font(RibbonThemeManager.NormalFont.FontFamily, 10, FontStyle.Bold)) {
                //    using (SolidBrush sb = new SolidBrush(SystemColors.ButtonHighlight/*Color.FromArgb(243, 145, 32)*/)) {
                //        g.DrawString("√", f, sb, p2.X - 12, p2.Y);
                //    }
                //}
            }
            finally {
                //if (path != null) {
                //    path.Dispose();
                //}
                //if (r != null) {
                //    r.Dispose();
                //}
                if (p != null) {
                    p.Dispose();
                }
                if (lb != null) {
                    lb.Dispose();
                }
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (disabledImageAttr != null) {
                disabledImageAttr.Dispose();
            }
            m_ListView = null;
        }
        #endregion
        ~RibbonListViewItemRenderWin7() {
            Dispose(false);
        }
    }

    public class RibbonListViewItemBoard
    {
        public virtual void Draw(Graphics g, RibbonListViewItem item) {

        }
    }

}
