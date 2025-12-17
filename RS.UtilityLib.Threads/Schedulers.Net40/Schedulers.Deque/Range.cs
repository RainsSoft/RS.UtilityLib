using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Schedulers
{

    // [IsReadOnly]
    public struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public static Range All
        {
            get
            {
                return new Range(Index.Start, Index.End);
            }
        }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        // [NullableContext(2)]
        public override bool Equals(object value)
        {
            if (value is Range)
            {
                Range range = (Range)value;
                if (range.Start.Equals(Start))
                {
                    return range.End.Equals(End);
                }
                return false;
            }
            return false;
        }

        public bool Equals(Range other)
        {
            if (other.Start.Equals(Start))
            {
                return other.End.Equals(End);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start.GetHashCode(), End.GetHashCode());
        }

        //[NullableContext(1)]
        public override string ToString()
        {
            return string.Format("{0},{1}",Start.IsFromEnd?~Start.Value:Start.Value,End.IsFromEnd?~End.Value:End.Value);
            //Span<char> span = new Span<char>(new char[24]); //stackalloc char[24];
            //int num = 0;
            //if (Start.IsFromEnd)
            //{
            //    span[0] = '^';
            //    num = 1;
            //}
            //int charsWritten;
            //bool flag = ((uint)Start.Value).TryFormat(new String( span.Slice(num).ToArray()), out charsWritten);
            //num += charsWritten;
            //span[num++] = '.';
            //span[num++] = '.';
            //if (End.IsFromEnd)
            //{
            //    span[num++] = '^';
            //}
            //flag = ((uint)End.Value).TryFormat(span.Slice(num), out charsWritten);
            //num += charsWritten;
            //return new string(span.Slice(0, num).ToArray());
        }

        public static Range StartAt(Index start)
        {
            return new Range(start, Index.End);
        }

        public static Range EndAt(Index end)
        {
            return new Range(Index.Start, end);
        }


    }






}
