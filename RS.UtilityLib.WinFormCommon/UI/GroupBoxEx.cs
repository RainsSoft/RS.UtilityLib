using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// 实现自定义GroupBox绘制 <see cref="GroupBox"/> <see cref="Control"/>.
    /// 可用于答题小题的分割
    /// </summary>
    public class GroupBoxEx : GroupBox
    {
        #region Private Fields

        /// <summary>
        ///标题区域高度
        /// </summary>
        private int mGroupCaptionHeight = 20;

        /// <summary>
        ///group分割线颜色
        /// </summary>
        private Color mSeperatorColor = SystemColors.ControlDark;

        /// <summary>
        /// 标题图片
        /// </summary>
        private Image mGroupImage;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [DefaultValue(20)]
        [Description("标题区域高度.")]
        public int GroupCaptionHeight {
            get {
                return mGroupCaptionHeight;
            }
            set {
                if (value > 0 && value < Height && !Equals(mGroupCaptionHeight, value)) {
                    mGroupCaptionHeight = value;

                    Padding = new Padding(
                        Margin.Left
                      , mGroupCaptionHeight
                      , Margin.Right
                      , Margin.Bottom);

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the seperator <see cref="Color"/>.
        /// </summary>
        [Browsable(true)]
        [Description("分割线颜色.")]
        public Color SeperatorColor {
            get {
                return mSeperatorColor;
            }
            set {
                if (!Equals(mSeperatorColor, value)) {
                    mSeperatorColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Description("标题图片.")]
        public Image GroupImage {
            get {
                return mGroupImage;
            }
            set {
                if (!Equals(mGroupImage, value)) {
                    mGroupImage = value;
                    Invalidate();
                }
            }
        }

        #endregion
        private Color _BorderColor = Color.Black;

        [Browsable(true), Description("边框颜色"), Category("自定义分组")]
        public Color BorderColor {
            get {
                return _BorderColor;
            }
            set {
                _BorderColor = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// 是否绘制边缘
        /// </summary>
        [Browsable(true)]
        [Description("是否绘制边框.")]
        public bool NeedDrawBorder {
            get;
            set;
        }
        #region Overridden Methods


        protected override void OnPaint(PaintEventArgs e) {
            if (Width > 0 && Height > 0) {
                if (mGroupImage != null) {
                    //绘制标题图片
                    e.Graphics.DrawImage(
                        mGroupImage
                      , new Rectangle(0, ((mGroupCaptionHeight - 16) >> 1), 16, 16)
                      , new Rectangle(0, 0, mGroupImage.Width, mGroupImage.Height)
                      , GraphicsUnit.Pixel);
                }
                if (NeedDrawBorder) {
                    //绘制边框
                    using (Pen vPen = new Pen(this._BorderColor, 1f)) {
                        Rectangle rec = e.ClipRectangle;
                        e.Graphics.DrawRectangle(vPen, new Rectangle(0, 0, rec.Width - 1, rec.Height - 1));
                    }
                }
                //绘制标题
                TextRenderer.DrawText(
                    e.Graphics
                  , Text
                  , Font
                  , new Rectangle(mGroupImage != null ? 20 : 0, 0, Width, mGroupCaptionHeight)
                  , ForeColor
                  , BackColor
                  , TextFormatFlags.SingleLine |
                    TextFormatFlags.TextBoxControl |
                    TextFormatFlags.VerticalCenter);

                if (!Equals(mSeperatorColor, Color.Transparent)) {
                    //绘制分割线
                    Rectangle hRect = new Rectangle(
                        0
                      , mGroupCaptionHeight
                      , Width
                      , 1);

                    using (Brush hBrush = new LinearGradientBrush(hRect, mSeperatorColor, BackColor, LinearGradientMode.Horizontal)) {
                        e.Graphics.FillRectangle(hBrush, hRect);
                    }
                }
            }
        }
        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            this.Focus();
        }
        #endregion

        #region Constructor

        /// <summary>
        /// GroupBoxEx
        /// </summary>
        public GroupBoxEx() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            Padding = new Padding(
                Margin.Left
              , mGroupCaptionHeight
              , Margin.Right
              , Margin.Bottom);
        }

        #endregion
    }
}
