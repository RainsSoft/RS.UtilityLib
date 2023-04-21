using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public delegate void RibbonHintWindowShowHandler();
    public delegate void RibbonHintWindowHideHandler();
    public class RibbonHintWindow : Form
    {

        //
        public event RibbonHintWindowShowHandler ShowTipEvent;
        public event RibbonHintWindowHideHandler HideTipEvent;
        private void HandlerShowTip() {
            if (ShowTipEvent != null) {
                ShowTipEvent();
            }
        }
        private void HandlerHideTip() {
            if (HideTipEvent != null) {
                HideTipEvent();
            }
        }
        //
        public RibbonHintWindow() {
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            //base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.ControlBox = false;
            base.FormBorderStyle = FormBorderStyle.None;
            base.MinimizeBox = false;
            base.MaximizeBox = false;
            base.ShowInTaskbar = false;
            this.Text = "";
            base.TopMost = true;
            base.StartPosition = FormStartPosition.Manual;
            //this.BackColor = Color.FromArgb(250, 250, 250);
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd); //WINAPI 设置当前活动窗体的句柄

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "AnimateWindow")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool AnimateWindow([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint dwTime, uint dwFlags);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "ShowWindow")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool ShowWindow([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nCmdShow);
        public static uint AW_VER_NEGATIVE = 8;
        public static int SW_SHOWNOACTIVATE = 4;

        public static void ShowHint(RibbonHintWindow hint, int x, int y) {
            hint.HandlerShowTip();
            hint.ShowHintWindow(x, y);

        }
        /// <summary>
        /// 触发HandlerHideTip事件
        /// </summary>
        /// <param name="hint"></param>
        public static void HideHint(RibbonHintWindow hint) {
            hint.HideHintWindow();
            hint.HandlerHideTip();
        }


        /// <summary>
        /// 触发HandlerShowTip 事件
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="obj"></param>
        public static void BingObjToHint(RibbonHintWindow hint, object obj) {
            hint.Tag = obj;
            //
            //hint.HandlerShowTip();
        }
        public object Tag {
            get;
            set;
        }
        protected virtual void ShowHintWindow(int x, int y) {
            //Point p = Control.MousePosition;
            //p.Offset(Cursor.Current.Size.Width, Cursor.Current.Size.Height);
            if (this.IsDisposed) {
                return;
            }
            this.Location = new Point(x, y);

            // AnimateWindow(this.Handle, 200, AW_VER_NEGATIVE);
            ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
            //this.Visible = true;
            //SetForegroundWindow(this.Handle);
        }
        protected virtual void HideHintWindow() {
            ShowWindow(this.Handle, 0);
        }
        //protected override void OnClosed(EventArgs e) {
        //    this.Visible = false;
        //    this.OnDeactivate(e);
        //    //base.OnClosed(e);
        //}
        //
    }
}
