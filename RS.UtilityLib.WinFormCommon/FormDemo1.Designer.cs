


namespace RS.UtilityLib.WinFormCommon
{
    partial class FormDemo1
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
            this.button2 = new System.Windows.Forms.Button();
            this.lV_ModelList = new RS.UtilityLib.WinFormCommon.RibbonUI.RibbonListView();
            this.lineEditor1 = new RS.UtilityLib.WinFormCommon.UI.LineEditor();
            this.degreePiePicture1 = new RS.UtilityLib.WinFormCommon.UI.DegreePiePicture();
            this.ribbonTreeView1 = new RS.UtilityLib.WinFormCommon.RibbonUI.RibbonTreeView();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button3 = new System.Windows.Forms.Button();
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
            // lV_ModelList
            // 
            this.lV_ModelList.BackgroundImage = global::RS.UtilityLib.WinFormCommon.Properties.Resources.listback3;
            this.lV_ModelList.ColorEnd = System.Drawing.Color.Transparent;
            this.lV_ModelList.ColorFactor = 0.4F;
            this.lV_ModelList.ColorLinearAngle = 90F;
            this.lV_ModelList.ColorStart = System.Drawing.SystemColors.Control;
            this.lV_ModelList.ColorUserSet = false;
            this.lV_ModelList.ColumnCount = 4;
            this.lV_ModelList.ColumnSp = 4;
            this.lV_ModelList.CurPosition = new System.Drawing.Point(0, 0);
            this.lV_ModelList.DockFlag = 0;
            this.lV_ModelList.FixHintPostion = true;
            this.lV_ModelList.Hint = null;
            this.lV_ModelList.HintType = RS.UtilityLib.WinFormCommon.RibbonUI.RibbonListView.ShowHintType.OnlyEnableItem;
            this.lV_ModelList.HoverItem = null;
            this.lV_ModelList.ImageLayout = 0;
            this.lV_ModelList.ImageList = null;
            this.lV_ModelList.ImagePath = null;
            this.lV_ModelList.ImageShowFormat = System.Windows.Forms.ImageLayout.Zoom;
            this.lV_ModelList.ImageSize = new System.Drawing.Point(48, 48);
            this.lV_ModelList.IsDrawBoard = false;
            this.lV_ModelList.IsShowHover = true;
            this.lV_ModelList.IsShowItemText = false;
            this.lV_ModelList.IsShowToolTip = false;
            this.lV_ModelList.IsToLagerHoverItem = true;
            this.lV_ModelList.ItemAutoSize = false;
            this.lV_ModelList.ItemBoard = null;
            this.lV_ModelList.LevelTab = 12;
            this.lV_ModelList.Location = new System.Drawing.Point(12, 12);
            this.lV_ModelList.MulitSelect = false;
            this.lV_ModelList.Name = "lV_ModelList";
            this.lV_ModelList.RectDisable = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.lV_ModelList.RectDown = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.lV_ModelList.RectHover = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.lV_ModelList.RectNormal = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.lV_ModelList.RibbonImageLayout = RS.UtilityLib.WinFormCommon.RibbonUI.RibbonImageLayout.Table;
            this.lV_ModelList.RowHeight = 48;
            this.lV_ModelList.RowSp = 4;
            this.lV_ModelList.SelectedMask = null;
            this.lV_ModelList.SetFlag = 0;
            this.lV_ModelList.Size = new System.Drawing.Size(232, 336);
            this.lV_ModelList.TabIndex = 4;
            this.lV_ModelList.ZOrder = 0;
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
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(255, 357);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(192, 137);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(255, 325);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FormDemo1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 542);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.lV_ModelList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lineEditor1);
            this.Controls.Add(this.degreePiePicture1);
            this.Controls.Add(this.ribbonTreeView1);
            this.Name = "FormDemo1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private RibbonUI.RibbonTreeView ribbonTreeView1;
        private UI.DegreePiePicture degreePiePicture1;
        private UI.LineEditor lineEditor1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private RibbonUI.RibbonListView lV_ModelList;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button3;
    }
}