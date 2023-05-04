using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a location of group/item in Better ListView.
	/// </summary>
	/// <summary>
	///   Represents a location of group/item in Better ListView.
	/// </summary>
	[Serializable]
	public sealed class BetterListViewAddress : ICloneable, IComparable<BetterListViewAddress>, IComparable, ISerializable, IXmlSerializable
	{
		private const char GroupSeparator = ':';

		private const char IndexSeparator = '.';

		private const string FieldIsGroup = "isGroup";

		private const string FieldParentAddress = "parentAddress";

		private const string FieldIndex = "index";

		private int index;

		private BetterListViewAddress parentAddress;

		private bool isGroup;

		/// <summary>
		///   Gets the index of the element within current context.
		/// </summary>
		/// <value>The index of the element within current context.</value>
		public int Index => this.index;

		/// <summary>
		///   Gets the parent element address.
		/// </summary>
		/// <value>The parent element address.</value>
		public BetterListViewAddress ParentAddress => this.parentAddress;

		/// <summary>
		///   Gets a value indicating whether this address is a group address.
		/// </summary>
		/// <value>
		///   <c>true</c> if this address is a group address; otherwise, <c>false</c>.
		/// </value>
		public bool IsGroup => this.isGroup;

		/// <summary>
		///   Gets the level of addressed element in item hierarchy..
		/// </summary>
		/// <value>The level of addressed element in item hierarchy.</value>
		public int Level {
			get {
				if (this.isGroup) {
					return -1;
				}
				int num = 0;
				BetterListViewAddress betterListViewAddress = this;
				while (betterListViewAddress.parentAddress != null && !betterListViewAddress.parentAddress.isGroup) {
					betterListViewAddress = betterListViewAddress.parentAddress;
					num++;
				}
				return num;
			}
		}

		/// <summary>
		///   Gets the index of addressed element group.
		/// </summary>
		/// <value>The index of addressed element group.</value>
		public int IndexGroup {
			get {
				if (this.isGroup) {
					return this.index;
				}
				if (!(this.parentAddress != null)) {
					return -1;
				}
				return this.parentAddress.IndexGroup;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAddress" /> class.
		/// </summary>
		/// <param name="index">Index of the element within current context.</param>
		public BetterListViewAddress(int index)
			: this(index, null) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAddress" /> class.
		/// </summary>
		/// <param name="index">Index of the element within current context.</param>
		/// <param name="parentAddress">Parent element address.</param>
		public BetterListViewAddress(int index, BetterListViewAddress parentAddress)
			: this(index, parentAddress, isGroup: false) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAddress" /> class.
		/// </summary>
		/// <param name="index">Index of the element within current context.</param>
		/// <param name="parentAddress">Parent element address.</param>
		/// <param name="isGroup">This address is a group address.</param>
		public BetterListViewAddress(int index, BetterListViewAddress parentAddress, bool isGroup) {
			Checks.CheckTrue(index == -1 || index >= 0, "index == BetterListViewElementBase.IndexUndefined || index >= 0");
			if (isGroup) {
				Checks.CheckTrue(parentAddress == null, "parentAddress == null");
			}
			this.index = index;
			this.parentAddress = parentAddress;
			this.isGroup = isGroup;
		}

		/// <summary>
		///   Test whether the two addresses are identical.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The two addresses are identical.</returns>
		public static bool operator ==(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return object.Equals(addressA, addressB);
		}

		/// <summary>
		///   Test whether the two addresses are different.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The two addresses are different.</returns>
		public static bool operator !=(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return !object.Equals(addressA, addressB);
		}

		/// <summary>
		///   Test whether the first specified address value is greater than the second specified address value.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The first specified address value is greater than the second specified address value.</returns>
		public static bool operator >(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return addressA.CompareTo(addressB) > 0;
		}

		/// <summary>
		///   Test whether the first specified address value is less than the second specified address value.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The first specified address value is less than the second specified address value.</returns>
		public static bool operator <(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return addressA.CompareTo(addressB) < 0;
		}

		/// <summary>
		///   Test whether the first specified address value is greater or equal to the second specified address value.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The first specified address value is greater or equal to the second specified address value.</returns>
		public static bool operator >=(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return addressA.CompareTo(addressB) >= 0;
		}

		/// <summary>
		///   Test whether the first specified address value is less or equal to the second specified address value.
		/// </summary>
		/// <param name="addressA">First BetterListViewAddress object.</param>
		/// <param name="addressB">Second BetterListViewAddress object.</param>
		/// <returns>The first specified address value is less or equal to the second specified address value.</returns>
		public static bool operator <=(BetterListViewAddress addressA, BetterListViewAddress addressB) {
			return addressA.CompareTo(addressB) <= 0;
		}

		/// <summary>
		///   Create a new BetterListViewAddress instance from its string representation.
		/// </summary>
		/// <param name="strAddress">String representation of a BetterListViewAddress.</param>
		/// <returns>BetterListViewAddress instance.</returns>
		public static BetterListViewAddress Parse(string strAddress) {
			Checks.CheckNotNull(strAddress, "strAddress");
			if (strAddress.Length == 0) {
				return null;
			}
			string[] array = strAddress.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			BetterListViewAddress betterListViewAddress;
			if (array.Length == 2) {
				betterListViewAddress = new BetterListViewAddress(int.Parse(array[0]), null, isGroup: true);
				strAddress = array[1];
			}
			else {
				betterListViewAddress = null;
			}
			string[] array2 = strAddress.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			BetterListViewAddress result = new BetterListViewAddress(int.Parse(array2[0]), betterListViewAddress);
			for (int i = 1; i < array2.Length; i++) {
				result = new BetterListViewAddress(int.Parse(array2[i]), result);
			}
			return result;
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>True</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		///   The <paramref name="obj" /> parameter is null.
		/// </exception>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewAddress)) {
				return false;
			}
			BetterListViewAddress betterListViewAddress = (BetterListViewAddress)obj;
			if (this.index == betterListViewAddress.index && ((this.parentAddress == null || betterListViewAddress.parentAddress == null) ? ((object)this.parentAddress == betterListViewAddress.parentAddress) : this.parentAddress.Equals(betterListViewAddress.parentAddress))) {
				return this.isGroup == betterListViewAddress.isGroup;
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.index.GetHashCode() ^ ((this.parentAddress != null) ? this.parentAddress.GetHashCode() : 0) ^ this.isGroup.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			return this.ToString(includeTypeName: true, includeGroupAddress: true);
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <param name="includeTypeName">Include name of this type.</param>
		/// <param name="includeGroupAddress">Include address of the element group.</param>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public string ToString(bool includeTypeName, bool includeGroupAddress) {
			StringBuilder stringBuilder = new StringBuilder();
			BetterListViewAddress betterListViewAddress = this;
			do {
				if (betterListViewAddress.IsGroup) {
					if (includeGroupAddress) {
						stringBuilder.Insert(0, betterListViewAddress.Index.ToString(CultureInfo.InvariantCulture) + ":");
					}
					break;
				}
				stringBuilder.Insert(0, betterListViewAddress.Index.ToString(CultureInfo.InvariantCulture) + ".");
				betterListViewAddress = betterListViewAddress.parentAddress;
			}
			while (betterListViewAddress != null);
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			if (includeTypeName) {
				stringBuilder.Insert(0, base.GetType().Name + ": ");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		///   Convert this address to numeric representation.
		/// </summary>
		/// <param name="groupIndex">Element group index.</param>
		/// <param name="colItemIndices">Numeric representation of this address.</param>
		internal void ToNumericAddress(out int groupIndex, out Stack<int> colItemIndices) {
			groupIndex = -1;
			colItemIndices = new Stack<int>();
			BetterListViewAddress betterListViewAddress = this;
			do {
				if (betterListViewAddress.isGroup) {
					groupIndex = betterListViewAddress.index;
				}
				else {
					colItemIndices.Push(betterListViewAddress.index);
				}
				betterListViewAddress = betterListViewAddress.parentAddress;
			}
			while (betterListViewAddress != null);
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public object Clone() {
			return new BetterListViewAddress(this.index, (this.parentAddress != null) ? ((BetterListViewAddress)this.parentAddress.Clone()) : null, this.isGroup);
		}

		/// <summary>
		///   Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
		/// </returns>
		public int CompareTo(BetterListViewAddress other) {
			Checks.CheckNotNull(other, "other");
			this.ToNumericAddress(out var groupIndex, out var colItemIndices);
			other.ToNumericAddress(out var groupIndex2, out var colItemIndices2);
			int num = groupIndex;
			int value = groupIndex2;
			int num2 = num.CompareTo(value);
			if (num2 != 0) {
				return num2;
			}
			do {
				num = colItemIndices.Pop();
				value = colItemIndices2.Pop();
				num2 = num.CompareTo(value);
				if (num2 != 0) {
					return num2;
				}
			}
			while (colItemIndices.Count != 0 && colItemIndices2.Count != 0);
			num = ((colItemIndices.Count != 0) ? 1 : 0);
			value = ((colItemIndices2.Count != 0) ? 1 : 0);
			return num.CompareTo(value);
		}

		/// <summary>
		///   Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
		///   Value
		///   Meaning
		///   Less than zero
		///   This instance is less than <paramref name="obj" />.
		///   Zero
		///   This instance is equal to <paramref name="obj" />.
		///   Greater than zero
		///   This instance is greater than <paramref name="obj" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="obj" /> is not the same type as this instance.
		/// </exception>
		int IComparable.CompareTo(object obj) {
			Checks.CheckType(obj, typeof(BetterListViewAddress), "obj");
			return this.CompareTo((BetterListViewAddress)obj);
		}

		/// <summary>
		///   Prevents a default instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAddress" /> class from being created.
		/// </summary>
		/// <remarks>
		///   Empty constructor needed for deserialization.
		/// </remarks>
		private BetterListViewAddress() {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewAddress" /> class.
		/// </summary>
		/// <param name="info">Serialization info.</param>
		/// <param name="context">Serialization context.</param>
		private BetterListViewAddress(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			this.parentAddress = (BetterListViewAddress)info.GetValue("parentAddress", typeof(BetterListViewAddress));
			this.index = info.GetInt32("index");
			this.isGroup = info.GetBoolean("isGroup");
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
			info.AddValue("parentAddress", this.parentAddress);
			info.AddValue("index", this.index);
			info.AddValue("isGroup", this.isGroup);
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
			this.index = int.Parse(reader.ReadElementString("index"), CultureInfo.InvariantCulture);
			if (reader.IsEmptyElement) {
				this.parentAddress = null;
				reader.Read();
			}
			else {
				reader.ReadStartElement("parentAddress");
				this.parentAddress = (BetterListViewAddress)new XmlSerializer(typeof(BetterListViewAddress)).Deserialize(reader);
				reader.ReadEndElement();
			}
			this.isGroup = bool.Parse(reader.ReadElementString("isGroup"));
			reader.ReadEndElement();
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. 
		/// </param>
		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			writer.WriteElementString("index", this.index.ToString(CultureInfo.InvariantCulture));
			writer.WriteStartElement("parentAddress");
			if (this.parentAddress != null) {
				new XmlSerializer(typeof(BetterListViewAddress)).Serialize(writer, this.parentAddress);
			}
			writer.WriteEndElement();
			writer.WriteElementString("isGroup", this.isGroup.ToString());
		}
	}
}