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


namespace SD.Tools.Algorithmia.Heaps
{
	/// <summary>
	/// 所有堆类的通用基类。 堆是一种专门的基于树的数据结构，如果堆是最大堆，则其元素的排序方式是每个父代都大于/等于其子代，如果堆是最大堆
	/// ，则每个父代都较小 /等于它的孩子。 参见：http://en.wikipedia.org/wiki/Heap_%28data_structure%29<br/>
	/// 堆类实现了对堆的基本操作：删除最大/最小值、增加/减少键、插入和合并。
	/// General base class for all Heap classes. A heap is a specialized tree-based datastructure which has its elements ordered in such a way that
	/// every parent is larger / equal than its children, if the heap is a max-heap and in the case of a min-heap every parent is smaller/equal than its
	/// children. See: http://en.wikipedia.org/wiki/Heap_%28data_structure%29<br/>
	/// The heap classes implement the basic operations on a heap: delete max/min, increase/decrease key, insert and merge. 
	/// </summary>
	/// <remarks>
	/// 由于此类既可以处理最小堆也可以处理最大堆，因此删除最大/最小称为“ExtractRoot”。 调用增减键方法
	/// As this class can both handle min heaps as well as max heaps, the delete max/min is called 'ExtractRoot'. The increase/decrease key method is called
	/// 'UpdateKey'.
	/// <para>
	/// 堆不能存储值类型。 这是因为改变一个值类型真的使它成为一个不同的值，而改变一个对象的内容并没有使它成为一个不同的对象。
	/// Heaps can't store value types. This is because changing a value type really makes it a different value, while changing the contents of an object doesn't
	/// make it a different object.
	/// </para>
	/// </remarks>
	/// <typeparam name="TElement">The type of the elements in this heap</typeparam>
	public abstract class HeapBase<TElement>
		where TElement : class
	{
		#region Class Member Declarations
		private readonly Comparison<TElement> _keyCompareFunc;
		private readonly bool _isMinHeap;
		private readonly Func<TElement, TElement, bool> _elementCompareFunc;
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="HeapBase&lt;TElement&gt;"/> class.
		/// </summary>
		/// <param name="keyCompareFunc">键比较函数，用于根据键比较两个元素。 根据堆的类型，最小堆或最大堆，
		/// 第一个元素被放置为父元素或子元素：如果这个堆是一个最小堆，并且第一个元素，根据 keyCompareFunc 大于第二个元素，
		/// 则 第二个元素将是第一个元素的父元素。 在最大堆中，在这种情况下，第一个元素将是第二个元素的父元素。
		/// The key compare func, which is used to compare two elements based on their key. Based on the type of the heap, min heap
		/// or max heap, the first element is placed as parent or as child: if this heap is a min-heap, and the first element, according to keyCompareFunc is
		/// bigger than the second element, the second element will be the parent of the first element. In a max-heap, the first element will be the parent of
		/// the second element in that situation.</param>
		/// <param name="isMinHeap">Flag to signal if this heap is a min heap or a max heap</param>
		protected HeapBase(Comparison<TElement> keyCompareFunc, bool isMinHeap)
		{
			_keyCompareFunc = keyCompareFunc;
			_isMinHeap = isMinHeap;

			// create element compare func to compare elements based on the type of the heap. 
			if(_isMinHeap)
			{
				// a min heap has the element with the lower key value as the parent of the element with the higher key value. 
				_elementCompareFunc = (a, b) => (_keyCompareFunc(a, b) <= 0);
			}
			else
			{
				// a max heap has the element with the higher key value as the parent of the element with the lower key value. 
				_elementCompareFunc = (a, b) => (_keyCompareFunc(a, b) >= 0);
			}
		}


		/// <summary>
		///提取并删除堆的根。 Extracts and removes the root of the heap.
		/// </summary>
		/// <returns>根元素已删除，如果堆为空，则为 null/default . the root element deleted, or null/default if the heap is empty</returns>
		public abstract TElement ExtractRoot();
		/// <summary>
		/// 更新传入的元素的键。仅对键是元素属性而不是元素本身的元素使用此方法。这意味着如果您有
		/// 一个具有值类型元素（例如整数）的堆，则更新键 正在更新元素本身的值，并且由于堆可能包含具有相同值的元素，
		/// 这可能导致未定义的结果。
		/// Updates the key of the element passed in. Only use this method for elements where the key is a property of the element, not the element itself.
		/// This means that if you have a heap with value typed elements (e.g. integers), updating the key is updating the value of the element itself, and because
		/// a heap might contain elements with the same value, this could lead to undefined results.
		/// </summary>
		/// <typeparam name="TKeyType">The type of the key type.</typeparam>
		/// <param name="element">The element which key has to be updated</param>
		/// <param name="keyUpdateFunc">The key update func, which takes 2 parameters: the element to update and the new key value.</param>
		/// <param name="newValue">The new value for the key.</param>
		/// <remarks>要更新的元素首先在堆中查找。 如果新键违反了堆属性（例如，键大于最大堆中父项的键），
		/// 则堆中的元素将以堆再次服从堆属性的方式进行重组。
		/// The element to update is first looked up in the heap. If the new key violates the heap property (e.g. the key is bigger than the
		/// key of the parent in a max-heap) the elements in the heap are restructured in such a way that the heap again obeys the heap property. </remarks>
		public abstract void UpdateKey<TKeyType>(TElement element, Action<TElement, TKeyType> keyUpdateFunc, TKeyType newValue);
		/// <summary>
		///将指定元素插入堆中的正确位置. Inserts the specified element into the heap at the right spot
		/// </summary>
		/// <param name="element">The element to insert.</param>
		public abstract void Insert(TElement element);
		/// <summary>
		/// Clears all elements from the heap
		/// </summary>
		public abstract void Clear();
		/// <summary>
		/// Removes the element specified
		/// </summary>
		/// <param name="element">The element to remove.</param>
		public abstract void Remove(TElement element);
		/// <summary>
		///确定此堆是否包含指定的元素. Determines whether this heap contains the element specified
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>true if the heap contains the element specified, false otherwise</returns>
		public abstract bool Contains(TElement element);


		#region Class Property Declarations
		/// <summary>
		///获取堆的根。 根据这是最小堆还是最大堆的事实，它返回具有最小键（最小堆）的元素或
		///具有最大键（最大堆）的元素。 如果堆为空，则返回 null / default. 
		///Gets the root of the heap. Depending on the fact if this is a min or max heap, it returns the element with the minimum key (min heap) or the element
		/// with the maximum key (max heap). If the heap is empty, null / default is returned
		/// </summary>
		public abstract TElement Root { get; }
		/// <summary>
		/// Gets the number of elements in the heap.
		/// </summary>
		public abstract int Count { get; }

		/// <summary>
		/// Gets the key compare func to compare elements based on key.
		/// </summary>
		protected Comparison<TElement> KeyCompareFunc
		{
			get { return _keyCompareFunc; }
		}

		/// <summary>
		///获取元素比较函数，该函数根据堆类型比较两个元素：如果第一个元素确实是第二个元素的正确父元素，
		///则该函数返回 true，否则返回 false。 
		///Gets the element compare func, which is the func to compare two elements based on the heap type: the function returns true if the first element
		/// is indeed the correct parent of the second element or false if not.
		/// </summary>
		protected Func<TElement, TElement, bool> ElementCompareFunc
		{
			get { return _elementCompareFunc; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a min heap (true) or a max heap (false)
		/// </summary>
		public bool IsMinHeap
		{
			get { return _isMinHeap; }
		}
		#endregion
	}
}
