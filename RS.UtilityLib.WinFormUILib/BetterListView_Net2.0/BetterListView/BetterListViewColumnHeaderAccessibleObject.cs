using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides information about BetterListViewColumnHeader that can be used by an accessibility application.
	/// </summary>
	internal sealed class BetterListViewColumnHeaderAccessibleObject : AccessibleObject
	{
		private readonly BetterListViewColumnHeader columnHeader;

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
				BetterListViewColumnHeaderBounds bounds = this.columnHeader.Bounds;
				if (bounds == null) {
					return Rectangle.Empty;
				}
				Rectangle clientRectangleInner = this.listView.ClientRectangleInner;
				Rectangle boundsOuter = bounds.BoundsOuter;
				if (!boundsOuter.IntersectsWith(clientRectangleInner)) {
					return Rectangle.Empty;
				}
				clientRectangleInner.Location = this.listView.PointToScreen(clientRectangleInner.Location);
				boundsOuter.Location = this.listView.PointToScreen(boundsOuter.Location);
				boundsOuter.Intersect(clientRectangleInner);
				return boundsOuter;
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
		public override string Name => this.columnHeader.Text;

		/// <summary>
		///   Gets the role of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values, or <see cref="F:System.Windows.Forms.AccessibleRole.None" /> if no role has been specified.
		/// </returns>
		public override AccessibleRole Role => AccessibleRole.ColumnHeader;

		/// <summary>
		///   Gets the state of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleStates" /> values, or <see cref="F:System.Windows.Forms.AccessibleStates.None" />, if no state has been set.
		/// </returns>
		public override AccessibleStates State {
			get {
				AccessibleStates accessibleStates = AccessibleStates.None;
				if (!this.listView.Visible) {
					accessibleStates |= AccessibleStates.Invisible;
				}
				else {
					if (!this.columnHeader.Visible) {
						accessibleStates |= AccessibleStates.Invisible;
					}
					if (this.Bounds.IsEmpty) {
						accessibleStates |= AccessibleStates.Offscreen;
					}
					if (this.listView.StateInfo.State == BetterListViewState.ColumnSelection && this.listView.StateInfo.ColumnSelectionStateInfo.Column == this.columnHeader) {
						accessibleStates |= AccessibleStates.Pressed;
					}
					if (this.listView.ReadOnly || this.columnHeader.GetStyle(this.listView) == BetterListViewColumnHeaderStyle.Nonclickable) {
						accessibleStates |= AccessibleStates.ReadOnly;
					}
					if (!this.listView.ReadOnly && this.columnHeader.AllowResize) {
						accessibleStates |= AccessibleStates.Sizeable;
					}
				}
				return accessibleStates;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewColumnHeaderAccessibleObject" /> class.
		/// </summary>
		/// <param name="columnHeader">Column header for which this accessible object is created.</param>
		/// <param name="listView">Owner list control.</param>
		/// <param name="parent">Parent accessible object.</param>
		public BetterListViewColumnHeaderAccessibleObject(BetterListViewColumnHeader columnHeader, BetterListView listView, AccessibleObject parent) {
			Checks.CheckNotNull(columnHeader, "columnHeader");
			Checks.CheckNotNull(listView, "listView");
			Checks.CheckNotNull(parent, "parent");
			this.columnHeader = columnHeader;
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
			this.listView.DoDefaultAction(this.columnHeader);
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
			BetterListViewColumnHeader betterListViewColumnHeader;
			switch (navdir) {
				case AccessibleNavigation.Left:
				case AccessibleNavigation.Previous:
					betterListViewColumnHeader = this.columnHeader.PreviousVisibleColumnHeader;
					break;
				case AccessibleNavigation.Right:
				case AccessibleNavigation.Next:
					betterListViewColumnHeader = this.columnHeader.NextVisibleColumnHeader;
					break;
				default:
					return null;
			}
			return betterListViewColumnHeader.GetAccessibilityInstance(this.listView, this.parent);
		}
	}
}