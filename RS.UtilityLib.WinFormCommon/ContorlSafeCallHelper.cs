using RS.UtilityLib.WinFormCommon.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
{
    public delegate DialogResult FuncMsgDialogResult<Form, DialogResult>(Form owner);
    public static partial class ContorlSafeCallHelper
    {

        #region 跨线程访问控件辅助方法
        /// <summary>
        /// 非安全模式执行 修改控件属性
        /// 前提需要设置  System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls=false;
        /// 先检查contrl c是否创建并没有销毁 然后执行action ac对contrl c的属性直接修改
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ac"></param>
        public static void UnSafeAction(this Control c, Action ac) {
            if (CheckHasCreatedControl(c) == false) {
                return;
            }
            try {
                ac();
            }
            catch (Exception ee) {
                System.Diagnostics.Debug.Assert(true, "UnSafeAction-action()错误");
                Debug.WriteLine("UnSafeAction: 执行action()异常" + ee.ToString());
            }

        }
        /// <summary>
        /// 非安全模式执行 获取控件属性
        /// 前提需要设置  System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls=false;
        /// 先检查contrl c是否创建并没有销毁 然后执行获取Func func对contrl c的属性对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="_default"></param>
        /// <returns></returns>
        public static T UnSafeFunc<T>(this Control c, Func<T> func, T _default) {
            if (CheckHasCreatedControl(c)) {
                try {
                    var TR = func();
                    return TR;
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-action()错误");
                    Debug.WriteLine("SafeFunc<T>: 执行func()异常" + ee.ToString());
                }
            }
            return _default;//default(T);
        }
        /// <summary>
        /// 安全读写Control属性
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ac"></param>
        /// <param name="asyn"></param>
        public static void SafeAction(this Control c, Action ac, bool asyn = false) {
            if (CheckHasCreatedControl(c)) {
                Action acWrapper = () => {
                    try {
                        ac();
                    }
                    catch (Exception ee) {
                        System.Diagnostics.Debug.Assert(true, "SafeAction-action()错误");
                        Debug.WriteLine("SafeAction: 执行action()异常" + ee.ToString());
                    }
                };
                try {
                    if (asyn) {
                        c.BeginInvoke(acWrapper);
                    }
                    else {
                        if (c.InvokeRequired) {
                            c.Invoke(acWrapper);
                        }
                        else {
                            acWrapper();
                        }
                    }
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeAction-控件委托错误");
                    Debug.WriteLine("SafeAction:执行控件委托异常：" + ee.ToString());
                }
            }
        }
        /// <summary>
        /// 安全获取Control属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="_default"></param>
        /// <returns></returns>
        public static T SafeFunc<T>(this Control c, Func<T> func, T _default) {
            if (CheckHasCreatedControl(c)) {
                var TR = _default;//default(T);
                Action acWrapper = () => {
                    try {
                        TR = func();
                    }
                    catch (Exception ee) {
                        System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-action()错误");
                        Debug.WriteLine("SafeFunc<T>: 执行func()异常" + ee.ToString());
                    }
                };

                try {
                    if (c.InvokeRequired) {
                        c.Invoke(acWrapper);
                        return TR;
                    }
                    else {
                        acWrapper();
                        return TR;
                    }
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-控件委托错误");
                    Debug.WriteLine("SafeFunc<T>:执行控件委托异常：" + ee.ToString());
                }
            }
            return _default;//default(T);
        }
        /// <summary>
        /// 检测Control是否已经创建好(已经Show)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckHasCreatedControl(this Control c) {
            try {
                if (c == null ||
                    c.IsDisposed ||
                    c.IsHandleCreated == false ||
                    c.FindForm().IsHandleCreated == false
                    ) {
                    // some exceptional condition:
                    // handle in whatever way is appropriate for your app
                    return false;
                }
                return true;
            }
            catch (Exception ee) {
                Debug.WriteLine("CheckHasCreatedControl:" + ee.ToString());
            }
            return false;
        }
        #endregion


        public static DialogResult ShowMsgDialogWithMask(this Form owner, FuncMsgDialogResult<Form, DialogResult> funcMsgDialogResult, bool overlapOwnerForm = false) {
            System.Windows.Forms.DialogResult Result = System.Windows.Forms.DialogResult.None;
            {
                var Temp = new System.Windows.Forms.Form();
                Temp.SuspendLayout();
                Temp.Dock = System.Windows.Forms.DockStyle.Fill;
                Temp.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                Temp.BackColor = System.Drawing.Color.Silver;
                Temp.Opacity = 0.7D;
                Temp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
#if !DEBUG
                Temp.ShowInTaskbar = false;
#endif

                Temp.KeyDown += (sender, e) => {
                    if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
                        Temp.Close();
                    }
                };
                if (owner != null && overlapOwnerForm) {
                    //覆盖owner窗体
                    Temp.WindowState = FormWindowState.Normal;
                    Temp.StartPosition = FormStartPosition.Manual;
                    Temp.Size = owner.Size;
                    Temp.Location = owner.Location;
                    Temp.Shown += (sender, e) => {
                        //
                        Result = funcMsgDialogResult(Temp);
                        Temp.Close();
                    };
                    Temp.Load += (sender, e) => {
                        try {
                            Point pt = Temp.PointToScreen(new Point(0, 0));
                            Bitmap b = new Bitmap(Temp.Width, Temp.Height);
                            using (Graphics g = Graphics.FromImage(b)) {
                                g.SmoothingMode = SmoothingMode.HighQuality; //高质量
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.CopyFromScreen(pt, new Point(), new Size(Temp.Width, Temp.Height));
                                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(128, Color.Silver))) {
                                    //增加四边阴影
                                    DrawShadow(b.Size, 4, Color.Silver, g);
                                    //修改图片亮度
                                    //g.FillRectangle(backBrush, new RectangleF(0, 0, b.Width, b.Height));                            
                                    float dwmleft = 8, dwmtop = 0, dwmright = 8, dwmbottom = 8;
                                    RectangleF rect = new RectangleF(dwmleft - 0.5f, dwmtop - 0.5f, b.Width - dwmleft - dwmright + 0.5f, b.Height - dwmtop - dwmbottom + 0.5f);
                                   
                                    g.FillRectangle(backBrush, rect);
                                    
                                }
                            }
                            //b.Save("test.bmp");
                            Temp.BackgroundImage = b;
                        }
                        catch {
                        }

                    };
                }
                else {
                    Temp.Load += (sender, e) => {
                        Result = funcMsgDialogResult(Temp);
                        Temp.Close();
                    };
                }
                Temp.ResumeLayout(false);
                if (owner != null) {
                    Temp.ShowDialog();
                }
                else { 
                    Temp.ShowDialog(owner);
                }
                    
            }
            return Result;
        }
        public static void ShowWithMask(this Form displayForm, Form owner, bool overlapOwnerForm = false) {
            //System.Diagnostics.Debug.Assert(owner != null);

            var Temp = new System.Windows.Forms.Form();
            Temp.SuspendLayout();
            Temp.Dock = System.Windows.Forms.DockStyle.Fill;
            Temp.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Temp.BackColor = System.Drawing.Color.Silver;
            Temp.Opacity = 0.7D;
            Temp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
#if !DEBUG
            Temp.ShowInTaskbar = false;
#endif
            if (owner != null && overlapOwnerForm) {
                //覆盖owner窗体              
                Temp.WindowState = FormWindowState.Normal;
                Temp.StartPosition = FormStartPosition.Manual;
                Temp.Size = owner.Size;
                Temp.Location = owner.Location;
                Temp.Shown += (sender, e) => {
                    UINotifier.UIFollowFormHelper uf = new UINotifier.UIFollowFormHelper(owner, Temp, 0, 0, UINotifier.UIFollowMode.Offset, true);
                };
                Temp.VisibleChanged += (sender, e) => {
                    try {
                        if (Temp.Visible) {
                            int House = 300;
                            //淡入特效
                            Int32 AW_BLEND = 0x00080000;
                            Int32 AW_ACTIVATE = 0x00020000;
                            AnimateWindow(Temp.Handle, House, AW_BLEND | AW_ACTIVATE);
                            //Temp.Opacity = 0.7D;
                            Temp.Update();
                        }
                        else {
                            //Temp.Opacity = 1;
                            //实现窗体的淡出
                            Int32 AW_BLEND = 0x00080000;
                            Int32 AW_HIDE = 0x00010000;
                            AnimateWindow(Temp.Handle, 150, AW_BLEND | AW_HIDE);
                            Temp.Update();

                        }
                    }
                    catch {
                    }
                };
            }

            Temp.KeyDown += (sender, e) => {
                if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
                    Temp.Close();
                }
            };
            Temp.Load += (sender, e) => {
                if (overlapOwnerForm && owner != null) {
                    try {
                        Point pt = Temp.PointToScreen(new Point(0, 0));
                        Bitmap b = new Bitmap(Temp.Width, Temp.Height);
                        using (Graphics g = Graphics.FromImage(b)) {
                            g.SmoothingMode = SmoothingMode.HighQuality; //高质量
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.CopyFromScreen(pt, new Point(), new Size(Temp.Width, Temp.Height));
                            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(128, Color.Silver))) {
                                //增加四边阴影
                                DrawShadow(b.Size, 4, Color.Silver, g);
                                //修改图片亮度
                                //g.FillRectangle(backBrush, new RectangleF(0, 0, b.Width, b.Height));                            
                                float dwmleft = 8, dwmtop = 0, dwmright = 8, dwmbottom = 8;
                                RectangleF rect = new RectangleF(dwmleft - 0.5f, dwmtop - 0.5f, b.Width - dwmleft - dwmright + 0.5f, b.Height - dwmtop - dwmbottom + 0.5f);
                                
                                g.FillRectangle(backBrush, rect);
                              
                            }
                        }
                        //b.Save("test.bmp");
                        Temp.BackgroundImage = b;
                    }
                    catch {
                    }
                }
                displayForm.Disposed += (sender2, e2) => {
                    Temp.Close();
                };
                displayForm.Show(Temp);
            };
            //
            Temp.ResumeLayout(false);
            if (owner != null) {
                Temp.Show(owner);
            }
            else {
                Temp.Show();
            }

        }
        public static void ShowDialogWithMask(this Form displayForm, Form owner, bool overlapOwnerForm = false) {
            //System.Diagnostics.Debug.Assert(owner != null);

            var Temp = new System.Windows.Forms.Form();
            Temp.SuspendLayout();
            Temp.Dock = System.Windows.Forms.DockStyle.Fill;
            Temp.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Temp.BackColor = System.Drawing.Color.Silver;
            Temp.Opacity = 0.7D;
            Temp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
#if !DEBUG
            Temp.ShowInTaskbar = false;
#endif


            Temp.KeyDown += (sender, e) => {
                if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
                    Temp.Close();
                }
            };
            if (owner != null && overlapOwnerForm) {
                //覆盖owner窗体              
                Temp.WindowState = FormWindowState.Normal;
                Temp.StartPosition = FormStartPosition.Manual;
                Temp.Size = owner.Size;
                Temp.Location = owner.Location;
                //
                Temp.Load += (sender, e) => {
                    try {
                        Point pt = Temp.PointToScreen(new Point(0, 0));
                        Bitmap b = new Bitmap(Temp.Width, Temp.Height);
                        using (Graphics g = Graphics.FromImage(b)) {
                            g.SmoothingMode = SmoothingMode.HighQuality; //高质量
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            //g.Clear(Color.FromArgb(0,0,0,0));
                            g.CopyFromScreen(pt, new Point(), new Size(Temp.Width, Temp.Height));
                            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(128, Color.Silver))) {
                                //增加四边阴影
                                DrawShadow(b.Size, 4, Color.Silver, g);
                                //修改图片亮度
                                //g.FillRectangle(backBrush, new RectangleF(0, 0, b.Width, b.Height));                            
                                float dwmleft = 8, dwmtop = 0, dwmright = 8, dwmbottom = 8;
                                RectangleF rect = new RectangleF(dwmleft - 0.5f, dwmtop - 0.5f, b.Width - dwmleft - dwmright + 0.5f, b.Height - dwmtop - dwmbottom + 0.5f);
                               
                                g.FillRectangle(backBrush, rect);
                                //brush.Dispose(); brush = null;
                            }
                        }
                        //b.Save("test.bmp");
                        Temp.BackgroundImage = b;
                    }
                    catch {
                    }
                };
                Temp.Shown += (sender, e) => {
                    displayForm.ShowDialog();
                    Temp.Close();
                };
            }
            else {
                Temp.Load += (sender, e) => {
                    displayForm.ShowDialog();
                    Temp.Close();
                };
            };
            //
            Temp.ResumeLayout(false);
            if (owner != null) {
                Temp.ShowDialog(owner);
            }
            else {
                Temp.ShowDialog();
            }

        }

        /// <summary>
        /// 绘制阴影效果
        /// </summary>
        /// <param name="size">控件尺寸</param>
        /// <param name="radius">阴影半径</param>
        /// <param name="g">绘图区</param>
        static void DrawShadow(Size size, int radius, Color Color_Black, Graphics g) {
            if (radius <= 0) return;
            int this_Width = size.Width;
            int this_Height = size.Height;
            int d = 2 * radius;
            #region[LinearGradientBrush]   
            ColorBlend blend = new ColorBlend();
            blend.Colors = new Color[] { Color.FromArgb(100, Color_Black), Color.FromArgb(40, Color_Black), Color.FromArgb(0, Color_Black) };
            blend.Positions = new float[] { 0, 0.4f, 1 };
            LinearGradientBrush brushleft = new LinearGradientBrush(new Point(radius, 0), new Point(0, 0), Color.FromArgb(60, Color_Black), Color.FromArgb(0, Color_Black));
            brushleft.InterpolationColors = blend;
            LinearGradientBrush brushright = new LinearGradientBrush(new Point(size.Width - radius - 1, 0), new Point(size.Width, 0), Color.FromArgb(80, Color_Black), Color.FromArgb(0, Color_Black));
            brushright.InterpolationColors = blend;
            LinearGradientBrush brushtop = new LinearGradientBrush(new Point(0, radius), new Point(0, 0), Color.FromArgb(60, Color_Black), Color.FromArgb(0, Color_Black));
            brushtop.InterpolationColors = blend;
            LinearGradientBrush brushbottom = new LinearGradientBrush(new Point(0, size.Height - radius - 1), new Point(0, size.Height), Color.FromArgb(80, Color_Black), Color.FromArgb(0, Color_Black));
            brushbottom.InterpolationColors = blend;
            #endregion
            #region[path]
            GraphicsPath pathlefttop = new GraphicsPath();
            pathlefttop.AddPie(new Rectangle(0, 0, d, d), 180, 90);
            GraphicsPath pathrighttop = new GraphicsPath();
            pathrighttop.AddPie(new Rectangle(this_Width - d, 0, d, d), 270, 90);
            GraphicsPath pathleftbottom = new GraphicsPath();
            pathleftbottom.AddPie(new Rectangle(0, this_Height - d, d, d), 90, 90);
            GraphicsPath pathrightbottom = new GraphicsPath();
            pathrightbottom.AddPie(new Rectangle(this_Width - d, this_Height - d, d, d), 0, 90);
            #endregion
            #region[PathGradientBrush]
            PathGradientBrush brushlefttop = new PathGradientBrush(pathlefttop);
            brushlefttop.CenterPoint = new Point(radius, radius);
            brushlefttop.CenterColor = Color.FromArgb(80, Color_Black);
            brushlefttop.SurroundColors = new Color[] { Color.FromArgb(0, Color_Black) };
            //brushlefttop.InterpolationColors = blend;
            PathGradientBrush brushrighttop = new PathGradientBrush(pathrighttop);
            brushrighttop.CenterPoint = new Point(this_Width - radius, radius);
            brushrighttop.CenterColor = Color.FromArgb(80, Color_Black);
            brushrighttop.SurroundColors = new Color[] { Color.FromArgb(0, Color_Black) };
            //brushrighttop.InterpolationColors = blend;
            PathGradientBrush brushleftbottom = new PathGradientBrush(pathleftbottom);
            brushleftbottom.CenterPoint = new Point(radius, this_Height - radius);
            brushleftbottom.CenterColor = Color.FromArgb(80, Color_Black);
            brushleftbottom.SurroundColors = new Color[] { Color.FromArgb(0, Color_Black) };
            //brushleftbottom.InterpolationColors = blend;
            PathGradientBrush brushrightbottom = new PathGradientBrush(pathrightbottom);
            brushrightbottom.CenterPoint = new Point(this_Width - radius, this_Height - radius);
            brushrightbottom.CenterColor = Color.FromArgb(80, Color_Black);
            brushrightbottom.SurroundColors = new Color[] { Color.FromArgb(0, Color_Black) };
            //brushrightbottom.InterpolationColors = blend;
            #endregion
            #region[draw]
            g.FillRectangle(brushleft, new RectangleF(1, radius - 0.5f, radius, this_Height - d + 0.5f));
            g.FillRectangle(brushright, new RectangleF(this_Width - radius - 1, radius - 0.5f, radius, this_Height - d + 0.5f));
            g.FillRectangle(brushtop, new RectangleF(radius - 0.5f, 0, this_Width - d + 0.5f, radius));
            g.FillRectangle(brushbottom, new RectangleF(radius - 0.5f, this_Height - radius - 1, this_Width - d + 0.5f, radius));
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
        /// 执行动画
        /// </summary>
        /// <param name="whnd">控件句柄</param>
        /// <param name="dwtime">动画时间</param>
        /// <param name="dwflag">动画组合名称</param>
        /// <returns>bool值，动画是否成功</returns>
        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr whnd, int dwtime, int dwflag);

    }
    partial class ContorlSafeCallHelper
    {
        static void ShowControlMaskByContextMenu(this Control c, string txt, System.Drawing.Bitmap display, int x, int y, int width, int height) {
            //Action ac = () => {
            //    MyMaskLayerCtl maskctl = new MyMaskLayerCtl();
            //    maskctl.BackColor = System.Drawing.Color.Gray;
            //    maskctl.ShowText = true;
            //    maskctl.TextDisplay = txt;
            //    maskctl.Size = new System.Drawing.Size(width, height);
            //    maskctl.Location = new System.Drawing.Point(x, y);
            //    //maskctl.Dock = DockStyle.Fill;
            //    c.SuspendLayout();
            //    c.Controls.Add(maskctl);
            //    c.ResumeLayout();
            //    maskctl.BringToFront();
            //};
            //c.SafeAction(ac);
            //return;
            // 创建自定义控件
            Button customButton = new Button();
            customButton.Text = "自定义按钮";

            // 创建 ToolStripControlHost，将自定义控件添加到其中
            CustomToolStripControlHost customControlHost = new CustomToolStripControlHost(customButton);

            // 创建 ContextMenuStrip
            CustomContextMenuStrip contextMenuStrip = new CustomContextMenuStrip();
            contextMenuStrip.Name = "ShowControlMaskByContextMenu";
            // 添加自定义控件到 ContextMenuStrip 中
            contextMenuStrip.Items.Add(customControlHost);

            // 设置菜单尺寸为指定大小
            contextMenuStrip.AutoSize = false;
            contextMenuStrip.Size = new System.Drawing.Size(width, height);
            contextMenuStrip.Opacity = 0.5d;
            // 显示 ContextMenuStrip
            //c.ContextMenuStrip = contextMenuStrip;
            contextMenuStrip.Show(c, new System.Drawing.Point(x, y));
        }
    }

    public class CustomContextMenuStrip : ContextMenuStrip
    {
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
        }
        protected override void OnNotifyMessage(Message m) {
            base.OnNotifyMessage(m);
        }
        protected override void OnItemClicked(ToolStripItemClickedEventArgs e) {
            base.OnItemClicked(e);
            this.Dispose();
        }
        protected override void OnClosing(ToolStripDropDownClosingEventArgs e) {
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
    public class CustomToolStripControlHost : ToolStripControlHost
    {
        private bool manuallyClosed = true;

        public CustomToolStripControlHost(Control c) : base(c) {
            //c.LostFocus += ControlLostFocus;
        }

        protected override void OnSubscribeControlEvents(Control control) {
            //control.LostFocus += ControlLostFocus;
            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control) {
            //control.LostFocus -= ControlLostFocus;
            base.OnUnsubscribeControlEvents(control);
        }

        private void ControlLostFocus(object sender, EventArgs e) {
            if (!manuallyClosed) {
                base.Owner.Dispose();
                //ToolStripDropDown.Close(ToolStripDropDownCloseReason.AppClicked);
            }
        }

        //public new void Show() {
        //    manuallyClosed = false;
        //    base.Show();
        //}

        //public new void Close() {
        //    manuallyClosed = true;
        //    base.Close();
        //}
    }
}
