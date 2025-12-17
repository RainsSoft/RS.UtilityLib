
namespace RohimToolBox {
  partial class frmMain {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlSide = new System.Windows.Forms.Panel();
            this.btnFileExplorer = new System.Windows.Forms.Button();
            this.btnCheckDuplicates = new System.Windows.Forms.Button();
            this.btnDiffSets = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.pnlSide.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSide
            // 
            this.pnlSide.Controls.Add(this.button1);
            this.pnlSide.Controls.Add(this.btnFileExplorer);
            this.pnlSide.Controls.Add(this.btnCheckDuplicates);
            this.pnlSide.Controls.Add(this.btnDiffSets);
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 0);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(135, 439);
            this.pnlSide.TabIndex = 1;
            // 
            // btnFileExplorer
            // 
            this.btnFileExplorer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnFileExplorer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileExplorer.FlatAppearance.BorderSize = 0;
            this.btnFileExplorer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileExplorer.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileExplorer.Location = new System.Drawing.Point(0, 3);
            this.btnFileExplorer.Name = "btnFileExplorer";
            this.btnFileExplorer.Size = new System.Drawing.Size(135, 23);
            this.btnFileExplorer.TabIndex = 0;
            this.btnFileExplorer.Text = "FileExplorer";
            this.btnFileExplorer.UseVisualStyleBackColor = false;
            this.btnFileExplorer.Click += new System.EventHandler(this.btnTWSBatHelper_Click);
            // 
            // btnCheckDuplicates
            // 
            this.btnCheckDuplicates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCheckDuplicates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckDuplicates.FlatAppearance.BorderSize = 0;
            this.btnCheckDuplicates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckDuplicates.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckDuplicates.Location = new System.Drawing.Point(0, 31);
            this.btnCheckDuplicates.Name = "btnCheckDuplicates";
            this.btnCheckDuplicates.Size = new System.Drawing.Size(135, 23);
            this.btnCheckDuplicates.TabIndex = 0;
            this.btnCheckDuplicates.Text = "CheckDuplicates";
            this.btnCheckDuplicates.UseVisualStyleBackColor = false;
            this.btnCheckDuplicates.Click += new System.EventHandler(this.btnCheckDuplicates_Click);
            // 
            // btnDiffSets
            // 
            this.btnDiffSets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDiffSets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiffSets.FlatAppearance.BorderSize = 0;
            this.btnDiffSets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDiffSets.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDiffSets.Location = new System.Drawing.Point(0, 60);
            this.btnDiffSets.Name = "btnDiffSets";
            this.btnDiffSets.Size = new System.Drawing.Size(135, 23);
            this.btnDiffSets.TabIndex = 0;
            this.btnDiffSets.Text = "DiffSets";
            this.btnDiffSets.UseVisualStyleBackColor = false;
            this.btnDiffSets.Click += new System.EventHandler(this.btnDiffSets_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(135, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.pnlMain.Size = new System.Drawing.Size(849, 439);
            this.pnlMain.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 439);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSide);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "frmMain";
            this.Text = "RohimToolBox";
            this.pnlSide.ResumeLayout(false);
            this.ResumeLayout(false);

    }

        #endregion
        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnDiffSets;
        private System.Windows.Forms.Button btnCheckDuplicates;
        private System.Windows.Forms.Button btnFileExplorer;
        private System.Windows.Forms.Button button1;
    }
}

