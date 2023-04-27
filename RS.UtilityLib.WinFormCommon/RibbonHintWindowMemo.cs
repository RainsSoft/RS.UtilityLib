using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using RS.UtilityLib.WinFormCommon.RibbonUI;

namespace RS.UtilityLib.WinFormCommon
{
   
    public class RibbonHintWindowMemo : RibbonHintWindow
    {
        /*
         * 左侧256*256的图,右侧文本
         * ToDo:
         */
        private string ImagePath = "Skin\\editor\\suolue.bmp";

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e) {
            //base.OnPaintBackground(e);
        }


        private static unsafe Image Convert(Image source) {
            if (source == null) {
                return null;
            }
            Bitmap img = source as Bitmap;
            if (source.PixelFormat != PixelFormat.Format32bppPArgb) {
                int w = img.Width;
                int h = img.Height;
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
                BitmapData data = img.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, img.PixelFormat);
                BitmapData data2 = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
                int* p1 = (int*)data.Scan0.ToPointer();
                int* p2 = (int*)data2.Scan0.ToPointer();
                for (int i = 0; i < h; i++) {
                    for (int j = 0; j < w; j++) {
                        //c.A, c.R * c.A / 255, c.G * c.A / 255, c.B * c.A / 255
                        //*p2 = *p1;
                        int alpha = (int)(((*p1) & 0xFF000000) >> 24);
                        int R = (int)(((*p1) & 0x00FF0000) >> 16);
                        int G = (int)(((*p1) & 0x0000FF00) >> 8);
                        int B = (int)((*p1) & 0xFF);
                        *p2 = (int)(alpha << 24) + (int)(((int)((alpha * R) / 255)) << 16) + (int)(((int)((alpha * G) / 255)) << 8) + (int)(((int)((alpha * B) / 255)));

                        p1++;
                        p2++;
                    }
                }
                img.UnlockBits(data);
                bmp.UnlockBits(data2);
                //bmp.Save("Resource\\nofound1.png",ImageFormat.Png);
                return bmp;
            }
            else {
                return img;
            }
        }
        #region Windows Form Designer generated code

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
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pb_Thumbnail = new System.Windows.Forms.PictureBox();
            this.m_lblName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_lblMemo = new System.Windows.Forms.Label();
            this.m_lblVer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Thumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_Thumbnail
            // 
            this.pb_Thumbnail.BackColor = System.Drawing.Color.Transparent;
            this.pb_Thumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pb_Thumbnail.Location = new System.Drawing.Point(17, 18);
            this.pb_Thumbnail.Name = "pb_Thumbnail";
            this.pb_Thumbnail.Size = new System.Drawing.Size(160, 160);
            this.pb_Thumbnail.TabIndex = 0;
            this.pb_Thumbnail.TabStop = false;
            // 
            // m_lblName
            // 
            this.m_lblName.AutoSize = true;
            this.m_lblName.BackColor = System.Drawing.Color.Transparent;
            this.m_lblName.Location = new System.Drawing.Point(47, 191);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(29, 12);
            this.m_lblName.TabIndex = 1;
            this.m_lblName.Text = "模块";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(182, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "规 格:";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(182, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "说 明:";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(182, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "名 称:";
            this.label4.Visible = false;
            // 
            // m_lblMemo
            // 
            this.m_lblMemo.BackColor = System.Drawing.Color.Transparent;
            this.m_lblMemo.Location = new System.Drawing.Point(47, 228);
            this.m_lblMemo.Name = "m_lblMemo";
            this.m_lblMemo.Size = new System.Drawing.Size(142, 48);
            this.m_lblMemo.TabIndex = 4;
            this.m_lblMemo.Text = "测试";
            this.m_lblMemo.UseMnemonic = false;
            // 
            // m_lblVer
            // 
            this.m_lblVer.AutoSize = true;
            this.m_lblVer.BackColor = System.Drawing.Color.Transparent;
            this.m_lblVer.Location = new System.Drawing.Point(47, 209);
            this.m_lblVer.Name = "m_lblVer";
            this.m_lblVer.Size = new System.Drawing.Size(47, 12);
            this.m_lblVer.TabIndex = 2;
            this.m_lblVer.Text = "版本1.0";
            // 
            // RibbonHintWindowMemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::RS.UtilityLib.WinFormCommon.Properties.Resources.suolue;
            this.ClientSize = new System.Drawing.Size(194, 282);
            this.Controls.Add(this.m_lblMemo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_lblVer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_lblName);
            this.Controls.Add(this.pb_Thumbnail);
            this.Name = "RibbonHintWindowMemo";
            ((System.ComponentModel.ISupportInitialize)(this.pb_Thumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        /// <summary>
        /// 缩略图
        /// </summary>
        private System.Windows.Forms.PictureBox pb_Thumbnail;
        private System.Windows.Forms.Label m_lblName;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label m_lblMemo;
        private System.Windows.Forms.Label m_lblVer;

        public RibbonHintWindowMemo()
            : base() {

            InitializeComponent();
            //this.Name = "InfoHintWndMemo";

            ////this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(pictureBox1_Paint); 
            //ImagePath = backimgpath;
            //if (!string.IsNullOrEmpty(ImagePath)) {
            //    this.BackgroundImage = RibbonUISkinHelper.GetPathImage(ImagePath);
            //}
            //this.BackgroundImage = RS.UtilityLib.WinFormCommon.Properties.Resources.suolue;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            // base.OnPaint(e);
            if (this.BackgroundImage != null) {
                e.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle);
            }
            //this.BackgroundImage.Save("memo.bmp");
        }
        /*
        public void SetHint(ModelInfo model) {
            Image memoImage = null;
            if (!string.IsNullOrEmpty(model.ModelLargeImage)) {
                memoImage = RibbonUISkinHelper.GetPathImage(model.ModelLargeImage);
                if (memoImage == null) {
                    memoImage = RibbonUISkinHelper.GetPathImage("skin\\res\\nofind_large.png");
                }
            }
            if (this.pb_Thumbnail.BackgroundImage != null) {
                this.pb_Thumbnail.BackgroundImage.Dispose();
            }
            this.pb_Thumbnail.BackgroundImage = memoImage;
            m_lblName.Text = model.FlagName;
            if (model.Memo.Contains("#")) {
                string[] s = model.Memo.Split('#');
                m_lblVer.Text = s[0];
                m_lblMemo.Text = s[1];
            }
            else {
                m_lblVer.Text = "";
                m_lblMemo.Text = model.Memo;
            }
        }
        */
    }
}
