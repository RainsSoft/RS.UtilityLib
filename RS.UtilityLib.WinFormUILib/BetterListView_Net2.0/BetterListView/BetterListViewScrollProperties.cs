using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Encapsulates properties related to scrolling
	/// </summary>
	public struct BetterListViewScrollProperties
	{
		private readonly bool enabled;

		private readonly int largeChange;

		private readonly int maximum;

		private readonly int minimum;

		private readonly int smallChange;

		private readonly int value;

		private readonly bool visible;

		/// <summary>
		///   the scroll bar is enabled
		/// </summary>
		public bool Enabled => this.enabled;

		/// <summary>
		///   LargeChange property value of the scroll bar
		/// </summary>
		public int LargeChange => this.largeChange;

		/// <summary>
		///   Maximum property value of the scroll bar
		/// </summary>
		public int Maximum => this.maximum;

		/// <summary>
		///   Minimum property value of the scroll bar
		/// </summary>
		public int Minimum => this.minimum;

		/// <summary>
		///   SmallChange property value of the scroll bar
		/// </summary>
		public int SmallChange => this.smallChange;

		/// <summary>
		///   Value property value of the scroll bar
		/// </summary>
		public int Value => this.value;

		/// <summary>
		///   the scroll bar is visible
		/// </summary>
		public bool Visible => this.visible;

		/// <summary>
		///   Initialize a new BetterListViewScrollProperties instance.
		/// </summary>
		/// <param name="scrollBar">scroll bar to get properties from</param>
		public BetterListViewScrollProperties(ScrollBar scrollBar)
			: this(scrollBar.Enabled, scrollBar.LargeChange, scrollBar.Minimum, scrollBar.Maximum, scrollBar.SmallChange, scrollBar.Value, scrollBar.Visible) {
		}

		/// <summary>
		///   Initialize a new BetterListViewScrollProperties instance.
		/// </summary>
		/// <param name="enabled">the scroll bar is enabled</param>
		/// <param name="largeChange">LargeChange property value of the scroll bar</param>
		/// <param name="minimum">Minimum property value of the scroll bar</param>
		/// <param name="maximum">Maximum property value of the scroll bar</param>
		/// <param name="smallChange">SmallChange property value of the scroll bar</param>
		/// <param name="value">Value property value of the scroll bar</param>
		/// <param name="visible">the scroll bar is visible</param>
		public BetterListViewScrollProperties(bool enabled, int largeChange, int minimum, int maximum, int smallChange, int value, bool visible) {
			Checks.CheckTrue(largeChange >= 0, "largeChange >= 0");
			Checks.CheckTrue(minimum <= maximum, "minimum <= maximum");
			Checks.CheckBounds(smallChange, 0, largeChange, "smallChange");
			Checks.CheckBounds(value, minimum, maximum, "value");
			this.enabled = enabled;
			this.largeChange = largeChange;
			this.minimum = minimum;
			this.maximum = maximum;
			this.smallChange = smallChange;
			this.value = value;
			this.visible = visible;
		}

		/// <summary>
		///   Test whether the two BetterListViewScrollProperties objects are identical.
		/// </summary>
		/// <param name="scrollPropertiesA">first BetterListViewScrollProperties object</param>
		/// <param name="scrollPropertiesB">second BetterListViewScrollProperties object</param>
		/// <returns>the two BetterListViewScrollProperties objects are identical</returns>
		public static bool operator ==(BetterListViewScrollProperties scrollPropertiesA, BetterListViewScrollProperties scrollPropertiesB) {
			return scrollPropertiesA.Equals(scrollPropertiesB);
		}

		/// <summary>
		///   Test whether the two BetterListViewScrollProperties objects are different.
		/// </summary>
		/// <param name="scrollPropertiesA">first BetterListViewScrollProperties object</param>
		/// <param name="scrollPropertiesB">second BetterListViewScrollProperties object</param>
		/// <returns>the two BetterListViewScrollProperties objects are different</returns>
		public static bool operator !=(BetterListViewScrollProperties scrollPropertiesA, BetterListViewScrollProperties scrollPropertiesB) {
			return !scrollPropertiesA.Equals(scrollPropertiesB);
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
			if (!(obj is BetterListViewScrollProperties betterListViewScrollProperties)) {
				return false;
			}
			if (this.enabled.Equals(betterListViewScrollProperties.enabled) && this.largeChange.Equals(betterListViewScrollProperties.largeChange) && this.minimum.Equals(betterListViewScrollProperties.minimum) && this.maximum.Equals(betterListViewScrollProperties.maximum) && this.smallChange.Equals(betterListViewScrollProperties.smallChange) && this.value.Equals(betterListViewScrollProperties.value)) {
				return this.visible.Equals(betterListViewScrollProperties.visible);
			}
			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.enabled.GetHashCode() ^ this.largeChange.GetHashCode() ^ this.minimum.GetHashCode() ^ this.maximum.GetHashCode() ^ this.smallChange.GetHashCode() ^ this.value.GetHashCode() ^ this.visible.GetHashCode();
		}
	}
}