using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace RS.UtilityLib.WinFormCommon.UINotifier
{
    public partial class UIMessageBoxEx : Form
    {
        public UIMessageBoxEx() {
            InitializeComponent();
        }
        public static DialogResult ShowOkAfter10Close(IWin32Window owner, string text, string caption, int secends = 10) {

            return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, secends, DialogResult.OK);
        }
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, int closeAfter, DialogResult defaultRet) {
            using (UIMessageBoxEx mb = new UIMessageBoxEx()) {
                mb.m_buttons = buttons;
                mb.m_icon = icon;
                mb.m_closeAfter = closeAfter;
                mb.m_ret = defaultRet;
                mb.m_watch = Stopwatch.StartNew();
                mb.Text = caption;
                mb.label1.Text = text;

                return mb.ShowDialog(owner);
            }
        }
        private MessageBoxButtons m_buttons;
        private MessageBoxIcon m_icon;
        private int m_closeAfter = 0;
        private DialogResult m_ret;
        private Stopwatch m_watch;
        private void MessageBoxEx_Load(object sender, EventArgs e) {
            CreateIcon();
            CreateAutoClose();
            CreateButtons();
        }

        private void CreateAutoClose() {
            if (m_closeAfter <= 0) {
                return;
            }
            timer1.Enabled = true;
            timer1.Start();
        }
        private void CreateIcon() {

            if (this.m_icon == MessageBoxIcon.Asterisk) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Asterisk.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Error) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Exclamation) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Information) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Information.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Question) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Stop) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
            }
            if (this.m_icon == MessageBoxIcon.Warning) {
                this.pictureBox1.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
            }

        }


        private void CreateButtons() {
            switch (m_buttons) {
                case MessageBoxButtons.OK:
                    CreateOkButton();
                    break;
                case MessageBoxButtons.OKCancel:
                    CreateOkCancelButton();
                    break;
                case MessageBoxButtons.YesNo:
                    CreateYesNoButton();
                    break;
                case MessageBoxButtons.YesNoCancel:
                    CreateYesNoCancelButton();
                    break;
                default:
                    break;
            }
        }

        private void CreateYesNoCancelButton() {
            var btn_ok = new Button();
            btn_ok.Text = "是";
            this.Controls.Add(btn_ok);

            var btn_no = new Button();
            btn_no.Text = "否";
            this.Controls.Add(btn_no);

            var btn_cancel = new Button();
            btn_cancel.Text = "取消";
            this.Controls.Add(btn_cancel);

            btn_ok.Click += (s, e) => {
                this.m_ret = DialogResult.Yes;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };
            btn_no.Click += (s, e) => {
                this.m_ret = DialogResult.No;
                this.DialogResult = DialogResult.No;
                this.Close();
            };
            btn_cancel.Click += (s, e) => {
                this.m_ret = DialogResult.Cancel;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            btn_ok.Top = btn_no.Top = btn_cancel.Top = Math.Max(this.pictureBox1.Bottom, this.label1.Bottom) + 16;
            btn_no.Left = (int)((this.Width - btn_no.Width) * 0.5f);
            btn_ok.Left = btn_no.Left - 32 - btn_ok.Width;
            btn_cancel.Left = btn_no.Left + btn_no.Width + 32;


        }

        private void CreateYesNoButton() {
            var btn_ok = new Button();
            btn_ok.Text = "是";
            this.Controls.Add(btn_ok);

            var btn_no = new Button();
            btn_no.Text = "否";
            this.Controls.Add(btn_no);


            btn_ok.Click += (s, e) => {
                this.m_ret = DialogResult.Yes;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };
            btn_no.Click += (s, e) => {
                this.m_ret = DialogResult.No;
                this.DialogResult = DialogResult.No;
                this.Close();
            };


            btn_ok.Top = btn_no.Top = Math.Max(this.pictureBox1.Bottom, this.label1.Bottom) + 16;
            btn_no.Left = (int)((this.Width + 32) * 0.5f);
            btn_ok.Left = (int)((this.Width - 32) * 0.5f) - btn_ok.Width;

        }

        private void CreateOkCancelButton() {
            var btn_ok = new Button();
            btn_ok.Text = "确定";
            this.Controls.Add(btn_ok);

            var btn_cancel = new Button();
            btn_cancel.Text = "取消";
            this.Controls.Add(btn_cancel);

            btn_ok.Click += (s, e) => {
                this.m_ret = DialogResult.Yes;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };
            btn_cancel.Click += (s, e) => {
                this.m_ret = DialogResult.Cancel;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            btn_ok.Top = btn_cancel.Top = Math.Max(this.pictureBox1.Bottom, this.label1.Bottom) + 16;
            btn_cancel.Left = (int)((this.Width + 32) * 0.5f);
            btn_ok.Left = (int)((this.Width - 32) * 0.5f) - btn_ok.Width;
        }

        private void CreateOkButton() {
            var btn_ok = new Button();
            btn_ok.Text = "确定";
            this.Controls.Add(btn_ok);


            btn_ok.Click += (s, e) => {
                this.m_ret = DialogResult.Yes;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };

            btn_ok.Top = Math.Max(this.pictureBox1.Bottom, this.label1.Bottom) + 16;
            btn_ok.Left = (int)((this.Width - btn_ok.Width) * 0.5f);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (m_watch.ElapsedMilliseconds > m_closeAfter * 1000) {
                this.DialogResult = this.m_ret;
                this.Close();
            }
        }


        #region designer

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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12 + 48 + 24, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MessageBoxEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(408, 129);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBoxEx";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MessageBoxEx";
            this.Load += new System.EventHandler(this.MessageBoxEx_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;

        #endregion
    }
}