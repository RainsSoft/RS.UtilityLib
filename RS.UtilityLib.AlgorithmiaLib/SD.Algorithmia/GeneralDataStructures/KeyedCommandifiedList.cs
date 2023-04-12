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
using System.Collections.Generic;

using SD.System_Linq;

using SD.Tools.Algorithmia.UtilityClasses;
using System.ComponentModel;
using SD.Tools.BCLExtensions.CollectionsRelated;

namespace SD.Tools.Algorithmia.GeneralDataStructures
{
	/// <summary>
	/// 特殊的命令列表，可以基于键创建索引。
	/// Special commandified list which can create an index based on a key. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKeyValue">The type of the key value.</typeparam>
	/// <remarks>此类可以是同步集合，方法是在构造函数中为 isSynchronized 传递 true。
	/// 若要同步对此类内容的访问，请锁定 SyncRoot 对象。此类对其内部元素使用与基类相同的锁，因为这些元素与基类的元素相关.
	/// This class can be a synchronized collection by passing true for isSynchronized in the constructor. To synchronize access to the contents of this class, 
	/// lock on the SyncRoot object. This class uses the same lock for its internal elements as the base class as these elements are related to the elements of the base class</remarks>
	public class KeyedCommandifiedList<T, TKeyValue> : CommandifiedList<T>
		where T : class
	{
		#region Class Member Declarations
		private readonly Func<T, TKeyValue> _keyValueProducerFunc;
		private readonly Dictionary<T, TKeyValue> _keyValuePerElement;
		private readonly MultiValueDictionary<TKeyValue, T> _elementPerKeyValue;
		private readonly string _keyPropertyName;
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="KeyedCommandifiedList&lt;T, TKeyValue&gt;"/> class.
		/// </summary>
		/// <param name="keyValueProducerFunc">The key value producer func.</param>
		/// <param name="keyPropertyName">Name of the key property which is used to track changes in individual elements.</param>
		public KeyedCommandifiedList(Func<T, TKeyValue> keyValueProducerFunc, string keyPropertyName) : this(keyValueProducerFunc, keyPropertyName, isSynchronized:false)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="KeyedCommandifiedList&lt;T, TKeyValue&gt;"/> class.
		/// </summary>
		/// <param name="keyValueProducerFunc">The key value producer func.</param>
		/// <param name="keyPropertyName">Name of the key property which is used to track changes in individual elements.</param>
		/// <param name="isSynchronized">如果设置为 <c>true</c>，则此列表是一个同步集合，使用 SyncRoot 上的锁来同步多线程方案中的活动.if set to <c>true</c> this list is a synchronized collection, using a lock on SyncRoot to synchronize activity in multithreading scenarios</param>
		public KeyedCommandifiedList(Func<T, TKeyValue> keyValueProducerFunc, string keyPropertyName, bool isSynchronized)
			: base(isSynchronized)
		{
			ArgumentVerifier.CantBeNull(keyValueProducerFunc, "keyValueProducerFunc");
			ArgumentVerifier.CantBeNull(keyPropertyName, "keyPropertyName");
			_elementPerKeyValue = new MultiValueDictionary<TKeyValue, T>();
			_keyValuePerElement = new Dictionary<T, TKeyValue>();
			_keyValueProducerFunc = keyValueProducerFunc;
			_keyPropertyName = keyPropertyName;
		}


		/// <summary>
		/// 确定指定的键是否存在
		/// Determines whether the specified key is present
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns><see langword="true"/> if the specified key is present, false otherwise. </returns>
		public bool ContainsKey(TKeyValue key)
		{
			return PerformSyncedAction(()=>_elementPerKeyValue.ContainsKey(key));
		}


		/// <summary>
		/// 查找指定了键值的实例。
		/// Finds the instances which have the keyValue specified.
		/// </summary>
		/// <param name="keyValue">The key value.</param>
		/// <returns>an IEnumerable with all the elements which have the keyvalue specified or an empty enumerable if not found</returns>
		public IEnumerable<T> FindByKey(TKeyValue keyValue)
		{
			return PerformSyncedAction(()=>
									   {
										   HashSet<T> elements;
										   if(!_elementPerKeyValue.TryGetValue(keyValue, out elements))
										   {
											   elements = new HashSet<T>();
										   }
										   return elements.ToList();
									   });
		}


		/// <summary>
		/// 帮助程序方法，返回与键值匹配的第一个元素，如果未找到，则返回 null。
		/// Helper method which returns the first element which matches the keyValue or null if not found.
		/// </summary>
		/// <param name="keyValue">The key value.</param>
		/// <returns>the first element which matches the keyValue or null if not found.</returns>
		public T FindFirstByKey(TKeyValue keyValue)
		{
			// is safe as the returned enumerable by FindByKey is a copy.
			return this.FindByKey(keyValue).FirstOrDefault();
		}


		/// <summary>
		/// 在传入的项即将添加到此列表之前调用。使用此方法可对此列表中的元素执行事件处理程序内务处理。
		/// Called right before the item passed in is about to be added to this list. Use this method to do event handler housekeeping on elements in this list.
		/// </summary>
		/// <param name="item">The item which is about to be added.</param>
		protected override void OnAddingItem(T item)
		{
			PerformSyncedAction(()=>
								{
									base.OnAddingItem(item);
									IndexElement(item);
								});
		}


		/// <summary>
		/// 在清除操作开始之前调用。使用此方法可对此列表中的元素执行事件处理程序内务处理。
		/// Called right before the clear action starts. Use this method to do event handler housekeeping on elements in this list.
		/// </summary>
		protected override void OnClearing()
		{
			PerformSyncedAction(()=>
								{
									base.OnClearing();
									_elementPerKeyValue.Clear();
									_keyValuePerElement.Clear();
								});
		}


		/// <summary>
		/// 在传入的项即将从此列表中删除之前调用。使用此方法可对此列表中的元素执行事件处理程序内务处理。
		/// Called right before the item passed in is about to be removed from this list. Use this method to do event handler housekeeping on elements in this list.
		/// </summary>
		/// <param name="item">The item which is about to be removed.</param>
		protected override void OnRemovingItem(T item)
		{
			PerformSyncedAction(()=>
								{
									base.OnRemovingItem(item);
									RemoveIndexOfElement(item);
								});
		}


		/// <summary>
		/// 当此列表中的元素引发 PropertyChanged 事件时调用。
		/// Called when the PropertyChanged event was raised by an element in this list.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event arguments instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == _keyPropertyName)
			{
				// re-index
				T senderAsT = (T)sender;
				PerformSyncedAction(()=>
									{
										RemoveIndexOfElement(senderAsT);
										IndexElement(senderAsT);
									});
			}
			base.OnElementPropertyChanged(sender, e);
		}


		/// <summary>
		/// Removes the index of element.
		/// </summary>
		/// <param name="element">The element.</param>
		private void RemoveIndexOfElement(T element)
		{
			PerformSyncedAction(()=>
								{
									TKeyValue currentKeyValue = _keyValuePerElement.GetValue(element);
									_keyValuePerElement.Remove(element);
									_elementPerKeyValue.Remove(currentKeyValue, element);
								});
		}


		/// <summary>
		/// Indexes the element.
		/// </summary>
		/// <param name="toIndex">To index.</param>
		private void IndexElement(T toIndex)
		{
			PerformSyncedAction(()=>
								{
									TKeyValue keyValue = _keyValueProducerFunc(toIndex);
									_keyValuePerElement.Add(toIndex, keyValue);
									_elementPerKeyValue.Add(keyValue, toIndex);
								});
		}
	}
}
