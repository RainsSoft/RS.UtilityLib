using System;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
	/// <summary>
	/// This control is designed primarily to be a progress bar. But it's shape is circular.<br/>
	/// Unlike the System.Windows.Forms.ProgressBar, it has no upper limit. You may use it in the processes that you don't know when it finishes.
	/// </summary>
	//[ToolboxBitmap(typeof(IRobotQHostEdit.MyUI.ProgressCircle), "CircleToolboxBitmap.bmp")]
	public class ProgressCircle : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Timer timer;

		private int SeperatorAngle = 5;

		private int _index;
		private int _numberOfArcs;
		private int _ringThickness;
		private Color _ringColor;
		private int _numberOfTail;

		/// <summary>
		/// This is the number of pies, following the moving pie.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Value can not be less than zero</exception>
		[
		Category("ProgressRing"),
		DefaultValue(4),
		Bindable(true)
		]
		public int NumberOfTail {
			get {
				return this._numberOfTail;
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("Value can not be zero");

				this._numberOfTail = value;

				this.UpdateStyles();
				this.Invalidate();
			}
		}

		/// <summary>
		/// Background color for the moving pie.
		/// </summary>
		/// <remarks>
		/// Default color is white
		/// </remarks>
		[
		Category("ProgressRing"),
		Bindable(true)
		]
		public Color RingColor {
			get {
				// Default ring color is White
				if (this._ringColor == Color.Empty)
					return Color.White;

				return this._ringColor;
			}
			set {
				this._ringColor = value;

				// Redraw the control
				this.UpdateStyles();
				this.Invalidate();
			}
		}

		/// <summary>
		/// Background color for the background pies.
		/// </summary>
		[
		Category("ProgressRing"),
		Bindable(true)
		]
		public override Color ForeColor {
			get {
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;

				// Redraw the control
				this.UpdateStyles();
				this.Invalidate();
			}
		}

		/// <summary>
		/// Number of pies that will be places inside the cicle.
		/// </summary>
		/// <remarks>
		/// Value should be a divisor of 360 (In other words when 360 is divided to value, the result must be integer).
		/// </remarks>
		/// <exception cref="System.ArgumentException">360 should be divisible by the value given.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Value must be greater than zero.</exception>
		[
		Category("ProgressRing"),
		DefaultValue(8),
		Bindable(true)
		]
		public int NumberOfArcs {
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException("Value must be greater than zero");

				// 360 degree is the total angle of a circle.
				if ((360 % value) != 0)
					throw new ArgumentException("360 should be divisible by NumberOfArcs property. 360 is not divisible by " + value.ToString());

				this._numberOfArcs = value;

				this.UpdateStyles();
				this.Invalidate();
			}
			get {
				return this._numberOfArcs;
			}
		}

		/// <summary>
		/// Radius of the circle.
		/// </summary>
		/// <remarks>Value must be greater than the half of the width or height.</remarks>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Value must be greater than zero<br/>
		/// </exception>
		[
		Category("ProgressRing"),
		DefaultValue(5),
		Bindable(true)
		]
		public int RingThickness {
			get {
				return this._ringThickness;
			}
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException("Value must be greater than zero");

				// Value cannot be bigger than the rectanle.
				int limit = Math.Min(this.Width, this.Height) / 2;
				if (value >= limit)
					throw new ArgumentOutOfRangeException(string.Format("Value must be smaller than {0} for this size, {1}", limit, this.ClientRectangle.ToString()));

				this._ringThickness = value;

				// Redraw control
				this.UpdateStyles();
				this.Invalidate();
			}
		}

		/// <summary>
		/// To start the animation, set this true.<br/>
		/// To stop, set it false.
		/// </summary>
		/// <remarks>
		/// After stopping the animation, you may clear the rotating part, by calling <c>Clear</c> method.
		/// </remarks>
		[
		System.ComponentModel.Browsable(false),
		DefaultValue(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool Rotate {
			get {
				return this.timer.Enabled;
			}
			set {
				this.timer.Enabled = value;
			}
		}

		/// <summary>
		/// This value is in miliseconds. Greater interval, slow animation.
		/// </summary>
		[
		Category("ProgressRing"),
		DefaultValue(150),
		Bindable(true)
		]
		public int Interval {
			get {
				return this.timer.Interval;
			}
			set {
				this.timer.Interval = value;
			}
		}

		private bool IsTransparent {
			get {
				return (this.BackColor == Color.Transparent);
			}
		}

		private int PieAngle {
			get {
				// value is the pie that will be drawn and the seperator angle
				int angleOfPieWithSeperator = 360 / this.NumberOfArcs;

				// This is the pie that will be drawn to the client
				int pieAngle = angleOfPieWithSeperator - this.SeperatorAngle;

				return pieAngle;
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ProgressCircle() {
			// To minimize the flicking
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			// Enable transparent BackColor
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			// Redraw the control after its size is changed
			this.ResizeRedraw = true;

			// Never use this property. This indicated which pie will be drawn.
			this._index = 1;

			this._numberOfArcs = 8;
			this._ringThickness = 5;
			this._ringColor = Color.Empty;
			this._numberOfTail = 4;

			this.timer = new Timer();
			this.timer.Interval = 150; // Each 150 miliseconds, the progress circle will be drawn again
			this.timer.Tick += new EventHandler(timer_Tick);
			this.timer.Enabled = true;
		}

		public void Clear() {
			// Clears the pies with default ring color
			using (Graphics grp = this.CreateGraphics())
				this.FillEmptyArcs(grp);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			if (!this.IsTransparent)
				base.OnPaintBackground(pevent);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			if (this.DesignMode) {
				this.timer.Enabled = false;
			}
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams p = base.CreateParams;
				if (IsTransparent)
					p.ExStyle |= 0x20;
				return p;
			}
		}

		protected override void OnMove(EventArgs e) {
			if (!IsTransparent)
				base.OnMove(e);
			else
				this.RecreateHandle();
		}

		protected override void OnBackColorChanged(EventArgs e) {
			this.UpdateStyles();
			base.OnBackColorChanged(e);
		}


		protected override void OnPaint(PaintEventArgs e) {
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			// Fill static ring part
			this.FillEmptyArcs(e.Graphics);

			// Fill animation part
			this.FillPieAndTail();
		}

		/// <summary>
		/// This method draws the static, non-animation part.
		/// </summary>
		/// <param name="grp"></param>
		private void FillEmptyArcs(Graphics grp) {
			int startAngle = 0;

			for (int i = 0; i < this.NumberOfArcs; i++) {
				this.DrawFilledArc(grp, this.RingColor, startAngle);

				startAngle += this.PieAngle + this.SeperatorAngle;
			}
		}

		private void DrawFilledArc(Graphics grp, Color color, int startAngle) {
			grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			Rectangle main = this.ClientRectangle;

			// If there is no region to be drawn, then this method terminates itself
			if (main.Width - (2 * this._ringThickness) < 1 || main.Height - (2 * this._ringThickness) <= 1)
				return;

			// Calculates the region that will be filled
			GraphicsPath outerPath = new GraphicsPath();
			outerPath.AddPie(main, startAngle, this.PieAngle);

			Rectangle sub = new Rectangle(main.X + this._ringThickness, main.Y + this._ringThickness, main.Width - (2 * this._ringThickness), main.Height - (2 * this._ringThickness));
			GraphicsPath innerPath = new GraphicsPath();
			innerPath.AddPie(sub, startAngle - 1, this.PieAngle + 2);

			System.Drawing.Region mainRegion = new Region(outerPath);
			System.Drawing.Region subRegion = new Region(innerPath);
			mainRegion.Exclude(subRegion);

			// Fill that region
			grp.FillRegion(new SolidBrush(color), mainRegion);
		}

		private void ChangeIndex() {
			// Fills the animation part
			this.FillPieAndTail();

			// After the invocation of this method, index is changed. So at another invocation of this method, next pie will be drawn
			this._index = (this._index + 1) % this.NumberOfArcs;
		}

		/// <summary>
		/// Draws the animation part
		/// </summary>
		private void FillPieAndTail() {
			Color color = this.ForeColor;

			for (int i = 0; i <= this.NumberOfTail; i++) {
				this.FillPieAccordingToTheIndex(this._index - i, color);

				// If there is tail, then the tail color is the lighter of the ForeColor
				color = ControlPaint.Light(color);
			}

			// Background Pie
			this.FillPieAccordingToTheIndex(this._index - (this.NumberOfTail + 1), this.RingColor);
		}

		private void FillPieAccordingToTheIndex(int index, Color color) {
			int count = index % this.NumberOfArcs;
			int angle = count * (this.PieAngle + this.SeperatorAngle);

			using (Graphics grp = this.CreateGraphics()) {
				grp.SmoothingMode = SmoothingMode.HighQuality;
				this.DrawFilledArc(grp, color, angle);
			}
		}

		private void timer_Tick(object sender, EventArgs e) {
			this.ChangeIndex();
		}
	}
}
