using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public class FormShadow : Form
    {

        public FormShadow() : base() {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        /// <summary>
        /// 界面加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
            //dwmInitialize();
            base.OnLoad(e);

        }


        protected bool dwmEnabled=true;
        protected int dwmleft=4;
        protected int dwmtop=1;
        protected int dwmright=4;
        protected int dwmbottom=4;
   
        private const int WS_MINIMIZEBOX = 0x00020000;
        /// <summary>
        /// 界面绘制
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);


            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (dwmEnabled) {
                e.Graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                DrawShadow(this.Size, dwmleft, e.Graphics);
            }
            RectangleF rect = new RectangleF(dwmleft - 0.5f, dwmtop - 0.5f, this.Width - dwmleft - dwmright + 0.5f, this.Height - dwmtop - dwmbottom + 0.5f);
            SolidBrush brush = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(brush, rect);
            brush.Dispose(); brush = null;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;
                return cp;
            }
        }
        /// <summary>
        /// 绘制阴影效果
        /// </summary>
        /// <param name="size">控件尺寸</param>
        /// <param name="radius">阴影半径</param>
        /// <param name="g">绘图区</param>
        private void DrawShadow(Size size, int radius, Graphics g) {
            if (radius <= 0) return;
            int d = 2 * radius;
            #region[LinearGradientBrush]   
            ColorBlend blend = new ColorBlend();
            blend.Colors = new Color[] { Color.FromArgb(100, Color.Black), Color.FromArgb(40, Color.Black), Color.FromArgb(0, Color.Black) };
            blend.Positions = new float[] { 0, 0.4f, 1 };
            LinearGradientBrush brushleft = new LinearGradientBrush(new Point(radius, 0), new Point(0, 0), Color.FromArgb(60, Color.Black), Color.FromArgb(0, Color.Black));
            brushleft.InterpolationColors = blend;
            LinearGradientBrush brushright = new LinearGradientBrush(new Point(size.Width - radius - 1, 0), new Point(size.Width, 0), Color.FromArgb(80, Color.Black), Color.FromArgb(0, Color.Black));
            brushright.InterpolationColors = blend;
            LinearGradientBrush brushtop = new LinearGradientBrush(new Point(0, radius), new Point(0, 0), Color.FromArgb(60, Color.Black), Color.FromArgb(0, Color.Black));
            brushtop.InterpolationColors = blend;
            LinearGradientBrush brushbottom = new LinearGradientBrush(new Point(0, size.Height - radius - 1), new Point(0, size.Height), Color.FromArgb(80, Color.Black), Color.FromArgb(0, Color.Black));
            brushbottom.InterpolationColors = blend;
            #endregion
            #region[path]
            GraphicsPath pathlefttop = new GraphicsPath();
            pathlefttop.AddPie(new Rectangle(0, 0, d, d), 180, 90);
            GraphicsPath pathrighttop = new GraphicsPath();
            pathrighttop.AddPie(new Rectangle(this.Width - d, 0, d, d), 270, 90);
            GraphicsPath pathleftbottom = new GraphicsPath();
            pathleftbottom.AddPie(new Rectangle(0, this.Height - d, d, d), 90, 90);
            GraphicsPath pathrightbottom = new GraphicsPath();
            pathrightbottom.AddPie(new Rectangle(this.Width - d, this.Height - d, d, d), 0, 90);
            #endregion
            #region[PathGradientBrush]
            PathGradientBrush brushlefttop = new PathGradientBrush(pathlefttop);
            brushlefttop.CenterPoint = new Point(radius, radius);
            brushlefttop.CenterColor = Color.FromArgb(80, Color.Black);
            brushlefttop.SurroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
            //brushlefttop.InterpolationColors = blend;
            PathGradientBrush brushrighttop = new PathGradientBrush(pathrighttop);
            brushrighttop.CenterPoint = new Point(this.Width - radius, radius);
            brushrighttop.CenterColor = Color.FromArgb(80, Color.Black);
            brushrighttop.SurroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
            //brushrighttop.InterpolationColors = blend;
            PathGradientBrush brushleftbottom = new PathGradientBrush(pathleftbottom);
            brushleftbottom.CenterPoint = new Point(radius, this.Height - radius);
            brushleftbottom.CenterColor = Color.FromArgb(80, Color.Black);
            brushleftbottom.SurroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
            //brushleftbottom.InterpolationColors = blend;
            PathGradientBrush brushrightbottom = new PathGradientBrush(pathrightbottom);
            brushrightbottom.CenterPoint = new Point(this.Width - radius, this.Height - radius);
            brushrightbottom.CenterColor = Color.FromArgb(80, Color.Black);
            brushrightbottom.SurroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
            //brushrightbottom.InterpolationColors = blend;
            #endregion
            #region[draw]
            g.FillRectangle(brushleft, new RectangleF(1, radius - 0.5f, radius, this.Height - d + 0.5f));
            g.FillRectangle(brushright, new RectangleF(this.Width - radius - 1, radius - 0.5f, radius, this.Height - d + 0.5f));
            g.FillRectangle(brushtop, new RectangleF(radius - 0.5f, 0, this.Width - d + 0.5f, radius));
            g.FillRectangle(brushbottom, new RectangleF(radius - 0.5f, this.Height - radius - 1, this.Width - d + 0.5f, radius));
            g.FillPath(brushlefttop, pathlefttop);
            g.FillPath(brushrighttop, pathrighttop);
            g.FillPath(brushleftbottom, pathleftbottom);
            g.FillPath(brushrightbottom, pathrightbottom);
            #endregion
            #region[dispose]
            brushleft.Dispose(); brushleft = null;
            brushright.Dispose(); brushright = null;
            brushtop.Dispose(); brushtop = null;
            brushbottom.Dispose(); brushbottom = null;
            pathlefttop.Dispose(); pathlefttop = null;
            pathrighttop.Dispose(); pathrighttop = null;
            pathleftbottom.Dispose(); pathleftbottom = null;
            pathrightbottom.Dispose(); pathrightbottom = null;
            brushlefttop.Dispose(); brushlefttop = null;
            brushrighttop.Dispose(); brushrighttop = null;
            brushleftbottom.Dispose(); brushleftbottom = null;
            brushrightbottom.Dispose(); brushrightbottom = null;
            #endregion
        }
    
        /// <summary>
        /// dwm初始化
        /// </summary>
        private void dwmInitialize() {
            #region[dwmInitialize]
            this.dwmEnabled = true;
            dwmleft = this.Padding.Left;
            dwmtop = this.Padding.Top;
            dwmright = this.Padding.Right;
            dwmbottom = this.Padding.Bottom;
            int flag = 0;
            MARGINS mg = new MARGINS();
            mg.m_Left = dwmleft;
            mg.m_Top = dwmtop;
            mg.m_Right = dwmright;
            mg.m_Bottom = dwmbottom;
            //判断Vista系统
            if (System.Environment.OSVersion.Version.Major >= 6) {
                DwmIsCompositionEnabled(ref flag);    //检测Aero是否为打开
                if (flag > 0) {
                    DwmExtendFrameIntoClientArea(this.Handle, ref mg);
                }
                else {
                    dwmEnabled = false;
                    dwmleft = 0;
                    dwmtop = 0;
                    dwmright = 0;
                    dwmbottom = 0;
                    //MessageBox.Show("Desktop Composition is Disabled!");
                }
            }
            else {
                dwmEnabled = false;
                dwmleft = 0;
                dwmtop = 0;
                dwmright = 0;
                dwmbottom = 0;
                //MessageBox.Show("Please run this on Windows Vista.");
            }
            GC.Collect();
            #endregion
        }



        /// <summary>
        /// 
        /// </summary>
        public struct MARGINS
        {
            public int m_Left;
            public int m_Right;
            public int m_Top;
            public int m_Bottom;
        };
        [DllImport("dwmapi.dll")]
        private static extern void DwmIsCompositionEnabled(ref int enabledptr);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();
        [DllImport("dwmapi.dll")]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margin);

        private void InitializeComponent() {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormShadow
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Name = "FormShadow";
            this.ResumeLayout(false);

        }
        private Button button1;
        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }


        /* 使用系统API实现窗口阴影，效果不好，影响其他窗体
        public FormShadow()
        {
            InitializeComponent();
            SetClassLong(this.Handle, GCL_STYLE, GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果
        }
        #region 窗体边框阴影效果变量申明

        const int CS_DropSHADOW = 0x20000;
        const int GCL_STYLE = (-26);
        //声明Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
         
         */
    }
}
