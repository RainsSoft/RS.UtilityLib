namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the ItemBeforeCheckKeyboard/ItemBeforeCheckMouse state.
	/// </summary>
	internal struct BetterListViewItemBeforeCheckStateInfo
	{
		/// <summary>
		///   empty BetterListViewItemBeforeCheckStateInfo structure
		/// </summary>
		public static readonly BetterListViewItemBeforeCheckStateInfo Empty = new BetterListViewItemBeforeCheckStateInfo(null, new BetterListViewReadOnlyItemSet());

		private BetterListViewItem item;

		private BetterListViewReadOnlyItemSet items;

		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				this.item = value;
			}
		}

		/// <summary>
		///   other selected items to be checked
		/// </summary>
		public BetterListViewReadOnlyItemSet Items {
			get {
				return this.items;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.items = value;
			}
		}

		/// <summary>
		///   this BetterListViewItemBeforeCheckStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewItemBeforeCheckStateInfo.Empty);

		public BetterListViewItemBeforeCheckStateInfo(BetterListViewItem item, BetterListViewReadOnlyItemSet items) {
			Checks.CheckNotNull(items, "items");
			this.item = item;
			this.items = items;
		}

		public static bool operator ==(BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfoA, BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfoB) {
			return itemBeforeCheckStateInfoA.Equals(itemBeforeCheckStateInfoB);
		}

		public static bool operator !=(BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfoA, BetterListViewItemBeforeCheckStateInfo itemBeforeCheckStateInfoB) {
			return !itemBeforeCheckStateInfoA.Equals(itemBeforeCheckStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewItemBeforeCheckStateInfo betterListViewItemBeforeCheckStateInfo)) {
				return false;
			}
			if (this.item == betterListViewItemBeforeCheckStateInfo.item) {
				return this.items.EqualsContent(betterListViewItemBeforeCheckStateInfo.items);
			}
			return false;
		}

		public override int GetHashCode() {
			return ((this.item != null) ? this.item.GetHashCode() : 0) ^ this.items.Count.GetHashCode();
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{Item = '{this.item}', Items = '{this.items.ToString(writeContent: true)}'}}";
		}
	}
}