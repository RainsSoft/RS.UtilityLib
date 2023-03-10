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

using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Krypton.Toolkit
{
    internal class KryptonListBoxActionList : DesignerActionList
    {
        #region Instance Fields
        private readonly KryptonListBox _listBox;
        private readonly IComponentChangeService _service;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the KryptonListBoxActionList class.
        /// </summary>
        /// <param name="owner">Designer that owns this action list instance.</param>
        public KryptonListBoxActionList(KryptonListBoxDesigner owner)
            : base(owner.Component)
        {
            // Remember the list box instance
            _listBox = owner.Component as KryptonListBox;

            // Cache service used to notify when a property has changed
            _service = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets and sets the syle used for list items.
        /// </summary>
        public ButtonStyle ItemStyle
        {
            get => _listBox.ItemStyle;

            set
            {
                if (_listBox.ItemStyle != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.ItemStyle, value);
                    _listBox.ItemStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the background drawing style.
        /// </summary>
        public PaletteBackStyle BackStyle
        {
            get => _listBox.BackStyle;

            set
            {
                if (_listBox.BackStyle != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.BackStyle, value);
                    _listBox.BackStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the border drawing style.
        /// </summary>
        public PaletteBorderStyle BorderStyle
        {
            get => _listBox.BorderStyle;

            set
            {
                if (_listBox.BorderStyle != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.BorderStyle, value);
                    _listBox.BorderStyle = value;
                }
            }
        }

        /// <summary>Gets or sets the context menu strip.</summary>
        /// <value>The context menu strip.</value>
        public ContextMenuStrip ContextMenuStrip
        {
            get => _listBox.ContextMenuStrip;

            set
            {
                if (_listBox.ContextMenuStrip != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.ContextMenuStrip, value);

                    _listBox.ContextMenuStrip = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the selection mode.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get => _listBox.SelectionMode;

            set
            {
                if (_listBox.SelectionMode != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.SelectionMode, value);
                    _listBox.SelectionMode = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the selection mode.
        /// </summary>
        public bool Sorted
        {
            get => _listBox.Sorted;

            set
            {
                if (_listBox.Sorted != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.Sorted, value);
                    _listBox.Sorted = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the palette mode.
        /// </summary>
        public PaletteMode PaletteMode
        {
            get => _listBox.PaletteMode;

            set
            {
                if (_listBox.PaletteMode != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.PaletteMode, value);
                    _listBox.PaletteMode = value;
                }
            }
        }

        /// <summary>Gets or sets the font.</summary>
        /// <value>The font.</value>
        public Font ShortTextFont
        {
            get => _listBox.StateCommon.Item.Content.ShortText.Font;

            set
            {
                if (_listBox.StateCommon.Item.Content.ShortText.Font != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.StateCommon.Item.Content.ShortText.Font, value);

                    _listBox.StateCommon.Item.Content.ShortText.Font = value;
                }
            }
        }

        /// <summary>Gets or sets the font.</summary>
        /// <value>The font.</value>
        public Font LongTextFont
        {
            get => _listBox.StateCommon.Item.Content.LongText.Font;

            set
            {
                if (_listBox.StateCommon.Item.Content.LongText.Font != value)
                {
                    _service.OnComponentChanged(_listBox, null, _listBox.StateCommon.Item.Content.LongText.Font, value);

                    _listBox.StateCommon.Item.Content.LongText.Font = value;
                }
            }
        }
        #endregion

        #region Public Override
        /// <summary>
        /// Returns the collection of DesignerActionItem objects contained in the list.
        /// </summary>
        /// <returns>A DesignerActionItem array that contains the items in this list.</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            // Create a new collection for holding the single item we want to create
            DesignerActionItemCollection actions = new DesignerActionItemCollection();

            // This can be null when deleting a control instance at design time
            if (_listBox != null)
            {
                // Add the list of list box specific actions
                actions.Add(new DesignerActionHeaderItem("Appearance"));
                actions.Add(new DesignerActionPropertyItem("BackStyle", "Back Style", "Appearance", "Style used to draw background."));
                actions.Add(new DesignerActionPropertyItem("BorderStyle", "Border Style", "Appearance", "Style used to draw the border."));
                actions.Add(new DesignerActionPropertyItem("ContextMenuStrip", "Context Menu Strip", "Appearance", "The context menu strip for the control."));
                actions.Add(new DesignerActionPropertyItem("ItemStyle", "Item Style", "Appearance", "How to display list items."));
                actions.Add(new DesignerActionPropertyItem("ShortTextFont", "Short Text Font", "Appearance", "The short text font."));
                actions.Add(new DesignerActionPropertyItem("LongTextFont", "Long Text Font", "Appearance", "The long text font."));
                actions.Add(new DesignerActionHeaderItem("Behavior"));
                actions.Add(new DesignerActionPropertyItem("SelectionMode", "Selection Mode", "Behavior", "Determines the selection mode."));
                actions.Add(new DesignerActionPropertyItem("Sorted", "Sorted", "Behavior", "Should items be sorted according to string."));
                actions.Add(new DesignerActionHeaderItem("Visuals"));
                actions.Add(new DesignerActionPropertyItem("PaletteMode", "Palette", "Visuals", "Palette applied to drawing"));
            }

            return actions;
        }
        #endregion
    }
}
