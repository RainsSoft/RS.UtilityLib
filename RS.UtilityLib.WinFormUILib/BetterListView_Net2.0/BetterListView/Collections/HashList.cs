using System;
using System.Collections;
using System.Collections.Generic;

namespace ComponentOwl.BetterListView.Collections
{

	public class HashList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private readonly Dictionary<TKey, int> innerDictionary;

		private readonly List<TValue> innerList;

		public ICollection<TKey> Keys => this.innerDictionary.Keys;

		public ICollection<TValue> Values => this.innerList;

		public TValue this[TKey key] {
			get {
				Checks.CheckNotNull(key, "key");
				if (!this.innerDictionary.TryGetValue(key, out var value)) {
					throw new KeyNotFoundException($"Key '{key}' is not preset in the hash list.");
				}
				return this.innerList[value];
			}
			set {
				Checks.CheckNotNull(key, "key");
				if (!this.innerDictionary.TryGetValue(key, out var value2)) {
					throw new KeyNotFoundException($"Key '{key}' is not preset in the hash list.");
				}
				this.innerList[value2] = value;
			}
		}

		public int Count => this.innerList.Count;

		public bool IsReadOnly => false;

		public HashList() {
			this.innerDictionary = new Dictionary<TKey, int>();
			this.innerList = new List<TValue>();
		}

		public HashList(IEnumerable<KeyValuePair<TKey, TValue>> enumerable) {
			this.innerDictionary = new Dictionary<TKey, int>();
			foreach (KeyValuePair<TKey, TValue> item in enumerable) {
				this.innerDictionary.Add(item.Key, this.innerList.Count);
				this.innerList.Add(item.Value);
			}
		}

		public HashList(IEqualityComparer<TKey> comparer) {
			this.innerDictionary = new Dictionary<TKey, int>(comparer);
			this.innerList = new List<TValue>();
		}

		public HashList(IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IEqualityComparer<TKey> comparer) {
			this.innerDictionary = new Dictionary<TKey, int>(comparer);
			this.innerList = new List<TValue>();
			foreach (KeyValuePair<TKey, TValue> item in enumerable) {
				this.innerDictionary.Add(item.Key, this.innerList.Count);
				this.innerList.Add(item.Value);
			}
		}

		public bool EqualsContent(HashList<TKey, TValue> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			Dictionary<TKey, int> dictionary = this.innerDictionary;
			Dictionary<TKey, int> dictionary2 = other.innerDictionary;
			if (dictionary.Count != dictionary2.Count) {
				return false;
			}
			foreach (KeyValuePair<TKey, int> item in dictionary) {
				TKey key = item.Key;
				int value = item.Value;
				if (!dictionary2.TryGetValue(key, out var value2)) {
					return false;
				}
				if (value != value2) {
					return false;
				}
			}
			IEnumerator<TValue> enumerator2 = this.innerList.GetEnumerator();
			IEnumerator<TValue> enumerator3 = other.innerList.GetEnumerator();
			enumerator2.Reset();
			enumerator3.Reset();
			while (enumerator2.MoveNext() && enumerator3.MoveNext()) {
				if (!object.Equals(enumerator2.Current, enumerator3.Current)) {
					return false;
				}
			}
			return true;
		}

		public void Add(TKey key, TValue value) {
			Checks.CheckNotNull(key, "key");
			this.innerDictionary.Add(key, this.innerList.Count);
			this.innerList.Add(value);
		}

		public bool ContainsKey(TKey key) {
			Checks.CheckNotNull(key, "key");
			return this.innerDictionary.ContainsKey(key);
		}

		public bool Remove(TKey key) {
			Checks.CheckNotNull(key, "key");
			if (this.innerDictionary.TryGetValue(key, out var value)) {
				this.innerDictionary.Remove(key);
				this.innerList.RemoveAt(value);
				foreach (TKey key2 in this.innerDictionary.Keys) {
					int num = this.innerDictionary[key2];
					if (num > value) {
						this.innerDictionary[key2] = num - 1;
					}
				}
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value) {
			Checks.CheckNotNull(key, "key");
			if (this.innerDictionary.TryGetValue(key, out var value2)) {
				value = this.innerList[value2];
				return true;
			}
			value = default(TValue);
			return false;
		}

		public void Add(KeyValuePair<TKey, TValue> item) {
			Checks.CheckNotNull(item, "item");
			this.Add(item.Key, item.Value);
		}

		public void Clear() {
			this.innerDictionary.Clear();
			this.innerList.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {
			Checks.CheckNotNull(item, "item");
			if (this.innerDictionary.TryGetValue(item.Key, out var value)) {
				return this.innerList[value].Equals(item);
			}
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			Checks.CheckNotNull(array, "array");
			if (array.Length != 0) {
				Checks.CheckBounds(arrayIndex, 0, array.Length - 1, "arrayIndex");
			}
			else {
				Checks.CheckEqual(arrayIndex, 0, "arrayIndex", "0");
			}
			for (int i = arrayIndex; i < Math.Min(array.Length, this.innerList.Count); i++) {
				array.SetValue(this.innerList[i], i);
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) {
			Checks.CheckNotNull(item, "item");
			if (this.Contains(item)) {
				this.Remove(item.Key);
				return true;
			}
			return false;
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			Dictionary<int, TKey> dictionary = new Dictionary<int, TKey>();
			foreach (KeyValuePair<TKey, int> item in this.innerDictionary) {
				dictionary.Add(item.Value, item.Key);
			}
			List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
			int num = 0;
			foreach (TValue inner in this.innerList) {
				list.Add(new KeyValuePair<TKey, TValue>(dictionary[num], inner));
				num++;
			}
			return list.GetEnumerator();
		}

		public IEnumerator GetEnumerator() {
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}
	}
}