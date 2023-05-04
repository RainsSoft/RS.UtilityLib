using System.Drawing;

namespace ComponentOwl.BetterListView
{

	internal struct BetterListViewCommonMeasurementItems
	{
		public static readonly BetterListViewCommonMeasurementItems Empty = new BetterListViewCommonMeasurementItems(Size.Empty, Size.Empty, Size.Empty, 0, 0, empty: true);

		private readonly Size sizeExpandButton;

		private readonly Size sizeCheckBoxCheck;

		private readonly Size sizeCheckBoxRadio;

		private readonly int widthBase;

		private readonly int heightBase;

		private readonly bool empty;

		public Size SizeExpandButton => this.sizeExpandButton;

		public Size SizeCheckBoxCheck => this.sizeCheckBoxCheck;

		public Size SizeCheckBoxRadio => this.sizeCheckBoxRadio;

		public int WidthBase => this.widthBase;

		public int HeightBase => this.heightBase;

		public bool IsEmpty => this.empty;

		public BetterListViewCommonMeasurementItems(Size sizeExpandButton, Size sizeCheckBoxCheck, Size sizeCheckBoxRadio, int widthBase, int heightBase) {
			Checks.CheckSize(sizeExpandButton, "sizeExpandButton");
			Checks.CheckSize(sizeCheckBoxCheck, "sizeCheckBoxCheck");
			Checks.CheckSize(sizeCheckBoxRadio, "sizeCheckBoxRadio");
			Checks.CheckTrue(widthBase >= 0, "widthBase >= 0");
			Checks.CheckTrue(heightBase >= 0, "heightBase >= 0");
			this.sizeExpandButton = sizeExpandButton;
			this.sizeCheckBoxCheck = sizeCheckBoxCheck;
			this.sizeCheckBoxRadio = sizeCheckBoxRadio;
			this.widthBase = widthBase;
			this.heightBase = heightBase;
			this.empty = false;
		}

		private BetterListViewCommonMeasurementItems(Size sizeExpandButton, Size sizeCheckBoxCheck, Size sizeCheckBoxRadio, int widthBase, int heightBase, bool empty) {
			Checks.CheckSize(sizeExpandButton, "sizeExpandButton");
			Checks.CheckSize(sizeCheckBoxCheck, "sizeCheckBoxCheck");
			Checks.CheckSize(sizeCheckBoxRadio, "sizeCheckBoxRadio");
			Checks.CheckTrue(widthBase >= 0, "widthBase >= 0");
			Checks.CheckTrue(heightBase >= 0, "heightBase >= 0");
			this.sizeExpandButton = sizeExpandButton;
			this.sizeCheckBoxCheck = sizeCheckBoxCheck;
			this.sizeCheckBoxRadio = sizeCheckBoxRadio;
			this.widthBase = widthBase;
			this.heightBase = heightBase;
			this.empty = empty;
		}

		public static bool operator ==(BetterListViewCommonMeasurementItems commonMeasurementItemsA, BetterListViewCommonMeasurementItems commonMeasurementItemsB) {
			return commonMeasurementItemsA.Equals(commonMeasurementItemsB);
		}

		public static bool operator !=(BetterListViewCommonMeasurementItems commonMeasurementItemsA, BetterListViewCommonMeasurementItems commonMeasurementItemsB) {
			return !commonMeasurementItemsA.Equals(commonMeasurementItemsB);
		}

		public Size GetCheckBoxSize(BetterListViewCheckBoxAppearance checkBoxAppearance, bool nonEmpty) {
			switch (checkBoxAppearance) {
				case BetterListViewCheckBoxAppearance.CheckBox:
					return this.sizeCheckBoxCheck;
				case BetterListViewCheckBoxAppearance.RadioButton:
					return this.sizeCheckBoxRadio;
				default:
					if (!nonEmpty) {
						return Size.Empty;
					}
					return this.sizeCheckBoxCheck;
			}
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewCommonMeasurementItems betterListViewCommonMeasurementItems)) {
				return false;
			}
			if (this.sizeExpandButton != betterListViewCommonMeasurementItems.sizeExpandButton || this.sizeCheckBoxCheck != betterListViewCommonMeasurementItems.sizeCheckBoxCheck || this.sizeCheckBoxRadio != betterListViewCommonMeasurementItems.sizeCheckBoxRadio || this.widthBase != betterListViewCommonMeasurementItems.widthBase || this.heightBase != betterListViewCommonMeasurementItems.heightBase) {
				return false;
			}
			return true;
		}

		public override int GetHashCode() {
			Size size = this.sizeExpandButton;
			int hashCode = size.GetHashCode();
			size = this.sizeCheckBoxCheck;
			int num = hashCode ^ size.GetHashCode();
			size = this.sizeCheckBoxRadio;
			return num ^ size.GetHashCode() ^ this.widthBase.GetHashCode() ^ this.heightBase.GetHashCode();
		}
	}
}