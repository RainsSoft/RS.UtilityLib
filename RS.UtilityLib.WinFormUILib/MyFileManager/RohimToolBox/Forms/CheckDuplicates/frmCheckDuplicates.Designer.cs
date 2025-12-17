
namespace RohimToolBox.Forms.CheckDuplicates
{
    partial class frmCheckDuplicates
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
      this.btnDistinct = new System.Windows.Forms.Button();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.txtLeft = new  System.Windows.Forms.RichTextBox();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // btnDistinct
      // 
      this.btnDistinct.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDistinct.Location = new System.Drawing.Point(3, 3);
      this.btnDistinct.Name = "btnDistinct";
      this.btnDistinct.Size = new System.Drawing.Size(82, 25);
      this.btnDistinct.TabIndex = 1;
      this.btnDistinct.Text = "Count";
      this.btnDistinct.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Location = new System.Drawing.Point(3, 33);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.txtLeft);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
      this.splitContainer1.Size = new System.Drawing.Size(834, 486);
      this.splitContainer1.SplitterDistance = 269;
      this.splitContainer1.TabIndex = 2;
      // 
      // txtLeft
      // 
      this.txtLeft.Dock = System.Windows.Forms.DockStyle.Fill;
      //this.txtLeft.HScrollBar = false;
      this.txtLeft.Location = new System.Drawing.Point(0, 0);
      this.txtLeft.Name = "txtLeft";
      this.txtLeft.Size = new System.Drawing.Size(269, 486);
      this.txtLeft.TabIndex = 0;
      this.txtLeft.Text = "aaa\r\nbbb\r\nccc\r\nddd\r\nddd\r\neee\r\neee\r\naaa\r\naaa\r\naaa\r\nfff\r\nggg\r\nhhh\r\niii\r\nddd";
      // 
      // dataGridView1
      // 
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colCount});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 0);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new System.Drawing.Size(561, 486);
      this.dataGridView1.TabIndex = 0;
      // 
      // colName
      // 
      this.colName.HeaderText = "Name";
      this.colName.Name = "colName";
      this.colName.ReadOnly = true;
      // 
      // colCount
      // 
      this.colCount.HeaderText = "Count";
      this.colCount.Name = "colCount";
      this.colCount.ReadOnly = true;
      // 
      // frmCheckDuplicates
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(849, 476);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.btnDistinct);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "frmCheckDuplicates";
      this.Text = "frmCheckDuplicates";
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDistinct;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox txtLeft;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCount;
    }
}