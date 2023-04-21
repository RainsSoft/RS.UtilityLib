using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
   
    //
    public delegate void RibbonPopupEventHandler(object sender);
   
    public interface RibbonUISkin
    {
        string Name {
            get;
        }
        /// <summary>
        /// 一般状态下 图片位置与 宽高  left top width height
        /// </summary>
        Rectangle RectNormal { get; set; }
        /// <summary>
        /// 鼠标放上去状态下  图片位置与 宽高  left top width height
        /// </summary>
        Rectangle RectHover { get; set; }
        /// <summary>
        /// 鼠标放上去按下状态下  图片位置与 宽高  left top width height
        /// </summary>
        Rectangle RectDown { get; set; }
        /// <summary>
        /// Enabel=false 状态下 图片位置与 宽高  left top width height
        /// </summary>
        Rectangle RectDisable { get; set; }
        /// <summary>
        /// 用于皮肤的路径 主要是能从该图片中抠出我们想要的图
        /// </summary>
        string ImagePath { get; set; }
        /// <summary>
        /// 更新设置,具体有什么作用，主要看控件的功能
        /// </summary>
        void UpdateSet();
        /// <summary>
        /// 设置标志 0表示默认，1图片，2颜色（渐变），3图片+颜色（渐变）
        /// </summary>
        int SetFlag { get; set; }
        /// <summary>
        /// 默认渐变系数
        /// </summary>
        float ColorFactor { get; set; }
        /// <summary>
        /// 用户指定颜色（如果不指定,这使用系统统一背景色+渐变系数指定的终止色）
        /// </summary>
        bool ColorUserSet { get; set; }

        Color ColorStart { get; set; }
        float ColorLinearAngle { get; set; }
        Color ColorEnd { get; set; }
        /// <summary>
        /// 图片的布局方式.
        /// 0:原始;1:居中;2:拉伸;3:平铺;4:纵向拉伸横向平铺;5:横向拉伸纵向平铺
        /// </summary>
        int ImageLayout {
            get;
            set;
        }
        /// <summary>
        /// 对齐方式.
        /// 0:原始;1:左停靠;2:右停靠;3:上停靠;4:下停靠;5:填充
        /// </summary>
        int DockFlag {
            get;
            set;
        }
        /// <summary>
        /// Z值.决定了先后关系.越大则越前.
        /// </summary>
        int ZOrder {
            get;
            set;
        }
    }
}
