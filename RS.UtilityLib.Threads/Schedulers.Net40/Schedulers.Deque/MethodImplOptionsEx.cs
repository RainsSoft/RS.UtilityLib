namespace Schedulers
{
    public class MethodImplOptionsEx
    {
        public const short Unmanaged = 4;
        public const short NoInlining = 8;
        public const short ForwardRef = 0x10;
        public const short Synchronized = 0x20;
        public const short NoOptimization = 0x40;
        public const short PreserveSig = 0x80;
        public const short AggressiveInlining = 0x100;
        public const short AggressiveOptimization = 0x200;
        public const short InternalCall = 0x1000;
    }

}
