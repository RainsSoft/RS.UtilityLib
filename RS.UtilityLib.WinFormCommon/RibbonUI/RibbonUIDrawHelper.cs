using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    internal class RibbonUIDrawHelper
    {
        internal enum DrawMode
        {
            拉伸 = 0,
            平铺 = 1,
        }
        /// <summary>
        /// 左右单独画，中间填充.
        /// 左右的宽度为8像素
        /// </summary>
        /// <param name="img"></param>
        /// <param name="descRect"></param>
        /// <param name="srcRect"></param>
        public static void DrawImageToRect1_V(Graphics g, Image img, Rectangle descRect, Rectangle srcRect, DrawMode mode) {
            Rectangle left = new Rectangle(descRect.X, descRect.Y, 8, descRect.Height);
            Rectangle leftSrc = new Rectangle(srcRect.X, srcRect.Y, 8, srcRect.Height);
            Rectangle right = new Rectangle(descRect.X + descRect.Width - 8, descRect.Y, 8, descRect.Height);
            Rectangle rightSrc = new Rectangle(srcRect.X + srcRect.Width - 8, srcRect.Y, 8, srcRect.Height);
            Rectangle middle = new Rectangle(descRect.X + 8, descRect.Y, descRect.Width - 16, descRect.Height);
            Rectangle middleSrc = new Rectangle(srcRect.X + 8, srcRect.Y, srcRect.Width - 16, srcRect.Height);

            g.DrawImage(img,
                left,
                leftSrc.X, leftSrc.Y, leftSrc.Width, leftSrc.Height,
                GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);

            using (TextureBrush tb = new TextureBrush(img, middleSrc, RibbonThemeManager.ImageAttr)) {
                //g.ResetTransform();
                if (mode == DrawMode.平铺) {
                    tb.TranslateTransform(middle.Left, middle.Top);
                }
                else {
                    tb.TranslateTransform(middle.Left, middle.Top);
                    tb.ScaleTransform((float)middle.Width / middleSrc.Width, (float)middle.Height / middleSrc.Height);
                }
                if (middle.Width != 0 || middle.Height != 0) {
                    g.FillRectangle(tb, middle);
                }
            }


            g.DrawImage(img, right,
                rightSrc.X, rightSrc.Y, rightSrc.Width, rightSrc.Height,
                GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
        }
        /// <summary>
        /// 上下单独画，中间填充。
        /// 上下的高度为8像素
        /// </summary>
        /// <param name="img"></param>
        /// <param name="descRect"></param>
        /// <param name="srcRect"></param>
        public static void DrawImageToRect1_H(Graphics g, Image img, Rectangle descRect, Rectangle srcRect, DrawMode mode) {
            Rectangle top = new Rectangle(descRect.X, descRect.Y, descRect.Width, 8);
            Rectangle topSrc = new Rectangle(srcRect.X, srcRect.Y, srcRect.Width, 8);
            Rectangle bottom = new Rectangle(descRect.X, descRect.Y + descRect.Height - 8, descRect.Width, 8);
            Rectangle bottomSrc = new Rectangle(srcRect.X, srcRect.Y + srcRect.Height - 8, srcRect.Width, 8);
            Rectangle middle = new Rectangle(descRect.X, descRect.Y + 8, descRect.Width, descRect.Height - 16);
            Rectangle middleSrc = new Rectangle(srcRect.X, srcRect.Y + 8, srcRect.Width, srcRect.Height - 16);

            g.DrawImage(img, top,
                topSrc.X, topSrc.Y, topSrc.Width, topSrc.Height,
                GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);

            using (TextureBrush tb = new TextureBrush(img, middleSrc)) {
                if (mode == DrawMode.平铺) {
                    tb.TranslateTransform(middle.Left, middle.Top);
                }
                else {
                    tb.TranslateTransform(middle.Left, middle.Top);
                    tb.ScaleTransform((float)middle.Width / middleSrc.Width, (float)middle.Height / middleSrc.Height);
                }
                if (middle.Width != 0 || middle.Height != 0) {
                    g.FillRectangle(tb, middle);
                }
            }
            g.DrawImage(img, bottom,
                bottomSrc.X, bottomSrc.Y, bottomSrc.Width, bottomSrc.Height,
                GraphicsUnit.Pixel, RibbonThemeManager.ImageAttr);
        }
    }
}
