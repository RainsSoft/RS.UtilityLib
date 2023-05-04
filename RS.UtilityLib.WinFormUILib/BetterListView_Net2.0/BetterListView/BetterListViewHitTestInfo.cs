using System;
using System.Text;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Represents a state information about elements in relation to mouse position and buttons pressed.
	/// </summary>
	public struct BetterListViewHitTestInfo
	{
		/// <summary>
		///   empty BetterListViewHitTestInfo structure
		/// </summary>
		public static readonly BetterListViewHitTestInfo Empty = new BetterListViewHitTestInfo(BetterListViewHitTestLocations.Custom, BetterListViewColumnHeaderStateInfo.Empty, BetterListViewItemStateInfo.Empty, BetterListViewGroupStateInfo.Empty, null, BetterListViewHitPart.Undefined, null, BetterListViewHitPart.Undefined, null, BetterListViewHitPart.Undefined, null, BetterListViewHitPart.Undefined, null, BetterListViewHitPart.Undefined);

		private readonly BetterListViewHitTestLocations locations;

		private readonly BetterListViewColumnHeaderStateInfo columnHeaderStateInfo;

		private readonly BetterListViewItemStateInfo itemStateInfo;

		private readonly BetterListViewGroupStateInfo groupStateInfo;

		private readonly BetterListViewColumnHeader columnHeader;

		private readonly BetterListViewHitPart columnHeaderPart;

		private readonly BetterListViewItem itemDisplay;

		private readonly BetterListViewHitPart itemPartDisplay;

		private readonly BetterListViewItem itemSelection;

		private readonly BetterListViewHitPart itemPartSelection;

		private readonly BetterListViewSubItem subItem;

		private readonly BetterListViewHitPart subItemPart;

		private readonly BetterListViewGroup group;

		private readonly BetterListViewHitPart groupPart;

		/// <summary>
		///   this BetterListViewHitTestInfo structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewHitTestInfo.Empty);

		/// <summary>
		///   locations of mouse pointer
		/// </summary>
		public BetterListViewHitTestLocations Locations => this.locations;

		/// <summary>
		///   state of a column header (if located)
		/// </summary>
		public BetterListViewColumnHeaderStateInfo ColumnHeaderStateInfo => this.columnHeaderStateInfo;

		/// <summary>
		///   state of an item (if located)
		/// </summary>
		public BetterListViewItemStateInfo ItemStateInfo => this.itemStateInfo;

		/// <summary>
		///   state of a group (if located)
		/// </summary>
		public BetterListViewGroupStateInfo GroupStateInfo => this.groupStateInfo;

		/// <summary>
		///   located column header
		/// </summary>
		public BetterListViewColumnHeader ColumnHeader => this.columnHeader;

		/// <summary>
		///   located part of a column header (if a column header itself is located)
		/// </summary>
		public BetterListViewHitPart ColumnHeaderPart => this.columnHeaderPart;

		/// <summary>
		///   located item
		/// </summary>
		public BetterListViewItem ItemDisplay => this.itemDisplay;

		/// <summary>
		///   located part of an item (if an item itself is located)
		/// </summary>
		public BetterListViewHitPart ItemPartDisplay => this.itemPartDisplay;

		/// <summary>
		///   located selectable item
		/// </summary>
		public BetterListViewItem ItemSelection => this.itemSelection;

		/// <summary>
		///   located part of a selectable item (if an item itself is located)
		/// </summary>
		public BetterListViewHitPart ItemPartSelection => this.itemPartSelection;

		/// <summary>
		///   located sub-item
		/// </summary>
		public BetterListViewSubItem SubItem => this.subItem;

		/// <summary>
		///   located part of a sub-item (if a sub-item itself is located)
		/// </summary>
		public BetterListViewHitPart SubItemPart => this.subItemPart;

		/// <summary>
		///   located group
		/// </summary>
		public BetterListViewGroup Group => this.group;

		/// <summary>
		///   located group part
		/// </summary>
		public BetterListViewHitPart GroupPart => this.groupPart;

		internal BetterListViewHitTestInfo(BetterListViewHitTestLocations locations, BetterListViewColumnHeaderStateInfo columnHeaderStateInfo, BetterListViewItemStateInfo itemStateInfo, BetterListViewGroupStateInfo groupStateInfo, BetterListViewColumnHeader columnHeader, BetterListViewHitPart columnHeaderPart, BetterListViewItem itemDisplay, BetterListViewHitPart itemPartDisplay, BetterListViewItem itemSelection, BetterListViewHitPart itemPartSelection, BetterListViewSubItem subItem, BetterListViewHitPart subItemPart, BetterListViewGroup group, BetterListViewHitPart groupPart) {
			this.locations = locations;
			this.columnHeaderStateInfo = columnHeaderStateInfo;
			this.itemStateInfo = itemStateInfo;
			this.groupStateInfo = groupStateInfo;
			this.columnHeader = columnHeader;
			this.columnHeaderPart = columnHeaderPart;
			this.itemDisplay = itemDisplay;
			this.itemPartDisplay = itemPartDisplay;
			this.itemSelection = itemSelection;
			this.itemPartSelection = itemPartSelection;
			this.subItem = subItem;
			this.subItemPart = subItemPart;
			this.group = group;
			this.groupPart = groupPart;
		}

		/// <summary>
		///   Test whether the two BetterListViewHitTestInfo objects are identical.
		/// </summary>
		/// <param name="hitTestInfoA"> first BetterListViewHitTestInfo object </param>
		/// <param name="hitTestInfoB"> second BetterListViewHitTestInfo object </param>
		/// <returns> the two BetterListViewHitTestInfo objects are identical </returns>
		public static bool operator ==(BetterListViewHitTestInfo hitTestInfoA, BetterListViewHitTestInfo hitTestInfoB) {
			return hitTestInfoA.Equals(hitTestInfoB);
		}

		/// <summary>
		///   Test whether the two BetterListViewHitTestInfo objects are different.
		/// </summary>
		/// <param name="hitTestInfoA"> first BetterListViewHitTestInfo object </param>
		/// <param name="hitTestInfoB"> second BetterListViewHitTestInfo object </param>
		/// <returns> the two BetterListViewHitTestInfo objects are different </returns>
		public static bool operator !=(BetterListViewHitTestInfo hitTestInfoA, BetterListViewHitTestInfo hitTestInfoB) {
			return !hitTestInfoA.Equals(hitTestInfoB);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns> true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
		/// <param name="obj"> Another object to compare to. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewHitTestInfo betterListViewHitTestInfo)) {
				return false;
			}
			if (this.locations == betterListViewHitTestInfo.locations && this.columnHeaderStateInfo == betterListViewHitTestInfo.columnHeaderStateInfo && this.itemStateInfo == betterListViewHitTestInfo.itemStateInfo && this.groupStateInfo == betterListViewHitTestInfo.groupStateInfo && this.columnHeader == betterListViewHitTestInfo.columnHeader && this.columnHeaderPart == betterListViewHitTestInfo.columnHeaderPart && this.itemDisplay == betterListViewHitTestInfo.itemDisplay && this.itemPartDisplay == betterListViewHitTestInfo.itemPartDisplay && this.itemSelection == betterListViewHitTestInfo.itemSelection && this.itemPartSelection == betterListViewHitTestInfo.itemPartSelection && this.subItem == betterListViewHitTestInfo.subItem && this.subItemPart == betterListViewHitTestInfo.subItemPart && this.group == betterListViewHitTestInfo.group) {
				return this.groupPart == betterListViewHitTestInfo.groupPart;
			}
			return false;
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		/// <returns> A 32-bit signed integer that is the hash code for this instance. </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			int hashCode = this.locations.GetHashCode();
			BetterListViewColumnHeaderStateInfo betterListViewColumnHeaderStateInfo = this.columnHeaderStateInfo;
			int num = hashCode ^ betterListViewColumnHeaderStateInfo.GetHashCode();
			BetterListViewItemStateInfo betterListViewItemStateInfo = this.itemStateInfo;
			int num2 = num ^ betterListViewItemStateInfo.GetHashCode();
			BetterListViewGroupStateInfo betterListViewGroupStateInfo = this.groupStateInfo;
			return num2 ^ betterListViewGroupStateInfo.GetHashCode() ^ ((this.columnHeader != null) ? this.columnHeader.GetHashCode() : 0) ^ this.columnHeaderPart.GetHashCode() ^ ((this.itemDisplay != null) ? this.itemDisplay.GetHashCode() : 0) ^ this.itemPartDisplay.GetHashCode() ^ ((this.itemSelection != null) ? this.itemSelection.GetHashCode() : 0) ^ this.itemPartSelection.GetHashCode() ^ ((this.subItem != null) ? this.subItem.GetHashCode() : 0) ^ this.subItemPart.GetHashCode() ^ ((this.group != null) ? this.group.GetHashCode() : 0) ^ this.groupPart.GetHashCode();
		}

		/// <summary>
		///   Returns a <see cref="T:System.String" /> that represents this instance.
		/// </summary>
		/// <returns> A <see cref="T:System.String" /> that represents this instance. </returns>
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsEmpty) {
				stringBuilder.Append(base.GetType().Name + ": {}");
			}
			else {
				stringBuilder.AppendLine(base.GetType().Name + ": {");
				stringBuilder.AppendLine($"Locations = '{this.locations}',");
				stringBuilder.AppendLine($"ColumnHeaderStateInfo = '{this.columnHeaderStateInfo}',");
				stringBuilder.AppendLine($"ItemStateInfo = '{this.itemStateInfo}',");
				if (this.columnHeader != null) {
					stringBuilder.AppendLine($"ColumnHeader = '{this.columnHeader}',");
					stringBuilder.AppendLine($"ColumnHeaderPart = '{this.columnHeaderPart}',");
				}
				if (this.itemDisplay != null) {
					stringBuilder.AppendLine($"ItemDisplay = '{this.itemDisplay}',");
					stringBuilder.AppendLine($"ItemPartDisplay = '{this.itemPartDisplay}',");
				}
				if (this.itemSelection != null) {
					stringBuilder.AppendLine($"ItemSelection = '{this.itemSelection}',");
					stringBuilder.AppendLine($"ItemPartSelection = '{this.itemPartSelection}',");
				}
				if (this.group != null) {
					stringBuilder.AppendLine($"Group = '{this.group}',");
					stringBuilder.AppendLine($"GroupPart = '{this.groupPart}',");
				}
				stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length - 1, Environment.NewLine.Length + 1);
				stringBuilder.AppendLine("}");
			}
			return stringBuilder.ToString();
		}
	}
}