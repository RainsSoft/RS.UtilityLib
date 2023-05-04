using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.HitTestChanged event.
	/// </summary>
	public class BetterListViewHitTestChangedEventArgs : EventArgs
	{
		private BetterListViewHitTestInfo hitTestInfoCurrent;

		private BetterListViewHitTestInfo hitTestInfoNew;

		/// <summary>
		///   Gets or sets current hit test information (before change).
		/// </summary>
		/// <value>
		///   Current hit test information (before change).
		/// </value>
		public BetterListViewHitTestInfo HitTestInfoCurrent {
			get {
				return this.hitTestInfoCurrent;
			}
			set {
				this.hitTestInfoCurrent = value;
			}
		}

		/// <summary>
		///   Gets or sets new hit test information.
		/// </summary>
		/// <value>
		///   New hit test information.
		/// </value>
		public BetterListViewHitTestInfo HitTestInfoNew {
			get {
				return this.hitTestInfoNew;
			}
			set {
				this.hitTestInfoNew = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewHitTestChangedEventArgs" /> class.
		/// </summary>
		/// <param name="hitTestInfoCurrent">Current hit test information (before change).</param>
		/// <param name="hitTestInfoNew">New hit test information.</param>
		public BetterListViewHitTestChangedEventArgs(BetterListViewHitTestInfo hitTestInfoCurrent, BetterListViewHitTestInfo hitTestInfoNew) {
			this.HitTestInfoCurrent = hitTestInfoCurrent;
			this.HitTestInfoNew = hitTestInfoNew;
		}
	}
}