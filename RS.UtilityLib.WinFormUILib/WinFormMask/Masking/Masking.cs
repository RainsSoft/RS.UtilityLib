using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 蒙版弹层
    /// </summary>
    public static class Masking
    {
        #region 属性

        private static MaskingSettings defaultMaskingSetting = new MaskingSettings(MaskingStylePattern.Dot, new Padding(0), Color.FromArgb(150, 0, 0, 0), Color.White, "加载中...", MaskingTextAlignment.Right, new Font("宋体", 11), Color.White);
        /// <summary>
        /// 加载等待蒙版全局配置
        /// </summary>
        public static MaskingSettings DefaultMaskingSetting
        {
            get { return defaultMaskingSetting; }
            set { defaultMaskingSetting = value; }
        }

        private static List<MaskingObject> maskingObjectCollection = new List<MaskingObject>();
        /// <summary>
        /// 存放蒙版信息集合
        /// </summary>
        internal static List<MaskingObject> MaskingObjectCollection
        {
            get { return maskingObjectCollection; }
            set { maskingObjectCollection = value; }
        }

        /// <summary>
        /// 画笔的数量
        /// </summary>
        internal static int BrushCount
        {
            get { return 8; }
        }

        #endregion

        #region 字段

        /// <summary>
        /// 旋转定时器
        /// </summary>
        private static Timer rotateTimer;

        #endregion

        static Masking()
        {

        }

        #region 公开方法

        /// <summary>
        /// 在指定窗体上显示蒙版弹层
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        public static void Show(Form form)
        {
            Show(form, null, null);
        }

        /// <summary>
        /// 在指定窗体上显示蒙版弹层
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        /// <param name="text">文本</param>
        public static void Show(Form form, string text)
        {
            Show(form, text, null);
        }

        /// <summary>
        /// 在指定窗体上显示蒙版弹层
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        /// <param name="setting">蒙版弹层配置</param>
        public static void Show(Form form, MaskingSettings setting)
        {
            Show(form, null, setting);
        }

        /// <summary>
        /// 在指定窗体上显示蒙版弹层
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        /// <param name="text">文本</param>
        /// <param name="setting">蒙版弹层配置</param>
        public static void Show(Form form, string text, MaskingSettings setting)
        {
            Hide(form);

            MaskingNativeWindow mf = new MaskingNativeWindow(form);
            MaskingObject mo = new MaskingObject() { MaskingNativeWindow = mf, OwnerForm = form, MaskingSetting = setting, Text = text };

            mo.OwnerForm.Resize += mo.MaskingNativeWindow.Owner_Resize;
            ControlChainBindEvent(mo.OwnerForm, mo);

            MaskingObjectCollection.Add(mo);

            MaskingSettings maskingSetting = mo.MaskingSetting != null ? mo.MaskingSetting : Masking.DefaultMaskingSetting;
            Point point = form.PointToScreen(form.ClientRectangle.Location);
            mf.Show(new Rectangle(point.X + maskingSetting.Padding.Left, point.Y + maskingSetting.Padding.Top, form.ClientRectangle.Width - maskingSetting.Padding.Left - maskingSetting.Padding.Right, form.ClientRectangle.Height - maskingSetting.Padding.Top - maskingSetting.Padding.Bottom));

            if (rotateTimer == null)
            {
                rotateTimer = new Timer();
                rotateTimer.Interval = 20;
                rotateTimer.Tick += rotateTimer_Tick;
            }
            if (rotateTimer.Enabled == false)
            {
                rotateTimer.Enabled = true;
            }
        }

        /// <summary>
        /// 隐藏所有蒙版弹层
        /// </summary>
        public static void Hide()
        {
            Hide(MaskingObjectCollection);
        }

        /// <summary>
        /// 隐藏指定蒙版弹层
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        public static void Hide(Form form)
        {
            Hide(MaskingObjectCollection.Where(a => a.OwnerForm == form).FirstOrDefault());
        }

        /// <summary>
        /// 隐藏指定蒙版弹层
        /// </summary>
        /// <param name="mo">蒙版弹层信息存储对象</param>
        internal static void Hide(MaskingObject mo)
        {
            if (mo == null)
            {
                return;
            }

            mo.OwnerForm.Resize -= mo.MaskingNativeWindow.Owner_Resize;
            foreach (Control control in mo.ControlChain)
            {
                control.LocationChanged -= mo.MaskingNativeWindow.OwnerControlChain_LocationChanged;
                control.VisibleChanged -= mo.MaskingNativeWindow.OwnerControlChain_VisibleChanged;
                control.ParentChanged -= mo.MaskingNativeWindow.OwnerControlChain_ParentChanged;
                control.Disposed -= mo.MaskingNativeWindow.OwnerControlChain_Dispose;
            }
            mo.ControlChain.Clear();

            mo.MaskingNativeWindow.Hide();
            mo.MaskingNativeWindow.Dispose();

            MaskingObjectCollection.Remove(mo);

            if (MaskingObjectCollection.Count < 1)
            {
                rotateTimer.Enabled = false;
            }

        }

        /// <summary>
        /// 隐藏蒙版弹层集合
        /// </summary>
        /// <param name="moList">蒙版弹层信息存储对象集合</param>
        internal static void Hide(List<MaskingObject> moList)
        {
            if (moList == null)
            {
                return;
            }

            foreach (MaskingObject mo in moList)
            {
                mo.OwnerForm.Resize -= mo.MaskingNativeWindow.Owner_Resize;
                foreach (Control control in mo.ControlChain)
                {
                    control.LocationChanged -= mo.MaskingNativeWindow.OwnerControlChain_LocationChanged;
                    control.VisibleChanged -= mo.MaskingNativeWindow.OwnerControlChain_VisibleChanged;
                    control.ParentChanged -= mo.MaskingNativeWindow.OwnerControlChain_ParentChanged;
                    control.Disposed -= mo.MaskingNativeWindow.OwnerControlChain_Dispose;
                }
                mo.ControlChain.Clear();

                mo.MaskingNativeWindow.Hide();
                mo.MaskingNativeWindow.Dispose();
            }

            foreach (MaskingObject mo in moList)
            {
                MaskingObjectCollection.Remove(mo);
            }

            if (MaskingObjectCollection.Count < 1)
            {
                rotateTimer.Enabled = false;
            }

        }

        /// <summary>
        /// 更改蒙版弹层文本
        /// </summary>
        /// <param name="form">蒙版弹层拥有者</param>
        /// <param name="text">文本</param>
        public static void UpdateText(Form form, string text)
        {
            MaskingObject mo = MaskingObjectCollection.Where(a => a.OwnerForm == form).FirstOrDefault();
            if (mo != null)
            {
                mo.Text = text;
                mo.MaskingNativeWindow.Invalidate();
            }
        }

        /// <summary>
        /// 蒙版弹层拥有者所在控件链添加事件
        /// </summary>
        /// <param name="control">蒙版弹层拥有者</param>
        /// <param name="mo">蒙版弹层信息存储对象</param>
        internal static void ControlChainBindEvent(Control control, MaskingObject mo)
        {
            control.LocationChanged += mo.MaskingNativeWindow.OwnerControlChain_LocationChanged;
            control.VisibleChanged += mo.MaskingNativeWindow.OwnerControlChain_VisibleChanged;
            control.ParentChanged += mo.MaskingNativeWindow.OwnerControlChain_ParentChanged;
            control.Disposed += mo.MaskingNativeWindow.OwnerControlChain_Dispose;
            mo.ControlChain.Add(control);

            if (control.Parent != null)
            {
                ControlChainBindEvent(control.Parent, mo);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 旋转动画定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void rotateTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < MaskingObjectCollection.Count; i++)
            {
                MaskingObjectCollection[i].AnimationTime += rotateTimer.Interval;
                if (MaskingObjectCollection[i].AnimationTime >= 100)
                {
                    MaskingObjectCollection[i].AnimationTime = 0;

                    MaskingObjectCollection[i].BrushStartIndex += 1;
                    if (MaskingObjectCollection[i].BrushStartIndex > Masking.BrushCount - 1)
                    {
                        MaskingObjectCollection[i].BrushStartIndex -= Masking.BrushCount - 1;
                    }
                    MaskingObjectCollection[i].MaskingNativeWindow.Invalidate();
                }
            }
        }

        #endregion 

    }

}
