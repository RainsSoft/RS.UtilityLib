/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    /// <summary>
    /// 颜色切换矩形面板控件
    /// </summary>
    [Description("颜色切换矩形面板控件")]
    public sealed class SwatchControl
        : Control
    {
        private bool IsDesignerHosted {
            get {
                if (DesignMode)
                    return DesignMode;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
        private List<ColorBgra> colors = new List<ColorBgra>();//{  ColorBgra.FromBgra(200,0,0,255), ColorBgra.FromBgra(0, 0, 0, 255) };
        private const int defaultUnscaledSwatchSize = 12;
        private int unscaledSwatchSize = defaultUnscaledSwatchSize;
        private bool mouseDown = false;
        private int mouseDownIndex = -1;
        private bool blinkHighlight = false;
        private const int blinkInterval = 500;
        private System.Windows.Forms.Timer blinkHighlightTimer;

        [Browsable(false)]
        public bool BlinkHighlight
        {
            get
            {
                return this.blinkHighlight;
            }

            set
            {
                this.blinkHighlight = value;
                this.blinkHighlightTimer.Enabled = value;
                Invalidate();
            }
        }

        public event EventHandler ColorsChanged;
        private void OnColorsChanged()
        {
            if (ColorsChanged != null)
            {
                ColorsChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]//,NonSerialized()]
        public ColorBgra[] Colors
        {
            get
            {
                return this.colors.ToArray();
            }

            set
            {
                this.colors = new List<ColorBgra>(value);
                this.mouseDown = false;
                Invalidate();
                OnColorsChanged();
            }
        }

        [DefaultValue(defaultUnscaledSwatchSize)]
        [Browsable(true)]
        public int UnscaledSwatchSize
        {
            get
            {
                return this.unscaledSwatchSize;
            }

            set
            {
                this.unscaledSwatchSize = value;
                this.mouseDown = false;
                Invalidate();
            }
        }

        public event PDNEvents.EventHandler<Pair<int, MouseButtons>> ColorClicked;
        private void OnColorClicked(int index, MouseButtons buttons)
        {
            if (ColorClicked != null)
            {
                ColorClicked(this, new PDNEvents.EventArgs<Pair<int, MouseButtons>>(new Pair<int, MouseButtons>(index, buttons)));
            }
        }

        public SwatchControl()
        {
            InitializeComponent();          
        }
        /// <summary>
        /// The required number of colors for a palette.
        /// </summary>
        /// <remarks>
        /// If a palette is loaded with fewer colors than this, then it will be padded with entries
        /// that are equal to DefaultColor. If a pelette is loaded with more colors than this, then
        /// the 61st through the last color will be discarded.
        /// </remarks>
        public const int PaletteColorCount = 96;
        public static ColorBgra[] DefaultPalette {
            get {
                return
                    new ColorBgra[PaletteColorCount]
                    {
                        ColorBgra.FromUInt32(0xff000000),
                        ColorBgra.FromUInt32(0xff404040),
                        ColorBgra.FromUInt32(0xffff0000),
                        ColorBgra.FromUInt32(0xffff6a00),
                        ColorBgra.FromUInt32(0xffffd800),
                        ColorBgra.FromUInt32(0xffb6ff00),
                        ColorBgra.FromUInt32(0xff4cff00),
                        ColorBgra.FromUInt32(0xff00ff21),
                        ColorBgra.FromUInt32(0xff00ff90),
                        ColorBgra.FromUInt32(0xff00ffff),
                        ColorBgra.FromUInt32(0xff0094ff),
                        ColorBgra.FromUInt32(0xff0026ff),
                        ColorBgra.FromUInt32(0xff4800ff),
                        ColorBgra.FromUInt32(0xffb200ff),
                        ColorBgra.FromUInt32(0xffff00dc),
                        ColorBgra.FromUInt32(0xffff006e),
                        ColorBgra.FromUInt32(0xffffffff),
                        ColorBgra.FromUInt32(0xff808080),
                        ColorBgra.FromUInt32(0xff7f0000),
                        ColorBgra.FromUInt32(0xff7f3300),
                        ColorBgra.FromUInt32(0xff7f6a00),
                        ColorBgra.FromUInt32(0xff5b7f00),
                        ColorBgra.FromUInt32(0xff267f00),
                        ColorBgra.FromUInt32(0xff007f0e),
                        ColorBgra.FromUInt32(0xff007f46),
                        ColorBgra.FromUInt32(0xff007f7f),
                        ColorBgra.FromUInt32(0xff004a7f),
                        ColorBgra.FromUInt32(0xff00137f),
                        ColorBgra.FromUInt32(0xff21007f),
                        ColorBgra.FromUInt32(0xff57007f),
                        ColorBgra.FromUInt32(0xff7f006e),
                        ColorBgra.FromUInt32(0xff7f0037),
                        ColorBgra.FromUInt32(0xffa0a0a0),
                        ColorBgra.FromUInt32(0xff303030),
                        ColorBgra.FromUInt32(0xffff7f7f),
                        ColorBgra.FromUInt32(0xffffb27f),
                        ColorBgra.FromUInt32(0xffffe97f),
                        ColorBgra.FromUInt32(0xffdaff7f),
                        ColorBgra.FromUInt32(0xffa5ff7f),
                        ColorBgra.FromUInt32(0xff7fff8e),
                        ColorBgra.FromUInt32(0xff7fffc5),
                        ColorBgra.FromUInt32(0xff7fffff),
                        ColorBgra.FromUInt32(0xff7fc9ff),
                        ColorBgra.FromUInt32(0xff7f92ff),
                        ColorBgra.FromUInt32(0xffa17fff),
                        ColorBgra.FromUInt32(0xffd67fff),
                        ColorBgra.FromUInt32(0xffff7fed),
                        ColorBgra.FromUInt32(0xffff7fb6),
                        ColorBgra.FromUInt32(0xffc0c0c0),
                        ColorBgra.FromUInt32(0xff606060),
                        ColorBgra.FromUInt32(0xff7f3f3f),
                        ColorBgra.FromUInt32(0xff7f593f),
                        ColorBgra.FromUInt32(0xff7f743f),
                        ColorBgra.FromUInt32(0xff6d7f3f),
                        ColorBgra.FromUInt32(0xff527f3f),
                        ColorBgra.FromUInt32(0xff3f7f47),
                        ColorBgra.FromUInt32(0xff3f7f62),
                        ColorBgra.FromUInt32(0xff3f7f7f),
                        ColorBgra.FromUInt32(0xff3f647f),
                        ColorBgra.FromUInt32(0xff3f497f),
                        ColorBgra.FromUInt32(0xff503f7f),
                        ColorBgra.FromUInt32(0xff6b3f7f),
                        ColorBgra.FromUInt32(0xff7f3f76),
                        ColorBgra.FromUInt32(0xff7f3f5b),
                        ColorBgra.FromUInt32(0x80000000),
                        ColorBgra.FromUInt32(0x80404040),
                        ColorBgra.FromUInt32(0x80ff0000),
                        ColorBgra.FromUInt32(0x80ff6a00),
                        ColorBgra.FromUInt32(0x80ffd800),
                        ColorBgra.FromUInt32(0x80b6ff00),
                        ColorBgra.FromUInt32(0x804cff00),
                        ColorBgra.FromUInt32(0x8000ff21),
                        ColorBgra.FromUInt32(0x8000ff90),
                        ColorBgra.FromUInt32(0x8000ffff),
                        ColorBgra.FromUInt32(0x800094ff),
                        ColorBgra.FromUInt32(0x800026ff),
                        ColorBgra.FromUInt32(0x804800ff),
                        ColorBgra.FromUInt32(0x80b200ff),
                        ColorBgra.FromUInt32(0x80ff00dc),
                        ColorBgra.FromUInt32(0x80ff006e),
                        ColorBgra.FromUInt32(0x80ffffff),
                        ColorBgra.FromUInt32(0x80808080),
                        ColorBgra.FromUInt32(0x807f0000),
                        ColorBgra.FromUInt32(0x807f3300),
                        ColorBgra.FromUInt32(0x807f6a00),
                        ColorBgra.FromUInt32(0x805b7f00),
                        ColorBgra.FromUInt32(0x80267f00),
                        ColorBgra.FromUInt32(0x80007f0e),
                        ColorBgra.FromUInt32(0x80007f46),
                        ColorBgra.FromUInt32(0x80007f7f),
                        ColorBgra.FromUInt32(0x80004a7f),
                        ColorBgra.FromUInt32(0x8000137f),
                        ColorBgra.FromUInt32(0x8021007f),
                        ColorBgra.FromUInt32(0x8057007f),
                        ColorBgra.FromUInt32(0x807f006e),
                        ColorBgra.FromUInt32(0x807f0037)
                    };
            }
        }
        private void InitializeComponent()
        {
            this.blinkHighlightTimer = new Timer();
            this.blinkHighlightTimer.Tick += new EventHandler(BlinkHighlightTimer_Tick);
            this.blinkHighlightTimer.Enabled = false;
            this.blinkHighlightTimer.Interval = blinkInterval;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        private void BlinkHighlightTimer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.blinkHighlightTimer != null)
                {
                    this.blinkHighlightTimer.Dispose();
                    this.blinkHighlightTimer = null;
                }
            }

            base.Dispose(disposing);
        }

        private int MouseXYToColorIndex(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ClientSize.Width || y >= ClientSize.Height)
            {
                return -1;
            }

            int scaledSwatchSize = UI.ScaleWidth(this.unscaledSwatchSize);
            int swatchColumns = this.ClientSize.Width / scaledSwatchSize;
            int row = y / scaledSwatchSize;
            int col = x / scaledSwatchSize;
            int index = col + (row * swatchColumns);

            // Make sure they aren't on the last item of a row that actually got clipped off
            if (col == swatchColumns)
            {
                index = -1;
            }

            return index;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseDown = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.mouseDown = true;
            this.mouseDownIndex = MouseXYToColorIndex(e.X, e.Y);
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            int colorIndex = MouseXYToColorIndex(e.X, e.Y);

            if (colorIndex == this.mouseDownIndex && 
                colorIndex >= 0 && 
                colorIndex < this.colors.Count)
            {
                OnColorClicked(colorIndex, e.Button);
            }

            this.mouseDown = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.IsDesignerHosted == false) {
                if (this.Colors == null || this.Colors.Length == 0) {
                    this.Colors = DefaultPalette;
                }
            }
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            int scaledSwatchSize = UI.ScaleWidth(this.unscaledSwatchSize);
            int swatchColumns = System.Math.Max(1,this.ClientSize.Width / scaledSwatchSize);

            Point mousePt = Control.MousePosition;
            mousePt = PointToClient(mousePt);
            int activeIndex = MouseXYToColorIndex(mousePt.X, mousePt.Y);

            for (int i = 0; i < this.colors.Count; ++i)
            {
                ColorBgra c = this.colors[i];

                int swatchX = i % swatchColumns;
                int swatchY = i / swatchColumns;

                Rectangle swatchRect = new Rectangle(
                    swatchX * scaledSwatchSize, 
                    swatchY * scaledSwatchSize, 
                    scaledSwatchSize, 
                    scaledSwatchSize);

                UI.ButtonState state;

                if (this.mouseDown)
                {
                    if (i == this.mouseDownIndex)
                    {
                        state = UI.ButtonState.Pressed;
                    }
                    else
                    {
                        state = UI.ButtonState.Normal;
                    }
                }
                else if (i == activeIndex)
                {
                    state = UI.ButtonState.Hot;
                }
                else
                {
                    state = UI.ButtonState.Normal;
                }

                bool drawOutline;

                switch (state)
                {
                    case UI.ButtonState.Hot:
                        drawOutline = true;
                        break;

                    case UI.ButtonState.Pressed:
                        drawOutline = false;
                        break;

                    case UI.ButtonState.Disabled:
                    case UI.ButtonState.Normal:
                        drawOutline = false;
                        break;

                    default:
                        throw new InvalidEnumArgumentException();
                }

                Utility.DrawColorRectangle(e.Graphics, swatchRect, c.ToColor(), drawOutline);
            }

            if (this.blinkHighlight)
            {
                int period = (Math.Abs(Environment.TickCount) / blinkInterval) % 2;
                Color color;
                
                switch (period)
                {
                    case 0:
                        color = SystemColors.Window;
                        break;

                    case 1:
                        color = SystemColors.Highlight;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                using (Pen pen = new Pen(color))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }

            base.OnPaint(e);
        }
    }
}
