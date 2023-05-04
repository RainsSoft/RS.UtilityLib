using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Information about sorted column.
	/// </summary>
	[Serializable]
	public struct BetterListViewSortInfo : ISerializable, IXmlSerializable
	{
		/// <summary>
		///   value of index that is not defined
		/// </summary>
		public const int IndexUndefined = -1;

		private const string FieldColumnIndex = "columnIndex";

		private const string FieldOrderAscending = "orderAscending";

		/// <summary>
		///   represents an empty BetterListViewSortInfo structure
		/// </summary>
		public static readonly BetterListViewSortInfo Empty = new BetterListViewSortInfo(-1, orderAscending: true);

		private int columnIndex;

		private bool orderAscending;

		/// <summary>
		///   index of the sorted column
		/// </summary>
		public int ColumnIndex => this.columnIndex;

		/// <summary>
		///   sort order
		/// </summary>
		public bool OrderAscending => this.orderAscending;

		/// <summary>
		///   this SortInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewSortInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewSortInfo instance.
		/// </summary>
		/// <param name="columnIndex">index of the sorted column</param>
		/// <param name="orderAscending">sort order</param>
		public BetterListViewSortInfo(int columnIndex, bool orderAscending) {
			Checks.CheckTrue(columnIndex == -1 || columnIndex >= 0, "columnIndex == IndexUndefined || columnIndex >= 0");
			this.columnIndex = columnIndex;
			this.orderAscending = orderAscending;
		}

		/// <summary>
		///   Test whether the two BetterListViewSortInfo objects are identical.
		/// </summary>
		/// <param name="sortInfoA">first BetterListViewSortInfo object</param>
		/// <param name="sortInfoB">second BetterListViewSortInfo object</param>
		/// <returns>the two BetterListViewSortInfo objects are identical</returns>
		public static bool operator ==(BetterListViewSortInfo sortInfoA, BetterListViewSortInfo sortInfoB) {
			return sortInfoA.Equals(sortInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewSortInfo objects are different.
		/// </summary>
		/// <param name="sortInfoA">first BetterListViewSortInfo object</param>
		/// <param name="sortInfoB">second BetterListViewSortInfo object</param>
		/// <returns>the two BetterListViewSortInfo objects are different</returns>
		public static bool operator !=(BetterListViewSortInfo sortInfoA, BetterListViewSortInfo sortInfoB) {
			return !sortInfoA.Equals(sortInfoB);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		///   true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">Another object to compare to. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewSortInfo betterListViewSortInfo)) {
				return false;
			}
			if (this.columnIndex == betterListViewSortInfo.columnIndex) {
				return this.orderAscending == betterListViewSortInfo.orderAscending;
			}
			return false;
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		///   A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return this.columnIndex.GetHashCode() ^ this.orderAscending.GetHashCode();
		}

		private BetterListViewSortInfo(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.columnIndex = info.GetInt32("columnIndex");
			this.orderAscending = info.GetBoolean("orderAscending");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			info.AddValue("columnIndex", this.columnIndex);
			info.AddValue("orderAscending", this.orderAscending);
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			this.columnIndex = int.Parse(reader.ReadElementString("columnIndex"), CultureInfo.InvariantCulture);
			this.orderAscending = bool.Parse(reader.ReadElementString("orderAscending"));
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteElementString("columnIndex", this.columnIndex.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("orderAscending", this.orderAscending.ToString());
		}
	}
}