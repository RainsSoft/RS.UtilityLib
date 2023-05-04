using System;
using System.Drawing;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Provides data for BetterListView.DrawItem events.
	/// </summary>
	public class BetterListViewDrawItemEventArgs : EventArgs
	{
		private Graphics graphics;

		private bool drawEnabled = true;

		private bool drawFocused = true;

		private BetterListViewItem item;

		private BetterListViewItemBounds itemBounds;

		private BetterListViewItemStateInfo itemStateInfo;

		private bool drawSelection = true;

		private bool drawExpandButton = true;

		private bool drawCheckBox = true;

		private bool[] drawSubItemImages = new bool[1] { true };

		private bool[] drawSubItemImageBorders = new bool[1] { true };

		private bool[] drawSubItemTexts = new bool[1] { true };

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
		///   Gets or sets a value indicating whether to draw item like if the control is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if draw item like if the control is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool DrawEnabled {
			get {
				return this.drawEnabled;
			}
			set {
				this.drawEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw item like if the control is focused.
		/// </summary>
		/// <value>
		///   <c>true</c> if draw item like if the control is focused; otherwise, <c>false</c>.
		/// </value>
		public bool DrawFocused {
			get {
				return this.drawFocused;
			}
			set {
				this.drawFocused = value;
			}
		}

		/// <summary>
		///   Gets or sets the item to draw.
		/// </summary>
		/// <value>
		///   The item to draw.
		/// </value>
		public BetterListViewItem Item {
			get {
				return this.item;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.item = value;
			}
		}

		/// <summary>
		///   Gets or sets the item boundaries.
		/// </summary>
		/// <value>
		///   The item boundaries.
		/// </value>
		public BetterListViewItemBounds ItemBounds {
			get {
				return this.itemBounds;
			}
			set {
				Checks.CheckNotNull(value, "value");
				this.itemBounds = value;
			}
		}

		/// <summary>
		///   Gets or sets the item state information.
		/// </summary>
		/// <value>
		///   The item state information.
		/// </value>
		public BetterListViewItemStateInfo ItemStateInfo {
			get {
				return this.itemStateInfo;
			}
			set {
				this.itemStateInfo = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw item selection.
		/// </summary>
		/// <value>
		///   <c>true</c> if item selection should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawSelection {
			get {
				return this.drawSelection;
			}
			set {
				this.drawSelection = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw item expand button.
		/// </summary>
		/// <value>
		///   <c>true</c> if item expand button should be drawn; otherwise, <c>false</c>.
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
		///   Gets or sets a value indicating whether to draw item check box.
		/// </summary>
		/// <value>
		///   <c>true</c> if item check box should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawCheckBox {
			get {
				return this.drawCheckBox;
			}
			set {
				this.drawCheckBox = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw item image.
		/// </summary>
		/// <value>
		///   <c>true</c> if item image should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawImage {
			get {
				return this.drawSubItemImages[0];
			}
			set {
				this.drawSubItemImages[0] = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether draw item image border.
		/// </summary>
		/// <value>
		///   <c>true</c> if item image border should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawImageBorder {
			get {
				return this.drawSubItemImageBorders[0];
			}
			set {
				this.drawSubItemImageBorders[0] = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether to draw item text.
		/// </summary>
		/// <value>
		///   <c>true</c> if item text should be drawn; otherwise, <c>false</c>.
		/// </value>
		public bool DrawText {
			get {
				return this.drawSubItemTexts[0];
			}
			set {
				this.drawSubItemTexts[0] = value;
			}
		}

		/// <summary>
		///   Gets or sets indicators for drawing sub-item images.
		/// </summary>
		/// <value>
		///   Indicators for drawing sub-item images.
		/// </value>
		public bool[] DrawSubItemImages {
			get {
				return this.drawSubItemImages;
			}
			set {
				Checks.CheckNotNull(value, "value");
				Checks.CheckTrue(value.Length >= 1, "value.Length >= 1");
				this.drawSubItemImages = value;
			}
		}

		/// <summary>
		///   Gets or sets indicators for drawing sub-item image borders.
		/// </summary>
		/// <value>
		///   Indicators for drawing sub-item image borders.
		/// </value>
		public bool[] DrawSubItemImageBorders {
			get {
				return this.drawSubItemImageBorders;
			}
			set {
				Checks.CheckNotNull(value, "value");
				Checks.CheckTrue(value.Length >= 1, "value.Length >= 1");
				this.drawSubItemImageBorders = value;
			}
		}

		/// <summary>
		///   Gets or sets indicators for drawing sub-item texts..
		/// </summary>
		/// <value>
		///   Indicators for drawing sub-item texts.
		/// </value>
		public bool[] DrawSubItemTexts {
			get {
				return this.drawSubItemTexts;
			}
			set {
				Checks.CheckNotNull(value, "value");
				Checks.CheckTrue(value.Length >= 1, "value.Length >= 1");
				this.drawSubItemTexts = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewDrawItemEventArgs" /> class.
		/// </summary>
		/// <param name="graphics">Graphics object used for drawing.</param>
		/// <param name="drawEnabled">Draw item like if the control is enabled.</param>
		/// <param name="drawFocused">Draw item like if the control is focused.</param>
		/// <param name="item">Item to draw.</param>
		/// <param name="itemBounds">Item boundaries.</param>
		/// <param name="itemStateInfo">Item state information.</param>
		public BetterListViewDrawItemEventArgs(Graphics graphics, bool drawEnabled, bool drawFocused, BetterListViewItem item, BetterListViewItemBounds itemBounds, BetterListViewItemStateInfo itemStateInfo) {
			this.Graphics = graphics;
			this.DrawEnabled = drawEnabled;
			this.DrawFocused = drawFocused;
			this.Item = item;
			this.ItemBounds = itemBounds;
			this.ItemStateInfo = itemStateInfo;
			bool[] array = new bool[item.SubItems.Count];
			bool[] array2 = new bool[item.SubItems.Count];
			bool[] array3 = new bool[item.SubItems.Count];
			for (int i = 0; i < item.SubItems.Count; i++) {
				array[i] = true;
				array2[i] = true;
				array3[i] = true;
			}
			this.DrawSubItemImages = array;
			this.DrawSubItemImageBorders = array2;
			this.DrawSubItemTexts = array3;
		}
	}
}