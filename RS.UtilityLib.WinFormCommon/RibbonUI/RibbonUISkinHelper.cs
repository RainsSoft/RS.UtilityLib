using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    //class RibbonUISkinHelper
    public class RibbonUISkinHelper
    {
        public static Bitmap Clone(Bitmap source, PixelFormat format) {
            // copy image if pixel m_TexFormat is the same
            //if (source.PixelFormat == m_TexFormat)
            //    return source;

            int width = source.Width;
            int height = source.Height;

            //// create new image with desired pixel m_TexFormat
            //Bitmap bitmap = new Bitmap(width, height, m_TexFormat);

            //// draw source image on the new one using Graphics
            //Graphics g = Graphics.FromImage(bitmap);
            //g.DrawImage(source, 0, 0, width, height);
            //g.Dispose();

            //return bitmap;
            return source.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppPArgb);
        }
        /*
        /// <summary>
        /// PNG图片格式转换
        /// </summary>
        /// <param name="colorIndex">第几种颜色</param>
        /// <param name="baseFile">图片来源</param>
        /// <param name="outputFile">图片保存名</param>
        public static unsafe void ConvertTransparancyPng(int colorIndex,string baseFile, string outputFile) {
            using (FileStream fs = new FileStream(baseFile, FileMode.Open, FileAccess.Read)) {
                Bitmap img = (Bitmap)Image.FromStream(fs,true, false);
                int width = img.Width;
                int height = img.Height;

                Bitmap resultbmp = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                ColorPalette palette = resultbmp.Palette;
                int n = 0;
                foreach (Color tc in img.Palette.Entries) {
                    palette.Entries[n] = Color.FromArgb(255, tc);
                    n++;
                }

                palette.Entries[colorIndex] = Color.FromArgb(0, palette.Entries[colorIndex]);
                resultbmp.Palette = palette;

                //now to copy the actual bitmap data 
                BitmapData src = img.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    img.PixelFormat);

                BitmapData dst = resultbmp.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    resultbmp.PixelFormat);

                byte* pSrc = (byte*)src.Scan0.ToPointer();
                byte* pDst = (byte*)dst.Scan0.ToPointer();
                int offset = src.Stride - width;

                //steps through each pixel 
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        pDst[0] = pSrc[0];
                        pDst++;
                        pSrc++;
                    }
                    pDst += offset;
                    pSrc += offset;
                }

                //unlock the bitmaps 
                img.UnlockBits(src);
                resultbmp.UnlockBits(dst);

                resultbmp.Save(outputFile, ImageFormat.Png);

                img.Dispose();
                resultbmp.Dispose();
            }
        }
        /// <summary>
        /// 获取图片中的一块图片,有可能透明通道会丢失（待测试）
        /// </summary>
        /// <param name="imgpath">图片路径</param>
        /// <param name="picRec">截取图片范围</param>
        /// <param name="tansColor">指定颜色透明</param>
        /// <returns></returns>
        public static Bitmap GetImageByUVHW(string imgpath, Rectangle picRec, Color tansColor) {

            int offsetX = picRec.X;
            int offsetY = picRec.Y;
            int width = picRec.Width;
            int height = picRec.Height;
            Bitmap uvimage = new Bitmap(width, height);
            uvimage.MakeTransparent(tansColor);
            //
            byte[] data = null;
            using (System.IO.FileStream fs = new System.IO.FileStream(imgpath, FileMode.Open, FileAccess.Read)) {
                //    data = new byte[fs.Length];
                //    fs.Read(data, 0, data.Length);
                //}
                //using(System.IO.MemoryStream ms=new System.IO.MemoryStream(data)){    
                Bitmap source = new Bitmap(fs);
                if (offsetX + width > source.Width)
                    return source;//取的图片超过范围
                else if (offsetY + height > source.Height)
                    return source;//取的图片超过范围
                //            
                byte[] _dstData = new byte[source.Width * source.Height];
                for (int i = 0; i < source.Width; i++) {
                    for (int j = 0; j < source.Height; j++) {
                        if ((i >= offsetX && i < offsetX + width) &&
                            (j >= offsetY && j < offsetY + height)) {
                            Color c = source.GetPixel(i, j);
                            if (c != tansColor) {
                                uvimage.SetPixel(i - offsetX, i - offsetY, c);
                            }
                            else {
                                uvimage.SetPixel(i - offsetX, i - offsetY, Color.FromArgb(0, 0, 0, 0));
                            }
                        }
                    }
                }
            }
            return uvimage;
        }
        */
        private static Dictionary<string, Image> m_Images = new Dictionary<string, Image>();
        /// <summary>
        /// 获取图片,当找不到图片时，使用默认的图片代替
        /// 避免错误，把异常写入日志中去
        /// </summary>
        /// <param name="pathImgName"></param>
        /// <returns></returns>
        public static Image GetPathImage(string pathImgName) {
            //if (m_Images.ContainsKey(pathImgName)) {
            //    return m_Images[pathImgName];
            //}
            //else {
            try {
                Image img = RibbonUIRes.GetImage(pathImgName);
                //Image ret = Clone((Bitmap)img, PixelFormat.Format32bppPArgb); //new Bitmap(img);
                Image ret = new Bitmap(img);
                img.Dispose();
                return ret;
                //using (System.IO.FileStream fs = new FileStream(pathImgName, FileMode.Open, FileAccess.Read)) {
                //    Image img = Image.FromStream(fs, true, true);
                //    //m_Images.Add(pathImgName, img);
                //    return img;
                //}
            }
            catch (Exception e) {
                Debug.WriteLine("打开图片文件：" + pathImgName + "失败！原因是:" + e.ToString());
            }

            //}
            //
            return null;
        }
        public static void ClearImages() {
            foreach (var key in m_Images.Keys) {
                try {
                    m_Images[key].Dispose();
                }
                catch {
                }
            }
            m_Images.Clear();
        }

        //
        /// <summary>
        /// 绘制渐变色 透明度又颜色来指定
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"></param>
        public static void DrawLinearColor(Graphics g, Rectangle rectangle, Color startColor, Color endColor, float angle) {
            if (rectangle.Width == 0 || rectangle.Height == 0)
                return;
            LinearGradientBrush brush =
              new LinearGradientBrush(rectangle, startColor, endColor, angle);
            g.FillRectangle(brush, rectangle);
            brush.Dispose();
        }
        /// <summary>
        /// 绘制图片 缩放还是原始大小，指定图片的大小与位置
        /// 这里处理我们需要的绘制就可以
        /// </summary>
        public static void DrawImage(Graphics g, Image img, Rectangle posSizeRec, Rectangle imageRec, bool imgCanScale) {
            if (imgCanScale) {
                g.DrawImage(img, posSizeRec.X, posSizeRec.Y, imageRec, GraphicsUnit.Pixel);
            }
            else {
                g.DrawImage(img, posSizeRec, imageRec, GraphicsUnit.Pixel);
            }
        }
    }
}
