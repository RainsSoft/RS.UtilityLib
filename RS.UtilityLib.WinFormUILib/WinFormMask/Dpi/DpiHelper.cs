using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 系统缩放比例
    /// </summary>
    public class DpiHelper
    {
        #region 公开方法

        /// <summary>
        /// 获取控件对应的系统缩放比例
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static float GetControlDpi(Control control)
        {
            IntPtr hDC = IntPtr.Zero;
            Graphics g = null;
            ControlHelper.GetWindowClientGraphics(control.Handle, out g, out hDC);

            float dpi = g.DpiX / 96f;

            g.Dispose();
            NativeMethods.ReleaseDC(control.Handle, hDC);

            return dpi;
        }

        /// <summary>
        /// 获取控件对应的系统缩放比例
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static float GetControlDpi(IntPtr hWnd)
        {
            IntPtr hDC = IntPtr.Zero;
            Graphics g = null;
            ControlHelper.GetWindowClientGraphics(hWnd, out g, out hDC);

            float dpi = g.DpiX / 96f;

            g.Dispose();
            NativeMethods.ReleaseDC(hWnd, hDC);

            return dpi;
        }
        #endregion
    }

}
