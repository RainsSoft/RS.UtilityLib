using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// 图片选择
    /// </summary>
    public partial class PictureSelector : PictureBox
    {
        class BrowseRect
        {
            internal Rectangle Rect;
            internal bool HasFocus;
            internal bool FirstFocus;
            internal bool FirstLostFocus;
            internal bool IsHover(int x, int y) {
                bool ret = Rect.Contains(x, y);
                HasFocus = ret;
                if (ret) {
                    if (FirstFocus == false) {
                        FirstFocus = true;
                    }
                    FirstLostFocus = false;
                }
                else {
                    FirstFocus = false;
                    if (FirstLostFocus == false) {
                        FirstLostFocus = true;
                    }
                }
                return ret;
            }
            internal void OnClick() {

            }
        }

        BrowseRect m_Browse;
        private Point m_ClickPoint = Point.Empty;
        private bool m_Hover = false;
        private bool m_HoverBrowse = false;
        public PictureSelector() {
            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.m_Browse = new BrowseRect();
            this.m_Browse.Rect = new Rectangle(this.ClientRectangle.Right - 32 - 2, this.ClientRectangle.Bottom - 2 - 16, 32, 16);
            this.Cursor = Cursors.Hand;
        }

        public event EventHandler CheckChange;
        /// <summary>
        /// 效果图.
        /// 多个效果整合在一张图上
        /// </summary>
        public Image EffectImage {
            get;
            set;
        }
        /// <summary>
        /// 未选择图片时显示的图案
        /// </summary>
        public Rectangle DefaultRect {
            get;
            set;
        }
        /// <summary>
        /// 鼠标覆盖时显示的图案
        /// </summary>
        public Rectangle HoverMaskRect {
            get;
            set;
        }
        /// <summary>
        /// 选中时显示的图案
        /// </summary>
        public Rectangle CheckedMaskRect {
            get;
            set;
        }
        /// <summary>
        /// 浏览默认的图案
        /// </summary>
        public Rectangle BrowseNormalRect {
            get;
            set;
        }
        /// <summary>
        /// 浏览覆盖时显示的图案
        /// </summary>
        public Rectangle BrowseHoverRect {
            get;
            set;
        }
        private bool m_ImageSelected = false;
        private bool m_Checked = false;
        public bool Checked {
            get {
                return m_Checked;
            }
            set {
                m_Checked = value;
                if (CheckChange != null) {
                    CheckChange(this, EventArgs.Empty);
                }
                this.Invalidate();
            }
        }
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.m_Browse.Rect = new Rectangle(this.ClientRectangle.Right - 32 - 2, this.ClientRectangle.Bottom - 2 - 16, 32, 16);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            m_ClickPoint = new Point(e.X, e.Y);
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            m_Hover = true;
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (m_Browse.IsHover(e.X, e.Y)) {
                if (m_Browse.FirstFocus) {
                    this.Invalidate();
                }
            }
            else {
                if (m_Browse.FirstLostFocus) {
                    this.Invalidate();
                }
            }
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_Hover = false;
            m_Browse.IsHover(int.MaxValue, int.MaxValue);
            this.Invalidate();
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            if (m_Browse.IsHover(m_ClickPoint.X, m_ClickPoint.Y)) {
                using (OpenFileDialog op = new OpenFileDialog()) {
                    op.Filter = "图像文件(*.BMP;*.JPG;*.jpeg;*.GIF;*.PNG)|*.BMP;*.JPG;*.jpeg;*.GIF;*.png";
                    op.RestoreDirectory = true;
                    if (op.ShowDialog() == DialogResult.OK) {
                        if (CheckFile(op.FileName) == false) {
                            MessageBox.Show("图片尺寸最大不能超过128x128", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        this.Image = Image.FromFile(op.FileName, true);
                        m_ImageSelected = true;
                    }
                }
            }
            else {
                //如果没有图片，则无法Checked
                if (this.Image != null) {
                    this.Checked = !this.Checked;
                }
            }
            this.Invalidate();
        }

        private Size m_ImgSize = Size.Empty;
        private bool CheckFile(string p) {
            using (Image img = Image.FromFile(p)) {
                m_ImgSize = img.Size;
                if (m_ImgSize.Height > 128 || m_ImgSize.Width > 128) {
                    return false;
                }
            }
            return true;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            if (DesignMode && this.EffectImage == null) {
                return;
            }
            if (m_Checked == false && this.Image == null) {
                pe.Graphics.DrawImage(this.EffectImage, this.ClientRectangle, this.DefaultRect.X, this.DefaultRect.Y, this.DefaultRect.Width, this.DefaultRect.Height, GraphicsUnit.Pixel);
            }
            if (m_Hover) {

                DrawHoverMask(pe.Graphics);
                if (m_Checked) {
                    DrawCheckedMask(pe.Graphics);
                }
            }
            else if (m_Checked) {
                DrawCheckedMask(pe.Graphics);
            }

        }

        private void DrawCheckedMask(Graphics graphics) {
            //画选中效果图
            graphics.DrawImage(this.EffectImage, this.ClientRectangle, this.CheckedMaskRect.X, this.CheckedMaskRect.Y, this.CheckedMaskRect.Width, this.CheckedMaskRect.Height, GraphicsUnit.Pixel);
        }

        private void DrawHoverMask(Graphics graphics) {
            //画焦点效果图
            graphics.DrawImage(this.EffectImage, this.ClientRectangle, this.HoverMaskRect.X, this.HoverMaskRect.Y, this.HoverMaskRect.Width, this.HoverMaskRect.Height, GraphicsUnit.Pixel);

            if (m_Browse.HasFocus) {
                //画browse焦点图
                graphics.DrawImage(this.EffectImage, m_Browse.Rect, this.BrowseHoverRect.X, this.BrowseHoverRect.Y, this.BrowseHoverRect.Width, this.BrowseHoverRect.Height, GraphicsUnit.Pixel);

            }
            else {
                //画browse普通图
                graphics.DrawImage(this.EffectImage, m_Browse.Rect, this.BrowseNormalRect.X, this.BrowseNormalRect.Y, this.BrowseNormalRect.Width, this.BrowseNormalRect.Height, GraphicsUnit.Pixel);

            }

        }

        //--------------
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

    }
}
