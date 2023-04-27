using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public  class RibbonTextBox : TextBox
    {


        //public event EventHandler TextChanged;
        private bool m_txtHasFocus = false;
        private Pen m_NormalPen;
        private Pen m_NormalPen2;
        private Pen m_FocusPen;
        private Rectangle m_PaintRect;
        private Rectangle m_NormalRect;
        enum ControlState
        {
            NoFocus = 0,
            Focus = 1,
            Hover = 2,
        }
        private ControlState m_MouseState = ControlState.NoFocus;
        public RibbonTextBox() {

            AutoSize = false;
            BorderStyle = BorderStyle.None;
            BorderStyle = BorderStyle.FixedSingle;
            m_FocusPen = new Pen(Color.FromArgb(80, RibbonThemeManager.BackColor), 2f);
            m_NormalPen = new Pen(Color.FromArgb(50, RibbonThemeManager.BackColor), 2f);
            m_NormalPen2 = new Pen(Color.FromArgb(240, RibbonThemeManager.BackColor), 1f);
            m_PaintRect = new Rectangle(1, 1, this.Width - 1, this.Height - 2);
            m_NormalRect = new Rectangle(0, 0, this.Width, this.Height);
            // Height = 14;
            ContextMenu = null;
            HookEvents();
        }

        void UnHookEvents() {
        }
        void HookEvents() {
            //txt_Internal.BackColorChanged += new EventHandler(txt_Internal_BackColorChanged);
            //this.txt_Internal.LostFocus += new EventHandler(txt_Internal_LostFocus);
            //this.txt_Internal.TextChanged += new System.EventHandler(this.txt_Internal_TextChanged);
            //this.txt_Internal.MouseLeave += new System.EventHandler(this.txt_Internal_MouseLeave);
            //this.txt_Internal.Click += new System.EventHandler(this.txt_Internal_Click);
            //this.txt_Internal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Internal_KeyDown);
            //this.txt_Internal.Leave += new System.EventHandler(this.txt_Internal_Leave);
            //this.txt_Internal.FontChanged += new System.EventHandler(this.txt_Internal_FontChanged);
            //this.txt_Internal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_Internal_KeyUp);
            //this.txt_Internal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Internal_KeyPress);
            //this.txt_Internal.Enter += new System.EventHandler(this.txt_Internal_Enter);
            //this.txt_Internal.MouseHover += new System.EventHandler(this.txt_Internal_MouseHover);
            //this.txt_Internal.SizeChanged += new System.EventHandler(this.txt_Internal_SizeChanged);
        }
        protected override void OnGotFocus(EventArgs e) {
            m_MouseState = ControlState.Focus;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e) {
            m_MouseState = ControlState.NoFocus;
            if (m_DigitalOnly) {
                float f = 0f;
                if (float.TryParse(this.Text, out f)) {
                    if (f > this.Max) {
                        this.Text = this.Max.ToString();
                    }
                    if (f < this.Min) {
                        this.Text = this.Min.ToString();
                    }
                }
            }
            base.OnLostFocus(e);
            if (m_DigitalOnly) {

                //如果为空只有负号或者点号，则认为是0
                string txt = this.Text.Trim();
                //if (string.IsNullOrEmpty(txt)
                //    || txt == m_decimalSeparator.ToString()
                //    || txt == m_negativeSign.ToString()) {

                MakeDigitalRound();
                //}
            }
        }
        private string m_Text = string.Empty;
        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (base.Text != value) {
                    //如果只是数字格式，则需要判定是否符合规定。
                    if (m_DigitalOnly) {
                        base.Text = MakeDigitalRound(value);
                    }
                    else {
                        base.Text = value;
                    }
                }
                this.SelectionStart = this.TextLength;
            }
        }

        /// <summary>
        /// 水印文字
        /// </summary>
        public string WaterMarkText {
            get;
            set;
        }
        /// <summary>
        /// 文字偏移
        /// </summary>
        public PointF WaterMarkTextOffset {
            get;
            set;
        }
        protected override void OnTextChanged(EventArgs e) {
            if (m_internalTextChg) {
                if (m_DigitalOnly) {
                    if (isNumber(this.Text) == false) {
                        this.Text = MakeDefaultDigital();
                    }
                }
                return;
            }
            base.OnTextChanged(e);
        }

        protected override void OnMouseHover(EventArgs e) {
            m_MouseState = ControlState.Hover;
            base.OnMouseHover(e);
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            m_MouseState = ControlState.NoFocus;
            this.Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {

            if (e.KeyCode == Keys.Enter) {
                if (this.DigitalOnly) {
                    float f = 0f;
                    if (float.TryParse(this.Text, out f)) {
                        if (f > this.Max) {
                            this.Text = this.Max.ToString();
                        }
                        if (f < this.Min) {
                            this.Text = this.Min.ToString();
                        }
                    }
                }
            }
            if (this.DigitalOnly) {
                if (string.IsNullOrEmpty(this.Text) == false && this.Text != m_negativeSign.ToString()) {
                    float f = 0f;
                    if (float.TryParse(this.Text, out f) == false) {
                        Console.WriteLine("非数字，本不应该出现. 文本：" + this.Text, true);
                        this.Text = this.Min.ToString();
                    }
                    //if (f > this.Max) {
                    //    e.Handled = true;
                    //    this.Text = this.Max.ToString();
                    //    this.SelectAll();
                    //    return;
                    //}
                    //if (f < this.Min) {
                    //    e.Handled = true;
                    //    this.Text = this.Min.ToString();
                    //    this.SelectAll();
                    //    return;
                    //}
                }
            }
            base.OnKeyUp(e);
        }


        #region 增加
        #region  member fields

        private const int m_MaxDecimalLength = 10;  // max dot length
        private const int m_MaxValueLength = 27; // decimal can be 28 bits.

        private const int WM_CHAR = 0x0102; // char key message
        private const int WM_CUT = 0x0300;  // mouse message in ContextMenu
        private const int WM_COPY = 0x0301;
        private const int WM_PASTE = 0x0302;
        private const int WM_CLEAR = 0x0303;

        private int m_decimalLength = 0;
        private bool m_allowNegative = true;
        private string m_valueFormatStr = string.Empty;

        private char m_decimalSeparator = '.';
        private char m_negativeSign = '-';
        #endregion
        /// <summary>
        /// 浮点模式 ^\d+(\.\d+)?$
        /// </summary>
        string regex = @"^(-?\d+)(\.\d+)?$";
        //
        protected virtual bool isNumber(string s) {
            return System.Text.RegularExpressions.Regex.IsMatch(s, regex);
        }

        #endregion
        protected override void OnKeyPress(KeyPressEventArgs e) {
            if (m_DigitalOnly) {
                //首先看看是不是数字
                if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar > '９' || e.KeyChar < '０') &&
                    e.KeyChar != m_negativeSign
                    && e.KeyChar != m_decimalSeparator
                    && e.KeyChar != '\b' && e.KeyChar != (char)3 && e.KeyChar != (char)1 && e.KeyChar != (char)22) {
                    e.Handled = true;
                    return;
                }
                //如果是ctrl+v，那要看看是不是数字
                if (e.KeyChar == (char)22) {
                    string s = System.Windows.Forms.Clipboard.GetText();
                    float f;
                    if (float.TryParse(s, out f) == false) {
                        e.Handled = true;
                        return;
                    }
                    else {
                        if (f > m_Max || f < m_Min) {
                            e.Handled = true;
                            return;
                        }
                    }
                }
                int len = this.Text.Length;

                //超出精度的处理
                if (e.KeyChar <= '９' && e.KeyChar >= '０') {
                    char cdiag = '0';
                    switch (e.KeyChar) {
                        case '０':
                            cdiag = '0';
                            break;
                        case '１':
                            cdiag = '1';
                            break;
                        case '２':
                            cdiag = '2';
                            break;
                        case '３':
                            cdiag = '3';
                            break;
                        case '４':
                            cdiag = '4';
                            break;
                        case '５':
                            cdiag = '5';
                            break;
                        case '６':
                            cdiag = '6';
                            break;
                        case '７':
                            cdiag = '7';
                            break;
                        case '８':
                            cdiag = '8';
                            break;
                        default:
                            cdiag = '9';
                            break;

                    }
                    e.KeyChar = cdiag;
                }
                if (char.IsDigit(e.KeyChar)) {
                    if (this.SelectionLength == 0) {
                        //如果光标位置在小数点之前，不需要处理精度
                        if (this.SelectionStart <= GetDSPos()) {
                            return;
                        }
                        //没有选中的情况，如果小数点后的位数已大于规定，则直接返回
                        if (len > 0) {
                            int pos = this.Text.IndexOf(m_decimalSeparator);

                            if (pos != -1 && len - pos > m_DigitalRound) {
                                e.Handled = true;
                                return;
                            }
                            //是否超出范围
                            //float v = float.Parse(txt_Internal.Text + new string(e.KeyChar, 1));
                            //if (v > m_Max || v < m_Min) {
                            //    e.Handled = true;
                            //    return;
                            //}
                        }
                    }
                    else {
                        //有选中，则覆盖
                        //是否超出范围
                        //string txt = txt_Internal.Text;

                    }

                }
                if (e.KeyChar == m_negativeSign) {
                    if (m_allowNegative == false) {
                        e.Handled = true;
                        return;
                    }
                }
                if (this.SelectionLength == len) {
                    //全选的情况下,随便怎么弄.
                    base.OnKeyPress(e);
                    return;
                }

                //负号处理，只允许一个，并且必须在开头
                if (e.KeyChar == m_negativeSign) {
                    if (m_allowNegative == false) {
                        e.Handled = true;
                        return;
                    }
                    if (len >= 1) {
                        if (this.SelectionStart > 0) {
                            e.Handled = true;
                            return;
                        }

                        //如果已经有一个负号了，则返回
                        if (this.Text.IndexOf(m_negativeSign) >= 0) {
                            e.Handled = true;
                            return;
                        }
                        //if (txt_Internal.SelectionLength == 0) {
                        //    e.Handled = true;
                        //    return;
                        //}

                    }
                }

                //点号处理，只允许一个
                if (e.KeyChar == m_decimalSeparator) {
                    if (this.Text.Length >= 1) {
                        if (this.SelectionLength > 0) {
                            //如果选中的里面有点号，则认为是覆盖
                            if (this.SelectedText.IndexOf(m_decimalSeparator) >= 0) {

                            }
                            else {
                                //否则直接返回
                                e.Handled = true;
                                return;
                            }
                        }
                        else if (this.Text.IndexOf(m_decimalSeparator) >= 0) {
                            //如果未被选中的文本里有点号，则直接返回
                            e.Handled = true;
                            return;
                        }
                    }
                    //return;
                }
            }
            base.OnKeyPress(e);
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            //下面的处理只是为了不引发keydown事件
            if (m_DigitalOnly) {
                if (e.KeyValue == 229 || (e.KeyValue >= 48 && e.KeyValue <= 57)) {
                    if (this.SelectionLength == 0) {
                        //如果光标位置在小数点之前，不需要处理精度
                        if (this.SelectionStart <= GetDSPos()) {
                            return;
                        }
                        //没有选中的情况，如果小数点后的位数已大于规定，则直接返回
                        int len = this.Text.Length;
                        if (len > 0) {
                            int pos = this.Text.IndexOf(m_decimalSeparator);

                            if (len - pos > m_DigitalRound) {
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                    else {
                        //有选中，则覆盖
                    }
                }

            }

            base.OnKeyDown(e);
        }

        protected override void OnEnter(EventArgs e) {
            m_txtHasFocus = true;
            base.OnEnter(e);
            this.Invalidate();

        }


        protected override void OnSizeChanged(EventArgs e) {
            m_PaintRect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
            m_NormalRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            base.OnSizeChanged(e);
        }


        public const int WM_PAINT = 15;
        public const int WM_CTLCOLOREDIT = 307;

        public const int WM_NCPAINT = 133;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern System.IntPtr GetWindowDC([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hDC);
        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_PASTE) {
                //如果是粘贴消息，那digitalonly就要处理一下
                if (this.DigitalOnly) {
                    string s = System.Windows.Forms.Clipboard.GetText();
                    float f;
                    if (float.TryParse(s, out f) == false) {
                        return;
                    }
                    else {
                        if (f > m_Max || f < m_Min) {
                            return;
                        }
                    }
                }
            }
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT || m.Msg == WM_CTLCOLOREDIT && this.BorderStyle != BorderStyle.None) {

                IntPtr hDC = GetWindowDC(m.HWnd);
                if (hDC.ToInt32() == 0) {
                    return;
                }
                if (this.Width < 1 || this.Height < 1) {
                    return;
                }
                using (System.Drawing.Graphics g = Graphics.FromHdc(hDC)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    try {
                        if (this.Focused == false) {
                            if (m_MouseState == ControlState.NoFocus) {
                                g.DrawRectangle(m_NormalPen2, m_NormalRect);
                            }
                            if (string.IsNullOrEmpty(this.Text) && string.IsNullOrEmpty(this.WaterMarkText) == false) {
                                using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Black))) {
                                    g.DrawString(this.WaterMarkText, SystemFonts.DefaultFont, sb, this.WaterMarkTextOffset);
                                }
                            }
                        }
                        if (this.Focused || m_MouseState == ControlState.Hover) {
                            g.DrawRectangle(m_FocusPen, 0, 0, this.Width - 1, this.Height - 1);
                        }
                    }
                    catch (Exception ee) {
                        Console.WriteLine("GDI+错误", ee, true);
                    }
                }
                // }
                //返回结果
                m.Result = IntPtr.Zero;
                //释放
                ReleaseDC(m.HWnd, hDC);
            }
        }

        #region IRibbon 成员

        public void OnRibbonThemeChanged() {
            if (m_FocusPen != null) {
                m_FocusPen = new Pen(Color.FromArgb(50, RibbonThemeManager.BackColor), 2f);
            }
            if (m_NormalPen != null) {
                m_NormalPen = new Pen(Color.FromArgb(40, RibbonThemeManager.BackColor), 2f);
            }
            if (m_NormalPen2 != null) {
                m_NormalPen2 = new Pen(Color.FromArgb(120, RibbonThemeManager.BackColor), 1f);
            }
        }

        #endregion

        /// <summary>
        /// 是否允许负数
        /// </summary>
        public bool AllowNegative {
            get {
                return m_allowNegative;
            }
            set {
                m_allowNegative = value;
            }
        }
        private bool m_DigitalOnly = false;
        public bool DigitalOnly {
            get {
                return m_DigitalOnly;
            }
            set {
                if (m_DigitalOnly != value) {
                    m_DigitalOnly = value;
                    if (m_DigitalOnly && string.IsNullOrEmpty(this.Text)) {
                        MakeDigitalRound();
                    }
                }
            }
        }
        private int m_DigitalRound = 2;
        public int DigitalRound {
            get {
                return m_DigitalRound;
            }
            set {
                m_DigitalRound = value;
                //txt_Internal.Text = (((float)Math.Round(f * 100f, 0)) / 100f).ToString();
                //txt_Internal.SelectionStart = txt_Internal.Text.Length;
            }
        }
        private float m_Max = float.MaxValue;
        private float m_Min = float.MinValue;
        public float Max {
            get {
                return m_Max;
            }
            set {
                m_Max = value;
            }
        }
        public float Min {
            get {
                return m_Min;
            }
            set {
                m_Min = value;
            }
        }
        //获取小数点后的数值
        private string GetStringAfterDS() {
            int pos = this.Text.IndexOf(m_decimalSeparator);
            if (pos < 0) {
                return "";
            }

            int len = this.Text.Length;

            return this.Text.Substring(pos, len - pos - 1);
        }
        private int GetDSPos() {
            return this.Text.IndexOf(m_decimalSeparator);
        }
        private int GetLenAfterDS(string str) {
            int pos = str.IndexOf(m_decimalSeparator);
            if (pos < 0) {
                return 0;
            }

            int len = str.Length;

            return len - pos - 1;
        }
        //构造默认精度位数的数字字符串
        private string MakeDefaultDigital() {
            if (m_DigitalRound == 0) {
                return "0";
            }
            else {
                return "0." + new string('0', m_DigitalRound);
            }
        }
        //补满精度位数
        private string MakeDigitalRound(string str) {
            if (string.IsNullOrEmpty(str)) {
                return MakeDefaultDigital();
            }

            int ds = GetLenAfterDS(str);
            if (ds == 0) {
                if (str.IndexOf(m_decimalSeparator) < 0) {
                    if (m_DigitalRound == 0) {
                        return str;
                    }
                    else {
                        return str + "." + new string('0', m_DigitalRound);
                    }
                }
                else {
                    return str + new string('0', m_DigitalRound);
                }
            }

            else {
                if (ds > m_DigitalRound) {
                    str = str.Substring(0, str.IndexOf(m_decimalSeparator) + m_DigitalRound + 1);
                    return str;
                }
                else {
                    return str + new string('0', m_DigitalRound - ds);
                }

            }
        }

        //内部文字变化，不引发事件。
        private bool m_internalTextChg = false;
        private void MakeDigitalRound() {
            m_internalTextChg = true;
            this.Text = MakeDigitalRound(this.Text);
            m_internalTextChg = false;
        }

        #region IGetValue 成员

        public string GetValue() {
            return this.Text;
        }

        public string GetValue(string arg) {
            //throw new NotImplementedException();
            return string.Empty;
        }

        #endregion
    }
}
