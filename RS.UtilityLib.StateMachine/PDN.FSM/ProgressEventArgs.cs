/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace RS.UtilityLib.StateMachine.PDN
{
    public class ProgressEventArgs
        : System.EventArgs
    {
        private double percent;
        public double Percent
        {
            get
            {
                return percent;
            }
        }

        public ProgressEventArgs(double percent)
        {
            this.percent = percent;
        }
    }
}
