using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// 组信息 .
    /// </summary>
    public class RibbonListViewGroup
    {
        //TODO:改进目标：将背景图和收缩、展开图标都整合在一个Image里。

        /// <summary>
        /// 背景Rect
        /// </summary>
        public Rectangle BackRect;
        /// <summary>
        /// 无内容时的Rect
        /// </summary>
        public Rectangle NoDataRect;
        /// <summary>
        /// 收起时的Rect
        /// </summary>
        public Rectangle CollapseRect;
        /// <summary>
        /// 展开时的Rect
        /// </summary>
        public Rectangle ExpandRect;
        //private Image m_image;
        /// <summary>
        /// 背景图
        /// </summary>
        public Image BackImage;
        public object Tag {
            get; set;
        }

        private RibbonListView m_View;
        public RibbonListView ListView {
            get {
                return m_View;
            }
            set {
                if (m_View != value) {
                    if (m_View != null) {
                        m_View.SizeChanged -= ListView_SizeChanged;
                    }
                }
                m_View = value;
                if (m_View != null) {
                    m_View.SizeChanged += ListView_SizeChanged;
                }
            }
        }
        public int Left = 0;
        public int Width {
            get {
                return ListView == null ? 0 : ListView.Width;
            }
        }
        public int Height = 32;
        /// <summary>
        /// 组头的尺寸
        /// </summary>
        public Rectangle Rect;
        /// <summary>
        /// 组的内容的尺寸
        /// </summary>
        public Rectangle GroupRect;

        public string Name;
        private Font m_Font;
        private bool m_UserFont = false;
        public Font Font {
            get {
                return m_Font;
            }
            set {
                if (m_UserFont && m_Font != null) {
                    m_Font.Dispose();
                }
                m_Font = value;
                m_UserFont = true;
            }
        }
        public Color ForeColor = RibbonThemeManager.TextColor;
        /// <summary>
        /// 显示的项
        /// </summary>
        public List<RibbonListViewItem> Items = new List<RibbonListViewItem>();
        /// <summary>
        /// Group的所有项
        /// </summary>
        internal List<RibbonListViewItem> m_AllItems = new List<RibbonListViewItem>();
        public int GroupIndex;

        public void AddItem(RibbonListViewItem item) {
            Items.Add(item);
            m_AllItems.Add(item);
            //item.Level = 1; //group.Level+1
            item.Group = this;
        }
        public void RemoveItem(RibbonListViewItem item) {
            Items.Remove(item);
            m_AllItems.Remove(item);
            item.Group = null;
        }
        //public void RemoveAllItem() {
        //    Items.Clear();
        //    m_AllItems.Clear();
        //}
        /// <summary>
        /// 是否处于收起状态
        /// </summary>
        public bool IsCollapse = false;
        public void Collapse() {
            IsCollapse = true;
        }
        public void Expand() {
            IsCollapse = false;
        }

        private Brush TextBrush;
        public RibbonListViewGroup() {
            TextBrush = new SolidBrush(ForeColor);
            Font = new Font(RibbonThemeManager.NormalFont, FontStyle.Bold);
            m_UserFont = true;
        }
        public virtual void OnPaint(PaintEventArgs e) {
            int xoffset = 0;//如果以图标的形式,则给文本留个空位.
            if (m_CanVisible) {
                if (BackImage != null) {
                    Rectangle rect;
                    if (Items.Count > 0) {
                        rect = IsCollapse ? CollapseRect : ExpandRect;
                    }
                    else {
                        rect = NoDataRect;
                    }
                    //画状态
                    Rectangle stateRect = new Rectangle(Rect.Left, Rect.Y - ListView.CurPosition.Y, 24, 24);// NoDataRect.Width, NoDataRect.Height);
                    e.Graphics.DrawImage(BackImage, stateRect, rect, GraphicsUnit.Pixel);
                    //画背景
                    //Rectangle bakDescRect = Rect;
                    //bakDescRect.Y -= ListView.CurPosition.Y;
                    //e.Graphics.DrawImage(BackImage, bakDescRect, BackRect, GraphicsUnit.Pixel);
                    SizeF size = e.Graphics.MeasureString(Name, Font);

                    if (this.ListView.ItemAutoSize && size.Width > Rect.Width - stateRect.Width) {
                        //换行
                        RectangleF rectf = new RectangleF(stateRect.Width + Rect.Left,
                            Rect.Top - ListView.CurPosition.Y,
                            Rect.Width - stateRect.Width,
                            Rect.Height);

                        e.Graphics.DrawString(Name, Font, TextBrush, rectf);
                    }
                    else {

                        e.Graphics.DrawString(/*(this.IsCollapse ? "+" : "-") +*/ Name, Font, TextBrush, stateRect.Width + Rect.Left, (float)Math.Ceiling(Rect.Top + (Rect.Height - size.Height) * 0.5f) - ListView.CurPosition.Y);
                    }

                }
                else {
                    SizeF size = e.Graphics.MeasureString(Name, Font);
                    if (this.ListView.ItemAutoSize && size.Width > Rect.Width - NoDataRect.Width) {
                        //换行
                        RectangleF rectf = new RectangleF(NoDataRect.Width + Rect.Left,
                            Rect.Top - ListView.CurPosition.Y,
                            Rect.Width - NoDataRect.Width,
                            Rect.Height);

                        e.Graphics.DrawString(Name, Font, TextBrush, rectf);
                    }
                    else {
                        e.Graphics.DrawString(/*(this.IsCollapse ? "+" : "-") +*/ Name, Font, TextBrush, NoDataRect.Width + Rect.Left, (float)Math.Ceiling(Rect.Top + (Rect.Height - size.Height) * 0.5f) - ListView.CurPosition.Y);
                    }
                }
            }

        }

        void ListView_SizeChanged(object sender, EventArgs e) {
            Rect = new Rectangle(0, Rect.Y, ListView.GetClientRect().Width, Height);
            GroupRect = new Rectangle(0, GroupRect.Y, ListView.GetClientRect().Width, GroupRect.Height);
        }
        /// <summary>
        /// 是否位于可视区域
        /// </summary>
        internal bool m_CanVisible = false;
        internal void ReComputerPosAndSize() {

        }
        /// <summary>
        /// 更新Group组的内容Rect.
        /// </summary>
        internal void UpdateGroupDataRect() {
            if (ListView.RibbonImageLayout == RibbonImageLayout.Table) {
                //行数*行高即可
                int c = 0;
                foreach (var v in Items) {
                    if (v.Visble) {
                        c++;
                    }
                }
                int lines = (int)Math.Ceiling((float)c / ListView.ColumnCount);
                GroupRect = new Rectangle(0, Rect.Bottom, ListView.GetClientRect().Width, lines * (ListView.RowHeight + ListView.RowSp));
            }
            else {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取Group的整个大小.
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetAllRect() {
            if (!IsCollapse) {
                return new Rectangle(0, Rect.Y, ListView.GetClientRect().Width, Rect.Height + GroupRect.Height);
            }
            else {
                return Rect;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            if (m_Disposed) {
                return;
            }

            if (TextBrush != null) {
                TextBrush.Dispose();
            }
            if (Font != null && m_UserFont && !RibbonThemeManager.IsCacheFont(Font)) {
                Font.Dispose();
                Font = null;
            }
            if (m_AllItems.Count > 0) {
                for (int i = 0; i < m_AllItems.Count; i++) {
                    RibbonListViewItem item = m_AllItems[0];
                    item.Dispose();
                }
            }
            m_AllItems.Clear();
            Items.Clear();
            if (BackImage != null) {
                BackImage.Dispose();
            }
            ListView = null;
            m_Disposed = true;
        }
        private bool m_Disposed = false;
        ~RibbonListViewGroup() {
            Dispose(false);
        }
    }
}
