using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DrawGroupBackground events.
	/// </summary>
	public class BetterListViewDrawGroupBackgroundEventArgs : EventArgs
	{
		private Graphics graphics;

		private BetterListViewGroup group;

		private BetterListViewGroupBounds groupBounds;

		/// <summary>
		///   Gets or sets the Graphics object used for drawing.
		/// </summary>
		/// <value>
		///   The Graphics object used for drawing.
		/// </value>
		public Graphics Graphics {
			get {
				return this.graphics;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.graphics = value;
			}
		}

		/// <summary>
		///   Gets or sets the group to draw.
		/// </summary>
		/// <value>
		///   The group to draw.
		/// </value>
		public BetterListViewGroup Group {
			get {
				return this.group;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.group = value;
			}
		}

		/// <summary>
		///   Gets or sets the group boundaries.
		/// </summary>
		/// <value>
		///   The group boundaries.
		/// </value>
		public BetterListViewGroupBounds GroupBounds {
			get {
				return this.groupBounds;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.groupBounds = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawGroupBackgroundEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="group">Group to draw.</param>
		/// <param name="groupBounds">Group boundaries.</param>
		public BetterListViewDrawGroupBackgroundEventArgs(Graphics graphics, BetterListViewGroup group, BetterListViewGroupBounds groupBounds) {
			this.Graphics = graphics;
			this.Group = group;
			this.GroupBounds = groupBounds;
		}
	}
}