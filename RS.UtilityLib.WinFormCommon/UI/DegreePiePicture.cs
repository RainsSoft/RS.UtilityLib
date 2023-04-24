using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public delegate void DegreePieChangeHandle(object sender, float degree);

    /// <summary>
    /// 角度拖动盘.可鼠标点击目标角度或者拖动.
    /// </summary>
    public partial class DegreePiePicture : UserControl
    {
        /// <summary>
        /// 移动更改
        /// </summary>
        public event DegreePieChangeHandle BeforeChange;
        /// <summary>
        /// 移动完成更改
        /// </summary>
        public event DegreePieChangeHandle AfterChange;
        /// <summary>
        /// 按下时更新事件
        /// </summary>
        public event DegreePieChangeHandle SelectChange;
        //
        private Font m_DegreeFont = new Font("宋体", 10);  
        private HorizontalAlignment m_DegreeTextAlign = HorizontalAlignment.Center;
        public enum VerticalAlignment { 
            Top,
            Center,
            Bottom
        }
        private VerticalAlignment m_DegreeTextAlignVertical = VerticalAlignment.Center;
        [Browsable(true)]
        public VerticalAlignment DegreeTextAlignVertical {
            get {
                return m_DegreeTextAlignVertical;
            }
            set {
                m_DegreeTextAlignVertical = value;
                this.Invalidate();
            }
        }
      
        /// <summary>
        /// 角度文字字体
        /// </summary>
        public Font DegreeFont {
            get {
                return m_DegreeFont;
            }
            set {
                m_DegreeFont = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// 角度文字对齐方式.目前只MiddleCenter有效,其他方式,文字统一位于...
        /// </summary>
        [Browsable(true)]
        public HorizontalAlignment DegreeTextAlign {
            get {
                return m_DegreeTextAlign;
            }
            set {
                m_DegreeTextAlign = value;
                this.Invalidate();
            }
        }
        private Color m_DegreeColor = Color.Blue;
        /// <summary>
        /// 角度文字颜色
        /// </summary>
        [Browsable(true),DefaultValue(true)]
        public Color DegreeColor {
            get { return m_DegreeColor; }
            set {
                //if (m_DegreeSB == null) {
                //    m_DegreeSB = new SolidBrush(value);
                //}
                //m_DegreeSB.Color = value;
                m_DegreeColor = value;
                this.Invalidate();
            }
        }
        private string m_Text = "X:";
        [Description("显示文本内容")]
        public new string Text {
            get { return m_Text; }
            set { 
                m_Text = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// 前端图片
        /// </summary>
        [Browsable(true)]
        public Image FrontImage {
            get;
            set;
        }
        /// <summary>
        /// 是否显示饼图,默认为true
        /// </summary>
        [Browsable(true), Description("显示饼图,默认为true")]
        public bool ShowPie {
            get;
            set;
        } = true;
        /// <summary>
        /// 是否显示角度文字,默认为true
        /// </summary>
        [Browsable(true), Description("是否显示角度,默认为true")]
        public bool ShowText {
            get;
            set;
        } = true;
        /// <summary>
        /// 目标角度
        /// </summary>
        [Browsable(true)]
        public float Degree {
            get { return m_Degree; }
            set {
                //if (this.BeforeChange != null) {
                //    this.BeforeChange(this, m_Degree);
                //}
                this.m_Degree = value % 180f;
                this.Invalidate();
                //if (this.AfterChange != null) {
                //    this.AfterChange(this, m_Degree);
                //}
            }
        }

        public DegreePiePicture() {
            InitializeComponent();
            //           
            //      
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            m_Image = this.BackgroundImage;
            this.BackgroundImage = null;

        }
        protected float m_Degree = 0f;
        protected Image m_Image;
        //protected SolidBrush m_DegreeSB;
        protected override void OnMouseMove(MouseEventArgs e) {

            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {

                if (this.BeforeChange != null) {
                    this.BeforeChange(this, m_Degree);
                }
                float r = this.Width * 0.5f;
                PointF center = new PointF(r, r);

                float dx = e.X - center.X;
                float dy = e.Y - center.Y;
                float l = (float)Math.Sqrt(dx * dx + dy * dy);
                float alpha = (float)Math.Asin((float)Math.Abs(dy) / l);

                m_Degree = alpha / 3.1415926f * 180f;
                if (dy > 0) {
                    m_Degree = 90 + m_Degree;
                }
                else {

                    m_Degree = 90 - m_Degree;
                }
                if (dx < 0) {
                    m_Degree = -m_Degree;
                }
                this.Invalidate();
                //if (this.AfterChange != null) {
                //    this.AfterChange(this, m_Degree);
                //}
            }

        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (this.AfterChange != null) {
                    this.AfterChange(this, m_Degree);
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (SelectChange != null) {
                    SelectChange(this, 0f);
                    this.Invalidate();
                }
                if (this.BeforeChange != null) {
                    this.BeforeChange(this, m_Degree);
                }

                float r = this.Width * 0.5f;
                PointF center = new PointF(r, r);

                float dx = e.X - center.X;
                float dy = e.Y - center.Y;
                float l = (float)Math.Sqrt(dx * dx + dy * dy);
                float alpha = (float)Math.Asin((float)Math.Abs(dy) / l);

                m_Degree = alpha / 3.1415926f * 180f;
                if (dy > 0) {
                    m_Degree = 90 + m_Degree;
                }
                else {

                    m_Degree = 90 - m_Degree;
                }
                if (dx < 0) {
                    m_Degree = -m_Degree;
                }
                this.Invalidate();

                //if (this.AfterChange != null) {
                //    this.AfterChange(this, m_Degree);
                //}
            }
        }



        protected override void OnPaint(PaintEventArgs e) {

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; //|
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            //base.OnPaint(e);
            DrawDegree(e.Graphics);
            //DrawFrontImage(e.Graphics, this.m_Degree);
            if (this.ShowPie) {
                DrawPie(e.Graphics, m_Degree, Color.FromArgb(128, Color.Blue));
            }

            if (this.ShowText) {
                DrawText(e.Graphics, this.m_Text + ((float)Math.Round(m_Degree, 1)).ToString() + "°", m_Degree, DegreeColor);
            }


        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            int max = Math.Max(this.Width, this.Height);
            if (this.Width == max && this.Height == max) {
                return;
            }
            else {
                this.SuspendLayout();
                this.Size = new Size(max, max);
                this.ResumeLayout();
            }
        }
        private void DrawDegree(Graphics g) {

            //15度画一小点.30度画一大点
            float r = this.Width * 0.5f;
            PointF center = new PointF(r, r);
            r = r - 4;
            int k = 0;
            for (int i = 0; i < 360; i += 15) {
                float x = r * (float)Math.Sin(i / 180f * 3.1415926f);
                float y = r * (float)Math.Cos(i / 180f * 3.1415926f);
                PointF p = new PointF(x + center.X, center.Y - y);
                SolidBrush b = new SolidBrush(Color.Black);
                if (k % 2 != 0) {
                    b.Color = Color.Gray;
                    g.FillEllipse(b, p.X - 4, p.Y - 4, 8, 8);
                }
                else {
                    b.Color = Color.Blue;
                    g.FillEllipse(b, p.X - 2, p.Y - 2, 4, 4);
                }
                k++;
                b.Dispose();
            }
        }
        private void DrawPie(Graphics g, float degree, Color color) {


            SolidBrush sb = new SolidBrush(color);
            Pen pen = new Pen(Color.Red, 2);
            Pen pen2 = new Pen(color, 1);


            //g.DrawEllipse(pen, this.Width/4,this.Height/4,this.Width/2,this.Height/2);// 
            g.DrawEllipse(pen2, 1, 1, this.Width-2, this.Height-2);
            //g.TranslateTransform(x1 + (w / 2), y1 + (h / 2));//重新设置坐标原点
            g.FillPie(sb, 0, 0, this.Width, this.Width, -90, degree);
            g.FillEllipse(Brushes.White, this.Width / 2 - 4, this.Height / 2 - 4, 8, 8);//绘制表盘中心
            //
            float r = this.Width * 0.5f;
            PointF center = new PointF(r, r);
            //int k = 0;

            float x = r * (float)Math.Sin(this.m_Degree / 180f * 3.1415926f);
            float y = r * (float)Math.Cos(this.m_Degree / 180f * 3.1415926f);
            int offsetx = 1, offsety = 1;
            if (x < 0)
                offsetx = -1;
            if (y < -0)
                offsety = -1;
            PointF p = new PointF(x + 1 * offsetx + center.X, center.Y - y + 1 * offsety);

            pen.EndCap = LineCap.ArrowAnchor;

            g.DrawLine(pen, center, new PointF(center.X, 0));
            g.DrawLine(pen, center, p);

            pen.Dispose();
            pen2.Dispose();
            sb.Dispose();

        }

        private void DrawFrontImage(Graphics g, float degree) {
            if (this.FrontImage == null) {
                return;
            }

            float r = this.Width * 0.5f;
            Matrix m = new Matrix();
            m.Translate(r, r);
            m.Rotate(degree);
            //m.Scale(((float)this.Width) / ((float)m_Image.Width), ((float)this.Height) / ((float)m_Image.Height));
            g.Transform = m;
            m.Dispose();
            g.DrawImage(this.FrontImage, -this.FrontImage.Width * 0.5f, -this.FrontImage.Height * 0.5f);
            g.ResetTransform();
        }

        private void DrawText(Graphics g, string txt, float degree, Color c) {
            float r = this.Width * 0.5f;
            PointF center = new PointF(r, r);
            float x = 8;
            float y = 0;
            var size= TextRenderer.MeasureText(txt, this.DegreeFont);
            if (this.DegreeTextAlignVertical == VerticalAlignment.Center) {
                //x = 0;// r - size.Width * 0.5f;
                y = r+4;
            }
            else if (this.DegreeTextAlignVertical == VerticalAlignment.Top) {
                //x = 0;
                y = 8+size.Height;
            }
            else {
                //x = 0;
                y = this.Height - 8-size.Height;
            }
            //if (this.DegreeTextAlign == ContentAlignment.MiddleCenter) {

            //}
            //else {
            //    x = 0.2f * r * (float)Math.Sin(degree / 180f * 3.1415926f * 0.5f);
            //    y = 0.2f * r * (float)Math.Cos(degree / 180f * 3.1415926f * 0.5f);
            //}
            //PointF p = new PointF(x + center.X, center.Y - y);
            //SolidBrush sb = new SolidBrush(c);
            //g.DrawString(txt, this.DegreeFont, this.m_DegreeSB, p);
            //sb.Dispose();            
            var flags = GetFormatFlags(this.DegreeTextAlign);
            Rectangle textRect = new Rectangle((int)x,(int)y,this.Width-(int)x,size.Height);//this.ClientRectangle;
            TextRenderer.DrawText(
                    g,
                    txt,
                    this.DegreeFont,
                    textRect,
                    c,
                    flags);

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

        //-------------------
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {

            if (this.DegreeFont != null) {
                this.DegreeFont.Dispose();
            }
            //if (this.m_DegreeSB != null) {
            //    this.m_DegreeSB.Dispose();
            //}
            if (this.FrontImage != null) {
                this.FrontImage.Dispose();
            }
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);


        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // IRQ_DegreePie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DoubleBuffered = true;
            this.Name = "IRQ_DegreePie";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
