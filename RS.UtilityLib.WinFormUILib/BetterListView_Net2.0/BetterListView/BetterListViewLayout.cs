using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common BetterListView layout.
	/// </summary>
	public abstract class BetterListViewLayout<TElement> : BetterListViewLayoutCommon<TElement> where TElement : BetterListViewElementBase
	{
		private const bool DefaultAutoSizeImages = true;

		private const bool DefaultAutoSizeText = true;

		private Padding elementInnerPadding;

		private Padding imagePadding;

		private Padding textPadding;

		private bool autoSizeImages;

		private bool autoSizeText;

		private BetterListViewImageAlignmentHorizontal defaultImageAlignmentHorizontal;

		private BetterListViewImageAlignmentVertical defaultImageAlignmentVertical;

		private TextAlignmentHorizontal defaultTextAlignmentHorizontal;

		private TextAlignmentVertical defaultTextAlignmentVertical;

		private TextTrimming defaultTextTrimming;

		/// <summary>
		///   padding around inner area of the element
		/// </summary>
		public Padding ElementInnerPadding {
			get {
				return this.elementInnerPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.elementInnerPadding == value)) {
					this.elementInnerPadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   element image paddng
		/// </summary>
		public Padding ImagePadding {
			get {
				return this.imagePadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.imagePadding == value)) {
					this.imagePadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   text area padding
		/// </summary>
		public Padding TextPadding {
			get {
				return this.textPadding;
			}
			set {
				Checks.CheckPadding(value, "value");
				if (!(this.textPadding == value)) {
					this.textPadding = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   Update ImageSize property automatically according to largest image size.
		/// </summary>
		public bool AutoSizeImages {
			get {
				return this.autoSizeImages;
			}
			set {
				if (this.autoSizeImages != value) {
					this.autoSizeImages = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Other);
				}
			}
		}

		/// <summary>
		///   Update EmptyText property automatically according to largest text height.
		/// </summary>
		public bool AutoSizeText {
			get {
				return this.autoSizeText;
			}
			set {
				if (this.autoSizeText != value) {
					this.autoSizeText = value;
					base.OnPropertyChanged(BetterListViewLayoutPropertyType.Text);
				}
			}
		}

		/// <summary>
		///   default horizontal element image alignment
		/// </summary>
		public BetterListViewImageAlignmentHorizontal DefaultImageAlignmentHorizontal {
			get {
				return this.defaultImageAlignmentHorizontal;
			}
			set {
				this.defaultImageAlignmentHorizontal = ((value == BetterListViewImageAlignmentHorizontal.Default) ? this.DefaultDefaultImageAlignmentHorizontal : value);
			}
		}

		/// <summary>
		///   default vertical element image alignment
		/// </summary>
		public BetterListViewImageAlignmentVertical DefaultImageAlignmentVertical {
			get {
				return this.defaultImageAlignmentVertical;
			}
			set {
				this.defaultImageAlignmentVertical = ((value == BetterListViewImageAlignmentVertical.Default) ? this.DefaultDefaultImageAlignmentVertical : value);
			}
		}

		/// <summary>
		///   default horizontal element text alignment
		/// </summary>
		public TextAlignmentHorizontal DefaultTextAlignmentHorizontal {
			get {
				return this.defaultTextAlignmentHorizontal;
			}
			set {
				this.defaultTextAlignmentHorizontal = ((value == TextAlignmentHorizontal.Default) ? this.DefaultDefaultTextAlignmentHorizontal : value);
			}
		}

		/// <summary>
		///   default vertical element text alignment
		/// </summary>
		public TextAlignmentVertical DefaultTextAlignmentVertical {
			get {
				return this.defaultTextAlignmentVertical;
			}
			set {
				this.defaultTextAlignmentVertical = ((value == TextAlignmentVertical.Default) ? this.DefaultDefaultTextAlignmentVertical : value);
			}
		}

		/// <summary>
		///   default element text trimming
		/// </summary>
		public TextTrimming DefaultTextTrimming {
			get {
				return this.defaultTextTrimming;
			}
			set {
				this.defaultTextTrimming = ((value == TextTrimming.Undefined) ? this.DefaultDefaultTextTrimming : value);
			}
		}

		/// <summary>
		///   default element inner padding
		/// </summary>
		protected abstract Padding DefaultElementInnerPadding { get; }

		/// <summary>
		///   default element image size
		/// </summary>
		protected abstract BetterListViewImageSize DefaultImageSize { get; }

		/// <summary>
		///   default element image padding
		/// </summary>
		protected abstract Padding DefaultImagePadding { get; }

		/// <summary>
		///   default element text padding
		/// </summary>
		protected abstract Padding DefaultTextPadding { get; }

		/// <summary>
		///   default horizontal element image alignment
		/// </summary>
		protected abstract BetterListViewImageAlignmentHorizontal DefaultDefaultImageAlignmentHorizontal { get; }

		/// <summary>
		///   default vertical element image alignment
		/// </summary>
		protected abstract BetterListViewImageAlignmentVertical DefaultDefaultImageAlignmentVertical { get; }

		/// <summary>
		///   default horizontal element text alignment
		/// </summary>
		protected abstract TextAlignmentHorizontal DefaultDefaultTextAlignmentHorizontal { get; }

		/// <summary>
		///   default vertical element text alignment
		/// </summary>
		protected abstract TextAlignmentVertical DefaultDefaultTextAlignmentVertical { get; }

		/// <summary>
		///   default item/sub-item text trimming
		/// </summary>
		protected abstract TextTrimming DefaultDefaultTextTrimming { get; }

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewLayout`1" /> class.
		/// </summary>
		/// <param name="listView">control containing the layout</param>
		protected BetterListViewLayout(BetterListView listView)
			: base((BetterListViewBase)listView) {
		}

		/// <summary>
		///   Set default layout properties without calling OnPropertyChanged.
		/// </summary>
		protected override void SetDefaultsInternal() {
			this.elementInnerPadding = this.DefaultElementInnerPadding;
			this.imagePadding = this.DefaultImagePadding;
			this.textPadding = this.DefaultTextPadding;
			this.autoSizeImages = true;
			this.autoSizeText = true;
			this.defaultImageAlignmentHorizontal = this.DefaultDefaultImageAlignmentHorizontal;
			this.defaultImageAlignmentVertical = this.DefaultDefaultImageAlignmentVertical;
			this.defaultTextAlignmentHorizontal = this.DefaultDefaultTextAlignmentHorizontal;
			this.defaultTextAlignmentVertical = this.DefaultDefaultTextAlignmentVertical;
			this.defaultTextTrimming = this.DefaultDefaultTextTrimming;
			base.SetDefaultsInternal();
		}
	}
}