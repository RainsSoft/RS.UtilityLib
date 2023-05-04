using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Represets a BetterListView item.
    /// </summary>
    [Serializable]
    [Designer(typeof(BetterListViewItemDesigner))]
    [ToolboxItem(false)]
    [TypeConverter(typeof(BetterListViewItemConverter))]
    [DefaultProperty("Text")]
    public class BetterListViewItem : BetterListViewElementBase, IBetterListViewLayoutElementDisplayable, IBetterListViewLayoutElementSelectable, IBetterListViewStateElement
    {
        private const BetterListViewCheckBoxAppearance DefaultCheckBoxAppearance = BetterListViewCheckBoxAppearance.CheckBox;

        private const CheckState DefaultCheckState = CheckState.Unchecked;

        private const int DefaultCustomHeight = 0;

        private const bool DefaultIsExpanded = true;

        private const bool DefaultVisible = true;

        private const bool DefaultAllowSelectChildItems = true;

        private const bool DefaultAllowShowExpandButton = true;

        private const bool DefaultAllowSortChildItems = true;

        private const bool DefaultSelectable = true;

        private const bool DefaultUseItemStyleForSubItems = true;

        private const bool DefaultIsCut = false;

        private const string FieldAllowSelectChildItems = "allowSelectChildItems";

        private const string FieldAllowShowExpandButton = "allowShowExpandButton";

        private const string FieldCheckBoxAppearance = "checkBoxAppearance";

        private const string FieldCheckState = "checkState";

        private const string FieldChildItems = "childItems";

        private const string FieldCustomDropDisplayExternal = "customDropDisplayExternal";

        private const string FieldCustomDropDisplayInternal = "customDropDisplayInternal";

        private const string FieldCustomHeight = "customHeight";

        private const string FieldIsCut = "isCut";

        private const string FieldIsExpanded = "isExpanded";

        private const string FieldSelectable = "selectable";

        private const string FieldSubItems = "subItems";

        private const string FieldUseItemStyleForSubItems = "useItemStyleForSubItems";

        private const string FieldVisible = "visible";

        private BetterListViewItemAccessibleObject cachedAccessibleObject;

        private BetterListView cachedListView;

        private bool cachedListViewSet;

        private BetterListViewItemBounds layoutBounds;

        private int layoutIndexDisplay = -1;

        private int layoutIndexSelection = -1;

        private BetterListViewCheckBoxAppearance checkBoxAppearance = BetterListViewCheckBoxAppearance.CheckBox;

        private CheckState checkState;

        private BetterListViewDragDropDisplay customDropDisplayExternal = BetterListViewDragDropDisplay.Default;

        private BetterListViewDragDropDisplay customDropDisplayInternal = BetterListViewDragDropDisplay.Default;

        private int customHeight;

        private bool isExpanded = true;

        private bool visible = true;

        private bool allowSelectChildItems = true;

        private bool allowShowExpandButton = true;

        private bool allowSortChildItems = true;

        [NonSerialized]
        private BetterListViewGroup group;

        private bool selectable = true;

        private bool useItemStyleForSubItems = true;

        private bool isCut;

        private BetterListViewItemCollection childItems;

        private BetterListViewSubItemCollection subItems;

        private readonly BetterListViewElementStateBox elementStateBox = new BetterListViewElementStateBox();

        BetterListViewElementBoundsBase IBetterListViewLayoutElementDisplayable.LayoutBounds {
            get {
                return this.layoutBounds;
            }
            set {
                Checks.CheckType(value, typeof(BetterListViewItemBounds), "value", allowNull: true);
                this.layoutBounds = (BetterListViewItemBounds)value;
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

        int IBetterListViewLayoutElementSelectable.LayoutIndexSelection {
            get {
                return this.layoutIndexSelection;
            }
            set {
                Checks.CheckTrue(value == -1 || value >= 0, "value == IndexUndefined || value >= 0");
                this.layoutIndexSelection = value;
            }
        }

        /// <summary>
        ///   horizontal alignment of the item text
        /// </summary>
        [Category("Appearance")]
        [Description("Horizontal alignment of the item text")]
        public TextAlignmentHorizontal AlignHorizontal {
            get {
                return this.ReferenceSubItem.AlignHorizontal;
            }
            set {
                this.ReferenceSubItem.AlignHorizontal = value;
            }
        }

        /// <summary>
        ///   horizontal alignment of the item image
        /// </summary>
        [Category("Appearance")]
        [Description("Horizontal alignment of the item image")]
        public BetterListViewImageAlignmentHorizontal AlignHorizontalImage {
            get {
                return this.ReferenceSubItem.AlignHorizontalImage;
            }
            set {
                this.ReferenceSubItem.AlignHorizontalImage = value;
            }
        }

        /// <summary>
        ///   vertical alignment of the item text
        /// </summary>
        [Description("Vertical alignment of the item text")]
        [Category("Appearance")]
        public TextAlignmentVertical AlignVertical {
            get {
                return this.ReferenceSubItem.AlignVertical;
            }
            set {
                this.ReferenceSubItem.AlignVertical = value;
            }
        }

        /// <summary>
        ///   vertical alignment of the item image
        /// </summary>
        [Category("Appearance")]
        [Description("Vertical alignment of the item image")]
        public BetterListViewImageAlignmentVertical AlignVerticalImage {
            get {
                return this.ReferenceSubItem.AlignVerticalImage;
            }
            set {
                this.ReferenceSubItem.AlignVerticalImage = value;
            }
        }

        /// <summary>
        ///   background color of the item
        /// </summary>
        [Description("Background color of the item")]
        [Category("Appearance")]
        public Color BackColor {
            get {
                return this.ReferenceSubItem.BackColor;
            }
            set {
                this.ReferenceSubItem.BackColor = value;
            }
        }

        /// <summary>
        ///   Determines how is the check box displayed.
        /// </summary>
        internal BetterListViewCheckBoxAppearance CheckBoxAppearance => this.checkBoxAppearance;

        /// <summary>
        ///   check state of this item's check box
        /// </summary>
        internal CheckState CheckState => this.checkState;

        /// <summary>
        ///   customized drop effect displayed on the item when dragging data from external source (external Drag and Drop)
        /// </summary>
        internal BetterListViewDragDropDisplay CustomDropDisplayExternal => this.customDropDisplayExternal;

        /// <summary>
        ///   customized drop effect displayed on the item when dragging data from internal source (internal Drag and Drop / item reorder)
        /// </summary>
        internal BetterListViewDragDropDisplay CustomDropDisplayInternal => this.customDropDisplayInternal;

        /// <summary>
        ///   customized height of the item
        /// </summary>
        internal int CustomHeight => this.customHeight;

        /// <summary>
        ///   Text formatted for display.
        /// </summary>
        internal string DisplayText => this.ReferenceSubItem.DisplayText;

        /// <summary>
        ///   font of the item text
        /// </summary>
        [Category("Appearance")]
        [Description("Font of the item text")]
        public override Font Font {
            get {
                return this.ReferenceSubItem.Font;
            }
            set {
                this.ReferenceSubItem.Font = value;
            }
        }

        /// <summary>
        ///   foreground color of the item content
        /// </summary>
        [Description("Foreground color of the item content")]
        [Category("Appearance")]
        public override Color ForeColor {
            get {
                return this.ReferenceSubItem.ForeColor;
            }
            set {
                this.ReferenceSubItem.ForeColor = value;
            }
        }

        /// <summary>
        ///   Pre-format item text for custom display.
        /// </summary>
        internal bool FormatText => this.ReferenceSubItem.FormatText;

        /// <summary>
        ///   Allow displaying hotkey prefix in item text (underline after '&amp;' character).
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Allow displaying hotkey prefix in item text (underline after '&' character).")]
        public bool HotkeyPrefix {
            get {
                return this.ReferenceSubItem.HotkeyPrefix;
            }
            set {
                this.ReferenceSubItem.HotkeyPrefix = value;
            }
        }

        /// <summary>
        ///   image for the item
        ///   this image is used prior to the image specified by ImageIndex/ImageKey property
        /// </summary>
        [DefaultValue(null)]
        [Category("Appearance")]
        [Description("Image for the item; this image is used prior to the image specified by ImageIndex/ImageKey property")]
        public override Image Image {
            get {
                return this.ReferenceSubItem.Image;
            }
            set {
                this.ReferenceSubItem.Image = value;
            }
        }

        /// <summary>
        ///   index specifying item image in the ImageList
        ///   set to BetterListViewElementBase.IndexUndefined, if not defined
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(-1)]
        [Description("Index specifying image for the item in ImageList; set to BetterListViewElementBase.IndexUndefined, if not defined")]
        public override int ImageIndex {
            get {
                return this.ReferenceSubItem.ImageIndex;
            }
            set {
                this.ReferenceSubItem.ImageIndex = value;
            }
        }

        /// <summary>
        ///   key specifying item image in the ImageList
        ///   set to String.Empty, if not defined
        /// </summary>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("Key specifying image for the item in ImageList; set to String.Empty, if not defined")]
        public override string ImageKey {
            get {
                return this.ReferenceSubItem.ImageKey;
            }
            set {
                this.ReferenceSubItem.ImageKey = value;
            }
        }

        /// <summary>
        ///   draw the item bold
        /// </summary>
        [DesignOnly(true)]
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Draw the item bold")]
        public bool IsBold {
            get {
                return this.ReferenceSubItem.IsBold;
            }
            set {
                this.ReferenceSubItem.IsBold = value;
            }
        }

        /// <summary>
        ///   child items are visible
        /// </summary>
        internal bool IsExpanded => this.isExpanded;

        /// <summary>
        ///   label of the item
        /// </summary>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("text of the item")]
        public string Text {
            get {
                return this.ReferenceSubItem.Text;
            }
            set {
                this.ReferenceSubItem.Text = value;
            }
        }

        /// <summary>
        ///   item text trimming
        /// </summary>
        internal override TextTrimming TextTrimming => this.ReferenceSubItem.TextTrimming;

        /// <summary>
        ///   information about ToolTips shown on this item
        /// </summary>
        internal BetterListViewToolTipInfoCollection ToolTips => this.ReferenceSubItem.ToolTips;

        /// <summary>
        ///   The item is visible.
        /// </summary>
        internal bool Visible => this.visible;

        private BetterListViewSubItem ReferenceSubItem => this.subItems[0];

        /// <summary>
        /// allow selecting individual child items
        /// </summary>
        internal bool AllowSelectChildItems => this.allowSelectChildItems;

        /// <summary>
        ///   allow displaying expand button on item
        /// </summary>
        internal bool AllowShowExpandButton => this.allowShowExpandButton;

        /// <summary>
        ///   allow sorting child items of this item along with other items
        /// </summary>
        internal bool AllowSortChildItems => this.allowSortChildItems;

        /// <summary>
        ///   group in which this item is contained
        /// </summary>
        [Category("Behavior")]
        [MergableProperty(true)]
        [DefaultValue(null)]
        [Description("Group in which is this item contained")]
        public BetterListViewGroup Group {
            get {
                if (this.group != null) {
                    return this.group;
                }
                return this.ParentItem?.Group;
            }
            set {
                this.SetGroup(value, quiet: false);
            }
        }

        /// <summary>
        ///   the item can be selected
        /// </summary>
        internal bool Selectable => this.selectable;

        /// <summary>
        ///   the Font, ForeColor, and BackColor properties for the item are used for all its sub-items
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [MergableProperty(true)]
        [Description("Gets or sets whether the Font, ForeColor, and BackColor properties for the item are used for all its sub-items")]
        public bool UseItemStyleForSubItems {
            get {
                return this.useItemStyleForSubItems;
            }
            set {
                if (this.useItemStyleForSubItems != value) {
                    this.useItemStyleForSubItems = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Layout);
                }
            }
        }

        /// <summary>
        ///   address of this item
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewAddress Address {
            get {
                if (this.ParentItem != null) {
                    return new BetterListViewAddress(base.Index, this.ParentItem.Address);
                }
                return new BetterListViewAddress(base.Index, (this.group != null) ? this.group.Address : null);
            }
        }

        /// <summary>
        ///   Gets item boundaries, including sub-items.
        /// </summary>
        /// <returns><see cref="T:ComponentOwl.BetterListView.BetterListViewItemBounds" /> instance if the element is active, null otherwise.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewItemBounds Bounds => this.ListView?.GetItemBounds(this);

        /// <summary>
        ///   this item is checked
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Checked {
            get {
                return this.checkState == CheckState.Checked;
            }
            set {
                this.SetCheckState(value ? CheckState.Checked : CheckState.Unchecked, BetterListViewCheckStateChangeMode.UserCode, quiet: false);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this item is focused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this item is focused; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Focused {
            get {
                BetterListView listView = this.ListView;
                if (listView == null) {
                    return false;
                }
                return listView.FocusedItem == this;
            }
            set {
                BetterListView listView = this.ListView;
                if (listView != null) {
                    listView.FocusedItem = this;
                }
            }
        }

        /// <summary>
        ///   the item is cut in the clipboard
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsCut {
            get {
                return this.isCut;
            }
            set {
                if (this.isCut != value) {
                    this.isCut = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Appearance);
                }
            }
        }

        /// <summary>
        ///   Gets the ImageList corresponding to this item.
        /// </summary>
        public ImageList ImageList => this.ListView?.ImageListCurrent;

        /// <summary>
        ///   value used for item comparison
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IComparable Key {
            get {
                return this.SubItems[0].Key;
            }
            set {
                if (this.Key != value) {
                    this.SubItems[0].Key = value;
                    base.OnElementPropertyChanged(BetterListViewElementPropertyType.Sorting);
                }
            }
        }

        /// <summary>
        ///   last child item of this item that can be reached without item expansion
        /// </summary>
        internal BetterListViewItem LastAvailableChildItem => null;

        /// <summary>
        ///   current level of item in item hierarchy
        /// </summary>
        internal int Level => 0;

        /// <summary>
        ///   BetterListView instance in which this item is contained
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListView ListView {
            get {
                if (!this.cachedListViewSet) {
                    if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                        this.cachedListView = (BetterListView)this.OwnerCollection.OwnerControl;
                    }
                    else {
                        this.cachedListView = null;
                    }
                    this.cachedListViewSet = true;
                }
                return this.cachedListView;
            }
        }

        /// <summary>
        ///   next item in the owner collection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewItem NextItem {
            get {
                if (this.OwnerCollection == null) {
                    return null;
                }
                BetterListViewItemCollection betterListViewItemCollection = (BetterListViewItemCollection)this.OwnerCollection;
                if (base.Index == betterListViewItemCollection.Count - 1) {
                    return null;
                }
                return betterListViewItemCollection[base.Index + 1];
            }
        }

        /// <summary>
        ///   next item visible in the control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewItem NextVisibleItem {
            get {
                if (!((IBetterListViewStateElement)this).IsActive) {
                    return null;
                }
                BetterListView listView = this.ListView;
                if (listView == null) {
                    return null;
                }
                ReadOnlyCollection<BetterListViewItem> layoutElementsItems = listView.LayoutElementsItems;
                int num = ((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay;
                if (num >= layoutElementsItems.Count - 1) {
                    return null;
                }
                return layoutElementsItems[num + 1];
            }
        }

        /// <summary>
        ///   parent item of this item in item hierarchy
        /// </summary>
        internal BetterListViewItem ParentItem => null;

        /// <summary>
        ///   previous item in the owner collection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewItem PreviousItem {
            get {
                if (this.OwnerCollection == null || base.Index == 0) {
                    return null;
                }
                return ((BetterListViewItemCollection)this.OwnerCollection)[base.Index - 1];
            }
        }

        /// <summary>
        ///   previous item visible in the control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewItem PreviousVisibleItem {
            get {
                if (!((IBetterListViewStateElement)this).IsActive) {
                    return null;
                }
                BetterListView listView = this.ListView;
                if (listView == null) {
                    return null;
                }
                ReadOnlyCollection<BetterListViewItem> layoutElementsItems = listView.LayoutElementsItems;
                int num = ((IBetterListViewLayoutElementDisplayable)this).LayoutIndexDisplay;
                if (num <= 0) {
                    return null;
                }
                return layoutElementsItems[num - 1];
            }
        }

        /// <summary>
        ///   get selectable item corresponding to this item
        /// </summary>
        internal BetterListViewItem SelectableItem {
            get {
                if (!this.Selectable) {
                    return null;
                }
                return this;
            }
        }

        /// <summary>
        ///   this item is selected
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Selected {
            get {
                if (this.OwnerCollection != null && this.OwnerCollection.OwnerControl != null) {
                    return ((BetterListView)this.OwnerCollection.OwnerControl).SelectedItemsSet.Contains(this);
                }
                return false;
            }
            set {
                Checks.CheckNotNull(this.OwnerCollection, "OwnerCollection", "Item is not contained in an item collection.");
                Checks.CheckNotNull(this.OwnerCollection.OwnerControl, "OwnerCollection.OwnerControl", "Item is not contained in any control.");
                BetterListView betterListView = (BetterListView)this.OwnerCollection.OwnerControl;
                BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(betterListView.SelectedItemsSet);
                if (value) {
                    betterListViewItemSet.Add(this);
                }
                else {
                    betterListViewItemSet.Remove(this);
                }
                betterListView.SelectedItemsSet = betterListViewItemSet;
                base.OnElementPropertyChanged(BetterListViewElementPropertyType.Selection);
            }
        }

        /// <summary>
        ///   corresponding value specified by the ValueMember property of the first column or the control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Value {
            get {
                return this.SubItems[0].Value;
            }
            set {
                this.SubItems[0].Value = value;
            }
        }

        /// <summary>
        ///   Check whether the item can be selected because Selectable property is set to false or some parent item has AllowSelectChildItems property set to false.
        /// </summary>
        internal bool IsSelectable {
            get {
                if (!this.Selectable) {
                    return false;
                }
                for (BetterListViewItem parentItem = this.ParentItem; parentItem != null; parentItem = parentItem.ParentItem) {
                    if (!parentItem.AllowSelectChildItems) {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        ///   child items of this item
        /// </summary>
        internal BetterListViewItemCollection ChildItems => this.childItems;

        /// <summary>
        ///   collection of sub-items of this item
        /// </summary>
        [MergableProperty(false)]
        [Category("Data")]
        [Description("Collection of sub-items of this item")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        public BetterListViewSubItemCollection SubItems => this.subItems;

        internal BetterListViewGroup GroupInternal => this.group;

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
        ///   Get accessible object instance for this item.
        /// </summary>
        /// <param name="listView">Owner list control.</param>
        /// <param name="parent">Parent accessible object.</param>
        /// <returns>Accessible object instance.</returns>
        protected internal virtual AccessibleObject GetAccessibilityInstance(BetterListView listView, AccessibleObject parent) {
            Checks.CheckNotNull(listView, "listView");
            Checks.CheckNotNull(parent, "parent");
            if (this.cachedAccessibleObject == null) {
                this.cachedAccessibleObject = new BetterListViewItemAccessibleObject(this, listView, parent);
            }
            return this.cachedAccessibleObject;
        }

        internal void ClearCache() {
            this.ClearCache(clearTextOnly: false);
        }

        internal void ClearCache(bool clearTextOnly) {
            this.cachedAccessibleObject = null;
            foreach (BetterListViewSubItem subItem in this.SubItems) {
                subItem.ClearCache(clearTextOnly);
            }
        }

        internal override void ResetIndex() {
            base.ResetIndex();
            BetterListViewItem.ResetCacheHierarchy(this);
            if (this.childItems != null && this.childItems.Count != 0) {
                BetterListViewItem.ResetCacheHierarchy(this.childItems);
            }
        }

        private static void ResetCacheHierarchy(BetterListViewItemCollection items) {
            foreach (BetterListViewItem item in items) {
                BetterListViewItem.ResetCacheHierarchy(item);
                if (item.childItems.Count != 0) {
                    BetterListViewItem.ResetCacheHierarchy(item.childItems);
                }
            }
        }

        private static void ResetCacheHierarchy(BetterListViewItem item) {
            item.cachedListView = null;
            item.cachedListViewSet = false;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        public BetterListViewItem()
            : this(string.Empty) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="image">Item image.</param>
        public BetterListViewItem(Image image)
            : this(image, string.Empty) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        public BetterListViewItem(string text)
            : this(null, new string[1] { text }) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="image">Item image.</param>
        /// <param name="text">Label of the item.</param>
        public BetterListViewItem(Image image, string text)
            : this(image, new string[1] { text }) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        public BetterListViewItem(string[] items)
            : this(null, items) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="image">Item image.</param>
        /// <param name="items">Array of sub-item labels.</param>
        public BetterListViewItem(Image image, string[] items)
            : base(new BetterListViewItemBounds(items.Length)) {
            Checks.CheckCollection(items, "items");
            this.childItems = new BetterListViewItemCollection(isInternal: true);
            this.childItems.OwnerItem = this;
            BetterListViewSubItem[] array = new BetterListViewSubItem[items.Length];
            for (int i = 0; i < items.Length; i++) {
                array[i] = new BetterListViewSubItem(items[i]);
            }
            this.subItems = new BetterListViewSubItemCollection(isInternal: true, array);
            this.subItems.OwnerItem = this;
            this.Image = image;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="subItems">Sub-items to be contained within this item.</param>
        private BetterListViewItem(BetterListViewSubItem[] subItems)
            : base(new BetterListViewItemBounds((subItems == null || subItems.Length == 0) ? 1 : subItems.Length)) {
            Checks.CheckCollection(subItems, "subItems");
            foreach (BetterListViewSubItem betterListViewSubItem in subItems) {
                Checks.CheckNotNull(betterListViewSubItem, "subItem");
                Checks.CheckTrue(betterListViewSubItem.Item == null, "subItem.Item == null", "Sub-item '" + betterListViewSubItem.Text + "' is already attached to some item.");
            }
            this.childItems = new BetterListViewItemCollection(isInternal: true);
            this.childItems.OwnerItem = this;
            this.subItems = new BetterListViewSubItemCollection(isInternal: true, subItems);
            this.subItems.OwnerItem = this;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="subItems">Sub-items to be contained within this item.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(BetterListViewSubItem[] subItems, string imageKey, BetterListViewGroup group)
            : this(subItems) {
            this.ImageKey = imageKey;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        /// <param name="foreColor">Foreground color of the item content.</param>
        /// <param name="backColor">Background color of the item.</param>
        /// <param name="font">Font of the item text.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, BetterListViewGroup group)
            : this(items) {
            this.ImageKey = imageKey;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string[] items, string imageKey, BetterListViewGroup group)
            : this(items) {
            this.ImageKey = imageKey;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string text, string imageKey, BetterListViewGroup group)
            : this(text) {
            this.ImageKey = imageKey;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="subItems">Sub-items to be contained within this item.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        public BetterListViewItem(BetterListViewSubItem[] subItems, string imageKey)
            : this(subItems) {
            this.ImageKey = imageKey;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        /// <param name="foreColor">Foreground color of the item content.</param>
        /// <param name="backColor">Background color of the item.</param>
        /// <param name="font">Font of the item text.</param>
        public BetterListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font)
            : this(items) {
            this.ImageKey = imageKey;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        public BetterListViewItem(string[] items, string imageKey)
            : this(items) {
            this.ImageKey = imageKey;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="imageKey">Key specifying item image in the ImageList.</param>
        public BetterListViewItem(string text, string imageKey)
            : this(text) {
            this.ImageKey = imageKey;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="subItems">Sub-items to be contained within this item.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(BetterListViewSubItem[] subItems, int imageIndex, BetterListViewGroup group)
            : this(subItems) {
            this.ImageIndex = imageIndex;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        /// <param name="foreColor">Foreground color of the item content.</param>
        /// <param name="backColor">Background color of the item.</param>
        /// <param name="font">Font of the item text.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, BetterListViewGroup group)
            : this(items) {
            this.ImageIndex = imageIndex;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string[] items, int imageIndex, BetterListViewGroup group)
            : this(items) {
            this.ImageIndex = imageIndex;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string[] items, BetterListViewGroup group)
            : this(items) {
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string text, int imageIndex, BetterListViewGroup group)
            : this(text) {
            this.ImageIndex = imageIndex;
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(string text, BetterListViewGroup group)
            : this(text) {
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="group">Group in which this item should be contained.</param>
        public BetterListViewItem(BetterListViewGroup group)
            : this() {
            this.SetGroup(group, quiet: false);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="subItems">Sub-items to be contained within this item.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        public BetterListViewItem(BetterListViewSubItem[] subItems, int imageIndex)
            : this(subItems) {
            this.ImageIndex = imageIndex;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        /// <param name="foreColor">Foreground color of the item content.</param>
        /// <param name="backColor">Background color of the item.</param>
        /// <param name="font">Font of the item text.</param>
        public BetterListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font)
            : this(items) {
            this.ImageIndex = imageIndex;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="items">Sub-item labels.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        public BetterListViewItem(string[] items, int imageIndex)
            : this(items) {
            this.ImageIndex = imageIndex;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> class.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="imageIndex">Index specifying item image in the ImageList.</param>
        public BetterListViewItem(string text, int imageIndex)
            : this(text) {
            this.ImageIndex = imageIndex;
        }

        /// <summary>
        ///   Edit label of this item.
        /// </summary>
        public void BeginEdit() {
            this.SubItems[0].BeginEdit();
        }

        /// <summary>
        ///   Ensure this item is visible within the control, scrolling contents of the control, if necessary.
        /// </summary>
        public virtual void EnsureVisible() {
            this.ListView?.EnsureVisible(this);
        }

        /// <summary>
        ///   Find the next item from this item, searching in the specified direction.
        /// </summary>
        /// <param name="searchDirection">Item search direction.</param>
        /// <returns>Item closest to coordinates of this item in the specified direction.</returns>
        public BetterListViewItem FindNearestItem(SearchDirectionHint searchDirection) {
            return this.ListView?.FindNearestItem(this, searchDirection);
        }

        /// <summary>
        ///   Retrieves the specified portion of the item boundaries.
        /// </summary>
        /// <param name="itemBoundsPortion">Portion of the item boundaries.</param>
        /// <returns>Portion of the item boundaries as rectangle, if defined. Rectangle.Empty, otherwise.</returns>
        public Rectangle GetBounds(BetterListViewItemBoundsPortion itemBoundsPortion) {
            BetterListViewItemBounds bounds = this.Bounds;
            if (bounds == null) {
                return Rectangle.Empty;
            }
            //return itemBoundsPortion switch {
            //	BetterListViewItemBoundsPortion.Entire => bounds.BoundsOuterExtended,
            //	BetterListViewItemBoundsPortion.Icon => bounds.SubItemBounds[0].BoundsImage,
            //	BetterListViewItemBoundsPortion.Label => bounds.SubItemBounds[0].BoundsText,
            //	BetterListViewItemBoundsPortion.ItemOnly => bounds.SubItemBounds[0].BoundsOuter,
            //	BetterListViewItemBoundsPortion.Selection => bounds.BoundsSelection,
            //	BetterListViewItemBoundsPortion.ExpandButton => bounds.BoundsExpandButton,
            //	BetterListViewItemBoundsPortion.CheckBox => bounds.BoundsCheckBox,
            //	_ => throw new ApplicationException($"Unknown item bounds portion: '{itemBoundsPortion}'."),
            //};
            switch (itemBoundsPortion) {
                case BetterListViewItemBoundsPortion.Entire:
                    return bounds.BoundsOuterExtended;
                    break;
                case BetterListViewItemBoundsPortion.Icon:
                    return bounds.SubItemBounds[0].BoundsImage;
                    break;
                case BetterListViewItemBoundsPortion.Label:
                    return bounds.SubItemBounds[0].BoundsText;
                    break;
                case BetterListViewItemBoundsPortion.ItemOnly:
                    return bounds.SubItemBounds[0].BoundsOuter;
                    break;
                case BetterListViewItemBoundsPortion.Selection:
                    return bounds.BoundsSelection;
                    break;
                case BetterListViewItemBoundsPortion.ExpandButton:
                    return bounds.BoundsExpandButton;
                    break;
                case BetterListViewItemBoundsPortion.CheckBox:
                    return bounds.BoundsCheckBox;
                    break;
                default:
                    throw new ApplicationException($"Unknown item bounds portion: '{itemBoundsPortion}'.");
                    break;
            };
        }

        /// <summary>
        ///   Get sub-item of this item located at the specified coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the point where to search sub-item.</param>
        /// <param name="y">Y-coordinate of the point where to search sub-item.</param>
        /// <returns></returns>
        public BetterListViewSubItem GetSubItemAt(int x, int y) {
            BetterListView listView = this.ListView;
            if (listView == null) {
                return null;
            }
            BetterListViewSubItem subItemAt = listView.GetSubItemAt(x, y);
            if (subItemAt != null && subItemAt.Item == this) {
                return subItemAt;
            }
            return null;
        }

        /// <summary>
        ///   Invalidate this item for redrawing.
        /// </summary>
        public void Invalidate() {
            BetterListView listView = this.ListView;
            if (listView != null && ((IBetterListViewStateElement)this).State == BetterListViewElementState.ActiveVisible) {
                Rectangle boundsSpacing = ((IBetterListViewLayoutElementDisplayable)this).LayoutBounds.BoundsSpacing;
                boundsSpacing.Offset(listView.OffsetContentFromAbsolute);
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
            BetterListViewItem betterListViewItem = new BetterListViewItem();
            this.CopyTo(betterListViewItem);
            return betterListViewItem;
        }

        /// <summary>
        ///   Remove element from its owner collection.
        /// </summary>
        /// <returns>success</returns>
        public override bool Remove() {
            if (this.OwnerCollection == null) {
                return false;
            }
            return ((BetterListViewItemCollection)this.OwnerCollection).Remove(this);
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
            if (!(other is BetterListViewItem betterListViewItem) || !base.EqualsContent(other)) {
                return false;
            }
            if (this.checkBoxAppearance != betterListViewItem.checkBoxAppearance || this.checkState != betterListViewItem.checkState || this.childItems.Count != betterListViewItem.childItems.Count || this.customDropDisplayExternal != betterListViewItem.customDropDisplayExternal || this.customDropDisplayInternal != betterListViewItem.customDropDisplayInternal || this.customHeight != betterListViewItem.customHeight || this.isCut != betterListViewItem.isCut || this.subItems.Count != betterListViewItem.subItems.Count || this.useItemStyleForSubItems != betterListViewItem.useItemStyleForSubItems) {
                return false;
            }
            for (int i = 0; i < this.childItems.Count; i++) {
                if (!this.childItems[i].EqualsContent(betterListViewItem.childItems[i])) {
                    return false;
                }
            }
            for (int j = 0; j < this.subItems.Count; j++) {
                if (!this.subItems[j].EqualsContent(betterListViewItem.subItems[j])) {
                    return false;
                }
            }
            return true;
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
                return base.GetType().Name + ": {Text = '" + this.Text + "'}";
            }
            return base.GetType().Name + ": {Address = '" + this.Address.ToString(includeTypeName: false, includeGroupAddress: true) + "', Text = '" + this.Text + "'}";
        }

        internal void GetAllChildItems(bool includeCollapsed, ref List<BetterListViewItem> childItems) {
            Checks.CheckNotNull(childItems, "childItems");
            if (this.ChildItems.Count == 0 || !(this.IsExpanded || includeCollapsed)) {
                return;
            }
            foreach (BetterListViewItem childItem in this.ChildItems) {
                childItems.Add(childItem);
                childItem.GetAllChildItems(includeCollapsed, ref childItems);
            }
        }

        /// <summary>
        ///   Set current group of this item.
        /// </summary>
        /// <param name="group">current group of this item</param>
        /// <param name="quiet">do not perform update</param>
        internal void SetGroup(BetterListViewGroup group, bool quiet) {
            if (this.group == group) {
                return;
            }
            if (!quiet) {
                if (this.group != null) {
                    this.group.Items.Remove(this);
                }
                group?.Items.Add(this);
            }
            this.group = group;
            if (!quiet) {
                base.OnElementPropertyChanged(BetterListViewElementPropertyType.Grouping);
            }
        }

        /// <summary>
        ///   Copy content of this instance to the specified BetterListViewElementBase instance.
        /// </summary>
        /// <param name="element">BetterListViewElementBase to copy the content to</param>
        protected override void CopyTo(BetterListViewElementBase element) {
            BetterListViewItem betterListViewItem = (BetterListViewItem)element;
            betterListViewItem.BackColor = this.BackColor;
            betterListViewItem.SetCheckState(this.CheckState, BetterListViewCheckStateChangeMode.UserCode, quiet: true);
            betterListViewItem.IsCut = this.IsCut;
            foreach (BetterListViewItem childItem in this.ChildItems) {
                betterListViewItem.ChildItems.Add((BetterListViewItem)childItem.Clone());
            }
            betterListViewItem.SubItems[0] = (BetterListViewSubItem)this.SubItems[0].Clone();
            for (int i = 1; i < this.SubItems.Count; i++) {
                betterListViewItem.SubItems.Add((BetterListViewSubItem)this.SubItems[i].Clone());
            }
            betterListViewItem.UseItemStyleForSubItems = this.UseItemStyleForSubItems;
            base.CopyTo(betterListViewItem);
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
            if (!(other is BetterListViewItem betterListViewItem)) {
                return 0;
            }
            return string.Compare(this.Text, betterListViewItem.Text, StringComparison.CurrentCulture);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose() {
            foreach (BetterListViewSubItem subItem in this.SubItems) {
                subItem.Dispose();
            }
            base.Dispose();
        }

        internal void SetCheckState(CheckState checkStateNew, BetterListViewCheckStateChangeMode checkStateChangeMode, bool quiet) {
            Checks.CheckNotEqual(checkStateChangeMode, BetterListViewCheckStateChangeMode.Undefined, "checkStateChangeMode", "BetterListViewCheckStateChangeMode.Undefined");
            if (this.checkState != checkStateNew) {
                CheckState checkState = this.checkState;
                this.checkState = checkStateNew;
                if (!quiet) {
                    this.OnElementPropertyChanged(BetterListViewElementPropertyType.Check, checkState, new BetterListViewItemCheckedEventArgs(this, checkState, checkStateNew, checkStateChangeMode));
                }
            }
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignHorizontal() {
            this.ReferenceSubItem.ResetAlignHorizontal();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignHorizontalImage() {
            this.ReferenceSubItem.ResetAlignHorizontalImage();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignVertical() {
            this.ReferenceSubItem.ResetAlignVertical();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAlignVerticalImage() {
            this.ReferenceSubItem.ResetAlignVerticalImage();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetBackColor() {
            this.ReferenceSubItem.ResetBackColor();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetFont() {
            this.ReferenceSubItem.ResetFont();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetForeColor() {
            this.ReferenceSubItem.ResetForeColor();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignHorizontal() {
            return this.ReferenceSubItem.ShouldSerializeAlignHorizontal();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignHorizontalImage() {
            return this.ReferenceSubItem.ShouldSerializeAlignHorizontalImage();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignVertical() {
            return this.ReferenceSubItem.ShouldSerializeAlignVertical();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAlignVerticalImage() {
            return this.ReferenceSubItem.ShouldSerializeAlignVerticalImage();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeBackColor() {
            return this.ReferenceSubItem.ShouldSerializeBackColor();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeFont() {
            return this.ReferenceSubItem.ShouldSerializeFont();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeForeColor() {
            return this.ReferenceSubItem.ShouldSerializeForeColor();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetSubItems() {
            this.SubItems.Clear();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeSubItems() {
            return this.SubItems.Count != 1;
        }

        private BetterListViewItem(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            Checks.CheckNotNull(info, "info");
            Checks.CheckNotNull(context, "context");
            this.allowSelectChildItems = info.GetBoolean("allowSelectChildItems");
            this.allowShowExpandButton = info.GetBoolean("allowShowExpandButton");
            this.checkBoxAppearance = (BetterListViewCheckBoxAppearance)Enum.Parse(typeof(BetterListViewCheckBoxAppearance), info.GetString("checkBoxAppearance"));
            this.checkState = (CheckState)Enum.Parse(typeof(CheckState), info.GetString("checkState"));
            this.childItems = (BetterListViewItemCollection)info.GetValue("childItems", typeof(BetterListViewItemCollection));
            this.customDropDisplayExternal = (BetterListViewDragDropDisplay)Enum.Parse(typeof(BetterListViewDragDropDisplay), info.GetString("customDropDisplayExternal"));
            this.customDropDisplayInternal = (BetterListViewDragDropDisplay)Enum.Parse(typeof(BetterListViewDragDropDisplay), info.GetString("customDropDisplayInternal"));
            this.customHeight = info.GetInt32("customHeight");
            this.isCut = info.GetBoolean("isCut");
            this.isExpanded = info.GetBoolean("isExpanded");
            this.selectable = info.GetBoolean("selectable");
            this.subItems = (BetterListViewSubItemCollection)info.GetValue("subItems", typeof(BetterListViewSubItemCollection));
            this.useItemStyleForSubItems = info.GetBoolean("useItemStyleForSubItems");
            this.visible = info.GetBoolean("visible");
            ((IBetterListViewLayoutElementDisplayable)this).LayoutBounds = new BetterListViewItemBounds(this.subItems.Count);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context) {
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context) {
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            this.childItems.OwnerItem = this;
            this.childItems.SetInternal();
            this.subItems.OwnerItem = this;
            this.subItems.SetInternal();
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
            info.AddValue("allowSelectChildItems", this.allowSelectChildItems);
            info.AddValue("allowShowExpandButton", this.allowShowExpandButton);
            info.AddValue("checkBoxAppearance", this.checkBoxAppearance.ToString());
            info.AddValue("checkState", this.checkState.ToString());
            info.AddValue("childItems", this.childItems);
            info.AddValue("customDropDisplayExternal", this.customDropDisplayExternal.ToString());
            info.AddValue("customDropDisplayInternal", this.customDropDisplayInternal.ToString());
            info.AddValue("customHeight", this.customHeight);
            info.AddValue("isCut", this.isCut);
            info.AddValue("isExpanded", this.isExpanded);
            info.AddValue("selectable", this.selectable);
            info.AddValue("subItems", this.subItems);
            info.AddValue("useItemStyleForSubItems", this.useItemStyleForSubItems);
            info.AddValue("visible", this.visible);
            base.GetObjectDataInternal(info, context);
        }

        /// <summary>
        ///   Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        protected override void ReadXmlInternal(XmlReader reader) {
            Checks.CheckNotNull(reader, "reader");
            reader.MoveToContent();
            reader.ReadStartElement();
            this.allowSelectChildItems = bool.Parse(reader.ReadElementString("allowSelectChildItems"));
            this.allowShowExpandButton = bool.Parse(reader.ReadElementString("allowShowExpandButton"));
            this.checkBoxAppearance = (BetterListViewCheckBoxAppearance)Enum.Parse(typeof(BetterListViewCheckBoxAppearance), reader.ReadElementString("checkBoxAppearance"));
            this.checkState = (CheckState)Enum.Parse(typeof(CheckState), reader.ReadElementString("checkState"));
            reader.ReadStartElement("childItems");
            this.childItems = (BetterListViewItemCollection)new XmlSerializer(typeof(BetterListViewItemCollection)).Deserialize(reader);
            this.childItems.OwnerItem = this;
            this.childItems.SetInternal();
            reader.ReadEndElement();
            this.customDropDisplayExternal = (BetterListViewDragDropDisplay)Enum.Parse(typeof(BetterListViewDragDropDisplay), reader.ReadElementString("customDropDisplayExternal"));
            this.customDropDisplayInternal = (BetterListViewDragDropDisplay)Enum.Parse(typeof(BetterListViewDragDropDisplay), reader.ReadElementString("customDropDisplayInternal"));
            this.customHeight = int.Parse(reader.ReadElementString("customHeight"));
            this.isCut = bool.Parse(reader.ReadElementString("isCut"));
            this.isExpanded = bool.Parse(reader.ReadElementString("isExpanded"));
            this.selectable = bool.Parse(reader.ReadElementString("selectable"));
            reader.ReadStartElement("subItems");
            this.subItems = (BetterListViewSubItemCollection)new XmlSerializer(typeof(BetterListViewSubItemCollection)).Deserialize(reader);
            this.subItems.OwnerItem = this;
            this.subItems.SetInternal();
            reader.ReadEndElement();
            this.useItemStyleForSubItems = bool.Parse(reader.ReadElementString("useItemStyleForSubItems"));
            this.visible = bool.Parse(reader.ReadElementString("visible"));
            base.ReadXmlInternal(reader);
            reader.ReadEndElement();
        }

        /// <summary>
        ///   Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        protected override void WriteXmlInternal(XmlWriter writer) {
            writer.WriteElementString("allowSelectChildItems", this.allowSelectChildItems.ToString());
            writer.WriteElementString("allowShowExpandButton", this.allowShowExpandButton.ToString());
            writer.WriteElementString("checkBoxAppearance", this.checkBoxAppearance.ToString());
            writer.WriteElementString("checkState", this.checkState.ToString());
            writer.WriteStartElement("childItems");
            new XmlSerializer(typeof(BetterListViewItemCollection)).Serialize(writer, this.childItems);
            writer.WriteEndElement();
            writer.WriteElementString("customDropDisplayExternal", this.customDropDisplayExternal.ToString());
            writer.WriteElementString("customDropDisplayInternal", this.customDropDisplayInternal.ToString());
            writer.WriteElementString("customHeight", this.customHeight.ToString());
            writer.WriteElementString("isCut", this.isCut.ToString());
            writer.WriteElementString("isExpanded", this.isExpanded.ToString());
            writer.WriteElementString("selectable", this.selectable.ToString());
            writer.WriteStartElement("subItems");
            new XmlSerializer(typeof(BetterListViewSubItemCollection)).Serialize(writer, this.subItems);
            writer.WriteEndElement();
            writer.WriteElementString("useItemStyleForSubItems", this.useItemStyleForSubItems.ToString());
            writer.WriteElementString("visible", this.visible.ToString());
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