using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RS.UtilityLib.WinFormCommon.RibbonUI;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public class LabelEx : Label
    {
        public enum MousePosState : byte
        {
            Normal = 1,
            Hover = 2,
            Press = 4
        }
        public LabelEx()
            : base() {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            TabButtonColor = Color.FromArgb(0xa9, 0x88, 0x56);// Color.FromArgb(0xde, 0x95, 0x2a);
            TabButtonColor_Selected = Color.FromArgb(0xde, 0x95, 0x2a);
            TabButtonColor_Hoverd = Color.FromArgb(0xde, 0x95, 0x2a);// Color.FromArgb(0xbd, 0x90, 0x4e);
            TabButtonColor_Disabled = Color.FromArgb(0xa9, 0x88, 0x56);
            this.MinimumSize = new Size(8, 8);
        }
        /// <summary>
        /// 该属性不可用
        /// </summary>
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                //base.BackgroundImage = value;
            }
        }
        /// <summary>
        /// 该属性不可用
        /// </summary>
        public new Image @Image {
            get { return base.Image; }
            set { }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent) {
            //绘制应该要绘制的背景 
            base.OnPaintBackground(pevent);
        }

        public bool IsSelected {
            get { return m_IsSelected; }
            set {
                if (m_IsSelected != value) {
                    m_IsSelected = value;
                    this.Invalidate();
                }
            }
        }
        public bool IsTristateImage {
            get { return this.m_IsTristateBackImage; }
            set {
                if (m_IsTristateBackImage != value) {
                    this.m_IsTristateBackImage = value;
                    this.Invalidate();
                }
            }
        }
        protected MousePosState @MouseState {
            get { return m_MouseState; }
            set {
                if (m_MouseState != value) {
                    m_MouseState = value;
                    this.Invalidate();
                }
            }
        }
        public Color TabButtonColor {
            get;
            set;
        }
        public Color TabButtonColor_Selected {
            get;
            set;
        }
        public Color TabButtonColor_Hoverd {
            get;
            set;
        }
        public Color TabButtonColor_Disabled {
            get;
            set;
        }
        public bool IsTabButton {
            get;
            set;
        }
        private bool m_IsSelected = false;
        private bool m_IsTristateBackImage = false;
        private MousePosState m_MouseState = MousePosState.Normal;
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            MouseState = MousePosState.Hover;
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            MouseState = MousePosState.Normal;
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            MouseState = MousePosState.Press;

        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            MouseState = MousePosState.Normal;

        }
        public Image ImageTristate {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (this.IsTristateImage) {
                if (this.ImageTristate != null) {
                    //画3态图
                    int imgwidth = (int)this.ImageTristate.Width / 3;
                    int imgheight = (int)this.ImageTristate.Height;
                    if (m_MouseState == MousePosState.Hover) {
                        e.Graphics.DrawImage(this.ImageTristate, this.ClientRectangle, imgwidth, 0, imgwidth, imgheight, GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
                    }
                    else if (m_MouseState == MousePosState.Press) {
                        e.Graphics.DrawImage(this.ImageTristate, this.ClientRectangle, imgwidth * 2, 0, imgwidth, imgheight, GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
                    }
                    else {
                        e.Graphics.DrawImage(this.ImageTristate, this.ClientRectangle, 0, 0, imgwidth, imgheight, GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
                    }
                    if (this.IsSelected) {
                        e.Graphics.DrawImage(this.ImageTristate, this.ClientRectangle, imgwidth * 2, 0, imgwidth, imgheight, GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
                    }
                }
                //base.OnPaint(e);
            }
            else if (this.IsTabButton) {
                if (string.IsNullOrEmpty(this.Text) == false) {
                    if (this.AutoSize) {
                        SizeF size = e.Graphics.MeasureString(this.Text, this.Font);
                        if ((int)size.Width > this.Height && this.Width != (int)size.Width + 8) {
                            this.Width = (int)size.Width + 8;
                            return;
                        }
                    }
                }
                //e.Graphics.Clear(Color.FromArgb(10,this.TabButtonColor));
                Rectangle tabRect = this.ClientRectangle;// e.ClipRectangle;
                float recRadio = 6;
                using (SolidBrush s1 = new SolidBrush(System.Drawing.Color.FromArgb(0x20, System.Drawing.Color.Black)),
                    s2 = new SolidBrush(System.Drawing.Color.FromArgb(0x10, System.Drawing.Color.Black))
                ) {
                    DrawRoundRectangleHelper.FillTopRoundRectangle(e.Graphics, s1, new Rectangle(tabRect.X - 2, tabRect.Y, tabRect.Width + 4, tabRect.Height + 4), recRadio);
                    DrawRoundRectangleHelper.FillTopRoundRectangle(e.Graphics, s2, new Rectangle(tabRect.X - 1, tabRect.Y, tabRect.Width + 2, tabRect.Height + 4), recRadio);
                }
                var color = TabButtonColor;

                if (this.Enabled == false) {
                    color = this.TabButtonColor;//
                    double h, s, v;
                    ColorToHSV(color, out h, out s, out v);
                    color = ColorFromHSV(h, s * 0.6f, v);
                    // color = this.TabButtonColor_Disabled;// Color.FromArgb(0xa9, 0x88, 0x56);
                }
                if (this.Enabled && this.m_MouseState == MousePosState.Hover) {
                    color = this.TabButtonColor_Hoverd;// Color.FromArgb(0xbd, 0x90, 0x4e);
                    //double h, s, v;
                    //ColorToHSV(color, out h, out s, out v);
                    //color = ColorFromHSV(h, s * 0.99f, v);
                }
                else if (this.Enabled && (this.m_MouseState == MousePosState.Press || this.IsSelected)) {
                    color = this.TabButtonColor_Selected;// Color.FromArgb(0xde, 0x95, 0x2a);
                                                         // double h, s, v;
                                                         // ColorToHSV(color, out h, out s, out v);
                                                         //color = ColorFromHSV(h*1.1f, s * 1.1f, v);
                }
                else if (this.Enabled) {
                    color = this.TabButtonColor_Selected;//
                    double h, s, v;
                    ColorToHSV(color, out h, out s, out v);
                    color = ColorFromHSV(h, s, v * 0.85f);
                }

                //using (LinearGradientBrush l1 = new LinearGradientBrush(tabRect, DrawRoundRectangleHelper.GetColor(color, 1.005), DrawRoundRectangleHelper.GetColor(color, 1.005), LinearGradientMode.Vertical)) {
                //    DrawRoundRectangleHelper.FillTopRoundRectangle(e.Graphics, l1, tabRect, recRadio);
                //}
                using (SolidBrush sb = new SolidBrush(color)) {
                    DrawRoundRectangleHelper.FillTopRoundRectangle(e.Graphics, sb, tabRect, recRadio);
                }
                using (Pen p1 = new Pen(DrawRoundRectangleHelper.GetColor(color, 0.75))) {
                    DrawRoundRectangleHelper.DrawTopRoundRectangle(e.Graphics, p1, tabRect, recRadio);
                }
                tabRect.Offset(1, 1);
                tabRect.Height--;
                //DrawRoundRectangleHelper.DrawRoundRectangle(e.Graphics, new Pen(new LinearGradientBrush(tabRect, DrawRoundRectangleHelper.GetColor(this.TabButtonColor,1.105), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)), tabRect, 8f);
                //Region region = new Region();
                //region.Exclude(new Rectangle(tabRect.X - 4, 0, tabRect.Width + 1, 2));
                //if (this.Region != null) {
                //    this.Region.Dispose();
                //}
                //this.Region = Region;
                Rectangle rect = new Rectangle(2, 2, this.Height - 4, this.Height - 4);
                if (this.ImageTristate != null) {
                    //当标题
                    e.Graphics.DrawImage(this.ImageTristate, rect);
                }
                if (this.Enabled && this.m_MouseState == MousePosState.Hover) {
                    rect = this.ClientRectangle;// e.ClipRectangle;
                    //if (rect.Height <= 0) {
                    //    rect.Height = 1;
                    //}
                    //if (rect.Width <= 0) {
                    //    rect.Width = 1;
                    //}
                    // DrawRoundRectangleHelper.RenderSelection(e.Graphics, rect, recRadio, false);
                }
                else if (this.Enabled && (this.m_MouseState == MousePosState.Press || this.IsSelected)) {
                    rect = this.ClientRectangle;// e.ClipRectangle;
                    //if (rect.Height <= 0) {
                    //    rect.Height = 1;
                    //}
                    //if (rect.Width <= 0) {
                    //    rect.Width = 1;
                    //}
                    // DrawRoundRectangleHelper.RenderSelection(e.Graphics, rect, recRadio, true);
                }
                using (StringFormat format = new StringFormat()) {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    using (SolidBrush s1 = new SolidBrush(this.ForeColor)) {
                        e.Graphics.DrawString(this.Text, this.Font, s1, tabRect, format);
                    }
                }
            }
            else {
                if (this.ImageTristate != null) {
                    //普通状态 当背景
                    e.Graphics.DrawImage(this.ImageTristate, this.ClientRectangle);
                }
                base.OnPaint(e);
                if (this.m_MouseState == MousePosState.Hover) {
                    Rectangle rect = e.ClipRectangle;
                    //if (rect.Height <= 0) {
                    //    rect.Height = 1;
                    //}
                    //if (rect.Width <= 0) {
                    //    rect.Width = 1;
                    //}
                    DrawRoundRectangleHelper.RenderSelection(e.Graphics, rect, 2f, false);
                }
                else if (this.m_MouseState == MousePosState.Press || this.IsSelected) {
                    Rectangle rect = e.ClipRectangle;
                    //if (rect.Height <= 0) {
                    //    rect.Height = 1;
                    //}
                    //if (rect.Width <= 0) {
                    //    rect.Width = 1;
                    //}
                    DrawRoundRectangleHelper.RenderSelection(e.Graphics, rect, 2f, true);
                }
            }

        }
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value) {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value) {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 133) {
                this.Invalidate();
            }
            base.WndProc(ref m);
        }
    }
}
