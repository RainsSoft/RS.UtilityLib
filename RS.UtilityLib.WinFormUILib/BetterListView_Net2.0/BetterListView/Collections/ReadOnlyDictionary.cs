using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Read-only dictionary.
	/// </summary>
	/// <typeparam name="TKey">item key type</typeparam>
	/// <typeparam name="TValue">item value type</typeparam>
	[Serializable]
	[XmlRoot("ReadWriteDictionary")]
	public class ReadOnlyDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>
	{
		/// <summary>
		///   Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public override bool IsReadOnly => true;

		/// <summary>
		///   Gets or sets the value with the specified key.
		/// </summary>
		public override TValue this[TKey key] {
			get {
				return base.InnerDictionary[key];
			}
			set {
				throw new InvalidOperationException("Cannot modify a read-only dictionary.");
			}
		}

		/// <summary>
		///   Initialize a new ReadOnlyDictionary instance.
		/// </summary>
		public ReadOnlyDictionary() {
		}

		/// <summary>
		///   Initialize a new ReadOnlyDictionary instance.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary) {
		}

		/// <summary>
		///   Initialize a new ReadOnlyDictionary instance.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		/// <param name="comparer">key comparer</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer) {
		}

		/// <summary>
		///   Adds the specified key/value pair to this dictionary.
		/// </summary>
		/// <param name="key">key of the key/value pair</param>
		/// <param name="value">value of the key/value pair</param>
		public override void Add(TKey key, TValue value) {
			throw new InvalidOperationException("Cannot modify a read-only dictionary.");
		}

		/// <summary>
		///   Adds the specified key/value pair to this dictionary.
		/// </summary>
		/// <param name="item">key/value pair to add</param>
		public override void Add(KeyValuePair<TKey, TValue> item) {
			throw new InvalidOperationException("Cannot modify a read-only dictionary.");
		}

		/// <summary>
		///   Clears this dictionary.
		/// </summary>
		public override void Clear() {
			throw new InvalidOperationException("Cannot modify a read-only dictionary.");
		}

		/// <summary>
		///   Remove key/value pair with the specified key from this dictionary.
		/// </summary>
		/// <param name="key">key of the key/value pair to remove</param>
		/// <returns>the specified key is present in the dictionary</returns>
		public override bool Remove(TKey key) {
			throw new InvalidOperationException();
		}

		/// <summary>
		///   Remove the specified key/value pair from this dictionary.
		/// </summary>
		/// <param name="item">key/value pair to remove</param>
		/// <returns>the specified key is present in the dictionary</returns>
		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			throw new InvalidOperationException("Cannot modify a read-only dictionary.");
		}

		private ReadOnlyDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}
}