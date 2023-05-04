using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item data holder for Drag and Drop operations.
	/// </summary>
	[Serializable]
	public sealed class BetterListViewItemDragData : ISerializable, IXmlSerializable
	{
		private const string FieldItems = "items";

		private const char Separator = ';';

		private const string FieldDragSourceName = "dragSourceName";

		private const string FieldDragSourceID = "dragSourceID";

		private string dragSourceName;

		private string dragSourceID;

		private BetterListViewItemCollection items;

		/// <summary>
		///   name of the source control
		/// </summary>
		public string DragSourceName => this.dragSourceName;

		/// <summary>
		///   identifier of the source control
		/// </summary>
		public string DragSourceID => this.dragSourceID;

		/// <summary>
		///   dragged items
		/// </summary>
		public BetterListViewItemCollection Items => this.items;

		/// <summary>
		///   Initialize a new BetterListViewItemDragData instance.
		/// </summary>
		public BetterListViewItemDragData() {
			this.dragSourceName = string.Empty;
			this.dragSourceID = string.Empty;
			this.items = new BetterListViewItemCollection();
		}

		/// <summary>
		///   Initialize a new BetterListViewItemDragData instance.
		/// </summary>
		/// <param name="dragSourceName">name of the source control</param>
		/// <param name="dragSourceID">identifier of the source control</param>
		/// <param name="items">dragged items</param>
		public BetterListViewItemDragData(string dragSourceName, string dragSourceID, BetterListViewItemCollection items) {
			Checks.CheckNotNull(dragSourceName, "dragSourceName");
			Checks.CheckNotNull(dragSourceID, "source");
			Checks.CheckNotNull(items, "items");
			this.dragSourceName = dragSourceName;
			this.dragSourceID = dragSourceID;
			this.items = items;
		}

		/// <summary>
		///   Check whether properties of this object are equal to properties of the specified object.
		/// </summary>
		/// <param name="other">Object to check.</param>
		/// <returns>Properties of this object are equal to properties of the specified object.</returns>
		public bool EqualsContent(BetterListViewItemDragData other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			if (this.dragSourceName.Equals(other.dragSourceName, StringComparison.Ordinal) && this.dragSourceID.Equals(other.dragSourceID, StringComparison.Ordinal)) {
				return this.items.EqualsContent(other.items);
			}
			return false;
		}

		private static ReadOnlyCollection<int> IndicesFromString(string strIndices) {
			if (strIndices.Length == 0) {
				return new ReadOnlyCollection<int>(new int[0]);
			}
			List<int> list = new List<int>();
			string[] array = strIndices.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			string[] array3 = array2;
			foreach (string s in array3) {
				list.Add(int.Parse(s));
			}
			return list.AsReadOnly();
		}

		private static string IndicesToString(ReadOnlyCollection<int> indices) {
			if (indices.Count == 0) {
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int index in indices) {
				stringBuilder.Append(index);
				stringBuilder.Append(';');
			}
			if (indices.Count != 0) {
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		private BetterListViewItemDragData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.dragSourceName = info.GetString("dragSourceName");
			this.dragSourceID = info.GetString("dragSourceID");
			this.items = (BetterListViewItemCollection)info.GetValue("items", typeof(BetterListViewItemCollection));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			info.AddValue("dragSourceName", this.dragSourceName);
			info.AddValue("dragSourceID", this.dragSourceID);
			info.AddValue("items", this.items, typeof(BetterListViewItemCollection));
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			this.dragSourceName = reader.ReadElementString("dragSourceName");
			this.dragSourceID = reader.ReadElementString("dragSourceID");
			reader.ReadStartElement("items");
			this.items = (BetterListViewItemCollection)new XmlSerializer(typeof(BetterListViewItemCollection)).Deserialize(reader);
			reader.ReadEndElement();
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteElementString("dragSourceName", this.dragSourceName);
			writer.WriteElementString("dragSourceID", this.dragSourceID);
			writer.WriteStartElement("items");
			new XmlSerializer(typeof(BetterListViewItemCollection)).Serialize(writer, this.items);
			writer.WriteEndElement();
		}
	}
}