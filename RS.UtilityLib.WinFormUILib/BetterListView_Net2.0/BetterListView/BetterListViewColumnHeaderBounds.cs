using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Column header boundaries.
	/// </summary>
	public sealed class BetterListViewColumnHeaderBounds : BetterListViewElementBounds
	{
		private Rectangle boundsBorder;

		private Rectangle boundsImageFrame;

		private Rectangle boundsImage;

		private Rectangle boundsText;

		private bool isTextShrunk;

		private Rectangle boundsSortGlyph;

		/// <summary>
		///   border area
		/// </summary>
		public Rectangle BoundsBorder {
			get {
				return this.boundsBorder;
			}
			set {
				this.boundsBorder = value;
			}
		}

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
		///   column header text is shrunk
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
		///   sort glyph area
		/// </summary>
		public Rectangle BoundsSortGlyph {
			get {
				return this.boundsSortGlyph;
			}
			set {
				this.boundsSortGlyph = value;
			}
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public override void Offset(Point offset) {
			this.boundsBorder = Painter.OffsetRectangle(this.boundsBorder, offset);
			this.boundsImageFrame = Painter.OffsetRectangle(this.boundsImageFrame, offset);
			this.boundsImage = Painter.OffsetRectangle(this.boundsImage, offset);
			this.boundsText = Painter.OffsetRectangle(this.boundsText, offset);
			this.boundsSortGlyph = Painter.OffsetRectangle(this.boundsSortGlyph, offset);
			base.Offset(offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public override void Reset() {
			this.boundsBorder = Rectangle.Empty;
			this.boundsImageFrame = Rectangle.Empty;
			this.boundsImage = Rectangle.Empty;
			this.boundsText = Rectangle.Empty;
			this.isTextShrunk = false;
			this.boundsSortGlyph = Rectangle.Empty;
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
			if (!(obj is BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds) || !base.Equals(obj)) {
				return false;
			}
			if (this.boundsBorder == betterListViewColumnHeaderBounds.boundsBorder && this.boundsImageFrame == betterListViewColumnHeaderBounds.boundsImageFrame && this.boundsImage == betterListViewColumnHeaderBounds.boundsImage && this.boundsText == betterListViewColumnHeaderBounds.boundsText && this.isTextShrunk == betterListViewColumnHeaderBounds.isTextShrunk) {
				return this.boundsSortGlyph == betterListViewColumnHeaderBounds.boundsSortGlyph;
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
			return this.boundsBorder.GetHashCode() ^ this.boundsImageFrame.GetHashCode() ^ this.boundsImage.GetHashCode() ^ this.boundsText.GetHashCode() ^ this.isTextShrunk.GetHashCode() ^ this.boundsSortGlyph.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override object Clone() {
			BetterListViewColumnHeaderBounds betterListViewColumnHeaderBounds = new BetterListViewColumnHeaderBounds();
			betterListViewColumnHeaderBounds.boundsBorder = this.boundsBorder;
			betterListViewColumnHeaderBounds.boundsImageFrame = this.boundsImageFrame;
			betterListViewColumnHeaderBounds.boundsImage = this.boundsImage;
			betterListViewColumnHeaderBounds.boundsText = this.boundsText;
			betterListViewColumnHeaderBounds.isTextShrunk = this.isTextShrunk;
			betterListViewColumnHeaderBounds.boundsSortGlyph = this.boundsSortGlyph;
			base.CopyTo(betterListViewColumnHeaderBounds);
			return betterListViewColumnHeaderBounds;
		}
	}
}