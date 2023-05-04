using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace RS.UtilityLib.WinFormCommon
{
    public class BitmapToRegion
    {
        public enum TransparencyMode
        {
            ColorKeyOpaque = 2,
            ColorKeyTransparent = 1
        }
        public static Region Convert(Bitmap bitmap, Color transparencyKey) {
            return Convert(bitmap, transparencyKey, TransparencyMode.ColorKeyTransparent);
        }

        public static unsafe Region Convert(Bitmap bitmap, Color transparencyKey, TransparencyMode mode) {
            if (bitmap == null) {
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");
            }
            bool flag = mode == TransparencyMode.ColorKeyOpaque;
            GraphicsUnit pixel = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref pixel);
            Rectangle rect = new Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
            uint num = (uint)((((transparencyKey.A << 0x18) | (transparencyKey.R << 0x10)) | (transparencyKey.G << 8)) | transparencyKey.B);
            BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            uint* numPtr = (uint*)bitmapdata.Scan0.ToPointer();
            int height = (int)bounds.Height;
            int width = (int)bounds.Width;
            GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < height; i++) {
                byte* numPtr2 = (byte*)numPtr;
                int num5 = 0;
                while (num5 < width) {
                    if (!(flag ^ (numPtr[0] == num))) {
                        int x = num5;
                        while ((num5 < width) && !(flag ^ (numPtr[0] == num))) {
                            num5++;
                            numPtr++;
                        }
                        path.AddRectangle(new Rectangle(x, i, num5 - x, 1));
                    }
                    num5++;
                    numPtr++;
                }
                numPtr = (uint*)(numPtr2 + bitmapdata.Stride);
            }
            Region region = new Region(path);
            path.Dispose();
            bitmap.UnlockBits(bitmapdata);
            return region;
        }

        public static unsafe Region Convert(Bitmap bitmap, Color transparencyKey, TransparencyMode mode, int orignX, int orignY) {
            if (bitmap == null) {
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");
            }
            bool flag = mode == TransparencyMode.ColorKeyOpaque;
            GraphicsUnit pixel = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref pixel);
            Rectangle rect = new Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
            uint num = (uint)((((transparencyKey.A << 0x18) | (transparencyKey.R << 0x10)) | (transparencyKey.G << 8)) | transparencyKey.B);
            BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            uint* numPtr = (uint*)bitmapdata.Scan0.ToPointer();
            int height = (int)bounds.Height;
            int width = (int)bounds.Width;
            GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < height; i++) {
                byte* numPtr2 = (byte*)numPtr;
                int num5 = 0;
                while (num5 < width) {
                    if (!(flag ^ (numPtr[0] == num))) {
                        int num6 = num5;
                        while ((num5 < width) && !(flag ^ (numPtr[0] == num))) {
                            num5++;
                            numPtr++;
                        }
                        path.AddRectangle(new Rectangle(num6 + orignX, i + orignY, num5 - num6, 1));
                    }
                    num5++;
                    numPtr++;
                }
                numPtr = (uint*)(numPtr2 + bitmapdata.Stride);
            }
            Region region = new Region(path);
            path.Dispose();
            bitmap.UnlockBits(bitmapdata);
            return region;
        }
    }


    class BitmapToRegion2
    {
        //This method uses only safe code (no pointers) to scan an imagine
        //and create a region from it
        public static Region getRegion(Bitmap inputBmp, Color transperancyKey, int tolerance) {
            //Stores all the rectangles for the region
            GraphicsPath path = new GraphicsPath();

            //Scan the image
            for (int x = 0; x < inputBmp.Width; x++) {
                for (int y = 0; y < inputBmp.Height; y++) {
                    if (!colorsMatch(inputBmp.GetPixel(x, y), transperancyKey, tolerance))
                        path.AddRectangle(new Rectangle(x, y, 1, 1));
                }
            }

            //Create the Region
            Region outputRegion = new Region(path);

            //Clean up
            path.Dispose();

            //Finish
            return outputRegion;
        }

        private static bool colorsMatch(Color color1, Color color2, int tolerance) {
            if (tolerance < 0) tolerance = 0;
            return Math.Abs(color1.R - color2.R) <= tolerance &&
                   Math.Abs(color1.G - color2.G) <= tolerance &&
                   Math.Abs(color1.B - color2.B) <= tolerance;
        }

        private unsafe static bool colorsMatch(uint* pixelPtr, Color color1, int tolerance) {
            if (tolerance < 0) tolerance = 0;

            //Convert the pixel pointer into a color
            byte a = (byte)(*pixelPtr >> 24);
            byte r = (byte)(*pixelPtr >> 16);
            byte g = (byte)(*pixelPtr >> 8);
            byte b = (byte)(*pixelPtr >> 0);
            Color pointer = Color.FromArgb(a, r, g, b);

            //Each value between the two colors cannot differ more than tolerance
            return Math.Abs(color1.A - pointer.A) <= tolerance &&
                   Math.Abs(color1.R - pointer.R) <= tolerance &&
                   Math.Abs(color1.G - pointer.G) <= tolerance &&
                   Math.Abs(color1.B - pointer.B) <= tolerance;
        }

        //Uses pointers to scan through the bitmap a LOT faster
        //Make sure to check "Allow unsafe code" in the project properties      
        public unsafe static Region getRegionFast(Bitmap bitmap, Color transparencyKey, int tolerance) {
            //Bounds
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.Left, (int)boundsF.Top,
                                             (int)boundsF.Width, (int)boundsF.Height);

            int yMax = (int)boundsF.Height;
            int xMax = (int)boundsF.Width;

            //Transparency Color
            if (tolerance <= 0) tolerance = 1;


            //Lock Image
            BitmapData bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            uint* pixelPtr = (uint*)bitmapData.Scan0.ToPointer();

            //Stores all the rectangles for the region
            GraphicsPath path = new GraphicsPath();

            //Scan the image, looking for lines that are NOT the transperancy color
            for (int y = 0; y < yMax; y++) {
                byte* basePos = (byte*)pixelPtr;

                for (int x = 0; x < xMax; x++, pixelPtr++) {
                    //Go on with the loop if its transperancy color

                    if (colorsMatch(pixelPtr, transparencyKey, tolerance))
                        continue;

                    //Line start
                    int x0 = x;

                    //Find the next transparency colored pixel
                    while (x < xMax && !colorsMatch(pixelPtr, transparencyKey, tolerance)) {
                        x++;
                        pixelPtr++;
                    }

                    //Add the line as a rectangle
                    path.AddRectangle(new Rectangle(x0, y, x - x0, 1));
                }

                //Go to next line
                pixelPtr = (uint*)(basePos + bitmapData.Stride);
            }

            //Create the Region
            Region outputRegion = new Region(path);

            //Clean Up
            path.Dispose();
            bitmap.UnlockBits(bitmapData);

            return outputRegion;
        }
    }
}
