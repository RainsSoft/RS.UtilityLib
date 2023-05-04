using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Base class for all element types.
	/// </summary>
	[Serializable]
	[DesignTimeVisible(false)]
	public abstract class BetterListViewElementBase : IComponent, IDisposable, ICloneable, IComparable<BetterListViewElementBase>, ISerializable, IXmlSerializable
	{
		/// <summary>
		///   index which is not defined
		/// </summary>
		public const int IndexUndefined = -1;

		private const string FieldName = "name";

		private const string NameRoot = "BetterListViewElementBase";

		[NonSerialized]
		private int index = -1;

		private string name = string.Empty;

		[NonSerialized]
		private BetterListViewElementCollectionBase ownerCollection;

		/// <summary>
		///   flag for quick checking duplicate items
		/// </summary>
		internal bool DuplicityFlag { get; set; }

		/// <summary>
		///   Gets or sets the <see cref="T:System.ComponentModel.ISite" /> associated with the <see cref="T:System.ComponentModel.IComponent" />.
		/// </summary>
		/// <returns>
		///   The <see cref="T:System.ComponentModel.ISite" /> object associated with the component; or null, if the component does not have a site.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ISite Site { get; set; }

		/// <summary>
		///   font of the element text
		/// </summary>
		public abstract Font Font { get; set; }

		/// <summary>
		///   foreground color the element content
		/// </summary>
		public abstract Color ForeColor { get; set; }

		/// <summary>
		///   image of the element
		/// </summary>
		public abstract Image Image { get; set; }

		/// <summary>
		///   index specifying image of the element in ImageList
		///   set to BetterListViewElementBase.IndexUndefined, if not defined
		/// </summary>
		public abstract int ImageIndex { get; set; }

		/// <summary>
		///   key specifying image of the element in ImageList
		///   set to String.Empty, if not defined
		/// </summary>
		public abstract string ImageKey { get; set; }

		/// <summary>
		///   element text trimming
		/// </summary>
		internal abstract TextTrimming TextTrimming { get; }

		/// <summary>
		///   name of the element
		/// </summary>
		[Browsable(false)]
		[DefaultValue("")]
		public string Name {
			get {
				string text = null;
				if (!string.IsNullOrEmpty(this.name)) {
					text = this.name;
				}
				else if (this.Site != null) {
					text = this.Site.Name;
				}
				return text ?? string.Empty;
			}
			set {
				this.name = value ?? string.Empty;
				if (this.Site != null) {
					this.Site.Name = this.name;
				}
				this.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
			}
		}

		/// <summary>
		///   index of the element in its owner collection
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index {
			get {
				return this.index;
			}
			internal set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.index = value;
			}
		}

		/// <summary>
		///   collection that owns this element
		/// </summary>
		internal virtual BetterListViewElementCollectionBase OwnerCollection {
			get {
				return this.ownerCollection;
			}
			set {
				this.ownerCollection = value;
			}
		}

		/// <summary>
		///   custom data attached to the element
		/// </summary>
		[DefaultValue(null)]
		[Localizable(false)]
		[Category("Miscellaneous")]
		[TypeConverter(typeof(StringConverter))]
		[Bindable(true)]
		[Description("Custom data attached to the element")]
		public object Tag { get; set; }

		/// <summary>
		///   Represents the method that handles the <see cref="E:System.ComponentModel.IComponent.Disposed" /> event of a component.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementBase" /> class.
		/// </summary>
		protected internal BetterListViewElementBase(BetterListViewElementBoundsBase layoutBounds) {
			if (this is IBetterListViewLayoutElementDisplayable betterListViewLayoutElementDisplayable) {
				betterListViewLayoutElementDisplayable.LayoutBounds = layoutBounds;
			}
			else if (layoutBounds != null) {
				throw new ApplicationException("Cannot set boundaries to non-displayable layout element.");
			}
			this.ResetIndex();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementBase" /> class.
		/// </summary>
		/// <param name="image">element image</param>
		/// <param name="layoutBounds">corresponding element boundaries</param>
		protected internal BetterListViewElementBase(Image image, BetterListViewElementBoundsBase layoutBounds) {
			if (this is IBetterListViewLayoutElementDisplayable betterListViewLayoutElementDisplayable) {
				betterListViewLayoutElementDisplayable.LayoutBounds = layoutBounds;
			}
			else if (layoutBounds != null) {
				throw new ApplicationException("Cannot set boundaries to non-displayable layout element.");
			}
			this.Image = image;
			this.ResetIndex();
		}

		/// <summary>
		///   Remove element from its owner collection.
		/// </summary>
		/// <returns>success</returns>
		public abstract bool Remove();

		/// <summary>
		/// Check whether properties of this element equals properties of the specified element.
		/// </summary>
		/// <param name="other">Element to check.</param>
		/// <returns>
		/// Properties of this element equals properties of the specified element.
		/// </returns>
		public virtual bool EqualsContent(BetterListViewElementBase other) {
			if (this == other) {
				return true;
			}
			if (other == null) {
				return false;
			}
			return this.name.Equals(other.name, StringComparison.Ordinal);
		}

		/// <summary>
		///   Called when element property has changed.
		/// </summary>
		/// <param name="elementPropertyType">element property type</param>
		internal void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType) {
			this.OnElementPropertyChanged(elementPropertyType, null, null);
		}

		/// <summary>
		///   Called when element property has changed.
		/// </summary>
		/// <param name="elementPropertyType">element property type</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		internal void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, object oldValue) {
			this.OnElementPropertyChanged(elementPropertyType, oldValue, null);
		}

		/// <summary>
		///   Report that property of this element has changed.
		/// </summary>
		/// <param name="elementPropertyType">type of the changed property</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		/// <param name="eventArgs">event data (in case of property change event should be fired)</param>
		internal virtual void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, object oldValue, EventArgs eventArgs) {
			if (this.OwnerCollection != null) {
				this.OwnerCollection.OnElementPropertyChanged(elementPropertyType, this, oldValue, eventArgs);
			}
		}

		/// <summary>
		///   Set item index to undefined value.
		/// </summary>
		/// <remarks>
		///   For internal use.
		/// </remarks>
		internal virtual void ResetIndex() {
			this.index = -1;
		}

		/// <summary>
		///   Copy content of this instance to the specified BetterListViewElementBase instance.
		/// </summary>
		/// <param name="element">BetterListViewElementBase to copy the content to</param>
		protected virtual void CopyTo(BetterListViewElementBase element) {
			element.Font = this.Font;
			element.ForeColor = this.ForeColor;
			element.Image = this.Image;
			element.ImageIndex = this.ImageIndex;
			element.ImageKey = this.ImageKey;
			element.Tag = this.Tag;
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose() {
			if (this.Disposed != null) {
				this.Disposed(this, EventArgs.Empty);
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
		///   Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
		///   Value 
		///   Meaning 
		///   Less than zero 
		///   This object is less than the <paramref name="other" /> parameter.
		///   Zero 
		///   This object is equal to <paramref name="other" />. 
		///   Greater than zero 
		///   This object is greater than <paramref name="other" />. 
		/// </returns>
		/// <param name="other">An object to compare with this object.
		/// </param>
		public abstract int CompareTo(BetterListViewElementBase other);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementBase" /> class.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected BetterListViewElementBase(SerializationInfo info, StreamingContext context) {
			this.name = info.GetString("name");
		}

		/// <summary>
		///   Custom implementation of the GetObjectData method.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected virtual void GetObjectDataInternal(SerializationInfo info, StreamingContext context) {
			info.AddValue("name", this.name);
		}

		/// <summary>
		///   Custom implementation of the ReadXml method.
		/// </summary>
		/// <param name="reader">XML reader</param>
		protected virtual void ReadXmlInternal(XmlReader reader) {
			reader.ReadStartElement("BetterListViewElementBase");
			this.name = reader.ReadElementString("name");
			reader.ReadEndElement();
		}

		/// <summary>
		///   Custom implementation of the WriteXml method.
		/// </summary>
		/// <param name="writer">XML writer</param>
		protected virtual void WriteXmlInternal(XmlWriter writer) {
			writer.WriteStartElement("BetterListViewElementBase");
			writer.WriteElementString("name", this.name);
			writer.WriteEndElement();
		}

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