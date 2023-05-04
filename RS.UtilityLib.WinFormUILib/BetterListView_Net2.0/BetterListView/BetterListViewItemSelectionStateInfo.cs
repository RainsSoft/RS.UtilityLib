using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents state information for the ItemBeforeSelection/ItemSelection state.
	/// </summary>
	internal struct BetterListViewItemSelectionStateInfo
	{
		/// <summary>
		///   empty BetterListViewItemSelectionStateInfo structure
		/// </summary>
		public static BetterListViewItemSelectionStateInfo Empty = new BetterListViewItemSelectionStateInfo(Point.Empty, Point.Empty, new BetterListViewReadOnlyGroupSet(), new BetterListViewReadOnlyItemSet(), modifierControl: false, modifierShift: false);

		private Point startPoint;

		private Point scrollPosition;

		private BetterListViewReadOnlyGroupSet selectedGroups;

		private BetterListViewReadOnlyItemSet selectedItems;

		private bool modifierControl;

		private bool modifierShift;

		/// <summary>
		///   mouse cursor position
		/// </summary>
		public Point StartPoint {
			get {
				return this.startPoint;
			}
			set {
				Checks.CheckTrue(value.X >= 0 && value.Y >= 0, "value.X >= 0 && value.Y >= 0");
				this.startPoint = value;
			}
		}

		/// <summary>
		///   postion of the scroll bars
		/// </summary>
		public Point ScrollPosition {
			get {
				return this.scrollPosition;
			}
			set {
				Checks.CheckTrue(value.X >= 0 && value.Y >= 0, "value.X >= 0 && value.Y >= 0");
				this.scrollPosition = value;
			}
		}

		/// <summary>
		///   selected groups
		/// </summary>
		public BetterListViewReadOnlyGroupSet SelectedGroups {
			get {
				return this.selectedGroups;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.selectedGroups = value;
			}
		}

		/// <summary>
		///   selected items
		/// </summary>
		public BetterListViewReadOnlyItemSet SelectedItems {
			get {
				return this.selectedItems;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.selectedItems = value;
			}
		}

		/// <summary>
		///   Control modifier key is pressed
		/// </summary>
		public bool ModifierControl {
			get {
				return this.modifierControl;
			}
			set {
				this.modifierControl = value;
			}
		}

		/// <summary>
		///   Shift modifier key is pressed
		/// </summary>
		public bool ModifierShift {
			get {
				return this.modifierShift;
			}
			set {
				this.modifierShift = value;
			}
		}

		/// <summary>
		///   this BetterListViewItemSelectionStateInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewItemSelectionStateInfo.Empty);

		/// <summary>
		///   Initialize a new BetterListViewItemSelectionStateInfo instance.
		/// </summary>
		/// <param name="startPoint">mouse cursor position</param>
		/// <param name="scrollPosition">postion of the scroll bars</param>
		/// <param name="selectedGroups">selected groups</param>
		/// <param name="selectedItems">selected items</param>
		/// <param name="modifierControl">Control modifier key is pressed</param>
		/// <param name="modifierShift">Shift modifier key is pressed</param>
		public BetterListViewItemSelectionStateInfo(Point startPoint, Point scrollPosition, BetterListViewReadOnlyGroupSet selectedGroups, BetterListViewReadOnlyItemSet selectedItems, bool modifierControl, bool modifierShift) {
			Checks.CheckTrue(startPoint.X >= 0 && startPoint.Y >= 0, "startPoint.X >= 0 && startPoint.Y >= 0");
			Checks.CheckTrue(scrollPosition.X >= 0 && scrollPosition.Y >= 0, "scrollPosition.X >= 0 && scrollPosition.Y >= 0");
			Checks.CheckNotNull(selectedGroups, "selectedGroups");
			Checks.CheckNotNull(selectedItems, "selectedItems");
			this.startPoint = startPoint;
			this.scrollPosition = scrollPosition;
			this.selectedGroups = selectedGroups;
			this.selectedItems = selectedItems;
			this.modifierControl = modifierControl;
			this.modifierShift = modifierShift;
		}

		public static bool operator ==(BetterListViewItemSelectionStateInfo itemSelectionStateInfoA, BetterListViewItemSelectionStateInfo itemSelectionStateInfoB) {
			return itemSelectionStateInfoA.Equals(itemSelectionStateInfoB);
		}

		public static bool operator !=(BetterListViewItemSelectionStateInfo itemSelectionStateInfoA, BetterListViewItemSelectionStateInfo itemSelectionStateInfoB) {
			return !itemSelectionStateInfoA.Equals(itemSelectionStateInfoB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewItemSelectionStateInfo betterListViewItemSelectionStateInfo)) {
				return false;
			}
			if (this.startPoint == betterListViewItemSelectionStateInfo.startPoint && this.scrollPosition == betterListViewItemSelectionStateInfo.scrollPosition && this.selectedGroups.EqualsContent(betterListViewItemSelectionStateInfo.selectedGroups) && this.selectedItems.EqualsContent(betterListViewItemSelectionStateInfo.selectedItems) && this.modifierControl == betterListViewItemSelectionStateInfo.modifierControl) {
				return this.modifierShift == betterListViewItemSelectionStateInfo.modifierShift;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.startPoint.GetHashCode() ^ this.scrollPosition.GetHashCode() ^ this.selectedGroups.Count.GetHashCode() ^ this.selectedItems.Count.GetHashCode() ^ this.modifierControl.GetHashCode() ^ this.modifierShift.GetHashCode();
		}

		public override string ToString() {
			if (this.IsEmpty) {
				return base.GetType().Name + ": {}";
			}
			return $"{base.GetType().Name}: {{StartPoint = '{this.startPoint}', ScrollPosition = '{this.scrollPosition}', SelectedGroups = '{this.selectedGroups.ToString(writeContent: true)}', SelectedItems = '{this.selectedItems.ToString(writeContent: true)}', ModifierControl = '{this.modifierControl}', ModifierShift = '{this.modifierShift}'}}";
		}
	}
}