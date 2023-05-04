using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common Better ListView element boundaries.
	/// </summary>
	public abstract class BetterListViewElementBounds : BetterListViewElementBoundsBase
	{
		private Rectangle boundsInner;

		/// <summary>
		///   inner area
		/// </summary>
		public Rectangle BoundsInner {
			get {
				return this.boundsInner;
			}
			set {
				this.boundsInner = value;
			}
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public override void Offset(Point offset) {
			this.boundsInner = Painter.OffsetRectangle(this.boundsInner, offset);
			base.Offset(offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public override void Reset() {
			this.boundsInner = Rectangle.Empty;
			base.Reset();
		}

		/// <summary>
		///   Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewElementBounds betterListViewElementBounds) || !base.Equals(obj)) {
				return false;
			}
			return this.boundsInner == betterListViewElementBounds.boundsInner;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.boundsInner.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		///   Copy content of this instance to another BetterListViewElementBounds instance.
		/// </summary>
		/// <param name="elementBounds">BetterListViewElementBounds to copy the content to</param>
		protected void CopyTo(BetterListViewElementBounds elementBounds) {
			elementBounds.boundsInner = this.boundsInner;
			base.CopyTo(elementBounds);
		}
	}
}