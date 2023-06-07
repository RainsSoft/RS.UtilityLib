
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
            this.scrollPanel1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.ScrollPanel();
            this.angleChooserControl1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.AngleChooserControl();
            this.arrowButton1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.ArrowButton();
            this.colorWheel1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.ColorWheel();
            this.swatchControl1 = new RS.UtilityLib.WinFormCommon.UISystemLayer.SwatchControl();
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
            this.ruler1.Size = new System.Drawing.Size(444, 31);
            this.ruler1.TabIndex = 1;
            this.ruler1.Value = 0F;
            // 
            // scrollPanel1
            // 
            this.scrollPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scrollPanel1.IgnoreSetFocus = false;
            this.scrollPanel1.Location = new System.Drawing.Point(50, 141);
            this.scrollPanel1.Name = "scrollPanel1";
            this.scrollPanel1.ScrollPosition = new System.Drawing.Point(0, 0);
            this.scrollPanel1.Size = new System.Drawing.Size(200, 100);
            this.scrollPanel1.TabIndex = 2;
            // 
            // angleChooserControl1
            // 
            this.angleChooserControl1.Location = new System.Drawing.Point(50, 277);
            this.angleChooserControl1.Name = "angleChooserControl1";
            this.angleChooserControl1.Size = new System.Drawing.Size(90, 98);
            this.angleChooserControl1.TabIndex = 3;
            this.angleChooserControl1.Value = 0;
            this.angleChooserControl1.ValueDouble = 0D;
            // 
            // arrowButton1
            // 
            this.arrowButton1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.arrowButton1.ArrowDirection = System.Windows.Forms.ArrowDirection.Right;
            this.arrowButton1.ArrowOutlineWidth = 1F;
            this.arrowButton1.BackColor = System.Drawing.Color.Transparent;
            this.arrowButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.arrowButton1.DrawWithGradient = false;
            this.arrowButton1.ForcedPushedAppearance = false;
            this.arrowButton1.Location = new System.Drawing.Point(267, 162);
            this.arrowButton1.Name = "arrowButton1";
            this.arrowButton1.ReverseArrowColors = false;
            this.arrowButton1.Size = new System.Drawing.Size(22, 33);
            this.arrowButton1.TabIndex = 4;
            this.arrowButton1.Text = "arrowButton1";
            // 
            // colorWheel1
            // 
            this.colorWheel1.Location = new System.Drawing.Point(224, 271);
            this.colorWheel1.Name = "colorWheel1";
            this.colorWheel1.Size = new System.Drawing.Size(82, 88);
            this.colorWheel1.TabIndex = 5;
            // 
            // swatchControl1
            // 
            this.swatchControl1.BlinkHighlight = false;
            this.swatchControl1.Colors = new RS.UtilityLib.WinFormCommon.UISystemLayer.ColorBgra[0];
            this.swatchControl1.Location = new System.Drawing.Point(362, 152);
            this.swatchControl1.Name = "swatchControl1";
            this.swatchControl1.Size = new System.Drawing.Size(80, 70);
            this.swatchControl1.TabIndex = 6;
            this.swatchControl1.Text = "swatchControl1";
            // 
            // testPDN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.swatchControl1);
            this.Controls.Add(this.colorWheel1);
            this.Controls.Add(this.arrowButton1);
            this.Controls.Add(this.angleChooserControl1);
            this.Controls.Add(this.scrollPanel1);
            this.Controls.Add(this.ruler1);
            this.Name = "testPDN";
            this.Text = "testPDN";
            this.Controls.SetChildIndex(this.ruler1, 0);
            this.Controls.SetChildIndex(this.scrollPanel1, 0);
            this.Controls.SetChildIndex(this.angleChooserControl1, 0);
            this.Controls.SetChildIndex(this.arrowButton1, 0);
            this.Controls.SetChildIndex(this.colorWheel1, 0);
            this.Controls.SetChildIndex(this.swatchControl1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Ruler ruler1;
        private ScrollPanel scrollPanel1;
        private AngleChooserControl angleChooserControl1;
        private ArrowButton arrowButton1;
        private ColorWheel colorWheel1;
        private SwatchControl swatchControl1;
    }
}