using System.Drawing;

namespace ComponentOwl.BetterListView
{

	internal struct BetterListViewCommonMeasurementGroups
	{
		public static readonly BetterListViewCommonMeasurementGroups Empty = new BetterListViewCommonMeasurementGroups(0, Size.Empty);

		public int HeightOuter;

		public Size ContentSizeItems;

		public bool IsEmpty => this.Equals(BetterListViewCommonMeasurementGroups.Empty);

		public BetterListViewCommonMeasurementGroups(int heightOuter, Size contentSizeItems) {
			Checks.CheckTrue(heightOuter >= 0, "heightOuter >= 0");
			Checks.CheckSize(contentSizeItems, "contentSizeItems");
			this.HeightOuter = heightOuter;
			this.ContentSizeItems = contentSizeItems;
		}

		public static bool operator ==(BetterListViewCommonMeasurementGroups commonMeasurementGroupsA, BetterListViewCommonMeasurementGroups commonMeasurementGroupsB) {
			return commonMeasurementGroupsA.Equals(commonMeasurementGroupsB);
		}

		public static bool operator !=(BetterListViewCommonMeasurementGroups commonMeasurementGroupsA, BetterListViewCommonMeasurementGroups commonMeasurementGroupsB) {
			return !commonMeasurementGroupsA.Equals(commonMeasurementGroupsB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewCommonMeasurementGroups betterListViewCommonMeasurementGroups)) {
				return false;
			}
			if (this.HeightOuter == betterListViewCommonMeasurementGroups.HeightOuter) {
				return this.ContentSizeItems == betterListViewCommonMeasurementGroups.ContentSizeItems;
			}
			return false;
		}

		public override int GetHashCode() {
			return this.HeightOuter.GetHashCode() ^ this.ContentSizeItems.GetHashCode();
		}
	}
}