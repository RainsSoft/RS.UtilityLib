
namespace BetterListViewDemo
{
    partial class Form1
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
            this.betterListView1 = new ComponentOwl.BetterListView.BetterListView();
            this.betterListView2 = new ComponentOwl.BetterListView.BetterListView();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView2)).BeginInit();
            this.SuspendLayout();
            // 
            // betterListView1
            // 
            this.betterListView1.Location = new System.Drawing.Point(99, 70);
            this.betterListView1.Name = "betterListView1";
            this.betterListView1.Size = new System.Drawing.Size(256, 256);
            this.betterListView1.TabIndex = 0;
            // 
            // betterListView2
            // 
            this.betterListView2.Location = new System.Drawing.Point(497, 58);
            this.betterListView2.Name = "betterListView2";
            this.betterListView2.Size = new System.Drawing.Size(256, 256);
            this.betterListView2.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.betterListView2);
            this.Controls.Add(this.betterListView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentOwl.BetterListView.BetterListView betterListView1;
        private ComponentOwl.BetterListView.BetterListView betterListView2;
    }
}