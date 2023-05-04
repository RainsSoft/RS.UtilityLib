using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of BetterListView Tooltips.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(BetterListViewToolTipInfoCollectionConverter))]
	[Editor(typeof(BetterListViewToolTipInfoCollectionEditor), typeof(UITypeEditor))]
	public sealed class BetterListViewToolTipInfoCollection : ICloneable, IList<BetterListViewToolTipInfo>, ICollection<BetterListViewToolTipInfo>, IEnumerable<BetterListViewToolTipInfo>, IEnumerable, IList, ICollection, ISerializable, IXmlSerializable
	{
		private const string FieldInnerList = "innerList";

		private List<BetterListViewToolTipInfo> innerList;

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
		public BetterListViewToolTipInfo this[int index] {
			get {
				return this.innerList[index];
			}
			set {
				this.innerList[index] = value;
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
				this[index] = (BetterListViewToolTipInfo)value;
			}
		}

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
		///   Initialize a new BetterListViewToolTipInfoCollection instance.
		/// </summary>
		public BetterListViewToolTipInfoCollection() {
			this.innerList = new List<BetterListViewToolTipInfo>();
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfoCollection instance.
		/// </summary>
		/// <param name="enumerable">enumerable with tooltips to fill this collection</param>
		public BetterListViewToolTipInfoCollection(IEnumerable<BetterListViewToolTipInfo> enumerable) {
			Checks.CheckNotNull(enumerable, "enumerable");
			this.innerList = new List<BetterListViewToolTipInfo>(enumerable);
		}

		/// <summary>
		///   Add multiple tooltips to this collection.
		/// </summary>
		/// <param name="enumerable">enumerable with tooltips to add</param>
		public void AddRange(IEnumerable<BetterListViewToolTipInfo> enumerable) {
			Checks.CheckNotNull(enumerable, "enumerable");
			foreach (BetterListViewToolTipInfo item in enumerable) {
				this.innerList.Add(item);
			}
		}

		/// <summary>
		///   Add multiple tooltips to this collection.
		/// </summary>
		/// <param name="values">array with BetterListViewToolTipInfo instances</param>
		public void AddRange(object[] values) {
			Checks.CheckNotNull(values, "values");
			foreach (object param in values) {
				Checks.CheckType(param, typeof(BetterListViewToolTipInfo), "value");
			}
			for (int i = 0; i < values.Length; i++) {
				BetterListViewToolTipInfo item = (BetterListViewToolTipInfo)values[i];
				this.innerList.Add(item);
			}
		}

		/// <summary>
		///   Check whether this collection contains the specified tooltip location.
		/// </summary>
		/// <param name="location">tooltip location</param>
		/// <returns>this collection contains the specified tooltip location</returns>
		public bool ContainsLocation(BetterListViewToolTipLocation location) {
			foreach (BetterListViewToolTipInfo inner in this.innerList) {
				if (inner.Location == location) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		///   Try to retrieve tooltip with the specified location from this collection.
		/// </summary>
		/// <param name="location">tooltip location</param>
		/// <param name="toolTipInfo">output tooltip</param>
		/// <returns>success</returns>
		public bool TryGetToolTipInfo(BetterListViewToolTipLocation location, out BetterListViewToolTipInfo toolTipInfo) {
			foreach (BetterListViewToolTipInfo inner in this.innerList) {
				if (inner.Location == location) {
					toolTipInfo = inner;
					return true;
				}
			}
			toolTipInfo = BetterListViewToolTipInfo.Empty;
			return false;
		}

		/// <summary>
		///   Check whether content of this collection equals content of the specified collection.
		/// </summary>
		/// <param name="other">Collection to check.</param>
		/// <returns>Content of this collection equals content of the specified collection.</returns>
		public bool EqualsContent(BetterListViewToolTipInfoCollection other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			if (this.Count != other.Count) {
				return false;
			}
			IEnumerator<BetterListViewToolTipInfo> enumerator = this.GetEnumerator();
			IEnumerator<BetterListViewToolTipInfo> enumerator2 = other.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				if (!enumerator.Current.Equals(enumerator2.Current)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public object Clone() {
			return new BetterListViewToolTipInfoCollection(this);
		}

		/// <summary>
		///   Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </summary>
		/// <returns>
		///   The index of <paramref name="item" /> if found in the list; otherwise, -1.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.
		/// </param>
		public int IndexOf(BetterListViewToolTipInfo item) {
			return this.innerList.IndexOf(item);
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
		public void Insert(int index, BetterListViewToolTipInfo item) {
			this.innerList.Insert(index, item);
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
			this.innerList.RemoveAt(index);
		}

		/// <summary>
		///   Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </exception>
		public void Add(BetterListViewToolTipInfo item) {
			this.innerList.Add(item);
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
		public bool Contains(BetterListViewToolTipInfo item) {
			return this.innerList.Contains(item);
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
		public void CopyTo(BetterListViewToolTipInfo[] array, int index) {
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
		public bool Remove(BetterListViewToolTipInfo item) {
			return this.innerList.Remove(item);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<BetterListViewToolTipInfo> GetEnumerator() {
			return ((IEnumerable<BetterListViewToolTipInfo>)this.innerList).GetEnumerator();
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
			Checks.CheckType(value, typeof(BetterListViewToolTipInfo), "value");
			this.Add((BetterListViewToolTipInfo)value);
			return this.Count - 1;
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
			Checks.CheckType(value, typeof(BetterListViewToolTipInfo), "value");
			return this.Contains((BetterListViewToolTipInfo)value);
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
			Checks.CheckType(value, typeof(BetterListViewToolTipInfo), "value");
			return this.IndexOf((BetterListViewToolTipInfo)value);
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
			Checks.CheckType(value, typeof(BetterListViewToolTipInfo), "value");
			this.Insert(index, (BetterListViewToolTipInfo)value);
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
			Checks.CheckType(value, typeof(BetterListViewToolTipInfo), "value");
			this.Remove((BetterListViewToolTipInfo)value);
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
				array.SetValue(this.innerList[i], i);
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
			return this.innerList.GetEnumerator();
		}

		private BetterListViewToolTipInfoCollection(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.innerList = (List<BetterListViewToolTipInfo>)info.GetValue("innerList", typeof(List<BetterListViewToolTipInfo>));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			info.AddValue("innerList", this.innerList);
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			reader.ReadStartElement("innerList");
			this.innerList = (List<BetterListViewToolTipInfo>)new XmlSerializer(typeof(List<BetterListViewToolTipInfo>)).Deserialize(reader);
			reader.ReadEndElement();
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteStartElement("innerList");
			new XmlSerializer(typeof(List<BetterListViewToolTipInfo>)).Serialize(writer, this.innerList);
			writer.WriteEndElement();
		}
	}
}