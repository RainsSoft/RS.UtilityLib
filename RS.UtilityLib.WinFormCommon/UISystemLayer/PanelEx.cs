/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    public class PanelEx : 
        UISystemLayer.ScrollPanel
    {
        private bool hideHScroll = false;

        public bool HideHScroll
        {
            get
            {
                return this.hideHScroll;
            }

            set
            {
                this.hideHScroll = value;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.hideHScroll)
            {
                UISystemLayer.UI.SuspendControlPainting(this);
            }

            base.OnSizeChanged(e);

            if (this.hideHScroll)
            {
                UISystemLayer.UI.HideHorizontalScrollBar(this);
                UISystemLayer.UI.ResumeControlPainting(this);
                Invalidate(true);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //base.OnMouseWheel(e);
        }
    }
}
