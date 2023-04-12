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

namespace SD.Tools.Algorithmia.Commands
{
	/// <summary>
	/// 对给定对象执行命令的命令类。 执行的内容由 lambda 表达式设置。 状态包含在该对象中。
	/// 这是命令模式的典型实现，具有撤消/重做功能。 参见：http://en.wikipedia.org/wiki/Command_pattern
	/// Command class which performs a command on a given object. What is performed is set by a lambda expression. State is contained inside this object.
	/// This is a typical implementation of the Command pattern, with undo/redo capabilities. See: http://en.wikipedia.org/wiki/Command_pattern
	/// </summary>
	/// <typeparam name="TState">命令中保存的状态类型The type of the state preserved inside the command.</typeparam>
	public class Command<TState> : CommandBase
	{
		#region Class Member Declarations
		private TState _originalState;
		private readonly bool _useParameterLessUndo;
		private readonly Action<TState> _undoFunc1;		// _undoFunc1 is the undo func for single parameter commands
		private readonly Func<TState> _getStateFunc;
		private readonly Action _doFunc;		// _undoFunc0 is the undo func for parameter less commands
		private readonly Action _undoFunc0;     // _undoFunc0 is the undo func for parameter less commands
		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="Command&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="doFunc">dofunc，用于执行Do操作，是一个不接受任何参数的func. The do func, used for performing the Do operation, which is a func which doesn't accept any parameters.</param>
		public Command(Action doFunc) : this(doFunc, null, string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Command&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="doFunc">The do func, used for performing the Do operation, which is a func which doesn't accept any parameters.</param>
		/// <param name="undoFunc">The undo func.</param>
		public Command(Action doFunc, Action undoFunc)
			: this(doFunc, undoFunc, string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Command&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="doFunc">The do func, used for performing the Do operation, which is a func which doesn't accept any parameters.</param>
		/// <param name="undoFunc">The undo func.</param>
		/// <param name="description">The description of the command.</param>
		public Command(Action doFunc, Action undoFunc, string description) : base(description)
		{
			ArgumentVerifier.CantBeNull(doFunc, "doFunc");
			_doFunc = doFunc;
			_undoFunc0 = undoFunc;
			_useParameterLessUndo = true;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Command&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="doFunc">The do func, used for performing the Do operation, which is a func which doesn't accept any parameters.</param>
		/// <param name="getStateFunc">The get state func, which is the func to use to get the state.</param>
		/// <param name="undoFunc">The undo func, which is the func to undo the action performed by the doFunc. Can be the same as
		/// doFunc.</param>
		public Command(Action doFunc, Func<TState> getStateFunc, Action<TState> undoFunc)
			: this(doFunc, getStateFunc, undoFunc, string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Command&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="doFunc">The do func, used for performing the Do operation, which is a func which doesn't accept any parameters.</param>
		/// <param name="getStateFunc">获取状态函数，这是用于获取状态的函数.The get state func, which is the func to use to get the state.</param>
		/// <param name="undoFunc">The undo func, which is the func to undo the action performed by the doFunc. Can be the same as
		/// doFunc.</param>
		/// <param name="description">The description of the command.</param>
		public Command(Action doFunc, Func<TState> getStateFunc, Action<TState> undoFunc, string description)
			: base(description)
		{
			ArgumentVerifier.CantBeNull(doFunc, "doFunc");
			ArgumentVerifier.CantBeNull(getStateFunc, "getStateFunc");
			ArgumentVerifier.CantBeNull(undoFunc, "undoFunc");
			_doFunc = doFunc;
			_getStateFunc = getStateFunc;
			_undoFunc1 = undoFunc;
		}


		/// <summary>
		/// 通过传递指定的函数来排队并运行新命令。 使用此快捷方式将多个语句包装到一个撤消块中。
		/// Enqueues and runs a new command by passing the function specified. Use this shortcut to wrap several statements into a single undo block.
		/// </summary>
		/// <param name="doFunc">The do func.</param>
		public static void DoNow(Action doFunc)
		{
			DoNow(doFunc, null, string.Empty);
		}


		/// <summary>
		/// 通过传递指定的函数来排队并运行新命令。 使用此快捷方式将多个语句包装到一个撤消块中。
		/// Enqueues and runs a new command by passing the function specified. Use this shortcut to wrap several statements into a single undo block.
		/// </summary>
		/// <param name="doFunc">The do func.</param>
		/// <param name="undoFunc">The undo func.</param>
		public static void DoNow(Action doFunc, Action undoFunc)
		{
			DoNow(doFunc, undoFunc, string.Empty);
		}


		/// <summary>
		/// 通过传递指定的函数来排队并运行新命令。 使用此快捷方式将多个语句包装到一个撤消块中。
		/// Enqueues and runs a new command by passing the function specified. Use this shortcut to wrap several statements into a single undo block.
		/// </summary>
		/// <param name="doFunc">The do func.</param>
		/// <param name="undoFunc">The undo func.</param>
		/// <param name="description">The description.</param>
		public static void DoNow(Action doFunc, Action undoFunc, string description)
		{
			CommandQueueManagerSingleton.GetInstance().EnqueueAndRunCommand(new Command<TState>(doFunc, undoFunc, description));
		}


		/// <summary>
		/// Executes the command.
		/// </summary>
		protected internal override void Do()
		{
			if(_getStateFunc != null)
			{
				// has state retrieval function so obtain the original state using that func. This state is then used in the undo action as the state to restore
				_originalState = _getStateFunc();
			}
			try
			{
				PushCommandQueueOnActiveStackIfRequired();
				_doFunc();
			}
			finally
			{
				PopCommandQueueFromActiveStackIfRequired();
			}
		}


		/// <summary>
		/// Undo's the action done with <see cref="Do"/>.
		/// </summary>
		protected internal override void Undo()
		{
			PopCommandQueueFromActiveStackIfRequired();
			// first undo commands in own command queue from back to front, that is: from the active command in the own command queue 
			while(this.OwnCommandQueue.CanUndo)
			{
				this.OwnCommandQueue.UndoPreviousCommand();
			}

			// then, if we're not in an Undoable period clear the own command queue and rollback ourselves. 
			if(!CommandQueueManagerSingleton.GetInstance().IsInUndoablePeriod)
			{
				this.OwnCommandQueue.Clear();
			}

			if(_useParameterLessUndo)
			{
				if(_undoFunc0 != null)
				{
					// undo ourselves, by setting the original state back using the undoFunc0. This func is responsible for the original state to set back.
					_undoFunc0();
				}
			}
			else
			{
				if(_undoFunc1 != null)
				{
					// undo ourselves, by setting the original state back to the state we gathered using the getStateFunc
					_undoFunc1(_originalState);
				}
			}
		}
	}
}
