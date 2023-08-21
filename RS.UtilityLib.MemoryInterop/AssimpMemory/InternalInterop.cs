using System;
using System.Collections.Generic;
using System.Text;
using Unsafe = RS.UtilityLib.MemoryInterop.MemoryInterop;
namespace RS.UtilityLib.MemoryInterop.Assimp
{
    /// <summary>
    /// Internal usage only for fast-interop - used to find where we need to patch up code and is removed after the patching process.
    /// </summary>
    internal static class InternalInterop
    {

        public static unsafe void WriteArray<T>(IntPtr pDest, T[] data, int startIndex, int count) where T : struct {
            byte* ptr = (byte*)pDest.ToPointer();
            int elementSize = Unsafe_SizeOf<T>();
            for (int i = startIndex; i < count; i++) {
                Unsafe_Write(ptr,ref data[i]);
                ptr += elementSize;
            }
        }

        public static unsafe void ReadArray<T>(IntPtr pSrc, T[] data, int startIndex, int count) where T : struct {
            byte* ptr = (byte*)pSrc.ToPointer();
            int elementSize = Unsafe_SizeOf<T>();
            for (int i = startIndex; i < count; i++) {
                data[i] = Unsafe_Read<T>(ptr);
                ptr += elementSize;
            }
        }
        /// <summary>
        /// Unsafe.Write
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pDest"></param>
        /// <param name="srcData"></param>
        public static unsafe void WriteInline<T>(void* pDest, ref T srcData) where T : struct {
            //Unsafe.Write(pDest, srcData);
            Unsafe.WriteInline(pDest, srcData);
        } 
        public static unsafe void Unsafe_Write<T>(void* pDest, ref T srcData) where T : struct {
            //Unsafe.Write(pDest, srcData);
            Unsafe.WriteInline(pDest, srcData);
        }
        /// <summary>
        /// Unsafe.Read
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pSrc"></param>
        /// <returns></returns>
        public static unsafe T ReadInline<T>(void* pSrc) where T : struct {
            //return Unsafe.Read<T>(pSrc);
            return Unsafe.ReadInline<T>(pSrc);
        } 
        public static unsafe T Unsafe_Read<T>(void* pSrc) where T : struct {
            //return Unsafe.Read<T>(pSrc);
            return Unsafe.ReadInline<T>(pSrc);
        }
        /// <summary>
        /// Unsafe.SizeOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static unsafe int SizeOfInline<T>() {
            //return Unsafe.SizeOf<T>();
            return Unsafe.SizeOfInline<T>();
        } 
        public static unsafe int Unsafe_SizeOf<T>() {
            //return Unsafe.SizeOf<T>();
            return Unsafe.SizeOfInline<T>();
        }
        /// <summary>
        /// Unsafe.CopyBlock
        /// </summary>
        /// <param name="pDest"></param>
        /// <param name="pSrc"></param>
        /// <param name="count"></param>
        public static unsafe void MemCopyInline(void* pDest, void* pSrc, uint count) {
            //Unsafe.CopyBlock(pDest, pSrc, (uint)count);
            Unsafe.MemCopyInline(pDest, pSrc, count);
        } 
        public static unsafe void Unsafe_CopyBlock(void* pDest, void* pSrc, uint count) {
            //Unsafe.CopyBlock(pDest, pSrc, (uint)count);
            Unsafe.MemCopyInline(pDest, pSrc, count);
        }
        /// <summary>
        /// Unsafe.InitBlock
        /// </summary>
        /// <param name="pDest"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        public static unsafe void MemSetInline(void* pDest, byte value, uint count) {
            //Unsafe.InitBlock(pDest, value, (uint)count);
            Unsafe.MemSetInline(pDest, value, count);
        }
        public static unsafe void Unsafe_InitBlock(void* pDest, byte value, uint count) {
            Unsafe.MemSetInline(pDest, value, count);
        }
    }

}
