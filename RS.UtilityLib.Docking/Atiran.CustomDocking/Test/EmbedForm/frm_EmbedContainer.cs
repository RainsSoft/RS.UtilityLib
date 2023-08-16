using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Atiran.CustomDocking.Docking;
using Atiran.CustomDocking.Desk;


namespace Test.EmbedForm
{
    /// <summary>
    /// 嵌入窗口容器，内部可嵌入进程窗口
    /// </summary>
    public partial class frm_EmbedContainer : DeskMain
    {
        //MainMenu m_MainMenu;
        ToolStripMenuItem m_FileMenuItem;
        private frm_EmbedContainer() : base() {
            InitializeComponent();
            //m_MainMenu = new MainMenu(this.components);
            //m_MainMenu.MenuItems.Add("扩展工具");
            //this.Menu = m_MainMenu;
            this.IsMdiContainer = false;
            this.m_SizeOriginal = this.Size;
            this.m_LocationOriginal = this.Location;
            // 
            //this.dockPanel1.AllowEndUserDocking = false;
            SetTheme(VisualStudioToolStripExtender.VsVersion.Vs2017, this.vS2017LightTheme1);

            //
            m_DriverProcess = new EmbedProcess2Container(this);


        }

        private void Instance_OnWorldLoadEnd(object sender, EventArgs e) {
            InitMenu();

        }

        private void InitMenu() {
            if (m_FileMenuItem != null) {
                return;
            }
            m_MainMenu.Visible = false;
            m_FileMenuItem = this.ToolStripMenuItem_扩展工具;//m_MainMenu.Items[0] as ToolStripMenuItem;
            m_FileMenuItem.Enabled = false;

            this.m_MainMenu.Visible = true;
            this.m_FileMenuItem.Enabled = true;
            this.ToolStripMenuItem_Web程序.Visible = false;
            this.ToolStripMenuItem_设备程序.Visible = false;

        }
    
#if DEBUG
        protected override void OnClosed(EventArgs e) {
            //
            base.OnClosed(e);
            //
        }
        protected override void OnClosing(CancelEventArgs e) {
            //
            //MessageBox.Show("");
            base.OnClosing(e);
        }
#endif
      
        #region API Native Methods 

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;
        static bool Environment_Is64BitProcess() {
            bool is64BitProcess = (IntPtr.Size == 8);
            return is64BitProcess;
        }
        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) {
            return Environment_Is64BitProcess()
                ? GetWindowLong64(hWnd, nIndex)
                : GetWindowLong32(hWnd, nIndex);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
            return Environment_Is64BitProcess()
                ? SetWindowLong64(hWnd, nIndex, dwNewLong)
                : SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        #endregion
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            //本窗口不要获得焦点
            // NoFocusThisWindow();
        }

        /// <summary>
        /// 当前窗口不获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoFocusThisWindow() {

            //int GWL_STYLE = (-16);
            //int WS_VISIBLE = 0x10000000 | 0x00800000 | 0x02000000 | 0x01000000;
            //int WS_VISIBLE2 = 0x40000000 | 0x00800000 | 0x02000000 | 0x01000000 | 0x10000000;  
            var handle = this.Handle;//new WindowInteropHelper(this).Handle;
            int WS_EX_NOACTIVATE = 0x08000000;
            int GWL_EXSTYLE = -20;
            var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE));
        }
        //private const int WS_EX_NOACTIVATE = 0x08000000;
        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams ps = base.CreateParams;
        //        ps.ExStyle |= WS_EX_NOACTIVATE;
        //        return (ps);
        //    }
        //}
        //protected override bool ShowWithoutActivation {
        //    get {
        //        return true;
        //    }
        //}

        #region 屏幕缩放
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        /// <summary>
        /// 获取屏幕缩放值
        /// </summary>
        /// <returns></returns>
        static int getSysDisplayScale() {
            try {
                //int major, minor;
                //OSVersionHelper.GetVersion(out major, out minor);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero)) {
                    IntPtr hdc = g.GetHdc();
                    //LOGPIXELSX 88
                    //int logicx_dpi = GetDeviceCaps(hdc, 88);
                    // int y = GetDeviceCaps(hdc, 90);
                    //VERTRES 10
                    int LogicalScreenHeight = GetDeviceCaps(hdc, 10);
                    //DESKTOPVERTRES  117
                    int PhysicalScreenHeight = GetDeviceCaps(hdc, 117);
                    //DESKTOPHORZRES	118
                    int PhysicalScreenWidth = GetDeviceCaps(hdc, 118);
                    float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
                    g.ReleaseHdc();
                    //SystemInfo ret = new SystemInfo {
                    //    Width = PhysicalScreenWidth,
                    //    Height = PhysicalScreenHeight,
                    //    Scale = (int)(ScreenScalingFactor * 100),
                    //    Major = major,
                    //    Minor = minor
                    //};
                    int displayScale = (int)(ScreenScalingFactor * 100);
                    return displayScale;
                }
            }
            catch (Exception ee) {
                Console.WriteLine(ee.ToString());
            }
            return 100;
        }
        static int m_displayScale = 100;
        #endregion

        #region 跟随移动

        /// <summary>
        /// 跟随模式
        /// </summary>
        public enum FollowMode
        {
            None,
            /// <summary>
            /// 右侧跟随
            /// </summary>
            Right,
            /// <summary>
            /// 下方跟随
            /// </summary>
            Down
        }
        /// <summary>
        /// 位置计算方式
        /// </summary>
        public enum FollowOffsetMode
        {
            /// <summary>
            /// 固定计算
            /// </summary>
            Fixed,
            /// <summary>
            /// 偏移计算
            /// </summary>
            Relative
        }
        /// <summary>
        /// 跟随主窗口模式
        /// </summary>
        public static FollowMode m_FollowMode {
            get;
            private set;
        }
        public static FollowOffsetMode m_FollowOffsetMode {
            get;
            private set;
        }
        #region 相对位置
        /// <summary>
        /// 父容器前一次位置，用于相对跟随移动
        /// </summary>
        private static Point m_OwnerFrm_PrevPos = new Point(0, 0);
        private static Point m_OwnerFrm_LocationOriginal = new Point(0, 0);
        /// <summary>
        /// 当前窗口初始初始
        /// </summary>
        private Size m_SizeOriginal = new Size(1, 1);
        /// <summary>
        /// 当前窗口初始位置
        /// </summary>
        private Point m_LocationOriginal = new Point(0, 0);
        #endregion

        #region FollowMode
        static bool noNeedFollow = false;
        static bool inFollowing = false;
        static int followDistance = 128;
        /// <summary>
        /// invoke模式检测是否需要跟随，请不要invoke嵌套造成死锁
        /// </summary>
        /// <returns></returns>
        static bool checkNeedFollow() {
            if (m_frmOwner.CheckHasCreatedControl() == false ||
                m_frm.CheckHasCreatedControl() == false) {
                return false;
            }
            bool safeCall = false;
            if (safeCall) {
                //todo:检查退出仿真返回小组仿真执行到这里的死锁问题
                FormWindowState frmWindowState = m_frm.SafeFunc<FormWindowState>(() => m_frm.WindowState, FormWindowState.Minimized);
                if (frmWindowState != FormWindowState.Normal)
                    return false;
                FormWindowState frmOwnerWindowState = m_frmOwner.SafeFunc<FormWindowState>(() => m_frmOwner.WindowState, FormWindowState.Minimized);
                if (frmWindowState != FormWindowState.Normal) {
                    return false;
                }
            }
            else {
                FormWindowState frmWindowState = m_frm.WindowState;
                FormWindowState frmOwnerWindowState = m_frmOwner.WindowState;
                if (frmWindowState != FormWindowState.Normal || frmOwnerWindowState != FormWindowState.Normal) {
                    return false;
                }
            }
            return true;
            Point ownerPos = Point.Empty;
            int ownerWidth = 1;
            int ownerHeight = 1;
            m_frmOwner.SafeAction(() => {
                ownerPos = m_frmOwner.Location;
                ownerWidth = m_frmOwner.Width;
                ownerHeight = m_frmOwner.Height;
            });
            Point thisPos = m_frm.SafeFunc<Point>(() => m_frm.Location, Point.Empty);
            //
            if (m_FollowMode == FollowMode.Right) {
                if (Math.Abs(ownerPos.X + ownerWidth - thisPos.X) < followDistance
                    //&& Math.Abs(ownerPos.Y - thisPos.Y) < followDistance
                    ) {
                    return true;
                }
            }
            else if (m_FollowMode == FollowMode.Down) {
                if (Math.Abs(ownerPos.Y + ownerHeight - thisPos.Y) < followDistance
                    //&&Math.Abs(ownerPos.X - thisPos.X) < followDistance
                    ) {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// invoke模式跟随，请不要invoke嵌套造成死锁
        /// </summary>
        static void FollowOwnerFrm() {
            inFollowing = true;
            //当前执行在渲染主线程
            if (m_FollowMode == FollowMode.Right) {
                Point ownerPos = Point.Empty;
                int ownerWidth = 8;
                int ownerHeight = 8;
                //m_frmOwner.SafeAction(() => {
                ownerPos = m_frmOwner.Location;
                ownerWidth = m_frmOwner.Width;
                ownerHeight = m_frmOwner.Height;
                //});
                m_frm.SafeAction(() => {
                    m_frm.Location = new System.Drawing.Point(
                     ownerPos.X + ownerWidth - 8,
                     ownerPos.Y
                    );
                }, true);

            }
            else if (m_FollowMode == FollowMode.Down) {
                Point ownerPos = Point.Empty;
                int ownerWidth = 8;
                int ownerHeight = 8;
                //m_frmOwner.SafeAction(() => {
                ownerPos = m_frmOwner.Location;
                ownerWidth = m_frmOwner.Width;
                ownerHeight = m_frmOwner.Height;
                //});
                m_frm.SafeAction(() => {
                    m_frm.Location = new System.Drawing.Point(
                      ownerPos.X,
                      ownerPos.Y + ownerHeight
                      );
                }, true);

            }
            inFollowing = false;
        }
        static void SetupOffset(Point ownerStartLocation) {
            //Point location = m_frmOwner.SafeFunc<Point>(() => m_frmOwner.Location, m_prev_pos);            
            m_OwnerFrm_PrevPos = ownerStartLocation;
            m_OwnerFrm_LocationOriginal = ownerStartLocation;
        }
        static void FollowOwnerFrmByOffset() {
            //当前执行在渲染主线程
            if (m_FollowMode != FollowMode.None) {
                inFollowing = true;
                bool safeCall = true;
                if (safeCall) {
                    //Point location = m_frmOwner.SafeFunc<Point>(() => m_frmOwner.Location, m_OwnerFrm_PrevPos);           
                    Point location = m_frmOwner.Location;
                    int offset_x = location.X - m_OwnerFrm_PrevPos.X;
                    int offset_y = location.Y - m_OwnerFrm_PrevPos.Y;
                    m_OwnerFrm_PrevPos = location;
                    m_frm.SafeAction(() => {
                        m_frm.Location = new Point(m_frm.Location.X + offset_x, m_frm.Location.Y + offset_y);
                    }, true);
                }
                else {
                    Point location = m_frmOwner.Location;
                    var offset_x = location.X - m_OwnerFrm_PrevPos.X;
                    var offset_y = location.Y - m_OwnerFrm_PrevPos.Y;
                    m_OwnerFrm_PrevPos = location;
                    m_frm.Location = new Point(m_frm.Location.X + offset_x, m_frm.Location.Y + offset_y);
                }
                inFollowing = false;
            }
        }
        #endregion



        private static Form m_frmOwner;
        //private static bool m_needBringFrontEmbedContainer = false;
        #region form owner       
        /// <summary>
        /// invoke模式，请不要invoke嵌套造成死锁
        /// </summary>
        private static void HookOwnerFormStateEvent() {
            if (m_frmOwner != null) {
                m_frmOwner.SafeAction(() => {
                    //OgreSystem.Singleton.GetRenderRoot().FrameEnded += RenderRoot_FrameEnd;
                    m_frmOwner.LocationChanged -= M_frmOwner_LocationChanged;
                    //m_frmOwner.SizeChanged -= M_frmOwner_SizeChanged;
                    ////m_frmOwner.AutoSizeChanged -= M_frmOwner_AutoSizeChanged;
                    m_frmOwner.Resize -= M_frmOwner_Resize;
                    //
                    m_frmOwner.LocationChanged += M_frmOwner_LocationChanged;
                    //m_frmOwner.SizeChanged += M_frmOwner_SizeChanged;
                    ////m_frmOwner.AutoSizeChanged += M_frmOwner_AutoSizeChanged;
                    m_frmOwner.Resize += M_frmOwner_Resize;
                    //
                    //m_frmOwner.Activated -= M_frmOwner_Activated;
                    //m_frmOwner.Activated += M_frmOwner_Activated;
                    //m_frmOwner.Deactivate-= M_frmOwner_Deactivate;
                    //m_frmOwner.Deactivate += M_frmOwner_Deactivate;
                    //m_frmOwner.GotFocus -= M_frmOwner_GotFocus;
                    //m_frmOwner.GotFocus += M_frmOwner_GotFocus;
                    //m_frmOwner.LostFocus -= M_frmOwner_LostFocus;
                    //m_frmOwner.LostFocus += M_frmOwner_LostFocus;
                    m_frmOwner.MouseCaptureChanged -= M_frmOwner_MouseCaptureChanged;
                    m_frmOwner.MouseCaptureChanged += M_frmOwner_MouseCaptureChanged;
                }, true);
            }
        }

        private static void M_frmOwner_MouseCaptureChanged(object sender, EventArgs e) {
            return;
            //解决点击桌面其他窗口，点任务栏还原显示当前仿真窗口时，本窗口会在桌面窗口后面，
            //点击当前仿真窗口时，就会把本窗口显示出来
            m_frm.SafeAction(() => {
                m_frm.TopMost = true;
                System.Threading.Thread.Sleep(10);
                m_frm.TopMost = false;
                Console.WriteLine("MouseCaptureChanged0000");
            }, true);
        }
        /*
        private static void M_frmOwner_GotFocus(object sender, EventArgs e) {
            m_frm.SafeAction(()=> {
                m_frm.TopMost = true;
                Console.WriteLine("GotFocus0000");
            },true);
        }
        private static void M_frmOwner_LostFocus(object sender, EventArgs e) {
            m_frm.SafeAction(() => {
                m_frm.TopMost = false;
                Console.WriteLine("LostFocus0000");
            }, true);
        }
        private static void M_frmOwner_Activated(object sender, EventArgs e) {
            m_frm.SafeAction(() => {
                m_frm.TopMost = true;
                Console.WriteLine("Activated0000");
            }, true);
        }
        private static void M_frmOwner_Deactivate(object sender, EventArgs e) {
            m_frm.SafeAction(() => {
                m_frm.TopMost = false;
                Console.WriteLine("Deactivate0000");
            }, true);
        }
        */
        private static void M_frmOwner_Resize(object sender, EventArgs e) {
            //Console.WriteLine("Resize");
            //LocationChanged
            //Resize
            //SizeChanged            
            //根据owner状态显示或者隐藏
            var wState = (sender as Form).WindowState;
            m_frm.SafeAction(() => {
                if (wState == FormWindowState.Minimized) {
                    m_frm.Visible = false;
                }
                else {
                    // m_frm.WindowState = FormWindowState.Normal;
                    //解决仿真窗口最小化状态还原到普通状态时，本窗口没有显示到前面的问题
                    if (m_frm.Visible == false) {
                        m_frm.Visible = true;
                        //m_frm.Activate();
                        //MessageBox.Show("saaa");
                    }
                }
            }, true);
            //
            if (inFollowing || noNeedFollow) {
                return;
            }
            if (checkNeedFollow() == false) {
                return;
            }
            if (m_FollowOffsetMode == FollowOffsetMode.Fixed) {
                FollowOwnerFrm();
            }
            else {
                FollowOwnerFrmByOffset();
            }
        }

        //private static void M_frmOwner_AutoSizeChanged(object sender, EventArgs e) {
        //    //不会触发
        //    Console.WriteLine("AutoSizeChanged");
        //}

        //private static void M_frmOwner_SizeChanged(object sender, EventArgs e) {
        //    Console.WriteLine("SizeChanged");
        //}

        private static void M_frmOwner_LocationChanged(object sender, EventArgs e) {
            // Console.WriteLine("LocationChanged");
            //LocationChanged
            //Resize
            //SizeChanged
            if (inFollowing || noNeedFollow) {
                return;
            }
            if (checkNeedFollow() == false) {
                return;
            }
            if (m_FollowOffsetMode == FollowOffsetMode.Fixed) {
                FollowOwnerFrm();
            }
            else {
                FollowOwnerFrmByOffset();
            }
        }
        /// <summary>
        /// invoke模式，请不要invoke嵌套造成死锁
        /// </summary>
        private static void UnHookOwnerFormStateEvent() {
            if (m_frmOwner != null) {
                m_frmOwner.SafeAction(() => {
                    //OgreSystem.Singleton.GetRenderRoot().FrameEnded -= RenderRoot_FrameEnd;
                    m_frmOwner.LocationChanged -= M_frmOwner_LocationChanged;
                    //m_frmOwner.SizeChanged -= M_frmOwner_SizeChanged;
                    ////m_frmOwner.AutoSizeChanged -= M_frmOwner_AutoSizeChanged;
                    m_frmOwner.Resize -= M_frmOwner_Resize;
                    //m_frmOwner.GotFocus -= M_frmOwner_GotFocus;
                    //m_frmOwner.LostFocus -= M_frmOwner_LostFocus;
                    //m_frmOwner.Activated -= M_frmOwner_Activated;
                    //m_frmOwner.Deactivate -= M_frmOwner_Deactivate;
                    m_frmOwner.MouseCaptureChanged -= M_frmOwner_MouseCaptureChanged;
                }, true);
            }
        }
        #endregion
        internal static DeskMain GetSingleton() {
            return m_frm;
        }
        private static frm_EmbedContainer m_frm;
        internal static void CloseContainer() {
            UnHookOwnerFormStateEvent();
            m_frm.SafeAction(() => {
                //解决默写情况下弹出窗口导致的死锁问题
                m_frm.needClose = true;
            }, true);
        }
        internal static void SetQuestionMenuItemEnable(bool enable) {
            //解决某些情况下，有弹出窗口导致卡住的情况
            Action ac = () => {
                m_frm.ToolStripMenuItem_Web程序.Enabled = enable;
                m_frm.ToolStripMenuItem_设备程序.Enabled = enable;
            };
            m_frm.UnSafeAction(ac);
        }
        /// <summary>
        /// 显示容器
        /// </summary>
        /// <param name="followMode"></param>
        internal static void ShowContainer(Form owner,FollowMode followMode = FollowMode.Right, FollowOffsetMode offsetMode = FollowOffsetMode.Relative) {
            if (m_frm.CheckHasCreatedControl()) {
                return;
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            //m_displayScale = getSysDisplayScale();
            m_FollowMode = followMode;
            m_FollowOffsetMode = offsetMode;
            m_frmOwner = owner;
#if DEBUG
            //RunForm();
            //return;
#endif
            //开启线程
            var m_frmThread = new Thread(RunForm);
            //
            m_frmThread.Name = "frm_EmbedContainer";
            m_frmThread.SetApartmentState(ApartmentState.STA);
            m_frmThread.IsBackground = true;
            m_frmThread.Start();
            //
            for (int i = 0; i < 3; i++) {
                System.Threading.Thread.Sleep(20);
                Application.DoEvents();
            }

            //p_frm.Focus();
        }
        #region 开线程运行容器窗口
        static void RunForm() {
            try {
                if (m_FollowMode == FollowMode.Right) {
                    RunForm_FollowRight();
                }
                else if (m_FollowMode == FollowMode.Down) {
                    RunForm_FollowDown();
                }
                else {
                    //Todo:
                }
            }
            catch (Exception ee) {
                Console.WriteLine(ee.ToString());
            }
            m_frm = null;
        }
        static void RunForm_FollowRight() {
            frm_EmbedContainer frm = new frm_EmbedContainer();
            frm.WindowState = FormWindowState.Normal;
            frm.StartPosition = FormStartPosition.Manual;
            //frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MinimizeBox = false;
            //frm.MaximizeBox = false;
            frm.Owner = m_frmOwner;
            //
            Point ownerOldPos = Point.Empty;
            int ownerWidth = 8;
            int ownerHeight = 8;
            m_frmOwner.SafeAction(() => {
                ownerOldPos = m_frmOwner.Location;
                ownerWidth = m_frmOwner.Width;
                ownerHeight = m_frmOwner.Height;
            });
            //计算窗口需要的大小
            int frmWidth = CaculDWByScreenWith();
            frm.m_SizeOriginal = new Size(frmWidth, ownerHeight);
            frm.Size = new Size(frmWidth, ownerHeight);
            frm.MinimumSize = new Size(300, ownerHeight);
            // frm.MaximumSize = new Size(Math.Max(300, frmWidth + ownerWidth / 2), ownerHeight);
            //计算位置 owner向左推                    
            int allW = ownerWidth + frmWidth;
            int allH = ownerHeight;//+ 230;
            int sW = 0;
            int sH = 0;
            noNeedFollow = true;
            m_frmOwner.SafeAction(() => {
                sW = Screen.FromControl(m_frmOwner).WorkingArea.Width;
                sH = Screen.FromControl(m_frmOwner).WorkingArea.Height;

            });
            int ox = Math.Max(0, (sW - allW) / 2);
            int oy = Math.Max(0, (sH - allH) / 2);
            m_frmOwner.SafeAction(() => {
                m_frmOwner.Location = new Point(ox, oy);
            }, true);
            SetupOffset(new Point(ox, oy));
            //
            //Application.DoEvents();
            //
            frm.m_LocationOriginal = new Point(ox + ownerWidth - 8, oy);
            frm.Location = new Point(ox + ownerWidth - 8, oy);
            m_frm = frm;
            noNeedFollow = false;
            //#if DEBUG
            //            frm.Show();
            //            return;
            //#endif
            Application.Run(frm);

            ////把主窗体回退到指定位置
            //m_frmOwner.SafeAction(()=> {
            //    m_frmOwner.Location = ownerOldPos;
            //});
            //计算位置 owner 回退到中间                 
            allW = ownerWidth;
            allH = ownerHeight;// + 230;
            m_frmOwner.SafeAction(() => {
                sW = Screen.FromControl(m_frmOwner).WorkingArea.Width;
                sH = Screen.FromControl(m_frmOwner).WorkingArea.Height;
                ox = Math.Max(0, (sW - allW) / 2);
                oy = Math.Max(0, (sH - allH) / 2);
                m_frmOwner.Location = new Point(ox, oy);
                //Application.DoEvents();
            });
            //完成后退出该线程消息循环
            //Application.ExitThread();

        }
        /// <summary>
        /// 根据屏幕计算 当前窗口需要展现的宽度.
        /// invoke模式，请不要invoke嵌套造成死锁
        /// </summary>
        /// <returns></returns>
        public static int CaculDWByScreenWith() {
            //float r = 550f / m_frmOwner.Width;
            int sw = m_frmOwner.SafeFunc<int>(() => Screen.FromControl(m_frmOwner).Bounds.Width - m_frmOwner.Width, 0);
            //
            sw = System.Math.Max(300, sw);
            sw = System.Math.Min(1000, sw);
            return sw;
        }
        /// <summary>
        /// 根据屏幕计算 当前窗口需要展现的高度.
        /// invoke模式，请不要invoke嵌套造成死锁
        /// </summary>
        /// <returns></returns>
        public static int CaculDHByScreenHeight() {
            //float r = 550f / m_frmOwner.Width;
            int sw = m_frmOwner.SafeFunc<int>(() => Screen.FromControl(m_frmOwner).Bounds.Height - m_frmOwner.Height, 0);
            //
            sw = System.Math.Max(100, sw);
            sw = System.Math.Min(400, sw);
            return sw;
        }
        static void RunForm_FollowDown() {
            frm_EmbedContainer frm = new frm_EmbedContainer();
            frm.WindowState = FormWindowState.Normal;
            frm.StartPosition = FormStartPosition.Manual;
            //frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MinimizeBox = false;
            frm.MaximizeBox = false;
            frm.Owner = m_frmOwner;
            //
            Point ownerOldPos = Point.Empty;
            int ownerWidth = 8;
            int ownerHeight = 8;
            m_frmOwner.SafeAction(() => {
                ownerOldPos = m_frmOwner.Location;
                ownerWidth = m_frmOwner.Width;
                ownerHeight = m_frmOwner.Height;
            });
            //计算窗口需要的大小
            int frmHeight = CaculDHByScreenHeight();
            frm.m_SizeOriginal = new Size(ownerWidth, frmHeight);
            frm.Size = new Size(ownerWidth, frmHeight);
            frm.MinimumSize = new Size(ownerWidth, 100);
            frm.MaximumSize = new Size(ownerWidth, Math.Max((frmHeight + ownerHeight / 2), 100));
            //计算位置 owner向上推                    
            int allW = ownerWidth;
            int allH = ownerHeight + frmHeight;
            int sW = 0;
            int sH = 0;
            noNeedFollow = true;
            m_frmOwner.SafeAction(() => {
                sW = Screen.FromControl(m_frmOwner).WorkingArea.Width;
                sH = Screen.FromControl(m_frmOwner).WorkingArea.Height;

            });
            int ox = Math.Max(0, (sW - allW) / 2);
            int oy = Math.Max(0, (sH - allH) / 2);
            m_frmOwner.SafeAction(() => {
                m_frmOwner.Location = new Point(ox, oy);
            }, true);
            SetupOffset(new Point(ox, oy));
            //
            //Application.DoEvents();
            //      
            frm.m_LocationOriginal = new Point(ox, ownerHeight + oy);
            frm.Location = new Point(ox, ownerHeight + oy);
            m_frm = frm;
            noNeedFollow = false;
            //#if DEBUG
            //            frm.Show();
            //            return;
            //#endif
            Application.Run(frm);

            ////把主窗体回退到指定位置
            //m_frmOwner.SafeAction(()=> {
            //    m_frmOwner.Location = ownerOldPos;
            //});
            //计算位置 owner 回退到中间                 
            allW = ownerWidth;
            allH = ownerHeight;// + 230;
            m_frmOwner.SafeAction(() => {
                sW = Screen.FromControl(m_frmOwner).WorkingArea.Width;
                sH = Screen.FromControl(m_frmOwner).WorkingArea.Height;
                ox = Math.Max(0, (sW - allW) / 2);
                oy = Math.Max(0, (sH - allH) / 2);
                m_frmOwner.Location = new Point(ox, oy);
                //Application.DoEvents();
            });
            //完成后退出该线程消息循环
            //Application.ExitThread();


        }
        #endregion


        //protected override void WndProc(ref Message m) {
        //    if(EmbedWindowHelper.WndProc_DisableDBClick2ResizeWindow(ref m)) {
        //        //禁止通过拖动，双击标题栏改变窗体大小
        //        return;
        //    }
        //    base.WndProc(ref m);
        //}

        //protected override void OnShown(EventArgs e) {
        //    base.OnShown(e);
        //    //           
        //}
        protected override void CustomOnShown(EventArgs e) {
            base.CustomOnShown(e);
            //EmbedWindowHelper.RemoveFormCloseMenu(this);
            HookOwnerFormStateEvent();
            HookThisFrmStateEvent();
        }
        protected override void CustomOnVisibleChanged(EventArgs e) {
            return;
            //base.CustomOnVisibleChanged(e);
            if (Visible) {
                //没设owner的时候需要当前方法把当前窗口前置
                this.SafeAction(() => {
                    this.TopMost = true;
                    System.Threading.Thread.Sleep(10);
                    this.TopMost = false;
                }, true);

            }
        }
#if DEBUG
        /// <summary>
        /// 重写,防止子窗体与Owner不在一个线程
        /// </summary>
        public new Form Owner2 {
            get;
            set;
        }
#endif

        #endregion

        #region 自身事件
        EmbedProcess2Container m_DriverProcess;
        public override DockPanel dockPanel {
            get {
                return this.dockPanel1;
            }
        }
        public override Type DeskDocFrm {
            get {
                return typeof(DeskDriverFrm);
            }
        }
        public void CloseDocmentByProcessID(int processID) {
            for (int i = this.m_AllDeskDocSelfCreated.Count - 1; i >= 0; i--) {
                DeskDriverFrm ddf = this.m_AllDeskDocSelfCreated[i] as DeskDriverFrm;
                if (ddf != null && ddf.ProcessID == processID) {
                    try {
                        if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                            ddf.Close();
                        }
                        else {
                            ddf.DockHandler.DockPanel = null;
                            ddf.DockHandler.Close();
                        }
                    }
                    catch {

                    }
                }
            }
        }
        private DeskDriverFrm CreateAndShowDeskDriverFrm(string text) {
            var doc = this.CreateNewDocument(text) as DeskDriverFrm;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                doc.MdiParent = this;
                doc.Show();
                return doc;
            }
            if (this.dockPanel.ActiveDocument != null &&
                doc != dockPanel.ActiveDocument) {
                var adoc = dockPanel.ActiveDocument;

                if (m_FollowMode != FollowMode.None) {
                    doc.Show(adoc.DockHandler.Pane, DockAlignment.Right, 0.5);
                    //doc.DockHandler.AllowEndUserDocking = false;
                }
                else {
                    doc.Show(adoc.DockHandler.Pane, null);
                    //doc.DockHandler.AllowEndUserDocking = false;
                }

            }
            else {
                doc.Show(this.dockPanel, DockState.Document);
                //doc.DockHandler.AllowEndUserDocking = false;
            }
            return doc;
        }
        private void HookThisFrmStateEvent() {
            {
                int displayScale = getSysDisplayScale();
                if (displayScale > 170) {
                    this.Text += "(当前屏幕缩放：" + displayScale + "%,请自行调整窗口尺寸与位置)";
                }
                //OgreSystem.Singleton.ActiveRenderWindow();

            }
            this.timer1.Start();
        }

        private void UnHookThisFrmStateEvent() {
            this.timer1.Stop();

        }
        private bool needClose = false;
        private void timer1_Tick(object sender, EventArgs e) {
            //
            if (m_DriverProcess != null) {
                m_DriverProcess.DoUpdate();
            }
            if (this.needClose) {
                this.timer1.Stop();
                this.Close();
                return;
            }
       

        }
        #endregion


        #region  Dispose
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            noNeedFollow = true;
            UnHookOwnerFormStateEvent();
            UnHookThisFrmStateEvent();
            m_DriverProcess.Dispose();
            m_DriverProcess = null;
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
            m_frm = null;
        }
        #endregion

        class EmbedProcess2Container : IDisposable
        {
            public EmbedProcess2Container(frm_EmbedContainer ownerDockingContainer) {
                frmDockingContainer = ownerDockingContainer;
                HasProcessHelper.Ok += HasProcessHelper_Ok;
                HasProcessHelper.Exit += HasProcessHelper_Exit;
            }
            private frm_EmbedContainer frmDockingContainer;
            public void DoUpdate() {
                ProcInfo okP = null;
                lock (m_qOK) {
                    if (m_qOK.Count > 0) {
                        okP = m_qOK.Dequeue();
                    }
                }
                if (okP != null) {
                    AddProc(okP);
                }

                ProcInfo exitP = null;
                lock (m_qExit) {
                    if (m_qExit.Count > 0) {
                        exitP = m_qExit.Dequeue();
                    }
                }
                if (exitP != null) {
                    RemoveProc(exitP);
                }
            }
            public void Dispose() {
                m_qExit.Clear();
                m_qOK.Clear();
                HasProcessHelper.Ok -= HasProcessHelper_Ok;
                HasProcessHelper.Exit -= HasProcessHelper_Exit;
            }
            private class ProcInfo
            {
                internal string Name;
                internal int ProcId;

            }
            private Queue<ProcInfo> m_qOK = new Queue<ProcInfo>();
            private Queue<ProcInfo> m_qExit = new Queue<ProcInfo>();
            private void AddProc(ProcInfo p) {
                if (p.Name == "Python Shell") {
#if DEBUG
#else
                    return;
#endif
                }
                try {
                    //System.Diagnostics.Debug.WriteLine("AAAAAAAA:"+p.ProcId.ToString() + "  " + DateTime.Now.ToString());
                    Process proc = Process.GetProcessById(p.ProcId);
                    if (proc == null || proc.HasExited) {
                        return;
                    }
                    this.frmDockingContainer.SafeAction(() => {

                        //System.Threading.Thread.Sleep(2000);
                        //frm_EmbedContainer.m_InEmbedProcess = true;
                        DeskDriverFrm doc = frmDockingContainer.CreateAndShowDeskDriverFrm(p.Name);
                        doc.ProcessID = p.ProcId;
                        //(doc as DeskDriverFrm).panelContainer.Visible = false;
                        ProcessEmbed.EmbedProcess(proc, (doc as DeskDriverFrm).panelContainer, true);
                        doc.Resize += Doc_Resize;
                        //doc.Tag = p.ProcId.ToString();
                        //System.Threading.Thread.Sleep(50);
                    }
                    );
                }
                catch (Exception ee) {
                    Console.WriteLine(ee.ToString());
                }
                finally {
                    System.Threading.Thread.Sleep(50);
                    //OgreSystem.Singleton.ActiveRenderWindow();
                    //System.Threading.Thread.Sleep(2000);
                    //frm_EmbedContainer.m_InEmbedProcess = false;
                }

            }

            private void Doc_Resize(object sender, EventArgs e) {
                return;
                //var frm = sender as Form;          
                ////var hp = frm.Tag as IHasProcess;
                //var hp = frm.Tag as ProcInfo;
                //if(hp == null)
                //    return;
                //try {                    
                //    IntPtr mfhandle= EmbedWindowHelper.GetMainWindowHandle(Process.GetProcessById(hp.ProcId));
                //    if(mfhandle != null && mfhandle!=IntPtr.Zero ) {
                //        EmbedWindowHelper.EmbedProcessOnlyResize(mfhandle, (frm as DeskDriverFrm).panelContainer);
                //    }
                //}
                //catch(Exception ee) {
                //    DebugLog.Log(ee.ToString());
                //}
            }

            private void RemoveProc(ProcInfo p) {
                try {
                    frmDockingContainer.CloseDocmentByProcessID(p.ProcId);
                }
                catch (Exception ex) {

                }

            }
            private void HasProcessHelper_Ok(IHasProcess obj) {
                lock (m_qOK) {
                    m_qOK.Enqueue(new ProcInfo {
                        Name = obj.ProcCtx.Name,
                        ProcId = obj.ProcCtx.Proc.Id
                    });
                }


            }
            private void HasProcessHelper_Exit(IHasProcess obj) {
                lock (m_qExit) {
                    m_qExit.Enqueue(new ProcInfo {
                        Name = obj.ProcCtx.Name,
                        ProcId = obj.ProcCtx.PrcoId
                    });
                }
            }


        }

        private void frm_EmbedContainer_Load(object sender, EventArgs e) {

        }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams {
            get {
                CreateParams _createParams = base.CreateParams;
                _createParams.ClassStyle = _createParams.ClassStyle | CP_NOCLOSE_BUTTON;
                return _createParams;
            }
        }
    }
}
