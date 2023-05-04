using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace RS.UtilityLib.WinFormCommon.UI
{
    public class MouseThroughForm : Form
    {

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);

        const int GWL_EXSTYLE = -20;

        const int WS_EX_TRANSPARENT = 0x20;

        const int WS_EX_LAYERED = 0x80000;
        private PictureBox pb_Halo;
  
        private Panel panel1;

        const int LWA_ALPHA = 2;
        public MouseThroughForm() {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e) {
            this.TopLevel = true;
            //SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            //SetLayeredWindowAttributes(Handle, 0, 255, LWA_ALPHA);

        }
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
            this.pb_Halo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Halo)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_Halo
            // 
            this.pb_Halo.BackColor = System.Drawing.Color.Transparent;
            this.pb_Halo.Location = new System.Drawing.Point(0, 0);
            this.pb_Halo.Name = "pb_Halo";
            this.pb_Halo.Size = new System.Drawing.Size(199, 221);
            this.pb_Halo.TabIndex = 0;
            this.pb_Halo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 247);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(772, 122);
            this.panel1.TabIndex = 2;
            // 
            // MouseThroughForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(950, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pb_Halo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MouseThroughForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MouseThroughForm";
            ((System.ComponentModel.ISupportInitialize)(this.pb_Halo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
