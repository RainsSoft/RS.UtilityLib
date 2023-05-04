using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common collection of BetterListView elements.
	/// </summary>
	/// <typeparam name="TElement">type of BetterListView element</typeparam>
	[Serializable]
	[ListBindable(false)]
	public abstract class BetterListViewElementCollection<TElement> : BetterListViewElementCollectionBase, IBetterListViewElementCollection<TElement>, IExtendedList<TElement>, IExtendedCollection<TElement>, ICollection<TElement>, IEnumerable<TElement>, IEnumerable, ICollection, IList<TElement>, IList where TElement : BetterListViewElementBase
	{
		/// <summary>
		///   'innerList' field name
		/// </summary>
		protected const string FieldInnerList = "innerList";

		private List<TElement> innerList;

		private bool isSynchronizing;

		/// <summary>
		///   underlying list data structure
		/// </summary>
		[Browsable(false)]
		protected List<TElement> InnerList => this.innerList;

		/// <summary>
		///   the collection is in synchronization state
		/// </summary>
		[Browsable(false)]
		internal bool IsSynchronizing => this.isSynchronizing;

		/// <summary>
		///   name of the collection element
		/// </summary>
		protected abstract string ElementName { get; }

		/// <summary>
		///   Gets BetterListView element.
		/// </summary>
		/// <param name="key">key of the element within underlying list</param>
		/// <returns>element instance</returns>
		public TElement this[string key] => (TElement)BetterListViewElementCollectionBase.GetElementByKey(this.innerList, key);

		/// <summary>
		///   Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>
		///   The element at the specified index.
		/// </returns>
		/// <param name="index">The zero-based index of the element to get or set.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
		/// </exception>
		public virtual TElement this[int index] {
			get {
				return this.innerList[index];
			}
			set {
				Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
				Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
				if (base.IsInternal) {
					this.CheckElement(value, ElementCheckType.NotContainedInOther);
				}
				TElement val = this.innerList[index];
				if (val == value) {
					return;
				}
				if (base.IsInternal && value.OwnerCollection != null) {
					this.innerList.RemoveAt(value.Index);
					if (value.Index > val.Index) {
						index--;
					}
				}
				this.innerList[index] = value;
				this.PerformCollectionChanged(value.Index, BetterListViewElementCollectionChangeType.Set, new TElement[2] { val, value }, new int[2] { value.Index, value.Index });
			}
		}

		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///   The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		public virtual int Count => this.innerList.Count;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		/// </returns>
		public virtual bool IsReadOnly => false;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		bool IList.IsFixedSize => false;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.IList" /> is read-only; otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		bool IList.IsReadOnly => false;

		/// <summary>
		///   Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>
		///   The element at the specified index.
		/// </returns>
		/// <param name="index">The zero-based index of the element to get or set. 
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. 
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList" /> is read-only. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				Checks.CheckType(value, typeof(TElement), "value");
				this[index] = (TElement)value;
			}
		}

		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>
		///   The number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		int ICollection.Count => this.Count;

		/// <summary>
		///   Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <returns>
		///   true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		bool ICollection.IsSynchronized => false;

		/// <summary>
		///   Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>
		///   An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		object ICollection.SyncRoot => this.innerList;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementCollection`1" /> class.
		/// </summary>
		/// <param name="isInternal">this collection is internal</param>
		protected BetterListViewElementCollection(bool isInternal)
			: this(isInternal, (IEnumerable<TElement>)null) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementCollection`1" /> class.
		/// </summary>
		/// <param name="isInternal">this collection is internal</param>
		/// <param name="elements">enumerable to create this collection from</param>
		protected BetterListViewElementCollection(bool isInternal, IEnumerable<TElement> elements)
			: base(isInternal) {
			if (elements != null) {
				if (base.IsInternal) {
					foreach (TElement element in elements) {
						this.CheckElement(element, ElementCheckType.NotContainedInOther);
					}
				}
				this.innerList = new List<TElement>(elements);
				if (!base.IsInternal) {
					return;
				}
				int num = 0;
				{
					foreach (TElement element2 in elements) {
						TElement current2 = element2;
						current2.OwnerCollection = this;
						current2.Index = num++;
					}
					return;
				}
			}
			this.innerList = new List<TElement>();
		}

		/// <summary>
		///   Add the specified element to collection.
		/// </summary>
		/// <param name="value">element to add</param>
		public void Add(object value) {
			this.AddRange(new object[1] { value });
		}

		public void AddRange(object[] values) {
			this.AddRange((IEnumerable)values);
		}

		/// <summary>
		/// Check whether content of this collection is equal to content of the specified list.
		/// </summary>
		/// <param name="other">List to check.</param>
		/// <returns>Content of this collection is equal to content of the specified list.</returns>
		public bool EqualsContent(IList<TElement> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			IList<TElement> list = this.innerList;
			if (list.Count != other.Count) {
				return false;
			}
			IEnumerator<TElement> enumerator = list.GetEnumerator();
			IEnumerator<TElement> enumerator2 = other.GetEnumerator();
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
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return this.ToString(writeContent: false);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <param name="writeContent">output content of the collection</param>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public string ToString(bool writeContent) {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetType().Name);
			if (writeContent) {
				stringBuilder.Append(": {");
				foreach (TElement inner in this.InnerList) {
					stringBuilder.AppendFormat("'{0}', ", inner);
				}
				if (this.InnerList.Count != 0) {
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
				}
				stringBuilder.Append("}");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		///   Check whether this collection contains element with the specified key.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <returns>This collection contains element with the specified key.</returns>
		public virtual bool ContainsKey(string key) {
			return BetterListViewElementCollectionBase.GetElementByKey(this, key) != null;
		}

		/// <summary>
		///   Get index of element with the specified key.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <returns>Index of element with the specified key if found, -1 otherwise.</returns>
		public virtual int IndexOfKey(string key) {
			return BetterListViewElementCollectionBase.GetElementByKey(this, key)?.Index ?? (-1);
		}

		/// <summary>
		/// Remove element with the specified key from the collection.
		/// </summary>
		/// <param name="key">Search key.</param>
		public virtual void RemoveByKey(string key) {
			int num = this.IndexOfKey(key);
			if (num != -1) {
				this.RemoveAt(num);
			}
		}

		/// <summary>
		///   Check whether the specified element is properly related to this collection.
		/// </summary>
		/// <param name="element">element to check</param>
		/// <param name="checkType">element check type</param>
		internal void CheckElement(TElement element, ElementCheckType checkType) {
			Checks.CheckNotNull(element, "element");
			if (checkType == ElementCheckType.ContainedInThis) {
				Checks.CheckTrue(element.OwnerCollection == this, "ReferenceEquals(element.OwnerCollection, this)", $"{element} '{this.ElementName}' is not contained in this collection.");
			}
			if (checkType == ElementCheckType.NotContainedInThis || checkType == ElementCheckType.NotContainedInAny) {
				Checks.CheckFalse(element.OwnerCollection == this, "ReferenceEquals(element.OwnerCollection, this)", $"{element} '{this.ElementName}' is contained in this collection.");
			}
			if (checkType == ElementCheckType.NotContainedInOther || checkType == ElementCheckType.NotContainedInAny) {
				Checks.CheckTrue(element.OwnerCollection == null || element.OwnerCollection == this || !element.OwnerCollection.IsInternal, "(element.OwnerCollection == null) || ReferenceEquals(element.OwnerCollection, this) || (element.OwnerCollection.IsInternal == false)", $"{element} '{this.ElementName}' is contained in some other collection.");
			}
		}

		/// <summary>
		///   Start synchronization mode - flag all changes as synchronization changes.
		/// </summary>
		internal void BeginSync() {
			this.isSynchronizing = true;
		}

		/// <summary>
		///   End synchronization mode - flag all changes as normal changes.
		/// </summary>
		internal void EndSync() {
			this.isSynchronizing = false;
		}

		/// <summary>
		///   Creeate a new element from its text.
		/// </summary>
		/// <param name="text">element text</param>
		/// <returns>element instance</returns>
		protected abstract TElement CreateElement(string text);

		private static List<int> GetElementIndices(IList<TElement> elements) {
			List<int> list = new List<int>();
			foreach (TElement element in elements) {
				TElement current = element;
				list.Add(current.Index);
			}
			return list;
		}

		private List<TElement> FilterEnumerable(IEnumerable items, bool removePresentItems) {
			Checks.CheckNotNull(items, "items");
			List<TElement> list = new List<TElement>();
			foreach (object item in items) {
				if (item is TElement val) {
					val.DuplicityFlag = false;
				}
			}
			foreach (object item2 in items) {
				TElement val2 = item2 as TElement;
				if (val2 == null) {
					if (item2 == null) {
						val2 = this.CreateElement(string.Empty);
					}
					else {
						TypeConverter converter = TypeDescriptor.GetConverter(item2);
						if (converter != null) {
							if (converter.CanConvertTo(typeof(TElement))) {
								val2 = (TElement)converter.ConvertTo(item2, typeof(TElement));
							}
							else if (converter.CanConvertTo(typeof(string))) {
								val2 = this.CreateElement(converter.ConvertToString(item2));
							}
						}
						if (val2 == null) {
							val2 = this.CreateElement(item2.ToString());
						}
					}
					if (val2 == null) {
						throw new NotSupportedException("This collection does not support creating elements from text.");
					}
				}
				else {
					if ((removePresentItems && val2.OwnerCollection == this) || val2.DuplicityFlag) {
						continue;
					}
					val2.DuplicityFlag = true;
				}
				list.Add(val2);
			}
			return list;
		}

		private void SortInternal(IEnumerable<int> indices, IComparer<TElement> comparer) {
			List<TElement> list = new List<TElement>();
			List<int> list2 = new List<int>();
			foreach (int index in indices) {
				list.Add(this.innerList[index]);
				list2.Add(index);
			}
			if (comparer != null) {
				list.Sort(comparer);
			}
			else {
				list.Sort();
			}
			for (int i = 0; i < list.Count; i++) {
				this.innerList[list2[i]] = list[i];
			}
		}

		private void PerformCollectionChanged(int reindexStart, BetterListViewElementCollectionChangeType changeType, IList<TElement> elements, IList<int> indices) {
			this.PerformCollectionChanged(reindexStart, this.innerList.Count - 1, changeType, elements, indices);
		}

		private void PerformCollectionChanged(int reindexStart, int reindexEnd, BetterListViewElementCollectionChangeType changeType, IList<TElement> elements, IList<int> indices) {
			if (base.IsInternal) {
				switch (changeType) {
					case BetterListViewElementCollectionChangeType.Add:
						foreach (TElement element in elements) {
							TElement current2 = element;
							current2.OwnerCollection = this;
							current2.ResetIndex();
						}
						break;
					case BetterListViewElementCollectionChangeType.Remove:
						foreach (TElement element2 in elements) {
							TElement current = element2;
							current.OwnerCollection = null;
							current.ResetIndex();
						}
						break;
					case BetterListViewElementCollectionChangeType.Set: {
							TElement val = elements[0];
							val.OwnerCollection = null;
							val.ResetIndex();
							TElement val2 = elements[1];
							val2.OwnerCollection = this;
							val2.ResetIndex();
							break;
						}
				}
				if (this.innerList.Count != 0) {
					reindexStart = Math.Min(Math.Max(reindexStart, 0), this.innerList.Count - 1);
					reindexEnd = Math.Min(Math.Max(reindexEnd, 0), this.innerList.Count - 1);
					for (int i = reindexStart; i <= reindexEnd; i++) {
						TElement val3 = this.innerList[i];
						val3.Index = i;
					}
				}
			}
			ReadWriteDictionary<BetterListViewElementBase, int> readWriteDictionary = new ReadWriteDictionary<BetterListViewElementBase, int>(new Dictionary<BetterListViewElementBase, int>());
			for (int j = 0; j < elements.Count; j++) {
				readWriteDictionary.Add(elements[j], indices[j]);
			}
			this.OnCollectionChanged(new BetterListViewElementCollectionChangeInfo(changeType, new ReadOnlyDictionary<BetterListViewElementBase, int>(readWriteDictionary), this.isSynchronizing));
		}

		/// <summary>
		///   Clone elements of this collection to a new array.
		/// </summary>
		/// <returns></returns>
		public TElement[] CloneToArray() {
			int count = this.Count;
			TElement[] array = new TElement[count];
			for (int i = 0; i < count; i++) {
				int num = i;
				TElement val = this[i];
				array[num] = (TElement)val.Clone();
			}
			return array;
		}

		/// <summary>
		///   Add specified items to the collection.
		/// </summary>
		/// <param name="items">items to add</param>
		public void AddRange(IEnumerable items) {
			this.InsertRange(this.innerList.Count, items);
		}

		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="index">start index of the range</param>
		/// <param name="count">length of the range</param>
		/// <returns>range of items from the list</returns>
		public List<TElement> GetRange(int index, int count) {
			return this.innerList.GetRange(index, count);
		}

		/// <summary>
		///   Get the specified range of items from the list.
		/// </summary>
		/// <param name="indices">indices of items to obtain</param>
		/// <returns>range of items from the list</returns>
		public List<TElement> GetRange(IEnumerable<int> indices) {
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
			List<TElement> list2 = new List<TElement>();
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
			List<TElement> list = this.FilterEnumerable(items, removePresentItems: true);
			Checks.CheckBounds(index, 0, this.innerList.Count, "index");
			if (base.IsInternal) {
				foreach (TElement item in list) {
					this.CheckElement(item, ElementCheckType.NotContainedInAny);
				}
			}
			if (list.Count != 0) {
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(list);
				this.innerList.InsertRange(index, list);
				this.PerformCollectionChanged(index, BetterListViewElementCollectionChangeType.Add, list, elementIndices);
			}
		}

		/// <summary>
		///   Remove items at the specified positions from the list.
		/// </summary>
		/// <param name="index">index of the first item to be removed</param>
		/// <param name="count">number of item to be removed</param>
		public void RemoveRange(int index, int count) {
			Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
			Checks.CheckBounds(index + count, 0, this.innerList.Count, "index + count");
			if (count != 0) {
				List<TElement> range = this.innerList.GetRange(index, count);
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(range);
				this.innerList.RemoveRange(index, count);
				this.PerformCollectionChanged(index, BetterListViewElementCollectionChangeType.Remove, range, elementIndices);
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
			foreach (int index3 in indices) {
				Checks.CheckBounds(index3, 0, maxValue, "index");
				flag = true;
			}
			if (!flag) {
				return;
			}
			Set<int> set = new Set<int>();
			List<int> list = new List<int>();
			foreach (int index4 in indices) {
				if (!set.Contains(index4)) {
					set.Add(index4);
					list.Add(index4);
				}
			}
			if (list.Count == 0) {
				return;
			}
			list.Sort();
			List<TElement> list2 = new List<TElement>();
			if (base.IsInternal) {
				for (int num = list.Count - 1; num >= 0; num--) {
					int index = list[num];
					TElement item = this.innerList[index];
					this.innerList.RemoveAt(index);
					list2.Insert(0, item);
				}
			}
			else {
				for (int num2 = list.Count - 1; num2 >= 0; num2--) {
					int index2 = list[num2];
					TElement item2 = this.innerList[index2];
					this.innerList.RemoveAt(index2);
					list2.Insert(0, item2);
				}
			}
			this.PerformCollectionChanged(list[0], BetterListViewElementCollectionChangeType.Remove, list2, list);
		}

		/// <summary>
		///   Remove specified items from the collection.
		/// </summary>
		/// <param name="items">items to remove</param>
		public void RemoveRange(IEnumerable items) {
			List<TElement> list = this.FilterEnumerable(items, removePresentItems: false);
			Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
			Checks.CheckNotNull(items, "items");
			if (base.IsInternal) {
				foreach (TElement item in items) {
					this.CheckElement(item, ElementCheckType.ContainedInThis);
				}
			}
			if (list.Count == 0) {
				return;
			}
			list.Sort(BetterListViewElementBaseIndexComparer<TElement>.Instance);
			List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(list);
			if (base.IsInternal) {
				for (int num = list.Count - 1; num >= 0; num--) {
					List<TElement> list2 = this.innerList;
					TElement val = list[num];
					list2.RemoveAt(val.Index);
				}
			}
			else {
				foreach (TElement item2 in list) {
					this.innerList.Remove(item2);
				}
			}
			this.PerformCollectionChanged(elementIndices[0], BetterListViewElementCollectionChangeType.Remove, list, elementIndices);
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
		public void Sort(IComparer<TElement> comparer) {
			if (this.innerList.Count != 0) {
				Checks.CheckNotNull(comparer, "comparer");
				List<TElement> elements = new List<TElement>(this.innerList);
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(elements);
				this.innerList.Sort(comparer ?? BetterListViewElementBaseIndexComparer<TElement>.Instance);
				this.PerformCollectionChanged(0, BetterListViewElementCollectionChangeType.Sort, elements, elementIndices);
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
		public void SortRange(int index, int count, IComparer<TElement> comparer) {
			Checks.CheckTrue(index >= 0 && count >= 0, "index >= 0 && count >= 0");
			if (count != 0 && this.innerList.Count != 0) {
				Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
				Checks.CheckBounds(index + count, 0, this.innerList.Count, "index + count");
				List<TElement> range = this.GetRange(index, count);
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(range);
				this.innerList.Sort(index, count, comparer ?? BetterListViewElementBaseIndexComparer<TElement>.Instance);
				this.PerformCollectionChanged(0, BetterListViewElementCollectionChangeType.Sort, range, elementIndices);
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
		public void SortRange(IEnumerable<int> indices, IComparer<TElement> comparer) {
			if (this.innerList.Count == 0) {
				return;
			}
			int minValue = 0;
			int maxValue = this.innerList.Count - 1;
			bool flag = false;
			Checks.CheckNotNull(indices, "indices");
			foreach (int index in indices) {
				Checks.CheckBounds(index, minValue, maxValue, "index");
				flag = true;
			}
			if (flag) {
				List<TElement> range = this.GetRange(indices);
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(range);
				List<int> list = new List<int>(indices);
				list.Sort();
				this.SortInternal(indices, comparer ?? BetterListViewElementBaseIndexComparer<TElement>.Instance);
				this.PerformCollectionChanged(list[0], list[list.Count - 1], BetterListViewElementCollectionChangeType.Sort, range, elementIndices);
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
		public void SortRange(IEnumerable items, IComparer<TElement> comparer) {
			if (this.innerList.Count == 0) {
				return;
			}
			List<TElement> list = this.FilterEnumerable(items, removePresentItems: false);
			List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(list);
			if (list.Count == 0) {
				return;
			}
			if (base.IsInternal) {
				foreach (TElement item2 in items) {
					this.CheckElement(item2, ElementCheckType.ContainedInThis);
				}
			}
			List<int> list2 = new List<int>();
			if (base.IsInternal) {
				foreach (TElement item3 in list) {
					TElement current = item3;
					int index = current.Index;
					list2.Add(index);
				}
			}
			else {
				foreach (TElement item4 in list) {
					int item = this.IndexOf(item4);
					list2.Add(item);
				}
			}
			list2.Sort();
			this.SortInternal(list2, comparer ?? BetterListViewElementBaseIndexComparer<TElement>.Instance);
			this.PerformCollectionChanged(list2[0], list2[list2.Count - 1], BetterListViewElementCollectionChangeType.Sort, list, elementIndices);
		}

		/// <summary>
		///   Try to get item at the specified index.
		/// </summary>
		/// <param name="index">index of the item</param>
		/// <param name="item">item to get</param>
		/// <returns>success</returns>
		public bool TryGetItem(int index, out TElement item) {
			if (index < 0 || index >= this.Count) {
				item = null;
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
		public virtual int IndexOf(TElement item) {
			if (base.IsInternal) {
				this.CheckElement(item, ElementCheckType.ContainedInThis);
			}
			else {
				Checks.CheckNotNull(item, "item");
			}
			if (!base.IsInternal) {
				return this.innerList.IndexOf(item);
			}
			return item.Index;
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
		public virtual void Insert(int index, TElement item) {
			Checks.CheckBounds(index, 0, this.innerList.Count, "index");
			if (base.IsInternal) {
				this.CheckElement(item, ElementCheckType.NotContainedInAny);
			}
			else {
				Checks.CheckNotNull(item, "element");
			}
			this.innerList.Insert(index, item);
			this.PerformCollectionChanged(index, BetterListViewElementCollectionChangeType.Add, new TElement[1] { item }, new int[1] { -1 });
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
		public virtual void RemoveAt(int index) {
			Checks.CheckTrue(this.innerList.Count != 0, "this.innerList.Count != 0");
			Checks.CheckBounds(index, 0, this.innerList.Count - 1, "index");
			TElement val = this[index];
			this.innerList.RemoveAt(index);
			this.PerformCollectionChanged(index, BetterListViewElementCollectionChangeType.Remove, new TElement[1] { val }, new int[1] { index });
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </exception>
		public virtual void Add(TElement item) {
			if (item != null && item.OwnerCollection != this) {
				if (base.IsInternal) {
					this.CheckElement(item, ElementCheckType.NotContainedInAny);
				}
				this.innerList.Add(item);
				this.PerformCollectionChanged(this.innerList.Count - 1, BetterListViewElementCollectionChangeType.Add, new TElement[1] { item }, new int[1] { -1 });
			}
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
		/// </exception>
		public virtual void Clear() {
			if (this.innerList.Count != 0) {
				List<TElement> elements = new List<TElement>(this.innerList);
				List<int> elementIndices = BetterListViewElementCollection<TElement>.GetElementIndices(elements);
				this.innerList.Clear();
				this.PerformCollectionChanged(0, BetterListViewElementCollectionChangeType.Remove, elements, elementIndices);
			}
		}

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
		/// </summary>
		/// <returns>
		///   true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		public virtual bool Contains(TElement item) {
			Checks.CheckNotNull(item, "item");
			if (!base.IsInternal) {
				return this.innerList.Contains(item);
			}
			return item.OwnerCollection == this;
		}

		/// <summary>
		///   Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.
		/// </param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than 0.
		/// </exception>
		/// <exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.
		///   -or-
		///   <paramref name="index" /> is equal to or greater than the length of <paramref name="array" />.
		///   -or-
		///   The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.
		/// </exception>
		public virtual void CopyTo(TElement[] array, int index) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(index, 0, "index", "0");
			}
			else {
				Checks.CheckBounds(index, 0, array.Length - 1, "index");
			}
			this.innerList.CopyTo(array, index);
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
		public virtual bool Remove(TElement item) {
			Checks.CheckNotNull(item, "item");
			bool result;
			int num;
			if (base.IsInternal) {
				if (this.Contains(item)) {
					result = true;
					num = item.Index;
					this.innerList.RemoveAt(num);
				}
				else {
					result = false;
					num = -1;
				}
			}
			else {
				result = this.innerList.Remove(item);
				num = -1;
			}
			this.PerformCollectionChanged(num, BetterListViewElementCollectionChangeType.Remove, new TElement[1] { item }, new int[1] { num });
			return result;
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public virtual IEnumerator<TElement> GetEnumerator() {
			return this.innerList.GetEnumerator();
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <returns>
		///   The position into which the new element was inserted.
		/// </returns>
		/// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />. 
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.
		///   -or- 
		///   The <see cref="T:System.Collections.IList" /> has a fixed size. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		int IList.Add(object value) {
			Checks.CheckType(value, typeof(TElement), "value");
			this.Add((TElement)value);
			return this.innerList.Count - 1;
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		void IList.Clear() {
			this.Clear();
		}

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, false.
		/// </returns>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		bool IList.Contains(object value) {
			Checks.CheckType(value, typeof(TElement), "value");
			return this.Contains((TElement)value);
		}

		/// <summary>
		///   Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <returns>
		///   The index of <paramref name="value" /> if found in the list; otherwise, -1.
		/// </returns>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		int IList.IndexOf(object value) {
			Checks.CheckType(value, typeof(TElement), "value");
			return this.IndexOf((TElement)value);
		}

		/// <summary>
		///   Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted. 
		/// </param>
		/// <param name="value">The <see cref="T:System.Object" /> to insert into the <see cref="T:System.Collections.IList" />. 
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. 
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.
		///   -or- 
		///   The <see cref="T:System.Collections.IList" /> has a fixed size. 
		/// </exception>
		/// <exception cref="T:System.NullReferenceException"><paramref name="value" /> is null reference in the <see cref="T:System.Collections.IList" />.
		/// </exception>
		/// <filterpriority>2</filterpriority>
		void IList.Insert(int index, object value) {
			Checks.CheckType(value, typeof(TElement), "value");
			this.Insert(index, (TElement)value);
		}

		/// <summary>
		///   Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />. 
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.
		///   -or- 
		///   The <see cref="T:System.Collections.IList" /> has a fixed size. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		void IList.Remove(object value) {
			Checks.CheckType(value, typeof(TElement), "value");
			this.Remove((TElement)value);
		}

		/// <summary>
		///   Removes the <see cref="T:System.Collections.IList" /> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove. 
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. 
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only.
		///   -or- 
		///   The <see cref="T:System.Collections.IList" /> has a fixed size. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		void IList.RemoveAt(int index) {
			this.RemoveAt(index);
		}

		/// <summary>
		///   Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing. 
		/// </param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins. 
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null. 
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than zero. 
		/// </exception>
		/// <exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.
		///   -or- 
		///   <paramref name="index" /> is equal to or greater than the length of <paramref name="array" />.
		///   -or- 
		///   The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />. 
		/// </exception>
		/// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />. 
		/// </exception>
		/// <filterpriority>2</filterpriority>
		void ICollection.CopyTo(Array array, int index) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(index, 0, "index", "0");
			}
			else {
				Checks.CheckBounds(index, 0, array.Length - 1, "index");
			}
			for (int i = index; i < Math.Min(array.Length, this.innerList.Count); i++) {
				array.SetValue(this[i], i);
			}
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

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementCollection`1" /> class.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected BetterListViewElementCollection(SerializationInfo info, StreamingContext context)
			: base(isInternal: false) {
			this.innerList = (List<TElement>)info.GetValue("innerList", typeof(List<TElement>));
		}

		/// <summary>
		///   Called when serializing this object.
		/// </summary>
		/// <param name="context">context of the serialized stream</param>
		[OnSerializing]
		protected void OnSerializing(StreamingContext context) {
		}

		/// <summary>
		///   Called when this object has been serialized.
		/// </summary>
		/// <param name="context">context of the serialized stream</param>
		[OnSerialized]
		protected void OnSerialized(StreamingContext context) {
		}

		/// <summary>
		///   Called when deserializing this object.
		/// </summary>
		/// <param name="context">context of the serialized stream</param>
		[OnDeserializing]
		protected void OnDeserializing(StreamingContext context) {
		}

		/// <summary>
		///   Called when this object has been deserialized.
		/// </summary>
		/// <param name="context">context of the serialized stream</param>
		[OnDeserialized]
		protected void OnDeserialized(StreamingContext context) {
			for (int i = 0; i < this.innerList.Count; i++) {
				TElement val = this.innerList[i];
				val.Index = i;
				val.OwnerCollection = this;
			}
		}

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		///   The caller does not have the required permission.
		/// </exception>
		protected override void GetObjectDataInternal(SerializationInfo info, StreamingContext context) {
			info.AddValue("innerList", this.innerList, typeof(List<TElement>));
		}

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		protected override void ReadXmlInternal(XmlReader reader) {
			reader.MoveToContent();
			reader.ReadStartElement();
			reader.ReadStartElement("innerList");
			this.innerList = (List<TElement>)new XmlSerializer(typeof(List<TElement>)).Deserialize(reader);
			reader.ReadEndElement();
			reader.ReadEndElement();
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		protected override void WriteXmlInternal(XmlWriter writer) {
			writer.WriteStartElement("innerList");
			new XmlSerializer(typeof(List<TElement>)).Serialize(writer, this.innerList);
			writer.WriteEndElement();
		}
	}
}