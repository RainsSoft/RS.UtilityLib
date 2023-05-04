using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a BetterListView sub-item.
	/// </summary>
	[Serializable]
	[DefaultProperty("Text")]
	[TypeConverter(typeof(BetterListViewSubItemConverter))]
	[Designer(typeof(BetterListViewSubItemDesigner))]
	[ToolboxItem(false)]
	public class BetterListViewSubItem : BetterListViewElement
	{
		internal const bool DefaultFormatText = true;

		internal const bool DefaultHotkeyPrefix = false;

		internal const bool DefaultIsBold = false;

		internal const int DefaultMaximumTextLines = 0;

		private const BetterListViewTextWrapping DefaultTextWrapping = BetterListViewTextWrapping.Layout;

		private const string FieldAlignHorizontal = "alignHorizontal";

		private const string FieldAlignHorizontalImage = "alignHorizontalImage";

		private const string FieldAlignVertical = "alignVertical";

		private const string FieldAlignVerticalImage = "alignVerticalImage";

		private const string FieldBackColor = "backColor";

		private const string FieldFont = "font";

		private const string FieldForeColor = "foreColor";

		private const string FieldFormatText = "formatText";

		private const string FieldHotkeyPrefix = "hotkeyPrefix";

		private const string FieldIsBold = "isBold";

		private const string FieldKey = "key";

		private const string FieldText = "text";

		private const string FieldTextTrimming = "textTrimming";

		private const string FieldToolTips = "toolTips";

		private BetterListViewSubItemAccessibleObject cachedAccessibleObject;

		private BetterListViewCachedImage cachedImage;

		private string cachedDisplayText;

		private MultilineText cachedMultilineText;

		private TextAlignmentHorizontal alignHorizontal = TextAlignmentHorizontal.Default;

		private BetterListViewImageAlignmentHorizontal alignHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;

		private TextAlignmentVertical alignVertical = TextAlignmentVertical.Default;

		private BetterListViewImageAlignmentVertical alignVerticalImage = BetterListViewImageAlignmentVertical.Default;

		private Color backColor = Color.Empty;

		private Font font;

		private Color foreColor = Color.Empty;

		private bool formatText = true;

		private bool hotkeyPrefix;

		private bool isBold;

		private string text = string.Empty;

		private TextTrimming textTrimming = TextTrimming.Undefined;

		private BetterListViewToolTipInfoCollection toolTips = new BetterListViewToolTipInfoCollection();

		private Font cachedFont;

		private IComparable key;

		private BetterListViewItem cachedItem;

		/// <summary>
		///   horizontal alignment of the sub-item text
		/// </summary>
		[Category("Appearance")]
		[Description("Horizontal alignment of the sub-item text")]
		public TextAlignmentHorizontal AlignHorizontal {
			get {
				return this.alignHorizontal;
			}
			set {
				if (this.alignHorizontal != value) {
					this.alignHorizontal = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   horizontal alignment of the sub-item image
		/// </summary>
		[Description("Horizontal alignment of the sub-item image")]
		[Category("Appearance")]
		public BetterListViewImageAlignmentHorizontal AlignHorizontalImage {
			get {
				return this.alignHorizontalImage;
			}
			set {
				if (this.alignHorizontalImage != value) {
					this.alignHorizontalImage = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   vertical alignment of the sub-item text
		/// </summary>
		[Category("Appearance")]
		[Description("Vertical alignment of the sub-item text")]
		public TextAlignmentVertical AlignVertical {
			get {
				return this.alignVertical;
			}
			set {
				if (this.alignVertical != value) {
					this.alignVertical = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   vertical alignment of the sub-item image
		/// </summary>
		[Category("Appearance")]
		[Description("Vertical alignment of the sub-item image")]
		public BetterListViewImageAlignmentVertical AlignVerticalImage {
			get {
				return this.alignVerticalImage;
			}
			set {
				if (this.alignVerticalImage != value) {
					this.alignVerticalImage = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   background color of the sub-item
		/// </summary>
		[Description("Background color of the sub-item")]
		[Category("Appearance")]
		public Color BackColor {
			get {
				return this.backColor;
			}
			set {
				if (!(this.backColor == value)) {
					this.backColor = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
				}
			}
		}

		/// <summary>
		///   Text formatted for display.
		/// </summary>
		internal string DisplayText => this.text;

		/// <summary>
		///   font of the sub-item text
		/// </summary>
		[Category("Appearance")]
		[Description("Font of the sub-item text")]
		public override Font Font {
			get {
				if (this.cachedFont == null) {
					if (this.font != null) {
						this.cachedFont = this.font;
					}
					else if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
						BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
						if (betterListView.FontItems != null) {
							this.cachedFont = betterListView.FontItems;
						}
					}
					if (this.cachedFont == null) {
						this.cachedFont = Control.DefaultFont;
					}
				}
				return this.cachedFont;
			}
			set {
				Checks.CheckNotNull(value, "value");
				if (this.font != value) {
					this.font = value;
					this.FlushCachedFont();
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   foreground color the sub-item text
		/// </summary>
		[Category("Appearance")]
		[Description("Foreground color the sub-item text")]
		public override Color ForeColor {
			get {
				if (!this.foreColor.IsEmpty) {
					return this.foreColor;
				}
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
					if (!betterListView.ForeColorItems.IsEmpty) {
						return betterListView.ForeColorItems;
					}
				}
				return SystemColors.ControlText;
			}
			set {
				if (!(this.foreColor == value)) {
					this.foreColor = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
				}
			}
		}

		/// <summary>
		///   Pre-format sub-item text for custom display.
		/// </summary>
		internal bool FormatText => this.formatText;

		/// <summary>
		///   Allow displaying hotkey prefix in sub-item text (underline after '&amp;' character).
		/// </summary>
		[Description("Allow displaying hotkey prefix in sub-item text (underline after '&' character).")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool HotkeyPrefix {
			get {
				return this.hotkeyPrefix;
			}
			set {
				if (this.hotkeyPrefix != value) {
					this.hotkeyPrefix = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
				}
			}
		}

		/// <summary>
		///   draw the item bold
		/// </summary>
		[DefaultValue(false)]
		[DesignOnly(true)]
		[Category("Appearance")]
		[Description("Draw the item bold")]
		public bool IsBold {
			get {
				return this.isBold;
			}
			set {
				if (this.isBold != value) {
					this.isBold = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
				}
			}
		}

		/// <summary>
		///   Maximum allowed number of lines in sub-item label.
		/// </summary>
		internal int MaximumTextLines => 0;

		/// <summary>
		///   Label of the sub-item.
		/// </summary>
		[Description("Label of the sub-item")]
		[DefaultValue("")]
		[Category("Appearance")]
		public string Text {
			get {
				return this.text;
			}
			set {
				value = value ?? string.Empty;
				if (!this.text.Equals(value, StringComparison.Ordinal)) {
					this.text = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.DataBinding);
				}
			}
		}

		/// <summary>
		///   sub-item text trimming
		/// </summary>
		internal override TextTrimming TextTrimming {
			get {
				if (this.textTrimming != TextTrimming.Undefined) {
					return this.textTrimming;
				}
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					return ((BetterListView)this.OwnerCollection.OwnerControl).LayoutItemsCurrent.DefaultTextTrimming;
				}
				return TextTrimming.None;
			}
		}

		/// <summary>
		///   sub-item text wrapping behavior
		/// </summary>
		internal BetterListViewTextWrapping TextWrapping => BetterListViewTextWrapping.Layout;

		/// <summary>
		///   information about ToolTips shown on this sub-item
		/// </summary>
		internal BetterListViewToolTipInfoCollection ToolTips => this.toolTips;

		/// <summary>
		///   Gets sub-item boundaries.
		/// </summary>
		/// <returns><see cref="T:ComponentOwl.BetterListView.BetterListViewSubItemBounds" /> instance if the element is active, null otherwise.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewSubItemBounds Bounds => this.ListView?.GetSubItemBounds(this);

		/// <summary>
		///   Gets of sets visual order of the sub-item with respect to visible column headers.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DisplayIndex {
			get {
				BetterListView listView = this.ListView;
				if (listView == null) {
					return -1;
				}
				if (base.Index == 0) {
					return 0;
				}
				if (listView.View != BetterListViewView.Details) {
					return -1;
				}
				if (base.Index >= listView.Columns.Count) {
					return -1;
				}
				return listView.Columns[base.Index].DisplayIndex;
			}
		}

		/// <summary>
		///   the item containing this sub-item
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListViewItem Item {
			get {
				if (this.cachedItem == null && this.OwnerCollection != null) {
					this.cachedItem = ((BetterListViewSubItemCollection)this.OwnerCollection).OwnerItem;
				}
				return this.cachedItem;
			}
		}

		/// <summary>
		///   value used for sub-item comparison
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IComparable Key {
			get {
				return this.key;
			}
			set {
				if (this.key != value) {
					this.key = value;
					base.OnElementPropertyChanged(BetterListViewElementPropertyType.Sorting);
				}
			}
		}

		/// <summary>
		///   BetterListView instance in which this sub-item is contained
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public BetterListView ListView {
			get {
				if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
					return (BetterListView)this.OwnerCollection.OwnerControl;
				}
				return null;
			}
		}

		/// <summary>
		///   corresponding value specified by the ValueMember property of the column
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value {
			get {
				if (this.Item.OwnerCollection == null || ((BetterListViewItemCollection)this.Item.OwnerCollection).OwnerItem != null) {
					return null;
				}
				return this.ListView?.DataGetValue(this.Item.Index, base.Index);
			}
			set {
				BetterListView listView = this.ListView;
				if (listView == null) {
					throw new InvalidOperationException("Cannot set bound value. Sub-item is not contained in any control.");
				}
				this.text = listView.DataSetValue(this.Item.Index, base.Index, value);
				base.OnElementPropertyChanged(BetterListViewElementPropertyType.Sorting);
			}
		}

		/// <summary>
		///   collection that owns this element
		/// </summary>
		internal override BetterListViewElementCollectionBase OwnerCollection {
			get {
				return base.OwnerCollection;
			}
			set {
				base.OwnerCollection = value;
				this.FlushCachedFont();
				this.FlushCachedItem();
			}
		}

		/// <summary>
		///   Get accessible object instance for this sub-item.
		/// </summary>
		/// <param name="listView">Owner list control.</param>
		/// <param name="parent">Parent accessible object.</param>
		/// <returns>Accessible object instance.</returns>
		protected internal virtual AccessibleObject GetAccessibilityInstance(BetterListView listView, AccessibleObject parent) {
			Checks.CheckNotNull(listView, "listView");
			Checks.CheckNotNull(parent, "parent");
			if (this.cachedAccessibleObject == null) {
				this.cachedAccessibleObject = new BetterListViewSubItemAccessibleObject(this, listView, parent);
			}
			return this.cachedAccessibleObject;
		}

		internal void ClearCache() {
			this.ClearCache(clearTextOnly: false);
		}

		internal void ClearCache(bool clearTextOnly) {
			this.cachedAccessibleObject = null;
			if (!clearTextOnly && this.cachedImage != null) {
				this.cachedImage.Dispose();
				this.cachedImage = null;
			}
			this.cachedDisplayText = null;
			if (this.cachedMultilineText != null) {
				this.cachedMultilineText.Dispose();
				this.cachedMultilineText = null;
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItem instance.
		/// </summary>
		public BetterListViewSubItem()
			: base(null, null) {
			this.text = string.Empty;
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItem instance.
		/// </summary>
		/// <param name="image">item image</param>
		public BetterListViewSubItem(Image image)
			: base(image, null) {
			this.text = string.Empty;
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItem instance.
		/// </summary>
		/// <param name="text">item text</param>
		public BetterListViewSubItem(string text)
			: base(null, null) {
			this.text = text ?? string.Empty;
		}

		/// <summary>
		///   Initialize a new BetterListViewSubItem instance.
		/// </summary>
		/// <param name="image">item image</param>
		/// <param name="text">item text</param>
		public BetterListViewSubItem(Image image, string text)
			: base(image, null) {
			this.text = text ?? string.Empty;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSubItem" /> class.
		/// </summary>
		/// <param name="item">The item that owns this sub-item (the sub-item will be added into it).</param>
		/// <param name="text">Label of the sub-item.</param>
		/// <param name="foreColor">Foreground color of the sub-item text.</param>
		/// <param name="backColor">Background color of the sub-item.</param>
		/// <param name="font">Font of the sub-item text.</param>
		public BetterListViewSubItem(BetterListViewItem item, string text, Color foreColor, Color backColor, Font font)
			: base(null, null) {
			this.ForeColor = foreColor;
			this.BackColor = backColor;
			this.Font = font;
			this.text = text ?? string.Empty;
			item?.SubItems.Add(this);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSubItem" /> class.
		/// </summary>
		/// <param name="item">The item that owns this sub-item (the sub-item will be added into it).</param>
		/// <param name="text">Label of the sub-item.</param>
		public BetterListViewSubItem(BetterListViewItem item, string text)
			: base(null, null) {
			this.text = text ?? string.Empty;
			item?.SubItems.Add(this);
		}

		/// <summary>
		///   Edit label of this sub-item.
		/// </summary>
		public void BeginEdit() {
			this.ListView?.BeginEdit(this);
		}

		/// <summary>
		///   Get font for display and measurement with respect to IsBold property.
		/// </summary>
		/// <param name="font">output font</param>
		/// <returns>new Font instance has been created</returns>
		public bool GetDisplayFont(out Font font) {
			bool flag;
			if (this.Item.UseItemStyleForSubItems) {
				flag = this.Item.IsBold;
				font = this.Item.Font;
			}
			else {
				flag = this.IsBold;
				font = this.Font;
			}
			if (flag) {
				font = new Font(font.Name, font.Size, font.Style | FontStyle.Bold);
				return true;
			}
			return false;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		public override object Clone() {
			BetterListViewSubItem betterListViewSubItem = new BetterListViewSubItem();
			this.CopyTo(betterListViewSubItem);
			return betterListViewSubItem;
		}

		/// <summary>
		///   Remove element from its owner collection.
		/// </summary>
		/// <returns>success</returns>
		public override bool Remove() {
			if (this.OwnerCollection == null) {
				return false;
			}
			return ((BetterListViewSubItemCollection)this.OwnerCollection).Remove(this);
		}

		/// <summary>
		///   Set default style of this sub-item (font and colors).
		/// </summary>
		public void ResetStyle() {
			this.backColor = Color.Empty;
			this.font = null;
			this.foreColor = Color.Empty;
		}

		/// <summary>
		///   Check whether properties of this element equals properties of the specified element.
		/// </summary>
		/// <param name="other">Element to check.</param>
		/// <returns>
		///   Properties of this element equals properties of the specified element.
		/// </returns>
		public override bool EqualsContent(BetterListViewElementBase other) {
			if (this == other) {
				return true;
			}
			if (!(other is BetterListViewSubItem betterListViewSubItem) || !base.EqualsContent(other)) {
				return false;
			}
			if (this.alignHorizontal == betterListViewSubItem.alignHorizontal && this.alignHorizontalImage == betterListViewSubItem.alignHorizontalImage && this.alignVertical == betterListViewSubItem.alignVertical && this.alignVerticalImage == betterListViewSubItem.alignVerticalImage && this.backColor.Equals(betterListViewSubItem.backColor) && object.Equals(this.font, betterListViewSubItem.font) && this.foreColor.Equals(betterListViewSubItem.foreColor) && this.isBold == betterListViewSubItem.isBold && object.Equals(this.key, betterListViewSubItem.key) && this.text.Equals(betterListViewSubItem.text, StringComparison.Ordinal) && this.textTrimming == betterListViewSubItem.textTrimming) {
				return this.toolTips.EqualsContent(betterListViewSubItem.toolTips);
			}
			return false;
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			if (this.OwnerCollection == null) {
				return base.GetType().Name + ": {Text = '" + this.text + "'}";
			}
			return base.GetType().Name + ": {Index = '" + base.Index.ToString(CultureInfo.InvariantCulture) + "', Text = '" + this.text + "'}";
		}

		/// <summary>
		///   Copy content of this instance to the specified BetterListViewElementBase instance.
		/// </summary>
		/// <param name="element">BetterListViewElementBase to copy the content to</param>
		protected override void CopyTo(BetterListViewElementBase element) {
			BetterListViewSubItem betterListViewSubItem = (BetterListViewSubItem)element;
			betterListViewSubItem.AlignHorizontal = this.AlignHorizontal;
			betterListViewSubItem.alignHorizontalImage = this.AlignHorizontalImage;
			betterListViewSubItem.AlignVertical = this.AlignVertical;
			betterListViewSubItem.AlignVerticalImage = this.AlignVerticalImage;
			betterListViewSubItem.BackColor = this.BackColor;
			betterListViewSubItem.IsBold = this.IsBold;
			betterListViewSubItem.Key = this.Key;
			betterListViewSubItem.Text = this.Text;
			base.CopyTo(betterListViewSubItem);
		}

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
		public override int CompareTo(BetterListViewElementBase other) {
			if (other == null) {
				return 0;
			}
			if (!(other is BetterListViewSubItem betterListViewSubItem)) {
				return 0;
			}
			return string.Compare(this.Text, betterListViewSubItem.Text, StringComparison.CurrentCulture);
		}

		/// <summary>
		///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public override void Dispose() {
			this.ClearCache();
			base.Dispose();
		}

		internal void DrawingDrawImage(Graphics graphics, Image image, Rectangle bounds, byte opacity, bool enabled, ImageBorderType borderType, int borderThickness, Color borderColor, bool useCache) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckNotNull(image, "image");
			Checks.CheckSize(bounds.Size, "bounds.Size");
			Checks.CheckNotEqual(borderType, ImageBorderType.Undefined, "borderType", "ImageBorderType.Undefined");
			Checks.CheckBounds(borderThickness, 0, 16, "borderThickness");
			if (image.Size == bounds.Size && opacity == byte.MaxValue && borderType == ImageBorderType.None) {
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
			}
			else if (useCache) {
				Padding borderPadding = Painter.GetBorderPadding(borderType, borderThickness);
				if (this.cachedImage == null || this.cachedImage.ImageSize != bounds.Size || this.cachedImage.Opacity != opacity || this.cachedImage.BorderType != borderType || this.cachedImage.BorderThickness != borderThickness || this.cachedImage.Enabled != enabled) {
					if (this.cachedImage != null) {
						this.cachedImage.Dispose();
					}
					Bitmap image2 = new Bitmap(bounds.Width + borderPadding.Horizontal, bounds.Height + borderPadding.Vertical, PixelFormat.Format32bppPArgb);
					Rectangle bounds2 = new Rectangle(borderPadding.Left, borderPadding.Top, bounds.Width, bounds.Height);
					Graphics graphics2 = Graphics.FromImage(image2);
					graphics2.Clear(Color.Transparent);
					Painter.DrawBorder(graphics2, bounds2, borderType, borderThickness, borderColor);
					BetterListViewBasePainter.DrawImage(graphics2, image, bounds2, opacity, enabled);
					graphics2.Dispose();
					this.cachedImage = new BetterListViewCachedImage(image2, bounds.Size, opacity, borderType, borderThickness, enabled);
				}
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				Image image3 = this.cachedImage.Image;
				graphics.DrawImage(this.cachedImage.Image, new Rectangle(bounds.Left - borderPadding.Left, bounds.Top - borderPadding.Top, image3.Width, image3.Height), 0, 0, image3.Width, image3.Height, GraphicsUnit.Pixel);
			}
			else {
				Painter.DrawBorder(graphics, bounds, borderType, borderThickness);
				BetterListViewBasePainter.DrawImage(graphics, image, bounds, opacity, enabled);
			}
		}

		internal void DrawingDrawText(Graphics graphics, Color color, Rectangle bounds, int maximumTextLines) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "bounds.Width >= 0 && bounds.Height >= 0");
			Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
			this.UpdateCachedMultilineText(maximumTextLines);
			MultilineTextRenderer.DrawText(graphics, color, this.cachedMultilineText, bounds, this.AlignHorizontal, this.AlignVertical);
		}

		/// <summary>
		///   Report that property of this element has changed.
		/// </summary>
		/// <param name="elementPropertyType">type of the changed property</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		/// <param name="eventArgs">event data (in case of property change event should be fired)</param>
		internal override void OnElementPropertyChanged(BetterListViewElementPropertyType elementPropertyType, object oldValue, EventArgs eventArgs) {
			this.ClearCache();
			base.OnElementPropertyChanged(elementPropertyType, oldValue, eventArgs);
		}

		internal int LayoutGetLineCount(Graphics graphics, int width, int maximumTextLines, bool allowUseTextBreaks) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
			this.UpdateCachedMultilineText(maximumTextLines);
			int lineCount = MultilineTextRenderer.GetLineCount(graphics, width, this.cachedMultilineText);
			if (allowUseTextBreaks && this.cachedMultilineText.CachedTextBreaks == null && this.cachedMultilineText.MaximumTextLines > 1 && lineCount > 1) {
				MultilineTextRenderer.MeasureTextBreaks(graphics, this.cachedMultilineText);
			}
			return lineCount;
		}

		internal TextSize LayoutGetTextSize(Graphics graphics, int width, int maximumTextLines) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
			this.UpdateCachedMultilineText(maximumTextLines);
			return MultilineTextRenderer.MeasureText(graphics, width, this.cachedMultilineText);
		}

		internal bool LayoutCheckTextIntersection(Graphics graphics, int maximumTextLines, Rectangle bounds, Point point) {
			Checks.CheckNotNull(graphics, "graphics");
			Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
			this.UpdateCachedMultilineText(maximumTextLines);
			if (this.cachedMultilineText.CachedLineCount.LineCount == 1) {
				if (point.X >= bounds.Left) {
					return point.X < bounds.Right;
				}
				return false;
			}
			return MultilineTextRenderer.CheckIntersection(graphics, this.cachedMultilineText, bounds, point);
		}

		private void UpdateCachedMultilineText(int maximumTextLines) {
			if (this.cachedMultilineText == null || this.cachedMultilineText.MaximumTextLines != maximumTextLines) {
				if (this.cachedMultilineText != null) {
					this.cachedMultilineText.Dispose();
				}
				Font font;
				bool displayFont = this.GetDisplayFont(out font);
				TextOptions textOptions = TextOptions.None;
				if (this.TextWrapping != BetterListViewTextWrapping.None) {
					textOptions |= TextOptions.AllowWrap;
				}
				if (this.HotkeyPrefix) {
					textOptions |= TextOptions.HotkeyPrefix;
				}
				if (this.cachedDisplayText == null) {
					this.UpdateCachedDisplayText();
				}
				this.cachedMultilineText = new MultilineText(this.cachedDisplayText, displayFont ? font : ((Font)font.Clone()), this.TextTrimming, maximumTextLines, textOptions);
			}
		}

		private void UpdateCachedDisplayText() {
			this.cachedDisplayText = this.Text;
		}

		internal void FlushCachedFont() {
			this.cachedFont = null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetAlignHorizontal() {
			this.AlignHorizontal = TextAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetAlignHorizontalImage() {
			this.AlignHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetAlignVertical() {
			this.AlignVertical = TextAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetAlignVerticalImage() {
			this.AlignVerticalImage = BetterListViewImageAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetBackColor() {
			this.BackColor = Color.Empty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetFont() {
			this.font = null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal void ResetForeColor() {
			this.foreColor = Color.Empty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeAlignHorizontal() {
			return this.AlignHorizontal != TextAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeAlignHorizontalImage() {
			return this.AlignHorizontalImage != BetterListViewImageAlignmentHorizontal.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeAlignVertical() {
			return this.AlignVertical != TextAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeAlignVerticalImage() {
			return this.AlignVerticalImage != BetterListViewImageAlignmentVertical.Default;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeBackColor() {
			return !this.BackColor.IsEmpty;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeFont() {
			return this.font != null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		internal bool ShouldSerializeForeColor() {
			return !this.foreColor.IsEmpty;
		}

		private void FlushCachedItem() {
			this.cachedItem = null;
		}

		private BetterListViewSubItem(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			Checks.CheckNotNull(info, "info");
			Checks.CheckNotNull(context, "context");
			ColorConverter colorConverter = new ColorConverter();
			this.alignHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), info.GetString("alignHorizontal"));
			this.alignHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), info.GetString("alignHorizontalImage"));
			this.alignVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), info.GetString("alignVertical"));
			this.alignVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), info.GetString("alignVerticalImage"));
			this.backColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("backColor"));
			string @string = info.GetString("font");
			this.font = ((@string.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, @string)) : null);
			this.foreColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("foreColor"));
			this.formatText = info.GetBoolean("formatText");
			this.hotkeyPrefix = info.GetBoolean("hotkeyPrefix");
			this.isBold = info.GetBoolean("isBold");
			object obj = SerializationUtils.DeserializeBinary((byte[])info.GetValue("key", typeof(byte[])));
			this.key = ((obj != null) ? ((IComparable)obj) : null);
			this.text = info.GetString("text");
			this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), info.GetString("textTrimming"));
			this.toolTips = (BetterListViewToolTipInfoCollection)info.GetValue("toolTips", typeof(BetterListViewToolTipInfoCollection));
		}

		/// <summary>
		///   Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		///   The caller does not have the required permission.
		/// </exception>
		protected override void GetObjectDataInternal(SerializationInfo info, StreamingContext context) {
			ColorConverter colorConverter = new ColorConverter();
			info.AddValue("alignHorizontal", this.alignHorizontal.ToString());
			info.AddValue("alignHorizontalImage", this.alignHorizontalImage.ToString());
			info.AddValue("alignVertical", this.alignVertical.ToString());
			info.AddValue("alignVerticalImage", this.alignVerticalImage.ToString());
			info.AddValue("backColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.backColor));
			info.AddValue("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
			info.AddValue("foreColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
			info.AddValue("formatText", this.formatText.ToString());
			info.AddValue("hotkeyPrefix", this.hotkeyPrefix);
			info.AddValue("isBold", this.isBold);
			info.AddValue("key", SerializationUtils.SerializeBinary(this.key));
			info.AddValue("text", this.text);
			info.AddValue("textTrimming", this.textTrimming.ToString());
			info.AddValue("toolTips", this.toolTips, typeof(BetterListViewToolTipInfoCollection));
			base.GetObjectDataInternal(info, context);
		}

		/// <summary>
		///   Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
		protected override void ReadXmlInternal(XmlReader reader) {
			reader.MoveToContent();
			reader.ReadStartElement();
			ColorConverter colorConverter = new ColorConverter();
			this.alignHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), reader.ReadElementString("alignHorizontal"));
			this.alignHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), reader.ReadElementString("alignHorizontalImage"));
			this.alignVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), reader.ReadElementString("alignVertical"));
			this.alignVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), reader.ReadElementString("alignVerticalImage"));
			this.backColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("backColor"));
			string text = reader.ReadElementString("font");
			this.font = ((text.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, text)) : null);
			this.foreColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("foreColor"));
			this.formatText = bool.Parse(reader.ReadElementString("formatText"));
			this.hotkeyPrefix = bool.Parse(reader.ReadElementString("hotkeyPrefix"));
			this.isBold = bool.Parse(reader.ReadElementString("isBold"));
			this.key = SerializationUtils.DeserializeXml<IComparable>(reader, "key");
			this.text = reader.ReadElementString("text");
			this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), reader.ReadElementString("textTrimming"));
			reader.ReadStartElement("toolTips");
			this.toolTips = (BetterListViewToolTipInfoCollection)new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Deserialize(reader);
			reader.ReadEndElement();
			base.ReadXmlInternal(reader);
			reader.ReadEndElement();
		}

		/// <summary>
		///   Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
		protected override void WriteXmlInternal(XmlWriter writer) {
			ColorConverter colorConverter = new ColorConverter();
			writer.WriteElementString("alignHorizontal", this.alignHorizontal.ToString());
			writer.WriteElementString("alignHorizontalImage", this.alignHorizontalImage.ToString());
			writer.WriteElementString("alignVertical", this.alignVertical.ToString());
			writer.WriteElementString("alignVerticalImage", this.alignVerticalImage.ToString());
			writer.WriteElementString("backColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.backColor));
			writer.WriteElementString("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
			writer.WriteElementString("foreColor", colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
			writer.WriteElementString("formatText", this.formatText.ToString());
			writer.WriteElementString("hotkeyPrefix", this.hotkeyPrefix.ToString());
			writer.WriteElementString("isBold", this.isBold.ToString());
			SerializationUtils.SerializeXml(writer, "key", this.key);
			writer.WriteElementString("text", this.text);
			writer.WriteElementString("textTrimming", this.textTrimming.ToString());
			writer.WriteStartElement("toolTips");
			new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Serialize(writer, this.toolTips);
			writer.WriteEndElement();
			base.WriteXmlInternal(writer);
		}
	}
}