using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ComponentOwl.BetterListView.Collections;
using ComponentOwl.BetterListView.Design;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Ultimate ListView control replacement.
    /// </summary>
    [Docking(DockingBehavior.Ask)]
    [DefaultEvent("SelectedIndexChanged")]
    [Description("Enhanced custom ListView control")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    [ToolboxBitmap(typeof(BetterListView), "Resources.BetterListView.bmp")]
    [ToolboxItemFilter("System.Windows.Forms")]
    [DefaultProperty("Items")]
    [DefaultBindingProperty("SelectedValue")]
    [Designer(typeof(BetterListViewDesigner))]
    public class BetterListView : BetterListViewBase, IEnumerable<BetterListViewItem>, IEnumerable
    {
        private delegate void AutoResizeColumnDelegate(BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle, int extraPadding, bool userCode);

        /// <summary>
        ///   Windows API calls.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            ///   Identifier for 'Hand' cursor.
            /// </summary>
            public const int IDC_HAND = 32649;

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetCapture();

            [DllImport("user32.dll")]
            public static extern int LoadCursor(int hInstance, int lpCursorName);

            [DllImport("user32.dll")]
            public static extern int SetCursor(int hCursor);
        }

        private delegate void EnsureVisibleDelegate(BetterListViewEnsureVisibleData ensureVisibleData, bool suppressRefresh);

        private const string DefaultGroupHeader = "Default";

        

        private const int WheelDelta = 120;

        /// <summary>
        ///   Default indentation of child items.
        /// </summary>
        internal const int DefaultIndent = -1;

        private const System.Drawing.ContentAlignment DefaultBackgroundImageAlignment = System.Drawing.ContentAlignment.BottomRight;

        private const byte DefaultBackgroundImageOpacity = byte.MaxValue;

        private const BetterListViewCheckBoxes DefaultCheckBoxes = BetterListViewCheckBoxes.Hide;

        private const bool DefaultCheckBoxesAlign = true;

        private const BetterListViewColumnHeaderDisplayMode DefaultColumnHeadersDisplayMode = BetterListViewColumnHeaderDisplayMode.ShowDetails;

        private const bool DefaultFullRowSelect = true;

        private const BetterListViewGridLines DefaultGridLines = BetterListViewGridLines.Vertical;

        private const BetterListViewScrollBarDisplayMode DefaultScrollBarDisplayMode = BetterListViewScrollBarDisplayMode.ShowIfNeeded;

        private const BetterListViewSortedColumnsRowsHighlight DefaultSortedColumnsRowsHighlight = BetterListViewSortedColumnsRowsHighlight.ShowMultiColumnOnly;

        private const BetterListViewView DefaultView = BetterListViewView.Details;

        private const ItemActivation DefaultActivation = ItemActivation.Standard;

        private const bool DefaultAllowAutoScroll = true;

        private const bool DefaultAllowAutoToolTips = true;

        private const bool DefaultAllowAutoToolTipsColumnHeaders = true;

        private const bool DefaultAllowAutoToolTipsGroups = true;

        private const bool DefaultAllowAutoToolTipsSubItems = true;

        private const DragDropEffects DefaultAllowedDragEffects = DragDropEffects.Scroll | DragDropEffects.Move;

        private const bool DefaultAllowDrag = false;

        private const bool DefaultAllowDrop = false;

        private const int DefaultAutoExpandDelay = 1000;

        private const bool DefaultCircularSelection = false;

        private const BetterListViewUnselectionBehavior DefaultCollapsedItemUnselection = BetterListViewUnselectionBehavior.SelectParent;

        private const BetterListViewColumnReorderMode DefaultColumnReorderMode = BetterListViewColumnReorderMode.Disabled;

        private const bool DefaultDragSelectionInversion = false;

        private const BetterListViewGroupHeaderBehavior DefaultGroupHeaderBehavior = BetterListViewGroupHeaderBehavior.All;

        private const bool DefaultGroupItemCheck = true;

        private const BetterListViewHeaderStyle DefaultHeaderStyle = BetterListViewHeaderStyle.Clickable;

        private const bool DefaultHideSelection = true;

        private const BetterListViewHideSelectionMode DefaultHideSelectionMode = BetterListViewHideSelectionMode.Hide;

        private const BetterListViewDragDropDisplay DefaultItemDropDisplayExternal = BetterListViewDragDropDisplay.Highlight;

        private const BetterListViewDragDropDisplay DefaultItemDropDisplayInternal = BetterListViewDragDropDisplay.InsertionMark;

        private const BetterListViewItemReorderMode DefaultItemReorderMode = BetterListViewItemReorderMode.Disabled;

        private const BetterListViewItemReorderOptions DefaultItemReorderOptions = BetterListViewItemReorderOptions.None;

        private const bool DefaultLabelEditDefaultAccept = true;

        private const BetterListViewLabelEditMode DefaultLabelEditModeItems = BetterListViewLabelEditMode.None;

        private const BetterListViewLabelEditMode DefaultLabelEditModeSubItems = BetterListViewLabelEditMode.None;

        private const int DefaultMaximumAutoSizeWidth = 1024;

        private const float DefaultMouseWheelScrollExtent = 2f;

        private const bool DefaultMultiSelect = true;

        private const bool DefaultReadOnly = false;

        private const int DefaultSearchTimeoutDelay = 1000;

        private const bool DefaultShowDefaultGroupHeader = true;

        private const bool DefaultShowEmptyGroups = false;

        private const bool DefaultShowGroups = false;

        private const bool DefaultShowItemExpandButtons = true;

        private const bool DefaultShowToolTips = false;

        private const bool DefaultShowToolTipsColumnHeaders = false;

        private const bool DefaultShowToolTipsGroups = false;

        private const bool DefaultShowToolTipsSubItems = false;

        private const bool DefaultSortVirtual = false;

        private const BetterListViewSubItemFocusBehavior DefaultSubItemFocusBehavior = BetterListViewSubItemFocusBehavior.Auto;

        private const bool DefaultDataBindColumns = false;

        private const bool DefaultDataBindPosition = true;

        private const bool DefaultAutoSizeItemsInDetailsView = false;

        private const int DefaultMaximumToolTipTextLength = 1000;

        private const bool DefaultOptimizedInvalidation = true;

        private const bool DefaultSortOnCollectionChange = true;

        private CurrencyManager currentDataManager;

        private BindingMemberInfo[] currentDisplayMembers = new BindingMemberInfo[1]
        {
        new BindingMemberInfo(string.Empty)
        };

        private bool isSettingDataConnection;

        private bool isDataSourceInitialized;

        private bool isDataSourceInitEventHooked;

        private bool isUpdatingContent;

        private Timer autoExpandTimer;

        private readonly Dictionary<BetterListViewView, ImageList> imageListsItems = new Dictionary<BetterListViewView, ImageList>();

        private bool allowDisplayFocusRectangle;

        private BetterListViewGroup defaultGroup;

        private Timer labelEditTimer;

        private EventHandler ehLabelEditEmbeddedControlOnLeave;

        private EventHandler ehLabelEditEmbeddedControlOnLostFocus;

        private EventHandler ehLabelEditEmbeddedControlOnRequestAccept;

        private EventHandler ehLabelEditEmbeddedControlOnRequestCancel;

        private BetterListViewLayoutColumns layoutColumns;

        private BetterListViewLayoutGroupsHorizontal layoutGroupsHorizontal;

        private BetterListViewLayoutGroupsVertical layoutGroupsVertical;

        private SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutGroups> layoutsGroups = new SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutGroups>();

        private SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutItems> layoutsItems = new SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutItems>();

        private ReadOnlyCollection<BetterListViewColumnHeader> layoutElementsColumns = new ReadOnlyCollection<BetterListViewColumnHeader>(new BetterListViewColumnHeader[0]);

        private ReadOnlyCollection<BetterListViewGroup> layoutElementsGroups = new ReadOnlyCollection<BetterListViewGroup>(new BetterListViewGroup[0]);

        private ReadOnlyCollection<BetterListViewItem> layoutElementsItemsDisplay = new ReadOnlyCollection<BetterListViewItem>(new BetterListViewItem[0]);

        private ReadOnlyCollection<BetterListViewItem> layoutElementsItemsSelection = new ReadOnlyCollection<BetterListViewItem>(new BetterListViewItem[0]);

        private BetterListViewCommonMeasurementItems commonMeasurementItems = BetterListViewCommonMeasurementItems.Empty;

        private ReadOnlyCollection<int> commonMeasurementItemsOffsets = new ReadOnlyCollection<int>(new int[1]);

        private BetterListViewContentMeasurement contentMeasurement;

        private BetterListViewLayoutMeasurement layoutMeasurementColumns;

        private BetterListViewLayoutMeasurement layoutMeasurementGroups;

        private bool enableExpandButtons;

        private Size smallItemSize;

        private BetterListViewLayoutVisibleRange visibleRangeColumns = BetterListViewLayoutVisibleRange.Undefined;

        private BetterListViewLayoutVisibleRange visibleRangeGroups = BetterListViewLayoutVisibleRange.Undefined;

        private BetterListViewLayoutVisibleRange visibleRangeItemsDisplay = BetterListViewLayoutVisibleRange.Undefined;

        private BetterListViewLayoutVisibleRange visibleRangeItemsSelection = BetterListViewLayoutVisibleRange.Undefined;

        /// <summary>
        ///   Auxiliary: Stored hit test to check whether mouse cursor in MouseUp hovers over the same element as in MouseDown event handler.
        /// </summary>
        private BetterListViewHitTestInfo hitTestInfoMouseDown = BetterListViewHitTestInfo.Empty;

        private static readonly Color DefaultColorColumnResizeLine;

        private static readonly Color DefaultColorGridLines;

        private static readonly Color DefaultColorSortedColumn;

        private System.Drawing.ContentAlignment backgroundImageAlignment = System.Drawing.ContentAlignment.BottomRight;

        private byte backgroundImageOpacity = byte.MaxValue;

        private BetterListViewCheckBoxes checkBoxes;

        private bool checkBoxesAlign = true;

        private Color colorColumnResizeLine = BetterListView.DefaultColorColumnResizeLine;

        private Color colorGridLines = BetterListView.DefaultColorGridLines;

        private Color colorInsertionMark = BetterListViewInsertionMark.Empty.Color;

        private Color colorSortedColumn = BetterListView.DefaultColorSortedColumn;

        private BetterListViewColumnHeaderDisplayMode columnsDisplayMode = BetterListViewColumnHeaderDisplayMode.ShowDetails;

        private Font fontColumns;

        private Font fontGroups;

        private Color foreColorColumns = Color.Empty;

        private Color foreColorGroups = Color.Empty;

        private bool fullRowSelect = true;

        private BetterListViewGridLines gridLines = BetterListViewGridLines.Vertical;

        private BetterListViewScrollBarDisplayMode hScrollBarDisplayMode = BetterListViewScrollBarDisplayMode.ShowIfNeeded;

        private int indent = -1;

        private BetterListViewSortedColumnsRowsHighlight sortedColumnsRowsHighlight = BetterListViewSortedColumnsRowsHighlight.ShowMultiColumnOnly;

        private BetterListViewScrollBarDisplayMode vScrollBarDisplayMode = BetterListViewScrollBarDisplayMode.ShowIfNeeded;

        private BetterListViewView view = BetterListViewView.Details;

        private static readonly BetterListViewSearchSettings DefaultSearchSettings;

        private ItemActivation activation;

        private bool allowAutoScroll = true;

        private bool allowAutoToolTips = true;

        private bool allowDrag;

        private bool allowDrop;

        private DragDropEffects allowedDragEffects = DragDropEffects.Scroll | DragDropEffects.Move;

        private int autoExpandDelay = 1000;

        private bool circularSelection;

        private BetterListViewUnselectionBehavior collapsedItemUnselection = BetterListViewUnselectionBehavior.SelectParent;

        private BetterListViewColumnReorderMode columnReorderMode;

        private readonly BetterListViewColumnHeaderCollection columns;

        private ContextMenuStrip contextMenuStrip;

        private bool dragSelectionInversion;

        private BetterListViewGroupHeaderBehavior groupHeaderBehavior = BetterListViewGroupHeaderBehavior.All;

        private bool groupItemCheck = true;

        private readonly BetterListViewGroupCollection groups;

        private BetterListViewHeaderStyle headerStyle = BetterListViewHeaderStyle.Clickable;

        private BetterListViewHideSelectionMode hideSelectionMode;

        private ImageList imageListColumns;

        private ImageList imageListGroups;

        private BetterListViewDragDropDisplay itemDropDisplayExternal = BetterListViewDragDropDisplay.Highlight;

        private BetterListViewDragDropDisplay itemDropDisplayInternal = BetterListViewDragDropDisplay.InsertionMark;

        private BetterListViewItemReorderMode itemReorderMode;

        private BetterListViewItemReorderOptions itemReorderOptions;

        private readonly BetterListViewItemCollection items;

        private BetterListViewLabelEditActivation labelEditActivation;

        private bool labelEditDefaultAccept = true;

        private BetterListViewLabelEditMode labelEditModeItems;

        private BetterListViewLabelEditMode labelEditModeSubItems;

        private int maximumAutoSizeWidth = 1024;

        private float mouseWheelScrollExtent = 2f;

        private bool multiSelect = true;

        private bool readOnly;

        private BetterListViewSearchSettings searchSettings = BetterListView.DefaultSearchSettings;

        private int searchTimeoutDelay = 1000;

        private bool showDefaultGroupHeader = true;

        private bool showEmptyGroups;

        private bool showGroups;

        private bool showItemExpandButtons = true;

        private bool showToolTips;

        private bool showToolTipsColumns;

        private bool showToolTipsGroups;

        private bool showToolTipsSubItems;

        private bool sortVirtual;

        private BetterListViewSubItemFocusBehavior subItemFocusBehavior = BetterListViewSubItemFocusBehavior.Auto;

        private BetterListViewToolTipOptions toolTipOptions = BetterListViewToolTipOptions.Default;

        private Cursor cursor = Cursors.Default;

        private BetterListViewInsertionMark insertionMark = BetterListViewInsertionMark.Empty;

        private BetterListViewItemComparer itemComparer = new BetterListViewItemComparer();

        private BetterListViewSortList sortList = new BetterListViewSortList();

        private bool dataBindColumns;

        private bool dataBindPosition = true;

        private object dataSource;

        private BindingMemberInfo displayMember = new BindingMemberInfo(string.Empty);

        private BindingMemberInfo valueMember = new BindingMemberInfo(string.Empty);

        private bool autoSizeItemsInDetailsView;

        private int maximumToolTipTextLength = 1000;

        private bool optimizedInvalidation = true;

        private bool sortOnCollectionChange = true;

        private string searchString = string.Empty;

        private DateTime searchLastTyping = DateTime.MinValue;

        private BetterListViewSelectedIndexCollection selectedIndices;

        private BetterListViewSelectedItemCollection selectedItems;

        private BetterListViewCheckedIndexCollection checkedIndices;

        private BetterListViewCheckedItemCollection checkedItems;

        private BetterListViewStateInfo stateInfo = BetterListViewStateInfo.Default;

        private BetterListViewSelectionInfo selectionInfo = BetterListViewSelectionInfo.Empty;

        private BetterListViewFocusInfo focusInfo = BetterListViewFocusInfo.Empty;

        private BetterListViewHitTestInfo hitTestInfo = BetterListViewHitTestInfo.Empty;

        private int selectionChangedSuspendLevel;

        private int sortSuspendLevel;

        private readonly Dictionary<BetterListViewPostponedCallType, BetterListViewPostponedCallData> postponedCalls = new Dictionary<BetterListViewPostponedCallType, BetterListViewPostponedCallData>(Enum.GetNames(typeof(BetterListViewPostponedCallType)).Length);

        private ToolTip toolTip;

        private bool AllowDisplayFocusRectangle {
            get {
                return this.allowDisplayFocusRectangle;
            }
            set {
                if (value != this.allowDisplayFocusRectangle) {
                    this.allowDisplayFocusRectangle = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                }
            }
        }

        private bool IsAnyColumnHeaderVisible {
            get {
                if (this.layoutElementsColumns.Count != 0) {
                    return !this.visibleRangeColumns.IsUndefined;
                }
                return false;
            }
        }

        private bool IsAnyItemSelectable {
            get {
                if (this.layoutElementsItemsSelection.Count != 0) {
                    return !this.visibleRangeItemsSelection.IsUndefined;
                }
                return false;
            }
        }

        private bool IsAnyItemVisible {
            get {
                if (this.layoutElementsItemsDisplay.Count != 0) {
                    return !this.visibleRangeItemsDisplay.IsUndefined;
                }
                return false;
            }
        }

        private bool IsAnyGroupVisible {
            get {
                if (this.layoutElementsGroups.Count != 0 && !this.visibleRangeGroups.IsUndefined) {
                    return this.ShowGroups;
                }
                return false;
            }
        }

        private bool AllowKeyboardSearch {
            get {
                if (this.SearchSettings.Mode == BetterListViewSearchMode.Disabled) {
                    return false;
                }
                if ((this.SearchSettings.Options & BetterListViewSearchOptions.CaseSensitive) == BetterListViewSearchOptions.CaseSensitive) {
                    if (Control.ModifierKeys != 0) {
                        return Control.ModifierKeys == Keys.Shift;
                    }
                    return true;
                }
                return Control.ModifierKeys == Keys.None;
            }
        }

        internal Rectangle BoundsColumnHeaders {
            get {
                if (!this.ColumnsVisible || this.layoutElementsColumns.Count == 0) {
                    return Rectangle.Empty;
                }
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                int height = Math.Min(clientRectangleInner.Height, ((IBetterListViewLayoutElementDisplayable)this.layoutElementsColumns[0]).LayoutBounds.BoundsOuter.Height);
                int width = (base.VScrollBarVisible ? (clientRectangleInner.Width - base.VScrollBar.Width) : clientRectangleInner.Width);
                return new Rectangle(clientRectangleInner.Left, clientRectangleInner.Top, width, height);
            }
        }

        /// <summary>
        ///   Gets column header layout elements.
        /// </summary>
        internal ReadOnlyCollection<BetterListViewColumnHeader> LayoutElementsColumns => this.layoutElementsColumns;

        /// <summary>
        ///   Gets group layout elements.
        /// </summary>
        internal ReadOnlyCollection<BetterListViewGroup> LayoutElementsGroups => this.layoutElementsGroups;

        /// <summary>
        ///   Gets item layout elements.
        /// </summary>
        internal ReadOnlyCollection<BetterListViewItem> LayoutElementsItems => this.layoutElementsItemsDisplay;

        internal Point OffsetColumnsFromAbsolute {
            get {
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                return new Point((this.LayoutItemsCurrent.OrientationVertical ? (-this.OffsetColumnsScroll.X) : 0) + clientRectangleInner.Left, clientRectangleInner.Top);
            }
        }

        internal Point OffsetColumnsScroll => new Point(base.ScrollPositionHorizontal, 0);

        internal Point OffsetColumnsToAbsolute {
            get {
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                return new Point((this.LayoutItemsCurrent.OrientationVertical ? this.OffsetColumnsScroll.X : 0) - clientRectangleInner.Left, -clientRectangleInner.Top);
            }
        }

        internal Point OffsetContentFromAbsolute {
            get {
                Point offsetContentScroll = this.OffsetContentScroll;
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                return new Point(-offsetContentScroll.X + clientRectangleInner.Left, this.layoutMeasurementColumns.Height - offsetContentScroll.Y + clientRectangleInner.Top);
            }
        }

        internal Point OffsetContentScroll => base.ScrollPosition;

        internal Point OffsetContentToAbsolute {
            get {
                Point offsetContentScroll = this.OffsetContentScroll;
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                return new Point(offsetContentScroll.X - clientRectangleInner.Left, -this.layoutMeasurementColumns.Height + offsetContentScroll.Y - clientRectangleInner.Top);
            }
        }

        private Size ExtraPadding {
            get {
                if (this.ShowGroups) {
                    BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
                    Size result = new Size(layoutGroupsCurrent.LayoutPadding.Left, layoutGroupsCurrent.LayoutPadding.Top);
                    return result;
                }
                return Size.Empty;
            }
        }

        /// <summary>
        ///   Gets or sets the default action description of the control for use by accessibility client applications.
        /// </summary>
        /// <returns>
        ///   The default action description of the control for use by accessibility client applications.
        /// </returns>
        [DefaultValue("")]
        [Category("Accessibility")]
        public new string AccessibleDefaultActionDescription {
            get {
                return base.AccessibleDefaultActionDescription;
            }
            set {
                base.AccessibleDefaultActionDescription = value;
            }
        }

        /// <summary>
        ///   Gets or sets the description of the control used by accessibility client applications.
        /// </summary>
        /// <returns>
        ///   The description of the control used by accessibility client applications. The default is null.
        /// </returns>
        [DefaultValue("List Control")]
        [Category("Accessibility")]
        public new string AccessibleDescription {
            get {
                return base.AccessibleDescription;
            }
            set {
                base.AccessibleDescription = value;
            }
        }

        /// <summary>
        ///   Gets or sets the name of the control used by accessibility client applications.
        /// </summary>
        /// <returns>
        ///   The name of the control used by accessibility client applications. The default is null.
        /// </returns>
        [Category("Accessibility")]
        [DefaultValue("BetterListView")]
        public new string AccessibleName {
            get {
                return base.AccessibleName;
            }
            set {
                base.AccessibleName = value;
            }
        }

        /// <summary>
        ///   Gets or sets the accessible role of the control
        /// </summary>
        /// <returns>
        ///   One of the values of <see cref="T:System.Windows.Forms.AccessibleRole" />. The default is Default.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
        ///   The value assigned is not one of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values.
        /// </exception>
        [DefaultValue(AccessibleRole.List)]
        [Category("Accessibility")]
        public new AccessibleRole AccessibleRole {
            get {
                return base.AccessibleRole;
            }
            set {
                base.AccessibleRole = value;
            }
        }

        /// <summary>
        ///   Gets or sets background image alignment.
        ///   Works with BackgroundImageLayout.None.
        /// </summary>
        [Description("Background image alignment; works with BackgroundImageLayout.None")]
        [Category("Appearance")]
        public System.Drawing.ContentAlignment BackgroundImageAlignment {
            get {
                return this.backgroundImageAlignment;
            }
            set {
                if (this.backgroundImageAlignment != value) {
                    this.backgroundImageAlignment = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the background image layout as defined in the <see cref="T:System.Windows.Forms.ImageLayout" /> enumeration.
        /// </summary>
        /// <returns>
        ///   One of the values of <see cref="T:System.Windows.Forms.ImageLayout" /> (<see cref="F:System.Windows.Forms.ImageLayout.Center" /> , <see cref="F:System.Windows.Forms.ImageLayout.None" />, <see cref="F:System.Windows.Forms.ImageLayout.Stretch" />, <see cref="F:System.Windows.Forms.ImageLayout.Tile" />, or <see cref="F:System.Windows.Forms.ImageLayout.Zoom" />). <see cref="F:System.Windows.Forms.ImageLayout.Tile" /> is the default value.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The specified enumeration value does not exist. 
        /// </exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        [Category("Appearance")]
        [Description("Gets or sets the background image layout as defined in the System.Windows.Forms.ImageLayout enumeration.")]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                if (base.BackgroundImageLayout != value) {
                    base.BackgroundImageLayout = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets background image opacity.
        /// </summary>
        [Category("Appearance")]
        [Description("Background image opacity")]
        public byte BackgroundImageOpacity {
            get {
                return this.backgroundImageOpacity;
            }
            set {
                if (this.backgroundImageOpacity != value) {
                    this.backgroundImageOpacity = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets display of the check boxes.
        /// </summary>
        [Description("Display of the check boxes")]
        [Category("Appearance")]
        public BetterListViewCheckBoxes CheckBoxes {
            get {
                return this.checkBoxes;
            }
            set {
                if (this.checkBoxes == value) {
                    return;
                }
                this.checkBoxes = value;
                if (value == BetterListViewCheckBoxes.TwoState) {
                    foreach (BetterListViewItem item in this.Items) {
                        if (item.CheckState == CheckState.Indeterminate) {
                            item.SetCheckState(CheckState.Unchecked, BetterListViewCheckStateChangeMode.UserCode, quiet: false);
                        }
                    }
                }
                foreach (BetterListViewItem current2 in this) {
                    ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Keep items with hidden check box aligned the same way as items with visible check box.
        /// </summary>
        [Description("Keep items with hidden check box aligned the same way as items with visible check box")]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool CheckBoxesAlign {
            get {
                return this.checkBoxesAlign;
            }
            set {
                if (this.checkBoxesAlign == value) {
                    return;
                }
                this.checkBoxesAlign = value;
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets color of the column resizing line.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the column resizing line")]
        public Color ColorColumnResizeLine {
            get {
                return this.colorColumnResizeLine;
            }
            set {
                this.colorColumnResizeLine = (value.IsEmpty ? BetterListView.DefaultColorColumnResizeLine : value);
            }
        }

        /// <summary>
        ///   Gets or sets color of the grid lines.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the grid lines")]
        public Color ColorGridLines {
            get {
                return this.colorGridLines;
            }
            set {
                if (!(this.colorGridLines == value)) {
                    this.colorGridLines = (value.IsEmpty ? BetterListView.DefaultColorGridLines : value);
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets color of the insertion mark (column headers).
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the insertion mark (column headers)")]
        public Color ColorInsertionMark {
            get {
                return this.colorInsertionMark;
            }
            set {
                if (!(this.colorInsertionMark == value)) {
                    this.colorInsertionMark = (value.IsEmpty ? BetterListViewInsertionMark.Empty.Color : value);
                }
            }
        }

        /// <summary>
        ///   Gets or sets color of the sorted column background.
        /// </summary>
        [Description("Color of the sorted column background")]
        [Category("Appearance")]
        public Color ColorSortedColumn {
            get {
                return this.colorSortedColumn;
            }
            set {
                if (!(this.colorSortedColumn == value)) {
                    this.colorSortedColumn = (value.IsEmpty ? BetterListView.DefaultColorSortedColumn : value);
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets display mode of the column headers.
        /// </summary>
        internal BetterListViewColumnHeaderDisplayMode ColumnsDisplayMode {
            get {
                return this.columnsDisplayMode;
            }
            set {
                if (this.columnsDisplayMode == value || this.columnsDisplayMode == value) {
                    return;
                }
                this.columnsDisplayMode = value;
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets font of column headers texts.
        /// </summary>
        [Category("Appearance")]
        [Description("Font of column headers texts")]
        public Font FontColumns {
            get {
                if (this.fontColumns != null) {
                    return this.fontColumns;
                }
                return this.Font;
            }
            set {
                if (this.fontColumns != value) {
                    this.fontColumns = value;
                    this.OnFontChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   Gets or sets font of group texts.
        /// </summary>
        [Category("Appearance")]
        [Description("Font of group texts")]
        public Font FontGroups {
            get {
                if (this.fontGroups != null) {
                    return this.fontGroups;
                }
                return BetterListViewGroup.DefaultFont;
            }
            set {
                if (this.fontGroups != value) {
                    this.fontGroups = value;
                    this.OnFontChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   Gets or sets font of item texts.
        /// </summary>
        [Description("Font of item texts")]
        [Category("Appearance")]
        public override Font FontItems {
            get {
                return base.FontItems;
            }
            set {
                base.FontItems = value;
                foreach (BetterListViewItem current in this) {
                    foreach (BetterListViewSubItem subItem in current.SubItems) {
                        subItem.FlushCachedFont();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets foreground color of column headers texts.
        /// </summary>
        [Description("Foreground color of column headers texts")]
        [Category("Appearance")]
        public Color ForeColorColumns {
            get {
                if (!this.foreColorColumns.IsEmpty) {
                    return this.foreColorColumns;
                }
                return this.ForeColor;
            }
            set {
                if (!(this.foreColorColumns == value)) {
                    this.foreColorColumns = value;
                    this.OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   Gets or sets foreground color of group texts.
        /// </summary>
        [Description("Foreground color of group texts")]
        [Category("Appearance")]
        public Color ForeColorGroups {
            get {
                if (!this.foreColorGroups.IsEmpty) {
                    return this.foreColorGroups;
                }
                return BetterListViewGroup.DefaultForeColor;
            }
            set {
                if (!(this.foreColorGroups == value)) {
                    this.foreColorGroups = value;
                    this.OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow selection across all columns (Details view).
        /// </summary>
        [Category("Appearance")]
        [Description("Allow selection across all columns (Details view)")]
        [DefaultValue(true)]
        public bool FullRowSelect {
            get {
                return this.fullRowSelect;
            }
            set {
                if (this.fullRowSelect == value) {
                    return;
                }
                this.fullRowSelect = value;
                if (this.View != BetterListViewView.Details || !this.ColumnsVisible) {
                    return;
                }
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets whether to show grid lines (in details view).
        /// </summary>
        [Description("Show grid lines (Details view)")]
        [Category("Appearance")]
        public BetterListViewGridLines GridLines {
            get {
                return this.gridLines;
            }
            set {
                if (this.gridLines != value) {
                    this.gridLines = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets display of the horizontal scroll bar.
        /// </summary>
        [Category("Appearance")]
        [Description("Display of the horizontal scroll bar")]
        public BetterListViewScrollBarDisplayMode HScrollBarDisplayMode {
            get {
                return this.hScrollBarDisplayMode;
            }
            set {
                if (this.hScrollBarDisplayMode == value) {
                    return;
                }
                this.hScrollBarDisplayMode = value;
                if (this.View == BetterListViewView.Details && this.autoSizeItemsInDetailsView) {
                    foreach (BetterListViewItem current in this) {
                        ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                else {
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureContent, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets indentation of child items (in pixels).
        /// </summary>
        internal int Indent {
            get {
                return this.indent;
            }
            set {
                Checks.CheckTrue(value == -1 || value >= 0, "value == DefaultIndent || value >= 0");
                if (this.indent == value) {
                    return;
                }
                this.indent = value;
                if (this.View != BetterListViewView.Details) {
                    return;
                }
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets display mode of a sorted column.
        ///   Highlight color is specified by ColorSortedColumn property.
        /// </summary>
        [Category("Appearance")]
        [Description("Display mode of a sorted column; highlight color is specified by ColorSortedColumn property")]
        public BetterListViewSortedColumnsRowsHighlight SortedColumnsRowsHighlight {
            get {
                return this.sortedColumnsRowsHighlight;
            }
            set {
                if (this.sortedColumnsRowsHighlight != value) {
                    this.sortedColumnsRowsHighlight = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets size of the tiles shown in Tile view.
        /// </summary>
        [Category("Appearance")]
        [Description("Size of the tiles shown in Tile view")]
        public Size TileSize {
            get {
                BetterListViewLayoutItems betterListViewLayoutItems = this.layoutsItems[BetterListViewViewInternal.Tile];
                return new Size(betterListViewLayoutItems.ItemWidth, betterListViewLayoutItems.ItemHeight);
            }
            set {
                BetterListViewLayoutItems betterListViewLayoutItems = this.layoutsItems[BetterListViewViewInternal.Tile];
                if (betterListViewLayoutItems.ItemWidth != value.Width || betterListViewLayoutItems.ItemHeight != value.Height) {
                    try {
                        base.BeginUpdate();
                        betterListViewLayoutItems.ItemWidth = value.Width;
                        betterListViewLayoutItems.ItemHeight = value.Height;
                    }
                    finally {
                        base.EndUpdate();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets client area ToolTip settings for this control.
        /// </summary>
        internal BetterListViewToolTipInfo ToolTipInfo => BetterListViewToolTipInfo.Empty;

        /// <summary>
        ///   Gets or sets the way of displaying BetterListView items.
        /// </summary>
        [Category("Appearance")]
        [Description("Way of displaying BetterListView items")]
        public BetterListViewView View {
            get {
                return this.view;
            }
            set {
                if (this.view == value) {
                    return;
                }
                BetterListViewView viewOld = this.view;
                this.imageListsItems.TryGetValue(this.view, out var _);
                this.imageListsItems.TryGetValue(value, out var _);
                this.view = value;
                foreach (BetterListViewItem current in this) {
                    current.ClearCache();
                }
                foreach (BetterListViewColumnHeader column in this.Columns) {
                    ((IBetterListViewStateElement)column).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                foreach (BetterListViewItem current2 in this) {
                    ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                foreach (BetterListViewGroup group in this.Groups) {
                    ((IBetterListViewStateElement)group).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                this.AllowDisplayFocusRectangle = false;
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.UserCode);
                this.RefreshView();
                this.OnViewChanged(new BetterListViewViewChangedEventArgs(viewOld, value));
            }
        }

        /// <summary>
        ///   Gets or sets display of the vertical scroll bar.
        /// </summary>
        [Category("Appearance")]
        [Description("Display of the vertical scroll bar")]
        public BetterListViewScrollBarDisplayMode VScrollBarDisplayMode {
            get {
                return this.vScrollBarDisplayMode;
            }
            set {
                if (this.vScrollBarDisplayMode == value) {
                    return;
                }
                this.vScrollBarDisplayMode = value;
                if (this.View == BetterListViewView.Details && this.autoSizeItemsInDetailsView) {
                    foreach (BetterListViewItem current in this) {
                        ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                else {
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureContent, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets item activation mode.
        /// </summary>
        [Description("Item activation mode")]
        [Category("Behavior")]
        public ItemActivation Activation {
            get {
                return this.activation;
            }
            set {
                this.activation = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow automatic scrolling of content when mouse is outside client area on some actions.
        /// </summary>
        [Category("Behavior")]
        [Description("Allow automatic scrolling of content when mouse is outside client area on some actions")]
        [DefaultValue(true)]
        public bool AllowAutoScroll {
            get {
                return this.allowAutoScroll;
            }
            set {
                this.allowAutoScroll = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow automatic ToolTips to be displayed on items.
        /// </summary>
        [Description("Allow automatic ToolTips to be displayed on items")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowAutoToolTips {
            get {
                return this.allowAutoToolTips;
            }
            set {
                this.allowAutoToolTips = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow automatic ToolTips to be displayed on column headers.
        /// </summary>
        internal bool AllowAutoToolTipsColumns => false;

        /// <summary>
        ///   Gets or sets a value indicating whether to allow automatic ToolTips to be displayed on groups.
        /// </summary>
        internal bool AllowAutoToolTipsGroups => false;

        /// <summary>
        ///   Gets or sets a value indicating whether to allow automatic ToolTips to be displayed on sub-items.
        /// </summary>
        internal bool AllowAutoToolTipsSubItems => false;

        /// <summary>
        ///   Gets or sets allowed effects for Drag and Drop operations.
        /// </summary>
        [Editor(typeof(FlagEnumUITypeEditor), typeof(UITypeEditor))]
        [Category("Behavior")]
        [Description("Allowed effects for Drag and Drop operations")]
        public DragDropEffects AllowedDragEffects {
            get {
                return this.allowedDragEffects;
            }
            set {
                this.allowedDragEffects = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow Drag and Drop operation to be initiated.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Allow Drag and Drop operation to be initiated")]
        public bool AllowDrag {
            get {
                return this.allowDrag;
            }
            set {
                this.allowDrag = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow dropping data on the control with Drag and Drop mechanism.
        /// </summary>
        [Description("Allow dropping data on the control with Drag and Drop mechanism")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public override bool AllowDrop {
            get {
                return base.AllowDrop;
            }
            set {
                if (this.allowDrop != value) {
                    this.allowDrop = value;
                    this.RefreshAllowDrop();
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow for sorting with multiple columns using Shift modifier key.
        /// </summary>
        public bool AllowMultiColumnSorting => false;

        /// <summary>
        ///   Gets or sets time period (in milliseconds) before group or item is expanded when dragging data over it.
        /// </summary>
        [Description("Time period (in milliseconds) before group or item is expanded when dragging data over it")]
        [Category("Behavior")]
        [DefaultValue(1000)]
        public int AutoExpandDelay {
            get {
                return this.autoExpandDelay;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                this.autoExpandDelay = value;
            }
        }

        /// <summary>
        ///   Allow jumping to item on another side of the list by moving selection.
        /// </summary>
        [Category("Behavior")]
        [Description("Allow jumping to item on another side of the list by moving selection")]
        [DefaultValue(false)]
        public bool CircularSelection {
            get {
                return this.circularSelection;
            }
            set {
                this.circularSelection = value;
            }
        }

        /// <summary>
        ///   Determines how the collapsed items are unselected.
        /// </summary>
        internal BetterListViewUnselectionBehavior CollapsedItemUnselection => this.collapsedItemUnselection;

        /// <summary>
        ///   Gets or sets column reordering mode.
        /// </summary>
        internal BetterListViewColumnReorderMode ColumnReorderMode => this.columnReorderMode;

        /// <summary>
        ///   Gets collection of column headers.
        /// </summary>
        [Category("Behavior")]
        [MergableProperty(false)]
        [Description("Collection of column headers")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        public BetterListViewColumnHeaderCollection Columns => this.columns;

        /// <summary>
        ///   Gets or sets common ContextMenuStrip.
        /// </summary>
        [Category("Behavior")]
        [Description("Common ContextMenuStrip")]
        [DefaultValue(null)]
        public override ContextMenuStrip ContextMenuStrip {
            get {
                return this.contextMenuStrip;
            }
            set {
                this.contextMenuStrip = value;
            }
        }

        /// <summary>
        ///   Gets or sets ContextMenuStrip available by clicking on the column header.
        /// </summary>
        internal ContextMenuStrip ContextMenuStripColumns => null;

        /// <summary>
        ///   Gets or sets ContextMenuStrip available by clicking the group area.
        /// </summary>
        internal ContextMenuStrip ContextMenuStripGroups => null;

        /// <summary>
        ///   Gets or sets ContextMenuStrip available by clicking the items area.
        /// </summary>
        internal ContextMenuStrip ContextMenuStripItems => null;

        /// <summary>
        ///   Gets or sets a value indicating whether to allow item selection inversion by holding Control key while drag selecting items.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Allow item selection inversion by holding Control key while drag selecting items")]
        public bool DragSelectionInversion {
            get {
                return this.dragSelectionInversion;
            }
            set {
                this.dragSelectionInversion = value;
            }
        }

        /// <summary>
        ///   Extra behavior of group headers when interacting with keyboard and mouse.
        /// </summary>
        [Category("Behavior")]
        [Editor(typeof(FlagEnumUITypeEditor), typeof(UITypeEditor))]
        [Description("Extra behavior of group headers when interacting with keyboard and mouse")]
        [DefaultValue(BetterListViewGroupHeaderBehavior.All)]
        public BetterListViewGroupHeaderBehavior GroupHeaderBehavior {
            get {
                return this.groupHeaderBehavior;
            }
            set {
                this.groupHeaderBehavior = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to check all selected items when a single selected item is checked.
        /// </summary>
        [Description("Check all selected items when a single selected item is checked")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool GroupItemCheck {
            get {
                return this.groupItemCheck;
            }
            set {
                this.groupItemCheck = value;
            }
        }

        /// <summary>
        ///   Gets collection of BetterListView groups.
        /// </summary>
        [Description("Collection of BetterListView groups")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        public BetterListViewGroupCollection Groups => this.groups;

        /// <summary>
        ///   Gets or sets appearance and behavor of column headers.
        /// </summary>
        [DefaultValue(BetterListViewHeaderStyle.Clickable)]
        [Category("Behavior")]
        [Description("Gets or sets appearance and behavor of column headers")]
        public BetterListViewHeaderStyle HeaderStyle {
            get {
                return this.headerStyle;
            }
            set {
                BetterListViewHeaderStyle betterListViewHeaderStyle = this.headerStyle;
                if (betterListViewHeaderStyle != value) {
                    this.headerStyle = value;
                    if (betterListViewHeaderStyle == BetterListViewHeaderStyle.None || value == BetterListViewHeaderStyle.None) {
                        base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    }
                    else {
                        base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    }
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to hide selection when the control loses focus.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Hide selection when the control loses focus")]
        public bool HideSelection {
            get {
                return this.hideSelectionMode == BetterListViewHideSelectionMode.Hide;
            }
            set {
                if (this.HideSelection != value) {
                    this.hideSelectionMode = ((!value) ? BetterListViewHideSelectionMode.Disable : BetterListViewHideSelectionMode.Hide);
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the item selection display mode when control loses focus.
        /// </summary>
        /// <value>
        ///   The item selection display mode when control loses focus.
        /// </value>
        [Category("Behavior")]
        [Description("The item selection display mode when control loses focus")]
        public BetterListViewHideSelectionMode HideSelectionMode {
            get {
                return this.hideSelectionMode;
            }
            set {
                if (this.hideSelectionMode != value) {
                    this.hideSelectionMode = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets images to be displayed on items (common for all views).
        /// </summary>
        [Description("Images to be displayed on items (common for all views)")]
        [Category("Behavior")]
        [DefaultValue(null)]
        public ImageList ImageList {
            get {
                ImageList imageList = null;
                foreach (ImageList value in this.imageListsItems.Values) {
                    if (imageList == null) {
                        imageList = value;
                    }
                    else if (imageList != value) {
                        return null;
                    }
                }
                return imageList;
            }
            set {
                foreach (BetterListViewView value2 in Enum.GetValues(typeof(BetterListViewView))) {
                    this.SetImageListItems(value2, value);
                }
            }
        }

        /// <summary>
        ///   Gets or sets images to be displayed on column headers.
        /// </summary>
        [DefaultValue(null)]
        [Description("Images to be displayed on column headers")]
        [Category("Behavior")]
        public ImageList ImageListColumns {
            get {
                return this.imageListColumns;
            }
            set {
                if (this.imageListColumns != value) {
                    this.imageListColumns = value;
                    if (this.IsAnyColumnHeaderVisible) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                        this.RefreshView();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets images to be displayed on groups.
        /// </summary>
        [Description("Images to be displayed on groups")]
        [Category("Behavior")]
        [DefaultValue(null)]
        public ImageList ImageListGroups {
            get {
                return this.imageListGroups;
            }
            set {
                if (this.imageListGroups != value) {
                    this.imageListGroups = value;
                    if (this.IsAnyGroupVisible) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                        this.RefreshView();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets item Drag and Drop display mode when dragging data from another control.
        /// </summary>
        [Category("Behavior")]
        [Description("Item Drag and Drop display mode when dragging data from another control")]
        public BetterListViewDragDropDisplay ItemDropDisplayExternal {
            get {
                return this.itemDropDisplayExternal;
            }
            set {
                if (value != this.itemDropDisplayExternal) {
                    this.itemDropDisplayExternal = ((value == BetterListViewDragDropDisplay.Default) ? BetterListViewDragDropDisplay.Highlight : value);
                    this.insertionMark = BetterListViewInsertionMark.Empty;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets item Drag and Drop display mode when dragging data within control.
        /// </summary>
        [Description("Item Drag and Drop display mode when dragging data within control")]
        [Category("Behavior")]
        public BetterListViewDragDropDisplay ItemDropDisplayInternal {
            get {
                return this.itemDropDisplayInternal;
            }
            set {
                if (value != this.itemDropDisplayInternal) {
                    this.itemDropDisplayInternal = ((value == BetterListViewDragDropDisplay.Default) ? BetterListViewDragDropDisplay.InsertionMark : value);
                    this.insertionMark = BetterListViewInsertionMark.Empty;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets item reordering mode.
        /// </summary>
        internal BetterListViewItemReorderMode ItemReorderMode => this.itemReorderMode;

        /// <summary>
        ///   Gets or sets item reordering options.
        /// </summary>
        internal BetterListViewItemReorderOptions ItemReorderOptions => this.itemReorderOptions;

        /// <summary>
        ///   Gets collection of BetterListView items.
        /// </summary>
        [MergableProperty(false)]
        [Category("Behavior")]
        [Description("Collection of BetterListView items")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        public BetterListViewItemCollection Items => this.items;

        /// <summary>
        ///   Gets or sets a value indicating whether to enable in-place item label editing.
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        [Description("Enable in-place item label editing")]
        public bool LabelEdit {
            get {
                if (this.labelEditModeItems == BetterListViewLabelEditMode.None) {
                    return this.labelEditModeSubItems != BetterListViewLabelEditMode.None;
                }
                return true;
            }
            set {
                this.LabelEditEnd(forced: true);
                BetterListViewLabelEditMode betterListViewLabelEditMode = (value ? BetterListViewLabelEditMode.Text : BetterListViewLabelEditMode.None);
                if (this.labelEditModeItems != betterListViewLabelEditMode) {
                    this.labelEditModeItems = betterListViewLabelEditMode;
                    if (!value) {
                        this.labelEditModeSubItems = BetterListViewLabelEditMode.None;
                    }
                    base.AccessibilityNotifyClients(AccessibleEvents.StateChange, 0);
                }
            }
        }

        /// <summary>
        ///   Gets or sets label editing activation method.
        /// </summary>
        [Editor(typeof(FlagEnumUITypeEditor), typeof(UITypeEditor))]
        [Category("Behavior")]
        [Description("Label editing activation method")]
        public BetterListViewLabelEditActivation LabelEditActivation {
            get {
                return this.labelEditActivation;
            }
            set {
                this.LabelEditEnd(forced: true);
                this.labelEditActivation = value;
            }
        }

        /// <summary>
        ///   Gets or sets default label editing action when label editing is ended by the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Gets or sets default label editing action when label editing is ended by the control")]
        public bool LabelEditDefaultAccept {
            get {
                return this.labelEditDefaultAccept;
            }
            set {
                this.labelEditDefaultAccept = value;
            }
        }

        /// <summary>
        ///   Gets or sets label editing mode for items.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Category("Behavior")]
        [Description("Label editing mode for items")]
        public BetterListViewLabelEditMode LabelEditModeItems {
            get {
                return this.labelEditModeItems;
            }
            set {
                this.LabelEditEnd(forced: true);
                this.labelEditModeItems = value;
                base.AccessibilityNotifyClients(AccessibleEvents.StateChange, 0);
            }
        }

        /// <summary>
        ///   Gets or sets label editing mode for sub-items.
        /// </summary>
        [Category("Behavior")]
        [Description("Label editing mode for sub-items")]
        public BetterListViewLabelEditMode LabelEditModeSubItems {
            get {
                return this.labelEditModeSubItems;
            }
            set {
                this.LabelEditEnd(forced: true);
                this.labelEditModeSubItems = value;
                base.AccessibilityNotifyClients(AccessibleEvents.StateChange, 0);
            }
        }

        /// <summary>
        ///   Gets or sets maximum allowed automatic size of the column.
        /// </summary>
        [DefaultValue(1024)]
        [Category("Behavior")]
        [Description("Maximum allowed automatic size of the column")]
        public int MaximumAutoSizeWidth {
            get {
                return this.maximumAutoSizeWidth;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                this.maximumAutoSizeWidth = value;
            }
        }

        /// <summary>
        ///   Gets or sets relative number of items to scroll for a single mouse wheel detent.
        /// </summary>
        [Description("Relative number of items to scroll for a single mouse wheel detent")]
        [Category("Behavior")]
        [DefaultValue(2f)]
        public float MouseWheelScrollExtent {
            get {
                return this.mouseWheelScrollExtent;
            }
            set {
                Checks.CheckSingle(value, "value");
                Checks.CheckTrue(value > 0f, "value > 0.0f");
                this.mouseWheelScrollExtent = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to allow multiple items to be selected.
        /// </summary>
        [Description("Allow multiple items to be selected")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool MultiSelect {
            get {
                return this.multiSelect;
            }
            set {
                if (this.multiSelect != value) {
                    this.multiSelect = value;
                    if (!value && this.SelectedItemsSet.Count > 1) {
                        this.SelectedItemsSet = new BetterListViewItem[1] { this.selectedItems[0] };
                    }
                }
            }
        }

        /// <summary>
        ///   The control does not accept keyboard and mouse input.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The control does not accept keyboard and mouse input")]
        public bool ReadOnly {
            get {
                return this.readOnly;
            }
            set {
                if (this.readOnly != value) {
                    this.readOnly = value;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets keyboard searching options.
        /// </summary>
        [Description("Keyboard searching options")]
        [Category("Behavior")]
        public BetterListViewSearchSettings SearchSettings {
            get {
                return this.searchSettings;
            }
            set {
                this.searchSettings = value;
            }
        }

        /// <summary>
        ///   Gets or sets period between searches after the user stopped typing (in milliseconds).
        /// </summary>
        [DefaultValue(1000)]
        [Description("Period between searches after the user stopped typing (in milliseconds)")]
        [Category("Behavior")]
        public int SearchTimeoutDelay {
            get {
                return this.searchTimeoutDelay;
            }
            set {
                Checks.CheckTrue(value >= 0, "value >= 0");
                this.searchTimeoutDelay = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show header of a default group.
        /// </summary>
        [Description("Show header of a default group")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ShowDefaultGroupHeader {
            get {
                return this.showDefaultGroupHeader;
            }
            set {
                if (this.showDefaultGroupHeader == value) {
                    return;
                }
                this.showDefaultGroupHeader = value;
                if (!value && this.FocusInfo.Element == this.defaultGroup) {
                    this.SetFocusInfo(BetterListViewFocusInfo.Empty, BetterListViewSelectionChangeMode.UserCode);
                }
                ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                foreach (BetterListViewGroup group in this.Groups) {
                    ((IBetterListViewStateElement)group).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show all groups (including the groups without items).
        /// </summary>
        [Description("Show all groups (including the groups without items)")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowEmptyGroups {
            get {
                return this.showEmptyGroups;
            }
            set {
                if (this.showEmptyGroups != value) {
                    this.showEmptyGroups = value;
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show expand buttons on groups.
        /// </summary>
        internal bool ShowGroupExpandButtons => false;

        /// <summary>
        ///   Gets or sets a value indicating whether to show groups containing items.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Show groups containing items")]
        public bool ShowGroups {
            get {
                return this.showGroups;
            }
            set {
                if (this.showGroups == value) {
                    return;
                }
                this.showGroups = value;
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show expand buttons on items.
        /// </summary>
        internal bool ShowItemExpandButtons => this.showItemExpandButtons;

        /// <summary>
        ///   Gets or sets a value indicating whether to show ToolTips on items.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Show ToolTips on items")]
        public bool ShowToolTips {
            get {
                return this.showToolTips;
            }
            set {
                this.showToolTips = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether show ToolTips on column headers.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Show ToolTips on column headers")]
        public bool ShowToolTipsColumns {
            get {
                return this.showToolTipsColumns;
            }
            set {
                this.showToolTipsColumns = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show ToolTips on groups.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Show ToolTips on groups")]
        public bool ShowToolTipsGroups {
            get {
                return this.showToolTipsGroups;
            }
            set {
                this.showToolTipsGroups = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show ToolTips on sub-items.
        /// </summary>
        [Category("Behavior")]
        [Description("Show ToolTips on sub-items")]
        [DefaultValue(false)]
        public bool ShowToolTipsSubItems {
            get {
                return this.showToolTipsSubItems;
            }
            set {
                this.showToolTipsSubItems = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show sorted state, but do not physically sort items.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Show sorted state, but do not physically sort items")]
        public bool SortVirtual {
            get {
                return this.sortVirtual;
            }
            set {
                if (this.sortVirtual != value) {
                    this.sortVirtual = value;
                    if (this.sortList.Count != 0 && !value) {
                        this.SortItems(columnClicked: false, sortAlways: false);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets sub-item focus rectangle behavior.
        /// </summary>
        /// <value>
        ///   Sub-item focus rectangle behavior.
        /// </value>
        [Description("Sub-item focus rectangle behavior")]
        [Category("Behavior")]
        public BetterListViewSubItemFocusBehavior SubItemFocusBehavior {
            get {
                return this.subItemFocusBehavior;
            }
            set {
                if (this.subItemFocusBehavior != value) {
                    this.subItemFocusBehavior = value;
                    //if (this.FocusInfo.ColumnIndex > 0 && this.SubItemFocusBehavior switch {
                    //    BetterListViewSubItemFocusBehavior.None => true,
                    //    BetterListViewSubItemFocusBehavior.All => false,
                    //    BetterListViewSubItemFocusBehavior.Auto => !((BetterListViewItem)this.FocusInfo.Element).AllowSelectChildItems,
                    //    _ => throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'."),
                    //})
                    bool ret = false;
                    switch (this.SubItemFocusBehavior) {
                        case BetterListViewSubItemFocusBehavior.None:
                            ret = true;
                            break;
                        case BetterListViewSubItemFocusBehavior.All:
                            ret = false;
                            break;
                        case BetterListViewSubItemFocusBehavior.Auto:
                            ret = !((BetterListViewItem)this.FocusInfo.Element).AllowSelectChildItems;
                            break;
                        default:
                            throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'.");
                            break;
                    }
                    if (this.FocusInfo.ColumnIndex > 0 && ret) {
                        this.SetFocusInfo(new BetterListViewFocusInfo((BetterListViewItem)this.FocusInfo.Element, 0), BetterListViewSelectionChangeMode.PropertyChanged);
                        this.RefreshView();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets ToolTip appearance and behavior options.
        /// </summary>
        [Category("Behavior")]
        [Description("ToolTip appearance and behavior options")]
        public BetterListViewToolTipOptions ToolTipOptions {
            get {
                return this.toolTipOptions;
            }
            set {
                this.toolTipOptions = value;
            }
        }

        /// <summary>
        ///   Gets the last visible column header.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewColumnHeader BottomColumn {
            get {
                if (this.visibleRangeGroups.IsUndefined) {
                    return null;
                }
                return this.layoutElementsColumns[this.visibleRangeColumns.IndexElementLast];
            }
        }

        /// <summary>
        ///   Gets the last visible column header index.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int BottomColumnIndex => this.BottomColumn?.Index ?? (-1);

        /// <summary>
        ///   Gets the last visible group.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewGroup BottomGroup {
            get {
                if (this.visibleRangeGroups.IsUndefined) {
                    return null;
                }
                BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[this.visibleRangeGroups.IndexElementLast];
                if (betterListViewGroup == this.defaultGroup) {
                    return null;
                }
                return betterListViewGroup;
            }
        }

        /// <summary>
        ///   Gets the last visible group index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BottomGroupIndex => this.BottomGroup?.Index ?? (-1);

        /// <summary>
        ///   Gets the last visible item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewItem BottomItem {
            get {
                if (this.visibleRangeItemsDisplay.IsUndefined) {
                    return null;
                }
                return this.layoutElementsItemsDisplay[this.visibleRangeItemsDisplay.IndexElementLast];
            }
        }

        /// <summary>
        ///   Gets the last visible item index.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int BottomItemIndex => this.BottomItem?.Index ?? (-1);

        /// <summary>
        ///   Gets a value indicating whether check boxes are visible.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CheckBoxesVisible => this.CheckBoxes != BetterListViewCheckBoxes.Hide;

        /// <summary>
        ///   Gets indices of checked items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewCheckedIndexCollection CheckedIndices => this.checkedIndices;

        /// <summary>
        ///   Gets collection of checked items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewCheckedItemCollection CheckedItems => this.checkedItems;

        /// <summary>
        ///   Gets or sets a value indicating whether column headers are visible.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool ColumnsVisible {
            get {
                if (this.ColumnsDisplayMode == BetterListViewColumnHeaderDisplayMode.Hide || this.Columns.Count == 0 || (this.ColumnsDisplayMode == BetterListViewColumnHeaderDisplayMode.ShowDetails && this.View != BetterListViewView.Details)) {
                    return false;
                }
                return true;
            }
            set {
                this.ColumnsDisplayMode = (value ? BetterListViewColumnHeaderDisplayMode.ShowDetails : BetterListViewColumnHeaderDisplayMode.Hide);
            }
        }

        /// <summary>
        ///   Gets or sets the cursor that is displayed when the mouse pointer is over the control.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Windows.Forms.Cursor" /> that represents the cursor to display when the mouse pointer is over the control.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor {
            get {
                return base.Cursor;
            }
            set {
                this.cursor = value;
                base.Cursor = value;
            }
        }

        /// <summary>
        ///   Gets or sets currently focused group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewGroup FocusedGroup {
            get {
                if (this.FocusInfo.IsEmpty) {
                    return null;
                }
                return this.FocusInfo.Element as BetterListViewGroup;
            }
            set {
                this.LabelEditEnd(forced: true);
                if (this.FocusedGroup != value) {
                    BetterListViewFocusInfo betterListViewFocusInfo = this.FocusInfo;
                    if (value != null && value.OwnerCollection != null) {
                        BetterListViewBase ownerControl = value.OwnerCollection.OwnerControl;
                        betterListViewFocusInfo = ((ownerControl == null || ownerControl != this) ? BetterListViewFocusInfo.Empty : new BetterListViewFocusInfo(value));
                    }
                    else {
                        betterListViewFocusInfo = BetterListViewFocusInfo.Empty;
                    }
                    this.SetFocusInfo(betterListViewFocusInfo, BetterListViewSelectionChangeMode.UserCode);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets currently focused item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewItem FocusedItem {
            get {
                if (this.FocusInfo.IsEmpty) {
                    return null;
                }
                return this.FocusInfo.Element as BetterListViewItem;
            }
            set {
                this.LabelEditEnd(forced: true);
                if (this.FocusedItem != value) {
                    BetterListViewFocusInfo betterListViewFocusInfo = this.FocusInfo;
                    if (value != null && value.OwnerCollection != null && value.IsSelectable) {
                        BetterListViewBase ownerControl = value.OwnerCollection.OwnerControl;
                        betterListViewFocusInfo = ((ownerControl == null || ownerControl != this) ? BetterListViewFocusInfo.Empty : new BetterListViewFocusInfo(value, 0));
                    }
                    else {
                        betterListViewFocusInfo = BetterListViewFocusInfo.Empty;
                    }
                    this.SetFocusInfo(betterListViewFocusInfo, BetterListViewSelectionChangeMode.UserCode);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets currently focused sub-item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewSubItem FocusedSubItem {
            get {
                if (this.FocusInfo.IsEmpty) {
                    return null;
                }
                return (this.FocusInfo.Element as BetterListViewItem)?.SubItems[this.FocusInfo.ColumnIndex];
            }
            set {
                this.LabelEditEnd(forced: true);
                if (this.FocusedSubItem != value) {
                    BetterListViewFocusInfo betterListViewFocusInfo = this.FocusInfo;
                    if (value != null && value.OwnerCollection != null) {
                        BetterListViewBase ownerControl = value.OwnerCollection.OwnerControl;
                        betterListViewFocusInfo = ((ownerControl == null || ownerControl != this) ? BetterListViewFocusInfo.Empty : new BetterListViewFocusInfo(value.Item, value.Index));
                    }
                    else {
                        betterListViewFocusInfo = BetterListViewFocusInfo.Empty;
                    }
                    this.SetFocusInfo(betterListViewFocusInfo, BetterListViewSelectionChangeMode.UserCode);
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets or sets images to be displayed on items and sub-items for the current view.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ImageList ImageListCurrent {
            get {
                if (this.imageListsItems.TryGetValue(this.View, out var value)) {
                    return value;
                }
                return null;
            }
            set {
                this.SetImageListItems(this.View, value);
            }
        }

        /// <summary>
        ///   Gets or sets the insertion mark.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewInsertionMark InsertionMark {
            get {
                return this.insertionMark;
            }
            set {
                this.LabelEditEnd(forced: true);
                if (this.SetInsertionMark(value)) {
                    this.RefreshView();
                }
            }
        }

        /// <summary>
        ///   Gets a value indicating whether there are any items selected.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAnythingSelected => this.SelectionInfo.SelectedItems.Count != 0;

        /// <summary>
        ///   Gets a value indicating whether raising of SelectionChanged events is suspended.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSelectionChangedSuspended => this.selectionChangedSuspendLevel != 0;

        /// <summary>
        ///   Gets a value indicating whether the control does not re-sort items while updating.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSortSuspended => this.sortSuspendLevel != 0;

        /// <summary>
        ///   Gets or sets comparer for item sorting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewItemComparer ItemComparer {
            get {
                return this.itemComparer;
            }
            set {
                BetterListViewItemComparer objB = value ?? new BetterListViewItemComparer();
                if (this.itemComparer != objB) {
                    this.itemComparer = objB;
                    if (!this.IsSortSuspended) {
                        this.Sort();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets column headers layout.
        /// </summary>
        internal BetterListViewLayoutColumns LayoutColumns => this.layoutColumns;

        /// <summary>
        ///   Gets group layout for the current view.
        /// </summary>
        internal BetterListViewLayoutGroups LayoutGroupsCurrent {
            get {
                if (this.layoutsGroups.TryGetValue(this.ViewInternal, out var value)) {
                    return value;
                }
                return null;
            }
        }

        /// <summary>
        ///   Gets item layout for the current view.
        /// </summary>
        internal BetterListViewLayoutItems LayoutItemsCurrent => this.layoutsItems[this.ViewInternal];

        /// <summary>
        ///   Gets the collection of selected item indices.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewSelectedIndexCollection SelectedIndices => this.selectedIndices;

        /// <summary>
        ///   Gets the collection of selected items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BetterListViewSelectedItemCollection SelectedItems => this.selectedItems;

        /// <summary>
        ///   Gets or sets selected value.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedValue {
            get {
                foreach (BetterListViewItem item in this.SelectedItemsSet) {
                    object value = item.Value;
                    if (value != null) {
                        return value;
                    }
                }
                return null;
            }
            set {
                foreach (BetterListViewItem item in this.Items) {
                    object value2 = item.Value;
                    if (value2 != null && value == value2) {
                        this.SelectedItemsSet = new BetterListViewItem[1] { item };
                        return;
                    }
                }
                this.SelectedItemsSet = new BetterListViewItem[0];
            }
        }

        /// <summary>
        ///   Gets or sets collection of selected values.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ReadOnlyCollection<object> SelectedValues {
            get {
                List<object> list = new List<object>();
                foreach (BetterListViewItem item in this.SelectedItemsSet) {
                    object value = item.Value;
                    if (value != null) {
                        list.Add(value);
                    }
                }
                return list.AsReadOnly();
            }
            set {
                Set<object> set = new Set<object>(value);
                BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet();
                foreach (BetterListViewItem item in this.Items) {
                    object value2 = item.Value;
                    if (value2 != null && set.Contains(value2)) {
                        betterListViewItemSet.Add(item);
                    }
                }
                this.SelectedItemsSet = betterListViewItemSet;
            }
        }

        /// <summary>
        ///   Gets or sets information about column sorting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewSortList SortList {
            get {
                return (BetterListViewSortList)this.sortList.Clone();
            }
            set {
                if (value != null) {
                    this.sortList = (BetterListViewSortList)value.Clone();
                    this.sortList.Update(this.Columns);
                    this.SortItems(columnClicked: false, sortAlways: false);
                    this.RefreshView();
                }
                else {
                    this.Unsort();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the text associated with this control.
        /// </summary>
        /// <returns>
        ///   The text associated with this control.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <summary>
        ///   Gets the first visible column header.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewColumnHeader TopColumn {
            get {
                if (this.visibleRangeColumns.IsUndefined) {
                    return null;
                }
                return this.layoutElementsColumns[this.visibleRangeColumns.IndexElementFirst];
            }
        }

        /// <summary>
        ///   Gets the first visible column header index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopColumnIndex => this.TopColumn?.Index ?? (-1);

        /// <summary>
        ///   Gets the first visible group.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewGroup TopGroup {
            get {
                if (this.visibleRangeGroups.IsUndefined) {
                    return null;
                }
                BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[this.visibleRangeGroups.IndexElementFirst];
                if (betterListViewGroup == this.defaultGroup) {
                    return null;
                }
                return betterListViewGroup;
            }
        }

        /// <summary>
        ///   Gets the first visible group index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopGroupIndex => this.TopGroup?.Index ?? (-1);

        /// <summary>
        ///   Gets the first visible item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BetterListViewItem TopItem {
            get {
                if (this.visibleRangeItemsDisplay.IsUndefined) {
                    return null;
                }
                return this.layoutElementsItemsDisplay[this.visibleRangeItemsDisplay.IndexElementFirst];
            }
        }

        /// <summary>
        ///   Gets the first visible item index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopItemIndex => this.TopItem?.Index ?? (-1);

        /// <summary>
        ///   Gets or sets currently selected items.
        /// </summary>
        internal ICollection<BetterListViewItem> SelectedItemsSet {
            get {
                return this.SelectionInfo.SelectedItems.Keys;
            }
            set {
                Checks.CheckNotNull(value, "value");
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                selectionInfoNew.SetSelectedItems(this.GetSelectedGroups(value), value);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.UserCode);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Gets the view corresponding to a specific layout (depending on current settings).
        /// </summary>
        protected internal BetterListViewViewInternal ViewInternal {
            get {
                switch (this.view) {
                    case BetterListViewView.Details:
                        if (!this.ColumnsVisible) {
                            return BetterListViewViewInternal.Details;
                        }
                        return BetterListViewViewInternal.DetailsColumns;
                    case BetterListViewView.SmallIcon:
                        return BetterListViewViewInternal.SmallIcon;
                    case BetterListViewView.LargeIcon:
                        return BetterListViewViewInternal.LargeIcon;
                    case BetterListViewView.List:
                        return BetterListViewViewInternal.List;
                    case BetterListViewView.Tile:
                        return BetterListViewViewInternal.Tile;
                    default:
                        throw new ApplicationException($"Unknown view: '{this.view}'.");
                }
            }
        }

        /// <summary>
        ///   Gets the default size of the control.
        /// </summary>
        /// <returns>
        ///   The default <see cref="T:System.Drawing.Size" /> of the control.
        /// </returns>
        protected override Size DefaultSize => new Size(256, 256);

        /// <summary>
        ///   Gets or sets a value indicating whether to synchronize columns with the bound data structure.
        /// </summary>
        [DefaultValue(false)]
        [Description("Synchronize columns with the bound data structure")]
        [Category("Data")]
        public bool DataBindColumns {
            get {
                return this.dataBindColumns;
            }
            set {
                if (this.dataBindColumns != value) {
                    this.dataBindColumns = value;
                    if (this.dataSource != null && value) {
                        this.DataSetDataConnection(this.dataSource, this.displayMember, force: true);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to synchronize selected item with the current position in bound data structure.
        /// </summary>
        [Category("Data")]
        [Description("Synchronize selected item with the current position in bound data structure")]
        [DefaultValue(true)]
        public bool DataBindPosition {
            get {
                return this.dataBindPosition;
            }
            set {
                if (this.dataBindPosition != value) {
                    this.dataBindPosition = value;
                    if (value && this.currentDataManager != null) {
                        this.DataUpdatePositionFromSource();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets data source for this control.
        /// </summary>
        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Description("Data source for this control")]
        [Category("Data")]
        [DefaultValue(null)]
        public object DataSource {
            get {
                return this.dataSource;
            }
            set {
                if (value != null) {
                    Checks.CheckType(value, new Type[2]
                    {
                    typeof(IList),
                    typeof(IListSource)
                    }, "value");
                }
                if (this.dataSource == value) {
                    return;
                }
                try {
                    this.DataSetDataConnection(value, this.displayMember, force: false);
                    if (value == null) {
                        this.DisplayMember = string.Empty;
                    }
                }
                catch {
                    this.DisplayMember = string.Empty;
                }
            }
        }

        /// <summary>
        ///   Gets or sets property to display on list items.
        /// </summary>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Property to display on list items")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string DisplayMember {
            get {
                return this.displayMember.BindingMember ?? string.Empty;
            }
            set {
                this.DataSetDataConnection(this.dataSource, new BindingMemberInfo(value), force: false);
            }
        }

        /// <summary>
        ///   Gets or sets property to use as a list item value.
        /// </summary>
        [Description("Property to use as a list item value")]
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DefaultValue("")]
        public string ValueMember {
            get {
                return this.valueMember.BindingMember ?? string.Empty;
            }
            set {
                this.valueMember = new BindingMemberInfo(value ?? string.Empty);
                this.DataSetDataConnection(this.dataSource, new BindingMemberInfo(value), force: false);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically resize items to client width in Details view without
        ///     columns.
        /// </summary>
        [DefaultValue(false)]
        [Description("Automatically resize items to client width in Details view without columns")]
        [Category("Performance")]
        public bool AutoSizeItemsInDetailsView {
            get {
                return this.autoSizeItemsInDetailsView;
            }
            set {
                if (this.autoSizeItemsInDetailsView == value) {
                    return;
                }
                this.autoSizeItemsInDetailsView = value;
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///     cache resized images for faster redrawing of the control
        /// </summary>
        [Description("Cache resized images for faster redrawing of the control")]
        [DefaultValue(true)]
        [Category("Performance")]
        public override bool CacheImages {
            get {
                return base.CacheImages;
            }
            set {
                base.CacheImages = value;
                if (value) {
                    return;
                }
                foreach (BetterListViewItem current in this) {
                    current.ClearCache();
                }
            }
        }

        /// <summary>
        ///     maximum allowed text length to be shown in automatic tooltip
        /// </summary>
        [Description("Maximum allowed text length to be shown in automatic tooltip")]
        [Category("Performance")]
        [DefaultValue(1000)]
        public int MaximumToolTipTextLength {
            get {
                return this.maximumToolTipTextLength;
            }
            set {
                Checks.CheckTrue(value > 1, "value > 1");
                this.maximumToolTipTextLength = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to invalidate only regions of client area where changes were made.
        /// </summary>
        [Category("Performance")]
        [Description("Invalidate only regions of client area where changes were made")]
        [DefaultValue(true)]
        public bool OptimizedInvalidation {
            get {
                return this.optimizedInvalidation;
            }
            set {
                this.optimizedInvalidation = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically sort items when collection is changed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Automatically sort items when collection is changed")]
        [Category("Performance")]
        public bool SortOnCollectionChange {
            get {
                return this.sortOnCollectionChange;
            }
            set {
                this.sortOnCollectionChange = value;
            }
        }

        private Rectangle DragSelectionRectangle {
            get {
                BetterListViewItemSelectionStateInfo itemSelectionStateInfo = this.StateInfo.ItemSelectionStateInfo;
                Point point = base.PointToClient(Control.MousePosition);
                Rectangle boundsContent = base.BoundsContent;
                Point offsetContentScroll = this.OffsetContentScroll;
                int num = Math.Min(Math.Max(point.X, boundsContent.Left), boundsContent.Right - 1);
                int num2 = Math.Min(Math.Max(point.Y, boundsContent.Top), boundsContent.Bottom - 1);
                int num3 = itemSelectionStateInfo.StartPoint.X + (itemSelectionStateInfo.ScrollPosition.X - offsetContentScroll.X);
                int num4 = itemSelectionStateInfo.StartPoint.Y + (itemSelectionStateInfo.ScrollPosition.Y - offsetContentScroll.Y);
                return new Rectangle(Math.Min(num, num3), Math.Min(num2, num4), Math.Abs(num - num3) + 1, Math.Abs(num2 - num4) + 1);
            }
        }

        internal BetterListViewStateInfo StateInfo => this.stateInfo;

        internal BetterListViewSelectionInfo SelectionInfo => this.selectionInfo;

        internal BetterListViewFocusInfo FocusInfo => this.focusInfo;

        internal BetterListViewHitTestInfo HitTestInfo => this.hitTestInfo;

        /// <summary>
        ///   Occurs before column header is clicked.
        /// </summary>
        [Description("Occurs before a column header is clicked")]
        [Category("Action")]
        public event BetterListViewColumnClickEventHandler ColumnClick;

        /// <summary>
        ///   Occurs after a column is clicked.
        /// </summary>
        [Category("Action")]
        [Description("Occurs after a column is clicked")]
        public event BetterListViewColumnClickedEventHandler ColumnClicked;

        /// <summary>
        ///   Occurs when HitTest result has changed.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when HitTest result has changed")]
        public event BetterListViewHitTestChangedEventHandler HitTestChanged;

        /// <summary>
        ///   Occurs when an item has been activated.
        /// </summary>
        [Description("Occurs when an item has been activated")]
        [Category("Action")]
        public event BetterListViewItemActivateEventHandler ItemActivate;

        /// <summary>
        ///   Occurs before items are checked.
        /// </summary>
        [Category("Action")]
        [Description("Occurs before items are checked")]
        public event BetterListViewItemCheckEventHandler ItemCheck;

        /// <summary>
        ///   Occurs when an item check state has changed.
        /// </summary>
        [Description("Occurs when an item check state has changed")]
        [Category("Action")]
        public event BetterListViewItemCheckedEventHandler ItemChecked;

        /// <summary>
        ///   Occurs when item searching has been performed.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when item searching has been performed")]
        public event BetterListViewItemSearchEventHandler ItemSearch;

        /// <summary>
        ///   Occurs when a custom editing control is requested.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a custom editing control is requested")]
        public event BetterListViewRequestEmbeddedControlEventHandler RequestEmbeddedControl;

        /// <summary>
        ///   Occurs when background is drawn.
        /// </summary>
        [Description("Occurs when background is drawn")]
        [Category("Appearance")]
        public event BetterListViewDrawBackgroundEventHandler DrawBackground;

        /// <summary>
        ///   Occurs when column header is drawn.
        /// </summary>
        [Description("Occurs when column header is drawn")]
        [Category("Appearance")]
        public event BetterListViewDrawColumnHeaderEventHandler DrawColumnHeader;

        /// <summary>
        ///   Occurs when column header background is drawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when column header background is drawn")]
        public event BetterListViewDrawColumnHeaderBackgroundEventHandler DrawColumnHeaderBackground;

        /// <summary>
        ///   Occurs when group is drawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when group is drawn")]
        public event BetterListViewDrawGroupEventHandler DrawGroup;

        /// <summary>
        ///   Occurs when group background is drawn.
        /// </summary>
        [Description("Occurs when group background is drawn")]
        [Category("Appearance")]
        public event BetterListViewDrawGroupBackgroundEventHandler DrawGroupBackground;

        /// <summary>
        ///   Occurs when insertion mark is drawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when insertion mark is drawn")]
        public event BetterListViewDrawInsertionMarkEventHandler DrawInsertionMark;

        /// <summary>
        ///   Occurs when item is drawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when item is drawn")]
        public event BetterListViewDrawItemEventHandler DrawItem;

        /// <summary>
        ///   Occurs when item background is drawn.
        /// </summary>
        [Description("Occurs when item background is drawn")]
        [Category("Appearance")]
        public event BetterListViewDrawItemBackgroundEventHandler DrawItemBackground;

        /// <summary>
        ///   Occurs when items has been sorted.
        /// </summary>
        [Description("Occurs when items has been sorted")]
        [Category("Behavior")]
        public event BetterListViewAfterItemSortEventHandler AfterItemSort;

        /// <summary>
        ///   Occurs after item/sub-item label is edited by the user.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the label for an item is edited by the user")]
        public event BetterListViewLabelEditEventHandler AfterLabelEdit;

        /// <summary>
        ///   Occurs after item/sub-item label is edited by the user; label edit still can be cancelled.
        /// </summary>
        [Description("Occurs when the label for an item is edited by the user; label edit still can be cancelled")]
        [Category("Behavior")]
        public event BetterListViewLabelEditCancelEventHandler AfterLabelEditCancel;

        /// <summary>
        ///   Occurs before items are sorted.
        /// </summary>
        [Description("Occurs before items are sorted")]
        [Category("Behavior")]
        public event BetterListViewBeforeItemSortEventHandler BeforeItemSort;

        /// <summary>
        ///   Occurs when the user starts editing the label of an item.
        /// </summary>
        [Description("Occurs when the user starts editing the label of an item")]
        [Category("Behavior")]
        public event BetterListViewLabelEditCancelEventHandler BeforeLabelEdit;

        /// <summary>
        ///   Occurs when the CheckedItems collection changes.
        /// </summary>
        [Description("Occurs when the CheckedItems collection changes")]
        [Category("Behavior")]
        public event BetterListViewCheckedItemsChangedEventHandler CheckedItemsChanged;

        /// <summary>
        ///   Occurs when selection focused element has been changed.
        /// </summary>
        [Description("Occurs when selection focused element has been changed")]
        [Category("Behavior")]
        public event BetterListViewFocusedItemChangedEventHandler FocusedItemChanged;

        /// <summary>
        ///   Occurs when selection state of an item changes.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when selection state of an item change")]
        public event BetterListViewItemSelectionChangedEventHandler ItemSelectionChanged;

        /// <summary>
        ///   Occurs when selected item index has changed.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when selected item index has changed")]
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        ///   Occurs when SelectedItems collection changes.
        /// </summary>
        [Description("Occurs when Indices collection changes")]
        [Category("Behavior")]
        public event BetterListViewSelectedItemsChangedEventHandler SelectedItemsChanged;

        /// <summary>
        ///   Occurs when DataSource property changes.
        /// </summary>
        [Category("Data")]
        [Description("Occurs when DataSource property changes")]
        public event EventHandler DataSourceChanged;

        /// <summary>
        ///   Occurs when DisplayMember property changed.
        /// </summary>
        [Category("Data")]
        [Description("Occurs when DisplayMember property changed")]
        public event EventHandler DisplayMemberChanged;

        /// <summary>
        ///   Occurs before Drag and Drop operation is initiated.
        /// </summary>
        [Description("Occurs before Drag and Drop operation is initiated")]
        [Category("Drag Drop")]
        public event BetterListViewBeforeDragEventHandler BeforeDrag;

        /// <summary>
        ///   Occurs when Drag and Drop effect is being set.
        /// </summary>
        [Category("Drag Drop")]
        [Description("Occurs when Drag and Drop effect is being set")]
        public event BetterListViewDragDropEffectSettingEventHandler DragDropEffectSetting;

        /// <summary>
        ///   Occurs when an exception is thrown during Drag and Drop operation.
        /// </summary>
        [Description("Occurs when an exception is thrown during Drag and Drop operation")]
        [Category("Drag Drop")]
        public event BetterListViewDragDropExceptionEventHandler DragDropException;

        /// <summary>
        ///   Occurs when an item is being dragged.
        /// </summary>
        [Category("Drag Drop")]
        [Description("Occurs when an item is being dragged")]
        public event BetterListViewItemDragEventHandler ItemDrag;

        /// <summary>
        ///   Occurs when an item is dropped on the control.
        /// </summary>
        [Category("Drag Drop")]
        [Description("Occurs when an item is dropped on the control")]
        public event BetterListViewItemDropEventHandler ItemDrop;

        /// <summary>
        ///   Occurs when mouse hovers over an item.
        /// </summary>
        [Category("Mouse")]
        [Description("Occurs when mouse hovers over an item")]
        public event BetterListViewItemMouseHoverEventHandler ItemMouseHover;

        /// <summary>
        ///   Occurs when column header width has been changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when column header width has been changed")]
        public event BetterListViewColumnWidthChangedEventHandler ColumnWidthChanged;

        /// <summary>
        ///   Occurs when column header width is being changed.
        /// </summary>
        [Description("Occurs when column header width is being changed")]
        [Category("Property Changed")]
        public event BetterListViewColumnWidthChangingEventHandler ColumnWidthChanging;

        /// <summary>
        ///   Occurs when View property value has been changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when View property value has been changed")]
        public event BetterListViewViewChangedEventHandler ViewChanged;

        /// <summary>
        ///   Initialize accessibility support.
        /// </summary>
        protected override void AccessibilityInitialize() {
            base.AccessibilityInitialize();
            this.AccessibleDefaultActionDescription = "";
            this.AccessibleDescription = "List Control";
            this.AccessibleName = "BetterListView";
            this.AccessibleRole = AccessibleRole.List;
        }

        /// <summary>
        ///   Creates a new accessibility object for the control.
        /// </summary>
        /// <returns>
        ///   A new <see cref="T:System.Windows.Forms.AccessibleObject" /> for the control.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new BetterListViewAccessibleObject(this);
        }

        /// <summary>
        ///   Retrieves the specified <see cref="T:System.Windows.Forms.AccessibleObject" />.
        /// </summary>
        /// <param name="objectId">An Int32 that identifies the <see cref="T:System.Windows.Forms.AccessibleObject" /> to retrieve.</param>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" />.
        /// </returns>
        protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
            return base.AccessibilityObject.GetChild(objectId);
        }

        /// <summary>
        ///   Switch order of the specified columns.
        /// </summary>
        /// <param name="indexSource">Index of the source column.</param>
        /// <param name="indexTarget">Index of the target column.</param>
        public void ReorderColumns(int indexSource, int indexTarget) {
            this.ReorderColumns(indexSource, indexTarget, BetterListViewColumnReorderOptions.ReorderItems);
        }

        /// <summary>
        ///   Switch order of the specified columns.
        /// </summary>
        /// <param name="indexSource">Index of the source column.</param>
        /// <param name="indexTarget">Index of the target column.</param>
        /// <param name="columnReorderOptions">Column header reordering options.</param>
        public void ReorderColumns(int indexSource, int indexTarget, BetterListViewColumnReorderOptions columnReorderOptions) {
            bool flag = (columnReorderOptions & BetterListViewColumnReorderOptions.VisibleIndices) == BetterListViewColumnReorderOptions.VisibleIndices;
            bool flag2 = (columnReorderOptions & BetterListViewColumnReorderOptions.ReorderItems) == BetterListViewColumnReorderOptions.ReorderItems;
            Checks.CheckFalse(this.DataBindColumns, "DataBindColumns", "Cannot reorder bound column headers.");
            Checks.CheckTrue(this.Columns.Count != 0, "Columns.Count != 0");
            if (flag) {
                Checks.CheckTrue(this.layoutElementsColumns.Count != 0, "this.layoutElementsColumns.Count != 0");
                Checks.CheckBounds(indexSource, 0, this.layoutElementsColumns.Count - 1, "indexSource");
                Checks.CheckBounds(indexTarget, 0, this.layoutElementsColumns.Count, "indexTarget");
                indexSource = this.layoutElementsColumns[indexSource].Index;
                indexTarget = ((indexTarget == this.layoutElementsColumns.Count - 1) ? this.Columns.Count : this.layoutElementsColumns[indexTarget].Index);
            }
            Checks.CheckTrue(this.Columns.Count != 0, "Columns.Count != 0");
            Checks.CheckBounds(indexSource, 0, this.Columns.Count - 1, "indexSource");
            Checks.CheckBounds(indexTarget, 0, this.Columns.Count, "indexTarget");
            if (indexSource == indexTarget || indexSource == indexTarget - 1) {
                return;
            }
            int num = Math.Max(indexSource, indexTarget) + 1;
            int index = ((indexTarget >= indexSource) ? (indexTarget - 1) : indexTarget);
            try {
                base.BeginUpdate();
                List<KeyValuePair<BetterListViewColumnHeader, bool>> list = null;
                if (flag2) {
                    list = new List<KeyValuePair<BetterListViewColumnHeader, bool>>();
                    foreach (BetterListViewSortInfo sort in this.sortList) {
                        list.Add(new KeyValuePair<BetterListViewColumnHeader, bool>(this.Columns[sort.ColumnIndex], sort.OrderAscending));
                    }
                }
                BetterListViewColumnHeader item = this.Columns[indexSource];
                this.Columns.RemoveAt(indexSource);
                this.Columns.Insert(index, item);
                if (!flag2) {
                    return;
                }
                if (this.dataSource != null) {
                    this.DataSetDataConnection(this.dataSource, this.displayMember, force: true);
                }
                else {
                    foreach (BetterListViewItem item3 in this.Items) {
                        List<BetterListViewItem> childItems = new List<BetterListViewItem>(new BetterListViewItem[1] { item3 });
                        item3.GetAllChildItems(includeCollapsed: true, ref childItems);
                        foreach (BetterListViewItem item4 in childItems) {
                            while (item4.SubItems.Count < num) {
                                item4.SubItems.Add(new BetterListViewSubItem());
                            }
                            BetterListViewSubItem item2 = item4.SubItems[indexSource];
                            item4.SubItems.RemoveAt(indexSource);
                            item4.SubItems.Insert(index, item2);
                        }
                    }
                }
                List<BetterListViewSortInfo> list2 = new List<BetterListViewSortInfo>();
                foreach (KeyValuePair<BetterListViewColumnHeader, bool> item5 in list) {
                    BetterListViewColumnHeader key = item5.Key;
                    bool value = item5.Value;
                    list2.Add(new BetterListViewSortInfo(key.Index, value));
                }
                this.sortList = new BetterListViewSortList(list2);
            }
            finally {
                base.EndUpdate();
            }
        }

        protected internal virtual void DoDefaultAction(BetterListViewColumnHeader columnHeader) {
            Checks.CheckNotNull(columnHeader, "columnHeader");
            Checks.CheckTrue(columnHeader.ListView == this, "ReferenceEquals(columnHeader.ListView, this)");
            try {
                base.BeginUpdate();
                BetterListViewColumnClickEventArgs betterListViewColumnClickEventArgs = new BetterListViewColumnClickEventArgs(this.HitTestInfo.ColumnHeader);
                this.OnColumnClick(betterListViewColumnClickEventArgs);
                if (betterListViewColumnClickEventArgs.Cancel) {
                    return;
                }
                BetterListViewColumnHeaderStyle style = this.HitTestInfo.ColumnHeader.GetStyle(this);
                if (style == BetterListViewColumnHeaderStyle.Sortable || style == BetterListViewColumnHeaderStyle.Unsortable) {
                    BetterListViewSortOptions betterListViewSortOptions = BetterListViewSortOptions.ColumnClicked;
                    bool flag = style == BetterListViewColumnHeaderStyle.Unsortable && this.sortList.Contains(this.HitTestInfo.ColumnHeader.Index) && this.sortList.GetSortOrder(this.HitTestInfo.ColumnHeader.Index) == BetterListViewSortOrder.Descending;
                    bool flag2 = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                    bool flag3 = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                    bool flag4;
                    if (this.AllowMultiColumnSorting && (flag2 || flag3)) {
                        betterListViewSortOptions = ((!flag2) ? (betterListViewSortOptions | ((!flag) ? BetterListViewSortOptions.AddColumn : BetterListViewSortOptions.RemoveColumn)) : (betterListViewSortOptions | ((!this.sortList.Contains(this.HitTestInfo.ColumnHeader.Index)) ? BetterListViewSortOptions.AddColumn : BetterListViewSortOptions.RemoveColumn)));
                        flag4 = true;
                    }
                    else if (flag) {
                        this.Unsort();
                        flag4 = false;
                    }
                    else {
                        flag4 = true;
                    }
                    if (flag4) {
                        if (this.sortList.Contains(this.HitTestInfo.ColumnHeader.Index)) {
                            this.SortItems(this.HitTestInfo.ColumnHeader.Index, betterListViewSortOptions);
                        }
                        else {
                            this.SortItems(this.HitTestInfo.ColumnHeader.Index, betterListViewSortOptions, this.HitTestInfo.ColumnHeader.PreferredSortOrderAscending);
                        }
                    }
                }
                this.OnColumnClicked(new BetterListViewColumnClickedEventArgs(columnHeader));
            }
            finally {
                base.EndUpdate(suppressRefresh: true);
            }
        }

        /// <summary>
        ///   Set new data source.
        /// </summary>
        internal void DataSetDataConnection() {
            this.DataSetDataConnection(this.dataSource, this.currentDisplayMembers[0], force: false);
        }

        /// <summary>
        ///   Get value from the bound data at the specified row and column.
        /// </summary>
        /// <param name="rowIndex"> Table row index. </param>
        /// <param name="columnIndex"> Table column index. </param>
        /// <returns> Value from the bound data at the specified row and column. </returns>
        internal object DataGetValue(int rowIndex, int columnIndex) {
            Checks.CheckTrue(this.Items.Count != 0, "Items.Count != 0");
            Checks.CheckBounds(rowIndex, 0, this.Items.Count - 1, "rowIndex");
            if (this.Columns.Count != 0) {
                Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            }
            else {
                Checks.CheckEqual(columnIndex, 0, "columnIndex", "0");
            }
            if (this.currentDataManager == null) {
                return null;
            }
            string text = ((columnIndex < this.Columns.Count && !string.IsNullOrEmpty(this.Columns[columnIndex].ValueMember)) ? this.Columns[columnIndex].ValueMember : ((columnIndex != 0 || string.IsNullOrEmpty(this.ValueMember)) ? null : this.ValueMember));
            object obj = this.currentDataManager.List[rowIndex];
            if (string.IsNullOrEmpty(text)) {
                if (columnIndex != 0) {
                    return null;
                }
                return obj;
            }
            return this.currentDataManager.GetItemProperties().Find(text, ignoreCase: false)?.GetValue(obj);
        }

        /// <summary>
        ///   Set value to the bound data at the specified row and column.
        /// </summary>
        /// <param name="rowIndex"> Table row index. </param>
        /// <param name="columnIndex"> Table column index. </param>
        /// <param name="value"> Value to set. </param>
        /// <returns> New value converted to sub-item text. </returns>
        internal string DataSetValue(int rowIndex, int columnIndex, object value) {
            Checks.CheckTrue(this.Items.Count != 0, "Items.Count != 0");
            Checks.CheckBounds(rowIndex, 0, this.Items.Count - 1, "rowIndex");
            if (this.Columns.Count != 0) {
                Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            }
            else {
                Checks.CheckEqual(columnIndex, 0, "columnIndex", "0");
            }
            if (this.currentDataManager == null) {
                throw new InvalidOperationException("Cannot set value. There is no data bound to the control.");
            }
            object obj = this.currentDataManager.List[rowIndex];
            PropertyDescriptorCollection itemProperties = this.currentDataManager.GetItemProperties();
            BindingMemberInfo bindingMemberInfo = this.currentDisplayMembers[columnIndex];
            PropertyDescriptor propertyDescriptor = ((!string.IsNullOrEmpty(bindingMemberInfo.BindingField)) ? itemProperties.Find(bindingMemberInfo.BindingField, ignoreCase: false) : null);
            if (propertyDescriptor == null) {
                throw new InvalidOperationException("Cannot set value. Unable to find property '" + bindingMemberInfo.BindingField + "'.");
            }
            propertyDescriptor.SetValue(obj, value);
            return this.DataGetSubItemText(obj, columnIndex, propertyDescriptor);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.BindingContextChanged" /> event.
        /// </summary>
        /// <param name="e"> An <see cref="T:System.EventArgs" /> that contains the event data. </param>
        protected override void OnBindingContextChanged(EventArgs e) {
            this.DataSetDataConnection(this.dataSource, this.displayMember, force: true);
            base.OnBindingContextChanged(e);
        }

        private void DataSetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force) {
            if (this.isSettingDataConnection) {
                return;
            }
            bool flag = this.dataSource != newDataSource;
            bool flag2 = false;
            BindingMemberInfo[] displayMembers = this.GetDisplayMembers(newDisplayMember);
            if (this.currentDisplayMembers.Length != displayMembers.Length) {
                flag2 = true;
            }
            else {
                for (int i = 0; i < displayMembers.Length; i++) {
                    if (this.currentDisplayMembers[i] != displayMembers[i]) {
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2) {
                this.currentDisplayMembers = displayMembers;
            }
            try {
                if (force || flag || flag2) {
                    this.isSettingDataConnection = true;
                    IList list = ((this.currentDataManager != null) ? this.currentDataManager.List : null);
                    bool flag3 = this.currentDataManager == null;
                    this.UnwireDataSource();
                    this.dataSource = newDataSource;
                    this.displayMember = newDisplayMember;
                    this.WireDataSource();
                    if (this.isDataSourceInitialized) {
                        CurrencyManager objB = ((newDataSource != null && this.BindingContext != null && newDataSource != Convert.DBNull) ? ((CurrencyManager)this.BindingContext[newDataSource, displayMembers[0].BindingPath]) : null);
                        if (this.currentDataManager != objB) {
                            if (this.currentDataManager != null) {
                                this.currentDataManager.ListChanged -= DataManagerOnListChanged;
                                this.currentDataManager.PositionChanged -= DataManagerOnPositionChanged;
                            }
                            this.currentDataManager = objB;
                            if (this.currentDataManager != null) {
                                this.currentDataManager.ListChanged += DataManagerOnListChanged;
                                this.currentDataManager.PositionChanged += DataManagerOnPositionChanged;
                            }
                        }
                        if (this.currentDataManager != null && (flag || flag2 || force) && (flag || flag2 || (force && (list != this.currentDataManager.List || flag3)))) {
                            this.DataUpdateContentFromSource(ListChangedType.PropertyDescriptorChanged, -1);
                        }
                    }
                }
                if (flag) {
                    this.OnDataSourceChanged(EventArgs.Empty);
                }
                if (flag2) {
                    this.OnDisplayMemberChanged(EventArgs.Empty);
                }
            }
            finally {
                this.isSettingDataConnection = false;
            }
        }

        private BindingMemberInfo[] GetDisplayMembers(BindingMemberInfo newDisplayMember) {
            BindingMemberInfo[] array = new BindingMemberInfo[Math.Max(this.Columns.Count, 1)];
            array[0] = ((this.Columns.Count == 0 || string.IsNullOrEmpty(this.Columns[0].DisplayMemberInternal.BindingMember)) ? newDisplayMember : this.Columns[0].DisplayMemberInternal);
            for (int i = 1; i < this.Columns.Count; i++) {
                array[i] = this.Columns[i].DisplayMemberInternal;
            }
            return array;
        }

        private void WireDataSource() {
            if (this.dataSource is IComponent component) {
                component.Disposed += DataSourceDisposed;
            }
            if (this.dataSource is ISupportInitializeNotification supportInitializeNotification && !supportInitializeNotification.IsInitialized) {
                supportInitializeNotification.Initialized += DataSourceInitialized;
                this.isDataSourceInitEventHooked = true;
                this.isDataSourceInitialized = true;
            }
            else {
                this.isDataSourceInitialized = true;
            }
        }

        private void UnwireDataSource() {
            if (this.dataSource is IComponent component) {
                component.Disposed -= DataSourceDisposed;
            }
            if (this.dataSource is ISupportInitializeNotification supportInitializeNotification && this.isDataSourceInitEventHooked) {
                supportInitializeNotification.Initialized -= DataSourceInitialized;
                this.isDataSourceInitialized = false;
            }
        }

        private void DataUpdateContentFromSource(ListChangedType listChangedType, int itemIndex) {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            switch (listChangedType) {
                case ListChangedType.Reset:
                case ListChangedType.ItemMoved:
                    this.DataUpdateItemsFromSource();
                    break;
                case ListChangedType.ItemAdded:
                    try {
                        this.Items.BeginSync();
                        this.Items.Insert(itemIndex, this.DataCreateItem(itemIndex));
                    }
                    finally {
                        this.Items.EndSync();
                    }
                    break;
                case ListChangedType.ItemChanged:
                    try {
                        this.Items.BeginSync();
                        if (itemIndex >= 0 && itemIndex < this.Items.Count) {
                            this.Items[itemIndex] = this.DataCreateItem(itemIndex);
                        }
                    }
                    finally {
                        this.Items.EndSync();
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    try {
                        this.Items.BeginSync();
                        if (itemIndex >= 0 && itemIndex < this.Items.Count) {
                            this.Items.RemoveAt(itemIndex);
                        }
                    }
                    finally {
                        this.Items.EndSync();
                    }
                    break;
                default:
                    try {
                        base.BeginUpdate();
                        if (this.DataBindColumns) {
                            this.DataUpdateColumnsFromSource();
                        }
                        this.DataUpdateItemsFromSource();
                    }
                    finally {
                        base.EndUpdate();
                    }
                    break;
            }
            if (this.DataBindPosition) {
                this.DataUpdatePositionFromSource();
            }
        }

        private void DataUpdateColumnsFromSource() {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            try {
                this.isUpdatingContent = true;
                base.BeginUpdate();
                this.Columns.BeginSync();
                BetterListViewColumnHeaderCollection betterListViewColumnHeaderCollection = new BetterListViewColumnHeaderCollection();
                foreach (PropertyDescriptor itemProperty in this.currentDataManager.GetItemProperties()) {
                    BetterListViewColumnHeader betterListViewColumnHeader = new BetterListViewColumnHeader(itemProperty.DisplayName);
                    betterListViewColumnHeader.DisplayMember = itemProperty.Name;
                    betterListViewColumnHeader.Name = itemProperty.Name;
                    betterListViewColumnHeaderCollection.Add(betterListViewColumnHeader);
                }
                this.Columns.Clear();
                this.Columns.AddRange(betterListViewColumnHeaderCollection);
                this.currentDisplayMembers = this.GetDisplayMembers(this.currentDisplayMembers[0]);
            }
            finally {
                this.Columns.EndSync();
                base.EndUpdate();
                this.isUpdatingContent = false;
            }
        }

        private void DataUpdateItemsFromSource() {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            try {
                base.BeginUpdate();
                this.Items.BeginSync();
                BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection();
                for (int i = 0; i < this.currentDataManager.Count; i++) {
                    betterListViewItemCollection.Add(this.DataCreateItem(i));
                }
                this.Items.Clear();
                this.Items.AddRange(betterListViewItemCollection);
                this.Items.EndSync();
            }
            finally {
                base.EndUpdate();
                this.isUpdatingContent = false;
            }
        }

        private void DataUpdateItemsToSource(BetterListViewElementCollectionChangeInfo changeInfo) {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            switch (changeInfo.ChangeType) {
                case BetterListViewElementCollectionChangeType.Add:
                    throw new InvalidOperationException("Cannot add items when there is a bound data source.");
                case BetterListViewElementCollectionChangeType.Remove: {
                        List<int> list2 = new List<int>(changeInfo.Elements.Values);
                        list2.Sort();
                        for (int num = list2.Count - 1; num >= 0; num--) {
                            this.currentDataManager.RemoveAt(list2[num]);
                        }
                        break;
                    }
                case BetterListViewElementCollectionChangeType.Sort: {
                        this.currentDataManager.SuspendBinding();
                        int position = this.currentDataManager.Position;
                        Dictionary<int, int> dictionary = new Dictionary<int, int>();
                        foreach (KeyValuePair<BetterListViewElementBase, int> element in changeInfo.Elements) {
                            BetterListViewElementBase key = element.Key;
                            int value = element.Value;
                            dictionary.Add(value, key.Index);
                        }
                        IList list = this.currentDataManager.List;
                        object[] array = new object[list.Count];
                        for (int i = 0; i < list.Count; i++) {
                            if (dictionary.ContainsKey(i)) {
                                array[dictionary[i]] = list[i];
                            }
                            else {
                                array[i] = list[i];
                            }
                        }
                        for (int j = 0; j < list.Count; j++) {
                            list[j] = array[j];
                        }
                        if (position != -1 && dictionary.ContainsKey(position)) {
                            this.currentDataManager.Position = dictionary[position];
                        }
                        this.currentDataManager.ResumeBinding();
                        break;
                    }
                case BetterListViewElementCollectionChangeType.Set:
                    throw new InvalidOperationException("Cannot set items when there is a bound data source.");
            }
        }

        private void DataUpdateSubItemToSource(BetterListViewSubItem subItem) {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            try {
                this.isUpdatingContent = true;
                string bindingField = this.currentDisplayMembers[subItem.Index].BindingField;
                bool flag = !string.IsNullOrEmpty(bindingField);
                PropertyDescriptor propertyDescriptor;
                Type type;
                TypeConverter converter;
                if (flag) {
                    propertyDescriptor = this.currentDataManager.GetItemProperties().Find(bindingField, ignoreCase: false);
                    type = propertyDescriptor.PropertyType;
                    converter = propertyDescriptor.Converter;
                }
                else {
                    object obj = this.currentDataManager.List[subItem.Item.Index];
                    type = obj.GetType();
                    converter = TypeDescriptor.GetConverter(obj);
                    propertyDescriptor = null;
                }
                if (converter == null) {
                    throw new InvalidOperationException("Unable to convert sub-item text to type: '" + type.FullName + "'. Type converter not provided.");
                }
                try {
                    if (flag) {
                        if (propertyDescriptor.Converter != null && propertyDescriptor.Converter.CanConvertFrom(typeof(string))) {
                            propertyDescriptor.SetValue(this.currentDataManager.List[subItem.Item.Index], propertyDescriptor.Converter.ConvertFromString(subItem.Text));
                        }
                    }
                    else if (converter.CanConvertFrom(typeof(string))) {
                        this.currentDataManager.List[subItem.Item.Index] = converter.ConvertFromString(subItem.Text);
                    }
                }
                catch (Exception innerException) {
                    throw new InvalidOperationException("Unable to convert sub-item text to type: '" + type.FullName + "'. Value: '" + subItem.Text + "'.", innerException);
                }
            }
            finally {
                this.isUpdatingContent = false;
            }
        }

        private void DataUpdatePositionFromSource() {
            if (this.DataBindPosition && !this.isUpdatingContent && this.currentDataManager != null && this.currentDataManager.Position != -1 && this.Items.Count != 0) {
                try {
                    this.isUpdatingContent = true;
                    int index = Math.Min(Math.Max(this.currentDataManager.Position, 0), this.Items.Count - 1);
                    BetterListViewItem[] array = new BetterListViewItem[1] { this.Items[index] };
                    BetterListViewSelectionInfo selectionInfoNew = new BetterListViewSelectionInfo(this.GetSelectedGroups(array), array, array[0]);
                    this.SetFocusInfo(new BetterListViewFocusInfo(array[0], 0), BetterListViewSelectionChangeMode.DataBinding);
                    this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.DataBinding);
                    this.EnsureVisible(array[0].Index);
                }
                finally {
                    this.isUpdatingContent = false;
                }
            }
        }

        private void DataUpdatePositionToSource() {
            if (this.isUpdatingContent || this.currentDataManager == null) {
                return;
            }
            try {
                this.isUpdatingContent = true;
                if (this.DataBindPosition && this.currentDataManager != null && this.selectedIndices.Count != 0 && this.currentDataManager.Position != this.selectedIndices[0]) {
                    this.currentDataManager.Position = this.selectedIndices[0];
                }
            }
            finally {
                this.isUpdatingContent = false;
            }
        }

        private BetterListViewItem DataCreateItem(int index) {
            object row = this.currentDataManager.List[index];
            PropertyDescriptorCollection itemProperties = this.currentDataManager.GetItemProperties();
            string[] array = new string[this.currentDisplayMembers.Length];
            for (int i = 0; i < this.currentDisplayMembers.Length; i++) {
                BindingMemberInfo bindingMemberInfo = this.currentDisplayMembers[i];
                PropertyDescriptor propertyDescriptor = ((!string.IsNullOrEmpty(bindingMemberInfo.BindingField)) ? itemProperties.Find(bindingMemberInfo.BindingField, ignoreCase: false) : null);
                array[i] = this.DataGetSubItemText(row, i, propertyDescriptor);
            }
            return new BetterListViewItem(array);
        }

        private string DataGetSubItemText(object row, int columnIndex, PropertyDescriptor propertyDescriptor) {
            if (propertyDescriptor != null) {
                object value = propertyDescriptor.GetValue(row);
                if (value != null && propertyDescriptor.Converter != null) {
                    return propertyDescriptor.Converter.ConvertToString(value);
                }
                if (value != null) {
                    return value.ToString();
                }
            }
            if (columnIndex == 0) {
                TypeConverter converter = TypeDescriptor.GetConverter(row);
                if (converter != null) {
                    return converter.ConvertToString(row);
                }
                return row.ToString();
            }
            return string.Empty;
        }

        private void DataSourceInitialized(object sender, EventArgs eventArgs) {
            this.DataSetDataConnection(this.dataSource, this.currentDisplayMembers[0], force: true);
        }

        private void DataSourceDisposed(object sender, EventArgs eventArgs) {
            this.DataSetDataConnection(null, new BindingMemberInfo(string.Empty), force: true);
        }

        private void DataManagerOnListChanged(object sender, ListChangedEventArgs eventArgs) {
            this.DataUpdateContentFromSource(eventArgs.ListChangedType, eventArgs.NewIndex);
        }

        private void DataManagerOnPositionChanged(object sender, EventArgs eventArgs) {
            this.DataUpdatePositionFromSource();
        }

        /// <summary>
        ///   Get element address from element location.
        /// </summary>
        /// <param name="insertionLocation">Element insertion location.</param>
        /// <returns>Element address.</returns>
        public BetterListViewAddress GetAddressFromLocation(BetterListViewInsertionLocation insertionLocation) {
            if (insertionLocation.IsEmpty) {
                return null;
            }
            if (insertionLocation.Address.IsGroup) {
                BetterListViewGroup groupFromAddressInternal = this.GetGroupFromAddressInternal(insertionLocation.Address, allowDefaultGroup: true);
                if (groupFromAddressInternal != null) {
                    ReadOnlyCollection<BetterListViewItem> readOnlyCollection = groupFromAddressInternal.GetItems(this);
                    if (readOnlyCollection.Count == 0) {
                        return groupFromAddressInternal.Address;
                    }
                    return readOnlyCollection[0].Address;
                }
                return null;
            }
            switch (insertionLocation.DropPart) {
                case BetterListViewDropPart.Before:
                    return insertionLocation.Address;
                case BetterListViewDropPart.After: {
                        BetterListViewItem betterListViewItem = this.GetItemFromAddress(insertionLocation.Address);
                        if (betterListViewItem != null) {
                            if (insertionLocation.Level != -1) {
                                while (insertionLocation.Level < betterListViewItem.Level) {
                                    betterListViewItem = betterListViewItem.ParentItem;
                                }
                            }
                            if (betterListViewItem.ParentItem != null) {
                                return new BetterListViewAddress(betterListViewItem.Index + 1, betterListViewItem.ParentItem.Address);
                            }
                            return new BetterListViewAddress(betterListViewItem.Index + 1, (betterListViewItem.Group != null) ? betterListViewItem.Group.Address : null);
                        }
                        return null;
                    }
                case BetterListViewDropPart.Inside:
                    return new BetterListViewAddress(0, insertionLocation.Address);
                case BetterListViewDropPart.On:
                    return insertionLocation.Address;
                default:
                    throw new ApplicationException($"Unknown drop part: '{insertionLocation.DropPart}'.");
            }
        }

        /// <summary>
        ///   Get BetterListViewGroup instance within this control from group address.
        /// </summary>
        /// <param name="address">Address of the group.</param>
        /// <returns>BetterListViewGroup instance.</returns>
        public BetterListViewGroup GetGroupFromAddress(BetterListViewAddress address) {
            Checks.CheckNotNull(address, "address");
            return this.GetGroupFromAddressInternal(address, allowDefaultGroup: false);
        }

        /// <summary>
        ///   Get BetterListViewItem instance within this control from item address.
        /// </summary>
        /// <param name="address">Address of the item.</param>
        /// <returns>BetterListViewItem instance.</returns>
        public BetterListViewItem GetItemFromAddress(BetterListViewAddress address) {
            Checks.CheckNotNull(address, "address");
            if (address.IsGroup) {
                return null;
            }
            address.ToNumericAddress(out var _, out var colItemIndices);
            int num = colItemIndices.Pop();
            if (num < 0 || num >= this.Items.Count) {
                return null;
            }
            BetterListViewItem betterListViewItem = this.Items[num];
            while (colItemIndices.Count != 0) {
                num = colItemIndices.Pop();
                if (num < 0 || num >= betterListViewItem.ChildItems.Count) {
                    return null;
                }
                betterListViewItem = betterListViewItem.ChildItems[num];
            }
            return betterListViewItem;
        }

        /// <summary>
        ///   Get insertion location for the specified screen coordinates.
        /// </summary>
        /// <param name="x">x-position within control in screen coordinates</param>
        /// <param name="y">y-position within control in screen coordinates</param>
        /// <param name="insertionEffect">The insertion locatio is based on insertion rather than drop effect.</param>
        /// <returns>Insertion location.</returns>
        public BetterListViewInsertionLocation GetDropInfo(int x, int y, bool insertionEffect) {
            Point location = base.PointToClient(new Point(x, y));
            BetterListViewGroup groupAt = this.GetGroupAt(location);
            if (groupAt != null) {
                ReadOnlyCollection<BetterListViewItem> readOnlyCollection = groupAt.GetItems(this);
                if (groupAt.IsExpanded && readOnlyCollection.Count != 0) {
                    return new BetterListViewInsertionLocation(readOnlyCollection[0].Address, BetterListViewDropPart.Before, 0);
                }
                return new BetterListViewInsertionLocation(groupAt.Address, BetterListViewDropPart.Inside);
            }
            bool directionVertical = this.LayoutItemsCurrent.DirectionVertical;
            BetterListViewItem item;
            BetterListViewHitPart itemPart;
            if (insertionEffect) {
                this.GetItemAt(location, selectableOnly: true, nearest: true, out item, out itemPart);
                if (item != null) {
                    if (this.View == BetterListViewView.Details) {
                        Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                        BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds.Clone();
                        betterListViewItemBounds.Offset(offsetContentFromAbsolute);
                        if (item.ParentItem != null && item.Index == ((ICollection)item.OwnerCollection).Count - 1 && location.X < betterListViewItemBounds.BoundsOuterExtended.Left && (itemPart & BetterListViewHitPart.Bottom) == BetterListViewHitPart.Bottom) {
                            BetterListViewDropPart dropPart = BetterListViewDropPart.After;
                            int num = item.Level;
                            BetterListViewItem betterListViewItem = item;
                            BetterListViewItemBounds betterListViewItemBounds2;
                            do {
                                num--;
                                betterListViewItem = betterListViewItem.ParentItem;
                                betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.Clone();
                                betterListViewItemBounds2.Offset(offsetContentFromAbsolute);
                            }
                            while (location.X < betterListViewItemBounds2.BoundsOuterExtended.Left && num > 0);
                            return new BetterListViewInsertionLocation(item.Address, dropPart, num);
                        }
                    }
                    return new BetterListViewInsertionLocation(item.Address, this.GetDropPart(directionVertical, itemPart), item.Level);
                }
            }
            else {
                this.GetItemAt(location, out item, out itemPart);
                if (item != null) {
                    return new BetterListViewInsertionLocation(item.Address, BetterListViewDropPart.On, item.Level);
                }
            }
            return BetterListViewInsertionLocation.Empty;
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.DragEnter" /> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="T:System.Windows.Forms.DragEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnDragEnter(DragEventArgs drgevent) {
            if (base.DesignMode || this.ReadOnly) {
                return;
            }
            try {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                betterListViewStateInfo.State = BetterListViewState.ItemDrag;
                BetterListViewItemDragData betterListViewItemDragData = ((!(drgevent.Data is DataObject) || !drgevent.Data.GetDataPresent(typeof(BetterListViewItemDragData))) ? null : ((BetterListViewItemDragData)drgevent.Data.GetData(typeof(BetterListViewItemDragData))));
                bool flag = betterListViewItemDragData?.DragSourceID.Equals(base.DragSourceID, StringComparison.Ordinal) ?? false;
                BetterListViewDragDropType dragDropType = ((this.ItemReorderMode != 0 && betterListViewItemDragData != null && flag) ? BetterListViewDragDropType.ItemReorder : (flag ? BetterListViewDragDropType.Internal : BetterListViewDragDropType.External));
                if (betterListViewItemDragData != null) {
                    betterListViewStateInfo.ItemDragStateInfo = new BetterListViewItemDragStateInfo(dragDropType, betterListViewItemDragData.Items);
                }
                else {
                    betterListViewStateInfo.ItemDragStateInfo = new BetterListViewItemDragStateInfo(dragDropType, new BetterListViewItemCollection());
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetHitTestInfo(this.HitTest());
                this.SetDropEffect(drgevent);
                this.RefreshView();
                base.OnDragEnter(drgevent);
            }
            catch (Exception exception) {
                BetterListViewDragDropExceptionEventArgs betterListViewDragDropExceptionEventArgs = new BetterListViewDragDropExceptionEventArgs(exception);
                this.OnDragDropException(betterListViewDragDropExceptionEventArgs);
                if (betterListViewDragDropExceptionEventArgs.ShowExceptionDialog) {
                    BetterListView.DragDropShowExceptionDialog(exception);
                }
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.DragOver" /> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="T:System.Windows.Forms.DragEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnDragOver(DragEventArgs drgevent) {
            if (base.DesignMode || this.ReadOnly) {
                return;
            }
            try {
                this.SetDropEffect(drgevent);
                Point point = base.PointToClient(new Point(drgevent.X, drgevent.Y));
                if (drgevent.Data is DataObject && drgevent.Data.GetDataPresent(typeof(BetterListViewItemDragData))) {
                    this.OnItemDrag(new BetterListViewItemDragEventArgs((BetterListViewItemDragData)drgevent.Data.GetData(typeof(BetterListViewItemDragData)), point));
                }
                BetterListViewElementBase betterListViewElementBase = (BetterListViewElementBase)(((object)this.GetItemAt(point)) ?? ((object)this.GetGroupAt(point)));
                if (betterListViewElementBase != this.StateInfo.ItemDragStateInfo.LastHoverElement) {
                    BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                    betterListViewStateInfo.ItemDragStateInfo = new BetterListViewItemDragStateInfo(betterListViewStateInfo.ItemDragStateInfo.DragDropType, betterListViewStateInfo.ItemDragStateInfo.SourceItems, betterListViewElementBase);
                    this.SetStateInfo(betterListViewStateInfo);
                    this.AutoExpandStart();
                }
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
                base.OnDragOver(drgevent);
            }
            catch (Exception exception) {
                BetterListViewDragDropExceptionEventArgs betterListViewDragDropExceptionEventArgs = new BetterListViewDragDropExceptionEventArgs(exception);
                this.OnDragDropException(betterListViewDragDropExceptionEventArgs);
                if (betterListViewDragDropExceptionEventArgs.ShowExceptionDialog) {
                    BetterListView.DragDropShowExceptionDialog(exception);
                }
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.DragDrop" /> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="T:System.Windows.Forms.DragEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnDragDrop(DragEventArgs drgevent) {
            if (base.DesignMode || this.ReadOnly) {
                return;
            }
            try {
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                BetterListViewInsertionLocation dropInfo = this.GetDropInfo(drgevent.X, drgevent.Y, insertionEffect: false);
                BetterListViewItem item = ((!dropInfo.IsEmpty) ? this.GetItemFromAddress(dropInfo.Address) : null);
                dropInfo = this.GetDropInfo(drgevent.X, drgevent.Y, this.GetDragDropDisplay(item) == BetterListViewDragDropDisplay.InsertionMark);
                this.AutoExpandStop();
                try {
                    base.BeginUpdate();
                    this.SuspendSelectionChanged();
                    if (!dropInfo.IsEmpty) {
                        BetterListViewItemDropEventArgs eventArgs = new BetterListViewItemDropEventArgs(drgevent.Data, drgevent.KeyState, drgevent.X, drgevent.Y, drgevent.AllowedEffect, drgevent.Effect, this.GetItemFromAddress(dropInfo.Address), dropInfo.DropPart);
                        this.OnItemDrop(eventArgs);
                    }
                    if (this.GetDragDropDisplay(item) == BetterListViewDragDropDisplay.InsertionMark) {
                        this.insertionMark = BetterListViewInsertionMark.Empty;
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                    BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    BetterListViewItem betterListViewItem = null;
                    if (selectionInfoNew.SelectedItems.Count != 0) {
                        using (IEnumerator<BetterListViewItem> enumerator = selectionInfoNew.SelectedItems.Keys.GetEnumerator()) {
                            if (enumerator.MoveNext()) {
                                BetterListViewItem current = enumerator.Current;
                                betterListViewItem = current;
                            }
                        }
                    }
                    BetterListViewFocusInfo focusInfoNew = ((betterListViewItem != null) ? new BetterListViewFocusInfo(betterListViewItem, 0) : BetterListViewFocusInfo.Empty);
                    this.SetStateInfo(betterListViewStateInfo);
                    this.SetFocusInfo(focusInfoNew, BetterListViewSelectionChangeMode.DragDrop);
                    this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.DragDrop);
                    this.ResumeSelectionChanged();
                }
                finally {
                    base.EndUpdate();
                }
                base.OnDragDrop(drgevent);
            }
            catch (Exception exception) {
                BetterListViewDragDropExceptionEventArgs betterListViewDragDropExceptionEventArgs = new BetterListViewDragDropExceptionEventArgs(exception);
                this.OnDragDropException(betterListViewDragDropExceptionEventArgs);
                if (betterListViewDragDropExceptionEventArgs.ShowExceptionDialog) {
                    BetterListView.DragDropShowExceptionDialog(exception);
                }
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.DragLeave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnDragLeave(EventArgs e) {
            if (base.DesignMode || this.ReadOnly) {
                return;
            }
            try {
                this.AutoExpandStop();
                if (this.GetDragDropDisplay() == BetterListViewDragDropDisplay.InsertionMark) {
                    this.insertionMark = BetterListViewInsertionMark.Empty;
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                }
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewHitTestInfo betterListViewHitTestInfo = this.HitTestInfo;
                betterListViewStateInfo.State = BetterListViewState.Normal;
                betterListViewHitTestInfo = BetterListViewHitTestInfo.Empty;
                this.SetStateInfo(betterListViewStateInfo);
                this.SetHitTestInfo(betterListViewHitTestInfo);
                this.RefreshView();
                base.OnDragLeave(e);
            }
            catch (Exception exception) {
                BetterListViewDragDropExceptionEventArgs betterListViewDragDropExceptionEventArgs = new BetterListViewDragDropExceptionEventArgs(exception);
                this.OnDragDropException(betterListViewDragDropExceptionEventArgs);
                if (betterListViewDragDropExceptionEventArgs.ShowExceptionDialog) {
                    BetterListView.DragDropShowExceptionDialog(exception);
                }
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.QueryContinueDrag" /> event.
        /// </summary>
        /// <param name="qcdevent">A <see cref="T:System.Windows.Forms.QueryContinueDragEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent) {
            if (base.DesignMode || this.ReadOnly) {
                return;
            }
            try {
                if (!this.AllowDrop && (qcdevent.EscapePressed || Control.MouseButtons != MouseButtons.Left)) {
                    BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    this.SetStateInfo(betterListViewStateInfo);
                    this.RefreshView();
                }
                base.OnQueryContinueDrag(qcdevent);
            }
            catch (Exception exception) {
                BetterListViewDragDropExceptionEventArgs betterListViewDragDropExceptionEventArgs = new BetterListViewDragDropExceptionEventArgs(exception);
                this.OnDragDropException(betterListViewDragDropExceptionEventArgs);
                if (betterListViewDragDropExceptionEventArgs.ShowExceptionDialog) {
                    BetterListView.DragDropShowExceptionDialog(exception);
                }
            }
        }

        private static void DragDropShowExceptionDialog(Exception exception) {
            MessageBox.Show(exception.ToString(), "Exception Thrown During Drag and Drop Operation", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void AddLocationsFlat(BetterListViewAddress targetAddress, BetterListViewAddress[] locationsSource, BetterListViewItemCollection items, Dictionary<BetterListViewAddress, BetterListViewAddress> locations, ref int indexItemLast) {
            for (int i = 0; i < locationsSource.Length; i++) {
                BetterListViewAddress betterListViewAddress = locationsSource[i];
                if (locations.ContainsKey(betterListViewAddress)) {
                    continue;
                }
                BetterListViewAddress value = new BetterListViewAddress(targetAddress.Index + indexItemLast, targetAddress.ParentAddress);
                locations.Add(betterListViewAddress, value);
                indexItemLast++;
                BetterListViewItem betterListViewItem = items[i];
                BetterListViewItemCollection childItems = betterListViewItem.ChildItems;
                if (childItems.Count != 0) {
                    BetterListViewAddress[] array = new BetterListViewAddress[childItems.Count];
                    for (int j = 0; j < childItems.Count; j++) {
                        array[j] = new BetterListViewAddress(j, betterListViewAddress);
                    }
                    BetterListView.AddLocationsFlat(targetAddress, array, childItems, locations, ref indexItemLast);
                }
            }
        }

        private static void AddLocationsHierarchy(BetterListViewAddress targetAddress, BetterListViewAddress[] locationsSource, BetterListViewItemCollection items, Dictionary<BetterListViewAddress, BetterListViewAddress> locations) {
            for (int i = 0; i < locationsSource.Length; i++) {
                BetterListViewAddress betterListViewAddress = locationsSource[i];
                BetterListViewAddress betterListViewAddress2 = new BetterListViewAddress(targetAddress.Index + i, targetAddress.ParentAddress);
                locations.Add(betterListViewAddress, betterListViewAddress2);
                BetterListViewItem betterListViewItem = items[i];
                BetterListViewItemCollection childItems = betterListViewItem.ChildItems;
                if (childItems.Count != 0) {
                    BetterListViewAddress targetAddress2 = new BetterListViewAddress(0, betterListViewAddress2);
                    BetterListViewAddress[] array = new BetterListViewAddress[childItems.Count];
                    for (int j = 0; j < childItems.Count; j++) {
                        array[j] = new BetterListViewAddress(j, betterListViewAddress);
                    }
                    BetterListView.AddLocationsHierarchy(targetAddress2, array, childItems, locations);
                }
            }
        }

        private void DragDropInitialize() {
            this.autoExpandTimer = new Timer();
            this.autoExpandTimer.Tick += TimerAutoExpandOnTick;
        }

        private BetterListViewDragDropDisplay GetDragDropDisplay() {
            return this.GetDragDropDisplay(null);
        }

        private BetterListViewDragDropDisplay GetDragDropDisplay(BetterListViewItem item) {
            if (this.StateInfo.State != BetterListViewState.ItemDrag) {
                return BetterListViewDragDropDisplay.None;
            }
            switch (this.StateInfo.ItemDragStateInfo.DragDropType) {
                case BetterListViewDragDropType.External:
                    if (item == null || item.CustomDropDisplayExternal == BetterListViewDragDropDisplay.Default) {
                        return this.ItemDropDisplayExternal;
                    }
                    return item.CustomDropDisplayExternal;
                case BetterListViewDragDropType.Internal:
                case BetterListViewDragDropType.ItemReorder:
                    if (item == null || item.CustomDropDisplayInternal == BetterListViewDragDropDisplay.Default) {
                        return this.ItemDropDisplayInternal;
                    }
                    return item.CustomDropDisplayInternal;
                default:
                    throw new ApplicationException($"Unsupported Drag and Drop type: '{this.StateInfo.ItemDragStateInfo.DragDropType}'");
            }
        }

        private bool SetInsertionMark(BetterListViewInsertionMark insertionMark) {
            if (insertionMark == this.insertionMark) {
                return false;
            }
            this.insertionMark = insertionMark;
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            return true;
        }

        private void SetDropEffect(DragEventArgs eventArgs) {
            bool flag = this.StateInfo.ItemDragStateInfo.DragDropType == BetterListViewDragDropType.ItemReorder;
            BetterListViewDragDropEffectSettingEventArgs betterListViewDragDropEffectSettingEventArgs = new BetterListViewDragDropEffectSettingEventArgs(eventArgs.Data, eventArgs.KeyState, eventArgs.X, eventArgs.Y, eventArgs.AllowedEffect, eventArgs.Effect, flag, updateInsertionMark: true);
            this.OnDragDropEffectSetting(betterListViewDragDropEffectSettingEventArgs);
            if (betterListViewDragDropEffectSettingEventArgs.Effect == DragDropEffects.None && flag && this.ItemReorderMode == BetterListViewItemReorderMode.Enabled) {
                eventArgs.Effect = DragDropEffects.Move;
            }
            else {
                eventArgs.Effect = betterListViewDragDropEffectSettingEventArgs.Effect;
            }
            BetterListViewInsertionLocation dropInfo = this.GetDropInfo(eventArgs.X, eventArgs.Y, insertionEffect: true);
            BetterListViewItem item = ((!dropInfo.IsEmpty) ? this.GetItemFromAddress(dropInfo.Address) : null);
            if (eventArgs.Effect != 0 && this.GetDragDropDisplay(item) == BetterListViewDragDropDisplay.InsertionMark && !dropInfo.IsEmpty) {
                bool enabled = true;
                this.SetInsertionMark(new BetterListViewInsertionMark(dropInfo, Color.Empty, enabled));
            }
            else if (betterListViewDragDropEffectSettingEventArgs.UpdateInsertionMark) {
                this.SetInsertionMark(BetterListViewInsertionMark.Empty);
            }
            if ((eventArgs.AllowedEffect & DragDropEffects.Scroll) == DragDropEffects.Scroll) {
                if (this.AllowAutoScroll && !base.AutoScrollEnabled) {
                    base.AutoScrollStart(BetterListViewAutoScrollMode.Inside);
                    eventArgs.Effect |= DragDropEffects.Scroll;
                }
            }
            else if (base.AutoScrollEnabled) {
                base.AutoScrollStop();
            }
        }

        private BetterListViewDropPart GetDropPart(bool isDirectionVertical, BetterListViewHitPart itemPart) {
            if (isDirectionVertical) {
                if ((itemPart & BetterListViewHitPart.Bottom) != BetterListViewHitPart.Bottom) {
                    return BetterListViewDropPart.Before;
                }
                return BetterListViewDropPart.After;
            }
            if ((itemPart & BetterListViewHitPart.Right) != BetterListViewHitPart.Right) {
                return BetterListViewDropPart.Before;
            }
            return BetterListViewDropPart.After;
        }

        private void AutoExpandStart() {
            this.autoExpandTimer.Stop();
            if (this.AutoExpandDelay != 0) {
                this.autoExpandTimer.Interval = this.AutoExpandDelay;
                this.autoExpandTimer.Start();
            }
            else {
                this.PerformAutoExpand();
            }
        }

        private void AutoExpandStop() {
            this.autoExpandTimer.Stop();
        }

        private void TimerAutoExpandOnTick(object sender, EventArgs eventArgs) {
            this.PerformAutoExpand();
        }

        private void PerformAutoExpand() {
            _ = this.StateInfo.State;
        }

        private BetterListViewGroup GetGroupFromAddressInternal(BetterListViewAddress address, bool allowDefaultGroup) {
            int indexGroup = address.IndexGroup;
            if (indexGroup == -1) {
                if (!allowDefaultGroup) {
                    return null;
                }
                return this.defaultGroup;
            }
            if (indexGroup < 0 || indexGroup >= this.Groups.Count) {
                return null;
            }
            return this.Groups[indexGroup];
        }

        private void PaintColumnHeaderBackground(Graphics graphics, BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderBounds columnHeaderBounds) {
            BetterListViewColumnHeaderStateInfo columnHeaderStateInfo = this.GetColumnHeaderStateInfo(columnHeader, (this.HitTestInfo.ColumnHeader == columnHeader) ? this.HitTestInfo.Locations : BetterListViewHitTestLocations.Custom);
            this.OnDrawColumnHeaderBackground(new BetterListViewDrawColumnHeaderBackgroundEventArgs(graphics, columnHeader, columnHeaderBounds, columnHeaderStateInfo));
        }

        private void PaintColumnHeaderForeground(Graphics graphics, BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderBounds columnHeaderBounds) {
            BetterListViewColumnHeaderStateInfo columnHeaderStateInfo = this.GetColumnHeaderStateInfo(columnHeader, (this.HitTestInfo.ColumnHeader == columnHeader) ? this.HitTestInfo.Locations : BetterListViewHitTestLocations.Custom);
            this.OnDrawColumnHeader(new BetterListViewDrawColumnHeaderEventArgs(graphics, columnHeader, columnHeaderBounds, columnHeaderStateInfo));
        }

        private void GetPainterElementsColumnHeader(int columnHeaderIndex, BetterListViewColumnHeaderStateInfo columnHeaderStateInfo, out BetterListViewPainterElementColumnHeader painterElementColumnHeader) {
            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.Undefined;
            if (columnHeaderIndex == 0) {
                if (base.Enabled) {
                    switch (columnHeaderStateInfo.ColumnHeaderState) {
                        case BetterListViewColumnHeaderState.Normal:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.LeftNormal;
                            break;
                        case BetterListViewColumnHeaderState.Hot:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.LeftHot;
                            break;
                        case BetterListViewColumnHeaderState.Pressed:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.LeftPressed;
                            break;
                        case BetterListViewColumnHeaderState.Sorted:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.LeftSorted;
                            break;
                    }
                }
                else {
                    painterElementColumnHeader = BetterListViewPainterElementColumnHeader.LeftNormal;
                }
            }
            else if (columnHeaderIndex == this.Columns.Count - 1) {
                if (base.Enabled) {
                    switch (columnHeaderStateInfo.ColumnHeaderState) {
                        case BetterListViewColumnHeaderState.Normal:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.RightNormal;
                            break;
                        case BetterListViewColumnHeaderState.Hot:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.RightHot;
                            break;
                        case BetterListViewColumnHeaderState.Pressed:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.RightPressed;
                            break;
                        case BetterListViewColumnHeaderState.Sorted:
                            painterElementColumnHeader = BetterListViewPainterElementColumnHeader.RightSorted;
                            break;
                    }
                }
                else {
                    painterElementColumnHeader = BetterListViewPainterElementColumnHeader.RightNormal;
                }
            }
            else if (base.Enabled) {
                switch (columnHeaderStateInfo.ColumnHeaderState) {
                    case BetterListViewColumnHeaderState.Normal:
                        painterElementColumnHeader = BetterListViewPainterElementColumnHeader.MiddleNormal;
                        break;
                    case BetterListViewColumnHeaderState.Hot:
                        painterElementColumnHeader = BetterListViewPainterElementColumnHeader.MiddleHot;
                        break;
                    case BetterListViewColumnHeaderState.Pressed:
                        painterElementColumnHeader = BetterListViewPainterElementColumnHeader.MiddlePressed;
                        break;
                    case BetterListViewColumnHeaderState.Sorted:
                        painterElementColumnHeader = BetterListViewPainterElementColumnHeader.MiddleSorted;
                        break;
                }
            }
            else {
                painterElementColumnHeader = BetterListViewPainterElementColumnHeader.MiddleNormal;
            }
        }

        /// <summary>
        ///   Internal method for redrawing doing the actual drawing.
        /// </summary>
        /// <param name="graphics">Graphics object used for redrawing.</param>
        protected override void DrawingRedrawCore(Graphics graphics) {
            Rectangle clientRectangleInner = base.ClientRectangleInner;
            if (clientRectangleInner.Width <= 0 || clientRectangleInner.Height <= 0) {
                return;
            }
            Point offsetColumnsFromAbsolute = this.OffsetColumnsFromAbsolute;
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            Rectangle rectangle = base.InvalidationInfo.Region;
            if (rectangle.IsEmpty) {
                rectangle = base.ClientRectangleInner;
            }
            graphics.SetClip(rectangle);
            BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = null;
            switch (this.SortedColumnsRowsHighlight) {
                case BetterListViewSortedColumnsRowsHighlight.ShowAlways:
                    if (this.sortList.Count != 0) {
                        BetterListViewColumnHeader betterListViewColumnHeader2 = this.Columns[this.sortList[0].ColumnIndex];
                        if (betterListViewColumnHeader2 != null) {
                            betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)betterListViewColumnHeader2).LayoutBounds;
                        }
                    }
                    break;
                case BetterListViewSortedColumnsRowsHighlight.ShowMultiColumnOnly:
                    if (this.sortList.Count > 1) {
                        BetterListViewColumnHeader betterListViewColumnHeader = this.Columns[this.sortList[0].ColumnIndex];
                        if (betterListViewColumnHeader != null) {
                            betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)betterListViewColumnHeader).LayoutBounds;
                        }
                    }
                    break;
            }
            if (betterListViewColumnHeaderBounds != null) {
                betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)betterListViewColumnHeaderBounds.Clone();
                betterListViewColumnHeaderBounds.Offset(offsetColumnsFromAbsolute);
            }
            Rectangle contentBounds = this.GetContentBounds(base.HScrollBarVisible, base.VScrollBarVisible);
            if (contentBounds.Width > 0 && contentBounds.Height > 0) {
                this.OnDrawBackground(new BetterListViewDrawBackgroundEventArgs(graphics, contentBounds, betterListViewColumnHeaderBounds));
            }
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Point point = base.PointToClient(Control.MousePosition);
            Rectangle boundsColumnHeaders = this.BoundsColumnHeaders;
            Rectangle boundsContent = base.BoundsContent;
            graphics.SetClip(boundsContent);
            graphics.IntersectClip(rectangle);
            if (this.IsAnyGroupVisible && base.Enabled) {
                for (int i = this.visibleRangeGroups.IndexElementFirst; i <= this.visibleRangeGroups.IndexElementLast; i++) {
                    BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[i];
                    if (betterListViewGroup != this.defaultGroup || this.ShowDefaultGroupHeader) {
                        BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds.Clone();
                        betterListViewGroupBounds.Offset(offsetContentFromAbsolute);
                        if (rectangle.IntersectsWith(betterListViewGroupBounds.BoundsSpacing)) {
                            this.PaintGroupBackground(graphics, betterListViewGroup, betterListViewGroupBounds);
                        }
                    }
                }
            }
            if (this.IsAnyItemVisible) {
                if (base.Enabled) {
                    for (int j = this.visibleRangeItemsDisplay.IndexElementFirst; j <= this.visibleRangeItemsDisplay.IndexElementLast; j++) {
                        BetterListViewItem betterListViewItem = this.layoutElementsItemsDisplay[j];
                        BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.Clone();
                        betterListViewItemBounds.Offset(offsetContentFromAbsolute);
                        if (rectangle.IntersectsWith(betterListViewItemBounds.BoundsOuterExtended)) {
                            this.PaintItemBackground(graphics, betterListViewItem, betterListViewItemBounds);
                        }
                    }
                }
                if (this.GridLines != 0 && this.View == BetterListViewView.Details && boundsContent.Width > 0 && boundsContent.Height > 0) {
                    BetterListViewPainter.DrawGridLines(graphics, this.GridLines, this.layoutElementsColumns, this.visibleRangeColumns, this.layoutElementsItemsSelection, this.visibleRangeItemsSelection, boundsContent, this.OffsetContentFromAbsolute, this.ColorGridLines);
                }
                for (int k = this.visibleRangeItemsDisplay.IndexElementFirst; k <= this.visibleRangeItemsDisplay.IndexElementLast; k++) {
                    BetterListViewItem betterListViewItem2 = this.layoutElementsItemsDisplay[k];
                    if (betterListViewItem2.ListView == this) {
                        BetterListViewItemBounds betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds.Clone();
                        betterListViewItemBounds2.Offset(offsetContentFromAbsolute);
                        if (rectangle.IntersectsWith(betterListViewItemBounds2.BoundsOuterExtended)) {
                            this.PaintItemForeground(graphics, betterListViewItem2, betterListViewItemBounds2);
                        }
                    }
                }
            }
            if (this.IsAnyGroupVisible) {
                for (int l = this.visibleRangeGroups.IndexElementFirst; l <= this.visibleRangeGroups.IndexElementLast; l++) {
                    BetterListViewGroup betterListViewGroup2 = this.layoutElementsGroups[l];
                    if (betterListViewGroup2 != this.defaultGroup || this.ShowDefaultGroupHeader) {
                        BetterListViewGroupBounds betterListViewGroupBounds2 = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup2).LayoutBounds.Clone();
                        betterListViewGroupBounds2.Offset(offsetContentFromAbsolute);
                        this.PaintGroupForeground(graphics, betterListViewGroup2, betterListViewGroupBounds2);
                    }
                }
            }
            if (this.IsAnyItemVisible || this.IsAnyGroupVisible) {
                this.PaintInsertionMark(graphics, this.InsertionMark);
            }
            if (this.StateInfo.State == BetterListViewState.ItemSelection) {
                BetterListViewPainter.DrawSelectionRectangle(graphics, this.DragSelectionRectangle);
            }
            graphics.SetClip(boundsColumnHeaders);
            graphics.IntersectClip(rectangle);
            if (this.IsAnyColumnHeaderVisible && boundsColumnHeaders.Height > 0) {
                Brush brush = new SolidBrush(this.BackColor);
                graphics.FillRectangle(brush, boundsColumnHeaders);
                brush.Dispose();
                for (int m = this.visibleRangeColumns.IndexElementFirst; m <= this.visibleRangeColumns.IndexElementLast; m++) {
                    BetterListViewColumnHeader betterListViewColumnHeader3 = this.layoutElementsColumns[m];
                    BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds2 = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)betterListViewColumnHeader3).LayoutBounds.Clone();
                    betterListViewColumnHeaderBounds2.Offset(offsetColumnsFromAbsolute);
                    if (rectangle.IntersectsWith(betterListViewColumnHeaderBounds2.BoundsOuter)) {
                        this.PaintColumnHeaderBackground(graphics, betterListViewColumnHeader3, betterListViewColumnHeaderBounds2);
                        this.PaintColumnHeaderForeground(graphics, betterListViewColumnHeader3, betterListViewColumnHeaderBounds2);
                    }
                }
                int num = ((!this.visibleRangeColumns.IsUndefined) ? ((IBetterListViewLayoutElementDisplayable)this.layoutElementsColumns[this.visibleRangeColumns.IndexElementLast]).LayoutBounds.BoundsOuter.Right : 0);
                num += offsetColumnsFromAbsolute.X;
                if (num < boundsColumnHeaders.Right) {
                    Rectangle rectangle2 = new Rectangle(num, boundsColumnHeaders.Top, boundsColumnHeaders.Width - num + 2, boundsColumnHeaders.Height);
                    if (rectangle.IntersectsWith(rectangle2)) {
                        BetterListViewPainter.DrawColumnHeaderBackground(graphics, rectangle2, BetterListViewPainterElementColumnHeader.MiddleNormal);
                    }
                }
            }
            if (this.StateInfo.State == BetterListViewState.ColumnReorder) {
                BetterListViewColumnReorderStateInfo columnReorderStateInfo = this.StateInfo.ColumnReorderStateInfo;
                BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds3 = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnReorderStateInfo.Column).LayoutBounds.Clone();
                betterListViewColumnHeaderBounds3.Offset(offsetColumnsFromAbsolute);
                Rectangle destRect = new Rectangle(betterListViewColumnHeaderBounds3.BoundsOuter.Left + (base.ScrollPositionHorizontal - columnReorderStateInfo.ScrollPositionHorizontal) + (point.X - columnReorderStateInfo.StartPoint.X), betterListViewColumnHeaderBounds3.BoundsOuter.Top, betterListViewColumnHeaderBounds3.BoundsOuter.Width, betterListViewColumnHeaderBounds3.BoundsOuter.Height);
                Bitmap columnBitmap = this.StateInfo.ColumnReorderStateInfo.ColumnBitmap;
                graphics.DrawImage(columnBitmap, destRect, 0, 0, columnBitmap.Width, columnBitmap.Height, GraphicsUnit.Pixel);
                this.GetColumnHeaderAt(new Point(Math.Min(Math.Max(point.X, boundsColumnHeaders.Left), boundsColumnHeaders.Left + this.layoutMeasurementColumns.Width - 1), boundsColumnHeaders.Top), out var columnHeader, out var columnHeaderPart);
                if (columnHeader == null) {
                    columnHeader = this.Columns[this.Columns.Count - 1];
                    columnHeaderPart = BetterListViewHitPart.Right;
                }
                betterListViewColumnHeaderBounds3 = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds.Clone();
                int num2 = -1;
                if (columnHeader.Index == this.Columns.Count - 1 && (columnHeaderPart & BetterListViewHitPart.Right) == BetterListViewHitPart.Right) {
                    num2 = betterListViewColumnHeaderBounds3.BoundsOuter.Right - 1 + offsetColumnsFromAbsolute.X;
                }
                else if ((columnHeaderPart & BetterListViewHitPart.Right) != BetterListViewHitPart.Right) {
                    num2 = betterListViewColumnHeaderBounds3.BoundsOuter.Left + offsetColumnsFromAbsolute.X;
                }
                else if (columnHeader.Index < this.Columns.Count - 1 && ((IBetterListViewStateElement)this.Columns[columnHeader.Index + 1]).IsActive) {
                    num2 = ((IBetterListViewLayoutElementDisplayable)this.Columns[columnHeader.Index + 1]).LayoutBounds.BoundsOuter.Left + offsetColumnsFromAbsolute.X;
                }
                if (num2 != -1) {
                    BetterListViewBasePainter.DrawInsertionMark(graphics, new Point(num2, betterListViewColumnHeaderBounds3.BoundsOuter.Top), betterListViewColumnHeaderBounds3.BoundsOuter.Height + offsetColumnsFromAbsolute.Y, horizontal: false, this.ColorInsertionMark.IsEmpty ? this.ForeColor : this.ColorInsertionMark, enabled: true);
                }
            }
            graphics.SetClip(clientRectangleInner);
            if (this.StateInfo.State == BetterListViewState.ColumnResize) {
                BetterListViewColumnResizeStateInfo columnResizeStateInfo = this.StateInfo.ColumnResizeStateInfo;
                if (!columnResizeStateInfo.IsSmooth) {
                    int num3 = ((IBetterListViewLayoutElementDisplayable)columnResizeStateInfo.Column).LayoutBounds.BoundsOuter.Left + offsetColumnsFromAbsolute.X + columnResizeStateInfo.ColumnWidthNew - 1;
                    Pen pen = new Pen(this.ColorColumnResizeLine);
                    if (this.View == BetterListViewView.Details) {
                        graphics.DrawLine(pen, num3, clientRectangleInner.Top, num3, boundsContent.Bottom - 1);
                    }
                    else {
                        graphics.DrawLine(pen, num3, clientRectangleInner.Top, num3, ((IBetterListViewLayoutElementDisplayable)columnResizeStateInfo.Column).LayoutBounds.BoundsOuter.Bottom - 1);
                    }
                    pen.Dispose();
                }
            }
            if (base.HScrollBarVisible && base.VScrollBarVisible && clientRectangleInner.Width > 0 && clientRectangleInner.Height > 0) {
                graphics.FillRectangle(SystemBrushes.Control, clientRectangleInner.Right - SystemInformation.VerticalScrollBarWidth, clientRectangleInner.Bottom - SystemInformation.HorizontalScrollBarHeight, SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            }
            graphics.ResetClip();
            base.DrawingRedrawCore(graphics);
        }

        /// <summary>
        ///   Redraw all items.
        /// </summary>
        public void RedrawItems() {
            if (this.Items.Count != 0 || this.Groups.Count != 0) {
                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
        }

        /// <summary>
        ///   Reset state of focus rectangle display to its initial value.
        ///   Focus rectangle will not be displayed until focused item is changed through keyboard input.
        /// </summary>
        public void ResetFocusRectangleDisplay() {
            this.AllowDisplayFocusRectangle = false;
            this.RefreshView();
        }

        /// <summary>
        ///   Set images to be displayed on items and sub-items.
        /// </summary>
        /// <param name="view">View to set item ImageList for.</param>
        /// <param name="imageList">Item ImageList to set.</param>
        public void SetImageListItems(BetterListViewView view, ImageList imageList) {
            bool flag;
            if (imageList != null) {
                if (this.imageListsItems.ContainsKey(view)) {
                    if (this.imageListsItems[view] == imageList) {
                        flag = false;
                    }
                    else {
                        this.imageListsItems[view] = imageList;
                        flag = true;
                    }
                }
                else {
                    this.imageListsItems.Add(view, imageList);
                    flag = true;
                }
            }
            else if (this.imageListsItems.ContainsKey(view)) {
                this.imageListsItems.Remove(view);
                flag = true;
            }
            else {
                flag = false;
            }
            if (!(this.View == view && flag)) {
                return;
            }
            foreach (BetterListViewItem current in this) {
                if (current.Image == null && (current.ImageIndex != -1 || current.ImageKey.Length != 0)) {
                    current.ClearCache();
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
            }
            base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.BackColorChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.EnabledChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnEnabledChanged(EventArgs e) {
            this.LabelEditEnd(forced: true);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
            base.OnEnabledChanged(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.FontChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            foreach (BetterListViewColumnHeader column in this.Columns) {
                column.ClearCache();
            }
            foreach (BetterListViewItem current2 in this) {
                current2.ClearCache();
            }
            this.defaultGroup.ClearCache();
            foreach (BetterListViewGroup group in this.Groups) {
                group.ClearCache();
            }
            foreach (BetterListViewItem current3 in this) {
                foreach (BetterListViewSubItem subItem in current3.SubItems) {
                    subItem.FlushCachedFont();
                }
            }
            foreach (BetterListViewItem current4 in this) {
                ((IBetterListViewStateElement)current4).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
            }
            base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.ForeColorChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:System.Windows.Forms.PaintEventArgs" /> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs eventArgs) {
            if (BetterListViewPainter.ReloadRenderers() || BetterListViewBasePainter.ReloadRenderers()) {
                foreach (BetterListViewItem current in this) {
                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                this.RefreshView();
            }
            else {
                base.OnPaint(eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnVisibleChanged(EventArgs eventArgs) {
            this.LabelEditEnd(forced: true);
            base.OnVisibleChanged(eventArgs);
        }

        private void PaintGroupBackground(Graphics graphics, BetterListViewGroup group, BetterListViewGroupBounds groupBounds) {
            this.OnDrawGroupBackground(new BetterListViewDrawGroupBackgroundEventArgs(graphics, group, groupBounds));
        }

        private void PaintGroupForeground(Graphics graphics, BetterListViewGroup group, BetterListViewGroupBounds groupBounds) {
            BetterListViewGroupStateInfo groupStateInfo = this.GetGroupStateInfo(group, (this.HitTestInfo.Group == group) ? this.HitTestInfo.Locations : BetterListViewHitTestLocations.Custom);
            this.OnDrawGroup(new BetterListViewDrawGroupEventArgs(graphics, group, groupBounds, groupStateInfo));
        }

        private void GetPainterElementsGroup(BetterListViewGroup group, BetterListViewGroupStateInfo groupStateInfo, out BetterListViewPainterElementGroup painterElementGroup, out BetterListViewPainterElementGroupExpandButton painterElementGroupExpandButton) {
            bool flag = (groupStateInfo.GroupState & BetterListViewGroupState.Focused) == BetterListViewGroupState.Focused;
            bool flag2 = (groupStateInfo.GroupState & BetterListViewGroupState.Hot) == BetterListViewGroupState.Hot;
            bool flag3 = !this.LayoutGroupsCurrent.AllowsExpandableGroups || group.IsExpanded;
            bool flag4 = this.SelectionInfo.SelectedGroups.Contains(group);
            bool enabled = base.Enabled;
            painterElementGroup = BetterListViewPainterElementGroup.Undefined;
            if (flag) {
                painterElementGroup = BetterListViewPainterElementGroup.Focused;
            }
            if (!flag3) {
                if (flag2) {
                    if (!flag) {
                        painterElementGroup = BetterListViewPainterElementGroup.CollapsedHot;
                    }
                    if (flag) {
                        painterElementGroup = BetterListViewPainterElementGroup.CollapsedHotFocused;
                    }
                }
                if (flag4) {
                    if (!enabled) {
                        if (!flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.CollapsedSelectedInactive;
                        }
                        if (flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.CollapsedSelectedHotInactive;
                        }
                    }
                    if (enabled) {
                        if (!flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.CollapsedSelectedActive;
                        }
                        if (flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.CollapsedSelectedHotActive;
                        }
                    }
                }
            }
            if (flag3) {
                if (flag2) {
                    if (!flag) {
                        painterElementGroup = BetterListViewPainterElementGroup.ExpandedHot;
                    }
                    if (flag) {
                        painterElementGroup = BetterListViewPainterElementGroup.ExpandedHotFocused;
                    }
                }
                if (flag4) {
                    if (!enabled) {
                        if (!flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedInactive;
                        }
                        if (flag2) {
                            painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedHotInactive;
                        }
                    }
                    if (enabled) {
                        if (!flag2) {
                            if (!flag) {
                                painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedActive;
                            }
                            if (flag) {
                                painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedFocusedActive;
                            }
                        }
                        if (flag2) {
                            if (!flag) {
                                painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedHotActive;
                            }
                            if (flag) {
                                painterElementGroup = BetterListViewPainterElementGroup.ExpandedSelectedHotFocusedActive;
                            }
                        }
                    }
                }
            }
            painterElementGroupExpandButton = BetterListViewPainterElementGroupExpandButton.Undefined;
            switch (groupStateInfo.ExpandButtonState) {
                case BetterListViewGroupExpandButtonState.CollapsedNormal:
                    painterElementGroupExpandButton = (flag4 ? BetterListViewPainterElementGroupExpandButton.SelectedCollapsedNormal : BetterListViewPainterElementGroupExpandButton.CollapsedNormal);
                    break;
                case BetterListViewGroupExpandButtonState.CollapsedHot:
                    painterElementGroupExpandButton = ((!flag4) ? BetterListViewPainterElementGroupExpandButton.CollapsedHot : BetterListViewPainterElementGroupExpandButton.SelectedCollapsedHot);
                    break;
                case BetterListViewGroupExpandButtonState.CollapsedPressed:
                    painterElementGroupExpandButton = (flag4 ? BetterListViewPainterElementGroupExpandButton.SelectedCollapsedPressed : BetterListViewPainterElementGroupExpandButton.CollapsedPressed);
                    break;
                case BetterListViewGroupExpandButtonState.ExpandedNormal:
                    painterElementGroupExpandButton = BetterListViewPainterElementGroupExpandButton.ExpandedNormal;
                    break;
                case BetterListViewGroupExpandButtonState.ExpandedHot:
                    painterElementGroupExpandButton = BetterListViewPainterElementGroupExpandButton.ExpandedHot;
                    break;
                case BetterListViewGroupExpandButtonState.ExpandedPressed:
                    painterElementGroupExpandButton = BetterListViewPainterElementGroupExpandButton.ExpandedPressed;
                    break;
                default:
                    throw new ApplicationException($"Unknown expand button state: '{groupStateInfo.ExpandButtonState}'.");
            }
        }

        private void PaintInsertionMark(Graphics graphics, BetterListViewInsertionMark insertionMark) {
            if (!base.Enabled || insertionMark.IsEmpty || insertionMark.InsertionLocation.IsEmpty || insertionMark.InsertionLocation.DropPart == BetterListViewDropPart.On) {
                return;
            }
            Point location;
            int num;
            bool flag;
            if (insertionMark.InsertionLocation.Address.IsGroup) {
                BetterListViewGroup groupFromAddressInternal = this.GetGroupFromAddressInternal(insertionMark.InsertionLocation.Address, allowDefaultGroup: true);
                BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)groupFromAddressInternal).LayoutBounds.Clone();
                betterListViewGroupBounds.Offset(this.OffsetContentFromAbsolute);
                int groupContentOffset = this.LayoutGroupsCurrent.GetGroupContentOffset(groupFromAddressInternal);
                betterListViewGroupBounds.Offset(new Point(groupContentOffset, 0));
                location = new Point(betterListViewGroupBounds.BoundsSelection.Left, betterListViewGroupBounds.BoundsSelection.Bottom - 1);
                num = betterListViewGroupBounds.BoundsSelection.Width - groupContentOffset;
                flag = true;
            }
            else {
                BetterListViewItem itemFromAddress = this.GetItemFromAddress(insertionMark.InsertionLocation.Address);
                if (((IBetterListViewStateElement)itemFromAddress).State != BetterListViewElementState.ActiveVisible) {
                    return;
                }
                Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)itemFromAddress).LayoutBounds.Clone();
                betterListViewItemBounds.Offset(offsetContentFromAbsolute);
                bool flag2 = this.View == BetterListViewView.Details;
                if (flag2 && insertionMark.InsertionLocation.Level != -1 && insertionMark.InsertionLocation.Level < itemFromAddress.Level) {
                    BetterListViewItem betterListViewItem = itemFromAddress;
                    if (insertionMark.InsertionLocation.Level != -1) {
                        while (betterListViewItem.ParentItem != null && insertionMark.InsertionLocation.Level < betterListViewItem.Level) {
                            betterListViewItem = betterListViewItem.ParentItem;
                        }
                    }
                    BetterListViewItemBounds betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.Clone();
                    betterListViewItemBounds2.Offset(offsetContentFromAbsolute);
                    location = new Point(betterListViewItemBounds2.BoundsOuterExtended.Left, insertionMark.ShowAfterItem ? betterListViewItemBounds.BoundsSpacing.Bottom : betterListViewItemBounds.BoundsOuterExtended.Top);
                    num = betterListViewItemBounds2.BoundsOuterExtended.Width;
                }
                else if (flag2 && insertionMark.InsertionLocation.DropPart == BetterListViewDropPart.Inside) {
                    int childItemOffset = this.LayoutItemsCurrent.GetChildItemOffset(graphics, this.Indent, itemFromAddress.Level + 1);
                    location = new Point(childItemOffset, betterListViewItemBounds.BoundsSpacing.Bottom);
                    num = Math.Max(betterListViewItemBounds.BoundsOuterExtended.Width - childItemOffset, 0);
                }
                else if (this.LayoutItemsCurrent.DirectionVertical) {
                    location = (insertionMark.ShowAfterItem ? new Point(betterListViewItemBounds.BoundsOuterExtended.Left, betterListViewItemBounds.BoundsSpacing.Bottom) : betterListViewItemBounds.BoundsOuterExtended.Location);
                    num = betterListViewItemBounds.BoundsOuterExtended.Width;
                }
                else {
                    location = (insertionMark.ShowAfterItem ? new Point(betterListViewItemBounds.BoundsSpacing.Right, betterListViewItemBounds.BoundsOuterExtended.Top) : betterListViewItemBounds.BoundsOuterExtended.Location);
                    num = betterListViewItemBounds.BoundsOuterExtended.Height;
                }
                flag = this.LayoutItemsCurrent.DirectionVertical;
            }
            int num2 = Math.Min(insertionMark.InsertionLocation.Offset, num);
            if (num2 != 0) {
                location = (flag ? new Point(location.X + num2, location.Y) : new Point(location.X, location.Y + num2));
                num -= num2;
            }
            Color color = insertionMark.Color;
            if (color.IsEmpty) {
                color = this.ColorInsertionMark;
                if (color.IsEmpty) {
                    color = BetterListViewInsertionMark.DefaultColor;
                }
            }
            BetterListViewDrawInsertionMarkEventArgs eventArgs = new BetterListViewDrawInsertionMarkEventArgs(graphics, insertionMark.InsertionLocation, location, num, flag, color, insertionMark.Enabled);
            this.OnDrawInsertionMark(eventArgs);
        }

        private void PaintItemBackground(Graphics graphics, BetterListViewItem item, BetterListViewItemBounds itemBounds) {
            this.OnDrawItemBackground(new BetterListViewDrawItemBackgroundEventArgs(graphics, item, itemBounds));
        }

        private void PaintItemForeground(Graphics graphics, BetterListViewItem item, BetterListViewItemBounds itemBounds) {
            BetterListViewHitTestLocations locations = ((this.HitTestInfo.ItemDisplay == item) ? this.HitTestInfo.Locations : ((this.HitTestInfo.ItemSelection != null && this.HitTestInfo.ItemSelection == item.SelectableItem && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemSelection) == BetterListViewHitTestLocations.ItemSelection) ? BetterListViewHitTestLocations.ItemSelection : BetterListViewHitTestLocations.Custom));
            BetterListViewItemStateInfo itemStateInfo = this.GetItemStateInfo(item, locations);
            this.OnDrawItem(new BetterListViewDrawItemEventArgs(graphics, base.Enabled, this.Focused, item, itemBounds, itemStateInfo));
        }

        private void GetPainterElementsItem(bool drawEnabled, bool drawFocused, BetterListViewItem item, BetterListViewItemStateInfo itemStateInfo, out BetterListViewPainterElementItem painterElementItem, out BetterListViewPainterElementNode painterElementNode) {
            BetterListViewItemState betterListViewItemState = itemStateInfo.ItemState;
            if (this.HideSelectionMode == BetterListViewHideSelectionMode.Hide && !drawFocused) {
                betterListViewItemState &= ~(BetterListViewItemState.Focused | BetterListViewItemState.Selected);
            }
            if (this.StateInfo.State == BetterListViewState.ItemDrag && this.GetDragDropDisplay(item) != BetterListViewDragDropDisplay.Highlight) {
                betterListViewItemState &= ~BetterListViewItemState.Hot;
            }
            painterElementItem = BetterListViewPainterElementItem.Undefined;
            bool flag = (betterListViewItemState & BetterListViewItemState.Selected) == BetterListViewItemState.Selected;
            bool flag2 = (betterListViewItemState & BetterListViewItemState.Hot) == BetterListViewItemState.Hot;
            bool flag3 = (betterListViewItemState & BetterListViewItemState.Focused) == BetterListViewItemState.Focused;
            bool flag4 = this.StateInfo.State == BetterListViewState.ItemDrag && (betterListViewItemState & BetterListViewItemState.DropHighlight) == BetterListViewItemState.DropHighlight;
            if (drawEnabled) {
                if (flag4) {
                    if (flag) {
                        painterElementItem = BetterListViewPainterElementItem.SelectedFocused;
                    }
                    else {
                        painterElementItem = BetterListViewPainterElementItem.Selected;
                    }
                }
                else if (flag) {
                    if (flag2) {
                        if (drawFocused) {
                            painterElementItem = (flag3 ? BetterListViewPainterElementItem.SelectedHotFocused : BetterListViewPainterElementItem.SelectedHot);
                        }
                        else {
                            painterElementItem = ((this.HideSelectionMode == BetterListViewHideSelectionMode.KeepSelection) ? BetterListViewPainterElementItem.SelectedHot : BetterListViewPainterElementItem.SelectedHotUnfocused);
                        }
                    }
                    else if (drawFocused) {
                        painterElementItem = (flag3 ? BetterListViewPainterElementItem.SelectedFocused : BetterListViewPainterElementItem.Selected);
                    }
                    else {
                        painterElementItem = ((this.HideSelectionMode == BetterListViewHideSelectionMode.KeepSelection) ? BetterListViewPainterElementItem.Selected : BetterListViewPainterElementItem.SelectedUnfocused);
                    }
                }
                else if (flag2) {
                    painterElementItem = (flag3 ? BetterListViewPainterElementItem.HotFocused : BetterListViewPainterElementItem.Hot);
                }
                else if (flag3) {
                    painterElementItem = BetterListViewPainterElementItem.Focused;
                }
            }
            else if (flag) {
                painterElementItem = BetterListViewPainterElementItem.Disabled;
            }
            painterElementNode = BetterListViewPainterElementNode.Undefined;
        }

        /// <summary>
        ///   Raises the ColumnClick event.
        /// </summary>
        /// <param name="eventArgs">The ColumnClickEventArgs instance containing the event data.</param>
        protected virtual void OnColumnClick(BetterListViewColumnClickEventArgs eventArgs) {
            if (this.ColumnClick != null) {
                this.ColumnClick(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the ColumnClicked event.
        /// </summary>
        /// <param name="eventArgs">The ColumnClickedEventArgs instance containing the event data.</param>
        protected virtual void OnColumnClicked(BetterListViewColumnClickedEventArgs eventArgs) {
            if (this.ColumnClicked != null) {
                this.ColumnClicked(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the HitTestChanged event.
        /// </summary>
        /// <param name="eventArgs">The HitTestChangedEventArgs instance containing the event data.</param>
        protected virtual void OnHitTestChanged(BetterListViewHitTestChangedEventArgs eventArgs) {
            if (this.HitTestChanged != null) {
                this.HitTestChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the ItemActivate event.
        /// </summary>
        /// <param name="eventArgs">The ItemActivateEventArgs instance containing the event data.</param>
        protected virtual void OnItemActivate(BetterListViewItemActivateEventArgs eventArgs) {
            if (this.ItemActivate != null) {
                this.ItemActivate(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the ItemCheck event.
        /// </summary>
        /// <param name="eventArgs">The ItemCheckEventArgs instance containing the event data.</param>
        protected virtual void OnItemCheck(BetterListViewItemCheckEventArgs eventArgs) {
            if (this.ItemCheck != null) {
                this.ItemCheck(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the ItemChecked event.
        /// </summary>
        /// <param name="eventArgs">The ItemCheckedEventArgs instance containing the event data.</param>
        protected virtual void OnItemChecked(BetterListViewItemCheckedEventArgs eventArgs) {
            if (this.ItemChecked != null) {
                this.ItemChecked(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the ItemSearch event.
        /// </summary>
        /// <param name="eventArgs">The ItemSearchEventArgs instance containing the event data.</param>
        protected virtual void OnItemSearch(BetterListViewItemSearchEventArgs eventArgs) {
            if (this.ItemSearch != null) {
                this.ItemSearch(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the RequestEmbeddedControl event.
        /// </summary>
        /// <param name="eventArgs">The RequestEmbeddedControlEventArgs instance containing the event data.</param>
        /// <returns>Requested embedded control instance.</returns>
        protected virtual IBetterListViewEmbeddedControl OnRequestEmbeddedControl(BetterListViewRequestEmbeddedControlEventArgs eventArgs) {
            int num = ((eventArgs != null && eventArgs.SubItem != null) ? eventArgs.SubItem.Index : 0);
            if ((num == 0 && this.LabelEditModeItems == BetterListViewLabelEditMode.CustomControl) || (num != 0 && this.LabelEditModeSubItems == BetterListViewLabelEditMode.CustomControl)) {
                if (this.RequestEmbeddedControl != null) {
                    return this.RequestEmbeddedControl(this, eventArgs);
                }
                return null;
            }
            if ((num == 0 && this.LabelEditModeItems == BetterListViewLabelEditMode.Text) || (num != 0 && this.LabelEditModeSubItems == BetterListViewLabelEditMode.Text)) {
                //eventArgs.ControlPlacement = new BetterListViewEmbeddedControlPlacement(alignment: (eventArgs.SubItem == null) ? System.Drawing.ContentAlignment.MiddleLeft : (((eventArgs.SubItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? this.LayoutItemsCurrent.DefaultTextAlignmentHorizontal : eventArgs.SubItem.AlignHorizontal) switch {
                //    TextAlignmentHorizontal.Center => System.Drawing.ContentAlignment.MiddleLeft,
                //    TextAlignmentHorizontal.Right => System.Drawing.ContentAlignment.MiddleRight,
                //    _ => System.Drawing.ContentAlignment.MiddleLeft,
                //}), boundsText: eventArgs.ControlPlacement.BoundsText, boundsCell: eventArgs.ControlPlacement.BoundsCell, useCellBounds: false);
                System.Drawing.ContentAlignment aligment;
                if ((eventArgs.SubItem == null)) {
                    aligment = System.Drawing.ContentAlignment.MiddleLeft;
                }
                else {
                    if ((eventArgs.SubItem.AlignHorizontal == TextAlignmentHorizontal.Default)) {
                        switch (this.LayoutItemsCurrent.DefaultTextAlignmentHorizontal) {
                            case TextAlignmentHorizontal.Center: aligment = System.Drawing.ContentAlignment.MiddleLeft; break;
                            case TextAlignmentHorizontal.Right: aligment = System.Drawing.ContentAlignment.MiddleRight; break;
                            default: aligment = System.Drawing.ContentAlignment.MiddleLeft; break;
                        }
                    }
                    else {
                        switch (eventArgs.SubItem.AlignHorizontal) {
                            case TextAlignmentHorizontal.Center: aligment = System.Drawing.ContentAlignment.MiddleLeft; break;
                            case TextAlignmentHorizontal.Right: aligment = System.Drawing.ContentAlignment.MiddleRight; break;
                            default: aligment = System.Drawing.ContentAlignment.MiddleLeft; break;
                        }
                    }
                }
                eventArgs.ControlPlacement = new BetterListViewEmbeddedControlPlacement(eventArgs.ControlPlacement.BoundsText, eventArgs.ControlPlacement.BoundsCell, false, aligment);

                return new BetterListViewTextBoxEmbeddedControl();
            }
            return null;
        }

        /// <summary>
        ///   Raises the DrawBackground event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawBackgroundEventArgs instance containing the event data.</param>
        protected virtual void OnDrawBackground(BetterListViewDrawBackgroundEventArgs eventArgs) {
            BetterListViewPainter.DrawBackground(eventArgs, this.BackgroundImage, this.BackgroundImageAlignment, this.BackgroundImageLayout, this.BackgroundImageOpacity, this.BackColor, this.ColorSortedColumn, base.Enabled);
            if (this.DrawBackground != null) {
                this.DrawBackground(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawColumnHeader event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawColumnHeaderEventArgs instance containing the event data.</param>
        protected virtual void OnDrawColumnHeader(BetterListViewDrawColumnHeaderEventArgs eventArgs) {
            if (eventArgs.ColumnHeader.GetStyle(this) != 0) {
                BetterListViewPainter.DrawColumnHeaderForeground(eventArgs, this.ImageListColumns, base.Enabled ? eventArgs.ColumnHeaderStateInfo.SortOrder : BetterListViewSortOrder.None, base.Enabled, this.LayoutColumns.MaximumTextLines);
            }
            if (this.DrawColumnHeader != null) {
                this.DrawColumnHeader(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawColumnHeaderBackground event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawColumnHeaderBackgroundEventArgs instance containing the event data.</param>
        protected virtual void OnDrawColumnHeaderBackground(BetterListViewDrawColumnHeaderBackgroundEventArgs eventArgs) {
            this.GetPainterElementsColumnHeader(eventArgs.ColumnHeader.Index, eventArgs.ColumnHeaderStateInfo, out var painterElementColumnHeader);
            BetterListViewPainter.DrawColumnHeaderBackground(eventArgs.Graphics, eventArgs.ColumnHeaderBounds.BoundsOuter, painterElementColumnHeader);
            if (this.DrawColumnHeaderBackground != null) {
                this.DrawColumnHeaderBackground(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawGroup event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawGroupEventArgs instance containing the event data.</param>
        protected virtual void OnDrawGroup(BetterListViewDrawGroupEventArgs eventArgs) {
            this.GetPainterElementsGroup(eventArgs.Group, eventArgs.GroupStateInfo, out var painterElementGroup, out var painterElementGroupExpandButton);
            BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
            BetterListViewPainter.DrawGroupForeground(eventArgs, this.ImageListGroups, layoutGroupsCurrent.DefaultTextAlignmentHorizontal, layoutGroupsCurrent.DefaultImageAlignmentHorizontal, painterElementGroup, painterElementGroupExpandButton, this.BackColor, base.Enabled);
            if (this.DrawGroup != null) {
                this.DrawGroup(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawGroupBackground event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawGroupBackgroundEventArgs instance containing the event data.</param>
        protected virtual void OnDrawGroupBackground(BetterListViewDrawGroupBackgroundEventArgs eventArgs) {
            BetterListViewGroupStateInfo groupStateInfo = this.GetGroupStateInfo(eventArgs.Group, (this.HitTestInfo.Group == eventArgs.Group) ? this.HitTestInfo.Locations : BetterListViewHitTestLocations.Custom);
            this.GetPainterElementsGroup(eventArgs.Group, groupStateInfo, out var _, out var _);
            BetterListViewPainter.DrawGroupBackground(eventArgs.Graphics, eventArgs.Group, eventArgs.GroupBounds);
            if (this.DrawGroupBackground != null) {
                this.DrawGroupBackground(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawInsertionMark event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawInsertionMarkEventArgs instance containing the event data.</param>
        protected virtual void OnDrawInsertionMark(BetterListViewDrawInsertionMarkEventArgs eventArgs) {
            BetterListViewBasePainter.DrawInsertionMark(eventArgs.Graphics, eventArgs.Location, eventArgs.Length, eventArgs.IsHorizontal, eventArgs.Color, eventArgs.IsEnabled);
            if (this.DrawInsertionMark != null) {
                this.DrawInsertionMark(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawItem event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawItemEventArgs instance containing the event data.</param>
        protected virtual void OnDrawItem(BetterListViewDrawItemEventArgs eventArgs) {
            this.GetPainterElementsItem(eventArgs.DrawEnabled, eventArgs.DrawFocused, eventArgs.Item, eventArgs.ItemStateInfo, out var painterElementItem, out var painterElementNode);
            if (eventArgs.DrawSelection && painterElementItem != BetterListViewPainterElementItem.Undefined && eventArgs.Item == eventArgs.Item.SelectableItem) {
                BetterListViewSelectionRenderingOptions betterListViewSelectionRenderingOptions = BetterListViewSelectionRenderingOptions.None;
                if (this.AllowDisplayFocusRectangle) {
                    betterListViewSelectionRenderingOptions |= BetterListViewSelectionRenderingOptions.AllowClassicFocusRectangle;
                }
                if (this.LayoutItemsCurrent.SingleBoundary && (eventArgs.ItemStateInfo.ItemState & BetterListViewItemState.DropHighlight) != BetterListViewItemState.DropHighlight) {
                    betterListViewSelectionRenderingOptions |= BetterListViewSelectionRenderingOptions.ExtendVertical;
                }
                BetterListViewPainter.DrawItemSelection(eventArgs.Graphics, eventArgs.ItemBounds, painterElementItem, this.BackColor, betterListViewSelectionRenderingOptions, (this.Focused && !this.FocusInfo.IsEmpty && this.FocusInfo.Element == eventArgs.Item) ? this.FocusInfo.ColumnIndex : (-1));
            }
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            BetterListViewPainter.DrawItemForeground(eventArgs, this.ImageListCurrent, layoutItemsCurrent.ImageBorderType, layoutItemsCurrent.ImageBorderThickness, layoutItemsCurrent.ImageBorderColor, painterElementItem, painterElementNode, eventArgs.ItemStateInfo.CheckBoxState, (this.View != BetterListViewView.Details) ? eventArgs.Item.SubItems.Count : ((!this.ColumnsVisible) ? 1 : this.Columns.Count), base.Enabled, this.View != BetterListViewView.Details || this.FullRowSelect, (this.StateInfo.State == BetterListViewState.LabelEdit && eventArgs.Item == this.StateInfo.LabelEditStateInfo.SubItem.Item) ? this.StateInfo.LabelEditStateInfo.SubItem.Index : (-1), this.CacheImages);
            if (this.DrawItem != null) {
                this.DrawItem(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the DrawItemBackground event.
        /// </summary>
        /// <param name="eventArgs">The BetterListViewDrawItemBackgroundEventArgs instance containing the event data.</param>
        protected virtual void OnDrawItemBackground(BetterListViewDrawItemBackgroundEventArgs eventArgs) {
            if (base.Enabled) {
                BetterListViewPainter.DrawItemBackground(eventArgs.Graphics, eventArgs.Item, eventArgs.ItemBounds, this.ColumnsVisible ? this.Columns.Count : 0);
            }
            if (this.DrawItemBackground != null) {
                this.DrawItemBackground(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the AfterItemSort event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewAfterItemSortEventArgs" /> instance containing the event data.</param>
        protected virtual void OnAfterItemSort(BetterListViewAfterItemSortEventArgs eventArgs) {
            if (this.AfterItemSort != null) {
                this.AfterItemSort(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the AfterLabelEdit event.
        /// </summary>
        /// <param name="eventArgs">The LabelEditEventArgs instance containing the event data.</param>
        protected virtual void OnAfterLabelEdit(BetterListViewLabelEditEventArgs eventArgs) {
            if (this.AfterLabelEdit != null) {
                this.AfterLabelEdit(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the AfterLabelEditCancel event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> instance containing the event data.</param>
        protected virtual void OnAfterLabelEditCancel(BetterListViewLabelEditCancelEventArgs eventArgs) {
            if (this.AfterLabelEditCancel != null) {
                this.AfterLabelEditCancel(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the BeforeItemSort event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewBeforeItemSortEventArgs" /> instance containing the event data.</param>
        protected virtual void OnBeforeItemSort(BetterListViewBeforeItemSortEventArgs eventArgs) {
            if (this.BeforeItemSort != null) {
                this.BeforeItemSort(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the BeforeLabelEdit event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> instance containing the event data.</param>
        protected virtual void OnBeforeLabelEdit(BetterListViewLabelEditCancelEventArgs eventArgs) {
            if (this.BeforeLabelEdit != null) {
                this.BeforeLabelEdit(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the CheckedItemsChanged event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewCheckedItemsChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnCheckedItemsChanged(BetterListViewCheckedItemsChangedEventArgs eventArgs) {
            if (this.CheckedItemsChanged != null) {
                this.CheckedItemsChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.FocusedItemChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The FocusedItemChangedEventArgs instance containing the event data.</param>
        protected virtual void OnFocusedItemChanged(BetterListViewFocusedItemChangedEventArgs eventArgs) {
            if (this.FocusedItemChanged != null) {
                this.FocusedItemChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.BeforeLabelEdit" /> event.
        /// </summary>
        /// <param name="eventArgs">The LabelEditEventArgs instance containing the event data.</param>
        protected virtual void OnItemSelectionChanged(BetterListViewItemSelectionChangedEventArgs eventArgs) {
            if (this.ItemSelectionChanged != null) {
                this.ItemSelectionChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectedIndexChanged(object sender, EventArgs eventArgs) {
            if (this.SelectedIndexChanged != null) {
                this.SelectedIndexChanged(sender, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the SelectedItemsChanged event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewSelectedItemsChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectedItemsChanged(BetterListViewSelectedItemsChangedEventArgs eventArgs) {
            if (this.SelectedItemsChanged != null) {
                this.SelectedItemsChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.DataSourceChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDataSourceChanged(EventArgs eventArgs) {
            if (this.DataSourceChanged != null) {
                this.DataSourceChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.DisplayMemberChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDisplayMemberChanged(EventArgs eventArgs) {
            if (this.DisplayMemberChanged != null) {
                this.DisplayMemberChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.BeforeDrag" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewBeforeDragEventArgs" /> instance containing the event data.</param>
        protected virtual void OnBeforeDrag(BetterListViewBeforeDragEventArgs eventArgs) {
            if (this.BeforeDrag != null) {
                this.BeforeDrag(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.DragDropEffectSetting" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewDragDropEffectSettingEventArgs" /> instance containing the event data.</param>
        protected virtual void OnDragDropEffectSetting(BetterListViewDragDropEffectSettingEventArgs eventArgs) {
            if (eventArgs.IsItemReorder && (eventArgs.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                eventArgs.Effect = DragDropEffects.Move;
            }
            else {
                eventArgs.Effect = DragDropEffects.None;
            }
            if (this.DragDropEffectSetting != null) {
                this.DragDropEffectSetting(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.DragDropException" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewDragDropExceptionEventArgs" /> instance containing the event data.</param>
        protected virtual void OnDragDropException(BetterListViewDragDropExceptionEventArgs eventArgs) {
            if (this.DragDropException != null) {
                this.DragDropException(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ItemDrag" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewItemDragEventArgs" /> instance containing the event data.</param>
        protected virtual void OnItemDrag(BetterListViewItemDragEventArgs eventArgs) {
            if (this.ItemDrag != null) {
                this.ItemDrag(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ItemDrop" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewItemDropEventArgs" /> instance containing the event data.</param>
        protected virtual void OnItemDrop(BetterListViewItemDropEventArgs eventArgs) {
            if (this.ItemDrop != null) {
                this.ItemDrop(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ItemMouseHover" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewItemMouseHoverEventArgs" /> instance containing the event data.</param>
        protected virtual void OnItemMouseHover(BetterListViewItemMouseHoverEventArgs eventArgs) {
            if (this.ItemMouseHover != null) {
                this.ItemMouseHover(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ColumnWidthChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnWidthChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnColumnWidthChanged(BetterListViewColumnWidthChangedEventArgs eventArgs) {
            if (this.ColumnWidthChanged != null) {
                this.ColumnWidthChanged(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ColumnWidthChanging" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnWidthChangingEventArgs" /> instance containing the event data.</param>
        protected virtual void OnColumnWidthChanging(BetterListViewColumnWidthChangingEventArgs eventArgs) {
            if (this.ColumnWidthChanging != null) {
                this.ColumnWidthChanging(this, eventArgs);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListView.ViewChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewViewChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnViewChanged(BetterListViewViewChangedEventArgs eventArgs) {
            if (this.ViewChanged != null) {
                this.ViewChanged(this, eventArgs);
            }
        }

        private void GroupsInitialize() {
            this.defaultGroup = new BetterListViewGroup(null, "Default", TextAlignmentHorizontal.Left, this);
            try {
                base.BeginUpdate();
                this.defaultGroup.AllowShowExpandButton = false;
            }
            finally {
                base.EndUpdate(suppressRefresh: true);
            }
        }

        private BetterListViewGroup GetVisibleGroup(BetterListViewItem item, bool allowNull) {
            BetterListViewGroup betterListViewGroup;
            if (this.ShowGroups) {
                betterListViewGroup = item.Group ?? this.defaultGroup;
            }
            else {
                if (allowNull) {
                    return null;
                }
                betterListViewGroup = this.defaultGroup;
            }
            if (allowNull && betterListViewGroup == this.defaultGroup && !this.ShowDefaultGroupHeader) {
                return null;
            }
            return betterListViewGroup;
        }

        private IEnumerable<BetterListViewGroup> GetSelectedGroups(IEnumerable<BetterListViewItem> selectedItems) {
            var sortedSet = new Collections.SortedSet<BetterListViewGroup>(BetterListViewElementBaseIndexComparer<BetterListViewGroup>.Instance);
            foreach (BetterListViewItem selectedItem in selectedItems) {
                BetterListViewGroup item = selectedItem.Group ?? this.defaultGroup;
                if (!sortedSet.Contains(item)) {
                    sortedSet.Add(item);
                }
            }
            return sortedSet;
        }

        private bool CanFocusGroupKeyboard(BetterListViewGroup group) {
            if (!this.ShowGroups) {
                return false;
            }
            if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.KeyboardFocus) != BetterListViewGroupHeaderBehavior.KeyboardFocus) {
                return false;
            }
            if (group == this.defaultGroup && !this.ShowDefaultGroupHeader) {
                return false;
            }
            return true;
        }

        /// <summary>
        ///   Remove all elements from the list.
        /// </summary>
        public void Clear() {
            this.Clear(itemsOnly: false);
        }

        /// <summary>
        ///   Remove all elements from the list.
        /// </summary>
        /// <param name="itemsOnly">Clear items only.</param>
        public void Clear(bool itemsOnly) {
            try {
                base.BeginUpdate();
                if (!itemsOnly) {
                    this.Columns.Clear();
                    this.Groups.Clear();
                }
                this.Items.Clear();
            }
            finally {
                base.EndUpdate();
            }
        }

        /// <summary>
        ///   Sort items in the list.
        /// </summary>
        public void Sort() {
            this.SortItems(columnClicked: false, sortAlways: true);
        }

        /// <summary>
        ///   Sort items in the list.
        /// </summary>
        /// <param name="columnIndex">Index of the column to sort by.</param>
        public void Sort(int columnIndex) {
            Checks.CheckTrue(this.Columns.Count != 0, "this.Columns.Count != 0");
            Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            bool ascendingOrder = this.Columns[columnIndex].SortOrder != BetterListViewSortOrder.Descending;
            this.SortItems(columnIndex, BetterListViewSortOptions.None, ascendingOrder);
            this.RefreshView();
        }

        /// <summary>
        ///   Sort items in the list.
        /// </summary>
        /// <param name="columnIndex">Index of the column to sort by.</param>
        /// <param name="ascendingOrder">Sort in ascending order (descending, otherwise).</param>
        public void Sort(int columnIndex, bool ascendingOrder) {
            Checks.CheckTrue(this.Columns.Count != 0, "this.Columns.Count != 0");
            Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            this.SortItems(columnIndex, BetterListViewSortOptions.None, ascendingOrder);
            this.RefreshView();
        }

        /// <summary>
        ///   Reset sorting.
        /// </summary>
        public void Unsort() {
            this.sortList.Clear();
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Perform default action on the specified item.
        /// </summary>
        /// <param name="item">Item to perform default action on.</param>
        protected internal void DoDefaultAction(BetterListViewItem item) {
            Checks.CheckNotNull(item, "item");
            Checks.CheckTrue(item.ListView == this, "ReferenceEquals(item.ListView, this)");
            this.OnItemActivate(new BetterListViewItemActivateEventArgs(item, BetterListViewItemActivationSource.Accessibility));
        }

        private static CheckState CycleCheckState(CheckState checkState, bool threeState) {
            if (threeState) {
                switch (checkState) {
                    case CheckState.Checked:
                        return CheckState.Indeterminate;
                    case CheckState.Indeterminate:
                        return CheckState.Unchecked;
                    case CheckState.Unchecked:
                        return CheckState.Checked;
                }
            }
            if (checkState != CheckState.Checked) {
                return CheckState.Checked;
            }
            return CheckState.Unchecked;
        }

        private void CheckItems(BetterListViewItem featuredItem, IEnumerable<BetterListViewItem> selectedItems, BetterListViewCheckStateChangeMode checkStateChangeMode) {
            BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection();
            foreach (BetterListViewItem selectedItem in selectedItems) {
                if (selectedItem.CheckBoxAppearance != 0) {
                    betterListViewItemCollection.Add(selectedItem);
                }
            }
            if (betterListViewItemCollection.Count == 0) {
                return;
            }
            betterListViewItemCollection.Sort(BetterListViewItemIndexComparer.Instance);
            CheckState checkState = BetterListView.CycleCheckState(featuredItem.CheckState, threeState: false);
            List<CheckState> list = new List<CheckState>();
            if (this.GroupItemCheck) {
                foreach (BetterListViewItem item in betterListViewItemCollection) {
                    if (item.CheckState != checkState) {
                        list.Add(item.CheckState);
                    }
                }
            }
            else {
                list.Add(featuredItem.CheckState);
            }
            BetterListViewItemCheckEventArgs betterListViewItemCheckEventArgs = new BetterListViewItemCheckEventArgs(featuredItem, betterListViewItemCollection, list.AsReadOnly(), checkState, checkStateChangeMode);
            this.OnItemCheck(betterListViewItemCheckEventArgs);
            if (betterListViewItemCheckEventArgs.Cancel) {
                return;
            }
            try {
                base.BeginUpdate();
                if (this.GroupItemCheck) {
                    foreach (BetterListViewItem selectedItem2 in selectedItems) {
                        selectedItem2.SetCheckState((checkState == CheckState.Checked) ? CheckState.Checked : CheckState.Unchecked, checkStateChangeMode, quiet: false);
                    }
                }
                else {
                    featuredItem.SetCheckState((checkState == CheckState.Checked) ? CheckState.Checked : CheckState.Unchecked, checkStateChangeMode, quiet: false);
                }
                this.SetHitTestInfo(this.HitTest());
            }
            finally {
                base.EndUpdate(suppressRefresh: true);
            }
            this.OnCheckedItemsChanged(new BetterListViewCheckedItemsChangedEventArgs(featuredItem, betterListViewItemCollection, list.AsReadOnly(), checkState, checkStateChangeMode));
        }

        private void SortItems(int columnIndex, BetterListViewSortOptions sortOptions) {
            Checks.CheckTrue(this.Columns.Count != 0, "this.Columns.Count != 0");
            Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            bool ascendingOrder = this.Columns[columnIndex].SortOrder != BetterListViewSortOrder.Ascending;
            this.SortItems(columnIndex, sortOptions, ascendingOrder);
        }

        private void SortItems(int columnIndex, BetterListViewSortOptions sortOptions, bool ascendingOrder) {
            bool flag = (sortOptions & BetterListViewSortOptions.AddColumn) == BetterListViewSortOptions.AddColumn;
            bool flag2 = (sortOptions & BetterListViewSortOptions.RemoveColumn) == BetterListViewSortOptions.RemoveColumn;
            bool columnClicked = (sortOptions & BetterListViewSortOptions.ColumnClicked) == BetterListViewSortOptions.ColumnClicked;
            BetterListViewBeforeItemSortEventArgs betterListViewBeforeItemSortEventArgs = new BetterListViewBeforeItemSortEventArgs(this.sortList, columnClicked);
            this.OnBeforeItemSort(betterListViewBeforeItemSortEventArgs);
            if (!betterListViewBeforeItemSortEventArgs.Cancel) {
                if (flag) {
                    this.sortList.Add(columnIndex, ascendingOrder);
                }
                else if (flag2) {
                    this.sortList.Remove(columnIndex);
                }
                else {
                    this.sortList.Set(columnIndex, ascendingOrder);
                }
                this.SortItems(columnClicked, sortAlways: false);
            }
        }

        private void SortItems(bool columnClicked, bool sortAlways) {
            this.itemComparer.SetSortList(this.sortList, this.Columns, sortAlways);
            if (this.SortVirtual) {
                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw, this.BoundsColumnHeaders);
                this.RefreshView();
            }
            else {
                this.Items.Sort(this.itemComparer);
            }
            this.OnAfterItemSort(new BetterListViewAfterItemSortEventArgs(columnClicked, this.sortList));
        }

        /// <summary>
        ///   Determines if a character is an input character that the control recognizes.
        /// </summary>
        /// <returns>
        ///   true if the character should be sent directly to the control and not preprocessed; otherwise, false.
        /// </returns>
        /// <param name="charCode">The character to test. 
        /// </param>
        protected override bool IsInputChar(char charCode) {
            if (!base.IsUpdating) {
                return this.AllowKeyboardSearch;
            }
            return false;
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.KeyPress" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyPressEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnKeyPress(KeyPressEventArgs e) {
            if (!base.IsUpdating && !this.ReadOnly && this.AllowKeyboardSearch && this.PerformSearch(e.KeyChar)) {
                e.Handled = true;
                this.RefreshView();
            }
            base.OnKeyPress(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.KeyDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e) {
            bool flag = true;
            if (!base.IsUpdating && !this.ReadOnly) {
                if (e.Handled) {
                    return;
                }
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewFocusInfo adjacentFocusInfo = this.FocusInfo;
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                if (betterListViewStateInfo.State == BetterListViewState.ColumnReorder && e.KeyCode == Keys.Escape) {
                    flag = false;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    this.ResetCursor();
                }
                if ((betterListViewStateInfo.State == BetterListViewState.ItemBeforeCheckKeyboard && e.KeyCode != Keys.Space) || betterListViewStateInfo.State == BetterListViewState.ItemBeforeCheckMouse) {
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                }
                bool control = e.Control;
                bool shift = e.Shift;
                bool alt = e.Alt;
                if (e.KeyCode == Keys.Prior) {
                    adjacentFocusInfo = this.GetAdjacentFocusInfo(adjacentFocusInfo, BetterListViewTargetElement.PageUp);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Next) {
                    adjacentFocusInfo = this.GetAdjacentFocusInfo(adjacentFocusInfo, BetterListViewTargetElement.PageDown);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Home) {
                    adjacentFocusInfo = this.GetAdjacentFocusInfo(adjacentFocusInfo, BetterListViewTargetElement.Home);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.End) {
                    adjacentFocusInfo = this.GetAdjacentFocusInfo(adjacentFocusInfo, BetterListViewTargetElement.End);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Apps || e.KeyData == (Keys.F10 | Keys.Shift) || e.KeyData == (Keys.F10 | Keys.Shift | Keys.Control)) {
                    if (this.SelectedItemsSet.Count != 0) {
                        if (this.ContextMenuStripItems != null) {
                            e.Handled = true;
                            this.ContextMenuStripItems.Show(Control.MousePosition);
                        }
                    }
                    else if (this.ContextMenuStrip != null) {
                        e.Handled = true;
                        this.ContextMenuStrip.Show(base.PointToScreen(base.BoundsContent.Location));
                    }
                }
                else if (e.KeyCode == Keys.Return) {
                    if (this.FocusedItem != null) {
                        BetterListViewItemActivateEventArgs eventArgs = new BetterListViewItemActivateEventArgs(this.FocusedItem, BetterListViewItemActivationSource.Keyboard);
                        try {
                            base.BeginUpdate();
                            this.OnItemActivate(eventArgs);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                        betterListViewStateInfo = this.StateInfo;
                        adjacentFocusInfo = this.FocusInfo;
                        selectionInfoNew = this.SelectionInfo;
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.F2 && (this.LabelEditActivation & BetterListViewLabelEditActivation.Keyboard) == BetterListViewLabelEditActivation.Keyboard) {
                    this.SetStateInfo(betterListViewStateInfo);
                    BetterListViewSubItem focusedSubItem = this.FocusedSubItem;
                    if (focusedSubItem != null) {
                        this.BeginEdit(focusedSubItem, immediate: true);
                        betterListViewStateInfo = this.StateInfo;
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }
                else if (e.KeyCode == Keys.Space && !alt) {
                    if (this.PerformSearch(' ')) {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    else if (this.CheckBoxes != 0 && e.Modifiers == Keys.None) {
                        BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection(this.SelectedItemsSet);
                        betterListViewItemCollection.Sort(BetterListViewItemIndexComparer.Instance);
                        BetterListViewReadOnlyItemSet betterListViewReadOnlyItemSet = null;
                        BetterListViewItem betterListViewItem = ((!adjacentFocusInfo.IsEmpty) ? (adjacentFocusInfo.Element as BetterListViewItem) : null);
                        if (betterListViewItem != null) {
                            betterListViewReadOnlyItemSet = ((!this.GroupItemCheck || !betterListViewItemCollection.Contains(betterListViewItem)) ? new BetterListViewReadOnlyItemSet(new BetterListViewItem[1] { betterListViewItem }) : new BetterListViewReadOnlyItemSet(betterListViewItemCollection));
                        }
                        else if (betterListViewItemCollection.Count != 0) {
                            betterListViewItem = betterListViewItemCollection[0];
                            betterListViewReadOnlyItemSet = ((!this.GroupItemCheck) ? new BetterListViewReadOnlyItemSet(new BetterListViewItem[1] { betterListViewItemCollection[0] }) : new BetterListViewReadOnlyItemSet(betterListViewItemCollection));
                        }
                        if (betterListViewItem != null && betterListViewReadOnlyItemSet.Count != 0) {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            betterListViewStateInfo.State = BetterListViewState.ItemBeforeCheckKeyboard;
                            betterListViewStateInfo.ItemBeforeCheckStateInfo = new BetterListViewItemBeforeCheckStateInfo(betterListViewItem, betterListViewReadOnlyItemSet);
                        }
                    }
                    else if (!adjacentFocusInfo.IsEmpty && adjacentFocusInfo.Element is BetterListViewItem) {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        this.MakeSelection(adjacentFocusInfo.Element, control, shift, BetterListViewSelectionOptions.ClearSearchQuery, BetterListViewSelectionChangeMode.Keyboard, ref selectionInfoNew, ref adjacentFocusInfo);
                    }
                }
                if (!adjacentFocusInfo.IsEmpty && adjacentFocusInfo != this.FocusInfo) {
                    try {
                        base.BeginUpdate();
                        if (!control || shift) {
                            this.MakeSelection(adjacentFocusInfo.Element, control, shift, BetterListViewSelectionOptions.TakeFocus | BetterListViewSelectionOptions.ClearSearchQuery, BetterListViewSelectionChangeMode.Keyboard, ref selectionInfoNew, ref adjacentFocusInfo);
                        }
                        this.ScrollToFocus(adjacentFocusInfo, subItemFocus: false);
                    }
                    finally {
                        base.EndUpdate(suppressRefresh: true);
                    }
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetFocusInfo(adjacentFocusInfo, BetterListViewSelectionChangeMode.Keyboard);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.Keyboard);
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
            }
            if (flag) {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnKeyUp(KeyEventArgs e) {
            if (!base.IsUpdating && !this.ReadOnly) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeCheckKeyboard) {
                    BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfo = betterListViewStateInfo.ItemBeforeCheckStateInfo;
                    if (e.KeyCode == Keys.Space) {
                        BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection();
                        foreach (BetterListViewItem item in itemBeforeCheckStateInfo.Items) {
                            betterListViewItemCollection.Add(item);
                        }
                        try {
                            base.BeginUpdate();
                            this.CheckItems(itemBeforeCheckStateInfo.Item, betterListViewItemCollection, BetterListViewCheckStateChangeMode.Keyboard);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    betterListViewStateInfo.ItemBeforeCheckStateInfo = BetterListViewItemBeforeCheckStateInfo.Empty;
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        ///   Processes a command key.
        /// </summary>
        /// <returns>
        ///   true if the character was processed by the control; otherwise, false.
        /// </returns>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the window message to process. 
        /// </param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process. 
        /// </param>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (this.StateInfo.State == BetterListViewState.LabelEditInit || this.StateInfo.State == BetterListViewState.LabelEdit) {
                return false;
            }
            if (!base.IsUpdating && !this.ReadOnly) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewFocusInfo betterListViewFocusInfo = this.FocusInfo;
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                bool subItemFocus = false;
                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeCheckKeyboard && keyData != Keys.Space) {
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                }
                bool flag = false;
                bool flag2 = (keyData & Keys.Control) == Keys.Control;
                bool flag3 = (keyData & Keys.Shift) == Keys.Shift;
                Keys keys = keyData & ~(Keys.Shift | Keys.Control);
                if (this.Items.Count != 0) {
                    switch (keys) {
                        case Keys.Up:
                            betterListViewFocusInfo = this.GetAdjacentFocusInfo(betterListViewFocusInfo, BetterListViewTargetElement.Up);
                            subItemFocus = betterListViewFocusInfo.ColumnIndex > 0;
                            flag = true;
                            break;
                        case Keys.Down:
                            betterListViewFocusInfo = this.GetAdjacentFocusInfo(betterListViewFocusInfo, BetterListViewTargetElement.Down);
                            subItemFocus = betterListViewFocusInfo.ColumnIndex > 0;
                            flag = true;
                            break;
                        case Keys.Left: {
                                BetterListViewGroup betterListViewGroup2 = betterListViewFocusInfo.Element as BetterListViewGroup;
                                if (!flag && betterListViewGroup2 != null && this.LayoutGroupsCurrent.OrientationVertical) {
                                    int scrollPositionHorizontal3 = base.ScrollPositionHorizontal;
                                    int num3 = Math.Max(base.ScrollPositionHorizontal - base.HScrollBar.SmallChange, base.HScrollBar.Minimum);
                                    if (num3 != scrollPositionHorizontal3) {
                                        try {
                                            base.BeginUpdate();
                                            base.ScrollPositionHorizontal = num3;
                                        }
                                        finally {
                                            base.EndUpdate(suppressRefresh: true);
                                        }
                                    }
                                    flag = true;
                                }
                                BetterListViewItem betterListViewItem3 = betterListViewFocusInfo.Element as BetterListViewItem;
                                if (!flag && betterListViewItem3 != null && this.View == BetterListViewView.Details) {
                                    if (betterListViewFocusInfo.ColumnIndex != 0) {
                                        BetterListViewItem betterListViewItem4 = (BetterListViewItem)betterListViewFocusInfo.Element;
                                        int num4 = betterListViewFocusInfo.ColumnIndex - 1;
                                        //if (this.SubItemFocusBehavior switch {
                                        //    BetterListViewSubItemFocusBehavior.None => false,
                                        //    BetterListViewSubItemFocusBehavior.All => true,
                                        //    BetterListViewSubItemFocusBehavior.Auto => betterListViewItem4.AllowSelectChildItems,
                                        //    _ => throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'."),
                                        //})
                                        bool ret = false;
                                        switch (this.SubItemFocusBehavior) {
                                            case BetterListViewSubItemFocusBehavior.None:
                                                ret = false;
                                                break;
                                            case BetterListViewSubItemFocusBehavior.All:
                                                ret = true;
                                                break;
                                            case BetterListViewSubItemFocusBehavior.Auto:
                                                ret = betterListViewItem4.AllowSelectChildItems;
                                                break;
                                            default:
                                                break;
                                        }
                                        if (ret) {
                                            while (num4 > 0 && !this.Columns[num4].Visible) {
                                                num4--;
                                            }
                                            betterListViewFocusInfo = new BetterListViewFocusInfo(betterListViewItem4, num4);
                                            subItemFocus = true;
                                            flag = true;
                                        }
                                    }
                                    if (!flag) {
                                        BetterListViewGroup visibleGroup = this.GetVisibleGroup(betterListViewItem3, allowNull: true);
                                        if (visibleGroup != null) {
                                            betterListViewFocusInfo = new BetterListViewFocusInfo(visibleGroup);
                                            subItemFocus = false;
                                        }
                                        else {
                                            int scrollPositionHorizontal4 = base.ScrollPositionHorizontal;
                                            int num5 = Math.Max(base.ScrollPositionHorizontal - base.HScrollBar.SmallChange, base.HScrollBar.Minimum);
                                            if (num5 != scrollPositionHorizontal4) {
                                                try {
                                                    base.BeginUpdate();
                                                    base.ScrollPositionHorizontal = num5;
                                                }
                                                finally {
                                                    base.EndUpdate(suppressRefresh: true);
                                                }
                                            }
                                        }
                                        flag = true;
                                    }
                                }
                                if (!flag) {
                                    betterListViewFocusInfo = this.GetAdjacentFocusInfo(betterListViewFocusInfo, BetterListViewTargetElement.Left);
                                    subItemFocus = false;
                                    flag = true;
                                }
                                break;
                            }
                        case Keys.Right: {
                                BetterListViewGroup betterListViewGroup = betterListViewFocusInfo.Element as BetterListViewGroup;
                                if (!flag && betterListViewGroup != null && this.LayoutGroupsCurrent.OrientationVertical) {
                                    int scrollPositionHorizontal = base.ScrollPositionHorizontal;
                                    int num = Math.Min(base.ScrollPositionHorizontal + base.HScrollBar.SmallChange, base.HScrollBar.Maximum - base.HScrollBar.LargeChange + 1);
                                    if (num != scrollPositionHorizontal) {
                                        try {
                                            base.BeginUpdate();
                                            base.ScrollPositionHorizontal = num;
                                        }
                                        finally {
                                            base.EndUpdate(suppressRefresh: true);
                                        }
                                    }
                                    flag = true;
                                }
                                BetterListViewItem betterListViewItem = betterListViewFocusInfo.Element as BetterListViewItem;
                                if (!flag && betterListViewItem != null && this.View == BetterListViewView.Details) {
                                    if ((!this.ShowItemExpandButtons || betterListViewItem.ChildItems.Count == 0 || !betterListViewItem.AllowShowExpandButton || betterListViewItem.IsExpanded) && betterListViewFocusInfo.ColumnIndex < ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds).SubItemBounds.Count - 1) {
                                        BetterListViewItem betterListViewItem2 = (BetterListViewItem)betterListViewFocusInfo.Element;
                                        int i = betterListViewFocusInfo.ColumnIndex + 1;
                                        //if (this.SubItemFocusBehavior switch {
                                        //    BetterListViewSubItemFocusBehavior.None => false,
                                        //    BetterListViewSubItemFocusBehavior.All => true,
                                        //    BetterListViewSubItemFocusBehavior.Auto => betterListViewItem2.AllowSelectChildItems,
                                        //    _ => throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'."),
                                        //})
                                        bool ret = false;
                                        switch (this.SubItemFocusBehavior) {
                                            case BetterListViewSubItemFocusBehavior.None:
                                                ret = false;
                                                break;
                                            case BetterListViewSubItemFocusBehavior.All:
                                                ret = true;
                                                break;
                                            case BetterListViewSubItemFocusBehavior.Auto:
                                                ret = betterListViewItem2.AllowSelectChildItems;
                                                break;
                                            default:
                                                throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'.");
                                                break;
                                        }
                                        if (ret) {
                                            for (; i < this.Columns.Count - 1 && !this.Columns[i].Visible; i++) {
                                            }
                                            betterListViewFocusInfo = new BetterListViewFocusInfo(betterListViewItem2, i);
                                            subItemFocus = true;
                                            flag = true;
                                        }
                                    }
                                    if (!flag) {
                                        int scrollPositionHorizontal2 = base.ScrollPositionHorizontal;
                                        int num2 = Math.Min(base.ScrollPositionHorizontal + base.HScrollBar.SmallChange, base.HScrollBar.Maximum - base.HScrollBar.LargeChange + 1);
                                        if (num2 != scrollPositionHorizontal2) {
                                            try {
                                                base.BeginUpdate();
                                                base.ScrollPositionHorizontal = num2;
                                            }
                                            finally {
                                                base.EndUpdate(suppressRefresh: true);
                                            }
                                        }
                                        flag = true;
                                    }
                                }
                                if (!flag) {
                                    betterListViewFocusInfo = this.GetAdjacentFocusInfo(betterListViewFocusInfo, BetterListViewTargetElement.Right);
                                    subItemFocus = false;
                                    flag = true;
                                }
                                break;
                            }
                    }
                    if (!betterListViewFocusInfo.IsEmpty && betterListViewFocusInfo != this.FocusInfo) {
                        try {
                            base.BeginUpdate();
                            if ((!flag2 || flag3) && (!(betterListViewFocusInfo.Element is BetterListViewGroup) || (this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.KeyboardSelectAndFocus) == BetterListViewGroupHeaderBehavior.KeyboardSelectAndFocus)) {
                                this.MakeSelection(betterListViewFocusInfo.Element, flag2, flag3, BetterListViewSelectionOptions.TakeFocus | BetterListViewSelectionOptions.ClearSearchQuery, BetterListViewSelectionChangeMode.Keyboard, ref selectionInfoNew, ref betterListViewFocusInfo);
                            }
                            this.ScrollToFocus(betterListViewFocusInfo, subItemFocus);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                    }
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.Keyboard);
                this.SetFocusInfo(betterListViewFocusInfo, BetterListViewSelectionChangeMode.Keyboard);
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
                if (flag) {
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ScrollToFocus(BetterListViewFocusInfo focusInfo, bool subItemFocus) {
            if (subItemFocus) {
                Checks.CheckTrue(focusInfo.Element is BetterListViewItem && focusInfo.ColumnIndex != -1, "(focusInfo.Element is BetterListViewItem) && (focusInfo.ColumnIndex != BetterListViewElementBase.IndexUndefined)");
            }
            else {
                Checks.CheckTrue(focusInfo.Element is BetterListViewItem || focusInfo.Element is BetterListViewGroup, "(focusInfo.Element is BetterListViewItem) || (focusInfo.Element is BetterListViewGroup)");
            }
            if (subItemFocus) {
                this.EnsureVisible(((BetterListViewItem)focusInfo.Element).SubItems[focusInfo.ColumnIndex]);
            }
            else {
                this.EnsureVisible(focusInfo.Element);
            }
            if (focusInfo.ColumnIndex == -1 || (focusInfo.ColumnIndex == 0 && !subItemFocus)) {
                this.EnsureVisible(focusInfo.Element);
            }
            else {
                this.EnsureVisible(((BetterListViewItem)focusInfo.Element).SubItems[focusInfo.ColumnIndex]);
            }
        }

        private BetterListViewFocusInfo GetAdjacentFocusInfo(BetterListViewFocusInfo focusInfoCurrent, BetterListViewTargetElement targetElement) {
            if (focusInfoCurrent.IsEmpty) {
                if (this.layoutElementsItemsSelection.Count != 0) {
                    return new BetterListViewFocusInfo(this.layoutElementsItemsSelection[0], 0);
                }
                if (this.ShowGroups && this.layoutElementsGroups.Count != 0 && this.layoutElementsGroups[0] != this.defaultGroup) {
                    return new BetterListViewFocusInfo(this.layoutElementsGroups[0]);
                }
                return BetterListViewFocusInfo.Empty;
            }
            if (focusInfoCurrent.Element is BetterListViewItem) {
                return this.GetAdjacentFocusInfoItem(focusInfoCurrent, targetElement);
            }
            if (focusInfoCurrent.Element is BetterListViewGroup) {
                return this.GetAdjacentFocusInfoGroup(focusInfoCurrent, targetElement);
            }
            throw new ApplicationException("Unknown element type: '" + focusInfoCurrent.Element.GetType().FullName + "'.");
        }

        private BetterListViewFocusInfo GetAdjacentFocusInfoGroup(BetterListViewFocusInfo focusInfoCurrent, BetterListViewTargetElement targetElement) {
            BetterListViewGroup betterListViewGroup = (BetterListViewGroup)focusInfoCurrent.Element;
            BetterListViewFocusInfo result;
            switch (targetElement) {
                case BetterListViewTargetElement.Up: {
                        int layoutIndexSelection = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                        int num;
                        if (this.CircularSelection) {
                            num = ((layoutIndexSelection > 0) ? (layoutIndexSelection - 1) : (this.layoutElementsGroups.Count - 1));
                            BetterListViewGroup betterListViewGroup3 = this.layoutElementsGroups[num];
                            if (betterListViewGroup3 == this.defaultGroup && !this.ShowDefaultGroupHeader && betterListViewGroup3.LayoutIndexItemSelectionLast == -1) {
                                num = this.layoutElementsGroups.Count - 1;
                            }
                        }
                        else {
                            num = layoutIndexSelection - 1;
                        }
                        if (num >= 0 && num != layoutIndexSelection) {
                            BetterListViewGroup betterListViewGroup4 = this.layoutElementsGroups[num];
                            if (betterListViewGroup4.IsExpanded && betterListViewGroup4.LayoutIndexItemSelectionLast != -1) {
                                result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup4.LayoutIndexItemSelectionLast], 0);
                                break;
                            }
                            if (!this.CanFocusGroupKeyboard(betterListViewGroup4)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(betterListViewGroup4);
                            break;
                        }
                        return focusInfoCurrent;
                    }
                case BetterListViewTargetElement.Down: {
                        int layoutIndexSelection3 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                        int num3 = ((!this.CircularSelection) ? (layoutIndexSelection3 + 1) : ((layoutIndexSelection3 < this.layoutElementsGroups.Count - 1) ? (layoutIndexSelection3 + 1) : 0));
                        if (num3 < this.layoutElementsGroups.Count && num3 != layoutIndexSelection3) {
                            BetterListViewGroup group2 = this.layoutElementsGroups[num3];
                            if (betterListViewGroup.IsExpanded && betterListViewGroup.LayoutIndexItemSelectionFirst != -1) {
                                result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup.LayoutIndexItemSelectionFirst], 0);
                                break;
                            }
                            if (!this.CanFocusGroupKeyboard(group2)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(group2);
                        }
                        else {
                            if (!betterListViewGroup.IsExpanded || betterListViewGroup.LayoutIndexItemSelectionFirst == -1) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup.LayoutIndexItemSelectionFirst], 0);
                        }
                        break;
                    }
                case BetterListViewTargetElement.Left: {
                        if (this.LayoutGroupsCurrent.OrientationVertical) {
                            return focusInfoCurrent;
                        }
                        int layoutIndexSelection4 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                        int num4 = ((!this.CircularSelection) ? (layoutIndexSelection4 - 1) : ((layoutIndexSelection4 > 0) ? (layoutIndexSelection4 - 1) : (this.layoutElementsGroups.Count - 1)));
                        if (num4 >= 0 && num4 != layoutIndexSelection4) {
                            BetterListViewGroup group3 = this.layoutElementsGroups[num4];
                            if (!this.CanFocusGroupKeyboard(group3)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(group3);
                            break;
                        }
                        return focusInfoCurrent;
                    }
                case BetterListViewTargetElement.Right: {
                        if (this.LayoutGroupsCurrent.OrientationVertical) {
                            return focusInfoCurrent;
                        }
                        int layoutIndexSelection2 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                        int num2 = ((!this.CircularSelection) ? (layoutIndexSelection2 + 1) : ((layoutIndexSelection2 < this.layoutElementsGroups.Count - 1) ? (layoutIndexSelection2 + 1) : 0));
                        if (num2 < this.layoutElementsGroups.Count && num2 != layoutIndexSelection2) {
                            BetterListViewGroup group = this.layoutElementsGroups[num2];
                            if (!this.CanFocusGroupKeyboard(group)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(group);
                            break;
                        }
                        return focusInfoCurrent;
                    }
                case BetterListViewTargetElement.PageUp: {
                        BetterListViewGroup betterListViewGroup8 = null;
                        for (int num5 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection - 1; num5 >= 0; num5--) {
                            BetterListViewGroup betterListViewGroup9 = this.layoutElementsGroups[num5];
                            if (betterListViewGroup9.LayoutIndexItemSelectionFirst != -1 && this.CanFocusGroupKeyboard(betterListViewGroup9)) {
                                betterListViewGroup8 = betterListViewGroup9;
                                break;
                            }
                        }
                        if (betterListViewGroup8 != null) {
                            BetterListViewItem item2 = this.layoutElementsItemsSelection[(betterListViewGroup.LayoutIndexItemSelectionFirst != -1) ? betterListViewGroup.LayoutIndexItemSelectionFirst : betterListViewGroup8.LayoutIndexItemSelectionLast];
                            return this.GetAdjacentFocusInfoItem(new BetterListViewFocusInfo(item2, 0), BetterListViewTargetElement.PageUp);
                        }
                        if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.KeyboardFocus) == BetterListViewGroupHeaderBehavior.KeyboardFocus) {
                            BetterListViewGroup adjacentElement2 = this.LayoutGroupsCurrent.GetAdjacentElement(this.layoutElementsGroups, ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection, this.layoutMeasurementGroups.ElementsPerRow, this.visibleRangeGroups.ElementCount, 0, this.layoutElementsGroups.Count - 1, BetterListViewTargetElement.PageUp);
                            adjacentElement2 = adjacentElement2 ?? this.layoutElementsGroups[0];
                            if (!this.CanFocusGroupKeyboard(adjacentElement2)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(adjacentElement2);
                            break;
                        }
                        return focusInfoCurrent;
                    }
                case BetterListViewTargetElement.PageDown: {
                        BetterListViewGroup betterListViewGroup5 = null;
                        for (int i = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection + 1; i < this.layoutElementsGroups.Count; i++) {
                            BetterListViewGroup betterListViewGroup6 = this.layoutElementsGroups[i];
                            if (betterListViewGroup6.LayoutIndexItemSelectionFirst != -1 && this.CanFocusGroupKeyboard(betterListViewGroup6)) {
                                betterListViewGroup5 = betterListViewGroup6;
                                break;
                            }
                        }
                        if (betterListViewGroup5 != null) {
                            BetterListViewItem item = this.layoutElementsItemsSelection[(betterListViewGroup.LayoutIndexItemSelectionFirst != -1) ? betterListViewGroup.LayoutIndexItemSelectionFirst : betterListViewGroup5.LayoutIndexItemSelectionLast];
                            return this.GetAdjacentFocusInfoItem(new BetterListViewFocusInfo(item, 0), BetterListViewTargetElement.PageDown);
                        }
                        if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.KeyboardFocus) == BetterListViewGroupHeaderBehavior.KeyboardFocus) {
                            BetterListViewGroup adjacentElement = this.LayoutGroupsCurrent.GetAdjacentElement(this.layoutElementsGroups, ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection, this.layoutMeasurementGroups.ElementsPerRow, this.visibleRangeGroups.ElementCount, 0, this.layoutElementsGroups.Count - 1, BetterListViewTargetElement.PageDown);
                            adjacentElement = adjacentElement ?? this.layoutElementsGroups[this.layoutElementsGroups.Count - 1];
                            if (!this.CanFocusGroupKeyboard(adjacentElement)) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(adjacentElement);
                            break;
                        }
                        return focusInfoCurrent;
                    }
                case BetterListViewTargetElement.Home: {
                        BetterListViewGroup betterListViewGroup7 = this.layoutElementsGroups[0];
                        if (betterListViewGroup7.LayoutIndexItemSelectionFirst != -1) {
                            result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup7.LayoutIndexItemSelectionFirst], 0);
                            break;
                        }
                        if (!this.CanFocusGroupKeyboard(betterListViewGroup7)) {
                            return focusInfoCurrent;
                        }
                        result = new BetterListViewFocusInfo(betterListViewGroup7);
                        break;
                    }
                case BetterListViewTargetElement.End: {
                        BetterListViewGroup betterListViewGroup2 = this.layoutElementsGroups[this.layoutElementsGroups.Count - 1];
                        if (betterListViewGroup2.LayoutIndexItemSelectionLast != -1) {
                            result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup2.LayoutIndexItemSelectionLast], 0);
                            break;
                        }
                        if (!this.CanFocusGroupKeyboard(betterListViewGroup2)) {
                            return focusInfoCurrent;
                        }
                        result = new BetterListViewFocusInfo(betterListViewGroup2);
                        break;
                    }
                default:
                    throw new ApplicationException($"Unknown layout search direction: '{targetElement}'.");
            }
            return result;
        }

        private BetterListViewFocusInfo GetAdjacentFocusInfoItem(BetterListViewFocusInfo focusInfoCurrent, BetterListViewTargetElement targetElement) {
            BetterListViewItem betterListViewItem = (BetterListViewItem)focusInfoCurrent.Element;
            BetterListViewGroup betterListViewGroup = this.GetVisibleGroup(betterListViewItem, allowNull: false);
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            int num = (this.IsAnyItemVisible ? this.visibleRangeItemsDisplay.ElementCount : (this.IsAnyGroupVisible ? this.visibleRangeGroups.ElementCount : 0));
            BetterListViewItem betterListViewItem2 = ((betterListViewGroup.LayoutIndexItemSelectionFirst == -1 || betterListViewGroup.LayoutIndexItemSelectionLast == -1) ? null : layoutItemsCurrent.GetAdjacentElement(this.layoutElementsItemsSelection, ((IBetterListViewLayoutElementSelectable)betterListViewItem).LayoutIndexSelection, betterListViewGroup.LayoutMeasurementItems.ElementsPerRow, Math.Min(num, this.layoutElementsItemsSelection.Count), betterListViewGroup.LayoutIndexItemSelectionFirst, betterListViewGroup.LayoutIndexItemSelectionLast, targetElement));
            BetterListViewFocusInfo result;
            if (betterListViewItem2 != null) {
                //result = new BetterListViewFocusInfo(betterListViewItem2, Math.Min(this.SubItemFocusBehavior switch {
                //    BetterListViewSubItemFocusBehavior.None => 0,
                //    BetterListViewSubItemFocusBehavior.All => focusInfoCurrent.ColumnIndex,
                //    BetterListViewSubItemFocusBehavior.Auto => betterListViewItem2.AllowSelectChildItems ? focusInfoCurrent.ColumnIndex : 0,
                //    _ => throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'."),
                //}, ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds).SubItemBounds.Count - 1));
                int columnIndex;
                switch (this.SubItemFocusBehavior) {
                    case BetterListViewSubItemFocusBehavior.None: columnIndex = 0; break;
                    case BetterListViewSubItemFocusBehavior.All: columnIndex = focusInfoCurrent.ColumnIndex; break;
                    case BetterListViewSubItemFocusBehavior.Auto: columnIndex = betterListViewItem2.AllowSelectChildItems ? focusInfoCurrent.ColumnIndex : 0; break;
                    default:
                        throw new ApplicationException($"Unknown sub item focus behavior: '{this.SubItemFocusBehavior}'.");
                        break;
                }
                result = new BetterListViewFocusInfo(betterListViewItem2, Math.Min(columnIndex, ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds).SubItemBounds.Count - 1));
            }
            else {
                switch (targetElement) {
                    case BetterListViewTargetElement.Up: {
                            int layoutIndexSelection = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                            while (true) {
                                if (this.CanFocusGroupKeyboard(betterListViewGroup)) {
                                    result = new BetterListViewFocusInfo(betterListViewGroup);
                                    break;
                                }
                                int layoutIndexSelection2 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                                if (this.CircularSelection) {
                                    layoutIndexSelection2 = ((layoutIndexSelection2 == 0) ? (this.layoutElementsGroups.Count - 1) : (layoutIndexSelection2 - 1));
                                }
                                else {
                                    if (layoutIndexSelection2 == 0) {
                                        return focusInfoCurrent;
                                    }
                                    layoutIndexSelection2--;
                                }
                                betterListViewGroup = this.layoutElementsGroups[layoutIndexSelection2];
                                if (betterListViewGroup.LayoutIndexItemSelectionLast != -1) {
                                    result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup.LayoutIndexItemSelectionLast], focusInfoCurrent.ColumnIndex);
                                    break;
                                }
                                if (this.CircularSelection && layoutIndexSelection2 == layoutIndexSelection) {
                                    return focusInfoCurrent;
                                }
                            }
                            break;
                        }
                    case BetterListViewTargetElement.Down: {
                            int layoutIndexSelection3 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                            while (true) {
                                int layoutIndexSelection4 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection;
                                if (this.CircularSelection) {
                                    layoutIndexSelection4 = ((layoutIndexSelection4 < this.layoutElementsGroups.Count - 1) ? (layoutIndexSelection4 + 1) : 0);
                                }
                                else {
                                    if (layoutIndexSelection4 == this.layoutElementsGroups.Count - 1) {
                                        return focusInfoCurrent;
                                    }
                                    layoutIndexSelection4++;
                                }
                                betterListViewGroup = this.layoutElementsGroups[layoutIndexSelection4];
                                if (this.CanFocusGroupKeyboard(betterListViewGroup)) {
                                    result = new BetterListViewFocusInfo(betterListViewGroup);
                                    break;
                                }
                                if (betterListViewGroup.LayoutIndexItemSelectionFirst != -1) {
                                    result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup.LayoutIndexItemSelectionFirst], focusInfoCurrent.ColumnIndex);
                                    break;
                                }
                                if (this.CircularSelection && layoutIndexSelection4 == layoutIndexSelection3) {
                                    return focusInfoCurrent;
                                }
                            }
                            break;
                        }
                    case BetterListViewTargetElement.Left: {
                            BetterListViewGroup betterListViewGroup6 = null;
                            if (this.CircularSelection) {
                                for (int num6 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection - 1; num6 >= 0; num6--) {
                                    BetterListViewGroup betterListViewGroup7 = this.layoutElementsGroups[num6];
                                    if (betterListViewGroup7.LayoutIndexItemSelectionFirst != -1) {
                                        betterListViewGroup6 = betterListViewGroup7;
                                        break;
                                    }
                                }
                                if (betterListViewGroup6 == null) {
                                    for (int num7 = this.layoutElementsGroups.Count - 1; num7 >= ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection; num7--) {
                                        BetterListViewGroup betterListViewGroup8 = this.layoutElementsGroups[num7];
                                        if (betterListViewGroup8.LayoutIndexItemSelectionFirst != -1) {
                                            betterListViewGroup6 = betterListViewGroup8;
                                            break;
                                        }
                                    }
                                }
                            }
                            else {
                                for (int num8 = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection - 1; num8 >= 0; num8--) {
                                    BetterListViewGroup betterListViewGroup9 = this.layoutElementsGroups[num8];
                                    if (betterListViewGroup9.LayoutIndexItemSelectionFirst != -1) {
                                        betterListViewGroup6 = betterListViewGroup9;
                                        break;
                                    }
                                }
                            }
                            if (betterListViewGroup6 != null) {
                                if (layoutItemsCurrent.OrientationVertical) {
                                    result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup6.LayoutIndexItemSelectionLast], focusInfoCurrent.ColumnIndex);
                                    break;
                                }
                                int val3 = betterListViewGroup6.LayoutIndexItemDisplayLast - (betterListViewGroup6.LayoutIndexItemDisplayLast - betterListViewGroup6.LayoutIndexItemDisplayFirst) % betterListViewGroup6.LayoutMeasurementItems.ElementsPerRow + (((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutIndexDisplay - betterListViewGroup.LayoutIndexItemDisplayFirst) % betterListViewGroup.LayoutMeasurementItems.ElementsPerRow;
                                val3 = Math.Min(val3, betterListViewGroup6.LayoutIndexItemDisplayLast);
                                while (val3 >= 0 && !this.layoutElementsItemsDisplay[val3].IsSelectable) {
                                    val3--;
                                }
                                if (val3 < 0) {
                                    return focusInfoCurrent;
                                }
                                result = new BetterListViewFocusInfo(this.layoutElementsItemsDisplay[val3], 0);
                                break;
                            }
                            return focusInfoCurrent;
                        }
                    case BetterListViewTargetElement.Right: {
                            BetterListViewGroup betterListViewGroup2 = null;
                            if (this.CircularSelection) {
                                for (int j = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection + 1; j < this.layoutElementsGroups.Count; j++) {
                                    BetterListViewGroup betterListViewGroup3 = this.layoutElementsGroups[j];
                                    if (betterListViewGroup3.LayoutIndexItemSelectionFirst != -1) {
                                        betterListViewGroup2 = betterListViewGroup3;
                                        break;
                                    }
                                }
                                if (betterListViewGroup2 == null) {
                                    for (int k = 0; k <= ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection; k++) {
                                        BetterListViewGroup betterListViewGroup4 = this.layoutElementsGroups[k];
                                        if (betterListViewGroup4.LayoutIndexItemSelectionFirst != -1) {
                                            betterListViewGroup2 = betterListViewGroup4;
                                            break;
                                        }
                                    }
                                }
                            }
                            else {
                                for (int l = ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection + 1; l < this.layoutElementsGroups.Count; l++) {
                                    BetterListViewGroup betterListViewGroup5 = this.layoutElementsGroups[l];
                                    if (betterListViewGroup5.LayoutIndexItemSelectionFirst != -1) {
                                        betterListViewGroup2 = betterListViewGroup5;
                                        break;
                                    }
                                }
                            }
                            if (betterListViewGroup2 != null) {
                                if (layoutItemsCurrent.OrientationVertical) {
                                    result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[betterListViewGroup2.LayoutIndexItemSelectionFirst], focusInfoCurrent.ColumnIndex);
                                    break;
                                }
                                int val2 = betterListViewGroup2.LayoutIndexItemDisplayFirst + (((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutIndexDisplay - betterListViewGroup.LayoutIndexItemDisplayFirst) % betterListViewGroup.LayoutMeasurementItems.ElementsPerRow;
                                for (val2 = Math.Min(val2, betterListViewGroup2.LayoutIndexItemDisplayLast); val2 < this.layoutElementsItemsDisplay.Count && !this.layoutElementsItemsDisplay[val2].IsSelectable; val2++) {
                                }
                                if (val2 >= this.layoutElementsItemsDisplay.Count) {
                                    return focusInfoCurrent;
                                }
                                result = new BetterListViewFocusInfo(this.layoutElementsItemsDisplay[val2], 0);
                                break;
                            }
                            return focusInfoCurrent;
                        }
                    case BetterListViewTargetElement.PageUp: {
                            int layoutIndexDisplay2 = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutIndexDisplay;
                            int num4 = Math.Max(layoutIndexDisplay2 - num, 0);
                            bool flag2 = this.layoutElementsItemsDisplay[num4].IsSelectable;
                            if (!flag2) {
                                for (int num5 = num4 - 1; num5 >= 0; num5--) {
                                    if (this.layoutElementsItemsDisplay[num5].IsSelectable) {
                                        num4 = num5;
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag2) {
                                for (int m = num4 + 1; m < layoutIndexDisplay2; m++) {
                                    if (this.layoutElementsItemsDisplay[m].IsSelectable) {
                                        num4 = m;
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag2) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(this.layoutElementsItemsDisplay[num4], focusInfoCurrent.ColumnIndex);
                            break;
                        }
                    case BetterListViewTargetElement.PageDown: {
                            int layoutIndexDisplay = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutIndexDisplay;
                            int num2 = Math.Min(layoutIndexDisplay + num, this.layoutElementsItemsDisplay.Count - 1);
                            bool flag = this.layoutElementsItemsDisplay[num2].IsSelectable;
                            if (!flag) {
                                for (int i = num2 + 1; i < this.layoutElementsItemsDisplay.Count; i++) {
                                    if (this.layoutElementsItemsDisplay[i].IsSelectable) {
                                        num2 = i;
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag) {
                                for (int num3 = num2 - 1; num3 > layoutIndexDisplay; num3--) {
                                    if (this.layoutElementsItemsDisplay[num3].IsSelectable) {
                                        num2 = num3;
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag) {
                                return focusInfoCurrent;
                            }
                            result = new BetterListViewFocusInfo(this.layoutElementsItemsDisplay[num2], focusInfoCurrent.ColumnIndex);
                            break;
                        }
                    case BetterListViewTargetElement.Home:
                        result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[0], focusInfoCurrent.ColumnIndex);
                        break;
                    case BetterListViewTargetElement.End:
                        result = new BetterListViewFocusInfo(this.layoutElementsItemsSelection[this.layoutElementsItemsSelection.Count - 1], focusInfoCurrent.ColumnIndex);
                        break;
                    default:
                        throw new ApplicationException($"Unknown layout search direction: '{targetElement}'.");
                }
            }
            return result;
        }

        /// <summary>
        ///   Edit the currently focused item.
        /// </summary>
        public void BeginEdit() {
            BetterListViewSubItem focusedSubItem = this.FocusedSubItem;
            if (focusedSubItem != null) {
                this.BeginEdit(focusedSubItem, immediate: true);
            }
        }

        /// <summary>
        ///   Edit the specified sub-item.
        /// </summary>
        /// <param name="subItem">Sub-item to edit.</param>
        public void BeginEdit(BetterListViewSubItem subItem) {
            Checks.CheckNotNull(subItem, "subItem");
            if (subItem.OwnerCollection != null) {
                BetterListViewBase ownerControl = subItem.OwnerCollection.OwnerControl;
                if (ownerControl != null) {
                    ((BetterListView)ownerControl).BeginEdit(subItem, immediate: true);
                }
            }
        }

        /// <summary>
        ///   Terminate label editing operation.
        /// </summary>
        public void EndEdit() {
            this.EndEdit(this.LabelEditDefaultAccept);
        }

        /// <summary>
        ///   Terminate label editing operation.
        /// </summary>
        /// <param name="accept">Accept data from the label editing control.</param>
        public void EndEdit(bool accept) {
            this.LabelEditEnd(forced: true, accept);
        }

        private void BeginEdit(BetterListViewSubItem subItem, bool immediate) {
            BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
            BetterListViewLabelEditStateInfo labelEditStateInfo = betterListViewStateInfo.LabelEditStateInfo;
            if (!this.labelEditTimer.Enabled && betterListViewStateInfo.State != BetterListViewState.LabelEditInit && betterListViewStateInfo.State != BetterListViewState.LabelEdit) {
                labelEditStateInfo.SubItem = subItem;
                betterListViewStateInfo.State = BetterListViewState.LabelEditInit;
                betterListViewStateInfo.LabelEditStateInfo = labelEditStateInfo;
                this.SetStateInfo(betterListViewStateInfo);
                if (immediate) {
                    this.PerformLabelEdit();
                }
                else {
                    this.labelEditTimer.Start();
                }
            }
        }

        private void LabelEditInitialize() {
            this.labelEditTimer = new Timer();
            this.labelEditTimer.Interval = SystemInformation.DoubleClickTime;
            this.labelEditTimer.Tick += TimerLabelEditOnTick;
            this.ehLabelEditEmbeddedControlOnLeave = EmbeddedControlOnLeave;
            this.ehLabelEditEmbeddedControlOnLostFocus = EmbeddedControlOnLostFocus;
            this.ehLabelEditEmbeddedControlOnRequestAccept = EmbeddedControlOnRequestAccept;
            this.ehLabelEditEmbeddedControlOnRequestCancel = EmbeddedControlOnRequestCancel;
        }

        private bool LabelEditEnd(bool forced) {
            return this.LabelEditEnd(forced, this.LabelEditDefaultAccept);
        }

        private bool LabelEditEnd(bool forced, bool accept) {
            if (this.labelEditTimer.Enabled) {
                this.labelEditTimer.Stop();
            }
            if (this.StateInfo.State == BetterListViewState.LabelEditInit || this.StateInfo.State == BetterListViewState.LabelEdit) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewLabelEditStateInfo labelEditStateInfo = betterListViewStateInfo.LabelEditStateInfo;
                IBetterListViewEmbeddedControl control = labelEditStateInfo.Control;
                IBetterListViewEmbeddedControlExtended betterListViewEmbeddedControlExtended = labelEditStateInfo.Control as IBetterListViewEmbeddedControlExtended;
                if (this.StateInfo.State == BetterListViewState.LabelEdit && !forced && betterListViewEmbeddedControlExtended != null && !betterListViewEmbeddedControlExtended.RequestEndEdit()) {
                    return false;
                }
                betterListViewStateInfo.State = BetterListViewState.Normal;
                betterListViewStateInfo.LabelEditStateInfo = BetterListViewLabelEditStateInfo.Empty;
                if (control != null) {
                    try {
                        base.BeginUpdate();
                        if (labelEditStateInfo.SubItem.ListView == this) {
                            ((IBetterListViewStateElement)labelEditStateInfo.SubItem.Item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                        this.SetStateInfo(betterListViewStateInfo);
                        BetterListViewLabelEditCancelEventArgs betterListViewLabelEditCancelEventArgs = new BetterListViewLabelEditCancelEventArgs(!accept, labelEditStateInfo.Control.LabelText, labelEditStateInfo.SubItem);
                        this.OnAfterLabelEditCancel(betterListViewLabelEditCancelEventArgs);
                        bool flag = labelEditStateInfo.SubItem.ListView == this;
                        int num;
                        if (flag) {
                            this.SuspendSort();
                            int index2 = labelEditStateInfo.SubItem.Item.Index;
                            num = labelEditStateInfo.SubItem.Index;
                            if (!betterListViewLabelEditCancelEventArgs.CancelEdit) {
                                labelEditStateInfo.Control.SetData(labelEditStateInfo.SubItem);
                                if (index2 < this.Items.Count) {
                                    BetterListViewItem betterListViewItem = this.Items[index2];
                                    if (num < betterListViewItem.SubItems.Count) {
                                        labelEditStateInfo.SubItem = betterListViewItem.SubItems[num];
                                    }
                                    else {
                                        labelEditStateInfo.SubItem = null;
                                    }
                                }
                                else {
                                    labelEditStateInfo.SubItem = null;
                                }
                            }
                        }
                        else {
                            int index = 0;
                            num = 0;
                        }
                        this.OnAfterLabelEdit(betterListViewLabelEditCancelEventArgs);
                        Control control2 = (Control)labelEditStateInfo.Control;
                        base.Controls.Remove(control2);
                        control2.Leave -= this.ehLabelEditEmbeddedControlOnLeave;
                        control2.LostFocus -= this.ehLabelEditEmbeddedControlOnLostFocus;
                        labelEditStateInfo.Control.RequestAccept -= this.ehLabelEditEmbeddedControlOnRequestAccept;
                        labelEditStateInfo.Control.RequestCancel -= this.ehLabelEditEmbeddedControlOnRequestCancel;
                        control2.Dispose();
                        if (flag) {
                            this.ResumeSort(!this.sortList.Contains(num));
                        }
                    }
                    finally {
                        base.EndUpdate(suppressRefresh: true);
                    }
                }
                else {
                    ((IBetterListViewStateElement)labelEditStateInfo.SubItem.Item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    this.SetStateInfo(betterListViewStateInfo);
                }
                this.RefreshView();
            }
            return true;
        }

        private void TimerLabelEditOnTick(object sender, EventArgs eventArgs) {
            this.labelEditTimer.Stop();
            this.PerformLabelEdit();
        }

        private void PerformLabelEdit() {
            BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
            BetterListViewLabelEditStateInfo labelEditStateInfo = betterListViewStateInfo.LabelEditStateInfo;
            BetterListViewSubItem subItem = labelEditStateInfo.SubItem;
            if (!this.GetSubItemCellBounds(subItem, out var boundsText, out var boundsCell)) {
                return;
            }
            BetterListViewLabelEditCancelEventArgs betterListViewLabelEditCancelEventArgs = new BetterListViewLabelEditCancelEventArgs(cancelEdit: false, labelEditStateInfo.SubItem.Text, labelEditStateInfo.SubItem);
            this.OnBeforeLabelEdit(betterListViewLabelEditCancelEventArgs);
            if (betterListViewLabelEditCancelEventArgs.CancelEdit) {
                return;
            }
            //BetterListViewEmbeddedControlPlacement betterListViewEmbeddedControlPlacement = new BetterListViewEmbeddedControlPlacement(alignment: ((subItem.AlignHorizontal == TextAlignmentHorizontal.Default) ? this.LayoutItemsCurrent.DefaultTextAlignmentHorizontal : subItem.AlignHorizontal) switch {
            //    TextAlignmentHorizontal.Center => System.Drawing.ContentAlignment.TopCenter,
            //    TextAlignmentHorizontal.Right => System.Drawing.ContentAlignment.TopRight,
            //    _ => System.Drawing.ContentAlignment.TopLeft,
            //}, boundsText: boundsText, boundsCell: boundsCell, useCellBounds: true);
            var alignment = System.Drawing.ContentAlignment.TopLeft;
            if (subItem.AlignHorizontal == TextAlignmentHorizontal.Default) {
                switch (this.LayoutItemsCurrent.DefaultTextAlignmentHorizontal) {
                    case TextAlignmentHorizontal.Center: alignment = System.Drawing.ContentAlignment.TopCenter; break;
                    case TextAlignmentHorizontal.Right: alignment = System.Drawing.ContentAlignment.TopRight; break;
                    default: alignment = System.Drawing.ContentAlignment.TopLeft; break;
                }
            }
            else {
                switch (subItem.AlignHorizontal) {
                    case TextAlignmentHorizontal.Center: alignment = System.Drawing.ContentAlignment.TopCenter; break;
                    case TextAlignmentHorizontal.Right: alignment = System.Drawing.ContentAlignment.TopRight; break;
                    default: alignment = System.Drawing.ContentAlignment.TopLeft; break;
                }
            }
            BetterListViewEmbeddedControlPlacement betterListViewEmbeddedControlPlacement = new BetterListViewEmbeddedControlPlacement(boundsText, boundsCell, true, alignment);

            BetterListViewRequestEmbeddedControlEventArgs betterListViewRequestEmbeddedControlEventArgs = new BetterListViewRequestEmbeddedControlEventArgs(subItem, betterListViewEmbeddedControlPlacement);
            IBetterListViewEmbeddedControl betterListViewEmbeddedControl = this.OnRequestEmbeddedControl(betterListViewRequestEmbeddedControlEventArgs);
            if (betterListViewEmbeddedControl == null) {
                this.LabelEditEnd(forced: true);
                return;
            }
            Checks.CheckType(betterListViewEmbeddedControl, typeof(Control), "embeddedControl");
            betterListViewEmbeddedControl.SetSize(subItem, betterListViewEmbeddedControlPlacement);
            Control control = (Control)betterListViewEmbeddedControl;
            control.Leave += this.ehLabelEditEmbeddedControlOnLeave;
            control.LostFocus += this.ehLabelEditEmbeddedControlOnLostFocus;
            betterListViewEmbeddedControl.RequestAccept += this.ehLabelEditEmbeddedControlOnRequestAccept;
            betterListViewEmbeddedControl.RequestCancel += this.ehLabelEditEmbeddedControlOnRequestCancel;
            control.Hide();
            control.Parent = this;
            control.SendToBack();
            betterListViewEmbeddedControl.GetData(subItem);
            int num = 0;
            int num2 = 0;
            if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopLeft || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleLeft || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomLeft) {
                num = 0;
            }
            else if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopCenter || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleCenter || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomCenter) {
                num = (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Width - control.Width) / 2;
            }
            else if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopRight || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleRight || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomRight) {
                num = betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Width - control.Width;
            }
            if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopLeft || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopCenter || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.TopRight) {
                num2 = 0;
            }
            else if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleLeft || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleCenter || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.MiddleRight) {
                num2 = (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Height - control.Height) / 2;
            }
            else if (betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomLeft || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomCenter || betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Alignment == System.Drawing.ContentAlignment.BottomRight) {
                num2 = betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Height - control.Height;
            }
            this.EnsureVisible(new Rectangle(betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Left + num, betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Top + num2, control.Width, control.Height));
            if (this.GetSubItemCellBounds(labelEditStateInfo.SubItem, out boundsText, out boundsCell)) {
                Rectangle boundsContent = base.BoundsContent;
                control.Location = new Point(Math.Max(Math.Min(betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Left + num, boundsContent.Right - control.Width), boundsContent.Left), Math.Max(Math.Min(betterListViewRequestEmbeddedControlEventArgs.ControlPlacement.Bounds.Top + num2, boundsContent.Bottom - control.Height), boundsContent.Top));
                labelEditStateInfo.SubItem = subItem;
                labelEditStateInfo.Control = betterListViewEmbeddedControl;
                betterListViewStateInfo.State = BetterListViewState.LabelEdit;
                betterListViewStateInfo.LabelEditStateInfo = labelEditStateInfo;
                this.RefreshView();
                control.Show();
                control.Focus();
                this.SetStateInfo(betterListViewStateInfo);
            }
        }

        private bool GetSubItemCellBounds(BetterListViewSubItem subItem, out Rectangle boundsText, out Rectangle boundsCell) {
            if (subItem.Item == null) {
                this.LabelEditEnd(forced: false);
                boundsText = Rectangle.Empty;
                boundsCell = Rectangle.Empty;
                return false;
            }
            BetterListViewItem item = subItem.Item;
            if (!((IBetterListViewStateElement)item).IsActive || ((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay < this.visibleRangeItemsDisplay.IndexElementFirst || ((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay > this.visibleRangeItemsDisplay.IndexElementLast) {
                this.LabelEditEnd(forced: true);
                boundsText = Rectangle.Empty;
                boundsCell = Rectangle.Empty;
                return false;
            }
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            BetterListViewSubItemBounds betterListViewSubItemBounds = ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds).SubItemBounds[subItem.Index];
            boundsText = new Rectangle(betterListViewSubItemBounds.BoundsText.Left + offsetContentFromAbsolute.X, betterListViewSubItemBounds.BoundsText.Top + offsetContentFromAbsolute.Y, betterListViewSubItemBounds.BoundsText.Width, betterListViewSubItemBounds.BoundsText.Height);
            boundsCell = new Rectangle(betterListViewSubItemBounds.BoundsCell.Left + offsetContentFromAbsolute.X, betterListViewSubItemBounds.BoundsCell.Top + offsetContentFromAbsolute.Y, betterListViewSubItemBounds.BoundsCell.Width, betterListViewSubItemBounds.BoundsCell.Height);
            return true;
        }

        private void EmbeddedControlOnLeave(object sender, EventArgs eventArgs) {
            this.LabelEditEnd(forced: false);
        }

        private void EmbeddedControlOnLostFocus(object sender, EventArgs eventArgs) {
            if (this.StateInfo.State == BetterListViewState.LabelEdit) {
                this.LabelEditEnd(forced: false);
            }
        }

        private void EmbeddedControlOnRequestAccept(object sender, EventArgs eventArgs) {
            this.LabelEditEnd(forced: true, accept: true);
        }

        private void EmbeddedControlOnRequestCancel(object sender, EventArgs eventArgs) {
            this.LabelEditEnd(forced: true, accept: false);
        }

        private void LayoutAdjust() {
            Size size = base.BoundsContent.Size;
            Size contentSize = ((this.View != BetterListViewView.Details || this.layoutElementsColumns.Count == 0) ? size : new Size(Math.Max(((IBetterListViewLayoutElementDisplayable)this.layoutElementsColumns[this.layoutElementsColumns.Count - 1]).LayoutBounds.BoundsOuter.Right, size.Width), size.Height));
            if (contentSize.Width > 0 && contentSize.Height > 0) {
                this.LayoutGroupsCurrent.AdjustElements(this.layoutElementsGroups, contentSize, base.CachedGraphics, this.ImageListGroups, this.ShowGroupExpandButtons, this.ShowDefaultGroupHeader ? null : this.defaultGroup);
            }
            if (this.ShowGroups && this.layoutElementsGroups.Count != 0 && !this.contentMeasurement.IsEmpty) {
                this.layoutMeasurementGroups = this.LayoutGroupsCurrent.MeasureLayout(this.layoutElementsGroups, 0, this.layoutElementsGroups.Count - 1, this.contentMeasurement.Size);
            }
            else if (this.layoutElementsItemsDisplay.Count != 0 && !this.contentMeasurement.IsEmpty) {
                ((IBetterListViewLayoutElementDisplayable)this.layoutElementsGroups[0]).LayoutBounds.Reset();
                BetterListViewLayoutMeasurement layoutMeasurementItems = this.LayoutItemsCurrent.MeasureLayout(this.layoutElementsItemsDisplay, 0, this.layoutElementsItemsDisplay.Count - 1, this.contentMeasurement.Size);
                this.layoutMeasurementGroups = new BetterListViewLayoutMeasurement(1, layoutMeasurementItems.Width, layoutMeasurementItems.Height);
                BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[0];
                betterListViewGroup.LayoutMeasurementItems = layoutMeasurementItems;
                ((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds.BoundsSpacing = new Rectangle(0, 0, layoutMeasurementItems.Width, layoutMeasurementItems.Height);
            }
            else {
                this.layoutMeasurementGroups = BetterListViewLayoutMeasurement.Empty;
            }
            if ((this.layoutElementsColumns.Count != 0 || this.layoutElementsItemsDisplay.Count != 0 || this.layoutElementsGroups.Count != 0) && this.contentMeasurement.Size.Width > 0 && this.contentMeasurement.Size.Height > 0) {
                int num = 0;
                int num2 = 0;
                if (this.layoutElementsColumns.Count != 0) {
                    num = Math.Max(num, this.layoutMeasurementColumns.Width - this.contentMeasurement.Size.Width);
                    num2 = Math.Max(num2, this.layoutMeasurementColumns.Height - this.contentMeasurement.Size.Height);
                }
                if (this.layoutElementsGroups.Count != 0) {
                    num = Math.Max(num, this.layoutMeasurementGroups.Width - this.contentMeasurement.Size.Width);
                    num2 = Math.Max(num2, this.layoutMeasurementGroups.Height - this.contentMeasurement.Size.Height);
                }
                try {
                    base.BeginUpdate();
                    Rectangle clientRectangleInner = base.ClientRectangleInner;
                    if (base.HScrollBarVisible) {
                        base.HScrollBar.Bounds = new Rectangle(clientRectangleInner.Left, clientRectangleInner.Bottom - SystemInformation.HorizontalScrollBarHeight, base.VScrollBarVisible ? (clientRectangleInner.Right - SystemInformation.VerticalScrollBarWidth) : clientRectangleInner.Width, SystemInformation.HorizontalScrollBarHeight);
                        int value = base.HScrollBar.Value;
                        int num3 = this.contentMeasurement.Size.Width;
                        if (base.VScrollBarVisible && num3 > SystemInformation.VerticalScrollBarWidth) {
                            num3 -= SystemInformation.VerticalScrollBarWidth;
                        }
                        base.HScrollBar.Maximum = num + num3 - 1;
                        base.HScrollBar.SmallChange = ((this.LayoutItemsCurrent.DirectionVertical && this.LayoutItemsCurrent.OrientationVertical) ? this.smallItemSize.Height : this.smallItemSize.Width);
                        base.HScrollBar.LargeChange = num3;
                        base.HScrollBar.Value = Math.Min(Math.Max(value, base.HScrollBar.Minimum), base.HScrollBar.Maximum - base.HScrollBar.LargeChange + 1);
                        this.OnHScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(base.HScrollProperties));
                    }
                    if (base.VScrollBarVisible) {
                        base.VScrollBar.Bounds = new Rectangle(clientRectangleInner.Right - SystemInformation.VerticalScrollBarWidth, clientRectangleInner.Top, SystemInformation.VerticalScrollBarWidth, base.HScrollBarVisible ? (clientRectangleInner.Bottom - SystemInformation.HorizontalScrollBarHeight) : clientRectangleInner.Height);
                        int value2 = base.VScrollBar.Value;
                        int num4 = this.contentMeasurement.Size.Height;
                        if (base.HScrollBarVisible && num4 > SystemInformation.HorizontalScrollBarHeight) {
                            num4 -= SystemInformation.HorizontalScrollBarHeight;
                        }
                        base.VScrollBar.Maximum = num2 + num4 - 1;
                        base.VScrollBar.SmallChange = ((!this.LayoutItemsCurrent.DirectionVertical && !this.LayoutItemsCurrent.OrientationVertical) ? this.smallItemSize.Width : this.smallItemSize.Height);
                        base.VScrollBar.LargeChange = num4;
                        base.VScrollBar.Value = Math.Min(Math.Max(value2, base.VScrollBar.Minimum), base.VScrollBar.Maximum - base.VScrollBar.LargeChange + 1);
                        this.OnVScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(base.VScrollProperties));
                    }
                    return;
                }
                finally {
                    base.EndUpdate(suppressRefresh: true);
                }
            }
            base.HScrollBarVisible = false;
            base.VScrollBarVisible = false;
        }

        /// <summary>
        ///   Get content area boundaries.
        /// </summary>
        /// <param name="widthExceeded">Layout width exceeds the content area.</param>
        /// <param name="heightExceeded">Layout height exceeds the content area.</param>
        /// <returns>Content area boundaries.</returns>
        protected override Rectangle GetContentBounds(bool widthExceeded, bool heightExceeded) {
            Rectangle clientRectangleInner = base.ClientRectangleInner;
            int num = clientRectangleInner.Top;
            int num2 = clientRectangleInner.Width;
            int num3 = clientRectangleInner.Height;
            if (this.layoutElementsColumns.Count != 0) {
                num += this.layoutMeasurementColumns.Height;
                num3 -= this.layoutMeasurementColumns.Height;
            }
            if (this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowAlways || (this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded && widthExceeded)) {
                num3 -= SystemInformation.HorizontalScrollBarHeight;
            }
            if (this.VScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowAlways || (this.VScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded && heightExceeded)) {
                num2 -= SystemInformation.VerticalScrollBarWidth;
            }
            return new Rectangle(clientRectangleInner.Left, num, Math.Max(num2, 0), Math.Max(num3, 0));
        }

        /// <summary>
        ///   Raises the Resize event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected override void OnResize(EventArgs eventArgs) {
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            if (base.ClientSizeInner.Width > 0 && base.ClientSizeInner.Height > 0 && layoutItemsCurrent != null) {
                if (base.CachedGraphics == null) {
                    return;
                }
                try {
                    base.BeginUpdate();
                    bool focused = this.Focused;
                    BetterListViewInvalidationInfo resizeInvalidationInfo = layoutItemsCurrent.ResizeInvalidationInfo;
                    BetterListViewInvalidationInfo resizeInvalidationInfo2 = this.LayoutGroupsCurrent.GetResizeInvalidationInfo(resizeInvalidationInfo);
                    BetterListViewInvalidationLevel betterListViewInvalidationLevel = (BetterListViewInvalidationLevel)Math.Max(Math.Max((int)resizeInvalidationInfo.Level, (int)resizeInvalidationInfo2.Level), 3);
                    BetterListViewInvalidationFlags flags = (resizeInvalidationInfo.Flags & ~BetterListViewInvalidationFlags.Draw) | (resizeInvalidationInfo2.Flags & ~BetterListViewInvalidationFlags.Draw);
                    if (betterListViewInvalidationLevel >= BetterListViewInvalidationLevel.MeasureElements) {
                        ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        foreach (BetterListViewGroup group in this.Groups) {
                            ((IBetterListViewStateElement)group).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        foreach (BetterListViewItem current2 in this) {
                            ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                    }
                    base.Invalidate(betterListViewInvalidationLevel, flags);
                    this.PerformRefreshView();
                    base.InvalidationInfo = new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.None, base.InvalidationInfo.Flags, Rectangle.Empty);
                    if (focused) {
                        base.Focus();
                    }
                }
                finally {
                    base.EndUpdate(suppressRefresh: true);
                }
            }
            base.OnResize(eventArgs);
        }

        private void LayoutInitialize() {
            this.layoutColumns = new BetterListViewLayoutColumns(this);
            this.layoutsItems = new SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutItems>();
            this.layoutsItems.Add(BetterListViewViewInternal.Details, new BetterListViewLayoutItemsDetails(this));
            this.layoutsItems.Add(BetterListViewViewInternal.DetailsColumns, new BetterListViewLayoutItemsDetailsColumns(this));
            this.layoutsItems.Add(BetterListViewViewInternal.SmallIcon, new BetterListViewLayoutItemsSmallIcon(this));
            this.layoutsItems.Add(BetterListViewViewInternal.LargeIcon, new BetterListViewLayoutItemsLargeIcon(this));
            this.layoutsItems.Add(BetterListViewViewInternal.List, new BetterListViewLayoutItemsList(this));
            this.layoutsItems.Add(BetterListViewViewInternal.Tile, new BetterListViewLayoutItemsTile(this));
            this.layoutGroupsHorizontal = new BetterListViewLayoutGroupsHorizontal(this);
            this.layoutGroupsVertical = new BetterListViewLayoutGroupsVertical(this);
            this.layoutsGroups = new SortedDictionary<BetterListViewViewInternal, BetterListViewLayoutGroups>();
            foreach (BetterListViewViewInternal value in Enum.GetValues(typeof(BetterListViewViewInternal))) {
                this.layoutsGroups.Add(value, this.layoutsItems[value].OrientationVertical ? ((BetterListViewLayoutGroups)this.layoutGroupsVertical) : ((BetterListViewLayoutGroups)this.layoutGroupsHorizontal));
            }
            this.layoutElementsColumns = new ReadOnlyCollection<BetterListViewColumnHeader>(new BetterListViewColumnHeader[0]);
            this.layoutElementsGroups = new ReadOnlyCollection<BetterListViewGroup>(new BetterListViewGroup[0]);
            this.layoutElementsItemsDisplay = new ReadOnlyCollection<BetterListViewItem>(new BetterListViewItem[0]);
            this.layoutElementsItemsSelection = new ReadOnlyCollection<BetterListViewItem>(new BetterListViewItem[0]);
        }

        private void AutoResizeColumn(BetterListViewColumnHeader columnHeader, BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle, int extraPadding, bool userCode) {
            if (base.IsUpdating) {
                this.PostponeCall(BetterListViewPostponedCallType.AutoResizeColumn, new AutoResizeColumnDelegate(AutoResizeColumn), new object[4] { columnHeader, columnHeaderAutoResizeStyle, extraPadding, userCode });
            }
            else {
                if (columnHeader.ListView != this || ((IBetterListViewStateElement)columnHeader).State != BetterListViewElementState.ActiveFine || !columnHeader.Visible) {
                    return;
                }
                Graphics cachedGraphics = base.CachedGraphics;
                BetterListViewLayoutItemsDetailsColumns betterListViewLayoutItemsDetailsColumns = (BetterListViewLayoutItemsDetailsColumns)this.layoutsItems[BetterListViewViewInternal.DetailsColumns];
                int num = this.layoutColumns.GetColumnHeaderAutoWidth(columnHeader, cachedGraphics, this.ImageListColumns, this.Columns.ConsiderSorting && this.sortList.Contains(columnHeader.Index));
                if (columnHeaderAutoResizeStyle == BetterListViewColumnHeaderAutoResizeStyle.ColumnContent) {
                    ImageList imageListCurrent = this.ImageListCurrent;
                    foreach (BetterListViewItem item in this.Items) {
                        if (columnHeader.Index < item.SubItems.Count) {
                            num = Math.Max(num, betterListViewLayoutItemsDetailsColumns.GetSubItemAutoWidth(cachedGraphics, item, imageListCurrent, columnHeader.Index, this.ShowItemExpandButtons, this.CheckBoxesVisible, this.MaximumAutoSizeWidth));
                        }
                    }
                }
                columnHeader.SetWidth(num + extraPadding, userCode ? BetterListViewColumnWidthChangeSource.AutoSizeUserCode : BetterListViewColumnWidthChangeSource.AutoSizeMouse, raiseEvent: true);
            }
        }

        private bool UpdateOverflows(Size contentSize, ref bool overflowColumnsHorizontal, ref bool overflowColumnsVertical, ref bool overflowContentHorizontal, ref bool overflowContentVertical) {
            bool overflowsHorizontal = false;
            bool overflowsVertical = false;
            if (this.layoutElementsColumns.Count != 0) {
                this.LayoutColumns.CheckOverflows(this.layoutElementsColumns, contentSize, ref overflowsHorizontal, ref overflowsVertical);
            }
            bool overflowsHorizontal2 = false;
            bool overflowsVertical2 = false;
            if (this.ShowGroups) {
                this.LayoutGroupsCurrent.CheckOverflows(this.layoutElementsGroups, this.layoutElementsItemsDisplay, this.LayoutItemsCurrent, contentSize, ref overflowsHorizontal2, ref overflowsVertical2);
            }
            else if (this.layoutElementsItemsDisplay.Count != 0) {
                this.LayoutItemsCurrent.CheckOverflows(this.layoutElementsItemsDisplay, 0, this.layoutElementsItemsDisplay.Count - 1, contentSize, ref overflowsHorizontal2, ref overflowsVertical2);
            }
            if (this.View == BetterListViewView.Details && this.layoutElementsColumns.Count != 0) {
                overflowsHorizontal2 = overflowsHorizontal;
            }
            bool flag = false;
            if (overflowsHorizontal != overflowColumnsHorizontal) {
                flag |= !overflowColumnsHorizontal && this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded;
                overflowColumnsHorizontal = overflowsHorizontal;
            }
            if (overflowsVertical != overflowColumnsVertical) {
                flag |= !overflowColumnsVertical && this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded;
                overflowColumnsVertical = overflowsVertical;
            }
            if (overflowsHorizontal2 != overflowContentHorizontal) {
                flag |= !overflowContentHorizontal && this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded;
                overflowContentHorizontal = overflowsHorizontal2;
            }
            if (overflowsVertical2 != overflowContentVertical) {
                flag |= !overflowContentVertical && this.HScrollBarDisplayMode == BetterListViewScrollBarDisplayMode.ShowIfNeeded;
                overflowContentVertical = overflowsVertical2;
            }
            return flag;
        }

        private void LayoutMeasureContent() {
            if (this.StateInfo.State == BetterListViewState.LabelEdit) {
                this.LabelEditEnd(forced: true);
            }
            bool overflowColumnsHorizontal = false;
            bool overflowColumnsVertical = false;
            bool overflowContentHorizontal = false;
            bool overflowContentVertical = false;
            Size size = this.GetContentBounds(widthExceeded: false, heightExceeded: false).Size;
            if ((this.layoutElementsColumns.Count != 0 || this.layoutElementsItemsDisplay.Count != 0 || this.layoutElementsGroups.Count != 0) && size.Width > 0 && size.Height > 0) {
                if (this.UpdateOverflows(size, ref overflowColumnsHorizontal, ref overflowColumnsVertical, ref overflowContentHorizontal, ref overflowContentVertical)) {
                    size = this.GetContentBounds(overflowColumnsHorizontal || overflowContentHorizontal, overflowColumnsVertical || overflowContentVertical).Size;
                    if (size.Width > 0 && size.Height > 0) {
                        if (this.layoutElementsColumns.Count == 0 && this.layoutElementsItemsDisplay.Count != 0 && this.View == BetterListViewView.Details && this.AutoSizeItemsInDetailsView) {
                            foreach (BetterListViewItem item in this.layoutElementsItemsDisplay) {
                                ((IBetterListViewStateElement)item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            }
                            this.LayoutMeasureElements(includeHScrollBar: false, overflowColumnsVertical || overflowContentVertical);
                        }
                        this.UpdateOverflows(size, ref overflowColumnsHorizontal, ref overflowColumnsVertical, ref overflowContentHorizontal, ref overflowContentVertical);
                    }
                    size = this.GetContentBounds(overflowColumnsHorizontal || overflowContentHorizontal, overflowColumnsVertical || overflowContentVertical).Size;
                    if (size.Width <= 0 || size.Height <= 0) {
                        size = new Size(0, 0);
                    }
                    else {
                        this.UpdateOverflows(size, ref overflowColumnsHorizontal, ref overflowColumnsVertical, ref overflowContentHorizontal, ref overflowContentVertical);
                        size = this.GetContentBounds(overflowColumnsHorizontal || overflowContentHorizontal, overflowColumnsVertical || overflowContentVertical).Size;
                    }
                }
                else if (this.layoutElementsColumns.Count == 0 && this.layoutElementsItemsDisplay.Count != 0 && this.View == BetterListViewView.Details && this.AutoSizeItemsInDetailsView) {
                    foreach (BetterListViewItem item2 in this.layoutElementsItemsDisplay) {
                        ((IBetterListViewStateElement)item2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    this.LayoutMeasureElements(includeHScrollBar: false, overflowColumnsVertical || overflowContentVertical);
                }
                this.contentMeasurement = new BetterListViewContentMeasurement(overflowColumnsHorizontal, overflowColumnsVertical, overflowContentHorizontal, overflowContentVertical, size);
            }
            else {
                this.contentMeasurement = BetterListViewContentMeasurement.Empty;
            }
            if (this.contentMeasurement.Size.Width > 0) {
                switch (this.HScrollBarDisplayMode) {
                    case BetterListViewScrollBarDisplayMode.Hide:
                        base.HScrollBarVisible = false;
                        break;
                    case BetterListViewScrollBarDisplayMode.ShowIfNeeded:
                        base.HScrollBarVisible = this.contentMeasurement.OverflowColumnsHorizontal | this.contentMeasurement.OverflowContentHorizontal;
                        break;
                    case BetterListViewScrollBarDisplayMode.ShowAlways:
                        base.HScrollBarVisible = true;
                        break;
                }
            }
            else {
                base.HScrollBarVisible = false;
            }
            if (this.contentMeasurement.Size.Height > 0) {
                switch (this.VScrollBarDisplayMode) {
                    case BetterListViewScrollBarDisplayMode.Hide:
                        base.VScrollBarVisible = false;
                        break;
                    case BetterListViewScrollBarDisplayMode.ShowIfNeeded:
                        base.VScrollBarVisible = this.contentMeasurement.OverflowColumnsVertical | this.contentMeasurement.OverflowContentVertical;
                        break;
                    case BetterListViewScrollBarDisplayMode.ShowAlways:
                        base.VScrollBarVisible = true;
                        break;
                }
            }
            else {
                base.VScrollBarVisible = false;
            }
        }

        private void LayoutMeasureElements(bool includeHScrollBar, bool includeVScrollBar) {
            Graphics cachedGraphics = base.CachedGraphics;
            BetterListViewLayoutColumns betterListViewLayoutColumns = this.layoutColumns;
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
            ImageList imageList = this.ImageListColumns;
            ImageList imageListCurrent = this.ImageListCurrent;
            ImageList imageList2 = this.ImageListGroups;
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            try {
                base.BeginUpdate();
                if (this.Columns.Count != 0) {
                    if (betterListViewLayoutColumns.AutoSizeImages) {
                        int num = 0;
                        int num2 = 0;
                        foreach (BetterListViewColumnHeader column in this.Columns) {
                            Image elementImage = BetterListViewBasePainter.GetElementImage(column, imageList);
                            if (elementImage != null) {
                                num = Math.Max(num, elementImage.Width);
                                num2 = Math.Max(num2, elementImage.Height);
                            }
                        }
                        betterListViewLayoutColumns.ImageSize = new BetterListViewImageSize(Size.Empty, new Size(num, num2));
                    }
                    if (betterListViewLayoutColumns.AutoSizeText) {
                        int num3 = 0;
                        foreach (BetterListViewColumnHeader column2 in this.Columns) {
                            Font font = column2.Font;
                            int hashCode = font.GetHashCode();
                            if (!dictionary.TryGetValue(hashCode, out var value)) {
                                value = TextSize.HeightFromLineCount(cachedGraphics, font, 1);
                                dictionary.Add(hashCode, value);
                            }
                            num3 = Math.Max(num3, value);
                        }
                        betterListViewLayoutColumns.MinimumTextHeight = num3;
                    }
                    bool zeroHeight = true;
                    foreach (BetterListViewColumnHeader layoutElementsColumn in this.layoutElementsColumns) {
                        if (layoutElementsColumn.GetStyle(this) != 0) {
                            zeroHeight = false;
                            break;
                        }
                    }
                    BetterListViewCommonMeasurementColumns commonMeasurement = betterListViewLayoutColumns.MeasureElements(this.layoutElementsColumns, cachedGraphics, this.sortList, this.imageListColumns, zeroHeight);
                    if (!commonMeasurement.IsEmpty) {
                        foreach (BetterListViewColumnHeader layoutElementsColumn2 in this.layoutElementsColumns) {
                            betterListViewLayoutColumns.MeasureElement(layoutElementsColumn2, cachedGraphics, imageList, this.sortList, commonMeasurement);
                            ((IBetterListViewStateElement)layoutElementsColumn2).ChangeState(BetterListViewElementStateChange.MeasureFine);
                        }
                        this.layoutMeasurementColumns = this.layoutColumns.MeasureLayout(this.layoutElementsColumns, 0, this.layoutElementsColumns.Count - 1, this.contentMeasurement.Size);
                    }
                    else {
                        foreach (BetterListViewColumnHeader layoutElementsColumn3 in this.layoutElementsColumns) {
                            ((IBetterListViewStateElement)layoutElementsColumn3).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        this.layoutMeasurementColumns = BetterListViewLayoutMeasurement.Empty;
                    }
                }
                if (this.Groups.Count != 0) {
                    if (layoutGroupsCurrent.AutoSizeImages) {
                        int num4 = 0;
                        int num5 = 0;
                        foreach (BetterListViewGroup group in this.Groups) {
                            Image elementImage2 = BetterListViewBasePainter.GetElementImage(group, imageList2);
                            if (elementImage2 != null) {
                                num4 = Math.Max(num4, elementImage2.Width);
                                num5 = Math.Max(num5, elementImage2.Height);
                            }
                        }
                        layoutGroupsCurrent.ImageSize = new BetterListViewImageSize(Size.Empty, new Size(num4, num5));
                    }
                    if (layoutGroupsCurrent.AutoSizeText) {
                        int num6 = 0;
                        foreach (BetterListViewGroup group2 in this.Groups) {
                            Font font2 = group2.Font;
                            int hashCode2 = font2.GetHashCode();
                            if (!dictionary.TryGetValue(hashCode2, out var value2)) {
                                value2 = TextSize.HeightFromLineCount(cachedGraphics, font2, 1);
                                dictionary.Add(hashCode2, value2);
                            }
                            num6 = Math.Max(num6, value2);
                        }
                        layoutGroupsCurrent.MinimumTextHeight = num6;
                    }
                }
                Size size = this.GetContentBounds(includeHScrollBar, includeVScrollBar).Size;
                Size contentSize;
                if (this.ShowGroups) {
                    BetterListViewCommonMeasurementGroups commonMeasurement2 = layoutGroupsCurrent.MeasureElements(this.layoutElementsGroups, cachedGraphics, size, this.ShowGroupExpandButtons);
                    if (!commonMeasurement2.IsEmpty) {
                        foreach (BetterListViewGroup layoutElementsGroup in this.layoutElementsGroups) {
                            if (((IBetterListViewStateElement)layoutElementsGroup).State == BetterListViewElementState.Active) {
                                layoutGroupsCurrent.MeasureElement(layoutElementsGroup, cachedGraphics, commonMeasurement2, imageList2, layoutElementsGroup != this.defaultGroup || this.ShowDefaultGroupHeader);
                                ((IBetterListViewStateElement)layoutElementsGroup).ChangeState(BetterListViewElementStateChange.MeasureFine);
                            }
                        }
                        contentSize = commonMeasurement2.ContentSizeItems;
                    }
                    else {
                        foreach (BetterListViewGroup layoutElementsGroup2 in this.layoutElementsGroups) {
                            ((IBetterListViewStateElement)layoutElementsGroup2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        contentSize = size;
                    }
                }
                else {
                    contentSize = size;
                }
                if (this.Items.Count == 0) {
                    return;
                }
                int num7 = ((!this.ColumnsVisible) ? 1 : this.Columns.Count);
                if (layoutItemsCurrent.AutoSizeImages) {
                    Size[] array = new Size[num7];
                    for (int i = 0; i < array.Length; i++) {
                        array[i] = Size.Empty;
                    }
                    foreach (BetterListViewItem current10 in this) {
                        for (int j = 0; j < Math.Min(current10.SubItems.Count, num7); j++) {
                            Image elementImage3 = BetterListViewBasePainter.GetElementImage(current10.SubItems[j], imageListCurrent);
                            if (elementImage3 != null) {
                                Size size2 = array[j];
                                array[j] = new Size(Math.Max(size2.Width, elementImage3.Width), Math.Max(size2.Height, elementImage3.Height));
                            }
                        }
                    }
                    BetterListViewImageSize[] array2 = new BetterListViewImageSize[num7];
                    for (int k = 0; k < num7; k++) {
                        array2[k] = new BetterListViewImageSize(Size.Empty, array[k]);
                    }
                    layoutItemsCurrent.SubItemImageSizes = array2;
                }
                if (layoutItemsCurrent.AutoSizeText) {
                    int num8 = 0;
                    foreach (BetterListViewItem current11 in this) {
                        int num9 = Math.Min(current11.SubItems.Count, num7);
                        for (int l = 0; l < num9; l++) {
                            Font font3 = current11.SubItems[l].Font;
                            int hashCode3 = font3.GetHashCode();
                            if (!dictionary.TryGetValue(hashCode3, out var value3)) {
                                value3 = TextSize.HeightFromLineCount(cachedGraphics, font3, 1);
                                dictionary.Add(hashCode3, value3);
                            }
                            num8 = Math.Max(num8, value3);
                        }
                    }
                    if (layoutItemsCurrent.OrientationVertical) {
                        layoutItemsCurrent.EmptyTextSize = new Size(layoutItemsCurrent.EmptyTextSize.Width, num8);
                    }
                    else {
                        layoutItemsCurrent.EmptyTextSize = new Size(layoutItemsCurrent.EmptyTextSize.Width, num8 * layoutItemsCurrent.MaximumTextLines);
                    }
                }
                ImageList imageListCurrent2 = this.ImageListCurrent;
                List<BetterListViewItem> list = new List<BetterListViewItem>();
                foreach (BetterListViewItem item in this.layoutElementsItemsDisplay) {
                    if (((IBetterListViewStateElement)item).State == BetterListViewElementState.Active) {
                        list.Add(item);
                    }
                }
                BetterListViewCommonMeasurementItems betterListViewCommonMeasurementItems = layoutItemsCurrent.MeasureElements(list, cachedGraphics, contentSize, this.layoutElementsColumns, this.enableExpandButtons, this.CheckBoxesVisible, this.Indent);
                if (betterListViewCommonMeasurementItems.IsEmpty) {
                    return;
                }
                this.commonMeasurementItems = betterListViewCommonMeasurementItems;
                int resizedColumnIndex = ((this.StateInfo.State == BetterListViewState.ColumnResize) ? this.StateInfo.ColumnResizeStateInfo.Column.Index : (-1));
                foreach (BetterListViewItem item2 in list) {
                    bool flag = item2.CheckBoxAppearance != BetterListViewCheckBoxAppearance.Hide;
                    layoutItemsCurrent.MeasureElementCoarse(item2, cachedGraphics, imageListCurrent2, this.layoutElementsColumns, this.FullRowSelect, this.enableExpandButtons && this.ShowItemExpandButtons, this.ShowItemExpandButtons && item2.AllowShowExpandButton && item2.ChildItems.Count != 0, this.CheckBoxesVisible && (this.CheckBoxesAlign || flag), this.CheckBoxesVisible && flag, this.ExtraPadding, this.commonMeasurementItems, this.commonMeasurementItemsOffsets, dictionary, resizedColumnIndex);
                    ((IBetterListViewStateElement)item2).ChangeState(BetterListViewElementStateChange.MeasureCoarse);
                }
                this.UpdateSmallItemSize();
            }
            finally {
                base.EndUpdate(suppressRefresh: true);
            }
        }

        private void UpdateSmallItemSize() {
            int val = int.MaxValue;
            int val2 = int.MaxValue;
            foreach (BetterListViewItem item in this.layoutElementsItemsDisplay) {
                BetterListViewElementState state = ((IBetterListViewStateElement)item).State;
                if (state == BetterListViewElementState.ActiveCoarse || state == BetterListViewElementState.ActiveFine || state == BetterListViewElementState.ActiveVisible) {
                    Rectangle boundsOuter = ((IBetterListViewLayoutElementDisplayable)item).LayoutBounds.BoundsOuter;
                    val = Math.Min(val, boundsOuter.Width);
                    val2 = Math.Min(val2, boundsOuter.Height);
                }
            }
            this.smallItemSize = new Size(Math.Max(val, this.LayoutItemsCurrent.EmptyTextSize.Width), Math.Max(val2, this.LayoutItemsCurrent.EmptyTextSize.Height));
        }

        /// <summary>
        ///   Set width of the specified column so that it shows whole items.
        ///   This applies only do Details view.
        /// </summary>
        /// <param name="columnIndex">Index of the column to resize automatically.</param>
        /// <param name="columnHeaderAutoResizeStyle">Column resizing method.</param>
        public void AutoResizeColumn(int columnIndex, BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle) {
            this.AutoResizeColumn(columnIndex, columnHeaderAutoResizeStyle, 0);
        }

        /// <summary>
        ///   Set width of the specified column so that it shows whole items.
        ///   This applies only do Details view.
        /// </summary>
        /// <param name="columnIndex">Index of the column to resize automatically.</param>
        /// <param name="columnHeaderAutoResizeStyle">Column resizing method.</param>
        /// <param name="extraPadding">Additional padding for each column (in pixels).</param>
        public void AutoResizeColumn(int columnIndex, BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle, int extraPadding) {
            Checks.CheckTrue(this.Columns.Count != 0, "Columns.Count != 0");
            Checks.CheckBounds(columnIndex, 0, this.Columns.Count - 1, "columnIndex");
            Checks.CheckNotEqual(columnHeaderAutoResizeStyle, BetterListViewColumnHeaderAutoResizeStyle.None, "columnHeaderAutoResizeStyle", "BetterListViewColumnHeaderAutoResizeStyle.None");
            Checks.CheckTrue(extraPadding >= 0, "extraPadding >= 0");
            this.AutoResizeColumn(this.Columns[columnIndex], columnHeaderAutoResizeStyle, extraPadding, userCode: true);
        }

        /// <summary>
        ///   Set width of all columns so that it shows whole items.
        ///   This applies only do Details view.
        /// </summary>
        /// <param name="columnHeaderAutoResizeStyle">Column resizing method.</param>
        public void AutoResizeColumns(BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle) {
            this.AutoResizeColumns(columnHeaderAutoResizeStyle, 0);
        }

        /// <summary>
        ///   Set width of all columns so that it shows whole items.
        ///   This applies only do Details view.
        /// </summary>
        /// <param name="columnHeaderAutoResizeStyle">Column resizing method.</param>
        /// <param name="extraPadding">Additional padding for each column (in pixels).</param>
        public void AutoResizeColumns(BetterListViewColumnHeaderAutoResizeStyle columnHeaderAutoResizeStyle, int extraPadding) {
            Checks.CheckNotEqual(columnHeaderAutoResizeStyle, BetterListViewColumnHeaderAutoResizeStyle.None, "columnHeaderAutoResizeStyle", "BetterListViewColumnHeaderAutoResizeStyle.None");
            Checks.CheckTrue(extraPadding >= 0, "extraPadding >= 0");
            try {
                base.BeginUpdate();
                for (int i = 0; i < this.Columns.Count; i++) {
                    this.AutoResizeColumn(this.Columns[i], columnHeaderAutoResizeStyle, extraPadding, userCode: true);
                }
            }
            finally {
                base.EndUpdate();
            }
        }

        /// <summary>
        ///   Get column header at the specified location in client coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the location to check.</param>
        /// <param name="y">Y-coordinate of the location to check.</param>
        /// <returns>BetterListViewColumnHeader instance, or null.</returns>
        public BetterListViewColumnHeader GetColumnHeaderAt(int x, int y) {
            return this.GetColumnHeaderAt(new Point(x, y));
        }

        /// <summary>
        ///   Get column header at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>BetterListViewColumnHeader instance, or null.</returns>
        public BetterListViewColumnHeader GetColumnHeaderAt(Point location) {
            this.GetColumnHeaderAt(location, out var columnHeader, out var _);
            return columnHeader;
        }

        /// <summary>
        ///   Get column header at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="columnHeader">BetterListViewColumnHeader instance, or null.</param>
        /// <param name="columnHeaderPart">Part of the column header at the specified location.</param>
        public void GetColumnHeaderAt(Point location, out BetterListViewColumnHeader columnHeader, out BetterListViewHitPart columnHeaderPart) {
            this.GetColumnHeaderAt(location, includeBorders: false, out columnHeader, out columnHeaderPart);
        }

        /// <summary>
        ///   Get column header at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="includeBorders">Include column header borders (if the column headers are allowed to resize).</param>
        /// <param name="columnHeader">BetterListViewColumnHeader instance, or null.</param>
        /// <param name="columnHeaderPart">Part of the column header at the specified location.</param>
        public void GetColumnHeaderAt(Point location, bool includeBorders, out BetterListViewColumnHeader columnHeader, out BetterListViewHitPart columnHeaderPart) {
            columnHeader = null;
            columnHeaderPart = BetterListViewHitPart.Undefined;
            Rectangle boundsColumnHeaders = this.BoundsColumnHeaders;
            if (!this.IsAnyColumnHeaderVisible || boundsColumnHeaders.Height == 0 || location.Y < boundsColumnHeaders.Top || location.Y >= boundsColumnHeaders.Bottom) {
                return;
            }
            bool flag = false;
            for (int num = this.visibleRangeColumns.IndexElementLast; num >= 0; num--) {
                BetterListViewColumnHeader betterListViewColumnHeader = this.layoutElementsColumns[num];
                BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)betterListViewColumnHeader).LayoutBounds;
                if (betterListViewColumnHeader.Visible) {
                    Rectangle rectangle = betterListViewColumnHeaderBounds.BoundsOuter;
                    Rectangle boundsBorder = betterListViewColumnHeaderBounds.BoundsBorder;
                    if (includeBorders && betterListViewColumnHeader.AllowResize) {
                        rectangle = new Rectangle(rectangle.Left, rectangle.Top, boundsBorder.Right - rectangle.Left + 1, rectangle.Height);
                    }
                    Point offsetColumnsFromAbsolute = this.OffsetColumnsFromAbsolute;
                    rectangle.Offset(offsetColumnsFromAbsolute);
                    boundsBorder.Offset(offsetColumnsFromAbsolute);
                    if (rectangle.Contains(location)) {
                        columnHeader = betterListViewColumnHeader;
                        columnHeaderPart = BetterListViewHitPart.Other;
                        if (location.Y >= rectangle.Top + rectangle.Bottom >> 1) {
                            columnHeaderPart |= BetterListViewHitPart.Bottom;
                        }
                        if (location.X >= rectangle.Left + rectangle.Right >> 1) {
                            columnHeaderPart |= BetterListViewHitPart.Right;
                        }
                        if (location.Y >= rectangle.Top + rectangle.Height / 3 && location.Y < rectangle.Top + rectangle.Height * 2 / 3) {
                            columnHeaderPart |= BetterListViewHitPart.VCenter;
                        }
                        if (location.X >= rectangle.Left + rectangle.Width / 3 && location.X < rectangle.Left + rectangle.Width * 2 / 3) {
                            columnHeaderPart |= BetterListViewHitPart.HCenter;
                        }
                        if (betterListViewColumnHeaderBounds.BoundsOuter.Width == 0) {
                            break;
                        }
                        flag = true;
                    }
                    else if (flag) {
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///   Get item at the specified location in client coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the location to check.</param>
        /// <param name="y">Y-coordinate of the location to check.</param>
        /// <returns>BetterListViewItem instance, or null.</returns>
        public BetterListViewItem GetItemAt(int x, int y) {
            return this.GetItemAt(new Point(x, y));
        }

        /// <summary>
        ///   Get item at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>BetterListViewItem instance, or null.</returns>
        public BetterListViewItem GetItemAt(Point location) {
            this.GetItemAt(location, out var item, out var _);
            return item;
        }

        /// <summary>
        ///   Get item at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="item">BetterListViewItem instance, or null.</param>
        /// <param name="itemPart">Part of the item at the specified location.</param>
        public void GetItemAt(Point location, out BetterListViewItem item, out BetterListViewHitPart itemPart) {
            this.GetItemAt(location, selectableOnly: false, nearest: false, out item, out itemPart);
        }

        /// <summary>
        ///   Get item nearest to the specified location in client coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the location to check.</param>
        /// <param name="y">Y-coordinate of the location to check.</param>
        /// <returns>BetterListViewItem instance, or null.</returns>
        public BetterListViewItem GetItemAtNearest(int x, int y) {
            return this.GetItemAtNearest(new Point(x, y));
        }

        /// <summary>
        ///   Get item nearest to the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>BetterListViewItem instance, or null.</returns>
        public BetterListViewItem GetItemAtNearest(Point location) {
            this.GetItemAtNearest(location, out var item, out var _);
            return item;
        }

        /// <summary>
        ///   Get item nearest to the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="item">BetterListViewItem instance, or null.</param>
        /// <param name="itemPart">Part of the item at the specified location.</param>
        public void GetItemAtNearest(Point location, out BetterListViewItem item, out BetterListViewHitPart itemPart) {
            this.GetItemAt(location, selectableOnly: false, nearest: true, out item, out itemPart);
        }

        /// <summary>
        ///   Get sub-item at the specified location in client coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the location to check.</param>
        /// <param name="y">Y-coordinate of the location to check.</param>
        /// <returns>BetterListViewSubItem instance, or null.</returns>
        public BetterListViewSubItem GetSubItemAt(int x, int y) {
            return this.GetSubItemAt(new Point(x, y));
        }

        /// <summary>
        ///   Get sub-item at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>BetterListViewSubItem instance, or null.</returns>
        public BetterListViewSubItem GetSubItemAt(Point location) {
            this.GetSubItemAt(location, out var subItem, out var _);
            return subItem;
        }

        /// <summary>
        ///   Get sub-item at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="subItem">BetterListViewSubItem instance, or null.</param>
        /// <param name="subItemPart">Part of the sub-item at the specified location.</param>
        public void GetSubItemAt(Point location, out BetterListViewSubItem subItem, out BetterListViewHitPart subItemPart) {
            if (!this.IsAnyItemVisible) {
                subItem = null;
                subItemPart = BetterListViewHitPart.Undefined;
                return;
            }
            if (!base.BoundsContent.Contains(location)) {
                subItem = null;
                subItemPart = BetterListViewHitPart.Undefined;
                return;
            }
            this.GetItemAt(location, out var item, out var _);
            if (item == null) {
                subItem = null;
                subItemPart = BetterListViewHitPart.Undefined;
                return;
            }
            ReadOnlyCollection<BetterListViewSubItemBounds> subItemBounds = ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds).SubItemBounds;
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            for (int i = 0; i < subItemBounds.Count; i++) {
                BetterListViewSubItemBounds betterListViewSubItemBounds = subItemBounds[i];
                Rectangle rectangle = new Rectangle(betterListViewSubItemBounds.BoundsOuter.Left + offsetContentFromAbsolute.X, betterListViewSubItemBounds.BoundsOuter.Top + offsetContentFromAbsolute.Y, betterListViewSubItemBounds.BoundsOuter.Width, betterListViewSubItemBounds.BoundsOuter.Height);
                if (rectangle.Contains(location)) {
                    subItem = item.SubItems[i];
                    subItemPart = BetterListViewHitPart.Other;
                    if (location.Y >= rectangle.Top + rectangle.Bottom >> 1) {
                        subItemPart |= BetterListViewHitPart.Bottom;
                    }
                    if (location.X >= rectangle.Left + rectangle.Right >> 1) {
                        subItemPart |= BetterListViewHitPart.Right;
                    }
                    if (location.Y >= rectangle.Top + rectangle.Height / 3 && location.Y < rectangle.Top + rectangle.Height * 2 / 3) {
                        subItemPart |= BetterListViewHitPart.VCenter;
                    }
                    if (location.X >= rectangle.Left + rectangle.Width / 3 && location.X < rectangle.Left + rectangle.Width * 2 / 3) {
                        subItemPart |= BetterListViewHitPart.VCenter;
                    }
                    return;
                }
            }
            subItem = null;
            subItemPart = BetterListViewHitPart.Undefined;
        }

        /// <summary>
        ///   Get group at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location in client coordinates.</param>
        /// <returns>BetterListViewGroup instance, or null.</returns>
        public BetterListViewGroup GetGroupAt(Point location) {
            this.GetGroupAt(location, out var group, out var _);
            return group;
        }

        /// <summary>
        ///   Get group at the specified location in client coordinates.
        /// </summary>
        /// <param name="location">Location in client coordinates.</param>
        /// <param name="group">Located group.</param>
        /// <param name="groupPart">Located group part.</param>
        public void GetGroupAt(Point location, out BetterListViewGroup group, out BetterListViewHitPart groupPart) {
            if (!this.IsAnyGroupVisible) {
                group = null;
                groupPart = BetterListViewHitPart.Undefined;
                return;
            }
            if (!base.BoundsContent.Contains(location)) {
                group = null;
                groupPart = BetterListViewHitPart.Undefined;
                return;
            }
            BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
            Point offsetContentToAbsolute = this.OffsetContentToAbsolute;
            BetterListViewGroup element = layoutGroupsCurrent.GetElement(this.layoutElementsGroups, new Point(location.X + offsetContentToAbsolute.X, location.Y + offsetContentToAbsolute.Y), this.visibleRangeGroups);
            if (element != null) {
                group = element;
                BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)element).LayoutBounds;
                Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                Rectangle rectangle = new Rectangle(betterListViewGroupBounds.BoundsSelection.Left + offsetContentFromAbsolute.X, betterListViewGroupBounds.BoundsSelection.Top + offsetContentFromAbsolute.Y, betterListViewGroupBounds.BoundsSelection.Width, betterListViewGroupBounds.BoundsSelection.Height);
                if (rectangle.Contains(location)) {
                    groupPart = ((location.Y >= rectangle.Top + rectangle.Bottom >> 1) ? BetterListViewHitPart.Bottom : BetterListViewHitPart.Other);
                    return;
                }
            }
            group = null;
            groupPart = BetterListViewHitPart.Undefined;
        }

        /// <summary>
        ///   Get boundaries of the specified column header.
        /// </summary>
        /// <param name="columnHeader">Column header to get boundaries for.</param>
        /// <returns>BetterListViewColumnHeaderBounds instance, or null.</returns>
        public BetterListViewColumnHeaderBounds GetColumnHeaderBounds(BetterListViewColumnHeader columnHeader) {
            Checks.CheckNotNull(columnHeader, "columnHeader");
            if (base.IsUpdating || !this.IsAnyColumnHeaderVisible) {
                return null;
            }
            BetterListView listView = columnHeader.ListView;
            if (listView == null) {
                return null;
            }
            if (listView != this) {
                return listView.GetColumnHeaderBounds(columnHeader);
            }
            if (((IBetterListViewStateElement)columnHeader).State != BetterListViewElementState.ActiveFine) {
                return null;
            }
            BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds.Clone();
            betterListViewColumnHeaderBounds.Offset(this.OffsetColumnsFromAbsolute);
            return betterListViewColumnHeaderBounds;
        }

        /// <summary>
        ///   Get column header bounding rectangle.
        /// </summary>
        /// <param name="indexColumn">Column header index.</param>
        /// <returns>Column header bounding rectangle.</returns>
        public Rectangle GetColumnHeaderRect(int indexColumn) {
            Checks.CheckTrue(this.Columns.Count != 0, "Columns.Count != 0");
            Checks.CheckBounds(indexColumn, 0, this.Columns.Count - 1, "indexColumn");
            return this.GetColumnHeaderBounds(this.Columns[indexColumn])?.BoundsOuter ?? Rectangle.Empty;
        }

        /// <summary>
        ///   Get boundaries of the specified group.
        /// </summary>
        /// <param name="group">Group to get boundaries for.</param>
        /// <returns>BetterListViewGroupBounds instance, or null.</returns>
        public BetterListViewGroupBounds GetGroupBounds(BetterListViewGroup group) {
            Checks.CheckNotNull(group, "group");
            if (base.IsUpdating || !this.IsAnyGroupVisible) {
                return null;
            }
            BetterListView listView = group.ListView;
            if (listView == null) {
                return null;
            }
            if (listView != this) {
                return listView.GetGroupBounds(group);
            }
            if (((IBetterListViewStateElement)group).State != BetterListViewElementState.ActiveFine) {
                return null;
            }
            BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds.Clone();
            betterListViewGroupBounds.Offset(this.OffsetContentFromAbsolute);
            return betterListViewGroupBounds;
        }

        /// <summary>
        ///   Get group bounding rectangle.
        /// </summary>
        /// <param name="indexGroup">Group index.</param>
        /// <returns>Group bounding rectangle.</returns>
        public Rectangle GetGroupRect(int indexGroup) {
            Checks.CheckTrue(this.Groups.Count != 0, "Groups.Count != 0");
            Checks.CheckBounds(indexGroup, 0, this.Groups.Count - 1, "indexGroup");
            return this.GetGroupBounds(this.Groups[indexGroup])?.BoundsOuter ?? Rectangle.Empty;
        }

        /// <summary>
        ///   Get boundaries of the specified item.
        /// </summary>
        /// <param name="item">Item to get boundaries for.</param>
        /// <returns>BetterListViewItemBounds instance, or null.</returns>
        public BetterListViewItemBounds GetItemBounds(BetterListViewItem item) {
            Checks.CheckNotNull(item, "item");
            if (base.IsUpdating || !this.IsAnyItemVisible) {
                return null;
            }
            BetterListView listView = item.ListView;
            if (listView == null) {
                return null;
            }
            if (listView != this) {
                return listView.GetItemBounds(item);
            }
            BetterListViewElementState state = ((IBetterListViewStateElement)item).State;
            if (state != BetterListViewElementState.ActiveFine && state != BetterListViewElementState.ActiveVisible) {
                return null;
            }
            BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds.Clone();
            betterListViewItemBounds.Offset(this.OffsetContentFromAbsolute);
            return betterListViewItemBounds;
        }

        /// <summary>
        ///   Get item bounding rectangle.
        /// </summary>
        /// <param name="indexItem">Item index.</param>
        /// <returns>Item bounding rectangle.</returns>
        public Rectangle GetItemRect(int indexItem) {
            Checks.CheckTrue(this.Items.Count != 0, "Items.Count != 0");
            Checks.CheckBounds(indexItem, 0, this.Items.Count - 1, "indexItem");
            return this.GetItemBounds(this.Items[indexItem])?.BoundsOuterExtended ?? Rectangle.Empty;
        }

        /// <summary>
        ///   Retrieves the specified portion of the item boundaries.
        /// </summary>
        /// <param name="indexItem">Index of the item for which to retrieve boundaries.</param>
        /// <param name="itemBoundsPortion">Portion of the item boundaries.</param>
        /// <returns>Portion of the item boundaries as rectangle, if defined. Rectangle.Empty, otherwise.</returns>
        public Rectangle GetItemRect(int indexItem, BetterListViewItemBoundsPortion itemBoundsPortion) {
            Checks.CheckTrue(this.Items.Count != 0, "Items.Count != 0");
            Checks.CheckBounds(indexItem, 0, this.Items.Count - 1, "indexItem");
            BetterListViewItemBounds itemBounds = this.GetItemBounds(this.Items[indexItem]);
            if (itemBounds == null) {
                return Rectangle.Empty;
            }
            //return itemBoundsPortion switch {
            //    BetterListViewItemBoundsPortion.Entire => itemBounds.BoundsOuterExtended,
            //    BetterListViewItemBoundsPortion.Icon => itemBounds.SubItemBounds[0].BoundsImage,
            //    BetterListViewItemBoundsPortion.Label => itemBounds.SubItemBounds[0].BoundsText,
            //    BetterListViewItemBoundsPortion.ItemOnly => itemBounds.SubItemBounds[0].BoundsOuter,
            //    BetterListViewItemBoundsPortion.Selection => itemBounds.BoundsSelection,
            //    BetterListViewItemBoundsPortion.ExpandButton => itemBounds.BoundsExpandButton,
            //    BetterListViewItemBoundsPortion.CheckBox => itemBounds.BoundsCheckBox,
            //    _ => throw new ApplicationException($"Unknown item bounds portion: '{itemBoundsPortion}'."),
            //};
            switch (itemBoundsPortion) {
                case BetterListViewItemBoundsPortion.Entire: return itemBounds.BoundsOuterExtended; break;
                case BetterListViewItemBoundsPortion.Icon: return itemBounds.SubItemBounds[0].BoundsImage; break;
                case BetterListViewItemBoundsPortion.Label: return itemBounds.SubItemBounds[0].BoundsText; break;
                case BetterListViewItemBoundsPortion.ItemOnly: return itemBounds.SubItemBounds[0].BoundsOuter; break;
                case BetterListViewItemBoundsPortion.Selection: return itemBounds.BoundsSelection; break;
                case BetterListViewItemBoundsPortion.ExpandButton: return itemBounds.BoundsExpandButton; break;
                case BetterListViewItemBoundsPortion.CheckBox: return itemBounds.BoundsCheckBox; break;
                default: throw new ApplicationException($"Unknown item bounds portion: '{itemBoundsPortion}'."); break;
            };
        }

        /// <summary>
        ///   Get boundaries of the specified sub-item.
        /// </summary>
        /// <param name="subItem">Sub-item to get boundaries for.</param>
        /// <returns>BetterListViewSubItemBounds instance, or null.</returns>
        public BetterListViewSubItemBounds GetSubItemBounds(BetterListViewSubItem subItem) {
            Checks.CheckNotNull(subItem, "subItem");
            if (base.IsUpdating || !this.IsAnyItemVisible) {
                return null;
            }
            BetterListView listView = subItem.ListView;
            if (listView == null) {
                return null;
            }
            if (listView != this) {
                return listView.GetSubItemBounds(subItem);
            }
            BetterListViewItemBounds itemBounds = this.GetItemBounds(subItem.Item);
            if (itemBounds == null || subItem.Index >= itemBounds.SubItemBounds.Count) {
                return null;
            }
            return itemBounds.SubItemBounds[subItem.Index];
        }

        /// <summary>
        ///   Get sub-item bounding rectangle.
        /// </summary>
        /// <param name="indexItem">Parent item index.</param>
        /// <param name="indexColumn">Corresponding column index.</param>
        /// <returns>Sub-item bounding rectangle.</returns>
        public Rectangle GetSubItemRect(int indexItem, int indexColumn) {
            Checks.CheckTrue(this.Items.Count != 0, "Items.Count != 0");
            Checks.CheckBounds(indexItem, 0, this.Items.Count - 1, "indexItem");
            Checks.CheckTrue(this.Columns.Count != 0, "Columns.Count != 0");
            Checks.CheckBounds(indexColumn, 0, this.Columns.Count - 1, "indexColumn");
            return this.GetSubItemBounds(this.Items[indexItem].SubItems[indexColumn])?.BoundsOuter ?? Rectangle.Empty;
        }

        private void GetItemAt(Point location, bool selectableOnly, bool nearest, out BetterListViewItem item, out BetterListViewHitPart itemPart) {
            if (!this.IsAnyItemVisible) {
                item = null;
                itemPart = BetterListViewHitPart.Undefined;
                return;
            }
            if (!base.BoundsContent.Contains(location)) {
                item = null;
                itemPart = BetterListViewHitPart.Undefined;
                return;
            }
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            Point offsetContentToAbsolute = this.OffsetContentToAbsolute;
            Point location2 = new Point(location.X + offsetContentToAbsolute.X, location.Y + offsetContentToAbsolute.Y);
            ReadOnlyCollection<BetterListViewItem> elements;
            BetterListViewLayoutVisibleRange visibleRange;
            if (selectableOnly) {
                elements = this.layoutElementsItemsSelection;
                visibleRange = this.visibleRangeItemsSelection;
            }
            else {
                elements = this.layoutElementsItemsDisplay;
                visibleRange = this.visibleRangeItemsDisplay;
            }
            BetterListViewItem betterListViewItem = ((!nearest) ? layoutItemsCurrent.GetElement(elements, location2, visibleRange) : layoutItemsCurrent.GetElementNearest(elements, location2));
            if (betterListViewItem != null) {
                item = betterListViewItem;
                BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds;
                Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                Rectangle rectangle = new Rectangle(betterListViewItemBounds.BoundsOuterExtended.X + offsetContentFromAbsolute.X, betterListViewItemBounds.BoundsOuterExtended.Y + offsetContentFromAbsolute.Y, betterListViewItemBounds.BoundsOuterExtended.Width, betterListViewItemBounds.BoundsOuterExtended.Height);
                itemPart = BetterListViewHitPart.Other;
                if (location.Y >= rectangle.Top + rectangle.Bottom >> 1) {
                    itemPart |= BetterListViewHitPart.Bottom;
                }
                if (location.X >= rectangle.Left + rectangle.Right >> 1) {
                    itemPart |= BetterListViewHitPart.Right;
                }
                if (location.Y >= rectangle.Top + rectangle.Height / 3 && location.Y < rectangle.Top + rectangle.Height * 2 / 3) {
                    itemPart |= BetterListViewHitPart.VCenter;
                }
                if (location.X >= rectangle.Left + rectangle.Width / 3 && location.X < rectangle.Left + rectangle.Width * 2 / 3) {
                    itemPart |= BetterListViewHitPart.HCenter;
                }
            }
            else {
                item = null;
                itemPart = BetterListViewHitPart.Undefined;
            }
        }

        private bool LayoutPosition() {
            if (this.contentMeasurement.IsEmpty) {
                return false;
            }
            if (this.layoutElementsColumns.Count != 0) {
                this.layoutColumns.PositionElements(this.layoutElementsColumns, 0, this.layoutElementsColumns.Count - 1);
            }
            if (this.ShowGroups && this.layoutElementsGroups.Count != 0) {
                BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
                layoutGroupsCurrent.PositionElements(this.layoutElementsGroups, this.layoutElementsItemsDisplay, (this.layoutElementsColumns.Count != 0) ? ((BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)this.layoutElementsColumns[0]).LayoutBounds) : null, this.contentMeasurement.Size, this.LayoutItemsCurrent);
            }
            else if (this.layoutElementsItemsDisplay.Count != 0) {
                ((IBetterListViewLayoutElementDisplayable)this.layoutElementsGroups[0]).LayoutBounds.Reset();
                BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
                layoutItemsCurrent.PositionElements(this.layoutElementsItemsDisplay, (this.layoutElementsColumns.Count != 0) ? ((BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)this.layoutElementsColumns[0]).LayoutBounds) : null, 0, this.layoutElementsItemsDisplay.Count - 1, this.contentMeasurement.Size, Size.Empty);
            }
            return true;
        }

        /// <summary>
        /// Called when value of one of the scroll bar changes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        protected override void OnScrollBarValueChanged(object sender, EventArgs eventArgs) {
            this.LayoutScroll();
            if (this.StateInfo.State == BetterListViewState.LabelEdit) {
                try {
                    base.BeginUpdate();
                    this.LabelEditEnd(forced: true);
                }
                finally {
                    base.EndUpdate(suppressRefresh: true);
                }
            }
            else if (this.StateInfo.State == BetterListViewState.ItemSelection) {
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                this.UpdateDragSelection(this.StateInfo.ItemSelectionStateInfo, ref selectionInfoNew);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.MouseDragSelection);
            }
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.SetHitTestInfo(this.HitTest());
            this.RefreshView();
            base.OnScrollBarValueChanged(sender, eventArgs);
        }

        private void LayoutScroll() {
            if (this.layoutElementsColumns.Count != 0) {
                int num;
                int num4;
                if (this.visibleRangeColumns.IsUndefined) {
                    num = 0;
                    num4 = 0;
                }
                else {
                    num = Math.Min(Math.Max(this.visibleRangeColumns.IndexElementFirst, 0), this.layoutElementsColumns.Count - 1);
                    num4 = Math.Min(Math.Max(this.visibleRangeColumns.IndexElementLast, 0), this.layoutElementsColumns.Count - 1);
                    if (num > num4) {
                        int num5 = num + num4 >> 1;
                        num = num5;
                        num4 = num5;
                    }
                    this.visibleRangeColumns = new BetterListViewLayoutVisibleRange(num, num4);
                }
                this.visibleRangeColumns = this.layoutColumns.GetVisibleRange(this.layoutElementsColumns, this.BoundsColumnHeaders, this.OffsetColumnsFromAbsolute, num, num4);
            }
            else {
                this.visibleRangeColumns = BetterListViewLayoutVisibleRange.Undefined;
            }
            if (!this.commonMeasurementItems.IsEmpty && this.layoutElementsItemsDisplay.Count != 0) {
                int num6;
                int num7;
                if (this.visibleRangeItemsDisplay.IsUndefined) {
                    num6 = 0;
                    num7 = 0;
                }
                else {
                    num6 = Math.Min(Math.Max(this.visibleRangeItemsDisplay.IndexElementFirst, 0), this.layoutElementsItemsDisplay.Count - 1);
                    num7 = Math.Min(Math.Max(this.visibleRangeItemsDisplay.IndexElementLast, 0), this.layoutElementsItemsDisplay.Count - 1);
                    for (int i = num6; i <= num7; i++) {
                        ((IBetterListViewStateElement)this.layoutElementsItemsDisplay[i]).ChangeState(BetterListViewElementStateChange.MakeInvisible);
                    }
                    if (num6 > num7) {
                        int num8 = num6 + num7 >> 1;
                        num6 = num8;
                        num7 = num8;
                    }
                }
                this.visibleRangeItemsDisplay = new BetterListViewLayoutVisibleRange(num6, num7);
                BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
                this.visibleRangeItemsDisplay = layoutItemsCurrent.GetVisibleRange(this.layoutElementsItemsDisplay, base.BoundsContent, this.OffsetContentFromAbsolute, num6, num7);
                if (!this.visibleRangeItemsDisplay.IsUndefined) {
                    Graphics cachedGraphics = base.CachedGraphics;
                    ImageList imageListCurrent = this.ImageListCurrent;
                    int num9 = int.MaxValue;
                    int num10 = int.MinValue;
                    bool flag = false;
                    for (int j = this.visibleRangeItemsDisplay.IndexElementFirst; j <= this.visibleRangeItemsDisplay.IndexElementLast; j++) {
                        BetterListViewItem betterListViewItem = this.layoutElementsItemsDisplay[j];
                        if (((IBetterListViewStateElement)betterListViewItem).State == BetterListViewElementState.ActiveCoarse) {
                            bool flag2 = betterListViewItem.CheckBoxAppearance != BetterListViewCheckBoxAppearance.Hide;
                            layoutItemsCurrent.MeasureElementFine(betterListViewItem, cachedGraphics, imageListCurrent, this.layoutElementsColumns, this.FullRowSelect, this.enableExpandButtons && this.ShowItemExpandButtons, this.ShowItemExpandButtons && betterListViewItem.AllowShowExpandButton && betterListViewItem.ChildItems.Count != 0, this.CheckBoxesVisible && (this.CheckBoxesAlign || flag2), this.CheckBoxesVisible && flag2, this.ExtraPadding, this.commonMeasurementItems, this.commonMeasurementItemsOffsets);
                            ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.MeasureFine);
                        }
                        if (((IBetterListViewStateElement)betterListViewItem).State == BetterListViewElementState.ActiveFine) {
                            ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.MakeVisible);
                        }
                        if (betterListViewItem.IsSelectable) {
                            int layoutIndexSelection = ((IBetterListViewLayoutElementSelectable)betterListViewItem).LayoutIndexSelection;
                            num9 = Math.Min(num9, layoutIndexSelection);
                            num10 = Math.Max(num10, layoutIndexSelection);
                            flag = true;
                        }
                    }
                    this.visibleRangeItemsSelection = (flag ? new BetterListViewLayoutVisibleRange(num9, num10) : BetterListViewLayoutVisibleRange.Undefined);
                }
                else {
                    this.visibleRangeItemsSelection = BetterListViewLayoutVisibleRange.Undefined;
                }
            }
            else {
                this.visibleRangeItemsDisplay = BetterListViewLayoutVisibleRange.Undefined;
                this.visibleRangeItemsSelection = BetterListViewLayoutVisibleRange.Undefined;
            }
            if (this.layoutElementsGroups.Count != 0) {
                int num11;
                int num2;
                if (this.visibleRangeGroups.IsUndefined) {
                    num11 = 0;
                    num2 = 0;
                }
                else {
                    num11 = Math.Min(Math.Max(this.visibleRangeGroups.IndexElementFirst, 0), this.layoutElementsGroups.Count - 1);
                    num2 = Math.Min(Math.Max(this.visibleRangeGroups.IndexElementLast, 0), this.layoutElementsGroups.Count - 1);
                    for (int k = num11; k <= num2; k++) {
                        ((IBetterListViewStateElement)this.layoutElementsGroups[k]).ChangeState(BetterListViewElementStateChange.MakeInvisible);
                    }
                    if (num11 > num2) {
                        int num3 = num11 + num2 >> 1;
                        num11 = num3;
                        num2 = num3;
                    }
                }
                this.visibleRangeGroups = new BetterListViewLayoutVisibleRange(num11, num2);
                this.visibleRangeGroups = this.LayoutGroupsCurrent.GetVisibleRange(this.layoutElementsGroups, base.BoundsContent, this.OffsetContentFromAbsolute, num11, num2);
                if (!this.visibleRangeGroups.IsUndefined) {
                    for (int l = this.visibleRangeGroups.IndexElementFirst; l <= this.visibleRangeGroups.IndexElementLast; l++) {
                        BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[l];
                        if (((IBetterListViewStateElement)betterListViewGroup).State == BetterListViewElementState.ActiveFine) {
                            ((IBetterListViewStateElement)betterListViewGroup).ChangeState(BetterListViewElementStateChange.MakeVisible);
                        }
                    }
                }
            }
            else {
                this.visibleRangeGroups = BetterListViewLayoutVisibleRange.Undefined;
            }
            this.SetHitTestInfo(this.HitTest());
        }

        private void LayoutSetup() {
            List<BetterListViewColumnHeader> list = new List<BetterListViewColumnHeader>();
            if (this.ColumnsVisible) {
                foreach (BetterListViewColumnHeader column in this.Columns) {
                    ((IBetterListViewLayoutElementDisplayable)column).LayoutIndexDisplay = column.Index;
                    ((IBetterListViewStateElement)column).ChangeState(BetterListViewElementStateChange.Activate);
                    list.Add(column);
                }
            }
            else {
                foreach (BetterListViewColumnHeader column2 in this.Columns) {
                    ((IBetterListViewStateElement)column2).ChangeState(BetterListViewElementStateChange.Deactivate);
                }
            }
            this.layoutElementsColumns = new ReadOnlyCollection<BetterListViewColumnHeader>(list);
            this.layoutMeasurementColumns = BetterListViewLayoutMeasurement.Empty;
            List<BetterListViewGroup> list2 = new List<BetterListViewGroup>();
            List<BetterListViewItem> elementsItemsDisplay = new List<BetterListViewItem>();
            List<BetterListViewItem> elementsItemsSelection = new List<BetterListViewItem>();
            int indexItemDisplay = 0;
            int indexItemSelection = 0;
            bool flag = false;
            this.visibleRangeItemsDisplay = BetterListViewLayoutVisibleRange.Undefined;
            this.visibleRangeItemsSelection = BetterListViewLayoutVisibleRange.Undefined;
            if (this.Items.Count != 0) {
                foreach (BetterListViewItem current3 in this) {
                    ((IBetterListViewStateElement)current3).ChangeState(BetterListViewElementStateChange.Deactivate);
                }
            }
            if (this.Groups.Count != 0) {
                ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.Deactivate);
                foreach (BetterListViewGroup group in this.Groups) {
                    ((IBetterListViewStateElement)group).ChangeState(BetterListViewElementStateChange.Deactivate);
                }
            }
            if (this.ShowGroups) {
                Dictionary<BetterListViewGroup, ReadOnlyCollection<BetterListViewItem>> dictionary = new Dictionary<BetterListViewGroup, ReadOnlyCollection<BetterListViewItem>>();
                List<BetterListViewGroup> list3 = new List<BetterListViewGroup>();
                List<BetterListViewItem> list4 = new List<BetterListViewItem>();
                ReadOnlyCollection<BetterListViewItem> readOnlyCollection = this.defaultGroup.GetItems(this);
                int num = 0;
                foreach (BetterListViewItem item in readOnlyCollection) {
                    if (item.Visible) {
                        num++;
                    }
                }
                if (num != 0) {
                    dictionary.Add(this.defaultGroup, readOnlyCollection);
                    list3.Add(this.defaultGroup);
                    list4.AddRange(readOnlyCollection);
                }
                foreach (BetterListViewGroup group2 in this.Groups) {
                    readOnlyCollection = group2.GetItems(this);
                    num = 0;
                    foreach (BetterListViewItem item2 in readOnlyCollection) {
                        if (item2.Visible) {
                            num++;
                        }
                    }
                    if (num != 0 || this.ShowEmptyGroups) {
                        dictionary.Add(group2, readOnlyCollection);
                        list3.Add(group2);
                        list4.AddRange(readOnlyCollection);
                    }
                }
                int num2 = 0;
                foreach (KeyValuePair<BetterListViewGroup, ReadOnlyCollection<BetterListViewItem>> item3 in dictionary) {
                    BetterListViewGroup key = item3.Key;
                    ReadOnlyCollection<BetterListViewItem> value = item3.Value;
                    ((IBetterListViewLayoutElementDisplayable)key).LayoutIndexDisplay = num2;
                    ((IBetterListViewLayoutElementSelectable)key).LayoutIndexSelection = num2;
                    ((IBetterListViewStateElement)key).ChangeState(BetterListViewElementStateChange.Activate);
                    list2.Add(key);
                    if (key.IsExpanded && value.Count != 0) {
                        int indexItemDisplay2 = indexItemDisplay;
                        int indexItemSelection2 = indexItemSelection;
                        this.AddItemsToLayoutElementCollection(key, value, this.View == BetterListViewView.Details, allowSelectable: true, ref elementsItemsDisplay, ref elementsItemsSelection, ref indexItemDisplay2, ref indexItemSelection2, ref flag);
                        if (indexItemDisplay != indexItemDisplay2) {
                            key.LayoutIndexItemDisplayFirst = indexItemDisplay;
                            key.LayoutIndexItemDisplayLast = indexItemDisplay2 - 1;
                        }
                        else {
                            key.LayoutIndexItemDisplayFirst = -1;
                            key.LayoutIndexItemDisplayLast = -1;
                        }
                        if (indexItemSelection != indexItemSelection2) {
                            key.LayoutIndexItemSelectionFirst = indexItemSelection;
                            key.LayoutIndexItemSelectionLast = indexItemSelection2 - 1;
                        }
                        else {
                            key.LayoutIndexItemSelectionFirst = -1;
                            key.LayoutIndexItemSelectionLast = -1;
                        }
                        indexItemDisplay = indexItemDisplay2;
                        indexItemSelection = indexItemSelection2;
                    }
                    else {
                        key.LayoutIndexItemDisplayFirst = -1;
                        key.LayoutIndexItemDisplayLast = -1;
                        key.LayoutIndexItemSelectionFirst = -1;
                        key.LayoutIndexItemSelectionLast = -1;
                    }
                    num2++;
                }
            }
            else if (this.Items.Count != 0) {
                ((IBetterListViewLayoutElementDisplayable)this.defaultGroup).LayoutIndexDisplay = 0;
                ((IBetterListViewLayoutElementSelectable)this.defaultGroup).LayoutIndexSelection = 0;
                ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.Activate);
                list2.Add(this.defaultGroup);
                this.AddItemsToLayoutElementCollection(this.defaultGroup, this.Items, this.View == BetterListViewView.Details, allowSelectable: true, ref elementsItemsDisplay, ref elementsItemsSelection, ref indexItemDisplay, ref indexItemSelection, ref flag);
                this.defaultGroup.LayoutIndexItemDisplayFirst = 0;
                this.defaultGroup.LayoutIndexItemDisplayLast = elementsItemsDisplay.Count - 1;
                this.defaultGroup.LayoutIndexItemSelectionFirst = 0;
                this.defaultGroup.LayoutIndexItemSelectionLast = elementsItemsSelection.Count - 1;
            }
            this.layoutElementsGroups = new ReadOnlyCollection<BetterListViewGroup>(list2);
            this.layoutElementsItemsDisplay = new ReadOnlyCollection<BetterListViewItem>(elementsItemsDisplay);
            this.layoutElementsItemsSelection = new ReadOnlyCollection<BetterListViewItem>(elementsItemsSelection);
            this.layoutMeasurementGroups = BetterListViewLayoutMeasurement.Empty;
            if (this.enableExpandButtons == flag) {
                return;
            }
            this.enableExpandButtons = flag;
            if (this.View != BetterListViewView.Details) {
                return;
            }
            foreach (BetterListViewItem current4 in this) {
                ((IBetterListViewStateElement)current4).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
            }
        }

        private void AddItemsToLayoutElementCollection(BetterListViewGroup group, IEnumerable<BetterListViewItem> items, bool recurse, bool allowSelectable, ref List<BetterListViewItem> elementsItemsDisplay, ref List<BetterListViewItem> elementsItemsSelection, ref int indexItemDisplay, ref int indexItemSelection, ref bool enableExpandButtons) {
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            int count = this.layoutElementsColumns.Count;
            foreach (BetterListViewItem item in items) {
                if (!item.Visible) {
                    continue;
                }
                IBetterListViewLayoutElementDisplayable betterListViewLayoutElementDisplayable = item;
                IBetterListViewLayoutElementSelectable betterListViewLayoutElementSelectable = item;
                IBetterListViewStateElement betterListViewStateElement = item;
                BetterListViewGroup visibleGroup = this.GetVisibleGroup(item, allowNull: false);
                BetterListViewGroup group2 = item.Group;
                if (visibleGroup != group || (group2 != null && group2.ListView != this)) {
                    continue;
                }
                int subItemCount = layoutItemsCurrent.GetSubItemCount(item, count);
                if (((BetterListViewItemBounds)betterListViewLayoutElementDisplayable.LayoutBounds).SubItemBounds.Count != subItemCount) {
                    betterListViewLayoutElementDisplayable.LayoutBounds = new BetterListViewItemBounds(subItemCount);
                }
                betterListViewLayoutElementDisplayable.LayoutIndexDisplay = indexItemDisplay++;
                betterListViewStateElement.ChangeState(BetterListViewElementStateChange.Activate);
                elementsItemsDisplay.Add(item);
                if (allowSelectable && item.Selectable) {
                    betterListViewLayoutElementSelectable.LayoutIndexSelection = indexItemSelection++;
                    elementsItemsSelection.Add(item);
                }
                else {
                    betterListViewLayoutElementSelectable.LayoutIndexSelection = -1;
                }
                if (item.ChildItems.Count != 0 && recurse) {
                    enableExpandButtons = true;
                    if (item.IsExpanded) {
                        this.AddItemsToLayoutElementCollection(group, item.ChildItems, recurse: true, allowSelectable && item.AllowSelectChildItems, ref elementsItemsDisplay, ref elementsItemsSelection, ref indexItemDisplay, ref indexItemSelection, ref enableExpandButtons);
                    }
                }
            }
        }

        /// <summary>
        ///   Resets the <see cref="P:System.Windows.Forms.Control.Cursor" /> property to its default value.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        public override void ResetCursor() {
            this.SetCursor(BetterListViewCursorType.Default);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseEnter(EventArgs e) {
            this.SetHitTestInfo(this.HitTest());
            this.RefreshView();
            base.OnMouseEnter(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseHover" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseHover(EventArgs e) {
            BetterListViewHitTestInfo betterListViewHitTestInfo = this.HitTestInfo;
            if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.Item) == BetterListViewHitTestLocations.Item) {
                this.OnItemMouseHover(new BetterListViewItemMouseHoverEventArgs(betterListViewHitTestInfo.ItemDisplay));
            }
            base.OnMouseHover(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseLeave(EventArgs e) {
            if (!base.IsUpdating) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewHitTestInfo betterListViewHitTestInfo = this.HitTestInfo;
                betterListViewHitTestInfo = BetterListViewHitTestInfo.Empty;
                this.SetStateInfo(betterListViewStateInfo);
                this.SetHitTestInfo(betterListViewHitTestInfo);
                this.ToolTipsMouseLeave();
                this.RefreshView();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e) {
            this.LabelEditEnd(forced: false, this.StateInfo.State == BetterListViewState.LabelEdit);
            BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
            BetterListViewFocusInfo focusInfoNew = this.FocusInfo;
            BetterListViewSelectionInfo empty = this.SelectionInfo;
            BetterListViewHitTestInfo betterListViewHitTestInfo = (this.hitTestInfoMouseDown = this.HitTest());
            if (!base.IsUpdating) {
                if (betterListViewStateInfo.State == BetterListViewState.ColumnReorder && e.Button == MouseButtons.Right) {
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                }
                betterListViewStateInfo.FocusedByMouseDown = !this.Focused;
                if (!this.Focused) {
                    try {
                        base.BeginUpdate();
                        base.Focus();
                    }
                    finally {
                        base.EndUpdate(suppressRefresh: true);
                    }
                }
                if (betterListViewStateInfo.State != BetterListViewState.ItemBeforeCheckKeyboard) {
                    if (!this.ReadOnly && (betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.Group) == BetterListViewHitTestLocations.Group && e.Button == MouseButtons.Left) {
                        betterListViewStateInfo.State = BetterListViewState.GroupBeforeSelection;
                    }
                    else if (!this.ReadOnly) {
                        if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ItemCheckBox) == BetterListViewHitTestLocations.ItemCheckBox) {
                            BetterListViewItem itemDisplay = betterListViewHitTestInfo.ItemDisplay;
                            BetterListViewReadOnlyItemSet betterListViewReadOnlyItemSet = ((!this.GroupItemCheck || !itemDisplay.Selected) ? new BetterListViewReadOnlyItemSet(new BetterListViewItem[1] { betterListViewHitTestInfo.ItemDisplay }) : new BetterListViewReadOnlyItemSet(this.SelectedItemsSet));
                            betterListViewStateInfo.State = BetterListViewState.ItemBeforeCheckMouse;
                            betterListViewStateInfo.ItemBeforeCheckStateInfo = new BetterListViewItemBeforeCheckStateInfo(itemDisplay, betterListViewReadOnlyItemSet);
                        }
                        else if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ItemSelection) == BetterListViewHitTestLocations.ItemSelection) {
                            bool flag = !this.MultiSelect || ((betterListViewHitTestInfo.Locations & (BetterListViewHitTestLocations.ItemImage | BetterListViewHitTestLocations.ItemText | BetterListViewHitTestLocations.SubItemImage | BetterListViewHitTestLocations.SubItemText)) != 0 && !betterListViewHitTestInfo.ItemSelection.Selected && (Control.ModifierKeys & Keys.Control) != Keys.Control);
                            if (flag) {
                                this.MakeSelection(betterListViewHitTestInfo.ItemSelection, !this.MultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control, (Control.ModifierKeys & Keys.Shift) == Keys.Shift, BetterListViewSelectionOptions.TakeFocus | BetterListViewSelectionOptions.ClearSearchQuery, BetterListViewSelectionChangeMode.MouseButton, ref empty, ref focusInfoNew);
                            }
                            bool flag2 = (betterListViewHitTestInfo.ItemSelection.Selected || flag) && (this.ItemReorderMode != 0 || this.AllowDrag);
                            if (flag2 || this.MultiSelect) {
                                betterListViewStateInfo.State = (flag2 ? BetterListViewState.ItemBeforeDrag : BetterListViewState.ItemBeforeSelection);
                            }
                            else if (betterListViewHitTestInfo.ItemSelection.Selected || flag) {
                                betterListViewStateInfo.State = BetterListViewState.ItemBeforeLabelEdit;
                            }
                            betterListViewStateInfo.ItemSelectionStateInfo = new BetterListViewItemSelectionStateInfo(e.Location, base.ScrollPosition, new BetterListViewReadOnlyGroupSet(this.GetSelectedGroups(this.SelectionInfo.SelectedItems.Keys)), new BetterListViewReadOnlyItemSet(this.SelectionInfo.SelectedItems.Keys), (Control.ModifierKeys & Keys.Control) == Keys.Control, (Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                            if (e.Button == MouseButtons.Left) {
                                focusInfoNew = new BetterListViewFocusInfo(betterListViewHitTestInfo.ItemSelection, 0);
                            }
                            try {
                                base.BeginUpdate();
                                this.EnsureVisible(betterListViewHitTestInfo.ItemSelection);
                            }
                            finally {
                                base.EndUpdate();
                            }
                        }
                        else if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ItemSelection) != BetterListViewHitTestLocations.ItemSelection && base.BoundsContent.Contains(e.Location)) {
                            if ((this.IsAnyItemSelectable || this.IsAnyGroupVisible) && this.MultiSelect) {
                                betterListViewStateInfo.State = BetterListViewState.ItemBeforeSelection;
                                betterListViewStateInfo.ItemSelectionStateInfo = new BetterListViewItemSelectionStateInfo(e.Location, base.ScrollPosition, new BetterListViewReadOnlyGroupSet(this.GetSelectedGroups(this.SelectionInfo.SelectedItems.Keys)), new BetterListViewReadOnlyItemSet(this.SelectionInfo.SelectedItems.Keys), (Control.ModifierKeys & Keys.Control) == Keys.Control, (Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                                if (this.AllowAutoScroll) {
                                    base.AutoScrollStart(BetterListViewAutoScrollMode.Outside);
                                }
                            }
                            if (this.IsAnyItemSelectable && !this.MultiSelect) {
                                empty = BetterListViewSelectionInfo.Empty;
                            }
                        }
                        else if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) == BetterListViewHitTestLocations.ColumnHeaderBorder && betterListViewHitTestInfo.ColumnHeader.AllowResize && this.ColumnsVisible && e.Button == MouseButtons.Left) {
                            betterListViewStateInfo.State = BetterListViewState.ColumnBeforeResize;
                            betterListViewStateInfo.ColumnResizeStateInfo = new BetterListViewColumnResizeStateInfo(e.Location, betterListViewHitTestInfo.ColumnHeader, betterListViewHitTestInfo.ColumnHeader.Width, betterListViewHitTestInfo.ColumnHeader.Width, betterListViewHitTestInfo.ColumnHeader.SmoothResize);
                        }
                        else if ((betterListViewHitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeader) == BetterListViewHitTestLocations.ColumnHeader && this.ColumnsVisible && e.Button == MouseButtons.Left) {
                            betterListViewStateInfo.State = BetterListViewState.ColumnSelection;
                            if (e.Location.X >= 0 && e.Location.Y >= 0) {
                                betterListViewStateInfo.ColumnSelectionStateInfo = new BetterListViewColumnSelectionStateInfo(e.Location, betterListViewHitTestInfo.ColumnHeader);
                            }
                        }
                    }
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetFocusInfo(focusInfoNew, BetterListViewSelectionChangeMode.MouseButton);
                this.SetSelectionInfo(empty, BetterListViewSelectionChangeMode.MouseButton);
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e) {
            if (!base.IsUpdating && !this.ReadOnly) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewFocusInfo empty = this.FocusInfo;
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                if (betterListViewStateInfo.State == BetterListViewState.ColumnReorder) {
                    int num = this.BoundsColumnHeaders.Height * 3;
                    if (e.Y < -num || e.Y >= this.BoundsColumnHeaders.Bottom + num) {
                        betterListViewStateInfo.State = BetterListViewState.Normal;
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.Normal) {
                    if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) == BetterListViewHitTestLocations.ColumnHeaderBorder && this.HitTestInfo.ColumnHeader.AllowResize) {
                        this.SetCursor(BetterListViewCursorType.ColumnResize);
                    }
                    else if (this.Activation == ItemActivation.OneClick && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.Item) == BetterListViewHitTestLocations.Item) {
                        this.SetCursor(BetterListViewCursorType.ItemActivation);
                    }
                    else {
                        this.ResetCursor();
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ColumnSelection && Math.Abs(e.Location.X - betterListViewStateInfo.ColumnSelectionStateInfo.StartPoint.X) >= SystemInformation.DragSize.Width && this.ColumnReorderMode != 0) {
                    BetterListViewColumnHeader columnHeader = this.HitTestInfo.ColumnHeader;
                    if (columnHeader != null && base.ClientRectangleInner.Contains(e.Location)) {
                        this.SetCursor(BetterListViewCursorType.ColumnReorder);
                        BetterListViewColumnHeaderBounds columnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds;
                        Bitmap columnBitmap = BetterListViewPainter.CreateColumnHeaderBitmap(this.ImageListColumns, columnHeader, columnHeaderBounds, this.sortList.Contains(columnHeader.Index) ? columnHeader.SortOrder : BetterListViewSortOrder.None, this.layoutColumns.MaximumTextLines, this.Columns.Count);
                        betterListViewStateInfo.State = BetterListViewState.ColumnReorder;
                        betterListViewStateInfo.ColumnReorderStateInfo = new BetterListViewColumnReorderStateInfo(e.Location, base.ScrollPositionHorizontal, columnHeader, columnBitmap);
                        if (this.AllowAutoScroll && this.LayoutItemsCurrent.OrientationVertical && base.HScrollBarVisible) {
                            base.AutoScrollStart(BetterListViewAutoScrollMode.Horizontal);
                        }
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ColumnReorder) {
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, this.BoundsColumnHeaders);
                }
                if (betterListViewStateInfo.State == BetterListViewState.ColumnBeforeResize || betterListViewStateInfo.State == BetterListViewState.ColumnResize) {
                    BetterListViewColumnResizeStateInfo columnResizeStateInfo = betterListViewStateInfo.ColumnResizeStateInfo;
                    int val = columnResizeStateInfo.ColumnWidthOriginal + e.X - columnResizeStateInfo.StartPoint.X;
                    val = Math.Min(val, columnResizeStateInfo.Column.MaximumWidth);
                    val = Math.Max(val, columnResizeStateInfo.Column.MinimumWidth);
                    if (val != columnResizeStateInfo.Column.Width) {
                        BetterListViewColumnWidthChangingEventArgs betterListViewColumnWidthChangingEventArgs = new BetterListViewColumnWidthChangingEventArgs(columnResizeStateInfo.Column, BetterListViewColumnWidthChangeSource.ResizeMouse, val);
                        try {
                            base.BeginUpdate();
                            this.OnColumnWidthChanging(betterListViewColumnWidthChangingEventArgs);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                        if (betterListViewColumnWidthChangingEventArgs.Cancel) {
                            betterListViewStateInfo.State = BetterListViewState.Normal;
                        }
                        else {
                            betterListViewStateInfo.State = BetterListViewState.ColumnResize;
                            if (columnResizeStateInfo.IsSmooth) {
                                columnResizeStateInfo.Column.SetWidth(val, BetterListViewColumnWidthChangeSource.ResizeMouse, raiseEvent: false);
                                if (this.View == BetterListViewView.Details) {
                                    foreach (BetterListViewItem current in this) {
                                        ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                                    }
                                }
                                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                            }
                            else {
                                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                            }
                        }
                        columnResizeStateInfo.ColumnWidthNew = val;
                        betterListViewStateInfo.ColumnResizeStateInfo = columnResizeStateInfo;
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeSelection) {
                    Rectangle dragSelectionRectangle = this.DragSelectionRectangle;
                    if (dragSelectionRectangle.Width >= SystemInformation.DragSize.Width || dragSelectionRectangle.Height >= SystemInformation.DragSize.Height) {
                        betterListViewStateInfo.State = BetterListViewState.ItemSelection;
                        BetterListViewItemSelectionStateInfo itemSelectionStateInfo = betterListViewStateInfo.ItemSelectionStateInfo;
                        itemSelectionStateInfo.ScrollPosition = base.ScrollPosition;
                        itemSelectionStateInfo.SelectedItems = new BetterListViewReadOnlyItemSet(this.SelectedItemsSet);
                        betterListViewStateInfo.ItemSelectionStateInfo = itemSelectionStateInfo;
                        if (this.HitTestInfo.Locations == BetterListViewHitTestLocations.ContentArea && (Control.ModifierKeys & Keys.Shift) != Keys.Shift) {
                            empty = BetterListViewFocusInfo.Empty;
                        }
                        if (this.AllowAutoScroll) {
                            base.AutoScrollStart(BetterListViewAutoScrollMode.Outside);
                        }
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ItemSelection) {
                    this.UpdateDragSelection(betterListViewStateInfo.ItemSelectionStateInfo, ref selectionInfoNew);
                }
                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeDrag && (this.ItemReorderMode != 0 || this.AllowDrag)) {
                    Rectangle dragSelectionRectangle2 = this.DragSelectionRectangle;
                    if (dragSelectionRectangle2.Width >= SystemInformation.DragSize.Width || dragSelectionRectangle2.Height >= SystemInformation.DragSize.Height) {
                        DataObject dataObject = new DataObject();
                        BetterListViewItemDragData data = new BetterListViewItemDragData(base.Name, base.DragSourceID, new BetterListViewItemCollection(this.SelectedItems));
                        dataObject.SetData(typeof(BetterListViewItemDragData), data);
                        BetterListViewBeforeDragEventArgs betterListViewBeforeDragEventArgs = new BetterListViewBeforeDragEventArgs(dataObject, 0, e.X, e.Y, this.AllowedDragEffects, DragDropEffects.None);
                        try {
                            base.BeginUpdate();
                            this.OnBeforeDrag(betterListViewBeforeDragEventArgs);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                        if (!betterListViewBeforeDragEventArgs.Cancel) {
                            betterListViewStateInfo.State = BetterListViewState.ItemDrag;
                            base.DoDragDrop(betterListViewBeforeDragEventArgs.Data, betterListViewBeforeDragEventArgs.AllowedEffect);
                            betterListViewStateInfo.State = BetterListViewState.Normal;
                            selectionInfoNew = this.SelectionInfo;
                            empty = this.FocusInfo;
                            foreach (BetterListViewItem current2 in this) {
                                ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            }
                            base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                        }
                    }
                }
                this.SetStateInfo(betterListViewStateInfo);
                this.SetFocusInfo(empty, BetterListViewSelectionChangeMode.MouseDragSelection);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.MouseDragSelection);
            }
            if (!base.IsUpdating) {
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
                this.ToolTipsMouseMove();
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e) {
            if (!base.IsUpdating) {
                ContextMenuStrip contextMenuStrip = null;
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                BetterListViewFocusInfo focusInfoNew = this.FocusInfo;
                BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
                BetterListViewSelectionChangeMode betterListViewSelectionChangeMode = BetterListViewSelectionChangeMode.Undefined;
                this.ResetCursor();
                if (betterListViewStateInfo.State == BetterListViewState.Normal) {
                    if (this.BoundsColumnHeaders.Contains(e.Location) && e.Button == MouseButtons.Right && this.ContextMenuStripColumns != null) {
                        contextMenuStrip = this.ContextMenuStripColumns;
                    }
                    else if (base.BoundsContent.Contains(e.Location) && e.Button == MouseButtons.Right && this.ContextMenuStrip != null) {
                        selectionInfoNew = new BetterListViewSelectionInfo(new BetterListViewGroup[0], new BetterListViewItem[0], selectionInfoNew.LastSelectedElement);
                        contextMenuStrip = this.ContextMenuStrip;
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ColumnSelection) {
                    if (this.HitTestInfo.ColumnHeader == betterListViewStateInfo.ColumnSelectionStateInfo.Column) {
                        BetterListViewColumnHeaderStyle style = this.HitTestInfo.ColumnHeader.GetStyle(this);
                        if (style == BetterListViewColumnHeaderStyle.Clickable || style == BetterListViewColumnHeaderStyle.Sortable || style == BetterListViewColumnHeaderStyle.Unsortable) {
                            this.DoDefaultAction(betterListViewStateInfo.ColumnSelectionStateInfo.Column);
                        }
                        else {
                            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                        }
                    }
                    else {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ColumnResize) {
                    try {
                        base.BeginUpdate();
                        BetterListViewColumnResizeStateInfo columnResizeStateInfo = betterListViewStateInfo.ColumnResizeStateInfo;
                        if (!columnResizeStateInfo.IsSmooth) {
                            columnResizeStateInfo.Column.SetWidth(columnResizeStateInfo.ColumnWidthNew, BetterListViewColumnWidthChangeSource.ResizeMouse, raiseEvent: false);
                            if (this.View == BetterListViewView.Details) {
                                foreach (BetterListViewItem current in this) {
                                    ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                                }
                                base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                            }
                            else {
                                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                            }
                        }
                        this.OnColumnWidthChanged(new BetterListViewColumnWidthChangedEventArgs(betterListViewStateInfo.ColumnResizeStateInfo.Column, BetterListViewColumnWidthChangeSource.ResizeMouse));
                    }
                    finally {
                        base.EndUpdate(suppressRefresh: true);
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.GroupBeforeSelection && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.Group) == BetterListViewHitTestLocations.Group && this.HitTestInfo.Group == this.hitTestInfoMouseDown.Group) {
                    betterListViewSelectionChangeMode = BetterListViewSelectionChangeMode.MouseButton;
                    if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.MouseSelect) == BetterListViewGroupHeaderBehavior.MouseSelect) {
                        BetterListViewSelectionOptions betterListViewSelectionOptions = BetterListViewSelectionOptions.ClearSearchQuery;
                        if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.MouseFocus) == BetterListViewGroupHeaderBehavior.MouseFocus) {
                            betterListViewSelectionOptions |= BetterListViewSelectionOptions.TakeFocus;
                        }
                        this.MakeSelection(this.HitTestInfo.Group, (Control.ModifierKeys & Keys.Control) == Keys.Control, (Control.ModifierKeys & Keys.Shift) == Keys.Shift, betterListViewSelectionOptions, BetterListViewSelectionChangeMode.MouseButton, ref selectionInfoNew, ref focusInfoNew);
                    }
                    else if ((this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.MouseFocus) == BetterListViewGroupHeaderBehavior.MouseFocus) {
                        focusInfoNew = new BetterListViewFocusInfo(this.HitTestInfo.Group);
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeCheckMouse && this.HitTestInfo.ItemDisplay == this.hitTestInfoMouseDown.ItemDisplay) {
                    BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfo = betterListViewStateInfo.ItemBeforeCheckStateInfo;
                    if (e.Button == MouseButtons.Left && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemCheckBox) == BetterListViewHitTestLocations.ItemCheckBox && itemBeforeCheckStateInfo.Items.Contains(this.HitTestInfo.ItemDisplay)) {
                        BetterListViewItem item = itemBeforeCheckStateInfo.Item;
                        try {
                            base.BeginUpdate();
                            if (item.Selected) {
                                this.CheckItems(item, this.SelectedItems, BetterListViewCheckStateChangeMode.Mouse);
                            }
                            else {
                                this.CheckItems(item, new BetterListViewItemCollection(new BetterListViewItem[1] { item }), BetterListViewCheckStateChangeMode.Mouse);
                            }
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                }
                if (betterListViewStateInfo.State == BetterListViewState.GroupBeforeSelection || betterListViewStateInfo.State == BetterListViewState.ItemBeforeSelection || betterListViewStateInfo.State == BetterListViewState.ItemSelection || betterListViewStateInfo.State == BetterListViewState.ItemBeforeDrag || betterListViewStateInfo.State == BetterListViewState.ItemBeforeLabelEdit) {
                    if (this.Activation == ItemActivation.OneClick && e.Button == MouseButtons.Left && e.Clicks == 1 && this.HitTestInfo.ItemSelection != null && e.Location.X == betterListViewStateInfo.ItemSelectionStateInfo.StartPoint.X && e.Location.Y == betterListViewStateInfo.ItemSelectionStateInfo.StartPoint.Y) {
                        BetterListViewItemActivateEventArgs eventArgs = new BetterListViewItemActivateEventArgs(this.HitTestInfo.ItemSelection, BetterListViewItemActivationSource.Mouse);
                        try {
                            base.BeginUpdate();
                            betterListViewStateInfo.State = BetterListViewState.Normal;
                            this.SetStateInfo(betterListViewStateInfo);
                            this.OnItemActivate(eventArgs);
                        }
                        finally {
                            base.EndUpdate(suppressRefresh: true);
                        }
                    }
                    if (((e.Button == MouseButtons.Left && betterListViewStateInfo.State != BetterListViewState.ItemSelection) || (e.Button == MouseButtons.Right && betterListViewStateInfo.State == BetterListViewState.ItemSelection)) && this.HitTestInfo.ItemSelection == this.hitTestInfoMouseDown.ItemSelection) {
                        Rectangle dragSelectionRectangle = this.DragSelectionRectangle;
                        BetterListViewItem elementCurrent = null;
                        int num = 0;
                        if (this.IsAnyItemSelectable) {
                            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                            for (int i = this.visibleRangeItemsSelection.IndexElementFirst; i <= this.visibleRangeItemsSelection.IndexElementLast; i++) {
                                BetterListViewItem betterListViewItem = this.layoutElementsItemsSelection[i];
                                Rectangle boundsOuterExtended = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                                boundsOuterExtended.Offset(offsetContentFromAbsolute);
                                if (dragSelectionRectangle.IntersectsWith(boundsOuterExtended)) {
                                    elementCurrent = betterListViewItem;
                                    num++;
                                }
                            }
                        }
                        if (num == 0 && this.HitTestInfo.Locations == BetterListViewHitTestLocations.ContentArea) {
                            this.SelectedItemsSet = new BetterListViewItem[0];
                        }
                        else if (num == 1 && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeader) != BetterListViewHitTestLocations.ColumnHeader && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemCheckBox) != BetterListViewHitTestLocations.ItemCheckBox) {
                            bool flag = (this.LabelEditActivation & BetterListViewLabelEditActivation.SingleClick) == BetterListViewLabelEditActivation.SingleClick;
                            bool flag2 = (this.LabelEditActivation & BetterListViewLabelEditActivation.ClickThrough) == BetterListViewLabelEditActivation.ClickThrough;
                            bool flag3 = false;
                            BetterListViewSubItem betterListViewSubItem = ((this.HitTestInfo.SubItem != null) ? this.HitTestInfo.SubItem : ((this.HitTestInfo.ItemSelection == null) ? null : this.HitTestInfo.ItemSelection.SubItems[0]));
                            bool flag4;
                            if (this.LabelEdit && betterListViewSubItem != null && e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None && selectionInfoNew.SelectedItems.Count == 1 && (flag2 || (!flag2 && !this.StateInfo.FocusedByMouseDown))) {
                                BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewSubItem.Item).LayoutBounds.Clone();
                                betterListViewItemBounds.Offset(this.OffsetContentFromAbsolute);
                                if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.SubItem) == BetterListViewHitTestLocations.SubItem) {
                                    if ((this.HitTestInfo.SubItem.Index == 0 && this.LabelEditModeItems != 0) || (this.HitTestInfo.SubItem.Index != 0 && this.LabelEditModeSubItems != 0)) {
                                        flag4 = betterListViewItemBounds.SubItemBounds[betterListViewSubItem.Index].BoundsCell.Contains(e.Location);
                                        BetterListViewItem[] array = new BetterListViewItem[selectionInfoNew.SelectedItems.Count];
                                        selectionInfoNew.SelectedItems.Keys.CopyTo(array, 0);
                                        flag4 &= betterListViewSubItem.Item == array[0];
                                    }
                                    else {
                                        flag4 = false;
                                    }
                                }
                                else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.Item) == BetterListViewHitTestLocations.Item) {
                                    if (this.LabelEditModeItems != 0) {
                                        flag4 = betterListViewItemBounds.SubItemBounds[0].BoundsCell.Contains(e.Location);
                                        BetterListViewItem[] array2 = new BetterListViewItem[selectionInfoNew.SelectedItems.Count];
                                        selectionInfoNew.SelectedItems.Keys.CopyTo(array2, 0);
                                        flag4 &= this.HitTestInfo.ItemSelection == array2[0];
                                    }
                                    else {
                                        flag4 = false;
                                    }
                                }
                                else {
                                    flag4 = false;
                                }
                            }
                            else {
                                flag4 = false;
                            }
                            if (flag4 && (flag || this.StateInfo.ItemSelectionStateInfo.SelectedItems.EqualsContent(new Set<BetterListViewItem>(selectionInfoNew.SelectedItems.Keys)))) {
                                this.BeginEdit(betterListViewSubItem, (this.LabelEditActivation & BetterListViewLabelEditActivation.Immediate) == BetterListViewLabelEditActivation.Immediate);
                                betterListViewStateInfo = this.StateInfo;
                                flag3 = true;
                            }
                            if (!flag3 && this.MultiSelect) {
                                BetterListViewSelectionOptions betterListViewSelectionOptions2 = BetterListViewSelectionOptions.ClearSearchQuery;
                                if (betterListViewStateInfo.State == BetterListViewState.ItemBeforeDrag || betterListViewStateInfo.State == BetterListViewState.ItemBeforeSelection) {
                                    betterListViewSelectionOptions2 |= BetterListViewSelectionOptions.TakeFocus;
                                }
                                betterListViewSelectionChangeMode = ((betterListViewStateInfo.State != BetterListViewState.ItemSelection) ? BetterListViewSelectionChangeMode.MouseButton : BetterListViewSelectionChangeMode.MouseDragSelection);
                                this.MakeSelection(elementCurrent, (Control.ModifierKeys & Keys.Control) == Keys.Control, (Control.ModifierKeys & Keys.Shift) == Keys.Shift, betterListViewSelectionOptions2, betterListViewSelectionChangeMode, ref selectionInfoNew, ref focusInfoNew);
                            }
                        }
                    }
                    if (e.Button == MouseButtons.Right) {
                        if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.Item) == BetterListViewHitTestLocations.Item) {
                            if (this.HitTestInfo.ItemDisplay.Selected) {
                                contextMenuStrip = this.ContextMenuStripItems ?? this.ContextMenuStrip;
                            }
                            else {
                                selectionInfoNew = new BetterListViewSelectionInfo(new BetterListViewGroup[0], new BetterListViewItem[0], selectionInfoNew.LastSelectedElement);
                                contextMenuStrip = this.ContextMenuStrip;
                            }
                        }
                        else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.Group) == BetterListViewHitTestLocations.Group && this.ContextMenuStripGroups != null) {
                            contextMenuStrip = this.ContextMenuStripGroups;
                        }
                        else if (base.BoundsContent.Contains(e.Location) && this.ContextMenuStrip != null) {
                            selectionInfoNew = new BetterListViewSelectionInfo(new BetterListViewGroup[0], new BetterListViewItem[0], selectionInfoNew.LastSelectedElement);
                            contextMenuStrip = this.ContextMenuStrip;
                        }
                    }
                    if (betterListViewStateInfo.State == BetterListViewState.ItemSelection) {
                        this.PerformDragSelectionEnd(betterListViewStateInfo.ItemSelectionStateInfo.SelectedItems);
                    }
                }
                if (betterListViewStateInfo.State != BetterListViewState.LabelEditInit && betterListViewStateInfo.State != BetterListViewState.LabelEdit) {
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                }
                this.SetStateInfo(betterListViewStateInfo);
                if (betterListViewSelectionChangeMode != BetterListViewSelectionChangeMode.Undefined) {
                    this.SetFocusInfo(focusInfoNew, betterListViewSelectionChangeMode);
                    this.SetSelectionInfo(selectionInfoNew, betterListViewSelectionChangeMode);
                }
                this.SetHitTestInfo(this.HitTest());
                this.RefreshView();
                contextMenuStrip?.Show(this, e.Location);
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.DoubleClick" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnDoubleClick(EventArgs e) {
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            Point point = base.PointToClient(Control.MousePosition);
            BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
            if (!base.IsUpdating && !this.ReadOnly && mouseEventArgs != null && mouseEventArgs.Button == MouseButtons.Left) {
                if (this.StateInfo.State == BetterListViewState.ColumnBeforeResize && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) == BetterListViewHitTestLocations.ColumnHeaderBorder && this.View == BetterListViewView.Details && this.Items.Count != 0 && Math.Abs(point.X - this.StateInfo.ColumnResizeStateInfo.StartPoint.X) <= SystemInformation.DoubleClickSize.Width && Math.Abs(point.Y - this.StateInfo.ColumnResizeStateInfo.StartPoint.Y) <= SystemInformation.DoubleClickSize.Height) {
                    this.AutoResizeColumn(this.HitTestInfo.ColumnHeader, BetterListViewColumnHeaderAutoResizeStyle.ColumnContent, 0, userCode: false);
                }
                if ((this.StateInfo.State == BetterListViewState.ItemBeforeDrag || this.StateInfo.State == BetterListViewState.ItemBeforeLabelEdit || this.StateInfo.State == BetterListViewState.ItemBeforeSelection || this.StateInfo.State == BetterListViewState.ItemSelection) && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemSelection) == BetterListViewHitTestLocations.ItemSelection && (this.Activation == ItemActivation.Standard || this.Activation == ItemActivation.TwoClick) && Math.Abs(point.X - this.StateInfo.ItemSelectionStateInfo.StartPoint.X) <= SystemInformation.DoubleClickSize.Width && Math.Abs(point.Y - this.StateInfo.ItemSelectionStateInfo.StartPoint.Y) <= SystemInformation.DoubleClickSize.Height) {
                    BetterListViewItemActivateEventArgs eventArgs = new BetterListViewItemActivateEventArgs(this.HitTestInfo.ItemSelection, BetterListViewItemActivationSource.Mouse);
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    this.SetStateInfo(betterListViewStateInfo);
                    this.OnItemActivate(eventArgs);
                }
                if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ContentArea) == BetterListViewHitTestLocations.ContentArea) {
                    betterListViewStateInfo.State = BetterListViewState.Normal;
                    this.SetStateInfo(betterListViewStateInfo);
                    base.OnDoubleClick(e);
                }
            }
            else {
                base.OnDoubleClick(e);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (!base.IsUpdating && !this.ReadOnly && Control.ModifierKeys == Keys.None && (float)e.Delta != 0f && (base.HScrollBarVisible || base.VScrollBarVisible)) {
                float num = (float)(-e.Delta) * this.MouseWheelScrollExtent / 120f;
                bool flag = this.LayoutItemsCurrent.OrientationVertical;
                if (Control.ModifierKeys == Keys.Shift || (flag && !base.VScrollBarVisible) || (!flag && !base.HScrollBarVisible)) {
                    flag = !flag;
                }
                if (flag) {
                    int scrollPositionVertical2 = base.ScrollPositionVertical;
                    int num3 = Math.Min(Math.Max(base.ScrollPositionVertical + (int)Math.Round((float)base.VScrollBar.SmallChange * num), base.VScrollBar.Minimum), base.VScrollBar.Maximum - base.VScrollBar.LargeChange + 1);
                    if (num3 != scrollPositionVertical2) {
                        base.ScrollPositionVertical = num3;
                    }
                }
                else {
                    int scrollPositionVertical = base.ScrollPositionHorizontal;
                    int num2 = Math.Min(Math.Max(base.ScrollPositionHorizontal + (int)Math.Round((float)base.HScrollBar.SmallChange * num), base.HScrollBar.Minimum), base.HScrollBar.Maximum - base.HScrollBar.LargeChange + 1);
                    if (num2 != scrollPositionVertical) {
                        base.ScrollPositionHorizontal = num2;
                    }
                }
            }
            base.OnMouseWheel(e);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.MouseCaptureChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnMouseCaptureChanged(EventArgs e) {
            if (this.StateInfo.State == BetterListViewState.ItemSelection) {
                this.PerformDragSelectionEnd(this.StateInfo.ItemSelectionStateInfo.SelectedItems);
            }
            if (this.StateInfo.State != BetterListViewState.ItemDrag && this.StateInfo.State != BetterListViewState.LabelEditInit && this.StateInfo.State != BetterListViewState.LabelEdit) {
                BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
                betterListViewStateInfo.State = BetterListViewState.Normal;
                this.SetStateInfo(betterListViewStateInfo);
            }
            base.OnMouseCaptureChanged(e);
        }

        private void SetCursor(BetterListViewCursorType cursorType) {
            switch (cursorType) {
                case BetterListViewCursorType.ColumnReorder:
                    base.Cursor = Cursors.SizeAll;
                    break;
                case BetterListViewCursorType.ColumnResize:
                    base.Cursor = Cursors.VSplit;
                    break;
                case BetterListViewCursorType.Default:
                    base.Cursor = this.cursor;
                    break;
                case BetterListViewCursorType.ItemActivation:
                    NativeMethods.SetCursor(NativeMethods.LoadCursor(0, 32649));
                    break;
            }
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetBackgroundImageAlignment() {
            this.BackgroundImageAlignment = System.Drawing.ContentAlignment.BottomRight;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetCheckBoxes() {
            this.CheckBoxes = BetterListViewCheckBoxes.Hide;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetColorColumnResizeLine() {
            this.ColorColumnResizeLine = BetterListView.DefaultColorColumnResizeLine;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetColorGridLines() {
            this.ColorGridLines = BetterListView.DefaultColorGridLines;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetColorInsertionMark() {
            this.ColorInsertionMark = BetterListViewInsertionMark.Empty.Color;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetColorSortedColumn() {
            this.ColorSortedColumn = BetterListView.DefaultColorSortedColumn;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetFontColumns() {
            this.fontColumns = null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetFontGroups() {
            this.fontGroups = null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetFontItems() {
            FieldInfo field = typeof(BetterListViewBase).GetField("fontItems", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(this, null);
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetForeColorColumns() {
            this.foreColorColumns = Color.Empty;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetForeColorGroups() {
            this.foreColorGroups = Color.Empty;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetGridLines() {
            this.GridLines = BetterListViewGridLines.Vertical;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetHScrollBarDisplayMode() {
            this.HScrollBarDisplayMode = BetterListViewScrollBarDisplayMode.ShowIfNeeded;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetSortedColumnsRowsHighlight() {
            this.SortedColumnsRowsHighlight = BetterListViewSortedColumnsRowsHighlight.ShowMultiColumnOnly;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetTileSize() {
            this.TileSize = new Size(216, 48);
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetVScrollBarDisplayMode() {
            this.VScrollBarDisplayMode = BetterListViewScrollBarDisplayMode.ShowIfNeeded;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetView() {
            this.View = BetterListViewView.Details;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeBackgroundImageAlignment() {
            return this.BackgroundImageAlignment != System.Drawing.ContentAlignment.BottomRight;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeBackgroundImageOpacity() {
            return this.BackgroundImageOpacity != byte.MaxValue;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeCheckBoxes() {
            return this.CheckBoxes != BetterListViewCheckBoxes.Hide;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeColorColumnResizeLine() {
            return this.ColorColumnResizeLine != BetterListView.DefaultColorColumnResizeLine;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeColorGridLines() {
            return this.ColorGridLines != BetterListView.DefaultColorGridLines;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeColorInsertionMark() {
            return this.ColorInsertionMark != BetterListViewInsertionMark.Empty.Color;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeColorSortedColumn() {
            return this.ColorSortedColumn != BetterListView.DefaultColorSortedColumn;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeFontColumns() {
            return this.fontColumns != null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeFontGroups() {
            if (this.fontGroups != null) {
                return !object.Equals(this.fontGroups, BetterListViewGroup.DefaultFont);
            }
            return false;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeFontItems() {
            FieldInfo field = typeof(BetterListViewBase).GetField("fontItems", BindingFlags.Instance | BindingFlags.NonPublic);
            return field.GetValue(this) != null;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeForeColorColumns() {
            return !this.foreColorColumns.IsEmpty;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeForeColorGroups() {
            if (!this.foreColorGroups.IsEmpty) {
                return this.foreColorGroups != BetterListViewGroup.DefaultForeColor;
            }
            return false;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeGridLines() {
            return this.GridLines != BetterListViewGridLines.Vertical;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeHScrollBarDisplayMode() {
            return this.HScrollBarDisplayMode != BetterListViewScrollBarDisplayMode.ShowIfNeeded;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeSortedColumnsRowsHighlight() {
            return this.SortedColumnsRowsHighlight != BetterListViewSortedColumnsRowsHighlight.ShowMultiColumnOnly;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeTileSize() {
            Size tileSize = this.TileSize;
            if (tileSize.Width == 216) {
                return tileSize.Height != 48;
            }
            return true;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeVScrollBarDisplayMode() {
            return this.VScrollBarDisplayMode != BetterListViewScrollBarDisplayMode.ShowIfNeeded;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeView() {
            return this.View != BetterListViewView.Details;
        }

        /// <summary>
        ///   Reset value of the AllowDrop property.
        /// </summary>
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        internal void ResetAllowDrop() {
            this.AllowDrop = false;
        }

        /// <summary>
        ///   Check whether to serialize AllowDrop property.
        /// </summary>
        /// <returns>AllowDrop property should be serialized.</returns>
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        internal bool ShouldSerializeAllowDrop() {
            return this.allowDrop;
        }

        /// <summary>
        ///   Used by AllowDrop and AllowItemReorder properties.
        /// </summary>
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void RefreshAllowDrop() {
            base.AllowDrop = this.allowDrop || this.ItemReorderMode != BetterListViewItemReorderMode.Disabled;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetActivation() {
            this.Activation = ItemActivation.Standard;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetAllowedDragEffects() {
            this.AllowedDragEffects = DragDropEffects.Scroll | DragDropEffects.Move;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetColumns() {
            this.Columns.Clear();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetGroupHeaderBehavior() {
            this.GroupHeaderBehavior = BetterListViewGroupHeaderBehavior.All;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetGroups() {
            this.Groups.Clear();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetHideSelectionMode() {
            this.HideSelectionMode = BetterListViewHideSelectionMode.Hide;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetItemDropDisplayExternal() {
            this.ItemDropDisplayExternal = BetterListViewDragDropDisplay.Highlight;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetItemDropDisplayInternal() {
            this.ItemDropDisplayInternal = BetterListViewDragDropDisplay.InsertionMark;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetItems() {
            this.Items.Clear();
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetLabelEditActivation() {
            this.LabelEditActivation = BetterListViewLabelEditActivation.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetLabelEditModeItems() {
            this.LabelEditModeItems = BetterListViewLabelEditMode.None;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetLabelEditModeSubItems() {
            this.LabelEditModeSubItems = BetterListViewLabelEditMode.None;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetSearchSettings() {
            this.SearchSettings = BetterListView.DefaultSearchSettings;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetSubItemFocusBehavior() {
            this.SubItemFocusBehavior = BetterListViewSubItemFocusBehavior.Auto;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private void ResetToolTipOptions() {
            this.ToolTipOptions = BetterListViewToolTipOptions.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeActivation() {
            return this.Activation != ItemActivation.Standard;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeAllowedDragEffects() {
            return this.AllowedDragEffects != (DragDropEffects.Scroll | DragDropEffects.Move);
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeColumns() {
            return this.Columns.Count != 0;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeGroupHeaderBehavior() {
            return this.GroupHeaderBehavior != BetterListViewGroupHeaderBehavior.All;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeGroups() {
            return this.Groups.Count != 0;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeHideSelectionMode() {
            return this.HideSelectionMode != BetterListViewHideSelectionMode.Hide;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeItemDropDisplayExternal() {
            return this.ItemDropDisplayExternal != BetterListViewDragDropDisplay.Highlight;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeItemDropDisplayInternal() {
            return this.ItemDropDisplayInternal != BetterListViewDragDropDisplay.InsertionMark;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeItems() {
            return this.Items.Count != 0;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeLabelEditActivation() {
            return this.LabelEditActivation != BetterListViewLabelEditActivation.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeLabelEditModeItems() {
            return this.LabelEditModeItems != BetterListViewLabelEditMode.None;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeLabelEditModeSubItems() {
            return this.LabelEditModeSubItems != BetterListViewLabelEditMode.None;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeSearchSettings() {
            return this.SearchSettings != BetterListView.DefaultSearchSettings;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeSubItemFocusBehavior() {
            return this.SubItemFocusBehavior != BetterListViewSubItemFocusBehavior.Auto;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeToolTipOptions() {
            return this.ToolTipOptions != BetterListViewToolTipOptions.Default;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private new void ResetForeColor() {
            this.ForeColor = SystemColors.ControlText;
        }

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        private bool ShouldSerializeForeColor() {
            return this.ForeColor != SystemColors.ControlText;
        }

        /// <summary>
        ///   Search for item within the control.
        /// </summary>
        /// <param name="queryString">Search query string.</param>
        /// <returns>Found item.</returns>
        public BetterListViewItem FindItemWithText(string queryString) {
            BetterListViewItemCollection betterListViewItemCollection = this.FindItemsWithText(queryString, this.SearchSettings);
            if (betterListViewItemCollection.Count == 0) {
                return null;
            }
            return betterListViewItemCollection[0];
        }

        /// <summary>
        ///   Search for item within the control.
        /// </summary>
        /// <param name="queryString">Search query string.</param>
        /// <param name="searchSettings">Search options.</param>
        /// <returns>Found items.</returns>
        public BetterListViewItem FindItemWithText(string queryString, BetterListViewSearchSettings searchSettings) {
            BetterListViewItemCollection betterListViewItemCollection = this.FindItemsWithText(queryString, 0, searchSettings);
            if (betterListViewItemCollection.Count == 0) {
                return null;
            }
            return betterListViewItemCollection[0];
        }

        /// <summary>
        ///   Search for items within the control.
        /// </summary>
        /// <param name="queryString">Search query string.</param>
        /// <returns>Found items.</returns>
        public BetterListViewItemCollection FindItemsWithText(string queryString) {
            return this.FindItemsWithText(queryString, this.SearchSettings);
        }

        /// <summary>
        ///   Search for items within the control.
        /// </summary>
        /// <param name="queryString">Search query string.</param>
        /// <param name="searchSettings">Search options.</param>
        /// <returns>Found items.</returns>
        public BetterListViewItemCollection FindItemsWithText(string queryString, BetterListViewSearchSettings searchSettings) {
            return this.FindItemsWithText(queryString, 0, searchSettings);
        }

        /// <summary>
        ///   Search for items within the control.
        /// </summary>
        /// <param name="queryString">Search query string.</param>
        /// <param name="startIndex">Index of item where the searching starts.</param>
        /// <param name="searchSettings">Search options.</param>
        /// <returns>Found items.</returns>
        public BetterListViewItemCollection FindItemsWithText(string queryString, int startIndex, BetterListViewSearchSettings searchSettings) {
            Checks.CheckNotNull(queryString, "queryString");
            if (this.Items.Count != 0) {
                Checks.CheckBounds(startIndex, 0, this.Items.Count - 1, "startIndex");
            }
            else {
                Checks.CheckEqual(startIndex, 0, "startIndex", "0");
            }
            while (startIndex < this.Items.Count && !((IBetterListViewStateElement)this.Items[startIndex]).IsActive) {
                startIndex++;
            }
            BetterListViewItemCollection result = ((startIndex >= this.Items.Count) ? new BetterListViewItemCollection() : this.FindItemsWithText(queryString, startIndex, searchSettings, 0));
            BetterListViewItemSearchEventArgs eventArgs = new BetterListViewItemSearchEventArgs(result, queryString, selectionChanged: false, BetterListViewItemSearchSource.User);
            this.OnItemSearch(eventArgs);
            return result;
        }

        private static bool CompareStrings(string queryString, string itemString, BetterListViewSearchMode mode, BetterListViewSearchOptions options, ref bool prefixPrefered) {
            if ((options & BetterListViewSearchOptions.PrefixPreference) == BetterListViewSearchOptions.PrefixPreference && itemString.StartsWith(queryString)) {
                prefixPrefered = true;
                return true;
            }
            prefixPrefered = false;
            List<string> list = new List<string>();
            if ((options & BetterListViewSearchOptions.WordSearch) == BetterListViewSearchOptions.WordSearch) {
                bool flag = (options & BetterListViewSearchOptions.FirstWordOnly) == BetterListViewSearchOptions.FirstWordOnly;
                string text = string.Empty;
                for (int i = 0; i < itemString.Length; i++) {
                    char c = itemString[i];
                    if (char.IsWhiteSpace(c) && text.Length != 0) {
                        list.Add(text);
                        text = string.Empty;
                        if (flag) {
                            break;
                        }
                    }
                    else {
                        text += c;
                    }
                }
                if (text.Length != 0) {
                    list.Add(text);
                }
            }
            else {
                list.Add(itemString);
            }
            if (mode == BetterListViewSearchMode.Prefix || mode == BetterListViewSearchMode.PrefixOrSubstring) {
                foreach (string item in list) {
                    if (item.StartsWith(queryString)) {
                        return true;
                    }
                }
                if (mode == BetterListViewSearchMode.Prefix) {
                    return false;
                }
            }
            foreach (string item2 in list) {
                if (item2.Contains(queryString)) {
                    return true;
                }
            }
            return false;
        }

        private static void AddSearchResult(BetterListViewItem item, bool prefixPrefered, bool prefixPreference, int searchLimit, ref BetterListViewItemCollection itemsFoundPrefered, ref BetterListViewItemCollection itemsFound) {
            if (prefixPrefered) {
                itemsFoundPrefered.Add(item);
                if (prefixPreference) {
                    int num = itemsFoundPrefered.Count + itemsFound.Count - searchLimit;
                    if (num > 0 && num <= itemsFound.Count) {
                        itemsFound.RemoveRange(itemsFound.Count - num, num);
                    }
                }
            }
            else if (!prefixPreference || searchLimit == 0 || itemsFoundPrefered.Count + itemsFound.Count < searchLimit) {
                itemsFound.Add(item);
            }
        }

        private bool PerformSearch(char letter) {
            if (!this.IsAnyItemVisible) {
                return false;
            }
            if (DateTime.Now.Subtract(this.searchLastTyping).TotalMilliseconds > (double)this.SearchTimeoutDelay) {
                this.ClearSearch();
            }
            BetterListViewFocusInfo betterListViewFocusInfo = this.FocusInfo;
            BetterListViewSelectionInfo betterListViewSelectionInfo = this.SelectionInfo;
            bool flag;
            if (this.searchString.Length == 0 && char.IsWhiteSpace(letter)) {
                flag = false;
            }
            else {
                this.searchString += letter;
                char c = this.searchString[0];
                bool flag2 = true;
                for (int i = 1; i < this.searchString.Length; i++) {
                    if (this.searchString[i] != c) {
                        flag2 = false;
                        break;
                    }
                }
                BetterListViewItem focusedItem = this.FocusedItem;
                int num = ((focusedItem != null && ((IBetterListViewStateElement)focusedItem).IsActive) ? ((IBetterListViewLayoutElementDisplayable)focusedItem).LayoutIndexDisplay : 0);
                BetterListViewItemCollection betterListViewItemCollection = ((!flag2) ? this.FindItemsWithText(searchSettings: new BetterListViewSearchSettings(this.searchSettings.Mode, this.searchSettings.Options | BetterListViewSearchOptions.SelectableItemsOnly, this.searchSettings.SubItemIndices), queryString: this.searchString, startIndex: num, searchLimit: 1) : this.FindItemsWithText(searchSettings: new BetterListViewSearchSettings(BetterListViewSearchMode.Prefix, this.searchSettings.Options | BetterListViewSearchOptions.SelectableItemsOnly, this.searchSettings.SubItemIndices), queryString: this.searchString[0].ToString(), startIndex: (num + 1) % this.layoutElementsItemsDisplay.Count, searchLimit: 1));
                if (betterListViewItemCollection.Count != 0) {
                    BetterListViewItem betterListViewItem = betterListViewItemCollection[0];
                    this.MakeSelection(betterListViewItem, BetterListViewSelectionOptions.TakeFocus | BetterListViewSelectionOptions.TakeSelection | BetterListViewSelectionOptions.TakeAnchor, BetterListViewSelectionChangeMode.Keyboard, ref betterListViewSelectionInfo, ref betterListViewFocusInfo);
                    flag = betterListViewSelectionInfo != this.SelectionInfo || betterListViewFocusInfo != this.FocusInfo;
                    if (flag) {
                        this.EnsureVisible(betterListViewItem);
                    }
                }
                else {
                    flag = false;
                }
                BetterListViewItemSearchEventArgs eventArgs = new BetterListViewItemSearchEventArgs(betterListViewItemCollection, this.searchString, flag, BetterListViewItemSearchSource.Keyboard);
                this.OnItemSearch(eventArgs);
            }
            this.searchLastTyping = DateTime.Now;
            if (flag) {
                this.SetFocusInfo(betterListViewFocusInfo, BetterListViewSelectionChangeMode.Keyboard);
                this.SetSelectionInfo(betterListViewSelectionInfo, BetterListViewSelectionChangeMode.Keyboard);
            }
            return flag;
        }

        private void ClearSearch() {
            this.searchString = string.Empty;
        }

        private BetterListViewItemCollection FindItemsWithText(string queryString, int startIndex, BetterListViewSearchSettings searchSettings, int searchLimit) {
            bool flag = (searchSettings.Options & BetterListViewSearchOptions.SelectableItemsOnly) == BetterListViewSearchOptions.SelectableItemsOnly;
            if ((flag && !this.IsAnyItemSelectable) || (!flag && !this.IsAnyItemVisible) || queryString.Length == 0) {
                return new BetterListViewItemCollection();
            }
            if ((searchSettings.Options & BetterListViewSearchOptions.CaseSensitive) != BetterListViewSearchOptions.CaseSensitive) {
                queryString = queryString.ToUpper();
            }
            BetterListViewItemCollection itemsFoundPrefered = new BetterListViewItemCollection();
            BetterListViewItemCollection itemsFound = new BetterListViewItemCollection();
            bool prefixPrefered = false;
            bool flag2 = (searchSettings.Options & BetterListViewSearchOptions.CaseSensitive) == BetterListViewSearchOptions.CaseSensitive;
            bool flag3 = (searchSettings.Options & BetterListViewSearchOptions.PrefixPreference) == BetterListViewSearchOptions.PrefixPreference;
            ReadOnlyCollection<BetterListViewItem> readOnlyCollection = (flag ? this.layoutElementsItemsSelection : this.layoutElementsItemsDisplay);
            for (int j = startIndex; j < readOnlyCollection.Count; j++) {
                BetterListViewItem betterListViewItem = readOnlyCollection[j];
                for (int k = 0; k < betterListViewItem.SubItems.Count; k++) {
                    if (searchLimit != 0) {
                        if (flag3) {
                            if (itemsFoundPrefered.Count == searchLimit) {
                                break;
                            }
                        }
                        else if (itemsFound.Count == searchLimit) {
                            break;
                        }
                    }
                    if (searchSettings.SubItemIndices.Count != 0 && !searchSettings.SubItemIndices.Contains(k)) {
                        continue;
                    }
                    string text = betterListViewItem.SubItems[k].Text;
                    if (!string.IsNullOrEmpty(text)) {
                        if (!flag2) {
                            text = text.ToUpper();
                        }
                        if (BetterListView.CompareStrings(queryString, text, searchSettings.Mode, searchSettings.Options, ref prefixPrefered)) {
                            BetterListView.AddSearchResult(betterListViewItem, prefixPrefered, flag3, searchLimit, ref itemsFoundPrefered, ref itemsFound);
                        }
                    }
                }
            }
            for (int i = 0; i < startIndex; i++) {
                BetterListViewItem betterListViewItem2 = readOnlyCollection[i];
                for (int l = 0; l < betterListViewItem2.SubItems.Count; l++) {
                    if (searchLimit != 0) {
                        if (flag3) {
                            if (itemsFoundPrefered.Count == searchLimit) {
                                break;
                            }
                        }
                        else if (itemsFound.Count == searchLimit) {
                            break;
                        }
                    }
                    if (searchSettings.SubItemIndices.Count != 0 && !searchSettings.SubItemIndices.Contains(l)) {
                        continue;
                    }
                    string text2 = betterListViewItem2.SubItems[l].Text;
                    if (!string.IsNullOrEmpty(text2)) {
                        if (!flag2) {
                            text2 = text2.ToUpper();
                        }
                        if (BetterListView.CompareStrings(queryString, text2, searchSettings.Mode, searchSettings.Options, ref prefixPrefered)) {
                            BetterListView.AddSearchResult(betterListViewItem2, prefixPrefered, flag3, searchLimit, ref itemsFoundPrefered, ref itemsFound);
                        }
                    }
                }
            }
            itemsFoundPrefered.AddRange(itemsFound);
            if (itemsFoundPrefered.Count == 0 && (searchSettings.Options & BetterListViewSearchOptions.PlaySound) == BetterListViewSearchOptions.PlaySound) {
                SystemSounds.Beep.Play();
            }
            return itemsFoundPrefered;
        }

        /// <summary>
        ///   Scroll control to make the item with the specified index visible.
        /// </summary>
        /// <param name="index">Index of the item to scroll to.</param>
        public void EnsureVisible(int index) {
            this.EnsureVisible(new int[1] { index });
        }

        /// <summary>
        ///   Scroll control to make most of the items with the specified indices visible with the first item always visible.
        /// </summary>
        /// <param name="indices">Indices of items to scroll at.</param>
        public void EnsureVisible(IEnumerable<int> indices) {
            Checks.CheckNotNull(indices, "indices");
            Rectangle boundsContent = base.BoundsContent;
            if (boundsContent.Width <= 0 || boundsContent.Height <= 0) {
                return;
            }
            BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection();
            int val = this.Items.Count - 1;
            foreach (int index in indices) {
                betterListViewItemCollection.Add(this.Items[Math.Min(Math.Max(index, 0), val)]);
            }
            this.EnsureVisible(betterListViewItemCollection);
        }

        /// <summary>
        ///   Scroll control to make most of the items with the specified indices visible with the first item always visible.
        /// </summary>
        /// <param name="items">Items to scroll at.</param>
        public void EnsureVisible(ICollection<BetterListViewItem> items) {
            BetterListViewEnsureVisibleData betterListViewEnsureVisibleData = new BetterListViewEnsureVisibleData(items);
            if (base.IsUpdating) {
                this.PostponeCall(BetterListViewPostponedCallType.EnsureVisible, new EnsureVisibleDelegate(EnsureVisible), new object[2] { betterListViewEnsureVisibleData, true });
            }
            else {
                this.EnsureVisible(betterListViewEnsureVisibleData, suppressRefresh: false);
            }
        }

        /// <summary>
        ///   Scroll control to make most of the items with the specified indices visible with the first item always visible.
        /// </summary>
        /// <param name="element">Element to scroll at.</param>
        public void EnsureVisible(BetterListViewElementBase element) {
            BetterListViewEnsureVisibleData betterListViewEnsureVisibleData = new BetterListViewEnsureVisibleData(element);
            if (base.IsUpdating) {
                this.PostponeCall(BetterListViewPostponedCallType.EnsureVisible, new EnsureVisibleDelegate(EnsureVisible), new object[2] { betterListViewEnsureVisibleData, true });
            }
            else {
                this.EnsureVisible(betterListViewEnsureVisibleData, suppressRefresh: false);
            }
        }

        /// <summary>
        ///   Scroll control to make most of the specified area visible.
        /// </summary>
        /// <param name="area">Area to scroll at.</param>
        public void EnsureVisible(Rectangle area) {
            BetterListViewEnsureVisibleData betterListViewEnsureVisibleData = new BetterListViewEnsureVisibleData(area);
            if (base.IsUpdating) {
                this.PostponeCall(BetterListViewPostponedCallType.EnsureVisible, new EnsureVisibleDelegate(EnsureVisible), new object[2] { betterListViewEnsureVisibleData, true });
            }
            else {
                this.EnsureVisible(betterListViewEnsureVisibleData, suppressRefresh: false);
            }
        }

        /// <summary>
        ///   Find the next item from the specified item item, searching in the specified direction.
        /// </summary>
        /// <param name="item">Item to search from.</param>
        /// <param name="searchDirection">Item search direction.</param>
        /// <returns>Item closest to coordinates of this item in the specified direction.</returns>
        public BetterListViewItem FindNearestItem(BetterListViewItem item, SearchDirectionHint searchDirection) {
            Checks.CheckNotNull(item, "item");
            Checks.CheckNotNull(item.ListView, "item.ListView", "Item is not contained in any control.");
            if (item.ListView != this) {
                return item.ListView.FindNearestItem(item, searchDirection);
            }
            if (!((IBetterListViewStateElement)item).IsActive) {
                return null;
            }
            bool orientationVertical = this.LayoutItemsCurrent.OrientationVertical;
            bool flag = !orientationVertical;
            BetterListViewGroup visibleGroup = this.GetVisibleGroup(item, allowNull: false);
            if (!flag || (searchDirection != SearchDirectionHint.Left && searchDirection != SearchDirectionHint.Right)) {
                if ((orientationVertical && searchDirection == SearchDirectionHint.Up) || (flag && searchDirection == SearchDirectionHint.Left)) {
                    if (((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay - visibleGroup.LayoutMeasurementItems.ElementsPerRow < visibleGroup.LayoutIndexItemDisplayFirst) {
                        return null;
                    }
                }
                else if ((orientationVertical && searchDirection == SearchDirectionHint.Down) || (flag && searchDirection == SearchDirectionHint.Right)) {
                    if (((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay + visibleGroup.LayoutMeasurementItems.ElementsPerRow > visibleGroup.LayoutIndexItemDisplayLast) {
                        return null;
                    }
                }
                else if ((orientationVertical && searchDirection == SearchDirectionHint.Left) || (flag && searchDirection == SearchDirectionHint.Up)) {
                    if (((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay % visibleGroup.LayoutMeasurementItems.ElementsPerRow == 0) {
                        return null;
                    }
                }
                else if (((orientationVertical && searchDirection == SearchDirectionHint.Right) || (flag && searchDirection == SearchDirectionHint.Down)) && (((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay + 1) % visibleGroup.LayoutMeasurementItems.ElementsPerRow == 0) {
                    return null;
                }
            }
            //BetterListViewFocusInfo adjacentFocusInfo = this.GetAdjacentFocusInfo(targetElement: searchDirection switch {
            //	SearchDirectionHint.Up => BetterListViewTargetElement.Up,
            //	SearchDirectionHint.Down => BetterListViewTargetElement.Down,
            //	SearchDirectionHint.Left => BetterListViewTargetElement.Left,
            //	SearchDirectionHint.Right => BetterListViewTargetElement.Right,
            //	_ => throw new ApplicationException($"Unknown search direction: '{searchDirection}'."),
            //}, focusInfoCurrent: new BetterListViewFocusInfo(item, 0));
            BetterListViewTargetElement targetElement;
            switch (searchDirection) {
                case SearchDirectionHint.Up: targetElement = BetterListViewTargetElement.Up; break;
                case SearchDirectionHint.Down: targetElement = BetterListViewTargetElement.Down; break;
                case SearchDirectionHint.Left: targetElement = BetterListViewTargetElement.Left; break;
                case SearchDirectionHint.Right: targetElement = BetterListViewTargetElement.Right; break;
                default:
                    throw new ApplicationException($"Unknown search direction: '{searchDirection}'.");
                    break;
            }
            var focusInfoCurrent = new BetterListViewFocusInfo(item, 0);
            BetterListViewFocusInfo adjacentFocusInfo = this.GetAdjacentFocusInfo(focusInfoCurrent, targetElement);


            if (adjacentFocusInfo.Element is BetterListViewItem && item != adjacentFocusInfo.Element) {
                return (BetterListViewItem)adjacentFocusInfo.Element;
            }
            return null;
        }

        internal void MakeSelection(BetterListViewElementBase elementCurrent, BetterListViewSelectionOptions selectionOptions, BetterListViewSelectionChangeMode selectionChangeMode) {
            Checks.CheckNotNull(elementCurrent, "elementCurrent");
            BetterListViewFocusInfo focusInfoNew = this.FocusInfo;
            BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
            this.MakeSelection(elementCurrent, selectionOptions, selectionChangeMode, ref selectionInfoNew, ref focusInfoNew);
            this.SetFocusInfo(focusInfoNew, selectionChangeMode);
            this.SetSelectionInfo(selectionInfoNew, selectionChangeMode);
            this.RefreshView();
        }

        private static void IsSelected(BetterListViewSelectionInfo selectionInfo, BetterListViewElementBase element, out bool isSelected, out bool isAnythingSelected) {
            if (element == null) {
                isSelected = false;
                isAnythingSelected = selectionInfo.SelectedItems.Count != 0;
                return;
            }
            if (element is BetterListViewItem betterListViewItem) {
                isSelected = selectionInfo.SelectedItems.Keys.Contains(betterListViewItem);
                isAnythingSelected = selectionInfo.SelectedItems.Count != 0;
                return;
            }
            if (element is BetterListViewGroup betterListViewGroup) {
                isSelected = selectionInfo.SelectedGroups.Contains(betterListViewGroup);
                isAnythingSelected = selectionInfo.SelectedGroups.Count != 0;
                return;
            }
            throw new ApplicationException("Unexpected element type: '" + element.GetType().FullName + "'.");
        }

        private void MakeSelection(BetterListViewElementBase elementCurrent, bool modifierControl, bool modifierShift, BetterListViewSelectionOptions selectionOptions, BetterListViewSelectionChangeMode selectionChangeMode, ref BetterListViewSelectionInfo selectionInfo, ref BetterListViewFocusInfo focusInfo) {
            if (!modifierControl && !modifierShift) {
                selectionOptions |= BetterListViewSelectionOptions.TakeSelection | BetterListViewSelectionOptions.TakeAnchor;
            }
            else if (modifierShift) {
                selectionOptions |= BetterListViewSelectionOptions.ExtendSelection;
                if (modifierControl) {
                    BetterListView.IsSelected(selectionInfo, selectionInfo.LastSelectedElement, out var isSelected, out var isAnythingSelected);
                    selectionOptions = (isSelected ? (selectionOptions | BetterListViewSelectionOptions.AddSelection) : ((this.MultiSelect || !isAnythingSelected) ? (selectionOptions | BetterListViewSelectionOptions.RemoveSelection) : (selectionOptions | BetterListViewSelectionOptions.TakeSelection)));
                }
                else {
                    selectionOptions |= BetterListViewSelectionOptions.TakeSelection;
                }
            }
            else if (modifierControl) {
                BetterListView.IsSelected(selectionInfo, elementCurrent, out var isSelected2, out var isAnythingSelected2);
                if (isSelected2) {
                    selectionOptions = BetterListViewSelectionOptions.RemoveSelection;
                    _ = selectionInfo.SelectedItems.Count;
                }
                else if (!this.MultiSelect && isAnythingSelected2) {
                    selectionOptions = BetterListViewSelectionOptions.TakeSelection;
                }
                else {
                    selectionOptions = BetterListViewSelectionOptions.AddSelection;
                    _ = selectionInfo.SelectedGroups.Count;
                }
                selectionOptions |= BetterListViewSelectionOptions.TakeAnchor;
            }
            this.MakeSelection(elementCurrent, selectionOptions, selectionChangeMode, ref selectionInfo, ref focusInfo);
        }

        private void MakeSelection(BetterListViewElementBase elementCurrent, BetterListViewSelectionOptions selectionOptions, BetterListViewSelectionChangeMode selectionChangeMode, ref BetterListViewSelectionInfo selectionInfo, ref BetterListViewFocusInfo focusInfo) {
            bool flag = (selectionOptions & BetterListViewSelectionOptions.TakeFocus) == BetterListViewSelectionOptions.TakeFocus;
            bool flag2 = (selectionOptions & BetterListViewSelectionOptions.TakeSelection) == BetterListViewSelectionOptions.TakeSelection;
            bool flag3 = (selectionOptions & BetterListViewSelectionOptions.ExtendSelection) == BetterListViewSelectionOptions.ExtendSelection;
            bool flag4 = (selectionOptions & BetterListViewSelectionOptions.AddSelection) == BetterListViewSelectionOptions.AddSelection;
            bool flag5 = (selectionOptions & BetterListViewSelectionOptions.RemoveSelection) == BetterListViewSelectionOptions.RemoveSelection;
            bool flag6 = (selectionOptions & BetterListViewSelectionOptions.TakeAnchor) == BetterListViewSelectionOptions.TakeAnchor;
            bool flag7 = (selectionOptions & BetterListViewSelectionOptions.ClearSearchQuery) == BetterListViewSelectionOptions.ClearSearchQuery;
            Checks.CheckFalse(flag4 && flag5, "optionsAddSelection & optionsRemoveSelection");
            Checks.CheckFalse(flag4 && flag2, "optionsAddSelection & optionsTakeSelection");
            Checks.CheckFalse(flag5 && flag2, "optionsRemoveSelection & optionsTakeSelection");
            BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet();
            BetterListViewElementBase betterListViewElementBase = selectionInfo.LastSelectedElement;
            if (betterListViewElementBase == null) {
                if (focusInfo.IsEmpty) {
                    betterListViewElementBase = ((this.ShowGroups && this.layoutElementsGroups.Count != 0) ? this.layoutElementsGroups[0] : null);
                    if (betterListViewElementBase == null) {
                        betterListViewElementBase = this.layoutElementsItemsSelection[0];
                    }
                }
                else {
                    betterListViewElementBase = focusInfo.Element;
                }
            }
            if (flag3) {
                if (flag2) {
                    if (this.MultiSelect) {
                        ReadOnlyCollection<BetterListViewItem> selectionRange = this.GetSelectionRange(betterListViewElementBase, elementCurrent);
                        if (flag4) {
                            betterListViewItemSet.AddRange(selectionRange);
                        }
                        else if (!flag5) {
                            betterListViewItemSet.AddRange(selectionRange);
                        }
                    }
                    else if (!flag5 && elementCurrent is BetterListViewItem betterListViewItem) {
                        betterListViewItemSet.Add(betterListViewItem);
                    }
                }
                else if (this.MultiSelect) {
                    ReadOnlyCollection<BetterListViewItem> selectionRange2 = this.GetSelectionRange(betterListViewElementBase, elementCurrent);
                    if (flag4) {
                        betterListViewItemSet.AddRange(selectionInfo.SelectedItems.Keys);
                        betterListViewItemSet.AddRange(selectionRange2);
                    }
                    else if (flag5) {
                        betterListViewItemSet.AddRange(selectionInfo.SelectedItems.Keys);
                        betterListViewItemSet.RemoveRange(selectionRange2);
                    }
                    else {
                        betterListViewItemSet.AddRange(selectionRange2);
                    }
                }
                else if (!flag5 && elementCurrent is BetterListViewItem betterListViewItem2) {
                    betterListViewItemSet.Add(betterListViewItem2);
                }
            }
            else if (flag2) {
                if (elementCurrent is BetterListViewItem betterListViewItem3) {
                    betterListViewItemSet.Add(betterListViewItem3);
                }
                if (this.MultiSelect && elementCurrent is BetterListViewGroup betterListViewGroup) {
                    betterListViewItemSet.AddRange(betterListViewGroup.GetItems(this));
                }
            }
            else if (flag4) {
                betterListViewItemSet.AddRange(selectionInfo.SelectedItems.Keys);
                if (elementCurrent is BetterListViewItem betterListViewItem4) {
                    betterListViewItemSet.Add(betterListViewItem4);
                }
                if (this.MultiSelect && elementCurrent is BetterListViewGroup betterListViewGroup2) {
                    betterListViewItemSet.AddRange(betterListViewGroup2.GetItems(this));
                }
            }
            else if (flag5) {
                betterListViewItemSet.AddRange(selectionInfo.SelectedItems.Keys);
                if (elementCurrent is BetterListViewItem betterListViewItem5) {
                    betterListViewItemSet.Remove(betterListViewItem5);
                }
                if (this.MultiSelect && elementCurrent is BetterListViewGroup betterListViewGroup3) {
                    betterListViewItemSet.RemoveRange(betterListViewGroup3.GetItems(this));
                }
            }
            if (flag6 && elementCurrent is BetterListViewItem) {
                betterListViewElementBase = elementCurrent;
            }
            if (flag && elementCurrent != focusInfo.Element) {
                if (elementCurrent is BetterListViewItem) {
                    focusInfo = new BetterListViewFocusInfo((BetterListViewItem)elementCurrent, 0);
                }
                else {
                    focusInfo = new BetterListViewFocusInfo((BetterListViewGroup)elementCurrent);
                }
                if (selectionChangeMode == BetterListViewSelectionChangeMode.Keyboard) {
                    _ = this.AllowDisplayFocusRectangle;
                    this.AllowDisplayFocusRectangle = true;
                }
            }
            if (flag7) {
                this.ClearSearch();
            }
            if (!betterListViewItemSet.EqualsContent(new Set<BetterListViewItem>(selectionInfo.SelectedItems.Keys)) && selectionChangeMode == BetterListViewSelectionChangeMode.Keyboard) {
                _ = this.AllowDisplayFocusRectangle;
                this.AllowDisplayFocusRectangle = true;
            }
            selectionInfo = new BetterListViewSelectionInfo(this.GetSelectedGroups(betterListViewItemSet), betterListViewItemSet, betterListViewElementBase);
        }

        private void EnsureVisible(BetterListViewEnsureVisibleData ensureVisibleData, bool suppressRefresh) {
            Rectangle boundsContent = base.BoundsContent;
            if (boundsContent.Width <= 0 || boundsContent.Height <= 0) {
                return;
            }
            try {
                base.BeginUpdate();
                Rectangle rectangle = Rectangle.Empty;
                if (ensureVisibleData.Element != null) {
                    BetterListViewItem betterListViewItem = ensureVisibleData.Element as BetterListViewItem;
                    BetterListViewGroup betterListViewGroup = ensureVisibleData.Element as BetterListViewGroup;
                    BetterListViewSubItem betterListViewSubItem = ensureVisibleData.Element as BetterListViewSubItem;
                    if (betterListViewItem == null && betterListViewGroup == null && betterListViewSubItem == null) {
                        throw new ArgumentException("Unsupported element type: '" + ensureVisibleData.Element.GetType().FullName + "'.");
                    }
                    if ((betterListViewItem != null && betterListViewItem.ListView != this) || (betterListViewGroup != null && betterListViewGroup.ListView != this) || (betterListViewSubItem != null && betterListViewSubItem.ListView != this)) {
                        return;
                    }
                    if (betterListViewItem != null) {
                        Rectangle boundsOuterExtended = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                        BetterListViewItem betterListViewItem2 = betterListViewItem;
                        while (betterListViewItem2.ParentItem != null) {
                            betterListViewItem2 = betterListViewItem2.ParentItem;
                        }
                        if (betterListViewItem == betterListViewItem2) {
                            rectangle = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                        }
                        else {
                            Rectangle boundsOuterExtended2 = ((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds.BoundsOuterExtended;
                            rectangle = Rectangle.Union(boundsOuterExtended, boundsOuterExtended2);
                        }
                    }
                    else if (betterListViewGroup != null) {
                        rectangle = ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds).BoundsSelection;
                    }
                    else {
                        BetterListViewItem item = betterListViewSubItem.Item;
                        if (item == null) {
                            return;
                        }
                        BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds;
                        if (betterListViewSubItem.Index >= betterListViewItemBounds.SubItemBounds.Count) {
                            return;
                        }
                        rectangle = betterListViewItemBounds.SubItemBounds[betterListViewSubItem.Index].BoundsOuter;
                    }
                    rectangle.Offset(this.OffsetContentFromAbsolute);
                }
                else if (ensureVisibleData.Items == null) {
                    rectangle = ensureVisibleData.Area;
                }
                else {
                    foreach (BetterListViewItem item2 in ensureVisibleData.Items) {
                        if (item2.ListView != this) {
                            return;
                        }
                    }
                    int num = int.MaxValue;
                    int num2 = int.MinValue;
                    foreach (BetterListViewItem item3 in ensureVisibleData.Items) {
                        if (item3 == null) {
                            continue;
                        }
                        int layoutIndexDisplay = ((IBetterListViewLayoutElementDisplayable)item3).LayoutIndexDisplay;
                        if (((IBetterListViewStateElement)item3).IsActive) {
                            if (layoutIndexDisplay < num) {
                                num = layoutIndexDisplay;
                            }
                            else if (layoutIndexDisplay > num2) {
                                num2 = layoutIndexDisplay;
                            }
                        }
                    }
                    if (num == int.MaxValue && num2 == int.MinValue) {
                        return;
                    }
                    if (num == int.MaxValue) {
                        num = num2;
                    }
                    if (num2 == int.MinValue) {
                        num2 = num;
                    }
                    Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                    BetterListViewItemBounds betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)this.layoutElementsItemsDisplay[num]).LayoutBounds;
                    Rectangle rectangle2 = new Rectangle(betterListViewItemBounds2.BoundsOuterExtended.X + offsetContentFromAbsolute.X, betterListViewItemBounds2.BoundsOuterExtended.Y + offsetContentFromAbsolute.Y, betterListViewItemBounds2.BoundsOuterExtended.Width, betterListViewItemBounds2.BoundsOuterExtended.Height);
                    int distanceDelta = BetterListView.GetDistanceDelta(rectangle2.Left, rectangle2.Right, boundsContent.Left, boundsContent.Right - 1);
                    int distanceDelta2 = BetterListView.GetDistanceDelta(rectangle2.Top, rectangle2.Bottom, boundsContent.Top, boundsContent.Bottom - 1);
                    int num4;
                    int num6;
                    if (num == num2) {
                        num4 = distanceDelta;
                        num6 = distanceDelta2;
                    }
                    else {
                        BetterListViewItemBounds betterListViewItemBounds3 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)this.layoutElementsItemsDisplay[num2]).LayoutBounds;
                        Rectangle rectangle3 = new Rectangle(betterListViewItemBounds3.BoundsOuterExtended.Left + offsetContentFromAbsolute.X, betterListViewItemBounds3.BoundsOuterExtended.Top + offsetContentFromAbsolute.Y, betterListViewItemBounds3.BoundsOuterExtended.Width, betterListViewItemBounds3.BoundsOuterExtended.Height);
                        if (distanceDelta <= 0) {
                            int distanceDelta3 = BetterListView.GetDistanceDelta(rectangle3.Left, rectangle3.Right, boundsContent.Left, boundsContent.Right - 1);
                            num4 = Math.Max(Math.Min(distanceDelta, distanceDelta3), boundsContent.Left - rectangle2.Left);
                        }
                        else {
                            num4 = distanceDelta;
                        }
                        if (distanceDelta2 <= 0) {
                            int distanceDelta4 = BetterListView.GetDistanceDelta(rectangle3.Top, rectangle3.Bottom, boundsContent.Top, boundsContent.Bottom - 1);
                            num6 = Math.Max(Math.Min(distanceDelta2, distanceDelta4), boundsContent.Top - rectangle2.Top);
                        }
                        else {
                            num6 = distanceDelta2;
                        }
                    }
                    if (num4 != 0) {
                        base.ScrollPositionHorizontal = Math.Min(Math.Max(base.ScrollPositionHorizontal - num4, base.HScrollBar.Minimum), base.HScrollBar.Maximum);
                    }
                    if (num6 != 0) {
                        base.ScrollPositionVertical = Math.Min(Math.Max(base.ScrollPositionVertical - num6, base.VScrollBar.Minimum), base.VScrollBar.Maximum);
                    }
                    if (num4 != 0 || num6 != 0) {
                        base.Invalidate(BetterListViewInvalidationLevel.Scroll, BetterListViewInvalidationFlags.Draw);
                    }
                    else {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                }
                if (!rectangle.IsEmpty) {
                    int num3 = BetterListView.GetDistanceDelta(rectangle.Left, rectangle.Right, boundsContent.Left, boundsContent.Right - 1);
                    int num5 = BetterListView.GetDistanceDelta(rectangle.Top, rectangle.Bottom, boundsContent.Top, boundsContent.Bottom - 1);
                    if (num3 != 0) {
                        base.ScrollPositionHorizontal = Math.Min(Math.Max(base.ScrollPositionHorizontal - num3, base.HScrollBar.Minimum), base.HScrollBar.Maximum);
                    }
                    if (num5 != 0) {
                        base.ScrollPositionVertical = Math.Min(Math.Max(base.ScrollPositionVertical - num5, base.VScrollBar.Minimum), base.VScrollBar.Maximum);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.Scroll, BetterListViewInvalidationFlags.Draw);
                }
            }
            finally {
                base.EndUpdate(suppressRefresh);
            }
        }

        private void SelectionsInitialize() {
            this.selectedIndices = new BetterListViewSelectedIndexCollection(this);
            this.selectedItems = new BetterListViewSelectedItemCollection(this);
            this.checkedIndices = new BetterListViewCheckedIndexCollection(this);
            this.checkedItems = new BetterListViewCheckedItemCollection(this);
        }

        private static int GetDistanceDelta(int itemStart, int itemEnd, int areaStart, int areaEnd) {
            if (itemEnd - itemStart >= areaEnd - areaStart) {
                if (itemEnd < areaStart || itemStart > areaEnd) {
                    return areaStart - itemStart;
                }
                return 0;
            }
            if (itemStart < areaStart) {
                return areaStart - itemStart;
            }
            if (itemEnd > areaEnd) {
                return areaEnd - itemEnd;
            }
            return 0;
        }

        private ReadOnlyCollection<BetterListViewItem> GetSelectionRange(BetterListViewElementBase layoutElementLast, BetterListViewElementBase layoutElementCurrent) {
            BetterListViewItem betterListViewItem = layoutElementLast as BetterListViewItem;
            BetterListViewGroup betterListViewGroup = layoutElementLast as BetterListViewGroup;
            bool flag = betterListViewItem != null;
            BetterListViewItem betterListViewItem2 = layoutElementCurrent as BetterListViewItem;
            BetterListViewGroup betterListViewGroup2 = layoutElementCurrent as BetterListViewGroup;
            bool flag2 = betterListViewItem2 != null;
            BetterListViewItem betterListViewItem3;
            BetterListViewItem betterListViewItem4;
            if (flag) {
                if (flag2) {
                    if (((IBetterListViewLayoutElementSelectable)betterListViewItem2).LayoutIndexSelection > ((IBetterListViewLayoutElementSelectable)betterListViewItem).LayoutIndexSelection) {
                        betterListViewItem3 = betterListViewItem;
                        betterListViewItem4 = betterListViewItem2;
                    }
                    else {
                        betterListViewItem3 = betterListViewItem2;
                        betterListViewItem4 = betterListViewItem;
                    }
                }
                else {
                    ReadOnlyCollection<BetterListViewItem> readOnlyCollection = betterListViewGroup2.GetItems(this);
                    if (((IBetterListViewLayoutElementSelectable)betterListViewGroup2).LayoutIndexSelection > ((IBetterListViewLayoutElementSelectable)this.GetVisibleGroup(betterListViewItem, allowNull: false)).LayoutIndexSelection) {
                        betterListViewItem3 = betterListViewItem;
                        betterListViewItem4 = readOnlyCollection[readOnlyCollection.Count - 1];
                    }
                    else {
                        betterListViewItem3 = readOnlyCollection[0];
                        betterListViewItem4 = betterListViewItem;
                    }
                }
            }
            else if (flag2) {
                ReadOnlyCollection<BetterListViewItem> readOnlyCollection2 = betterListViewGroup.GetItems(this);
                if (((IBetterListViewLayoutElementSelectable)this.GetVisibleGroup(betterListViewItem2, allowNull: false)).LayoutIndexSelection >= ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection) {
                    betterListViewItem3 = readOnlyCollection2[0];
                    betterListViewItem4 = betterListViewItem2;
                }
                else {
                    betterListViewItem3 = betterListViewItem2;
                    betterListViewItem4 = readOnlyCollection2[readOnlyCollection2.Count - 1];
                }
            }
            else {
                ReadOnlyCollection<BetterListViewItem> readOnlyCollection3 = betterListViewGroup2.GetItems(this);
                ReadOnlyCollection<BetterListViewItem> readOnlyCollection4 = betterListViewGroup.GetItems(this);
                if (((IBetterListViewLayoutElementSelectable)betterListViewGroup2).LayoutIndexSelection >= ((IBetterListViewLayoutElementSelectable)betterListViewGroup).LayoutIndexSelection) {
                    betterListViewItem3 = readOnlyCollection4[0];
                    betterListViewItem4 = readOnlyCollection3[readOnlyCollection3.Count - 1];
                }
                else {
                    betterListViewItem3 = readOnlyCollection3[0];
                    betterListViewItem4 = readOnlyCollection4[readOnlyCollection4.Count - 1];
                }
            }
            List<BetterListViewItem> list = new List<BetterListViewItem>();
            if (this.ShowGroups) {
                bool allowsExpandableGroups = this.LayoutGroupsCurrent.AllowsExpandableGroups;
                for (int i = ((IBetterListViewLayoutElementSelectable)this.GetVisibleGroup(betterListViewItem3, allowNull: false)).LayoutIndexSelection; i <= ((IBetterListViewLayoutElementSelectable)this.GetVisibleGroup(betterListViewItem4, allowNull: false)).LayoutIndexSelection; i++) {
                    BetterListViewGroup betterListViewGroup3 = this.layoutElementsGroups[i];
                    if (allowsExpandableGroups && (!betterListViewGroup3.IsExpanded || betterListViewGroup3.LayoutIndexItemSelectionFirst == -1)) {
                        list.AddRange(betterListViewGroup3.GetItems(this));
                        continue;
                    }
                    int num = ((((IBetterListViewLayoutElementSelectable)betterListViewItem3).LayoutIndexSelection >= betterListViewGroup3.LayoutIndexItemSelectionFirst && ((IBetterListViewLayoutElementSelectable)betterListViewItem3).LayoutIndexSelection <= betterListViewGroup3.LayoutIndexItemSelectionLast) ? ((IBetterListViewLayoutElementSelectable)betterListViewItem3).LayoutIndexSelection : betterListViewGroup3.LayoutIndexItemSelectionFirst);
                    int num2 = ((((IBetterListViewLayoutElementSelectable)betterListViewItem4).LayoutIndexSelection >= betterListViewGroup3.LayoutIndexItemSelectionFirst && ((IBetterListViewLayoutElementSelectable)betterListViewItem4).LayoutIndexSelection <= betterListViewGroup3.LayoutIndexItemSelectionLast) ? ((IBetterListViewLayoutElementSelectable)betterListViewItem4).LayoutIndexSelection : betterListViewGroup3.LayoutIndexItemSelectionLast);
                    for (int j = num; j <= num2; j++) {
                        list.Add(this.layoutElementsItemsSelection[j]);
                    }
                }
            }
            else {
                for (int k = ((IBetterListViewLayoutElementSelectable)betterListViewItem3).LayoutIndexSelection; k <= ((IBetterListViewLayoutElementSelectable)betterListViewItem4).LayoutIndexSelection; k++) {
                    list.Add(this.layoutElementsItemsSelection[k]);
                }
            }
            return list.AsReadOnly();
        }

        private void UpdateDragSelection(BetterListViewItemSelectionStateInfo itemSelectionStateInfo, ref BetterListViewSelectionInfo selectionInfo) {
            Rectangle dragSelectionRectangle = this.DragSelectionRectangle;
            if (dragSelectionRectangle.Width < SystemInformation.DragSize.Width && dragSelectionRectangle.Height < SystemInformation.DragSize.Height) {
                return;
            }
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            Point offsetContentToAbsolute = this.OffsetContentToAbsolute;
            BetterListViewItem betterListViewItem = null;
            BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet();
            bool flag = itemSelectionStateInfo.ModifierControl;
            bool flag2 = itemSelectionStateInfo.ModifierShift;
            if (!this.DragSelectionInversion && flag) {
                flag = false;
                flag2 = true;
            }
            Point location = new Point(dragSelectionRectangle.Left + offsetContentToAbsolute.X, dragSelectionRectangle.Top + offsetContentToAbsolute.Y);
            Point location2 = new Point(dragSelectionRectangle.Right - 1 + offsetContentToAbsolute.X, dragSelectionRectangle.Bottom - 1 + offsetContentToAbsolute.Y);
            bool isAnyItemSelectable = this.IsAnyItemSelectable;
            int num;
            int num2;
            if (isAnyItemSelectable) {
                BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
                num = ((IBetterListViewLayoutElementSelectable)layoutItemsCurrent.GetElementNearest(this.layoutElementsItemsSelection, location)).LayoutIndexSelection;
                num2 = ((IBetterListViewLayoutElementSelectable)layoutItemsCurrent.GetElementNearest(this.layoutElementsItemsSelection, location2)).LayoutIndexSelection;
            }
            else {
                num = 0;
                num2 = 0;
            }
            bool isAnyGroupVisible = this.IsAnyGroupVisible;
            int num3;
            int num4;
            if (isAnyGroupVisible) {
                BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
                num3 = ((IBetterListViewLayoutElementSelectable)layoutGroupsCurrent.GetElementNearest(this.layoutElementsGroups, location)).LayoutIndexSelection;
                num4 = ((IBetterListViewLayoutElementSelectable)layoutGroupsCurrent.GetElementNearest(this.layoutElementsGroups, location2)).LayoutIndexSelection;
            }
            else {
                num3 = 0;
                num4 = 0;
            }
            if (flag) {
                if (isAnyItemSelectable) {
                    for (int i = num; i <= num2; i++) {
                        BetterListViewItem betterListViewItem2 = this.layoutElementsItemsSelection[i];
                        Rectangle boundsSelection = ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds).BoundsSelection;
                        boundsSelection.Offset(offsetContentFromAbsolute);
                        if (dragSelectionRectangle.IntersectsWith(boundsSelection) ^ itemSelectionStateInfo.SelectedItems.Contains(betterListViewItem2)) {
                            if (betterListViewItem == null) {
                                betterListViewItem = betterListViewItem2;
                            }
                            betterListViewItemSet.Add(betterListViewItem2);
                        }
                    }
                }
                foreach (BetterListViewItem selectedItem in itemSelectionStateInfo.SelectedItems) {
                    if (selectedItem == null || ((IBetterListViewStateElement)selectedItem).State != BetterListViewElementState.ActiveVisible) {
                        betterListViewItemSet.Add(selectedItem);
                    }
                }
                if (isAnyGroupVisible) {
                    for (int j = num3; j <= num4; j++) {
                        BetterListViewGroup betterListViewGroup = this.layoutElementsGroups[j];
                        if (!betterListViewGroup.IsExpanded) {
                            Rectangle boundsOuter = ((IBetterListViewLayoutElementDisplayable)betterListViewGroup).LayoutBounds.BoundsOuter;
                            boundsOuter.Offset(offsetContentFromAbsolute);
                            if (dragSelectionRectangle.IntersectsWith(boundsOuter)) {
                                betterListViewItemSet.AddRange(betterListViewGroup.GetItems(this));
                            }
                        }
                    }
                }
            }
            else if (flag2) {
                if (isAnyItemSelectable) {
                    for (int k = num; k <= num2; k++) {
                        BetterListViewItem betterListViewItem3 = this.layoutElementsItemsSelection[k];
                        Rectangle boundsSelection2 = ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem3).LayoutBounds).BoundsSelection;
                        boundsSelection2.Offset(offsetContentFromAbsolute);
                        if (dragSelectionRectangle.IntersectsWith(boundsSelection2) || itemSelectionStateInfo.SelectedItems.Contains(betterListViewItem3)) {
                            if (betterListViewItem == null) {
                                betterListViewItem = betterListViewItem3;
                            }
                            betterListViewItemSet.Add(betterListViewItem3);
                        }
                    }
                }
                foreach (BetterListViewItem selectedItem2 in itemSelectionStateInfo.SelectedItems) {
                    if (selectedItem2 == null || ((IBetterListViewStateElement)selectedItem2).State != BetterListViewElementState.ActiveVisible) {
                        betterListViewItemSet.Add(selectedItem2);
                    }
                }
                if (isAnyGroupVisible) {
                    for (int l = num3; l <= num4; l++) {
                        BetterListViewGroup betterListViewGroup2 = this.layoutElementsGroups[l];
                        if (!betterListViewGroup2.IsExpanded) {
                            Rectangle boundsOuter2 = ((IBetterListViewLayoutElementDisplayable)betterListViewGroup2).LayoutBounds.BoundsOuter;
                            boundsOuter2.Offset(offsetContentFromAbsolute);
                            if (dragSelectionRectangle.IntersectsWith(boundsOuter2)) {
                                betterListViewItemSet.AddRange(betterListViewGroup2.GetItems(this));
                            }
                        }
                    }
                }
            }
            else {
                if (isAnyItemSelectable) {
                    for (int m = num; m <= num2; m++) {
                        BetterListViewItem betterListViewItem4 = this.layoutElementsItemsSelection[m];
                        Rectangle boundsSelection3 = ((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)betterListViewItem4).LayoutBounds).BoundsSelection;
                        boundsSelection3.Offset(offsetContentFromAbsolute);
                        if (dragSelectionRectangle.IntersectsWith(boundsSelection3)) {
                            if (betterListViewItem == null) {
                                betterListViewItem = betterListViewItem4;
                            }
                            betterListViewItemSet.Add(betterListViewItem4);
                        }
                    }
                }
                if (isAnyGroupVisible) {
                    for (int n = num3; n <= num4; n++) {
                        BetterListViewGroup betterListViewGroup3 = this.layoutElementsGroups[n];
                        if (!betterListViewGroup3.IsExpanded) {
                            Rectangle boundsOuter3 = ((IBetterListViewLayoutElementDisplayable)betterListViewGroup3).LayoutBounds.BoundsOuter;
                            boundsOuter3.Offset(offsetContentFromAbsolute);
                            if (dragSelectionRectangle.IntersectsWith(boundsOuter3)) {
                                betterListViewItemSet.AddRange(betterListViewGroup3.GetItems(this));
                            }
                        }
                    }
                }
            }
            if (betterListViewItem != null) {
                selectionInfo.LastSelectedElement = betterListViewItem;
            }
            selectionInfo.SetSelectedItems(this.GetSelectedGroups(betterListViewItemSet), betterListViewItemSet);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
        }

        private void PerformDragSelectionEnd(BetterListViewReadOnlyItemSet selectedItemsOldSet) {
            BetterListViewReadOnlyItemSet betterListViewReadOnlyItemSet = new BetterListViewReadOnlyItemSet(this.SelectedItemsSet);
            if (!selectedItemsOldSet.EqualsContent(betterListViewReadOnlyItemSet) && !this.IsSelectionChangedSuspended) {
                try {
                    base.BeginUpdate();
                    BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection(selectedItemsOldSet);
                    BetterListViewItemCollection betterListViewItemCollection2 = new BetterListViewItemCollection(betterListViewReadOnlyItemSet);
                    betterListViewItemCollection.Sort(BetterListViewItemIndexComparer.Instance);
                    betterListViewItemCollection2.Sort(BetterListViewItemIndexComparer.Instance);
                    this.OnSelectedItemsChanged(new BetterListViewSelectedItemsChangedEventArgs(BetterListViewSelectionChangeMode.MouseDragSelectionEnd, betterListViewItemCollection, betterListViewItemCollection2));
                }
                finally {
                    base.EndUpdate(suppressRefresh: true);
                }
            }
        }

        /// <summary>
        ///   Refresh the control with current invalidation information.
        /// </summary>
        internal override void RefreshView() {
            if (!base.IsUpdating && !base.Disposing && !base.IsDisposed && base.CachedGraphics != null) {
                this.PerformRefreshView();
                base.RefreshView();
                this.ResurrectCalls();
            }
        }

        internal void SetStateInfo(BetterListViewStateInfo stateInfoNew) {
            BetterListViewStateInfo stateInfoB = this.StateInfo;
            if (!(stateInfoNew == stateInfoB)) {
                this.stateInfo = stateInfoNew;
                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            }
        }

        internal void SetSelectionInfo(BetterListViewSelectionInfo selectionInfoNew, BetterListViewSelectionChangeMode selectionChangeMode) {
            BetterListViewSelectionInfo selectionInfoB = this.SelectionInfo;
            if (selectionInfoNew == selectionInfoB) {
                return;
            }
            this.selectedIndices.ClearCache();
            this.selectedItems.ClearCache();
            this.selectionInfo = selectionInfoNew;
            if (!selectionInfoNew.SelectedItems.EqualsContent(selectionInfoB.SelectedItems) && !this.IsSelectionChangedSuspended) {
                List<KeyValuePair<BetterListViewItem, BetterListViewAddress>> list = new List<KeyValuePair<BetterListViewItem, BetterListViewAddress>>();
                foreach (KeyValuePair<BetterListViewItem, BetterListViewAddress> selectedItem in selectionInfoB.SelectedItems) {
                    BetterListViewItem key = selectedItem.Key;
                    if (key.ListView == this && !selectionInfoNew.SelectedItems.ContainsKey(key)) {
                        list.Add(selectedItem);
                    }
                }
                List<KeyValuePair<BetterListViewItem, BetterListViewAddress>> list2 = new List<KeyValuePair<BetterListViewItem, BetterListViewAddress>>();
                foreach (KeyValuePair<BetterListViewItem, BetterListViewAddress> selectedItem2 in selectionInfoNew.SelectedItems) {
                    BetterListViewItem key2 = selectedItem2.Key;
                    if (!selectionInfoB.SelectedItems.ContainsKey(key2)) {
                        list2.Add(selectedItem2);
                    }
                }
                int num = list.Count + list2.Count;
                BetterListViewItem[] array = null;
                if (num != 0) {
                    array = new BetterListViewItem[num];
                    KeyValuePair<BetterListViewAddress, bool>[] array2 = new KeyValuePair<BetterListViewAddress, bool>[num];
                    int num2 = 0;
                    foreach (KeyValuePair<BetterListViewItem, BetterListViewAddress> item2 in list) {
                        array[num2] = item2.Key;
                        array2[num2] = new KeyValuePair<BetterListViewAddress, bool>(item2.Value, value: false);
                        num2++;
                    }
                    foreach (KeyValuePair<BetterListViewItem, BetterListViewAddress> item3 in list2) {
                        array[num2] = item3.Key;
                        array2[num2] = new KeyValuePair<BetterListViewAddress, bool>(item3.Value, value: true);
                        num2++;
                    }
                    Array.Sort(array, array2, BetterListViewItemIndexComparer.Instance);
                    for (num2 = 0; num2 < num; num2++) {
                        BetterListViewItem item = array[num2];
                        BetterListViewAddress key3 = array2[num2].Key;
                        bool value = array2[num2].Value;
                        this.OnItemSelectionChanged(new BetterListViewItemSelectionChangedEventArgs(value, item, key3, key3.Index));
                    }
                }
                BetterListViewItemCollection betterListViewItemCollection = new BetterListViewItemCollection(selectionInfoNew.SelectedItems.Keys);
                BetterListViewItemCollection betterListViewItemCollection2 = new BetterListViewItemCollection(selectionInfoB.SelectedItems.Keys);
                betterListViewItemCollection.Sort(BetterListViewItemIndexComparer.Instance);
                betterListViewItemCollection2.Sort(BetterListViewItemIndexComparer.Instance);
                this.OnSelectedItemsChanged(new BetterListViewSelectedItemsChangedEventArgs(selectionChangeMode, betterListViewItemCollection, betterListViewItemCollection2));
                this.OnSelectedIndexChanged(this, EventArgs.Empty);
                if (array != null) {
                    Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
                    int num3 = int.MaxValue;
                    int num4 = int.MinValue;
                    int num5 = int.MaxValue;
                    int num6 = int.MinValue;
                    BetterListViewItem[] array3 = array;
                    BetterListViewItem[] array4 = array3;
                    foreach (BetterListViewItem betterListViewItem in array4) {
                        Rectangle boundsOuterExtended = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                        num3 = Math.Min(num3, boundsOuterExtended.Top);
                        num4 = Math.Max(num4, boundsOuterExtended.Bottom);
                        num5 = Math.Min(num5, boundsOuterExtended.Left);
                        num6 = Math.Max(num6, boundsOuterExtended.Right);
                    }
                    Rectangle region = Rectangle.Intersect(new Rectangle(num5 + offsetContentFromAbsolute.X, num3 + offsetContentFromAbsolute.Y, num6 - num5 + 1, num4 - num3 + 1), base.BoundsContent);
                    if (this.LayoutItemsCurrent.SingleBoundary) {
                        region = new Rectangle(region.Left, region.Top - 1, region.Width, region.Height + 2);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region);
                }
                if (betterListViewItemCollection.Count == 1) {
                    BetterListViewItem betterListViewItem2 = betterListViewItemCollection[0];
                    if (betterListViewItem2.ListView != null) {
                        base.AccessibilityNotifyClients(AccessibleEvents.Selection, BetterListViewAccessibleObject.GetItemId(betterListViewItem2));
                    }
                }
                else if (list2.Count == 1 && list.Count == 0) {
                    BetterListViewItem key4 = list2[0].Key;
                    if (key4.ListView != null) {
                        base.AccessibilityNotifyClients(AccessibleEvents.SelectionAdd, BetterListViewAccessibleObject.GetItemId(key4));
                    }
                }
                else if (list2.Count == 0 && list.Count == 1) {
                    BetterListViewItem key5 = list[0].Key;
                    if (key5.ListView != null) {
                        base.AccessibilityNotifyClients(AccessibleEvents.SelectionRemove, BetterListViewAccessibleObject.GetItemId(key5));
                    }
                }
                else {
                    base.AccessibilityNotifyClients(AccessibleEvents.SelectionWithin, 0);
                }
            }
            if (this.dataSource != null && selectionChangeMode != BetterListViewSelectionChangeMode.DataBinding) {
                this.DataUpdatePositionToSource();
            }
        }

        internal void SetFocusInfo(BetterListViewFocusInfo focusInfoNew, BetterListViewSelectionChangeMode selectionChangeMode) {
            BetterListViewFocusInfo focusInfoB = this.FocusInfo;
            if (focusInfoNew == focusInfoB) {
                return;
            }
            if (selectionChangeMode == BetterListViewSelectionChangeMode.Keyboard) {
                _ = this.AllowDisplayFocusRectangle;
                this.AllowDisplayFocusRectangle = true;
            }
            BetterListViewItem betterListViewItem = this.focusInfo.Element as BetterListViewItem;
            BetterListViewItem betterListViewItem2 = focusInfoNew.Element as BetterListViewItem;
            int focusedColumnIndexOld = ((betterListViewItem != null) ? this.focusInfo.ColumnIndex : (-1));
            int num = ((betterListViewItem2 != null) ? focusInfoNew.ColumnIndex : (-1));
            BetterListViewFocusedItemChangedEventArgs eventArgs = new BetterListViewFocusedItemChangedEventArgs(this.focusInfo.Element as BetterListViewGroup, betterListViewItem, focusedColumnIndexOld, focusInfoNew.Element as BetterListViewGroup, betterListViewItem2, num);
            this.focusInfo = focusInfoNew;
            bool singleBoundary = this.LayoutItemsCurrent.SingleBoundary;
            if (betterListViewItem != null && betterListViewItem.ListView == this) {
                Rectangle region = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                region.Offset(this.OffsetContentFromAbsolute);
                if (singleBoundary) {
                    region = new Rectangle(region.Left, region.Top - 1, region.Width, region.Height + 2);
                }
                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region);
            }
            if (betterListViewItem2 != null) {
                Rectangle region2 = ((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds.BoundsOuterExtended;
                region2.Offset(this.OffsetContentFromAbsolute);
                if (singleBoundary) {
                    region2 = new Rectangle(region2.Left, region2.Top - 1, region2.Width, region2.Height + 2);
                }
                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region2);
            }
            this.OnFocusedItemChanged(eventArgs);
            if (betterListViewItem2 != null) {
                int itemId = BetterListViewAccessibleObject.GetItemId(betterListViewItem2);
                if (this.ViewInternal == BetterListViewViewInternal.DetailsColumns) {
                    base.AccessibilityNotifyClients(AccessibleEvents.Focus, itemId, num);
                }
                else {
                    base.AccessibilityNotifyClients(AccessibleEvents.Focus, itemId);
                }
            }
        }

        internal void SetHitTestInfo(BetterListViewHitTestInfo hitTestInfoNew) {
            BetterListViewHitTestInfo betterListViewHitTestInfo = this.HitTestInfo;
            if (hitTestInfoNew == betterListViewHitTestInfo) {
                return;
            }
            Point offsetColumnsFromAbsolute = this.OffsetColumnsFromAbsolute;
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            if (this.OptimizedInvalidation) {
                BetterListViewColumnHeader columnHeader = betterListViewHitTestInfo.ColumnHeader;
                BetterListViewColumnHeader columnHeader2 = hitTestInfoNew.ColumnHeader;
                Rectangle region;
                if (columnHeader != null) {
                    region = ((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds.BoundsOuter;
                    region.Offset(offsetColumnsFromAbsolute);
                }
                else {
                    region = Rectangle.Empty;
                }
                Rectangle region2;
                if (columnHeader2 != null) {
                    region2 = ((IBetterListViewLayoutElementDisplayable)columnHeader2).LayoutBounds.BoundsOuter;
                    region2.Offset(offsetColumnsFromAbsolute);
                }
                else {
                    region2 = Rectangle.Empty;
                }
                if (columnHeader != columnHeader2) {
                    if (columnHeader != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region);
                    }
                    if (columnHeader2 != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region2);
                    }
                }
                else if (columnHeader != null) {
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region);
                }
                bool singleBoundary = this.LayoutItemsCurrent.SingleBoundary;
                BetterListViewItem betterListViewItem = betterListViewHitTestInfo.ItemSelection ?? betterListViewHitTestInfo.ItemDisplay;
                BetterListViewItem betterListViewItem2 = hitTestInfoNew.ItemSelection ?? betterListViewHitTestInfo.ItemDisplay;
                Rectangle region3;
                if (betterListViewItem != null) {
                    region3 = ((IBetterListViewLayoutElementDisplayable)betterListViewItem).LayoutBounds.BoundsOuterExtended;
                    region3.Offset(offsetContentFromAbsolute);
                    if (singleBoundary) {
                        region3 = new Rectangle(region3.Left, region3.Top - 1, region3.Width, region3.Height + 2);
                    }
                }
                else {
                    region3 = Rectangle.Empty;
                }
                Rectangle region4;
                if (betterListViewItem2 != null) {
                    region4 = ((IBetterListViewLayoutElementDisplayable)betterListViewItem2).LayoutBounds.BoundsOuterExtended;
                    region4.Offset(offsetContentFromAbsolute);
                    if (singleBoundary) {
                        region4 = new Rectangle(region4.Left, region4.Top - 1, region4.Width, region4.Height + 2);
                    }
                }
                else {
                    region4 = Rectangle.Empty;
                }
                if (betterListViewItem != betterListViewItem2) {
                    if (betterListViewItem != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region3);
                    }
                    if (betterListViewItem2 != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region4);
                    }
                }
                else if (betterListViewItem != null) {
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region3);
                }
                BetterListViewGroup group = betterListViewHitTestInfo.Group;
                BetterListViewGroup group2 = hitTestInfoNew.Group;
                Rectangle region5;
                if (group != null) {
                    region5 = ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds).BoundsSelection;
                    region5.Offset(offsetContentFromAbsolute);
                }
                else {
                    region5 = Rectangle.Empty;
                }
                Rectangle region6;
                if (group2 != null) {
                    region6 = ((BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group2).LayoutBounds).BoundsSelection;
                    region6.Offset(offsetContentFromAbsolute);
                }
                else {
                    region6 = Rectangle.Empty;
                }
                if (group != group2) {
                    if (group != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region5);
                    }
                    if (group2 != null) {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region6);
                    }
                }
                else if (group != null) {
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, region5);
                }
            }
            else {
                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            }
            this.hitTestInfo = hitTestInfoNew;
            this.OnHitTestChanged(new BetterListViewHitTestChangedEventArgs(betterListViewHitTestInfo, hitTestInfoNew));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected override void OnHandleCreated(EventArgs eventArgs) {
            base.OnHandleCreated(eventArgs);
            this.ResurrectCalls();
        }

        private void PerformRefreshView() {
            if (base.InvalidationInfo.Level == BetterListViewInvalidationLevel.Setup) {
                this.LayoutSetup();
            }
            if (this.FocusInfo.Element is IBetterListViewStateElement betterListViewStateElement && !betterListViewStateElement.IsActive) {
                this.SetFocusInfo(BetterListViewFocusInfo.Empty, BetterListViewSelectionChangeMode.UserCode);
            }
            if (base.InvalidationInfo.Level >= BetterListViewInvalidationLevel.MeasureElements) {
                this.LayoutMeasureElements(base.HScrollBarVisible, base.VScrollBarVisible);
            }
            if (base.InvalidationInfo.Level >= BetterListViewInvalidationLevel.MeasureContent) {
                this.LayoutMeasureContent();
            }
            if ((base.InvalidationInfo.Flags & BetterListViewInvalidationFlags.Position) == BetterListViewInvalidationFlags.Position && this.LayoutPosition()) {
                base.InvalidationInfo = new BetterListViewInvalidationInfo(base.InvalidationInfo.Level, base.InvalidationInfo.Flags & ~BetterListViewInvalidationFlags.Position, base.InvalidationInfo.Region);
            }
            if (base.InvalidationInfo.Level >= BetterListViewInvalidationLevel.Adjust) {
                this.LayoutAdjust();
            }
            this.ResurrectCall(BetterListViewPostponedCallType.EnsureVisible);
            if (base.InvalidationInfo.Level >= BetterListViewInvalidationLevel.Scroll) {
                this.LayoutScroll();
            }
            if ((base.InvalidationInfo.Flags & BetterListViewInvalidationFlags.Draw) == BetterListViewInvalidationFlags.Draw && base.Visible) {
                base.DrawingRedraw();
                base.InvalidationInfo = new BetterListViewInvalidationInfo(base.InvalidationInfo.Level, base.InvalidationInfo.Flags & ~BetterListViewInvalidationFlags.Draw, base.InvalidationInfo.Region);
            }
        }

        private void PostponeCall(BetterListViewPostponedCallType type, Delegate method, object[] parameters) {
            this.postponedCalls.Remove(type);
            this.postponedCalls.Add(type, new BetterListViewPostponedCallData(method, parameters));
        }

        private void ResurrectCalls() {
            if (base.IsUpdating || !base.IsHandleCreated) {
                return;
            }
            foreach (BetterListViewPostponedCallType value in Enum.GetValues(typeof(BetterListViewPostponedCallType))) {
                this.ResurrectCall(value);
            }
        }

        private void ResurrectCall(BetterListViewPostponedCallType type) {
            if (!base.IsUpdating && base.IsHandleCreated && this.postponedCalls.TryGetValue(type, out var value)) {
                this.postponedCalls.Remove(type);
                base.Invoke(value.Method, value.Parameters);
            }
        }

        private BetterListViewColumnHeaderStateInfo GetColumnHeaderStateInfo(BetterListViewColumnHeader columnHeader, BetterListViewHitTestLocations locations) {
            BetterListViewColumnHeaderStyle style = columnHeader.GetStyle(this);
            bool flag;
            bool flag2;
            if (style == BetterListViewColumnHeaderStyle.None) {
                flag = false;
                flag2 = false;
            }
            else {
                flag = style == BetterListViewColumnHeaderStyle.Clickable || style == BetterListViewColumnHeaderStyle.Sortable || style == BetterListViewColumnHeaderStyle.Unsortable;
                flag2 = this.SortList.Contains(columnHeader.Index) && columnHeader.SortOrder != BetterListViewSortOrder.None;
            }
            BetterListViewColumnHeaderState columnHeaderState = (((locations & BetterListViewHitTestLocations.ColumnHeader) == BetterListViewHitTestLocations.ColumnHeader && !base.DesignMode && !this.ReadOnly) ? ((this.StateInfo.State != BetterListViewState.Normal) ? ((this.StateInfo.State == BetterListViewState.ColumnSelection && columnHeader == this.StateInfo.ColumnSelectionStateInfo.Column) ? (flag ? BetterListViewColumnHeaderState.Pressed : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal)) : ((this.StateInfo.State == BetterListViewState.ColumnBeforeResize && columnHeader == this.StateInfo.ColumnResizeStateInfo.Column) ? (flag ? BetterListViewColumnHeaderState.Hot : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal)) : ((this.StateInfo.State != BetterListViewState.ColumnResize) ? ((this.StateInfo.State == BetterListViewState.ColumnReorder) ? (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal) : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal)) : (flag ? BetterListViewColumnHeaderState.Hot : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal))))) : (flag ? BetterListViewColumnHeaderState.Hot : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal))) : (flag2 ? BetterListViewColumnHeaderState.Sorted : BetterListViewColumnHeaderState.Normal));
            return new BetterListViewColumnHeaderStateInfo(columnHeaderState, columnHeader.SortOrder);
        }

        private BetterListViewItemStateInfo GetItemStateInfo(BetterListViewItem item, BetterListViewHitTestLocations locations) {
            BetterListViewItemState betterListViewItemState = BetterListViewItemState.Normal;
            if (!this.ReadOnly) {
                BetterListViewItem objA = ((!this.FocusInfo.IsEmpty) ? (this.FocusInfo.Element as BetterListViewItem) : null);
                if (objA == item && this.Focused) {
                    betterListViewItemState |= BetterListViewItemState.Focused;
                }
                if ((locations & BetterListViewHitTestLocations.ItemSelection) == BetterListViewHitTestLocations.ItemSelection && item.IsSelectable) {
                    if (this.StateInfo.State == BetterListViewState.ItemBeforeCheckKeyboard || this.StateInfo.State == BetterListViewState.ItemBeforeCheckMouse || this.StateInfo.State == BetterListViewState.ItemBeforeSelection || this.StateInfo.State == BetterListViewState.ItemSelection || this.StateInfo.State == BetterListViewState.Normal) {
                        betterListViewItemState |= BetterListViewItemState.Hot;
                    }
                    else if (this.StateInfo.State == BetterListViewState.ItemDrag && this.GetDragDropDisplay(item) == BetterListViewDragDropDisplay.Highlight) {
                        betterListViewItemState |= BetterListViewItemState.DropHighlight;
                    }
                }
            }
            BetterListViewItem selectableItem = item.SelectableItem;
            if (selectableItem != null && selectableItem.Selected) {
                betterListViewItemState |= BetterListViewItemState.Selected;
            }
            BetterListViewItemExpandButtonState expandButtonState = BetterListViewItemExpandButtonState.Collapsed;
            CheckBoxState checkBoxState = CheckBoxState.UncheckedNormal;
            bool flag;
            bool flag2;
            if (base.DesignMode || this.ReadOnly) {
                flag = false;
                flag2 = false;
            }
            else if (this.StateInfo.State == BetterListViewState.ItemBeforeCheckKeyboard) {
                flag = (flag2 = this.StateInfo.ItemBeforeCheckStateInfo.Item == item);
            }
            else if (this.StateInfo.State == BetterListViewState.ItemBeforeCheckMouse) {
                flag = (flag2 = this.StateInfo.ItemBeforeCheckStateInfo.Item == item && (locations & BetterListViewHitTestLocations.ItemCheckBox) == BetterListViewHitTestLocations.ItemCheckBox);
            }
            else if (this.StateInfo.State == BetterListViewState.Normal) {
                flag = (locations & BetterListViewHitTestLocations.ItemCheckBox) == BetterListViewHitTestLocations.ItemCheckBox;
                flag2 = false;
            }
            else {
                flag = false;
                flag2 = false;
            }
            if (flag2) {
                switch (item.CheckState) {
                    case CheckState.Checked:
                        checkBoxState = CheckBoxState.CheckedPressed;
                        break;
                    case CheckState.Indeterminate:
                        checkBoxState = CheckBoxState.MixedPressed;
                        break;
                    case CheckState.Unchecked:
                        checkBoxState = CheckBoxState.UncheckedPressed;
                        break;
                }
            }
            else if (flag) {
                switch (item.CheckState) {
                    case CheckState.Checked:
                        checkBoxState = CheckBoxState.CheckedHot;
                        break;
                    case CheckState.Indeterminate:
                        checkBoxState = CheckBoxState.MixedHot;
                        break;
                    case CheckState.Unchecked:
                        checkBoxState = CheckBoxState.UncheckedHot;
                        break;
                }
            }
            else {
                switch (item.CheckState) {
                    case CheckState.Checked:
                        checkBoxState = CheckBoxState.CheckedNormal;
                        break;
                    case CheckState.Indeterminate:
                        checkBoxState = CheckBoxState.MixedNormal;
                        break;
                    case CheckState.Unchecked:
                        checkBoxState = CheckBoxState.UncheckedNormal;
                        break;
                }
            }
            return new BetterListViewItemStateInfo(betterListViewItemState, expandButtonState, checkBoxState);
        }

        private BetterListViewGroupStateInfo GetGroupStateInfo(BetterListViewGroup group, BetterListViewHitTestLocations locations) {
            BetterListViewGroupState betterListViewGroupState = BetterListViewGroupState.Normal;
            if (!this.ReadOnly) {
                BetterListViewGroup objA = ((!this.FocusInfo.IsEmpty) ? (this.FocusInfo.Element as BetterListViewGroup) : null);
                if (objA == group && this.Focused) {
                    betterListViewGroupState |= BetterListViewGroupState.Focused;
                }
                if ((locations & BetterListViewHitTestLocations.Group) == BetterListViewHitTestLocations.Group && (this.StateInfo.State == BetterListViewState.GroupBeforeSelection || this.StateInfo.State == BetterListViewState.Normal) && (this.GroupHeaderBehavior & BetterListViewGroupHeaderBehavior.MouseHighlight) == BetterListViewGroupHeaderBehavior.MouseHighlight) {
                    betterListViewGroupState |= BetterListViewGroupState.Hot;
                }
            }
            BetterListViewGroupExpandButtonState expandButtonState = BetterListViewGroupExpandButtonState.ExpandedNormal;
            return new BetterListViewGroupStateInfo(betterListViewGroupState, expandButtonState);
        }

        /// <summary>
        ///   Get information about mouse location with respect to control state.
        /// </summary>
        /// <returns>Information about mouse location with respect to control state.</returns>
        public BetterListViewHitTestInfo HitTest() {
            return this.HitTest(base.PointToClient(Control.MousePosition));
        }

        /// <summary>
        ///   Get information about mouse location with respect to control state.
        /// </summary>
        /// <param name="ptMouse">Mouse location (client coordinates).</param>
        /// <returns>Information about mouse location with respect to control state.</returns>
        public BetterListViewHitTestInfo HitTest(Point ptMouse) {
            if (base.IsUpdating) {
                return BetterListViewHitTestInfo.Empty;
            }
            if (this.StateInfo.State != BetterListViewState.ItemDrag) {
                IntPtr capture = NativeMethods.GetCapture();
                if (capture != IntPtr.Zero && capture != base.Handle) {
                    return BetterListViewHitTestInfo.Empty;
                }
            }
            BetterListViewHitTestLocations betterListViewHitTestLocations = BetterListViewHitTestLocations.Custom;
            BetterListViewColumnHeader columnHeader;
            BetterListViewHitPart columnHeaderPart;
            if (this.StateInfo.State == BetterListViewState.ColumnBeforeResize || this.StateInfo.State == BetterListViewState.ColumnResize) {
                columnHeader = this.StateInfo.ColumnResizeStateInfo.Column;
                columnHeaderPart = BetterListViewHitPart.Right;
            }
            else {
                this.GetColumnHeaderAt(ptMouse, includeBorders: true, out columnHeader, out columnHeaderPart);
            }
            this.GetItemAt(ptMouse, selectableOnly: false, nearest: false, out var item, out var itemPart);
            this.GetItemAt(ptMouse, selectableOnly: true, nearest: false, out var item2, out var itemPart2);
            this.GetSubItemAt(ptMouse, out var subItem, out var subItemPart);
            this.GetGroupAt(ptMouse, out var group, out var groupPart);
            BetterListViewColumnHeaderStateInfo columnHeaderStateInfo = BetterListViewColumnHeaderStateInfo.Empty;
            BetterListViewItemStateInfo itemStateInfo = BetterListViewItemStateInfo.Empty;
            BetterListViewGroupStateInfo groupStateInfo = BetterListViewGroupStateInfo.Empty;
            Point offsetColumnsFromAbsolute = this.OffsetColumnsFromAbsolute;
            Point offsetContentFromAbsolute = this.OffsetContentFromAbsolute;
            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
            if (this.BoundsColumnHeaders.Width != 0 && this.BoundsColumnHeaders.Height != 0 && this.BoundsColumnHeaders.Contains(ptMouse)) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnArea;
            }
            if (base.BoundsContent.Width != 0 && base.BoundsContent.Height != 0 && base.BoundsContent.Contains(ptMouse)) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.ContentArea;
            }
            if (columnHeader != null) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnHeader;
                BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds.Clone();
                betterListViewColumnHeaderBounds.Offset(offsetColumnsFromAbsolute);
                if (!betterListViewColumnHeaderBounds.BoundsImage.Size.IsEmpty && betterListViewColumnHeaderBounds.BoundsImage.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnHeaderImage;
                }
                if (!betterListViewColumnHeaderBounds.BoundsText.Size.IsEmpty && betterListViewColumnHeaderBounds.BoundsText.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnHeaderText;
                }
                if (!betterListViewColumnHeaderBounds.BoundsSortGlyph.Size.IsEmpty && betterListViewColumnHeaderBounds.BoundsSortGlyph.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnHeaderSortGlyph;
                }
                if (betterListViewColumnHeaderBounds.BoundsBorder.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ColumnHeaderBorder;
                }
                columnHeaderStateInfo = this.GetColumnHeaderStateInfo(columnHeader, betterListViewHitTestLocations);
            }
            if (item != null) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.Item;
                BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds.Clone();
                betterListViewItemBounds.Offset(offsetContentFromAbsolute);
                if (item.CheckBoxAppearance != 0 && !betterListViewItemBounds.BoundsCheckBox.Size.IsEmpty && betterListViewItemBounds.BoundsCheckBox.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ItemCheckBox;
                }
                Rectangle boundsImage = betterListViewItemBounds.SubItemBounds[0].BoundsImage;
                if (!boundsImage.Size.IsEmpty && boundsImage.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ItemImage;
                }
                Rectangle boundsText = betterListViewItemBounds.SubItemBounds[0].BoundsText;
                BetterListViewSubItem betterListViewSubItem = item.SubItems[0];
                if (!boundsText.Size.IsEmpty && betterListViewSubItem.LayoutCheckTextIntersection(base.CachedGraphics, layoutItemsCurrent.GetMaximumTextLines(betterListViewSubItem), boundsText, ptMouse) && (betterListViewHitTestLocations & BetterListViewHitTestLocations.ItemImage) != BetterListViewHitTestLocations.ItemImage) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ItemText;
                }
            }
            if (item2 != null) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.Item;
                BetterListViewItemBounds betterListViewItemBounds2 = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item2).LayoutBounds.Clone();
                betterListViewItemBounds2.Offset(offsetContentFromAbsolute);
                if (!betterListViewItemBounds2.BoundsSelection.Size.IsEmpty && betterListViewItemBounds2.BoundsSelection.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.ItemSelection;
                }
                itemStateInfo = this.GetItemStateInfo(item2, betterListViewHitTestLocations);
            }
            if (subItem != null) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.SubItem;
                BetterListViewSubItemBounds betterListViewSubItemBounds = (BetterListViewSubItemBounds)((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)subItem.Item).LayoutBounds).SubItemBounds[subItem.Index].Clone();
                betterListViewSubItemBounds.Offset(offsetContentFromAbsolute);
                if (!betterListViewSubItemBounds.BoundsImage.Size.IsEmpty && betterListViewSubItemBounds.BoundsImage.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.SubItemImage;
                }
                if (!betterListViewSubItemBounds.BoundsText.Size.IsEmpty && subItem.LayoutCheckTextIntersection(base.CachedGraphics, layoutItemsCurrent.GetMaximumTextLines(subItem), betterListViewSubItemBounds.BoundsText, ptMouse) && (betterListViewHitTestLocations & BetterListViewHitTestLocations.SubItemImage) != BetterListViewHitTestLocations.SubItemImage) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.SubItemText;
                }
            }
            if (group != null) {
                betterListViewHitTestLocations |= BetterListViewHitTestLocations.Group;
                BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds.Clone();
                betterListViewGroupBounds.Offset(offsetContentFromAbsolute);
                if (!betterListViewGroupBounds.BoundsImage.Size.IsEmpty && betterListViewGroupBounds.BoundsImage.Contains(ptMouse)) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.GroupImage;
                }
                if (!betterListViewGroupBounds.BoundsText.Size.IsEmpty && ptMouse.X >= betterListViewGroupBounds.BoundsText.Left && ptMouse.X < betterListViewGroupBounds.BoundsText.Right) {
                    betterListViewHitTestLocations |= BetterListViewHitTestLocations.GroupText;
                }
                groupStateInfo = this.GetGroupStateInfo(group, betterListViewHitTestLocations);
            }
            return new BetterListViewHitTestInfo(betterListViewHitTestLocations, columnHeaderStateInfo, itemStateInfo, groupStateInfo, columnHeader, columnHeaderPart, item, itemPart, item2, itemPart2, subItem, subItemPart, group, groupPart);
        }

        public void SuspendSelectionChanged() {
            this.selectionChangedSuspendLevel++;
        }

        /// <summary>
        ///   Resume raising SelectionChanged events.
        /// </summary>
        public void ResumeSelectionChanged() {
            this.selectionChangedSuspendLevel = Math.Max(this.selectionChangedSuspendLevel - 1, 0);
        }

        /// <summary>
        ///   Suspend sorting items while control content changes.
        /// </summary>
        public void SuspendSort() {
            this.sortSuspendLevel++;
        }

        /// <summary>
        ///   Resume sorting items while control content changes.
        /// </summary>
        public void ResumeSort() {
            this.ResumeSort(suppressSort: false);
        }

        /// <summary>
        ///   Resume sorting items while control content changes.
        /// </summary>
        /// <param name="suppressSort">Do not re-sort items immediately.</param>
        public void ResumeSort(bool suppressSort) {
            if (this.sortSuspendLevel != 0) {
                this.sortSuspendLevel--;
                if (this.sortSuspendLevel == 0 && !suppressSort && this.sortList.Count != 0) {
                    this.SortItems(columnClicked: false, sortAlways: false);
                }
            }
        }

        private void ToolTipsInitialize() {
            this.toolTip = new ToolTip();
            this.toolTip.Tag = BetterListViewToolTipTarget.Empty;
            this.toolTip.Draw += ToolTipOnDraw;
            this.toolTip.Popup += ToolTipOnPopup;
        }

        private void ToolTipsMouseMove() {
            BetterListViewElementBase element = null;
            Rectangle bounds = Rectangle.Empty;
            bool flag = false;
            BetterListViewToolTipInfo toolTipInfo = BetterListViewToolTipInfo.Empty;
            Point pt = base.PointToClient(Control.MousePosition);
            bool flag2 = this.ViewInternal == BetterListViewViewInternal.DetailsColumns;
            if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeader) == BetterListViewHitTestLocations.ColumnHeader && this.ShowToolTipsColumns) {
                BetterListViewColumnHeader columnHeader = this.HitTestInfo.ColumnHeader;
                BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)((IBetterListViewLayoutElementDisplayable)columnHeader).LayoutBounds.Clone();
                betterListViewColumnHeaderBounds.Offset(this.OffsetColumnsFromAbsolute);
                BetterListViewToolTipInfoCollection betterListViewToolTipInfoCollection = (BetterListViewToolTipInfoCollection)columnHeader.ToolTips.Clone();
                if (this.AllowAutoToolTipsColumns && !betterListViewToolTipInfoCollection.ContainsLocation(BetterListViewToolTipLocation.Client)) {
                    betterListViewToolTipInfoCollection.Add(new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Client, this.GetToolTipText(columnHeader.Text), showOnPartialTextVisibility: true));
                }
                foreach (BetterListViewToolTipInfo item2 in betterListViewToolTipInfoCollection) {
                    if (item2.Location == BetterListViewToolTipLocation.Custom) {
                        Rectangle bounds2 = item2.Bounds;
                        bounds2.Offset(betterListViewColumnHeaderBounds.BoundsInner.Location);
                        if (bounds2.Contains(pt)) {
                            bounds = item2.Bounds;
                            toolTipInfo = item2;
                            break;
                        }
                    }
                }
                if (toolTipInfo.IsEmpty) {
                    if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderImage) == BetterListViewHitTestLocations.ColumnHeaderImage && betterListViewToolTipInfoCollection.TryGetToolTipInfo(BetterListViewToolTipLocation.Image, out toolTipInfo)) {
                        bounds = betterListViewColumnHeaderBounds.BoundsImage;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderText) == BetterListViewHitTestLocations.ColumnHeaderText && betterListViewToolTipInfoCollection.TryGetToolTipInfo(BetterListViewToolTipLocation.Text, out toolTipInfo)) {
                        bounds = betterListViewColumnHeaderBounds.BoundsText;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) == BetterListViewHitTestLocations.ColumnHeaderBorder && betterListViewToolTipInfoCollection.TryGetToolTipInfo(BetterListViewToolTipLocation.Border, out toolTipInfo)) {
                        bounds = betterListViewColumnHeaderBounds.BoundsInner;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderSortGlyph) == BetterListViewHitTestLocations.ColumnHeaderSortGlyph && betterListViewToolTipInfoCollection.TryGetToolTipInfo(BetterListViewToolTipLocation.SortGlyph, out toolTipInfo)) {
                        bounds = betterListViewColumnHeaderBounds.BoundsSortGlyph;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ColumnHeaderBorder) != BetterListViewHitTestLocations.ColumnHeaderBorder && betterListViewToolTipInfoCollection.TryGetToolTipInfo(BetterListViewToolTipLocation.Client, out toolTipInfo)) {
                        bounds = betterListViewColumnHeaderBounds.BoundsOuter;
                    }
                }
                if (betterListViewColumnHeaderBounds.IsTextShrunk) {
                    flag = true;
                }
                else {
                    Rectangle boundsText = betterListViewColumnHeaderBounds.BoundsText;
                    Rectangle contentBounds = this.GetContentBounds(base.HScrollBarVisible, base.VScrollBarVisible);
                    flag = boundsText.Left < contentBounds.Left || boundsText.Right > contentBounds.Right || boundsText.Top < contentBounds.Top || boundsText.Bottom > contentBounds.Bottom;
                }
            }
            else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.SubItem) == BetterListViewHitTestLocations.SubItem && this.HitTestInfo.SubItem.Index != 0 && this.ShowToolTipsSubItems) {
                BetterListViewSubItem subItem = this.HitTestInfo.SubItem;
                BetterListViewItem item = subItem.Item;
                BetterListViewSubItemBounds betterListViewSubItemBounds = (BetterListViewSubItemBounds)((BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)item).LayoutBounds).SubItemBounds[subItem.Index].Clone();
                betterListViewSubItemBounds.Offset(this.OffsetContentFromAbsolute);
                BetterListViewToolTipInfoCollection betterListViewToolTipInfoCollection3 = (BetterListViewToolTipInfoCollection)subItem.ToolTips.Clone();
                if (this.AllowAutoToolTipsSubItems && !betterListViewToolTipInfoCollection3.ContainsLocation(BetterListViewToolTipLocation.Client)) {
                    betterListViewToolTipInfoCollection3.Add(new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Client, this.GetToolTipText(subItem.Text), showOnPartialTextVisibility: true));
                }
                foreach (BetterListViewToolTipInfo item3 in betterListViewToolTipInfoCollection3) {
                    if (item3.Location == BetterListViewToolTipLocation.Custom) {
                        Rectangle bounds3 = item3.Bounds;
                        bounds3.Offset(betterListViewSubItemBounds.BoundsInner.Location);
                        if (bounds3.Contains(pt)) {
                            bounds = item3.Bounds;
                            toolTipInfo = item3;
                            break;
                        }
                    }
                }
                if (toolTipInfo.IsEmpty) {
                    if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.SubItemImage) == BetterListViewHitTestLocations.SubItemImage && betterListViewToolTipInfoCollection3.TryGetToolTipInfo(BetterListViewToolTipLocation.Image, out toolTipInfo)) {
                        bounds = betterListViewSubItemBounds.BoundsImage;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.SubItemText) == BetterListViewHitTestLocations.SubItemText && betterListViewToolTipInfoCollection3.TryGetToolTipInfo(BetterListViewToolTipLocation.Text, out toolTipInfo)) {
                        bounds = betterListViewSubItemBounds.BoundsText;
                    }
                    else if (betterListViewToolTipInfoCollection3.TryGetToolTipInfo(BetterListViewToolTipLocation.Client, out toolTipInfo)) {
                        bounds = betterListViewSubItemBounds.BoundsOuter;
                    }
                }
                if (betterListViewSubItemBounds.IsTextShrunk) {
                    flag = true;
                }
                else {
                    Rectangle boundsText2 = betterListViewSubItemBounds.BoundsText;
                    Rectangle contentBounds2 = this.GetContentBounds(base.HScrollBarVisible, base.VScrollBarVisible);
                    flag = boundsText2.Left < contentBounds2.Left || boundsText2.Right > contentBounds2.Right || boundsText2.Top < contentBounds2.Top || boundsText2.Bottom > contentBounds2.Bottom;
                }
            }
            else if (((flag2 && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.SubItem) == BetterListViewHitTestLocations.SubItem && this.HitTestInfo.SubItem.Index == 0) || (!flag2 && (this.HitTestInfo.Locations & BetterListViewHitTestLocations.Item) == BetterListViewHitTestLocations.Item)) && this.ShowToolTips) {
                BetterListViewItem itemDisplay = this.HitTestInfo.ItemDisplay;
                BetterListViewItemBounds betterListViewItemBounds = (BetterListViewItemBounds)((IBetterListViewLayoutElementDisplayable)itemDisplay).LayoutBounds.Clone();
                betterListViewItemBounds.Offset(this.OffsetContentFromAbsolute);
                BetterListViewToolTipInfoCollection betterListViewToolTipInfoCollection4 = (BetterListViewToolTipInfoCollection)itemDisplay.ToolTips.Clone();
                if (this.AllowAutoToolTips && !betterListViewToolTipInfoCollection4.ContainsLocation(BetterListViewToolTipLocation.Client)) {
                    betterListViewToolTipInfoCollection4.Add(new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Client, this.GetToolTipText(itemDisplay.Text), showOnPartialTextVisibility: true));
                }
                foreach (BetterListViewToolTipInfo item4 in betterListViewToolTipInfoCollection4) {
                    if (item4.Location == BetterListViewToolTipLocation.Custom) {
                        Rectangle bounds4 = item4.Bounds;
                        bounds4.Offset(betterListViewItemBounds.BoundsInner.Location);
                        if (bounds4.Contains(pt)) {
                            bounds = item4.Bounds;
                            toolTipInfo = item4;
                            break;
                        }
                    }
                }
                if (toolTipInfo.IsEmpty) {
                    if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemCheckBox) == BetterListViewHitTestLocations.ItemCheckBox && betterListViewToolTipInfoCollection4.TryGetToolTipInfo(BetterListViewToolTipLocation.CheckBox, out toolTipInfo)) {
                        bounds = betterListViewItemBounds.BoundsCheckBox;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemImage) == BetterListViewHitTestLocations.ItemImage && betterListViewToolTipInfoCollection4.TryGetToolTipInfo(BetterListViewToolTipLocation.Image, out toolTipInfo)) {
                        bounds = betterListViewItemBounds.SubItemBounds[0].BoundsImage;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ItemText) == BetterListViewHitTestLocations.ItemText && betterListViewToolTipInfoCollection4.TryGetToolTipInfo(BetterListViewToolTipLocation.Text, out toolTipInfo)) {
                        bounds = betterListViewItemBounds.SubItemBounds[0].BoundsText;
                    }
                    else if (betterListViewToolTipInfoCollection4.TryGetToolTipInfo(BetterListViewToolTipLocation.Client, out toolTipInfo)) {
                        bounds = betterListViewItemBounds.BoundsOuter;
                    }
                }
                if (betterListViewItemBounds.SubItemBounds[0].IsTextShrunk) {
                    flag = true;
                }
                else {
                    Rectangle boundsText3 = betterListViewItemBounds.SubItemBounds[0].BoundsText;
                    Rectangle contentBounds3 = this.GetContentBounds(base.HScrollBarVisible, base.VScrollBarVisible);
                    flag = boundsText3.Left < contentBounds3.Left || boundsText3.Right > contentBounds3.Right || boundsText3.Top < contentBounds3.Top || boundsText3.Bottom > contentBounds3.Bottom;
                }
            }
            else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.Group) == BetterListViewHitTestLocations.Group && this.ShowToolTipsGroups) {
                BetterListViewGroup group = this.HitTestInfo.Group;
                BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)((IBetterListViewLayoutElementDisplayable)group).LayoutBounds.Clone();
                betterListViewGroupBounds.Offset(this.OffsetContentFromAbsolute);
                BetterListViewToolTipInfoCollection betterListViewToolTipInfoCollection2 = (BetterListViewToolTipInfoCollection)group.ToolTips.Clone();
                if (this.AllowAutoToolTipsGroups && !betterListViewToolTipInfoCollection2.ContainsLocation(BetterListViewToolTipLocation.Client)) {
                    betterListViewToolTipInfoCollection2.Add(new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Client, this.GetToolTipText(group.Header), showOnPartialTextVisibility: true));
                }
                foreach (BetterListViewToolTipInfo item5 in betterListViewToolTipInfoCollection2) {
                    if (item5.Location == BetterListViewToolTipLocation.Custom) {
                        Rectangle bounds5 = item5.Bounds;
                        bounds5.Offset(betterListViewGroupBounds.BoundsInner.Location);
                        if (bounds5.Contains(pt)) {
                            bounds = item5.Bounds;
                            toolTipInfo = item5;
                            break;
                        }
                    }
                }
                if (toolTipInfo.IsEmpty) {
                    if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.GroupImage) == BetterListViewHitTestLocations.GroupImage && betterListViewToolTipInfoCollection2.TryGetToolTipInfo(BetterListViewToolTipLocation.Image, out toolTipInfo)) {
                        bounds = betterListViewGroupBounds.BoundsImage;
                    }
                    else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.GroupText) == BetterListViewHitTestLocations.GroupText && betterListViewToolTipInfoCollection2.TryGetToolTipInfo(BetterListViewToolTipLocation.Text, out toolTipInfo)) {
                        bounds = betterListViewGroupBounds.BoundsText;
                    }
                    else if (betterListViewToolTipInfoCollection2.TryGetToolTipInfo(BetterListViewToolTipLocation.Client, out toolTipInfo)) {
                        bounds = betterListViewGroupBounds.BoundsOuter;
                    }
                }
                if (betterListViewGroupBounds.IsTextShrunk) {
                    flag = true;
                }
                else {
                    Rectangle boundsText4 = betterListViewGroupBounds.BoundsText;
                    Rectangle contentBounds4 = this.GetContentBounds(base.HScrollBarVisible, base.VScrollBarVisible);
                    flag = boundsText4.Left < contentBounds4.Left || boundsText4.Right > contentBounds4.Right || boundsText4.Top < contentBounds4.Top || boundsText4.Bottom > contentBounds4.Bottom;
                }
            }
            else if ((this.HitTestInfo.Locations & BetterListViewHitTestLocations.ContentArea) == BetterListViewHitTestLocations.ContentArea && !this.ToolTipInfo.IsEmpty) {
                Rectangle clientRectangleInner = base.ClientRectangleInner;
                if (this.ToolTipInfo.Location == BetterListViewToolTipLocation.Custom) {
                    BetterListViewToolTipInfo toolTipInfo2 = this.ToolTipInfo;
                    Rectangle rectangle = new Rectangle(clientRectangleInner.Left + toolTipInfo2.Bounds.Left, clientRectangleInner.Top + toolTipInfo2.Bounds.Top, toolTipInfo2.Bounds.Width, toolTipInfo2.Bounds.Height);
                    if (rectangle.Contains(pt)) {
                        bounds = toolTipInfo2.Bounds;
                        toolTipInfo = toolTipInfo2;
                    }
                }
                if (toolTipInfo.IsEmpty) {
                    bounds = clientRectangleInner;
                    toolTipInfo = this.ToolTipInfo;
                }
                flag = false;
            }
            BetterListViewToolTipTarget betterListViewToolTipTarget = new BetterListViewToolTipTarget(element, bounds);
            BetterListViewToolTipTarget betterListViewToolTipLocationB = (BetterListViewToolTipTarget)this.toolTip.Tag;
            if (betterListViewToolTipTarget != betterListViewToolTipLocationB) {
                this.HideToolTip();
            }
            if (betterListViewToolTipLocationB.IsEmpty && !toolTipInfo.IsEmpty && (!toolTipInfo.ShowOnPartialTextVisibility || flag)) {
                this.HideToolTip();
                this.InitializeToolTip();
                this.toolTip.Tag = betterListViewToolTipTarget;
                this.toolTip.BackColor = (toolTipInfo.ToolTipBackColor.IsEmpty ? BetterListViewToolTipInfo.DefaultToolTipBackColor : toolTipInfo.ToolTipBackColor);
                this.toolTip.ForeColor = (toolTipInfo.ToolTipForeColor.IsEmpty ? BetterListViewToolTipInfo.DefaultToolTipForeColor : toolTipInfo.ToolTipForeColor);
                this.toolTip.IsBalloon = toolTipInfo.ToolTipIsBalloon;
                this.toolTip.OwnerDraw = toolTipInfo.ToolTipOwnerDraw;
                this.toolTip.StripAmpersands = toolTipInfo.ToolTipStripAmpersands;
                this.toolTip.ToolTipIcon = toolTipInfo.ToolTipIcon;
                this.toolTip.ToolTipTitle = toolTipInfo.ToolTipTitle;
                this.toolTip.SetToolTip(this, toolTipInfo.Text);
            }
        }

        private void ToolTipsMouseLeave() {
            this.HideToolTip();
        }

        private void InitializeToolTip() {
            this.toolTip.AutomaticDelay = this.ToolTipOptions.AutomaticDelay;
            this.toolTip.AutoPopDelay = this.ToolTipOptions.AutoPopDelay;
            this.toolTip.InitialDelay = this.ToolTipOptions.InitialDelay;
            this.toolTip.ReshowDelay = this.ToolTipOptions.ReshowDelay;
            this.toolTip.ShowAlways = this.ToolTipOptions.ShowAlways;
            this.toolTip.UseAnimation = this.ToolTipOptions.UseAnimation;
            this.toolTip.UseFading = this.ToolTipOptions.UseFading;
        }

        private void HideToolTip() {
            this.toolTip.RemoveAll();
            this.toolTip.Tag = BetterListViewToolTipTarget.Empty;
        }

        private void ToolTipsDispose() {
            this.toolTip.Draw -= ToolTipOnDraw;
            this.toolTip.Popup -= ToolTipOnPopup;
            this.toolTip.Dispose();
        }

        private void ToolTipOnDraw(object obj, DrawToolTipEventArgs drawToolTipEventArgs) {
        }

        private void ToolTipOnPopup(object obj, PopupEventArgs popupEventArgs) {
        }

        private string GetToolTipText(string sourceText) {
            if (sourceText.Length < this.MaximumToolTipTextLength) {
                return sourceText;
            }
            return sourceText.Substring(0, this.MaximumToolTipTextLength - 1) + "…";
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListView" /> class.
        /// </summary>
        public BetterListView() {
            try {
                base.BeginUpdate();
                this.columns = new BetterListViewColumnHeaderCollection(isInternal: true);
                this.groups = new BetterListViewGroupCollection(isInternal: true);
                this.items = new BetterListViewItemCollection(isInternal: true);
                this.columns.OwnerControl = this;
                this.groups.OwnerControl = this;
                this.items.OwnerControl = this;
                base.CausesValidation = false;
                this.DragDropInitialize();
                this.GroupsInitialize();
                this.LayoutInitialize();
                this.LabelEditInitialize();
                this.SelectionsInitialize();
                this.ToolTipsInitialize();
                EventHandler value = ScrollBarOnGotFocus;
                base.HScrollBar.GotFocus += value;
                base.VScrollBar.GotFocus += value;
            }
            finally {
                base.EndUpdate(suppressRefresh: true);
            }
        }

        /// <summary>
        ///   Check whether this control contains the specified element.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <returns>This control contains the specified element.</returns>
        public bool Contains(BetterListViewElementBase element) {
            if (element == null || element.OwnerCollection == null) {
                return false;
            }
            return element.OwnerCollection.OwnerControl == this;
        }

        /// <summary>
        ///   Handles the situation where some collection has changed.
        /// </summary>
        /// <param name="collection">Collection that has changed.</param>
        /// <param name="changeInfo">Information about changes made to the collection.</param>
        internal override void OnCollectionChanged(BetterListViewElementCollectionBase collection, BetterListViewElementCollectionChangeInfo changeInfo) {
            if (base.InvokeRequired) {
                base.BeginInvoke(new BetterListViewElementCollectionChangedDelegate(OnCollectionChanged), collection, changeInfo);
                return;
            }
            Checks.CheckNotNull(collection, "collection");
            Checks.CheckNotNull(changeInfo, "changeInfo");
            BetterListViewFocusInfo empty = this.FocusInfo;
            BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
            try {
                base.BeginUpdate();
                BetterListViewColumnHeaderCollection betterListViewColumnHeaderCollection = collection as BetterListViewColumnHeaderCollection;
                if (betterListViewColumnHeaderCollection != null) {
                    this.sortList.Clear();
                    this.DataUpdateContentFromSource(ListChangedType.PropertyDescriptorChanged, -1);
                    foreach (BetterListViewItem current in this) {
                        ((IBetterListViewStateElement)current).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                BetterListViewGroupCollection betterListViewGroupCollection = collection as BetterListViewGroupCollection;
                if (betterListViewGroupCollection != null) {
                    if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove) {
                        BetterListViewItemSet betterListViewItemSet = new BetterListViewItemSet(this.SelectionInfo.SelectedItems.Keys);
                        foreach (KeyValuePair<BetterListViewElementBase, int> element2 in changeInfo.Elements) {
                            BetterListViewGroup betterListViewGroup = (BetterListViewGroup)element2.Key;
                            betterListViewItemSet.RemoveRange(betterListViewGroup.GetItems(this));
                        }
                        selectionInfoNew.SetSelectedItems(this.GetSelectedGroups(betterListViewItemSet), betterListViewItemSet);
                    }
                    this.checkedIndices.ClearCache();
                    this.checkedItems.ClearCache();
                    selectionInfoNew.LastSelectedElement = null;
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                }
                BetterListViewItemCollection betterListViewItemCollection = collection as BetterListViewItemCollection;
                if (betterListViewItemCollection != null) {
                    List<BetterListViewElementBase> list = new List<BetterListViewElementBase>();
                    List<BetterListViewElementBase> list2 = new List<BetterListViewElementBase>();
                    if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add) {
                        list.AddRange(changeInfo.Elements.Keys);
                    }
                    if ((changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add || changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Set) && betterListViewItemCollection.OwnerItem == null && this.sortList.Count != 0 && this.SortOnCollectionChange && !this.IsSortSuspended && !this.SortVirtual) {
                        this.SortItems(columnClicked: false, sortAlways: false);
                    }
                    if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Set) {
                        BetterListViewElementBase[] array = new BetterListViewElementBase[2];
                        changeInfo.Elements.Keys.CopyTo(array, 0);
                        list.Add(array[1]);
                        list2.Add(array[0]);
                    }
                    if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove) {
                        list2.AddRange(changeInfo.Elements.Keys);
                    }
                    if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove || changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Set) {
                        BetterListViewItemSet betterListViewItemSet2 = new BetterListViewItemSet(this.SelectionInfo.SelectedItems.Keys);
                        foreach (KeyValuePair<BetterListViewElementBase, int> element3 in changeInfo.Elements) {
                            BetterListViewItem item = (BetterListViewItem)element3.Key;
                            betterListViewItemSet2.Remove(item);
                        }
                        selectionInfoNew.SetSelectedItems(this.GetSelectedGroups(betterListViewItemSet2), betterListViewItemSet2);
                    }
                    if (list.Count != 0 && this.ShowItemExpandButtons && !this.enableExpandButtons) {
                        bool flag = false;
                        foreach (BetterListViewItem item2 in list) {
                            if (item2.ChildItems.Count != 0) {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) {
                            foreach (BetterListViewItem current2 in this) {
                                ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            }
                        }
                    }
                    if (list2.Count != 0 && this.ShowItemExpandButtons && this.enableExpandButtons) {
                        bool flag2 = false;
                        foreach (BetterListViewItem item3 in list2) {
                            if (item3.ChildItems.Count != 0) {
                                flag2 = true;
                                break;
                            }
                        }
                        if (flag2) {
                            foreach (BetterListViewItem current3 in this) {
                                ((IBetterListViewStateElement)current3).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            }
                        }
                    }
                    this.selectedIndices.ClearCache();
                    this.selectedItems.ClearCache();
                    this.checkedIndices.ClearCache();
                    this.checkedItems.ClearCache();
                    this.defaultGroup.ClearCachedItems();
                    foreach (BetterListViewGroup group in this.Groups) {
                        group.ClearCachedItems();
                    }
                    selectionInfoNew.LastSelectedElement = null;
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    if (!betterListViewItemCollection.IsSynchronizing && betterListViewItemCollection.OwnerItem == null) {
                        this.DataUpdateItemsToSource(changeInfo);
                    }
                }
                BetterListViewSubItemCollection betterListViewSubItemCollection = collection as BetterListViewSubItemCollection;
                if (betterListViewSubItemCollection != null) {
                    bool flag3 = false;
                    foreach (BetterListViewSubItem item4 in betterListViewSubItemCollection) {
                        if (this.sortList.Contains(item4.Index)) {
                            flag3 = true;
                            break;
                        }
                    }
                    ((IBetterListViewStateElement)betterListViewSubItemCollection.OwnerItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    if (flag3 && this.SortOnCollectionChange && !this.IsSortSuspended) {
                        this.SortItems(columnClicked: false, sortAlways: false);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    if (betterListViewSubItemCollection.OwnerItem.Level == 0) {
                        for (int i = 0; i < Math.Min(betterListViewSubItemCollection.Count, this.Columns.Count); i++) {
                            this.DataUpdateSubItemToSource(betterListViewSubItemCollection[i]);
                        }
                    }
                }
                if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Add) {
                    if (betterListViewItemCollection != null) {
                        if (betterListViewItemCollection.OwnerItem != null) {
                            ((IBetterListViewStateElement)betterListViewItemCollection.OwnerItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        foreach (BetterListViewItem key in changeInfo.Elements.Keys) {
                            ((IBetterListViewStateElement)key).ChangeState(BetterListViewElementStateChange.ResetElement);
                            key.ClearCache();
                        }
                    }
                    else if (betterListViewSubItemCollection != null) {
                        foreach (BetterListViewSubItem item5 in betterListViewSubItemCollection) {
                            item5.ClearCache();
                        }
                    }
                    else if (betterListViewColumnHeaderCollection != null || betterListViewGroupCollection != null) {
                        foreach (BetterListViewElement key2 in changeInfo.Elements.Keys) {
                            ((IBetterListViewStateElement)key2).ChangeState(BetterListViewElementStateChange.ResetElement);
                        }
                    }
                }
                else if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Remove) {
                    if (betterListViewItemCollection != null && betterListViewItemCollection.OwnerItem != null) {
                        ((IBetterListViewStateElement)betterListViewItemCollection.OwnerItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                }
                else if (changeInfo.ChangeType == BetterListViewElementCollectionChangeType.Set) {
                    if (betterListViewItemCollection != null) {
                        foreach (BetterListViewItem key3 in changeInfo.Elements.Keys) {
                            if (key3.ListView == this) {
                                ((IBetterListViewStateElement)key3).ChangeState(BetterListViewElementStateChange.ResetElement);
                            }
                        }
                    }
                    else if (betterListViewColumnHeaderCollection != null || betterListViewGroupCollection != null) {
                        foreach (BetterListViewElement key4 in changeInfo.Elements.Keys) {
                            if (key4.OwnerCollection.OwnerControl == this) {
                                ((IBetterListViewStateElement)key4).ChangeState(BetterListViewElementStateChange.ResetElement);
                            }
                        }
                    }
                }
                if (!empty.IsEmpty) {
                    if (empty.ColumnIndex == -1 || empty.ColumnIndex == 0) {
                        BetterListViewElementBase element = empty.Element;
                        if (element is BetterListViewGroup betterListViewGroup2 && betterListViewGroup2.ListView != this) {
                            empty = BetterListViewFocusInfo.Empty;
                        }
                        if (element is BetterListViewItem betterListViewItem5 && betterListViewItem5.ListView != this) {
                            empty = BetterListViewFocusInfo.Empty;
                        }
                    }
                    else {
                        BetterListViewItem betterListViewItem6 = (BetterListViewItem)empty.Element;
                        if (empty.ColumnIndex >= betterListViewItem6.SubItems.Count || betterListViewItem6.SubItems[empty.ColumnIndex].ListView != this) {
                            empty = BetterListViewFocusInfo.Empty;
                        }
                    }
                }
                this.SetHitTestInfo(BetterListViewHitTestInfo.Empty);
                this.SetFocusInfo(empty, BetterListViewSelectionChangeMode.UserCode);
                this.SetSelectionInfo(selectionInfoNew, BetterListViewSelectionChangeMode.UserCode);
                base.AccessibilityNotifyClients(AccessibleEvents.Reorder, 0);
            }
            finally {
                base.EndUpdate();
            }
        }

        /// <summary>
        ///   Called when property of some collection item has changed.
        /// </summary>
        /// <param name="collection">collection containing the element</param>
        /// <param name="elementPropertyType">element property type</param>
        /// <param name="element">element whose property has changed</param>
        /// <param name="oldValue">value of the property before the property was set</param>
        /// <param name="eventArgs">event data (in case of property change event should be fired)</param>
        internal override void OnElementPropertyChanged(BetterListViewElementCollectionBase collection, BetterListViewElementPropertyType elementPropertyType, BetterListViewElementBase element, object oldValue, EventArgs eventArgs) {
            if (base.InvokeRequired) {
                base.BeginInvoke(new BetterListViewElementPropertyChangedDelegate(OnElementPropertyChanged), collection, elementPropertyType, element, oldValue, eventArgs);
                return;
            }
            if (element != this.defaultGroup) {
                Checks.CheckNotNull(collection, "collection");
            }
            BetterListViewFocusInfo focusInfoNew = this.FocusInfo;
            BetterListViewSelectionInfo selectionInfoNew = this.SelectionInfo;
            BetterListViewSelectionChangeMode betterListViewSelectionChangeMode = BetterListViewSelectionChangeMode.Undefined;
            BetterListViewColumnHeader betterListViewColumnHeader = element as BetterListViewColumnHeader;
            BetterListViewItem betterListViewItem = element as BetterListViewItem;
            BetterListViewSubItem betterListViewSubItem = element as BetterListViewSubItem;
            BetterListViewGroup betterListViewGroup = element as BetterListViewGroup;
            switch (elementPropertyType) {
                case BetterListViewElementPropertyType.Appearance:
                    if (betterListViewColumnHeader != null && ((IBetterListViewStateElement)betterListViewColumnHeader).State == BetterListViewElementState.ActiveVisible) {
                        betterListViewColumnHeader.Invalidate();
                    }
                    else if (betterListViewItem != null && ((IBetterListViewStateElement)betterListViewItem).State == BetterListViewElementState.ActiveVisible) {
                        betterListViewItem.Invalidate();
                    }
                    else if (betterListViewSubItem != null && ((IBetterListViewStateElement)betterListViewSubItem.Item).State == BetterListViewElementState.ActiveVisible) {
                        betterListViewSubItem.Item.Invalidate();
                    }
                    else if (betterListViewGroup != null && ((IBetterListViewStateElement)betterListViewGroup).State == BetterListViewElementState.ActiveVisible) {
                        betterListViewGroup.Invalidate();
                    }
                    else {
                        base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    }
                    break;
                case BetterListViewElementPropertyType.Check:
                    this.checkedIndices.ClearCache();
                    this.checkedItems.ClearCache();
                    if (((IBetterListViewStateElement)betterListViewItem).State == BetterListViewElementState.ActiveVisible) {
                        betterListViewItem.Invalidate();
                    }
                    this.OnItemChecked((BetterListViewItemCheckedEventArgs)eventArgs);
                    break;
                case BetterListViewElementPropertyType.Collapse: {
                        BetterListViewItem betterListViewItem2 = ((!focusInfoNew.IsEmpty) ? (focusInfoNew.Element as BetterListViewItem) : null);
                        if (betterListViewItem != null) {
                            ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        else if (betterListViewGroup != null && betterListViewItem2 != null) {
                            foreach (BetterListViewItem item in betterListViewGroup) {
                                if (item == betterListViewItem2) {
                                    focusInfoNew = new BetterListViewFocusInfo(betterListViewGroup);
                                    break;
                                }
                            }
                        }
                        betterListViewSelectionChangeMode = BetterListViewSelectionChangeMode.Collapse;
                        base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                        break;
                    }
                case BetterListViewElementPropertyType.DataBinding:
                case BetterListViewElementPropertyType.Sorting:
                    if (betterListViewColumnHeader != null) {
                        if (!this.IsSortSuspended) {
                            this.SortItems(columnClicked: false, sortAlways: false);
                        }
                    }
                    else {
                        int num2;
                        int num3;
                        if (betterListViewItem != null) {
                            num2 = 0;
                            num3 = betterListViewItem.Level;
                            ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        else if (betterListViewSubItem != null) {
                            num2 = betterListViewSubItem.Index;
                            num3 = betterListViewSubItem.Item.Level;
                            ((IBetterListViewStateElement)betterListViewSubItem.Item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        }
                        else {
                            num2 = -1;
                            num3 = 0;
                        }
                        if (num2 != -1 && num3 == 0 && this.sortList.Contains(num2) && !this.IsSortSuspended) {
                            this.SortItems(columnClicked: false, sortAlways: false);
                        }
                        else {
                            base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                        }
                    }
                    if (elementPropertyType == BetterListViewElementPropertyType.DataBinding && betterListViewSubItem != null && betterListViewSubItem.Item.Level == 0 && betterListViewSubItem.Index < this.Columns.Count) {
                        this.DataUpdateSubItemToSource(betterListViewSubItem);
                    }
                    break;
                case BetterListViewElementPropertyType.Grouping:
                    this.defaultGroup.ClearCachedItems();
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.Selectability:
                    ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.Selection:
                    base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.Layout:
                    if (betterListViewColumnHeader != null) {
                        ((IBetterListViewStateElement)betterListViewColumnHeader).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        if (this.View == BetterListViewView.Details) {
                            foreach (BetterListViewItem current2 in this) {
                                ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            }
                        }
                    }
                    else if (betterListViewItem != null) {
                        ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    else if (betterListViewSubItem != null) {
                        ((IBetterListViewStateElement)betterListViewSubItem.Item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    else {
                        ((IBetterListViewStateElement)betterListViewGroup)?.ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.LayoutImage: {
                        int num;
                        if (betterListViewSubItem != null) {
                            num = betterListViewSubItem.DisplayIndex;
                            if (num == -1) {
                                return;
                            }
                        }
                        else {
                            num = -1;
                        }
                        ImageList imageListCurrent = this.ImageListCurrent;
                        Image image;
                        if (oldValue == null) {
                            image = null;
                        }
                        else {
                            int imageIndex;
                            string imageKey;
                            if (oldValue is Image) {
                                image = (Image)oldValue;
                                imageIndex = -1;
                                imageKey = string.Empty;
                            }
                            else if (oldValue is int) {
                                image = null;
                                imageIndex = (int)oldValue;
                                imageKey = string.Empty;
                            }
                            else {
                                if (!(oldValue is string)) {
                                    throw new ApplicationException("Unknown value type '" + oldValue.GetType().FullName + "'.");
                                }
                                image = null;
                                imageIndex = -1;
                                imageKey = (string)oldValue;
                            }
                            image = BetterListViewBasePainter.GetElementImage(image, imageIndex, imageKey, imageListCurrent);
                        }
                        Size sz = image?.Size ?? Size.Empty;
                        Size size = BetterListViewBasePainter.GetElementImage(element, imageListCurrent)?.Size ?? Size.Empty;
                        bool autoSizeImages;
                        BetterListViewImageSize betterListViewImageSize;
                        IBetterListViewStateElement betterListViewStateElement;
                        IBetterListViewLayoutElementDisplayable betterListViewLayoutElementDisplayable;
                        bool flag;
                        if (betterListViewItem != null) {
                            BetterListViewLayoutItems layoutItemsCurrent = this.LayoutItemsCurrent;
                            autoSizeImages = layoutItemsCurrent.AutoSizeImages;
                            betterListViewImageSize = layoutItemsCurrent.ImageSize;
                            betterListViewStateElement = betterListViewItem;
                            betterListViewLayoutElementDisplayable = betterListViewItem;
                            flag = ((BetterListViewItemBounds)betterListViewLayoutElementDisplayable.LayoutBounds).SubItemBounds[0].BoundsImageFrame.IsEmpty;
                        }
                        else if (betterListViewSubItem != null) {
                            BetterListViewLayoutItems layoutItemsCurrent2 = this.LayoutItemsCurrent;
                            autoSizeImages = layoutItemsCurrent2.AutoSizeImages;
                            betterListViewImageSize = layoutItemsCurrent2.GetSubItemImageSize(betterListViewSubItem.Index);
                            betterListViewStateElement = betterListViewSubItem.Item;
                            betterListViewLayoutElementDisplayable = betterListViewSubItem.Item;
                            ReadOnlyCollection<BetterListViewSubItemBounds> subItemBounds = ((BetterListViewItemBounds)betterListViewLayoutElementDisplayable.LayoutBounds).SubItemBounds;
                            flag = num >= subItemBounds.Count || subItemBounds[num].BoundsImageFrame.IsEmpty;
                        }
                        else if (betterListViewColumnHeader != null) {
                            BetterListViewLayoutColumns betterListViewLayoutColumns = this.LayoutColumns;
                            autoSizeImages = betterListViewLayoutColumns.AutoSizeImages;
                            betterListViewImageSize = betterListViewLayoutColumns.ImageSize;
                            betterListViewStateElement = betterListViewColumnHeader;
                            betterListViewLayoutElementDisplayable = betterListViewColumnHeader;
                            flag = ((BetterListViewColumnHeaderBounds)betterListViewLayoutElementDisplayable.LayoutBounds).BoundsImageFrame.IsEmpty;
                        }
                        else {
                            if (betterListViewGroup == null) {
                                throw new ApplicationException("Unknown element type: '" + element.GetType().FullName + "'.");
                            }
                            BetterListViewLayoutGroups layoutGroupsCurrent = this.LayoutGroupsCurrent;
                            autoSizeImages = layoutGroupsCurrent.AutoSizeImages;
                            betterListViewImageSize = layoutGroupsCurrent.ImageSize;
                            betterListViewStateElement = betterListViewGroup;
                            betterListViewLayoutElementDisplayable = betterListViewGroup;
                            flag = ((BetterListViewGroupBounds)betterListViewLayoutElementDisplayable.LayoutBounds).BoundsImageFrame.IsEmpty;
                        }
                        if (sz == size) {
                            if (betterListViewStateElement.State == BetterListViewElementState.ActiveVisible) {
                                Rectangle boundsSpacing = betterListViewLayoutElementDisplayable.LayoutBounds.BoundsSpacing;
                                boundsSpacing.Offset(this.OffsetContentFromAbsolute);
                                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, boundsSpacing);
                            }
                        }
                        else if (autoSizeImages || !betterListViewImageSize.IsFixed || flag) {
                            betterListViewStateElement.ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                            base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                        }
                        else if (betterListViewStateElement.State == BetterListViewElementState.ActiveFine || betterListViewStateElement.State == BetterListViewElementState.ActiveVisible) {
                            if (betterListViewItem != null) {
                                BetterListViewSubItemBounds betterListViewSubItemBounds = ((BetterListViewItemBounds)betterListViewLayoutElementDisplayable.LayoutBounds).SubItemBounds[0];
                                betterListViewSubItemBounds.BoundsImage = BetterListViewImageSize.GetImageBounds(betterListViewSubItemBounds.BoundsImageFrame, size, betterListViewItem.AlignHorizontalImage, betterListViewItem.AlignVerticalImage);
                            }
                            else if (betterListViewSubItem != null) {
                                BetterListViewSubItemBounds betterListViewSubItemBounds2 = ((BetterListViewItemBounds)betterListViewLayoutElementDisplayable.LayoutBounds).SubItemBounds[num];
                                betterListViewSubItemBounds2.BoundsImage = BetterListViewImageSize.GetImageBounds(betterListViewSubItemBounds2.BoundsImageFrame, size, betterListViewSubItem.AlignHorizontalImage, betterListViewSubItem.AlignVerticalImage);
                            }
                            else if (betterListViewColumnHeader != null) {
                                BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = (BetterListViewColumnHeaderBounds)betterListViewLayoutElementDisplayable.LayoutBounds;
                                betterListViewColumnHeaderBounds.BoundsImage = BetterListViewImageSize.GetImageBounds(betterListViewColumnHeaderBounds.BoundsImageFrame, size, betterListViewColumnHeader.AlignHorizontalImage, betterListViewColumnHeader.AlignVerticalImage);
                            }
                            else {
                                BetterListViewGroupBounds betterListViewGroupBounds = (BetterListViewGroupBounds)betterListViewLayoutElementDisplayable.LayoutBounds;
                                betterListViewGroupBounds.BoundsImage = BetterListViewImageSize.GetImageBounds(betterListViewGroupBounds.BoundsImageFrame, size, betterListViewGroup.HeaderAlignmentHorizontalImage, betterListViewGroup.HeaderAlignmentVerticalImage);
                            }
                            if (betterListViewStateElement.State == BetterListViewElementState.ActiveVisible) {
                                Rectangle boundsSpacing2 = betterListViewLayoutElementDisplayable.LayoutBounds.BoundsSpacing;
                                boundsSpacing2.Offset(this.OffsetContentFromAbsolute);
                                base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, boundsSpacing2);
                            }
                        }
                        break;
                    }
                case BetterListViewElementPropertyType.LayoutText:
                    if (betterListViewColumnHeader != null) {
                        ((IBetterListViewStateElement)betterListViewColumnHeader).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    else if (betterListViewItem != null) {
                        ((IBetterListViewStateElement)betterListViewItem).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    else if (betterListViewSubItem != null) {
                        ((IBetterListViewStateElement)betterListViewSubItem.Item).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    else {
                        if (betterListViewGroup == null) {
                            throw new ApplicationException("Unknown element type: '" + element.GetType().FullName + "'.");
                        }
                        ((IBetterListViewStateElement)betterListViewGroup).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    }
                    base.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.LayoutSetup:
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                case BetterListViewElementPropertyType.Expand:
                    ((IBetterListViewStateElement)betterListViewItem)?.ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    base.Invalidate(BetterListViewInvalidationLevel.Setup, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
                    break;
                default:
                    throw new ApplicationException($"Unknown item property type: '{elementPropertyType}'.");
            }
            if (betterListViewColumnHeader != null && eventArgs is BetterListViewColumnWidthChangedEventArgs betterListViewColumnWidthChangedEventArgs) {
                this.OnColumnWidthChanged(betterListViewColumnWidthChangedEventArgs);
            }
            if (betterListViewSelectionChangeMode != BetterListViewSelectionChangeMode.Undefined) {
                this.SetFocusInfo(focusInfoNew, betterListViewSelectionChangeMode);
                this.SetSelectionInfo(selectionInfoNew, betterListViewSelectionChangeMode);
            }
            this.RefreshView();
        }

        /// <summary>
        ///   Called when layout property has changed.
        /// </summary>
        /// <param name="layout">Layout whose property has changed.</param>
        /// <param name="layoutPropertyType">Update type of the layout property.</param>
        internal override void OnLayoutPropertyChanged(BetterListViewLayoutBase layout, BetterListViewLayoutPropertyType layoutPropertyType) {
            Checks.CheckNotNull(layout, "layout");
            BetterListViewLayoutColumns betterListViewLayoutColumns = layout as BetterListViewLayoutColumns;
            BetterListViewLayoutItems betterListViewLayoutItems = layout as BetterListViewLayoutItems;
            BetterListViewLayoutGroups betterListViewLayoutGroups = layout as BetterListViewLayoutGroups;
            if (layoutPropertyType == BetterListViewLayoutPropertyType.Setup || layoutPropertyType == BetterListViewLayoutPropertyType.Text) {
                if (betterListViewLayoutColumns != null) {
                    foreach (BetterListViewColumnHeader column in this.Columns) {
                        ((IBetterListViewStateElement)column).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        column.ClearCache();
                    }
                }
                else if (betterListViewLayoutItems != null) {
                    foreach (BetterListViewItem current2 in this) {
                        ((IBetterListViewStateElement)current2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        current2.ClearCache();
                    }
                }
                else if (betterListViewLayoutGroups != null) {
                    ((IBetterListViewStateElement)this.defaultGroup).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                    this.defaultGroup.ClearCache();
                    foreach (BetterListViewGroup group in this.Groups) {
                        ((IBetterListViewStateElement)group).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                        group.ClearCache();
                    }
                }
            }
            if (betterListViewLayoutGroups != null) {
                foreach (BetterListViewGroup group2 in this.Groups) {
                    ((IBetterListViewStateElement)group2).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
                }
            }
            foreach (BetterListViewItem current3 in this) {
                ((IBetterListViewStateElement)current3).ChangeState(BetterListViewElementStateChange.ResetMeasurement);
            }
            base.OnLayoutPropertyChanged(layout, layoutPropertyType);
        }

        /// <summary>
        ///   Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control" /> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                try {
                    base.BeginUpdate();
                    this.SuspendSelectionChanged();
                    this.DataSource = null;
                    this.columns.Clear();
                    this.groups.Clear();
                    this.items.Clear();
                }
                finally {
                    this.ResumeSelectionChanged();
                    base.EndUpdate(suppressRefresh: true);
                }
                this.ToolTipsDispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
        }

        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.LostFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
        /// </param>
        protected override void OnLostFocus(EventArgs e) {
            BetterListViewStateInfo betterListViewStateInfo = this.StateInfo;
            if (betterListViewStateInfo.State == BetterListViewState.ItemDrag) {
                this.SetInsertionMark(BetterListViewInsertionMark.Empty);
            }
            if (betterListViewStateInfo.State == BetterListViewState.ItemSelection) {
                this.PerformDragSelectionEnd(betterListViewStateInfo.ItemSelectionStateInfo.SelectedItems);
            }
            betterListViewStateInfo.State = BetterListViewState.Normal;
            this.SetStateInfo(betterListViewStateInfo);
            base.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
            this.RefreshView();
            base.OnLostFocus(e);
        }

        private void ScrollBarOnGotFocus(object sender, EventArgs eventArgs) {
            this.LabelEditEnd(forced: true);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<BetterListViewItem> GetEnumerator() {
            return this.items.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        static BetterListView() {
            BetterListView.DefaultColorColumnResizeLine = Color.FromArgb(128, SystemColors.ControlDark);
            BetterListView.DefaultColorGridLines = SystemColors.Control;
            BetterListView.DefaultColorSortedColumn = Color.FromArgb(64, SystemColors.Control);
            int[] enumerable = new int[1];
            BetterListView.DefaultSearchSettings = new BetterListViewSearchSettings(BetterListViewSearchMode.PrefixOrSubstring, BetterListViewSearchOptions.FirstWordOnly | BetterListViewSearchOptions.PrefixPreference | BetterListViewSearchOptions.WordSearch, new ReadOnlySet<int>(enumerable));
        }
    }
}