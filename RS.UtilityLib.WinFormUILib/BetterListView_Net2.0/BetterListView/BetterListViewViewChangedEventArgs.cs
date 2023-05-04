using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ViewChanged event.
	/// </summary>
	public class BetterListViewViewChangedEventArgs : EventArgs
	{
		private BetterListViewView viewOld;

		private BetterListViewView viewNew;

		/// <summary>
		///   Gets or sets the old View property value.
		/// </summary>
		/// <value>
		///   The old View property value.
		/// </value>
		public BetterListViewView ViewOld {
			get {
				return this.viewOld;
			}
			set {
				this.viewOld = value;
			}
		}

		/// <summary>
		///   Gets or sets the new View property value.
		/// </summary>
		/// <value>
		///   The new View property value.
		/// </value>
		public BetterListViewView ViewNew {
			get {
				return this.viewNew;
			}
			set {
				this.viewNew = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewViewChangedEventArgs" /> class.
		/// </summary>
		/// <param name="viewOld">The old View property value.</param>
		/// <param name="viewNew">The new View property value.</param>
		public BetterListViewViewChangedEventArgs(BetterListViewView viewOld, BetterListViewView viewNew) {
			this.ViewOld = viewOld;
			this.ViewNew = viewNew;
		}
	}
}