/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace RS.UtilityLib.WinFormCommon.UISystemLayer
{
    public enum ResamplingAlgorithm
    {
        NearestNeighbor,
        Bilinear,
        Bicubic,
        SuperSampling
    }
}
