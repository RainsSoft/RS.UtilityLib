using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// FlowLayoutPanel
    /// -支持  背景色，背景图(支持分割) 
    /// </summary>
    public class RibbonFlowPanel : FlowLayoutPanel, RibbonUISkin
    {

        public RibbonScrollbar VScroll;
        #region
        public RibbonFlowPanel() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Margin = new Padding(1);
            base.Padding = new Padding(1);
            base.Size = new Size(100, 50);
            //
        }


        protected override void OnPaint(PaintEventArgs e) {

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
            e.Graphics.Clear(RibbonTabControl.GetColor(1.031));
            using (Pen p = new Pen(Color.Transparent)) {
                e.Graphics.DrawRectangle(p, rect);
            }
            //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
            switch (this.m_SetFlag) {
                case 0://默认：为原始的，我们不做修改
                    break;
                case 1://图片支持
                    //图片支持      
                    DrawPicture(e);
                    break;
                case 2: //渐变色支持
                    DrawColor(e);
                    break;
                case 3: //图片与渐变色都支持
                    //先画图再画渐变色
                    DrawPicture(e);
                    DrawColor(e);
                    break;
                default:
                    //待扩展
                    break;
            }
        }

        private void DrawColor(PaintEventArgs e) {
            if (this.ColorUserSet) {
                //使用自己设置的颜色,这样就不受themManager的统一管理
                RibbonUISkinHelper.DrawLinearColor(e.Graphics, this.ClientRectangle, this.m_ColorStart, this.m_ColorEnd, this.m_ColorLinearAngle);
            }
            else {
                //
                Color endColor = ControlPaint.Dark(RibbonThemeManager.BackColor, this.m_ColorFactor);
                RibbonUISkinHelper.DrawLinearColor(e.Graphics, this.ClientRectangle, RibbonThemeManager.BackColor, endColor, this.m_ColorLinearAngle);
            }
        }

        private void DrawPicture(PaintEventArgs e) {
            if (this.Image != null) {
                if (this.RectNormal != Rectangle.Empty) {
                    RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_NormalRect, true);
                }
                else {
                    e.Graphics.DrawImage(this.Image, this.ClientRectangle);
                }

            }
        }


        private bool m_CreatedScrollBar = false;
        /// <summary>
        /// 创建滚动条
        /// </summary>
        public void CreateScrollBar() {
            //这里不支持
            if (m_CreatedScrollBar) return;
            m_CreatedScrollBar = true;

            RibbonScrollbar ribbonScrollbar1;
            ribbonScrollbar1 = new RibbonScrollbar();

            this.Controls.Add(ribbonScrollbar1);
            ribbonScrollbar1.Dock = DockStyle.Right;
            this.VScroll = ribbonScrollbar1;
            //
            ribbonScrollbar1.BindControl = this;
            ribbonScrollbar1.Scroll += delegate (object sender, ScrollEventArgs e) {
                this.UpdateScroll(e.NewValue);
            };

        }
        /// <summary>
        /// 驱动其更新可见项
        /// </summary>
        /// <param name="NewScrollValue"></param>
        protected void UpdateScroll(int NewScrollValue) {
            //由于SCROLL控件独立出来，这里使用外面来驱动
            int x = this.AutoScrollPosition.X;
            this.AutoScrollPosition = new Point(x, NewScrollValue);
            this.Invalidate();
        }

        #endregion
        protected override void OnSizeChanged(EventArgs e) {
            this.Invalidate();
            base.OnSizeChanged(e);
        }
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

        public string ImagePath {
            get;
            set;
        }
        private Image Image {
            get;
            set;
        }
        public int ImageLayout {
            get;
            set;
        }

        public int DockFlag {
            get;
            set;
        }

        public int ZOrder {
            get;
            set;
        }

        public void UpdateSet() {
            //更新方法
            if (!string.IsNullOrEmpty(ImagePath)) {
                this.Image = RibbonUISkinHelper.GetPathImage(ImagePath);
            }
        }
        //对颜色的支持
        protected int m_SetFlag = 0;//颜色
        [Description("设置标志，0默认1:图片,2:颜色,3:颜色+图片")]
        public virtual int SetFlag {
            get { return m_SetFlag; }
            set { m_SetFlag = value; }
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
            get { return m_ColorLinearAngle; }
            set {
                m_ColorLinearAngle = value;
            }
        }
        #endregion
    }
}
