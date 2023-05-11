/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;

namespace RS.UtilityLib.SizeOfGenericT.BGRA
{
    internal class Utility
    {
        #region Clamp
        public static double Clamp(double x, double min, double max) {
            if (x < min) {
                return min;
            }
            else if (x > max) {
                return max;
            }
            else {
                return x;
            }
        }

        public static float Clamp(float x, float min, float max) {
            if (x < min) {
                return min;
            }
            else if (x > max) {
                return max;
            }
            else {
                return x;
            }
        }

        public static int Clamp(int x, int min, int max) {
            if (x < min) {
                return min;
            }
            else if (x > max) {
                return max;
            }
            else {
                return x;
            }
        }

        public static byte ClampToByte(double x) {
            if (x > 255) {
                return 255;
            }
            else if (x < 0) {
                return 0;
            }
            else {
                return (byte)x;
            }
        }

        public static byte ClampToByte(float x) {
            if (x > 255) {
                return 255;
            }
            else if (x < 0) {
                return 0;
            }
            else {
                return (byte)x;
            }
        }

        public static byte ClampToByte(int x) {
            if (x > 255) {
                return 255;
            }
            else if (x < 0) {
                return 0;
            }
            else {
                return (byte)x;
            }
        }
        #endregion

        public static int FastScaleByteByByte(byte a, byte b) {
            int r1 = a * b + 0x80;
            int r2 = ((r1 >> 8) + r1) >> 8;
            return (byte)r2;
        }

        #region Lerp
        public static float Lerp(float from, float to, float frac) {
            return (from + frac * (to - from));
        }

        public static double Lerp(double from, double to, double frac) {
            return (from + frac * (to - from));
        }

        public static PointF Lerp(PointF from, PointF to, float frac) {
            return new PointF(Lerp(from.X, to.X, frac), Lerp(from.Y, to.Y, frac));
        }
        #endregion

        public static void GCFullCollect() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #region stream
        /// <summary>
        /// 从流中读取数据到buffer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int ReadFromStream(Stream input, byte[] buffer, int offset, int count) {
            int totalBytesRead = 0;

            while (totalBytesRead < count) {
                int bytesRead = input.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (bytesRead == 0) {
                    throw new IOException("ran out of data");
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        public static long CopyStream(Stream input, Stream output, long maxBytes) {
            long bytesCopied = 0;
            byte[] buffer = new byte[4096];

            while (true) {
                int bytesRead = input.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0) {
                    break;
                }
                else {
                    int bytesToCopy;

                    if (maxBytes != -1 && (bytesCopied + bytesRead) > maxBytes) {
                        bytesToCopy = (int)(maxBytes - bytesCopied);
                    }
                    else {
                        bytesToCopy = bytesRead;
                    }

                    output.Write(buffer, 0, bytesRead);
                    bytesCopied += bytesToCopy;

                    if (bytesToCopy != bytesRead) {
                        break;
                    }
                }
            }

            return bytesCopied;
        }

        public static long CopyStream(Stream input, Stream output) {
            return CopyStream(input, output, -1);
        }
        #endregion

        #region io
        /// <summary>
        /// Reads a 16-bit unsigned integer from a Stream in little-endian format.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>-1 on failure, else the 16-bit unsigned integer that was read.</returns>
        public static int ReadUInt16(Stream stream) {
            int byte1 = stream.ReadByte();

            if (byte1 == -1) {
                return -1;
            }

            int byte2 = stream.ReadByte();

            if (byte2 == -1) {
                return -1;
            }

            return byte1 + (byte2 << 8);
        }

        public static void WriteUInt16(Stream stream, UInt16 word) {
            stream.WriteByte((byte)(word & 0xff));
            stream.WriteByte((byte)(word >> 8));
        }

        public static void WriteUInt24(Stream stream, int uint24) {
            stream.WriteByte((byte)(uint24 & 0xff));
            stream.WriteByte((byte)((uint24 >> 8) & 0xff));
            stream.WriteByte((byte)((uint24 >> 16) & 0xff));
        }

        public static void WriteUInt32(Stream stream, UInt32 uint32) {
            stream.WriteByte((byte)(uint32 & 0xff));
            stream.WriteByte((byte)((uint32 >> 8) & 0xff));
            stream.WriteByte((byte)((uint32 >> 16) & 0xff));
            stream.WriteByte((byte)((uint32 >> 24) & 0xff));
        }

        /// <summary>
        /// Reads a 24-bit unsigned integer from a Stream in little-endian format.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>-1 on failure, else the 24-bit unsigned integer that was read.</returns>
        public static int ReadUInt24(Stream stream) {
            int byte1 = stream.ReadByte();

            if (byte1 == -1) {
                return -1;
            }

            int byte2 = stream.ReadByte();

            if (byte2 == -1) {
                return -1;
            }

            int byte3 = stream.ReadByte();

            if (byte3 == -1) {
                return -1;
            }

            return byte1 + (byte2 << 8) + (byte3 << 16);
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from a Stream in little-endian format.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>-1 on failure, else the 32-bit unsigned integer that was read.</returns>
        public static long ReadUInt32(Stream stream) {
            int byte1 = stream.ReadByte();

            if (byte1 == -1) {
                return -1;
            }

            int byte2 = stream.ReadByte();

            if (byte2 == -1) {
                return -1;
            }

            int byte3 = stream.ReadByte();

            if (byte3 == -1) {
                return -1;
            }

            int byte4 = stream.ReadByte();

            if (byte4 == -1) {
                return -1;
            }

            return unchecked((long)((uint)(byte1 + (byte2 << 8) + (byte3 << 16) + (byte4 << 24))));
        }
        #endregion

        #region swap
        public static void Swap(ref int a, ref int b) {
            int t;

            t = a;
            a = b;
            b = t;
        }

        public static void Swap<T>(ref T a, ref T b) {
            T t;

            t = a;
            a = b;
            b = t;
        }
        #endregion

    }
}