﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Atiran.CustomDocking {
    internal delegate TResult Func<TResult>();
    internal delegate TResult Func<T, TResult>(T a);
    internal delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    //internal delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    //internal delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    internal static class LinqHelper {
        /// <summary>
        /// Creates an array from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        internal static TSource[] ToArray<TSource>(
            this IEnumerable<TSource> source) {
            return source.ToList().ToArray();
        }
        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        internal static List<TSource> ToList<TSource>(
            this IEnumerable<TSource> source) {
            if(source == null)
                throw new ArgumentNullException("source");

            return new List<TSource>(source);
        }
        /// <summary>
        /// Returns the first element of the sequence that satisfies a 
        /// condition or a default value if no such element is found.
        /// </summary>

        internal static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            return FirstOrDefault(source.Where(predicate));
        }
        /// <summary>
        /// Returns the first element of a sequence, or a default value if 
        /// the sequence contains no elements.
        /// </summary>

        internal static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source) {
            return source.FirstImpl(Futures<TSource>.Default);
        }

        private static class Futures<T> {
            public static readonly Func<T> Default = () => default(T);
            public static readonly Func<T> Undefined = () => { throw new InvalidOperationException(); };
        }
        /// <summary>
        /// Base implementation of First operator.
        /// </summary>

        private static TSource FirstImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> empty) {
            if(source == null)
                throw new ArgumentNullException("source");
            Debug.Assert(empty != null);

            var list = source as IList<TSource>;    // optimized case for lists
            if(list != null)
                return list.Count > 0 ? list[0] : empty();

            using(var e = source.GetEnumerator())  // fallback for enumeration
                return e.MoveNext() ? e.Current : empty();
        }


        /// <summary>
        /// Returns the last element of a sequence, or a default value if 
        /// the sequence contains no elements.
        /// </summary>

        internal static TSource LastOrDefault<TSource>(
            this IEnumerable<TSource> source) {
            return source.LastImpl(Futures<TSource>.Default);
        }
        /// <summary>
        /// Base implementation of Last operator.
        /// </summary>

        private static TSource LastImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> empty) {
            if(source == null)
                throw new ArgumentNullException("source");

            var list = source as IList<TSource>;    // optimized case for lists
            if(list != null)
                return list.Count > 0 ? list[list.Count - 1] : empty();

            using(var e = source.GetEnumerator()) {
                if(!e.MoveNext())
                    return empty();

                var last = e.Current;
                while(e.MoveNext())
                    last = e.Current;

                return last;
            }
        }


        /// <summary>
        /// Determines whether all elements of a sequence satisfy a condition.
        /// </summary>

        internal static bool All<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            if(source == null)
                throw new ArgumentNullException("source");
            if(predicate == null)
                throw new ArgumentNullException("predicate");

            foreach(var item in source)
                if(!predicate(item))
                    return false;

            return true;
        }


        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>

        internal static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            if(predicate == null)
                throw new ArgumentNullException("predicate");

            return source.Where((item, i) => predicate(item));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate. 
        /// Each element's index is used in the logic of the predicate function.
        /// </summary>

        internal static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate) {
            if(source == null)
                throw new ArgumentNullException("source");
            if(predicate == null)
                throw new ArgumentNullException("predicate");

            return WhereYield(source, predicate);
        }

        private static IEnumerable<TSource> WhereYield<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate) {
            var i = 0;
            foreach(var item in source)
                if(predicate(item, i++))
                    yield return item;
        }




        /// <summary>
        /// Filters the elements of an <see cref="IEnumerable"/> based on a specified type.
        /// </summary>

        internal static IEnumerable<TResult> OfType<TResult>(
            this IEnumerable source) {
            if(source == null)
                throw new ArgumentNullException("source");

            return OfTypeYield<TResult>(source);
        }

        private static IEnumerable<TResult> OfTypeYield<TResult>(
            IEnumerable source) {
            foreach(var item in source)
                if(item is TResult)
                    yield return (TResult)item;
        }
    }
}

namespace System.Linq {
    public class CustomDockingHelper { 
        
    }
}