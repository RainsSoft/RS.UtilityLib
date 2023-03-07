
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RS.UtilityLib.RetryLib
{
    public delegate void Action();
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    public delegate TResult Func<TResult>();
    public delegate TResult Func<T, TResult>(T a);
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
    }
}

namespace System.Linq {
    public class CustomDockingHelper { 
        
    }
}