
namespace RS.UtilityLib.WinFormCommon
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
            this.button1 = new System.Windows.Forms.Button();
            this.lineEditor1 = new RS.UtilityLib.WinFormCommon.UI.LineEditor();
            this.degreePiePicture1 = new RS.UtilityLib.WinFormCommon.UI.DegreePiePicture();
            this.ribbonTreeView1 = new RS.UtilityLib.WinFormCommon.RibbonUI.RibbonTreeView();
            this.ribbonSysListView1 = new RS.UtilityLib.WinFormCommon.RibbonUI.RibbonSysListView();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(565, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lineEditor1
            // 
            this.lineEditor1.BottomAligned = true;
            this.lineEditor1.Editable = false;
            this.lineEditor1.Location = new System.Drawing.Point(520, 305);
            this.lineEditor1.Name = "lineEditor1";
            this.lineEditor1.Selectable = true;
            this.lineEditor1.SelectedIndex = -1;
            this.lineEditor1.SelectedLine = null;
            this.lineEditor1.ShowSelection = true;
            this.lineEditor1.Size = new System.Drawing.Size(100, 50);
            this.lineEditor1.TabIndex = 3;
            this.lineEditor1.YScroll = 0;
            // 
            // degreePiePicture1
            // 
            this.degreePiePicture1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.degreePiePicture1.Degree = 45F;
            this.degreePiePicture1.DegreeColor = System.Drawing.Color.Maroon;
            this.degreePiePicture1.DegreeFont = new System.Drawing.Font("宋体", 10F);
            this.degreePiePicture1.DegreeTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.degreePiePicture1.DegreeTextAlignVertical = RS.UtilityLib.WinFormCommon.UI.DegreePiePicture.VerticalAlignment.Center;
            this.degreePiePicture1.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.degreePiePicture1.FrontImage = global::RS.UtilityLib.WinFormCommon.Properties.Resources.chilun_1;
            this.degreePiePicture1.Location = new System.Drawing.Point(520, 104);
            this.degreePiePicture1.Name = "degreePiePicture1";
            this.degreePiePicture1.ShowPie = true;
            this.degreePiePicture1.ShowText = true;
            this.degreePiePicture1.Size = new System.Drawing.Size(150, 150);
            this.degreePiePicture1.TabIndex = 2;
            // 
            // ribbonTreeView1
            // 
            this.ribbonTreeView1.Location = new System.Drawing.Point(255, 50);
            this.ribbonTreeView1.Name = "ribbonTreeView1";
            this.ribbonTreeView1.Size = new System.Drawing.Size(192, 256);
            this.ribbonTreeView1.TabIndex = 1;
            // 
            // ribbonSysListView1
            // 
            this.ribbonSysListView1.HideSelection = false;
            this.ribbonSysListView1.Location = new System.Drawing.Point(26, 50);
            this.ribbonSysListView1.Name = "ribbonSysListView1";
            this.ribbonSysListView1.Size = new System.Drawing.Size(207, 249);
            this.ribbonSysListView1.TabIndex = 0;
            this.ribbonSysListView1.UseCompatibleStateImageBehavior = false;
            this.ribbonSysListView1.View = System.Windows.Forms.View.Details;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(255, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lineEditor1);
            this.Controls.Add(this.degreePiePicture1);
            this.Controls.Add(this.ribbonTreeView1);
            this.Controls.Add(this.ribbonSysListView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private RibbonUI.RibbonSysListView ribbonSysListView1;
        private RibbonUI.RibbonTreeView ribbonTreeView1;
        private UI.DegreePiePicture degreePiePicture1;
        private UI.LineEditor lineEditor1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}