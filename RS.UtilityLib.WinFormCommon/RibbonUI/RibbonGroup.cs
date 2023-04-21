using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    /// <summary>
    /// 背景图 ?
    /// </summary>
    public class RibbonGroup : GroupBox, RibbonUISkin
    {
        #region
        private bool hoverplus;
        private bool pressed;
        public CaptionPosition CaptionPosition {
            get;
            set;
        }
        public event RibbonPopupEventHandler OnPopup;

        public RibbonGroup() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Margin = new Padding(1);
            base.Size = new Size(0x4f, 0x4f);
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            if ((this.OnPopup != null) && this.hoverplus) {
                this.OnPopup(this);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            this.pressed = true;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            this.hoverplus = false;
            base.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            Rectangle rectangle = new Rectangle(base.Width - 20, base.Height - 20, 0x12, 0x12);
            if (rectangle.Contains(e.X, e.Y)) {
                this.hoverplus = true;
            }
            else {
                this.hoverplus = false;
            }
            base.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            this.pressed = false;
            base.Invalidate();
        }
        #endregion
        #region paint
        Pen m_Pen;
        Pen m_Pen2;
        SolidBrush m_ContBrush;
        SolidBrush m_TextBrush;
        // SolidBrush m_
        StringFormat m_TexFormat;

        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rectangle;
            Rectangle rectangle2;
            Rectangle rectangle3;
            GraphicsPath path;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;


            if (this.CaptionPosition == CaptionPosition.Bottom) {
                //底部
                rectangle = new Rectangle(1, 1, base.Width - 2, base.Height - 2);
                rectangle2 = new Rectangle(0, base.Height - 0x12, base.Width - 2, 0x10);
                rectangle3 = new Rectangle(base.Width - 0x12, base.Height - 0x12, 15, 15);
            }
            else {
                //上部

                rectangle2 = new Rectangle(1, 1, base.Width - 2, 0x10);
                rectangle3 = new Rectangle(base.Width - 0x12, 1, 15, 15);
                rectangle = new Rectangle(1, 2, base.Width - 2, base.Height - 2);

            }
            if (m_Pen == null) {
                m_Pen = new Pen(RibbonTabControl.GetColor(1.05));
            }
            else {
                m_Pen.Color = RibbonTabControl.GetColor(1.05);
            }
            RibbonTabControl.DrawRoundRectangle(e.Graphics, m_Pen, rectangle, 3f);

            //开始画中间容纳部分
            path = new GraphicsPath();
            path.AddArc((float)rectangle2.X, rectangle2.Y - 4f, 6f, 6f, 180f, 90f);
            path.AddArc((float)((rectangle2.X + rectangle2.Width) - 6f), (float)(rectangle2.Y - 5f), (float)6f, (float)6f, 270f, 90f);
            path.AddArc((float)((rectangle2.X + rectangle2.Width) - 6f), (float)((rectangle2.Y + rectangle2.Height) - 6f), (float)6f, (float)6f, 0f, 90f);
            path.AddArc((float)rectangle2.X, (rectangle2.Y + rectangle2.Height) - 6f, 6f, 6f, 90f, 90f);
            path.CloseFigure();
            e.Graphics.SetClip(path, CombineMode.Intersect);
            path.Dispose();
            if (m_ContBrush == null) {
                m_ContBrush = new SolidBrush(RibbonTabControl.GetColor(0.975));
            }
            else {
                m_ContBrush.Color = RibbonTabControl.GetColor(0.975);
            }
            e.Graphics.FillRectangle(m_ContBrush, rectangle2);
            // e.Graphics.FillRectangle(new SolidBrush(Color.Red), rectangle2);


            e.Graphics.Clip = new Region();


            //
            //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
            switch (this.m_SetFlag) {
                case 0://默认：为原始的，我们不做修改
                    break;
                case 1://图片支持  
                    DrawPicture(e);
                    break;
                case 2: //渐变色支持
                    rectangle = DrawColor(e, rectangle);
                    break;
                case 3: //图片与渐变色都支持
                    DrawPicture(e);
                    DrawColor(e, rectangle);
                    break;
                default:
                    //待扩展
                    break;
            }

            //开始画文字部分
            if (m_TexFormat == null) {
                m_TexFormat = new StringFormat();
                m_TexFormat.Alignment = StringAlignment.Center;
                m_TexFormat.LineAlignment = StringAlignment.Center;
            }
            if (m_TextBrush == null) {
                m_TextBrush = new SolidBrush(RibbonThemeManager.TextColor);
            }
            else {
                m_TextBrush.Color = RibbonThemeManager.TextColor;
            }
            e.Graphics.DrawString(this.Text, this.Font, m_TextBrush, rectangle2, m_TexFormat);
            if (this.OnPopup != null) {
                if (this.hoverplus) {
                    RibbonTabControl.RenderSelection(e.Graphics, rectangle3, 2f, this.pressed);
                }
                using (Brush b = new SolidBrush(this.ForeColor)) {
                    e.Graphics.DrawString("+", this.Font, b, rectangle3, m_TexFormat);
                }
            }
            rectangle.Offset(0, -1);
            //rectangle.Offset(1, -1);

            if (m_Pen2 == null) {
                m_Pen2 = new Pen(RibbonTabControl.GetColor(0.85));
            }
            else {
                m_Pen2.Color = RibbonTabControl.GetColor(0.85);
            }
            RibbonTabControl.DrawRoundRectangle(e.Graphics, m_Pen2, rectangle, 3f);
            rectangle.Offset(1, -1);
            e.Graphics.Clip.Dispose();
        }
        protected override void Dispose(bool disposing) {
            if (m_Pen != null) {
                m_Pen.Dispose();
                m_Pen = null;
            }
            if (m_Pen2 != null) {
                m_Pen2.Dispose();
                m_Pen2 = null;
            }
            if (m_ContBrush != null) {
                m_ContBrush.Dispose();
                m_ContBrush = null;
            }
            if (m_TextBrush != null) {
                m_TextBrush.Dispose();
                m_TextBrush = null;
            }
            if (m_TexFormat != null) {
                m_TexFormat.Dispose();
                m_TexFormat = null;
            }
            base.Dispose(disposing);
        }
        private Rectangle DrawColor(PaintEventArgs e, Rectangle rectangle) {
            if (this.ColorUserSet) {
                //使用自己设置的颜色,这样就不受themManager的统一管理
                RibbonUISkinHelper.DrawLinearColor(e.Graphics, rectangle, this.m_ColorStart, this.m_ColorEnd, this.m_ColorLinearAngle);
            }
            else {
                //
                Color endColor = ControlPaint.Light(RibbonThemeManager.BackColor, this.m_ColorFactor);
                RibbonUISkinHelper.DrawLinearColor(e.Graphics, rectangle, endColor, RibbonThemeManager.BackColor, this.m_ColorLinearAngle);
            }
            return rectangle;
        }

        private void DrawPicture(PaintEventArgs e) {
            if (this.Image != null) {
                if (base.Enabled) {
                    if (this.RectNormal != Rectangle.Empty) {
                        //if ((this.RectNormal.X + this.RectNormal.Width < this.Image.Width) && this.RectNormal.Y + this.RectNormal.Height < this.Image.Height) {//判断图片能否截取下来
                        RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_NormalRect, true);
                        //}
                    }
                }
                else {
                    //if ((this.RectNormal.X + this.RectNormal.Width < this.Image.Width) && this.RectNormal.Y + this.RectNormal.Height < this.Image.Height) {//判断图片能否截取下来
                    RibbonUISkinHelper.DrawImage(e.Graphics, Image, this.ClientRectangle, this.m_DisableRect, true);
                    //}
                }
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
            if (!string.IsNullOrEmpty(ImagePath) && this.RectNormal != Rectangle.Empty && this.RectDisable != Rectangle.Empty) {
                this.Image = RibbonUISkinHelper.GetPathImage(ImagePath);//可能不需要支持image

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
