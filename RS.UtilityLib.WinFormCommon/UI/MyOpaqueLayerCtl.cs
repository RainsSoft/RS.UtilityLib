using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace RS.UtilityLib.WinFormCommon.UI
{
    [ToolboxBitmap(typeof(MyOpaqueLayerCtl))]
    public partial class MyOpaqueLayerCtl : UserControl
    {

        private bool _transparentBG = true;
        private int _alpha = 125;
        private bool _showText = false;
        private string _text = "遮罩层";
        //    public MyOpaqueLayerCtl() {
        //        InitializeComponent();
        //    }

        public MyOpaqueLayerCtl()
            : this(125, false) {

        }

        public MyOpaqueLayerCtl(int Alpha, bool showText) {
            SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Opaque |
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.UserPaint,
            //    true);
            //base.CreateControl();
            InitializeComponent();
            //SetStyle(ControlStyles.ResizeRedraw, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            ////SetStyle(ControlStyles.DoubleBuffer, true);
            //SetStyle(ControlStyles.UserPaint, true);
            this._alpha = Alpha;
            this._showText = showText;
            //if(showLoadingImage) {
            //    PictureBox pictureBox_Loading = new PictureBox();
            //    pictureBox_Loading.BackColor = System.Drawing.Color.White;
            //    pictureBox_Loading.Image = global::MyOpaqueLayer.Properties.Resources.loading;
            //    pictureBox_Loading.Name = "pictureBox_Loading";
            //    pictureBox_Loading.Size = new System.Drawing.Size(48, 48);
            //    pictureBox_Loading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            //    Point Location = new Point(this.Location.X + (this.Width - pictureBox_Loading.Width) / 2, this.Location.Y + (this.Height - pictureBox_Loading.Height) / 2);
            //    pictureBox_Loading.Location = Location;
            //    pictureBox_Loading.Anchor = AnchorStyles.None;
            //    this.Controls.Add(pictureBox_Loading);
            //}

        }

        #region Designer      
        private bool IsDesignerHosted {
            get {
                if (DesignMode)
                    return DesignMode;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
        #endregion

        /// <summary>
        /// 自定义绘制窗体
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            if (_transparentBG) {
                //完全透明，不用画了
                return;
            }
            //
            float vlblControlWidth;
            float vlblControlHeight;

            Pen labelBorderPen;
            SolidBrush labelBackColorBrush;
            //if(_transparentBG) {
            Color drawColor = Color.FromArgb(this._alpha, this.BackColor);
            labelBorderPen = new Pen(drawColor, 0);
            labelBackColorBrush = new SolidBrush(drawColor);
            //}          
            //            
            base.OnPaint(e);
            //

            vlblControlWidth = this.Size.Width;
            vlblControlHeight = this.Size.Height;
            e.Graphics.DrawRectangle(labelBorderPen, 0, 0, vlblControlWidth, vlblControlHeight);
            e.Graphics.FillRectangle(labelBackColorBrush, 0, 0, vlblControlWidth, vlblControlHeight);
            //
            labelBorderPen.Dispose();
            labelBackColorBrush.Dispose();

            //画文本有一点不正常的地方就是在父容器状态变化时不能有效展现
            if (this._showText && !string.IsNullOrEmpty(this._text)) {
                var sizef = e.Graphics.MeasureString(this._text, this.Font);
                int pX = (int)((vlblControlWidth - sizef.Width) * 0.5f);
                int pY = (int)((vlblControlHeight - sizef.Height) * 0.5f);
                DrawText(e.Graphics, this._text, new Point(Math.Max(0, pX), Math.Max(0, pY)), this.ForeColor);
            }
        }
        private void DrawText(Graphics g, string text, Point pos, Color colour) {
            //g.Clear(Color.Transparent);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(colour)) {
                // 
                //Point txtPoint = Point.Empty;
                //// txtPoint是绘制文字的定位点
                //using(SolidBrush backBrush = new SolidBrush(Color.Gray)) {
                //    txtPoint.Offset(-1, 0);  // 绘制左背景文字
                //    g.DrawString(text, font, backBrush, txtPoint);
                //    txtPoint.Offset(2, 0);  // 绘制右背景文字
                //    g.DrawString(text, font, backBrush, txtPoint);
                //    txtPoint.Offset(-1, -1);  // 绘制下背景文字
                //    g.DrawString(text, font, backBrush, txtPoint);
                //    txtPoint.Offset(0, 2);  // 绘制上背景文字
                //    g.DrawString(text, font, backBrush, txtPoint);
                //    txtPoint.Offset(0, -1);  // 定位点归位
                //}
                ////// 绘制前景文字
                //g.DrawString(text, font, brush, txtPoint);
                Font font = this.Font;
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Winding)) {
                    path.AddString(text, font.FontFamily, (int)FontStyle.Bold, font.Size, pos, StringFormat.GenericTypographic);
                    g.DrawPath(Pens.WhiteSmoke, path);
                    g.FillPath(brush, path);
                }
            }
        }
        /// <summary>
        /// 不支持双缓冲
        /// </summary>
        protected override CreateParams CreateParams//v1.10 
        {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;  // 开启 WS_EX_TRANSPARENT,使控件支持透明
                return cp;
            }
        }

        [Category("myOpaqueLayer"), Description("是否完全透明,默认为True")]
        public bool TransparentBG {
            get {
                return _transparentBG;
            }
            set {
                _transparentBG = value;
                this.Invalidate();
            }
        }

        [Category("myOpaqueLayer"), Description("设置透明度")]
        public int Alpha {
            get {
                return _alpha;
            }
            set {
                _alpha = value;
                this.Invalidate();
            }
        }
        [Category("myOpaqueLayer"), Description("是否显示文本,默认为True")]
        public bool ShowText {
            get {
                return _showText;
            }
            set {
                _showText = value;
                this.Invalidate();
            }
        }
        [Category("myOpaqueLayer"), Description("显示文本内容")]
        public string TextDisplay {
            get {
                return _text;
            }
            set {
                _text = value;
                this.Invalidate();
            }
        }
        //protected override void OnSizeChanged(EventArgs e) {
        //    base.OnSizeChanged(e);
        //    this.Invalidate();
        //}
        //protected override void OnAutoSizeChanged(EventArgs e) {
        //    base.OnAutoSizeChanged(e);
        //    this.Invalidate();
        //}
        //protected override void OnResize(EventArgs e) {
        //    base.OnResize(e);
        //    this.Invalidate();
        //}

        //protected override void OnClick(EventArgs e) {
        //    base.OnClick(e);
        //    this.Invalidate();
        //}
        //private System.ComponentModel.Container components = new System.ComponentModel.Container();
        //protected override void Dispose(bool disposing) {
        //    if(disposing) {
        //        if(!((components == null))) {
        //            components.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}


        //protected override void WndProc(ref Message m) {
        //    base.WndProc(ref m);
        //    Console.WriteLine(this.Name + "OLayer:" + m.ToString());
        //    //if(m.Msg == WM_NCPAINT || m.Msg == WM_PAINT || m.Msg == WM_CTLCOLOREDIT
        //    //   || m.Msg == 7 || m.Msg == 8
        //    //   //&& BorderColor != Color.Transparent &&
        //    //   //BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D
        //    //   ) {
        //    //}
        //    if(m.Msg == 0x85) {
        //        doREdrawForm();
        //    }
        //}

        //private void doREdrawForm() {
        //    var frm = this.FindForm();
        //    if(frm != null) {
        //        frm.Invalidate();// frm.RectangleToClient(this.RectangleToScreen(this.ClientRectangle)), false);
        //    }
        //}
        //int WM_SETCURSOR = 0x20;
        //int WM_SETFOCUS = 0x7;

        //myOpaqueLayerCtl2_LoginOLayer:msg=0x281 (WM_IME_SETCONTEXT) hwnd=0xd21520 wparam=0x1 lparam=0xffffffffc000000f result=0x0
        //myOpaqueLayerCtl2_LoginOLayer:msg=0x7 (WM_SETFOCUS) hwnd=0xd21520 wparam=0x1e42d4 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x85 (WM_NCPAINT) hwnd=0x4442d0 wparam=0x680490a8 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xe (WM_GETTEXTLENGTH) hwnd=0x4442d0 wparam=0x0 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xd (WM_GETTEXT) hwnd=0x4442d0 wparam=0x2 lparam=0x3dd528 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x14 (WM_ERASEBKGND) hwnd=0x4442d0 wparam=0xffffffffbf0160ff lparam=0x0 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x20 (WM_SETCURSOR) hwnd=0x4442d0 wparam=0x4442d0 lparam=0x2000001 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xc1b8 hwnd=0x4442d0 wparam=0x0 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x200 (WM_MOUSEMOVE) hwnd=0x4442d0 wparam=0x0 lparam=0x15304b7 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x169fd37 result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x20 (WM_SETCURSOR) hwnd=0x4442d0 wparam=0x4442d0 lparam=0x2000001 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x200 (WM_MOUSEMOVE) hwnd=0x4442d0 wparam=0x0 lparam=0x15304b7 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xe (WM_GETTEXTLENGTH) hwnd=0x4442d0 wparam=0x0 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xd (WM_GETTEXT) hwnd=0x4442d0 wparam=0x2 lparam=0x3de51c result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0xf (WM_PAINT) hwnd=0x4442d0 wparam=0x0 lparam=0x0 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x2a1 (WM_MOUSEHOVER) hwnd=0x4442d0 wparam=0x0 lparam=0x15304b7 result=0x0
        //线程 0x77d8 已退出，返回值为 0 (0x0)。
        //线程 0x66d4 已退出，返回值为 0 (0x0)。
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x177fd2c result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x177fd2c result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x177fd2c result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x84 (WM_NCHITTEST) hwnd=0x4442d0 wparam=0x0 lparam=0x177fd2c result=0x1
        //myOpaqueLayerCtl1_BackOLayer:msg=0x20 (WM_SETCURSOR) hwnd=0x4442d0 wparam=0x4442d0 lparam=0x2000001 result=0x0
        //myOpaqueLayerCtl1_BackOLayer:msg=0x200 (WM_MOUSEMOVE) hwnd=0x4442d0 wparam=0x0 lparam=0x16104ac result=0x0
        #region designer
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
        #endregion
    }
}
