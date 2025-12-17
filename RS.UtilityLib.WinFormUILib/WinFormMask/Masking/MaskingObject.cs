using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 蒙版弹层信息存储对象
    /// </summary>
    internal class MaskingObject
    {
        #region 属性

        /// <summary>
        /// 蒙版弹层
        /// </summary>
        public MaskingNativeWindow MaskingNativeWindow { get; set; }

        /// <summary>
        /// 蒙版弹层拥有者
        /// </summary>
        public Form OwnerForm { get; set; }

        /// <summary>
        /// 蒙版弹层事件监听的控件链（蒙版弹层拥有者为第一个）
        /// </summary>
        public List<Control> ControlChain { get; set; }

        /// <summary>
        /// 蒙版弹层配置 
        /// </summary>
        public MaskingSettings MaskingSetting { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 加载动画已累加的时间
        /// </summary>
        internal double AnimationTime { get; set; }

        /// <summary>
        /// 加载动画画笔开始索引
        /// </summary>
        internal int BrushStartIndex { get; set; }

        #endregion

        public MaskingObject()
        {
            this.ControlChain = new List<Control>();
        }

    }

}
