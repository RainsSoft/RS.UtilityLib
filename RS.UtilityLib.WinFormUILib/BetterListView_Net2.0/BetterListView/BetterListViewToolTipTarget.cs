using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Information about location of a ToolTip.
	/// </summary>
	internal struct BetterListViewToolTipTarget
	{
		/// <summary>
		///   empty <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipTarget" /> instance
		/// </summary>
		public static readonly BetterListViewToolTipTarget Empty = new BetterListViewToolTipTarget(null, Rectangle.Empty);

		private readonly BetterListViewElementBase element;

		private readonly Rectangle bounds;

		/// <summary>
		///   element on which a ToolTip is displayed
		/// </summary>
		public BetterListViewElementBase Element => this.element;

		/// <summary>
		///   area in which the tooltip should be active
		/// </summary>
		public Rectangle Bounds => this.bounds;

		/// <summary>
		///   this <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipTarget" /> structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewToolTipTarget.Empty);

		/// <summary>
		///   Initialize a new <see cref="T:ComponentOwl.BetterListView.BetterListViewToolTipTarget" /> instance.
		/// </summary>
		/// <param name="element">element on which a ToolTip is displayed</param>
		/// <param name="bounds">area in which the tooltip should be active</param>
		public BetterListViewToolTipTarget(BetterListViewElementBase element, Rectangle bounds) {
			this.element = element;
			this.bounds = bounds;
		}

		public static bool operator ==(BetterListViewToolTipTarget betterListViewToolTipLocationA, BetterListViewToolTipTarget betterListViewToolTipLocationB) {
			return betterListViewToolTipLocationA.Equals(betterListViewToolTipLocationB);
		}

		public static bool operator !=(BetterListViewToolTipTarget betterListViewToolTipLocationA, BetterListViewToolTipTarget betterListViewToolTipLocationB) {
			return !betterListViewToolTipLocationA.Equals(betterListViewToolTipLocationB);
		}

		public override bool Equals(object obj) {
			if (!(obj is BetterListViewToolTipTarget betterListViewToolTipTarget)) {
				return false;
			}
			if (this.element == betterListViewToolTipTarget.element) {
				Rectangle rectangle = this.bounds;
				return rectangle.Equals(betterListViewToolTipTarget.bounds);
			}
			return false;
		}

		public override int GetHashCode() {
			int hashCode = this.element.GetHashCode();
			Rectangle rectangle = this.bounds;
			return hashCode ^ rectangle.GetHashCode();
		}
	}
}