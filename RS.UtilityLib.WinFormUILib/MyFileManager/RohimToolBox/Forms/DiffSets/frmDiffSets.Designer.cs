
namespace RohimToolBox.Forms.DiffSets
{
    partial class frmDiffSets
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
      this.btnDiff = new System.Windows.Forms.Button();
      this.splTextBoxes = new System.Windows.Forms.SplitContainer();
      this.txtLeft = new  System.Windows.Forms.RichTextBox();
      this.txtRight = new  System.Windows.Forms.RichTextBox();
      ((System.ComponentModel.ISupportInitialize)(this.splTextBoxes)).BeginInit();
      this.splTextBoxes.Panel1.SuspendLayout();
      this.splTextBoxes.Panel2.SuspendLayout();
      this.splTextBoxes.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnDiff
      // 
      this.btnDiff.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDiff.Location = new System.Drawing.Point(3, 3);
      this.btnDiff.Name = "btnDiff";
      this.btnDiff.Size = new System.Drawing.Size(82, 25);
      this.btnDiff.TabIndex = 0;
      this.btnDiff.Text = "Diff";
      this.btnDiff.UseVisualStyleBackColor = true;
      this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
      // 
      // splTextBoxes
      // 
      this.splTextBoxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splTextBoxes.Location = new System.Drawing.Point(3, 33);
      this.splTextBoxes.Name = "splTextBoxes";
      // 
      // splTextBoxes.Panel1
      // 
      this.splTextBoxes.Panel1.Controls.Add(this.txtLeft);
      // 
      // splTextBoxes.Panel2
      // 
      this.splTextBoxes.Panel2.Controls.Add(this.txtRight);
      this.splTextBoxes.Size = new System.Drawing.Size(843, 442);
      this.splTextBoxes.SplitterDistance = 419;
      this.splTextBoxes.TabIndex = 1;
      // 
      // txtLeft
      // 
      this.txtLeft.Dock = System.Windows.Forms.DockStyle.Fill;
      //this.txtLeft.HScrollBar = false;
      this.txtLeft.Location = new System.Drawing.Point(0, 0);
      this.txtLeft.Name = "txtLeft";
      //this.txtLeft.PasteConvertEndings = false;
      this.txtLeft.Size = new System.Drawing.Size(419, 442);
      this.txtLeft.TabIndex = 0;
      this.txtLeft.Text = "aaa\r\nbbb\r\nccc\r\nddd\r\nddd\r\neee";
      //this.txtLeft.ViewWhitespace = ScintillaNET.WhitespaceMode.VisibleAlways;
      // 
      // txtRight
      // 
      this.txtRight.Dock = System.Windows.Forms.DockStyle.Fill;
      //this.txtRight.HScrollBar = false;
      this.txtRight.Location = new System.Drawing.Point(0, 0);
      this.txtRight.Name = "txtRight";
      //this.txtRight.PasteConvertEndings = false;
      this.txtRight.Size = new System.Drawing.Size(420, 442);
      this.txtRight.TabIndex = 0;
      this.txtRight.Text = "ccc\r\nggg\r\nddd\r\nfff";
      //this.txtRight.ViewWhitespace = ScintillaNET.WhitespaceMode.VisibleAlways;
      // 
      // frmDiffSets
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(849, 476);
      this.Controls.Add(this.splTextBoxes);
      this.Controls.Add(this.btnDiff);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "frmDiffSets";
      this.Text = "frmDiffSets";
      this.splTextBoxes.Panel1.ResumeLayout(false);
      this.splTextBoxes.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splTextBoxes)).EndInit();
      this.splTextBoxes.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDiff;
        private System.Windows.Forms.SplitContainer splTextBoxes;
        private System.Windows.Forms.RichTextBox txtLeft;
        private System.Windows.Forms.RichTextBox txtRight;
    }
}