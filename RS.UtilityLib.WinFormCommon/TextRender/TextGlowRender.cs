using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.TextRender
{

    /// <summary>
    ///创建 发光文本 图片
    /// </summary>
    public class TextGlowRender
    {

        Color _lgbColorOne = Color.DeepSkyBlue;
        Color _lgbColorTwo = Color.DarkBlue;
        Size _clientSize = new Size(128, 64);
        Rectangle _clientRectangle = Rectangle.Empty;
        Graphics g = null;
        string _glowString = @"Rains";
        int _glowPenWidth = 8;

        string _logoFileName = string.Empty;
        int _logoWidth = 188;
        int _logoHeight = 200;
        int _addOffsetY = -128;
        bool _isUsingMinSize = true;
        int _wordsFontSize = 66;
        int _adjustImageOffsetY = 200;
        Bitmap _imageLogo = null;

        public TextGlowRender(Graphics graphics, Rectangle clientRect) {
            this.ClientSize = new Size(clientRect.Width, clientRect.Height);
            this.g = graphics;
            this.ClientRectangle = clientRect;
        }

        #region Public Properties

        /// <summary>
        /// 获取或设置背景渐变色的起始色(上边)
        /// </summary>
        public Color LgbColorOne {
            get { return _lgbColorOne; }
            set { _lgbColorOne = value; }
        }

        /// <summary>
        /// 获取或设置背景渐变色的终止色(下边)
        /// </summary>
        public Color LgbColorTwo {
            get { return _lgbColorTwo; }
            set { _lgbColorTwo = value; }
        }

        /// <summary>
        /// 图像的文件路径及文件名称
        /// </summary>
        public string LogoFileName {
            get {
                return _logoFileName;
            }
            set {
                _logoFileName = value;
            }
        }

        /// <summary>
        /// 获取或设置图像宽度
        /// </summary>
        public int LogoWidth {
            get {
                return _logoWidth;
            }
            set {
                _logoWidth = value;
            }
        }

        /// <summary>
        /// 获取或设置图像高度
        /// </summary>
        public int LogoHeight {
            get {
                return _logoHeight;
            }
            set {
                _logoHeight = value;
            }
        }

        /// <summary>
        /// 图像在垂直高度上的偏移大小
        /// </summary>
        public int AddOffsetY {
            get {
                return _addOffsetY;
            }
            set {
                _addOffsetY = value;
            }
        }

        /// <summary>
        /// 获取或设置图像
        /// </summary>
        public Bitmap ImageLogo {
            get {
                if (!string.IsNullOrEmpty(this.LogoFileName)) {
                    _imageLogo = (Bitmap)Bitmap.FromFile(this.LogoFileName);
                }
                else {
                    //System.Reflection.Assembly assembly = this.GetType().Assembly;
                    //_imageLogo = new Bitmap(assembly.GetManifestResourceStream("RS.UtilityLib.WinFormCommon.Properties.unknown"));
                    _imageLogo = new Bitmap(RS.UtilityLib.WinFormCommon.Properties.Resources.unknown);
                }
                return _imageLogo;
            }
            set {
                _imageLogo = value;
            }
        }

        /// <summary>
        /// 是否使用给定尺寸、屏幕尺寸及图像实际尺寸中的最小值，以保证图像的最佳效果
        /// </summary>
        public bool IsUsingMinSize {
            get {
                return _isUsingMinSize;
            }
            set {
                _isUsingMinSize = value;
            }
        }

        /// <summary>
        /// 获取或设置词语的大小
        /// </summary>
        public int WordsFontSize {
            get { return _wordsFontSize; }
            set { _wordsFontSize = value; }
        }

        /// <summary>
        /// Logo图片垂直位置调整
        /// </summary>
        public int AdjustImageOffsetY {
            get { return _adjustImageOffsetY; }
            set { _adjustImageOffsetY = value; }
        }

        /// <summary>
        /// 获取或设置ClientSize
        /// </summary>
        public Size ClientSize {
            get {
                return _clientSize;
            }
            set {
                _clientSize = value;
            }
        }

        /// <summary>
        /// 获取或设置ClientRectangle
        /// </summary>
        public Rectangle ClientRectangle {
            get {
                return _clientRectangle;
            }
            set {
                _clientRectangle = value;
            }
        }

        /// <summary>
        /// 获取或设置发光文字的字符串
        /// </summary>
        public string GlowString {
            get {
                return _glowString;
            }
            set {
                _glowString = value;
            }
        }

        /// <summary>
        /// 获取或设置发光文字的辉光笔画宽度
        /// </summary>
        public int GlowPenWidth {
            get { return _glowPenWidth; }
            set { _glowPenWidth = value; }
        }

        #endregion

        #region Public Method
        /// <summary>
        /// 绘制
        /// </summary>
        public void Draw(bool drawLogo) {
            LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, ClientRectangle.Height), _lgbColorOne, _lgbColorTwo);
            g.FillRectangle(lgb, ClientRectangle);
            if (drawLogo) {
                this.DrawBrawDrawLogo();
            }
            this.DrawWords();
        }
        #endregion

        #region Private Method

        private void DrawWords() {
            FontFamily fontFamily = new FontFamily("Times New Roman");

            Bitmap bm = new Bitmap(this.ClientSize.Width / 8, this.ClientSize.Height / 8);
            GraphicsPath pth = new GraphicsPath();

            pth.AddString(this.GlowString, fontFamily, (int)FontStyle.Bold, _wordsFontSize, new Point(3, 3), StringFormat.GenericTypographic);
            int w = Convert.ToInt32(pth.GetBounds().Width);
            int h = Convert.ToInt32(pth.GetBounds().Height);
            Graphics gWords = Graphics.FromImage(bm);
            Matrix mx = new Matrix(1.0f / 8, 0, 0, 1.0f / 8, -1.0f / 8, -1.0f / 8);
            gWords.SmoothingMode = SmoothingMode.HighQuality;
            gWords.Transform = mx;

            //Pen p=new Pen( this.GetRandomColor(), 8);
            Pen p = new Pen(Color.Yellow, _glowPenWidth);

            gWords.DrawPath(p, pth);

            gWords.FillPath(Brushes.Yellow, pth);

            gWords.Dispose();

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int offsetX = Convert.ToInt32((ClientRectangle.Width - w) / 2);
            int offsetY = Convert.ToInt32(ClientRectangle.Height / 2) - h + 20;
            g.Transform = new Matrix(1, 0, 0, 1, offsetX, offsetY);
            Rectangle destRect;
            destRect = ClientRectangle;
            g.TranslateTransform(0, _adjustImageOffsetY);
            g.DrawImage(bm, destRect, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);

            LinearGradientBrush lgb = new LinearGradientBrush(new PointF(0, pth.GetBounds().Top), new PointF(0, pth.GetBounds().Bottom - pth.GetBounds().Height / 2), _lgbColorTwo, _lgbColorOne);
            //g.FillPath(new SolidBrush( Color.Red ), pth);
            g.FillPath(lgb, pth);

            pth.Dispose();
            bm.Dispose();
        }

        private void DrawBrawDrawLogo() {
            //屏幕尺寸
            int screenWidth = this.ClientRectangle.Width;
            int screenHeight = this.ClientRectangle.Height;
            //图像实际尺寸
            int width = this.ImageLogo.Width;
            int height = this.ImageLogo.Height;
            //指定尺寸
            int paramWidth = this.LogoWidth;
            int paramHeight = this.LogoHeight;
            //求得指定尺寸与屏幕尺寸的合适值
            int clipWidth;
            int clipHeight;

            if (this.IsUsingMinSize) {
                clipWidth = Math.Min(Math.Min(screenWidth, paramWidth), width);
                clipHeight = Math.Min(Math.Min(screenHeight, paramHeight), height);
            }
            else {
                clipWidth = Math.Min(screenWidth, paramWidth);
                clipHeight = Math.Min(screenHeight, paramHeight);
            }
            //求得缩放比例
            double scale = Math.Min(clipWidth * 1.0 / width, clipHeight * 1.0 / height);

            int newWidth = Convert.ToInt32(width * scale);
            int newHeight = Convert.ToInt32(height * scale);

            int offsetX = (ClientRectangle.Width - newWidth) / 2;
            int offsetY = (ClientRectangle.Height - newHeight) / 2 + AddOffsetY;
            Rectangle destRect = new Rectangle(offsetX, offsetY, newWidth, newHeight);
            g.DrawImage(this.ImageLogo, destRect, new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
        }
        #endregion
    }

}
