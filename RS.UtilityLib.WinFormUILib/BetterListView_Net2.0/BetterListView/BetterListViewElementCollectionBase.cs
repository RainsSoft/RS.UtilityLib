using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Base class for all collection used by BetterListView.
	/// </summary>
	[Serializable]
	public abstract class BetterListViewElementCollectionBase : ICloneable, ISerializable, IXmlSerializable
	{
		[NonSerialized]
		private bool isInternal;

		/// <summary>
		///   control that owns this collection
		/// </summary>
		internal abstract BetterListViewBase OwnerControl { get; set; }

		/// <summary>
		///   this collection is internal (attached to a specific control)
		/// </summary>
		internal bool IsInternal {
			get {
				return this.isInternal;
			}
			set {
				this.isInternal = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementCollectionBase" /> class.
		/// </summary>
		/// <param name="isInternal">this collection is internal (attached to a specific control)</param>
		protected BetterListViewElementCollectionBase(bool isInternal) {
			this.isInternal = isInternal;
		}

		/// <summary>
		/// Get element from the specified enumeration with the provided key.
		/// </summary>
		/// <param name="elements">Elements to search.</param>
		/// <param name="key">Search key.</param>
		/// <returns>Fount element or null.</returns>
		internal static BetterListViewElementBase GetElementByKey(IEnumerable elements, string key) {
			Checks.CheckNotNull(elements, "elements");
			if (string.IsNullOrEmpty(key)) {
				return null;
			}
			foreach (BetterListViewElementBase element in elements) {
				if (!string.IsNullOrEmpty(element.Name) && key.Equals(element.Name, StringComparison.InvariantCultureIgnoreCase)) {
					return element;
				}
			}
			return null;
		}

		/// <summary>
		///   Report that this collection has changed.
		/// </summary>
		/// <param name="changeInfo">information about changes made to the collection</param>
		internal virtual void OnCollectionChanged(BetterListViewElementCollectionChangeInfo changeInfo) {
			Checks.CheckNotNull(changeInfo, "changeInfo");
			if (this.OwnerControl != null) {
				this.OwnerControl.OnCollectionChanged(this, changeInfo);
			}
		}

		/// <summary>
		///   Called when property of some element within this collection has changed.
		/// </summary>
		/// <param name="elementPropertyType">type of the changed property</param>
		/// <param name="element">the element whose property has changed</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		internal void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, BetterListViewElementBase element, object oldValue) {
			this.OnElementPropertyChanged(elementPropertyType, element, oldValue, null);
		}

		/// <summary>
		///   Called when property of some element within this collection has changed.
		/// </summary>
		/// <param name="elementPropertyType">type of the changed property</param>
		/// <param name="element">the element whose property has changed</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		/// <param name="eventArgs">event data (in case of property change event should be fired)</param>
		internal void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, BetterListViewElementBase element, object oldValue, EventArgs eventArgs) {
			Checks.CheckNotNull(element, "element");
			if (this.OwnerControl != null) {
				this.OwnerControl.OnElementPropertyChanged(this, elementPropertyType, element, oldValue, eventArgs);
			}
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public abstract object Clone();

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		///   The caller does not have the required permission.
		/// </exception>
		protected abstract void GetObjectDataInternal(SerializationInfo info, StreamingContext context);

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		protected abstract void ReadXmlInternal(XmlReader reader);

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		protected abstract void WriteXmlInternal(XmlWriter writer);

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		///   The caller does not have the required permission.
		/// </exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			this.GetObjectDataInternal(info, context);
		}

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		void IXmlSerializable.ReadXml(XmlReader reader) {
			this.ReadXmlInternal(reader);
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer) {
			this.WriteXmlInternal(writer);
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