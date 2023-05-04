using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using AnimatorNS;
using System.Diagnostics;

namespace AnimatorNS {
    public partial class TabControlEx : System.Windows.Forms.TabControl {
        Animator animator=null;

        public TabControlEx()  {
            InitializeComponent();
            animator = new Animator();
            animator.AnimationType = AnimationType.VertSlide;
            animator.DefaultAnimation.TimeCoeff = 1f;
            animator.DefaultAnimation.AnimateOnlyDifferences = false;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation Animation {
            get {
                return animator?.DefaultAnimation;
            }
            set {
                if(animator != null) {
                    animator.DefaultAnimation = value;
                }
            }
        }
        #region Designer
        //protected override void OnHandleCreated(EventArgs e) {
        //    base.OnHandleCreated(e);
        //    if(this.IsDesignerHosted) {
        //        this.Font = new System.Drawing.Font(SystemFonts.DefaultFont.FontFamily, 10.5f, FontStyle.Regular);
        //    }
        //}
        private bool IsDesignerHosted {
            get {
                if(DesignMode)
                    return DesignMode;
                if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
        #endregion
        protected override void OnSelecting(TabControlCancelEventArgs e) {
            //base.OnSelecting(e);
            //if(animator != null) {

            //    animator.BeginUpdateSync(this, false, null, new Rectangle(0, ItemSize.Height + 3, Width, Height - ItemSize.Height - 3));
            //    BeginInvoke(new MethodInvoker(() => animator.EndUpdate(this)));
            //}
            if(this.IsDesignerHosted) {
                base.OnSelecting(e);
                return;
            }
            MethodInvoker method = null;
            try {
                base.OnSelecting(e);
                this.animator.BeginUpdateSync(this, false, null, new Rectangle(0, base.ItemSize.Height + 3, base.Width, (base.Height - base.ItemSize.Height) - 3));
                if(method == null) {
                    method = () => this.animator.EndUpdate(this);
                }
                base.BeginInvoke(method);
            }
            catch(Exception exception) {
                //MessageBox.Show(exception.Message);
            }
        }
    }
}
