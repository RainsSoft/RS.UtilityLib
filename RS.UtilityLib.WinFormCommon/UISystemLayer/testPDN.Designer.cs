
namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    partial class testPDN
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
            this.ruler1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.Ruler();
            this.SuspendLayout();
            // 
            // ruler1
            // 
            this.ruler1.HighlightEnabled = false;
            this.ruler1.HighlightLength = 0F;
            this.ruler1.HighlightStart = 0F;
            this.ruler1.Location = new System.Drawing.Point(50, 46);
            this.ruler1.MeasurementUnit = RS.UtilityLib.WinFormCommon.UISystemLayer.MeasurementUnit.Inch;
            this.ruler1.Name = "ruler1";
            this.ruler1.Offset = 0F;
            this.ruler1.Size = new System.Drawing.Size(444, 150);
            this.ruler1.TabIndex = 1;
            this.ruler1.Value = 0F;
            // 
            // testPDN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ruler1);
            this.Name = "testPDN";
            this.Text = "testPDN";
            this.Controls.SetChildIndex(this.ruler1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Ruler ruler1;
    }
}