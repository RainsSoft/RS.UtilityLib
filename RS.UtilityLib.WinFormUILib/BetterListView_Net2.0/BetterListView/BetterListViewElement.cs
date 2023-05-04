using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents an BetterListView element.
	/// </summary>
	[Serializable]
	public abstract class BetterListViewElement : BetterListViewElementBase
	{
		private const string FieldImage = "image";

		private const string FieldImageIndex = "imageIndex";

		private const string FieldImageKey = "imageKey";

		private const string NameRoot = "BetterListViewElement";

		private Image image;

		private int imageIndex = -1;

		private string imageKey = string.Empty;

		/// <summary>
		///   image of the element
		///   this image is used prior to the image specified by ImageIndex/ImageKey property
		/// </summary>
		[Category("Appearance")]
		[Description("Image of the element; this image is used prior to the image specified by ImageIndex/ImageKey property")]
		[DefaultValue(null)]
		public override Image Image {
			get {
				return this.image;
			}
			set {
				if (this.image != value) {
					object oldValue = this.image;
					this.image = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.LayoutImage, oldValue);
				}
			}
		}

		/// <summary>
		///   index specifying image of the element in ImageList
		///   set to BetterListViewElementBase.IndexUndefined, if not defined
		/// </summary>
		[Description("Index specifying image of the element in ImageList; set to BetterListViewElementBase.IndexUndefined, if not defined")]
		[DefaultValue(-1)]
		[Category("Appearance")]
		public override int ImageIndex {
			get {
				return this.imageIndex;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
				if (this.imageIndex != value) {
					object oldValue = this.imageIndex;
					this.imageIndex = value;
					this.imageKey = string.Empty;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.LayoutImage, oldValue);
				}
			}
		}

		/// <summary>
		///   key specifying image of the element in ImageList
		///   set to String.Empty, if not defined
		/// </summary>
		[DefaultValue("")]
		[Description("Key specifying image of the element in ImageList; set to String.Empty, if not defined")]
		[Category("Appearance")]
		public override string ImageKey {
			get {
				return this.imageKey;
			}
			set {
				value = value ?? string.Empty;
				if (!this.imageKey.Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
					object oldValue = this.imageKey;
					this.imageIndex = -1;
					this.imageKey = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.LayoutImage, oldValue);
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElement" /> class.
		/// </summary>
		/// <param name="image">element image</param>
		/// <param name="layoutBounds">corresponding element boundaries</param>
		protected BetterListViewElement(Image image, BetterListViewElementBoundsBase layoutBounds)
			: base(image, layoutBounds) {
		}

		/// <summary>
		/// Check whether properties of this element equals properties of the specified element.
		/// </summary>
		/// <param name="other">Element to check.</param>
		/// <returns>
		/// Properties of this element equals properties of the specified element.
		/// </returns>
		public override bool EqualsContent(BetterListViewElementBase other) {
			if (this == other) {
				return true;
			}
			if (!(other is BetterListViewElement betterListViewElement) || !base.EqualsContent(other)) {
				return false;
			}
			string text = ((this.image != null) ? SerializationUtils.ImageToBase64String(this.image, ImageFormat.Png) : string.Empty);
			string value = ((betterListViewElement.image != null) ? SerializationUtils.ImageToBase64String(betterListViewElement.image, ImageFormat.Png) : string.Empty);
			if (text.Equals(value, StringComparison.Ordinal) && this.imageIndex == betterListViewElement.imageIndex) {
				return this.imageKey.Equals(betterListViewElement.imageKey, StringComparison.Ordinal);
			}
			return false;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElement" /> class.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected BetterListViewElement(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			this.image = SerializationUtils.ImageFromBase64String(info.GetString("image"));
			this.imageIndex = info.GetInt32("imageIndex");
			this.imageKey = info.GetString("imageKey");
		}

		/// <summary>
		///   Custom implementation of the GetObjectData method.
		/// </summary>
		/// <param name="info">serialization info</param>
		/// <param name="context">serialization context</param>
		protected override void GetObjectDataInternal(SerializationInfo info, StreamingContext context) {
			info.AddValue("image", SerializationUtils.ImageToBase64String(this.image, ImageFormat.Png));
			info.AddValue("imageIndex", this.imageIndex);
			info.AddValue("imageKey", this.imageKey);
			base.GetObjectDataInternal(info, context);
		}

		/// <summary>
		///   Custom implementation of the ReadXml method.
		/// </summary>
		/// <param name="reader">XML reader</param>
		protected override void ReadXmlInternal(XmlReader reader) {
			reader.ReadStartElement("BetterListViewElement");
			this.image = SerializationUtils.ImageFromBase64String(reader.ReadElementString("image"));
			this.imageIndex = int.Parse(reader.ReadElementString("imageIndex"), CultureInfo.InvariantCulture);
			this.imageKey = reader.ReadElementString("imageKey");
			base.ReadXmlInternal(reader);
			reader.ReadEndElement();
		}

		/// <summary>
		///   Custom implementation of the WriteXml method.
		/// </summary>
		/// <param name="writer">XML writer</param>
		protected override void WriteXmlInternal(XmlWriter writer) {
			writer.WriteStartElement("BetterListViewElement");
			writer.WriteElementString("image", SerializationUtils.ImageToBase64String(this.image, ImageFormat.Png));
			writer.WriteElementString("imageIndex", this.imageIndex.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("imageKey", this.imageKey);
			base.WriteXmlInternal(writer);
			writer.WriteEndElement();
		}
	}
}