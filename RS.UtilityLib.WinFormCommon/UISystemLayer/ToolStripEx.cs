using System;
using System.Windows.Forms;


namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    /// <summary>
    /// 此类添加到System.Windows.Forms.ToolStrip中提供的功能中
    /// This class adds on to the functionality provided in System.Windows.Forms.ToolStrip.
    /// </summary>
    /// <remarks>
    ///关于ToolStrip，我发现的第一件令人恼火的事情是它没有“点击通过”。
    ///如果托管ToolStrip的表单没有处于活动状态，并且你点击了工具栏中的按钮，它会将焦点设置为表单，但不会点击按钮。
    ///这在很多情况下都是有意义的，但对于Paint.NET来说绝对不是。
    /// The first aggravating thing I found out about ToolStrip is that it does not "click through."
    /// If the form that is hosting a ToolStrip is not active and you click on a button in the tool
    /// strip, it sets focus to the form but does NOT click the button. This makes sense in many
    /// situations, but definitely not for Paint.NET.
    /// </remarks>
    public class ToolStripEx
        : ToolStrip
    {
        private bool clickThrough = true;
        private bool managedFocus = true;
        private static int enteredComboBox = 0;

        public ToolStripEx() {
            ToolStripProfessionalRenderer tspr = this.Renderer as ToolStripProfessionalRenderer;

            if (tspr != null) {
                tspr.ColorTable.UseSystemColors = true;
                tspr.RoundedEdges = false;
            }

            this.ImageScalingSize = new System.Drawing.Size(UISystemLayer.UI.ScaleWidth(16), UISystemLayer.UI.ScaleHeight(16));
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData) {
            bool processed = false;
            Form form = this.FindForm();
            FormEx formEx = FormEx.FindFormEx(form);

            if (formEx != null) {
                processed = formEx.RelayProcessCmdKey(keyData);
            }

            return processed;
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
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const uint MA_ACTIVATEANDEAT = 2;
        public const uint MA_ACTIVATE = 1;
        /// <summary>
        /// This WndProc implements click-through functionality. Some controls (MenuStrip, ToolStrip) will not
        /// recognize a click unless the form they are hosted in is active. So the first click will activate the
        /// form and then a second is required to actually make the click happen.
        /// </summary>
        /// <param name="m">The Message that was passed to your WndProc.</param>
        /// <returns>true if the message was processed, false if it was not</returns>
        /// <remarks>
        /// You should first call base.WndProc(), and then call this method. This method is only intended to
        /// change a return value, not to change actual processing before that.
        /// </remarks>
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
        /// <summary>
        /// This event is raised when this toolstrip instance wishes to relinquish focus.
        /// </summary>
        public event EventHandler RelinquishFocus;

        private void OnRelinquishFocus() {
            if (RelinquishFocus != null) {
                RelinquishFocus(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets whether the toolstrip manages focus.
        /// </summary>
        /// <remarks>
        /// If this is true, the toolstrip will capture focus when the mouse enters its client area. It will then
        /// relinquish focus (via the RelinquishFocus event) when the mouse leaves. It will not capture or
        /// attempt to relinquish focus if MenuStripEx.IsAnyMenuActive returns true.
        /// </remarks>
        public bool ManagedFocus {
            get {
                return this.managedFocus;
            }

            set {
                this.managedFocus = value;
            }
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e) {
            ToolStripComboBox tscb = e.Item as ToolStripComboBox;

            if (tscb == null) {
                e.Item.MouseEnter += OnItemMouseEnter;
            }
            else {
                tscb.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);
                tscb.Enter += new EventHandler(ComboBox_Enter);
                tscb.Leave += new EventHandler(ComboBox_Leave);
            }

            base.OnItemAdded(e);
        }

        private void ComboBox_Leave(object sender, EventArgs e) {
            --enteredComboBox;
        }

        private void ComboBox_Enter(object sender, EventArgs e) {
            ++enteredComboBox;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e) {
            OnRelinquishFocus();
        }

        protected override void OnItemRemoved(ToolStripItemEventArgs e) {
            ToolStripComboBox tscb = e.Item as ToolStripComboBox;

            if (tscb == null) {
                e.Item.MouseEnter -= OnItemMouseEnter;
            }
            else {
                tscb.DropDownClosed -= new EventHandler(ComboBox_DropDownClosed);
                tscb.Enter -= new EventHandler(ComboBox_Enter);
                tscb.Leave -= new EventHandler(ComboBox_Leave);
            }

            base.OnItemRemoved(e);
        }

        private void OnItemMouseEnter(object sender, EventArgs e) {
            if (this.managedFocus && !MenuStripEx.IsAnyMenuActive && UI.IsOurAppActive && enteredComboBox == 0) {
                this.Focus();
            }
        }

        protected override void OnMouseLeave(EventArgs e) {
            if (this.managedFocus && !MenuStripEx.IsAnyMenuActive && UI.IsOurAppActive && enteredComboBox == 0) {
                OnRelinquishFocus();
            }

            base.OnMouseLeave(e);
        }
    }
}
