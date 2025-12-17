#define NAMEOF_ENABLE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq.Expressions;

namespace HML
{
    /// <summary>
    /// 控件工具类
    /// </summary>
    public static class ControlHelper
    {
        #region Graphics

        /// <summary>
        /// 获取指定窗口（包括非工作区）的Graphics
        /// </summary>
        /// <param name="handle">指定窗口的handle</param>
        /// <param name="g">返回g</param>
        /// <param name="hDC">返回hDC</param>
        public static void GetWindowGraphics(IntPtr handle, out Graphics g, out IntPtr hDC)
        {
            hDC = NativeMethods.GetWindowDC(handle);
            g = Graphics.FromHdc(hDC);
        }

        /// <summary>
        /// 获取指定窗口（只包括工作区）的Graphics
        /// </summary>
        /// <param name="handle">指定窗口的handle</param>
        /// <param name="g">返回g</param>
        /// <param name="hDC">返回hDC</param>
        public static void GetWindowClientGraphics(IntPtr handle, out Graphics g, out IntPtr hDC)
        {
            hDC = NativeMethods.GetDC(handle);
            g = Graphics.FromHdc(hDC);
        }

        #endregion

        #region 圆角、边框

        /// <summary>
        /// 绘制Rectangle 1个像素边框（圆角边框半径为3个像素）
        /// </summary>
        /// <param name="control">要绘制边框的控件</param>
        /// <param name="g">要绘制边框的控件画布</param>
        /// <param name="roundCorner">是否为圆角边框（圆角边框半径为3个像素）</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="borderOutOverColor">父控件背景色（圆角边框平滑过度颜色）</param>
        /// <param name="borderInOverColor">控件背景色（圆角边框平滑过度颜色）</param>
        public static void DrawControlBorder(Control control, Graphics g, bool roundCorner, Color borderColor, Color borderOutOverColor, Color borderInOverColor)
        {
            if (borderColor == Color.Empty)
                return;

            Rectangle rect = control.ClientRectangle;

            SmoothingMode smoothingMode = g.SmoothingMode;
            CompositingMode compositingMode = g.CompositingMode;
            CompositingQuality compositingQuality = g.CompositingQuality;
            InterpolationMode interpolationMode = g.InterpolationMode;

            g.SmoothingMode = SmoothingMode.None;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.Default;
            g.InterpolationMode = InterpolationMode.Default;

            if (roundCorner)
            {
                Pen border_pen = new Pen(borderColor);

                //边框
                g.DrawPolygon(border_pen, new Point[] {
                   new Point(rect.Left,rect.Top+1),
                   new Point(rect.Left+1,rect.Top+1),
                   new Point(rect.Left+1,rect.Top),

                   new Point(rect.Right-1-1,rect.Top),
                   new Point(rect.Right-1-1,rect.Top+1),
                   new Point(rect.Right-1,rect.Top+1),

                   new Point(rect.Right-1,rect.Bottom-1-1),
                   new Point(rect.Right-1-1,rect.Bottom-1-1),
                   new Point(rect.Right-1-1,rect.Bottom-1),

                   new Point(rect.Left+1,rect.Bottom-1),
                   new Point(rect.Left+1,rect.Bottom-1-1),
                   new Point(rect.Left,rect.Bottom-1-1),
                 });

                //边框圆角外平滑过度
                border_pen.Color = Color.FromArgb(borderColor.R + (borderOutOverColor.R - borderColor.R) / 4, borderColor.G + (borderOutOverColor.G - borderColor.G) / 4, borderColor.B + (borderOutOverColor.B - borderColor.B) / 4);
                g.DrawLine(border_pen, new Point(rect.Left, rect.Top + 1), new Point(rect.Left + 1, rect.Top));
                g.DrawLine(border_pen, new Point(rect.Right - 2, rect.Top), new Point(rect.Right - 1, rect.Top + 1));
                g.DrawLine(border_pen, new Point(rect.Right - 1, rect.Bottom - 2), new Point(rect.Right - 2, rect.Bottom - 1));
                g.DrawLine(border_pen, new Point(rect.Left + 1, rect.Bottom - 1), new Point(rect.Left, rect.Bottom - 2));

                //边框圆角内平滑过度
                SolidBrush sb = new SolidBrush(Color.FromArgb((borderColor.R + borderInOverColor.R) / 2, (borderColor.G + borderInOverColor.G) / 2, (borderColor.B + borderInOverColor.B) / 2));
                g.FillRectangle(sb, new Rectangle(rect.Left + 1, rect.Top + 1, 1, 1));
                g.FillRectangle(sb, new Rectangle(rect.Right - 2, rect.Top + 1, 1, 1));
                g.FillRectangle(sb, new Rectangle(rect.Right - 2, rect.Bottom - 2, 1, 1));
                g.FillRectangle(sb, new Rectangle(rect.Left + 1, rect.Bottom - 2, 1, 1));

                sb.Dispose();
                border_pen.Dispose();
            }
            else
            {
                Pen border_pen = new Pen(borderColor);
                g.DrawRectangle(border_pen, new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1));
                border_pen.Dispose();
            }

            g.SmoothingMode = smoothingMode;
            g.CompositingMode = compositingMode;
            g.CompositingQuality = compositingQuality;
            g.InterpolationMode = interpolationMode;
        }

        /// <summary>
        /// 计算Rectangle 1个像素圆角形状路径（圆角边框半径为3个像素）
        /// </summary>
        /// <param name="rect">rect</param>
        /// <returns></returns>
        public static GraphicsPath AdjustRectangleShapePath(Rectangle rect)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new Point[] {

            new Point(rect.Left,rect.Top+1),
            new Point(rect.Left+1,rect.Top+1),
            new Point(rect.Left+1,rect.Top),

            new Point(rect.Right-1,rect.Top),
            new Point(rect.Right-1,rect.Top+1),
            new Point(rect.Right,rect.Top+1),

            new Point(rect.Right,rect.Bottom-1),
            new Point(rect.Right-1,rect.Bottom-1),
            new Point(rect.Right-1,rect.Bottom),

            new Point(rect.Left+1,rect.Bottom),
            new Point(rect.Left+1,rect.Bottom-1),
            new Point(rect.Left,rect.Bottom-1),

            });

            return gp;
        }

        /// <summary>
        /// 计算点到矩形的最大圆形最大半径
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static int AdjustRectangleMaxRadiusForPoint(Rectangle rect, Point point)
        {
            int radius = 0;

            radius = Math.Max(radius, (int)Math.Ceiling(Math.Sqrt(Math.Pow(Math.Abs(point.X - rect.X), 2) + Math.Pow(Math.Abs(point.Y - rect.Y), 2))));
            radius = Math.Max(radius, (int)Math.Ceiling(Math.Sqrt(Math.Pow(Math.Abs(point.X - rect.Right), 2) + Math.Pow(Math.Abs(point.Y - rect.Y), 2))));
            radius = Math.Max(radius, (int)Math.Ceiling(Math.Sqrt(Math.Pow(Math.Abs(point.X - rect.Right), 2) + Math.Pow(Math.Abs(point.Y - rect.Bottom), 2))));
            radius = Math.Max(radius, (int)Math.Ceiling(Math.Sqrt(Math.Pow(Math.Abs(point.X - rect.X), 2) + Math.Pow(Math.Abs(point.Y - rect.Bottom), 2))));

            return radius;
        }

        /// <summary>
        /// 转换成圆角
        /// </summary>
        /// <param name="rect">要转换的rect</param>
        /// <param name="radius">圆角半径的大小</param>
        /// <returns></returns>
        public static GraphicsPath AdjustCircularPath(Rectangle rect, int radius)
        {
            return AdjustCircularPath(rect, radius, radius, radius, radius);
        }

        /// <summary>
        /// 转换成圆角
        /// </summary>
        /// <param name="rect">要转换的rect</param>
        /// <param name="leftTopRadius">左上角</param>
        /// <param name="rightTopRadius">右上角</param>
        /// <param name="rightBottomRadius">右下角</param>
        /// <param name="leftBottomRadius">左下角</param>
        /// <returns></returns>
        public static GraphicsPath AdjustCircularPath(Rectangle rect, int leftTopRadius, int rightTopRadius, int rightBottomRadius, int leftBottomRadius)
        {
            leftTopRadius = Math.Abs(leftTopRadius);
            int leftTopDiameter = leftTopRadius * 2;
            rightTopRadius = Math.Abs(rightTopRadius);
            int rightTopDiameter = rightTopRadius * 2;
            rightBottomRadius = Math.Abs(rightBottomRadius);
            int rightBottomDiameter = rightBottomRadius * 2;
            leftBottomRadius = Math.Abs(leftBottomRadius);
            int leftBottomDiameter = leftBottomRadius * 2;

            PointF leftTop_x = new PointF(rect.Left, rect.Top);
            PointF leftTop_y = new PointF(rect.Left, rect.Top);
            if (leftTopRadius > 0)
            {
                leftTop_x = new PointF(rect.Left + leftTopRadius, rect.Top);
                leftTop_y = new PointF(rect.Left, rect.Top + leftTopRadius);
            }

            PointF rightTop_x = new PointF(rect.Right, rect.Top);
            PointF rightTop_y = new PointF(rect.Right, rect.Top);
            if (rightTopRadius > 0)
            {
                rightTop_x = new PointF(rect.Right - rightTopRadius, rect.Top);
                rightTop_y = new PointF(rect.Right, rect.Top + rightTopRadius);
            }

            PointF rightBottom_x = new PointF(rect.Right, rect.Bottom);
            PointF rightBottom_y = new PointF(rect.Right, rect.Bottom);
            if (rightBottomRadius > 0)
            {
                rightBottom_x = new PointF(rect.Right - rightBottomRadius, rect.Bottom);
                rightBottom_y = new PointF(rect.Right, rect.Bottom - rightBottomRadius);
            }

            PointF leftBottom_x = new PointF(rect.Left, rect.Bottom);
            PointF leftBottom_y = new PointF(rect.Left, rect.Bottom);
            if (leftBottomRadius > 0)
            {
                leftBottom_x = new PointF(rect.Left + leftBottomRadius, rect.Bottom);
                leftBottom_y = new PointF(rect.Left, rect.Bottom - leftBottomRadius);
            }

            GraphicsPath gp = new GraphicsPath();

            if (leftTopRadius > 0)
            {
                RectangleF lefttop_rect = new RectangleF(rect.Left, rect.Top, leftTopDiameter, leftTopDiameter);
                gp.AddArc(lefttop_rect, 180, 90);
            }
            gp.AddLine(leftTop_x, rightTop_x);

            if (rightTopRadius > 0)
            {
                RectangleF righttop_rect = new RectangleF(rect.Right - rightTopDiameter, rect.Top, rightTopDiameter, rightTopDiameter);
                gp.AddArc(righttop_rect, 270, 90);
            }
            gp.AddLine(rightTop_y, rightBottom_y);

            if (rightBottomRadius > 0)
            {
                RectangleF rightbottom_rect = new RectangleF(rect.Right - rightBottomDiameter, rect.Bottom - rightBottomDiameter, rightBottomDiameter, rightBottomDiameter);
                gp.AddArc(rightbottom_rect, 0, 90);
            }
            gp.AddLine(rightBottom_x, leftBottom_x);

            if (leftBottomRadius > 0)
            {
                RectangleF leftbottom_rect = new RectangleF(rect.Left, rect.Bottom - leftBottomDiameter, leftBottomDiameter, leftBottomDiameter);
                gp.AddArc(leftbottom_rect, 90, 90);
            }
            gp.AddLine(leftBottom_y, leftTop_y);

            gp.CloseAllFigures();
            return gp;
        }

        /// <summary>
        /// 转换成圆角
        /// </summary>
        /// <param name="rect">要转换的rect</param>
        /// <param name="leftTopRadius">左上角</param>
        /// <param name="rightTopRadius">右上角</param>
        /// <param name="rightBottomRadius">右下角</param>
        /// <param name="leftBottomRadius">左下角</param>
        /// <returns></returns>
        public static Region AdjustCircularRegion(Rectangle rect, int leftTopRadius, int rightTopRadius, int rightBottomRadius, int leftBottomRadius)
        {
            Region r = new Region(rect);
            GraphicsPath gp = new GraphicsPath();

            leftTopRadius = Math.Abs(leftTopRadius);
            int leftTopDiameter = leftTopRadius * 2;
            rightTopRadius = Math.Abs(rightTopRadius);
            int rightTopDiameter = rightTopRadius * 2;
            rightBottomRadius = Math.Abs(rightBottomRadius);
            int rightBottomDiameter = rightBottomRadius * 2;
            leftBottomRadius = Math.Abs(leftBottomRadius);
            int leftBottomDiameter = leftBottomRadius * 2;

            if (leftTopRadius > 0)
            {
                Region lefttop_r = new Region(new Rectangle(rect.Left, rect.Top, leftTopRadius, leftTopRadius));
                gp.Reset();
                gp.AddPie(new Rectangle(rect.Left, rect.Top, leftTopDiameter, leftTopDiameter), 180, 90);
                lefttop_r.Exclude(gp);
                r.Exclude(lefttop_r);
                lefttop_r.Dispose();
            }

            if (rightTopRadius > 0)
            {
                Region righttop_r = new Region(new Rectangle(rect.Right - rightTopRadius, rect.Top, rightTopRadius, rightTopRadius));
                gp.Reset();
                gp.AddPie(new Rectangle(rect.Right - rightTopDiameter, rect.Top, rightTopDiameter, rightTopDiameter), 270, 90);
                righttop_r.Exclude(gp);
                r.Exclude(righttop_r);
                righttop_r.Dispose();
            }

            if (rightBottomRadius > 0)
            {
                Region rightbottom_r = new Region(new Rectangle(rect.Right - rightBottomRadius, rect.Bottom - rightBottomRadius, rightBottomRadius, rightBottomRadius));
                gp.Reset();
                gp.AddPie(new Rectangle(rect.Right - rightBottomDiameter, rect.Bottom - rightBottomDiameter, rightBottomDiameter, rightBottomDiameter), 0, 90);
                rightbottom_r.Exclude(gp);
                r.Exclude(rightbottom_r);
                rightbottom_r.Dispose();
            }

            if (leftBottomRadius > 0)
            {
                Region leftbottom_r = new Region(new Rectangle(rect.Left, rect.Bottom - leftBottomRadius, leftBottomRadius, leftBottomRadius));
                gp.Reset();
                gp.AddPie(new Rectangle(rect.Left, rect.Bottom - leftBottomDiameter, leftBottomDiameter, leftBottomDiameter), 90, 90);
                leftbottom_r.Exclude(gp);
                r.Exclude(leftbottom_r);
                leftbottom_r.Dispose();
            }

            gp.Dispose();
            return r;
        }

        /// <summary>
        /// 根据画笔大小计算出真是rectf
        /// </summary>
        /// <param name="rectf">要转换的rectf</param>
        /// <param name="pen">画笔大小大小</param>
        /// <param name="alignment">画笔对齐方式</param>
        /// <returns></returns>
        public static RectangleF TransformRectangleF(RectangleF rectf, float pen, PenAlignment alignment = PenAlignment.Center)
        {
            if (pen <= 0)
            {
                return rectf;
            }

            RectangleF result = new RectangleF();
            if (alignment == PenAlignment.Center || alignment == PenAlignment.Left || alignment == PenAlignment.Right || alignment == PenAlignment.Outset)
            {
                result.Width = rectf.Width - pen;
                result.Height = rectf.Height - pen;
                result.X = rectf.X + ((int)pen) / 2;
                result.Y = rectf.Y + ((int)pen) / 2;
            }
            else if (alignment == PenAlignment.Inset)
            {

                if (pen > 0 && pen < 2)
                {
                    result.Width = rectf.Width - 1;
                    result.Height = rectf.Height - 1;
                }
                else
                {
                    result.Width = rectf.Width;
                    result.Height = rectf.Height;
                }

                result.X = rectf.X;
                result.Y = rectf.Y;
            }

            return result;
        }

        /// <summary>
        /// 根据画笔大小计算出真是rectf
        /// </summary>
        /// <param name="rect">要转换的rect</param>
        /// <param name="pen">画笔大小大小</param>
        /// <param name="alignment">画笔对齐方式</param>
        /// <returns></returns>
        public static Rectangle TransformRectangle(Rectangle rect, int pen, PenAlignment alignment = PenAlignment.Center)
        {
            if (pen <= 0)
            {
                return rect;
            }

            Rectangle result = new Rectangle();
            if (alignment == PenAlignment.Center || alignment == PenAlignment.Left || alignment == PenAlignment.Right || alignment == PenAlignment.Outset)
            {
                result.Width = rect.Width - pen;
                result.Height = rect.Height - pen;
                result.X = rect.X + ((int)pen) / 2;
                result.Y = rect.Y + ((int)pen) / 2;
            }
            else if (alignment == PenAlignment.Inset)
            {

                if (pen > 0 && pen < 2)
                {
                    result.Width = rect.Width - 1;
                    result.Height = rect.Height - 1;
                }
                else
                {
                    result.Width = rect.Width;
                    result.Height = rect.Height;
                }

                result.X = rect.X;
                result.Y = rect.Y;
            }

            return result;
        }

        #endregion

        #region 字体

        private static object getTextMetrics_object = new object();
        /// <summary>
        /// 通过字体所在控件获取字体真实高度（FontHeight = tmHeight + tmExternalLeading + tmInternalLeading）
        /// </summary>
        /// <param name="hWnd">控件</param>
        /// <param name="font">字体</param>
        /// <returns></returns>
        public static NativeMethods.TEXTMETRIC GetFontMetrics(IntPtr hWnd, Font font)
        {
            lock (getTextMetrics_object)
            {
                IntPtr hdc = NativeMethods.GetDC(hWnd);

                IntPtr hFont = font.ToHfont();
                IntPtr hOldFont = NativeMethods.SelectObject(hdc, hFont);

                NativeMethods.TEXTMETRIC tm = new NativeMethods.TEXTMETRIC();
                NativeMethods.GetTextMetrics(hdc, ref tm);

                NativeMethods.SelectObject(hdc, hOldFont);
                NativeMethods.ReleaseDC(hWnd, hdc);
                // 释放字体句柄
                NativeMethods.DeleteObject(hFont);
                return tm;
            }
        }

        #endregion

        #region  (Size、Point)角度

        /// <summary>
        /// 计算指定角度的坐标
        /// </summary>
        /// <param name="center">圆心坐标</param>
        /// <param name="radius">圆半径</param>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static PointF CalculatePointForAngle(PointF center, float radius, float angle)
        {
            if (radius == 0)
                return center;

            float w;
            float h;
            if (angle <= 90)
            {
                w = radius * (float)Math.Cos(Math.PI / 180 * angle);
                h = radius * (float)Math.Sin(Math.PI / 180 * angle);
            }
            else if (angle <= 180)
            {
                w = -radius * (float)Math.Sin(Math.PI / 180 * (angle - 90));
                h = radius * (float)Math.Cos(Math.PI / 180 * (angle - 90));

            }
            else if (angle <= 270)
            {
                w = -radius * (float)Math.Cos(Math.PI / 180 * (angle - 180));
                h = -radius * (float)Math.Sin(Math.PI / 180 * (angle - 180));
            }
            else
            {
                w = radius * (float)Math.Sin(Math.PI / 180 * (angle - 270));
                h = -radius * (float)Math.Cos(Math.PI / 180 * (angle - 270));

            }
            return new PointF(center.X + w, center.Y + h);
        }

        /// <summary>
        /// 计算两个坐标的夹角
        /// </summary>
        /// <param name="point1">坐标1</param>
        /// <param name="point2">坐标2</param>
        /// <returns></returns>
        public static double CalculateAngleForPoint(PointF point1, PointF point2)
        {
            double tmp = Math.Atan2((point2.Y - point1.Y), (point2.X - point1.X)) * 180 / Math.PI;
            if (tmp < 0)
            {
                tmp = 180 + 180 - Math.Abs(tmp);
            }
            return tmp;
        }

        /// <summary>
        /// 获取Size旋转指定角度后新的Size
        /// </summary>
        /// <param name="size">源Size</param>
        /// <param name="angle">指定角度</param>
        /// <returns>新的Size</returns>
        public static SizeF ConvertToAngleSize(SizeF size, float angle)
        {
            PointF angle_point = ControlHelper.CalculatePointForAngle(new PointF(0, 0), size.Width / 2, (angle + 360) % 360);
            PointF righttop_point = ControlHelper.CalculatePointForAngle(angle_point, size.Height / 2, (angle - 90 + 360) % 360);
            PointF rightbottom_point = ControlHelper.CalculatePointForAngle(angle_point, size.Height / 2, (angle + 90 + 360) % 360);

            float w = Math.Max(Math.Abs(righttop_point.X), Math.Abs(rightbottom_point.X)) * 2;
            float h = Math.Max(Math.Abs(righttop_point.Y), Math.Abs(rightbottom_point.Y)) * 2;

            return new SizeF(w, h);
        }

        #endregion

        #region 画笔Rectangle转换

        /// <summary>
        /// 根据画笔大小转换rectf
        /// </summary>
        /// <param name="rectf">要转换的rectf</param>
        /// <param name="pen">画笔大小大小</param>
        /// <returns></returns>
        public static RectangleF TransformRectangleByPen(RectangleF rectf, float pen)
        {
            RectangleF result = new RectangleF();
            result.Width = rectf.Width - (pen < 1 ? 0 : pen);
            result.Height = rectf.Height - (pen < 1 ? 0 : pen);
            result.X = rectf.X + (float)(pen / 2);
            result.Y = rectf.Y + (float)(pen / 2);
            return result;
        }

        #endregion

        #region 颜色

        /// <summary>
        /// 转换成灰色颜色值
        /// </summary>
        /// <param name="color">要转换的颜色</param>
        /// <returns></returns>
        public static Color ConvertToDisableColor(Color color)
        {
            int tmp = (color.R + color.G + color.B) / 3;
            if (Math.Abs(tmp - color.R) <= 1 && Math.Abs(tmp - color.G) <= 1 && Math.Abs(tmp - color.B) <= 1)
                tmp -= 10;
            if (tmp < 0)
                tmp += Math.Abs(tmp * 2);
            return Color.FromArgb(color.A, tmp, tmp, tmp);
        }

        /// <summary>
        /// 根据控件Enabled状态转换成对应颜色值
        /// </summary>
        /// <param name="control">颜色所属的控件</param>
        /// <param name="color">要转换的颜色</param>
        /// <returns></returns>
        public static Color ConvertToAutoColor(Control control, Color color)
        {
            if (control == null || control.Enabled)
            {
                return color;
            }
            return ConvertToDisableColor(color);
        }

        /// <summary>
        /// 根据状态转换成对应颜色值
        /// </summary>
        /// <param name="disable">是否为禁用状态</param>
        /// <param name="color">要转换的颜色</param>
        /// <returns></returns>
        public static Color ConvertToAutoColor(bool disable, Color color)
        {
            if (!disable)
            {
                return color;
            }
            return ConvertToDisableColor(color);
        }

        /// <summary>
        /// 转换成动画效果颜色值
        /// </summary>
        /// <param name="color">要转换的颜色</param>
        /// <returns></returns>
        public static Color ConvertToAnimationColor(Color color)
        {
            if (color.GetBrightness() >= 0.5)
            {
                Color tmp = ControlPaint.Dark(color);
                return Color.FromArgb(15, tmp.R, tmp.G, tmp.B);
            }
            else
            {
                Color tmp = ControlPaint.Light(color);
                return Color.FromArgb(80, tmp.R, tmp.G, tmp.B);
            }
        }

        /// <summary>
        /// 根据控件Enabled状态和背景色转换文本字体对应颜色值
        /// </summary>
        /// <param name="control">颜色所属的控件</param>
        /// <param name="backColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <returns></returns>
        public static Color ConvertToForeColorByBackColor(Control control, Color backColor, Color foreColor)
        {
            if (control == null || control.Enabled)
            {
                return foreColor;
            }

            Color tmp = ConvertToAutoColor(control, backColor);
            if (Math.Abs(tmp.GetBrightness() - foreColor.GetBrightness()) < 0.2)
            {
                return tmp.GetBrightness() >= 0.5 ? ControlPaint.Dark(backColor) : ControlPaint.Light(backColor);
            }
            else
            {
                return ConvertToDisableColor(foreColor);
            }

        }

        /// <summary>
        /// 根据状态和背景色转换文本字体对应颜色值
        /// </summary>
        /// <param name="disable">是否为禁用状态</param>
        /// <param name="backColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <returns></returns>
        public static Color ConvertToForeColorByBackColor(bool disable, Color backColor, Color foreColor)
        {
            if (!disable)
            {
                return foreColor;
            }

            Color tmp = ConvertToAutoColor(disable, backColor);
            if (Math.Abs(tmp.GetBrightness() - foreColor.GetBrightness()) < 0.2)
            {
                return tmp.GetBrightness() >= 0.5 ? ControlPaint.Dark(backColor) : ControlPaint.Light(backColor);
            }
            else
            {
                return ConvertToDisableColor(foreColor);
            }

        }

        /// <summary>
        /// 根据指定颜色Brightness转换成有对比颜色
        /// </summary>
        /// <param name="color">指定颜色</param>
        /// <returns></returns>
        public static Color ConvertToDarkOrLightColor(Color color)
        {
            return ((double)color.GetBrightness() >= 0.5) ? ControlPaint.Dark(color) : ControlPaint.Light(color);
        }

        /// <summary>
        /// 检查RGB值ed有效范围
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static int VerifyRGB(int rgb)
        {
            if (rgb < 0)
                return 0;
            if (rgb > 255)
                return 255;
            return rgb;
        }

        #endregion

        #region 图片

        private static ColorMatrix disabledImageColorMatrix;
        /// <summary>
        /// 用于创建禁用状态灰色图片（NET源码拷贝）
        /// </summary>
        private static ColorMatrix DisabledImageColorMatrix
        {
            get
            {
                if (disabledImageColorMatrix == null)
                {
                    float[][] greyscale = new float[5][];
                    greyscale[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
                    greyscale[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0, 0 };
                    greyscale[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
                    greyscale[3] = new float[5] { 0, 0, 0, 1, 0 };
                    greyscale[4] = new float[5] { 0.38f, 0.38f, 0.38f, 0, 1 };

                    float[][] transparency = new float[5][];
                    transparency[0] = new float[5] { 1, 0, 0, 0, 0 };
                    transparency[1] = new float[5] { 0, 1, 0, 0, 0 };
                    transparency[2] = new float[5] { 0, 0, 1, 0, 0 };
                    transparency[3] = new float[5] { 0, 0, 0, .7F, 0 };
                    transparency[4] = new float[5] { 0, 0, 0, 0, 0 };


                    int size = 5;
                    float[][] result = new float[size][];
                    for (int row = 0; row < size; row++)
                    {
                        result[row] = new float[size];
                    }

                    float[] column = new float[size];
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            column[k] = transparency[k][j];
                        }
                        for (int i = 0; i < size; i++)
                        {
                            float[] row = greyscale[i];
                            float s = 0;
                            for (int k = 0; k < size; k++)
                            {
                                s += row[k] * column[k];
                            }
                            result[i][j] = s;
                        }
                    }

                    disabledImageColorMatrix = new ColorMatrix(result);

                }

                return disabledImageColorMatrix;
            }
        }

        /// <summary>
        /// 创建禁用状态灰色图片（NET源码拷贝）
        /// </summary>
        /// <param name="normalImage">要处理的图片</param>
        /// <returns></returns>
        public static Image CreateDisabledImage(Image normalImage)
        {
            ImageAttributes imgAttrib = new ImageAttributes();
            imgAttrib.ClearColorKey();
            imgAttrib.SetColorMatrix(DisabledImageColorMatrix);

            Size size = normalImage.Size;
            Bitmap disabledBitmap = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(disabledBitmap);

            graphics.DrawImage(normalImage, new Rectangle(0, 0, size.Width, size.Height), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, imgAttrib);
            graphics.Dispose();

            return disabledBitmap;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="sourceImage">源图片</param>
        /// <param name="targetSize">目标Size</param>
        /// <returns></returns>
        public static Image ScaleImage(Image sourceImage, Size targetSize)
        {
            if (sourceImage == null)
                return null;

            Bitmap bmp = new Bitmap(targetSize.Width, targetSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(sourceImage, new Rectangle(0, 0, targetSize.Width, targetSize.Height), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel);

            g.Dispose();

            return bmp;
        }

        #endregion

        #region 小数位数截取

        /// <summary>
        /// 保留指定位数小数
        /// </summary>
        /// <param name="value">要处理数</param>
        /// <param name="places">要保留小数位数</param>
        public static float ReserveDecimals(float value, int places)
        {
            return (float)ReserveDecimals(value, places);
        }

        /// <summary>
        /// 保留指定位数小数
        /// </summary>
        /// <param name="value">要处理数</param>
        /// <param name="places">要保留小数位数</param>
        public static double ReserveDecimals(double value, int places)
        {
            return (double)ReserveDecimals(value, places);
        }

        /// <summary>
        /// 保留指定位数小数
        /// </summary>
        /// <param name="value">要处理数</param>
        /// <param name="places">要保留小数位数</param>
        public static string ReserveDecimals(string value, int places)
        {
            if (places < 0)
                return value;

            value = value.Trim();
            int index = value.IndexOf('.');

            if (places == 0 && index > -1)
            {
                return value.Substring(0, index);
            }

            if (index > -1)
            {
                value = value.Substring(0, Math.Min(index + places + 1, value.Length));
                value = value.PadRight(value.IndexOf('.') + places + 1, '0');
            }
            else if (places > 0)
            {
                value = (value + ".").PadRight(value.Length + 1 + places, '0');
            }

            return value;
        }

        #endregion

    }

}
