// *****************************************************************************
// BSD 3-Clause License (https://github.com/ComponentFactory/Krypton/blob/master/LICENSE)
//  © Component Factory Pty Ltd, 2006 - 2016, All rights reserved.
// The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to license terms.
// 
//  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV), et al. 2017 - 2021. All rights reserved. (https://github.com/Krypton-Suite/Standard-Toolkit)
//  Version 6.0.0  
// *****************************************************************************

using System;
using System.Drawing;

namespace Krypton.Toolkit
{
    /// <summary>
    /// Color event data.
    /// </summary>
    public class ColorEventArgs : EventArgs
    {
        #region Instance Fields

        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ColorEventArgs class.
        /// </summary>
        /// <param name="color">Color associated with the event.</param>
        public ColorEventArgs(Color color)
        {
            Color = color;
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color { get; }

        #endregion
    }
}
