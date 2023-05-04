using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   List that can keep its items sorted.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	[Serializable]
	public class SortedList<TItem> : IExtendedList<TItem>, IExtendedCollection<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable, ICollection, IList<TItem>, IList
	{
		private const string FieldInnerList = "innerList";

		private readonly IComparer<TItem> comparer;

		private readonly List<TItem> innerList;

		/// <summary>
		///   get or set item at the specified index in the list
		/// </summary>
		/// <param name="index">item index</param>
		/// <returns></returns>
		public TItem this[int index] {
			get {
				Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
				Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
				return this.innerList[index];
			}
			set {
				Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
				Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
				Checks.CheckNotNull(value, "value");
				this.RemoveAt(index);
				this.Add(value);
			}
		}

		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///   The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		public int Count => this.innerList.Count;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly => false;

		bool IList.IsFixedSize => false;

		bool IList.IsReadOnly => false;

		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				Checks.CheckType(value, typeof(TItem), "value");
				this[index] = (TItem)value;
			}
		}

		int ICollection.Count => this.Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this.innerList;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Collections.SortedList`1" /> class.
		///   A default comparer is used.
		/// </summary>
		public SortedList()
			: this((IComparer<TItem>)Comparer<TItem>.Default) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Collections.SortedList`1" /> class.
		///   A default comparer is used.
		/// </summary>
		/// <param name="enumerable">enumerable to create this instance from</param>
		public SortedList(IEnumerable<TItem> enumerable)
			: this((IComparer<TItem>)Comparer<TItem>.Default, enumerable) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Collections.SortedList`1" /> class.
		/// </summary>
		/// <param name="comparer">value comparer</param>
		public SortedList(IComparer<TItem> comparer)
			: this(comparer, (IEnumerable<TItem>)null) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Collections.SortedList`1" /> class.
		/// </summary>
		/// <param name="comparer">value comparer</param>
		/// <param name="enumerable">enumerable to create this instance from</param>
		public SortedList(IComparer<TItem> comparer, IEnumerable<TItem> enumerable) {
			Checks.CheckNotNull(comparer, "comparer");
			this.innerList = ((enumerable != null) ? new List<TItem>(enumerable) : new List<TItem>());
			this.comparer = comparer;
			this.Sort();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.Collections.SortedList`1" /> class.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected SortedList(SerializationInfo info, StreamingContext context) {
			this.innerList = (List<TItem>)info.GetValue("innerList", typeof(List<TItem>));
		}

		/// <summary>
		///   Add the specified item to collection.
		/// </summary>
		/// <param name="item">item to add</param>
		/// <returns>destination index of item</returns>
		public int Add(TItem item) {
			Checks.CheckNotNull(item, "item");
			int count = this.innerList.Count;
			int num;
			if (count == 0) {
				num = 0;
				this.innerList.Add(item);
			}
			else {
				int num2 = 0;
				int num3 = count - 1;
				int num4 = 0;
				while (num2 != num3) {
					num4 = num2 + num3 >> 1;
					int num5 = this.comparer.Compare(item, this.innerList[num4]);
					if (num5 < 0) {
						num3 = num4;
						continue;
					}
					if (num5 <= 0) {
						break;
					}
					if (num4 == num2) {
						num4++;
						break;
					}
					num2 = num4;
				}
				num = ((this.comparer.Compare(item, this.innerList[num4]) > 0) ? (num4 + 1) : num4);
				this.innerList.Insert(num, item);
			}
			return num;
		}

		/// <summary>
		/// Check whether contents of this list equals contents of the specified list.
		/// </summary>
		/// <param name="other">List to check.</param>
		/// <returns>Contents of this list equals contents of the specified list.</returns>
		public bool EqualsContent(SortedList<TItem> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			List<TItem> list = this.innerList;
			List<TItem> list2 = other.innerList;
			if (list.Count != list2.Count) {
				return false;
			}
			IEnumerator<TItem> enumerator = list.GetEnumerator();
			IEnumerator<TItem> enumerator2 = list2.GetEnumerator();
			enumerator.Reset();
			enumerator2.Reset();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				if (!object.Equals(enumerator.Current, enumerator2.Current)) {
					return false;
				}
			}
			return true;
		}

		private void SortInternal(IEnumerable<int> indices) {
			this.SortInternal(indices, null);
		}

		private void SortInternal(IEnumerable<int> indices, IComparer<TItem> comparer) {
			List<TItem> list = new List<TItem>();
			List<int> list2 = new List<int>();
			foreach (int index in indices) {
				list.Add(this.innerList[index]);
				list2.Add(index);
			}
			if (comparer != null) {
				list.Sort(comparer);
			}
			this.innerList.Sort(this.comparer);
			for (int i = 0; i < list.Count; i++) {
				this.innerList[list2[i]] = list[i];
			}
		}

		/// <summary>
		///   Add specified items to the collection.
		/// </summary>
		/// <param name="items">items to add</param>
		public void AddRange(IEnumerable items) {
			bool flag = false;
			Checks.CheckNotNull(items, "items");
			foreach (object item2 in items) {
				Checks.CheckType(item2, typeof(TItem), "value");
				flag = true;
			}
			if (!flag) {
				return;
			}
			foreach (TItem item3 in items) {
				this.innerList.Add(item3);
			}
			this.Sort();
		}

		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="index">start index of the range</param>
		/// <param name="count">length of the range</param>
		/// <returns>range of items from the list</returns>
		public List<TItem> GetRange(int index, int count) {
			return this.innerList.GetRange(index, count);
		}

		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="indices">indices of items to obtain</param>
		/// <returns>range of items from the list</returns>
		public List<TItem> GetRange(IEnumerable<int> indices) {
			Checks.CheckNotNull(indices, "indices");
			int maxValue = this.innerList.Count - 1;
			foreach (int index in indices) {
				Checks.CheckBounds(index, 0, maxValue, "index");
			}
			Set<int> set = new Set<int>();
			foreach (int index2 in indices) {
				set.Add(index2);
			}
			List<int> list = new List<int>(set);
			list.Sort();
			List<TItem> list2 = new List<TItem>();
			foreach (int item in list) {
				list2.Add(this.innerList[item]);
			}
			return list2;
		}

		/// <summary>
		///   Insert specified items to the specified position in the list.
		/// </summary>
		/// <param name="index">insertion position</param>
		/// <param name="items">items to insert</param>
		public void InsertRange(int index, IEnumerable items) {
			bool flag = false;
			Checks.CheckBounds(index, 0, this.innerList.Count, "index");
			Checks.CheckNotNull(items, "elements");
			foreach (object item2 in items) {
				Checks.CheckType(item2, typeof(TItem), "value");
				flag = true;
			}
			if (!flag) {
				return;
			}
			List<TItem> list = new List<TItem>();
			foreach (TItem item3 in items) {
				list.Add(item3);
			}
			this.innerList.InsertRange(index, list);
			this.Sort();
		}

		/// <summary>
		///   Remove items at the specified positions from the list.
		/// </summary>
		/// <param name="index">index of the first item to be removed</param>
		/// <param name="count">number of item to be removed</param>
		public void RemoveRange(int index, int count) {
			Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
			Checks.CheckTrue(count >= 0, "count >= 0");
			Checks.CheckBounds(index + count, 0, this.innerList.Count, "index + count");
			if (count != 0) {
				this.innerList.RemoveRange(index, count);
			}
		}

		/// <summary>
		///   Remove items with the specified items from the list.
		/// </summary>
		/// <param name="indices">indices of items to remove</param>
		public void RemoveRange(IEnumerable<int> indices) {
			bool flag = false;
			Checks.CheckNotNull(indices, "indices");
			int maxValue = this.innerList.Count - 1;
			foreach (int index2 in indices) {
				Checks.CheckBounds(index2, 0, maxValue, "index");
				flag = true;
			}
			if (!flag) {
				return;
			}
			Set<int> set = new Set<int>();
			List<int> list = new List<int>();
			foreach (int index3 in indices) {
				if (!set.Contains(index3)) {
					set.Add(index3);
					list.Add(index3);
				}
			}
			if (list.Count != 0) {
				list.Sort();
				for (int num = list.Count - 1; num >= 0; num--) {
					int index = list[num];
					this.innerList.RemoveAt(index);
				}
			}
		}

		/// <summary>
		///   Remove specified items from the collection.
		/// </summary>
		/// <param name="items">items to remove</param>
		public void RemoveRange(IEnumerable items) {
			bool flag = false;
			Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
			Checks.CheckNotNull(items, "items");
			foreach (object item2 in items) {
				Checks.CheckType(item2, typeof(TItem), "value");
				flag = true;
			}
			if (!flag) {
				return;
			}
			foreach (TItem item3 in items) {
				this.Remove(item3);
			}
		}

		/// <summary>
		///   Sort this list.
		/// </summary>
		public void Sort() {
			this.Sort(null);
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="comparer">item comparer</param>
		public void Sort(IComparer<TItem> comparer) {
			if (this.innerList.Count != 0) {
				if (comparer != null) {
					this.innerList.Sort(comparer);
				}
				this.innerList.Sort(this.comparer);
			}
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="index">start index of item interval to sort</param>
		/// <param name="count">number of item within interval to sort</param>
		public void SortRange(int index, int count) {
			this.SortRange(index, count, null);
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="index">start index of item interval to sort</param>
		/// <param name="count">number of item within interval to sort</param>
		/// <param name="comparer">item comparer</param>
		public void SortRange(int index, int count, IComparer<TItem> comparer) {
			if (this.innerList.Count != 0 && count != 0) {
				Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
				Checks.CheckTrue(index + count <= this.innerList.Count, "(index + count) <= this.innerList.Count");
				if (comparer != null) {
					this.innerList.Sort(index, count, comparer);
				}
				this.innerList.Sort(index, count, this.comparer);
			}
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="indices">indices of items to sort</param>
		public void SortRange(IEnumerable<int> indices) {
			this.SortRange(indices, null);
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="indices">indices of items to sort</param>
		/// <param name="comparer">item comparer</param>
		public void SortRange(IEnumerable<int> indices, IComparer<TItem> comparer) {
			if (this.innerList.Count == 0) {
				return;
			}
			bool flag = false;
			Checks.CheckNotNull(indices, "indices");
			int maxValue = this.innerList.Count - 1;
			foreach (int index in indices) {
				Checks.CheckBounds(index, 0, maxValue, "index");
				flag = true;
			}
			if (flag) {
				this.SortInternal(indices, comparer);
			}
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="items">items to sort</param>
		public void SortRange(IEnumerable items) {
			this.SortRange(items, null);
		}

		/// <summary>
		///   Sort items in the list.
		/// </summary>
		/// <param name="items">items to sort</param>
		/// <param name="comparer">item comparer</param>
		public void SortRange(IEnumerable items, IComparer<TItem> comparer) {
			if (this.innerList.Count == 0) {
				return;
			}
			bool flag = false;
			Checks.CheckNotNull(items, "items");
			foreach (object item3 in items) {
				Checks.CheckType(item3, typeof(TItem), "value");
				flag = true;
			}
			if (!flag) {
				return;
			}
			List<int> list = new List<int>();
			Set<TItem> set = new Set<TItem>();
			foreach (TItem item4 in items) {
				set.Add(item4);
			}
			for (int i = 0; i < this.innerList.Count; i++) {
				TItem item2 = this.innerList[i];
				if (!set.Contains(item2)) {
					set.Add(item2);
					list.Add(i);
				}
			}
			this.SortInternal(list, comparer);
		}

		/// <summary>
		///   Try to get item at the specified index.
		/// </summary>
		/// <param name="index">index of the item</param>
		/// <param name="item">item to get</param>
		/// <returns>success</returns>
		public bool TryGetItem(int index, out TItem item) {
			if (index < 0 || index >= this.Count) {
				item = default(TItem);
				return false;
			}
			item = this[index];
			return true;
		}

		/// <summary>
		///   Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </summary>
		/// <returns>
		///   The index of <paramref name="item" /> if found in the list; otherwise, -1.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </param>
		public int IndexOf(TItem item) {
			Checks.CheckNotNull(item, "item");
			return this.innerList.BinarySearch(item, this.comparer);
		}

		/// <summary>
		///   Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.
		/// </param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
		/// </exception>
		public void Insert(int index, TItem item) {
			Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
			Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
			Checks.CheckNotNull(item, "item");
			this.Add(item);
		}

		/// <summary>
		///   Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
		/// </exception>
		public void RemoveAt(int index) {
			Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
			Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
			this.innerList.RemoveAt(index);
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </exception>
		void ICollection<TItem>.Add(TItem item) {
			this.Add(item);
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
		/// </exception>
		public void Clear() {
			this.innerList.Clear();
		}

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
		/// </summary>
		/// <returns>
		///   true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		public bool Contains(TItem item) {
			Checks.CheckNotNull(item, "item");
			return this.innerList.BinarySearch(item, this.comparer) >= 0;
		}

		/// <summary>
		///   Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.
		/// </exception>
		/// <exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.
		///   -or-
		///   <paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.
		///   -or-
		///   The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
		/// </exception>
		public void CopyTo(TItem[] array, int arrayIndex) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(arrayIndex, 0, "arrayIndex", "0");
			}
			else {
				Checks.CheckBounds(arrayIndex, 0, array.Length - 1, "arrayIndex");
			}
			this.innerList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///   Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///   true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </exception>
		public bool Remove(TItem item) {
			int num = this.innerList.BinarySearch(item, this.comparer);
			if (num >= 0) {
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TItem> GetEnumerator() {
			return this.innerList.GetEnumerator();
		}

		int IList.Add(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Add((TItem)value);
			return this.innerList.Count - 1;
		}

		void IList.Clear() {
			this.Clear();
		}

		bool IList.Contains(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			return this.Contains((TItem)value);
		}

		int IList.IndexOf(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			return this.IndexOf((TItem)value);
		}

		void IList.Insert(int index, object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Insert(index, (TItem)value);
		}

		void IList.Remove(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Remove((TItem)value);
		}

		void IList.RemoveAt(int index) {
			this.RemoveAt(index);
		}

		void ICollection.CopyTo(Array array, int arrayIndex) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(arrayIndex, 0, "arrayIndex", "0");
			}
			else {
				Checks.CheckBounds(arrayIndex, 0, array.Length - 1, "arrayIndex");
			}
			for (int i = arrayIndex; i < Math.Min(array.Length, this.innerList.Count); i++) {
				array.SetValue(this[i], i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.innerList.GetEnumerator();
		}
	}
}