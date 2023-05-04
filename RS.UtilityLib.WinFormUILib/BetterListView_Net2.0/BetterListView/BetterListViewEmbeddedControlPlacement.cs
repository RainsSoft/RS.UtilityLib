using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Embedded control placement information.
	/// </summary>
	public struct BetterListViewEmbeddedControlPlacement
	{
		/// <summary>
		///   an empty BetterListViewEmbeddedControlPlacement structure
		/// </summary>
		public static readonly BetterListViewEmbeddedControlPlacement Empty = new BetterListViewEmbeddedControlPlacement(Rectangle.Empty, Rectangle.Empty, useCellBounds: false, ContentAlignment.TopLeft);

		private Rectangle boundsText;

		private Rectangle boundsCell;

		private bool useCellBounds;

		private ContentAlignment alignment;

		/// <summary>
		///   sub-item text area
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
		///   sub-item cell area
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
		///   use cell area for placement (instead of text area)
		/// </summary>
		public bool UseCellBounds {
			get {
				return this.useCellBounds;
			}
			set {
				this.useCellBounds = value;
			}
		}

		/// <summary>
		///   embedded control alignment within the specified text or cell area
		/// </summary>
		public ContentAlignment Alignment {
			get {
				return this.alignment;
			}
			set {
				this.alignment = value;
			}
		}

		/// <summary>
		///   area for embedded control placement (depending on current settings)
		/// </summary>
		public Rectangle Bounds {
			get {
				if (!this.useCellBounds) {
					return this.boundsText;
				}
				return this.boundsCell;
			}
		}

		/// <summary>
		///   this BetterListViewEmbeddedControlPlacement structure is empty
		/// </summary>
		public bool IsEmpty => this.Equals(BetterListViewEmbeddedControlPlacement.Empty);

		/// <summary>
		///   Initialize a new BetterListViewEmbeddedControlPlacement structure.
		/// </summary>
		/// <param name="boundsText">sub-item text area</param>
		/// <param name="boundsCell">sub-item cell area</param>
		/// <param name="useCellBounds">use cell area for placement (instead of text area)</param>
		/// <param name="alignment">embedded control alignment within the specified text or cell area</param>
		public BetterListViewEmbeddedControlPlacement(Rectangle boundsText, Rectangle boundsCell, bool useCellBounds, ContentAlignment alignment) {
			this.boundsText = boundsText;
			this.boundsCell = boundsCell;
			this.useCellBounds = useCellBounds;
			this.alignment = alignment;
		}

		/// <summary>
		///   Test whether the two BetterListViewEmbeddedControlPlacement objects are identical.
		/// </summary>
		/// <param name="embeddedControlPlacementA">first BetterListViewEmbeddedControlPlacement object</param>
		/// <param name="embeddedControlPlacementB">second BetterListViewEmbeddedControlPlacement object</param>
		/// <returns>the two BetterListViewEmbeddedControlPlacement objects are identical</returns>
		public static bool operator ==(BetterListViewEmbeddedControlPlacement embeddedControlPlacementA, BetterListViewEmbeddedControlPlacement embeddedControlPlacementB) {
			return embeddedControlPlacementA.Equals(embeddedControlPlacementB);
		}

		/// <summary>
		///   Test whether the two BetterListViewEmbeddedControlPlacement objects are different.
		/// </summary>
		/// <param name="embeddedControlPlacementA">first BetterListViewEmbeddedControlPlacement object</param>
		/// <param name="embeddedControlPlacementB">second BetterListViewEmbeddedControlPlacement object</param>
		/// <returns>the two BetterListViewEmbeddedControlPlacement objects are different</returns>
		public static bool operator !=(BetterListViewEmbeddedControlPlacement embeddedControlPlacementA, BetterListViewEmbeddedControlPlacement embeddedControlPlacementB) {
			return !embeddedControlPlacementA.Equals(embeddedControlPlacementB);
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
			if (obj == null || !(obj is BetterListViewEmbeddedControlPlacement betterListViewEmbeddedControlPlacement)) {
				return false;
			}
			if (this.boundsText.Equals(betterListViewEmbeddedControlPlacement.boundsText) && this.boundsCell.Equals(betterListViewEmbeddedControlPlacement.boundsCell) && this.useCellBounds.Equals(betterListViewEmbeddedControlPlacement.useCellBounds)) {
				return this.alignment.Equals(betterListViewEmbeddedControlPlacement.alignment);
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
			return this.boundsText.GetHashCode() ^ this.boundsCell.GetHashCode() ^ this.useCellBounds.GetHashCode() ^ this.alignment.GetHashCode();
		}
	}
}