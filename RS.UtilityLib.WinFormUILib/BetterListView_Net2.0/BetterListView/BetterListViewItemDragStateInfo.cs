namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the ItemDrag state.
	/// </summary>
	internal struct BetterListViewItemDragStateInfo
	{
		/// <summary>
		///   empty BetterListViewItemDragStateInfo stucture
		/// </summary>
		public static readonly BetterListViewItemDragStateInfo Empty = new BetterListViewItemDragStateInfo(BetterListViewDragDropType.Undefined, new BetterListViewItemCollection());

		private BetterListViewDragDropType dragDropType;

		private BetterListViewItemCollection sourceItems;

		private BetterListViewElementBase lastHoverElement;

		public BetterListViewDragDropType DragDropType {
			get {
				return this.dragDropType;
			}
			set {
				this.dragDropType = value;
			}
		}

		public BetterListViewItemCollection SourceItems {
			get {
				return this.sourceItems;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.sourceItems = value;
			}
		}

		public BetterListViewElementBase LastHoverElement {
			get {
				return this.lastHoverElement;
			}
			set {
				this.lastHoverElement = value;
			}
		}

		/// <summary>
		///   this BetterListViewItemDragStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewItemDragStateInfo.Empty);

		public BetterListViewItemDragStateInfo(BetterListViewDragDropType dragDropType, BetterListViewItemCollection sourceItems)
			: this(dragDropType, sourceItems, null) {
		}

		/// <summary>
		///   Initialize a new BetterListViewItemDragStateInfo instance.
		/// </summary>
		public BetterListViewItemDragStateInfo(BetterListViewDragDropType dragDropType, BetterListViewItemCollection sourceItems, BetterListViewElementBase lastHoverElement) {
			this.dragDropType = dragDropType;
			this.sourceItems = sourceItems;
			this.lastHoverElement = lastHoverElement;
		}

		public static bool operator ==(BetterListViewItemDragStateInfo itemDragStateInfoA, BetterListViewItemDragStateInfo itemDragStateInfoB) {
			return itemDragStateInfoA.Equals(itemDragStateInfoB);
		}

		public static bool operator !=(BetterListViewItemDragStateInfo itemDragStateInfoA, BetterListViewItemDragStateInfo itemDragStateInfoB) {
			return !itemDragStateInfoA.Equals(itemDragStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewItemDragStateInfo betterListViewItemDragStateInfo)) {
				return false;
			}
			if (this.dragDropType == betterListViewItemDragStateInfo.dragDropType && object.Equals(this.sourceItems, betterListViewItemDragStateInfo.sourceItems)) {
				return this.lastHoverElement == betterListViewItemDragStateInfo.lastHoverElement;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.dragDropType.GetHashCode() ^ ((this.sourceItems != null) ? this.sourceItems.Count.GetHashCode() : 0) ^ ((this.lastHoverElement != null) ? this.lastHoverElement.GetHashCode() : 0);
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{DragDropType = '{this.dragDropType}', SourceItems = '{this.sourceItems.ToString(writeContent: true)}', LastHoverElement = '{this.lastHoverElement}'}}";
		}
	}
}