
namespace RohimToolBox.Forms.TWSBatHelper
{
    partial class frmFileExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
	  this.tabControl1 = new System.Windows.Forms.TabControl();
	  this.tabPage1 = new System.Windows.Forms.TabPage();
	  this.panel1 = new System.Windows.Forms.Panel();
	  this.btnHelp = new RohimToolBox.CustControls.RoundButton();
	  this.tabControl1.SuspendLayout();
	  this.panel1.SuspendLayout();
	  this.SuspendLayout();
	  // 
	  // tabControl1
	  // 
	  this.tabControl1.Controls.Add(this.tabPage1);
	  this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
	  this.tabControl1.Location = new System.Drawing.Point(0, 25);
	  this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
	  this.tabControl1.Name = "tabControl1";
	  this.tabControl1.SelectedIndex = 0;
	  this.tabControl1.Size = new System.Drawing.Size(849, 451);
	  this.tabControl1.TabIndex = 3;
	  // 
	  // tabPage1
	  // 
	  this.tabPage1.Location = new System.Drawing.Point(4, 22);
	  this.tabPage1.Margin = new System.Windows.Forms.Padding(1);
	  this.tabPage1.Name = "tabPage1";
	  this.tabPage1.Padding = new System.Windows.Forms.Padding(1);
	  this.tabPage1.Size = new System.Drawing.Size(841, 425);
	  this.tabPage1.TabIndex = 0;
	  this.tabPage1.Text = "Folder";
	  this.tabPage1.UseVisualStyleBackColor = true;
	  // 
	  // panel1
	  // 
	  this.panel1.BackColor = System.Drawing.Color.OldLace;
	  this.panel1.Controls.Add(this.btnHelp);
	  this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
	  this.panel1.Location = new System.Drawing.Point(0, 0);
	  this.panel1.Name = "panel1";
	  this.panel1.Size = new System.Drawing.Size(849, 25);
	  this.panel1.TabIndex = 4;
	  // 
	  // btnHelp
	  // 
	  this.btnHelp.BackColor = System.Drawing.Color.Wheat;
	  this.btnHelp.Cursor = System.Windows.Forms.Cursors.Hand;
	  this.btnHelp.FlatAppearance.BorderSize = 0;
	  this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
	  this.btnHelp.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
	  this.btnHelp.Location = new System.Drawing.Point(816, 1);
	  this.btnHelp.Name = "btnHelp";
	  this.btnHelp.Size = new System.Drawing.Size(22, 22);
	  this.btnHelp.TabIndex = 0;
	  this.btnHelp.Text = "?";
	  this.btnHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
	  this.btnHelp.UseVisualStyleBackColor = false;
	  this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
	  // 
	  // frmTWSBatHelper
	  // 
	  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
	  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	  this.ClientSize = new System.Drawing.Size(849, 476);
	  this.Controls.Add(this.tabControl1);
	  this.Controls.Add(this.panel1);
	  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
	  this.Margin = new System.Windows.Forms.Padding(1);
	  this.Name = "frmTWSBatHelper";
	  this.Text = "frmTWSBatHelper";
	  this.tabControl1.ResumeLayout(false);
	  this.panel1.ResumeLayout(false);
	  this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
	private System.Windows.Forms.Panel panel1;
	private CustControls.RoundButton btnHelp;
  }
}