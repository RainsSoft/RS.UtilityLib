//////////////////////////////////////////////////////////////////////
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
using SD.Tools.Algorithmia.GeneralDataStructures.EventArguments;

namespace SD.Tools.Algorithmia.GeneralInterfaces
{
	/// <summary>
	/// 用于引发详细事件的接口，该事件包含有关元素中已更改内容的更改通知。
	/// CommandifiedList 选取这些事件，并向订阅者冒泡此事件。
	/// Interface for raising a detailed event containing the change notification about what has changed in the element. CommandifiedList picks up these
	/// events and bubbles upwards this event to subscribers. 
	/// </summary>
	public interface IDetailedNotifyElementChanged<TChangeType, TElement>
	{
		/// <summary>
		/// 在实现对象的数据更改时引发。更改的内容包含在事件参数中。与INotifyElementChanged.ElementChanged类似的事件，
		/// 但是此变体具有有关发生更改的详细信息。
		/// Raised when the implementing object's data changed. What has changed is enclosed in the event arguments. Similar event as INotifyElementChanged.ElementChanged
		/// however this variant has detailed information about which change took place.
		/// </summary>
		event EventHandler<ElementChangedEventArgs<TChangeType, TElement>> DetailedElementChanged;
	}
}
