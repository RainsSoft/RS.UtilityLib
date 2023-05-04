using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common BetterListView layout.
	/// </summary>
	public abstract class BetterListViewLayoutBase
	{
		private readonly BetterListViewBase itemsView;

		private Padding layoutPadding = Padding.Empty;

		private Size elementOuterPadding = Size.Empty;

		/// <summary>
		///   layout elements are oriented vertically
		/// </summary>
		public abstract bool OrientationVertical { get; }

		/// <summary>
		///   padding of the whole layout
		/// </summary>
		public Padding LayoutPadding {
			get {
				return this.layoutPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.layoutPadding == value)) {
					this.layoutPadding = value;
					this.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   outer padding of an element
		/// </summary>
		public Size ElementOuterPadding {
			get {
				return this.elementOuterPadding;
			}
			set {
				Checks.CheckSize(value, "value");
				if (!(this.elementOuterPadding == value)) {
					this.elementOuterPadding = value;
					this.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   layout positioning options
		/// </summary>
		internal abstract BetterListViewLayoutPositioningOptions PositioningOptions { get; }

		/// <summary>
		///   each row contains just a single item, even if there is enough space on the row for more items
		/// </summary>
		protected abstract bool SingleItemPerRow { get; }

		/// <summary>
		///   default layout padding
		/// </summary>
		protected abstract Padding DefaultLayoutPadding { get; }

		/// <summary>
		///   default outer padding of an element
		/// </summary>
		protected abstract Size DefaultElementOuterPadding { get; }

		/// <summary>
		///   owner control of this layout
		/// </summary>
		protected BetterListViewBase ItemsView => this.itemsView;

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayoutBase" /> class.
		/// </summary>
		/// <param name="itemsView">BetterListView instance.</param>
		protected internal BetterListViewLayoutBase(BetterListViewBase itemsView) {
			Checks.CheckNotNull(itemsView, "itemsView");
			this.itemsView = itemsView;
			this.SetDefaultsInternal();
		}

		/// <summary>
		///   Called when layout property has been changed.
		/// </summary>
		/// <param name="layoutPropertyType">update type of a layout property</param>
		internal void OnPropertyChanged(BetterListViewLayoutPropertyType layoutPropertyType) {
			this.itemsView.OnLayoutPropertyChanged(this, layoutPropertyType);
		}

		/// <summary>
		///   Set default layout properties.
		/// </summary>
		public void SetDefaults() {
			this.SetDefaultsInternal();
			this.OnPropertyChanged(BetterListViewLayoutPropertyType.Setup);
		}

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected virtual void SetDefaultsInternal() {
			this.layoutPadding = this.DefaultLayoutPadding;
			this.elementOuterPadding = this.DefaultElementOuterPadding;
		}
	}
}