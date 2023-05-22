using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// 可缩放移动窗口
    /// </summary>
    public class ToolWnd : Form
    {

        public ToolWnd() {
            InitializeComponent();
            //
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            m_LockSize = this.Size;
            //
            this.gly_Caption_ToolWnd.MouseLeave += this.gly_Caption_MouseLeave;
            this.gly_Caption_ToolWnd.MouseMove += this.gly_Caption_MouseMove;
            this.gly_Caption_ToolWnd.MouseDown += this.gly_Caption_MouseDown;
            this.gly_Caption_ToolWnd.MouseUp += this.gly_Caption_MouseUp;
        }

        protected Label gly_Caption_ToolWnd;

        protected PictureBox btn_ToolWndMin;        //收缩小
        protected PictureBox btn_ToolWndMax;        //收缩大
        protected PictureBox btn_ToolWndClose;             //关闭
        //
        private Panel panelToolTop;
        //  

        protected virtual void UnHookEvents() {
            this.btn_ToolWndClose.MouseUp -= btn_Help_MouseClick;
            this.btn_ToolWndMax.MouseUp -= btn_Help_MouseClick;
            this.btn_ToolWndMax.MouseUp -= btn_Help_MouseClick;
        }
        protected virtual void HookEvents() {
            //
            this.btn_ToolWndClose.MouseUp += btn_Help_MouseClick;
            this.btn_ToolWndMax.MouseUp += btn_Help_MouseClick;
            this.btn_ToolWndMax.MouseUp += btn_Help_MouseClick;
        }
        private bool m_Max;
        private void btn_Help_MouseClick(object arg1, MouseEventArgs arg2) {
            string name = (arg1 as Control).Name;

            if (name == this.btn_ToolWndClose.Name) {
                this.Hide();
            }
            else if (name == this.btn_ToolWndMax.Name || name == this.btn_ToolWndMax.Name) {
                if (!m_Max) {
                    m_Max = true;
                    this.m_oldSize = this.Size;
                    this.Size = new Size(this.Width, MinHeight);
                    //btn_Resize.Text = "□";
                    //显示还原图标
                    this.btn_ToolWndMax.Visible = true;
                    this.btn_ToolWndMax.Visible = false;
                }
                else {
                    m_Max = false;
                    this.Size = this.m_oldSize;
                    //btn_Resize.Text = "-";
                    //显示放大图标
                    this.btn_ToolWndMax.Visible = false;
                    this.btn_ToolWndMax.Visible = true;
                }
            }
        }
        

        #region 移动
        protected Size m_LockSize;
        /// <summary>
        /// 是否锁定大小
        /// </summary>
        public Size LockSize {
            get {
                return m_LockSize;
            }
        }
        protected bool m_IsLockSize;
        public bool IsLockSize {
            get {
                return m_IsLockSize;
            }
        }
        protected bool m_CanSize;
        /// <summary>
        /// 是否起用拉伸窗口模式
        /// </summary>
        public bool CanSize {
            get {
                return m_CanSize;
            }
        }
        /// <summary>
        /// 收缩后的高度
        /// </summary>
        protected readonly int MinHeight = 30;
        /// <summary>
        /// 最小宽度
        /// </summary>
        protected readonly int MinWidth = 250;

        protected const int WM_NCHITTEST = 0x0084;
        protected const int WM_SIZING = 532;

        private int HTCLIENT = 1;
        private int HTCAPTION = 2;
        private int HTLEFT = 10;
        private int HTRIGHT = 11;
        private int HTTOP = 12;
        private int HTTOPLEFT = 13;
        private int HTTOPRIGHT = 14;
        private int HTBOTTOM = 15;
        private int HTBOTTOMLEFT = 10;
        private int HTBOTTOMRIGHT = 17;


        private bool m_OnMove = false;
        private Point m_MoveDelta = Point.Empty;
        protected Size m_oldSize = Size.Empty;

        private IntPtr m_Captured = IntPtr.Zero;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCapture")]
        public static extern System.IntPtr SetCapture([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);
        private bool m_OnSizingStopRender = false;
        protected unsafe override void WndProc(ref Message m) {
            //m_OnSizingStopRender = false;
            switch (m.Msg) {
                case 5:
                    base.WndProc(ref m);
                    //m_OnSizingStopRender = false;
                    break;

                case 532: //尺寸变化
                    int* rect = (int*)m.LParam;
                    int left = *rect;
                    int top = *(rect + 1);
                    int right = *(rect + 2);
                    int bottom = *(rect + 3);
                    if (!IsLockSize) {
                        if (right - left < this.MinWidth) {
                            *(rect + 2) = left + this.MinWidth;
                        }
                        if (bottom - top < this.MinHeight) {
                            *(rect + 3) = top + this.MinHeight;
                        }
                    }
                    else {
                        if (right - left < this.LockSize.Width) {
                            *(rect + 2) = left + this.LockSize.Width;
                        }
                        if (bottom - top < this.LockSize.Height) {
                            *(rect + 3) = top + this.LockSize.Height;
                        }
                    }
                    base.WndProc(ref m);
                    //m_OnSizingStopRender = true;
                    Application.RaiseIdle(EventArgs.Empty);
                    break;

                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    m_OnSizingStopRender = false;
                    if (!CanSize)
                        return;
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);

                    if (vPoint.X <= 5) {
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else
                            m.Result = (IntPtr)HTLEFT;

                    }
                    else if (vPoint.X >= ClientSize.Width - 5) {
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else
                            m.Result = (IntPtr)HTRIGHT;

                    }
                    else if (vPoint.Y <= 5) {
                        m.Result = (IntPtr)HTTOP;

                    }
                    else if (vPoint.Y >= ClientSize.Height - 5) {
                        m.Result = (IntPtr)HTBOTTOM;

                    }

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
            //if (m_OnSizingStopRender)
            // Application.RaiseIdle(EventArgs.Empty);
        }

        private void gly_Caption_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                //Win32Native.ReleaseCapture();
                //Win32Native.SendMessage(this.FindForm().Handle,
                //    (int)Win32Native.WindowMessage.SystemCommand,
                //    (int)(Win32Native.WindowMessage.HTCAPTION | Win32Native.WindowMessage.SC_MOVE), 0);
                m_OnMove = true;
                m_MoveDelta = e.Location;
                //this.m_Captured=SetCapture(this.Handle);
            }
        }

        private void gly_Caption_MouseUp(object sender, MouseEventArgs e) {
            m_OnMove = false;
        }

        private void gly_Caption_MouseMove(object sender, MouseEventArgs e) {
            //System.Diagnostics.Debug.WriteLine(e.Button);
            if (m_OnMove && e.Button == MouseButtons.Left) {
                //Point p = gly_Caption.PointToScreen(new Point(e.X, e.Y));
                this.Left += (e.X - m_MoveDelta.X);
                this.Top += (e.Y - m_MoveDelta.Y);
                // m_MoveDelta = new Point(e.X, e.Y);
                // System.Diagnostics.Debug.WriteLine(p.ToString());
            }
        }

        private void gly_Caption_MouseLeave(object sender, EventArgs e) {
            m_OnMove = false;
        }
        #endregion

        #region 设计
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.gly_Caption_ToolWnd = new System.Windows.Forms.Label();
            this.panelToolTop = new System.Windows.Forms.Panel();
            this.btn_ToolWndClose = new System.Windows.Forms.PictureBox();
            this.btn_ToolWndMax = new System.Windows.Forms.PictureBox();
            this.btn_ToolWndMin = new System.Windows.Forms.PictureBox();
            this.panelToolTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndMin)).BeginInit();
            this.SuspendLayout();
            // 
            // gly_Caption_ToolWnd
            // 
            this.gly_Caption_ToolWnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gly_Caption_ToolWnd.BackColor = System.Drawing.Color.LightGray;
            this.gly_Caption_ToolWnd.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gly_Caption_ToolWnd.ForeColor = System.Drawing.Color.White;
            this.gly_Caption_ToolWnd.Location = new System.Drawing.Point(0, 0);
            this.gly_Caption_ToolWnd.Margin = new System.Windows.Forms.Padding(0);
            this.gly_Caption_ToolWnd.Name = "gly_Caption_ToolWnd";
            this.gly_Caption_ToolWnd.Size = new System.Drawing.Size(175, 28);
            this.gly_Caption_ToolWnd.TabIndex = 1;
            this.gly_Caption_ToolWnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelToolTop
            // 
            this.panelToolTop.Controls.Add(this.btn_ToolWndClose);
            this.panelToolTop.Controls.Add(this.btn_ToolWndMax);
            this.panelToolTop.Controls.Add(this.btn_ToolWndMin);
            this.panelToolTop.Controls.Add(this.gly_Caption_ToolWnd);
            this.panelToolTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolTop.Location = new System.Drawing.Point(0, 0);
            this.panelToolTop.Name = "panelToolTop";
            this.panelToolTop.Size = new System.Drawing.Size(258, 32);
            this.panelToolTop.TabIndex = 2;
            // 
            // btn_ToolWndClose
            // 
            this.btn_ToolWndClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ToolWndClose.BackColor = System.Drawing.Color.Maroon;
            this.btn_ToolWndClose.Location = new System.Drawing.Point(231, 4);
            this.btn_ToolWndClose.Name = "btn_ToolWndClose";
            this.btn_ToolWndClose.Size = new System.Drawing.Size(24, 24);
            this.btn_ToolWndClose.TabIndex = 2;
            this.btn_ToolWndClose.TabStop = false;
            // 
            // btn_ToolWndMax
            // 
            this.btn_ToolWndMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ToolWndMax.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.btn_ToolWndMax.Location = new System.Drawing.Point(205, 4);
            this.btn_ToolWndMax.Name = "btn_ToolWndMax";
            this.btn_ToolWndMax.Size = new System.Drawing.Size(24, 24);
            this.btn_ToolWndMax.TabIndex = 2;
            this.btn_ToolWndMax.TabStop = false;
            // 
            // btn_ToolWndMin
            // 
            this.btn_ToolWndMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ToolWndMin.BackColor = System.Drawing.Color.Maroon;
            this.btn_ToolWndMin.Location = new System.Drawing.Point(178, 4);
            this.btn_ToolWndMin.Name = "btn_ToolWndMin";
            this.btn_ToolWndMin.Size = new System.Drawing.Size(24, 24);
            this.btn_ToolWndMin.TabIndex = 2;
            this.btn_ToolWndMin.TabStop = false;
            // 
            // ToolWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(258, 398);
            this.ControlBox = false;
            this.Controls.Add(this.panelToolTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolWnd";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.panelToolTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ToolWndMin)).EndInit();
            this.ResumeLayout(false);

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.F4 | Keys.Alt:
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        #endregion


    }
}
