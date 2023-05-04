using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Item boundaries.
	/// </summary>
	public sealed class BetterListViewItemBounds : BetterListViewElementBounds
	{
		private Rectangle boundsSelection = Rectangle.Empty;

		private Rectangle boundsCheckBox = Rectangle.Empty;

		private ReadOnlyCollection<BetterListViewSubItemBounds> subItemBounds = new ReadOnlyCollection<BetterListViewSubItemBounds>(new BetterListViewSubItemBounds[1]
		{
		new BetterListViewSubItemBounds()
		});

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

		public Rectangle BoundsCheckBox {
			get {
				return this.boundsCheckBox;
			}
			set {
				this.boundsCheckBox = value;
			}
		}

		/// <summary>
		///   corresponding sub-item boundaries
		/// </summary>
		public ReadOnlyCollection<BetterListViewSubItemBounds> SubItemBounds => this.subItemBounds;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemBounds" /> class.
		/// </summary>
		/// <param name="subItemCount">number of considered sub-items</param>
		public BetterListViewItemBounds(int subItemCount) {
			Checks.CheckTrue(subItemCount >= 1, "subItemCount >= 1");
			BetterListViewSubItemBounds[] array = new BetterListViewSubItemBounds[subItemCount];
			for (int i = 0; i < subItemCount; i++) {
				array[i] = new BetterListViewSubItemBounds();
			}
			this.subItemBounds = new ReadOnlyCollection<BetterListViewSubItemBounds>(array);
		}

		/// <summary>
		///   Prevents a default instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewItemBounds" /> class from being created.
		/// </summary>
		private BetterListViewItemBounds() {
		}

		/// <summary>
		///   Shift boundaries by the specified offset.
		/// </summary>
		/// <param name="offset">amount of shift</param>
		public override void Offset(Point offset) {
			this.boundsSelection = Painter.OffsetRectangle(this.boundsSelection, offset);
			this.boundsCheckBox = Painter.OffsetRectangle(this.boundsCheckBox, offset);
			foreach (BetterListViewSubItemBounds subItemBound in this.subItemBounds) {
				subItemBound.Offset(offset);
			}
			base.Offset(offset);
		}

		/// <summary>
		///   Reset values of the boundaries.
		/// </summary>
		public override void Reset() {
			this.boundsSelection = Rectangle.Empty;
			this.boundsCheckBox = Rectangle.Empty;
			foreach (BetterListViewSubItemBounds subItemBound in this.subItemBounds) {
				subItemBound.Reset();
			}
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
			if (!(obj is BetterListViewItemBounds betterListViewItemBounds) || !base.Equals(obj)) {
				return false;
			}
			if (this.boundsSelection != betterListViewItemBounds.boundsSelection || this.boundsCheckBox != betterListViewItemBounds.boundsCheckBox || this.subItemBounds.Count != betterListViewItemBounds.subItemBounds.Count) {
				return false;
			}
			IEnumerator<BetterListViewSubItemBounds> enumerator = this.subItemBounds.GetEnumerator();
			IEnumerator<BetterListViewSubItemBounds> enumerator2 = this.subItemBounds.GetEnumerator();
			enumerator.Reset();
			enumerator2.Reset();
			while (enumerator.MoveNext() && enumerator2.MoveNext()) {
				if (!object.Equals(enumerator.Current, enumerator2.Current)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return this.boundsSelection.GetHashCode() ^ this.boundsCheckBox.GetHashCode() ^ this.subItemBounds.Count.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		///   Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		///   A new object that is a copy of this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override object Clone() {
			BetterListViewItemBounds betterListViewItemBounds = new BetterListViewItemBounds();
			betterListViewItemBounds.boundsSelection = this.boundsSelection;
			betterListViewItemBounds.boundsCheckBox = this.boundsCheckBox;
			List<BetterListViewSubItemBounds> list = new List<BetterListViewSubItemBounds>();
			foreach (BetterListViewSubItemBounds subItemBound in this.subItemBounds) {
				list.Add((BetterListViewSubItemBounds)subItemBound.Clone());
			}
			betterListViewItemBounds.subItemBounds = list.AsReadOnly();
			base.CopyTo(betterListViewItemBounds);
			return betterListViewItemBounds;
		}
	}
}