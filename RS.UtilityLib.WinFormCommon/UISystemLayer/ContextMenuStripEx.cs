using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    /// <summary>
    ///ContextMenuStrip，我发现的第一件令人恼火的事情是它没有“点击通过”。
    ///如果托管ContextMenuStrip的表单没有处于活动状态，并且你点击了工具栏中的按钮，它会将焦点设置为表单，但不会点击按钮。
    ///这在很多情况下都是有意义的，但对于Paint.NET来说绝对不是。
    /// </summary>
    public class ContextMenuStripEx : ContextMenuStrip
    {
        private bool clickThrough = true;

        public ContextMenuStripEx()
            : base() {
        }
        public ContextMenuStripEx(System.ComponentModel.IContainer contain)
            : base(contain) {
        }
        /// <summary>
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
        /// not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is true, which is the opposite of the behavior provided by the base
        /// ToolStrip class.
        /// </remarks>
        public bool ClickThrough {
            get {
                return this.clickThrough;
            }

            set {
                this.clickThrough = value;
            }
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            if (this.clickThrough) {
                ClickThroughWndProc(ref m);
            }
        }
        internal const uint WM_MOUSEACTIVATE = 0x21;
        internal const uint MA_ACTIVATEANDEAT = 2;
        internal const uint MA_ACTIVATE = 1;
        internal static bool ClickThroughWndProc(ref Message m) {
            bool returnVal = false;
            if (m.Msg == WM_MOUSEACTIVATE) {
                if (m.Result == (IntPtr)MA_ACTIVATEANDEAT) {
                    m.Result = (IntPtr)MA_ACTIVATE;
                    returnVal = true;
                }
            }
            return returnVal;
        }

    }
    public class ContextMenuStripItemEx : ToolStripControlHost
    {
        internal ContextMenuStripItemEx(Control userctl)
            : base(userctl) {
        }

        protected override void OnSubscribeControlEvents(Control control) {
            base.OnSubscribeControlEvents(control);
            control.Click += (control_Click);

        }

        //如果不订制Item的Click事件,则需要采用如下方式触发菜单的Click事件
        void control_Click(object sender, EventArgs e) {
            this.Tag = this.Control.Tag;
            //
            this.PerformClick();
        }
        protected override void OnUnsubscribeControlEvents(Control control) {
            base.OnUnsubscribeControlEvents(control);
            control.Click -= (control_Click);
        }

    }
}
