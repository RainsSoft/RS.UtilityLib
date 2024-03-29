﻿//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2018 Solutions Design. All rights reserved.
// https://github.com/SolutionsDesign/Algorithmia
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2018 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;


namespace SD.Tools.Algorithmia.GeneralDataStructures.EventArguments
{
	/// <summary>
	/// 自定义事件参数类，用于指示命令化成员中的更改
	/// Custom event argument class which is used to signal a change in a commandified member
	/// </summary>
	/// <remarks>当值是可变对象并且其内容发生突变时，包含该值的命令化成员将引发其 ValueChanged 事件。
	/// 在这种情况下，originalValue 和 newValue 将是相同的。
	/// When the value is a mutable object and its contents is mutated, a commandified member containing that value will raise its ValueChanged
	/// event. In that case, originalValue and newValue will be the same.</remarks>
	[Serializable]
	public class MemberChangedEventArgs<TChangeType, TValue> : EventArgs
	{
		/// <summary>
		/// CTor
		/// </summary>
		/// <param name="typeOfChange">The change type to pass on to subscribers</param>
		/// <param name="originalValue">The original value.</param>
		/// <param name="newValue">The new value.</param>
		public MemberChangedEventArgs(TChangeType typeOfChange, TValue originalValue, TValue newValue)
		{
			if(!typeof(TChangeType).IsEnum)
			{
				throw new ArgumentException("The generic type of MemberChangedEventArgs has to be an enum type");
			}
			this.TypeOfChange = typeOfChange;
			this.NewValue = newValue;
			this.OriginalValue = originalValue;
		}


		#region Class Property Declarations
		/// <summary>
		/// Gets / sets typeOfChange
		/// </summary>
		public TChangeType TypeOfChange { get; set; }
		/// <summary>
		/// Gets or sets the new value. OriginalValue and NewValue differ when the member raising the event this argument is used in is set to a new value.
		/// Original and NewValue are equal if the value of the member raising this event is changed internally (if it's mutable) and it implements INotifyElementChanged
		/// </summary>
		public TValue NewValue { get; set; }
		/// <summary>
		/// Gets or sets the original value. OriginalValue and NewValue differ when the member raising the event this argument is used in is set to a new value.
		/// Original and NewValue are equal if the value of the member raising this event is changed internally (if it's mutable) and it implements INotifyElementChanged
		/// </summary>
		public TValue OriginalValue { get; set; }
		#endregion
	}
}
