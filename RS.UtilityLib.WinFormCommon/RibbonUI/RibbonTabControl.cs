using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public delegate void RibbonCtrlCollapse(object sender);
    /// <summary>
    /// 如果需要放到容器中,目前只支持一层父亲控件
    /// 支持背景色，背景图(支持分割) 
    /// </summary>
    public class RibbonTabControl : TabControl, RibbonUISkin
    {
        #region
        private int height;
        private int hoverindex;
        private bool pressed;
        private bool m_FirstIsMenu = false;
        public bool FirstTabIsMenu {
            get;
            set;
        }
        /// <summary>
        /// 双击时收缩面板
        /// </summary>
        public bool CollpaseWhenDblClick {
            get;
            set;
        }
        public event RibbonPopupEventHandler OnPopup;
        /// <summary>
        /// 面板收缩事件
        /// </summary>
        public event RibbonCtrlCollapse OnCollapse;
        public RibbonTabControl() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Size = new Size(0x80, 0x80);
            this.height = 0x80;
            this.Dock = DockStyle.Left | DockStyle.Top;
        }

        #region 静态辅助方法

        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
            path.CloseFigure();
            g.DrawPath(pen, path);
            path.Dispose();
        }

        public static void DrawTopRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
            path.CloseFigure();
            g.DrawPath(pen, path);
            path.Dispose();

        }

        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
            path.CloseFigure();
            g.FillPath(brush, path);
            path.Dispose();
        }

        public static void FillTopRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
            path.CloseFigure();
            g.FillPath(brush, path);
            path.Dispose();
        }

        public static Color GetColor(double luminance) {
            Color color = HSL2RGB(((double)RibbonThemeManager.BackColor.GetHue()) / 360.0, ((double)RibbonThemeManager.BackColor.GetSaturation()) / 2.0, RibbonThemeManager.BackColor.GetBrightness() * 1.025);
            double num = color.R * luminance;
            double num2 = color.G * luminance;
            double num3 = color.B * luminance;
            if (num > 255.0) {
                num = 255.0;
            }
            if (num2 > 255.0) {
                num2 = 255.0;
            }
            if (num3 > 255.0) {
                num3 = 255.0;
            }
            return Color.FromArgb((int)num, (int)num2, (int)num3);
        }
        public static Color GetColor(Color c, double luminance) {
            Color color = HSL2RGB(((double)c.GetHue()) / 360.0, ((double)c.GetSaturation()) / 2.0, c.GetBrightness() * 1.025);
            double num = color.R * luminance;
            double num2 = color.G * luminance;
            double num3 = color.B * luminance;
            if (num > 255.0) {
                num = 255.0;
            }
            if (num2 > 255.0) {
                num2 = 255.0;
            }
            if (num3 > 255.0) {
                num3 = 255.0;
            }
            return Color.FromArgb((int)num, (int)num2, (int)num3);
        }

        public static Color HSL2RGB(double hue, double saturation, double luminance) {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            if (luminance == 0.0) {
                num = num2 = num3 = 0.0;
            }
            else if (saturation == 0.0) {
                num = num2 = num3 = luminance;
            }
            else {
                double num4 = (luminance <= 0.5) ? (luminance * (1.0 + saturation)) : ((luminance + saturation) - (luminance * saturation));
                double num5 = (2.0 * luminance) - num4;
                double[] numArray = new double[] { hue + 0.33333333333333331, hue, hue - 0.33333333333333331 };
                double[] numArray2 = new double[3];
                for (int i = 0; i < 3; i++) {
                    if (numArray[i] < 0.0) {
                        numArray[i]++;
                    }
                    if (numArray[i] > 1.0) {
                        numArray[i]--;
                    }
                    if ((6.0 * numArray[i]) < 1.0) {
                        numArray2[i] = num5 + (((num4 - num5) * numArray[i]) * 6.0);
                    }
                    else if ((2.0 * numArray[i]) < 1.0) {
                        numArray2[i] = num4;
                    }
                    else if ((3.0 * numArray[i]) < 2.0) {
                        numArray2[i] = num5 + (((num4 - num5) * (0.66666666666666663 - numArray[i])) * 6.0);
                    }
                    else {
                        numArray2[i] = num5;
                    }
                }
                num = numArray2[0];
                num2 = numArray2[1];
                num3 = numArray2[2];
            }
            if (num > 1.0) {
                num = 1.0;
            }
            if (num2 > 1.0) {
                num2 = 1.0;
            }
            if (num3 > 1.0) {
                num3 = 1.0;
            }
            return System.Drawing.Color.FromArgb((int)(255.0 * num), (int)(255.0 * num2), (int)(255.0 * num3));
        }
        public static void RenderSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
            Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            Color[] colorArray2;
            if (pressed) {
                //colorArray2 = new Color[] { Color.FromArgb(70, 0xfe, 0xd8, 170), Color.FromArgb(70, 0xfb, 0xb5, 0x65), Color.FromArgb(70, 250, 0x9d, 0x34), Color.FromArgb(70, 0xfd, 0xee, 170) };

                //colorArray = colorArray2;
                //numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                Color c1 = RibbonTabControl.GetColor(Color.White, 1);

                //颜色
                colorArray2 = new Color[] {
                    Color.FromArgb(90, c1.R, c1.G, c1.B),
                    Color.FromArgb(05, c1.R, c1.G, c1.B),
                    Color.FromArgb(15, c1.R, c1.G, c1.B),
                    Color.FromArgb(60, c1.R, c1.G, c1.B) };

                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


                //colorArray2 = new Color[] { Color.FromArgb(70, 0, 0, 0), Color.FromArgb(70, 10, 10, 10), Color.FromArgb(70, 20, 20, 20), Color.FromArgb(70, 50, 50, 50) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.85f, 1.0f }; //调整对应颜色的位置
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(Color.FromArgb(0xab, 0xa1, 140))) {
                    DrawRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                //DrawRoundRectangle(g, new Pen(new LinearGradientBrush(rectangle, Color.FromArgb(0xdf, 0xb7, 0x88), Color.Transparent, LinearGradientMode.ForwardDiagonal)), rectangle, radius);
            }
            else {
                Color c1 = RibbonTabControl.GetColor(Color.White, 1);
                colorArray2 = new Color[] {
                    Color.FromArgb(90, c1.R, c1.G, c1.B),
                    Color.FromArgb(05, c1.R, c1.G, c1.B),
                    Color.FromArgb(15, c1.R, c1.G, c1.B),
                    Color.FromArgb(60, c1.R, c1.G, c1.B) };

                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


                //colorArray2 = new Color[] { Color.FromArgb(70, 0, 0, 0), Color.FromArgb(70, 10, 10, 10), Color.FromArgb(70, 20, 20, 20), Color.FromArgb(70, 50, 50, 50) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.35f, 0.8f, 1.0f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(Color.FromArgb(210, 0xc0, 0x8d))) {
                    DrawRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;

                using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle,
                    Color.FromArgb(0xff, 0xff, 0xf7),
                    Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                    using (Pen p2 = new Pen(brush2)) {
                        DrawRoundRectangle(g, p2, rectangle, 2f);
                    }
                }
            }
            brush.Dispose();

        }

        public static void RenderTopSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
            Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            Color[] colorArray2;
            if (pressed) {
                colorArray2 = new Color[] { Color.FromArgb(0xfe, 0xd8, 170), Color.FromArgb(0xfb, 0xb5, 0x65), Color.FromArgb(250, 0x9d, 0x34), Color.FromArgb(0xfd, 0xee, 170) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillTopRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(Color.FromArgb(0xab, 0xa1, 140))) {
                    DrawTopRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                using (Brush brush2 = new LinearGradientBrush(rectangle,
                        Color.FromArgb(0xdf, 0xb7, 0x88),
                        Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                    using (Pen p = new Pen(brush2)) {
                        DrawTopRoundRectangle(g, p, rectangle, radius);
                    }
                }
            }
            else {
                colorArray2 = new Color[] { Color.FromArgb(0xff, 0xfe, 0xe3), Color.FromArgb(0xff, 0xe7, 0x97), Color.FromArgb(0xff, 0xd7, 80), Color.FromArgb(0xff, 0xe7, 150) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillTopRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(Color.FromArgb(210, 0xc0, 0x8d))) {
                    DrawTopRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;

                using (Brush b = new LinearGradientBrush(rectangle,
                       Color.FromArgb(0xff, 0xff, 0xf7),
                       Color.Transparent,
                       LinearGradientMode.ForwardDiagonal)) {
                    using (Pen p = new Pen(b)) {

                        DrawTopRoundRectangle(g, p, rectangle, 2f);
                    }
                }
            }
            brush.Dispose();
        }
        #endregion

        private bool m_InResize = false;
        //protected override on
        protected override void OnResize(EventArgs e) {
            if (this.FindForm() != null && this.FindForm().WindowState != FormWindowState.Minimized) {
                int k = 0;
            }
            m_InResize = true;
            base.OnResize(e);
            m_InResize = false;
            this.Invalidate();
        }
        protected override void OnCreateControl() {
            base.OnCreateControl();
            if (this.FirstTabIsMenu) {
                base.SelectedIndex = 1;
            }
            else {
                base.SelectedIndex = 0;
            }
            this.hoverindex = -1;
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            if (!this.CollpaseWhenDblClick) return;
            if (this.FirstTabIsMenu) {
                //
                if (!(base.GetTabRect(0).Contains(e.X, e.Y) || (base.Height != 0x1a))) {
                    //if (this.Parent != null) {
                    //    this.Parent.Size = new Size(this.Parent.Size.Width, this.height + m_Dy);
                    //}
                    base.Size = new Size(base.Width, this.height);
                    if (this.OnCollapse != null) {
                        this.OnCollapse(this);
                    }
                }
            }
            else {
                if (base.Height == 0x1a) {
                    //if (this.Parent != null) {
                    //    this.Parent.Size = new Size(this.Parent.Size.Width, this.height + m_Dy);
                    //}
                    base.Size = new Size(base.Width, this.height);
                    if (this.OnCollapse != null) {
                        this.OnCollapse(this);
                    }
                }
            }
        }
        //private int m_Dy = 0;//保存和父控件的高度差
        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
            if (!this.CollpaseWhenDblClick) return;
            if (this.FirstTabIsMenu) {
                if (!base.GetTabRect(0).Contains(e.X, e.Y)) {
                    this.height = base.Height;
                    base.Size = new Size(base.Width, 0x1a);
                    if (this.OnCollapse != null) {
                        this.OnCollapse(this);
                    }
                    //if (this.Parent != null) {
                    //    m_Dy = this.Parent.Height - this.height;
                    //    this.Parent.Size = new Size(this.Parent.Size.Width, 0x1a + m_Dy);
                    //}
                    //(this.GetContainerControl() as Control).Size = this.Size;
                }
            }
            else {
                this.height = base.Height;
                base.Size = new Size(base.Width, 0x1a);
                if (this.OnCollapse != null) {
                    this.OnCollapse(this);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            this.pressed = true;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            this.hoverindex = -1;
            base.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            for (int i = 0; i < base.TabCount; i++) {
                if (base.GetTabRect(i).Contains(e.X, e.Y)) {
                    this.hoverindex = i;
                    break;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            this.pressed = false;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (this.m_InResize) {
                return;
            }
            Rectangle rectangle;
            int num;
            Rectangle tabRect;
            Region region;
            StringFormat format;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            #region 不要
            /*
            switch (RibbonThemeManager.ColorScheme) {

                case ColorScheme.Blue:
                    #region
                    e.Graphics.Clear(System.Drawing.Color.FromArgb(0xbf, 0xdb, 0xff));
                    rectangle = new Rectangle(0, 0, base.Width, base.Height);
                    if (base.Height > 0x1a) {
                        FillRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x11), 3f);
                        FillRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x10), 3f);
                    }
                    num = 0;
                    foreach (TabPage page in base.TabPages) {
                        tabRect = base.GetTabRect(num);
                        tabRect.Height += 4;
                        if (((base.SelectedIndex > 0) && (base.Height > 0x1a)) && (page == base.SelectedTab)) {
                            FillTopRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(tabRect.X - 2, tabRect.Y, tabRect.Width + 4, tabRect.Height + 4), 3f);
                            FillTopRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x10, System.Drawing.Color.Black)), new Rectangle(tabRect.X - 1, tabRect.Y, tabRect.Width + 2, tabRect.Height + 4), 3f);
                            FillTopRoundRectangle(e.Graphics, new LinearGradientBrush(tabRect, System.Drawing.Color.FromArgb(0xeb, 0xf3, 0xfe), System.Drawing.Color.FromArgb(220, 0xe7, 0xf5), LinearGradientMode.Vertical), tabRect, 3f);
                            DrawTopRoundRectangle(e.Graphics, new Pen(System.Drawing.Color.FromArgb(0x8d, 0xb2, 0xe3)), tabRect, 3f);
                            tabRect.Offset(1, 1);
                            tabRect.Width -= 2;
                            tabRect.Height--;
                            DrawRoundRectangle(e.Graphics, new Pen(new LinearGradientBrush(tabRect, System.Drawing.Color.FromArgb(0xf8, 0xfb, 0xff), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)), tabRect, 3f);
                            region = new Region();
                            region.Exclude(new Rectangle(tabRect.X - 4, 0, tabRect.Width + 1, 2));
                            page.Region = region;
                        }
                        else if (num == this.hoverindex) {
                            tabRect.Width--;
                            if (this.FirstTabIsMenu && num == 0) {
                                tabRect.Height -= 3;
                                RenderSelection(e.Graphics, tabRect, 3f, this.pressed);
                                tabRect.Height += 3;
                            }
                            else {
                                RenderTopSelection(e.Graphics, tabRect, 3f, this.pressed);
                            }
                            tabRect.Width++;
                        }
                        tabRect.Height -= 2;
                        format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(page.Text, page.Font, new SolidBrush(System.Drawing.Color.FromArgb(0x15, 0x42, 0x8b)), tabRect, format);
                        num++;
                    }
                    #endregion
                    break;

                case ColorScheme.Gray:
                    #region
                    e.Graphics.Clear(System.Drawing.Color.FromArgb(0x53, 0x53, 0x53));
                    rectangle = new Rectangle(0, 0, base.Width, base.Height);
                    if (base.Height > 0x1a) {
                        FillRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x11), 3f);
                        FillRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x10), 3f);
                    }
                    num = 0;
                    foreach (TabPage page in base.TabPages) {
                        tabRect = base.GetTabRect(num);
                        tabRect.Height += 4;
                        format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        if (((base.SelectedIndex > 0) && (base.Height > 0x1a)) && (page == base.SelectedTab)) {
                            FillTopRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)), new Rectangle(tabRect.X - 2, tabRect.Y, tabRect.Width + 4, tabRect.Height + 4), 3f);
                            FillTopRoundRectangle(e.Graphics, new SolidBrush(System.Drawing.Color.FromArgb(0x10, System.Drawing.Color.Black)), new Rectangle(tabRect.X - 1, tabRect.Y, tabRect.Width + 2, tabRect.Height + 4), 3f);
                            FillTopRoundRectangle(e.Graphics, new LinearGradientBrush(tabRect, System.Drawing.Color.FromArgb(0xed, 0xee, 0xef), System.Drawing.Color.FromArgb(0xce, 210, 0xd9), LinearGradientMode.Vertical), tabRect, 3f);
                            DrawTopRoundRectangle(e.Graphics, new Pen(System.Drawing.Color.FromArgb(190, 190, 190)), tabRect, 3f);
                            tabRect.Offset(1, 1);
                            tabRect.Width -= 2;
                            tabRect.Height--;
                            DrawRoundRectangle(e.Graphics, new Pen(new LinearGradientBrush(tabRect, System.Drawing.Color.FromArgb(0xf9, 0xf9, 0xf9), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)), tabRect, 3f);
                            region = new Region();
                            region.Exclude(new Rectangle(tabRect.X - 4, 0, tabRect.Width + 1, 2));
                            page.Region = region;
                            tabRect.Height -= 2;
                            e.Graphics.DrawString(page.Text, page.Font, new SolidBrush(System.Drawing.Color.Black), tabRect, format);
                        }
                        else if (num == this.hoverindex) {
                            tabRect.Width--;
                            if (this.FirstTabIsMenu && num == 0) {
                                tabRect.X += 2;
                                tabRect.Width -= 2;
                                tabRect.Height -= 3;
                                RenderSelection(e.Graphics, tabRect, 3f, this.pressed);
                                tabRect.X -= 2;
                                tabRect.Width += 2;
                                tabRect.Height += 3;
                            }
                            else {
                                RenderTopSelection(e.Graphics, tabRect, 3f, this.pressed);
                            }
                            tabRect.Width++;
                            tabRect.Height -= 2;
                            e.Graphics.DrawString(page.Text, page.Font, new SolidBrush(System.Drawing.Color.Black), tabRect, format);
                        }
                        else {
                            tabRect.Height -= 2;
                            e.Graphics.DrawString(page.Text, page.Font, new SolidBrush(System.Drawing.Color.White), tabRect, format);
                        }
                        num++;
                    }
                    #endregion
                    break;
                    */
            #endregion
            //case ColorScheme.Custom:
            e.Graphics.Clear(RibbonThemeManager.BackColor);
            rectangle = new Rectangle(0, 0, base.Width, base.Height);
            if (base.Height > 0x1a) {

                //FillRoundRectangle(e.Graphics, new SolidBrush(Color.FromArgb(0x20, Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x11), 3f);
                //FillRoundRectangle(e.Graphics, new SolidBrush(Color.FromArgb(0x20, Color.Black)), new Rectangle(5, base.Height - 20, base.Width - 10, 0x10), 3f);

                //增加对图片/渐变色的支持 图片显示的地方根据控件的需求不同而显示在不同的地方
                bool goBack = false;
                switch (this.m_SetFlag) {
                    case 0://默认：为原始的，我们不做修改
                        break;
                    case 1://图片支持
                        //图片支持      好象不支持
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
            //下面绘制子控件 
            num = 0;
            foreach (TabPage page in base.TabPages) {
                tabRect = base.GetTabRect(num);
                //if (tabRect == Rectangle.Empty) continue;
                tabRect.Height += 4;

                if (((base.SelectedIndex > 0 || !this.FirstTabIsMenu) && (base.Height > 0x1a)) && (page == base.SelectedTab)) {
                    using (Brush b = new SolidBrush(Color.FromArgb(0x20, Color.Black))) {
                        FillTopRoundRectangle(e.Graphics, b, new Rectangle(tabRect.X - 2, tabRect.Y, tabRect.Width + 4, tabRect.Height + 4), 3f);
                    }
                    using (Brush b = new SolidBrush(Color.FromArgb(0x10, Color.Black))) {
                        FillTopRoundRectangle(e.Graphics, b, new Rectangle(tabRect.X - 1, tabRect.Y, tabRect.Width + 2, tabRect.Height + 4), 3f);
                    }
                    using (Brush b = new LinearGradientBrush(tabRect, GetColor(1.085), GetColor(1.025), LinearGradientMode.Vertical)) {
                        FillTopRoundRectangle(e.Graphics, b, tabRect, 3f);
                    }
                    using (Pen p = new Pen(GetColor(0.75))) {
                        DrawTopRoundRectangle(e.Graphics, p, tabRect, 3f);
                    }
                    tabRect.Offset(1, 1);
                    if (base.SelectedIndex >= 0) {
                        tabRect.Width -= 2;
                    }
                    tabRect.Height--;
                    using (Brush b = new LinearGradientBrush(tabRect, GetColor(1.105), Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                        using (Pen p = new Pen(b)) {
                            DrawRoundRectangle(e.Graphics, p, tabRect, 3f);
                        }
                    }
                    region = new Region();
                    region.Exclude(new Rectangle(tabRect.X - 4, 0, tabRect.Width + 1, 2));
                    if (page.Region != null) {
                        page.Region.Dispose();
                    }
                    page.Region = region;
                }
                else if (num == this.hoverindex) {
                    tabRect.Width--;
                    if (num == 0 && this.FirstTabIsMenu) {
                        tabRect.Height -= 3;
                        RenderSelection(e.Graphics, tabRect, 3f, this.pressed);
                        tabRect.Height += 3;
                    }
                    else {
                        RenderTopSelection(e.Graphics, tabRect, 3f, this.pressed);
                    }
                    tabRect.Width++;
                }
                tabRect.Height -= 2;
                format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                using (Brush b = new SolidBrush(RibbonThemeManager.TextColor)) {
                    e.Graphics.DrawString(page.Text, page.Font, b, tabRect, format);
                }
                num++;
            }
            // break;
            // }
        }

        private void DrawPicture(PaintEventArgs e) {
            //好象不支持
            if (this.Image != null) {
                e.Graphics.DrawImage(this.Image, this.ClientRectangle);
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

        protected override void OnSelecting(TabControlCancelEventArgs e) {
            base.OnSelecting(e);

            if (this.FirstTabIsMenu && e.TabPageIndex == 0) {
                e.Cancel = true;
                if (this.OnPopup != null) {
                    this.OnPopup(this);
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
