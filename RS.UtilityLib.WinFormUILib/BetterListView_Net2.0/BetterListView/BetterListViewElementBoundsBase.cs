using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   BetterListView element boundaries.
	/// </summary>
	public abstract class BetterListViewElementBoundsBase : ICloneable
	{
		private Rectangle boundsSpacing;

		private Rectangle boundsOuter;

		private Rectangle boundsOuterExtended;

		/// <summary>
		///   spacing area
		/// </summary>
		public Rectangle BoundsSpacing {
			get {
				return this.boundsSpacing;
			}
			set {
				this.boundsSpacing = value;
			}
		}

		/// <summary>
		///   outer area
		/// </summary>
		public Rectangle BoundsOuter {
			get {
				return this.boundsOuter;
			}
			set {
				this.boundsOuter = value;
			}
		}

		/// <summary>
		/// extended outer area
		/// </summary>
		public Rectangle BoundsOuterExtended {
			get {
				return this.boundsOuterExtended;
			}
			set {
				this.boundsOuterExtended = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewElementBoundsBase" /> class.
		/// </summary>
		protected BetterListViewElementBoundsBase() {
			this.Reset();
		}

		/// <summary>
		///   Move boundaries to the specified location.
		/// </summary>
		/// <param name="location">target location</param>
		public void Relocate(Point location) {
			this.Offset(new Point(-this.boundsOuter.Left, -this.boundsOuter.Top));
			this.Offset(location);
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public virtual void Offset(Point offset) {
			this.boundsSpacing = Painter.OffsetRectangle(this.boundsSpacing, offset);
			this.boundsOuter = Painter.OffsetRectangle(this.boundsOuter, offset);
			this.boundsOuterExtended = Painter.OffsetRectangle(this.boundsOuterExtended, offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public virtual void Reset() {
			this.boundsSpacing = Rectangle.Empty;
			this.boundsOuter = Rectangle.Empty;
			this.boundsOuterExtended = Rectangle.Empty;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj" /> parameter is null.
		///   </exception>
		public override bool Equals(object obj) {
			if (!(obj is BetterListViewElementBoundsBase betterListViewElementBoundsBase)) {
				return false;
			}
			if (this.boundsSpacing == betterListViewElementBoundsBase.boundsSpacing && this.boundsOuter == betterListViewElementBoundsBase.boundsOuter) {
				return this.boundsOuterExtended == betterListViewElementBoundsBase.boundsOuterExtended;
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.boundsSpacing.GetHashCode() ^ this.boundsOuter.GetHashCode() ^ this.boundsOuterExtended.GetHashCode();
		}

		/// <summary>
		///   Copy content of this instance to the specified BetterListViewElementBounds instance.
		/// </summary>
		/// <param name="elementBounds">BetterListViewElementBounds instance to copy the content to</param>
		protected void CopyTo(BetterListViewElementBoundsBase elementBounds) {
			elementBounds.boundsSpacing = this.boundsSpacing;
			elementBounds.boundsOuter = this.boundsOuter;
			elementBounds.boundsOuterExtended = this.boundsOuterExtended;
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public abstract object Clone();
	}
}