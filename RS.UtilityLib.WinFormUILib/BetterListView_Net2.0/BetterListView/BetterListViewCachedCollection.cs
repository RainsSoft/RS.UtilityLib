using System;
using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common collection tied to Better ListView state.
	/// </summary>
	/// <typeparam name="TItem">Collection item type.</typeparam>
	public abstract class BetterListViewCachedCollection<TItem> : IList<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable, IList, ICollection
	{
		private readonly List<TItem> cachedItems = new List<TItem>();

		private readonly BetterListView listView;

		/// <summary>
		///   Gets items viewed by this collection.
		/// </summary>
		protected List<TItem> CachedItems {
			get {
				if (this.cachedItems.Count == 0) {
					this.CollectCachedItems(this.cachedItems);
				}
				return this.cachedItems;
			}
		}

		/// <summary>
		///   Gets the <see cref="T:ComponentOwl.BetterListView.BetterListView" /> instance that owns this collection.
		/// </summary>
		protected BetterListView ListView => this.listView;

		/// <summary>
		///   Gets or sets the item at the specified index.
		/// </summary>
		/// <returns>The item at the specified index.</returns>
		public abstract TItem this[int index] { get; set; }

		/// <summary>
		///   Gets the number of elements contained in the collection.
		/// </summary>
		/// <returns>The number of elements contained in the collection.</returns>
		public int Count => this.CachedItems.Count;

		/// <summary>
		///   Gets a value indicating whether the collection is read-only.
		/// </summary>
		/// <returns>true if the collection is read-only; otherwise, false.</returns>
		public bool IsReadOnly => false;

		/// <summary>
		///   Gets a value indicating whether the list has a fixed size.
		/// </summary>
		/// <returns>true if the list has a fixed size; otherwise, false.</returns>
		bool IList.IsFixedSize => false;

		/// <summary>
		///   Gets a value indicating whether the collection is read-only.
		/// </summary>
		/// <returns>true if the collection is read-only; otherwise, false.</returns>
		bool IList.IsReadOnly => false;

		/// <summary>
		///   Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>The element at the specified index.</returns>
		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				Checks.CheckType(value, typeof(TItem), "value");
				this[index] = (TItem)value;
			}
		}

		/// <summary>
		///   Gets a value indicating whether access to the collection is synchronized (thread safe).
		/// </summary>
		/// <returns>true if access to the collection is synchronized (thread safe); otherwise, false.</returns>
		bool ICollection.IsSynchronized => false;

		/// <summary>
		///   Gets an object that can be used to synchronize access to the collection.
		/// </summary>
		/// <returns>
		///   An object that can be used to synchronize access to the collection.</returns>
		object ICollection.SyncRoot => this;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewCachedCollection`1" /> class.
		/// </summary>
		/// <param name="listView"><see cref="T:ComponentOwl.BetterListView.BetterListView" /> that owns this collection.</param>
		protected BetterListViewCachedCollection(BetterListView listView) {
			Checks.CheckNotNull(listView, "listView");
			this.listView = listView;
		}

		/// <summary>
		///   Add multiple items into this collection.
		/// </summary>
		/// <param name="items">Items to add into this colletion.</param>
		public abstract void AddRange(IEnumerable<TItem> items);

		/// <summary>
		///   Remove multiple items from this collection.
		/// </summary>
		/// <param name="items">Items to remove from this collection.</param>
		public abstract void RemoveRange(IEnumerable<TItem> items);

		/// <summary>
		///   Set different items as a content of this collection.
		/// </summary>
		/// <param name="items">Items to be contained within this collection.</param>
		public abstract void Set(IEnumerable<TItem> items);

		/// <summary>
		///   Check whether content of this collection is same as the content of the specified collection.
		/// </summary>
		/// <param name="other">Collection to check equality for.</param>
		/// <returns>Content of this collection is same as the content of the specified collection.</returns>
		public bool EqualsContent(IList<TItem> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			IList<TItem> list = this.CachedItems;
			if (list.Count != other.Count) {
				return false;
			}
			IEnumerator<TItem> enumerator = list.GetEnumerator();
			IEnumerator<TItem> enumerator2 = other.GetEnumerator();
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
		///   Reset the cached view so that the collection will be recreated according to new control state when used.
		/// </summary>
		internal void ClearCache() {
			this.cachedItems.Clear();
		}

		/// <summary>
		///   Recreated cached view by collecting items this collection should represent.
		/// </summary>
		/// <param name="cachedItems">Items viewed by this collection.</param>
		protected abstract void CollectCachedItems(List<TItem> cachedItems);

		/// <summary>
		///   Determines the index of the specified item in the list.
		/// </summary>
		/// <param name="item">The object to locate in the list.</param>
		/// <returns>
		///   The index of item if found in the list; otherwise, -1.
		/// </returns>
		public abstract int IndexOf(TItem item);

		/// <summary>
		///   Inserts an item to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The item to insert into the list.</param>
		public abstract void Insert(int index, TItem item);

		/// <summary>
		///   Removes the item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public abstract void RemoveAt(int index);

		/// <summary>
		///   Adds an item to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		public abstract void Add(TItem item);

		/// <summary>
		///   Removes all items from the collection.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		///   Determines whether the collection contains a specific value.
		/// </summary>
		/// <param name="item">The item to locate in the collection.</param>
		/// <returns>
		///   true if item is found in the collection; otherwise, false.
		/// </returns>
		public abstract bool Contains(TItem item);

		/// <summary>
		///   Removes the first occurrence of the specified item from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>
		///   true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.
		/// </returns>
		public abstract bool Remove(TItem item);

		/// <summary>
		///   Copies the elements of the collection to an array, starting at a particular array index.
		/// </summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(TItem[] array, int arrayIndex) {
			this.CachedItems.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<TItem> GetEnumerator() {
			return new List<TItem>(this.CachedItems).GetEnumerator();
		}

		/// <summary>
		///   Adds an item to the list.
		/// </summary>
		/// <param name="value">The object to add to the list.</param>
		/// <returns>
		///   The position into which the new element was inserted.
		/// </returns>
		int IList.Add(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Add((TItem)value);
			return this.listView.SelectedItemsSet.Count - 1;
		}

		/// <summary>
		///   Removes all items from the collection.
		/// </summary>
		void IList.Clear() {
			this.Clear();
		}

		/// <summary>
		///   Determines whether the list contains a specific value.
		/// </summary>
		/// <param name="value">The object to locate in the list.</param>
		/// <returns>
		///   true if the object is found in the list; otherwise, false.
		/// </returns>
		bool IList.Contains(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			return this.Contains((TItem)value);
		}

		/// <summary>
		///   Determines the index of the specified item in the list.
		/// </summary>
		/// <param name="value">The object to locate in the list.</param>
		/// <returns>
		///   The index of value if found in the list; otherwise, -1.
		/// </returns>
		int IList.IndexOf(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			return this.IndexOf((TItem)value);
		}

		/// <summary>
		///   Inserts an item to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted.</param>
		/// <param name="value">The object to insert into the list.</param>
		void IList.Insert(int index, object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Insert(index, (TItem)value);
		}

		/// <summary>
		///   Removes the first occurrence of a specific object from the list.
		/// </summary>
		/// <param name="value">The object to remove from the list.</param>
		void IList.Remove(object value) {
			Checks.CheckType(value, typeof(TItem), "value");
			this.Remove((TItem)value);
		}

		/// <summary>
		///   Removes the list item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		void IList.RemoveAt(int index) {
			this.RemoveAt(index);
		}

		/// <summary>
		///   Copies the elements of the collection to an array, starting at a particular index.
		/// </summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(index, 0, "index", "0");
			}
			else {
				Checks.CheckBounds(index, 0, array.Length - 1, "index");
			}
			List<TItem> list = this.CachedItems;
			for (int i = index; i < Math.Min(array.Length, list.Count); i++) {
				array.SetValue(list[i], i);
			}
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An enumerator object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}