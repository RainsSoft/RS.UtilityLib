using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.FocusedItemChanged event.
	/// </summary>
	public class BetterListViewFocusedItemChangedEventArgs : EventArgs
	{
		private BetterListViewGroup focusedGroupOld;

		private BetterListViewItem focusedItemOld;

		private int focusedColumnIndexOld;

		private BetterListViewGroup focusedGroupNew;

		private BetterListViewItem focusedItemNew;

		private int focusedColumnIndexNew;

		/// <summary>
		///   Recently focused group.
		/// </summary>
		public BetterListViewGroup FocusedGroupOld {
			get {
				return this.focusedGroupOld;
			}
			set {
				this.focusedGroupOld = value;
			}
		}

		/// <summary>
		///   Recently focused item.
		/// </summary>
		public BetterListViewItem FocusedItemOld {
			get {
				return this.focusedItemOld;
			}
			set {
				this.focusedItemOld = value;
			}
		}

		/// <summary>
		///   Recently focused column (sub-item) index.
		/// </summary>
		public int FocusedColumnIndexOld {
			get {
				return this.focusedColumnIndexOld;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "(value == BetterListViewElementBase.IndexUndefined) || (value >= 0)");
				this.focusedColumnIndexOld = value;
			}
		}

		/// <summary>
		///   Currently focused group.
		/// </summary>
		public BetterListViewGroup FocusedGroupNew {
			get {
				return this.focusedGroupNew;
			}
			set {
				this.focusedGroupNew = value;
			}
		}

		/// <summary>
		///   Currently focused item.
		/// </summary>
		public BetterListViewItem FocusedItemNew {
			get {
				return this.focusedItemNew;
			}
			set {
				this.focusedItemNew = value;
			}
		}

		/// <summary>
		///   Currently focused column (sub-item) index.
		/// </summary>
		public int FocusedColumnIndexNew {
			get {
				return this.focusedColumnIndexNew;
			}
			set {
				Checks.CheckTrue(value == -1 || value >= 0, "(value == BetterListViewElementBase.IndexUndefined) || (value >= 0)");
				this.focusedColumnIndexNew = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewFocusedItemChangedEventArgs" /> class.
		/// </summary>
		/// <param name="focusedGroupOld">Recently focused group.</param>
		/// <param name="focusedItemOld">Recently focused item.</param>
		/// <param name="focusedColumnIndexOld">Recently focused column (sub-item) index.</param>
		/// <param name="focusedGroupNew">Currently focused group.</param>
		/// <param name="focusedItemNew">Currently focused item.</param>
		/// <param name="focusedColumnIndexNew">Recently focused column (sub-item) index.</param>
		public BetterListViewFocusedItemChangedEventArgs(BetterListViewGroup focusedGroupOld, BetterListViewItem focusedItemOld, int focusedColumnIndexOld, BetterListViewGroup focusedGroupNew, BetterListViewItem focusedItemNew, int focusedColumnIndexNew) {
			bool flag = focusedGroupOld != null;
			bool flag2 = focusedItemOld != null;
			bool flag3 = focusedColumnIndexOld != -1;
			bool flag4 = focusedGroupNew != null;
			bool flag5 = focusedItemNew != null;
			bool flag6 = focusedColumnIndexNew != -1;
			Checks.CheckTrue((!flag && !flag2) ^ (flag ^ flag2), "(!isFocusedGroupOld && !isFocusedItemOld) ^ (isFocusedGroupOld ^ isFocusedItemOld)", "No element or either group or item should be focused.");
			Checks.CheckTrue(flag2 == flag3, "isFocusedItemOld == isFocusedSubItemOld", "Sub-item is focused whenever item is focused.");
			Checks.CheckTrue((!flag4 && !flag5) ^ (flag4 ^ flag5), "(!isFocusedGroupNew && !isFocusedItemNew) ^ (isFocusedGroupNew ^ isFocusedItemNew)", "No element or either group or item should be focused.");
			Checks.CheckTrue(flag5 == flag6, "isFocusedItemNew == isFocusedSubItemNew", "Sub-item is focused whenever item is focused.");
			this.FocusedGroupOld = focusedGroupOld;
			this.FocusedItemOld = focusedItemOld;
			this.FocusedColumnIndexOld = focusedColumnIndexOld;
			this.FocusedGroupNew = focusedGroupNew;
			this.FocusedItemNew = focusedItemNew;
			this.FocusedColumnIndexNew = focusedColumnIndexNew;
		}
	}
}