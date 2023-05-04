using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace RS.UtilityLib.WinFormCommon
{
    /// <summary>
    /// 绘制边界半透明层
    /// </summary>
    public static class DrawRoundRectangleHelper
    {

        //
        public static void RenderSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
            System.Drawing.Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            System.Drawing.Color[] colorArray2;

            if (rectangle.Height <= 0) {
                rectangle.Height = 1;
            }
            if (rectangle.Width <= 0) {
                rectangle.Width = 1;
            }

            if (pressed) {
                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0xfe, 0xd8, 170), System.Drawing.Color.FromArgb(70, 0xfb, 0xb5, 0x65), System.Drawing.Color.FromArgb(70, 250, 0x9d, 0x34), System.Drawing.Color.FromArgb(70, 0xfd, 0xee, 170) };

                //colorArray = colorArray2;
                //numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                Color c1 = GetColor(Color.Green, 1);

                //颜色
                colorArray2 = new System.Drawing.Color[] {
                    System.Drawing.Color.FromArgb(90, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(05, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(15, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(60, c1.R, c1.G, c1.B) };

                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0, 0, 0), System.Drawing.Color.FromArgb(70, 10, 10, 10), System.Drawing.Color.FromArgb(70, 20, 20, 20), System.Drawing.Color.FromArgb(70, 50, 50, 50) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.85f, 1.0f }; //调整对应颜色的位置
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(System.Drawing.Color.FromArgb(0xab, 0xa1, 140))) {
                    DrawRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                //DrawRoundRectangle(g, new Pen(new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xdf, 0xb7, 0x88), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)), rectangle, radius);
            }
            else {
                Color c1 = GetColor(Color.White, 1);
                colorArray2 = new System.Drawing.Color[] {
                    System.Drawing.Color.FromArgb(90, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(05, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(15, c1.R, c1.G, c1.B),
                    System.Drawing.Color.FromArgb(60, c1.R, c1.G, c1.B) };

                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0, 0, 0), System.Drawing.Color.FromArgb(70, 10, 10, 10), System.Drawing.Color.FromArgb(70, 20, 20, 20), System.Drawing.Color.FromArgb(70, 50, 50, 50) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.35f, 0.8f, 1.0f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p = new Pen(System.Drawing.Color.FromArgb(210, 0xc0, 0x8d))) {
                    DrawRoundRectangle(g, p, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;

                using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle,
                    Color.FromArgb(0xff, 0xff, 0xf7),
                    Color.Transparent, LinearGradientMode.ForwardDiagonal)) {

                    using (Pen p2 = new Pen(brush2)) {
                        DrawRoundRectangle(g, p2, rectangle, 2f);
                    }
                }
            }
            brush.Dispose();

        }
        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
            path.CloseFigure();
            g.FillPath(brush, path);
            path.Dispose();
        }
        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
            path.CloseFigure();
            g.DrawPath(pen, path);
            path.Dispose();
        }
        //tab 
        public static void RenderTopSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
            System.Drawing.Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            System.Drawing.Color[] colorArray2;
            if (pressed) {
                colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(0xfe, 0xd8, 170), System.Drawing.Color.FromArgb(0xfb, 0xb5, 0x65), System.Drawing.Color.FromArgb(250, 0x9d, 0x34), System.Drawing.Color.FromArgb(0xfd, 0xee, 170) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillTopRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p1 = new Pen(System.Drawing.Color.FromArgb(0xab, 0xa1, 140))) {
                    DrawTopRoundRectangle(g, p1, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                using (LinearGradientBrush lgb1 = new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xdf, 0xb7, 0x88), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                    using (Pen p1 = new Pen(lgb1)) {
                        DrawTopRoundRectangle(g, p1, rectangle, radius);
                    }
                }
            }
            else {
                colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(0xff, 0xfe, 0xe3), System.Drawing.Color.FromArgb(0xff, 0xe7, 0x97), System.Drawing.Color.FromArgb(0xff, 0xd7, 80), System.Drawing.Color.FromArgb(0xff, 0xe7, 150) };
                colorArray = colorArray2;
                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
                blend = new ColorBlend();
                blend.Colors = colorArray;
                blend.Positions = numArray;
                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
                brush.InterpolationColors = blend;
                FillTopRoundRectangle(g, brush, rectangle, 2f);
                using (Pen p1 = new Pen(System.Drawing.Color.FromArgb(210, 0xc0, 0x8d))) {
                    DrawTopRoundRectangle(g, p1, rectangle, radius);
                }
                rectangle.Offset(1, 1);
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                using (LinearGradientBrush lgb1 = new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xff, 0xff, 0xf7), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                    using (Pen p1 = new Pen(lgb1)) {
                        DrawTopRoundRectangle(g, p1, rectangle, 2f);
                    }
                }
            }
            brush.Dispose();
        }
        public static void FillTopRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
            path.CloseFigure();
            g.FillPath(brush, path);
            path.Dispose();
        }

        public static void DrawTopRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
            float width = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
            path.CloseFigure();
            g.DrawPath(pen, path);
            path.Dispose();

        }


        public static System.Drawing.Color GetColor(Color c, double luminance) {
            System.Drawing.Color color = HSL2RGB(((double)c.GetHue()) / 360.0, ((double)c.GetSaturation()) / 2.0, c.GetBrightness() * 1.025);
            double num = color.R * luminance;
            double num2 = color.G * luminance;
            double num3 = color.B * luminance;
            if (num > 255.0) {
                num = 255.0;
            }
            if (num2 > 255.0) {
                num2 = 255.0;
            }
            if (num3 > 255.0) {
                num3 = 255.0;
            }
            return System.Drawing.Color.FromArgb((int)num, (int)num2, (int)num3);
        }
        public static System.Drawing.Color HSL2RGB(double hue, double saturation, double luminance) {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            if (luminance == 0.0) {
                num = num2 = num3 = 0.0;
            }
            else if (saturation == 0.0) {
                num = num2 = num3 = luminance;
            }
            else {
                double num4 = (luminance <= 0.5) ? (luminance * (1.0 + saturation)) : ((luminance + saturation) - (luminance * saturation));
                double num5 = (2.0 * luminance) - num4;
                double[] numArray = new double[] { hue + 0.33333333333333331, hue, hue - 0.33333333333333331 };
                double[] numArray2 = new double[3];
                for (int i = 0; i < 3; i++) {
                    if (numArray[i] < 0.0) {
                        numArray[i]++;
                    }
                    if (numArray[i] > 1.0) {
                        numArray[i]--;
                    }
                    if ((6.0 * numArray[i]) < 1.0) {
                        numArray2[i] = num5 + (((num4 - num5) * numArray[i]) * 6.0);
                    }
                    else if ((2.0 * numArray[i]) < 1.0) {
                        numArray2[i] = num4;
                    }
                    else if ((3.0 * numArray[i]) < 2.0) {
                        numArray2[i] = num5 + (((num4 - num5) * (0.66666666666666663 - numArray[i])) * 6.0);
                    }
                    else {
                        numArray2[i] = num5;
                    }
                }
                num = numArray2[0];
                num2 = numArray2[1];
                num3 = numArray2[2];
            }
            if (num > 1.0) {
                num = 1.0;
            }
            if (num2 > 1.0) {
                num2 = 1.0;
            }
            if (num3 > 1.0) {
                num3 = 1.0;
            }
            return System.Drawing.Color.FromArgb((int)(255.0 * num), (int)(255.0 * num2), (int)(255.0 * num3));
        }
    }
}
