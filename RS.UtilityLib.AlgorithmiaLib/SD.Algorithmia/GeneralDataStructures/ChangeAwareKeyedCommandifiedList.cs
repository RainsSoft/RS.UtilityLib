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

using SD.System_Linq;

using SD.Tools.Algorithmia.GeneralInterfaces;
using SD.Tools.Algorithmia.GeneralDataStructures.EventArguments;
using SD.Tools.BCLExtensions.SystemRelated;

namespace SD.Tools.Algorithmia.GeneralDataStructures
{
	/// <summary>
	/// 扩展 KeyedCommandifiedList 的类，以便它选取此列表中元素的详细更改，并在单个事件中将它们传播给订阅者。
	/// 因此，订阅者不必订阅列表中所有元素的所有详细更改事件。
	/// Class which extends KeyedCommandifiedList so that it picks up detailed changes in elements in this list and propagates them to subscribers in a single event.
	/// Subscribers therefore don't have to subscribe to all detailed change events of all the elements in the list. 
	/// </summary>
	/// <remarks>此类可以是同步集合，方法是在构造函数中为 isSynchronized 传递 true。
	/// 若要同步对此类内容的访问，请锁定 SyncRoot 对象。此类对其内部元素使用与基类相同的锁，因为这些元素与基类的元素相关.
	/// This class can be a synchronized collection by passing true for isSynchronized in the constructor. To synchronize access to the contents of this class, 
	/// lock on the SyncRoot object. This class uses the same lock for its internal elements as the base class as these elements are related to the elements of the base class</remarks>
	public class ChangeAwareKeyedCommandifiedList<T, TKeyValue, TChangeType> : KeyedCommandifiedList<T, TKeyValue>
		where T : class, IDetailedNotifyElementChanged<TChangeType, T>
	{
		#region Events
		/// <summary>
		/// 当此列表中的元素引发其 DetailedElementChanged 事件时引发。事件参数包含有关更改内容的详细信息。
		/// Raised when an element in this list raised its DetailedElementChanged event. The event args contain detailed information about what was changed. 
		/// </summary>
		public event EventHandler<ElementInListChangedEventArgs<TChangeType, T>> DetailedElementInListChanged;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeAwareKeyedCommandifiedList&lt;T, TKeyValue, TChangeType&gt;"/> class.
		/// </summary>
		/// <param name="keyValueProducerFunc">The key value producer func.</param>
		/// <param name="keyPropertyName">Name of the key property which is used to track changes in individual elements.</param>
		public ChangeAwareKeyedCommandifiedList(Func<T, TKeyValue> keyValueProducerFunc, string keyPropertyName)
			: this(keyValueProducerFunc, keyPropertyName, isSynchronized:false)
		{ }


		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeAwareKeyedCommandifiedList&lt;T, TKeyValue, TChangeType&gt;" /> class.
		/// </summary>
		/// <param name="keyValueProducerFunc">The key value producer func.</param>
		/// <param name="keyPropertyName">Name of the key property which is used to track changes in individual elements.</param>
		/// <param name="isSynchronized">if set to <c>true</c> this list is a synchronized collection, using a lock on SyncRoot to synchronize activity in multithreading
		/// scenarios</param>
		public ChangeAwareKeyedCommandifiedList(Func<T, TKeyValue> keyValueProducerFunc, string keyPropertyName, bool isSynchronized)
			: base(keyValueProducerFunc, keyPropertyName, isSynchronized)
		{
		}

		/// <summary>
		/// 在传入的项即将添加到此列表之前调用。使用此方法可对此列表中的元素执行事件处理程序内务处理。
		/// Called right before the item passed in is about to be added to this list. Use this method to do event handler housekeeping on elements in this list.
		/// </summary>
		/// <param name="item">The item which is about to be added.</param>
		protected override void OnAddingItem(T item)
		{
			base.OnAddingItem(item);
			item.DetailedElementChanged += item_DetailedElementChanged;
		}


		/// <summary>
		/// 从此列表中删除传入的项后立即调用。
		/// Called right after the item passed in has been removed from this list.
		/// </summary>
		/// <param name="item">The item.</param>
		protected override void OnRemovingItemComplete(T item)
		{
			base.OnRemovingItemComplete(item);
			item.DetailedElementChanged -= item_DetailedElementChanged;
		}


		/// <summary>
		/// 处理项控件的 DetailedElementChanged 事件。
		/// Handles the DetailedElementChanged event of the item control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments instance containing the event data.</param>
		private void item_DetailedElementChanged(object sender, ElementChangedEventArgs<TChangeType, T> e)
		{
			this.DetailedElementInListChanged.RaiseEvent(this, new ElementInListChangedEventArgs<TChangeType, T>(e.TypeOfChange, e.InvolvedElement));
		}
	}
}
