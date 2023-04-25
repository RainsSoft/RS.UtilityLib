using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace RS.UtilityLib.WinFormCommon
{
    public class FastBitmap
    {
        void test() {
            // TextRenderer tr = new TextRenderer();
            // var bp=tr.MakeTextTexture(new TextRenderer.Text_() { font=new TextRenderer.FontCi() {
            //  } });

            // FastBitmap fbp = new FastBitmap() { bmp = bp };
            // fbp.Lock();
            //var c1 =bp.GetPixel(0, 0);
            // var c2 = fbp.GetPixel(0,0);
            // fbp.Unlock();
        }
        public Bitmap bmp {
            get; set;
        }
        BitmapData bmd;
        public void Lock() {
            if (bmd != null) {
                throw new Exception("Already locked.");
            }
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) {
                bmp = new Bitmap(bmp);
            }
            bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
        }
        /// <summary>
        /// 前提是先 Lock()
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetPixel(int x, int y) {
            if (bmd == null) {
                throw new Exception();
            }
            unsafe {
                int* row = (int*)((byte*)bmd.Scan0 + (y * bmd.Stride));
                return row[x];
            }
        }
        /// <summary>
        /// 前提是先 Lock()
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, int color) {
            if (bmd == null) {
                throw new Exception();
            }
            unsafe {
                int* row = (int*)((byte*)bmd.Scan0 + (y * bmd.Stride));
                row[x] = color;
            }
        }
        public void Unlock() {
            if (bmd == null) {
                throw new Exception("Not locked.");
            }
            bmp.UnlockBits(bmd);
            bmd = null;
        }

        /// <summary>
        /// 创建带边缘背景的一张指定文字图片
        /// </summary>
        /// <param name="backImg">必须指定背景图</param>
        /// <param name="word"></param>
        /// <param name="fontName">必须指定存在字体</param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bitmap GetSideWordImage(Image backImg, string word, string fontName, float fontSize, Color color) {
            if (string.IsNullOrEmpty(word)) {
                return null;
            }
            if (backImg == null) {
                return null;
            }
            Bitmap bp = null;
#if DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            Console.WriteLine("开始创建图");
            sw.Start();
#endif

            using (Font font = new Font(fontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel)) {
                if (font == null) {
                    Debug.WriteLine("不存在字体 " + fontName);
                    return null;
                }
                using (Graphics g = Graphics.FromImage(backImg)) {
                    SizeF size = g.MeasureString(word, font);
                    int pad = 6;
                    int with = 500 + pad * 2;
                    int row = (int)(size.Width / with) + 1;
                    int height = (int)(size.Height * row) + pad * 2;

                    //
                    string strp = "";
                    string msg = word;
                    int len = (Encoding.UTF8.GetBytes(word).Length / row);
                    //if (len % 2 != 0) {
                    len = len + 1;
                    int maxWidth = 0;
                    //}
                    //int index = 0;
                    for (int i = 0; i < row; i++) {
                        if (i < row - 1) {
                            string stp = GetFixLenCtr(msg, len); //word.Substring(i * row *len , len)+"\n";
                            SizeF sz = g.MeasureString(stp, font);
                            int rlen = len;
                            while (sz.Width + pad * 2 < with) {
                                rlen++;
                                stp = GetFixLenCtr(msg, rlen);
                                sz = g.MeasureString(stp, font);
                            }
                            while (sz.Width > with + 2 * pad) {
                                rlen--;
                                if (rlen < 2)
                                    break;
                                stp = GetFixLenCtr(msg, rlen);
                                sz = g.MeasureString(stp, font);
                            }
                            maxWidth = (int)(maxWidth < sz.Width ? sz.Width : maxWidth);
                            strp += stp + "\n";
                            msg = msg.Remove(0, stp.Length < msg.Length ? stp.Length : msg.Length);
                        }
                        else {
                            strp += msg;//word.Substring(i * len, word.Length-len*i); 
                            SizeF sz = g.MeasureString(msg, font);
                            maxWidth = (int)(maxWidth < sz.Width ? sz.Width : maxWidth);
                        }
                    }

                    //绘制背景图
                    bp = new Bitmap(with, with);
                    using (Graphics g2 = Graphics.FromImage(bp)) {
                        g2.DrawImage(backImg, 0, 0, maxWidth + 2 * (pad > with ? with : (maxWidth + 2 * pad)), height < with ? height : with);
                        // Set up all the string parameters.
                        //FontFamily family = font.FontFamily;
                        //int fontStyle = (int)FontStyle.Bold;
                        Point origin = new Point(2, 2);
                        //StringFormat format = StringFormat.GenericDefault;
                        //g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;//设置文本输出质量
                        //g2.SmoothingMode = SmoothingMode.AntiAlias;
                        //g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //绘制边缘                    
                        //GraphicsPath myPath = new GraphicsPath();
                        // Add the string to the path.
                        //myPath.AddString(strp,
                        //    family,
                        //    fontStyle,
                        //    fontSize,
                        //    origin,
                        //    format);

                        //Draw the path to the screen.
                        //
                        //Color _lgbColorOne = Color.DeepSkyBlue;
                        //Color _lgbColorTwo = Color.DarkBlue;
                        //float oy = myPath.GetBounds().Bottom - myPath.GetBounds().Height / 2;
                        //LinearGradientBrush lgb = new LinearGradientBrush(new PointF(0, myPath.GetBounds().Top), new PointF(0, oy), _lgbColorOne, _lgbColorTwo);
                        //lgb.SetBlendTriangularShape(0.5f);
                        //g2.FillPath(lgb, myPath);
                        ////
                        //lgb.RotateTransform(180);
                        //lgb.TranslateTransform(0, oy);
                        //g2.FillPath(lgb, myPath);
                        //                   
                        //g2.DrawPath(System.Drawing.Pens.YellowGreen, myPath);

                        //绘制文字
                        using (SolidBrush sb = new SolidBrush(color)) {
                            g2.DrawString(strp, font, sb, new Point(origin.X, origin.Y));//绘制阴影
                            sb.Dispose();
                        }
                        //myPath.Dispose();

                    }
                }
            }
#if DEBUG
            bp.Save("drawWord.png");
            Console.WriteLine("绘制图片" + sw.ElapsedTicks / (float)System.Diagnostics.Stopwatch.Frequency);
            sw.Stop();
#endif
            return bp;
        }

        static string GetFixLenCtr(string pstr, int Num) {
            //System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(pstr), 0, Num);
            string StrNum = pstr;
            byte[] bytes1 = System.Text.Encoding.Default.GetBytes(StrNum.Trim());
            int icha = bytes1.Length;
            if (icha > Num) {
                //byte[] bytes2 = System.Text.Encoding.Default.GetBytes(pstr.Trim().Substring(0, Num)); 
                string strNum1 = System.Text.Encoding.Default.GetString(bytes1, 0, Num);
                //strNum1 = System.Text.Encoding.Default.GetString(bytes2, 0, Num);
                int len = strNum1.Length;
                string subStr = pstr.Substring(0, len);
                if (subStr != strNum1) {
                    StrNum = System.Text.Encoding.Default.GetString(bytes1, 0, Num - 1);
                }
                else {
                    StrNum = strNum1;
                }

            }
            else {
                int slen = icha;
                for (int k = 0; k < Num - slen; k++) {
                    StrNum += " ";
                }

            }
            return StrNum;

        }
    }
}
