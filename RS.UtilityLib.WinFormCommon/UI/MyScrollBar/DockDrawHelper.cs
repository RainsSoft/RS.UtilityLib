using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.UI.MyScrollBar
{
    public class DockDrawHelper
    {
        public static readonly Color ColorSubmenuBG = Color.FromArgb(255, 240, 240, 240);
        public static readonly Color ColorSelectedBG_Blue = Color.FromArgb(255, 186, 228, 246);
        public static readonly Color ColorSelectedBG_Header_Blue = Color.FromArgb(255, 146, 202, 230);
        public static readonly Color ColorSelectedBG_White = Color.FromArgb(255, 241, 248, 251);
        public static readonly Color ColorSelectedBG_Border = Color.FromArgb(255, 125, 162, 206);
        public static readonly Color ColorCheckBG = Color.FromArgb(255, 206, 237, 250);

        /// <summary>
        /// 
        /// </summary>
        public static void DrawRoundedRectangle(Graphics graphics, int xAxis, int yAxis, int width, int height, int diameter, Color color) {
            int radius = diameter / 2;
            Pen pen = new Pen(color);
            RectangleF BaseRect = new RectangleF(xAxis, yAxis, width, height);
            RectangleF ArcRect = new RectangleF(BaseRect.Location, new SizeF(diameter, diameter));
            graphics.DrawArc(pen, ArcRect, 180, 90);
            graphics.DrawLine(pen, xAxis + radius, yAxis, xAxis + width - radius, yAxis);
            ArcRect.X = BaseRect.Right - diameter;
            graphics.DrawArc(pen, ArcRect, 270, 90);
            graphics.DrawLine(pen, xAxis + width, yAxis + radius, xAxis + width, yAxis + height - radius);
            ArcRect.Y = BaseRect.Bottom - diameter;
            graphics.DrawArc(pen, ArcRect, 0, 90);
            graphics.DrawLine(pen, xAxis + radius, yAxis + height, xAxis + width - radius, yAxis + height);
            ArcRect.X = BaseRect.Left;
            graphics.DrawArc(pen, ArcRect, 90, 90);
            graphics.DrawLine(pen, xAxis, yAxis + radius, xAxis, yAxis + height - radius);
        }

    }
}
