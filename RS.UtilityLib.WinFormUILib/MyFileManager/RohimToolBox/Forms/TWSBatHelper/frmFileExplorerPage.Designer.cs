
namespace RohimToolBox.Forms.TWSBatHelper
{
    partial class frmFileExplorerPage
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlDummy1 = new System.Windows.Forms.Panel();
            this.lvwFiles = new System.Windows.Forms.ListView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtWorkingPath = new System.Windows.Forms.TextBox();
            this.btnNavUpper = new System.Windows.Forms.Button();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.cmsSearchText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiUseRegex = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCaseInsensitive = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.cmsSearchText.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 393);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.statusStrip1.Size = new System.Drawing.Size(841, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.pnlDummy1);
            this.splitContainer1.Panel2.Controls.Add(this.lvwFiles);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(841, 393);
            this.splitContainer1.SplitterDistance = 185;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // pnlDummy1
            // 
            this.pnlDummy1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDummy1.Location = new System.Drawing.Point(0, 21);
            this.pnlDummy1.Name = "pnlDummy1";
            this.pnlDummy1.Size = new System.Drawing.Size(653, 3);
            this.pnlDummy1.TabIndex = 4;
            // 
            // lvwFiles
            // 
            this.lvwFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFiles.HideSelection = false;
            this.lvwFiles.Location = new System.Drawing.Point(0, 21);
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.Size = new System.Drawing.Size(653, 372);
            this.lvwFiles.TabIndex = 3;
            this.lvwFiles.UseCompatibleStateImageBehavior = false;
            this.lvwFiles.ItemActivate += new System.EventHandler(this.lvwFiles_ItemActivate);
            this.lvwFiles.SelectedIndexChanged += new System.EventHandler(this.lvwFiles_SelectedIndexChanged);
            this.lvwFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwFiles_KeyDown);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel1.Controls.Add(this.txtWorkingPath);
            this.splitContainer2.Panel1.Controls.Add(this.btnNavUpper);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.txtSearchText);
            this.splitContainer2.Size = new System.Drawing.Size(653, 21);
            this.splitContainer2.SplitterDistance = 401;
            this.splitContainer2.TabIndex = 2;
            // 
            // txtWorkingPath
            // 
            this.txtWorkingPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWorkingPath.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWorkingPath.Location = new System.Drawing.Point(33, 0);
            this.txtWorkingPath.Margin = new System.Windows.Forms.Padding(1);
            this.txtWorkingPath.Name = "txtWorkingPath";
            this.txtWorkingPath.Size = new System.Drawing.Size(368, 23);
            this.txtWorkingPath.TabIndex = 0;
            // 
            // btnNavUpper
            // 
            this.btnNavUpper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavUpper.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnNavUpper.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavUpper.Location = new System.Drawing.Point(0, 0);
            this.btnNavUpper.Margin = new System.Windows.Forms.Padding(1);
            this.btnNavUpper.Name = "btnNavUpper";
            this.btnNavUpper.Size = new System.Drawing.Size(33, 21);
            this.btnNavUpper.TabIndex = 1;
            this.btnNavUpper.Text = "↑";
            this.btnNavUpper.UseVisualStyleBackColor = true;
            this.btnNavUpper.Click += new System.EventHandler(this.btnNavUpper_Click);
            // 
            // txtSearchText
            // 
            this.txtSearchText.ContextMenuStrip = this.cmsSearchText;
            this.txtSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchText.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchText.ForeColor = System.Drawing.Color.DarkGray;
            this.txtSearchText.Location = new System.Drawing.Point(0, 0);
            this.txtSearchText.Margin = new System.Windows.Forms.Padding(1);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(248, 23);
            this.txtSearchText.TabIndex = 2;
            this.txtSearchText.Text = "Search...";
            this.txtSearchText.Enter += new System.EventHandler(this.txtSearchText_Enter);
            this.txtSearchText.Leave += new System.EventHandler(this.txtSearchText_Leave);
            // 
            // cmsSearchText
            // 
            this.cmsSearchText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmsSearchText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUseRegex,
            this.tsmiCaseInsensitive});
            this.cmsSearchText.Name = "cmsSearchText";
            this.cmsSearchText.Size = new System.Drawing.Size(187, 48);
            this.cmsSearchText.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.cmsSearchText_Closing);
            // 
            // tsmiUseRegex
            // 
            this.tsmiUseRegex.CheckOnClick = true;
            this.tsmiUseRegex.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsmiUseRegex.Name = "tsmiUseRegex";
            this.tsmiUseRegex.Size = new System.Drawing.Size(186, 22);
            this.tsmiUseRegex.Text = "Use Regex";
            // 
            // tsmiCaseInsensitive
            // 
            this.tsmiCaseInsensitive.Checked = true;
            this.tsmiCaseInsensitive.CheckOnClick = true;
            this.tsmiCaseInsensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiCaseInsensitive.Name = "tsmiCaseInsensitive";
            this.tsmiCaseInsensitive.Size = new System.Drawing.Size(186, 22);
            this.tsmiCaseInsensitive.Text = "Case Insensitive";
            // 
            // frmFileExplorerPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 415);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "frmFileExplorerPage";
            this.Text = "frmTWSBatHelperPage";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.cmsSearchText.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtWorkingPath;
        private System.Windows.Forms.Button btnNavUpper;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TextBox txtSearchText;
    private System.Windows.Forms.ContextMenuStrip cmsSearchText;
    private System.Windows.Forms.ToolStripMenuItem tsmiUseRegex;
    private System.Windows.Forms.ToolStripMenuItem tsmiCaseInsensitive;
    private System.Windows.Forms.ListView lvwFiles;
    private System.Windows.Forms.Panel pnlDummy1;
	private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
  }
}