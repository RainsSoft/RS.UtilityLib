using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides information about BetterListViewItem that can be used by an accessibility application.
	/// </summary>
	internal sealed class BetterListViewItemAccessibleObject : AccessibleObject
	{
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
				if (bounds == null) {
					return Rectangle.Empty;
				}
				Rectangle clientRectangleInner = this.listView.ClientRectangleInner;
				Rectangle boundsSelection = bounds.BoundsSelection;
				clientRectangleInner.Location = this.listView.PointToScreen(clientRectangleInner.Location);
				boundsSelection.Location = this.listView.PointToScreen(boundsSelection.Location);
				boundsSelection.Intersect(clientRectangleInner);
				return boundsSelection;
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
		public override string Name => this.item.DisplayText;

		/// <summary>
		///   Gets the role of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values, or <see cref="F:System.Windows.Forms.AccessibleRole.None" /> if no role has been specified.
		/// </returns>
		public override AccessibleRole Role => AccessibleRole.ListItem;

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
		///   Gets the state of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleStates" /> values, or <see cref="F:System.Windows.Forms.AccessibleStates.None" />, if no state has been set.
		/// </returns>
		public override AccessibleStates State {
			get {
				AccessibleStates accessibleStates = AccessibleStates.Focusable | AccessibleStates.Selectable;
				if (this.listView.ShowItemExpandButtons && this.item.AllowShowExpandButton && this.item.ChildItems.Count != 0) {
					accessibleStates |= (this.item.IsExpanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed);
				}
				if (this.listView.CheckBoxesVisible && this.item.CheckBoxAppearance != 0) {
					switch (this.item.CheckState) {
						case CheckState.Checked:
							accessibleStates |= AccessibleStates.Checked;
							break;
						case CheckState.Indeterminate:
							accessibleStates |= AccessibleStates.Mixed;
							break;
					}
				}
				if (this.listView.FocusInfo.Element == this.item && this.listView.FocusInfo.ColumnIndex == 0) {
					accessibleStates |= AccessibleStates.Focused;
				}
				if (!this.item.Visible) {
					accessibleStates |= AccessibleStates.Invisible;
				}
				if (this.Bounds.IsEmpty) {
					accessibleStates |= AccessibleStates.Offscreen;
				}
				if (this.listView.ReadOnly) {
					accessibleStates |= AccessibleStates.ReadOnly;
				}
				if (this.item.Selected) {
					accessibleStates |= AccessibleStates.Selected;
				}
				return accessibleStates;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemAccessibleObject" /> class.
		/// </summary>
		/// <param name="item">Item for which this accessible object is created.</param>
		/// <param name="listView">Owner list control.</param>
		/// <param name="parent">Parent accessible object.</param>
		public BetterListViewItemAccessibleObject(BetterListViewItem item, BetterListView listView, AccessibleObject parent) {
			Checks.CheckNotNull(item, "item");
			Checks.CheckNotNull(listView, "listView");
			Checks.CheckNotNull(parent, "parent");
			this.item = item;
			this.listView = listView;
			this.parent = parent;
		}

		/// <summary>
		///   Performs the default action associated with this accessible object.
		/// </summary>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">
		///   The default action for the control cannot be performed.
		/// </exception>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
		/// </PermissionSet>
		public override void DoDefaultAction() {
			this.listView.DoDefaultAction(this.item);
		}

		/// <summary>
		///   Retrieves the accessible child corresponding to the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the accessible child.</param>
		/// <returns>
		///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the accessible child corresponding to the specified index.
		/// </returns>
		public override AccessibleObject GetChild(int index) {
			return this.item.SubItems[index].GetAccessibilityInstance(this.listView, this);
		}

		/// <summary>
		///   Retrieves the number of children belonging to an accessible object.
		/// </summary>
		/// <returns>
		///   The number of children belonging to an accessible object.
		/// </returns>
		public override int GetChildCount() {
			return Math.Min(this.item.SubItems.Count, this.listView.LayoutElementsColumns.Count);
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
			if (this.listView.FocusInfo.Element != this.item) {
				return null;
			}
			int columnIndex = this.listView.FocusInfo.ColumnIndex;
			if (columnIndex == -1) {
				return this;
			}
			return this.item.SubItems[columnIndex].GetAccessibilityInstance(this.listView, this);
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
			BetterListViewSubItem subItemAt = this.listView.GetSubItemAt(this.listView.PointToClient(new Point(x, y)));
			if (subItemAt == null || subItemAt.Item == this.item) {
				return null;
			}
			return subItemAt.GetAccessibilityInstance(this.listView, this);
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
			BetterListViewItem betterListViewItem = null;
			BetterListViewSubItem betterListViewSubItem = null;
			switch (navdir) {
				case AccessibleNavigation.Down:
					betterListViewItem = this.listView.FindNearestItem(this.item, SearchDirectionHint.Down);
					break;
				case AccessibleNavigation.FirstChild:
					betterListViewSubItem = this.item.SubItems[0];
					break;
				case AccessibleNavigation.LastChild:
					betterListViewSubItem = this.item.SubItems[Math.Min(this.listView.LayoutElementsColumns.Count, this.item.SubItems.Count) - 1];
					break;
				case AccessibleNavigation.Left:
					betterListViewItem = this.listView.FindNearestItem(this.item, SearchDirectionHint.Left);
					break;
				case AccessibleNavigation.Next:
					betterListViewItem = this.item.NextVisibleItem;
					break;
				case AccessibleNavigation.Previous:
					betterListViewItem = this.item.PreviousVisibleItem;
					break;
				case AccessibleNavigation.Right:
					betterListViewItem = this.listView.FindNearestItem(this.item, SearchDirectionHint.Right);
					break;
				case AccessibleNavigation.Up:
					betterListViewItem = this.listView.FindNearestItem(this.item, SearchDirectionHint.Up);
					break;
			}
			if (betterListViewItem != null) {
				return betterListViewItem.GetAccessibilityInstance(this.listView, this.parent);
			}
			return betterListViewSubItem?.GetAccessibilityInstance(this.listView, this);
		}

		/// <summary>
		///   Modifies the selection or moves the keyboard focus of the accessible object.
		/// </summary>
		/// <param name="flags">One of the <see cref="T:System.Windows.Forms.AccessibleSelection" /> values.</param>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">
		///   The selection cannot be performed.
		/// </exception>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
		/// </PermissionSet>
		public override void Select(AccessibleSelection flags) {
			if (flags != 0) {
				BetterListViewSelectionOptions betterListViewSelectionOptions = BetterListViewSelectionOptions.None;
				if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus) {
					betterListViewSelectionOptions |= BetterListViewSelectionOptions.TakeFocus | BetterListViewSelectionOptions.TakeAnchor;
				}
				if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection) {
					betterListViewSelectionOptions |= BetterListViewSelectionOptions.TakeSelection;
				}
				if ((flags & AccessibleSelection.ExtendSelection) == AccessibleSelection.ExtendSelection) {
					betterListViewSelectionOptions |= BetterListViewSelectionOptions.ExtendSelection;
				}
				if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection) {
					betterListViewSelectionOptions |= BetterListViewSelectionOptions.AddSelection;
				}
				if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection) {
					betterListViewSelectionOptions |= BetterListViewSelectionOptions.RemoveSelection;
				}
				this.listView.MakeSelection(this.item, betterListViewSelectionOptions, BetterListViewSelectionChangeMode.Accessiblity);
			}
		}
	}
}