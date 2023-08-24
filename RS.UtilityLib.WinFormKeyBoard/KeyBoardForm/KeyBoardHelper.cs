using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormKeyBoard
{
    public class KeyBoardHelper
    {
        public class NativeMethods
        {
            [DllImport("gdi32.dll")]
            public static extern int CreateRoundRectRgn(
          int x1, int y1, int x2, int y2, int x3, int y3);


            [DllImport("user32.dll")]
            public static extern int SetWindowRgn(
         IntPtr hwnd, int hRgn, Boolean bRedraw);

            /// <summary>
            /// 执行动画
            /// </summary>
            /// <param name="whnd">控件句柄</param>
            /// <param name="dwtime">动画时间</param>
            /// <param name="dwflag">动画组合名称</param>
            /// <returns>bool值，动画是否成功</returns>
            [DllImport("user32.dll")]
            public static extern bool AnimateWindow(IntPtr whnd, int dwtime, int dwflag);
        }
        public static class AW
        {
            /// <summary>
            /// 从左到右显示
            /// </summary>
            public const Int32 AW_HOR_POSITIVE = 0x00000001;
            /// <summary>
            /// 从右到左显示
            /// </summary>
            public const Int32 AW_HOR_NEGATIVE = 0x00000002;
            /// <summary>
            /// 从上到下显示
            /// </summary>
            public const Int32 AW_VER_POSITIVE = 0x00000004;
            /// <summary>
            /// 从下到上显示
            /// </summary>
            public const Int32 AW_VER_NEGATIVE = 0x00000008;
            /// <summary>
            /// 若使用了AW_HIDE标志，则使窗口向内重叠，即收缩窗口；否则使窗口向外扩展，即展开窗口
            /// </summary>
            public const Int32 AW_CENTER = 0x00000010;
            /// <summary>
            /// 隐藏窗口，缺省则显示窗口
            /// </summary>
            public const Int32 AW_HIDE = 0x00010000;
            /// <summary>
            /// 激活窗口。在使用了AW_HIDE标志后不能使用这个标志
            /// </summary>
            public const Int32 AW_ACTIVATE = 0x00020000;
            /// <summary>
            /// 使用滑动类型。缺省则为滚动动画类型。当使用AW_CENTER标志时，这个标志就被忽略
            /// </summary>
            public const Int32 AW_SLIDE = 0x00040000;
            /// <summary>
            /// 透明度从高到低
            /// </summary>
            public const Int32 AW_BLEND = 0x00080000;
        }
        /// <summary>
        /// 绘制组件圆角
        /// </summary>
        /// <param name="frm">要绘制的组件</param>
        /// <param name="RgnRadius">圆角大小</param>
        public static void CreateRegion(Control ctrl, int RgnRadius) {
            int Rgn = NativeMethods.CreateRoundRectRgn(0, 0, ctrl.ClientRectangle.Width + 1, ctrl.ClientRectangle.Height + 1, RgnRadius, RgnRadius);
            NativeMethods.SetWindowRgn(ctrl.Handle, Rgn, true);
        }
    }
}
