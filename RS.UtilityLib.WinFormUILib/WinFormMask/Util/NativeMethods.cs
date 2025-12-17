using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HML
{
    /// <summary>
    /// Win32
    /// </summary>
    public static class NativeMethods
    {
        #region 消息

        public const int WM_USER = 0x0400;//用户自定义消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSEENTER = NativeMethods.WM_USER + 10;// DropDrowListPlus MouseEnter消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSELEAVE = NativeMethods.WM_USER + 11;// DropDrowListPlus MouseLeave消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSEDOWN = NativeMethods.WM_USER + 12;// DropDrowListPlus MouseDown消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSEUP = NativeMethods.WM_USER + 13;// DropDrowListPlus MouseUp消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSEMOVE = NativeMethods.WM_USER + 14;// DropDrowListPlus MouseMove消息
        public const int WM_USER_DROPDROWLISTPLUS_CLOSED = NativeMethods.WM_USER + 15;// DropDrowListPlus 下拉面板Closed消息
        public const int WM_USER_DROPDROWLISTPLUS_MOUSEWHEEL = NativeMethods.WM_USER + 16;// DropDrowListPlus 下拉面板MouseWheel消息
        public const int WM_USER_DROPDROWLISTPLUS_PAINT = NativeMethods.WM_USER + 17;// DropDrowListPlus 下拉面板Paint消息


        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;

        #endregion

        #region 结构

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            public System.Drawing.Size Size
            {
                get
                {
                    return new System.Drawing.Size(this.right - this.left, this.bottom - this.top);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        }

        /// <summary>
        /// 用于描述字体的度量信息Unicode(用GetTextMetricsW来获取)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TEXTMETRIC
        {
            /// <summary>
            /// 字符的整体高度（以像素为单位）
            /// </summary>
            public int tmHeight;
            /// <summary>
            /// 字符的上部高度（以像素为单位）
            /// </summary>
            public int tmAscent;
            /// <summary>
            /// 字符的下部高度（以像素为单位）
            /// </summary>
            public int tmDescent;
            /// <summary>
            /// 字符的内部行间距（以像素为单位）
            /// </summary>
            public int tmInternalLeading;
            /// <summary>
            /// 字符的外部行间距（以像素为单位）
            /// </summary>
            public int tmExternalLeading;
            /// <summary>
            /// 字符的平均宽度（以像素为单位）
            /// </summary>
            public int tmAveCharWidth;
            /// <summary>
            /// 字符的最大宽度（以像素为单位）
            /// </summary>
            public int tmMaxCharWidth;
            /// <summary>
            /// 字符的粗细程度（0-1000之间的值）
            /// </summary>
            public int tmWeight;
            /// <summary>
            /// 字符的悬挂宽度（以像素为单位）
            /// </summary>
            public int tmOverhang;
            /// <summary>
            /// 字符的X轴像素宽度
            /// </summary>
            public int tmDigitizedAspectX;
            /// <summary>
            /// 字符的Y轴像素宽度
            /// </summary>
            public int tmDigitizedAspectY;
            /// <summary>
            /// 字体中的第一个字符
            /// </summary>
            public char tmFirstChar;
            /// <summary>
            /// 字体中的最后一个字符
            /// </summary>
            public char tmLastChar;
            /// <summary>
            /// 字体中的默认字符
            /// </summary>
            public char tmDefaultChar;
            /// <summary>
            /// 字体中的换行字符
            /// </summary>
            public char tmBreakChar;
            /// <summary>
            /// 是否为斜体字体（非零表示是）
            /// </summary>
            public byte tmItalic;
            /// <summary>
            /// 是否有下划线（非零表示有）
            /// </summary>
            public byte tmUnderlined;
            /// <summary>
            /// 是否有删除线（非零表示有）
            /// </summary>
            public byte tmStruckOut;
            /// <summary>
            /// 字体的字间距和字体系列
            /// </summary>
            public byte tmPitchAndFamily;
            /// <summary>
            /// 字体的字符集
            /// </summary>
            public byte tmCharSet;



            /// <summary>
            /// 获取字体高度，用于绘制（ tmHeight = 字符的整体高度 ）
            /// </summary>
            /// <returns></returns>
            public int GetFontRealHeight()
            {
                return this.tmHeight;
            }

            /// <summary>
            /// 获取字体高度，也就是自带库中Font.Height（ tmHeight + tmInternalLeading + tmExternalLeading = 字符的整体高度 + 字符的内部行间距 + 字符的外部行间距 ）
            /// </summary>
            /// <returns></returns>
            public int GetFontHeight()
            {
                return this.tmHeight + this.tmInternalLeading + this.tmExternalLeading;
            }

            /// <summary>
            /// 获取高度，用于定型时使用高度（ tmExternalLeading + tmInternalLeading + tmHeight + tmInternalLeading + tmExternalLeading = 字符的外部行间距 + 字符的内部行间距 + 字符的整体高度 + 字符的内部行间距 + 字符的外部行间距 ）
            /// </summary>
            /// <returns></returns>
            public int GetFontStereotypesHeight()
            {
                return this.tmHeight + this.tmInternalLeading + this.tmExternalLeading;
            }

            /// <summary>
            /// 获取外间距，用于定型时外间距（ tmExternalLeading + tmInternalLeading = 字符的外部行间距 + 字符的内部行间距 ）
            /// </summary>
            /// <returns></returns>
            public int GetFontStereotypesMargim()
            {
                return this.tmExternalLeading;
            }

        }

        /// <summary>
        /// 用于描述字体的度量信息Ansi(用GetTextMetricsA来获取)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TEXTMETRICA
        {
            /// <summary>
            /// 字符的整体高度（以像素为单位）
            /// </summary>
            public int tmHeight;
            /// <summary>
            /// 字符的上部高度（以像素为单位）
            /// </summary>
            public int tmAscent;
            /// <summary>
            /// 字符的下部高度（以像素为单位）
            /// </summary>
            public int tmDescent;
            /// <summary>
            /// 字符的内部行间距（以像素为单位）
            /// </summary>
            public int tmInternalLeading;
            /// <summary>
            /// 字符的外部行间距（以像素为单位）
            /// </summary>
            public int tmExternalLeading;
            /// <summary>
            /// 字符的平均宽度（以像素为单位）
            /// </summary>
            public int tmAveCharWidth;
            /// <summary>
            /// 字符的最大宽度（以像素为单位）
            /// </summary>
            public int tmMaxCharWidth;
            /// <summary>
            /// 字符的粗细程度（0-1000之间的值）
            /// </summary>
            public int tmWeight;
            /// <summary>
            /// 字符的悬挂宽度（以像素为单位）
            /// </summary>
            public int tmOverhang;
            /// <summary>
            /// 字符的X轴像素宽度
            /// </summary>
            public int tmDigitizedAspectX;
            /// <summary>
            /// 字符的Y轴像素宽度
            /// </summary>
            public int tmDigitizedAspectY;
            /// <summary>
            /// 字体中的第一个字符
            /// </summary>
            public byte tmFirstChar;
            /// <summary>
            /// 字体中的最后一个字符
            /// </summary>
            public byte tmLastChar;
            /// <summary>
            /// 字体中的默认字符
            /// </summary>
            public byte tmDefaultChar;
            /// <summary>
            /// 字体中的换行字符
            /// </summary>
            public byte tmBreakChar;
            /// <summary>
            /// 是否为斜体字体（非零表示是）
            /// </summary>
            public byte tmItalic;
            /// <summary>
            /// 是否有下划线（非零表示有）
            /// </summary>
            public byte tmUnderlined;
            /// <summary>
            /// 是否有删除线（非零表示有）
            /// </summary>
            public byte tmStruckOut;
            /// <summary>
            /// 字体的字间距和字体系列
            /// </summary>
            public byte tmPitchAndFamily;
            /// <summary>
            /// 字体的字符集
            /// </summary>
            public byte tmCharSet;
        }

        #endregion

        #region 扩展

        [DllImport("User32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("User32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport("User32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 检索指定窗口的边界矩形的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="hWnd">窗口的句柄</param>
        /// <param name="rect">指向 RECT 结构的指针，用于接收窗口左上角和右下角的屏幕坐标</param>
        /// <returns></returns>
        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref RECT rect);

        /// <summary>
        /// 激活指定窗口
        /// </summary>
        /// <param name="hWnd">窗口的句柄</param>
        /// <returns></returns>
        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hWnd);



        /// <summary>
        /// 获得的设备环境覆盖了整个窗口（包括非客户区）
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        /// <summary>
        /// 用于获得hWnd参数所指定窗口的客户区域的一个设备环境
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("User32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        /// <summary>
        /// 上下文设置对象
        /// </summary>
        /// <param name="hDC">上下文</param>
        /// <param name="hObject">要设置对象</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        public static extern int GetTextMetricsW(IntPtr hDC, [In, Out] ref TEXTMETRIC lptm);
        [DllImport("gdi32", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern int GetTextMetricsA(IntPtr hDC, [In, Out] ref TEXTMETRICA lptm);
        /// <summary>
        /// 获取描述字体的度量信息(自动识别Unicode、Ansi)
        /// </summary>
        /// <param name="hDC">字体对象所在上下文</param>
        /// <param name="lptm">用于存储用于描述字体的度量信息</param>
        /// <returns>获取是否成功</returns>
        public static int GetTextMetrics(IntPtr hDC, ref TEXTMETRIC lptm)
        {
            if (Marshal.SystemDefaultCharSize == 1)
            {
                NativeMethods.TEXTMETRICA lptmA = new NativeMethods.TEXTMETRICA();
                int retVal = GetTextMetricsA(hDC, ref lptmA);

                lptm.tmHeight = lptmA.tmHeight;
                lptm.tmAscent = lptmA.tmAscent;
                lptm.tmDescent = lptmA.tmDescent;
                lptm.tmInternalLeading = lptmA.tmInternalLeading;
                lptm.tmExternalLeading = lptmA.tmExternalLeading;
                lptm.tmAveCharWidth = lptmA.tmAveCharWidth;
                lptm.tmMaxCharWidth = lptmA.tmMaxCharWidth;
                lptm.tmWeight = lptmA.tmWeight;
                lptm.tmOverhang = lptmA.tmOverhang;
                lptm.tmDigitizedAspectX = lptmA.tmDigitizedAspectX;
                lptm.tmDigitizedAspectY = lptmA.tmDigitizedAspectY;
                lptm.tmFirstChar = (char)lptmA.tmFirstChar;
                lptm.tmLastChar = (char)lptmA.tmLastChar;
                lptm.tmDefaultChar = (char)lptmA.tmDefaultChar;
                lptm.tmBreakChar = (char)lptmA.tmBreakChar;
                lptm.tmItalic = lptmA.tmItalic;
                lptm.tmUnderlined = lptmA.tmUnderlined;
                lptm.tmStruckOut = lptmA.tmStruckOut;
                lptm.tmPitchAndFamily = lptmA.tmPitchAndFamily;
                lptm.tmCharSet = lptmA.tmCharSet;

                return retVal;
            }
            else
            {
                return GetTextMetricsW(hDC, ref lptm);
            }
        }



        /// <summary>
        /// DPI的缩放由程序自己处理
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);



        public static int MAKELONG(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        public static IntPtr MAKELPARAM(int low, int high)
        {
            return (IntPtr)((high << 16) | (low & 0xffff));
        }

        /// <summary>
        /// 高16位（无符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int)(long)n));
        }
        /// <summary>
        /// 高16位（无符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        /// <summary>
        /// 低16位（无符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int)(long)n));
        }
        /// <summary>
        /// 低16位（无符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        /// <summary>
        /// 高16位（有符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int SignedHIWORD(IntPtr n)
        {
            return SignedHIWORD(unchecked((int)(long)n));
        }
        /// <summary>
        /// 高16位（有符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int SignedHIWORD(int n)
        {
            return (int)(short)((n >> 16) & 0xffff);
        }

        /// <summary>
        /// 低16位（有符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int SignedLOWORD(IntPtr n)
        {
            return SignedLOWORD(unchecked((int)(long)n));
        }
        /// <summary>
        /// 低16位（有符号）
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int SignedLOWORD(int n)
        {
            return (int)(short)(n & 0xFFFF);
        }




        /// <summary>
        /// 结构转指针
        /// </summary>
        /// <typeparam name="T">结构类型</typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IntPtr StructToIntPtr<T>(T info)
        {
            int size = Marshal.SizeOf(info);
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(info, intPtr, true);
            return intPtr;
        }

        /// <summary>
        /// 指针转结构
        /// </summary>
        /// <typeparam name="T">结构类型</typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static T IntPtrToStruct<T>(IntPtr info)
        {
            return (T)Marshal.PtrToStructure(info, typeof(T));
        }



        #endregion
    }
}
