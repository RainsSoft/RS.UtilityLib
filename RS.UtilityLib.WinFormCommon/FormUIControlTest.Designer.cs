
namespace RS.UtilityLib.WinFormCommon
{
    partial class FormUIControlTest
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "11",
            "22"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "33",
            "44"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "11",
            "11-1",
            "56"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "22",
            "22-2",
            "fsdfsfs"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "11",
            "22"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "33",
            "44"}, -1);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewEx21 = new RS.UtilityLib.WinFormCommon.UI.ListViewExp();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewEx1 = new RS.UtilityLib.WinFormCommon.UI.ListViewEx();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxNumEx1 = new RS.UtilityLib.WinFormCommon.UI.TextBoxNumEx();
            this.textBoxEx2 = new RS.UtilityLib.WinFormCommon.UI.TextBoxEx();
            this.textBoxEx1 = new RS.UtilityLib.WinFormCommon.UI.TextBoxEx();
            this.waterMarkTextBox2 = new RS.UtilityLib.WinFormCommon.UI.WaterMarkTextBox();
            this.waterMarkTextBox1 = new RS.UtilityLib.WinFormCommon.UI.WaterMarkTextBox();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.listView1.Location = new System.Drawing.Point(48, 140);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(161, 85);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // listViewEx21
            // 
            this.listViewEx21.AllowColumnReorder = true;
            this.listViewEx21.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listViewEx21.DoubleClickActivation = false;
            this.listViewEx21.FullRowSelect = true;
            this.listViewEx21.HideSelection = false;
            this.listViewEx21.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.listViewEx21.Location = new System.Drawing.Point(48, 348);
            this.listViewEx21.Name = "listViewEx21";
            this.listViewEx21.OwnerDraw = true;
            this.listViewEx21.ProgressColor = System.Drawing.Color.Red;
            this.listViewEx21.ProgressTextColor = System.Drawing.Color.Black;
            this.listViewEx21.Size = new System.Drawing.Size(223, 97);
            this.listViewEx21.TabIndex = 7;
            this.listViewEx21.UseCompatibleStateImageBehavior = false;
            this.listViewEx21.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "index";
            this.columnHeader5.Width = 70;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "link";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "progress";
            // 
            // listViewEx1
            // 
            this.listViewEx1.AllowColumnReorder = true;
            this.listViewEx1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listViewEx1.DoubleClickActivation = false;
            this.listViewEx1.FullRowSelect = true;
            this.listViewEx1.HideSelection = false;
            this.listViewEx1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem5,
            listViewItem6});
            this.listViewEx1.Location = new System.Drawing.Point(48, 231);
            this.listViewEx1.Name = "listViewEx1";
            this.listViewEx1.Size = new System.Drawing.Size(161, 97);
            this.listViewEx1.TabIndex = 6;
            this.listViewEx1.UseCompatibleStateImageBehavior = false;
            this.listViewEx1.View = System.Windows.Forms.View.Details;
            // 
            // textBoxNumEx1
            // 
            this.textBoxNumEx1.BorderColor = System.Drawing.Color.LightSeaGreen;
            this.textBoxNumEx1.DecimalLength = 1;
            this.textBoxNumEx1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.textBoxNumEx1.Location = new System.Drawing.Point(48, 129);
            this.textBoxNumEx1.Name = "textBoxNumEx1";
            this.textBoxNumEx1.ShowSpaces = true;
            this.textBoxNumEx1.ShowTabs = true;
            this.textBoxNumEx1.Size = new System.Drawing.Size(100, 23);
            this.textBoxNumEx1.SpaceKeyColor = System.Drawing.Color.Red;
            this.textBoxNumEx1.TabIndex = 4;
            this.textBoxNumEx1.Text = "23.0";
            this.textBoxNumEx1.WaterMarkText = "请输入...";
            this.textBoxNumEx1.WaterMarkTextFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic);
            this.textBoxNumEx1.WaterMarkTextOffsetX = 4F;
            this.textBoxNumEx1.WaterMarkTextOffsetY = 4F;
            // 
            // textBoxEx2
            // 
            this.textBoxEx2.BorderColor = System.Drawing.Color.LightSeaGreen;
            this.textBoxEx2.Font = new System.Drawing.Font("宋体", 10.5F);
            this.textBoxEx2.Location = new System.Drawing.Point(184, 86);
            this.textBoxEx2.Name = "textBoxEx2";
            this.textBoxEx2.ShowSpaces = true;
            this.textBoxEx2.ShowTabs = true;
            this.textBoxEx2.Size = new System.Drawing.Size(100, 23);
            this.textBoxEx2.SpaceKeyColor = System.Drawing.Color.Red;
            this.textBoxEx2.TabIndex = 3;
            this.textBoxEx2.Text = " 444 erte ";
            this.textBoxEx2.WaterMarkText = "请输入...";
            this.textBoxEx2.WaterMarkTextFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic);
            this.textBoxEx2.WaterMarkTextOffsetX = 4F;
            this.textBoxEx2.WaterMarkTextOffsetY = 4F;
            // 
            // textBoxEx1
            // 
            this.textBoxEx1.BorderColor = System.Drawing.Color.LightSeaGreen;
            this.textBoxEx1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.textBoxEx1.Location = new System.Drawing.Point(184, 56);
            this.textBoxEx1.Name = "textBoxEx1";
            this.textBoxEx1.ShowSpaces = true;
            this.textBoxEx1.ShowTabs = true;
            this.textBoxEx1.Size = new System.Drawing.Size(100, 23);
            this.textBoxEx1.SpaceKeyColor = System.Drawing.Color.Red;
            this.textBoxEx1.TabIndex = 2;
            this.textBoxEx1.WaterMarkText = "请输入...";
            this.textBoxEx1.WaterMarkTextFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic);
            this.textBoxEx1.WaterMarkTextOffsetX = 4F;
            this.textBoxEx1.WaterMarkTextOffsetY = 4F;
            // 
            // waterMarkTextBox2
            // 
            this.waterMarkTextBox2.Font = new System.Drawing.Font("宋体", 9F);
            this.waterMarkTextBox2.Location = new System.Drawing.Point(48, 84);
            this.waterMarkTextBox2.Name = "waterMarkTextBox2";
            this.waterMarkTextBox2.Size = new System.Drawing.Size(100, 21);
            this.waterMarkTextBox2.TabIndex = 1;
            this.waterMarkTextBox2.Text = " 444 erte ";
            this.waterMarkTextBox2.WaterMarkColor = System.Drawing.Color.Gray;
            this.waterMarkTextBox2.WaterMarkText = "Water Mark";
            // 
            // waterMarkTextBox1
            // 
            this.waterMarkTextBox1.Font = new System.Drawing.Font("宋体", 9F);
            this.waterMarkTextBox1.Location = new System.Drawing.Point(48, 56);
            this.waterMarkTextBox1.Name = "waterMarkTextBox1";
            this.waterMarkTextBox1.Size = new System.Drawing.Size(100, 21);
            this.waterMarkTextBox1.TabIndex = 0;
            this.waterMarkTextBox1.WaterMarkColor = System.Drawing.Color.Gray;
            this.waterMarkTextBox1.WaterMarkText = "Water Mark";
            // 
            // FormUIControlTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listViewEx21);
            this.Controls.Add(this.listViewEx1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBoxNumEx1);
            this.Controls.Add(this.textBoxEx2);
            this.Controls.Add(this.textBoxEx1);
            this.Controls.Add(this.waterMarkTextBox2);
            this.Controls.Add(this.waterMarkTextBox1);
            this.Name = "FormUIControlTest";
            this.Text = "FormUIControlTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.WaterMarkTextBox waterMarkTextBox1;
        private UI.WaterMarkTextBox waterMarkTextBox2;
        private UI.TextBoxEx textBoxEx1;
        private UI.TextBoxEx textBoxEx2;
        private UI.TextBoxNumEx textBoxNumEx1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private UI.ListViewEx listViewEx1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private UI.ListViewExp listViewEx21;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}