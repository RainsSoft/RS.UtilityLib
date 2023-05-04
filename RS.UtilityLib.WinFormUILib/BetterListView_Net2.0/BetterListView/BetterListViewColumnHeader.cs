using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Represents a BetterListView column header.
    /// </summary>
    [Serializable]
    [ToolboxItem(false)]
    [TypeConverter(typeof(BetterListViewColumnHeaderConverter))]
    [Designer(typeof(BetterListViewElementDesigner))]
    [DefaultProperty("Text")]
    public class BetterListViewColumnHeader : BetterListViewElement, IBetterListViewLayoutElementDisplayable, IBetterListViewStateElement
    {
        /// <summary>
        ///   default method of column sorting
        /// </summary>
        public const BetterListViewSortMethod DefaultSortMethod = BetterListViewSortMethod.Auto;

        /// <summary>
        ///   default order of column sorting
        /// </summary>
        public const BetterListViewSortOrder DefaultSortOrder = BetterListViewSortOrder.None;

        private const TextTrimming DefaultTextTrimming = TextTrimming.EllipsisCharacter;

        private const int DefaultWidth = 128;

        private const bool DefaultPreferredSortOrderAscending = true;

        private const bool DefaultAllowResize = true;

        private const int DefaultMaximumWidth = 8192;

        private const int DefaultMinimumWidth = 0;

        private const bool DefaultSmoothResize = true;

        private const string FieldAlignHorizontal = "alignHorizontal";

        private const string FieldAlignHorizontalImage = "alignHorizontalImage";

        private const string FieldAlignVertical = "alignVertical";

        private const string FieldAlignVerticalImage = "alignVerticalImage";

        private const string FieldFont = "font";

        private const string FieldForeColor = "foreColor";

        private const string FieldMinimumWidth = "minimumWidth";

        private const string FieldPreferredSortOrderAscending = "preferredSortOrderAscending";

        private const string FieldSmoothResize = "smoothResize";

        private const string FieldSortMethod = "sortMethod";

        private const string FieldStyle = "style";

        private const string FieldText = "text";

        private const string FieldTextTrimming = "textTrimming";

        private const string FieldToolTips = "toolTips";

        private const string FieldWidth = "width";

        private BetterListViewColumnHeaderAccessibleObject cachedAccessibleObject;

        private MultilineText cachedMultilineText;

        private BetterListViewColumnHeaderBounds layoutBounds;

        private int layoutIndexDisplay = -1;

        private TextAlignmentHorizontal alignHorizontal = TextAlignmentHorizontal.Default;

        private BetterListViewImageAlignmentHorizontal alignHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;

        private TextAlignmentVertical alignVertical = TextAlignmentVertical.Default;

        private BetterListViewImageAlignmentVertical alignVerticalImage = BetterListViewImageAlignmentVertical.Default;

        private Font font;

        private Color foreColor = Color.Empty;

        private string text = string.Empty;

        private TextTrimming textTrimming = TextTrimming.EllipsisCharacter;

        private BetterListViewToolTipInfoCollection toolTips = new BetterListViewToolTipInfoCollection();

        private int width;

        private bool allowResize = true;

        private int maximumWidth = 8192;

        private int minimumWidth;

        private bool preferredSortOrderAscending = true;

        private BetterListViewSortMethod sortMethod;

        private BetterListViewColumnHeaderStyle style = BetterListViewColumnHeaderStyle.Default;

        private BindingMemberInfo displayMember = new BindingMemberInfo(string.Empty);

        private BindingMemberInfo valueMember = new BindingMemberInfo(string.Empty);

        private bool smoothResize = true;

        private readonly BetterListViewElementStateBox elementStateBox = new BetterListViewElementStateBox();

        BetterListViewElementBoundsBase IBetterListViewLayoutElementDisplayable.LayoutBounds {
            get {
                return this.layoutBounds;
            }
            set {
                Checks.CheckType(value, typeof(BetterListViewColumnHeaderBounds), "value", allowNull: true);
                this.layoutBounds = (BetterListViewColumnHeaderBounds)value;
            }
        }

        int IBetterListViewLayoutElementDisplayable.LayoutIndexDisplay {
            get {
                return this.layoutIndexDisplay;
            }
            set {
                Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
                this.layoutIndexDisplay = value;
            }
        }

        /// <summary>
        ///   horizontal alignment of the column header text
        /// </summary>
        [Category("Appearance")]
        [Description("Horizontal alignment of the column header text")]
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
        ///   horizontal alignment of the column header image
        /// </summary>
        [Description("Horizontal alignment of the column header image")]
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
        ///   vertical alignment of the column header text
        /// </summary>
        [Description("Horizontal alignment of the column header text")]
        [Category("Appearance")]
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
        ///   vertical alignment of the column header image
        /// </summary>
        [Category("Appearance")]
        [Description("Horizontal alignment of the column header image")]
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
        ///   font of the column header text
        /// </summary>
        [Description("Font of the column header text")]
        [Category("Appearance")]
        public override Font Font {
            get {
                if (this.font != null) {
                    return this.font;
                }
                if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                    BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
                    if (betterListView.FontColumns != null) {
                        return betterListView.FontColumns;
                    }
                }
                return Control.DefaultFont;
            }
            set {
                Checks.CheckNotNull(value, "value");
                if (this.font != value) {
                    this.font = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
                }
            }
        }

        /// <summary>
        ///   foreground color the column header text
        /// </summary>
        [Category("Appearance")]
        [Description("Foreground color the column header text")]
        public override Color ForeColor {
            get {
                if (!this.foreColor.IsEmpty) {
                    return this.foreColor;
                }
                if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                    BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
                    if (!betterListView.ForeColorColumns.IsEmpty) {
                        return betterListView.ForeColorColumns;
                    }
                }
                return SystemColors.ControlText;
            }
            set {
                Checks.CheckFalse(value.IsEmpty, "value.IsEmpty");
                if (!(this.foreColor == value)) {
                    this.foreColor = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
                }
            }
        }

        /// <summary>
        ///   the column header is visible
        /// </summary>
        internal bool Visible => true;

        /// <summary>
        ///   text of the column header
        /// </summary>
        [Description("Text of the column header")]
        [Category("Appearance")]
        [DefaultValue("")]
        public string Text {
            get {
                return this.text;
            }
            set {
                value = value ?? string.Empty;
                if (!this.text.Equals(value, StringComparison.Ordinal)) {
                    this.text = value;
                    this.ClearCache();
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Sorting);
                }
            }
        }

        /// <summary>
        ///   column header text trimming
        /// </summary>
        internal override TextTrimming TextTrimming => this.textTrimming;

        /// <summary>
        ///   information about ToolTips shown on this column header
        /// </summary>
        internal BetterListViewToolTipInfoCollection ToolTips => this.toolTips;

        /// <summary>
        ///   column header width in pixels
        /// </summary>
        [DefaultValue(128)]
        [Category("Appearance")]
        [Description("Column header width in pixels")]
        public int Width {
            get {
                return this.width;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                this.SetWidth(value, BetterListViewColumnWidthChangeSource.ResizeUserCode, raiseEvent: true);
            }
        }

        /// <summary>
        ///   allow column to be resized
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Allow column to be resized")]
        public bool AllowResize {
            get {
                return this.allowResize;
            }
            set {
                this.allowResize = value;
            }
        }

        /// <summary>
        ///   maximum allowed width of the column header
        /// </summary>
        [DefaultValue(8192)]
        [Category("Behavior")]
        [Description("Maximum allowed width of the column header")]
        public int MaximumWidth {
            get {
                return this.maximumWidth;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                if (this.maximumWidth != value) {
                    this.maximumWidth = value;
                    this.minimumWidth = Math.Min(this.minimumWidth, value);
                    this.SetWidth(this.Width, BetterListViewColumnWidthChangeSource.ResizeUserCode, raiseEvent: true);
                }
            }
        }

        /// <summary>
        ///   minimum allowed width of the column header
        /// </summary>
        [Description("Minimum allowed width of the column header")]
        [DefaultValue(0)]
        [Category("Behavior")]
        public int MinimumWidth {
            get {
                return this.minimumWidth;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                if (this.minimumWidth != value) {
                    this.minimumWidth = value;
                    this.maximumWidth = Math.Max(this.maximumWidth, value);
                    this.SetWidth(this.Width, BetterListViewColumnWidthChangeSource.ResizeUserCode, raiseEvent: true);
                }
            }
        }

        /// <summary>
        ///   preferred sort order for this column is ascending order
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("preferred sort order for this column is ascending order")]
        public bool PreferredSortOrderAscending {
            get {
                return this.preferredSortOrderAscending;
            }
            set {
                this.preferredSortOrderAscending = value;
            }
        }

        /// <summary>
        ///   column sorting method
        /// </summary>
        [Category("Behavior")]
        [Description("Column sorting method")]
        public BetterListViewSortMethod SortMethod {
            get {
                return this.sortMethod;
            }
            set {
                if (this.sortMethod != value) {
                    this.sortMethod = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Sorting);
                }
            }
        }

        /// <summary>
        ///   Column header appearance and behavior.
        /// </summary>
        [Category("Behavior")]
        [Description("Column header appearance and behavior")]
        public BetterListViewColumnHeaderStyle Style {
            get {
                return this.style;
            }
            set {
                BetterListViewColumnHeaderStyle betterListViewColumnHeaderStyle = this.style;
                if (betterListViewColumnHeaderStyle != value) {
                    this.style = value;
                    if (betterListViewColumnHeaderStyle == BetterListViewColumnHeaderStyle.None || value == BetterListViewColumnHeaderStyle.None) {
                        base.OnElementPropertyChanged(BetterListViewElementPropertyType.LayoutSetup);
                    }
                    else {
                        base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets column header boundaries.
        /// </summary>
        /// <returns><see cref="T:ComponentOwl.BetterListView.BetterListViewColumnHeaderBounds" /> instance if the element is active, null otherwise.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewColumnHeaderBounds Bounds => this.ListView?.GetColumnHeaderBounds(this);

        /// <summary>
        ///   Gets of sets visual order of the column header with respect to other visible column headers.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int DisplayIndex {
            get {
                BetterListView listView = this.ListView;
                if (listView == null || !((IBetterListViewStateElement)this).IsActive) {
                    return -1;
                }
                return ((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay;
            }
            set {
                BetterListView listView = this.ListView;
                if (listView == null) {
                    throw new InvalidOperationException("Cannot set display index of a column that is not part of some ListView.");
                }
                listView.ReorderColumns(((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay, value, BetterListViewColumnReorderOptions.ReorderItems | BetterListViewColumnReorderOptions.VisibleIndices);
            }
        }

        /// <summary>
        ///   Gets the ImageList associated with this column header.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ImageList ImageList => this.ListView?.ImageListColumns;

        /// <summary>
        ///   BetterListView instance in which this column header is contained
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListView ListView {
            get {
                if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                    return (BetterListView)this.OwnerCollection.OwnerControl;
                }
                return null;
            }
        }

        /// <summary>
        ///   next column header visible in the control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewColumnHeader NextVisibleColumnHeader {
            get {
                BetterListView listView = this.ListView;
                if (listView == null) {
                    return null;
                }
                ReadOnlyCollection<BetterListViewColumnHeader> layoutElementsColumns = listView.LayoutElementsColumns;
                int num = ((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay;
                if (num >= layoutElementsColumns.Count - 1) {
                    return null;
                }
                return layoutElementsColumns[num + 1];
            }
        }

        /// <summary>
        ///   previous column header visible in the control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewColumnHeader PreviousVisibleColumnHeader {
            get {
                BetterListView listView = this.ListView;
                if (listView == null) {
                    return null;
                }
                ReadOnlyCollection<BetterListViewColumnHeader> layoutElementsColumns = listView.LayoutElementsColumns;
                int num = ((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay;
                if (num <= 0) {
                    return null;
                }
                return layoutElementsColumns[num - 1];
            }
        }

        /// <summary>
        ///   column sorting order
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewSortOrder SortOrder {
            get {
                if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                    return ((BetterListView)this.OwnerCollection.OwnerControl).SortList.GetSortOrder(base.Index);
                }
                return BetterListViewSortOrder.None;
            }
        }

        /// <summary>
        ///   property to display on list items in the specific column
        /// </summary>
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Category("Data")]
        [DefaultValue("")]
        [Description("Property to display on list items in the specific column")]
        public string DisplayMember {
            get {
                return this.displayMember.BindingMember ?? string.Empty;
            }
            set {
                this.displayMember = new BindingMemberInfo(value);
                if (this.ListView != null) {
                    this.ListView.DataSetDataConnection();
                }
            }
        }

        /// <summary>
        ///   property to use for a corresponding sub-item value
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Property to use for a corresponding sub-item value")]
        public string ValueMember {
            get {
                return this.valueMember.BindingMember ?? string.Empty;
            }
            set {
                this.valueMember = new BindingMemberInfo(value ?? string.Empty);
                if (this.ListView != null) {
                    this.ListView.DataSetDataConnection();
                }
            }
        }

        /// <summary>
        ///   property to display on list items in the specific column
        /// </summary>
        internal BindingMemberInfo DisplayMemberInternal => this.displayMember;

        /// <summary>
        ///   Gets or sets a value indicating whether to dynamically resize items when resizing the column.
        /// </summary>
        [Category("Performance")]
        [Description("Dynamically resize items when resizing the column")]
        [DefaultValue(true)]
        public bool SmoothResize {
            get {
                return this.smoothResize;
            }
            set {
                this.smoothResize = value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this element is in active state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this element is in active state; otherwise, <c>false</c>.
        /// </value>
        bool IBetterListViewStateElement.IsActive {
            get {
                BetterListViewElementState state = this.elementStateBox.State;
                if (state != BetterListViewElementState.Active && state != BetterListViewElementState.ActiveCoarse && state != BetterListViewElementState.ActiveFine) {
                    return state == BetterListViewElementState.ActiveVisible;
                }
                return true;
            }
        }

        /// <summary>
        ///   Current state of the element.
        /// </summary>
        BetterListViewElementState IBetterListViewStateElement.State => this.elementStateBox.State;

        /// <summary>
        ///   Get accessible object instance for this column header.
        /// </summary>
        /// <param name="listView">Owner list control.</param>
        /// <param name="parent">Parent accessible object.</param>
        /// <returns>Accessible object instance.</returns>
        protected internal virtual AccessibleObject GetAccessibilityInstance(BetterListView listView, AccessibleObject parent) {
            Checks.CheckNotNull(listView, "listView");
            Checks.CheckNotNull(parent, "parent");
            if (this.cachedAccessibleObject == null) {
                this.cachedAccessibleObject = new BetterListViewColumnHeaderAccessibleObject(this, listView, parent);
            }
            return this.cachedAccessibleObject;
        }

        internal void ClearCache() {
            this.cachedAccessibleObject = null;
            if (this.cachedMultilineText != null) {
                this.cachedMultilineText.Dispose();
                this.cachedMultilineText = null;
            }
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        public BetterListViewColumnHeader()
            : this(null, string.Empty, 128) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="image">column header image</param>
        public BetterListViewColumnHeader(Image image)
            : this(image, string.Empty, 128) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="text">column header text</param>
        public BetterListViewColumnHeader(string text)
            : this(null, text, 128) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="image">column header image</param>
        /// <param name="text">column header text</param>
        public BetterListViewColumnHeader(Image image, string text)
            : this(image, text, 128) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="image">column header image</param>
        /// <param name="width">column header width</param>
        public BetterListViewColumnHeader(Image image, int width)
            : this(image, string.Empty, width) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="text">column header text</param>
        /// <param name="width">column header width</param>
        public BetterListViewColumnHeader(string text, int width)
            : this(null, text, width) {
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        /// <param name="image">column header image</param>
        /// <param name="text">column header text</param>
        /// <param name="width">column header width</param>
        public BetterListViewColumnHeader(Image image, string text, int width)
            : base(image, new BetterListViewColumnHeaderBounds()) {
            Checks.CheckTrue(width >= 0, "width >= 0");
            this.text = text ?? string.Empty;
            this.width = width;
        }

        /// <summary>
        ///   Initialize a new BetterListViewColumnHeader instance.
        /// </summary>
        public BetterListViewColumnHeader(int imageIndex)
            : this(null, string.Empty, 128) {
            this.ImageIndex = imageIndex;
        }

        /// <summary>
        ///   Resize this column to fit its content.
        /// </summary>
        /// <param name="columnHeaderAutoResizeStyle">Column resizing method.</param>
        public void AutoResize(BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle) {
            this.ListView?.AutoResizeColumn(base.Index, columnHeaderAutoResizeStyle);
        }

        /// <summary>
        ///   Invalidate this column header for redrawing.
        /// </summary>
        public void Invalidate() {
            BetterListView listView = this.ListView;
            if (listView != null && ((IBetterListViewStateElement)this).State == BetterListViewElementState.ActiveVisible) {
                Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)this).LayoutBounds.BoundsSpacing;
                boundsSpacing.Offset(listView.OffsetColumnsFromAbsolute);
                listView.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, boundsSpacing);
            }
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public override object Clone() {
            BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader();
            this.CopyTo(betterListViewColumnHeader);
            return betterListViewColumnHeader;
        }

        /// <summary>
        ///   Remove element from its owner collection.
        /// </summary>
        /// <returns>success</returns>
        public override bool Remove() {
            if (this.OwnerCollection == null) {
                return false;
            }
            return ((BetterListViewColumnHeaderCollection)this.OwnerCollection).Remove(this);
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
            if (!(other is BetterListViewColumnHeader betterListViewColumnHeader) || !base.EqualsContent(other)) {
                return false;
            }
            if (this.alignHorizontal == betterListViewColumnHeader.alignHorizontal && this.alignHorizontalImage == betterListViewColumnHeader.alignHorizontalImage && this.alignVertical == betterListViewColumnHeader.alignVertical && this.alignVerticalImage == betterListViewColumnHeader.alignVerticalImage && object.Equals(this.font, betterListViewColumnHeader.font) && this.foreColor.Equals(betterListViewColumnHeader.foreColor) && this.minimumWidth == betterListViewColumnHeader.minimumWidth && this.preferredSortOrderAscending == betterListViewColumnHeader.preferredSortOrderAscending && this.sortMethod == betterListViewColumnHeader.sortMethod && this.style == betterListViewColumnHeader.style && this.text.Equals(betterListViewColumnHeader.text, StringComparison.Ordinal) && this.textTrimming == betterListViewColumnHeader.textTrimming && this.toolTips.EqualsContent(betterListViewColumnHeader.toolTips)) {
                return this.width == betterListViewColumnHeader.width;
            }
            return false;
        }

        /// <summary>
        ///   Set new column width.
        /// </summary>
        /// <param name="newWidth">new column width</param>
        /// <param name="columnWidthChangeSource">reason for column width change</param>
        /// <param name="raiseEvent">raise event informing about the width change</param>
        internal void SetWidth(int newWidth, BetterListViewColumnWidthChangeSource columnWidthChangeSource, bool raiseEvent) {
            newWidth = Math.Min(newWidth, this.MaximumWidth);
            newWidth = Math.Max(newWidth, this.MinimumWidth);
            if (newWidth != this.width) {
                int num = this.width;
                this.width = newWidth;
                if (raiseEvent) {
                    this.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout, num, new BetterListViewColumnWidthChangedEventArgs(this, columnWidthChangeSource));
                }
            }
        }

        /// <summary>
        ///   Copy content of this instance to the specified BetterListViewElementBase instance.
        /// </summary>
        /// <param name="element">BetterListViewElementBase to copy the content to</param>
        protected override void CopyTo(BetterListViewElementBase element) {
            BetterListViewColumnHeader betterListViewColumnHeader = (BetterListViewColumnHeader)element;
            betterListViewColumnHeader.AlignHorizontal = this.AlignHorizontal;
            betterListViewColumnHeader.AlignHorizontalImage = this.AlignHorizontalImage;
            betterListViewColumnHeader.AlignVertical = this.AlignVertical;
            betterListViewColumnHeader.AlignVerticalImage = this.AlignVerticalImage;
            betterListViewColumnHeader.Image = this.Image;
            betterListViewColumnHeader.MinimumWidth = this.MinimumWidth;
            betterListViewColumnHeader.PreferredSortOrderAscending = this.PreferredSortOrderAscending;
            betterListViewColumnHeader.SortMethod = this.SortMethod;
            betterListViewColumnHeader.Style = this.Style;
            betterListViewColumnHeader.Text = this.Text;
            betterListViewColumnHeader.Width = this.width;
            base.CopyTo(betterListViewColumnHeader);
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
            if (!(other is BetterListViewColumnHeader betterListViewColumnHeader)) {
                return 0;
            }
            return string.Compare(this.Text, betterListViewColumnHeader.Text, StringComparison.CurrentCulture);
        }

        internal void DrawingDrawText(Graphics graphics, Color color, Rectangle bounds, int maximumTextLines, bool useGdiPlus) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckTrue(bounds.Width >= 0 && bounds.Height >= 0, "bounds.Width >= 0 && bounds.Height >= 0");
            Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
            this.UpdateCachedMultilineText(maximumTextLines);
            MultilineText multilineText = ((!useGdiPlus) ? this.cachedMultilineText : new MultilineText(this.cachedMultilineText.Font, this.cachedMultilineText.TextTrimming, this.cachedMultilineText.MaximumTextLines, this.cachedMultilineText.TextOptions | TextOptions.UseGdiPlus, this.cachedMultilineText.TextLines, this.cachedMultilineText.VerticalEllipsis, this.cachedMultilineText.CachedWidth));
            MultilineTextRenderer.DrawText(graphics, color, multilineText, bounds, this.AlignHorizontal, this.AlignVertical);
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

        internal int LayoutGetLineCount(Graphics graphics, int width, int maximumTextLines) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckTrue(width >= 0, "width >= 0");
            Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
            this.UpdateCachedMultilineText(maximumTextLines);
            return MultilineTextRenderer.GetLineCount(graphics, width, this.cachedMultilineText);
        }

        internal TextSize LayoutGetTextSize(Graphics graphics, int width, int maximumTextLines) {
            Checks.CheckNotNull(graphics, "graphics");
            Checks.CheckTrue(width >= 0, "width >= 0");
            Checks.CheckTrue(maximumTextLines > 0, "maximumTextLines > 0");
            this.UpdateCachedMultilineText(maximumTextLines);
            return MultilineTextRenderer.MeasureText(graphics, width, this.cachedMultilineText);
        }

        private void UpdateCachedMultilineText(int maximumTextLines) {
            if (this.cachedMultilineText == null || this.cachedMultilineText.MaximumTextLines != maximumTextLines) {
                if (this.cachedMultilineText != null) {
                    this.cachedMultilineText.Dispose();
                }
                this.cachedMultilineText = new MultilineText(this.Text, (Font)this.Font.Clone(), this.TextTrimming, maximumTextLines, TextOptions.AllowWrap);
            }
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignHorizontal() {
            this.AlignHorizontal = TextAlignmentHorizontal.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignHorizontalImage() {
            this.AlignHorizontalImage = BetterListViewImageAlignmentHorizontal.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignVertical() {
            this.AlignVertical = TextAlignmentVertical.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignVerticalImage() {
            this.AlignVerticalImage = BetterListViewImageAlignmentVertical.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetFont() {
            this.font = null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetForeColor() {
            this.foreColor = Color.Empty;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignHorizontal() {
            return this.AlignHorizontal != TextAlignmentHorizontal.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignHorizontalImage() {
            return this.AlignHorizontalImage != BetterListViewImageAlignmentHorizontal.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignVertical() {
            return this.AlignVertical != TextAlignmentVertical.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignVerticalImage() {
            return this.AlignVerticalImage != BetterListViewImageAlignmentVertical.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeFont() {
            return this.font != null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeForeColor() {
            return !this.foreColor.IsEmpty;
        }

        /// <summary>
        ///   Get column header style with respest to BetterListViewHeaderStyle.Default value (inheriting value from BetterListView instance).
        /// </summary>
        /// <returns>Style of this column header.</returns>
        internal BetterListViewColumnHeaderStyle GetStyle() {
            return this.GetStyle(this.ListView);
        }

        /// <summary>
        ///   Get column header style with respest to BetterListViewHeaderStyle.Default value (inheriting value from BetterListView instance).
        /// </summary>
        /// <param name="listView">BetterListView to obtain column header style from.</param>
        /// <returns>Style of this column header.</returns>
        internal BetterListViewColumnHeaderStyle GetStyle(BetterListView listView) {
            Checks.CheckNotNull(listView, "listView");
            if (this.style != BetterListViewColumnHeaderStyle.Default) {
                return this.style;
            }
            //return listView.HeaderStyle switch {
            //	BetterListViewHeaderStyle.None => BetterListViewColumnHeaderStyle.None,
            //	BetterListViewHeaderStyle.Nonclickable => BetterListViewColumnHeaderStyle.Nonclickable,
            //	BetterListViewHeaderStyle.Clickable => BetterListViewColumnHeaderStyle.Clickable,
            //	BetterListViewHeaderStyle.Sortable => BetterListViewColumnHeaderStyle.Sortable,
            //	BetterListViewHeaderStyle.Unsortable => BetterListViewColumnHeaderStyle.Unsortable,
            //	_ => throw new ApplicationException($"Unknown header style: '{listView.HeaderStyle}'"),
            //};
            switch (listView.HeaderStyle) {
                case BetterListViewHeaderStyle.None: return BetterListViewColumnHeaderStyle.None; break;
                case BetterListViewHeaderStyle.Nonclickable: return BetterListViewColumnHeaderStyle.Nonclickable; break;
                case BetterListViewHeaderStyle.Clickable: return BetterListViewColumnHeaderStyle.Clickable; break;
                case BetterListViewHeaderStyle.Sortable: return BetterListViewColumnHeaderStyle.Sortable; break;
                case BetterListViewHeaderStyle.Unsortable: return BetterListViewColumnHeaderStyle.Unsortable; break;
                default: throw new ApplicationException($"Unknown header style: '{listView.HeaderStyle}'"); break;
            }
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetSortMethod() {
            this.SortMethod = BetterListViewSortMethod.Auto;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetStyle() {
            this.Style = BetterListViewColumnHeaderStyle.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeSortMethod() {
            return this.SortMethod != BetterListViewSortMethod.Auto;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeStyle() {
            return this.Style != BetterListViewColumnHeaderStyle.Default;
        }

        private BetterListViewColumnHeader(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            Checks.CheckNotNull(info, "info");
            Checks.CheckNotNull(context, "context");
            this.alignHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), info.GetString("alignHorizontal"));
            this.alignHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), info.GetString("alignHorizontalImage"));
            this.alignVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), info.GetString("alignVertical"));
            this.alignVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), info.GetString("alignVerticalImage"));
            string @string = info.GetString("font");
            this.font = ((@string.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, @string)) : null);
            this.foreColor = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, info.GetString("foreColor"));
            this.minimumWidth = info.GetInt32("minimumWidth");
            this.preferredSortOrderAscending = info.GetBoolean("preferredSortOrderAscending");
            this.smoothResize = info.GetBoolean("smoothResize");
            this.sortMethod = (BetterListViewSortMethod)Enum.Parse(typeof(BetterListViewSortMethod), info.GetString("sortMethod"));
            this.style = (BetterListViewColumnHeaderStyle)Enum.Parse(typeof(BetterListViewColumnHeaderStyle), info.GetString("style"));
            this.text = info.GetString("text");
            this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), info.GetString("textTrimming"));
            this.toolTips = (BetterListViewToolTipInfoCollection)info.GetValue("toolTips", typeof(BetterListViewToolTipInfoCollection));
            this.width = info.GetInt32("width");
            ((IBetterListViewLayoutElementDisplayable)this).LayoutBounds = new BetterListViewColumnHeaderBounds();
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
            info.AddValue("alignHorizontal", this.alignHorizontal.ToString());
            info.AddValue("alignHorizontalImage", this.alignHorizontalImage.ToString());
            info.AddValue("alignVertical", this.alignVertical.ToString());
            info.AddValue("alignVerticalImage", this.alignVerticalImage.ToString());
            info.AddValue("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
            info.AddValue("foreColor", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
            info.AddValue("minimumWidth", this.minimumWidth);
            info.AddValue("preferredSortOrderAscending", this.preferredSortOrderAscending);
            info.AddValue("smoothResize", this.smoothResize);
            info.AddValue("sortMethod", this.sortMethod.ToString());
            info.AddValue("style", this.style.ToString());
            info.AddValue("text", this.text);
            info.AddValue("textTrimming", this.textTrimming.ToString());
            info.AddValue("toolTips", this.toolTips, typeof(BetterListViewToolTipInfoCollection));
            info.AddValue("width", this.width);
            base.GetObjectDataInternal(info, context);
        }

        /// <summary>
        ///   Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        protected override void ReadXmlInternal(XmlReader reader) {
            reader.MoveToContent();
            reader.ReadStartElement();
            this.alignHorizontal = (TextAlignmentHorizontal)Enum.Parse(typeof(TextAlignmentHorizontal), reader.ReadElementString("alignHorizontal"));
            this.alignHorizontalImage = (BetterListViewImageAlignmentHorizontal)Enum.Parse(typeof(BetterListViewImageAlignmentHorizontal), reader.ReadElementString("alignHorizontalImage"));
            this.alignVertical = (TextAlignmentVertical)Enum.Parse(typeof(TextAlignmentVertical), reader.ReadElementString("alignVertical"));
            this.alignVerticalImage = (BetterListViewImageAlignmentVertical)Enum.Parse(typeof(BetterListViewImageAlignmentVertical), reader.ReadElementString("alignVerticalImage"));
            string text = reader.ReadElementString("font");
            this.font = ((text.Length != 0) ? ((Font)new FontConverter().ConvertFromString(null, CultureInfo.InvariantCulture, text)) : null);
            this.foreColor = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadElementString("foreColor"));
            this.minimumWidth = int.Parse(reader.ReadElementString("minimumWidth"), CultureInfo.InvariantCulture);
            this.preferredSortOrderAscending = bool.Parse(reader.ReadElementString("preferredSortOrderAscending"));
            this.smoothResize = bool.Parse(reader.ReadElementString("smoothResize"));
            this.sortMethod = (BetterListViewSortMethod)Enum.Parse(typeof(BetterListViewSortMethod), reader.ReadElementString("sortMethod"));
            this.style = (BetterListViewColumnHeaderStyle)Enum.Parse(typeof(BetterListViewColumnHeaderStyle), reader.ReadElementString("style"));
            this.text = reader.ReadElementString("text");
            this.textTrimming = (TextTrimming)Enum.Parse(typeof(TextTrimming), reader.ReadElementString("textTrimming"));
            reader.ReadStartElement("toolTips");
            this.toolTips = (BetterListViewToolTipInfoCollection)new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Deserialize(reader);
            reader.ReadEndElement();
            this.width = int.Parse(reader.ReadElementString("width"), CultureInfo.InvariantCulture);
            base.ReadXmlInternal(reader);
            reader.ReadEndElement();
        }

        /// <summary>
        ///   Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        protected override void WriteXmlInternal(XmlWriter writer) {
            writer.WriteElementString("alignHorizontal", this.alignHorizontal.ToString());
            writer.WriteElementString("alignHorizontalImage", this.alignHorizontalImage.ToString());
            writer.WriteElementString("alignVertical", this.alignVertical.ToString());
            writer.WriteElementString("alignVerticalImage", this.alignVerticalImage.ToString());
            writer.WriteElementString("font", (this.font != null) ? new FontConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.font) : string.Empty);
            writer.WriteElementString("foreColor", new ColorConverter().ConvertToString(null, CultureInfo.InvariantCulture, this.foreColor));
            writer.WriteElementString("minimumWidth", this.minimumWidth.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("preferredSortOrderAscending", this.preferredSortOrderAscending.ToString());
            writer.WriteElementString("smoothResize", this.smoothResize.ToString());
            writer.WriteElementString("sortMethod", this.sortMethod.ToString());
            writer.WriteElementString("style", this.style.ToString());
            writer.WriteElementString("text", this.text);
            writer.WriteElementString("textTrimming", this.textTrimming.ToString());
            writer.WriteStartElement("toolTips");
            new XmlSerializer(typeof(BetterListViewToolTipInfoCollection)).Serialize(writer, this.toolTips);
            writer.WriteEndElement();
            writer.WriteElementString("width", this.width.ToString(CultureInfo.InvariantCulture));
            base.WriteXmlInternal(writer);
        }

        /// <summary>
        ///   Change state of the element with the specified state transition.
        /// </summary>
        /// <param name="stateChange">State transition.</param>
        void IBetterListViewStateElement.ChangeState(BetterListViewElementStateChange stateChange) {
            this.elementStateBox.ChangeState(stateChange);
        }
    }
}