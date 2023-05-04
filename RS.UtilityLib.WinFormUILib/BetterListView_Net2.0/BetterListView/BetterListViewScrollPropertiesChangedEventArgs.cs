using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.HScrollPropertiesChanged and BetterListView.VScrollPropertiesChanged events.
	/// </summary>
	public class BetterListViewScrollPropertiesChangedEventArgs : EventArgs
	{
		private BetterListViewScrollProperties scrollProperties;

		/// <summary>
		///   scroll bar properties
		/// </summary>
		public BetterListViewScrollProperties ScrollProperties {
			get {
				return this.scrollProperties;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.scrollProperties = value;
			}
		}

		/// <summary>
		///   Initialize a new BetterListViewScrollPropertiesChangedEventArgs instance.
		/// </summary>
		/// <param name="scrollProperties">scroll bar properties</param>
		public BetterListViewScrollPropertiesChangedEventArgs(BetterListViewScrollProperties scrollProperties) {
			this.ScrollProperties = scrollProperties;
		}
	}
}