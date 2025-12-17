using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.EmbedForm
{
    /// <summary>
    /// 进程嵌入窗口
    /// </summary>
    public static class ProcessEmbed {
        /// <summary>
        /// 将程序嵌入控件
        /// </summary>
        /// <param name="app">嵌入程序</param>
        /// <param name="control">指定控件</param>
        public static void EmbedProcess(Process app, Control control, bool removeAppWinCloseAndMin = true, bool removeAppFormControlBox = false) {
            //验证程序和控件非空
            if(app == null || control == null || app.HasExited)
                return;

            ProcessMainWinHandleHelper.RefreshProcess(app, 500);
            IntPtr pHandle = app.MainWindowHandle;
            try {
                //    if(app.HasExited ||
                //    ProcessMainWinHandleHelper.GetMainWindowHandle(app) == IntPtr.Zero) {
                //    return;
                //}
                //IntPtr pHandle = ProcessMainWinHandleHelper.GetMainWindowHandle(app);
                if(app.HasExited || app.MainWindowHandle == IntPtr.Zero) {
                    return;
                }

                //核心代码：嵌入程序  

                Win32API.SetParent(pHandle, control.Handle);
                // if(app.MainWindowTitle.Contains("Python") == false) {
                if(removeAppWinCloseAndMin) {
                    IntPtr Win_MENU = Win32API.GetSystemMenu(pHandle, false);
                    uint SC_CLOSE = 0xF060;//关闭
                    uint SC_MINIMIZE = 0xF020;   //最小化
                    uint SC_MAXIMIZE = 0xF030;   //最大化
                    uint SC_RESTORE = 0xF120;     //还原
                    uint MF_Disable = 0x0;//禁用
                    uint MF_REMOVE = 0x1000;//移除
                    Win32API.RemoveMenu(Win_MENU, SC_CLOSE, MF_Disable);//禁用关闭按钮                    
                    Win32API.RemoveMenu(Win_MENU, SC_MINIMIZE, MF_REMOVE);//移除最小化按钮
                }
                if(removeAppFormControlBox == false) {
                    //移除嵌入的窗口的窗口标题栏
                    // int GWL_STYLE = (-16);
                    // int WS_VISIBLE = 0x10000000;
                    // Win32API.SetWindowLong(new HandleRef(control, pHandle), Win32API.GWL_STYLE, Win32API.WS_VISIBLE);
                    Win32API.SetWindowLong(pHandle, Win32API.GWL_STYLE, Win32API.WS_VISIBLE);
                }
                // }

            }
            catch(Exception) {
            }
            try {
                Win32API.ShowWindow(pHandle, 1);
                //System.Threading.Thread.Sleep(10);
                //将嵌入的窗口欧放置到合适位置，填满宽高
                Win32API.MoveWindow(pHandle, 0, 0, control.Width, control.Height, true);
                //正常1，最小化2，最大化3              
                //ShowWindow(pHandle, 3);
            }
            catch(Exception) { }

        }
        public static void EmbedProcessOnlyResize(Process app, Control control) {
            if(app == null || control == null || app.HasExited)
                return;
            ProcessMainWinHandleHelper.RefreshProcess(app, 500);
            if(app.HasExited ||
                ProcessMainWinHandleHelper.GetMainWindowHandle(app) == IntPtr.Zero) {
                return;
            }
            try {
                //将嵌入的窗口欧放置到合适位置，填满宽高
                Win32API.MoveWindow(ProcessMainWinHandleHelper.GetMainWindowHandle(app), 0, 0, control.Width, control.Height, true);
            }
            catch(Exception) { }
        }
        public static void EmbedProcessOnlyResize(IntPtr appMainFormHandle, Control control) {
            if(appMainFormHandle == null || control == null || appMainFormHandle == IntPtr.Zero)
                return;
            try {
                //将嵌入的窗口欧放置到合适位置，填满宽高
                Win32API.MoveWindow(appMainFormHandle, 0, 0, control.Width, control.Height, true);
            }
            catch(Exception) { }
        }
  
        /// <summary>
        /// 禁止通过拖动，双击标题栏改变窗体大小。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool WndProc_DisableDBClick2ResizeWindow(ref Message m) {
            //禁止通过拖动，双击标题栏改变窗体大小。
            const int WM_NCLBUTTONDBLCLK = 0xA3;
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 2;
            if(m.Msg == WM_NCLBUTTONDOWN && m.WParam.ToInt32() == HTCAPTION)
                return true;
            if(m.Msg == WM_NCLBUTTONDBLCLK)
                return true;

            return false;
        }
        /// <summary>
        /// 禁用窗口的close按钮
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        public static bool RemoveFormCloseMenu(Form frm) {
            try {
                IntPtr CLOSE_MENU = Win32API.GetSystemMenu(frm.Handle, false);
                const int SC_CLOSE = 0xF060;
                Win32API.RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);//关闭按钮禁用
                return true;
            }
            catch(Exception) {

            }
            return false;
        }

        public static bool EmbedForm(Form childFrm, Control control, bool useWin32API) {
            try {
                if(useWin32API) {
                    childFrm.Show();
                    // Place the window in the top left of the parent window without resizing it
                    //SetWindowPos(childFrm.Handle, 0, 0, 0, 0, 0, 0x0001 | 0x0040);
                    try {
                        //核心代码：嵌入程序
                        Win32API.SetParent(childFrm.Handle, control.Handle);
                    }
                    catch(Exception) { }
                    //try {
                    //    //移除嵌入的窗口的窗口标题栏
                    //    SetWindowLong(new HandleRef(this, childFrm.Handle), GWL_STYLE, WS_VISIBLE);
                    //}
                    //catch(Exception) { }
                    try {
                        //将嵌入的窗口欧放置到合适位置，填满宽高
                        Win32API.MoveWindow(childFrm.Handle, 0, 0, control.Width, control.Height, true);
                    }
                    catch(Exception) { }
                    return true;
                }
                else {
                    childFrm.TopLevel = false;
                    childFrm.Dock = DockStyle.Fill;
                    control.Controls.Add(childFrm);
                    childFrm.Show();
                }
                RemoveFormCloseMenu(childFrm);
            }
            catch(Exception ee) { }
            return false;
        }

        public static bool EmbendForm2MdiContainer(Form childFrm, Form mdiContainer) {
            if(mdiContainer.IsMdiContainer && childFrm.IsMdiContainer == false) {
                try {
                    childFrm.MdiParent = mdiContainer;
                    childFrm.Show();
                    //
                    IntPtr CLOSE_MENU = Win32API.GetSystemMenu(childFrm.Handle, false);
                    uint SC_CLOSE = 0xF060;
                    Win32API.RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);//关闭按钮禁用
                    return true;
                }
                catch(Exception ee) {
                }
            }
            return false;
        }

        internal class Win32API {
            #region Win32 API
            [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
                 CharSet = CharSet.Unicode, ExactSpelling = true,
                 CallingConvention = CallingConvention.StdCall)]
            public static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
            public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
            //public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong) {
            //    if(IntPtr.Size == 4) {
            //        return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            //    }
            //    return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            //}
            [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
            public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, int dwNewLong);
            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
            public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

            [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
            public static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, uint lParam);

            /// <summary>
            /// 获取系统错误信息描述
            /// </summary>
            /// <param name="errCode">系统错误码</param>
            /// <returns></returns>
            public static string GetLastError() {
                var errCode = Marshal.GetLastWin32Error();
                IntPtr tempptr = IntPtr.Zero;
                string msg = null;
                FormatMessage(0x1300, ref tempptr, errCode, 0, ref msg, 255, ref tempptr);
                return msg;
            }
            /// <summary>
            /// 获取系统错误信息描述
            /// </summary>
            /// <param name="errCode">系统错误码</param>
            /// <returns></returns>
            public static string GetLastErrorString(int errCode) {
                IntPtr tempptr = IntPtr.Zero;
                string msg = null;
                FormatMessage(0x1300, ref tempptr, errCode, 0, ref msg, 255, ref tempptr);
                return msg;
            }

            [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
            public extern static int FormatMessage(int flag, ref IntPtr source, int msgid, int langid, ref string buf, int size, ref IntPtr args);


            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetParent(IntPtr hwnd);
            ///// <summary>
            ///// ShellExecute(IntPtr.Zero, "Open", "C:/Program Files/TTPlayer/TTPlayer.exe", "", "", 1);
            ///// </summary>
            ///// <param name="hwnd"></param>
            ///// <param name="lpOperation"></param>
            ///// <param name="lpFile"></param>
            ///// <param name="lpParameters"></param>
            ///// <param name="lpDirectory"></param>
            ///// <param name="nShowCmd"></param>
            ///// <returns></returns>
            //[DllImport("shell32.dll", EntryPoint = "ShellExecute")]
            //public static extern int ShellExecute(
            //IntPtr hwnd,
            //string lpOperation,
            //string lpFile,
            //string lpParameters,
            //string lpDirectory,
            //int nShowCmd
            //);
            //[DllImport("kernel32.dll")]
            //public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId); 


            /// <summary>
            ///窗口 最小化2 最大化3 正常1, 隐藏0
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="nCmdShow"></param>
            /// <returns></returns>
            [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll ", EntryPoint = "GetSystemMenu")]
            public extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport("user32.dll ", EntryPoint = "RemoveMenu")]
            public extern static bool RemoveMenu(IntPtr hMenu, uint nPos, uint flags);

            public const int SWP_NOOWNERZORDER = 0x200;
            public const int SWP_NOREDRAW = 0x8;
            public const int SWP_NOZORDER = 0x4;
            public const int SWP_SHOWWINDOW = 0x0040;
            public const int WS_EX_MDICHILD = 0x40;
            public const int SWP_FRAMECHANGED = 0x20;
            public const int SWP_NOACTIVATE = 0x10;
            public const int SWP_ASYNCWINDOWPOS = 0x4000;
            public const int SWP_NOMOVE = 0x2;
            public const int SWP_NOSIZE = 0x1;
            public const int GWL_STYLE = (-16);
            public const uint WS_VISIBLE = 0x10000000;
            public const int WM_CLOSE = 0x10;
            public const int WS_CHILD = 0x40000000;

            public const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
            public const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
            public const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
            public const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
            public const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
            public const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
            public const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
            public const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
            public const int SW_MINIMIZE = 6; //{最小化, 不激活}
            public const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
            public const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
            public const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
            public const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
            public const int SW_MAX = 10; //{同 SW_SHOWNORMAL}

            //const int PROCESS_ALL_ACCESS = 0x1F0FFF;
            //const int PROCESS_VM_READ = 0x0010;
            //const int PROCESS_VM_WRITE = 0x0020;     
            #endregion Win32 API


        }

    }
}
