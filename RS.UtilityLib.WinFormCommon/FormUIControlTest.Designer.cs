
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
            this.textBoxEx2 = new RS.UtilityLib.WinFormCommon.UI.TextBoxEx();
            this.textBoxEx1 = new RS.UtilityLib.WinFormCommon.UI.TextBoxEx();
            this.waterMarkTextBox2 = new RS.UtilityLib.WinFormCommon.UI.WaterMarkTextBox();
            this.waterMarkTextBox1 = new RS.UtilityLib.WinFormCommon.UI.WaterMarkTextBox();
            this.textBoxNumEx1 = new RS.UtilityLib.WinFormCommon.UI.TextBoxNumEx();
            this.SuspendLayout();
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
            // FormUIControlTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}