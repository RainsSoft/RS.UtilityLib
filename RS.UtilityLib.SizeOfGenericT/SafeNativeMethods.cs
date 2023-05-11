using Mono.Posix;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.UtilityLib.SizeOfGenericT
{
    internal class SafeNativeMethods
    {
        private static bool isAdmin = GetIsAdministrator();

        private static bool GetIsAdministrator() {
            return (Syscall.getuid() == 0);
        }

        /// <summary>
        /// Gets a flag indicating whether the user has administrator-level privileges.
        /// </summary>
        /// <remarks>
        /// This is used to control access to actions that require the user to be an administrator.
        /// An example is checking for and installing updates, actions which are not normally able
        /// to be performed by normal or "limited" users. A user must also be an administrator in
        /// order to write to any Settings.SystemWide entries.
        /// </remarks>
        public static bool IsAdministrator {
            get {
                return isAdmin;
            }
        }
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern unsafe void memcpy(void* dst, void* src, UIntPtr length);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern unsafe void memset(void* dst, int c, UIntPtr length);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForSingleObject(
     IntPtr hHandle,
     uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForMultipleObjects(
            uint nCount,
            IntPtr[] lpHandles,
            [MarshalAs(UnmanagedType.Bool)] bool bWaitAll,
            uint dwMilliseconds);

        internal static uint WaitForMultipleObjects(IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds) {
            return WaitForMultipleObjects((uint)lpHandles.Length, lpHandles, bWaitAll, dwMilliseconds);
        }
    }
}
