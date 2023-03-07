namespace Atiran.CustomDocking.Desk {
    partial class DeskMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
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
            this.components = new System.ComponentModel.Container();
            this.vsToolStripExtender1 = new Atiran.CustomDocking.Docking.VisualStudioToolStripExtender(this.components);
            this.SuspendLayout();
            // 
            // vsToolStripExtender1
            // 
            this.vsToolStripExtender1.DefaultRenderer = null;
            // 
            // DeskMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.IsMdiContainer = true;
            this.Name = "DeskMain";
            this.Text = "DeskMain";
            this.ResumeLayout(false);

        }

        #endregion

        protected Docking.VisualStudioToolStripExtender vsToolStripExtender1;
    }
}