using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.MyScrollBar
{
    class CustomScrollBarsSample : Form
    {
        public CustomScrollBarsSample() {
            InitializeComponent();
        }

        private void AdjustScrollbars() {
            //AdjustScrollbar(vMyScrollBar, fctb.VerticalScroll.Maximum, fctb.VerticalScroll.Value, fctb.ClientSize.Height);
            //AdjustScrollbar(hMyScrollBar, fctb.HorizontalScroll.Maximum, fctb.HorizontalScroll.Value, fctb.ClientSize.Width);

            //AdjustScrollbar(vScrollBar, fctb.VerticalScroll.Maximum, fctb.VerticalScroll.Value, fctb.ClientSize.Height);
            //AdjustScrollbar(hScrollBar, fctb.HorizontalScroll.Maximum, fctb.HorizontalScroll.Value, fctb.ClientSize.Width);
        }

        /// <summary>
        /// This method for MyScrollBar
        /// </summary>
        private void AdjustScrollbar(ScrollBarSimple scrollBar, int max, int value, int clientSize) {
            scrollBar.Maximum = max;
            scrollBar.Visible = max > 0;
            scrollBar.Value = Math.Min(scrollBar.Maximum, value);
        }

        /// <summary>
        /// This method for System.Windows.Forms.ScrollBar and inherited classes
        /// </summary>
        private void AdjustScrollbar(ScrollBar scrollBar, int max, int value, int clientSize) {
            scrollBar.LargeChange = clientSize / 3;
            scrollBar.SmallChange = clientSize / 11;
            scrollBar.Maximum = max + scrollBar.LargeChange;
            scrollBar.Visible = max > 0;
            scrollBar.Value = Math.Min(scrollBar.Maximum, value);
        }

        private void fctb_ScrollbarsUpdated(object sender, EventArgs e) {
            AdjustScrollbars();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e) {
            //fctb.OnScroll(e, e.Type != ScrollEventType.ThumbTrack && e.Type != ScrollEventType.ThumbPosition);
        }

        private RichTextBox richTextBox1;


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
            this.label2 = new System.Windows.Forms.Label();
            this.sysVScrollBar = new System.Windows.Forms.VScrollBar();
            this.sysHScrollBar = new System.Windows.Forms.HScrollBar();
            this.hMyScrollBar = new RS.UtilityLib.WinFormCommon.UI.MyScrollBar.ScrollBarSimple();
            this.vMyScrollBar = new RS.UtilityLib.WinFormCommon.UI.MyScrollBar.ScrollBarSimple();
            this.fctb = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1002, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "Here we create custom outside scrollbars for FCTB.";
            // 
            // sysVScrollBar
            // 
            this.sysVScrollBar.Location = new System.Drawing.Point(411, 59);
            this.sysVScrollBar.Name = "sysVScrollBar";
            this.sysVScrollBar.Size = new System.Drawing.Size(17, 222);
            this.sysVScrollBar.TabIndex = 8;
            this.sysVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
            // 
            // sysHScrollBar
            // 
            this.sysHScrollBar.Location = new System.Drawing.Point(41, 312);
            this.sysHScrollBar.Name = "sysHScrollBar";
            this.sysHScrollBar.Size = new System.Drawing.Size(387, 17);
            this.sysHScrollBar.TabIndex = 9;
            this.sysHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
            // 
            // hMyScrollBar
            // 
            this.hMyScrollBar.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(71)))), ((int)(((byte)(38)))));
            this.hMyScrollBar.BackColor = System.Drawing.Color.Gold;
            this.hMyScrollBar.BorderColor = System.Drawing.Color.Gray;
            this.hMyScrollBar.InactiveColor = System.Drawing.SystemColors.ScrollBar;
            this.hMyScrollBar.Location = new System.Drawing.Point(41, 285);
            this.hMyScrollBar.Maximum = 100;
            this.hMyScrollBar.Name = "hMyScrollBar";
            this.hMyScrollBar.Orientation = System.Windows.Forms.ScrollOrientation.HorizontalScroll;
            this.hMyScrollBar.Size = new System.Drawing.Size(326, 13);
            this.hMyScrollBar.TabIndex = 7;
            this.hMyScrollBar.Text = "0%100";
            this.hMyScrollBar.ThumbColor = System.Drawing.Color.Red;
            this.hMyScrollBar.ThumbSize = 10;
            this.hMyScrollBar.Value = 0;
            this.hMyScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
            // 
            // vMyScrollBar
            // 
            this.vMyScrollBar.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(71)))), ((int)(((byte)(38)))));
            this.vMyScrollBar.BackColor = System.Drawing.Color.Gold;
            this.vMyScrollBar.BorderColor = System.Drawing.Color.Gray;
            this.vMyScrollBar.InactiveColor = System.Drawing.SystemColors.ScrollBar;
            this.vMyScrollBar.Location = new System.Drawing.Point(384, 59);
            this.vMyScrollBar.Maximum = 100;
            this.vMyScrollBar.Name = "vMyScrollBar";
            this.vMyScrollBar.Orientation = System.Windows.Forms.ScrollOrientation.VerticalScroll;
            this.vMyScrollBar.Size = new System.Drawing.Size(14, 220);
            this.vMyScrollBar.TabIndex = 6;
            this.vMyScrollBar.Text = "0\n%\n100";
            this.vMyScrollBar.ThumbColor = System.Drawing.Color.Red;
            this.vMyScrollBar.ThumbSize = 10;
            this.vMyScrollBar.Value = 0;
            this.vMyScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
            // 
            // fctb
            // 
            this.fctb.BackColor = System.Drawing.Color.Gold;
            this.fctb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.fctb.Location = new System.Drawing.Point(41, 57);
            this.fctb.Name = "fctb";
            this.fctb.Size = new System.Drawing.Size(326, 222);
            this.fctb.TabIndex = 3;
            this.fctb.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(498, 59);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(257, 248);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // CustomScrollBarsSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 427);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.sysHScrollBar);
            this.Controls.Add(this.sysVScrollBar);
            this.Controls.Add(this.hMyScrollBar);
            this.Controls.Add(this.vMyScrollBar);
            this.Controls.Add(this.fctb);
            this.Controls.Add(this.label2);
            this.Name = "CustomScrollBarsSample";
            this.Text = "CustomScrollBarsSample";
            this.Load += new System.EventHandler(this.CustomScrollBarsSample_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private RichTextBox fctb;
        private ScrollBarSimple vMyScrollBar;
        private ScrollBarSimple hMyScrollBar;
        private System.Windows.Forms.VScrollBar sysVScrollBar;
        private System.Windows.Forms.HScrollBar sysHScrollBar;

        private void CustomScrollBarsSample_Load(object sender, EventArgs e) {
            ScrollBarEx.Attach(this.richTextBox1);
        }
    }
}
