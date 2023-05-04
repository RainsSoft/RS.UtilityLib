using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Group boundaries.
	/// </summary>
	public sealed class BetterListViewGroupBounds : BetterListViewElementBounds
	{
		private Rectangle boundsSelection = Rectangle.Empty;

		private Rectangle boundsImageFrame = Rectangle.Empty;

		private Rectangle boundsImage = Rectangle.Empty;

		private Rectangle boundsText = Rectangle.Empty;

		private bool isTextShrunk;

		/// <summary>
		///   selection area
		/// </summary>
		public Rectangle BoundsSelection {
			get {
				return this.boundsSelection;
			}
			set {
				this.boundsSelection = value;
			}
		}

		/// <summary>
		///   expand button area
		/// </summary>
		internal Rectangle BoundsExpandButton => Rectangle.Empty;

		/// <summary>
		///   image frame area
		/// </summary>
		public Rectangle BoundsImageFrame {
			get {
				return this.boundsImageFrame;
			}
			set {
				this.boundsImageFrame = value;
			}
		}

		/// <summary>
		///   image area
		/// </summary>
		public Rectangle BoundsImage {
			get {
				return this.boundsImage;
			}
			set {
				this.boundsImage = value;
			}
		}

		/// <summary>
		///   text area
		/// </summary>
		public Rectangle BoundsText {
			get {
				return this.boundsText;
			}
			set {
				this.boundsText = value;
			}
		}

		/// <summary>
		///   group text is shrunk
		/// </summary>
		public bool IsTextShrunk {
			get {
				return this.isTextShrunk;
			}
			set {
				this.isTextShrunk = value;
			}
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public override void Offset(Point offset) {
			this.boundsSelection = Painter.OffsetRectangle(this.boundsSelection, offset);
			this.boundsImageFrame = Painter.OffsetRectangle(this.boundsImageFrame, offset);
			this.boundsImage = Painter.OffsetRectangle(this.boundsImage, offset);
			this.boundsText = Painter.OffsetRectangle(this.boundsText, offset);
			base.Offset(offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public override void Reset() {
			this.boundsSelection = Rectangle.Empty;
			this.boundsImageFrame = Rectangle.Empty;
			this.boundsImage = Rectangle.Empty;
			this.boundsText = Rectangle.Empty;
			this.isTextShrunk = false;
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
			if (!(obj is BetterListViewGroupBounds betterListViewGroupBounds) || !base.Equals(obj)) {
				return false;
			}
			if (this.boundsSelection == betterListViewGroupBounds.boundsSelection && this.boundsImageFrame == betterListViewGroupBounds.boundsImageFrame && this.boundsImage == betterListViewGroupBounds.boundsImage && this.boundsText == betterListViewGroupBounds.boundsText) {
				return this.isTextShrunk == betterListViewGroupBounds.isTextShrunk;
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
			return this.boundsSelection.GetHashCode() ^ this.boundsImageFrame.GetHashCode() ^ this.boundsImage.GetHashCode() ^ this.boundsText.GetHashCode() ^ this.isTextShrunk.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override object Clone() {
			BetterListViewGroupBounds betterListViewGroupBounds = new BetterListViewGroupBounds();
			betterListViewGroupBounds.boundsSelection = this.boundsSelection;
			betterListViewGroupBounds.boundsImageFrame = this.boundsImageFrame;
			betterListViewGroupBounds.boundsImage = this.boundsImage;
			betterListViewGroupBounds.boundsText = this.boundsText;
			betterListViewGroupBounds.isTextShrunk = this.isTextShrunk;
			base.CopyTo(betterListViewGroupBounds);
			return betterListViewGroupBounds;
		}
	}
}