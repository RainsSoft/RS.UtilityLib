using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.IME
{
    public partial class IMEForm : Form
    {
        public IMEForm() {
            InitializeComponent();
        }
        /*
         msg=0x100 (WM_KEYDOWN) hwnd=0x1602498 wparam=0x53 lparam=0x1f0001 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x102 (WM_CHAR) hwnd=0x1602498 wparam=0x73 lparam=0x1f0001 result=0x0
msg=0x101 (WM_KEYUP) hwnd=0x1602498 wparam=0x44 lparam=0xc0200001 result=0x0
msg=0x101 (WM_KEYUP) hwnd=0x1602498 wparam=0x53 lparam=0xc01f0001 result=0x0
msg=0x101 (WM_KEYUP) hwnd=0x1602498 wparam=0x41 lparam=0xc01e0001 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x100 (WM_KEYDOWN) hwnd=0x1602498 wparam=0x44 lparam=0x200001 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x102 (WM_CHAR) hwnd=0x1602498 wparam=0x64 lparam=0x200001 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x87 (WM_GETDLGCODE) hwnd=0x1602498 wparam=0x0 lparam=0x0 result=0x0
msg=0x100 (WM_KEYDOWN) hwnd=0x1602498 wparam=0x41 lparam=0x1e0001 result=0x0
         */
        private void imeControl1_OnWndProcImeMsgEvent(IIMEControl imeControl, ref Message m) {
            return;
            IMEControl ctl = imeControl as IMEControl;//Control.FromChildHandle(m.HWnd);
            if (ctl == null) return;
            //IIMEControl imeCtl = ctl as IIMEControl;
            //winMsg = m;
#if DEBUG
            //Console.WriteLine(m.ToString());
#endif
            if (imeControl.ImeAllowed == false) {
                return;
            }
            {
                if (m.Msg == 0x102) {
                    char c = (char)m.WParam.ToInt32();
                    //Console.WriteLine(c.ToString());                    
                    if (imeControl.IsBackspaceChar(c)) {
                        int len = ctl.RenderText.Length;
                        if (len > 0) {
                            ctl.RenderText = ctl.RenderText.Remove(len-1);
                            ctl.Invalidate();
                            return;
                        }
                    }
                    if (imeControl.IsAsciiPrintChar(c)) {
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
            }
            if (m.Msg == NativeIME.WM_IME_SETCONTEXT) {
                if (m.WParam.ToInt32() == 1) {                    
                    //必须在自定义控件内的onload先获取ime的handle
                    //这里这样调用不起作用
                    var FhImc = imeControl.HImcHandle;//NativeIME.ImmGetContext(ctl.Handle);
                    NativeIME.ImmAssociateContext(ctl.Handle, FhImc);
                    NativeIME.ImmReleaseContext(ctl.Handle, FhImc);
                }
            }
            return;
            if (m.Msg == NativeIME.WM_IME_COMPOSITION) {
                if ((m.LParam.ToInt32() & (int)NativeIME.GCS_RESULTSTR) != 0) {
                    string txt;
                    bool ret = IMEHandler.GetResult(ctl.Handle, (uint)m.LParam.ToInt32(), out txt);
                    if (ret) {
                        for (int i = 0; i < txt.Length; i++) {
                            //InputManager.Instance.InjectKeyPress(KeyCode.None,
                            //txt[i]);
                        }
                        ctl.RenderText += txt;
                        ctl.Invalidate();
                        return;
                    }
                }


            }
        }
    }
}
