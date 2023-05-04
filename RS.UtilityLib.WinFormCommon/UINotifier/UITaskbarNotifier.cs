// C# TaskbarNotifier Class v1.0
// by John O'Byrne - 02 december 2002
// 01 april 2003 : Small fix in the OnMouseUp handler
// 11 january 2003 : Patrick Vanden Driessche <pvdd@devbrains.be> added a few enhancements
//           Small Enhancements/Bugfix
//           Small bugfix: When Content text measures larger than the corresponding ContentRectangle
//                         the focus rectangle was not correctly drawn. This has been solved.
//           Added KeepVisibleOnMouseOver
//           Added ReShowOnMouseOver
//           Added If the Title or Content are too long to fit in the corresponding Rectangles,
//                 the text is truncateed and the ellipses are appended (StringTrimming).

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace RS.UtilityLib.WinFormCommon.UINotifier
{
    /// <summary>
    /// TaskbarNotifier allows to display MSN style/Skinned instant messaging popups
    /// </summary>
    public class UITaskbarNotifier : System.Windows.Forms.Form
    {
        private static UITaskbarNotifier m_Instance;
        public static UITaskbarNotifier Sington {
            get {
                if (m_Instance == null || m_Instance.IsDisposed) {
                    m_Instance = new UITaskbarNotifier();
                }
                return m_Instance;
            }
        }


        #region TaskbarNotifier Protected Members
        protected Bitmap BackgroundBitmap = null;
        protected Bitmap CloseBitmap = null;
        protected Point CloseBitmapLocation;
        protected Size CloseBitmapSize;
        protected Rectangle RealTitleRectangle;
        protected Rectangle RealContentRectangle;
        protected Rectangle WorkAreaRectangle;
        protected Timer timer = new Timer();
        protected TaskbarStates taskbarState = TaskbarStates.hidden;
        protected string titleText;
        protected string contentText;
        protected Color normalTitleColor = Color.FromArgb(255, 0, 0);
        protected Color hoverTitleColor = Color.FromArgb(255, 0, 0);
        protected Color normalContentColor = Color.FromArgb(0, 0, 0);
        protected Color hoverContentColor = Color.FromArgb(0, 0, 0x66);
        protected Font normalTitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverTitleFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        protected Font normalContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected int nShowEvents;
        protected int nHideEvents;
        protected int nVisibleEvents;
        protected int nIncrementShow;
        protected int nIncrementHide;
        protected bool bIsMouseOverPopup = false;
        protected bool bIsMouseOverClose = false;
        protected bool bIsMouseOverContent = false;
        protected bool bIsMouseOverTitle = false;
        protected bool bIsMouseDown = false;
        protected bool bKeepVisibleOnMouseOver = true;          // Added Rev 002
        protected bool bReShowOnMouseOver = false;              // Added Rev 002
        #endregion

        #region TaskbarNotifier Public Members
        public Rectangle TitleRectangle;
        public Rectangle ContentRectangle;
        public bool TitleClickable = false;
        public bool ContentClickable = true;
        public bool CloseClickable = true;
        public bool EnableSelectionRectangle = true;
        public event EventHandler CloseClick = null;
        public event EventHandler TitleClick = null;
        public event EventHandler ContentClick = null;
        #endregion

        #region TaskbarNotifier Enums
        /// <summary>
        /// List of the different popup animation status
        /// </summary>
        public enum TaskbarStates
        {
            hidden = 0,
            appearing = 1,
            visible = 2,
            disappearing = 3
        }
        #endregion

        #region TaskbarNotifier Constructor
        /// <summary>
        /// The Constructor for TaskbarNotifier
        /// </summary>
        private UITaskbarNotifier() {
            // Window Style
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            base.Show();
            base.Hide();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = false;
            TopMost = true;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;

            timer.Enabled = true;
            timer.Tick += new EventHandler(OnTimer);
            //默认
            //this.SetBackgroundBitmap(UIRes.GetImage(@"Skin\Res\TaskbarNotifier_Back.bmp"), Color.FromArgb(255, 0, 255));
            //this.SetCloseBitmap(UIRes.GetImage(@"Skin\Res\TaskbarNotifier_Close.bmp"), Color.FromArgb(255, 0, 255), new Point(280, 57));
            this.SetBackgroundBitmap(Properties.Resources.TaskbarNotifier_Back, Color.FromArgb(255, 0, 255));
            this.SetCloseBitmap(Properties.Resources.TaskbarNotifier_Close, Color.FromArgb(255, 0, 255), new Point(280, 57));
            this.TitleRectangle = new Rectangle(150, 57, 125, 28);
            this.ContentRectangle = new Rectangle(75, 92, 215, 55);
        }
        #endregion

        #region TaskbarNotifier Properties
        /// <summary>
        /// Get the current TaskbarState (hidden, showing, visible, hiding)
        /// </summary>
        public TaskbarStates TaskbarState {
            get {
                return taskbarState;
            }
        }

        /// <summary>
        /// Get/Set the popup Title Text
        /// </summary>
        public string TitleText {
            get {
                return titleText;
            }
            set {
                titleText = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the popup Content Text
        /// </summary>
        public string ContentText {
            get {
                return contentText;
            }
            set {
                contentText = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Normal Title Color
        /// </summary>
        public Color NormalTitleColor {
            get {
                return normalTitleColor;
            }
            set {
                normalTitleColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Hover Title Color
        /// </summary>
        public Color HoverTitleColor {
            get {
                return hoverTitleColor;
            }
            set {
                hoverTitleColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Normal Content Color
        /// </summary>
        public Color NormalContentColor {
            get {
                return normalContentColor;
            }
            set {
                normalContentColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Hover Content Color
        /// </summary>
        public Color HoverContentColor {
            get {
                return hoverContentColor;
            }
            set {
                hoverContentColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Normal Title Font
        /// </summary>
        public Font NormalTitleFont {
            get {
                return normalTitleFont;
            }
            set {
                normalTitleFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Hover Title Font
        /// </summary>
        public Font HoverTitleFont {
            get {
                return hoverTitleFont;
            }
            set {
                hoverTitleFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Normal Content Font
        /// </summary>
        public Font NormalContentFont {
            get {
                return normalContentFont;
            }
            set {
                normalContentFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// Get/Set the Hover Content Font
        /// </summary>
        public Font HoverContentFont {
            get {
                return hoverContentFont;
            }
            set {
                hoverContentFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates if the popup should remain visible when the mouse pointer is over it.
        /// Added Rev 002
        /// </summary>
        public bool KeepVisibleOnMousOver {
            get {
                return bKeepVisibleOnMouseOver;
            }
            set {
                bKeepVisibleOnMouseOver = value;
            }
        }

        /// <summary>
        /// Indicates if the popup should appear again when mouse moves over it while it's disappearing.
        /// Added Rev 002
        /// </summary>
        public bool ReShowOnMouseOver {
            get {
                return bReShowOnMouseOver;
            }
            set {
                bReShowOnMouseOver = value;
            }
        }

        #endregion

        #region TaskbarNotifier Public Methods
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
        /// <summary>
        /// Displays the popup for a certain amount of time
        /// </summary>
        /// <param name="strTitle">The string which will be shown as the title of the popup</param>
        /// <param name="strContent">The string which will be shown as the content of the popup</param>
        /// <param name="nTimeToShow">Duration of the showing animation (in milliseconds)</param>
        /// <param name="nTimeToStay">Duration of the visible state before collapsing (in milliseconds)</param>
        /// <param name="nTimeToHide">Duration of the hiding animation (in milliseconds)</param>
        /// <returns>Nothing</returns>
        public void Show(string strTitle, string strContent, int nTimeToShow, int nTimeToStay, int nTimeToHide) {
            WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);
            this.titleText = strTitle;
            this.contentText = strContent;
            this.nVisibleEvents = nTimeToStay;
            int n_TimeToShow = nTimeToShow;
            int n_TimeToHide = nTimeToHide;
            //线程安全
            Action ac = () => {
                //todo:
                CalculateMouseRectangles();
                // We calculate the pixel increment and the timer value for the showing animation
                int nEvents;
                if (n_TimeToShow > 10) {
                    nEvents = Math.Min((n_TimeToShow / 10), BackgroundBitmap.Height);
                    nShowEvents = n_TimeToShow / nEvents;
                    nIncrementShow = BackgroundBitmap.Height / nEvents;
                }
                else {
                    nShowEvents = 10;
                    nIncrementShow = BackgroundBitmap.Height;
                }

                // We calculate the pixel increment and the timer value for the hiding animation
                if (n_TimeToHide > 10) {
                    nEvents = Math.Min((n_TimeToHide / 10), BackgroundBitmap.Height);
                    nHideEvents = n_TimeToHide / nEvents;
                    nIncrementHide = BackgroundBitmap.Height / nEvents;
                }
                else {
                    nHideEvents = 10;
                    nIncrementHide = BackgroundBitmap.Height;
                }

                switch (taskbarState) {
                    case TaskbarStates.hidden:
                        taskbarState = TaskbarStates.appearing;
                        SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - 1, BackgroundBitmap.Width, 0);
                        timer.Interval = nShowEvents;
                        timer.Start();
                        // We Show the popup without stealing focus
                        ShowWindow(this.Handle, 4);
                        break;

                    case TaskbarStates.appearing:
                        Refresh();
                        break;

                    case TaskbarStates.visible:
                        timer.Stop();
                        timer.Interval = nVisibleEvents;
                        timer.Start();
                        Refresh();
                        break;

                    case TaskbarStates.disappearing:
                        timer.Stop();
                        taskbarState = TaskbarStates.visible;
                        SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - BackgroundBitmap.Height - 1, BackgroundBitmap.Width, BackgroundBitmap.Height);
                        timer.Interval = nVisibleEvents;
                        timer.Start();
                        Refresh();
                        break;
                }
            };
            safeActionAysn(ac);
        }
        private void safeActionAysn(Action ac) {
            if (this.InvokeRequired) {
                this.BeginInvoke(ac);
            }
            else {
                ac();
            }
        }
        /// <summary>
        /// Hides the popup
        /// </summary>
        /// <returns>Nothing</returns>
        public new void Hide() {
            if (taskbarState != TaskbarStates.hidden) {
                timer.Stop();
                taskbarState = TaskbarStates.hidden;
                base.Hide();
            }
        }

        /// <summary>
        /// Sets the background bitmap and its transparency color
        /// </summary>
        /// <param name="strFilename">Path of the Background Bitmap on the disk</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// <returns>Nothing</returns>
        public void SetBackgroundBitmap(string strFilename, Color transparencyColor) {
            BackgroundBitmap = new Bitmap(strFilename);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        /// Sets the background bitmap and its transparency color
        /// </summary>
        /// <param name="image">Image/Bitmap object which represents the Background Bitmap</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// <returns>Nothing</returns>
        public void SetBackgroundBitmap(Image image, Color transparencyColor) {
            BackgroundBitmap = new Bitmap(image);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        /// Sets the 3-State Close Button bitmap, its transparency color and its coordinates
        /// </summary>
        /// <param name="strFilename">Path of the 3-state Close button Bitmap on the disk (width must a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// <param name="position">Location of the close button on the popup</param>
        /// <returns>Nothing</returns>
        public void SetCloseBitmap(string strFilename, Color transparencyColor, Point position) {
            CloseBitmap = new Bitmap(strFilename);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }

        /// <summary>
        /// Sets the 3-State Close Button bitmap, its transparency color and its coordinates
        /// </summary>
        /// <param name="image">Image/Bitmap object which represents the 3-state Close button Bitmap (width must be a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// /// <param name="position">Location of the close button on the popup</param>
        /// <returns>Nothing</returns>
        public void SetCloseBitmap(Image image, Color transparencyColor, Point position) {
            CloseBitmap = new Bitmap(image);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }
        #endregion

        #region TaskbarNotifier Protected Methods
        protected void DrawCloseButton(Graphics grfx) {
            if (CloseBitmap != null) {
                Rectangle rectDest = new Rectangle(CloseBitmapLocation, CloseBitmapSize);
                Rectangle rectSrc;

                if (bIsMouseOverClose) {
                    if (bIsMouseDown)
                        rectSrc = new Rectangle(new Point(CloseBitmapSize.Width * 2, 0), CloseBitmapSize);
                    else
                        rectSrc = new Rectangle(new Point(CloseBitmapSize.Width, 0), CloseBitmapSize);
                }
                else
                    rectSrc = new Rectangle(new Point(0, 0), CloseBitmapSize);


                grfx.DrawImage(CloseBitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        protected void DrawText(Graphics grfx) {
            if (titleText != null && titleText.Length != 0) {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;				// Added Rev 002
                if (bIsMouseOverTitle) {
                    using (SolidBrush s1 = new SolidBrush(hoverTitleColor)) {
                        grfx.DrawString(titleText, hoverTitleFont, s1, TitleRectangle, sf);
                    }
                }
                else {
                    using (SolidBrush s2 = new SolidBrush(normalTitleColor)) {
                        grfx.DrawString(titleText, normalTitleFont, s2, TitleRectangle, sf);
                    }
                }
            }

            if (contentText != null && contentText.Length != 0) {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                sf.Trimming = StringTrimming.Word;							// Added Rev 002

                if (bIsMouseOverContent) {
                    using (SolidBrush s1 = new SolidBrush(hoverContentColor)) {
                        grfx.DrawString(contentText, hoverContentFont, s1, ContentRectangle, sf);
                    }
                    if (EnableSelectionRectangle)
                        ControlPaint.DrawBorder3D(grfx, RealContentRectangle, Border3DStyle.Etched, Border3DSide.Top | Border3DSide.Bottom | Border3DSide.Left | Border3DSide.Right);

                }
                else {
                    using (SolidBrush s1 = new SolidBrush(normalContentColor))
                        grfx.DrawString(contentText, normalContentFont, s1, ContentRectangle, sf);
                }
            }
        }

        protected void CalculateMouseRectangles() {
            Graphics grfx = CreateGraphics();
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            SizeF sizefTitle = grfx.MeasureString(titleText, hoverTitleFont, TitleRectangle.Width, sf);
            SizeF sizefContent = grfx.MeasureString(contentText, hoverContentFont, ContentRectangle.Width, sf);
            grfx.Dispose();

            // Added Rev 002
            //We should check if the title size really fits inside the pre-defined title rectangle
            if (sizefTitle.Height > TitleRectangle.Height) {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, TitleRectangle.Width, TitleRectangle.Height);
            }
            else {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, (int)sizefTitle.Width, (int)sizefTitle.Height);
            }
            RealTitleRectangle.Inflate(0, 2);

            // Added Rev 002
            //We should check if the Content size really fits inside the pre-defined Content rectangle
            if (sizefContent.Height > ContentRectangle.Height) {
                RealContentRectangle = new Rectangle((ContentRectangle.Width - (int)sizefContent.Width) / 2 + ContentRectangle.Left, ContentRectangle.Top, (int)sizefContent.Width, ContentRectangle.Height);
            }
            else {
                RealContentRectangle = new Rectangle((ContentRectangle.Width - (int)sizefContent.Width) / 2 + ContentRectangle.Left, (ContentRectangle.Height - (int)sizefContent.Height) / 2 + ContentRectangle.Top, (int)sizefContent.Width, (int)sizefContent.Height);
            }
            RealContentRectangle.Inflate(0, 2);
        }

        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor) {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int height = bitmap.Height;
            int width = bitmap.Width;

            GraphicsPath path = new GraphicsPath();

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++) {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;

                    int x0 = i;

                    while ((i < width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;

                    path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }

            Region region = new Region(path);
            path.Dispose();
            return region;
        }

        private static Region BitmapToRegion(Bitmap bitmapSource, Color transparencyColor, float scale = 1f) {

            int tColor = transparencyColor.ToArgb();
            int width = (int)(bitmapSource.Width * scale);
            int height = (int)(bitmapSource.Height * scale);
            if (width < 2) {
                width = 2;
            }
            if (height < 2) {
                height = 2;
            }
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.DrawImage(bitmapSource, new Rectangle(0, 0, width, height));
            }
            //创建region
            BitmapData bd = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            Region region = new Region(Rectangle.Empty);

            unsafe {
                uint* pixel = (uint*)bd.Scan0.ToInt32();

                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        if ((*pixel & tColor) > 0) {//0xFF000000) > 0) {
                            region.Union(new Rectangle(x, y, 1, 1));
                        }

                        pixel++;
                    }
                }
            }

            bitmap.UnlockBits(bd);

            return region;
        }
        #endregion

        #region TaskbarNotifier Events Overrides
        protected void OnTimer(Object obj, EventArgs ea) {
            switch (taskbarState) {
                case TaskbarStates.appearing:
                    if (Height < BackgroundBitmap.Height)
                        SetBounds(Left, Top - nIncrementShow, Width, Height + nIncrementShow);
                    else {
                        timer.Stop();
                        Height = BackgroundBitmap.Height;
                        timer.Interval = nVisibleEvents;
                        taskbarState = TaskbarStates.visible;
                        timer.Start();
                    }
                    break;

                case TaskbarStates.visible:
                    timer.Stop();
                    timer.Interval = nHideEvents;
                    // Added Rev 002
                    if ((bKeepVisibleOnMouseOver && !bIsMouseOverPopup) || (!bKeepVisibleOnMouseOver)) {
                        taskbarState = TaskbarStates.disappearing;
                    }
                    //taskbarState = TaskbarStates.disappearing;		// Rev 002
                    timer.Start();
                    break;

                case TaskbarStates.disappearing:
                    // Added Rev 002
                    if (bReShowOnMouseOver && bIsMouseOverPopup) {
                        taskbarState = TaskbarStates.appearing;
                    }
                    else {
                        if (Top < WorkAreaRectangle.Bottom)
                            SetBounds(Left, Top + nIncrementHide, Width, Height - nIncrementHide);
                        else
                            Hide();
                    }
                    break;
            }

        }

        protected override void OnMouseEnter(EventArgs ea) {
            base.OnMouseEnter(ea);
            bIsMouseOverPopup = true;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs ea) {
            base.OnMouseLeave(ea);
            bIsMouseOverPopup = false;
            bIsMouseOverClose = false;
            bIsMouseOverTitle = false;
            bIsMouseOverContent = false;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs mea) {
            base.OnMouseMove(mea);

            bool bContentModified = false;

            if ((mea.X > CloseBitmapLocation.X) && (mea.X < CloseBitmapLocation.X + CloseBitmapSize.Width) && (mea.Y > CloseBitmapLocation.Y) && (mea.Y < CloseBitmapLocation.Y + CloseBitmapSize.Height) && CloseClickable) {
                if (!bIsMouseOverClose) {
                    bIsMouseOverClose = true;
                    bIsMouseOverTitle = false;
                    bIsMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else if (RealContentRectangle.Contains(new Point(mea.X, mea.Y)) && ContentClickable) {
                if (!bIsMouseOverContent) {
                    bIsMouseOverClose = false;
                    bIsMouseOverTitle = false;
                    bIsMouseOverContent = true;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else if (RealTitleRectangle.Contains(new Point(mea.X, mea.Y)) && TitleClickable) {
                if (!bIsMouseOverTitle) {
                    bIsMouseOverClose = false;
                    bIsMouseOverTitle = true;
                    bIsMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else {
                if (bIsMouseOverClose || bIsMouseOverTitle || bIsMouseOverContent)
                    bContentModified = true;

                bIsMouseOverClose = false;
                bIsMouseOverTitle = false;
                bIsMouseOverContent = false;
                Cursor = Cursors.Default;
            }

            if (bContentModified)
                Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs mea) {
            base.OnMouseDown(mea);
            bIsMouseDown = true;

            if (bIsMouseOverClose)
                Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs mea) {
            base.OnMouseUp(mea);
            bIsMouseDown = false;

            if (bIsMouseOverClose) {
                Hide();

                if (CloseClick != null)
                    CloseClick(this, new EventArgs());
            }
            else if (bIsMouseOverTitle) {
                if (TitleClick != null)
                    TitleClick(this, new EventArgs());
            }
            else if (bIsMouseOverContent) {
                if (ContentClick != null)
                    ContentClick(this, new EventArgs());
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pea) {
            Graphics grfx = pea.Graphics;
            grfx.PageUnit = GraphicsUnit.Pixel;

            Graphics offScreenGraphics;
            Bitmap offscreenBitmap;

            offscreenBitmap = new Bitmap(BackgroundBitmap.Width, BackgroundBitmap.Height);
            offScreenGraphics = Graphics.FromImage(offscreenBitmap);

            if (BackgroundBitmap != null) {
                offScreenGraphics.DrawImage(BackgroundBitmap, 0, 0, BackgroundBitmap.Width, BackgroundBitmap.Height);
            }

            DrawCloseButton(offScreenGraphics);
            DrawText(offScreenGraphics);

            grfx.DrawImage(offscreenBitmap, 0, 0);
        }
        #endregion
    }
}
