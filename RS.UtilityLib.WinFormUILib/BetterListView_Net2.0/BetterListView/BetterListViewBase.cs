using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Control with items in a specific layout.
	/// </summary>
	public abstract class BetterListViewBase : Control, ISupportInitialize
	{
		private delegate void AutoScrollStartDelegate(BetterListViewAutoScrollMode autoScrollMode);

		private delegate void AutoScrollStopDelegate();

		private delegate void BeginUpdateDelegate();

		private delegate void EndUpdateDelegate(bool suppressRefresh);

		/// <summary>
		///   'Action' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryAction = "Action";

		/// <summary>
		///   'Appearance' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryAppearance = "Appearance";

		/// <summary>
		///   'Behavior' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryBehavior = "Behavior";

		/// <summary>
		///   'Data' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryData = "Data";

		/// <summary>
		///   'Drag Drop' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryDragDrop = "Drag Drop";

		/// <summary>
		///   'Mouse' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryMouse = "Mouse";

		/// <summary>
		///   'Property Changed' event category for PropertyGrid
		/// </summary>
		internal const string EventsCategoryPropertyChanged = "Property Changed";

		private const BorderStyle DefaultBorderStyle = BorderStyle.Fixed3D;

		private const int DefaultAutoScrollBoundary = 32;

		private const int DefaultAutoScrollStep = 32;

		private const int DefaultAutoScrollStepDelay = 50;

		private const int MinimumAutoScrollStepDelay = 50;

		/// <summary>
		/// 'Accessibility' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryAccessibility = "Accessibility";

		/// <summary>
		///   'Appearance' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryAppearance = "Appearance";

		/// <summary>
		///   'Behavior' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryBehavior = "Behavior";

		/// <summary>
		///   'Data' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryData = "Data";

		/// <summary>
		///   'Focus' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryFocus = "Focus";

		/// <summary>
		///   'Miscellaneous' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryMiscellaneous = "Miscellaneous";

		/// <summary>
		///   'Performance' property category for the property grid
		/// </summary>
		internal const string PropertiesCategoryPerformance = "Performance";

		internal const bool DefaultCacheImages = true;

		private BetterListViewAutoScrollMode autoScrollMode;

		private Timer autoScrollTimer;

		private BufferedGraphicsContext backBufferContext;

		private BufferedGraphics backBufferGraphics;

		private bool backBufferEmpty = true;

		private readonly HScrollBar hScrollBar = new HScrollBar();

		private readonly VScrollBar vScrollBar = new VScrollBar();

		private static readonly Color DefaultBackColorValue = SystemColors.Window;

		private static readonly Color DefaultForeColorItems = Color.Empty;

		private BorderStyle borderStyle = BorderStyle.Fixed3D;

		private Font fontItems;

		private Color foreColorItems = BetterListViewBase.DefaultForeColorItems;

		private int autoScrollBoundary = 32;

		private int autoScrollStep = 32;

		private int autoScrollStepDelay = 50;

		private string dragSourceID;

		private bool cacheImages = true;

		private BetterListViewInvalidationInfo invalidationInfo = BetterListViewInvalidationInfo.Empty;

		private int updateLevel;

		/// <summary>
		///   auto-scroll feature is enabled
		/// </summary>
		protected bool AutoScrollEnabled => this.autoScrollTimer != null;

		/// <summary>
		///   Stored Graphics object for re-use.
		/// </summary>
		protected Graphics CachedGraphics => this.backBufferGraphics.Graphics;

		private bool DrawBorder => this.BorderStyle != BorderStyle.None;

		private bool DrawBorder3D {
			get {
				if (!Application.RenderWithVisualStyles) {
					return this.BorderStyle == BorderStyle.Fixed3D;
				}
				return false;
			}
		}

		/// <summary>
		///   horizontal scroll bar
		/// </summary>
		protected internal HScrollBar HScrollBar => this.hScrollBar;

		/// <summary>
		///   vertical scroll bar
		/// </summary>
		protected internal VScrollBar VScrollBar => this.vScrollBar;

		/// <summary>
		///   content area
		/// </summary>
		protected Rectangle BoundsContent => this.GetContentBounds(this.HScrollBarVisible, this.VScrollBarVisible);

		/// <summary>
		///   position of the scroll bars
		/// </summary>
		protected Point ScrollPosition => new Point(this.ScrollPositionHorizontal, this.ScrollPositionVertical);

		/// <summary>
		///   position of the horizontal scroll bar
		/// </summary>
		protected int ScrollPositionHorizontal {
			get {
				if (!this.HScrollBarVisible) {
					return 0;
				}
				return this.HScrollBar.Value;
			}
			set {
				if (this.HScrollBarVisible) {
					this.HScrollBar.Value = value;
				}
			}
		}

		/// <summary>
		///   position of the vertical scroll bar
		/// </summary>
		protected int ScrollPositionVertical {
			get {
				if (!this.VScrollBarVisible) {
					return 0;
				}
				return this.VScrollBar.Value;
			}
			set {
				if (this.VScrollBarVisible) {
					this.VScrollBar.Value = value;
				}
			}
		}

		/// <summary>
		///   Gets or sets the background color for the control.
		/// </summary>
		/// <returns>
		///   A <see cref="T:System.Drawing.Color" /> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor" /> property.
		/// </returns>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		[Category("Appearance")]
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}

		/// <summary>
		///   border style of the control
		/// </summary>
		[Description("Border style of the control")]
		[Category("Appearance")]
		public BorderStyle BorderStyle {
			get {
				return this.borderStyle;
			}
			set {
				if (this.borderStyle != value) {
					this.borderStyle = value;
					this.OnBorderStyleChanged(EventArgs.Empty);
					this.OnResize(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		///   Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <returns>
		///   The <see cref="T:System.Drawing.Font" /> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont" /> property.
		/// </returns>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
		///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		[Category("Appearance")]
		[MergableProperty(true)]
		[Description("Gets or sets the font of the text displayed by the control.")]
		public override Font Font {
			get {
				return base.Font;
			}
			set {
				base.Font = value;
			}
		}

		/// <summary>
		///   font of item texts
		/// </summary>
		[Category("Appearance")]
		[Description("Font of item texts")]
		public virtual Font FontItems {
			get {
				if (this.fontItems != null) {
					return this.fontItems;
				}
				return this.Font;
			}
			set {
				if (this.fontItems != value) {
					this.fontItems = value;
					this.OnFontChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		///   foreground color of the control
		/// </summary>
		[Category("Appearance")]
		[MergableProperty(true)]
		[Description("Foreground color of the control")]
		public override Color ForeColor {
			get {
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
			}
		}

		/// <summary>
		///   foreground color of item texts
		/// </summary>
		[Category("Appearance")]
		[Description("Foreground color of item texts")]
		public Color ForeColorItems {
			get {
				if (!this.foreColorItems.IsEmpty) {
					return this.foreColorItems;
				}
				return this.ForeColor;
			}
			set {
				if (!(this.foreColorItems == value)) {
					this.foreColorItems = value;
					this.OnForeColorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		///   offset from the items area border where auto scroll is initiated
		/// </summary>
		[Category("Behavior")]
		[Description("Offset from the items area border where auto scroll is initiated")]
		[DefaultValue(32)]
		public int AutoScrollBoundary {
			get {
				return this.autoScrollBoundary;
			}
			set {
				Checks.CheckTrue(value >= 0, "value >= 0");
				this.autoScrollBoundary = value;
			}
		}

		/// <summary>
		///   step size of automatic scrolling (in pixels)
		/// </summary>
		[Description("Step size of automatic scrolling (in pixels)")]
		[Category("Behavior")]
		[DefaultValue(32)]
		public int AutoScrollStep {
			get {
				return this.autoScrollStep;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.autoScrollStep = value;
			}
		}

		/// <summary>
		///   delay between two steps of automatic scrolling
		/// </summary>
		[DefaultValue(50)]
		[Category("Behavior")]
		[Description("Delay between two steps of automatic scrolling")]
		public int AutoScrollStepDelay {
			get {
				return this.autoScrollStepDelay;
			}
			set {
				Checks.CheckTrue(value >= 50, "value >= MinimumAutoScrollStepDelay");
				this.autoScrollStepDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control can respond to user interaction.
		/// </summary>
		/// <returns>true if the control can respond to user interaction; otherwise, false. The default is true.</returns>
		[DefaultValue(true)]
		[Description("Indicated whether the control is enabled.")]
		[Category("Behavior")]
		public new bool Enabled {
			get {
				return base.Enabled;
			}
			set {
				if (base.Enabled != value) {
					base.Enabled = value;
					this.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
					this.RefreshView();
				}
			}
		}

		/// <summary>
		///   ClientRectangle without border
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Rectangle ClientRectangleInner {
			get {
				if (this.DrawBorder) {
					Rectangle clientRectangle = base.ClientRectangle;
					Size size = (this.DrawBorder3D ? SystemInformation.Border3DSize : SystemInformation.BorderSize);
					return new Rectangle(clientRectangle.Left + size.Width, clientRectangle.Top + size.Height, clientRectangle.Width - (size.Width << 1), clientRectangle.Height - (size.Height << 1));
				}
				return base.ClientRectangle;
			}
		}

		/// <summary>
		///   ClientSize without border
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ClientSizeInner => this.ClientRectangleInner.Size;

		/// <summary>
		///   string identifying this control as a drag operation source
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string DragSourceID {
			get {
				return this.dragSourceID;
			}
			set {
				this.dragSourceID = value;
			}
		}

		/// <summary>
		///   horizontal scroll bar is visible
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool HScrollBarVisible {
			get {
				return this.HScrollBar.Parent != null;
			}
			protected set {
				if (value != this.HScrollBarVisible) {
					this.HScrollBar.Parent = (value ? this : null);
					if (!value) {
						this.HScrollBar.Value = 0;
					}
				}
			}
		}

		/// <summary>
		///   horizontal scroll bar properties
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewScrollProperties HScrollProperties => new BetterListViewScrollProperties(this.HScrollBar);

		/// <summary>
		///   the control is currently being updated
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsUpdating => this.updateLevel != 0;

		/// <summary>
		///   vertical scroll bar is visible
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool VScrollBarVisible {
			get {
				return this.VScrollBar.Parent != null;
			}
			protected set {
				if (value != this.VScrollBarVisible) {
					this.VScrollBar.Parent = (value ? this : null);
					if (!value) {
						this.VScrollBar.Value = 0;
					}
				}
			}
		}

		/// <summary>
		///   vertical scroll bar properties
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BetterListViewScrollProperties VScrollProperties => new BetterListViewScrollProperties(this.VScrollBar);

		/// <summary>
		///   Gets or sets a value indicating whether the control causes validation to be performed on any controls that require validation when it receives focus.
		/// </summary>
		/// <returns>true if the control causes validation to be performed on any controls requiring validation when it receives focus; otherwise, false. The default is true.
		/// </returns>
		[DefaultValue(false)]
		[Category("Focus")]
		public new bool CausesValidation {
			get {
				return base.CausesValidation;
			}
			set {
				base.CausesValidation = value;
			}
		}

		/// <summary>
		///   cache resized images for faster redrawing of the control
		/// </summary>
		[Description("Cache resized images for faster redrawing of the control")]
		[DefaultValue(true)]
		[Category("Performance")]
		public virtual bool CacheImages {
			get {
				return this.cacheImages;
			}
			set {
				this.cacheImages = value;
			}
		}

		/// <summary>
		///   information about the invalidation state
		/// </summary>
		protected BetterListViewInvalidationInfo InvalidationInfo {
			get {
				return this.invalidationInfo;
			}
			set {
				this.invalidationInfo = value;
			}
		}

		/// <summary>
		///   occurs when BorderStyle property has changed
		/// </summary>
		[Description("Occurs when BorderStyle property has changed")]
		[Category("Property Changed")]
		public event EventHandler BorderStyleChanged;

		/// <summary>
		///   occurs when horizontal scroll bar properties has changed
		/// </summary>
		[Description("Occurs when horizontal scroll bar properties has changed")]
		[Category("Property Changed")]
		public event BetterListViewScrollPropertiesChangedEventHandler HScrollPropertiesChanged;

		/// <summary>
		///   occurs when vertical scroll bar properties has changed
		/// </summary>
		[Description("Occurs when vertical scroll bar properties has changed")]
		[Category("Property Changed")]
		public event BetterListViewScrollPropertiesChangedEventHandler VScrollPropertiesChanged;

		/// <summary>
		/// Initialize accessibility support.
		/// </summary>
		protected virtual void AccessibilityInitialize() {
			this.hScrollBar.AccessibleDescription = "Horizontal scroll bar";
			this.hScrollBar.AccessibleName = "Horizontal";
			this.hScrollBar.AccessibleRole = AccessibleRole.ScrollBar;
			this.vScrollBar.AccessibleDescription = "Vertical scroll bar";
			this.vScrollBar.AccessibleName = "Vertical";
			this.vScrollBar.AccessibleRole = AccessibleRole.ScrollBar;
		}

		/// <summary>
		///   Start the auto-scroll feature.
		/// </summary>
		/// <param name="autoScrollMode">auto-scroll mode</param>
		protected void AutoScrollStart(BetterListViewAutoScrollMode autoScrollMode) {
			if (base.InvokeRequired) {
				base.BeginInvoke(new AutoScrollStartDelegate(AutoScrollStart), autoScrollMode);
				return;
			}
			this.autoScrollMode = autoScrollMode;
			if (!this.AutoScrollEnabled) {
				this.autoScrollTimer = new Timer();
				this.autoScrollTimer.Interval = this.AutoScrollStepDelay;
				this.autoScrollTimer.Tick += TimerAutoScrollOnTick;
				this.autoScrollTimer.Start();
			}
		}

		/// <summary>
		///   Stop the auto-scroll feature.
		/// </summary>
		protected void AutoScrollStop() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new AutoScrollStopDelegate(AutoScrollStop));
			}
			else if (this.AutoScrollEnabled) {
				this.autoScrollTimer.Stop();
				this.autoScrollTimer.Tick -= TimerAutoScrollOnTick;
				this.autoScrollTimer.Dispose();
				this.autoScrollTimer = null;
			}
		}

		private void AutoScrollDispose() {
			this.AutoScrollStop();
		}

		private void TimerAutoScrollOnTick(object sender, EventArgs eventArgs) {
			if (Control.MouseButtons == MouseButtons.None) {
				this.AutoScrollStop();
				return;
			}
			Point pt = base.PointToClient(Control.MousePosition);
			int num = this.AutoScrollBoundary;
			int num2 = 0;
			int num3 = 0;
			Rectangle boundsContent = this.BoundsContent;
			switch (this.autoScrollMode) {
				case BetterListViewAutoScrollMode.Horizontal:
					if (pt.X < boundsContent.Left + num) {
						num2 = pt.X - (boundsContent.Left + num);
					}
					else if (pt.X > boundsContent.Right - num) {
						num2 = pt.X - (boundsContent.Right - num);
					}
					break;
				case BetterListViewAutoScrollMode.Inside:
					if (this.ClientRectangleInner.Contains(pt)) {
						if (pt.X <= boundsContent.Left + num) {
							num2 = -32;
						}
						else if (pt.X > boundsContent.Right - num) {
							num2 = 32;
						}
						if (pt.Y <= boundsContent.Top + num) {
							num3 = -32;
						}
						else if (pt.Y > boundsContent.Bottom - num) {
							num3 = 32;
						}
					}
					break;
				case BetterListViewAutoScrollMode.Outside:
					if (pt.X < boundsContent.Left) {
						num2 = pt.X - boundsContent.Left;
					}
					else if (pt.X >= boundsContent.Right) {
						num2 = pt.X - boundsContent.Right + 1;
					}
					if (pt.Y < boundsContent.Top) {
						num3 = pt.Y - boundsContent.Top;
					}
					else if (pt.Y >= boundsContent.Bottom) {
						num3 = pt.Y - boundsContent.Bottom + 1;
					}
					break;
				default:
					throw new ApplicationException($"Unsupported auto scroll mode: '{this.autoScrollMode}'");
			}
			if (num2 == 0 && num3 == 0) {
				return;
			}
			HScrollBar hScrollBar = this.HScrollBar;
			VScrollBar vScrollBar = this.VScrollBar;
			int scrollPositionHorizontal = this.ScrollPositionHorizontal;
			int scrollPositionVertical = this.ScrollPositionVertical;
			int num4 = Math.Min(Math.Max(this.ScrollPositionHorizontal + num2, hScrollBar.Minimum), hScrollBar.Maximum - hScrollBar.LargeChange + 1);
			int num5 = Math.Min(Math.Max(this.ScrollPositionVertical + num3, vScrollBar.Minimum), vScrollBar.Maximum - vScrollBar.LargeChange + 1);
			try {
				this.BeginUpdate();
				if (scrollPositionHorizontal != num4) {
					this.ScrollPositionHorizontal = num4;
				}
				if (scrollPositionVertical != num5) {
					this.ScrollPositionVertical = num5;
				}
			}
			finally {
				this.EndUpdate(suppressRefresh: true);
			}
			if (scrollPositionHorizontal != num4 || scrollPositionVertical != num5) {
				this.OnScrollBarValueChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListView" /> class.
		/// </summary>
		protected BetterListViewBase() {
			try {
				this.BeginUpdate();
				this.BackColor = BetterListViewBase.DefaultBackColorValue;
				this.Font = Control.DefaultFont;
				this.ForeColor = Control.DefaultForeColor;
				base.ResizeRedraw = false;
				base.SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.Selectable | ControlStyles.StandardDoubleClick | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
				base.SetStyle(ControlStyles.SupportsTransparentBackColor, value: false);
				this.AccessibilityInitialize();
				this.DrawingInitialize();
				this.dragSourceID = Guid.NewGuid().ToString();
				this.hScrollBar.EnabledChanged += HScrollBarOnEnabledChanged;
				this.hScrollBar.ValueChanged += OnScrollBarValueChanged;
				this.hScrollBar.VisibleChanged += HScrollBarOnVisibleChanged;
				this.vScrollBar.EnabledChanged += VScrollBarOnEnabledChanged;
				this.vScrollBar.ValueChanged += OnScrollBarValueChanged;
				this.vScrollBar.VisibleChanged += VScrollBarOnVisibleChanged;
			}
			finally {
				this.EndUpdate(suppressRefresh: true);
			}
		}

		/// <summary>
		///   Handles the situation where some collection has changed.
		/// </summary>
		/// <param name="collection">collection that has changed</param>
		/// <param name="changeInfo">information about changes made to the collection</param>
		internal abstract void OnCollectionChanged(BetterListViewElementCollectionBase collection, BetterListViewElementCollectionChangeInfo changeInfo);

		/// <summary>
		///   Called when property of some collection item has changed.
		/// </summary>
		/// <param name="collection">collection containing the element</param>
		/// <param name="elementPropertyType">element property type</param>
		/// <param name="element">element whose property has changed</param>
		/// <param name="oldValue">value of the property before the property was set</param>
		/// <param name="eventArgs">event data (in case of property change event should be fired)</param>
		internal abstract void OnElementPropertyChanged(BetterListViewElementCollectionBase collection, BetterListViewElementPropertyType elementPropertyType, BetterListViewElementBase element, object oldValue, EventArgs eventArgs);

		/// <summary>
		///   Called when layout property has changed.
		/// </summary>
		/// <param name="layout">layout whose property has changed</param>
		/// <param name="layoutPropertyType">update type of the layout property</param>
		internal virtual void OnLayoutPropertyChanged(BetterListViewLayoutBase layout, BetterListViewLayoutPropertyType layoutPropertyType) {
			Checks.CheckNotNull(layout, "layout");
			this.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
			this.RefreshView();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnLostFocus(EventArgs e) {
			this.AutoScrollStop();
			base.OnLostFocus(e);
		}

		/// <summary>
		///   Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control" /> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.AutoScrollDispose();
				this.DrawingDispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		///   Signals the object that initialization is starting.
		/// </summary>
		void ISupportInitialize.BeginInit() {
			this.BeginUpdate();
		}

		/// <summary>
		///   Signals the object that initialization is complete.
		/// </summary>
		void ISupportInitialize.EndInit() {
			this.EndUpdate();
		}

		/// <summary>
		///   Redraw the control.
		/// </summary>
		protected void DrawingRedraw() {
			this.DrawingRedraw(refillBuffer: false);
		}

		/// <summary>
		///   Redraw the control.
		/// </summary>
		/// <param name="graphics">Graphics object used for redrawing</param>
		protected void DrawingRedraw(Graphics graphics) {
			this.DrawingRedraw(graphics, refillBuffer: false);
		}

		/// <summary>
		///   Internal method for redrawing doing the actual drawing.
		/// </summary>
		/// <param name="graphics">Graphics object used for redrawing</param>
		protected virtual void DrawingRedrawCore(Graphics graphics) {
			if (this.DrawBorder) {
				if (this.DrawBorder3D) {
					ControlPaint.DrawBorder3D(graphics, base.ClientRectangle, Border3DStyle.Sunken);
				}
				else {
					ControlPaint.DrawBorder(graphics, base.ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);
				}
			}
		}

		/// <summary>
		///   Raises the <see cref="E:Paint" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.Windows.Forms.PaintEventArgs" /> instance containing the event data.</param>
		protected override void OnPaint(PaintEventArgs eventArgs) {
			if (this.backBufferEmpty) {
				this.backBufferEmpty = false;
				this.DrawingRedraw(refillBuffer: true);
			}
			if (this.backBufferGraphics != null) {
				this.backBufferGraphics.Render(eventArgs.Graphics);
			}
		}

		/// <summary>
		///   Raises the <see cref="E:ParentChanged" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
		protected override void OnParentChanged(EventArgs eventArgs) {
			base.OnParentChanged(eventArgs);
			if (!base.Disposing && !base.IsDisposed && base.IsHandleCreated) {
				this.Invalidate(BetterListViewInvalidationLevel.MeasureElements, BetterListViewInvalidationFlags.Position | BetterListViewInvalidationFlags.Draw);
				this.RefreshView();
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
		protected override void OnVisibleChanged(EventArgs eventArgs) {
			this.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
			this.RefreshView();
			base.OnVisibleChanged(eventArgs);
		}

		private void DrawingRedraw(bool refillBuffer) {
			if (!this.IsUpdating || refillBuffer) {
				this.backBufferEmpty = false;
				if (this.backBufferGraphics != null) {
					this.DrawingRedraw(this.backBufferGraphics.Graphics, refillBuffer);
				}
			}
		}

		private void DrawingRedraw(Graphics graphics, bool refillBuffer) {
			this.DrawingRedrawCore(graphics);
			if (!refillBuffer) {
				base.Invalidate();
				base.Update();
			}
		}

		private void DrawingInitialize() {
			this.backBufferContext = new BufferedGraphicsContext();
			this.DrawingRecreateBuffers();
		}

		private void DrawingRecreateBuffers() {
			if (this.backBufferContext != null) {
				this.backBufferContext.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
				if (this.backBufferGraphics != null) {
					this.backBufferGraphics.Dispose();
				}
				this.backBufferGraphics = this.backBufferContext.Allocate(base.CreateGraphics(), new Rectangle(0, 0, Math.Max(base.Width, 1), Math.Max(base.Height, 1)));
				this.backBufferEmpty = true;
				base.Invalidate();
			}
		}

		private void DrawingDispose() {
			if (this.backBufferGraphics != null) {
				this.backBufferGraphics.Dispose();
				this.backBufferGraphics = null;
			}
			if (this.backBufferContext != null) {
				this.backBufferContext.Dispose();
				this.backBufferContext = null;
			}
		}

		/// <summary>
		///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListViewBase.BorderStyleChanged" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
		protected virtual void OnBorderStyleChanged(EventArgs eventArgs) {
			if (this.BorderStyleChanged != null) {
				this.BorderStyleChanged(this, eventArgs);
			}
		}

		/// <summary>
		///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListViewBase.HScrollPropertiesChanged" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewScrollPropertiesChangedEventArgs" /> instance containing the event data.</param>
		protected virtual void OnHScrollPropertiesChanged(BetterListViewScrollPropertiesChangedEventArgs eventArgs) {
			if (this.HScrollPropertiesChanged != null) {
				this.HScrollPropertiesChanged(this, eventArgs);
			}
		}

		/// <summary>
		///   Raises the <see cref="E:ComponentOwl.BetterListView.BetterListViewBase.VScrollPropertiesChanged" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:ComponentOwl.BetterListView.BetterListViewScrollPropertiesChangedEventArgs" /> instance containing the event data.</param>
		protected virtual void OnVScrollPropertiesChanged(BetterListViewScrollPropertiesChangedEventArgs eventArgs) {
			if (this.VScrollPropertiesChanged != null) {
				this.VScrollPropertiesChanged(this, eventArgs);
			}
		}

		private void HScrollBarOnEnabledChanged(object sender, EventArgs eventArgs) {
			this.OnHScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.HScrollProperties));
		}

		private void HScrollBarOnVisibleChanged(object sender, EventArgs eventArgs) {
			this.OnHScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.HScrollProperties));
		}

		private void VScrollBarOnEnabledChanged(object sender, EventArgs eventArgs) {
			this.OnVScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.VScrollProperties));
		}

		private void VScrollBarOnVisibleChanged(object sender, EventArgs eventArgs) {
			this.OnVScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.VScrollProperties));
		}

		/// <summary>
		///   Get content area boundaries.
		/// </summary>
		/// <param name="widthExceeded">layout width exceeds the content area</param>
		/// <param name="heightExceeded">layout height exceeds the content area</param>
		/// <returns>content area boundaries</returns>
		protected abstract Rectangle GetContentBounds(bool widthExceeded, bool heightExceeded);

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.Control.Resize" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
		protected override void OnResize(EventArgs eventArgs) {
			base.OnResize(eventArgs);
			if (!base.Disposing && !base.IsDisposed && base.IsHandleCreated) {
				this.DrawingRecreateBuffers();
				this.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
			}
			this.RefreshView();
		}

		/// <summary>
		/// Called when value of one of the scroll bar changes.
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="e">Event data.</param>
		protected virtual void OnScrollBarValueChanged(object sender, EventArgs eventArgs) {
			if (sender == this.hScrollBar) {
				this.OnHScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.HScrollProperties));
			}
			else {
				this.OnVScrollPropertiesChanged(new BetterListViewScrollPropertiesChangedEventArgs(this.VScrollProperties));
			}
		}

		/// <summary>
		///   Resets the <see cref="P:System.Windows.Forms.Control.BackColor" /> property to its default value.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetBackColor() {
			this.BackColor = BetterListViewBase.DefaultBackColorValue;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetBorderStyle() {
			this.BorderStyle = BorderStyle.Fixed3D;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private new void ResetFont() {
			base.Font = Control.DefaultFont;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private new void ResetForeColor() {
			this.ForeColor = Control.DefaultForeColor;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetFontItems() {
			this.fontItems = null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private void ResetForeColorItems() {
			this.foreColorItems = BetterListViewBase.DefaultForeColorItems;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeBackColor() {
			return this.BackColor != BetterListViewBase.DefaultBackColorValue;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeBorderStyle() {
			return this.BorderStyle != BorderStyle.Fixed3D;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeFont() {
			return !base.Font.Equals(Control.DefaultFont);
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeFontItems() {
			return this.fontItems != null;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeForeColor() {
			return this.ForeColor != Control.DefaultForeColor;
		}

		[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
		private bool ShouldSerializeForeColorItems() {
			return this.foreColorItems != BetterListViewBase.DefaultForeColorItems;
		}

		/// <summary>
		///   Suspend refreshing logic for the control.
		/// </summary>
		public void BeginUpdate() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new BeginUpdateDelegate(BeginUpdate));
			}
			else {
				this.updateLevel++;
			}
		}

		/// <summary>
		///   Resume refreshing logic for the control.
		/// </summary>
		public void EndUpdate() {
			this.EndUpdate(suppressRefresh: false);
		}

		/// <summary>
		///   Resume refreshing logic for the control.
		/// </summary>
		/// <param name="suppressRefresh">do not refresh the control immediately</param>
		public void EndUpdate(bool suppressRefresh) {
			if (base.InvokeRequired) {
				base.BeginInvoke(new EndUpdateDelegate(EndUpdate), suppressRefresh);
			}
			else if (this.updateLevel != 0) {
				this.updateLevel--;
				if (this.updateLevel == 0 && !suppressRefresh) {
					this.RefreshView();
				}
			}
		}

		/// <summary>
		///   Forces the control to invalidate its client area and immediately redraw itself and any child controls.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		public override void Refresh() {
			this.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw, this.ClientRectangleInner);
			this.RefreshView();
			base.Refresh();
		}

		/// <summary>
		///   Invalidate the control for later update.
		/// </summary>
		/// <param name="level">control invalidation level</param>
		/// <param name="flags">control invalidation options</param>
		internal void Invalidate(BetterListViewInvalidationLevel level, BetterListViewInvalidationFlags flags) {
			this.Invalidate(new BetterListViewInvalidationInfo(level, flags, this.ClientRectangleInner));
		}

		internal void Invalidate(BetterListViewInvalidationLevel level, BetterListViewInvalidationFlags flags, Rectangle region) {
			this.Invalidate(new BetterListViewInvalidationInfo(level, flags, region));
		}

		/// <summary>
		///   Invalidate the control for later update.
		/// </summary>
		/// <param name="invalidationInfo">control invalidation information</param>
		internal void Invalidate(BetterListViewInvalidationInfo invalidationInfo) {
			this.invalidationInfo = this.invalidationInfo.UnionWith(invalidationInfo);
		}

		/// <summary>
		///   Refresh the control with current invalidation information.
		/// </summary>
		internal virtual void RefreshView() {
			if (!this.IsUpdating && !base.Disposing && !base.IsDisposed && this.CachedGraphics != null) {
				this.invalidationInfo = new BetterListViewInvalidationInfo(BetterListViewInvalidationLevel.None, this.invalidationInfo.Flags, Rectangle.Empty);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated" /> event.
		/// </summary>
		/// <param name="eventArgs">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
		protected override void OnHandleCreated(EventArgs eventArgs) {
			this.Invalidate(BetterListViewInvalidationLevel.None, BetterListViewInvalidationFlags.Draw);
			this.RefreshView();
			base.OnHandleCreated(eventArgs);
		}
	}
}