using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Schedulers
{
    public struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public static Index Start => new Index(0);

        public static Index End => new Index(-1);

        public int Value
        {
            get
            {
                if (_value < 0)
                {
                    return ~_value;
                }
                return _value;
            }
        }

        public bool IsFromEnd
        {
            get
            {
                return _value < 0;
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
            {
                throw new Exception("ThrowValueArgumentOutOfRange_NeedNonNegNumException()");
            }
            if (fromEnd)
            {
                _value = ~value;
            }
            else
            {
                _value = value;
            }
        }

        private Index(int value)
        {
            _value = value;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Index FromStart(int value)
        {
            if (value < 0)
            {
                throw new Exception("ThrowValueArgumentOutOfRange_NeedNonNegNumException()");
            }
            return new Index(value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Index FromEnd(int value)
        {
            if (value < 0)
            {
                throw new Exception("ThrowValueArgumentOutOfRange_NeedNonNegNumException()");
            }
            return new Index(~value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public int GetOffset(int length)
        {
            int num = _value;
            if (IsFromEnd)
            {
                num += length + 1;
            }
            return num;
        }

        //[NullableContext(2)]
        public override bool Equals(object value)
        {
            if (value is Index)
            {
                return _value == ((Index)value)._value;
            }
            return false;
        }

        public bool Equals(Index other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static implicit operator Index(int value)
        {
            return FromStart(value);
        }

        //[NullableContext(1)]
        public override string ToString()
        {
            if (IsFromEnd)
            {
                return ToStringFromEnd();
            }
            return ((uint)Value).ToString();
        }

        private string ToStringFromEnd()
        {
            return string.Format("^{0}",this.IsFromEnd?~Value: Value);
            //Span<char> span = new Span<char>(new char[11]);
            //int charsWritten;
            //bool flag = ((uint)Value).TryFormat(span.Slice(1), out charsWritten);
            //span[0] = '^';
            //return new string(span.Slice(0, charsWritten + 1).ToArray());
        }
    }
}
