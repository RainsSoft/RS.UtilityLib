
namespace RS.UtilityLib.WinFormCommon.IME
{
    partial class IMEForm
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
            this.imeControl1 = new RS.UtilityLib.WinFormCommon.IME.IMEControl();
            this.SuspendLayout();
            // 
            // imeControl1
            // 
            this.imeControl1.BackColor = System.Drawing.Color.White;
            this.imeControl1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.imeControl1.Location = new System.Drawing.Point(188, 129);
            this.imeControl1.Name = "imeControl1";
            this.imeControl1.RenderText = "输入法测试";
            this.imeControl1.Size = new System.Drawing.Size(150, 150);
            this.imeControl1.TabIndex = 0;
            this.imeControl1.OnWndProcImeMsgEvent += new RS.UtilityLib.WinFormCommon.IME.WndProcImeMsgHandler(this.imeControl1_OnWndProcImeMsgEvent);
            // 
            // IMEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.imeControl1);
            this.Name = "IMEForm";
            this.Text = "IMEForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IMEControl imeControl1;
    }
}