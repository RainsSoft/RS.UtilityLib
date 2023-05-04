using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BetterListViewFormatItem event.
	/// </summary>
	public class BetterListViewFormatItemEventArgs : EventArgs
	{
		private BetterListViewItem item;

		private int subItemIndex;

		private string currentText;

		private string newText;

		/// <summary>
		///   Item whose label is to be formatted.
		/// </summary>
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
		///   Index of sub-item whose label is to be formatted.
		/// </summary>
		public int SubItemIndex {
			get {
				return this.subItemIndex;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.subItemIndex = value;
			}
		}

		/// <summary>
		///   Original text to be formatted.
		/// </summary>
		public string CurrentText {
			get {
				return this.currentText;
			}
			set {
				this.currentText = value ?? string.Empty;
			}
		}

		/// <summary>
		///   Formatted text.
		/// </summary>
		public string NewText {
			get {
				return this.newText;
			}
			set {
				this.newText = value ?? string.Empty;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewFormatItemEventArgs" /> class.
		/// </summary>
		/// <param name="item">Item whose label is to be formatted.</param>
		/// <param name="subItemIndex">Index of sub-item whose label is to be formatted.</param>
		/// <param name="currentText">Original text to be formatted.</param>
		/// <param name="newText">Formatted text.</param>
		public BetterListViewFormatItemEventArgs(BetterListViewItem item, int subItemIndex, string currentText, string newText) {
			this.Item = item;
			this.SubItemIndex = subItemIndex;
			this.CurrentText = currentText;
			this.NewText = newText;
		}
	}
}