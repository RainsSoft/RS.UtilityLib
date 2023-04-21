using System;
using System.Collections.Generic;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public enum RibbonImageLayout
    {
        /// <summary>
        /// 平铺
        /// </summary>
        Title = 0,
        /// <summary>
        /// 固定数值的行列显示
        /// </summary>
        Table = 1,
    }

    public enum CaptionPosition
    {
        Top = 0,
        Bottom = 1
    }
    internal enum MouseState
    {
        Disable,
        Out,
        Hover,
        Down
    }

    public enum ColorScheme
    {
        Blue,
        Gray,
        Custom
    }
    public enum ControlAnimationType
    {
        Trans,

    }
}
