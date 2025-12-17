using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace Schedulers
{
    internal class ArrayEx
    {
        private static readonly Dictionary<Type, Array> _emptyArrays = new Dictionary<Type, Array>(128);
        public static readonly Type[] EmptyTypes = new Type[0];
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            var type = typeof(T);
            if (type == typeof(Type))
            {
                return (T[])(object)EmptyTypes;
            }

            if (_emptyArrays.TryGetValue(type, out var array))
            {
                return (T[])array;
            }

            var result = new T[0];
            _emptyArrays[type] = result;
            return result;
        }
    }
}
