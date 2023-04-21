using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public class RibbonTab : TabPage, RibbonUISkin
    {
        #region
        public RibbonTab() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
        protected override void OnAutoSizeChanged(EventArgs e) {
            base.OnAutoSizeChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e) {
            //foreach (Control c in this.Controls) {
            //    Control d = c;
            //}
            base.OnSizeChanged(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rectangle;
            Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            Color[] colorArray2;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            /*
            switch (RibbonThemeManager.ColorScheme) {
                case ColorScheme.Blue:
                    e.Graphics.Clear(Color.FromArgb(0xbf, 0xdb, 0xff));
                    rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
                    colorArray2 = new Color[] { Color.FromArgb(0xe1, 0xea, 0xf5), Color.FromArgb(0xd1, 0xdf, 240), Color.FromArgb(0xc7, 0xd8, 0xed), Color.FromArgb(0xe7, 0xf2, 0xff) };
                    colorArray = colorArray2;
                    numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
                    blend = new ColorBlend();
                    blend.Colors = colorArray;
                    blend.Positions = numArray;
                    brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                    brush.InterpolationColors = blend;
                    RibbonTabControl.FillRoundRectangle(e.Graphics, brush, rectangle, 3f);
                    RibbonTabControl.DrawRoundRectangle(e.Graphics, new Pen(Color.FromArgb(0x8d, 0xb2, 0xe3)), rectangle, 3f);
                    rectangle.Offset(1, 1);
                    rectangle.Width -= 2;
                    rectangle.Height -= 2;
                    RibbonTabControl.DrawRoundRectangle(e.Graphics, new Pen(new LinearGradientBrush(rectangle, Color.FromArgb(0xe7, 0xef, 0xf8), Color.Transparent, LinearGradientMode.ForwardDiagonal)), rectangle, 3f);
                    break;

                case ColorScheme.Gray:
                    e.Graphics.Clear(Color.FromArgb(0x53, 0x53, 0x53));
                    rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
                    colorArray2 = new Color[] { Color.FromArgb(210, 0xd6, 0xdd), Color.FromArgb(0xc1, 0xc6, 0xcf), Color.FromArgb(180, 0xbb, 0xc5), Color.FromArgb(0xe7, 240, 0xf1) };
                    colorArray = colorArray2;
                    numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
                    blend = new ColorBlend();
                    blend.Colors = colorArray;
                    blend.Positions = numArray;
                    brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                    brush.InterpolationColors = blend;
                    RibbonTabControl.FillRoundRectangle(e.Graphics, brush, rectangle, 3f);
                    RibbonTabControl.DrawRoundRectangle(e.Graphics, new Pen(Color.FromArgb(190, 190, 190)), rectangle, 3f);
                    rectangle.Offset(1, 1);
                    rectangle.Width -= 2;
                    rectangle.Height -= 2;
                    RibbonTabControl.DrawRoundRectangle(e.Graphics, new Pen(new LinearGradientBrush(rectangle, Color.FromArgb(0xe7, 0xe9, 0xed), Color.Transparent, LinearGradientMode.ForwardDiagonal)), rectangle, 3f);
                    break;

                case ColorScheme.Custom:
                    */
            e.Graphics.Clear(RibbonThemeManager.BackColor);
            rectangle = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
            colorArray2 = new Color[] { RibbonTabControl.GetColor(1.025), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(0.975), RibbonTabControl.GetColor(1.075) };
            colorArray = colorArray2;
            numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
            blend = new ColorBlend();
            blend.Colors = colorArray;
            blend.Positions = numArray;
            using (brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical)) {
                brush.InterpolationColors = blend;
                RibbonTabControl.FillRoundRectangle(e.Graphics, brush, rectangle, 3f);
            }
            using (Pen p = new Pen(RibbonTabControl.GetColor(0.75))) {
                RibbonTabControl.DrawRoundRectangle(e.Graphics, p, rectangle, 3f);
            }
            rectangle.Offset(1, 1);
            rectangle.Width -= 2;
            rectangle.Height -= 2;
            using (Brush b = new LinearGradientBrush(rectangle, RibbonTabControl.GetColor(1.05), Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                using (Pen p = new Pen(b)) {
                    RibbonTabControl.DrawRoundRectangle(e.Graphics, p, rectangle, 3f);
                }
            }
            //增加对图片的支持
            rectangle = DrawPicture(e, rectangle);
            //break;
            // }
            if (this.Parent == null)
                return;
            Rectangle rc = new Rectangle(this.Location, this.Size);
            this.Parent.Invalidate(rc, true);
        }

        private Rectangle DrawPicture(PaintEventArgs e, Rectangle rectangle) {
            if ((this.ImagePath != null) && this.TopImage != null) {
                if (base.Enabled) {
                    if (this.RectNormal != Rectangle.Empty) {
                        //if ((this.RectNormal.X + this.RectNormal.Width < this.TopImage.Width) && this.RectNormal.Y + this.RectNormal.Height < this.TopImage.Height) {
                        e.Graphics.DrawImage(this.TopImage, rectangle, this.RectNormal, GraphicsUnit.Pixel);
                        // }
                    }
                }
                else {
                    //if ((this.RectNormal.X + this.RectNormal.Width < this.TopImage.Width) && this.RectNormal.Y + this.RectNormal.Height < this.TopImage.Height) {
                    e.Graphics.DrawImage(this.TopImage, rectangle, this.RectDisable, GraphicsUnit.Pixel);
                    //}
                }
            }
            return rectangle;
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

        private Image TopImage {
            get;
            set;
        }
        public void UpdateSet() {
            //更新方法
            if (!string.IsNullOrEmpty(ImagePath) && this.RectNormal != Rectangle.Empty && this.RectDisable != Rectangle.Empty) {
                this.TopImage = RibbonUISkinHelper.GetPathImage(ImagePath);
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
