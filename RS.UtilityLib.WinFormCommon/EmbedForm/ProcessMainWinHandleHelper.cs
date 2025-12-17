using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.EmbedForm
{
    /// <summary>
    /// 根据进程ID获取进程窗口辅助类
    /// </summary>
    public class ProcessMainWinHandleHelper {
        /// <summary>
        /// 自定义获取进程主窗口(可以正确获得python shell的显示窗口句柄)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IntPtr GetMainWindowHandle(Process p) {
#if !DEBUG
            return p.MainWindowHandle;
#endif
            try {
                //方法1
                ProcessMainWinHandleHelper pwh = new ProcessMainWinHandleHelper();
                if(p.ProcessName == "pythonw") {
                    //特殊处理python shell
                    pwh.ExcludeWindowTitles.Insert(0,"idle");
                    pwh.IncludeWindowTitleChars = "Python";
                    pwh.IncludeWindowTitleCharsIsStart = true;
                }
                return pwh.GetMainWindowHandle(p.Id);

                //方法2
                IntPtr _hWnds = IntPtr.Zero;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                do {
                    System.Threading.Thread.Sleep(10);
                    string windowName = p.MainWindowTitle;
                    //如果窗口标题栏名称不固定 则这个不好使
                    _hWnds = FindWindow(null, windowName);
                    if(sw.ElapsedMilliseconds > 500) {
                        break;
                    }
                } while(_hWnds == IntPtr.Zero);
                return _hWnds;
                //方法3
                p.Refresh();
                if(p.HasExited == false) {
                    //隐藏窗口是取不到MainWindowHandle的
                    return p.MainWindowHandle;
                }
            }
            catch(Exception ee) {
                Console.WriteLine("进程未启动或已退出：" + ee.ToString());
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 刷新进程,直到找到窗口句柄
        /// </summary>
        /// <param name="p">进程</param>
        /// <param name="timeoutMS">超时：单位毫秒</param>
        public static void RefreshProcess(Process p, int timeoutMS = 100) {
            try {
                if(p == null || p.HasExited) {
                    return;
                }
                int timeout = timeoutMS;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while(GetMainWindowHandle(p) == IntPtr.Zero) {
                    //System.Threading.Thread.Sleep(10);
                    p.Refresh();
                    if(p.HasExited)
                        break;
                    try {
                        p.WaitForInputIdle(10);
                    }
                    catch {
                        //非图形界面
                    }
                    if(sw.ElapsedMilliseconds > timeout) {
                        sw.Stop();
                        break;
                    }
                }
            }
            catch(Exception ee) {

            }
        }
        /// <summary>
        /// 检查进程是否在运行
        /// </summary>
        /// <param name="appProcess"></param>
        /// <returns></returns>
        public static bool CheckProcessRun(Process appProcess) {
            try {

                if(appProcess == null || appProcess.HasExited) {
                    return false;
                }
                RefreshProcess(appProcess, 500);
                if(appProcess.HasExited ||
                    GetMainWindowHandle(appProcess) == IntPtr.Zero) {
                    return false;
                }
                return true;
            }
            catch(Exception ee) {

            }
            return false;
        }

#region 方法2
        ///// <summary>
        ///// 根据进程标题内容获取进程窗口ID，可获取到隐藏窗口handle
        ///// </summary>
        ///// <param name="MainWindowTitle"></param>
        ///// <returns></returns>
        //static IntPtr GetMainWindowHandle(string MainWindowTitle) {
        //    IntPtr _hWnds = IntPtr.Zero;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    do {
        //        System.Threading.Thread.Sleep(10);
        //        string windowName = MainWindowTitle;
        //        //如果窗口标题栏名称不固定 则这个不好使
        //        _hWnds = FindWindow(null, windowName);
        //        if(sw.ElapsedMilliseconds > 50) {
        //            //超时跳出
        //            break;
        //        }
        //    } while(_hWnds == IntPtr.Zero);
        //    return _hWnds;
        //}
#endregion

#region 方法 3
        ///// <summary>
        ///// 当前方法是取不到隐藏窗口的handle
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //static IntPtr GetMainWindowHandle(Process p) {
        //    try {
        //        //方法3
        //        p.Refresh();
        //        if(p.HasExited == false) {
        //            //隐藏窗口是取不到MainWindowHandle的
        //            return p.MainWindowHandle;
        //        }
        //    }
        //    catch(Exception ee) {

        //    }
        //    return IntPtr.Zero;
        //}
#endregion

        private bool haveMainWindow = false;
        private IntPtr mainWindowHandle = IntPtr.Zero;
        private int processId = 0;
        /// <summary>
        /// 排除窗口标题
        /// </summary>
        public List<string> ExcludeWindowTitles = new List<string>() {
            "","A","B","C","D","E","F","G","H","I","J","K",
            "L","M","N","O","P","Q","R","S","T","U","V","W",
            "X","Y","Z"
        };
        /// <summary>
        /// 窗口标题包含文本
        /// </summary>
        public string IncludeWindowTitleChars;
        public bool IncludeWindowTitleCharsIsStart=false;
        /// <summary>
        /// 是否仅查找可见窗口
        /// </summary>
        public bool OnlyVisibleWindow = false;
        ///// <summary>
        ///// 是否包含空标题窗口
        ///// </summary>
        //public bool IncludeEmptyWindowTitle = true;
        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        public IntPtr GetMainWindowHandle(int processId) {
            if(!this.haveMainWindow) {
                this.mainWindowHandle = IntPtr.Zero;
                this.processId = processId;
                //https://www.coder.work/article/1627797
                //https://stackoverflow.com/questions/52360046/
                EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
                EnumWindows(callback, IntPtr.Zero);
                GC.KeepAlive(callback);

                this.haveMainWindow = true;
            }
            return this.mainWindowHandle;
        }

        private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter) {
            int num;
            GetWindowThreadProcessId(new HandleRef(this, handle), out num);
            if((num == this.processId)
                && this.IsMainWindow(handle)
                ) {
                this.mainWindowHandle = handle;
                return false;
            }
            return true;
        }

        private bool IsMainWindow(IntPtr handle) {
            //是否找到窗口
            bool handPtrZero = (GetWindow(new HandleRef(this, handle), 4) == IntPtr.Zero);
            if(handPtrZero == false) {
                return false;
            }
            //是否仅查找可见窗口          
            if(this.OnlyVisibleWindow) {
                if(IsWindowVisible(new HandleRef(this, handle)) == false) {
                    return false;
                }
            }
            //标题匹配
            //bool titleMatch = true;            
            int len = GetWindowTextLength(handle);
            string title = "";
            if(len > 0) {
                StringBuilder text = new StringBuilder(len +1);
                GetWindowText(handle, text, text.Capacity);
                title = text.ToString();
            }
            //if(this.IncludeEmptyWindowTitle == false && string.IsNullOrEmpty(title)) {
            //    //titleMatch = false;
            //    //不包含空标题窗口
            //    return false;
            //}
            if(this.ExcludeWindowTitles != null && this.ExcludeWindowTitles.Count > 0) {
                if(this.ExcludeWindowTitles.Contains(title)) {
#if DEBUG
                    Console.WriteLine("排除进程窗口标题：" + title);
#endif
                    return false;
                }
            }
            if(!string.IsNullOrEmpty(this.IncludeWindowTitleChars)) {
                bool ret = this.IncludeWindowTitleCharsIsStart ? title.StartsWith(this.IncludeWindowTitleChars) : title.Contains(this.IncludeWindowTitleChars);
                //if(title.IndexOf(this.IncludeWindowTitleChars) < 0) {
                if(ret == false) {                
                    //没有包含字符串
                    return false;
                }
            }
#if DEBUG
            Console.WriteLine("获取到进程窗口标题：" + title);
#endif
            return true;
            //return (!(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) 
            //    //&& IsWindowVisible(new HandleRef(this, handle))
            //    &&!string.IsNullOrEmpty(title)
            //    );
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HandleRef hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        //  int GetWindowText(
        //      __in   HWND hWnd,
        //      __out  LPTSTR lpString,
        //      __in   int nMaxCount
        //  );
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowText(IntPtr hWnd, string lpString);
    }
}
