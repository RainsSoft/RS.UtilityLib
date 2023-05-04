using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemActivate event.
	/// </summary>
	public class BetterListViewItemActivateEventArgs : EventArgs
	{
		private BetterListViewItem item;

		private BetterListViewItemActivationSource activationSource;

		/// <summary>
		///   Gets or sets the activated item.
		/// </summary>
		/// <value>
		///   The activated item.
		/// </value>
		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.item = value;
			}
		}

		/// <summary>
		///   Gets or sets the cause of item activation.
		/// </summary>
		/// <value>
		///   The cause of item activation.
		/// </value>
		public BetterListViewItemActivationSource ActivationSource {
			get {
				return this.activationSource;
			}
			set {
				this.activationSource = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemActivateEventArgs" /> class.
		/// </summary>
		/// <param name="item">Activated item.</param>
		/// <param name="activationSource">Cause of item activation.</param>
		public BetterListViewItemActivateEventArgs(BetterListViewItem item, BetterListViewItemActivationSource activationSource) {
			this.Item = item;
			this.ActivationSource = activationSource;
		}
	}
}