using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Diagnostics;

namespace RS.UtilityLib.WinFormCommon.UI.ScrollPanel
{


    [Designer(typeof(ScrollbarControlDesigner))]
    public class CustomScrollbar : UserControl
    {
        public CustomScrollbar() {

            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            moChannelColor = Color.FromArgb(51, 166, 3);
            //UpArrowImage = Resource.uparrow;
            //DownArrowImage = Resource.downarrow;


            //ThumbBottomImage = Resource.ThumbBottom;
            //ThumbBottomSpanImage = Resource.ThumbSpanBottom;
            //ThumbTopImage = Resource.ThumbTop;
            //ThumbTopSpanImage = Resource.ThumbSpanTop;

            //ThumbMiddleImage = Resource.ThumbMiddle;
            InitRect_V();
            //this.m_ScoreImg = global::RS.UtilityLib.WinFormCommon.Properties.Resources.tempScrollBar;
            try {
                Image img = global::RS.UtilityLib.WinFormCommon.Properties.Resources.PanelScrollBar;
                if (img != null) {
                    this.m_ScoreImg = img;
                }
            }
            catch {
            }
            this.Width = UpArrowImage.Width;
            base.MinimumSize = new Size(UpArrowImage.Width, UpArrowImage.Height + DownArrowImage.Height + GetThumbHeight());
            //

        }

        private void InitRect_V() {
            UpArrowImage = new Rectangle(0 * 19, 0, 16, 16);
            DownArrowImage = new Rectangle(1 * 19, 0, 16, 16);

            ThumbTopImage = new Rectangle(2 * 19, 0, 16, 16);
            ThumbBottomImage = new Rectangle(5 * 19, 0, 16, 16);

            ThumbTopSpanImage = new Rectangle(3 * 19, 0, 16, 16);
            ThumbBottomSpanImage = new Rectangle(3 * 19, 0, 16, 16);

            ThumbMiddleImage = new Rectangle(3 * 19, 0, 16, 16);

            moThumbBackImage = new Rectangle(4 * 19, 0, 16, 16);
        }

        protected Color moChannelColor = Color.Empty;

        protected Rectangle moUpArrowImage;
        //protected Rectangle moUpArrowImage_Over = null;
        //protected Rectangle moUpArrowImage_Down = null;
        protected Rectangle moDownArrowImage;
        //protected Rectangle moDownArrowImage_Over = null;
        //protected Rectangle moDownArrowImage_Down = null;
        protected Rectangle moThumbArrowImage;

        protected Rectangle moThumbTopImage;
        protected Rectangle moThumbTopSpanImage;
        protected Rectangle moThumbBottomImage;
        protected Rectangle moThumbBottomSpanImage;
        protected Rectangle moThumbMiddleImage;

        protected Rectangle moThumbBackImage;

        protected Image m_ScoreImg = null;

        protected int moLargeChange = 10;
        protected int moSmallChange = 1;
        protected int moMinimum = 0;
        protected int moMaximum = 100;
        protected int moValue = 0;
        private int nClickPoint;

        protected int moThumbTop = 0;

        protected bool moAutoSize = false;

        private bool moThumbDown = false;
        private bool moThumbDragging = false;

        public new event EventHandler Scroll = null;
        public event EventHandler ValueChanged = null;

        private int GetThumbHeight() {
            int nTrackHeight = (this.Height - (UpArrowImage.Height + DownArrowImage.Height));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight) {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }
            if (nThumbHeight < 56) {
                nThumbHeight = 56;
                fThumbHeight = 56;
            }

            return nThumbHeight;
        }



        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("LargeChange")]
        public int LargeChange {
            get { return moLargeChange; }
            set {
                moLargeChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("SmallChange")]
        public int SmallChange {
            get { return moSmallChange; }
            set {
                moSmallChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Minimum")]
        public int Minimum {
            get { return moMinimum; }
            set {
                moMinimum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Maximum")]
        public int Maximum {
            get { return moMaximum; }
            set {
                moMaximum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Value")]
        public int Value {
            get { return moValue; }
            set {
                moValue = value;

                int nTrackHeight = (this.Height - (UpArrowImage.Height + DownArrowImage.Height));
                float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
                int nThumbHeight = (int)fThumbHeight;

                if (nThumbHeight > nTrackHeight) {
                    nThumbHeight = nTrackHeight;
                    fThumbHeight = nTrackHeight;
                }
                if (nThumbHeight < 56) {
                    nThumbHeight = 56;
                    fThumbHeight = 56;
                }

                //figure out value
                int nPixelRange = nTrackHeight - nThumbHeight;
                int nRealRange = (Maximum - Minimum) - LargeChange;
                float fPerc = 0.0f;
                if (nRealRange != 0) {
                    fPerc = (float)moValue / (float)nRealRange;

                }

                float fTop = fPerc * nPixelRange;
                moThumbTop = (int)fTop;


                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Channel Color")]
        public Color ChannelColor {
            get { return moChannelColor; }
            set { moChannelColor = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle UpArrowImage {
            get { return moUpArrowImage; }
            set { moUpArrowImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle DownArrowImage {
            get { return moDownArrowImage; }
            set { moDownArrowImage = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbBackImage {
            get { return moThumbBackImage; }
            set { moThumbBackImage = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbTopImage {
            get { return moThumbTopImage; }
            set { moThumbTopImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbTopSpanImage {
            get { return moThumbTopSpanImage; }
            set { moThumbTopSpanImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbBottomImage {
            get { return moThumbBottomImage; }
            set { moThumbBottomImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbBottomSpanImage {
            get { return moThumbBottomSpanImage; }
            set { moThumbBottomSpanImage = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        public Rectangle ThumbMiddleImage {
            get { return moThumbMiddleImage; }
            set { moThumbMiddleImage = value; }
        }
        public int GetScorllLength() {
            int topDown = (UpArrowImage.Height + DownArrowImage.Height);
            int nTrackHeight = (this.Height - topDown);
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;
            return (int)(nThumbHeight + topDown);
        }
        protected override void OnPaint(PaintEventArgs e) {
            //base.OnPaint(e);
            if (this.m_ScoreImg == null) {
                return;
            }
            if (!this.Visible) {
                //base.OnPaint(e);
                return;
            }
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //绘制背景
            //Brush oBrush = new SolidBrush(moChannelColor);
            //System.Drawing.TextureBrush tb = new TextureBrush(m_ScoreImg, System.Drawing.Drawing2D.WrapMode.Tile, moThumbBackImage);
            //e.Graphics.FillRectangle(tb, this.ClientRectangle);
            //oBrush.Dispose();
            //画背景图
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(0, 0, this.Width, this.Height + 24 * 2), moThumbBackImage, GraphicsUnit.Pixel);
            //画上荐头
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(new Point(0, 0), new Size(this.Width, UpArrowImage.Height)), UpArrowImage, GraphicsUnit.Pixel);

            //Brush oBrush = new SolidBrush(moChannelColor);
            //Brush oWhiteBrush = new SolidBrush(Color.FromArgb(255,255,255));

            ////draw channel left and right border colors
            //e.Graphics.FillRectangle(oWhiteBrush, new Rectangle(0,UpArrowImage.Height, 1, (this.Height-DownArrowImage.Height)));
            //e.Graphics.FillRectangle(oWhiteBrush, new Rectangle(this.Width-1, UpArrowImage.Height, 1, (this.Height - DownArrowImage.Height)));

            ////draw channel
            //e.Graphics.FillRectangle(oBrush, new Rectangle(1, UpArrowImage.Height, this.Width-2, (this.Height-DownArrowImage.Height)));

            //draw thumb
            int nTrackHeight = (this.Height - (UpArrowImage.Height + DownArrowImage.Height));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight) {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }
            if (nThumbHeight < 56) {
                nThumbHeight = 56;
                fThumbHeight = 56;
            }

            //Debug.WriteLine(nThumbHeight.ToString());

            float fSpanHeight = (fThumbHeight - (ThumbMiddleImage.Height + ThumbTopImage.Height + ThumbBottomImage.Height)) / 2.0f;
            int nSpanHeight = (int)fSpanHeight;

            int nTop = moThumbTop;
            nTop += UpArrowImage.Height;

            //draw top
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(0, nTop, this.Width, ThumbTopImage.Height), ThumbTopImage, GraphicsUnit.Pixel);
            /*
            nTop += ThumbTopImage.Height;
            //draw top span
            Rectangle rect = new Rectangle(1, nTop, this.Width - 2, nSpanHeight);


            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(1, nTop, this.Width - 2, (int)(fSpanHeight * 2)), ThumbTopSpanImage, GraphicsUnit.Pixel);

            nTop += nSpanHeight;
            //draw middle
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(1, nTop, this.Width - 2, ThumbMiddleImage.Height), ThumbMiddleImage, GraphicsUnit.Pixel);


            nTop += ThumbMiddleImage.Height;
            //draw top span
            rect = new Rectangle(1, nTop, this.Width - 2, nSpanHeight*2);
            e.Graphics.DrawImage(m_ScoreImg, rect, ThumbBottomSpanImage, GraphicsUnit.Pixel);

            nTop += nSpanHeight;
            */
            //绘制中间
            int middleheight = nSpanHeight * 2 + moThumbTopImage.Height + ThumbMiddleImage.Height;
            TextureBrush tbm = new TextureBrush(m_ScoreImg, System.Drawing.Drawing2D.WrapMode.Tile, moThumbMiddleImage);
            e.Graphics.FillRectangle(tbm, new Rectangle(0, nTop + moThumbTopImage.Height, this.Width, middleheight));
            tbm.Dispose();
            //绘制下面 
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(0, nTop + middleheight, this.Width, moThumbBottomImage.Height), ThumbBottomImage, GraphicsUnit.Pixel);
            //下箭头draw bottom
            e.Graphics.DrawImage(m_ScoreImg, new Rectangle(new Point(0, (this.Height - DownArrowImage.Height)), new Size(this.Width, DownArrowImage.Height)), DownArrowImage, GraphicsUnit.Pixel);


        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            //base.OnPaintBackground(e);
        }

        public override bool AutoSize {
            get {
                return base.AutoSize;
            }
            set {
                base.AutoSize = value;
                if (base.AutoSize) {
                    this.Width = moUpArrowImage.Width;
                }
            }
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // CustomScrollbar
            // 
            this.Name = "CustomScrollbar";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomScrollbar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CustomScrollbar_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomScrollbar_MouseUp);
            this.ResumeLayout(false);

        }

        private void CustomScrollbar_MouseDown(object sender, MouseEventArgs e) {
            Point ptPoint = this.PointToClient(Cursor.Position);
            int nTrackHeight = (this.Height - (UpArrowImage.Height + DownArrowImage.Height));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight) {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }
            if (nThumbHeight < 56) {
                nThumbHeight = 56;
                fThumbHeight = 56;
            }

            int nTop = moThumbTop;
            nTop += UpArrowImage.Height;


            Rectangle thumbrect = new Rectangle(new Point(1, nTop), new Size(ThumbMiddleImage.Width, nThumbHeight));
            if (thumbrect.Contains(ptPoint)) {

                //hit the thumb
                nClickPoint = (ptPoint.Y - nTop);
                //MessageBox.Show(Convert.ToString((ptPoint.Y - nTop)));
                this.moThumbDown = true;
            }

            Rectangle uparrowrect = new Rectangle(new Point(1, 0), new Size(UpArrowImage.Width, UpArrowImage.Height));
            if (uparrowrect.Contains(ptPoint)) {

                int nRealRange = (Maximum - Minimum) - LargeChange;
                int nPixelRange = (nTrackHeight - nThumbHeight);
                if (nRealRange > 0) {
                    if (nPixelRange > 0) {
                        if ((moThumbTop - SmallChange) < 0)
                            moThumbTop = 0;
                        else
                            moThumbTop -= SmallChange;

                        //figure out value
                        float fPerc = (float)moThumbTop / (float)nPixelRange;
                        float fValue = fPerc * (Maximum - LargeChange);

                        moValue = (int)fValue;
                        Debug.WriteLine(moValue.ToString());

                        if (ValueChanged != null)
                            ValueChanged(this, new EventArgs());

                        if (Scroll != null)
                            Scroll(this, new EventArgs());

                        Invalidate();
                    }
                }
            }

            Rectangle downarrowrect = new Rectangle(new Point(1, UpArrowImage.Height + nTrackHeight), new Size(UpArrowImage.Width, UpArrowImage.Height));
            if (downarrowrect.Contains(ptPoint)) {
                int nRealRange = (Maximum - Minimum) - LargeChange;
                int nPixelRange = (nTrackHeight - nThumbHeight);
                if (nRealRange > 0) {
                    if (nPixelRange > 0) {
                        if ((moThumbTop + SmallChange) > nPixelRange)
                            moThumbTop = nPixelRange;
                        else
                            moThumbTop += SmallChange;

                        //figure out value
                        float fPerc = (float)moThumbTop / (float)nPixelRange;
                        float fValue = fPerc * (Maximum - LargeChange);

                        moValue = (int)fValue;
                        Debug.WriteLine(moValue.ToString());

                        if (ValueChanged != null)
                            ValueChanged(this, new EventArgs());

                        if (Scroll != null)
                            Scroll(this, new EventArgs());

                        Invalidate();
                    }
                }
            }
        }

        private void CustomScrollbar_MouseUp(object sender, MouseEventArgs e) {
            this.moThumbDown = false;
            this.moThumbDragging = false;
        }

        private void MoveThumb(int y) {
            int nRealRange = Maximum - Minimum;
            int nTrackHeight = (this.Height - (UpArrowImage.Height + DownArrowImage.Height));
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            int nThumbHeight = (int)fThumbHeight;

            if (nThumbHeight > nTrackHeight) {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }
            if (nThumbHeight < 56) {
                nThumbHeight = 56;
                fThumbHeight = 56;
            }

            int nSpot = nClickPoint;

            int nPixelRange = (nTrackHeight - nThumbHeight);
            if (moThumbDown && nRealRange > 0) {
                if (nPixelRange > 0) {
                    int nNewThumbTop = y - (UpArrowImage.Height + nSpot);

                    if (nNewThumbTop < 0) {
                        moThumbTop = nNewThumbTop = 0;
                    }
                    else if (nNewThumbTop > nPixelRange) {
                        moThumbTop = nNewThumbTop = nPixelRange;
                    }
                    else {
                        moThumbTop = y - (UpArrowImage.Height + nSpot);
                    }

                    //figure out value
                    float fPerc = (float)moThumbTop / (float)nPixelRange;
                    float fValue = fPerc * (Maximum - LargeChange);
                    moValue = (int)fValue;
                    //Debug.WriteLine(moValue.ToString());

                    //Application.DoEvents();

                    Invalidate();
                }
            }
        }

        private void CustomScrollbar_MouseMove(object sender, MouseEventArgs e) {
            if (moThumbDown == true) {
                this.moThumbDragging = true;
            }

            if (this.moThumbDragging) {

                MoveThumb(e.Y);
            }

            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());

            if (Scroll != null)
                Scroll(this, new EventArgs());
        }
        protected override void OnGotFocus(EventArgs e) {
            //base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e) {
            //base.OnLostFocus(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            //base.OnMouseWheel(e);
            //bool md= moThumbDown;
            //moThumbDown = true;
            //MoveThumb((int)(e.Delta / 120 * this.moSmallChange));
            //moThumbDown = md;
            //if (ValueChanged != null)
            //    ValueChanged(this, new EventArgs());
            //if (Scroll != null)
            //    Scroll(this, new EventArgs());
        }
        protected override void Dispose(bool disposing) {
            //if (m_ScoreImg != null) {
            //    m_ScoreImg.Dispose();
            //}
            base.Dispose(disposing);
        }
    }

    public class ScrollbarControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {



        public override SelectionRules SelectionRules {
            get {
                SelectionRules selectionRules = base.SelectionRules;
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Component)["AutoSize"];
                if (propDescriptor != null) {
                    bool autoSize = (bool)propDescriptor.GetValue(this.Component);
                    if (autoSize) {
                        selectionRules = SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.BottomSizeable | SelectionRules.TopSizeable;
                    }
                    else {
                        selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
                    }
                }
                return selectionRules;
            }
        }
    }
}
