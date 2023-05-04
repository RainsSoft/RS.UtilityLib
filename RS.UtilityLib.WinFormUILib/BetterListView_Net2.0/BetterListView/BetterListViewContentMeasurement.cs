using System.Drawing;

namespace ComponentOwl.BetterListView
{

	internal struct BetterListViewContentMeasurement
	{
		/// <summary>
		///   Empty BetterListViewContentMeasurement structure.
		/// </summary>
		public static BetterListViewContentMeasurement Empty = new BetterListViewContentMeasurement(overflowColumnsHorizontal: false, overflowColumnsVertical: false, overflowContentHorizontal: false, overflowContentVertical: false, Size.Empty);

		private readonly bool overflowColumnsHorizontal;

		private readonly bool overflowColumnsVertical;

		private readonly bool overflowContentHorizontal;

		private readonly bool overflowContentVertical;

		private readonly Size size;

		public bool OverflowColumnsHorizontal => this.overflowColumnsHorizontal;

		public bool OverflowColumnsVertical => this.overflowColumnsVertical;

		public bool OverflowContentHorizontal => this.overflowContentHorizontal;

		public bool OverflowContentVertical => this.overflowContentVertical;

		public Size Size => this.size;

		public bool IsEmpty => this.Equals(BetterListViewContentMeasurement.Empty);

		public BetterListViewContentMeasurement(bool overflowColumnsHorizontal, bool overflowColumnsVertical, bool overflowContentHorizontal, bool overflowContentVertical, Size size) {
			Checks.CheckSize(size, "Size");
			this.overflowColumnsHorizontal = overflowColumnsHorizontal;
			this.overflowColumnsVertical = overflowColumnsVertical;
			this.overflowContentHorizontal = overflowContentHorizontal;
			this.overflowContentVertical = overflowContentVertical;
			this.size = size;
		}

		public static bool operator ==(BetterListViewContentMeasurement contentMeasurementA, BetterListViewContentMeasurement contentMeasurementB) {
			return contentMeasurementA.Equals(contentMeasurementB);
		}

		public static bool operator !=(BetterListViewContentMeasurement contentMeasurementA, BetterListViewContentMeasurement contentMeasurementB) {
			return !contentMeasurementA.Equals(contentMeasurementB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewContentMeasurement betterListViewContentMeasurement)) {
				return false;
			}
			if (this.overflowColumnsHorizontal == betterListViewContentMeasurement.overflowColumnsHorizontal && this.overflowColumnsVertical == betterListViewContentMeasurement.overflowColumnsVertical && this.overflowContentHorizontal == betterListViewContentMeasurement.overflowContentHorizontal && this.overflowContentVertical == betterListViewContentMeasurement.overflowContentVertical) {
				return this.size == betterListViewContentMeasurement.size;
			}
			return false;
		}

		public override int GetHashCode() {
			int num = this.overflowColumnsHorizontal.GetHashCode() ^ this.overflowColumnsVertical.GetHashCode() ^ this.overflowContentHorizontal.GetHashCode() ^ this.overflowContentVertical.GetHashCode();
			Size size = this.size;
			return num ^ size.GetHashCode();
		}
	}
}