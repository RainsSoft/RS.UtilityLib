using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Insertion mark for pinpointing the place of item insertion within other items.
	/// </summary>
	[Serializable]
	public struct BetterListViewInsertionMark : ISerializable, IXmlSerializable
	{
		private const string FieldInsertionLocation = "insertionLocation";

		private const string FieldColor = "color";

		private const string FieldEnabled = "enabled";

		/// <summary>
		///   default color of the insertion mark
		/// </summary>
		public static readonly Color DefaultColor = Control.DefaultForeColor;

		/// <summary>
		///   represents an empty InsertionMark structure
		/// </summary>
		public static readonly BetterListViewInsertionMark Empty = new BetterListViewInsertionMark(BetterListViewInsertionLocation.Empty);

		private BetterListViewInsertionLocation insertionLocation;

		private Color color;

		private bool enabled;

		/// <summary>
		///   index of item on which this insertion mark lies
		/// </summary>
		public int Index {
			get {
				return this.insertionLocation.Address.Index;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.insertionLocation = new BetterListViewInsertionLocation(new BetterListViewAddress(value), this.insertionLocation.DropPart, 0);
			}
		}

		/// <summary>
		///   insertion mark is displayed after the item
		/// </summary>
		public bool ShowAfterItem {
			get {
				return this.insertionLocation.DropPart == BetterListViewDropPart.After;
			}
			set {
				this.insertionLocation = new BetterListViewInsertionLocation(this.insertionLocation.Address, value ? BetterListViewDropPart.After : BetterListViewDropPart.Before, 0);
			}
		}

		/// <summary>
		///   exact location of the insertion mark
		/// </summary>
		public BetterListViewInsertionLocation InsertionLocation {
			get {
				return this.insertionLocation;
			}
			set {
				this.insertionLocation = value;
			}
		}

		/// <summary>
		///   display color
		/// </summary>
		public Color Color {
			get {
				return this.color;
			}
			set {
				Checks.CheckFalse(value.IsEmpty, "value.IsEmpty");
				this.color = value;
			}
		}

		/// <summary>
		///   show insertion mark in disabled state
		/// </summary>
		public bool Enabled {
			get {
				return this.enabled;
			}
			set {
				this.enabled = value;
			}
		}

		/// <summary>
		///   this InsertionMark structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewInsertionMark.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="index">index of item on which this insertion mark lies</param>
		public BetterListViewInsertionMark(int index)
			: this(index, showAfterItem: true) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="index">index of item on which this insertion mark lies</param>
		/// <param name="showAfterItem"> insertion mark is displayed after the item</param>
		public BetterListViewInsertionMark(int index, bool showAfterItem)
			: this(index, showAfterItem, Color.Empty) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="index">index of item on which this insertion mark lies</param>
		/// <param name="showAfterItem"> insertion mark is displayed after the item</param>
		/// <param name="color">display color</param>
		public BetterListViewInsertionMark(int index, bool showAfterItem, Color color)
			: this(index, showAfterItem, color, enabled: true) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="index">index of item on which this insertion mark lies</param>
		/// <param name="showAfterItem"> insertion mark is displayed after the item</param>
		/// <param name="color">display color</param>
		/// <param name="enabled">show insertion mark in enabled state</param>
		public BetterListViewInsertionMark(int index, bool showAfterItem, Color color, bool enabled)
			: this(new BetterListViewInsertionLocation(new BetterListViewAddress(index), showAfterItem ? BetterListViewDropPart.After : BetterListViewDropPart.Before, 0), color, enabled) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="insertionLocation">exact insertion location</param>
		public BetterListViewInsertionMark(BetterListViewInsertionLocation insertionLocation)
			: this(insertionLocation, Color.Empty) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="insertionLocation">exact insertion location</param>
		/// <param name="color">display color</param>
		public BetterListViewInsertionMark(BetterListViewInsertionLocation insertionLocation, Color color)
			: this(insertionLocation, color, enabled: true) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewInsertionMark" /> struct.
		/// </summary>
		/// <param name="insertionLocation">exact insertion location</param>
		/// <param name="color">display color</param>
		/// <param name="enabled">show insertion mark in enabled state</param>
		public BetterListViewInsertionMark(BetterListViewInsertionLocation insertionLocation, Color color, bool enabled) {
			this.insertionLocation = insertionLocation;
			this.color = color;
			this.enabled = enabled;
		}

		/// <summary>
		///   Test whether the two BetterListViewInsertionMark objects are identical.
		/// </summary>
		/// <param name="insertionMarkA">first BetterListViewInsertionMark object</param>
		/// <param name="insertionMarkB">second BetterListViewInsertionMark object</param>
		/// <returns>the two BetterListViewInsertionMark objects are identical</returns>
		public static bool operator ==(BetterListViewInsertionMark insertionMarkA, BetterListViewInsertionMark insertionMarkB) {
			return insertionMarkA.Equals(insertionMarkB);
		}

		/// <summary>
		///   Test whether the two BetterListViewInsertionMark objects are different.
		/// </summary>
		/// <param name="insertionMarkA">first BetterListViewInsertionMark object</param>
		/// <param name="insertionMarkB">second BetterListViewInsertionMark object</param>
		/// <returns>the two BetterListViewInsertionMark objects are different</returns>
		public static bool operator !=(BetterListViewInsertionMark insertionMarkA, BetterListViewInsertionMark insertionMarkB) {
			return !insertionMarkA.Equals(insertionMarkB);
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
			if (!(obj is BetterListViewInsertionMark betterListViewInsertionMark)) {
				return false;
			}
			if (this.insertionLocation.Equals(betterListViewInsertionMark.insertionLocation) && this.color == betterListViewInsertionMark.color) {
				return this.enabled == betterListViewInsertionMark.enabled;
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
			return this.insertionLocation.GetHashCode() ^ this.color.GetHashCode() ^ this.enabled.GetHashCode();
		}

		private BetterListViewInsertionMark(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.insertionLocation = (BetterListViewInsertionLocation)info.GetValue("insertionLocation", typeof(BetterListViewInsertionLocation));
			this.color = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("color"));
			this.enabled = info.GetBoolean("enabled");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			info.AddValue("insertionLocation", this.insertionLocation);
			info.AddValue("color", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.color));
			info.AddValue("enabled", this.enabled);
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			this.insertionLocation = (BetterListViewInsertionLocation)new XmlSerializer(typeof(BetterListViewInsertionLocation)).Deserialize(reader);
			this.color = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("color"));
			this.enabled = bool.Parse(reader.ReadElementString("enabled"));
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			new XmlSerializer(typeof(BetterListViewInsertionLocation)).Serialize(writer, this.insertionLocation);
			writer.WriteElementString("color", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.color));
			writer.WriteElementString("enabled", this.enabled.ToString());
		}
	}
}