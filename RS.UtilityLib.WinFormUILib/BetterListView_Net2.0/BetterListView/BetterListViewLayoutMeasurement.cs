namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Measurement of the layout.
	/// </summary>
	internal struct BetterListViewLayoutMeasurement
	{
		/// <summary>
		///   represents an empty BetterListViewLayoutMeasurement structure
		/// </summary>
		public static readonly BetterListViewLayoutMeasurement Empty = new BetterListViewLayoutMeasurement(1, 0, 0);

		private readonly int elementsPerRow;

		private readonly int width;

		private readonly int height;

		/// <summary>
		///   number of layout elements per row
		/// </summary>
		public int ElementsPerRow => this.elementsPerRow;

		/// <summary>
		///   layout width
		/// </summary>
		public int Width => this.width;

		/// <summary>
		///   layout height
		/// </summary>
		public int Height => this.height;

		/// <summary>
		///   this BetterListViewLayoutMeasurement structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewLayoutMeasurement.Empty);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutMeasurement" /> struct.
		/// </summary>
		/// <param name="elementsPerRow">number of layout elements per row</param>
		/// <param name="width">layout width</param>
		/// <param name="height">layout height.</param>
		public BetterListViewLayoutMeasurement(int elementsPerRow, int width, int height) {
			Checks.CheckTrue(elementsPerRow >= 1, "ElementsPerRow >= 1");
			Checks.CheckTrue(width >= 0, "width >= 0");
			Checks.CheckTrue(height >= 0, "height >= 0");
			this.elementsPerRow = elementsPerRow;
			this.width = width;
			this.height = height;
		}

		/// <summary>
		///   Test whether the two BetterListViewLayoutMeasurement objects are identical.
		/// </summary>
		/// <param name="layoutMeasurementA">first layout measurement to compare</param>
		/// <param name="layoutMeasurementB">second layout measurement compare</param>
		/// <returns>
		///   the two BetterListViewLayoutMeasurement objects are identical
		/// </returns>
		public static bool operator ==(BetterListViewLayoutMeasurement layoutMeasurementA, BetterListViewLayoutMeasurement layoutMeasurementB) {
			return layoutMeasurementA.Equals(layoutMeasurementB);
		}

		/// <summary>
		///   Test whether the two BetterListViewLayoutMeasurement objects are difference.
		/// </summary>
		/// <param name="layoutMeasurementA">first layout measurement to compare</param>
		/// <param name="layoutMeasurementB">second layout measurement compare</param>
		/// <returns>
		///   the two BetterListViewLayoutMeasurement objects are different
		/// </returns>
		public static bool operator !=(BetterListViewLayoutMeasurement layoutMeasurementA, BetterListViewLayoutMeasurement layoutMeasurementB) {
			return !layoutMeasurementA.Equals(layoutMeasurementB);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		///   true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">Another object to compare to. 
		/// </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewLayoutMeasurement betterListViewLayoutMeasurement)) {
				return false;
			}
			if (this.elementsPerRow == betterListViewLayoutMeasurement.elementsPerRow && this.width == betterListViewLayoutMeasurement.width) {
				return this.height == betterListViewLayoutMeasurement.height;
			}
			return false;
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		///   A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return this.elementsPerRow.GetHashCode() ^ this.width.GetHashCode() ^ this.height.GetHashCode();
		}
	}
}