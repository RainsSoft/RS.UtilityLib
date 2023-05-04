using System.Text;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents BetterListView dependent state variables.
	/// </summary>
	internal struct BetterListViewStateInfo
	{
		/// <summary>
		///   represents an empty BetterListViewStateInfo structure
		/// </summary>
		public static readonly BetterListViewStateInfo Default = new BetterListViewStateInfo(BetterListViewState.Normal, focusedByMouseDown: false, BetterListViewColumnSelectionStateInfo.Empty, BetterListViewColumnResizeStateInfo.Empty, BetterListViewColumnReorderStateInfo.Empty, BetterListViewItemBeforeCheckStateInfo.Empty, BetterListViewItemDragStateInfo.Empty, BetterListViewItemSelectionStateInfo.Empty, BetterListViewLabelEditStateInfo.Empty);

		private BetterListViewState state;

		private bool focusedByMouseDown;

		private BetterListViewColumnSelectionStateInfo columnSelectionStateInfo;

		private BetterListViewColumnResizeStateInfo columnResizeStateInfo;

		private BetterListViewColumnReorderStateInfo columnReorderStateInfo;

		private BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfo;

		private BetterListViewItemDragStateInfo itemDragStateInfo;

		private BetterListViewItemSelectionStateInfo itemSelectionStateInfo;

		private BetterListViewLabelEditStateInfo labelEditStateInfo;

		/// <summary>
		///   type of a BetterListView state
		/// </summary>
		public BetterListViewState State {
			get {
				return this.state;
			}
			set {
				this.state = value;
			}
		}

		/// <summary>
		///   control has been focused by pressing a mouse button
		/// </summary>
		public bool FocusedByMouseDown {
			get {
				return this.focusedByMouseDown;
			}
			set {
				this.focusedByMouseDown = value;
			}
		}

		/// <summary>
		///   state information for the ColumnSelection state
		/// </summary>
		public BetterListViewColumnSelectionStateInfo ColumnSelectionStateInfo {
			get {
				return this.columnSelectionStateInfo;
			}
			set {
				this.columnSelectionStateInfo = value;
			}
		}

		/// <summary>
		///   state information for ColumnBeforeResize and ColumnResize states
		/// </summary>
		public BetterListViewColumnResizeStateInfo ColumnResizeStateInfo {
			get {
				return this.columnResizeStateInfo;
			}
			set {
				this.columnResizeStateInfo = value;
			}
		}

		/// <summary>
		///   state information for the ColumnReorder state
		/// </summary>
		public BetterListViewColumnReorderStateInfo ColumnReorderStateInfo {
			get {
				return this.columnReorderStateInfo;
			}
			set {
				this.columnReorderStateInfo = value;
			}
		}

		/// <summary>
		///   state information for the ItemBeforeCheckKeyboard/ItemBeforeCheckMouse state
		/// </summary>
		public BetterListViewItemBeforeCheckStateInfo ItemBeforeCheckStateInfo {
			get {
				return this.itemBeforeCheckStateInfo;
			}
			set {
				this.itemBeforeCheckStateInfo = value;
			}
		}

		/// <summary>
		///   state information for the ItemDrag state
		/// </summary>
		public BetterListViewItemDragStateInfo ItemDragStateInfo {
			get {
				return this.itemDragStateInfo;
			}
			set {
				this.itemDragStateInfo = value;
			}
		}

		/// <summary>
		///   state information for the ItemBeforeSelection/ItemSelection state
		/// </summary>
		public BetterListViewItemSelectionStateInfo ItemSelectionStateInfo {
			get {
				return this.itemSelectionStateInfo;
			}
			set {
				this.itemSelectionStateInfo = value;
			}
		}

		/// <summary>
		///   state information for the LabelEdit state
		/// </summary>
		public BetterListViewLabelEditStateInfo LabelEditStateInfo {
			get {
				return this.labelEditStateInfo;
			}
			set {
				this.labelEditStateInfo = value;
			}
		}

		/// <summary>
		///   this BetterListViewStateInfo structure is has the default values set
		/// </summary>
		public bool IsDefault => this.Equals(BetterListViewStateInfo.Default);

		/// <summary>
		///   Initialize a new BetterListViewStateInfo instance.
		/// </summary>
		/// <param name="state">type of a BetterListView state</param>
		/// <param name="focusedByMouseDown">control has been focused by pressing a mouse button</param>
		/// <param name="columnSelectionStateInfo">state information for the ColumnSelection state</param>
		/// <param name="columnResizeStateInfo">state information for the ColumnResize state</param>
		/// <param name="columnReorderStateInfo">state information for the ColumnReorder state</param>
		/// <param name="itemBeforeCheckStateInfo">state information for the ItemBeforeCheckKeyboard/ItemBeforeCheckMouse state</param>
		/// <param name="itemDragStateInfo">state information for the ItemDrag state</param>
		/// <param name="itemSelectionStateInfo">state information for the ItemBeforeSelection/ItemSelection state</param>
		/// <param name="labelEditStateInfo">state information for the LabelEdit state</param>
		public BetterListViewStateInfo(BetterListViewState state, bool focusedByMouseDown, BetterListViewColumnSelectionStateInfo columnSelectionStateInfo, BetterListViewColumnResizeStateInfo columnResizeStateInfo, BetterListViewColumnReorderStateInfo columnReorderStateInfo, BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfo, BetterListViewItemDragStateInfo itemDragStateInfo, BetterListViewItemSelectionStateInfo itemSelectionStateInfo, BetterListViewLabelEditStateInfo labelEditStateInfo) {
			this.state = state;
			this.focusedByMouseDown = focusedByMouseDown;
			this.columnSelectionStateInfo = columnSelectionStateInfo;
			this.columnResizeStateInfo = columnResizeStateInfo;
			this.columnReorderStateInfo = columnReorderStateInfo;
			this.itemBeforeCheckStateInfo = itemBeforeCheckStateInfo;
			this.itemDragStateInfo = itemDragStateInfo;
			this.itemSelectionStateInfo = itemSelectionStateInfo;
			this.labelEditStateInfo = labelEditStateInfo;
		}

		public static bool operator ==(BetterListViewStateInfo stateInfoA, BetterListViewStateInfo stateInfoB) {
			return stateInfoA.Equals(stateInfoB);
		}

		public static bool operator !=(BetterListViewStateInfo stateInfoA, BetterListViewStateInfo stateInfoB) {
			return !stateInfoA.Equals(stateInfoB);
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewStateInfo betterListViewStateInfo)) {
				return false;
			}
			if (this.state == betterListViewStateInfo.state && this.focusedByMouseDown == betterListViewStateInfo.focusedByMouseDown && this.columnSelectionStateInfo == betterListViewStateInfo.columnSelectionStateInfo && this.columnResizeStateInfo == betterListViewStateInfo.columnResizeStateInfo && this.columnReorderStateInfo == betterListViewStateInfo.columnReorderStateInfo && this.itemBeforeCheckStateInfo == betterListViewStateInfo.itemBeforeCheckStateInfo && this.itemDragStateInfo == betterListViewStateInfo.itemDragStateInfo && this.itemSelectionStateInfo == betterListViewStateInfo.itemSelectionStateInfo) {
				return this.labelEditStateInfo == betterListViewStateInfo.labelEditStateInfo;
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.state.GetHashCode() ^ this.focusedByMouseDown.GetHashCode() ^ this.columnSelectionStateInfo.GetHashCode() ^ this.columnResizeStateInfo.GetHashCode() ^ this.columnReorderStateInfo.GetHashCode() ^ this.itemBeforeCheckStateInfo.GetHashCode() ^ this.itemDragStateInfo.GetHashCode() ^ this.itemSelectionStateInfo.GetHashCode() ^ this.labelEditStateInfo.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.String" /> that represents this instance.
		/// </returns>
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetType().Name + ": {{");
			stringBuilder.AppendLine($"State = '{this.state}',");
			stringBuilder.AppendLine($"FocusedByMouseDown = '{this.focusedByMouseDown}',");
			stringBuilder.AppendLine($"ColumnSelectionStateInfo = '{this.columnSelectionStateInfo}',");
			stringBuilder.AppendLine($"ColumnResizeStateInfo = '{this.columnResizeStateInfo}',");
			stringBuilder.AppendLine($"ColumnReorderStateInfo = '{this.columnReorderStateInfo}',");
			stringBuilder.AppendLine($"ItemBeforeCheckStateInfo = '{this.itemBeforeCheckStateInfo}',");
			stringBuilder.AppendLine($"ItemDragStateInfo = '{this.itemDragStateInfo}',");
			stringBuilder.AppendLine($"ItemSelectionStateInfo = '{this.itemSelectionStateInfo}',");
			stringBuilder.Append($"LabelEditStateInfo = '{this.labelEditStateInfo}'}}");
			return stringBuilder.ToString();
		}
	}
}