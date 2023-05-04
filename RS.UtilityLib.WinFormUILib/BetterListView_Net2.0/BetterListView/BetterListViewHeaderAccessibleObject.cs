using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides information about Better ListView columns that can be used by an accessibility application.
	/// </summary>
	internal sealed class BetterListViewHeaderAccessibleObject : AccessibleObject
	{
		private readonly BetterListView listView;

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
				Rectangle boundsColumnHeaders = this.listView.BoundsColumnHeaders;
				if (!boundsColumnHeaders.IsEmpty) {
					boundsColumnHeaders.Location = this.listView.PointToScreen(boundsColumnHeaders.Location);
				}
				return boundsColumnHeaders;
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
		public override string Name => "Header";

		/// <summary>
		///   Gets the role of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleRole" /> values, or <see cref="F:System.Windows.Forms.AccessibleRole.None" /> if no role has been specified.
		/// </returns>
		public override AccessibleRole Role => AccessibleRole.List;

		/// <summary>
		///   Gets the state of this accessible object.
		/// </summary>
		/// <returns>
		///   One of the <see cref="T:System.Windows.Forms.AccessibleStates" /> values, or <see cref="F:System.Windows.Forms.AccessibleStates.None" />, if no state has been set.
		/// </returns>
		public override AccessibleStates State {
			get {
				AccessibleStates accessibleStates = AccessibleStates.Default;
				if (this.Bounds.IsEmpty) {
					accessibleStates |= AccessibleStates.Invisible;
				}
				return accessibleStates;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewHeaderAccessibleObject" /> class.
		/// </summary>
		/// <param name="listView">Owner list control.</param>
		public BetterListViewHeaderAccessibleObject(BetterListView listView) {
			Checks.CheckNotNull(listView, "listView");
			this.listView = listView;
		}

		/// <summary>
		///   Retrieves the accessible child corresponding to the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the accessible child.</param>
		/// <returns>
		///   An <see cref="T:System.Windows.Forms.AccessibleObject" /> that represents the accessible child corresponding to the specified index.
		/// </returns>
		public override AccessibleObject GetChild(int index) {
			return this.listView.LayoutElementsColumns[index].GetAccessibilityInstance(this.listView, this);
		}

		/// <summary>
		///   Retrieves the number of children belonging to an accessible object.
		/// </summary>
		/// <returns>
		///   The number of children belonging to an accessible object.
		/// </returns>
		public override int GetChildCount() {
			return this.listView.LayoutElementsColumns.Count;
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
			return this.listView.GetColumnHeaderAt(this.listView.PointToClient(new Point(x, y)))?.GetAccessibilityInstance(this.listView, this);
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
			BetterListViewColumnHeader betterListViewColumnHeader = null;
			switch (navdir) {
				case AccessibleNavigation.FirstChild:
					if (this.listView.LayoutElementsColumns.Count != 0) {
						betterListViewColumnHeader = this.listView.LayoutElementsColumns[0];
					}
					break;
				case AccessibleNavigation.LastChild:
					if (this.listView.LayoutElementsColumns.Count != 0) {
						betterListViewColumnHeader = this.listView.LayoutElementsColumns[this.listView.LayoutElementsColumns.Count - 1];
					}
					break;
			}
			return betterListViewColumnHeader?.GetAccessibilityInstance(this.listView, this);
		}
	}
}