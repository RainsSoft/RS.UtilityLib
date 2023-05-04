using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.ItemSearch event.
	/// </summary>
	public class BetterListViewItemSearchEventArgs : EventArgs
	{
		private BetterListViewItemCollection items;

		private string queryString;

		private bool selectionChanged;

		private BetterListViewItemSearchSource source;

		/// <summary>
		///   Gets or sets the found item(s).
		/// </summary>
		/// <value>
		///   The found item(s).
		/// </value>
		public BetterListViewItemCollection Items {
			get {
				return this.items;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.items = value;
			}
		}

		/// <summary>
		///   Gets or sets the query string used for searching.
		/// </summary>
		/// <value>
		///   The query string used for searching.
		/// </value>
		public string QueryString {
			get {
				return this.queryString;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.queryString = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether the selection has been changed by the searching.
		/// </summary>
		/// <value>
		///   <c>true</c> if the selection has been changed by the searching; otherwise, <c>false</c>.
		/// </value>
		public bool SelectionChanged {
			get {
				return this.selectionChanged;
			}
			set {
				this.selectionChanged = value;
			}
		}

		/// <summary>
		///   Gets or sets the search source.
		/// </summary>
		/// <value>
		///   The search source.
		/// </value>
		public BetterListViewItemSearchSource Source {
			get {
				return this.source;
			}
			set {
				this.source = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemSearchEventArgs" /> class.
		/// </summary>
		/// <param name="items">Found item(s).</param>
		/// <param name="queryString">Query string used for searching.</param>
		/// <param name="selectionChanged">Selection has been changed by the searching.</param>
		/// <param name="source">Search source.</param>
		public BetterListViewItemSearchEventArgs(BetterListViewItemCollection items, string queryString, bool selectionChanged, BetterListViewItemSearchSource source) {
			this.Items = items;
			this.QueryString = queryString;
			this.SelectionChanged = selectionChanged;
			this.Source = source;
		}
	}
}