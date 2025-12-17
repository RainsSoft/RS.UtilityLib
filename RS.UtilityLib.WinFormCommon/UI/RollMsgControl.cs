using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public delegate void OnClickRollControlLink(string link);
    /// <summary>
    /// 可滚动的控件  带链接功能 
    /// 可通过访问m_MsgSourceURL，获取滚动的内容。
    /// 滚动内容使用|分割。如msg1|url1|msg2|url2
    /// </summary>
    public partial class RollMsgControl : UserControl
    {
        public static void OpenWeb(string url) {
            //int ret = ShellExecute(IntPtr.Zero, "open", url, IntPtr.Zero, IntPtr.Zero, 1);
            //if (ret <= 32) {
            //    DebugLog.Log("打开浏览器失败!错误号:" + ret.ToString());
            //}
            Console.WriteLine("访问URL：" + url);
            bool isxp = false;
            bool upxp = false;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                && Environment.OSVersion.Version.Major == 5) {
                isxp = true;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                && Environment.OSVersion.Version.Major >= 6) {
                upxp = true;
            }
            string strBrowse = string.Empty;
            if (isxp) {
                Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command");
                if (k != null) {
                    object o = k.GetValue("");
                    if (o != null) {
                        string exe = o.ToString();
                        int start = exe.IndexOf('"');
                        int end = exe.IndexOf('"', start + 1);
                        if (end > 0 && end > start) {
                            strBrowse = exe.Substring(start + 1, end - start - 1);
                        }
                    }
                }
            }
            if (upxp) {
                Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice");
                if (k != null) {
                    object o = k.GetValue("Progid");
                    if (o != null) {
                        string progid = o.ToString();
                        k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(progid + "\\shell\\open\\command");
                        if (k != null) {
                            o = k.GetValue("");
                            string exe = o as string;
                            if (string.IsNullOrEmpty(exe) == false) {
                                int start = exe.IndexOf('"');
                                int end = exe.IndexOf('"', start + 1);
                                if (end > 0 && end > start) {
                                    strBrowse = exe.Substring(start + 1, end - start - 1);
                                }
                            }
                        }

                    }
                }
            }
            if (string.IsNullOrEmpty(strBrowse)) {
                strBrowse = "iexplore.exe";
            }
            try {
                Process.Start(strBrowse, url);
            }
            catch {
            }
        }
        class RollMsg
        {
            internal int Left;
            internal int With;
            internal string Msg;
            internal string Url;
        }
        private string m_MsgSourceURL = string.Empty;
        private int m_GetMsgTimer = 0;
        /// <summary>
        /// 单击联接时发生 传出连接地址 http://xxx
        /// </summary>
        public event OnClickRollControlLink OnClickLinkEvent;
        List<RollMsg> m_Msgs = new List<RollMsg>();
        public RollMsgControl() {
            this.InitializeComponent();
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.Size = new Size(800, 102);
            this.DoubleBuffered = true;
            //
            this.timer_Roll.Tick += new EventHandler(timer_Roll_Tick);

            //
            this.timer_Roll.Stop();
        }

        public RollMsgControl(string msgSourceURL)
            : this() {
            //每隔5分钟访问一次msgSourceURL，获取信息
            m_MsgSourceURL = msgSourceURL;
            m_GetRollMsgData = GetRollMsgData;
        }

        System.Windows.Forms.MethodInvoker m_GetRollMsgData;
        private void GetRollMsgData() {
            //TODO:这里有个问题需要解决,那就是异步操作结束时,此控件已经被Dispose的情况.
            if (string.IsNullOrEmpty(m_MsgSourceURL)) {
                return;
            }
            //System.Threading.Thread.Sleep(20000);
            HttpWebRequest req = WebRequest.Create(m_MsgSourceURL) as HttpWebRequest;
            try {
                WebResponse res = req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8)) {
                    string s = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(s) == false) {
                        string[] arys = s.Split('|');
                        ClearMsg();

                        for (int i = 0; i < arys.Length; i += 2) {
                            if (string.IsNullOrEmpty(arys[i]) == false && string.IsNullOrEmpty(arys[i + 1]) == false) {
                                AddMsg(arys[i], arys[i + 1]);
                            }
                        }

                    }
                }
            }
            catch (Exception ee1) {
                Console.WriteLine("获取滚动消息失败:" +ee1.ToString());
                ClearMsg();
                AddDefaultMsg();
            }
        }
        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            try {
                int x = e.X - m_MsgStartPos;
                RollMsg[] msgs;
                lock (m_locker) {
                    msgs = m_Msgs.ToArray();
                }
                foreach (var v in msgs) {
                    if (!string.IsNullOrEmpty(v.Url) && x > v.Left && x < v.Left + v.With) {
                        if (string.IsNullOrEmpty(v.Url)) {
                            v.Url = "http://www.baidu.com";
                        }
                        OpenWeb(v.Url);
                        if (this.OnClickLinkEvent != null) {
                            this.OnClickLinkEvent(v.Url);
                        }
                        break;
                    }
                }
            }
            catch (Exception ee) {
                Console.WriteLine( "无法打开目标链接:"+ ee.ToString());
            }
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (m_IsRoll) {
                this.StartRoll();
            }
            m_MousePos = Point.Empty;
        }

        Point m_MousePos = Point.Empty;
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            this.PauseRoll();
            m_MousePos = new Point(e.X, e.Y);
            bool hand = false;
            RollMsg[] msgs;
            lock (m_locker) {
                msgs = m_Msgs.ToArray();
            }
            foreach (var v in msgs) {
                if (!string.IsNullOrEmpty(v.Url) && m_MousePos.X > m_MsgStartPos + v.Left && m_MousePos.X < m_MsgStartPos + v.Left + v.With) {
                    hand = true;
                    break;
                }
            }
            if (hand) {
                this.Cursor = Cursors.Hand;
            }
            else {
                this.Cursor = Cursors.Default;
            }
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            AddDefaultMsg();
        }


        void timer_Roll_Tick(object sender, EventArgs e) {
            if (m_MsgStartPos < -m_MsgLength) {
                m_MsgStartPos = this.Width;
            }
            m_MsgStartPos -= 8;
            this.Invalidate();
            if (m_GetMsgTimer == 0 || m_GetMsgTimer > 5 * 60 * 1000) {
                m_GetRollMsgData.BeginInvoke(null, null);
                m_GetMsgTimer = 1;
            }
            else {
                m_GetMsgTimer += timer_Roll.Interval;
            }
        }
        private bool m_IsRoll = false;
        public bool IsRoll {
            get {
                return m_IsRoll;
            }
        }
        private object m_locker = new object();
        private void AddDefaultMsg() {
            AddMsg("欢迎...", "http://www.baidu.com");
        }
        /// <summary>
        /// 最多10条滚动信息
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string Msg, string httpURL) {
            RollMsg msg = new RollMsg();
            msg.Msg = Msg;
            msg.Url = httpURL;
            lock (m_locker) {
                if (this.m_Msgs.Count > 10) {
                    this.m_Msgs.RemoveAt(0);
                }
                m_Msgs.Add(msg);
            }
        }
        System.Drawing.Graphics m_MsgGrp;
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            m_MsgGrp = e.Graphics;
            AutoRollPosAndWidth();
            //绘制文字
            RollMsg[] msgs;
            lock (m_locker) {
                msgs = m_Msgs.ToArray();
            }
            foreach (var v in msgs) {
                int x = v.Left + m_MsgStartPos;
                if (x + v.With > 0 && x < this.Width) {
                    m_MsgGrp.DrawString(v.Msg, this.Font, SystemBrushes.ControlLightLight, m_MsgStartPos + v.Left, 0);
                    if (m_MousePos != Point.Empty && !string.IsNullOrEmpty(v.Url) && m_MousePos.X > m_MsgStartPos + v.Left && m_MousePos.X < m_MsgStartPos + v.Left + v.With) {
                        m_MsgGrp.DrawLine(SystemPens.ActiveBorder, m_MsgStartPos + v.Left, this.Height - 2, m_MsgStartPos + v.Left + v.With, this.Height - 2);
                    }
                }
            }

        }
        /// <summary>
        /// 销毁/清理显示的内容
        /// </summary>
        public void ClearMsg() {
            lock (m_locker) {
                this.m_Msgs.Clear();
            }
        }
        /// <summary>
        /// 重置位置 不影响滚动
        /// </summary>
        public void ResetRoll() {
            m_MsgStartPos = this.Width;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void StartRoll() {
            if (!this.timer_Roll.Enabled)
                this.timer_Roll.Start();
            m_IsRoll = true;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        protected void PauseRoll() {
            if (this.timer_Roll.Enabled) {
                this.Invalidate();
            }
            this.timer_Roll.Stop();
        }
        public void StopRoll() {
            this.timer_Roll.Stop();
            m_IsRoll = false;
        }
        int m_MsgStartPos = 1;
        int m_MsgLength = 1;
        private void AutoRollPosAndWidth() {
            int left = 0;
            int jiage = 8;
            //int with = 0;
            RollMsg[] msgs;
            lock (m_locker) {
                msgs = m_Msgs.ToArray();
            }
            for (int i = 0; i < msgs.Length; i++) {
                msgs[i].Left = left;
                if (m_MsgGrp != null) {
                    msgs[i].With = (int)m_MsgGrp.MeasureString(msgs[i].Msg, this.Font).Width;
                }
                left += msgs[i].With + jiage;
                //with += m_Msgs[i].With;
            }
            m_MsgLength = left;

        }
        private void DisposeRollRes() {
            this.timer_Roll.Stop();
            this.timer_Roll.Dispose();
            this.ClearMsg();
        }

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
            DisposeRollRes();
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
            this.components = new System.ComponentModel.Container();
            this.timer_Roll = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer_Roll
            // 
            this.timer_Roll.Interval = 500;
            // 
            // IRQ_RollMsgControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "IRQ_RollMsgControl";
            this.Size = new System.Drawing.Size(430, 14);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer_Roll;
        #endregion
    }

    //internal static class RollMsgManager {

    //    private void GetRollMsgData() {
    //        if (string.IsNullOrEmpty(m_MsgSourceURL)) {
    //            return;
    //        }
    //        System.Threading.Thread.Sleep(20000);
    //        HttpWebRequest req = WebRequest.Create(m_MsgSourceURL) as HttpWebRequest;
    //        try {
    //            WebResponse res = req.GetResponse();
    //            using (StreamReader sr = new StreamReader(res.GetResponseStream())) {
    //                string s = sr.ReadToEnd();
    //                if (string.IsNullOrEmpty(s) == false) {
    //                    string[] arys = s.Split('|');
    //                    for (int i = 0; i < arys.Length; i += 2) {
    //                        if (string.IsNullOrEmpty(arys[i]) == false && string.IsNullOrEmpty(arys[i + 1]) == false) {
    //                            AddMsg(arys[i], arys[i + 1]);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ee1) {
    //            DebugLog.Log("获取滚动消息失败", ee1, true);
    //            ClearMsg();
    //            AddDefaultMsg();
    //        }
    //    }
    //}
}
