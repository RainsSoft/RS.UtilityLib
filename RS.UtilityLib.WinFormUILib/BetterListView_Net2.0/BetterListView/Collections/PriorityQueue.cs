using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   A queue returning item with highest priority first while removing the item from collection.
	///   This implementation differs from System.Collections.SortedList because allows adding items with non-unique priorities.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public class PriorityQueue<TItem> : IEnumerable<TItem>, IEnumerable where TItem : IComparable<TItem>
	{
		private readonly List<TItem> innerList;

		private readonly int capacity;

		/// <summary>
		///   get number of items within queue
		/// </summary>
		public int Count => this.innerList.Count;

		/// <summary>
		///   get item with highest priority
		/// </summary>
		public TItem HeadValue {
			get {
				if (this.innerList.Count != 0) {
					return this.innerList[0];
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   get item with lowest priority
		/// </summary>
		public TItem TailValue {
			get {
				int count = this.innerList.Count;
				if (count != 0) {
					return this.innerList[count - 1];
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   Initialize a new PriorityQueue instance.
		/// </summary>
		public PriorityQueue() {
			this.innerList = new List<TItem>();
			this.capacity = int.MaxValue;
		}

		/// <summary>
		///   Initialize a new PriorityQueue instance.
		/// </summary>
		/// <param name="capacity">queue capacity</param>
		public PriorityQueue(int capacity) {
			Checks.CheckTrue(capacity > 0, "capacity > 0");
			this.innerList = new List<TItem>(capacity);
			this.capacity = capacity;
		}

		/// <summary>
		///   Get read-only collection representing this priority queue.
		/// </summary>
		/// <returns></returns>
		public ReadOnlyCollection<TItem> AsReadOnly() {
			return this.innerList.AsReadOnly();
		}

		/// <summary>
		///   Clear items from the collection.
		/// </summary>
		public void Clear() {
			this.innerList.Clear();
		}

		/// <summary>
		///   Dequeue item from the collection.
		/// </summary>
		/// <returns>item with highest priority, or null</returns>
		public TItem Dequeue() {
			TItem result = default(TItem);
			if (this.innerList.Count != 0) {
				result = this.innerList[0];
				this.innerList.RemoveAt(0);
			}
			return result;
		}

		/// <summary>
		///   Enqueue item in the collection.
		/// </summary>
		/// <param name="item">item to enqueue</param>
		/// <returns>item has been added to queue</returns>
		public bool Enqueue(TItem item) {
			int num = this.innerList.Count;
			if (num == this.capacity) {
				if (item.CompareTo(this.innerList[num - 1]) >= 0) {
					return false;
				}
				this.innerList.RemoveAt(num - 1);
				num--;
			}
			if (num == 0) {
				this.innerList.Add(item);
				return true;
			}
			int num2 = 0;
			int num3 = num - 1;
			int num4 = 0;
			while (num2 != num3) {
				num4 = num2 + num3 >> 1;
				int num5 = item.CompareTo(this.innerList[num4]);
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
			if (item.CompareTo(this.innerList[num4]) <= 0) {
				this.innerList.Insert(num4, item);
			}
			else {
				this.innerList.Insert(num4 + 1, item);
			}
			return true;
		}

		/// <summary>
		/// Check whether contents of queue set equals contents of the specified queue.
		/// </summary>
		/// <param name="other">Queue to check.</param>
		/// <returns>Contents of this queue equals contents of the specified queue.</returns>
		public bool EqualsContent(PriorityQueue<TItem> other) {
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

		/// <summary>
		///   Convert this collection to array.
		/// </summary>
		/// <returns>Array of collection items.</returns>
		public TItem[] ToArray() {
			return this.innerList.ToArray();
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

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
	/// <summary>
	///   A queue returning item with highest priority first while removing the item from collection.
	///   This implementation differs from System.Collections.SortedList because allows adding items with non-unique priorities.
	/// </summary>
	/// <typeparam name="TPriority">prority measure type</typeparam>
	/// <typeparam name="TItem">item type</typeparam>
	public class PriorityQueue<TPriority, TItem> : IEnumerable<TItem>, IEnumerable where TPriority : IComparable<TPriority>
	{
		private readonly List<KeyValuePair<TPriority, TItem>> innerList;

		private readonly int capacity;

		/// <summary>
		///   get number of items within queue
		/// </summary>
		public int Count => this.innerList.Count;

		/// <summary>
		///   get item with highest priority
		/// </summary>
		public TItem HeadValue {
			get {
				if (this.innerList.Count != 0) {
					return this.innerList[0].Value;
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   get the highest priority value
		/// </summary>
		public TPriority HeadPriority {
			get {
				if (this.innerList.Count != 0) {
					return this.innerList[0].Key;
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   get item with lowest priority
		/// </summary>
		public TItem TailValue {
			get {
				int count = this.innerList.Count;
				if (count != 0) {
					return this.innerList[count - 1].Value;
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   get the lowest priority value
		/// </summary>
		public TPriority TailPriority {
			get {
				int count = this.innerList.Count;
				if (count != 0) {
					return this.innerList[count - 1].Key;
				}
				throw new InvalidOperationException("Queue is empty.");
			}
		}

		/// <summary>
		///   Initialize a new PriorityQueue instance.
		/// </summary>
		public PriorityQueue() {
			this.innerList = new List<KeyValuePair<TPriority, TItem>>();
			this.capacity = int.MaxValue;
		}

		/// <summary>
		///   Initialize a new PriorityQueue instance.
		/// </summary>
		/// <param name="capacity">queue capacity</param>
		public PriorityQueue(int capacity) {
			Checks.CheckTrue(capacity > 0, "capacity > 0");
			this.innerList = new List<KeyValuePair<TPriority, TItem>>(capacity);
			this.capacity = capacity;
		}

		/// <summary>
		///   Get read-only collection representing this priority queue.
		/// </summary>
		/// <returns></returns>
		public ReadOnlyCollection<KeyValuePair<TPriority, TItem>> AsReadOnly() {
			return this.innerList.AsReadOnly();
		}

		/// <summary>
		///   Clear items from the collection.
		/// </summary>
		public void Clear() {
			this.innerList.Clear();
		}

		/// <summary>
		///   Dequeue item from the collection.
		/// </summary>
		/// <returns>item with highest priority, or null</returns>
		public TItem Dequeue() {
			TItem result = default(TItem);
			if (this.innerList.Count != 0) {
				result = this.innerList[0].Value;
				this.innerList.RemoveAt(0);
			}
			return result;
		}

		/// <summary>
		///   Enqueue item in the collection.
		/// </summary>
		/// <param name="priority">item priority</param>
		/// <param name="item">item to enqueue</param>
		/// <returns>item has been added to queue</returns>
		public bool Enqueue(TPriority priority, TItem item) {
			int num = this.innerList.Count;
			if (num == this.capacity) {
				if (priority.CompareTo(this.innerList[num - 1].Key) >= 0) {
					return false;
				}
				this.innerList.RemoveAt(num - 1);
				num--;
			}
			if (num == 0) {
				this.innerList.Add(new KeyValuePair<TPriority, TItem>(priority, item));
				return true;
			}
			int num2 = 0;
			int num3 = num - 1;
			int num4 = 0;
			while (num2 != num3) {
				num4 = num2 + num3 >> 1;
				int num5 = priority.CompareTo(this.innerList[num4].Key);
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
			if (priority.CompareTo(this.innerList[num4].Key) <= 0) {
				this.innerList.Insert(num4, new KeyValuePair<TPriority, TItem>(priority, item));
			}
			else {
				this.innerList.Insert(num4 + 1, new KeyValuePair<TPriority, TItem>(priority, item));
			}
			return true;
		}

		/// <summary>
		/// Check whether contents of queue set equals contents of the specified queue.
		/// </summary>
		/// <param name="other">Queue to check.</param>
		/// <returns>Contents of this queue equals contents of the specified queue.</returns>
		public bool EqualsContent(PriorityQueue<TPriority, TItem> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			List<KeyValuePair<TPriority, TItem>> list = this.innerList;
			List<KeyValuePair<TPriority, TItem>> list2 = other.innerList;
			if (list.Count != list2.Count) {
				return false;
			}
			IEnumerator<KeyValuePair<TPriority, TItem>> enumerator = list.GetEnumerator();
			IEnumerator<KeyValuePair<TPriority, TItem>> enumerator2 = list2.GetEnumerator();
			enumerator.Reset();
			enumerator2.Reset();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				KeyValuePair<TPriority, TItem> current = enumerator.Current;
				KeyValuePair<TPriority, TItem> current2 = enumerator2.Current;
				TPriority key = current.Key;
				TPriority key2 = current2.Key;
				TItem value = current.Value;
				TItem value2 = current2.Value;
				if (key.CompareTo(key2) != 0 || !object.Equals(value, value2)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TItem> GetEnumerator() {
			List<TItem> list = new List<TItem>();
			foreach (KeyValuePair<TPriority, TItem> inner in this.innerList) {
				list.Add(inner.Value);
			}
			return list.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}