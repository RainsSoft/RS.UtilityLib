namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.BeforeLabelEdit and BetterListView.AfterLabelEditCancel events.
	/// </summary>
	public class BetterListViewLabelEditCancelEventArgs : BetterListViewLabelEditEventArgs
	{
		private bool cancelEdit;

		/// <summary>
		///   Gets or sets a value indicating whether to cancel changes made to the sub-item being edited.
		/// </summary>
		/// <value>
		///   <c>true</c> if cancel changes made to the sub-item being edited; otherwise, <c>false</c>.
		/// </value>
		public bool CancelEdit {
			get {
				return this.cancelEdit;
			}
			set {
				this.cancelEdit = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> class.
		/// </summary>
		/// <param name="subItem">Sub-item being edited.</param>
		public BetterListViewLabelEditCancelEventArgs(BetterListViewSubItem subItem)
			: this(cancelEdit: false, string.Empty, subItem) {
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLabelEditEventArgs" /> class.
		/// </summary>
		/// <param name="cancelEdit">Cancel changes made to the sub-item being edited.</param>
		/// <param name="label">New text assigned to the sub-item.</param>
		/// <param name="subItem">Sub-item being edited.</param>
		public BetterListViewLabelEditCancelEventArgs(bool cancelEdit, string label, BetterListViewSubItem subItem)
			: base(label, subItem) {
			this.cancelEdit = cancelEdit;
		}
	}
}