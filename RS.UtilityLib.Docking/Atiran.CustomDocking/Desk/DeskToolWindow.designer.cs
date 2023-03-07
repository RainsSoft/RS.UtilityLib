namespace Atiran.CustomDocking.Desk {
    partial class DeskToolWindow {
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
            this.SuspendLayout();
            // 
            // DeskToolWindow
            // 
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.DockAreas = ((Atiran.CustomDocking.Docking.DockAreas)(((((Atiran.CustomDocking.Docking.DockAreas.Float | Atiran.CustomDocking.Docking.DockAreas.DockLeft) 
            | Atiran.CustomDocking.Docking.DockAreas.DockRight) 
            | Atiran.CustomDocking.Docking.DockAreas.DockTop) 
            | Atiran.CustomDocking.Docking.DockAreas.DockBottom)));
            this.Name = "DeskToolWindow";
            this.TabText = "DeskToolWindow";
            this.Text = "DeskToolWindow";
            this.ResumeLayout(false);

        }

        #endregion
    }
}