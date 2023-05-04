using System.ComponentModel;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   ToolTip appearance and behavior options.
	/// </summary>
	[TypeConverter(typeof(BetterListViewToolTipOptionsConverter))]
	public struct BetterListViewToolTipOptions
	{
		private const int DefaultAutomaticDelay = 500;

		private const int DefaultAutoPopDelay = 3000;

		private const int DefaultInitialDelay = 500;

		private const int DefaultReshowDelay = 100;

		private const bool DefaultShowAlways = false;

		private const bool DefaultUseAnimation = true;

		private const bool DefaultUseFading = true;

		/// <summary>
		///   BetterListViewToolTipOptions structure with default values
		/// </summary>
		public static readonly BetterListViewToolTipOptions Default = new BetterListViewToolTipOptions(500, 3000, 500, 100, showAlways: false, useAnimation: true, useFading: true);

		private int automaticDelay;

		private int autoPopDelay;

		private int initialDelay;

		private int reshowDelay;

		private bool showAlways;

		private bool useAnimation;

		private bool useFading;

		/// <summary>
		///   automatic delay for the ToolTip
		/// </summary>
		public int AutomaticDelay {
			get {
				return this.automaticDelay;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.automaticDelay = value;
			}
		}

		/// <summary>
		///   period of time the ToolTip remains visible if the pointer is stationary on a control
		/// </summary>
		public int AutoPopDelay {
			get {
				return this.autoPopDelay;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.autoPopDelay = value;
			}
		}

		/// <summary>
		///   time that passes before the ToolTip appears
		/// </summary>
		public int InitialDelay {
			get {
				return this.initialDelay;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.initialDelay = value;
			}
		}

		/// <summary>
		///   length of time that must transpire before subsequent ToolTip windows appear as the pointer moves from one control part to another
		/// </summary>
		public int ReshowDelay {
			get {
				return this.reshowDelay;
			}
			set {
				Checks.CheckTrue(value > 0, "value > 0");
				this.reshowDelay = value;
			}
		}

		/// <summary>
		///   ToolTip window is displayed, even when the control is not active
		/// </summary>
		public bool ShowAlways {
			get {
				return this.showAlways;
			}
			set {
				this.showAlways = value;
			}
		}

		/// <summary>
		///   use animation effect when displaying a ToolTip
		/// </summary>
		public bool UseAnimation {
			get {
				return this.useAnimation;
			}
			set {
				this.useAnimation = value;
			}
		}

		/// <summary>
		///   use fade effect when displaying a ToolTip
		/// </summary>
		public bool UseFading {
			get {
				return this.useFading;
			}
			set {
				this.useFading = value;
			}
		}

		/// <summary>
		///   this BetterListViewToolTipOptions structure contains default values
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDefault => this.Equals(BetterListViewToolTipOptions.Default);

		/// <summary>
		///   Initialize a new BetterListViewToolTipOptions instance.
		/// </summary>
		/// <param name="automaticDelay">automatic delay for the ToolTip</param>
		/// <param name="autoPopDelay">period of time the ToolTip remains visible if the pointer is stationary on a control</param>
		/// <param name="initialDelay">time that passes before the ToolTip appears</param>
		/// <param name="reshowDelay">length of time that must transpire before subsequent ToolTip windows appear as the pointer moves from one control part to another</param>
		/// <param name="showAlways">ToolTip window is displayed, even when the control is not active</param>
		/// <param name="useAnimation">use animation effect when displaying a ToolTip</param>
		/// <param name="useFading">use fade effect when displaying a ToolTip</param>
		public BetterListViewToolTipOptions(int automaticDelay, int autoPopDelay, int initialDelay, int reshowDelay, bool showAlways, bool useAnimation, bool useFading) {
			Checks.CheckTrue(automaticDelay > 0, "automaticDelay > 0");
			Checks.CheckTrue(autoPopDelay > 0, "autoPopDelay > 0");
			Checks.CheckTrue(initialDelay > 0, "initialDelay > 0");
			Checks.CheckTrue(reshowDelay > 0, "reshowDelay > 0");
			this.automaticDelay = automaticDelay;
			this.autoPopDelay = autoPopDelay;
			this.initialDelay = initialDelay;
			this.reshowDelay = reshowDelay;
			this.showAlways = showAlways;
			this.useAnimation = useAnimation;
			this.useFading = useFading;
		}

		/// <summary>
		///   Test whether the two BetterListViewToolTipOptions objects are identical.
		/// </summary>
		/// <param name="toolTipOptionsA">first BetterListViewToolTipOptions object</param>
		/// <param name="toolTipOptionsB">second BetterListViewToolTipOptions object</param>
		/// <returns>the two BetterListViewToolTipOptions objects are identical</returns>
		public static bool operator ==(BetterListViewToolTipOptions toolTipOptionsA, BetterListViewToolTipOptions toolTipOptionsB) {
			return toolTipOptionsA.Equals(toolTipOptionsB);
		}

		/// <summary>
		///   Test whether the two BetterListViewToolTipOptions objects are different.
		/// </summary>
		/// <param name="toolTipOptionsA">first BetterListViewToolTipOptions object</param>
		/// <param name="toolTipOptionsB">second BetterListViewToolTipOptions object</param>
		/// <returns>the two BetterListViewToolTipOptions objects are different</returns>
		public static bool operator !=(BetterListViewToolTipOptions toolTipOptionsA, BetterListViewToolTipOptions toolTipOptionsB) {
			return !toolTipOptionsA.Equals(toolTipOptionsB);
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
			if (!(obj is BetterListViewToolTipOptions betterListViewToolTipOptions)) {
				return false;
			}
			if (this.automaticDelay == betterListViewToolTipOptions.automaticDelay && this.autoPopDelay == betterListViewToolTipOptions.autoPopDelay && this.initialDelay == betterListViewToolTipOptions.initialDelay && this.reshowDelay == betterListViewToolTipOptions.reshowDelay && this.showAlways == betterListViewToolTipOptions.showAlways && this.useAnimation == betterListViewToolTipOptions.useAnimation) {
				return this.useFading == betterListViewToolTipOptions.useFading;
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
			return this.automaticDelay.GetHashCode() ^ this.autoPopDelay.GetHashCode() ^ this.initialDelay.GetHashCode() ^ this.reshowDelay.GetHashCode() ^ this.showAlways.GetHashCode() ^ this.useAnimation.GetHashCode() ^ this.useFading.GetHashCode();
		}
	}
}