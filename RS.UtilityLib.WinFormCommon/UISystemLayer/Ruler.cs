using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    #region
    /// <summary>
    /// Specifies the unit of measure for the given data.
    /// </summary>
    /// <remarks>
    /// These enumeration values correspond to the values used in the EXIF ResolutionUnit tag.
    /// </remarks>
    public enum MeasurementUnit
        : int
    {
        Pixel = 1,
        Inch = 2,
        Centimeter = 3
    }
    /// <summary>
    /// Encapsulates functionality for zooming/scaling coordinates.
    /// Includes methods for Size[F]'s, Point[F]'s, Rectangle[F]'s,
    /// and various scalars
    /// </summary>
    public struct ScaleFactor
    {
        private int denominator;
        private int numerator;

        public int Denominator {
            get {
                return denominator;
            }
        }

        public int Numerator {
            get {
                return numerator;
            }
        }

        public double Ratio {
            get {
                return (double)numerator / (double)denominator;
            }
        }

        public static readonly ScaleFactor OneToOne = new ScaleFactor(1, 1);
        public static readonly ScaleFactor MinValue = new ScaleFactor(1, 100);
        public static readonly ScaleFactor MaxValue = new ScaleFactor(32, 1);

        private void Clamp() {
            if (this < MinValue) {
                this = MinValue;
            }
            else if (this > MaxValue) {
                this = MaxValue;
            }
        }

        public static ScaleFactor UseIfValid(int numerator, int denominator, ScaleFactor lastResort) {
            if (numerator <= 0 || denominator <= 0) {
                return lastResort;
            }
            else {
                return new ScaleFactor(numerator, denominator);
            }
        }

        public static ScaleFactor Min(int n1, int d1, int n2, int d2, ScaleFactor lastResort) {
            ScaleFactor a = UseIfValid(n1, d1, lastResort);
            ScaleFactor b = UseIfValid(n2, d2, lastResort);
            return ScaleFactor.Min(a, b);
        }

        public static ScaleFactor Max(int n1, int d1, int n2, int d2, ScaleFactor lastResort) {
            ScaleFactor a = UseIfValid(n1, d1, lastResort);
            ScaleFactor b = UseIfValid(n2, d2, lastResort);
            return ScaleFactor.Max(a, b);
        }

        public static ScaleFactor Min(ScaleFactor lhs, ScaleFactor rhs) {
            if (lhs < rhs) {
                return lhs;
            }
            else {
                return rhs;
            }
        }

        public static ScaleFactor Max(ScaleFactor lhs, ScaleFactor rhs) {
            if (lhs > rhs) {
                return lhs;
            }
            else {
                return lhs;
            }
        }

        public static bool operator ==(ScaleFactor lhs, ScaleFactor rhs) {
            return (lhs.numerator * rhs.denominator) == (rhs.numerator * lhs.denominator);
        }

        public static bool operator !=(ScaleFactor lhs, ScaleFactor rhs) {
            return !(lhs == rhs);
        }

        public static bool operator <(ScaleFactor lhs, ScaleFactor rhs) {
            return (lhs.numerator * rhs.denominator) < (rhs.numerator * lhs.denominator);
        }

        public static bool operator <=(ScaleFactor lhs, ScaleFactor rhs) {
            return (lhs.numerator * rhs.denominator) <= (rhs.numerator * lhs.denominator);
        }

        public static bool operator >(ScaleFactor lhs, ScaleFactor rhs) {
            return (lhs.numerator * rhs.denominator) > (rhs.numerator * lhs.denominator);
        }

        public static bool operator >=(ScaleFactor lhs, ScaleFactor rhs) {
            return (lhs.numerator * rhs.denominator) >= (rhs.numerator * lhs.denominator);
        }

        public override bool Equals(object obj) {
            if (obj is ScaleFactor) {
                ScaleFactor rhs = (ScaleFactor)obj;
                return this == rhs;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            return numerator.GetHashCode() ^ denominator.GetHashCode();
        }

        // private static string percentageFormat = PdnResources.GetString("ScaleFactor.Percentage.Format");
        //public override string ToString() {
        //    return string.Format(percentageFormat, Math.Round(100 * Ratio));
        //}

        public int ScaleScalar(int x) {
            return (int)(((long)x * numerator) / denominator);
        }

        public int UnscaleScalar(int x) {
            return (int)(((long)x * denominator) / numerator);
        }

        public float ScaleScalar(float x) {
            return (x * (float)numerator) / (float)denominator;
        }

        public float UnscaleScalar(float x) {
            return (x * (float)denominator) / (float)numerator;
        }

        public double ScaleScalar(double x) {
            return (x * (double)numerator) / (double)denominator;
        }

        public double UnscaleScalar(double x) {
            return (x * (double)denominator) / (double)numerator;
        }

        public Point ScalePoint(Point p) {
            return new Point(ScaleScalar(p.X), ScaleScalar(p.Y));
        }

        public PointF ScalePoint(PointF p) {
            return new PointF(ScaleScalar(p.X), ScaleScalar(p.Y));
        }

        public PointF ScalePointJustX(PointF p) {
            return new PointF(ScaleScalar(p.X), p.Y);
        }

        public PointF ScalePointJustY(PointF p) {
            return new PointF(p.X, ScaleScalar(p.Y));
        }

        public PointF UnscalePoint(PointF p) {
            return new PointF(UnscaleScalar(p.X), UnscaleScalar(p.Y));
        }

        public PointF UnscalePointJustX(PointF p) {
            return new PointF(UnscaleScalar(p.X), p.Y);
        }

        public PointF UnscalePointJustY(PointF p) {
            return new PointF(p.X, UnscaleScalar(p.Y));
        }

        public Point ScalePointJustX(Point p) {
            return new Point(ScaleScalar(p.X), p.Y);
        }

        public Point ScalePointJustY(Point p) {
            return new Point(p.X, ScaleScalar(p.Y));
        }

        public Point UnscalePoint(Point p) {
            return new Point(UnscaleScalar(p.X), UnscaleScalar(p.Y));
        }

        public Point UnscalePointJustX(Point p) {
            return new Point(UnscaleScalar(p.X), p.Y);
        }

        public Point UnscalePointJustY(Point p) {
            return new Point(p.X, UnscaleScalar(p.Y));
        }

        public SizeF ScaleSize(SizeF s) {
            return new SizeF(ScaleScalar(s.Width), ScaleScalar(s.Height));
        }

        public SizeF UnscaleSize(SizeF s) {
            return new SizeF(UnscaleScalar(s.Width), UnscaleScalar(s.Height));
        }

        public Size ScaleSize(Size s) {
            return new Size(ScaleScalar(s.Width), ScaleScalar(s.Height));
        }

        public Size UnscaleSize(Size s) {
            return new Size(UnscaleScalar(s.Width), UnscaleScalar(s.Height));
        }

        public RectangleF ScaleRectangle(RectangleF rectF) {
            return new RectangleF(ScalePoint(rectF.Location), ScaleSize(rectF.Size));
        }

        public RectangleF UnscaleRectangle(RectangleF rectF) {
            return new RectangleF(UnscalePoint(rectF.Location), UnscaleSize(rectF.Size));
        }

        public Rectangle ScaleRectangle(Rectangle rect) {
            return new Rectangle(ScalePoint(rect.Location), ScaleSize(rect.Size));
        }

        public Rectangle UnscaleRectangle(Rectangle rect) {
            return new Rectangle(UnscalePoint(rect.Location), UnscaleSize(rect.Size));
        }

        private static readonly double[] scales =
            {
                0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.08, 0.12, 0.16, 0.25, 0.33, 0.50, 0.66, 1,
                2, 3, 4, 5, 6, 7, 8, 12, 16, 24, 32
            };

        /// <summary>
        /// Gets a list of values that GetNextLarger() and GetNextSmaller() will cycle through.
        /// </summary>
        /// <remarks>
        /// 1.0 is guaranteed to be in the array returned by this property. This list is also
        /// sorted in ascending order.
        /// </remarks>
        public static double[] PresetValues {
            get {
                double[] returnValue = new double[scales.Length];
                scales.CopyTo(returnValue, 0);
                return returnValue;
            }
        }

        /// <summary>
        /// Rounds the current scaling factor up to the next power of two.
        /// </summary>
        /// <returns>The new ScaleFactor value.</returns>
        public ScaleFactor GetNextLarger() {
            double ratio = Ratio + 0.005;

            int index = Array.FindIndex(
                scales,
                delegate (double scale) {
                    return ratio <= scale;
                });

            if (index == -1) {
                index = scales.Length;
            }

            index = Math.Min(index, scales.Length - 1);

            return ScaleFactor.FromDouble(scales[index]);
        }

        public ScaleFactor GetNextSmaller() {
            double ratio = Ratio - 0.005;

            int index = Array.FindIndex(
                scales,
                delegate (double scale) {
                    return ratio <= scale;
                });

            --index;

            if (index == -1) {
                index = 0;
            }

            index = Math.Max(index, 0);

            return ScaleFactor.FromDouble(scales[index]);
        }

        private static ScaleFactor Reduce(int numerator, int denominator) {
            int factor = 2;

            while (factor < denominator && factor < numerator) {
                if ((numerator % factor) == 0 && (denominator % factor) == 0) {
                    numerator /= factor;
                    denominator /= factor;
                }
                else {
                    ++factor;
                }
            }

            return new ScaleFactor(numerator, denominator);
        }

        public static ScaleFactor FromDouble(double scalar) {
            int numerator = (int)(Math.Floor(scalar * 1000.0));
            int denominator = 1000;
            return Reduce(numerator, denominator);
        }

        public ScaleFactor(int numerator, int denominator) {
            if (denominator <= 0) {
                throw new ArgumentOutOfRangeException("denominator", "must be greater than 0");
            }

            if (numerator < 0) {
                throw new ArgumentOutOfRangeException("numerator", "must be greater than 0");
            }

            this.numerator = numerator;
            this.denominator = denominator;
            this.Clamp();
        }
    }
    #endregion
    public sealed class Ruler
        : UserControl
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private MeasurementUnit measurementUnit = MeasurementUnit.Inch;
        public MeasurementUnit @MeasurementUnit {
            get {
                return measurementUnit;
            }

            set {
                if (value != measurementUnit) {
                    measurementUnit = value;
                    Invalidate();
                }
            }
        }

        private Orientation orientation = Orientation.Horizontal;

        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation {
            get {
                return orientation;
            }

            set {
                if (orientation != value) {
                    orientation = value;
                    Invalidate();
                }
            }
        }

        private double dpu = 96;

        [DefaultValue(96.0)]
        public double Dpu {
            get {
                return dpu;
            }

            set {
                if (value != dpu) {
                    dpu = value;
                    Invalidate();
                }
            }
        }

        private ScaleFactor scaleFactor = ScaleFactor.OneToOne;

        [Browsable(false)]
        public ScaleFactor ScaleFactor {
            get {
                return scaleFactor;
            }

            set {
                if (scaleFactor != value) {
                    scaleFactor = value;
                    Invalidate();
                }
            }
        }

        private float offset = 0;

        [DefaultValue(0)]
        public float Offset {
            get {
                return offset;
            }

            set {
                if (offset != value) {
                    offset = value;
                    Invalidate();
                }
            }
        }

        private float rulerValue = 0.0f;

        [DefaultValue(0)]
        public float Value {
            get {
                return rulerValue;
            }

            set {
                if (this.rulerValue != value) {
                    float oldStart = this.scaleFactor.ScaleScalar(this.rulerValue - offset) - 1;
                    float oldEnd = this.scaleFactor.ScaleScalar(this.rulerValue + 1 - offset) + 1;
                    RectangleF oldRect;

                    if (this.orientation == Orientation.Horizontal) {
                        oldRect = new RectangleF(oldStart, this.ClientRectangle.Top, oldEnd - oldStart, this.ClientRectangle.Height);
                    }
                    else // if (this.orientation == Orientation.Vertical)
                    {
                        oldRect = new RectangleF(this.ClientRectangle.Left, oldStart, this.ClientRectangle.Width, oldEnd - oldStart);
                    }

                    float newStart = this.scaleFactor.ScaleScalar(value - offset);
                    float newEnd = this.scaleFactor.ScaleScalar(value + 1 - offset);
                    RectangleF newRect;

                    if (this.orientation == Orientation.Horizontal) {
                        newRect = new RectangleF(newStart, this.ClientRectangle.Top, newEnd - newStart, this.ClientRectangle.Height);
                    }
                    else // if (this.orientation == Orientation.Vertical)
                    {
                        newRect = new RectangleF(this.ClientRectangle.Left, newStart, this.ClientRectangle.Width, newEnd - newStart);
                    }

                    this.rulerValue = value;

                    Invalidate(UISystemLayer.Utility.RoundRectangle(oldRect));
                    Invalidate(UISystemLayer.Utility.RoundRectangle(newRect));
                }
            }
        }

        private float highlightStart = 0.0f;
        public float HighlightStart {
            get {
                return this.highlightStart;
            }

            set {
                if (this.highlightStart != value) {
                    this.highlightStart = value;
                    Invalidate();
                }
            }
        }

        private float highlightLength = 0.0f;
        public float HighlightLength {
            get {
                return this.highlightLength;
            }

            set {
                if (this.highlightLength != value) {
                    this.highlightLength = value;
                    Invalidate();
                }
            }
        }

        private bool highlightEnabled = false;
        public bool HighlightEnabled {
            get {
                return this.highlightEnabled;
            }

            set {
                if (this.highlightEnabled != value) {
                    this.highlightEnabled = value;
                    Invalidate();
                }
            }
        }

        public Ruler() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            
        }

        protected override void OnPaint(PaintEventArgs e) {
            float valueStart = this.scaleFactor.ScaleScalar(this.rulerValue - offset);
            float valueEnd = this.scaleFactor.ScaleScalar(this.rulerValue + 1.0f - offset);
            float highlightStartPx = this.scaleFactor.ScaleScalar(this.highlightStart - offset);
            float highlightEndPx = this.scaleFactor.ScaleScalar(this.highlightStart + this.highlightLength - offset);

            RectangleF highlightRect;
            RectangleF valueRect;

            if (this.orientation == Orientation.Horizontal) {
                valueRect = new RectangleF(valueStart, this.ClientRectangle.Top, valueEnd - valueStart, this.ClientRectangle.Height);
                highlightRect = new RectangleF(highlightStartPx, this.ClientRectangle.Top, highlightEndPx - highlightStartPx, this.ClientRectangle.Height);
            }
            else // if (this.orientation == Orientation.Vertical)
            {
                valueRect = new RectangleF(this.ClientRectangle.Left, valueStart, this.ClientRectangle.Width, valueEnd - valueStart);
                highlightRect = new RectangleF(this.ClientRectangle.Left, highlightStartPx, this.ClientRectangle.Width, highlightEndPx - highlightStartPx);
            }

            if (!this.highlightEnabled) {
                highlightRect = RectangleF.Empty;
            }
            if (UI.IsInitScales==false) {
                UI.InitScaling(this);
            }
            if (this.orientation == Orientation.Horizontal) {
                e.Graphics.DrawLine(
                    SystemPens.WindowText,
                    UISystemLayer.UI.ScaleWidth(15),
                    ClientRectangle.Top,
                    UISystemLayer.UI.ScaleWidth(15),
                    ClientRectangle.Bottom);

                string abbStringName = "MeasurementUnit." + this.MeasurementUnit.ToString() + ".Abbreviation";
                string abbString = abbStringName;//PdnResources.GetString(abbStringName);
                e.Graphics.DrawString(abbString, Font, SystemBrushes.WindowText, UISystemLayer.UI.ScaleWidth(-2), 0);
            }

            Region clipRegion = new Region(highlightRect);
            clipRegion.Xor(valueRect);

            if (this.orientation == Orientation.Horizontal) {
                clipRegion.Exclude(new Rectangle(0, 0, UISystemLayer.UI.ScaleWidth(16), ClientRectangle.Height));
            }

            e.Graphics.SetClip(clipRegion, CombineMode.Replace);
            DrawRuler(e, true);

            clipRegion.Xor(this.ClientRectangle);

            if (this.orientation == Orientation.Horizontal) {
                clipRegion.Exclude(new Rectangle(0, 0, UISystemLayer.UI.ScaleWidth(16), ClientRectangle.Height - 1));
            }

            e.Graphics.SetClip(clipRegion, CombineMode.Replace);
            DrawRuler(e, false);
        }

        private static readonly float[] majorDivisors =
            new float[]
            {
                2.0f,
                2.5f,
                2.0f
            };

        private int[] GetSubdivs(MeasurementUnit unit) {
            switch (unit) {
                case MeasurementUnit.Centimeter: {
                        return new int[] { 2, 5 };
                    }

                case MeasurementUnit.Inch: {
                        return new int[] { 2 };
                    }

                default: {
                        return null;
                    }
            }
        }

        void SubdivideX(
            Graphics g,
            Pen pen,
            float x,
            float delta,
            int index,
            float y,
            float height,
            int[] subdivs) {
            g.DrawLine(pen, x, y, x, y + height);

            if (index > 10) {
                return;
            }

            float div;

            if (subdivs != null && index >= 0) {
                div = subdivs[index % subdivs.Length];
            }
            else if (index < 0) {
                div = majorDivisors[(-index - 1) % majorDivisors.Length];
            }
            else {
                return;
            }

            for (int i = 0; i < div; i++) {
                if ((delta / div) > 3.5) {
                    SubdivideX(g, pen, x + delta * i / div, delta / div, index + 1, y, height / div + 0.5f, subdivs);
                }
            }
        }

        void SubdivideY(
            Graphics g,
            Pen pen,
            float y,
            float delta,
            int index,
            float x,
            float width,
            int[] subdivs) {
            g.DrawLine(pen, x, y, x + width, y);

            if (index > 10) {
                return;
            }

            float div;

            if (subdivs != null && index >= 0) {
                div = subdivs[index % subdivs.Length];
            }
            else if (index < 0) {
                div = majorDivisors[(-index - 1) % majorDivisors.Length];
            }
            else {
                return;
            }

            for (int i = 0; i < div; i++) {
                if ((delta / div) > 3.5) {
                    SubdivideY(g, pen, y + delta * i / div, delta / div, index + 1, x, width / div + 0.5f, subdivs);
                }
            }
        }

        private void DrawRuler(PaintEventArgs e, bool highlighted) {
            Pen pen;
            Brush cursorBrush;
            Brush textBrush;
            StringFormat textFormat = new StringFormat();
            int maxPixel;
            Color cursorColor;

            if (highlighted) {
                e.Graphics.Clear(SystemColors.Highlight);
                pen = SystemPens.HighlightText;
                textBrush = SystemBrushes.HighlightText;
                cursorColor = SystemColors.Window;
            }
            else {
                e.Graphics.Clear(SystemColors.Window);
                pen = SystemPens.WindowText;
                textBrush = SystemBrushes.WindowText;
                cursorColor = SystemColors.Highlight;
            }

            cursorColor = Color.FromArgb(128, cursorColor);
            cursorBrush = new SolidBrush(cursorColor);

            if (orientation == Orientation.Horizontal) {
                maxPixel = ScaleFactor.UnscaleScalar(ClientRectangle.Width);
                textFormat.Alignment = StringAlignment.Near;
                textFormat.LineAlignment = StringAlignment.Far;
            }
            else // if (orientation == Orientation.Vertical)
            {
                maxPixel = ScaleFactor.UnscaleScalar(ClientRectangle.Height);
                textFormat.Alignment = StringAlignment.Near;
                textFormat.LineAlignment = StringAlignment.Near;
                textFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
            }

            float majorSkip = 1;
            int majorSkipPower = 0;
            float majorDivisionLength = (float)dpu;
            float majorDivisionPixels = (float)ScaleFactor.ScaleScalar(majorDivisionLength);
            int[] subdivs = GetSubdivs(measurementUnit);
            float offsetPixels = ScaleFactor.ScaleScalar((float)offset);
            int startMajor = (int)(offset / majorDivisionLength) - 1;
            int endMajor = (int)((offset + maxPixel) / majorDivisionLength) + 1;

            if (orientation == Orientation.Horizontal) {
                // draw Value
                if (!highlighted) {
                    PointF pt = scaleFactor.ScalePointJustX(new PointF(ClientRectangle.Left + Value - Offset, ClientRectangle.Top));
                    SizeF size = new SizeF(Math.Max(1, scaleFactor.ScaleScalar(1.0f)), ClientRectangle.Height);

                    pt.X -= 0.5f;

                    CompositingMode oldCM = e.Graphics.CompositingMode;
                    e.Graphics.CompositingMode = CompositingMode.SourceOver;
                    e.Graphics.FillRectangle(cursorBrush, new RectangleF(pt, size));
                    e.Graphics.CompositingMode = oldCM;
                }

                // draw border
                e.Graphics.DrawLine(SystemPens.WindowText, new Point(ClientRectangle.Left, ClientRectangle.Bottom - 1),
                    new Point(ClientRectangle.Right - 1, ClientRectangle.Bottom - 1));
            }
            else if (orientation == Orientation.Vertical) {
                // draw Value
                if (!highlighted) {
                    PointF pt = scaleFactor.ScalePointJustY(new PointF(ClientRectangle.Left, ClientRectangle.Top + Value - Offset));
                    SizeF size = new SizeF(ClientRectangle.Width, Math.Max(1, scaleFactor.ScaleScalar(1.0f)));

                    pt.Y -= 0.5f;

                    CompositingMode oldCM = e.Graphics.CompositingMode;
                    e.Graphics.CompositingMode = CompositingMode.SourceOver;
                    e.Graphics.FillRectangle(cursorBrush, new RectangleF(pt, size));
                    e.Graphics.CompositingMode = oldCM;
                }

                // draw border
                e.Graphics.DrawLine(SystemPens.WindowText, new Point(ClientRectangle.Right - 1, ClientRectangle.Top),
                    new Point(ClientRectangle.Right - 1, ClientRectangle.Bottom - 1));
            }

            while (majorDivisionPixels * majorSkip < 60) {
                majorSkip *= majorDivisors[majorSkipPower % majorDivisors.Length];
                ++majorSkipPower;
            }

            startMajor = (int)(majorSkip * Math.Floor(startMajor / (double)majorSkip));

            for (int major = startMajor; major <= endMajor; major += (int)majorSkip) {
                float majorMarkPos = (major * majorDivisionPixels) - offsetPixels;
                string majorText = (major).ToString();

                if (orientation == Orientation.Horizontal) {
                    SubdivideX(e.Graphics, pen, ClientRectangle.Left + majorMarkPos, majorDivisionPixels * majorSkip, -majorSkipPower, ClientRectangle.Top, ClientRectangle.Height, subdivs);
                    e.Graphics.DrawString(majorText, Font, textBrush, new PointF(ClientRectangle.Left + majorMarkPos, ClientRectangle.Bottom), textFormat);
                }
                else // if (orientation == Orientation.Vertical)
                {
                    SubdivideY(e.Graphics, pen, ClientRectangle.Top + majorMarkPos, majorDivisionPixels * majorSkip, -majorSkipPower, ClientRectangle.Left, ClientRectangle.Width, subdivs);
                    e.Graphics.DrawString(majorText, Font, textBrush, new PointF(ClientRectangle.Left, ClientRectangle.Top + majorMarkPos), textFormat);
                }
            }

            textFormat.Dispose();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                    components = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
