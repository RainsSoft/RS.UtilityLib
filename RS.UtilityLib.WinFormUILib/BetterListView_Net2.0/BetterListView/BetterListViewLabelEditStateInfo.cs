namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the LabelEdit state.
	/// </summary>
	internal struct BetterListViewLabelEditStateInfo
	{
		/// <summary>
		///   empty BetterListViewLabelEditStateInfo structure
		/// </summary>
		public static BetterListViewLabelEditStateInfo Empty = new BetterListViewLabelEditStateInfo(null, null);

		private BetterListViewSubItem subItem;

		private IBetterListViewEmbeddedControl control;

		/// <summary>
		///   sub-item being edited
		/// </summary>
		public BetterListViewSubItem SubItem {
			get {
				return this.subItem;
			}
			set {
				this.subItem = value;
			}
		}

		/// <summary>
		///   embedded edit control
		/// </summary>
		public IBetterListViewEmbeddedControl Control {
			get {
				return this.control;
			}
			set {
				this.control = value;
			}
		}

		/// <summary>
		///   this BetterListViewLabelEditStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewLabelEditStateInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewLabelEditStateInfo instance.
		/// </summary>
		/// <param name="subItem">sub-item being edited</param>
		/// <param name="control">embedded edit control</param>
		public BetterListViewLabelEditStateInfo(BetterListViewSubItem subItem, IBetterListViewEmbeddedControl control) {
			this.subItem = subItem;
			this.control = control;
		}

		public static bool operator ==(BetterListViewLabelEditStateInfo labelEditStateInfoA, BetterListViewLabelEditStateInfo labelEditStateInfoB) {
			return labelEditStateInfoA.Equals(labelEditStateInfoB);
		}

		public static bool operator !=(BetterListViewLabelEditStateInfo labelEditStateInfoA, BetterListViewLabelEditStateInfo labelEditStateInfoB) {
			return !labelEditStateInfoA.Equals(labelEditStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewLabelEditStateInfo betterListViewLabelEditStateInfo)) {
				return false;
			}
			if (this.subItem == betterListViewLabelEditStateInfo.subItem) {
				return this.control == betterListViewLabelEditStateInfo.control;
			}
			return false;
		}

		public override int GetHashCode() {
			return ((this.subItem != null) ? this.subItem.GetHashCode() : 0) ^ ((this.control != null) ? this.control.GetHashCode() : 0);
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{SubItem = '{this.subItem}', Control = '{this.control}'}}";
		}
	}
}