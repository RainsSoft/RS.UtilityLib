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
//		- Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using SD.Tools.Algorithmia.GeneralDataStructures;
using SD.Tools.Algorithmia.UtilityClasses;
using SD.Tools.BCLExtensions.SystemRelated;
using System.ComponentModel;
using System.Timers;

namespace SD.Tools.Algorithmia.GeneralDataStructures
{
	/// <summary>
	/// 限制事件管道的类。它在固定间隔内提供独特的事件引发。实例收集事件，并且在每个间隔中仅引发该批次中的唯一事件（最后一个事件优先）。
	/// 在批处理中找到的每个等于已引发的事件（具有相等的事件参数）的事件都将被忽略。
	/// 使用此类来限制大量相等的事件，并导致例如重新绘制 UI，以充当事件。
	/// 在使用事件来操作 UI 对象或其他对象并且重复更改会导致相同情况的情况下，
	/// 这可以大大提高性能（例如，元素在短时间内更改的事件的 100 倍（例如 100 毫秒），
	/// 每次重新绘制 TreeNode，因为“元素已更改”是多余的： 批处理中的最后一个事件就足够了，它已经完全重新绘制了节点）。
	/// Class which throttles an event pipeline. It offers unique event raising during a fixed interval.
	/// Instances collect events and with each interval only the unique events (last event first) from that batch are raised. Every event 
	/// found in the batch which is equal to an already raised event (which has equal event arguments) is ignored. Use this class to limit a 
	/// large amount of events which are equal and lead to e.g. repainting a UI, to act like a event instead. This can greatly increase performance in 
	/// scenarios where events are used to manipulate UI objects or other objects and repetitive changes lead to the same situation (e.g. 100 times the
	/// event that an element has changed in a short time (e.g. 100ms) where each time a TreeNode is repainted because 'the element has changed' is redundant:
	/// the last event in the batch is enough, it already repaints the node completely).
	/// <br/><br/>
	/// Events are compared based on the event args, which are compared to event arguments already processed. Use one throttler per event handler.
	/// </summary>
	/// <typeparam name="TElement">The type of the element involved in the event. If possible, use a more specific object than the event sender.</typeparam>
	/// <typeparam name="TEventArgs">The type of the event args.</typeparam>
	public class EventThrottler<TElement, TEventArgs>
		where TElement : class
		where TEventArgs : EventArgs
	{
		#region Class Member Declarations
		private Queue<Pair<TElement, TEventArgs>> _taskQueue;
		private Timer _timer;
		private bool _queueProcessingInProgress;
		private IEqualityComparer<TEventArgs> _eventArgsComparer;
		#endregion

		#region Events
		/// <summary>
		/// 当唯一事件从任务队列中受到限制并被批准由观察者处理时引发。
		/// Raised when a unique event was throttled from the task queue and approved to be processed by observers. 
		/// </summary>
		public event EventHandler<TEventArgs> EventThrottled;
		/// <summary>
		/// 启动队列处理时引发。
		/// Raised when the queue processing was started.
		/// </summary>
		public event EventHandler QueueProcessingStarted;
		/// <summary>
		/// 队列处理完成时引发。
		/// Raised when the queue processing was finished.
		/// </summary>
		public event EventHandler QueueProcessingFinished;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="EventThrottler&lt;TElement, TChangeType&gt;"/> class.
		/// </summary>
		/// <param name="synchronizingObject">同步对象。如果为 null，则 EventThrottled 事件可能由其他线程引发，
		/// 因为在这种情况下，用于队列处理的系统计时器正在使用线程池。
		/// 如果处理程序必须在与 UI 相同的线程上运行，请将 UI 对象指定为 synchronizingObject，
		/// 例如，使用此限制器的窗体对象或控件对象。
		/// The synchronizing object. If null, the EventThrottled event might be raised by a different thread, as the System
		/// timer used for queue processing is using a threadpool in that case. If the handler has to run on the same thread as the UI, specify a UI object
		/// as synchronizingObject, e.g. a form object or control object on which this throttler is used.</param>
		/// <remarks>Uses an interval of 500 and the default comparer</remarks>
		public EventThrottler(ISynchronizeInvoke synchronizingObject)
			: this(synchronizingObject, 500, null)
		{
		}
		

		/// <summary>
		/// Initializes a new instance of the <see cref="EventThrottler&lt;TElement, TChangeType&gt;"/> class.
		/// </summary>
		/// <param name="synchronizingObject">The synchronizing object. If null, the EventThrottled event might be raised by a different thread, as the System
		/// timer used for queue processing is using a threadpool in that case. If the handler has to run on the same thread as the UI, specify a UI object
		/// as synchronizingObject, e.g. a form object or control object on which this throttler is used.</param>
		/// <param name="interval">The interval, in milliseconds. If smaller than 100, it's set to 500</param>
		/// <remarks>Uses the default comparer</remarks>
		public EventThrottler(ISynchronizeInvoke synchronizingObject, int interval) : this(synchronizingObject, interval, null)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EventThrottler&lt;TElement, TChangeType&gt;"/> class.
		/// </summary>
		/// <param name="synchronizingObject">The synchronizing object. If null, the EventThrottled event might be raised by a different thread, as the System
		/// timer used for queue processing is using a threadpool in that case. If the handler has to run on the same thread as the UI, specify a UI object
		/// as synchronizingObject, e.g. a form object or control object on which this throttler is used.</param>
		/// <param name="interval">The interval, in milliseconds. If smaller than 100, it's set to 500</param>
		/// <param name="eventArgsComparer">The event args comparer. Can be null, in which the default equality comparer is used.</param>
		public EventThrottler(ISynchronizeInvoke synchronizingObject, int interval, IEqualityComparer<TEventArgs> eventArgsComparer)
		{
			double intervalToUse = Convert.ToDouble(interval);
			if(intervalToUse<100.0)
			{
				intervalToUse = 500.0;
			}
			_taskQueue = new Queue<Pair<TElement, TEventArgs>>();
			_queueProcessingInProgress = false;
			_timer = new Timer(intervalToUse);
			_timer.SynchronizingObject = synchronizingObject;
			_timer.AutoReset = false;
			_timer.Elapsed += _timer_Elapsed;
			_eventArgsComparer = eventArgsComparer;
			this.Enabled = true;
		}


		/// <summary>
		/// 清除此实例的排队任务并停止计时器（如果已设置）。
		/// Clears this instance's queued tasks and stops the timer, if it was set.
		/// </summary>
		public void Clear()
		{
			_timer.Stop();
			_taskQueue.Clear();
			_queueProcessingInProgress = false;
		}


		/// <summary>
		/// 将任务队列中的事件排入队列。
		/// Enqueues the event in the task queue.
		/// </summary>
		/// <param name="involvedElement">The involved element. This doesn't have to be the sender. Preverably this is the element involved in the
		/// event.</param>
		/// <param name="eventArgs">The TEventArgs instance containing the event data.</param>
		public void EnqueueEvent(TElement involvedElement, TEventArgs eventArgs)
		{
			ArgumentVerifier.CantBeNull(involvedElement, "involvedElement");
			ArgumentVerifier.CantBeNull(eventArgs, "eventArgs");

			if(!this.Enabled)
			{
				this.EventThrottled.RaiseEvent(involvedElement, eventArgs);
				return;
			}
			_taskQueue.Enqueue(new Pair<TElement, TEventArgs>(involvedElement, eventArgs));
			if(_timer.Enabled)
			{
				// timer already runs, no futher action
				return;
			}
			if(_queueProcessingInProgress)
			{
				return;
			}
			// not processing, no timer running yet, start timer
			_timer.Start();
		}
		

		/// <summary>
		/// Handles the Elapsed event of the _timer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				_timer.Stop();
				_queueProcessingInProgress = true;
				if(_taskQueue.Count <= 0)
				{
					return;
				}
				var tasksAlreadyPerformed = new MultiValueDictionary<TElement, TEventArgs>(_eventArgsComparer);
				this.QueueProcessingStarted.RaiseEvent(this);
				while(_taskQueue.Count > 0)
				{
					// process the entire queue from back to front, skipping any action we've already done. Any action resulting from an action
					// processed is added to the queue again.
					var tasksToProcess = new List<Pair<TElement, TEventArgs>>(_taskQueue.Count);
					while(_taskQueue.Count > 0)
					{
						tasksToProcess.Add(_taskQueue.Dequeue());
					}
					tasksAlreadyPerformed.Clear();
					for(int i = tasksToProcess.Count - 1; i >= 0; i--)
					{
						var toProcess = tasksToProcess[i];
						if(tasksAlreadyPerformed.ContainsValue(toProcess.Value1, toProcess.Value2))
						{
							continue;
						}
						this.EventThrottled.RaiseEvent(toProcess.Value1, toProcess.Value2);
						tasksAlreadyPerformed.Add(toProcess.Value1, toProcess.Value2);
					}
				}
				this.QueueProcessingFinished.RaiseEvent(this);
				if(_taskQueue.Count > 0)
				{
					// re-enable timer to process the left-overs
					_timer.Start();
				}
			}
			finally
			{
				_queueProcessingInProgress = false;
			}
		}


		#region Class Property Declarations
		/// <summary>
		/// 获取或设置一个值，该值指示是否启用了此值&lt;请参阅 cref=“EventThrottler&lt;TElement， TEventArgs&gt;”/&gt;。如果禁用，它只会重新引发排队的事件。
		/// Gets or sets a value indicating whether this <see cref="EventThrottler&lt;TElement, TEventArgs&gt;"/> is enabled. If disabled, it simply re-raises
		/// the events enqueued.
		/// </summary>
		public bool Enabled { get; set; }
		#endregion
	}
}
