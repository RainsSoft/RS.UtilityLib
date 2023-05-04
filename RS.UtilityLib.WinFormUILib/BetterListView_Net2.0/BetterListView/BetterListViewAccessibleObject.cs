using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Provides information about BetterListView that can be used by an accessibility application.
    /// </summary>
    internal sealed class BetterListViewAccessibleObject : Control.ControlAccessibleObject
    {
        /// <summary>
        ///   Default value for the DefaultAction property.
        /// </summary>
        public const string DefaultDefaultAction = "";

        /// <summary>
        ///   Default value for the Description property.
        /// </summary>
        public const string DefaultDescription = "List Control";

        /// <summary>
        ///   Default value for the Name property.
        /// </summary>
        public const string DefaultName = "BetterListView";

        /// <summary>
        ///   Default value for the Role property.
        /// </summary>
        public const AccessibleRole DefaultRole = AccessibleRole.List;

        private readonly BetterListView listView;

        private BetterListViewHeaderAccessibleObject cachedHeaderAccessibleObject;

        /// <summary>
        /// </summary>
        /// <returns>
        ///   A description of the default action for an object, or null if this object has no default action.
        /// </returns>
        public override string DefaultAction => "";

        /// <summary>
        ///   Gets the description of the <see cref="T:System.Windows.Forms.Control.ControlAccessibleObject" />.
        /// </summary>
        /// <returns>
        ///   A string describing the <see cref="T:System.Windows.Forms.Control.ControlAccessibleObject" />.
        /// </returns>
        public override string Description => "List Control";

        /// <summary>
        ///   Gets or sets the accessible object name.
        /// </summary>
        /// <returns>
        ///   The accessible object name.
        /// </returns>
        public override string Name {
            get {
                string accessibleName = base.Owner.AccessibleName;
                if (!string.IsNullOrEmpty(accessibleName)) {
                    return accessibleName;
                }
                return "BetterListView";
            }
            set {
                base.Name = value;
            }
        }

        /// <summary>
        ///   Gets the role of this accessible object.
        /// </summary>
        /// <returns>
        ///   One of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values.
        /// </returns>
        public override AccessibleRole Role {
            get {
                AccessibleRole accessibleRole = base.Owner.AccessibleRole;
                if (accessibleRole != AccessibleRole.Default) {
                    return accessibleRole;
                }
                return AccessibleRole.List;
            }
        }

        /// <summary>
        ///   Gets the state of this accessible object.
        /// </summary>
        /// <returns>
        ///   One of the <see cref="T:System.Windows.Forms.AccessibleStates" /> values, or <see cref="F:System.Windows.Forms.AccessibleStates.None" />, if no state has been set.
        /// </returns>
        public override AccessibleStates State {
            get {
                BetterListView betterListView = (BetterListView)base.Owner;
                AccessibleStates accessibleStates = AccessibleStates.Focusable | AccessibleStates.Selectable;
                if (betterListView.Focused) {
                    accessibleStates |= AccessibleStates.Focused;
                }
                if (betterListView.MultiSelect) {
                    accessibleStates |= AccessibleStates.MultiSelectable;
                }
                return accessibleStates;
            }
        }

        private BetterListViewHeaderAccessibleObject HeaderAccessibleObject {
            get {
                if (this.cachedHeaderAccessibleObject == null) {
                    this.cachedHeaderAccessibleObject = new BetterListViewHeaderAccessibleObject(this.listView);
                }
                return this.cachedHeaderAccessibleObject;
            }
        }

        /// <summary>
        ///   Initialize a new BetterListViewAccessibleObject instance.
        /// </summary>
        /// <param name="ownerControl">control for which the AccessibleObject is created</param>
        public BetterListViewAccessibleObject(BetterListView ownerControl)
            : base(ownerControl) {
            this.listView = ownerControl;
        }

        /// <summary>
        ///   Retrieves the accessible child corresponding to the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the accessible child.</param>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the accessible child corresponding to the specified index.
        /// </returns>
        public override AccessibleObject GetChild(int index) {
            if (this.GetChildCount() == 0) {
                return null;
            }
            if (this.listView.ColumnsVisible && this.listView.BoundsColumnHeaders.Height > 0) {
                if (index == 0) {
                    return this.HeaderAccessibleObject;
                }
                index--;
            }
            if (this.listView.LayoutElementsItems.Count != 0) {
                if (index >= 0 && index < this.listView.LayoutElementsItems.Count) {
                    return this.listView.LayoutElementsItems[index].GetAccessibilityInstance(this.listView, this);
                }
                index -= this.listView.LayoutElementsItems.Count;
            }
            if (this.listView.HScrollBarVisible) {
                if (index == 0) {
                    return this.listView.HScrollBar.AccessibilityObject;
                }
                index--;
            }
            if (this.listView.VScrollBarVisible) {
                if (index == 0) {
                    return this.listView.VScrollBar.AccessibilityObject;
                }
                index--;
            }
            return null;
        }

        /// <summary>
        ///   Retrieves the number of children belonging to an accessible object.
        /// </summary>
        /// <returns>
        ///   The number of children belonging to an accessible object.
        /// </returns>
        public override int GetChildCount() {
            int num = 0;
            if (this.listView.ColumnsVisible && this.listView.BoundsColumnHeaders.Height > 0) {
                num++;
            }
            num += this.listView.LayoutElementsItems.Count;
            if (this.listView.HScrollBarVisible) {
                num++;
            }
            if (this.listView.VScrollBarVisible) {
                num++;
            }
            return num;
        }

        /// <summary>
        ///   Retrieves the object that has the keyboard focus.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        ///   The control cannot be retrieved.
        /// </exception>
        public override AccessibleObject GetFocused() {
            if (!this.listView.Focused || this.listView.FocusedItem == null) {
                return null;
            }
            return this.listView.FocusedItem.GetAccessibilityInstance(this.listView, this);
        }

        /// <summary>
        ///   Retrieves the currently selected child.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the currently selected child. This method returns the calling object if the object itself is selected. Returns null if is no child is currently selected and the object itself does not have focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        ///   The selected child cannot be retrieved.
        /// </exception>
        public override AccessibleObject GetSelected() {
            if (!this.listView.Focused || this.listView.FocusedItem == null || !this.listView.FocusedItem.Selected) {
                return null;
            }
            return this.listView.FocusedItem.GetAccessibilityInstance(this.listView, this);
        }

        /// <summary>
        ///   Retrieves the child object at the specified screen coordinates.
        /// </summary>
        /// <param name="x">The horizontal screen coordinate.</param>
        /// <param name="y">The vertical screen coordinate.</param>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the child object at the given screen coordinates. This method returns the calling object if the object itself is at the location specified. Returns null if no object is at the tested location.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        ///   The control cannot be hit tested.
        /// </exception>
        public override AccessibleObject HitTest(int x, int y) {
            BetterListViewHitTestInfo betterListViewHitTestInfo = this.listView.HitTest(this.listView.PointToClient(new Point(x, y)));
            BetterListViewColumnHeader columnHeader = betterListViewHitTestInfo.ColumnHeader;
            if (columnHeader != null) {
                return betterListViewHitTestInfo.ColumnHeader.GetAccessibilityInstance(this.listView, this);
            }
            return betterListViewHitTestInfo.SubItem?.GetAccessibilityInstance(this.listView, betterListViewHitTestInfo.SubItem.Item.GetAccessibilityInstance(this.listView, this));
        }

        /// <summary>
        ///   Navigates to another accessible object.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation" /> values.</param>
        /// <returns>
        ///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation" /> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        ///   The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public override AccessibleObject Navigate(AccessibleNavigation navdir) {
            //return navdir switch {
            //	AccessibleNavigation.FirstChild => this.GetChild(0),
            //	AccessibleNavigation.LastChild => this.GetChild(this.GetChildCount() - 1),
            //	_ => null,
            //};
            AccessibleObject aobj = null;
            switch (navdir) {
                case AccessibleNavigation.FirstChild:
                    aobj = this.GetChild(0);
                    break;
                case AccessibleNavigation.LastChild:
                    aobj = this.GetChild(this.GetChildCount() - 1);
                    break;
                default: break;
            }
            return aobj;
        }

        /// <summary>
        ///   Get accessible object ID for the specified item.
        /// </summary>
        /// <param name="item">Item to get object ID for.</param>
        /// <returns>Accessible object ID.</returns>
        internal static int GetItemId(BetterListViewItem item) {
            BetterListView betterListView = item.ListView;
            int num = ((betterListView.ColumnsVisible && betterListView.BoundsColumnHeaders.Height > 0) ? 1 : 0);
            return num + ((IBetterListViewLayoutElementDisplayable)item).LayoutIndexDisplay;
        }
    }
}