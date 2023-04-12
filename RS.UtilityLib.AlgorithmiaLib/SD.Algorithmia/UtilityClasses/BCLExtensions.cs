using System;
using System.Collections;
using System.Collections.Generic;

using SD.System_Linq;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using SD.Tools.Algorithmia;
using SD.Tools.Algorithmia.UtilityClasses;


namespace SD.Tools.BCLExtensions.SystemRelated
{
	/// <summary>
	///事件相关扩展方法类 Class for event related extension methods
	/// </summary>
	public static class EventExtensionMethods
	{
		/// <summary>
		///引发由指定的处理程序表示的事件。 Raises the event which is represented by the handler specified. 
		/// </summary>
		/// <typeparam name="T">type of the event args</typeparam>
		/// <param name="handler">The handler of the event to raise.</param>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="arguments">The arguments to pass to the handler.</param>
		public static void RaiseEvent<T>(this EventHandler<T> handler, object sender, T arguments)
			where T : System.EventArgs {
			if (handler != null) {
				handler(sender, arguments);
			}
		}


		/// <summary>
		///如果处理程序不为空，则引发 PropertyChanged 事件，否则为空操作. Raises the PropertyChanged event, if the handler isn't null, otherwise a no-op
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="propertyName">Name of the property.</param>
		public static void RaiseEvent(this PropertyChangedEventHandler handler, object sender, string propertyName) {
			if (handler != null) {
				handler(sender, new PropertyChangedEventArgs(propertyName));
			}
		}


		/// <summary>
		///在使用默认空参数传入的处理程序上引发事件. Raises the event on the handler passed in with default empty arguments
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="sender">The sender.</param>
		public static void RaiseEvent(this EventHandler handler, object sender) {
			if (handler != null) {
				handler(sender, EventArgs.Empty);
			}
		}
	}
}

namespace SD.Tools.BCLExtensions.CollectionsRelated
{
	/// <summary>
	///字典相关扩展方法的类。 Class for Dictionary related extension methods.
	/// </summary>
	public static class DictionaryExtensionMethods
	{
		/// <summary>
		/// 将指定的范围添加到指定的字典中，使用键生成器函数生成键值。 如果该键已经存在，则该键的值将被要添加的值覆盖。
		/// Adds the range specified to the dictionary specified, using the key producer func to produce the key values.
		/// If the key already exists, the key's value is overwritten with the value to add.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="container">The container.</param>
		/// <param name="keyProducerFunc">The key producer func.</param>
		/// <param name="rangeToAdd">The range to add.</param>
		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> container, Func<TValue, TKey> keyProducerFunc, IEnumerable<TValue> rangeToAdd) {
			if ((container == null) || (rangeToAdd == null)) {
				return;
			}

			ArgumentVerifier.CantBeNull(keyProducerFunc, "keyProducerFunc");

			foreach (TValue toAdd in rangeToAdd) {
				container[keyProducerFunc(toAdd)] = toAdd;
			}
		}


		/// <summary>
		/// 从指定的字典中获取键的值，如果找不到键或键为空，则为 null / default(TValue)。
		/// Gets the value for the key from the dictionary specified, or null / default(TValue) if key not found or the key is null.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns>the value for the key from the dictionary specified, or null / default(TValue) if key not found or the key is null.</returns>
		public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
			TValue toReturn;
			if (((object)key == null) || !dictionary.TryGetValue(key, out toReturn)) {
				toReturn = default(TValue);
			}
			return toReturn;
		}
	}


	/// <summary>
	///HashSet 相关扩展方法的类。 Class for HashSet related extension methods.
	/// </summary>
	public static class HashSetExtensionMethods
	{
		/// <summary>
		///将源定义的范围添加到目标。 Adds the range defined by source to the destination.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public static void AddRange<T>(this HashSet<T> destination, IEnumerable<T> source) {
			if (source != null) {
				foreach (T element in source) {
					destination.Add(element);
				}
			}
		}
	}

	/// <summary>
	///IEnumerable 和 IEnumerable(Of T) 相关扩展方法的类。 Class for IEnumberable and IEnumerable(Of T) related extension methods.
	/// </summary>
	public static class IEnumerableExtensionMethods
	{
		/// <summary>
		///将可枚举对象转换为 ReadOnlyCollection。 Converts the enumerable to a ReadOnlyCollection.
		/// </summary>
		/// <param name="source">the enumerable to convert</param>
		/// <returns>具有传入可枚举元素的 ReadOnlyCollection，如果源为 null 或为空，则为空 ReadOnlyCollection。
		/// A ReadOnlyCollection with the elements of the passed in enumerable, or an empty ReadOnlyCollection if source is null or empty</returns>
		public static ReadOnlyCollection<TDestination> ToReadOnlyCollection<TDestination>(this IEnumerable source) {
			List<TDestination> sourceAsDestination = new List<TDestination>();
			if (source != null) {
				foreach (TDestination toAdd in source) {
					sourceAsDestination.Add(toAdd);
				}
			}
			return new ReadOnlyCollection<TDestination>(sourceAsDestination);
		}


		/// <summary>
		///创建一个新的哈希集并将源添加到它。 Creates a new hashset and adds the source to it. 
		/// </summary>
		/// <typeparam name="TDestination">The type of the destination.</typeparam>
		/// <param name="source">The source.</param>
		/// <returns>Hashset with all the unique values in source</returns>
		public static HashSet<TDestination> ToHashSet<TDestination>(this IEnumerable<TDestination> source) {
			return new HashSet<TDestination>(source);
		}


		/// <summary>
		/// 检查要比较的可枚举是否等于源可枚举，元素明智。 如果元素的顺序不同，该方法仍将返回 true。
		/// 这与考虑顺序的 SequenceEqual 不同。
		/// Checks whether the enumerable to compare with is equal to the source enumerable, element wise. If elements are in a different order, the
		/// method will still return true. This is different from SequenceEqual which does take order into account
		/// </summary>
		/// <typeparam name="T">type of the element in the sequences to compare</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="toCompareWith">the sequence to compare with.</param>
		/// <returns>true if the source and the sequence to compare with have the same elements, regardless of ordering</returns>
		public static bool SetEqual<T>(this IEnumerable<T> source, IEnumerable<T> toCompareWith) {
			if ((source == null) || (toCompareWith == null)) {
				return false;
			}
			return source.SetEqual(toCompareWith, null);
		}


		/// <summary>
		/// 检查要比较的可枚举是否等于源可枚举，元素明智。 如果元素的顺序不同，该方法仍将返回 true。 
		/// 这与考虑顺序的 SequenceEqual 不同
		/// Checks whether the enumerable to compare with is equal to the source enumerable, element wise. If elements are in a different order, the
		/// method will still return true. This is different from SequenceEqual which does take order into account
		/// </summary>
		/// <typeparam name="T">type of the element in the sequences to compare</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="toCompareWith">the sequence to compare with.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>
		/// true if the source and the sequence to compare with have the same elements, regardless of ordering
		/// </returns>
		public static bool SetEqual<T>(this IEnumerable<T> source, IEnumerable<T> toCompareWith, IEqualityComparer<T> comparer) {
			if ((source == null) || (toCompareWith == null)) {
				return false;
			}
			int countSource = source.Count();
			int countToCompareWith = toCompareWith.Count();
			if (countSource != countToCompareWith) {
				return false;
			}
			if (countSource == 0) {
				return true;
			}

			IEqualityComparer<T> comparerToUse = comparer ?? EqualityComparer<T>.Default;
			// check whether the intersection of both sequences has the same number of elements
			return (source.Intersect(toCompareWith, comparerToUse).Count() == countSource);
		}
	}
	/// <summary>
	///IList 相关扩展方法的类。 Class for IList related extension methods.
	/// </summary>
	public static class IListExtensionMethods
	{
		/// <summary>
		///确定传入的列表是否为 null 或为空。 Determines whether the passed in list is null or empty.
		/// </summary>
		/// <typeparam name="T">the type of the elements in the list to check</typeparam>
		/// <param name="toCheck">the list to check.</param>
		/// <returns>true if the passed in list is null or empty, false otherwise</returns>
		public static bool IsNullOrEmpty<T>(this IList<T> toCheck) {
			return ((toCheck == null) || (toCheck.Count <= 0));
		}


		/// <summary>
		///交换索引和索引处的值。 Swaps the values at indexA and indexB.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="indexA">The index for A.</param>
		/// <param name="indexB">The index for B.</param>
		public static void SwapValues<T>(this IList<T> source, int indexA, int indexB) {
			if ((indexA < 0) || (indexA >= source.Count)) {
				throw new IndexOutOfRangeException("indexA is out of range");
			}
			if ((indexB < 0) || (indexB >= source.Count)) {
				throw new IndexOutOfRangeException("indexB is out of range");
			}

			if (indexA == indexB) {
				return;
			}

			T tempValue = source[indexA];
			source[indexA] = source[indexB];
			source[indexB] = tempValue;
		}


		/// <summary>
		///将指定的范围添加到 IList(Of T) 类型的容器中。 Adds the range specified to an IList(Of T) typed container.
		/// </summary>
		/// <typeparam name="T">type of the element in the list</typeparam>
		/// <param name="container">The container.</param>
		/// <param name="rangeToAdd">The range to add.</param>
		public static void AddRange<T>(this IList<T> container, IEnumerable<T> rangeToAdd) {
			if ((container == null) || (rangeToAdd == null)) {
				return;
			}
			foreach (T toAdd in rangeToAdd) {
				container.Add(toAdd);
			}
		}


		/// <summary>
		/// 使用二分法搜索 http://en.wikipedia.org/wiki/Binary_search 在指定的排序列表中搜索指定的元素。 
		/// 该算法在这里重新实现，以便能够在任何排序的 IList 实现数据结构中进行搜索（.NET 的 BCL 仅在数组和 List(Of T) 上实现 BinarySearch。
		/// 如果没有可用的 IComparer(Of T)，请尝试使用 Algorithmia 的 ComparisonBasedComparer，
		/// Searches for the element specified in the sorted list specified using binary search http://en.wikipedia.org/wiki/Binary_search. The algorithm
		/// is re-implemented here to be able to search in any sorted IList implementing data structure (.NET's BCL only implements BinarySearch on arrays and
		/// List(Of T). If no IComparer(Of T) is available, try using Algorithmia's ComparisonBasedComparer, 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sortedList">排序列表The sorted list.</param>
		/// <param name="element">The element.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The index of the element searched or the bitwise complement of the index of the next element that is larger than 
		/// <i>element</i> or if there is no larger element the bitwise complement of Count. Bitwise complements have their original bits negated. Use
		/// the '~' operator in C# to get the real value. Bitwise complements are used to avoid returning a value which is in the range of valid indices so 
		/// callers can't check whether the value returned is an index or if the element wasn't found. If the value returned is negative, the bitwise complement
		/// can be used as index to insert the element in the sorted list to keep the list sorted</returns>
		/// <remarks>假定 sortedList 是升序排序的。 如果传入降序排序列表，请确保也调整了比较器. Assumes that sortedList is sorted ascending. If you pass in a descending sorted list, be sure the comparer is adjusted as well.</remarks>
		public static int BinarySearch<T>(this IList sortedList, T element, IComparer<T> comparer) {
			ArgumentVerifier.CantBeNull(sortedList, "sortedList");
			ArgumentVerifier.CantBeNull(comparer, "comparer");
			if (sortedList.Count <= 0) {
				return -1;
			}

			int left = 0;
			int right = sortedList.Count - 1;
			while (left <= right) {
				// determine the index in the list to compare with. This is the middle of the segment we're searching in.
				int index = left + (right - left) / 2;
				int compareResult = comparer.Compare((T)sortedList[index], element);
				if (compareResult == 0) {
					// found it, done. Return the index
					return index;
				}
				if (compareResult < 0) {
					// element is bigger than the element at index, so we can skip all elements at the left of index including the element at index.
					left = index + 1;
				}
				else {
					// element is smaller than the element at index, so we can skip all elements at the right of index including the element at index.
					right = index - 1;
				}
			}
			return ~left;
		}
	}
}
