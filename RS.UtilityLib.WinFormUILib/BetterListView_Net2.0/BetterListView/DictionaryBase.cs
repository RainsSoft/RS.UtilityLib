using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Base class for dictionaries.
	/// </summary>
	/// <typeparam name="TKey">type of item key</typeparam>
	/// <typeparam name="TValue">type of item value</typeparam>
	[Serializable]
	public abstract class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ICollection, ISerializable, IXmlSerializable
	{
		private const string NameItem = "Item";

		private const string NameKey = "Key";

		private const string NameValue = "Value";

		[NonSerialized]
		private readonly IDictionary<TKey, TValue> innerDictionary;

		/// <summary>
		///   Underlying dictionary.
		/// </summary>
		protected IDictionary<TKey, TValue> InnerDictionary => this.innerDictionary;

		/// <summary>
		///   Gets or sets the element with the specified key.
		/// </summary>
		/// <returns>
		///   The element with the specified key.
		/// </returns>
		/// <param name="key">The key of the element to get or set.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.
		/// </exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
		/// </exception>
		public abstract TValue this[TKey key] { get; set; }

		/// <summary>
		///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		public ICollection<TKey> Keys => this.innerDictionary.Keys;

		/// <summary>
		///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		public ICollection<TValue> Values => this.innerDictionary.Values;

		/// <summary>
		///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		/// </returns>
		public abstract bool IsReadOnly { get; }

		/// <summary>
		///   Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///   The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		public int Count => this.innerDictionary.Count;

		/// <summary>
		///   Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <returns>
		///   true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public bool IsSynchronized => false;

		/// <summary>
		///   Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>
		///   An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public object SyncRoot => this;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.DictionaryBase`2" /> class.
		/// </summary>
		protected DictionaryBase() {
			this.innerDictionary = new Dictionary<TKey, TValue>();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.DictionaryBase`2" /> class.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		protected DictionaryBase(IDictionary<TKey, TValue> dictionary) {
			Checks.CheckNotNull(dictionary, "dictionary");
			this.innerDictionary = new Dictionary<TKey, TValue>(dictionary);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.DictionaryBase`2" /> class.
		/// </summary>
		/// <param name="dictionary">dictionary to create this dictionary from</param>
		/// <param name="comparer">key comparer</param>
		protected DictionaryBase(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
			Checks.CheckNotNull(dictionary, "dictionary");
			Checks.CheckNotNull(comparer, "comparer");
			this.innerDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
		}

		/// <summary>
		/// Check whether content of this dictionary equals content of the specified dictionary.
		/// </summary>
		/// <param name="other">Dictionary to check.</param>
		/// <returns>Content of this dictionary equals content of the specified dictionary.</returns>
		public virtual bool EqualsContent(IDictionary<TKey, TValue> other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			if (this.Count != other.Count) {
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> item in this.innerDictionary) {
				TKey key = item.Key;
				TValue value = item.Value;
				if (!other.TryGetValue(key, out var value2)) {
					return false;
				}
				if (!object.Equals(value, value2)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.
		/// </param>
		/// <param name="value">The object to use as the value of the element to add.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
		/// </exception>
		public abstract void Add(TKey key, TValue value);

		/// <summary>
		///   Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>
		///   true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		/// <param name="key">The key of the element to remove.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
		/// </exception>
		public abstract bool Remove(TKey key);

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
		/// </summary>
		/// <returns>
		///   true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
		/// </returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.
		/// </exception>
		public bool ContainsKey(TKey key) {
			return this.innerDictionary.ContainsKey(key);
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
		public void CopyTo(Array array, int index) {
			Checks.CheckNotNull(array, "array");
			if (array.Length != 0) {
				Checks.CheckBounds(index, 0, array.Length - 1, "index");
			}
			else {
				Checks.CheckEqual(index, 0, "index", "0");
			}
			foreach (KeyValuePair<TKey, TValue> item in this.innerDictionary) {
				array.SetValue(item, index++);
				if (index == array.Length) {
					break;
				}
			}
		}

		/// <summary>
		///   Gets the value associated with the specified key.
		/// </summary>
		/// <returns>
		///   true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <param name="key">The key whose value to get.
		/// </param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.
		/// </exception>
		public bool TryGetValue(TKey key, out TValue value) {
			return this.innerDictionary.TryGetValue(key, out value);
		}

		/// <summary>
		///   Add the specified key/value pair to the dictionary.
		/// </summary>
		/// <param name="item">key/value pair to add</param>
		public abstract void Add(KeyValuePair<TKey, TValue> item);

		/// <summary>
		///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
		/// </exception>
		public abstract void Clear();

		/// <summary>
		///   Remove the specified key/value pair from the dictionary.
		/// </summary>
		/// <param name="item">key/value pair to remove</param>
		/// <returns>the specified key is present in the dictionary</returns>
		public abstract bool Remove(KeyValuePair<TKey, TValue> item);

		/// <summary>
		///   Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
		/// </summary>
		/// <returns>
		///   true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return this.innerDictionary.Contains(item);
		}

		/// <summary>
		///   Copy content of this dictionary to the specified array.
		/// </summary>
		/// <param name="array">target array</param>
		/// <param name="index">index on whitch the copying begins</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) {
			this.innerDictionary.CopyTo(array, index);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return this.innerDictionary.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)this.innerDictionary).GetEnumerator();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.DictionaryBase`2" /> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected DictionaryBase(SerializationInfo info, StreamingContext context) {
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			Type typeFromHandle = typeof(TKey);
			Type typeFromHandle2 = typeof(TValue);
			for (int i = 0; i < info.MemberCount >> 1; i++) {
				TKey key = (TKey)info.GetValue("Key" + i, typeFromHandle);
				TValue value = (TValue)info.GetValue("Value" + i, typeFromHandle2);
				dictionary.Add(key, value);
			}
			this.innerDictionary = dictionary;
		}

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. 
		/// </param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization. 
		/// </param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. 
		/// </exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			int num = 0;
			Type typeFromHandle = typeof(TKey);
			Type typeFromHandle2 = typeof(TValue);
			foreach (KeyValuePair<TKey, TValue> item in this.innerDictionary) {
				info.AddValue("Key" + num, item.Key, typeFromHandle);
				info.AddValue("Value" + num, item.Value, typeFromHandle2);
				num++;
			}
		}

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		void IXmlSerializable.ReadXml(XmlReader reader) {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
			reader.MoveToContent();
			if (!reader.IsEmptyElement) {
				reader.ReadStartElement();
				while (reader.NodeType != XmlNodeType.EndElement) {
					reader.ReadStartElement("Item");
					reader.ReadStartElement("Key");
					TKey key = (TKey)xmlSerializer.Deserialize(reader);
					reader.ReadEndElement();
					reader.ReadStartElement("Value");
					TValue value = (TValue)xmlSerializer2.Deserialize(reader);
					reader.ReadEndElement();
					this.innerDictionary.Add(key, value);
					reader.ReadEndElement();
					reader.MoveToContent();
				}
				reader.ReadEndElement();
			}
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer) {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
			foreach (TKey key in this.Keys) {
				writer.WriteStartElement("Item");
				writer.WriteStartElement("Key");
				xmlSerializer.Serialize(writer, key);
				writer.WriteEndElement();
				writer.WriteStartElement("Value");
				TValue val = this[key];
				xmlSerializer2.Serialize(writer, val);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		/// <summary>
		///   This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
		/// </returns>
		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}
	}
}