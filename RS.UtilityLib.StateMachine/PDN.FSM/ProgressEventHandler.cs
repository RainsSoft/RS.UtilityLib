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
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
    public delegate void EventHandler<T>(object sender, EventArgs<T> e);
    public class EventArgs<T>
       : EventArgs
    {
        private T data;
        public T Data {
            get {
                return data;
            }
        }

        public EventArgs(T data) {
            this.data = data;
        }
    }
}
