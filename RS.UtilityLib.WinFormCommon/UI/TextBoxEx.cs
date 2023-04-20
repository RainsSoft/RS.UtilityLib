using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    ///输入框: 支持可高亮显示首尾空格/无输入时水印提示
    /// </summary>
    public class TextBoxEx : System.Windows.Forms.TextBox
    {
        //RedrawWindow
        const uint RDW_INVALIDATE = 0x1;
        const uint RDW_IUPDATENOW = 0x100;
        const uint RDW_FRAME = 0x400;
        //Msg Paint
        const int WM_NCPAINT = 0x85;
        public const int WM_PAINT = 0xF;//15
        public const int WM_CTLCOLOREDIT = 0x133;//307
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);
        private void doREdrawWindow() {
            //this.Invalidate();
            //return;
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                 RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);

        }

        //enum ControlState {
        //    NoFocus = 0,
        //    Focus = 1,
        //    Hover = 2,
        //}
        //private ControlState m_MouseState = ControlState.NoFocus;
        //protected override void OnHandleCreated(EventArgs e) {
        //    base.OnHandleCreated(e);
        //    SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
        //        ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        //}
        #region Designer
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (this.IsDesignerHosted) {
                this.Font = new System.Drawing.Font(SystemFonts.DefaultFont.FontFamily, 10.5f, FontStyle.Regular);
            }
        }
        private bool IsDesignerHosted {
            get {
                if (DesignMode)
                    return DesignMode;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
        #endregion

        #region override
        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            //SetStyle(ControlStyles.UserPaint, true);
            doREdrawWindow();
        }
        protected override void OnGotFocus(EventArgs e) {
            //m_MouseState = ControlState.Focus;
            base.OnGotFocus(e);
            doREdrawWindow();
        }
        protected override void OnLostFocus(EventArgs e) {
            //m_MouseState = ControlState.NoFocus;
            base.OnLostFocus(e);
            doREdrawWindow();
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            //
            doREdrawWindow();
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            doREdrawWindow();
        }
        protected override void OnMouseHover(EventArgs e) {
            //m_MouseState = ControlState.Hover;
            base.OnMouseHover(e);
            //
            doREdrawWindow();
        }

        protected override void OnMouseLeave(EventArgs e) {
            //m_MouseState = ControlState.NoFocus;
            //
            base.OnMouseLeave(e);
            //
            doREdrawWindow();
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            doREdrawWindow();
        }
        protected override void OnMouseUp(MouseEventArgs mevent) {
            base.OnMouseUp(mevent);
            doREdrawWindow();
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            doREdrawWindow();
        }
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            doREdrawWindow();
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            doREdrawWindow();
        }
        #endregion

        #region 移除多行支持
        /// <summary>
        /// 不支持多行显示
        /// </summary>
        [Description("不支持多行显示"), Browsable(false)]
        public override bool Multiline {
            get {
                return base.Multiline;
            }

            set {
                base.Multiline = false;
            }
        }
        [Description("不支持多行显示"), Browsable(false)]
        public new bool WordWrap {
            get {
                return base.WordWrap;
            }
            set {
                base.WordWrap = false;
            }
        }
        [Description("不支持多行显示"), Browsable(false)]
        public new ScrollBars ScrollBars {
            get {
                return base.ScrollBars;
            }
            set {
                base.ScrollBars = ScrollBars.None;

            }
        }
        #endregion

        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
                //
                doREdrawWindow();
            }
        }

        //
        [Description("高亮首尾空格字符")]
        public bool ShowSpaces {
            get; set;
        } = true;
        [Description("高亮首尾Tab字符")]
        public bool ShowTabs {
            get; set;
        } = true;
        Color borderColor = Color.LightSeaGreen;
        public Color BorderColor {
            get {
                return borderColor;
            }
            set {
                borderColor = value;
                //
                doREdrawWindow();
            }
        }
        private Color m_SpaceKeyColor = Color.Red;
        public Color SpaceKeyColor {
            get {
                return m_SpaceKeyColor;
            }
            set {
                m_SpaceKeyColor = value;
                //
                doREdrawWindow();
            }
        }
        #region 首尾空格高亮展示
        private string m_SpaceOrTabText_Start = "";
        private string m_SpaceOrTabText_End = "";
        void modifySpaceText(string txt) {
            if (string.IsNullOrEmpty(txt)) {
                m_SpaceOrTabText_Start = "";
                m_SpaceOrTabText_End = "";
                return;
            }
            //var sizeX = GetCharSize(this.Font, '中');
            // StringBuilder sb = new StringBuilder();
            //foreach(var v in txt) {
            //    if(v == ' ') {
            //        //"€¢«¶»¿ø"
            //        sb.Append(this.ShowSpaces ? "\u00B7" : " ");//("♤");\u00B7                  

            //    }
            //    else if(v == '\t') {
            //        sb.Append(this.ShowTabs ? "\u00BB\u00BB\u00BB\u00BB" : "\t");//("♤♤♤♤");\u00BB\u00BB\u00BB\u00BB

            //    }
            //    else {
            //        if(IsUnicodeChar(v)) {
            //            //中文
            //            sb.Append("  ");
            //        }
            //        else {
            //            sb.Append(" ");
            //        }                    
            //    }
            //}
            int start2end_len = 0;
            int end2start_len = 0;
            for (int i = 0; i < txt.Length; i++) {
                if (txt[i] == ' ' || txt[i] == '\t') {
                    start2end_len++;
                }
                else {
                    break;
                }
            }
            for (int i = txt.Length - 1; i >= start2end_len; i--) {
                if (txt[i] == ' ' || txt[i] == '\t') {
                    end2start_len++;
                }
                else {
                    break;
                }
            }
            if (start2end_len == 0) {
                m_SpaceOrTabText_Start = "";
            }
            else {
                var startStr = txt.Substring(0, start2end_len);
                StringBuilder sb = new StringBuilder();
                foreach (var v in startStr) {
                    if (v == ' ') {
                        //"¢«¶»¿ø« ®ˆˆ‚¢£¬­¶º¿ø
                        sb.Append(this.ShowSpaces ? "." : " ");//("♤ø¢¶¿®ˆ£¬");\u00B7[183·] \u00AB[171«]    ˆ              

                    }
                    else if (v == '\t') {
                        sb.Append(this.ShowTabs ? "¬¬¬¬" : "\t");//("♤♤♤♤");\u00BB\u00BB\u00BB\u00BB[187»]

                    }
                }
                m_SpaceOrTabText_Start = sb.ToString();
            }
            if (end2start_len == 0) {
                m_SpaceOrTabText_End = "";
            }
            else {
                var endStr = txt.Substring(txt.Length - end2start_len, end2start_len);
                StringBuilder sb = new StringBuilder();
                foreach (var v in endStr) {
                    if (v == ' ') {
                        //"€¢«¶»¿ø"ˆˆ
                        sb.Append(this.ShowSpaces ? "." : " ");//("♤");\u00B7                  

                    }
                    else if (v == '\t') {
                        sb.Append(this.ShowTabs ? "¬¬¬¬" : "\t");//("♤♤♤♤");\u00BB\u00BB\u00BB\u00BB

                    }
                }
                m_SpaceOrTabText_End = sb.ToString();
            }
        }
        void drawTextWithHightHTSpaceKey(Graphics g) {
            modifySpaceText(this.Text);


            //using(var g = this.CreateGraphics()) {
            //g.Clear(this.BackColor);            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            if (BorderColor != Color.Transparent &&
                BorderStyle != System.Windows.Forms.BorderStyle.None) {
                bool empty = string.IsNullOrEmpty(this.Text);
                //绘制边框
                using (var p = new Pen(empty ? Color.FromArgb(64, this.BorderColor) : this.BorderColor)) {
                    g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }
            //using(SolidBrush sbh = new SolidBrush(this.ForeColor)) {
            //    //绘制文本
            //    g.DrawString(this.Text, this.Font, sbh, 0f, 0f);
            //}
            bool needDrawSpace = !(string.IsNullOrEmpty(this.m_SpaceOrTabText_Start) && string.IsNullOrEmpty(this.m_SpaceOrTabText_End));
            if (!needDrawSpace) {
                return;
            }

            using (SolidBrush sbh = new SolidBrush(this.m_SpaceKeyColor)) {
                //绘制首尾空格
                if (!string.IsNullOrEmpty(this.m_SpaceOrTabText_Start)) {
                    //TextRenderer.DrawText(g, this.m_SpaceOrTabText_Start, this.Font,
                    //this.ClientRectangle, Color.FromArgb(128, this.SpaceColor), Color.Transparent, TextFormatFlags.TextBoxControl);
                    ////TextFormatFlags.Top | TextFormatFlags.Left);
                    ////       
                    float h = Math.Max(this.Height * 0.8f - 2f, 2f);
                    g.DrawString(this.m_SpaceOrTabText_Start, this.Font, sbh, 0f, -h * 0.5f);
                    //var sizeDot = GetCharSize(this.Font, ".");
                    //int w = Math.Max( (int)sizeDot.Width,2);

                    //using(LinearGradientBrush headerGradient = new LinearGradientBrush(new Point(0, 0), new Point(0, w), Color.FromArgb(0, this.SpaceKeyColor.R, this.SpaceKeyColor.G, this.SpaceKeyColor.B), this.SpaceKeyColor)) {
                    //    //g.FillRectangle(headerGradient, halfHeight, 0, lineWidth, gradientWidth);
                    //    for(int i = 0; i < this.m_SpaceOrTabText_Start.Length; i++) {
                    //        g.FillEllipse(headerGradient, i * (int)w, h, w-2, w-2);
                    //    }
                    //}
                }
                if (!string.IsNullOrEmpty(this.m_SpaceOrTabText_End)) {
                    var frontStr = this.Text.Substring(0, this.Text.Length - this.m_SpaceOrTabText_End.Length);
                    var frontSizeX = TextRenderer.MeasureText(frontStr, this.Font);
                    var sizeDot = GetCharSize(this.Font, ".");
                    //
                    int w = Math.Max((int)sizeDot.Width, 2);
                    float h = Math.Max(this.Height * 0.8f - 2f, 2f);
                    if (frontSizeX.Width < this.Width) {
                        float endX = frontSizeX.Width - sizeDot.Width;
                        //没有超过文本框长度
                        g.DrawString(this.m_SpaceOrTabText_End, this.Font, sbh, endX + 1, -h * 0.5f);
                        //using(LinearGradientBrush headerGradient = new LinearGradientBrush(new Point(0, 0), new Point(0, w), Color.FromArgb(0, this.SpaceKeyColor.R, this.SpaceKeyColor.G, this.SpaceKeyColor.B), this.SpaceKeyColor)) {
                        //    //g.FillRectangle(headerGradient, halfHeight, 0, lineWidth, gradientWidth);
                        //    for(int i = 0; i < this.m_SpaceOrTabText_End.Length; i++) {
                        //        g.FillEllipse(headerGradient, endX+i * (int)w, h, w - 2, w - 2);
                        //    }
                        //}
                    }
                    else {
                        //如果超过文本框长度，只绘制最后一个指示符
                        g.DrawString(this.m_SpaceOrTabText_End, this.Font, sbh, this.Width - sizeDot.Width - 2, -h * 0.5f);
                        //using(LinearGradientBrush headerGradient = new LinearGradientBrush(new Point(0, 0), new Point(0, w), Color.FromArgb(0, this.SpaceKeyColor.R, this.SpaceKeyColor.G, this.SpaceKeyColor.B), this.SpaceKeyColor)) {
                        //    //g.FillRectangle(headerGradient, halfHeight, 0, lineWidth, gradientWidth);

                        //    g.FillEllipse(headerGradient, this.Width - w- 2, h, w - 2, w - 2);

                        //}
                    }
                }

            }

        }
        public static SizeF GetCharSize(Font font, string c) {
            Size sz2 = TextRenderer.MeasureText("<" + c + ">", font);
            Size sz3 = TextRenderer.MeasureText("<>", font);

            return new SizeF(sz2.Width - sz3.Width, /*sz2.Height*/font.Height);
        }
        #endregion

        #region watermark
        public Font WaterMarkTextFont {
            get;
            set;
        } = new Font(SystemFonts.DefaultFont, FontStyle.Italic);
        /// <summary>
        /// 水印文字
        /// </summary>
        public string WaterMarkText {
            get;
            set;
        } = "请输入...";
        /// <summary>
        /// 文字偏移
        /// </summary>
        public float WaterMarkTextOffsetX {
            get;
            set;
        } = 4f;
        public float WaterMarkTextOffsetY {
            get;
            set;
        } = 4f;
        void drawWaterMask(Graphics g) {
            if (!string.IsNullOrEmpty(this.Text) ||
               string.IsNullOrEmpty(this.WaterMarkText) ||
               this.WaterMarkTextFont == null) {
                return;
            }

            if (this.Focused == false) {
                //if(m_MouseState == ControlState.NoFocus) {
                //    g.DrawRectangle(m_NormalPen2, m_NormalRect);
                //}
                //if(string.IsNullOrEmpty(this.Text) && string.IsNullOrEmpty(this.WaterMarkText) == false) {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(128, Color.LightGray))) {
                    g.DrawString(this.WaterMarkText, this.WaterMarkTextFont, sb, this.WaterMarkTextOffsetX, this.WaterMarkTextOffsetY);

                }
                //}
            }
            //if(this.Focused || m_MouseState == ControlState.Hover) {
            //    g.DrawRectangle(m_FocusPen, 0, 0, this.Width - 1, this.Height - 1);
            //}

        }
        #endregion

        //protected override void OnPaint(PaintEventArgs e) {
        //    base.OnPaint(e);
        //    drawTextWithHightHTSpaceKey(e.Graphics);
        //}
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            //return;
            //if(m.Msg == 15 || m.Msg == 7 || m.Msg == 8) {
            //}
            if (m.Msg == WM_NCPAINT || m.Msg == WM_PAINT || m.Msg == WM_CTLCOLOREDIT
                || m.Msg == 7 || m.Msg == 8
                //&& BorderColor != Color.Transparent &&
                //BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D
                ) {

                var hdc = GetWindowDC(this.Handle);
                if (hdc.ToInt32() == 0) {
                    return;
                }
                if (this.Width < 1 || this.Height < 1) {
                    goto label_rdc;
                }
                using (var g = Graphics.FromHdcInternal(hdc)) {
                    //if(BorderColor != Color.Transparent &&
                    //BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D) {
                    //        using(var p = new Pen(this.BorderColor)) {
                    //            g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
                    //        }
                    //    }
                    drawTextWithHightHTSpaceKey(g);
                    //
                    if (m.Msg == WM_PAINT
                        || m.Msg == WM_CTLCOLOREDIT
                        //&&this.BorderStyle != BorderStyle.None
                        ) {
                        drawWaterMask(g);
                    }
                }

                label_rdc:
                {
                    //返回结果
                    //m.Result = IntPtr.Zero;
                    ReleaseDC(this.Handle, hdc);
                }

            }


        }

    }
}
