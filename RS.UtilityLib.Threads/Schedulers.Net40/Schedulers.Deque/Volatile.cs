using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Schedulers
{
    internal class Volatile
    {
      
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        internal static long Read(ref long location)
        {
            return Thread.VolatileRead(ref location);
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        internal static void Write(ref long location, long value)
        {
            Thread.VolatileWrite(ref location, value);
        }

    }
}
