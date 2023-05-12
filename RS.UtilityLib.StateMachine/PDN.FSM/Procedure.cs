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
    public delegate void Procedure();
    public delegate void Procedure<T>(T t);
    public delegate void Procedure<T, U>(T t, U u);
    public delegate void Procedure<T, U, V>(T t, U u, V v);
    public delegate void Procedure<T, U, V, W>(T t, U u, V v, W w);
    //

}
