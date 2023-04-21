using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public delegate void RibbonScrollHandle(RibbonScrollbar sender, ScrollEventArgs e);
    internal enum ScrollBarState
    {
        MouseIn = 0,
        Dragging = 1,
        UpDown = 2,
        BottomDown = 3,
        LargeDown = 4,
    }
    /// <summary>
    /// 这里需要添加一些方法 主要是 至少包含了4张图片
    /// </summary>
    public class RibbonScrollbar : UserControl, RibbonUISkin
    {

        #region
        private void InitRect_V() {
            moUpArrowImage = new Rectangle(0 * 19, 0, 16, 16);
            moDownArrowImage = new Rectangle(1 * 19, 0, 16, 16);

            moThumbTopImage = new Rectangle(2 * 19, 0, 16, 16);
            moThumbBottomImage = new Rectangle(4 * 19, 0, 16, 16);

            moThumbTopSpanImage = new Rectangle(3 * 19, 0, 16, 16);
            moThumbBottomSpanImage = new Rectangle(3 * 19, 0, 16, 16);

            moThumbMiddleImage = new Rectangle(3 * 19, 0, 16, 16);

            moThumbBackImage = new Rectangle(5 * 19, 0, 16, 16);
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



        private Image m_ScrollImage = null;
        protected int m_LargeChange = 10;
        protected int m_SmallChange = 1;
        internal int m_InternalLargeChange;
        internal int m_InternalSmallChange;
        protected int m_Minimum = 0;
        protected int m_Maximum = 100;
        protected int m_Value = 0;

        public ScrollEventHandler Scroll = null;
        private System.Windows.Forms.Timer m_timer = new Timer();

        private MouseState[] m_MouseState = new MouseState[3]{
            MouseState.Out,//Up
            MouseState.Out,//Down
            MouseState.Out //MiddleBlock
        };

        public RibbonScrollbar() {

            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            Orientation = ScrollOrientation.VerticalScroll;//默认垂直


            if (Orientation == ScrollOrientation.VerticalScroll) {
                UpRect = new Rectangle(0, 0, 16, 16);
                BottomRect = new Rectangle(0, 176, 16, 16);
                MiddleRect = new Rectangle(0, 16, 16, 160);
                MiddleBlockRect = new Rectangle(0, 16, 16, 100);
                //
                InitRect_V();
            }
            // m_InternalLargeChange = GetNMap(m_LargeChange);
            //  m_InternalSmallChange = GetNMap(m_SmallChange);
            m_InternalLargeChange = m_LargeChange;
            m_InternalSmallChange = m_SmallChange;

            this.Width = 16;
            this.MinimumSize = new Size(16, 16);

            //this.m_UpArrowImage = UI_Res.up1;
            //this.m_UpArrowDownImage = UI_Res.up2;
            this.m_ScrollImage = global::RS.UtilityLib.WinFormCommon.Properties.Resources.滚动条;
            m_timer.Interval = 100;
            m_timer.Tick += this.m_timer_Tick;

        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.BindControl = null;
                m_timer.Dispose();
            }
            base.Dispose(disposing);
        }
        private MouseEventArgs m_MouseEventArgs;
        void m_timer_Tick(object sender, EventArgs e) {
            if (m_MouseEventArgs != null) {
                OnMouseDown(m_MouseEventArgs);
            }
        }

        protected override void OnSizeChanged(EventArgs e) {

            ResetSize();

            base.OnSizeChanged(e);
        }
        //依据新的this尺寸和Maximum重新计算各个Rect
        private void ResetSize() {

            if (Orientation == ScrollOrientation.VerticalScroll) {
                //上下箭头和中间空白
                UpRect = new Rectangle(0, 0, 16, 16);
                BottomRect = new Rectangle(0, this.Height - UpRect.Height, 16, 16);
                MiddleRect = new Rectangle(0, UpRect.Height, 16, this.Height - 2 * UpRect.Height);
                if (MiddleRect.Height <= 0) {
                    MiddleRect.Height = 1;
                }

                //滚动滑块
                if (MiddleBlockRect.Y > UpRect.Height) {
                    MiddleBlockRect = new Rectangle(0, MiddleBlockRect.Y, 16, 100);
                }
                else {
                    MiddleBlockRect = new Rectangle(0, UpRect.Height, 16, 100);
                }
                //滑块高度=((控件高度-两头)/Max)*LargeChange,最小是8
                //int h = (int)(MiddleRect.Height * MiddleRect.Height / ((float)(m_Maximum - m_Minimum) * m_PixelLogic));
                int h = (int)((MiddleRect.Height) / (float)(m_Maximum - m_Minimum) * m_LargeChange);
                //int h = (int)((m_LargeChange / (float)Maximum) * MiddleRect.Height);
                MiddleBlockRect.Height = h < 8 ? 8 : h;
                if (MiddleBlockRect.Height >= MiddleRect.Height) {
                    MiddleBlockRect.Height = MiddleRect.Height - m_InternalSmallChange;
                }

                BottomRect.Y = UpRect.Height + MiddleRect.Height;
            }
            else {
                UpRect = new Rectangle(0, 0, 16, 16);
                BottomRect = new Rectangle(this.Width - UpRect.Width, 0, 16, 16);
                MiddleRect = new Rectangle(UpRect.Right, 0, BottomRect.Left, 16);
                MiddleBlockRect = new Rectangle(UpRect.Right, 0, 100, 16);

                MiddleRect.Width = this.Width - 2 * UpRect.Width;

                int w = (int)(MiddleRect.Width * MiddleRect.Width / (float)((m_Maximum - m_Minimum) * m_PixelLogic));
                //int h = (int)((m_LargeChange / (float)Maximum) * MiddleRect.Height);
                MiddleBlockRect.Width = w < 8 ? 8 : w;
                if (MiddleBlockRect.Width >= MiddleRect.Width) {
                    MiddleBlockRect.Width = MiddleRect.Width - m_InternalSmallChange;
                }

                BottomRect.X = UpRect.Width + MiddleRect.Width;
            }

        }
        private void ResetMax() {

            if (Orientation == ScrollOrientation.VerticalScroll) {

                //int h = (int)(MiddleRect.Height * MiddleRect.Height / ((float)(m_Maximum - m_Minimum) * m_PixelLogic + m_Maximum / MiddleRect.Height));
                int h = (int)((MiddleRect.Height) / (float)(m_Maximum - m_Minimum) * m_LargeChange);
                MiddleBlockRect.Height = h < 8 ? 8 : h;
                if (MiddleBlockRect.Height >= MiddleRect.Height) {
                    MiddleBlockRect.Height = MiddleRect.Height - m_InternalSmallChange;
                }
            }
            else {


                int w = (int)(MiddleRect.Width * MiddleRect.Width / (float)((m_Maximum - m_Minimum) * m_PixelLogic));
                //int h = (int)((m_LargeChange / (float)Maximum) * MiddleRect.Height);
                MiddleBlockRect.Width = w < 8 ? 8 : w;
                if (MiddleBlockRect.Width >= MiddleRect.Width) {
                    MiddleBlockRect.Width = MiddleRect.Width - m_InternalSmallChange;
                }
            }
            ResetSize();
        }
        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("LargeChange")]
        public int LargeChange {
            get { return m_LargeChange; }
            set {
                m_LargeChange = value;
                //if (m_LargeChange > m_Maximum + 1) {
                //    m_LargeChange = m_Maximum + 1;
                //}
                m_InternalLargeChange = m_LargeChange;// GetNMap(value);
                //Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("SmallChange")]
        public int SmallChange {
            get { return m_SmallChange; }
            set {
                m_SmallChange = value;
                m_InternalSmallChange = m_SmallChange;// GetNMap(value);
                //Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Minimum")]
        public int Minimum {
            get { return m_Minimum; }
            set {
                m_Minimum = value;
                ResetSize();
                Invalidate();
            }
        }

        /// <summary>
        /// 滚动条最大值.该值通常是外部需要滚动的内容的实际高度减去呈现高度.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Maximum")]
        public int Maximum {
            get { return m_Maximum; }
            set {
                m_Maximum = value;
                //if (m_LargeChange > m_Maximum + 1) {
                //    this.LargeChange = m_Maximum + 1;
                //}
                if (value == 0) {
                    this.Visible = false;
                }
                else {
                    this.Visible = true;
                    ResetMax();
                }
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Value")]
        public int Value {
            get { return m_Value; }
            set {
                m_Value = value;
                if (value == 0) {
                    MiddleBlockRect.Y = 0;
                }
                //暂时不支持设置值.
                //float percent = m_Value / (float)(m_Maximum - m_Minimum);
                //if (Orientation == ScrollOrientation.VerticalScroll) {
                //    MiddleBlockRect.Y = (int)(MiddleRect.Height * percent);
                //}
                //Invalidate();
            }
        }
        /*
        public void ScrollTo(int value) {
            if (this.Orientation == ScrollOrientation.VerticalScroll) {
                #region V
                if (value >= m_Maximum) {
                    return;
                }


                Move(ScrollEventType.ThumbPosition, Math.Sign(d), Math.Abs(d));

                MiddleBlockRect.Y = e.Y - m_MiddleDownPoint.Y;
                //int dy = e.Y - m_MiddleDownPoint.Y;
                //MiddleBlockRect.Y += dy;
                if (MiddleBlockRect.Y > MiddleRect.Bottom - MiddleBlockRect.Height) {
                    MiddleBlockRect.Y = MiddleRect.Bottom - MiddleBlockRect.Height;
                }
                else {
                    if (MiddleBlockRect.Y < UpRect.Bottom) {
                        MiddleBlockRect.Y = UpRect.Bottom;
                    }
                }
                #endregion
            }
            else {
                #region H
                MiddleBlockRect.X = e.X - m_MiddleDownPoint.X;
                if (MiddleBlockRect.X < UpRect.Right) {
                    MiddleBlockRect.X = UpRect.Right;
                }
                int v = GetMouseAtValue(e.X - UpRect.Right, MiddleBlockRect.Y);// e.Y-MiddleBlockRect.Height);
                int d = v - m_Value;
                System.Diagnostics.Debug.WriteLine(v);
                if (d == 0) {
                    return;
                }


                Move(ScrollEventType.ThumbPosition, Math.Sign(d), Math.Abs(d));

                MiddleBlockRect.X = e.X - m_MiddleDownPoint.X;
                //int dy = e.Y - m_MiddleDownPoint.Y;
                //MiddleBlockRect.Y += dy;
                if (MiddleBlockRect.X > MiddleRect.Right - MiddleBlockRect.Width) {
                    MiddleBlockRect.X = MiddleRect.Right - MiddleBlockRect.Width;
                }
                else {
                    if (MiddleBlockRect.X < UpRect.Right) {
                        MiddleBlockRect.X = UpRect.Right;
                    }
                }
                #endregion
            }


            Invalidate();

            base.OnMouseMove(e);
        }
        */
        //[EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        //public Image UpArrowImage {
        //    get { return m_UpArrowImage; }
        //    set { m_UpArrowImage = value; }
        //}

        //[EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("Up Arrow Graphic")]
        //public Image DownArrowImage {
        //    get { return m_DownArrowImage; }
        //    set { m_DownArrowImage = value; }
        //}
        private int m_PixelLogic = 1;
        /// <summary>
        /// 像素和逻辑单位的比例系数
        /// </summary>
        public int PixelLogicFactor {
            get {
                return m_PixelLogic;
            }
            set {
                m_PixelLogic = value;
            }
        }
        private Control m_Ctl;
        /// <summary>
        /// 设置绑定的控件.绑定以后,控件的滚动条将由本滚动条代替.
        /// 如果需要绑定H V两个方向,则在绑定完毕以后调用静态方法UpdateHVScroll进行更新
        /// </summary>
        public Control BindControl {
            get {
                return m_Ctl;
            }
            set {
                if (value == null) {
                    if (m_Ctl == null) {
                        return;
                    }
                    m_Ctl.Layout -= new LayoutEventHandler(m_Ctl_Layout);
                    m_Ctl.SizeChanged -= new EventHandler(m_Ctl_SizeChanged);
                    m_Ctl.LocationChanged -= new EventHandler(m_Ctl_LocationChanged);
                    m_Ctl.MouseWheel -= new MouseEventHandler(m_Ctl_MouseWheel);
                }
                else {
                    m_Ctl = value;
                    m_Ctl.Layout += new LayoutEventHandler(m_Ctl_Layout);
                    m_Ctl.SizeChanged += new EventHandler(m_Ctl_SizeChanged);
                    m_Ctl.LocationChanged += new EventHandler(m_Ctl_LocationChanged);
                    m_Ctl.ParentChanged += new EventHandler(m_Ctl_ParentChanged);
                    m_Ctl.MouseWheel += new MouseEventHandler(m_Ctl_MouseWheel);

                    ScrollableControl scrollCtl = (m_Ctl as ScrollableControl);

                    if (scrollCtl != null) {
                        scrollCtl.AutoScroll = true;
                        scrollCtl.VerticalScroll.Visible = false;
                        scrollCtl.HorizontalScroll.Visible = false;
                    }
                    else {
                        //TODO:普通的控件另外处理
                    }
                    InitLayoutByBindCtl();
                    //这里不能把this加到m_Ctl.Controls里面去.一旦加进去,滚动条就成了控件的一部分,控件滚动时,滚动条也被滚动
                    //m_Ctl.Controls.Add(this); //不能加
                }
            }
        }

        void m_Ctl_MouseWheel(object sender, MouseEventArgs e) {
            int d = (int)Win32Native.MakeUInt32(0, (short)e.Delta);
            Win32Native.SendMessage(this.Handle, 522, d, 0);
        }

        /// <summary>
        /// 同一个控件上绑定H V两个滚动条后,调用此方法更新
        /// </summary>
        public static void UpdateHVScroll(RibbonScrollbar hbar, RibbonScrollbar vbar) {
            hbar.Width -= 16;
            vbar.Height -= 16;
            //TODO:要对两者的Max进行更新
        }

        private void InitLayoutByBindCtl() {
            if (m_Ctl == null) {
                return;
            }

            m_Ctl_Layout(m_Ctl, null);
            m_Ctl_SizeChanged(m_Ctl, null);
            m_Ctl_LocationChanged(m_Ctl, null);
            m_Ctl_ParentChanged(m_Ctl, null);
        }

        void m_Ctl_LocationChanged(object sender, EventArgs e) {
            Control bindCtl = sender as Control;
            Point p = bindCtl.Location;
            if (this.Orientation == ScrollOrientation.VerticalScroll) {
                this.Location = new Point(p.X + bindCtl.Width - this.Width, p.Y);
            }
            else {
                this.Location = new Point(p.X, p.Y + bindCtl.Height - this.Height);
            }
        }

        void m_Ctl_SizeChanged(object sender, EventArgs e) {
            Control bindCtl = sender as Control;

            if (this.Orientation == ScrollOrientation.VerticalScroll) {
                this.Height = bindCtl.Height;
            }
            else {
                this.Width = bindCtl.Width;
            }
        }

        void m_Ctl_Layout(object sender, LayoutEventArgs e) {
            if (this.Orientation == ScrollOrientation.VerticalScroll) {
                this.Maximum = m_Ctl.DisplayRectangle.Height;
                ScrollableControl scrollCtl = (m_Ctl as ScrollableControl);
                if (scrollCtl != null) {
                    scrollCtl.VerticalScroll.Minimum = this.Minimum;
                    scrollCtl.VerticalScroll.Maximum = this.Maximum;
                    scrollCtl.VerticalScroll.LargeChange = this.LargeChange;
                    scrollCtl.VerticalScroll.SmallChange = this.SmallChange;
                }
                else {
                    //if (scrollCtl is ListBox) {
                    //    this.Maximum = (m_Ctl as ListBox).Items.Count;
                    //}
                }
            }
            else {
                this.Maximum = m_Ctl.DisplayRectangle.Width;
                ScrollableControl scrollCtl = (m_Ctl as ScrollableControl);
                if (scrollCtl != null) {
                    scrollCtl.HorizontalScroll.Minimum = this.Minimum;
                    scrollCtl.HorizontalScroll.Maximum = this.Maximum;
                    scrollCtl.HorizontalScroll.LargeChange = this.LargeChange;
                    scrollCtl.HorizontalScroll.SmallChange = this.SmallChange;
                }
                else {
                    //if (scrollCtl is ListBox) {
                    //    this.Maximum = (m_Ctl as ListBox).Items.Count;
                    //}
                }
            }
        }
        void m_Ctl_ParentChanged(object sender, EventArgs e) {
            if (m_Ctl.Parent != null) {
                m_Ctl.Parent.Controls.Add(this);
            }
            else {
                if (this.Parent != null) {
                    this.Parent.Controls.Remove(this);
                    this.Parent = null;
                }
            }
            this.BringToFront();
        }

        private void OnScroll(ScrollEventArgs e) {
            if (this.m_Ctl != null) {
                ScrollableControl scr = m_Ctl as ScrollableControl;
                switch (e.Type) {
                    case ScrollEventType.LargeIncrement:
                        Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 3, 0);
                        break;
                    case ScrollEventType.LargeDecrement:
                        Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 2, 0);
                        break;
                    case ScrollEventType.SmallDecrement:
                        Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 0, 0);
                        break;
                    case ScrollEventType.SmallIncrement:
                        //e.NewValue 
                        Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 1, 0);
                        break;
                    case ScrollEventType.ThumbPosition:
                        //if (scr == null) {
                        //    int nBar = (m_Orientation == ScrollOrientation.VerticalScrol?1:0);

                        //    m_scrollInfo.fMask=16;
                        //    SetScrollInfo(m_Ctl.Handle, nBar, ref m_scrollInfo, true);
                        //    Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 4, 0);
                        //}
                        //int k = Win32Native.MakeUInt32(4, (short)e.NewValue);
                        //Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 4, 0);
                        break;
                    default:
                        break;
                }



                if (scr != null) {

                    if (e.Type != ScrollEventType.ThumbPosition) {
                        if (m_Orientation == ScrollOrientation.VerticalScroll) {
                            scr.VerticalScroll.Value = e.NewValue;
                        }
                        else {
                            scr.HorizontalScroll.Value = e.NewValue;
                            System.Diagnostics.Debug.WriteLine("H: " + e.NewValue);
                        }
                    }
                }
                else {
                    //普通控件
                    //ListBox l;

                }

            }

            if (this.Scroll != null) {
                Scroll(this, e);
            }

        }

        public const int SB_LINEDOWN = 1;
        public const int WM_VSCROLL = 277;
        public const int WM_HSCROLL = 276;

        protected void OnPaint_V(PaintEventArgs e) {
            //base.OnPaint(e);
            if (this.m_ScrollImage == null) {
                return;
            }
            if (!this.Visible) {
                base.OnPaint(e);
                return;
            }
            int i = 0;
            if (m_UpDown || m_MiddleDown || m_MiddleBlockDown || m_BottomDown) {
                //鼠标按下
                i = 2;
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                i = 1;
            }
            else {
                //正常情况
            }
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //绘制背景
            //Brush oBrush = new SolidBrush(moChannelColor);
            //System.Drawing.TextureBrush tb = new TextureBrush(m_ScoreImg, System.Drawing.Drawing2D.WrapMode.Tile, moThumbBackImage);
            //e.Graphics.FillRectangle(tb, this.ClientRectangle);
            //oBrush.Dispose();
            //画背景图
            e.Graphics.DrawImage(m_ScrollImage, new Rectangle(0, 0, this.Width, this.Height + 24 * 2), new Rectangle(moThumbBackImage.X, moThumbBackImage.Y + i * 19, moThumbBackImage.Width, moThumbBackImage.Height), GraphicsUnit.Pixel);

            //Brush oBrush = new SolidBrush(moChannelColor);
            //Brush oWhiteBrush = new SolidBrush(Color.FromArgb(255,255,255));

            ////draw channel left and right border colors
            //e.Graphics.FillRectangle(oWhiteBrush, new Rectangle(0,UpArrowImage.Height, 1, (this.Height-DownArrowImage.Height)));
            //e.Graphics.FillRectangle(oWhiteBrush, new Rectangle(this.Width-1, UpArrowImage.Height, 1, (this.Height - DownArrowImage.Height)));

            ////draw channel
            //e.Graphics.FillRectangle(oBrush, new Rectangle(1, UpArrowImage.Height, this.Width-2, (this.Height-DownArrowImage.Height)));

            //draw thumb
            //int nTrackHeight = (this.Height - (moUpArrowImage.Height + moDownArrowImage.Height));
            //float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;
            //int nThumbHeight = (int)fThumbHeight;

            //if (nThumbHeight > nTrackHeight) {
            //    nThumbHeight = nTrackHeight;
            //    fThumbHeight = nTrackHeight;
            //}
            //if (nThumbHeight < 56) {
            //    nThumbHeight = 56;
            //    fThumbHeight = 56;
            //}

            ////Debug.WriteLine(nThumbHeight.ToString());

            //float fSpanHeight = (fThumbHeight - (moThumbMiddleImage.Height + moThumbTopImage.Height + moThumbBottomImage.Height)) / 2.0f;
            //int nSpanHeight = (int)fSpanHeight;

            //int nTop = MiddleBlockRect.Top;//moThumbTop;
            //nTop += moUpArrowImage.Height;

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
            //int middleheight = nSpanHeight * 2 + moThumbTopImage.Height + moThumbMiddleImage.Height;
            TextureBrush tbm = new TextureBrush(m_ScrollImage, System.Drawing.Drawing2D.WrapMode.Tile, new Rectangle(moThumbMiddleImage.X, moThumbMiddleImage.Y + i * 19, moThumbMiddleImage.Width, moThumbMiddleImage.Height));
            //e.Graphics.FillRectangle(tbm, new Rectangle(0, nTop + moThumbTopImage.Height, this.Width, middleheight));
            e.Graphics.FillRectangle(tbm, this.MiddleBlockRect);
            tbm.Dispose();
            //draw top
            int h = moThumbTopImage.Height;
            if (this.MiddleBlockRect.Height < 16 && this.MiddleBlockRect.Height < h) {
                h = 4;

            }
            else {
                e.Graphics.DrawImage(m_ScrollImage, new Rectangle(0, MiddleBlockRect.Top, this.Width, h), new Rectangle(moThumbTopImage.X, moThumbTopImage.Y + i * 19, moThumbTopImage.Width, h), GraphicsUnit.Pixel);

                //绘制下面 
                e.Graphics.DrawImage(m_ScrollImage, new Rectangle(0, MiddleBlockRect.Bottom - moThumbBottomImage.Height, this.Width, h), new Rectangle(moThumbBottomImage.X, moThumbBottomImage.Y + i * 19, moThumbBottomImage.Width, h), GraphicsUnit.Pixel);

            }

            //画上荐头
            e.Graphics.DrawImage(m_ScrollImage, new Rectangle(new Point(0, 0), new Size(this.Width, moUpArrowImage.Height)), new Rectangle(moUpArrowImage.X, moUpArrowImage.Y + i * 19, moUpArrowImage.Width, moUpArrowImage.Height), GraphicsUnit.Pixel);
            //下箭头draw bottom
            e.Graphics.DrawImage(m_ScrollImage, new Rectangle(new Point(0, (this.Height - moDownArrowImage.Height)), new Size(this.Width, moDownArrowImage.Height)), new Rectangle(moDownArrowImage.X, moDownArrowImage.Y + i * 19, moDownArrowImage.Width, moDownArrowImage.Height), GraphicsUnit.Pixel);


        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            //base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e) {

            // e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (Orientation == ScrollOrientation.VerticalScroll) {

                OnPaint_V(e);
                //DrawBack_V(e.Graphics); 
                //DrawUpArrow_V(e.Graphics);
                ////DrawMiddle_V(e.Graphics);
                //DrawMiddleBlock_V(e.Graphics);
                //DrawBottomArrow_V(e.Graphics);
            }
            else {
                DrawBack_H(e.Graphics);
                DrawUpArrow_H(e.Graphics);
                //DrawMiddle_H(e.Graphics);
                DrawMiddleBlock_H(e.Graphics);
                DrawBottomArrow_H(e.Graphics);
            }
        }

        private void DrawBack_V(Graphics graphics) {
            graphics.DrawImage(this.TopImage, this.ClientRectangle, m_ThumbBackImage_V, GraphicsUnit.Pixel);
        }

        private void DrawBack_H(Graphics graphics) {
            graphics.DrawImage(this.TopImage, this.ClientRectangle, m_ThumbBackImage_H, GraphicsUnit.Pixel);

        }

        //H和V的底图之间的Y差值
        private const int m_H2V_Y = 57;
        private void DrawMiddleBlock_H(Graphics graphics) {
            if (m_MiddleDown || m_MiddleBlockDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Width <= 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.Right - 16, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X + 8, MiddleBlockRect.Y, MiddleBlockRect.Width - 16, MiddleBlockRect.Height);
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Width % 16;
                        rectM.X = rectM.Right - k;
                        rectM.Width = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                    }

                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Width == 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.Right - 16, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X + 8, MiddleBlockRect.Y, MiddleBlockRect.Width - 16, MiddleBlockRect.Height);
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Right % 16;
                        rectM.X = rectM.Right - k;
                        rectM.Width = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                    }
                }
            }
            else {
                //正常情况
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Width == 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.Right - 16, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, m_H2V_Y, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X + 8, MiddleBlockRect.Y, MiddleBlockRect.Width - 16, MiddleBlockRect.Height);
                        graphics.DrawImage(m_ScrollImage, rectM, 57, m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Width % 16;
                        rectM.X = rectM.Right - k;
                        rectM.Width = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, m_H2V_Y, 16, 16, GraphicsUnit.Pixel);


                    }
                }
            }

            //TODO:依据ThemeManager的BackColor进行颜色混合

        }

        private void DrawMiddle_H(Graphics graphics) {

            /*
            Color[] colorArray;
            float[] numArray;
            ColorBlend blend;
            LinearGradientBrush brush;
            Color[] colorArray2;
            Rectangle rectangle = MiddleRect;

            Color c = ControlPaint.Light(RibbonThemeManager.BackColor);
            colorArray2 = new Color[] { RibbonTabControl.GetColor(c, 1.025), RibbonTabControl.GetColor(c, 1.0), RibbonTabControl.GetColor(c, 0.975), RibbonTabControl.GetColor(c, 1.075) };
            colorArray = colorArray2;
            numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
            blend = new ColorBlend();
            blend.Colors = colorArray;
            blend.Positions = numArray;
            brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
            brush.InterpolationColors = blend;
            RibbonTabControl.FillRoundRectangle(graphics, brush, rectangle, 0.1f);
            using (Pen p = new Pen(RibbonTabControl.GetColor(c, 0.75))) {
                RibbonTabControl.DrawRoundRectangle(graphics, p, rectangle, 0.1f);
            }
            //rectangle.Offset(1, 1);
            rectangle.Width -= 2;
            rectangle.Height -= 2;
            using (Brush b = new LinearGradientBrush(rectangle, RibbonTabControl.GetColor(c, 1.05), Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
                using (Pen p2 = new Pen(b)) {
                    RibbonTabControl.DrawRoundRectangle(graphics, p2, rectangle, 0.1f);
                }
            }

            brush.Dispose();
            //SolidBrush sb = new SolidBrush(Color.White);
            //graphics.FillRectangle(sb,MiddleRect);
            //sb.Dispose();
             */
        }

        private void DrawBottomArrow_H(Graphics graphics) {

            if (m_BottomDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else {
                //正常情况
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 0 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }

            //TODO:依据ThemeManager的BackColor进行颜色混合
        }

        private void DrawUpArrow_H(Graphics graphics) {
            if (m_UpDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 38 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 19 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else {
                //正常情况
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 0 + m_H2V_Y, 16, 16, GraphicsUnit.Pixel);
                }
            }
            //TODO:依据ThemeManager的BackColor进行颜色混合
        }

        private void DrawMiddleBlock_V(Graphics graphics) {
            if (m_MiddleDown || m_MiddleBlockDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Height == 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 38, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 38, 16, 16, GraphicsUnit.Pixel);

                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Bottom - 16, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, 38, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y + 8, MiddleBlockRect.Width, MiddleBlockRect.Height - 16);
                        TextureBrush tb = new TextureBrush(m_ScrollImage, WrapMode.Tile, new Rectangle(57, 19, 16, 16));
                        graphics.FillRectangle(tb, rectM);
                        tb.Dispose();
                        //graphics.DrawImage(m_ScrollImage, rectM, 57, 38, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Height % 16;
                        rectM.Y = rectM.Bottom - k;
                        rectM.Height = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 38, 16, 16, GraphicsUnit.Pixel);

                    }

                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Height == 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 0, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 0, 16, 16, GraphicsUnit.Pixel);
                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Bottom - 16, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, 0, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y + 8, MiddleBlockRect.Width, MiddleBlockRect.Height - 16);
                        TextureBrush tb = new TextureBrush(m_ScrollImage, WrapMode.Tile, new Rectangle(57, 0, 16, 16));
                        graphics.FillRectangle(tb, rectM);
                        tb.Dispose();
                        //graphics.DrawImage(m_ScrollImage, rectM, 57, 0, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Height % 16;
                        rectM.Y = rectM.Bottom - k;
                        rectM.Height = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 0, 16, 16, GraphicsUnit.Pixel);

                    }
                }
            }
            else {
                //正常情况
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    if (MiddleBlockRect.Height == 16) {
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 19, 16, 16, GraphicsUnit.Pixel);
                    }
                    else {
                        //画开始
                        Rectangle rectB = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectB, 38, 19, 16, 16, GraphicsUnit.Pixel);

                        //画结束
                        Rectangle rectE = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Bottom - 16, 16, 16);
                        graphics.DrawImage(m_ScrollImage, rectE, 38, 19, 16, 16, GraphicsUnit.Pixel);

                        //画中间
                        Rectangle rectM = new Rectangle(MiddleBlockRect.X, MiddleBlockRect.Y + 8, MiddleBlockRect.Width, MiddleBlockRect.Height - 16);
                        TextureBrush tb = new TextureBrush(m_ScrollImage, WrapMode.Tile, new Rectangle(57, 19, 16, 16));
                        graphics.FillRectangle(tb, rectM);
                        tb.Dispose();
                        //graphics.DrawImage(m_ScrollImage, rectM, 57, 19, 16, 16, GraphicsUnit.Pixel);
                        //不足一个单位的剩余部分区域,需要单独绘制,不使用系统提供的自动插值
                        int k = rectM.Height % 16;
                        //rectM.Height = k;
                        rectM.Y = rectM.Bottom - k;
                        rectM.Height = k;
                        graphics.DrawImage(m_ScrollImage, rectM, 57, 19, 16, 16, GraphicsUnit.Pixel);


                    }
                }
            }

            //TODO:依据ThemeManager的BackColor进行颜色混合

        }
        Rectangle m_ThumbBackImage_V = new Rectangle(4 * 19, 0, 16, 16);
        Rectangle m_ThumbBackImage_H = new Rectangle(4 * 19, 3 * 19, 16, 16);
        private void DrawMiddle_V(Graphics graphics) {
            /*
     Color[] colorArray;
     float[] numArray;
     ColorBlend blend;
     LinearGradientBrush brush;
     Color[] colorArray2;
     Rectangle rectangle = MiddleRect;

     Color c = ControlPaint.Light(RibbonThemeManager.BackColor);
     colorArray2 = new Color[] { RibbonTabControl.GetColor(c, 1.025), RibbonTabControl.GetColor(c, 1.0), RibbonTabControl.GetColor(c, 0.975), RibbonTabControl.GetColor(c, 1.075) };
     colorArray = colorArray2;
     numArray = new float[] { 0f, 0.2f, 0.2f, 1f };
     blend = new ColorBlend();
     blend.Colors = colorArray;
     blend.Positions = numArray;
     brush = new LinearGradientBrush(rectangle, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
     brush.InterpolationColors = blend;
     RibbonTabControl.FillRoundRectangle(graphics, brush, rectangle, 0.1f);
     using (Pen p = new Pen(RibbonTabControl.GetColor(c, 0.75))) {
         RibbonTabControl.DrawRoundRectangle(graphics, p, rectangle, 0.1f);
     }
     //rectangle.Offset(1, 1);
     rectangle.Width -= 2;
     rectangle.Height -= 2;
     using (Brush b = new LinearGradientBrush(rectangle, RibbonTabControl.GetColor(c, 1.05), Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
         using (Pen p2 = new Pen(b)) {
             RibbonTabControl.DrawRoundRectangle(graphics, p2, rectangle, 0.1f);
         }
     }

     brush.Dispose();
     //SolidBrush sb = new SolidBrush(Color.White);
     //graphics.FillRectangle(sb,MiddleRect);
     //sb.Dispose();
      * */
        }

        private void DrawBottomArrow_V(Graphics graphics) {

            if (m_BottomDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 38, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 0, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else {
                //正常情况
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, BottomRect, 19, 19, 16, 16, GraphicsUnit.Pixel);
                }
            }

            //TODO:依据ThemeManager的BackColor进行颜色混合
        }

        private void DrawUpArrow_V(Graphics graphics) {
            if (m_UpDown) {
                //鼠标按下
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 38, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else if (m_MouseIn) {
                //鼠标在滚动条范围内
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 0, 16, 16, GraphicsUnit.Pixel);
                }
            }
            else {
                //正常情况
                if (m_ScrollImage != null) {
                    graphics.DrawImage(m_ScrollImage, UpRect, 0, 19, 16, 16, GraphicsUnit.Pixel);
                }
            }

            //TODO:依据ThemeManager的BackColor进行颜色混合


        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // CustomScrollbar
            // 
            this.Name = "RibbonScrollbar";
            this.ResumeLayout(false);

        }

        ////获取正向映射值(滚动条滚动距离和实际值的变化的对应关系)
        //private int GetMap(int value) {
        //    if (this.Orientation == ScrollOrientation.VerticalScroll) {
        //        int d = (int)(value * (float)(m_Maximum - m_Minimum) / (this.MiddleRect.Height - this.MiddleBlockRect.Height));
        //        return d;
        //    }
        //    return 0;
        //}

        //private int GetNMap(int value) {
        //    if (this.Orientation == ScrollOrientation.VerticalScroll) {
        //        //int d = (int)(value * (this.MiddleRect.Height-this.MiddleBlockRect.Height) / (float)(m_Maximum - m_Minimum));
        //        //滚动条轨道空余的部分对应的值范围
        //        int v = (int)((float)(this.MiddleRect.Height - this.MiddleBlockRect.Height) / MiddleRect.Height * (m_Maximum - m_Minimum));

        //        float percent = (float)value / (v);//计算实际值占范围的百分比
        //        //依据百分比计算value对应在滚动条上的位移
        //        int d = (int)((MiddleRect.Height - MiddleBlockRect.Height) * percent);

        //        return d;
        //    }
        //    return 0;
        //}

        protected override void WndProc(ref Message m) {
            //if(m_Ctl!=null){
            //    int style=NativeAPI.GetWindowLong(m_Ctl.Handle,NativeAPI.GWL_STYLE);
            //    bool vShow=((style & NativeAPI.WS_VSCROLL)==NativeAPI.WS_VSCROLL);
            //    bool hShow=((style & NativeAPI.WS_HSCROLL)==NativeAPI.WS_HSCROLL);
            //    if(m_Orientation == ScrollOrientation.VerticalScroll){
            //        if(vShow){

            //        }
            //    }

            if (m.Msg == (int)Win32Native.WindowMessage.MouseWheel) { //mousewheel
                if (this.Visible == false) {
                    return;
                }
                int delta = Win32Native.HiWord((uint)m.WParam);
                System.Diagnostics.Debug.WriteLine(delta);
                //int k = Math.Abs(delta) / 120;

                if (delta < 0) {

                    // if (k <2) {
                    Move(ScrollEventType.SmallIncrement, 1, m_InternalSmallChange);
                    // }
                    // else {
                    //     Move(ScrollEventType.LargeIncrement, 1, m_InternalLargeChange);
                    // }
                }
                else {
                    // if (k < 2) {
                    Move(ScrollEventType.SmallDecrement, -1, m_InternalSmallChange);
                    // }
                    // else {
                    //     Move(ScrollEventType.LargeDecrement, -1, m_InternalLargeChange);
                    // }
                }
                m.Result = IntPtr.Zero;
                Invalidate();
                return;
            }

            base.WndProc(ref m);
        }
        private Point m_MiddleDownPoint = Point.Empty;
        protected override void OnMouseDown(MouseEventArgs e) {
            m_MouseEventArgs = e;
            m_BottomDown = false;
            m_UpDown = false;
            m_MiddleBlockDown = false;
            m_MiddleDown = false;

            //m_MouseState[0] == MouseState.Out;
            //m_MouseState[1] == MouseState.Out;
            //m_MouseState[2] == MouseState.Out;

            if (this.MiddleBlockRect.Contains(e.X, e.Y)) {
                m_State = ScrollBarState.Dragging;
                m_MiddleBlockDown = true;
                m_MouseState[1] = MouseState.Down;
                if (m_Orientation == ScrollOrientation.VerticalScroll) {
                    // m_MiddleDownPoint = new Point(e.X, e.Y - MiddleBlockRect.Y);
                    m_MiddleDownPoint = new Point(e.X, e.Y);
                }
                else {
                    m_MiddleDownPoint = new Point(e.X, e.Y);// - MiddleBlockRect.X, e.Y);
                }
            }
            else if (this.BottomRect.Contains(e.X, e.Y)) {
                m_State = ScrollBarState.BottomDown;
                m_BottomDown = true;
                m_MouseState[2] = MouseState.Down;
                m_timer.Enabled = true;
                m_timer.Start();
            }
            else if (this.MiddleRect.Contains(e.X, e.Y)) {
                m_MiddleDown = true;
                m_State = ScrollBarState.LargeDown;
                m_timer.Enabled = true;
                m_timer.Start();
            }
            else if (this.UpRect.Contains(e.X, e.Y)) {
                m_UpDown = true;
                m_State = ScrollBarState.UpDown;
                m_MouseState[0] = MouseState.Down;
                m_timer.Enabled = true;
                m_timer.Start();
            }

            int dir = 1; //1向下,-1,向上
            switch (m_State) {
                case ScrollBarState.LargeDown:
                    //如果鼠标点击在滚动条中间块上,则什么也不做.
                    //if (this.Orientation == ScrollOrientation.VerticalScroll) {
                    if (MiddleBlockRect.Contains(e.X, e.Y)) {
                        return;
                    }
                    else {
                        //向下或向上滚动一个大单位
                        //如果在中间块的下方点击,则方向向下
                        bool whichDir = false;//向左/向右/向上/向下
                        if (m_Orientation == ScrollOrientation.VerticalScroll) {
                            whichDir = (e.Y > MiddleBlockRect.Y + MiddleBlockRect.Height * 0.5f);//是否向下
                        }
                        else {
                            whichDir = (e.X > MiddleBlockRect.X + MiddleBlockRect.Width * 0.5f);//是否向右
                        }
                        int dPixel = m_InternalLargeChange;// ValueToPos_V(m_InternalLargeChange);
                        if (whichDir) {
                            dir = 1;

                            Move(ScrollEventType.LargeIncrement, dir, dPixel);
                        }
                        else {
                            dir = -1;
                            Move(ScrollEventType.LargeDecrement, dir, dPixel);
                        }

                    }
                    //}
                    //else {
                    //}
                    break;
                case ScrollBarState.Dragging:
                    break;
                case ScrollBarState.UpDown:
                    //向上滚动一个最小单位
                    Move(ScrollEventType.SmallDecrement, -1, m_InternalSmallChange);
                    break;
                case ScrollBarState.BottomDown:
                    Move(ScrollEventType.SmallIncrement, 1, m_InternalSmallChange);
                    break;
                default:
                    break;
            }
            Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            m_BottomDown = false;
            m_UpDown = false;
            m_MiddleBlockDown = false;
            m_MiddleDown = false;

            if (m_State == ScrollBarState.Dragging) {
                //if (OnScroll != null) {
                //OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, m_Value));

                //}
                if (m_Ctl != null) {
                    ScrollableControl scr = m_Ctl as ScrollableControl;
                    if (scr != null) {
                        Win32Native.SendMessage(m_Ctl.Handle, m_Orientation == ScrollOrientation.VerticalScroll ? WM_VSCROLL : WM_HSCROLL, 4, 0);

                        if (m_Orientation == ScrollOrientation.VerticalScroll) {
                            //Point p = m_Ctl.AutoScrollOffset;
                            //p.Y = m_Value;
                            //m_Ctl.AutoScrollOffset = p;
                            scr.VerticalScroll.Value = m_Value;
                        }
                        else {
                            // Point p = m_Ctl.AutoScrollOffset;
                            //p.X = m_Value;
                            //m_Ctl.AutoScrollOffset = p;
                            scr.HorizontalScroll.Value = m_Value;
                            // System.Diagnostics.Debug.WriteLine("H: " + m_Value);
                        }
                    }
                }
            }

            m_State = ScrollBarState.MouseIn;
            m_timer.Enabled = false;
            m_timer.Stop();
            Invalidate();
            base.OnMouseUp(e);
        }
        private bool m_Reach = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="dir"></param>
        /// <param name="d">变化量，屏幕像素为单位</param>
        internal void Move(ScrollEventType set, int dir, int d) {

            int oldValue = this.m_Value;
            if (this.Orientation == ScrollOrientation.VerticalScroll) {

                Move_V(dir, d);
            }
            else {
                Move_H(dir, d);

            }

            OnScroll(new ScrollEventArgs(set, oldValue, m_Value));



        }
        public void ResetScroll() {
            OnScroll(new ScrollEventArgs(ScrollEventType.First, 0, 0));
        }
        private void Move_H(int dir, int d) {
            int tempValue = m_Value + (dir * d);
            int v = (int)((float)(this.MiddleRect.Width - this.MiddleBlockRect.Width) / MiddleRect.Width * (m_Maximum - m_Minimum));
            if (v == 0) {
                return;
            }
            float percent = tempValue / (float)(v);

            int blockPos = (int)((MiddleRect.Width - MiddleBlockRect.Width) * percent);
            //m_Value += (dir*d);
            //delta = dir * delta;
            if (dir == 1) {

                if (MiddleBlockRect.Width + blockPos > MiddleRect.Width) {
                    //到底了.
                    //m_Value = Maximum;
                    if (!m_Reach) {
                        m_Value = tempValue;
                        MiddleBlockRect.X = MiddleRect.X + MiddleRect.Width - MiddleBlockRect.Width;
                        m_Reach = true;
                    }
                }
                else {
                    m_Reach = false;
                    MiddleBlockRect.X = blockPos + UpRect.Right;
                    m_Value = tempValue;
                }
            }
            else {
                if (blockPos <= 0) {
                    m_Reach = true;
                    //到顶了
                    m_Value = Minimum;
                    MiddleBlockRect.X = MiddleRect.X;
                }
                else {
                    m_Reach = false;
                    //MiddleBlockRect.Y = blockPos;
                    MiddleBlockRect.X = blockPos + UpRect.Right;
                    m_Value = tempValue;
                }
            }
        }
        /// <summary>
        /// 将鼠标位置转换成滚动条的值
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int PosToValue_V(int y) {
            //只算值和空白部分的映射
            float ret = (float)(y - MiddleRect.Top) / (MiddleRect.Height - MiddleBlockRect.Height) * (m_Maximum - m_Minimum);
            return (int)ret;
        }
        /// <summary>
        /// 将值变化量转换成滚动条的像素
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int ValueToPos_V(int v) {
            float ret = (float)(MiddleRect.Height - MiddleBlockRect.Height) * v / (m_Maximum - m_Minimum);
            return (int)ret + MiddleRect.Top;
        }
        private void Move_V(int dir, int d) {
            int dPixel = d;
            if (dir == 1) {
                //向下
                #region 修改
                float f = (float)(m_Value + d) / this.Maximum;
                int imgD = (int)((MiddleRect.Height - MiddleBlockRect.Height) * f);
                if (m_Value + d > this.Maximum) {
                    m_Value = m_Maximum;
                    MiddleBlockRect.Y = MiddleRect.Bottom - MiddleBlockRect.Height;
                    return;
                }
                MiddleBlockRect.Y = MiddleRect.Top + imgD;
                m_Value += d;
                return;
                #endregion

                if (MiddleBlockRect.Y + dPixel - MiddleRect.Y > MiddleRect.Height - MiddleBlockRect.Height) {
                    //如果已经到底了
                    m_Value = m_Maximum;
                    MiddleBlockRect.Y = MiddleRect.Bottom - MiddleBlockRect.Height;
                    return;
                }

                m_Value = PosToValue_V(MiddleBlockRect.Y + dPixel);
                MiddleBlockRect.Y += dPixel;
            }
            else {
                #region 修改
                if (m_Value - d < 0) {
                    //如果已经到顶了
                    m_Value = m_Minimum;
                    MiddleBlockRect.Y = MiddleRect.Top;
                    return;
                }
                float f = (float)(m_Value - d) / this.Maximum;
                int imgD = (int)((MiddleRect.Height - MiddleBlockRect.Height) * f);
                MiddleBlockRect.Y = MiddleRect.Top + imgD;
                m_Value -= d;
                return;
                #endregion

                if (MiddleBlockRect.Y - dPixel < MiddleRect.Y) {
                    //如果已经到顶了
                    m_Value = m_Minimum;
                    MiddleBlockRect.Y = MiddleRect.Top;
                    return;
                }
                m_Value = PosToValue_V(MiddleBlockRect.Y - dPixel);
                MiddleBlockRect.Y -= dPixel;
            }
        }
        /// <summary>
        /// 获取鼠标所在的位置的滚动条的值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GetMouseAtValue(int x, int y) {
            if (m_Orientation == ScrollOrientation.VerticalScroll) {
                float percent = y / (float)(MiddleRect.Height - MiddleBlockRect.Height);
                //int v = (int)((float)(this.MiddleRect.Height - this.MiddleBlockRect.Height) / MiddleRect.Height * (m_Maximum - m_Minimum));
                int v = m_Maximum - m_Minimum;
                int value = (int)(v * percent);
                return value;
            }
            else {
                float percent = x / (float)(MiddleRect.Width - MiddleBlockRect.Width);
                int v = (int)((float)(this.MiddleRect.Width - this.MiddleBlockRect.Width) / MiddleRect.Width * (m_Maximum - m_Minimum));

                int value = (int)(v * percent);
                return value;
            }

        }
        protected override void OnMouseEnter(EventArgs e) {
            m_MouseIn = true;
            Invalidate();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            m_MouseIn = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        private int m_prevPos = 0;
        protected override void OnMouseMove(MouseEventArgs e) {

            if (m_State == ScrollBarState.Dragging) { //当滑动块被拖动时,滑动内容

                if (this.Orientation == ScrollOrientation.VerticalScroll) {
                    #region V
                    int dy = e.Y - m_MiddleDownPoint.Y;

                    float f = (float)dy / (MiddleRect.Height - MiddleBlockRect.Height);
                    dy = (int)(this.Maximum * f);
                    Move(ScrollEventType.ThumbPosition, Math.Sign(dy), Math.Abs(dy));
                    //System.Diagnostics.Debug.WriteLine(m_MiddleDownPoint.Y.ToString() + "  " + e.Y.ToString() + "  " + d.ToString() + "  " + m_Value.ToString());

                    #endregion
                    m_MiddleDownPoint = new Point(e.X, e.Y);
                }
                else {
                    #region H

                    #endregion
                }
            }

            Invalidate();

            base.OnMouseMove(e);
        }


        private bool m_UpDown;
        private bool m_BottomDown;
        private bool m_MiddleDown;
        private bool m_MiddleBlockDown;

        private bool m_MouseInMiddle;
        private bool m_MouseInMiddleBlock;
        private bool m_MouseInUp;
        private bool m_MouseInBottom;

        private bool m_MouseIn; //鼠标在滚动条范围内

        public Rectangle UpRect;
        public Rectangle BottomRect;
        public Rectangle MiddleRect;
        public Rectangle MiddleBlockRect;

        ScrollBarState m_State;
        private ScrollOrientation m_Orientation;
        /// <summary>
        /// 滚动条方向,先支持竖向
        /// </summary>
        public ScrollOrientation Orientation {
            get {
                return m_Orientation;
            }
            set {
                m_Orientation = value;
                if (m_Orientation == ScrollOrientation.HorizontalScroll) {
                    this.Width = 150;
                    this.Height = 16;
                }
                else {
                    this.Width = 16;
                    this.Height = 150;
                }
                InitLayoutByBindCtl();
            }
        }
        #endregion

        #region UISKin 成员
        private Rectangle m_NormalRect = Rectangle.Empty;
        private Rectangle m_HoverRect = Rectangle.Empty;
        private Rectangle m_DownRect = Rectangle.Empty;
        private Rectangle m_DisableRect = Rectangle.Empty;
        public Rectangle RectNormal {
            get {
                return m_NormalRect;
            }
            set {
                m_NormalRect = value;
            }
        }
        public Rectangle RectHover {
            get {
                return m_HoverRect;
            }
            set {
                m_HoverRect = value;
            }
        }

        public Rectangle RectDown {
            get {
                return m_DownRect;
            }
            set {
                m_DownRect = value;
            }
        }

        public Rectangle RectDisable {
            get {
                return m_DisableRect;
            }
            set {
                m_DisableRect = value;
            }
        }

        public string ImagePath {
            get;
            set;
        }

        public int ImageLayout {
            get;
            set;
        }

        public int ZOrder {
            get;
            set;
        }

        public int DockFlag {
            get;
            set;
        }

        internal Image TopImage {
            get {
                return m_ScrollImage;
            }
            set {
                m_ScrollImage = value;
            }
        }
        public void UpdateSet() {
            //更新方法
            if (!string.IsNullOrEmpty(ImagePath) && this.RectNormal != Rectangle.Empty && this.RectDisable != Rectangle.Empty) {
                //if (this.TopImage != null) {
                //    this.TopImage.Dispose();
                //}
                this.TopImage = RibbonUISkinHelper.GetPathImage(ImagePath);
                //if (this.m_ScrollImage != null) {
                //    m_ScrollImage.Dispose();
                //}
                //m_ScrollImage = this.TopImage;
            }
        }
        //对颜色的支持
        protected int m_SetFlag = 0;//颜色
        [Description("设置标志，0默认1:图片,2:颜色,3:颜色+图片")]
        public virtual int SetFlag {
            get { return m_SetFlag; }
            set { m_SetFlag = value; }
        }

        protected float m_ColorFactor = 0.4f;
        /// <summary>
        /// 从BACKCOLOR取渐变ENDCOLOR系数
        /// </summary>
        [Description("从BACKCOLOR取渐变ENDCOLOR系数")]
        public virtual float ColorFactor {
            get {
                return m_ColorFactor;
            }
            set {
                m_ColorFactor = value;
            }
        }
        /// <summary>
        /// 是否自己设置颜色 默认为false
        /// </summary>
        [Description("是否自己设置颜色 默认为false")]
        public virtual bool ColorUserSet {
            get;
            set;
        }
        protected Color m_ColorStart = RibbonThemeManager.BackColor;
        /// <summary>
        /// 自定义颜色起始色
        /// </summary>
        [Description("自定义颜色起始色")]
        public Color ColorStart {
            get {
                return m_ColorStart;
            }
            set {
                m_ColorStart = value;
            }
        }
        protected Color m_ColorEnd = Color.Transparent;
        /// <summary>
        /// 自定义颜色结束色
        /// </summary>
        [Description("自定义颜色结束色")]
        public Color ColorEnd {
            get {
                return m_ColorEnd;
            }
            set {
                m_ColorEnd = value;
            }
        }
        protected float m_ColorLinearAngle = 90;
        [Description("渐变角度")]
        public float ColorLinearAngle {
            get { return m_ColorLinearAngle; }
            set {
                m_ColorLinearAngle = value;
            }
        }
        #endregion
    }
}
