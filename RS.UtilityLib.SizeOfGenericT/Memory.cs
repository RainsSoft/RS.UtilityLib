﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Mono.Unix;
using Mono.Unix.Native;

namespace RS.UtilityLib.SizeOfGenericT
{
    /// <summary>
    /// 包含用于对固定（固定）在内存中的内存进行分配、释放和执行操作的方法。
    /// Contains methods for allocating, freeing, and performing operations on memory 
    /// that is fixed (pinned) in memory.
    /// </summary>
    [CLSCompliant(false)]
    public unsafe static class Memory
    {
        static Memory() {
            //Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        /// <summary>
        /// 获取系统中的物理内存（RAM）总量。Int32.MaxValue;
        /// Gets the total amount of physical memory (RAM) in the system.
        /// </summary>
        public static ulong TotalPhysicalBytes {
            get {
                Console.WriteLine("PORT: TotalPhysicalBytes");
                return Int32.MaxValue;
            }
        }

        //private static void DestroyHeap() {
        //}

        //private static void Application_ApplicationExit(object sender, EventArgs e) {
        //    DestroyHeap();
        //}

        /// <summary>
        /// 分配的内存块至少与请求的量一样大。
        /// Allocates a block of memory at least as large as the amount requested.
        /// </summary>
        /// <param name="bytes">The number of bytes you want to allocate.</param>
        /// <returns>A pointer to a block of memory at least as large as <b>bytes</b>.</returns>
        /// <exception cref="OutOfMemoryException">Thrown if the memory manager could not fulfill the request for a memory block at least as large as <b>bytes</b>.</exception>
        public static IntPtr Allocate(ulong bytes) {
            return System.Runtime.InteropServices.Marshal.AllocHGlobal((IntPtr)bytes);
        }

        /// <summary>
        /// 分配的内存块至少与请求的量一样大。
        /// Allocates a block of memory at least as large as the amount requested.
        /// </summary>
        /// <param name="bytes">The number of bytes you want to allocate.</param>
        /// <returns>A pointer to a block of memory at least as large as bytes</returns>
        /// <remarks>
        /// This method uses an alternate method for allocating memory (VirtualAlloc in Windows). The allocation
        /// granularity is the page size of the system (usually 4K). Blocks allocated with this method may also
        /// be protected using the ProtectBlock method.
        /// </remarks>
		public static IntPtr AllocateLarge(ulong bytes) {
            IntPtr block = Syscall.mmap(IntPtr.Zero, bytes,
                                         MmapProts.PROT_READ | MmapProts.PROT_WRITE,
                                         MmapFlags.MAP_ANONYMOUS, -1, 0);
            if (block == IntPtr.Zero)
                throw new OutOfMemoryException("mmap returned a null pointer");

            if (bytes > 0)
                System.GC.AddMemoryPressure((long)bytes);
            return block;
        }

        /// <summary>
        /// 分配给定高度和宽度的位图。像素数据可以直接读取/写入，也可以使用PdnGraphics.DrawBitmap（）将其绘制到屏幕上。
        /// Allocates a bitmap of the given height and width. Pixel data may be read/written directly, 
        /// and it may be drawn to the screen using PdnGraphics.DrawBitmap().
        /// </summary>
        /// <param name="width">The width of the bitmap to allocate.</param>
        /// <param name="height">The height of the bitmap to allocate.</param>
        /// <param name="handle">Receives a handle to the bitmap.</param>
        /// <returns>A pointer to the bitmap's pixel data.</returns>
        /// <remarks>
        ///以下不变量可能对实现者有用：
        ///*位图总是每像素32位，BGRA。
        ///*位图的跨步始终为宽度*4。
        ///*位图的左上角像素（0,0）位于返回指针指向的第一个内存位置。
        ///*位图是自上而下的（“内存正确”排序）。
        ///*“handle”可以是您想要的任何类型的数据，但在位图的生存期内必须是唯一的，并且不能是IntPtr.Zero。
        ///*PdnGraphics.DrawBitmap（）必须理解句柄的值。
        ///*位图总是通过直接读写返回值指向的内存来修改。
        ///*PdnGraphics.DrawBitmap（）必须始终从此内存位置渲染（即，它必须将内存视为“易失性”）
        /// The following invariants may be useful for implementors:
        /// * The bitmap is always 32-bits per pixel, BGRA.
        /// * Stride for the bitmap is always width * 4.
        /// * The upper-left pixel of the bitmap (0,0) is located at the first memory location pointed to by the returned pointer.
        /// * The bitmap is top-down ("memory correct" ordering).
        /// * The 'handle' may be any type of data you want, but must be unique for the lifetime of the bitmap, and must not be IntPtr.Zero.
        /// * The handle's value must be understanded by PdnGraphics.DrawBitmap().
        /// * The bitmap is always modified by directly reading and writing to the memory pointed to by the return value.
        /// * PdnGraphics.DrawBitmap() must always render from this memory location (i.e. it must treat the memory as 'volatile')
        /// </remarks>
        public static IntPtr AllocateBitmap(int width, int height, out IntPtr handle) {
            IntPtr pvBits = System.Runtime.InteropServices.Marshal.AllocHGlobal(width * height * 4);
            if (pvBits == IntPtr.Zero)
                throw new OutOfMemoryException("AllocHGlobal returned NULL (" + Marshal.GetLastWin32Error().ToString() + ") while attempting to allocate " + width + "x" + height + " bitmap");

            GdipCreateBitmapFromScan0(width, height, width * 4, PixelFormat.Format32bppArgb, pvBits, out handle);
            return pvBits;
        }
        [DllImport("gdiplus.dll", SetLastError = true)]
        internal static extern int GdipCreateBitmapFromScan0(int width, int height, int stride, PixelFormat format, IntPtr scan0, out IntPtr bmp);

        /// <summary>
        /// 释放以前使用AllocateBitmap分配的位图。
        /// Frees a bitmap previously allocated with AllocateBitmap.
        /// </summary>
        /// <param name="handle">上次调用AllocateBitmap时返回的句柄The handle that was returned from a previous call to AllocateBitmap.</param>
        /// <param name="width">The width of the bitmap, as specified in the original call to AllocateBitmap.</param>
        /// <param name="height">The height of the bitmap, as specified in the original call to AllocateBitmap.</param>
        public static void FreeBitmap(IntPtr handle, int width, int height) {
            GdipDisposeImage(handle);
        }
        [DllImport("gdiplus.dll")]
        internal static extern int GdipDisposeImage(IntPtr image);

        /// <summary>
        /// 释放以前使用Allocate（）分配的内存块。
        /// Frees a block of memory previously allocated with Allocate().
        /// </summary>
        /// <param name="block">The block to free.</param>
        /// <exception cref="InvalidOperationException">There was an error freeing the block.</exception>
        public static void Free(IntPtr block) {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(block);
        }

        /// <summary>
        /// 释放以前使用AllocateLarge（）分配的内存块。
        /// Frees a block of memory previous allocated with AllocateLarge().
        /// </summary>
        /// <param name="block">The block to free.</param>
        /// <param name="bytes">The size of the block.</param>
        public static void FreeLarge(IntPtr block, ulong bytes) {
            int retval = Syscall.munmap(block, bytes);
            UnixMarshal.ThrowExceptionForLastErrorIf(retval);
            if (bytes > 0)
                System.GC.RemoveMemoryPressure((long)bytes);
        }

        /// <summary>
        /// 对以前使用AllocateLarge分配的块设置保护。
        /// Sets protection on a block previously allocated with AllocateLarge.
        /// </summary>
        /// <param name="block">要设置保护的起始内存地址The starting memory address to set protection for.</param>
        /// <param name="size">The size of the block.</param>
        /// <param name="readAccess">是否允许读取访问。Whether to allow read access.</param>
        /// <param name="writeAccess">是否允许写访问。Whether to allow write access.</param>
        /// <remarks>
        ///如果不为写访问指定false，则不能为读访问指定false。
        ///实现者注意：此方法不能保证实际设置读/写能力
        ///在存储器块上，并且可以在参数验证之后被实现为无操作。
        /// You may not specify false for read access without also specifying false for write access.
        /// Note to implementors: This method is not guaranteed to actually set read/write-ability 
        /// on a block of memory, and may instead be implemented as a no-op after parameter validation.
        /// </remarks>
        public static void ProtectBlockLarge(IntPtr block, ulong size, bool readAccess, bool writeAccess) {
            MmapProts prot;
            if (readAccess && writeAccess)
                prot = MmapProts.PROT_READ | MmapProts.PROT_WRITE;
            else if (readAccess && !writeAccess)
                prot = MmapProts.PROT_READ;
            else if (!readAccess && !writeAccess)
                prot = MmapProts.PROT_NONE;
            else
                throw new InvalidOperationException("May not specify a page to be write-only");

#if DEBUGSPEW
            Tracing.Ping("ProtectBlockLarge: block #" + block.ToString() + ", read: " + readAccess + ", write: " + writeAccess);
#endif

            Syscall.mprotect(block, size, prot);
        }

        /// <summary>
        /// 将字节从内存的一个区域复制到另一个区域。由于仅此功能接受指针，它不能进行任何边界检查
        /// Copies bytes from one area of memory to another. Since this function only
        /// takes pointers, it can not do any bounds checking.
        /// </summary>
        /// <param name="dst">将字节复制到的起始地址。The starting address of where to copy bytes to.</param>
        /// <param name="src">从何处复制字节的起始地址.The starting address of where to copy bytes from.</param>
        /// <param name="length">The number of bytes to copy</param>
        public static void Copy(IntPtr dst, IntPtr src, ulong length) {
            Copy(dst.ToPointer(), src.ToPointer(), length);
        }

        /// <summary>
        /// 将字节从内存的一个区域复制到另一个区域。由于仅此功能接受指针，它不能进行任何边界检查。
        /// Copies bytes from one area of memory to another. Since this function only
        /// takes pointers, it can not do any bounds checking.
        /// </summary>
        /// <param name="dst">将字节复制到的起始地址。The starting address of where to copy bytes to.</param>
        /// <param name="src">从何处复制字节的起始地址.The starting address of where to copy bytes from.</param>
        /// <param name="length">The number of bytes to copy</param>
        public static void Copy(void* dst, void* src, ulong length) {
            SafeNativeMethods.memcpy(dst, src, new UIntPtr(length));
        }

        public static void SetToZero(IntPtr dst, ulong length) {
            SetToZero(dst.ToPointer(), length);
        }

        public static void SetToZero(void* dst, ulong length) {
            SafeNativeMethods.memset(dst, 0, new UIntPtr(length));
        }
    }

  
}
