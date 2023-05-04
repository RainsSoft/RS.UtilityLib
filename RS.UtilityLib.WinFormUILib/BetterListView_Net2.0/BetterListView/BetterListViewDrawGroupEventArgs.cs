using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DrawGroup events.
	/// </summary>
	public class BetterListViewDrawGroupEventArgs : EventArgs
	{
		private Graphics graphics;

		private BetterListViewGroup group;

		private BetterListViewGroupBounds groupBounds;

		private BetterListViewGroupStateInfo groupStateInfo;

		private bool drawFace = true;

		private bool drawExpandButton = true;

		private bool drawImage = true;

		private bool drawText = true;

		private bool drawSeparator = true;

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
		///   Gets or sets the group state information.
		/// </summary>
		/// <value>
		///   The group group state information.
		/// </value>
		public BetterListViewGroupStateInfo GroupStateInfo {
			get {
				return this.groupStateInfo;
			}
			set {
				this.groupStateInfo = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw group face.
		/// </summary>
		/// <value>
		///   <c>true</c> if group face should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawFace {
			get {
				return this.drawFace;
			}
			set {
				this.drawFace = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw group expand button.
		/// </summary>
		/// <value>
		///   <c>true</c> if group expand button should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawExpandButton {
			get {
				return this.drawExpandButton;
			}
			set {
				this.drawExpandButton = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw group image.
		/// </summary>
		/// <value>
		///   <c>true</c> if group image should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawImage {
			get {
				return this.drawImage;
			}
			set {
				this.drawImage = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw group text.
		/// </summary>
		/// <value>
		///   <c>true</c> if group text should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawText {
			get {
				return this.drawText;
			}
			set {
				this.drawText = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw group separator.
		/// </summary>
		/// <value>
		///   <c>true</c> if group separator should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawSeparator {
			get {
				return this.drawSeparator;
			}
			set {
				this.drawSeparator = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawGroupEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="group">Group to draw.</param>
		/// <param name="groupBounds">Group boundaries.</param>
		/// <param name="groupStateInfo">Group state information.</param>
		public BetterListViewDrawGroupEventArgs(Graphics graphics, BetterListViewGroup group, BetterListViewGroupBounds groupBounds, BetterListViewGroupStateInfo groupStateInfo) {
			this.Graphics = graphics;
			this.Group = group;
			this.GroupBounds = groupBounds;
			this.GroupStateInfo = groupStateInfo;
		}
	}
}