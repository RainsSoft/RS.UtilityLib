using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Sorted hash set implementation.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public class SortedSet<TItem> : ICollection<TItem>, IEnumerable<TItem>, IEnumerable
	{
		private readonly SortedDictionary<TItem, object> innerSet;

		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
		public int Count => this.innerSet.Count;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
		public bool IsReadOnly => false;

		/// <summary>
		///   Initialize a new SortedSet{T} instance.
		/// </summary>
		/// <param name="items">items to be contained in the set</param>
		public SortedSet(IEnumerable<TItem> items)
			: this() {
			Checks.CheckNotNull(items, "items");
			foreach (TItem item in items) {
				this.innerSet.Add(item, null);
			}
		}

		/// <summary>
		///   Initialize a new SortedSet{T} instance.
		/// </summary>
		/// <param name="comparer">item comparer</param>
		public SortedSet(IComparer<TItem> comparer) {
			this.innerSet = new SortedDictionary<TItem, object>(comparer);
		}

		/// <summary>
		///   Initialize a new SortedSet{T} instance.
		/// </summary>
		public SortedSet() {
			this.innerSet = new SortedDictionary<TItem, object>();
		}

		/// <summary>
		///   Copy this set to the specified array.
		/// </summary>
		/// <param name="array">array to copy this set to</param>
		public void CopyTo(TItem[] array) {
			Checks.CheckNotNull(array, "array");
			this.innerSet.Keys.CopyTo(array, 0);
		}

		/// <summary>
		///   Remove items contained in the specified IEnumerable{T}.
		/// </summary>
		/// <param name="enumerable">enumerable containing items that should not be contained in the set</param>
		public void ExceptWith(IEnumerable<TItem> enumerable) {
			foreach (TItem item in enumerable) {
				this.Remove(item);
			}
		}

		/// <summary>
		///   Change the set to contain only items common to the set and the specified enumerable.
		/// </summary>
		/// <param name="enumerable">items to intersect with items in the set</param>
		public void SymmetricExceptWith(IEnumerable<TItem> enumerable) {
			List<TItem> list = new List<TItem>();
			foreach (TItem item in enumerable) {
				if (this.Contains(item)) {
					list.Add(item);
				}
			}
			foreach (TItem item2 in list) {
				this.Remove(item2);
			}
		}

		/// <summary>
		///   Change the set to contain both items of the set and items of the specified enumerable.
		/// </summary>
		/// <param name="enumerable">items to union with the set</param>
		public void UnionWith(IEnumerable<TItem> enumerable) {
			foreach (TItem item in enumerable) {
				this.Add(item);
			}
		}

		/// <summary>
		/// Check whether contents of this set equals contents of the specified set.
		/// </summary>
		/// <param name="other">Set to check.</param>
		/// <returns>Contents of this set equals contents of the specified set.</returns>
		public bool EqualsContent(SortedSet<TItem> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			SortedDictionary<TItem, object> sortedDictionary = this.innerSet;
			SortedDictionary<TItem, object> sortedDictionary2 = other.innerSet;
			if (sortedDictionary.Count != sortedDictionary2.Count) {
				return false;
			}
			foreach (TItem key in sortedDictionary.Keys) {
				if (!sortedDictionary2.ContainsKey(key)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public void Add(TItem item) {
			if (!this.innerSet.ContainsKey(item)) {
				this.innerSet.Add(item, null);
			}
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
		public void Clear() {
			this.innerSet.Clear();
		}

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		/// <returns>
		///   true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
		/// </returns>
		public bool Contains(TItem item) {
			return this.innerSet.ContainsKey(item);
		}

		/// <summary>
		///   Copy this set to the specified array.
		/// </summary>
		/// <param name="array">array to copy this set to</param>
		/// <param name="arrayIndex">start index in the target array</param>
		public void CopyTo(TItem[] array, int arrayIndex) {
			Checks.CheckNotNull(array, "array");
			if (array.Length == 0) {
				Checks.CheckEqual(arrayIndex, 0, "arrayIndex", "0");
			}
			else {
				Checks.CheckBounds(arrayIndex, 0, array.Length - 1, "arrayIndex");
			}
			this.innerSet.Keys.CopyTo(array, arrayIndex);
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
			if (!this.innerSet.ContainsKey(item)) {
				return false;
			}
			this.innerSet.Remove(item);
			return true;
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() {
			return this.innerSet.Keys.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator() {
			return this.innerSet.Keys.GetEnumerator();
		}
	}
}