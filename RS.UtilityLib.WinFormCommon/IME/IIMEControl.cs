﻿using RS.UtilityLib.WinFormCommon.TextRender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.IME
{
    /// <summary>
    /// 输入法消息委托
    /// </summary>
    /// <param name="m"></param>
    public delegate void WndProcImeMsgHandler(IIMEControl imeControl, ref Message m);
    /// <summary>
    /// 渲染控件
    /// </summary>
    public interface IIMEControl
    {

        /// <summary>
        /// 控件消息处理,处理输入法输入事件
        /// </summary>
        event WndProcImeMsgHandler OnWndProcImeMsgEvent;
        bool ImeAllowed { get; }
        IntPtr HImcHandle { get; }
        /// <summary>
        /// 判断是否是可打印ascii字符 0~127
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsAsciiPrintChar(char c);
        /// <summary>
        /// 判断是否回退字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsBackspaceChar(char c);
        /// <summary>
        /// 判断是否回车字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsEnterChar(char c);
        string RenderText { get; set; }
    }
    public class IMEControl : UserControl, IIMEControl
    {
        public IMEControl() : base() {
            this.ImeMode = ImeMode.OnHalf;
        }
        /// <summary>
        /// Indicates that IME is allowed (for CJK language entering)
        /// </summary>
        [Browsable(false)]
        public bool ImeAllowed {
            get {
                return ImeMode != ImeMode.Disable &&
                       ImeMode != ImeMode.Off &&
                       ImeMode != ImeMode.NoControl;
            }
        }
        /// <summary>
        /// ime handle
        /// </summary>
        public IntPtr HImcHandle {
            get {
                return m_hImc;
            }
        }
        private IntPtr m_hImc = IntPtr.Zero;
        public bool IsAsciiPrintChar(char c) {
            int cno = (int)c;
            if (cno < 33 || cno == 127) {
                //0~32以及127是不可打印字符
                return false;
            }
            if (c > 127) {
                return false;
            }
            return true;
        }
        public bool IsBackspaceChar(char c) {
            return c == '\b';
        }
        public bool IsEnterChar(char c) {
            return c == '\r';
        }
        #region
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_ENDSCROLL = 0x8;
        #endregion
        /// <summary>
        /// 控件消息处理,处理输入法输入事件
        /// </summary>
        public event WndProcImeMsgHandler OnWndProcImeMsgEvent;
        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL) {
                if (m.WParam.ToInt32() != SB_ENDSCROLL) {
                    Invalidate();
                }
            }
            //MsgProcManager.OnMsgProc(this.Handle, m);
            base.WndProc(ref m);
            if (ImeAllowed) {
                // the usercontrol will receive a WM_IME_SETCONTEXT message
                //when it gets focused and loses focus respectively
                // when the usercontrol gets focused, the m.WParam is 1
                // when the usercontrol loses focus, the m.WParam is 0
                // only when the usercontrol gets focused, we need to call the
                //IMM function to associate itself to the default input context
                //if (m.Msg == WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1) {
                //    NativeIME.ImmAssociateContext(Handle, m_hImc);
                //}
                bool simpleProcessImeInput = false;
                if (simpleProcessImeInput) {
                    processImeInputSimple(this, ref m);
                }
                else {
                    //比如容易集成到第3方不支持ime输入法的文本输入库输入
                    processImeInput(this, ref m);
                }
            }
            if (OnWndProcImeMsgEvent != null) {
                OnWndProcImeMsgEvent(this, ref m);
            }
        }
        private void processImeInputSimple(IIMEControl imeControl, ref Message m) {
            IMEControl ctl = imeControl as IMEControl;//Control.FromChildHandle(m.HWnd);
            if (imeControl.ImeAllowed == false) {
                return;
            }
            if (m.Msg == 0x102) {
                char c = (char)m.WParam.ToInt32();
                //Console.WriteLine(c.ToString());                    
                if (imeControl.IsBackspaceChar(c)) {
                    int len = ctl.RenderText.Length;
                    if (len > 0) {
                        ctl.RenderText = ctl.RenderText.Remove(len - 1);
                        ctl.Invalidate();
                        return;
                    }
                }
                else if (imeControl.IsEnterChar(c)) {
                    ctl.RenderText += "\n";
                }
                else if (imeControl.IsAsciiPrintChar(c) || c == ' ') {
                    ctl.RenderText += c.ToString();
                    ctl.Invalidate();
                    return;
                }

            }
            if (m.Msg == IMENative.WM_IME_CHAR) {
                //输入法输出字符
                char c = (char)m.WParam.ToInt32();
                ctl.RenderText += c.ToString();
                ctl.Invalidate();

            }
            if (m.Msg == IMENative.WM_IME_SETCONTEXT) {
                //打开输入法
                if (m.WParam.ToInt32() == 1) {
                    //必须在自定义控件内的onload先获取ime的handle
                    //这里这样调用不起作用
                    var FhImc = imeControl.HImcHandle;//NativeIME.ImmGetContext(ctl.Handle);
                    IMENative.ImmAssociateContext(ctl.Handle, FhImc);
                    IMENative.ImmReleaseContext(ctl.Handle, FhImc);
                }
            }
        }
        private void processImeInput(IIMEControl imeControl, ref Message m) {
            IMEControl ctl = imeControl as IMEControl;//Control.FromChildHandle(m.HWnd);
            if (imeControl.ImeAllowed == false) {
                return;
            }
            if (m.Msg == 0x102) {
                //非输入法状态下输入
                char c = (char)m.WParam.ToInt32();
                //Console.WriteLine(c.ToString());                    
                if (imeControl.IsBackspaceChar(c)) {
                    int len = ctl.RenderText.Length;
                    if (len > 0) {
                        ctl.RenderText = ctl.RenderText.Remove(len - 1);
                        ctl.Invalidate();
                        return;
                    }
                }
                else if (imeControl.IsEnterChar(c)) {
                    ctl.RenderText += "\n";
                }
                else if (imeControl.IsAsciiPrintChar(c) || c == ' ') {
                    ctl.RenderText += c.ToString();
                    ctl.Invalidate();
                    return;
                }

            }
            //下面为输入法输入处理
            if (m.Msg == IMENative.WM_IME_SETCONTEXT) {
                //打开输入法
                if (m.WParam.ToInt32() == 1) {
                    //必须在自定义控件内的onload先获取ime的handle
                    //这里这样调用不起作用
                    var FhImc = imeControl.HImcHandle;//NativeIME.ImmGetContext(ctl.Handle);
                    IMENative.ImmAssociateContext(ctl.Handle, FhImc);
                    IMENative.ImmReleaseContext(ctl.Handle, FhImc);
                }
            }
            if (m.Msg == IMENative.WM_IME_COMPOSITION) {
                if ((m.LParam.ToInt32() & (int)IMENative.GCS_RESULTSTR) != 0) {
                    string txt;
                    bool ret = IMEHandler.GetResult(ctl.Handle, (uint)m.LParam.ToInt32(), out txt);
                    if (ret) {
                        //获取输入法内容
                        for (int i = 0; i < txt.Length; i++) {
                            //MyGui.InputManager.Instance.InjectKeyPress(KeyCode.None,
                            //txt[i]);
                        }
                        ctl.RenderText += txt;
                        ctl.Invalidate();
                        return;
                    }
                }


            }
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            //需要在onload的时候取得ime输入法句柄，其他地方不一定能取到输入法句柄
            m_hImc = IMENative.ImmGetContext(Handle);
        }

        private const int IME_CMODE_FULLSHAPE = 0x8;
        private const int IME_CHOTKEY_SHAPE_TOGGLE = 0x11;
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if (this.ImeAllowed) {
                //修改输入法的位置
                var p=this.PointToScreen(new Point(this.Width,this.Height));
                Ime.SetIMEWindowLocation(this.HImcHandle,p.X,p.Y);
            }
            return;
            //
            this.FindForm().ImeMode = ImeMode.On;
            var handle = this.FindForm().Handle;
            IntPtr HIme = IMENative.ImmGetContext(handle);
            //如果输入法处于打开状态  
            if (IMENative.ImmGetOpenStatus(HIme)) {
                int iMode = 0;
                int iSentence = 0;
                //检索输入法信息   
                bool bSuccess = ImmExtensions.ImmGetConversionStatus(HIme, ref iMode, ref iSentence);
                if (bSuccess) {
                    //如果是全角,转换成半角  
                    if ((iMode & IME_CMODE_FULLSHAPE) > 0)
                        ImmExtensions.ImmSimulateHotKey(this.Handle, IME_CHOTKEY_SHAPE_TOGGLE);
                }
            }
        }

        //private const int WM_IME_SETCONTEXT = 0x281;
        private const int WM_IME_NOTIFY = 0x282;
        //private const int WM_SETFOCUS = 0x7;
        //private const int WM_KILLFOCUS = 0x8;

        //protected override void OnGotFocus(EventArgs e) {
        //    base.OnGotFocus(e);

        //    // 发送WM_IME_SETCONTEXT消息
        //    // 使输入法在控件中显示
        //    SendMessage(this.Handle, WM_IME_SETCONTEXT, 1, 0);
        //    // 发送WM_IME_NOTIFY消息
        //    // 告诉输入法控件已经获得焦点
        //    SendMessage(this.Handle, WM_IME_NOTIFY, 0x2, 0); 

        //    InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
        //    var ptr= NativeIME.ImmGetContext(this.Handle);
        //    NativeIME.ImmSetOpenStatus(ptr, true);
        //}

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            return;
            var handle = this.FindForm().Handle;
            // 发送WM_IME_NOTIFY消息
            // 告诉输入法控件已经失去焦点
            SendMessage(handle, WM_IME_NOTIFY, 0x1, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        TextShadowRender m_TextShadowRender;
        public string RenderText { get; set; } = "输入法测试";
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (m_TextShadowRender == null) {
                m_TextShadowRender = new TextShadowRender() {
                    Alpha = 240,
                    Angle = 1,
                    Distance = 1,
                    Radius = 1
                };
            }
            if (!string.IsNullOrEmpty(this.RenderText)) {
                m_TextShadowRender.Draw(e.Graphics,
                    this.RenderText,
                    System.Drawing.SystemFonts.DefaultFont,
                    e.ClipRectangle
                    );
            }
        }

        #region
        /*
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            int x = getMatchGUIX(e.X, m_renderWnd, m_RenderCtl);
            int y = getMatchGUIY(e.Y, m_renderWnd, m_RenderCtl);
            InputManager.Instance.InjectMousePress(
               x,
               y,
               IMEInputKeyHelper.MouseFlagToMouseButton(e.Button));
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            int x = getMatchGUIX(e.X, m_renderWnd, m_RenderCtl);
            int y = getMatchGUIY(e.Y, m_renderWnd, m_RenderCtl);
            InputManager.Instance.InjectMouseRelease(
                x,
                y,
                IMEInputKeyHelper.MouseFlagToMouseButton(e.Button));
        }
        private static IntPoint m_MouseInRenderPos = new IntPoint();
        private static int getMatchGUIX(int oisX, Mogre.RenderWindow window, System.Windows.Forms.Control renderCtl) {
            int matchWidth = (int)(((float)(oisX * window.Width)) / (float)renderCtl.Width);
            m_MouseInRenderPos.left = matchWidth;
            return matchWidth;
        }
        private static int getMatchGUIY(int oisY, Mogre.RenderWindow window, System.Windows.Forms.Control renderCtl) {
            int matchHeight = (int)(((float)(oisY * window.Height)) / (float)renderCtl.Height);
            m_MouseInRenderPos.top = matchHeight;
            return matchHeight;
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            InputManager.Instance.InjectKeyPress(
                      IMEInputKeyHelper.VirtualKeyToScanCode(e.KeyCode),
                      IMEInputKeyHelper.VirtualKeyToChar(e.KeyCode));
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            InputManager.Instance.InjectKeyRelease(
              IMEInputKeyHelper.VirtualKeyToScanCode(e.KeyCode));
        }
        */
        #endregion


        #region ime
        /// <summary>
        /// Used internally, not for own use.
        /// </summary>
        internal class Ime
        {
            public Ime(IntPtr hWnd, Font font) {
                // For unknown reasons, the IME support is causing crashes when used in a WOW64 process
                // or when used in .NET 4.0. We'll disable IME support in those cases.
                string PROCESSOR_ARCHITEW6432 = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
                if (PROCESSOR_ARCHITEW6432 == "IA64" || PROCESSOR_ARCHITEW6432 == "AMD64" || Environment.OSVersion.Platform == PlatformID.Unix || Environment.Version >= new Version(4, 0)) {
                    disableIME = true;
                }
                else {
                    this.hIMEWnd = ImmGetDefaultIMEWnd(hWnd);
                }
                this.hWnd = hWnd;
                this.font = font;
                SetIMEWindowFont(font);
            }

            private Font font = null;
            public Font Font {
                get {
                    return font;
                }
                set {
                    if (!value.Equals(font)) {
                        font = value;
                        lf = null;
                        SetIMEWindowFont(value);
                    }
                }
            }

            public IntPtr HWnd {
                set {
                    if (this.hWnd != value) {
                        this.hWnd = value;
                        if (!disableIME)
                            this.hIMEWnd = ImmGetDefaultIMEWnd(value);
                        SetIMEWindowFont(font);
                    }
                }
            }

            [DllImport("imm32.dll")]
            private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, COMPOSITIONFORM lParam);
            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lParam);

            [StructLayout(LayoutKind.Sequential)]
            private class COMPOSITIONFORM
            {
                public int dwStyle = 0;
                public POINT ptCurrentPos = null;
                public RECT rcArea = null;
            }

            [StructLayout(LayoutKind.Sequential)]
            private class POINT
            {
                public int x = 0;
                public int y = 0;
            }

            [StructLayout(LayoutKind.Sequential)]
            private class RECT
            {
                public int left = 0;
                public int top = 0;
                public int right = 0;
                public int bottom = 0;
            }

            private const int WM_IME_CONTROL = 0x0283;

            private const int IMC_SETCOMPOSITIONWINDOW = 0x000c;
            private IntPtr hIMEWnd;
            private IntPtr hWnd;
            private const int CFS_POINT = 0x0002;

            [StructLayout(LayoutKind.Sequential)]
            private class LOGFONT
            {
                public int lfHeight = 0;
                public int lfWidth = 0;
                public int lfEscapement = 0;
                public int lfOrientation = 0;
                public int lfWeight = 0;
                public byte lfItalic = 0;
                public byte lfUnderline = 0;
                public byte lfStrikeOut = 0;
                public byte lfCharSet = 0;
                public byte lfOutPrecision = 0;
                public byte lfClipPrecision = 0;
                public byte lfQuality = 0;
                public byte lfPitchAndFamily = 0;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string lfFaceName = null;
            }
            private const int IMC_SETCOMPOSITIONFONT = 0x000a;
            LOGFONT lf = null;
            static bool disableIME;

            private void SetIMEWindowFont(Font f) {
                if (disableIME || hIMEWnd == IntPtr.Zero) return;

                if (lf == null) {
                    lf = new LOGFONT();
                    f.ToLogFont(lf);
                    lf.lfFaceName = f.Name;  // This is very important! "Font.ToLogFont" Method sets invalid value to LOGFONT.lfFaceName
                }

                try {
                    SendMessage(
                            hIMEWnd,
                            WM_IME_CONTROL,
                            new IntPtr(IMC_SETCOMPOSITIONFONT),
                            lf
                    );
                }
                catch (AccessViolationException ex) {
                    Handle(ex);
                }
            }

            public static void SetIMEWindowLocation(IntPtr hIMEWnd, int x, int y) {
                if (disableIME || hIMEWnd == IntPtr.Zero) return;

                POINT p = new POINT();
                p.x = x;
                p.y = y;

                COMPOSITIONFORM lParam = new COMPOSITIONFORM();
                lParam.dwStyle = CFS_POINT;
                lParam.ptCurrentPos = p;
                lParam.rcArea = new RECT();

                try {
                    SendMessage(
                            hIMEWnd,
                            WM_IME_CONTROL,
                            new IntPtr(IMC_SETCOMPOSITIONWINDOW),
                            lParam
                    );
                }
                catch (AccessViolationException ex) {
                    Handle(ex);
                }
            }

            static void Handle(Exception ex) {
                Console.WriteLine(ex);
                if (!disableIME) {
                    disableIME = true;
                    MessageBox.Show("Error calling IME: " + ex.Message + "\nIME is disabled.", "IME error");
                }
            }
        }
        #endregion
    }

}
