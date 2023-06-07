using System;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    /// <summary>
    /// This class adds on to the functionality provided in System.Windows.Forms.MenuStrip.
    /// </summary>
    /// <remarks>
    /// 关于MenuStrip，我发现的第一件令人恼火的事情是它没有“点击通过”。
    ///如果托管MenuStrip的表单没有处于活动状态，并且你点击了工具栏中的按钮，它会将焦点设置为表单，但不会点击按钮。
    ///这在很多情况下都是有意义的，但对于Paint.NET来说绝对不是。
    /// The first aggravating thing I found out about the new toolstrips is that they do not "click through."
    /// If the form that is hosting a toolstrip is not active and you click on a button in the toolstrip, it 
    /// sets focus to the form but does NOT click the button. This makes sense in many situations, but 
    /// definitely not for Paint.NET.
    /// </remarks>
    public class MenuStripEx
        : MenuStrip
    {
        private bool clickThrough = true;
        private static int openCount = 0;

        public MenuStripEx() {
            this.ImageScalingSize = new System.Drawing.Size(UI.ScaleWidth(16), UI.ScaleHeight(16));
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
                UI.ClickThroughWndProc(ref m);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any menu is currently open.
        /// </summary>
        /// <remarks>
        /// To be precise, this will return true if any menu has raised its MenuActivate event
        /// but has yet to raise its MenuDeactivate event.</remarks>
        public static bool IsAnyMenuActive {
            get {
                return openCount > 0;
            }
        }

        public static void PushMenuActivate() {
            ++openCount;
        }

        public static void PopMenuActivate() {
            --openCount;
        }

        protected override void OnMenuActivate(EventArgs e) {
            ++openCount;
            base.OnMenuActivate(e);
        }

        protected override void OnMenuDeactivate(EventArgs e) {
            --openCount;
            base.OnMenuDeactivate(e);
        }

    }
}
