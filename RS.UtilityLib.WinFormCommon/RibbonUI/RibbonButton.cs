using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// Button 图(支持分割) 
    /// </summary>
    public class RibbonButton : Label, RibbonUISkin
    {
        #region
        //private IUIConfig m_config;

        private bool hover;
        private bool isflat;
        private bool ispressed;
        private bool pressed;
        private bool m_CheckButton = false;
        private bool m_Checked = false;
        public event RibbonPopupEventHandler OnPopup;

        public bool CheckButton {
            get { return m_CheckButton; }
            set { m_CheckButton = value; }
        }
        [System.ComponentModel.Browsable(false)]
        public bool Checked {
            get { return m_Checked; }
            set { m_Checked = value; }
        }
        public RibbonButton() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Margin = new Padding(1);
            base.Padding = new Padding(2);
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.isflat = false;
            this.ispressed = false;

        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            if (this.OnPopup != null) {
                this.OnPopup(this);
            }
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            this.AutoSize = false;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            this.pressed = true;
            base.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            this.hover = true;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            this.hover = false;
            base.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            this.m_Checked = !this.m_Checked;
            if (this.m_CheckButton && this.m_Checked) {
                this.ispressed = true;
                foreach (var c in this.Parent.Controls) {
                    if (c is RibbonButton && (c as RibbonButton).CheckButton && c != this) {
                        (c as RibbonButton).Checked = false;
                        (c as RibbonButton).IsPressed = false;
                        (c as RibbonButton).Invalidate();
                    }
                }
            }
            else {
                this.ispressed = false;
            }
            this.pressed = false;
            base.Invalidate();
        }
        //protected override void OnEnabledChanged(EventArgs e) {
        //    if (this.m_UserImage != null) {
        //        Graphics g = Graphics.FromImage(this.Image);
        //        if (this.Enabled) {
        //            g.DrawImage(m_UserImage, 0, 0, m_NormalRect, GraphicsUnit.Pixel);
        //        }
        //        else {
        //            g.DrawImage(m_UserImage, 0, 0, m_DisableRect, GraphicsUnit.Pixel);
        //        }
        //        g.Dispose();
        //    }
        //    base.OnEnabledChanged(e);
        //}
        protected override void OnPaint(PaintEventArgs e) {
            if (this.IsDisposed) {
                return;
            }
            /*
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);

            //if (this.Image != null) {
            //    if (this.Enabled) {
            //        e.Graphics.DrawImage(this.Image, 0, 0, this.m_NormalRect, GraphicsUnit.Pixel);
            //    }
            //    else {
            //        e.Graphics.DrawImage(this.Image, 0, 0, this.m_DisableRect, GraphicsUnit.Pixel);
            //    }
            //}

            if (!this.isflat) {
                rectangle = DrawFlatButton(e, rectangle);

            }
            base.OnPaint(e);
            if (this.hover) {
                RibbonTabControl.RenderSelection(e.Graphics, rectangle, 2f, this.pressed);
            }
            else if (this.ispressed) {
                RibbonTabControl.RenderSelection(e.Graphics, rectangle, 2f, true);
            }


            return;
            */

            //画底图
            //base.OnPaint(e);
            //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
            //bool goBack = false;
            Rectangle rectangle = new Rectangle(0, 0, base.Width, base.Height);
            switch (this.m_SetFlag) {
                case 0://默认：为原始的，我们不做修改
                    if (!this.isflat) {
                        rectangle = DrawFlatButton(e, rectangle);
                    }
                    if (this.hover) {
                        RibbonTabControl.RenderSelection(e.Graphics, rectangle, 2f, this.pressed);
                    }
                    else if (this.ispressed) {
                        RibbonTabControl.RenderSelection(e.Graphics, rectangle, 2f, true);
                    }
                    break;
                case 1://图片支持
                    DrawPicture(e);
                    //base.OnPaint(e);
                    break;
                case 2: //渐变色支持
                    DrawColor(e);
                    //base.OnPaint(e);
                    break;
                case 3: //图片与渐变色都支持
                    DrawPicture(e);
                    DrawColor(e);
                    //base.OnPaint(e);
                    break;
                default:
                    //待扩展
                    break;
            }


        }
        protected override void Dispose(bool disposing) {
            //if (m_UserImage != null) {
            //    m_UserImage.Dispose();
            //    m_UserImage = null;
            //}
            if (this.Image != null) {
                this.Image.Dispose();
                this.Image = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnSizeChanged(EventArgs e) {
            /*
             * 采用这样的方式,在初期加载阶段因为会调用多次,影响效率.
             * 
             * 
             */
            if (!this.isflat) {
                if (m_FillBrush != null) {
                    m_FillBrush.Dispose();
                }
                if (m_EdgeBrush != null) {
                    m_EdgeBrush.Dispose();
                }
                Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
                m_FillBrush = new LinearGradientBrush(
                    rect,
                    Color.Transparent,
                    Color.Transparent,
                    LinearGradientMode.Vertical);

                m_EdgeBrush = new LinearGradientBrush(
                    rect,
                    RibbonTabControl.GetColor(1.05),
                    Color.Transparent,
                    LinearGradientMode.ForwardDiagonal);
            }
            base.OnSizeChanged(e);
        }
        private LinearGradientBrush m_FillBrush;
        private LinearGradientBrush m_EdgeBrush;
        private Pen m_EdgePen;
        private Pen m_Pen;
        private Rectangle DrawFlatButton(PaintEventArgs e, Rectangle rectangle) {
            Color[] colorArray;
            float[] numArray;
            ColorBlend blend;

            Color[] colorArray2;

            colorArray2 = new Color[] { RibbonTabControl.GetColor(1.025), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(0.975), RibbonTabControl.GetColor(1.075) };
            colorArray = colorArray2;
            numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
            blend = new ColorBlend();
            blend.Colors = colorArray;
            blend.Positions = numArray;
            //brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);

            m_FillBrush.InterpolationColors = blend;
            RibbonTabControl.FillRoundRectangle(e.Graphics, m_FillBrush, rectangle, 2f);

            if (m_Pen == null) {
                m_Pen = new Pen(RibbonTabControl.GetColor(0.75));
            }
            else {
                m_Pen.Color = RibbonTabControl.GetColor(0.75);
            }
            RibbonTabControl.DrawRoundRectangle(e.Graphics, m_Pen, rectangle, 2f);
            rectangle.Offset(1, 1);
            rectangle.Width -= 2;
            rectangle.Height -= 2;


            m_EdgeBrush.LinearColors[0] = RibbonTabControl.GetColor(1.05);
            m_EdgeBrush.LinearColors[1] = Color.Transparent;

            if (m_EdgePen == null) {
                m_EdgePen = new Pen(m_EdgeBrush);
            }
            else {
                m_EdgePen.Brush = m_EdgeBrush;
            }
            RibbonTabControl.DrawRoundRectangle(e.Graphics,
                m_EdgePen,
                rectangle, 2f);

            return rectangle;
        }
        protected override void DestroyHandle() {
            if (m_EdgeBrush != null) {
                m_EdgeBrush.Dispose();
                m_EdgeBrush = null;
            }
            if (m_EdgePen != null) {
                m_EdgePen.Dispose();
                m_EdgePen = null;
            }
            if (m_Pen != null) {
                m_Pen.Dispose();
                m_Pen = null;
            }
            if (m_FillBrush != null) {
                m_FillBrush.Dispose();
                m_FillBrush = null;
            }

            base.DestroyHandle();
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
                            if (this.ispressed) {
                                RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_DownRect, true);
                            }
                            else if (this.pressed) {
                                RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_DownRect, true);
                            }
                            else if (this.hover) {
                                RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_HoverRect, true);
                            }
                            else {//判断图片能否截取下来
                                RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_NormalRect, true);
                            }
                        }
                        else {
                            if (this.ispressed) {
                                e.Graphics.DrawImage(this.Image, 0, 0, this.m_DownRect, GraphicsUnit.Pixel);
                            }
                            else if (this.pressed) {
                                e.Graphics.DrawImage(this.Image, 0, 0, this.m_DownRect, GraphicsUnit.Pixel);
                            }
                            else if (this.hover) {
                                e.Graphics.DrawImage(this.Image, 0, 0, this.m_HoverRect, GraphicsUnit.Pixel);
                            }

                            else {//判断图片能否截取下来
                                e.Graphics.DrawImage(this.Image, 0, 0, this.m_NormalRect, GraphicsUnit.Pixel);
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
                        e.Graphics.DrawImage(this.Image, 0, 0, this.m_DisableRect, GraphicsUnit.Pixel);
                        //}
                    }
                }
            }

        }
        public bool IsBackImageCanScale {
            get;
            set;
        }
        public bool IsFlat {
            get {
                return this.isflat;
            }
            set {
                this.isflat = value;
            }
        }

        public bool IsPressed {
            get {
                return this.ispressed;
            }
            set {
                this.ispressed = value;
            }
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

        public string ImagePath {
            get;
            set;
        }
        //private Image m_UserImage;
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
                this.Image = RibbonUISkinHelper.GetPathImage(ImagePath);//
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
