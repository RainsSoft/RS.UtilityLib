using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// Panel
    /// -支持  背景色，背景图(支持分割) 
    /// </summary>
    public class RibbonPanel : Panel, RibbonUISkin
    {

        private RibbonScrollbar m_VScroll;
        private RibbonScrollbar m_HScroll;

        public RibbonPanel() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Margin = new Padding(0);
            base.Padding = new Padding(0);
            base.Size = new Size(100, 50);
            IsBackImageCanScale = false;
            //
        }
        //3d tag cloud
        bool m_show3dTagCloud = false;
        public void SetShow3DTag(bool show3dtag) {
            m_show3dTagCloud = show3dtag;
        }
        List<string> m_3dTags;
        public void SetTagString(List<string> tags3d) {
        }
        protected override void OnPaint(PaintEventArgs e) {

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);

            e.Graphics.Clear(RibbonTabControl.GetColor(1.031));
            using (Pen p = new Pen(RibbonTabControl.GetColor(0.946))) {
                e.Graphics.DrawRectangle(p, rect);
            }

            //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
            switch (this.m_SetFlag) {
                case 0://默认：为原始的，我们不做修改
                    break;
                case 1://图片支持  
                    DrawPicture(e);
                    break;
                case 2: //渐变色支持
                    DrawColor(e);
                    break;
                case 3: //图片与渐变色都支持
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
                if (base.Enabled) {
                    if (this.IsBackImageCanScale) {
                        RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_NormalRect, true);
                    }
                    else {
                        e.Graphics.DrawImage(this.Image, this.ClientRectangle);
                    }
                }
            }
        }



        /// <summary>
        /// 创建滚动条
        /// </summary>
        private void CreateScrollBar() {
            m_HScroll = new RibbonScrollbar();
            m_VScroll = new RibbonScrollbar();
            m_HScroll.Orientation = ScrollOrientation.HorizontalScroll;
            m_VScroll.Orientation = ScrollOrientation.VerticalScroll;

            m_HScroll.BindControl = this;
            m_VScroll.BindControl = this;

            RibbonScrollbar.UpdateHVScroll(m_HScroll, m_VScroll);
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

        protected override void OnSizeChanged(EventArgs e) {
            this.Invalidate();
            base.OnSizeChanged(e);
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 133) {
                this.Invalidate();
            }
            base.WndProc(ref m);
        }
        #region UISKin 成员
        /// <summary>
        /// 是否在panel后缩放要绘制的图片 
        /// </summary>
        public bool IsBackImageCanScale {
            get;
            set;
        }
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
                this.Invalidate();
                //
            }
        }
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                //base.BackgroundImage = value;
            }
        }
        //
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
