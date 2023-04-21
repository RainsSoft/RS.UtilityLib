using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public static class NativeAPI
    {

        [DllImport("user32.dll", EntryPoint = "ShowScrollBar")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar([In()] IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        public const int GWL_STYLE = -16;
        public const int WS_HSCROLL = 1048576;
        public const int WS_VSCROLL = 2097152;
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);


        public const int LVM_FIRST = 0x1000;
        public const int LVM_INSERTITEMA = LVM_FIRST + 7;
        public const int LVM_INSERTITEMW = LVM_FIRST + 77;
        public const int LVM_DELETEITEM = LVM_FIRST + 8;
        public const int LVM_SCROLL = LVM_FIRST + 20;
    }

    internal class Win32Native
    {
        #region Win32 User Messages / Structures

        /// <summary>Window messages</summary>
        public enum WindowMessage : uint
        {
            // Misc messages
            Destroy = 0x0002,
            Close = 0x0010,
            Quit = 0x0012,
            Paint = 0x000F,
            SetCursor = 0x0020,
            ActivateApplication = 0x001C,
            EnterMenuLoop = 0x0211,
            ExitMenuLoop = 0x0212,
            NonClientHitTest = 0x0084,
            PowerBroadcast = 0x0218,
            SystemCommand = 0x0112,
            GetMinMax = 0x0024,

            // Keyboard messages
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            Character = 0x0102,
            SystemKeyDown = 0x0104,
            SystemKeyUp = 0x0105,
            SystemCharacter = 0x0106,

            // Mouse messages
            MouseMove = 0x0200,
            LeftButtonDown = 0x0201,
            LeftButtonUp = 0x0202,
            LeftButtonDoubleClick = 0x0203,
            RightButtonDown = 0x0204,
            RightButtonUp = 0x0205,
            RightButtonDoubleClick = 0x0206,
            MiddleButtonDown = 0x0207,
            MiddleButtonUp = 0x0208,
            MiddleButtonDoubleClick = 0x0209,
            MouseWheel = 0x020a,
            XButtonDown = 0x020B,
            XButtonUp = 0x020c,
            XButtonDoubleClick = 0x020d,
            MouseFirst = LeftButtonDown, // Skip mouse move, it happens a lot and there is another message for that
            MouseLast = XButtonDoubleClick,

            // Sizing
            EnterSizeMove = 0x0231,
            ExitSizeMove = 0x0232,
            Size = 0x0005,

            //windows自定义
            SC_MOVE = 0xF010,
            HTCAPTION = 0x0002,

        }

        /// <summary>Mouse buttons</summary>
        public enum MouseButtons
        {
            Left = 0x0001,
            Right = 0x0002,
            Middle = 0x0010,
            Side1 = 0x0020,
            Side2 = 0x0040,
        }

        /// <summary>Windows Message</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public WindowMessage msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        /// <summary>MinMax Info structure</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInformation
        {
            public System.Drawing.Point reserved;
            public System.Drawing.Point MaxSize;
            public System.Drawing.Point MaxPosition;
            public System.Drawing.Point MinTrackSize;
            public System.Drawing.Point MaxTrackSize;
        }

        /// <summary>Monitor Info structure</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInformation
        {
            public uint Size; // Size of this structure
            public System.Drawing.Rectangle MonitorRectangle;
            public System.Drawing.Rectangle WorkRectangle;
            public uint Flags; // Possible flags
        }
        #endregion
        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
            //public SCROLLINFO() {
            //    this.cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            //}

            public SCROLLINFO(int mask, int min, int max, int page, int pos) {
                this.cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
                this.fMask = mask;
                this.nMin = min;
                this.nMax = max;
                this.nPage = page;
                this.nPos = pos;
                nTrackPos = 0;
            }
        }

        private SCROLLINFO m_scrollInfo = new SCROLLINFO();
        #region Windows API calls
        [DllImport("user32.dll", EntryPoint = "SetScrollInfo")]
        public static extern int SetScrollInfo([In()] IntPtr hwnd, int nBar, [In()] ref SCROLLINFO lpsi, [MarshalAs(UnmanagedType.Bool)] bool redraw);
        [DllImport("user32.dll", EntryPoint = "GetScrollInfo")]
        public static extern int GetScrollInfo([In()] IntPtr hwnd, int nBar, [In()] ref SCROLLINFO lpsi);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        public static extern IntPtr timeBeginPeriod(uint period);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(ref long PerformanceCount);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hWnd, ref MonitorInformation info);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(uint key);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetCapture(IntPtr handle);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReleaseCapture();
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out System.Drawing.Point p);
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetCaretBlinkTime();

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetKeyState")]
        private static extern short GetKeyState(int nVirtKey);
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "GetCurrentThreadId")]
        public static extern uint GetCurrentThreadId();
        public struct tagRECT
        {

            /// LONG->int
            public int left;

            /// LONG->int
            public int top;

            /// LONG->int
            public int right;

            /// LONG->int
            public int bottom;
        }
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowRect")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool GetWindowRect([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, [System.Runtime.InteropServices.OutAttribute()] out tagRECT lpRect);
        #endregion

        #region 常量

        #endregion

        #region Class Methods
        /// <summary>Returns the low word</summary>
        public static short LoWord(uint l) {
            return (short)(l & 0xffff);
        }
        /// <summary>Returns the high word</summary>
        public static short HiWord(uint l) {
            return (short)(l >> 16);
        }

        /// <summary>Makes two shorts into a long</summary>
        public static uint MakeUInt32(short l, short r) {
            return (uint)((l & 0xffff) | ((r & 0xffff) << 16));
        }

        /// <summary>Is this key down right now 注意control不在此范围内</summary>
        public static bool IsKeyDown(System.Windows.Forms.Keys key) {
            return (GetAsyncKeyState((uint)key) & 0x8000f) != 0;

        }

        /// <summary>
        /// 获取控件的滚动位置
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nBar">0:H 1:V</param>
        /// <returns>如果失败,返回-1</returns>
        public static bool GetScrollPos(IntPtr hwnd, int nBar, out SCROLLINFO result) {
            result = new SCROLLINFO(16 + 1 + 2 + 4, 0, 0, 0, 0); //获取位置+获取Range
            int ret = GetScrollInfo(hwnd, nBar, ref result);
            if (ret != 0) {
                return true;
            }
            else {
                return false;
            }
        }
        #endregion

    }
}
