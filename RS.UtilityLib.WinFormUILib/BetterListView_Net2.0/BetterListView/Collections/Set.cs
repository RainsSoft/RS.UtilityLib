using System;
using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Hash set.
	/// </summary>
	/// <typeparam name="TItem">item type</typeparam>
	public class Set<TItem> : SetBase<TItem>, IExtendedCollection<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable, ICollection
	{
		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///   The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		int ICollection<TItem>.Count => base.Count;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		/// </returns>
		bool ICollection<TItem>.IsReadOnly => false;

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
		object ICollection.SyncRoot => this;

		/// <summary>
		///   Initialize a new Set instance.
		/// </summary>
		public Set() {
		}

		/// <summary>
		///   Initialize a new Set instance.
		/// </summary>
		/// <param name="enumerable">IEnumerable to create this collection from</param>
		public Set(IEnumerable<TItem> enumerable)
			: base(enumerable) {
		}

		/// <summary>
		///   Initialize a new Set instance.
		/// </summary>
		/// <param name="comparer">item comparer</param>
		public Set(IEqualityComparer<TItem> comparer)
			: base(comparer) {
		}

		/// <summary>
		///   Initialize a new Set instance.
		/// </summary>
		/// <param name="enumerable">IEnumerable to create this collection from</param>
		/// <param name="comparer">item comparer</param>
		public Set(IEnumerable<TItem> enumerable, IEqualityComparer<TItem> comparer)
			: base(enumerable, comparer) {
		}

		/// <summary>
		///   Get read-only variant of this collection.
		/// </summary>
		/// <returns>read-only variant of this collection</returns>
		public ReadOnlySet<TItem> AsReadOnly() {
			return new ReadOnlySet<TItem>(this);
		}

		/// <summary>
		///   Remove items contained in the specified IEnumerable{T}.
		/// </summary>
		/// <param name="enumerable">enumerable containing items that should not be contained in the set</param>
		public virtual void ExceptWith(IEnumerable<TItem> enumerable) {
			foreach (TItem item in enumerable) {
				this.Remove(item);
			}
		}

		/// <summary>
		///   Change the set to contain only items common to the set and the specified enumerable.
		/// </summary>
		/// <param name="enumerable">items to intersect with items in the set</param>
		public virtual void SymmetricExceptWith(IEnumerable<TItem> enumerable) {
			List<TItem> list = new List<TItem>();
			foreach (TItem item in enumerable) {
				if (base.Contains(item)) {
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
		public virtual void UnionWith(IEnumerable<TItem> enumerable) {
			foreach (TItem item in enumerable) {
				this.Add(item);
			}
		}

		/// <summary>
		///   Add the specified items in the set.
		/// </summary>
		/// <param name="items">items to add</param>
		public virtual void AddRange(IEnumerable items) {
			Checks.CheckNotNull(items, "items");
			foreach (object item2 in items) {
				Checks.CheckType(item2, typeof(TItem), "value");
			}
			foreach (TItem item3 in items) {
				this.Add(item3);
			}
		}

		/// <summary>
		///   Remove the specified items from the set.
		/// </summary>
		/// <param name="items">items to remove</param>
		public virtual void RemoveRange(IEnumerable items) {
			Checks.CheckNotNull(items, "items");
			foreach (object item2 in items) {
				Checks.CheckType(item2, typeof(TItem), "value");
			}
			foreach (TItem item3 in items) {
				this.Remove(item3);
			}
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </exception>
		public virtual void Add(TItem item) {
			if (!base.InnerSet.ContainsKey(item)) {
				base.InnerSet.Add(item, null);
			}
		}

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
		/// </exception>
		public virtual void Clear() {
			base.InnerSet.Clear();
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
		public virtual bool Remove(TItem item) {
			if (!base.InnerSet.ContainsKey(item)) {
				return false;
			}
			base.InnerSet.Remove(item);
			return true;
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
			foreach (TItem key in base.InnerSet.Keys) {
				array.SetValue(key, index++);
				if (index == array.Length) {
					break;
				}
			}
		}
	}
}