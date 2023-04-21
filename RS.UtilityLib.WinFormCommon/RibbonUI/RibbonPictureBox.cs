using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// 1：支持单张图片显示，鼠标移上高亮显示
    /// 2：支持多张图片放一张图片中，鼠标移上切换不同的状态
    /// </summary>
    public class RibbonPictureBox : PictureBox, RibbonUISkin
    {
        #region
        private bool m_Hover;
        private bool m_isPressed;




        public bool IsUseSkin {
            get {
                return m_SetFlag != 0;
            }
        }
        public RibbonPictureBox() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Margin = new Padding(0);
            base.Size = new Size(0x40, 0x30);
            this.m_isPressed = false;
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            this.AutoSize = false;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            this.m_isPressed = true;
            base.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            this.m_Hover = true;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            this.m_Hover = false;
            base.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            this.m_isPressed = false;
            base.Invalidate();
        }
        #endregion
        #region paint

        protected override void OnPaint(PaintEventArgs e) {
            //this.BorderStyle = BorderStyle.None;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rectangle = new Rectangle(0, 0, base.Width, base.Height);
            //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
            switch (this.m_SetFlag) {
                case 0://默认：为原始的，我们不做修改
                    rectangle = DrawDefault(e, rectangle);
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

        private Rectangle DrawDefault(PaintEventArgs e, Rectangle rectangle) {
            if (rectangle.IntersectsWith(e.ClipRectangle)) {
                if (this.m_Hover) {
                    RibbonTabControl.RenderSelection(e.Graphics, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width - 2, rectangle.Height - 2), 2f, this.m_isPressed);
                }
                else if (this.m_isPressed) {
                    RibbonTabControl.RenderSelection(e.Graphics, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width - 2, rectangle.Height - 2), 2f, true);
                }
                if ((base.Image != null)) {
                    if (base.Enabled) {
                        e.Graphics.DrawImage(base.Image, 4, 4, (int)(base.Width - 8), (int)(base.Height - 8));
                    }
                    else {
                        ControlPaint.DrawImageDisabled(e.Graphics, base.Image, 4, 4, Color.Transparent);
                    }
                    //
                }
            }
            return rectangle;
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
                    if (this.RectNormal != Rectangle.Empty) {
                        if (this.IsBackImageCanScale) {
                            if (this.m_Hover) {
                                if (this.m_isPressed) {
                                    RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_DownRect, true);
                                }
                                else {
                                    RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_HoverRect, true);
                                }
                            }

                            else {//判断图片能否截取下来
                                RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_NormalRect, true);
                            }
                        }
                        else {
                            if (this.m_Hover) {
                                if (this.m_isPressed) {
                                    e.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.m_DownRect.Width, this.m_DownRect.Height), this.m_DownRect, GraphicsUnit.Pixel);
                                }
                                else {
                                    e.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.m_HoverRect.Width, this.m_HoverRect.Height), this.m_HoverRect, GraphicsUnit.Pixel);
                                }
                            }
                            else {//判断图片能否截取下来
                                e.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.m_NormalRect.Width, this.m_NormalRect.Height), this.m_NormalRect, GraphicsUnit.Pixel);
                                //e.Graphics.DrawImage(this.Image, 0, 0, this.m_NormalRect, GraphicsUnit.Pixel);
                            }
                        }
                    }
                }
                else {
                    if (this.IsBackImageCanScale) {
                        //if ((this.RectNormal.X + this.RectNormal.Width < this.Image.Width) && this.RectNormal.Y + this.RectNormal.Height < this.Image.Height) {//判断图片能否截取下来
                        RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_DisableRect, true);
                        //}
                    }
                    else {
                        //if ((this.RectNormal.X + this.RectNormal.Width < this.Image.Width) && this.RectNormal.Y + this.RectNormal.Height < this.Image.Height) {//判断图片能否截取下来
                        e.Graphics.DrawImage(this.Image, this.ClientRectangle, this.m_DisableRect, GraphicsUnit.Pixel);
                        //}
                    }
                }
            }
        }

        public bool IsBackImageCanScale {
            get;
            set;
        }
        public bool IsPressed {
            get {
                return this.m_isPressed;
            }
            set {
                this.m_isPressed = value;
            }
        }
        #endregion
        //如果起用皮肤 完成图片的切换 normal       
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
        //private Image TopImage {
        //    get;
        //    set;
        //}

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
            if (!string.IsNullOrEmpty(ImagePath) && this.RectNormal != Rectangle.Empty && this.RectDisable != Rectangle.Empty) {
                this.Image = RibbonUISkinHelper.GetPathImage(ImagePath);
                this.Invalidate();
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
        private bool m_UseUserColor = false;
        /// <summary>
        /// 是否自己设置颜色 默认为false
        /// </summary>
        [Description("是否自己设置颜色 默认为false")]
        public virtual bool ColorUserSet {
            get {
                return m_UseUserColor;
            }
            set {
                m_UseUserColor = value;
            }
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
