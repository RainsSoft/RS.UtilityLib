using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RS.UtilityLib.MemoryInterop
{
    public static class Unsafe
    {
        /// <summary>
        /// Casts the by-ref value from one type to another.
        /// </summary>
        /// <typeparam name="Object">Type to cast from.</typeparam>
        /// <typeparam name="T">Type to cast to.</typeparam>
        /// <param name="obj">By-ref value.</param>
        /// <returns>Ref to the value, as the new type.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T As<T>(object obj) where T : class {
            return MemoryInterop.As<object, T>(ref obj);
            //return (T)obj;
        }
        /// <summary>
        /// Casts the by-ref value to a pointer (inlined). Note: This does not do any sort of pinning.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="value">Ref to a value.</param>
        /// <returns>Pointer to the memory location.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void* AsPointer<T>(ref T value) {
            return MemoryInterop.AsPointerInline<T>(ref value).ToPointer();
            //return (IntPtr)value;
        }
        /// <summary>
        /// Copies the number of bytes from one pointer to the other (inlined, unaligned copy).
        /// </summary>
        /// <param name="dst">Pointer to the destination memory location.</param>
        /// <param name="src">Pointer to the source memory location</param>
        /// <param name="size">Number of bytes to copy</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void CopyBlock(void* dst, void* src, uint size) {
            MemoryInterop.MemCopyUnalignedInline(dst,src,size);
            //memcpy(dst, src, size);
        }
        /// <summary>
        /// Clears the memory to a specified value (inlined, unaligned init).
        /// </summary>
        /// <param name="dst">Pointer to the memory location.</param>
        /// <param name="initValue">Value the memory will be cleared to.</param>
        /// <param name="size">Number of bytes to to set.</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void InitBlock(void* dst, byte initValue, uint size) {
            MemoryInterop.MemSetUnalignedInline(dst, initValue, size);
            //meminit(dst, initValue, size);
        }
        /// <summary>
        /// Reads a single element from the memory location (inlined).
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="p">Pointer to memory location.</param>
        /// <returns>Value read.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe T Read<T>(void* p) {
            return MemoryInterop.ReadInline<T>(p);
            //return *(((T*)p));
        }
        /// <summary>
        /// Computes the size of the struct type.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int SizeOf<T>() {
            return MemoryInterop.SizeOfInline<T>();
            //return sizeof(T);
        }
        /// <summary>
        /// Writes a single element to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="p">Pointer to memory location</param>
        /// <param name="value">The value to write</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void Write<T>(void* p, T value) {
            MemoryInterop.WriteInline<T>(p, value);
            //p[0] = value;
        }
        /// <summary>
        /// Writes a single element to the memory location, taking into account the alignment(考虑到对准).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="p">Pointer to memory location</param>
        /// <param name="value">The value to write</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void Write<T>(void* p, ref T value) {
            int pad = (int)(((IntPtr)p).ToInt64() % (long)IntPtr.Size);

            Unsafe.Write<T>(AddIntPtr((IntPtr)p, pad).ToPointer(), value);
            //p[0] = value;
        }
        /// <summary>
        /// Adds an offset to the pointer.
        /// </summary>
        /// <param name="ptr">Pointer</param>
        /// <param name="offset">Offset</param>
        /// <returns>Pointer plus the offset</returns>
        public static IntPtr AddIntPtr(IntPtr ptr, int offset) {
            return new IntPtr(ptr.ToInt64() + offset);
        }
    }



}
