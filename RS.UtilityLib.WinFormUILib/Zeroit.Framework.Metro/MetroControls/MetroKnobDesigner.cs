// ***********************************************************************
// Assembly         : Zeroit.Framework.Metro
// Author           : ZEROIT
// Created          : 11-29-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-18-2018
// ***********************************************************************
// <copyright file="MetroKnobDesigner.cs" company="Zeroit Dev Technologies">
//    This program is for creating Metro Style controls.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;

namespace Zeroit.Framework.Metro
{

    #region Smart Tag Code

    #region Cut and Paste it on top of the component class

    //--------------- [Designer(typeof(ZeroitMetroKnobDesigner))] --------------------//
    #endregion

    #region ControlDesigner
    /// <summary>
    /// Class ZeroitMetroKnobDesigner.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Design.ControlDesigner" />
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class ZeroitMetroKnobDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// The action lists
        /// </summary>
        private DesignerActionListCollection actionLists;

        // Use pull model to populate smart tag menu.
        /// <summary>
        /// Gets the design-time action lists supported by the component associated with the designer.
        /// </summary>
        /// <value>The action lists.</value>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(new ZeroitMetroKnobSmartTagActionList(this.Component));
                }
                return actionLists;
            }
        }

        #region Zeroit Filter (Remove Properties)
        /// <summary>
        /// Remove Button and Control properties that are
        /// not supported by the <see cref="MACButton" />.
        /// </summary>
        /// <param name="Properties">The properties.</param>
        protected override void PostFilterProperties(IDictionary Properties)
        {
            //Properties.Remove("AllowDrop");
            //Properties.Remove("FlatStyle");
            //Properties.Remove("ForeColor");
            //Properties.Remove("ImageIndex");
            //Properties.Remove("ImageList");
        }
        #endregion

    }

    #endregion

    #region SmartTagActionList
    /// <summary>
    /// Class ZeroitMetroKnobSmartTagActionList.
    /// </summary>
    /// <seealso cref="System.ComponentModel.Design.DesignerActionList" />
    public class ZeroitMetroKnobSmartTagActionList : System.ComponentModel.Design.DesignerActionList
    {
        //Replace SmartTag with the Component Class Name. In this case the component class name is SmartTag
        /// <summary>
        /// The col user control
        /// </summary>
        private ZeroitMetroKnob colUserControl;


        /// <summary>
        /// The designer action UI SVC
        /// </summary>
        private DesignerActionUIService designerActionUISvc = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroitMetroKnobSmartTagActionList"/> class.
        /// </summary>
        /// <param name="component">A component related to the <see cref="T:System.ComponentModel.Design.DesignerActionList" />.</param>
        public ZeroitMetroKnobSmartTagActionList(IComponent component) : base(component)
        {
            this.colUserControl = component as ZeroitMetroKnob;

            // Cache a reference to DesignerActionUIService, so the 
            // DesigneractionList can be refreshed. 
            this.designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        // Helper method to retrieve control properties. Use of GetProperties enables undo and menu updates to work properly.
        /// <summary>
        /// Gets the name of the property by.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <returns>PropertyDescriptor.</returns>
        /// <exception cref="System.ArgumentException">Matching ColorLabel property not found!</exception>
        private PropertyDescriptor GetPropertyByName(String propName)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(colUserControl)[propName];
            if (null == prop)
                throw new ArgumentException("Matching ColorLabel property not found!", propName);
            else
                return prop;
        }

        #region Properties that are targets of DesignerActionPropertyItem entries.

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>The color of the back.</value>
        public Color BackColor
        {
            get
            {
                return colUserControl.BackColor;
            }
            set
            {
                GetPropertyByName("BackColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        public Color ForeColor
        {
            get
            {
                return colUserControl.ForeColor;
            }
            set
            {
                GetPropertyByName("ForeColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [draw line shadow].
        /// </summary>
        /// <value><c>true</c> if [draw line shadow]; otherwise, <c>false</c>.</value>
        public bool DrawLineShadow
        {
            get
            {
                return colUserControl.DrawLineShadow;
            }
            set
            {
                GetPropertyByName("DrawLineShadow").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the accent.
        /// </summary>
        /// <value>The color of the accent.</value>
        public Color AccentColor
        {
            get
            {
                return colUserControl.AccentColor;
            }
            set
            {
                GetPropertyByName("AccentColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public Color BorderColor
        {
            get
            {
                return colUserControl.BorderColor;
            }
            set
            {
                GetPropertyByName("BorderColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the default color.
        /// </summary>
        /// <value>The default color.</value>
        public Color DefaultColor
        {
            get
            {
                return colUserControl.DefaultColor;
            }
            set
            {
                GetPropertyByName("DefaultColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>The color of the fill.</value>
        public Color FillColor
        {
            get
            {
                return colUserControl.FillColor;
            }
            set
            {
                GetPropertyByName("FillColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the gradient.
        /// </summary>
        /// <value>The color of the gradient.</value>
        public Color GradientColor
        {
            get
            {
                return colUserControl.GradientColor;
            }
            set
            {
                GetPropertyByName("GradientColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        public Color LineColor
        {
            get
            {
                return colUserControl.LineColor;
            }
            set
            {
                GetPropertyByName("LineColor").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>The fill mode.</value>
        public ZeroitMetroKnob.KnobFillModes FillMode
        {
            get
            {
                return colUserControl.FillMode;
            }
            set
            {
                GetPropertyByName("FillMode").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the hatch style.
        /// </summary>
        /// <value>The hatch style.</value>
        public System.Drawing.Drawing2D.HatchStyle HatchStyle
        {
            get
            {
                return colUserControl.HatchStyle;
            }
            set
            {
                GetPropertyByName("HatchStyle").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the knob style.
        /// </summary>
        /// <value>The knob style.</value>
        public ZeroitMetroKnob.KnobStyles KnobStyle
        {
            get
            {
                return colUserControl.KnobStyle;
            }
            set
            {
                GetPropertyByName("KnobStyle").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public Design.Style Style
        {
            get
            {
                return colUserControl.Style;
            }
            set
            {
                GetPropertyByName("Style").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the line.
        /// </summary>
        /// <value>The length of the line.</value>
        public int LineLength
        {
            get
            {
                return colUserControl.LineLength;
            }
            set
            {
                GetPropertyByName("LineLength").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        /// <value>The width of the line.</value>
        public int LineWidth
        {
            get
            {
                return colUserControl.LineWidth;
            }
            set
            {
                GetPropertyByName("LineWidth").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public int Maximum
        {
            get
            {
                return colUserControl.Maximum;
            }
            set
            {
                GetPropertyByName("Maximum").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public int Minimum
        {
            get
            {
                return colUserControl.Minimum;
            }
            set
            {
                GetPropertyByName("Minimum").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the blocked angle.
        /// </summary>
        /// <value>The blocked angle.</value>
        public float BlockedAngle
        {
            get
            {
                return colUserControl.BlockedAngle;
            }
            set
            {
                GetPropertyByName("BlockedAngle").SetValue(colUserControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value
        {
            get
            {
                return colUserControl.Value;
            }
            set
            {
                GetPropertyByName("Value").SetValue(colUserControl, value);
            }
        }






        #endregion

        #region DesignerActionItemCollection

        /// <summary>
        /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem" /> objects contained in the list.
        /// </summary>
        /// <returns>A <see cref="T:System.ComponentModel.Design.DesignerActionItem" /> array that contains the items in this list.</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            //Define static section header entries.
            items.Add(new DesignerActionHeaderItem("Appearance"));

            items.Add(new DesignerActionPropertyItem("DrawLineShadow",
                                 "Show Line Shadow", "Appearance",
                                 "Set to show the line shadow."));

            items.Add(new DesignerActionPropertyItem("AccentColor",
                                 "Accent Color", "Appearance",
                                 "Sets the accent color."));

            items.Add(new DesignerActionPropertyItem("BorderColor",
                                 "Border Color", "Appearance",
                                 "Sets the border color."));

            items.Add(new DesignerActionPropertyItem("DefaultColor",
                                 "Default Color", "Appearance",
                                 "Sets the default color."));

            items.Add(new DesignerActionPropertyItem("FillColor",
                "Fill Color", "Appearance",
                "Sets the fill color."));

            items.Add(new DesignerActionPropertyItem("GradientColor",
                "Gradient Color", "Appearance",
                "Sets the gradient color."));

            items.Add(new DesignerActionPropertyItem("LineColor",
                "Line Color", "Appearance",
                "Sets the line color."));

            items.Add(new DesignerActionPropertyItem("FillMode",
                "Fill Mode", "Appearance",
                "Sets the fill mode."));

            items.Add(new DesignerActionPropertyItem("HatchStyle",
                "Hatch Style", "Appearance",
                "Sets the hatch style."));

            items.Add(new DesignerActionPropertyItem("KnobStyle",
                "Knob Style", "Appearance",
                "Sets the knob style."));

            items.Add(new DesignerActionPropertyItem("Style",
                "Style", "Appearance",
                "Sets the style."));

            items.Add(new DesignerActionPropertyItem("LineLength",
                "Line Length", "Appearance",
                "Sets the line length."));

            items.Add(new DesignerActionPropertyItem("LineWidth",
                "Line Width", "Appearance",
                "Set the line width."));

            items.Add(new DesignerActionPropertyItem("Maximum",
                "Maximum", "Appearance",
                "Sets the maximum value."));

            items.Add(new DesignerActionPropertyItem("Minimum",
                "Minimum", "Appearance",
                "Sets the minimum value."));

            items.Add(new DesignerActionPropertyItem("BlockedAngle",
                "Blocked Angle", "Appearance",
                "Sets the blocked angle."));

            items.Add(new DesignerActionPropertyItem("Value",
                "Value", "Appearance",
                "Sets the value."));
            

            //Create entries for static Information section.
            StringBuilder location = new StringBuilder("Product: ");
            location.Append(colUserControl.ProductName);
            StringBuilder size = new StringBuilder("Version: ");
            size.Append(colUserControl.ProductVersion);
            items.Add(new DesignerActionTextItem(location.ToString(),
                             "Information"));
            items.Add(new DesignerActionTextItem(size.ToString(),
                             "Information"));

            return items;
        }

        #endregion




    }

    #endregion

    #endregion

}