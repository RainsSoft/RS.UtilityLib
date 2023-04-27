using RS.UtilityLib.WinFormCommon.TextRender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    processImeInput(this,ref m);
                }
            }
            if (OnWndProcImeMsgEvent != null) {
                OnWndProcImeMsgEvent(this,ref m);
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
                else if (imeControl.IsAsciiPrintChar(c)||c==' ') {
                    ctl.RenderText += c.ToString();
                    ctl.Invalidate();
                    return;
                }

            }
            if (m.Msg == NativeIME.WM_IME_CHAR) {
                //输入法输出字符
                char c = (char)m.WParam.ToInt32();
                ctl.RenderText += c.ToString();
                ctl.Invalidate();

            }
            if (m.Msg == NativeIME.WM_IME_SETCONTEXT) {
                //打开输入法
                if (m.WParam.ToInt32() == 1) {
                    //必须在自定义控件内的onload先获取ime的handle
                    //这里这样调用不起作用
                    var FhImc = imeControl.HImcHandle;//NativeIME.ImmGetContext(ctl.Handle);
                    NativeIME.ImmAssociateContext(ctl.Handle, FhImc);
                    NativeIME.ImmReleaseContext(ctl.Handle, FhImc);
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
            if (m.Msg == NativeIME.WM_IME_SETCONTEXT) {
                //打开输入法
                if (m.WParam.ToInt32() == 1) {
                    //必须在自定义控件内的onload先获取ime的handle
                    //这里这样调用不起作用
                    var FhImc = imeControl.HImcHandle;//NativeIME.ImmGetContext(ctl.Handle);
                    NativeIME.ImmAssociateContext(ctl.Handle, FhImc);
                    NativeIME.ImmReleaseContext(ctl.Handle, FhImc);
                }
            }
            if (m.Msg == NativeIME.WM_IME_COMPOSITION) {
                if ((m.LParam.ToInt32() & (int)NativeIME.GCS_RESULTSTR) != 0) {
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
            m_hImc = NativeIME.ImmGetContext(Handle);
        }

        private const int IME_CMODE_FULLSHAPE = 0x8;
        private const int IME_CHOTKEY_SHAPE_TOGGLE = 0x11;
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            return;
            //
            this.FindForm().ImeMode = ImeMode.On;
            var handle = this.FindForm().Handle;
            IntPtr HIme = NativeIME.ImmGetContext(handle);
            //如果输入法处于打开状态  
            if (NativeIME.ImmGetOpenStatus(HIme)) {
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
    }

}
