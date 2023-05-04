using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView.Collections
{

	/// <summary>
	///   Custom dictionary.
	/// </summary>
	/// <typeparam name="TKey">item key type</typeparam>
	/// <typeparam name="TValue">item value type</typeparam>
	[Serializable]
	[XmlRoot("ReadWriteDictionary")]
	public class ReadWriteDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>
	{
		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public override bool IsReadOnly => false;

		/// <summary>
		/// Gets or sets the value with the specified key.
		/// </summary>
		public override TValue this[TKey key] {
			get {
				return base.InnerDictionary[key];
			}
			set {
				Checks.CheckTrue(base.InnerDictionary.ContainsKey(key), "this.InnerDictionary.ContainsKey(key)", "Dictionary does not contain the specified key.");
				base.InnerDictionary[key] = value;
			}
		}

		/// <summary>
		///   Initialize a new ReadWriteDictionary instance.
		/// </summary>
		public ReadWriteDictionary() {
		}

		/// <summary>
		///   Initialize a new ReadWriteDictionary instance.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		public ReadWriteDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary) {
		}

		/// <summary>
		///   Initialize a new ReadWriteDictionary instance.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		/// <param name="comparer">key comparer</param>
		public ReadWriteDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer) {
		}

		/// <summary>
		/// Adds the specified key/value pair to this dictionary.
		/// </summary>
		/// <param name="key">key of the key/value pair</param>
		/// <param name="value">value of the key/value pair</param>
		public override void Add(TKey key, TValue value) {
			base.InnerDictionary.Add(key, value);
		}

		/// <summary>
		/// Adds the specified key/value pair to this dictionary.
		/// </summary>
		/// <param name="item">key/value pair to add</param>
		public override void Add(KeyValuePair<TKey, TValue> item) {
			base.InnerDictionary.Add(item);
		}

		/// <summary>
		/// Clears this dictionary.
		/// </summary>
		public override void Clear() {
			base.InnerDictionary.Clear();
		}

		/// <summary>
		/// Remove key/value pair with the specified key from this dictionary.
		/// </summary>
		/// <param name="key">key of the key/value pair to remove</param>
		/// <returns>the specified key is present in the dictionary</returns>
		public override bool Remove(TKey key) {
			return base.InnerDictionary.Remove(key);
		}

		/// <summary>
		/// Remove the specified key/value pair from this dictionary.
		/// </summary>
		/// <param name="item">key/value pair to remove</param>
		/// <returns>the specified key is present in the dictionary</returns>
		public override bool Remove(KeyValuePair<TKey, TValue> item) {
			return base.InnerDictionary.Remove(item);
		}

		private ReadWriteDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}
}