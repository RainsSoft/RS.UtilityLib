using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents insertion location within Better ListView.
	/// </summary>
	[Serializable]
	public struct BetterListViewInsertionLocation : ISerializable, IXmlSerializable
	{
		/// <summary>
		///   value for insertion level that is undefined
		/// </summary>
		public const int LevelUndefined = -1;

		private const string FieldAddress = "address";

		private const string FieldDropPart = "dropPart";

		private const string FieldLevel = "level";

		private const string FieldOffset = "offset";

		/// <summary>
		///   empty BetterListViewInsertionLocation structure
		/// </summary>
		public static readonly BetterListViewInsertionLocation Empty = new BetterListViewInsertionLocation(isEmpty: true, null, BetterListViewDropPart.Undefined, -1, 0);

		private BetterListViewAddress address;

		private BetterListViewDropPart dropPart;

		private int level;

		private int offset;

		/// <summary>
		///   address of target element
		/// </summary>
		public BetterListViewAddress Address => this.address;

		/// <summary>
		///   part of target element
		/// </summary>
		public BetterListViewDropPart DropPart => this.dropPart;

		/// <summary>
		///   target element level
		/// </summary>
		public int Level => this.level;

		/// <summary>
		///   insertion mark placement offset
		/// </summary>
		public int Offset => this.offset;

		/// <summary>
		///   this BetterListViewInsertionLocation structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewInsertionLocation.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionLocation" /> struct.
		/// </summary>
		/// <param name="address">address of target element</param>
		public BetterListViewInsertionLocation(BetterListViewAddress address)
			: this(address, BetterListViewDropPart.After) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionLocation" /> struct.
		/// </summary>
		/// <param name="address">address of target element</param>
		/// <param name="dropPart">part of target element.</param>
		public BetterListViewInsertionLocation(BetterListViewAddress address, BetterListViewDropPart dropPart)
			: this(address, dropPart, -1) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionLocation" /> struct.
		/// </summary>
		/// <param name="address">address of target element</param>
		/// <param name="dropPart">part of target element.</param>
		/// <param name="level">target element level</param>
		public BetterListViewInsertionLocation(BetterListViewAddress address, BetterListViewDropPart dropPart, int level)
			: this(address, dropPart, level, 0) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionLocation" /> struct.
		/// </summary>
		/// <param name="address">address of target element</param>
		/// <param name="dropPart">part of target element.</param>
		/// <param name="level">target element level</param>
		/// <param name="offset">insertion mark placement offset</param>
		public BetterListViewInsertionLocation(BetterListViewAddress address, BetterListViewDropPart dropPart, int level, int offset)
			: this(isEmpty: false, address, dropPart, level, offset) {
		}

		private BetterListViewInsertionLocation(bool isEmpty, BetterListViewAddress address, BetterListViewDropPart dropPart, int level, int offset) {
			if (isEmpty) {
				Checks.CheckTrue(address == null, "address == null");
				Checks.CheckEqual(dropPart, BetterListViewDropPart.Undefined, "dropPart", "BetterListViewDropPart.Undefined");
				Checks.CheckEqual(level, -1, "level", "LevelUndefined");
			}
			else {
				Checks.CheckNotNull(address, "address");
				if (address.IsGroup) {
					Checks.CheckEqual(level, -1, "level", "LevelUndefined");
					Checks.CheckEqual(dropPart, BetterListViewDropPart.Inside, "dropPart", "BetterListViewDropPart.Inside");
				}
				else {
					Checks.CheckNotEqual(dropPart, BetterListViewDropPart.Undefined, "dropPart", "BetterListViewDropPart.Undefined");
					Checks.CheckTrue(level == -1 || level <= address.Level, "(level == LevelUndefined) || (level <= address.Level)");
				}
			}
			this.address = address;
			this.dropPart = dropPart;
			this.level = level;
			this.offset = offset;
		}

		private BetterListViewInsertionLocation(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.address = (BetterListViewAddress)info.GetValue("address", typeof(BetterListViewAddress));
			this.dropPart = (BetterListViewDropPart)Enum.Parse(typeof(BetterListViewDropPart), info.GetString("dropPart"));
			this.level = info.GetInt32("level");
			this.offset = info.GetInt32("offset");
		}

		/// <summary>
		///   Test whether the two BetterListViewInsertionLocation objects are identical.
		/// </summary>
		/// <param name="insertionLocationA">first BetterListViewInsertionLocation object</param>
		/// <param name="insertionLocationB">second BetterListViewInsertionLocation object</param>
		/// <returns>the two BetterListViewInsertionLocation objects are identical</returns>
		public static bool operator ==(BetterListViewInsertionLocation insertionLocationA, BetterListViewInsertionLocation insertionLocationB) {
			return insertionLocationA.Equals(insertionLocationB);
		}

		/// <summary>
		///   Test whether the two BetterListViewInsertionLocation objects are different.
		/// </summary>
		/// <param name="insertionLocationA">first BetterListViewInsertionLocation object</param>
		/// <param name="insertionLocationB">second BetterListViewInsertionLocation object</param>
		/// <returns>the two BetterListViewInsertionLocation objects are different</returns>
		public static bool operator !=(BetterListViewInsertionLocation insertionLocationA, BetterListViewInsertionLocation insertionLocationB) {
			return !insertionLocationA.Equals(insertionLocationB);
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
			if (!(obj is BetterListViewInsertionLocation betterListViewInsertionLocation)) {
				return false;
			}
			if (this.address == betterListViewInsertionLocation.address && this.dropPart == betterListViewInsertionLocation.dropPart && this.level == betterListViewInsertionLocation.level) {
				return this.offset == betterListViewInsertionLocation.offset;
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
			return this.address.GetHashCode() ^ this.dropPart.GetHashCode() ^ this.level.GetHashCode() ^ this.offset.GetHashCode();
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
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			info.AddValue("address", this.address);
			info.AddValue("dropPart", this.dropPart);
			info.AddValue("level", this.level);
			info.AddValue("offset", this.offset);
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

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. 
		/// </param>
		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			if (!reader.IsEmptyElement) {
				reader.ReadStartElement("address");
				this.address = (BetterListViewAddress)new XmlSerializer(typeof(BetterListViewAddress)).Deserialize(reader);
				reader.ReadEndElement();
			}
			else {
				reader.Read();
			}
			this.dropPart = (BetterListViewDropPart)Enum.Parse(typeof(BetterListViewDropPart), reader.ReadElementString("dropPart"));
			this.level = int.Parse(reader.ReadElementString("level"));
			this.offset = int.Parse(reader.ReadElementString("offset"));
			reader.ReadEndElement();
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. 
		/// </param>
		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteStartElement("address");
			if (this.address != null) {
				new XmlSerializer(typeof(BetterListViewAddress)).Serialize(writer, this.address);
			}
			writer.WriteEndElement();
			writer.WriteElementString("dropPart", this.dropPart.ToString());
			writer.WriteElementString("level", this.level.ToString());
			writer.WriteElementString("offset", this.offset.ToString());
		}
	}
}