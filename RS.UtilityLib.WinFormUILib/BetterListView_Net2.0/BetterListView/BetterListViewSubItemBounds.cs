using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Sub-item boundaries.
	/// </summary>
	public sealed class BetterListViewSubItemBounds : BetterListViewElementBounds
	{
		private Rectangle boundsImageFrame = Rectangle.Empty;

		private Rectangle boundsImage = Rectangle.Empty;

		private Rectangle boundsText = Rectangle.Empty;

		private int maximumTextLines;

		private bool isTextShrunk;

		private Rectangle boundsCell = Rectangle.Empty;

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
		///   maximum allowed lines of sub-item label
		/// </summary>
		public int MaximumTextLines {
			get {
				return this.maximumTextLines;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.maximumTextLines = value;
			}
		}

		/// <summary>
		///   sub-item text is shrunk
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
		///   cell area
		/// </summary>
		public Rectangle BoundsCell {
			get {
				return this.boundsCell;
			}
			set {
				this.boundsCell = value;
			}
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public override void Offset(Point offset) {
			this.boundsImageFrame = Painter.OffsetRectangle(this.boundsImageFrame, offset);
			this.boundsImage = Painter.OffsetRectangle(this.boundsImage, offset);
			this.boundsText = Painter.OffsetRectangle(this.boundsText, offset);
			this.boundsCell = Painter.OffsetRectangle(this.boundsCell, offset);
			base.Offset(offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public override void Reset() {
			this.boundsImageFrame = Rectangle.Empty;
			this.boundsImage = Rectangle.Empty;
			this.boundsText = Rectangle.Empty;
			this.maximumTextLines = 1;
			this.isTextShrunk = false;
			this.boundsCell = Rectangle.Empty;
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
			if (!(obj is BetterListViewSubItemBounds betterListViewSubItemBounds) || !base.Equals(obj)) {
				return false;
			}
			if (this.boundsImageFrame == betterListViewSubItemBounds.boundsImageFrame && this.boundsImage == betterListViewSubItemBounds.boundsImage && this.boundsText == betterListViewSubItemBounds.boundsText && this.maximumTextLines == betterListViewSubItemBounds.maximumTextLines && this.isTextShrunk == betterListViewSubItemBounds.isTextShrunk) {
				return this.boundsCell == betterListViewSubItemBounds.boundsCell;
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
			return this.boundsImageFrame.GetHashCode() ^ this.boundsImage.GetHashCode() ^ this.boundsText.GetHashCode() ^ this.maximumTextLines.GetHashCode() ^ this.isTextShrunk.GetHashCode() ^ this.boundsCell.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override object Clone() {
			BetterListViewSubItemBounds betterListViewSubItemBounds = new BetterListViewSubItemBounds();
			betterListViewSubItemBounds.boundsImageFrame = this.boundsImageFrame;
			betterListViewSubItemBounds.boundsImage = this.boundsImage;
			betterListViewSubItemBounds.boundsText = this.boundsText;
			betterListViewSubItemBounds.maximumTextLines = this.maximumTextLines;
			betterListViewSubItemBounds.isTextShrunk = this.isTextShrunk;
			betterListViewSubItemBounds.boundsCell = this.boundsCell;
			base.CopyTo(betterListViewSubItemBounds);
			return betterListViewSubItemBounds;
		}
	}
}