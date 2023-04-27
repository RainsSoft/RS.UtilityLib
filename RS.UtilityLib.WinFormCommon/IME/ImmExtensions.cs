using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.IME
{
    /// <summary>
    /// 输入法拓展
    /// </summary>
    public static class ImmExtensions
    {

        /// <summary>
        /// 判断指定的窗口中输入法是否打开
        /// </summary>
        /// <returns>输入法是否打开</returns>
        public static bool ImmIsOpen(IntPtr win_Handle) {
            int hImc = ImmGetContext(win_Handle);
            if (hImc == 0)
                return false;
            bool res = ImmGetOpenStatus(hImc);
            ImmReleaseContext(win_Handle, hImc);
            return res;
        }

        /// <summary>
        /// 为指定的窗口设置输入法的位置
        /// </summary>
        /// <param name="win"></param>
        /// <param name="x">输入法位置的X坐标</param>
        /// <param name="y">输入法位置的Y坐标</param>
        public static void ImmSetPos(IntPtr win_Handle, int x, int y) {
            int hImc = ImmGetContext(win_Handle);
            if (hImc != 0) {
                CompositionForm frm2 = new CompositionForm();
                frm2.CurrentPos.X = x;
                frm2.CurrentPos.Y = y;
                frm2.Style = (int)CandidateFormStyle.CFS_POINT;
                ImmSetCompositionWindow(hImc, ref frm2);
                //iError = Kernel32.GetLastError();
                Marshal.GetLastWin32Error();
                ImmReleaseContext(win_Handle, hImc);
            }
        }


        private const int IME_CMODE_FULLSHAPE = 0x8;
        private const int IME_CHOTKEY_SHAPE_TOGGLE = 0x11;
        /// <summary>
        /// 为指定的窗口打开输入法并设置为中文半角状态。
        /// </summary>
        /// <returns>操作是否成功。</returns>
        public static bool ImmSetHalf(IntPtr win_Handle) {
            int hIMC = ImmGetContext(win_Handle);
            if (hIMC == 0)
                return false;
            int iMode = 0;
            int iSentence = 0;
            bool isSuccess = ImmGetConversionStatus(hIMC, ref iMode, ref iSentence); //检索输入法信息
            if (isSuccess) {
                if ((iMode & IME_CMODE_FULLSHAPE) > 0) //如果处于全角状态
                {
                    int _hr = ImmSimulateHotKey(win_Handle, IME_CHOTKEY_SHAPE_TOGGLE); //转换成半角状态
                    return true;
                }
            }
            return false;

        }



        private static int IMode = int.MinValue;
        private static int ISentence = int.MinValue;


        /// <summary>
        /// 备份转换状态
        /// </summary>
        /// <returns>操作是否成功</returns>
        public static bool ImmBackConversionStatus(IntPtr win_Handle) {
            int hIMC = ImmGetContext(win_Handle);
            if (hIMC == 0) {
                ISentence = int.MinValue;
                IMode = int.MinValue;
                return false;
            }
            bool result = ImmGetConversionStatus(hIMC, ref IMode, ref ISentence);
            ImmReleaseContext(win_Handle, hIMC);
            return result;
        }

        /// <summary>
        /// 还原转换状态
        /// </summary>
        /// <returns>操作是否成功</returns>
        public static bool ImmRestoreConversionStatus(IntPtr win_Handle) {
            if (IMode == int.MinValue)
                return false;
            int hIMC = ImmGetContext(win_Handle);
            if (hIMC == 0)
                return false;

            int m = 0;
            int s = 0;
            ImmGetConversionStatus(hIMC, ref m, ref s);
            ImmSetConversionStatus(hIMC, IMode, ISentence);
            ImmReleaseContext(win_Handle, hIMC);
            return true;
        }


        /// <summary>
        /// 是否为更新输入法位置的消息
        /// </summary>
       // public static bool Imm_IsWM_IME_NOTIFY_IMN_SETOPENSTATUS(this IWin32Window win, Message msg) => win != null && msg.Msg == 642 && msg.WParam.ToInt32() == 8;




        #region Data Struct

        private enum CandidateFormStyle
        {
            CFS_DEFAULT = 0x0000,
            CFS_RECT = 0x0001,
            CFS_POINT = 0x0002,
            CFS_FORCE_POSITION = 0x0020,
            CFS_CANDIDATEPOS = 0x0040,
            CFS_EXCLUDE = 0x0080
        }


        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        private struct CandidateForm
        {
#pragma warning disable 0649
            public int dwIndex;
            public int dwStyle;
            public Point ptCurrentPos;
            public RECT rcArea;
#pragma warning restore 0649
            //public Rectangle rcArea;
        }

        private struct CompositionForm
        {
            public int Style;
            public Point CurrentPos;
            public RECT Area;
            //public Rectangle Area;
        }

        #endregion


        #region Windows API
        //[SecurityCritical]
        //[SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint ImmGetCompositionString(IntPtr hIMC, uint dwIndex, byte[] lpBuf, uint dwBufLen);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        internal static extern int ImmGetContext(IntPtr hwnd);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern int ImmCreateContext();

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmDestroyContext(int hImc);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmSetCandidateWindow(int hImc, ref CandidateForm frm);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmSetStatusWindowPos(int hImc, ref Point pos);


        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ImmReleaseContext(IntPtr hwnd, int hImc);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmGetOpenStatus(int hImc);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmSetCompositionWindow(int hImc, ref CompositionForm frm);


        /// <summary>
        /// 获取当前转换状态，用于判断中文半角或全角。
        /// </summary>
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmGetConversionStatus(int hIMC, ref int iMode, ref int iSentence);
        /// <summary>
        /// 设置当前转换状态，用于设置中文半角或全角。
        /// </summary>
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmSetConversionStatus(int hIMC, int iMode, int iSentence);
        /// <summary>
        /// 在指定的窗口中模拟一个特定的IME热键动作，以触发该窗口相应的响应动作。
        /// </summary>
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        internal static extern int ImmSimulateHotKey(IntPtr hWnd, int lngHotkey);

        #endregion
        //[DllImport("imm32.dll")]
        //public static extern IntPtr ImmGetContext(IntPtr hwnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        public static extern bool ImmGetOpenStatus(IntPtr himc);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr himc, bool b);
        [DllImport("imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr himc, ref int lpdw, ref int lpdw2);
        [DllImport("imm32.dll")]
        //public static extern int ImmSimulateHotKey(IntPtr hwnd, int lngHotkey);
        //[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        static extern int ImmGetCompositionString(IntPtr hIMC, uint dwIndex, char[] lpBuf, uint dwBufLen);
        [DllImport("imm32.dll")]
        public static extern bool ImmIsIME(int hkl);
        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);
        static uint GCS_RESULTSTR = 0x0800;
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int MultiByteToWideChar(int CodePage, int dwFlags, char[] lpMultiByteStr, int cchMultiByte, char[] lpWideCharStr, int cchWideChar);

    }
}
