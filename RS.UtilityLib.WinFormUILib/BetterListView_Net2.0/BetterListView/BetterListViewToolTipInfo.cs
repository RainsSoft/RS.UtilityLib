using System;
using System.ComponentModel;
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
	///   Information about current location of the ToolTip.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(BetterListViewToolTipInfoConverter))]
	public struct BetterListViewToolTipInfo : ISerializable, IXmlSerializable
	{
		private const bool DefaultToolTipIsBalloon = false;

		private const bool DefaultToolTipOwnerDraw = false;

		private const bool DefaultToolTipStripAmpersands = false;

		private const ToolTipIcon DefaultToolTipIcon = ToolTipIcon.None;

		private const string FieldLocation = "location";

		private const string FieldBounds = "bounds";

		private const string FieldText = "text";

		private const string FieldShowOnPartialTextVisibility = "showOnPartialTextVisibility";

		private const string FieldToolTipBackColor = "toolTipBackColor";

		private const string FieldToolTipForeColor = "toolTipForeColor";

		private const string FieldToolTipIsBalloon = "toolTipIsBalloon";

		private const string FieldToolTipOwnerDraw = "toolTipOwnerDraw";

		private const string FieldToolTipStripAmpersands = "toolTipStripAmpersands";

		private const string FieldToolTipIcon = "toolTipIcon";

		private const string FieldToolTipTitle = "toolTipTitle";

		/// <summary>
		///   default background color of a ToolTip
		/// </summary>
		public static readonly Color DefaultToolTipBackColor = Color.White;

		/// <summary>
		///   default foreground color of a ToolTip
		/// </summary>
		public static readonly Color DefaultToolTipForeColor = Color.Black;

		/// <summary>
		///   represents an empty BetterListViewToolTipInfo structure
		/// </summary>
		public static readonly BetterListViewToolTipInfo Empty = new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Custom, Rectangle.Empty, string.Empty, showOnPartialTextVisibility: false, Color.Empty, Color.Empty, toolTipIsBalloon: false, toolTipOwnerDraw: false, toolTipStripAmpersands: false, ToolTipIcon.None, string.Empty);

		private BetterListViewToolTipLocation location;

		private Rectangle bounds;

		private string text;

		private bool showOnPartialTextVisibility;

		private Color toolTipBackColor;

		private Color toolTipForeColor;

		private bool toolTipIsBalloon;

		private bool toolTipOwnerDraw;

		private bool toolTipStripAmpersands;

		private ToolTipIcon toolTipIcon;

		private string toolTipTitle;

		/// <summary>
		///   location of the ToolTip
		/// </summary>
		public BetterListViewToolTipLocation Location {
			get {
				return this.location;
			}
			set {
				this.location = value;
			}
		}

		/// <summary>
		///   custom area for which a ToolTip is shown
		///   active only when Location is set to Custom
		/// </summary>
		public Rectangle Bounds {
			get {
				return this.bounds;
			}
			set {
				this.bounds = value;
			}
		}

		/// <summary>
		///   ToolTip text
		/// </summary>
		public string Text {
			get {
				return this.text;
			}
			set {
				this.text = value ?? string.Empty;
			}
		}

		/// <summary>
		///   show ToolTip only when item/sub-item text is partially visible
		/// </summary>
		public bool ShowOnPartialTextVisibility {
			get {
				return this.showOnPartialTextVisibility;
			}
			set {
				this.showOnPartialTextVisibility = value;
			}
		}

		/// <summary>
		///   background color of the corresponding ToolTip
		/// </summary>
		public Color ToolTipBackColor {
			get {
				return this.toolTipBackColor;
			}
			set {
				this.toolTipBackColor = value;
			}
		}

		/// <summary>
		///   foreground color of the corresponding ToolTip
		/// </summary>
		public Color ToolTipForeColor {
			get {
				return this.toolTipForeColor;
			}
			set {
				this.toolTipForeColor = value;
			}
		}

		/// <summary>
		///   the corresponding ToolTip should use a balloon window
		/// </summary>
		public bool ToolTipIsBalloon {
			get {
				return this.toolTipIsBalloon;
			}
			set {
				this.toolTipIsBalloon = value;
			}
		}

		/// <summary>
		///   specifies wheter the corresponding ToolTip is drawn by the operating system or by the user
		/// </summary>
		public bool ToolTipOwnerDraw {
			get {
				return this.toolTipOwnerDraw;
			}
			set {
				this.toolTipOwnerDraw = value;
			}
		}

		/// <summary>
		///   strip ampersand (&amp;) characters
		/// </summary>
		public bool ToolTipStripAmpersands {
			get {
				return this.toolTipStripAmpersands;
			}
			set {
				this.toolTipStripAmpersands = value;
			}
		}

		/// <summary>
		///   type of ToolTip icon to be displayed alongside text of the corresponding ToolTip
		/// </summary>
		public ToolTipIcon ToolTipIcon {
			get {
				return this.toolTipIcon;
			}
			set {
				this.toolTipIcon = value;
			}
		}

		/// <summary>
		///   title for window of the corresponding ToolTip
		/// </summary>
		public string ToolTipTitle {
			get {
				return this.toolTipTitle;
			}
			set {
				this.toolTipTitle = value ?? string.Empty;
			}
		}

		/// <summary>
		///   this BetterListViewToolTipInfo instance is empty
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty => this.Equals(BetterListViewToolTipInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="location">location of the ToolTip</param>
		/// <param name="text">ToolTip text</param>
		public BetterListViewToolTipInfo(BetterListViewToolTipLocation location, string text)
			: this(location, text, showOnPartialTextVisibility: false) {
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="location">location of the ToolTip</param>
		/// <param name="text">ToolTip text</param>
		/// <param name="showOnPartialTextVisibility">show ToolTip only when item/sub-item text is partially visible</param>
		public BetterListViewToolTipInfo(BetterListViewToolTipLocation location, string text, bool showOnPartialTextVisibility)
			: this(location, Rectangle.Empty, text, showOnPartialTextVisibility, BetterListViewToolTipInfo.DefaultToolTipBackColor, BetterListViewToolTipInfo.DefaultToolTipForeColor, toolTipIsBalloon: false, toolTipOwnerDraw: false, toolTipStripAmpersands: false, ToolTipIcon.None, string.Empty) {
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="bounds">custom area for which a ToolTip is shown</param>
		/// <param name="text">ToolTip text</param>
		public BetterListViewToolTipInfo(Rectangle bounds, string text)
			: this(BetterListViewToolTipLocation.Custom, bounds, text, showOnPartialTextVisibility: false, BetterListViewToolTipInfo.DefaultToolTipBackColor, BetterListViewToolTipInfo.DefaultToolTipForeColor, toolTipIsBalloon: false, toolTipOwnerDraw: false, toolTipStripAmpersands: false, ToolTipIcon.None, string.Empty) {
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="location">location of the ToolTip</param>
		/// <param name="text">ToolTip text</param>
		/// <param name="showOnPartialTextVisibility">show ToolTip only when item/sub-item text is partially visible</param>
		/// <param name="toolTipBackColor">background color of the corresponding ToolTip</param>
		/// <param name="toolTipForeColor">foreground color of the corresponding ToolTip</param>
		/// <param name="toolTipIsBalloon">the corresponding ToolTip should use a balloon window</param>
		/// <param name="toolTipOwnerDraw">specifies wheter the corresponding ToolTip is drawn by the operating system or by the user</param>
		/// <param name="toolTipStripAmpersands">strip ampersand (&amp;) characters</param>
		/// <param name="toolTipIcon">type of ToolTip icon to be displayed alongside text of the corresponding ToolTip </param>
		/// <param name="toolTipTitle">title for window of the corresponding ToolTip</param>
		public BetterListViewToolTipInfo(BetterListViewToolTipLocation location, string text, bool showOnPartialTextVisibility, Color toolTipBackColor, Color toolTipForeColor, bool toolTipIsBalloon, bool toolTipOwnerDraw, bool toolTipStripAmpersands, ToolTipIcon toolTipIcon, string toolTipTitle)
			: this(location, Rectangle.Empty, text, showOnPartialTextVisibility, toolTipBackColor, toolTipForeColor, toolTipIsBalloon, toolTipOwnerDraw, toolTipStripAmpersands, toolTipIcon, toolTipTitle) {
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="bounds">custom area for which a ToolTip is shown</param>
		/// <param name="text">ToolTip text</param>
		/// <param name="toolTipBackColor">background color of the corresponding ToolTip</param>
		/// <param name="toolTipForeColor">foreground color of the corresponding ToolTip</param>
		/// <param name="toolTipIsBalloon">the corresponding ToolTip should use a balloon window</param>
		/// <param name="toolTipOwnerDraw">specifies wheter the corresponding ToolTip is drawn by the operating system or by the user</param>
		/// <param name="toolTipStripAmpersands">strip ampersand (&amp;) characters</param>
		/// <param name="toolTipIcon">type of ToolTip icon to be displayed alongside text of the corresponding ToolTip </param>
		/// <param name="toolTipTitle">title for window of the corresponding ToolTip</param>
		public BetterListViewToolTipInfo(Rectangle bounds, string text, Color toolTipBackColor, Color toolTipForeColor, bool toolTipIsBalloon, bool toolTipOwnerDraw, bool toolTipStripAmpersands, ToolTipIcon toolTipIcon, string toolTipTitle)
			: this(BetterListViewToolTipLocation.Custom, bounds, text, showOnPartialTextVisibility: false, toolTipBackColor, toolTipForeColor, toolTipIsBalloon, toolTipOwnerDraw, toolTipStripAmpersands, toolTipIcon, toolTipTitle) {
		}

		/// <summary>
		///   Initialize a new BetterListViewToolTipInfo instance.
		/// </summary>
		/// <param name="location">location of the ToolTip</param>
		/// <param name="bounds">custom area for which a ToolTip is shown</param>
		/// <param name="text">ToolTip text</param>
		/// <param name="showOnPartialTextVisibility">show ToolTip only when item/sub-item text is partially visible</param>
		/// <param name="toolTipBackColor">background color of the corresponding ToolTip</param>
		/// <param name="toolTipForeColor">foreground color of the corresponding ToolTip</param>
		/// <param name="toolTipIsBalloon">the corresponding ToolTip should use a balloon window</param>
		/// <param name="toolTipOwnerDraw">specifies wheter the corresponding ToolTip is drawn by the operating system or by the user</param>
		/// <param name="toolTipStripAmpersands">strip ampersand (&amp;) characters</param>
		/// <param name="toolTipIcon">type of ToolTip icon to be displayed alongside text of the corresponding ToolTip </param>
		/// <param name="toolTipTitle">title for window of the corresponding ToolTip</param>
		public BetterListViewToolTipInfo(BetterListViewToolTipLocation location, Rectangle bounds, string text, bool showOnPartialTextVisibility, Color toolTipBackColor, Color toolTipForeColor, bool toolTipIsBalloon, bool toolTipOwnerDraw, bool toolTipStripAmpersands, ToolTipIcon toolTipIcon, string toolTipTitle) {
			this.location = location;
			this.bounds = bounds;
			this.text = text ?? string.Empty;
			this.showOnPartialTextVisibility = showOnPartialTextVisibility;
			this.toolTipBackColor = toolTipBackColor;
			this.toolTipForeColor = toolTipForeColor;
			this.toolTipIsBalloon = toolTipIsBalloon;
			this.toolTipOwnerDraw = toolTipOwnerDraw;
			this.toolTipStripAmpersands = toolTipStripAmpersands;
			this.toolTipIcon = toolTipIcon;
			this.toolTipTitle = toolTipTitle ?? string.Empty;
		}

		/// <summary>
		///   Performs an explicit conversion from <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipInfo" /> to <see cref="T:System.String" />.
		/// </summary>
		/// <param name="toolTipInfo"><see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipInfo" /> instance</param>
		/// <returns>
		///   Tooltip text
		/// </returns>
		public static explicit operator string(BetterListViewToolTipInfo toolTipInfo) {
			return toolTipInfo.Text;
		}

		/// <summary>
		///   Performs an implicit conversion from <see cref="T:System.String" /> to <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipInfo" />.
		/// </summary>
		/// <param name="text">Tooltip text</param>
		/// <returns>
		///   <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipInfo" /> instance
		/// </returns>
		public static implicit operator BetterListViewToolTipInfo(string text) {
			return new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Client, text);
		}

		/// <summary>
		///   Test wheter the two BetterListViewToolTipInfo objects are identical.
		/// </summary>
		/// <param name="toolTipInfoA">first BetterListViewToolTipInfo object</param>
		/// <param name="toolTipInfoB">second BetterListViewToolTipInfo object</param>
		/// <returns>
		///   the two BetterListViewToolTipInfo objects are identical
		/// </returns>
		public static bool operator ==(BetterListViewToolTipInfo toolTipInfoA, BetterListViewToolTipInfo toolTipInfoB) {
			return toolTipInfoA.Equals(toolTipInfoB);
		}

		/// <summary>
		///   Test wheter the two BetterListViewToolTipInfo objects are different.
		/// </summary>
		/// <param name="toolTipInfoA">first BetterListViewToolTipInfo object</param>
		/// <param name="toolTipInfoB">second BetterListViewToolTipInfo object</param>
		/// <returns>
		///   the two BetterListViewToolTipInfo objects are different
		/// </returns>
		public static bool operator !=(BetterListViewToolTipInfo toolTipInfoA, BetterListViewToolTipInfo toolTipInfoB) {
			return !toolTipInfoA.Equals(toolTipInfoB);
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
			if (!(obj is BetterListViewToolTipInfo betterListViewToolTipInfo)) {
				return false;
			}
			if (this.location == betterListViewToolTipInfo.location && this.bounds == betterListViewToolTipInfo.bounds && string.Equals(this.text ?? string.Empty, betterListViewToolTipInfo.text ?? string.Empty, StringComparison.Ordinal) && this.showOnPartialTextVisibility == betterListViewToolTipInfo.showOnPartialTextVisibility && this.toolTipBackColor == betterListViewToolTipInfo.toolTipBackColor && this.toolTipForeColor == betterListViewToolTipInfo.toolTipForeColor && this.toolTipIsBalloon == betterListViewToolTipInfo.toolTipIsBalloon && this.toolTipOwnerDraw == betterListViewToolTipInfo.toolTipOwnerDraw && this.toolTipStripAmpersands == betterListViewToolTipInfo.toolTipStripAmpersands && this.toolTipIcon == betterListViewToolTipInfo.toolTipIcon) {
				return string.Equals(this.toolTipTitle ?? string.Empty, betterListViewToolTipInfo.toolTipTitle ?? string.Empty, StringComparison.Ordinal);
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
			return this.location.GetHashCode() ^ this.bounds.GetHashCode() ^ ((!string.IsNullOrEmpty(this.text)) ? this.text.GetHashCode() : 0) ^ this.showOnPartialTextVisibility.GetHashCode() ^ this.toolTipBackColor.GetHashCode() ^ this.toolTipForeColor.GetHashCode() ^ this.toolTipIsBalloon.GetHashCode() ^ this.toolTipOwnerDraw.GetHashCode() ^ this.toolTipStripAmpersands.GetHashCode() ^ this.toolTipIcon.GetHashCode() ^ ((!string.IsNullOrEmpty(this.toolTipTitle)) ? this.toolTipTitle.GetHashCode() : 0);
		}

		/// <summary>
		///   Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			return this.text;
		}

		private BetterListViewToolTipInfo(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			ColorConverter colorConverter = new ColorConverter();
			this.location = (BetterListViewToolTipLocation)info.GetInt32("location");
			this.bounds = (Rectangle)new RectangleConverter().ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("bounds"));
			this.text = info.GetString("text");
			this.showOnPartialTextVisibility = info.GetBoolean("showOnPartialTextVisibility");
			this.toolTipBackColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("toolTipBackColor"));
			this.toolTipForeColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("toolTipForeColor"));
			this.toolTipIsBalloon = info.GetBoolean("toolTipIsBalloon");
			this.toolTipOwnerDraw = info.GetBoolean("toolTipOwnerDraw");
			this.toolTipStripAmpersands = info.GetBoolean("toolTipStripAmpersands");
			this.toolTipIcon = (ToolTipIcon)Enum.Parse(typeof(ToolTipIcon), info.GetString("toolTipIcon"));
			this.toolTipTitle = info.GetString("toolTipTitle");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			ColorConverter colorConverter = new ColorConverter();
			info.AddValue("location", (int)this.location);
			info.AddValue("bounds", new RectangleConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.bounds));
			info.AddValue("text", this.text);
			info.AddValue("showOnPartialTextVisibility", this.showOnPartialTextVisibility);
			info.AddValue("toolTipBackColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.toolTipBackColor));
			info.AddValue("toolTipForeColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.toolTipForeColor));
			info.AddValue("toolTipIsBalloon", this.toolTipIsBalloon);
			info.AddValue("toolTipOwnerDraw", this.toolTipOwnerDraw);
			info.AddValue("toolTipStripAmpersands", this.toolTipStripAmpersands);
			info.AddValue("toolTipIcon", this.toolTipIcon.ToString());
			info.AddValue("toolTipTitle", this.toolTipTitle);
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			Checks.CheckNotNull(reader, "reader");
			reader.MoveToContent();
			reader.ReadStartElement();
			ColorConverter colorConverter = new ColorConverter();
			this.location = (BetterListViewToolTipLocation)int.Parse(reader.ReadElementString("location"), CultureInfo.InvariantCulture);
			this.bounds = (Rectangle)new RectangleConverter().ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("bounds"));
			if (reader.Name.Equals("text", StringComparison.Ordinal) && !reader.IsEmptyElement) {
				this.text = reader.ReadElementString("text");
			}
			else {
				this.text = null;
				reader.Read();
			}
			this.showOnPartialTextVisibility = bool.Parse(reader.ReadElementString("showOnPartialTextVisibility"));
			this.toolTipBackColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("toolTipBackColor"));
			this.toolTipForeColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("toolTipForeColor"));
			this.toolTipIsBalloon = bool.Parse(reader.ReadElementString("toolTipIsBalloon"));
			this.toolTipOwnerDraw = bool.Parse(reader.ReadElementString("toolTipOwnerDraw"));
			this.toolTipStripAmpersands = bool.Parse(reader.ReadElementString("toolTipStripAmpersands"));
			this.toolTipIcon = (ToolTipIcon)Enum.Parse(typeof(ToolTipIcon), reader.ReadElementString("toolTipIcon"));
			if (reader.Name.Equals("toolTipTitle", StringComparison.Ordinal) && !reader.IsEmptyElement) {
				this.toolTipTitle = reader.ReadElementString("toolTipTitle");
			}
			else {
				this.toolTipTitle = null;
				reader.Read();
			}
			reader.ReadEndElement();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			Checks.CheckNotNull(writer, "writer");
			ColorConverter colorConverter = new ColorConverter();
			writer.WriteElementString("location", Convert.ToString((int)this.location));
			writer.WriteElementString("bounds", new RectangleConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.bounds));
			writer.WriteElementString("text", this.text);
			writer.WriteElementString("showOnPartialTextVisibility", this.showOnPartialTextVisibility.ToString());
			writer.WriteElementString("toolTipBackColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.toolTipBackColor));
			writer.WriteElementString("toolTipForeColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.toolTipForeColor));
			writer.WriteElementString("toolTipIsBalloon", this.toolTipIsBalloon.ToString());
			writer.WriteElementString("toolTipOwnerDraw", this.toolTipOwnerDraw.ToString());
			writer.WriteElementString("toolTipStripAmpersands", this.toolTipStripAmpersands.ToString());
			writer.WriteElementString("toolTipIcon", this.toolTipIcon.ToString());
			writer.WriteElementString("toolTipTitle", this.toolTipTitle);
		}
	}

}