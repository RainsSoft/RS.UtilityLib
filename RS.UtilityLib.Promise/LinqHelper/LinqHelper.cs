﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RS.UtilityLib.PromiseLib.RSG
{
    public delegate void Action();
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    public delegate TResult Func<TResult>();
    public delegate TResult Func<T, TResult>(T a);
    internal delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    //internal delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    //internal delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
 
    internal static class LinqHelper
    {
        //public static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, System.Threading.Timeout.Infinite);
        //public static TimeSpan InfiniteTimeSpan() { 
        //    TimeSpan infinite = TimeSpan.FromMilliseconds(-1);
        //    return infinite;
        //}
        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static List<TSource> ToList<TSource>(
            this IEnumerable<TSource> source) {
            if (source == null) throw new ArgumentNullException("source");

            return new List<TSource>(source);
        }
        /// <summary>
        /// Creates an array from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static TSource[] ToArray<TSource>(
            this IEnumerable<TSource> source) {
            return source.ToList().ToArray();
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>

        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return source.Where((item, i) => predicate(item));
        }
        /// <summary>
        /// Filters a sequence of values based on a predicate. 
        /// Each element's index is used in the logic of the predicate function.
        /// </summary>

        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate) {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return WhereYield(source, predicate);
        }

        private static IEnumerable<TSource> WhereYield<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate) {
            var i = 0;
            foreach (var item in source)
                if (predicate(item, i++))
                    yield return item;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Single" /> values.
        /// </summary>

        public static float? Max(
            this IEnumerable<float?> source) {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }
        /// <summary>
        /// Returns the maximum value in a generic sequence.
        /// </summary>

        public static TSource Max<TSource>(
            this IEnumerable<TSource> source) {
            var comparer = Comparer<TSource>.Default;
            return source.MinMaxImpl((x, y) => comparer.Compare(x, y) > 0);
        }

        /// <summary>
        /// Base implementation for Min/Max operator.
        /// </summary>

        private static TSource MinMaxImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> lesser) {
            if (source == null) throw new ArgumentNullException("source");
            System.Diagnostics.Debug.Assert(lesser != null);

            if (typeof(TSource).IsClass) // ReSharper disable CompareNonConstrainedGenericWithNull                
                source = source.Where(e => e != null).DefaultIfEmpty(); // ReSharper restore CompareNonConstrainedGenericWithNull

            return source.Aggregate((a, item) => lesser(a, item) ? a : item);
        }

        /// <summary>
        /// Base implementation for Min/Max operator for nullable types.
        /// </summary>

        private static TSource? MinMaxImpl<TSource>(
            this IEnumerable<TSource?> source,
            TSource? seed, Func<TSource?, TSource?, bool> lesser) where TSource : struct {
            if (source == null) throw new ArgumentNullException("source");
            System.Diagnostics.Debug.Assert(lesser != null);

            return source.Aggregate(seed, (a, item) => lesser(a, item) ? a : item);
            //  == MinMaxImpl(Repeat<TSource?>(null, 1).Concat(source), lesser);
        }
        /// <summary>
        /// Makes an enumerator seen as enumerable once more.
        /// </summary>
        /// <remarks>
        /// The supplied enumerator must have been started. The first element
        /// returned is the element the enumerator was on when passed in.
        /// DO NOT use this method if the caller must be a generator. It is
        /// mostly safe among aggregate operations.
        /// </remarks>

        private static IEnumerable<T> Renumerable<T>(this IEnumerator<T> e) {
            System.Diagnostics.Debug.Assert(e != null);

            do { yield return e.Current; } while (e.MoveNext());
        }
        /// <summary>
        /// Applies an accumulator function over a sequence.
        /// </summary>

        public static TSource Aggregate<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func) {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");

            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext())
                    throw new InvalidOperationException();

                return e.Renumerable().Skip(1).Aggregate(e.Current, func);
            }
        }
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then 
        /// returns the remaining elements.
        /// </summary>

        public static IEnumerable<TSource> Skip<TSource>(
            this IEnumerable<TSource> source,
            int count) {
            return source.SkipWhile((item, i) => i < count);
        }
        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition 
        /// is true and then returns the remaining elements. The element's 
        /// index is used in the logic of the predicate function.
        /// </summary>

        public static IEnumerable<TSource> SkipWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate) {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SkipWhileYield(source, predicate);
        }
        private static IEnumerable<TSource> SkipWhileYield<TSource>(
         IEnumerable<TSource> source,
         Func<TSource, int, bool> predicate) {
            using (var e = source.GetEnumerator()) {
                for (var i = 0; ; i++) {
                    if (!e.MoveNext())
                        yield break;

                    if (!predicate(e.Current, i))
                        break;
                }

                do { yield return e.Current; } while (e.MoveNext());
            }
        }
        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single" /> values.
        /// </summary>

        public static float? Average(
            this IEnumerable<float?> source) {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked {
                    sum += (float)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (float?)sum / count;
        }
        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single" /> values.
        /// </summary>

        public static float Average(
            this IEnumerable<float> source) {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            long count = 0;

            foreach (var num in source)
                checked {
                    sum += (float)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (float)sum / count;
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified 
        /// seed value is used as the initial accumulator value.
        /// </summary>

        public static TAccumulate Aggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func) {
            return Aggregate(source, seed, func, r => r);
        }
        /// <summary>
        /// Applies an accumulator function over a sequence. The specified 
        /// seed value is used as the initial accumulator value, and the 
        /// specified function is used to select the result value.
        /// </summary>

        public static TResult Aggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector) {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            var result = seed;

            foreach (var item in source)
                result = func(result, item);

            return resultSelector(result);
        }
        /// <summary>
        /// Returns the elements of the specified sequence or the type 
        /// parameter's default value in a singleton collection if the 
        /// sequence is empty.
        /// </summary>

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IEnumerable<TSource> source) {
            return source.DefaultIfEmpty(default(TSource));
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the specified 
        /// value in a singleton collection if the sequence is empty.
        /// </summary>

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IEnumerable<TSource> source,
            TSource defaultValue) {
            if (source == null) throw new ArgumentNullException("source");

            return DefaultIfEmptyYield(source, defaultValue);
        }

        private static IEnumerable<TSource> DefaultIfEmptyYield<TSource>(
            IEnumerable<TSource> source,
            TSource defaultValue) {
            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext())
                    yield return defaultValue;
                else
                    do { yield return e.Current; } while (e.MoveNext());
            }
        }

        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T}"/> that has the 
        /// specified type argument.
        /// </summary>

        public static IEnumerable<TResult> Empty<TResult>() {
            return Sequence<TResult>.Empty;
        }


        private static class Sequence<T>
        {
            public static readonly IEnumerable<T> Empty = new T[0];
        }
        #region Count

        public static int Count<TSource>(this IEnumerable<TSource> source) {
            Check.Source(source);

            var collection = source as ICollection<TSource>;
            if (collection != null)
                return collection.Count;

            int counter = 0;
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    checked { counter++; }

            return counter;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            Check.SourceAndSelector(source, predicate);

            int counter = 0;
            foreach (var element in source)
                if (predicate(element))
                    checked { counter++; }

            return counter;
        }

        #endregion

        #region Any

        public static bool Any<TSource>(this IEnumerable<TSource> source) {
            Check.Source(source);

            var collection = source as ICollection<TSource>;
            if (collection != null)
                return collection.Count > 0;

            using (var enumerator = source.GetEnumerator())
                return enumerator.MoveNext();
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            Check.SourceAndPredicate(source, predicate);

            foreach (TSource element in source)
                if (predicate(element))
                    return true;

            return false;
        }

        #endregion

        #region Select

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            Check.SourceAndSelector(source, selector);

            return CreateSelectIterator(source, selector);
        }

        static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            foreach (var element in source)
                yield return selector(element);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector) {
            Check.SourceAndSelector(source, selector);

            return CreateSelectIterator(source, selector);
        }

        static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector) {
            int counter = 0;
            foreach (TSource element in source) {
                yield return selector(element, counter);
                counter++;
            }
        }

        #endregion

        #region Sum

        public static int Sum(this IEnumerable<int> source) {
            Check.Source(source);
            int total = 0;

            foreach (var element in source)
                total = checked(total + element);
            return total;
        }

        public static int? Sum(this IEnumerable<int?> source) {
            Check.Source(source);

            int total = 0;
            foreach (var element in source) {
                if (element.HasValue)
                    total = checked(total + element.Value);
            }
            return total;
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) {
            Check.SourceAndSelector(source, selector);
            int total = 0;

            foreach (var element in source)
                total = checked(total + selector(element));

            return total;
        }

        public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) {
            Check.SourceAndSelector(source, selector);

            int total = 0;
            foreach (var element in source) {
                var value = selector(element);
                if (value.HasValue)
                    total = checked(total + value.Value);
            }
            return total;
        }

        public static long Sum(this IEnumerable<long> source) {
            Check.Source(source);

            long total = 0;

            foreach (var element in source)
                total = checked(total + element);
            return total;
        }

        public static long? Sum(this IEnumerable<long?> source) {
            Check.Source(source);

            long total = 0;
            foreach (var element in source) {
                if (element.HasValue)
                    total = checked(total + element.Value);
            }
            return total;
        }

        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) {
            Check.SourceAndSelector(source, selector);

            long total = 0;
            foreach (var element in source)
                total = checked(total + selector(element));
            return total;
        }

        public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) {
            Check.SourceAndSelector(source, selector);

            long total = 0;
            foreach (var element in source) {
                var value = selector(element);
                if (value.HasValue)
                    total = checked(total + value.Value);
            }
            return total;
        }

        public static double Sum(this IEnumerable<double> source) {
            Check.Source(source);

            double total = 0;

            foreach (var element in source)
                total = checked(total + element);
            return total;
        }

        public static double? Sum(this IEnumerable<double?> source) {
            Check.Source(source);

            double total = 0;
            foreach (var element in source) {
                if (element.HasValue)
                    total = checked(total + element.Value);
            }
            return total;
        }

        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) {
            Check.SourceAndSelector(source, selector);

            double total = 0;

            foreach (var element in source)
                total = checked(total + selector(element));
            return total;
        }

        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) {
            Check.SourceAndSelector(source, selector);

            double total = 0;
            foreach (var element in source) {
                var value = selector(element);
                if (value.HasValue)
                    total = checked(total + value.Value);
            }
            return total;
        }

        public static float Sum(this IEnumerable<float> source) {
            Check.Source(source);

            float total = 0;

            foreach (var element in source)
                total = checked(total + element);
            return total;
        }

        public static float? Sum(this IEnumerable<float?> source) {
            Check.Source(source);

            float total = 0;
            foreach (var element in source) {
                if (element.HasValue)
                    total = checked(total + element.Value);
            }
            return total;

        }

        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) {
            Check.SourceAndSelector(source, selector);
            float total = 0;
            foreach (var element in source)
                total = checked(total + selector(element));
            return total;
        }

        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) {
            Check.SourceAndSelector(source, selector);

            float total = 0;
            foreach (var element in source) {
                var value = selector(element);
                if (value.HasValue)
                    total = checked(total + value.Value);
            }
            return total;
        }

        public static decimal Sum(this IEnumerable<decimal> source) {
            Check.Source(source);
            decimal total = 0;

            foreach (var element in source)
                total = checked(total + element);
            return total;
        }

        public static decimal? Sum(this IEnumerable<decimal?> source) {
            Check.Source(source);

            decimal total = 0;
            foreach (var element in source) {
                if (element.HasValue)
                    total = checked(total + element.Value);
            }
            return total;

        }

        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) {
            Check.SourceAndSelector(source, selector);
            decimal total = 0;

            foreach (var element in source)
                total = checked(total + selector(element));
            return total;
        }

        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) {
            Check.SourceAndSelector(source, selector);

            decimal total = 0;
            foreach (var element in source) {
                var value = selector(element);
                if (value.HasValue)
                    total = checked(total + value.Value);
            }
            return total;
        }

        #endregion

        #region Concat

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) {
            Check.FirstAndSecond(first, second);

            return CreateConcatIterator(first, second);
        }

        static IEnumerable<TSource> CreateConcatIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second) {
            foreach (TSource element in first)
                yield return element;
            foreach (TSource element in second)
                yield return element;
        }

        #endregion

        #region SelectMany

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
            Check.SourceAndSelector(source, selector);

            return CreateSelectManyIterator(source, selector);
        }

        static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
            foreach (TSource element in source)
                foreach (TResult item in selector(element))
                    yield return item;
        }
        #endregion

        #region Check
        static class Check
        {

            public static void Source(object source) {
                if (source == null)
                    throw new ArgumentNullException("source");
            }

            public static void Source1AndSource2(object source1, object source2) {
                if (source1 == null)
                    throw new ArgumentNullException("source1");
                if (source2 == null)
                    throw new ArgumentNullException("source2");
            }

            public static void SourceAndFuncAndSelector(object source, object func, object selector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (func == null)
                    throw new ArgumentNullException("func");
                if (selector == null)
                    throw new ArgumentNullException("selector");
            }


            public static void SourceAndFunc(object source, object func) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (func == null)
                    throw new ArgumentNullException("func");
            }

            public static void SourceAndSelector(object source, object selector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (selector == null)
                    throw new ArgumentNullException("selector");
            }

            public static void SourceAndPredicate(object source, object predicate) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (predicate == null)
                    throw new ArgumentNullException("predicate");
            }

            public static void FirstAndSecond(object first, object second) {
                if (first == null)
                    throw new ArgumentNullException("first");
                if (second == null)
                    throw new ArgumentNullException("second");
            }

            public static void SourceAndKeySelector(object source, object keySelector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (keySelector == null)
                    throw new ArgumentNullException("keySelector");
            }

            public static void SourceAndKeyElementSelectors(object source, object keySelector, object elementSelector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (keySelector == null)
                    throw new ArgumentNullException("keySelector");
                if (elementSelector == null)
                    throw new ArgumentNullException("elementSelector");
            }
            public static void SourceAndKeyResultSelectors(object source, object keySelector, object resultSelector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (keySelector == null)
                    throw new ArgumentNullException("keySelector");
                if (resultSelector == null)
                    throw new ArgumentNullException("resultSelector");
            }

            public static void SourceAndCollectionSelectorAndResultSelector(object source, object collectionSelector, object resultSelector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (collectionSelector == null)
                    throw new ArgumentNullException("collectionSelector");
                if (resultSelector == null)
                    throw new ArgumentNullException("resultSelector");
            }

            public static void SourceAndCollectionSelectors(object source, object collectionSelector, object selector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (collectionSelector == null)
                    throw new ArgumentNullException("collectionSelector");
                if (selector == null)
                    throw new ArgumentNullException("selector");
            }

            public static void JoinSelectors(object outer, object inner, object outerKeySelector, object innerKeySelector, object resultSelector) {
                if (outer == null)
                    throw new ArgumentNullException("outer");
                if (inner == null)
                    throw new ArgumentNullException("inner");
                if (outerKeySelector == null)
                    throw new ArgumentNullException("outerKeySelector");
                if (innerKeySelector == null)
                    throw new ArgumentNullException("innerKeySelector");
                if (resultSelector == null)
                    throw new ArgumentNullException("resultSelector");
            }

            public static void GroupBySelectors(object source, object keySelector, object elementSelector, object resultSelector) {
                if (source == null)
                    throw new ArgumentNullException("source");
                if (keySelector == null)
                    throw new ArgumentNullException("keySelector");
                if (elementSelector == null)
                    throw new ArgumentNullException("elementSelector");
                if (resultSelector == null)
                    throw new ArgumentNullException("resultSelector");
            }
        }
        #endregion

        #region Take

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count) {
            Check.Source(source);

            return CreateTakeIterator(source, count);
        }

        static IEnumerable<TSource> CreateTakeIterator<TSource>(IEnumerable<TSource> source, int count) {
            if (count <= 0)
                yield break;

            int counter = 0;
            foreach (TSource element in source) {
                yield return element;

                if (++counter == count)
                    yield break;
            }
        }

        #endregion

        #region TakeWhile

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            Check.SourceAndPredicate(source, predicate);

            return CreateTakeWhileIterator(source, predicate);
        }

        static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var element in source) {
                if (!predicate(element))
                    yield break;

                yield return element;
            }
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate) {
            Check.SourceAndPredicate(source, predicate);

            return CreateTakeWhileIterator(source, predicate);
        }

        static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate) {
            int counter = 0;
            foreach (var element in source) {
                if (!predicate(element, counter))
                    yield break;

                yield return element;
                counter++;
            }
        }

        #endregion

        #region OfType

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source) {
            Check.Source(source);

            return CreateOfTypeIterator<TResult>(source);
        }

        static IEnumerable<TResult> CreateOfTypeIterator<TResult>(IEnumerable source) {
            foreach (object element in source)
                if (element is TResult)
                    yield return (TResult)element;
        }

        #endregion

        #region First

        static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool fallbackThrow) {
            foreach (var element in source)
                if (predicate(element))
                    return element;

            //if (fallback == Fallback.Throw)
            //    throw NoMatchingElement();
            if (fallbackThrow) {
                throw new Exception("NoMatchingElement()");
            }
            return default(TSource);
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source) {
            Check.Source(source);

            var list = source as IList<TSource>;
            if (list != null) {
                if (list.Count != 0)
                    return list[0];
            }
            else {
                using (var enumerator = source.GetEnumerator()) {
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }

            //throw EmptySequence();
            throw new Exception("EmptySequence()");
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            Check.SourceAndPredicate(source, predicate);

            return source.First(predicate, true);// Fallback.Throw);
        }

        #endregion

        #region FirstOrDefault

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source) {
            Check.Source(source);

            //#if !FULL_AOT_RUNTIME
            //            return source.First(PredicateOf<TSource>.Always, Fallback.Default);
            //#else
            // inline the code to reduce dependency o generic causing AOT errors on device (e.g. bug #3285)
            foreach (var element in source)
                return element;

            return default(TSource);
            //#endif
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            Check.SourceAndPredicate(source, predicate);

            return source.First(predicate, false);// Fallback.Default);
        }

        #endregion

    }
}

namespace System.Linq {
    internal class PromiseLinqHelper
    { 
        
    }
}