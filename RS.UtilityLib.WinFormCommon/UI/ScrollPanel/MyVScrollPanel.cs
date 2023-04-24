using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.ScrollPanel
{
    /// <summary>
    /// 请不要直接拉控件进来
    /// </summary>
    public partial class MyVScrollPanel : UserControl
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
        public MyVScrollPanel() {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            //
            this.AutoScroll = false;
            //
            this.m_ScorllPanel.AutoScroll = true;
            this.m_VScrollMy.Scroll += new EventHandler(m_VScrollMy_Scroll);
            //this.m_VScrollMy.MouseUp += new MouseEventHandler(m_VScrollMy_MouseUp);
            //this.m_ScorllPanel.MouseWheel += new MouseEventHandler(m_ScorllPanel_MouseWheel);
            this.m_ScorllPanel.SizeChanged += new EventHandler(m_ScorllPanel_SizeChanged);
            //
            //this.GotFocus += PanelScroll_GotFocus;
            //this.m_ScorllPanel.GotFocus += PanelScroll_GotFocus;
        }

        public override Color BackColor {
            get { return base.BackColor; }
            set {
                base.BackColor = value;
                this.m_ScorllPanel.BackColor = value;
            }
        }
        #region
        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
        }
        void m_ScorllPanel_SizeChanged(object sender, EventArgs e) {
            CaculScrollPosAndSize();
        }

        public void ReCaculScrollPosAndSize() {
            CaculScrollPosAndSize();
        }
        private void CaculScrollPosAndSize() {
            //this.m_ScorllPanel.Width = this.Width + 20;
            //this.m_ScorllPanel.Height = this.Height;
            //      
            //int h = 0;
            //foreach (Control c in Controls) {
            //    int b = c.Top + c.Height;
            //    if (b > h) {
            //        h = b;
            //    }
            //}
            //
            this.m_VScrollMy.Minimum = 0;
            this.m_VScrollMy.Maximum = this.m_ScorllPanel.DisplayRectangle.Height+8;
            this.m_VScrollMy.LargeChange = m_VScrollMy.Maximum / m_VScrollMy.Height + this.Height;
            this.m_VScrollMy.SmallChange = 15;
            //Point sp= GetScrollPoint(this);
            this.m_VScrollMy.Value = Math.Abs(this.m_ScorllPanel.AutoScrollPosition.Y);
            //
            if (this.m_ScorllPanel.DisplayRectangle.Height > this.m_ScorllPanel.Height) {
                this.m_VScrollMy.Visible = true;
            }
            else {
#if DEBUG
                if (IsDesignerHosted) {
                    this.m_VScrollMy.Visible = true;
                    this.m_VScrollMy.Invalidate();
                    return;
                }
#endif
                this.m_VScrollMy.Visible = false;
            }
            //
            m_VScrollMy.Invalidate();
        }
        /// <summary>
        /// 增量模式移动滚动条
        /// </summary>
        /// <param name="addUpOrDown"></param>
        public void ScrollPanelIncrement(int addUpOrDown) {
            int now = m_VScrollMy.Value - addUpOrDown;
            if (now < 0) now = 0;
            int len = this.m_VScrollMy.GetScorllLength();
            //len = this.m_VScrollMy.GetThumbHeight();
            if (now > this.m_VScrollMy.Maximum - len) {
                now = this.m_VScrollMy.Maximum - len;
            }
            this.m_VScrollMy.Value = now;
            this.m_VScrollMy_Scroll(null, null);
        }
        /// <summary>
        /// 当得到焦点前 同步滚动到的位置
        /// </summary>
        public void ReflashScrollPos() {
            int value = m_VScrollMy.Value;
            //this.m_ScorllPanel.VerticalScroll.Maximum = m_VScrollMy.Maximum;    
            Point p = new Point(0, value);
            this.m_ScorllPanel.AutoScrollOffset = p;
            this.m_ScorllPanel.AutoScrollPosition = p;
        }
        /// <summary>
        /// 
        /// </summary>
        public CustomScrollPanel GetScrollPanel() {
            //get {
            return m_ScorllPanel;
            //}
        }
        public CustomScrollbar GetScrollBar() {
            //get {
            return m_VScrollMy;
            //}
        }
        void m_VScrollMy_Scroll(object sender, EventArgs e) {
            //MoveBar(1, m_VScrollMy.Value, this);
            int value = m_VScrollMy.Value;
            //this.m_ScorllPanel.VerticalScroll.Maximum = m_VScrollMy.Maximum;    
            Point p = new Point(0, value);
            this.m_ScorllPanel.AutoScrollOffset = p;
            this.m_ScorllPanel.AutoScrollPosition = p;
            //this.m_VScrollMy.Invalidate();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.Invalidate();
        }
        #region
        protected override void OnMouseWheel(MouseEventArgs e) {
            this.ScrollPanelIncrement(e.Delta);
            //base.OnMouseWheel(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            //base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            //base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            //base.OnMouseUp(e);
        }

        #endregion

        public Size GetContainSize() {
            return this.m_ScorllPanel.Size;
        }
        /// <summary>
        /// 添加控件,默认重新计算滚动条的位置
        /// </summary>
        /// <param name="c"></param>
        public void AddControl(Control c, bool reCaculScrollPosAndSize = true) {
            if (c.Parent != null) {
                c.Parent.Controls.Remove(c);
            }
            this.m_ScorllPanel.Controls.Add(c);
            //
            if (reCaculScrollPosAndSize) {
                ReCaculScrollPosAndSize();
            }
        }
        /// <summary>
        /// 指定位置 添加控件,默认重新计算滚动条的位置
        /// </summary>
        /// <param name="c"></param>
        /// <param name="location"></param>
        /// <param name="reCaculScrollPosAndSize"></param>
        public void AddControl(Control c, Point location, bool reCaculScrollPosAndSize = true) {
            c.Location = location;
            AddControl(c, reCaculScrollPosAndSize);
        }
        /// <summary>
        ///移除控件,默认重新计算滚动条的位置
        /// </summary>
        /// <param name="c"></param>
        /// <param name="reCaculScrollPosAndSize"></param>
        public void RemoveControl(Control c, bool reCaculScrollPosAndSize = true) {
            this.m_ScorllPanel.Controls.Remove(c);
            if (reCaculScrollPosAndSize) {
                ReCaculScrollPosAndSize();
            }
        }
        /// <summary>
        /// 移除控件,默认重新计算滚动条的位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="reCaculScrollPosAndSize"></param>
        public void RemoveControlAt(int index, bool reCaculScrollPosAndSize = true) {
            this.m_ScorllPanel.Controls.RemoveAt(index);
            if (reCaculScrollPosAndSize) {
                ReCaculScrollPosAndSize();
            }
        }
        //

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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.m_ScorllPanel = new RS.UtilityLib.WinFormCommon.UI.ScrollPanel.CustomScrollPanel();
            this.m_VScrollMy = new RS.UtilityLib.WinFormCommon.UI.ScrollPanel.CustomScrollbar();
            this.SuspendLayout();
            // 
            // m_ScorllPanel
            // 
            this.m_ScorllPanel.BackColor = System.Drawing.SystemColors.Control;
            this.m_ScorllPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ScorllPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ScorllPanel.Name = "m_ScorllPanel";
            this.m_ScorllPanel.Size = new System.Drawing.Size(184, 200);
            this.m_ScorllPanel.TabIndex = 2;
            // 
            // m_VScrollMy
            // 
            this.m_VScrollMy.ChannelColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(166)))), ((int)(((byte)(3)))));
            this.m_VScrollMy.Dock = System.Windows.Forms.DockStyle.Right;
            this.m_VScrollMy.DownArrowImage = new System.Drawing.Rectangle(19, 0, 16, 16);
            this.m_VScrollMy.LargeChange = 10;
            this.m_VScrollMy.Location = new System.Drawing.Point(184, 0);
            this.m_VScrollMy.Maximum = 100;
            this.m_VScrollMy.Minimum = 0;
            this.m_VScrollMy.MinimumSize = new System.Drawing.Size(16, 88);
            this.m_VScrollMy.Name = "m_VScrollMy";
            this.m_VScrollMy.Size = new System.Drawing.Size(16, 200);
            this.m_VScrollMy.SmallChange = 1;
            this.m_VScrollMy.TabIndex = 3;
            this.m_VScrollMy.ThumbBackImage = new System.Drawing.Rectangle(76, 0, 16, 16);
            this.m_VScrollMy.ThumbBottomImage = new System.Drawing.Rectangle(95, 0, 16, 16);
            this.m_VScrollMy.ThumbBottomSpanImage = new System.Drawing.Rectangle(57, 0, 16, 16);
            this.m_VScrollMy.ThumbMiddleImage = new System.Drawing.Rectangle(57, 0, 16, 16);
            this.m_VScrollMy.ThumbTopImage = new System.Drawing.Rectangle(38, 0, 16, 16);
            this.m_VScrollMy.ThumbTopSpanImage = new System.Drawing.Rectangle(57, 0, 16, 16);
            this.m_VScrollMy.UpArrowImage = new System.Drawing.Rectangle(0, 0, 16, 16);
            this.m_VScrollMy.Value = 0;
            // 
            // MyScrollPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            base.Controls.Add(this.m_ScorllPanel);
            base.Controls.Add(this.m_VScrollMy);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MyVScrollPanel";
            this.Size = new System.Drawing.Size(200, 200);
            this.ResumeLayout(false);

        }
        /*
        private void InitCtl() {
            this.m_VScrollMy = new CustomScrollbar();
            this.m_ScorllPanel = new CustomScrollPanel();
            // 
            // m_VScrollMy
            // 
            this.m_VScrollMy.ChannelColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(166)))), ((int)(((byte)(3)))));
            this.m_VScrollMy.Dock = System.Windows.Forms.DockStyle.Right;
            // 
            // m_ScorllPanel
            // 
            this.m_ScorllPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ScorllPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ScorllPanel.Name = "m_ScorllPanel";
            this.m_ScorllPanel.Size = new System.Drawing.Size(182, 200);
            this.m_ScorllPanel.TabIndex = 2;
            //
            base.Controls.Add(this.m_ScorllPanel);
            base.Controls.Add(this.m_VScrollMy);
        }
        */
        #endregion

        private CustomScrollbar m_VScrollMy;
        private CustomScrollPanel m_ScorllPanel;
    }
}
