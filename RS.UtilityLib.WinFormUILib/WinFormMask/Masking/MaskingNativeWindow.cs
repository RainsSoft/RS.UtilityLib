using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 蒙版弹层NativeWindow
    /// 
    /// 提供窗口句柄和窗口过程的低级封装
    /// 使用分层窗体为例能绘制透明。
    /// </summary>
    internal class MaskingNativeWindow : NativeWindow, IDpiControl, IDisposable
    {
        #region 属性

        private float scaleDpi = 1.0f;
        /// <summary>
        /// 控件当前缩放Dpi
        /// </summary>
        [Browsable(false)]
        [Localizable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual float ScaleDpi
        {
            get { return this.scaleDpi; }
            set
            {
                if (value <= 0)
                    return;

                this.scaleDpi = value;
                this.OnScaleDpiChangedInitialize();
            }
        }

        /// <summary>
        /// 蒙版弹层的显示状态
        /// </summary>
        public bool Visible
        {
            get
            {
                if (this.Handle == IntPtr.Zero)
                    return false;

                return NativeMethods.IsWindowVisible(this.Handle);
            }
        }

        /// <summary>
        /// 控件大小
        /// </summary>
        public Size Size
        {
            get { return this.Bounds.Size; }
            set
            {
                NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, value.Width, value.Height, SWP_NOMOVE | SWP_NOACTIVATE);
                this.Invalidate();
            }
        }

        /// <summary>
        /// 控件位置
        /// </summary>
        public Point Location
        {
            get { return this.Bounds.Location; }
            set
            {
                NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, value.X, value.Y, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE);
                this.Invalidate();
            }
        }

        /// <summary>
        /// 控件大小和位置
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                if (this.Handle == IntPtr.Zero)
                    return Rectangle.Empty;

                NativeMethods.RECT r = new NativeMethods.RECT();
                NativeMethods.GetWindowRect(this.Handle, ref r);
                return new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
            }
        }

        #endregion

        #region 字段

        /// <summary>
        /// 蒙版拥有者
        /// </summary>
        private Form ownerForm;

        #endregion

        #region 扩展

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int SWP_HIDEWINDOW = 0x0080;
        private const int WM_GETOBJECT = 0x003D;
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_MOUSEACTIVATE = 0x0021;//鼠标激活窗体
        private const int MA_NOACTIVATE = 3;// 不激活窗口，但也不会放弃鼠标消息

        private const int WS_CAPTION = 0x00C00000;
        private const int WS_CLIPSIBLINGS = 0x04000000;
        private const int WS_POPUP = unchecked((int)0x80000000);
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int WS_EX_CONTROLPARENT = 0x00010000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x80000;
        private const int CS_SAVEBITS = 0x0800;
        private const int GWL_EXSTYLE = (-20);
        private const int GWL_HWNDPARENT = (-8);

        private const int ULW_ALPHA = 0x00000002;//使用 pblend 作為混合函式
        private const byte AC_SRC_OVER = 0;//定義要執行的混合作業
        private const byte AC_SRC_ALPHA = 1;//混合作業方式

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteObject(IntPtr ho);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        #endregion

        public MaskingNativeWindow(Form ownerForm)
        {
            //设置弹层拥有者
            this.ownerForm = ownerForm;

            //设置窗体样式（分层窗体、禁止出现在导航列表）
            CreateParams cp = new CreateParams();
            cp.Style &= ~(WS_CAPTION | WS_CLIPSIBLINGS);
            cp.ExStyle &= ~(WS_EX_APPWINDOW);
            cp.Style |= WS_POPUP;
            cp.ExStyle |= (WS_EX_CONTROLPARENT);
            cp.ExStyle |= WS_EX_LAYERED;
            cp.ClassStyle |= CS_SAVEBITS;

            //创建控件句柄
            this.CreateHandle(cp);

            //禁止出啊现在任务栏
            int flags = unchecked((int)(long)NativeMethods.GetWindowLong(this.Handle, GWL_EXSTYLE)) | WS_EX_TOOLWINDOW;
            NativeMethods.SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)flags);

        }

        #region 重写

        protected override void OnHandleChange()
        {
            base.OnHandleChange();

            this.ScaleDpi = DpiHelper.GetControlDpi(this.Handle);
        }

        protected virtual void OnScaleDpiChangedInitialize()
        {
            this.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_MOUSEACTIVATE://禁止激活窗体但接受鼠标事件
                    {
                        m.Result = new IntPtr(MA_NOACTIVATE);
                        return;
                    }
                case WM_SETCURSOR://设置光标
                    {
                        DefWndProc(ref m);

                        if (Cursor.Current != Cursors.Default)
                        {
                            Cursor.Current = Cursors.Default;
                        }
                        break;
                    }
                default:
                    {
                        this.DefWndProc(ref m);
                        break;
                    }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //清理托管资源
            }
            this.DestroyHandle();
            //清理非托管资源
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 显示蒙版弹层
        /// </summary>
        public void Show()
        {
            NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            NativeMethods.SetWindowLong(this.Handle, GWL_HWNDPARENT, this.ownerForm.Handle);
            this.Invalidate();
        }

        /// <summary>
        /// 显示蒙版弹层
        /// </summary>
        /// <param name="bounds"></param>
        public void Show(Rectangle bounds)
        {
            NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, bounds.X, bounds.Y, bounds.Width, bounds.Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
            NativeMethods.SetWindowLong(this.Handle, GWL_HWNDPARENT, this.ownerForm.Handle);
            this.Invalidate();
        }

        /// <summary>
        /// 隐藏蒙版弹层
        /// </summary>
        public void Hide()
        {
            NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_HIDEWINDOW);
        }

        /// <summary>
        /// 绘制分层窗体
        /// </summary>
        public void Invalidate()
        {
            if (this.Handle == IntPtr.Zero || !this.Visible || this.ownerForm == null || (this.ownerForm != null && !this.ownerForm.Visible))
            {
                return;
            }

            MaskingObject mo = Masking.MaskingObjectCollection.Where(a => a.MaskingNativeWindow == this).FirstOrDefault();
            if (mo == null)
            {
                return;
            }

            Rectangle rect = this.Bounds;
            if (rect.Width == 0 || rect.Height == 0)
                return;

            #region 绘制界面图片

            MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
            int scale_brush_size = (int)(Math.Ceiling(maskingSetting.BrushSize * this.ScaleDpi) / 2f) * 2;
            PointF center_point = new PointF(rect.Width / 2f, rect.Height * 2f / 5f);

            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            #region 背景

            SolidBrush back_sb = new SolidBrush(maskingSetting.BackColor);
            g.FillRectangle(back_sb, g.VisibleClipBounds);
            back_sb.Dispose();

            #endregion

            #region 图案

            switch (maskingSetting.StylePattern)
            {
                case MaskingStylePattern.Line:
                    {
                        int current_index = mo.BrushStartIndex;
                        Pen line_pen = new Pen(Color.Transparent, scale_brush_size);
                        for (int i = 0; i < Masking.BrushCount; i++)
                        {
                            current_index = current_index % Masking.BrushCount;
                            PointF point_start = ControlHelper.CalculatePointForAngle(center_point, scale_brush_size * 1.8f, maskingSetting.BrushAngles[current_index]);
                            PointF point_end = ControlHelper.CalculatePointForAngle(center_point, scale_brush_size * 3.8f, maskingSetting.BrushAngles[current_index]);
                            line_pen.Color = maskingSetting.BrushColors[i];

                            g.DrawLine(line_pen, point_start, point_end);

                            current_index++;
                        }
                        line_pen.Dispose();

                        break;
                    }
                case MaskingStylePattern.Dot:
                    {
                        int current_index = mo.BrushStartIndex;
                        SolidBrush dot_sb = new SolidBrush(Color.Transparent);
                        for (int i = 0; i < Masking.BrushCount; i++)
                        {
                            current_index = current_index % Masking.BrushCount;
                            PointF point_start = ControlHelper.CalculatePointForAngle(center_point, scale_brush_size * 1.6f, maskingSetting.BrushAngles[current_index]);
                            dot_sb.Color = maskingSetting.BrushColors[i];

                            g.FillEllipse(dot_sb, point_start.X - scale_brush_size / 2, point_start.Y - scale_brush_size / 2, scale_brush_size, scale_brush_size);

                            current_index++;
                        }
                        dot_sb.Dispose();
                        break;
                    }
            }

            #endregion

            #region 文本

            string str = ((maskingSetting.TextAlignment == MaskingTextAlignment.Bottom) ? "" : " ") + (mo.MaskingSetting != null ? mo.Text : Masking.DefaultMaskingSetting.Text);
            if (!String.IsNullOrWhiteSpace(str))
            {
                int w_h = 0;
                if (maskingSetting.StylePattern == MaskingStylePattern.Line)
                {
                    PointF point_left = ControlHelper.CalculatePointForAngle(new PointF(0, 0), scale_brush_size * 3.8f, 180);
                    PointF point_right = ControlHelper.CalculatePointForAngle(new PointF(0, 0), scale_brush_size * 3.8f, 360);
                    w_h = (int)(point_right.X - point_left.X + scale_brush_size);
                }
                else
                {
                    PointF point_left = ControlHelper.CalculatePointForAngle(new PointF(0, 0), scale_brush_size * 1.6f, 180);
                    PointF point_right = ControlHelper.CalculatePointForAngle(new PointF(0, 0), scale_brush_size * 1.6f, 360);
                    w_h = (int)(point_right.X - point_left.X + scale_brush_size);
                }
                PointF text_point = new PointF(center_point.X + (int)Math.Ceiling(w_h / 2F), center_point.Y - maskingSetting.TextFont.Height / 3f);
                if (maskingSetting.TextAlignment == MaskingTextAlignment.Bottom)
                {
                    text_point = new PointF(center_point.X - Size.Ceiling(g.MeasureString(str, maskingSetting.TextFont, int.MaxValue, StringFormat.GenericTypographic)).Width / 2f, center_point.Y + (int)Math.Ceiling(w_h / 2F) + maskingSetting.TextFont.Height / 2f);
                }

                SolidBrush text_sb = new SolidBrush(maskingSetting.TextColor);
                g.DrawString(str, maskingSetting.TextFont, text_sb, text_point, StringFormat.GenericTypographic);
                text_sb.Dispose();
            }

            #endregion

            g.Dispose();

            #endregion

            #region  把界面图片绑定到窗口上

            IntPtr hdcDst = GetDC(this.Handle);
            IntPtr hdcSrc = CreateCompatibleDC(hdcDst);

            IntPtr newBitmap = bitmap.GetHbitmap(Color.FromArgb(0));//创建一张位图
            IntPtr oldBitmap = SelectObject(hdcSrc, newBitmap);//位图绑定到DC设备上

            Point pptDst = new Point(rect.Left, rect.Top);
            Size psize = new Size(bitmap.Width, bitmap.Height);
            Point pptSrc = new Point(0, 0);

            BLENDFUNCTION pblend = new BLENDFUNCTION();
            pblend.BlendOp = AC_SRC_OVER;
            pblend.SourceConstantAlpha = 255;
            pblend.AlphaFormat = AC_SRC_ALPHA;
            pblend.BlendFlags = 0;

            UpdateLayeredWindow(this.Handle, hdcDst, ref pptDst, ref psize, hdcSrc, ref pptSrc, 0, ref pblend, ULW_ALPHA);

            SelectObject(hdcSrc, oldBitmap);
            ReleaseDC(this.Handle, hdcDst);
            DeleteObject(newBitmap);
            DeleteDC(hdcSrc);

            bitmap.Dispose();

            #endregion
        }

        /// <summary>
        /// 蒙版弹层拥有者Resize事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Owner_Resize(object sender, EventArgs e)
        {
            List<MaskingObject> moList = Masking.MaskingObjectCollection.Where(a => a.OwnerForm == sender).ToList<MaskingObject>();
            foreach (MaskingObject mo in moList)
            {
                MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
                mo.MaskingNativeWindow.Size = new Size(mo.OwnerForm.ClientRectangle.Width - maskingSetting.Padding.Left - maskingSetting.Padding.Right, mo.OwnerForm.ClientRectangle.Height - maskingSetting.Padding.Top - maskingSetting.Padding.Bottom);
            }
        }

        /// <summary>
        /// 蒙版弹层拥有者所在控件链LocationChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OwnerControlChain_LocationChanged(object sender, EventArgs e)
        {
            List<MaskingObject> moList = Masking.MaskingObjectCollection.Where(a => a.ControlChain.Contains(sender)).ToList<MaskingObject>();
            foreach (MaskingObject mo in moList)
            {
                MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
                Point point = mo.OwnerForm.PointToScreen(mo.OwnerForm.ClientRectangle.Location);
                mo.MaskingNativeWindow.Location = new Point(point.X + maskingSetting.Padding.Left, point.Y + maskingSetting.Padding.Top);
            }
        }

        /// <summary>
        /// 蒙版弹层拥有者所在控件链VisibleChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OwnerControlChain_VisibleChanged(object sender, EventArgs e)
        {
            List<MaskingObject> moList = Masking.MaskingObjectCollection.Where(a => a.ControlChain.Contains(sender)).ToList<MaskingObject>();
            foreach (MaskingObject mo in moList)
            {
                if (mo.ControlChain.Where(a => a.Visible == false).Count() > 0)
                {
                    mo.MaskingNativeWindow.Hide();
                }
                else
                {
                    MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
                    Point point = mo.OwnerForm.PointToScreen(mo.OwnerForm.ClientRectangle.Location);
                    mo.MaskingNativeWindow.Show(new Rectangle(point.X + maskingSetting.Padding.Left, point.Y + maskingSetting.Padding.Top, mo.OwnerForm.ClientRectangle.Width - maskingSetting.Padding.Left - maskingSetting.Padding.Right, mo.OwnerForm.ClientRectangle.Height - maskingSetting.Padding.Top - maskingSetting.Padding.Bottom));

                }
            }
        }

        /// <summary>
        /// 蒙版弹层拥有者所在控件链ParentChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OwnerControlChain_ParentChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            List<MaskingObject> moList = Masking.MaskingObjectCollection.Where(a => a.ControlChain.Contains(sender)).ToList<MaskingObject>();
            foreach (MaskingObject mo in moList)
            {
                int index = mo.ControlChain.IndexOf(control);
                for (int k = index + 1; k < mo.ControlChain.Count; k++)
                {
                    mo.ControlChain[k].LocationChanged -= mo.MaskingNativeWindow.OwnerControlChain_LocationChanged;
                    mo.ControlChain[k].VisibleChanged -= mo.MaskingNativeWindow.OwnerControlChain_VisibleChanged;
                    mo.ControlChain[k].ParentChanged -= mo.MaskingNativeWindow.OwnerControlChain_ParentChanged;
                    mo.ControlChain[k].Disposed -= mo.MaskingNativeWindow.OwnerControlChain_Dispose;
                }

                List<Control> controlChain_tmp = new List<Control>();
                for (int j = 0; j <= index; j++)
                {
                    controlChain_tmp.Add(mo.ControlChain[j]);
                }
                if (control.Parent != null)
                {
                    Control tmp = control.Parent;
                    while (tmp != null)
                    {
                        tmp.LocationChanged += mo.MaskingNativeWindow.OwnerControlChain_LocationChanged;
                        tmp.VisibleChanged += mo.MaskingNativeWindow.OwnerControlChain_VisibleChanged;
                        tmp.ParentChanged += mo.MaskingNativeWindow.OwnerControlChain_ParentChanged;
                        tmp.Disposed += mo.MaskingNativeWindow.OwnerControlChain_Dispose;
                        controlChain_tmp.Add(control);
                        tmp = tmp.Parent;
                    }
                }
                mo.ControlChain = controlChain_tmp;

                bool isvisible = true;
                Control control_tmp = mo.OwnerForm;
                while (true)
                {
                    if (control_tmp.Visible == false)
                    {
                        isvisible = false;
                        goto result;
                    }

                    if (control_tmp.Parent == null)
                    {
                        break;
                    }

                    control_tmp = control_tmp.Parent;
                }
                if (isvisible && (!(control_tmp is Form) || !((Form)control_tmp).TopLevel))
                {
                    isvisible = false;
                }

            result:
                {
                    if (isvisible)
                    {
                        MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
                        Point point = mo.OwnerForm.PointToScreen(mo.OwnerForm.ClientRectangle.Location);
                        mo.MaskingNativeWindow.Show(new Rectangle(point.X + maskingSetting.Padding.Left, point.Y + maskingSetting.Padding.Top, mo.OwnerForm.ClientRectangle.Width - maskingSetting.Padding.Left - maskingSetting.Padding.Right, mo.OwnerForm.ClientRectangle.Height - maskingSetting.Padding.Top - maskingSetting.Padding.Bottom));
                    }
                    else
                    {
                        mo.MaskingNativeWindow.Hide();
                    }
                }
            }
        }

        /// <summary>
        /// 蒙版弹层拥有者所在控件链Dispose事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OwnerControlChain_Dispose(object sender, EventArgs e)
        {
            Masking.Hide(Masking.MaskingObjectCollection.Where(a => a.ControlChain.Contains(sender)).ToList<MaskingObject>());
        }

        /// <summary>
        /// 显式调用
        /// </summary>
        public void Dispose()
        {
            Dispose(true);//释放托管和非托管资源
            System.GC.SuppressFinalize(this);//阻止GC调用析构函数
        }

        #endregion

        //GC调用的析构函数
        ~MaskingNativeWindow()
        {
            Dispose(false);//释放非托管资源
        }

    }
}
