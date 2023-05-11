using System;
using System.Runtime.InteropServices;

namespace RS.UtilityLib.SizeOfGenericT
{
    class NativeInterop
    {
        [DllImport("kernel32")]
        public extern static int LoadLibrary(string librayName);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public extern static int GetProcAddress(int hwnd, string procedureName);

        [DllImport("Kernel32", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [System.Diagnostics.DebuggerDisplay("{Data}")]
    unsafe struct astring
    {
        public astring(string init)
        {
            allocator = (void*)0;
#if DEBUG
            v0 = 0;
#endif
            v3 = 0;
            v4 = 0;
            v5 = 0;

            if (init.Length > 15)
            {
                data = Marshal.StringToHGlobalAnsi(init).ToPointer();
                reserved = Memory2.SizeOf(data);
            }
            else
            {
                fixed (void* dataptr = &data)
                {
                    IntPtr dataTemp = Marshal.StringToHGlobalAnsi(init);
                    NativeInterop.MoveMemory(new IntPtr(dataptr), dataTemp, init.Length + 1);
                }
                reserved = 15;
            }
            size = init.Length;
        }

        public void Free()
        {
            if (size > 15)
                Marshal.FreeHGlobal(new IntPtr(data));
        }

#if DEBUG
        int v0;
#endif
        void* allocator;
        void* data;
        int v3, v4, v5;
        int size;
        int reserved;

        public string Data
        {
            get
            {
                if (data != null)
                {
                    if (size > 15)
                    {
                        return Marshal.PtrToStringAnsi(new IntPtr(data), size);
                    }
                    else
                    {
                        fixed (void* dataptr = &data)
                            return Marshal.PtrToStringAnsi(new IntPtr(dataptr), size);
                    }
                }
                return null;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    [System.Diagnostics.DebuggerDisplay("{Data}")]
    unsafe struct wstring
    {
        public wstring(string init)
        {
            allocator = (void*)0;
#if DEBUG
            v0 = 0;
#endif
            v3 = 0;
            v4 = 0;
            v5 = 0;

            if (init.Length > 7)
            {
                data = Marshal.StringToHGlobalUni(init).ToPointer();
                reserved = Memory2.SizeOf(data);
            }
            else
            {
                fixed (void* dataptr = &data)
                {
                    IntPtr dataTemp = Marshal.StringToHGlobalUni(init);
                    NativeInterop.MoveMemory(new IntPtr(dataptr), dataTemp, (init.Length + 1) * 2);
                }
                reserved = 7;
            }
            size = init.Length;
        }

        public void Free()
        {
            if (size > 7)
                Marshal.FreeHGlobal(new IntPtr(data));
        }

        public string Data
        {
            get
            {
                if (data != null)
                {
                    if (size > 7)
                    {
                        return Marshal.PtrToStringUni(new IntPtr(data), size);
                    }
                    else
                    {
                        fixed (void* dataptr = &data)
                            return Marshal.PtrToStringUni(new IntPtr(dataptr), size);
                    }
                }
                return null;
            }
        }

#if DEBUG
        int v0;
#endif
        void* allocator;
        void* data;
        int v3, v4, v5;
        int size;
        int reserved;
    }
}
