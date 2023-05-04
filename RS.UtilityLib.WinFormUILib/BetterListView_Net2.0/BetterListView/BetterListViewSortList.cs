using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   List with unique sort infos.
	/// </summary>
	[Serializable]
	public sealed class BetterListViewSortList : ICloneable, IEnumerable<BetterListViewSortInfo>, IEnumerable, ISerializable, IXmlSerializable
	{
		private const string FieldInnerList = "innerList";

		private List<BetterListViewSortInfo> innerList;

		/// <summary>
		///   size of the sort list
		/// </summary>
		public int Count => this.innerList.Count;

		/// <summary>
		///   get column sort information for the specified position in the sort list
		/// </summary>
		/// <param name="index">position in the sort list</param>
		/// <returns>column sort information</returns>
		public BetterListViewSortInfo this[int index] {
			get {
				Checks.CheckTrue(this.Count != 0, "this.Count != 0");
				Checks.CheckBounds(index, 0, this.Count - 1, "index");
				return this.innerList[index];
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewSortList instance.
		/// </summary>
		public BetterListViewSortList()
			: this(new BetterListViewSortInfo[0]) {
		}

		/// <summary>
		///   Initialize a new BetterListViewSortList instance.
		/// </summary>
		/// <param name="enumerable">enumerable to create this BetterListViewSortList from</param>
		public BetterListViewSortList(IEnumerable<BetterListViewSortInfo> enumerable) {
			Checks.CheckNotNull(enumerable, "enumerable");
			foreach (BetterListViewSortInfo item in enumerable) {
				Checks.CheckFalse(item.IsEmpty, "sortInfo.IsEmpty");
			}
			this.innerList = new List<BetterListViewSortInfo>(enumerable);
		}

		/// <summary>
		///   Add a the specified sorted column in the list assuming ascending order.
		/// </summary>
		/// <param name="columnIndex">sorted column index</param>
		public void Add(int columnIndex) {
			this.Add(new BetterListViewSortInfo(columnIndex, orderAscending: true));
		}

		/// <summary>
		///   Add a the specified sorted column in the list.
		/// </summary>
		/// <param name="columnIndex">sorted column index</param>
		/// <param name="orderAscending">sorted column sort order</param>
		public void Add(int columnIndex, bool orderAscending) {
			this.Add(new BetterListViewSortInfo(columnIndex, orderAscending));
		}

		/// <summary>
		///   Add a the specified sorted column in the list.
		/// </summary>
		/// <param name="sortInfo">information about the sorted column</param>
		public void Add(BetterListViewSortInfo sortInfo) {
			Checks.CheckFalse(sortInfo.IsEmpty, "sortInfo.IsEmpty");
			this.Remove(sortInfo.ColumnIndex);
			this.innerList.Add(sortInfo);
		}

		/// <summary>
		///   Clear the list.
		/// </summary>
		public void Clear() {
			this.innerList.Clear();
		}

		/// <summary>
		///   Check whether the specified column index is contained in the list.
		/// </summary>
		/// <param name="columnIndex">column index to check</param>
		/// <returns>column index is contained in the list</returns>
		public bool Contains(int columnIndex) {
			Checks.CheckTrue(columnIndex >= 0, "columnIndex >= 0");
			return this.IndexOf(columnIndex) != -1;
		}

		/// <summary>
		///   Get sort order for the specified column index.
		/// </summary>
		/// <param name="columnIndex">column index to retrieve sort order for</param>
		/// <returns>sort order for the specified column index</returns>
		public BetterListViewSortOrder GetSortOrder(int columnIndex) {
			Checks.CheckTrue(columnIndex >= 0, "columnIndex >= 0");
			int num = this.IndexOf(columnIndex);
			if (num != -1) {
				if (!this.innerList[num].OrderAscending) {
					return BetterListViewSortOrder.Descending;
				}
				return BetterListViewSortOrder.Ascending;
			}
			return BetterListViewSortOrder.None;
		}

		/// <summary>
		///   Remove the specified column index from the list.
		/// </summary>
		/// <param name="columnIndex">column index to be removed</param>
		/// <returns>success</returns>
		public bool Remove(int columnIndex) {
			Checks.CheckTrue(columnIndex >= 0, "columnIndex >= 0");
			int num = this.IndexOf(columnIndex);
			if (num != -1) {
				this.innerList.RemoveAt(num);
				return true;
			}
			return false;
		}

		/// <summary>
		///   Set the specified sorted column as the only one in the list assuming ascending order.
		/// </summary>
		/// <param name="columnIndex">sorted column index</param>
		public void Set(int columnIndex) {
			this.Set(columnIndex, orderAscending: true);
		}

		/// <summary>
		///   Set the specified sorted column as the only one in the list.
		/// </summary>
		/// <param name="columnIndex">sorted column index</param>
		/// <param name="orderAscending">sorted column sort order</param>
		public void Set(int columnIndex, bool orderAscending) {
			this.Set(new BetterListViewSortInfo(columnIndex, orderAscending));
		}

		/// <summary>
		///   Set the specified sorted column as the only one in the list.
		/// </summary>
		/// <param name="sortInfo">information about the sorted column</param>
		public void Set(BetterListViewSortInfo sortInfo) {
			this.Clear();
			this.Add(sortInfo);
		}

		/// <summary>
		///   Check whether contents of this list equals contents of the specified list.
		/// </summary>
		/// <param name="other">List to check.</param>
		/// <returns>Contents of this list equals contents of the specified list.</returns>
		public bool EqualsContent(BetterListViewSortList other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			List<BetterListViewSortInfo> list = this.innerList;
			List<BetterListViewSortInfo> list2 = other.innerList;
			if (list.Count != list2.Count) {
				return false;
			}
			IEnumerator<BetterListViewSortInfo> enumerator = list.GetEnumerator();
			IEnumerator<BetterListViewSortInfo> enumerator2 = list2.GetEnumerator();
			enumerator.Reset();
			enumerator2.Reset();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				if (!enumerator.Current.Equals(enumerator2.Current)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Update the sort list for consistency with the specified column header collection.
		/// </summary>
		/// <param name="columnHeaders">column header collection to update against</param>
		internal void Update(BetterListViewColumnHeaderCollection columnHeaders) {
			Checks.CheckNotNull(columnHeaders, "columnHeaders");
			int num = ((columnHeaders.Count != 0) ? (columnHeaders.Count - 1) : (-1));
			Set<int> set = new Set<int>();
			for (int i = 0; i < this.innerList.Count; i++) {
				BetterListViewSortInfo betterListViewSortInfo = this.innerList[i];
				if (betterListViewSortInfo.ColumnIndex > num || columnHeaders[betterListViewSortInfo.ColumnIndex].SortOrder == BetterListViewSortOrder.None) {
					set.Add(betterListViewSortInfo.ColumnIndex);
					continue;
				}
				BetterListViewSortOrder betterListViewSortOrder = (betterListViewSortInfo.OrderAscending ? BetterListViewSortOrder.Ascending : BetterListViewSortOrder.Descending);
				if (betterListViewSortOrder != columnHeaders[betterListViewSortInfo.ColumnIndex].SortOrder) {
					this.innerList[i] = new BetterListViewSortInfo(betterListViewSortInfo.ColumnIndex, !betterListViewSortInfo.OrderAscending);
				}
			}
			foreach (int item in set) {
				this.Remove(item);
			}
		}

		private int IndexOf(int columnIndex) {
			for (int i = 0; i < this.innerList.Count; i++) {
				if (this.innerList[i].ColumnIndex == columnIndex) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public object Clone() {
			return new BetterListViewSortList(this.innerList);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<BetterListViewSortInfo> GetEnumerator() {
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
			return this.innerList.GetEnumerator();
		}

		private BetterListViewSortList(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.innerList = (List<BetterListViewSortInfo>)info.GetValue("innerList", typeof(List<BetterListViewSortInfo>));
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
			this.innerList = (List<BetterListViewSortInfo>)new XmlSerializer(typeof(List<BetterListViewSortInfo>)).Deserialize(reader);
			reader.ReadEndElement();
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteStartElement("innerList");
			new XmlSerializer(typeof(List<BetterListViewSortInfo>)).Serialize(writer, this.innerList);
			writer.WriteEndElement();
		}
	}
}