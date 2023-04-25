using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.TextRender
{
    /// <summary>
    /// 文本阴影
    /// </summary>
    public class TextShadowRender
    {
        private int radius = 5;
        private int distance = 10;
        private double angle = 60;
        private byte alpha = 192;

        ///
        /// 高斯卷积矩阵
        /// 
        private int[] gaussMatrix;
        /// 
        /// 卷积核
        /// 
        private int nuclear = 0;
        /// 
        /// 阴影半径
        /// 
        public int Radius {
            get {
                return radius;
            }
            set {
                if (radius != value) {
                    radius = value;
                    MakeGaussMatrix();
                }
            }
        }

        /// 
        ///  阴影距离
        /// 
        public int Distance {
            get {
                return distance;
            }
            set {
                distance = value;
            }
        }
        /// 
        ///  阴影输出角度(左边平行处为0度。顺时针方向)
        /// 
        public double Angle {
            get {
                return angle;
            }
            set {
                angle = value;
            }
        }
        /// 
        /// 阴影文字的不透明度
        /// 
        public byte Alpha {
            get {
                return alpha;
            }
            set {
                alpha = value;
            }
        }
        /// 
        /// 对文字阴影位图按阴影半径计算的高斯矩阵进行卷积模糊
        /// 
        /// 文字阴影位图
        private unsafe void MaskShadow(Bitmap bmp) {
            if (nuclear == 0)
                MakeGaussMatrix();
            Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            // 克隆临时位图，作为卷积源
            Bitmap tmp = (Bitmap)bmp.Clone();
            BitmapData dest = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            BitmapData source = tmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try {
                // 源首地址(0, 0)的Alpha字节，也就是目标首像素的第一个卷积乘数的像素点
                byte* ps = (byte*)source.Scan0;
                ps += 3;
                // 目标地址为卷积半径点(radius, radius)的Alpha字节
                byte* pd = (byte*)dest.Scan0;
                pd += (radius * (dest.Stride + 4) + 3);
                // 位图实际卷积的部分
                int width = dest.Width - radius * 2;
                int height = dest.Height - radius * 2;
                int matrixSize = radius * 2 + 1;
                // 卷积矩阵字节偏移
                int mOffset = dest.Stride - matrixSize * 4;
                // 行尾卷积半径(radius)的偏移
                int rOffset = radius * 8;
                int count = matrixSize * matrixSize;

                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {

                        byte* s = ps - mOffset;
                        int v = 0;
                        for (int i = 0; i < count; i++, s += 4) {
                            if ((i % matrixSize) == 0)
                                s += mOffset;           // 卷积矩阵的换行
                            v += gaussMatrix[i] * *s;   // 位图像素点Alpha的卷积值求和
                        }
                        // 目标位图被卷积像素点Alpha等于卷积和除以卷积核
                        *pd = (byte)(v / nuclear);
                        pd += 4;
                        ps += 4;
                    }
                    pd += rOffset;
                    ps += rOffset;
                }
            }
            finally {
                tmp.UnlockBits(source);
                bmp.UnlockBits(dest);
                tmp.Dispose();
            }
        }

        /// 
        /// 按给定的阴影半径生成高斯卷积矩阵
        /// 
        protected virtual void MakeGaussMatrix() {
            double Q = (double)radius / 2.0;
            if (Q == 0.0)
                Q = 0.1;
            int n = radius * 2 + 1;
            int index = 0;
            nuclear = 0;
            gaussMatrix = new int[n * n];

            for (int x = -radius; x <= radius; x++) {
                for (int y = -radius; y <= radius; y++) {
                    gaussMatrix[index] = (int)Math.Round(Math.Exp(-((double)x * x + y * y) / (2.0 * Q * Q)) /
                                                         (2.0 * Math.PI * Q * Q) * 1000.0);
                    nuclear += gaussMatrix[index];
                    index++;
                }
            }
        }
        public TextShadowRender() {
            //
            // TODO: Add constructor logic here
            //
        }
        /// 
        /// 画文字阴影
        /// 
        /// 画布
        /// 文字串
        /// 字体
        /// 文字串的布局矩形
        /// 文字串输出格式
        public void Draw(Graphics g, string text, Font font, RectangleF layoutRect, StringFormat format) {
            RectangleF sr = new RectangleF((float)(radius * 2), (float)(radius * 2), layoutRect.Width, layoutRect.Height);
            // 根据文字布局矩形长宽扩大文字阴影半径4倍建立一个32位ARGB格式的位图
            Bitmap bmp = new Bitmap((int)sr.Width + radius * 4, (int)sr.Height + radius * 4, PixelFormat.Format32bppArgb);
            // 按文字阴影不透明度建立阴影画刷
            Brush brush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
            Graphics bg = Graphics.FromImage(bmp);
            try {
                // 在位图上画文字阴影
                bg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                bg.DrawString(text, font, brush, sr, format);
                // 制造阴影模糊
                MaskShadow(bmp);
                // 按文字阴影角度、半径和距离输出文字阴影到给定的画布
                RectangleF dr = layoutRect;
                dr.Offset((float)(Math.Cos(Math.PI * angle / 180.0) * distance),
                          (float)(Math.Sin(Math.PI * angle / 180.0) * distance));
                sr.Inflate((float)radius, (float)radius);
                dr.Inflate((float)radius, (float)radius);
                g.DrawImage(bmp, dr, sr, GraphicsUnit.Pixel);
            }
            finally {
                bg.Dispose();
                brush.Dispose();
                bmp.Dispose();
            }
        }

        /// 
        /// 画文字阴影
        /// 
        /// 画布
        /// 文字串
        /// 字体
        /// 文字串的布局矩形
        public void Draw(Graphics g, string text, Font font, RectangleF layoutRect) {
            Draw(g, text, font, layoutRect, null);
        }
        /// 
        /// 画文字阴影
        /// 
        /// 画布
        /// 文字串
        /// 字体
        /// 文字串的输出原点
        /// 文字串输出格式
        public void Draw(Graphics g, string text, Font font, PointF origin, StringFormat format) {
            RectangleF rect = new RectangleF(origin, g.MeasureString(text, font, origin, format));
            Draw(g, text, font, rect, format);
        }
        /// 
        /// 画文字阴影
        /// 
        /// 画布
        /// 文字串
        /// 字体
        /// 文字串的输出原点
        public void Draw(Graphics g, string text, Font font, PointF origin) {
            Draw(g, text, font, origin, null);
        }
    }

}
