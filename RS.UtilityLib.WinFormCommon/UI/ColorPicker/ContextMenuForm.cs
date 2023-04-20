using System;
using System.Drawing;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.ColorPicker
{
    /// <summary>
    /// Provides a System.Windows.Forms.Form that have a ContextMenu behavior.
    /// Use this Form by extending it or by adding the control using the method:
    /// <code>SetContainingControl(Control control)</code>
    /// </summary>
    public partial class ContextMenuForm : Form
    {
        protected bool _locked = false;

        /// <summary>
        /// Gets or sets a value indicating that the form is locked.
        /// The form should be locked when opening a Dialog on it.
        /// 如果锁定，失去焦点不会隐藏
        /// </summary>
        public bool Locked {
            get { return _locked; }
            set {
                _locked = value;
            }
        }

        protected Control _parentControl = null;

        /// <summary>
        /// Initialize a new instace of the ContextMenuForm in order to hold a control that
        /// needes to have a ContextMenu behavior.
        /// </summary>
        public ContextMenuForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the form on the specifies parent in the specifies location.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="startLocation"></param>
        /// <param name="width"></param>
        public void Show(Control parent, Point startLocation, int width) {
            _parentControl = parent;
            Point location = parent.PointToScreen(startLocation);
            this.Location = location;
            this.Width = width;
            this.Show();
        }
        /// <summary>
        /// Set the control that will populate the ContextMenuForm.
        /// 添加控件并填充
        /// <remarks>
        /// Any scrolling should be implemented in the control it self, the 
        /// ContextMenuForm will not support scrolling.
        /// </remarks>
        /// </summary>
        /// <param name="control"></param>
        public void SetContainingControl(Control control) {
            this.Controls.Clear();
            control.Dock = DockStyle.Fill;
            this.Controls.Add(control);
        }

        private void ContextMenuPanel_Deactivate(object sender, EventArgs e) {
            if (!Locked) {
                Hide();
            }
        }

        private void ContextMenuPanel_Leave(object sender, EventArgs e) {
            if (!Locked) {
                Hide();
            }
        }

        new public void Hide() {
            base.Hide();
        }
    }

    partial class ContextMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContextMenuForm));
            this.SuspendLayout();
            // 
            // ContextMenuForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;            
            this.Name = "ContextMenuForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ContextMenuPanel";
            this.Deactivate += new System.EventHandler(this.ContextMenuPanel_Deactivate);
            this.Leave += new System.EventHandler(this.ContextMenuPanel_Leave);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
