using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.RequestEmbeddedControl event.
	/// </summary>
	public class BetterListViewRequestEmbeddedControlEventArgs : CancelEventArgs
	{
		private BetterListViewSubItem subItem;

		private BetterListViewEmbeddedControlPlacement controlPlacement;

		/// <summary>
		///   Gets or sets the sub-item to be modified by embedded control.
		/// </summary>
		/// <value>
		///   The sub-item to be modified by embedded control.
		/// </value>
		public BetterListViewSubItem SubItem {
			get {
				return this.subItem;
			}
			set {
				this.subItem = value;
			}
		}

		/// <summary>
		///   Gets or sets the embedded control placement within the specified sub-item.
		/// </summary>
		/// <value>
		///   The embedded control placement within the specified sub-item.
		/// </value>
		public BetterListViewEmbeddedControlPlacement ControlPlacement {
			get {
				return this.controlPlacement;
			}
			set {
				this.controlPlacement = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewRequestEmbeddedControlEventArgs" /> class.
		/// </summary>
		public BetterListViewRequestEmbeddedControlEventArgs()
			: this(null) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewRequestEmbeddedControlEventArgs" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item to be modified by embedded control.</param>
		public BetterListViewRequestEmbeddedControlEventArgs(BetterListViewSubItem subItem)
			: this(subItem, BetterListViewEmbeddedControlPlacement.Empty) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewRequestEmbeddedControlEventArgs" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item to be modified by embedded control.</param>
		/// <param name="controlPlacement">Embedded control placement within the specified sub-item.</param>
		public BetterListViewRequestEmbeddedControlEventArgs(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement controlPlacement)
			: this(subItem, controlPlacement, cancel: false) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewRequestEmbeddedControlEventArgs" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item to be modified by embedded control.</param>
		/// <param name="controlPlacement">Embedded control placement within the specified sub-item.</param>
		/// <param name="cancel">Cancel the embedded control request.</param>
		public BetterListViewRequestEmbeddedControlEventArgs(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement controlPlacement, bool cancel)
			: base(cancel) {
			this.subItem = subItem;
			this.controlPlacement = controlPlacement;
		}
	}
}