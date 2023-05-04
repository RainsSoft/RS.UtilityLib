namespace BetterListView
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
            ComponentOwl.BetterListView.BetterListViewItem betterListViewItem1;
            ComponentOwl.BetterListView.BetterListViewItem betterListViewItem2;
            this.betterListView1 = new ComponentOwl.BetterListView.BetterListView();
            this.betterListViewColumnHeader1 = new ComponentOwl.BetterListView.BetterListViewColumnHeader();
            this.betterListViewColumnHeader2 = new ComponentOwl.BetterListView.BetterListViewColumnHeader();
            betterListViewItem1 = new ComponentOwl.BetterListView.BetterListViewItem();
            betterListViewItem2 = new ComponentOwl.BetterListView.BetterListViewItem();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // betterListView1
            // 
            this.betterListView1.Columns.AddRange(new object[] {
            this.betterListViewColumnHeader1,
            this.betterListViewColumnHeader2});
            this.betterListView1.Items.AddRange(new object[] {
            betterListViewItem1,
            betterListViewItem2});
            this.betterListView1.Location = new System.Drawing.Point(66, 57);
            this.betterListView1.Name = "betterListView1";
            this.betterListView1.Size = new System.Drawing.Size(256, 256);
            this.betterListView1.TabIndex = 0;
            // 
            // betterListViewColumnHeader1
            // 
            this.betterListViewColumnHeader1.Name = "betterListViewColumnHeader1";
            this.betterListViewColumnHeader1.Text = "betterListViewColumnHeader1";
            // 
            // betterListViewColumnHeader2
            // 
            this.betterListViewColumnHeader2.Name = "betterListViewColumnHeader2";
            this.betterListViewColumnHeader2.Text = "betterListViewColumnHeader2";
            // 
            // betterListViewItem1
            // 
            betterListViewItem1.Name = "betterListViewItem1";
            betterListViewItem1.Text = "1";
            // 
            // betterListViewItem2
            // 
            betterListViewItem2.Name = "betterListViewItem2";
            betterListViewItem2.Text = "2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.betterListView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentOwl.BetterListView.BetterListView betterListView1;
        private ComponentOwl.BetterListView.BetterListViewColumnHeader betterListViewColumnHeader1;
        private ComponentOwl.BetterListView.BetterListViewColumnHeader betterListViewColumnHeader2;
    }
}