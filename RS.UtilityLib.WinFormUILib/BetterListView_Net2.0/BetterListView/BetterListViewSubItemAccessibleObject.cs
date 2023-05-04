using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides information about BetterListViewSubItem that can be used by an accessibility application.
	/// </summary>
	internal sealed class BetterListViewSubItemAccessibleObject : AccessibleObject
	{
		private readonly BetterListViewSubItem subItem;

		private readonly BetterListViewItem item;

		private readonly BetterListView listView;

		private readonly AccessibleObject parent;

		/// <summary>
		///   Gets the location and size of the accessible object.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Drawing.Rectangle" /> that represents the bounds of the accessible object.
		/// </returns>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">
		///   The bounds of control cannot be retrieved.
		/// </exception>
		public override Rectangle Bounds {
			get {
				BetterListViewItemBounds bounds = this.item.Bounds;
				if (bounds == null || this.subItem.Index >= bounds.SubItemBounds.Count) {
					return Rectangle.Empty;
				}
				Rectangle clientRectangleInner = this.listView.ClientRectangleInner;
				Rectangle boundsInner = bounds.SubItemBounds[this.subItem.Index].BoundsInner;
				clientRectangleInner.Location = this.listView.PointToScreen(clientRectangleInner.Location);
				boundsInner.Location = this.listView.PointToScreen(boundsInner.Location);
				boundsInner.Intersect(clientRectangleInner);
				return boundsInner;
			}
		}

		/// <summary>
		///   Gets or sets the object name.
		/// </summary>
		/// <returns>
		///   The object name, or null if the property has not been set.
		/// </returns>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">
		///   The name of the control cannot be retrieved or set.
		/// </exception>
		public override string Name {
			get {
				if (this.listView.ViewInternal == BetterListViewViewInternal.DetailsColumns) {
					return this.listView.LayoutElementsColumns[this.subItem.Index].Text;
				}
				return "";
			}
		}

		/// <summary>
		///   Gets the role of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values, or <see cref="F:System.Windows.Forms.AccessibleRole.None" /> if no role has been specified.
		/// </returns>
		public override AccessibleRole Role => AccessibleRole.StaticText;

		/// <summary>
		///   Gets the state of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleStates" /> values, or <see cref="F:System.Windows.Forms.AccessibleStates.None" />, if no state has been set.
		/// </returns>
		public override AccessibleStates State {
			get {
				AccessibleStates accessibleStates = AccessibleStates.Focusable;
				if (this.listView.FocusInfo.Element == this.item && this.listView.FocusInfo.ColumnIndex == this.subItem.Index) {
					accessibleStates |= AccessibleStates.Focused;
				}
				if (this.Bounds.IsEmpty) {
					accessibleStates |= AccessibleStates.Offscreen;
				}
				return accessibleStates;
			}
		}

		/// <summary>
		///   Gets the parent of an accessible object.
		/// </summary>
		/// <returns>
		///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the parent of an accessible object, or null if there is no parent object.
		/// </returns>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
		/// </PermissionSet>
		public override AccessibleObject Parent => this.parent;

		/// <summary>
		///   Gets or sets the value of an accessible object.
		/// </summary>
		/// <returns>
		///   The value of an accessible object, or null if the object has no value set.
		/// </returns>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">
		///   The value cannot be set or retrieved.
		/// </exception>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
		/// </PermissionSet>
		public override string Value => this.subItem.DisplayText;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewSubItemAccessibleObject" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item for which this accessible object is created.</param>
		/// <param name="listView">Owner list control.</param>
		/// <param name="parent">Parent accessible object.</param>
		public BetterListViewSubItemAccessibleObject(BetterListViewSubItem subItem, BetterListView listView, AccessibleObject parent) {
			Checks.CheckNotNull(subItem, "subItem");
			Checks.CheckNotNull(listView, "listView");
			Checks.CheckNotNull(parent, "parent");
			this.subItem = subItem;
			this.item = this.subItem.Item;
			this.listView = listView;
			this.parent = parent;
			Checks.CheckNotNull(this.item, "this.item");
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
			if (this.listView.FocusInfo.Element != this.item || this.listView.FocusInfo.ColumnIndex != this.subItem.Index) {
				return null;
			}
			return this;
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
			if (!this.Bounds.Contains(x, y)) {
				return null;
			}
			return this;
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
			BetterListViewSubItem betterListViewSubItem = null;
			switch (navdir) {
				case AccessibleNavigation.Left:
				case AccessibleNavigation.Previous:
					betterListViewSubItem = ((this.subItem.Index > 0) ? this.item.SubItems[this.subItem.Index - 1] : null);
					break;
				case AccessibleNavigation.Right:
				case AccessibleNavigation.Next:
					betterListViewSubItem = ((this.subItem.Index < Math.Min(this.listView.LayoutElementsColumns.Count, this.item.SubItems.Count) - 1) ? this.item.SubItems[this.subItem.Index + 1] : null);
					break;
			}
			return betterListViewSubItem?.GetAccessibilityInstance(this.listView, this.parent);
		}
	}
}