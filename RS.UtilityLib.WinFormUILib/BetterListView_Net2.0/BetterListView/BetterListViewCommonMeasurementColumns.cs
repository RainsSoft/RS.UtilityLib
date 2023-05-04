using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	internal struct BetterListViewCommonMeasurementColumns
	{
		public static readonly BetterListViewCommonMeasurementColumns Empty = new BetterListViewCommonMeasurementColumns(Size.Empty, Padding.Empty, 0, 0, new Dictionary<int, int>());

		public Size SizeSortGlyph;

		public Padding PaddingSortGlyph;

		public int HeightOuter;

		public int HeightOuterMinimum;

		public IDictionary<int, int> WidthsText;

		public bool IsEmpty => this.Equals(BetterListViewCommonMeasurementColumns.Empty);

		public BetterListViewCommonMeasurementColumns(Size sizeSortGlyph, Padding paddingSortGlyph, int heightOuter, int heightOuterMinimum, IDictionary<int, int> widthsText) {
			Checks.CheckSize(sizeSortGlyph, "sizeSortGlyph");
			Checks.CheckPadding(paddingSortGlyph, "paddingSortGlyph");
			Checks.CheckTrue(heightOuter >= 0, "heightOuter >= 0");
			Checks.CheckTrue(heightOuterMinimum >= 0, "heightOuterMinimum >= 0");
			Checks.CheckNotNull(widthsText, "widthsText");
			foreach (KeyValuePair<int, int> item in widthsText) {
				int key = item.Key;
				int value = item.Value;
				Checks.CheckTrue(key >= 0, "index >= 0");
				Checks.CheckTrue(value >= 0, "width >= 0");
			}
			this.SizeSortGlyph = sizeSortGlyph;
			this.PaddingSortGlyph = paddingSortGlyph;
			this.HeightOuter = heightOuter;
			this.HeightOuterMinimum = heightOuterMinimum;
			this.WidthsText = widthsText;
		}

		public static bool operator ==(BetterListViewCommonMeasurementColumns commonMeasurementColumnsA, BetterListViewCommonMeasurementColumns commonMeasurementColumnsB) {
			return commonMeasurementColumnsA.Equals(commonMeasurementColumnsB);
		}

		public static bool operator !=(BetterListViewCommonMeasurementColumns commonMeasurementColumnsA, BetterListViewCommonMeasurementColumns commonMeasurementColumnsB) {
			return !commonMeasurementColumnsA.Equals(commonMeasurementColumnsB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewCommonMeasurementColumns betterListViewCommonMeasurementColumns)) {
				return false;
			}
			if (this.SizeSortGlyph != betterListViewCommonMeasurementColumns.SizeSortGlyph || this.PaddingSortGlyph != betterListViewCommonMeasurementColumns.PaddingSortGlyph || this.HeightOuter != betterListViewCommonMeasurementColumns.HeightOuter || this.HeightOuterMinimum != betterListViewCommonMeasurementColumns.HeightOuterMinimum || this.WidthsText.Count != betterListViewCommonMeasurementColumns.WidthsText.Count) {
				return false;
			}
			IEnumerator<KeyValuePair<int, int>> enumerator = this.WidthsText.GetEnumerator();
			IEnumerator<KeyValuePair<int, int>> enumerator2 = betterListViewCommonMeasurementColumns.WidthsText.GetEnumerator();
			enumerator.Reset();
			enumerator2.Reset();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				KeyValuePair<int, int> current = enumerator.Current;
				KeyValuePair<int, int> current2 = enumerator2.Current;
				if (current.Key != current2.Key || current.Value != current2.Value) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			int num = this.SizeSortGlyph.GetHashCode() ^ this.PaddingSortGlyph.GetHashCode() ^ this.HeightOuter.GetHashCode() ^ this.HeightOuterMinimum.GetHashCode();
			foreach (KeyValuePair<int, int> item in this.WidthsText) {
				num ^= item.Key ^ item.Value;
			}
			return num;
		}
	}
}