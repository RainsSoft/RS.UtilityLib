

using System;

namespace Silver.UI
{
    #region Enumerations

    // The value of the enumerations are the char values
    // of marlett font which will be used to draw scroll arrow
    // for each direction. Only up and down directions are used now.
    //
    // 01/10/2005 - Marlett font is no longer used for scroll buttons.
    
    [Serializable]
    public enum ScrollDirection
    {
        Left  = 3,
        Right = 4,
        Up    = 5,
        Down  = 6,
        /// <summary>
        /// ÷–º‰SCROLL_BAR
        /// </summary>
        Center=7,
    }

    [Serializable]
    public enum ViewMode
    {
        LargeIcons,
        SmallIcons,
        List,
    }

    [Serializable]
    public enum TextPosition
    {
        Top,
        Bottom,
        Hidden,
    }

    #endregion //Enumerations

}
//----------------------------------------------------------------------------
