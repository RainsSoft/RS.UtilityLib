using System;
using System.Collections.Generic;

namespace GuiLabs.Undo
{
    /// <summary>
    /// https://github.com/KirillOsenkov/Undo
    /// 操作管理器是撤消框架的中心类。您的域模型（业务对象）将具有负责执行操作的 ActionManager 引用。
    /// 以下是它的工作原理：
    /// 1. 声明一个实现 IAction 的类 
    /// 2.您创建它的实例，并为其提供应用或回滚更改所需的所有必要信息 
    /// 3.你调用 ActionManager.RecordAction（yourAction） 
    /// 然后你也可以调用 ActionManager.Undo（） 或 ActionManager.Redo（）
    /// Action Manager is a central class for the Undo Framework.
    /// Your domain model (business objects) will have an ActionManager reference that would 
    /// take care of executing actions.
    /// 
    /// Here's how it works:
    /// 1. You declare a class that implements IAction
    /// 2. You create an instance of it and give it all necessary info that it needs to know
    ///    to apply or rollback a change
    /// 3. You call ActionManager.RecordAction(yourAction)
    /// 
    /// Then you can also call ActionManager.Undo() or ActionManager.Redo()
    /// </summary>
    public class ActionManager
    {
        public ActionManager()
        {
            History = new SimpleHistory();
        }

        #region Events

        /// <summary>
        /// 侦听此事件，以便在添加新操作、执行、撤消或重做时收到通知
        /// Listen to this event to be notified when a new action is added, executed, undone or redone
        /// </summary>
        public event EventHandler CollectionChanged;
        protected void RaiseUndoBufferChanged(object sender, EventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        #endregion

        #region RecordAction

        #region Running

        /// <summary>
        /// 当前正在运行的操作（在撤消或重做过程中）
        /// Currently running action (during an Undo or Redo process)
        /// </summary>
        /// <remarks>如果没有发生撤消或重做，则为 null.
        /// null if no Undo or Redo is taking place</remarks>
        public IAction CurrentAction { get; internal set; }

        /// <summary>
        /// 检查我们是否在撤消或重做操作中
        /// Checks if we're inside an Undo or Redo operation
        /// </summary>
        public bool ActionIsExecuting
        {
            get
            {
                return CurrentAction != null;
            }
        }

        #endregion

        /// <summary>
        /// 定义我们是否应该将操作记录到 Undo 缓冲区然后执行，或者只是执行它而不成为历史记录的一部分
        /// Defines whether we should record an action to the Undo buffer and then execute,
        /// or just execute it without it becoming a part of history
        /// </summary>
        public bool ExecuteImmediatelyWithoutRecording { get; set; }

        /// <summary>
        /// 用于添加和执行新操作的中心方法。
        /// Central method to add and execute a new action.
        /// </summary>
        /// <param name="existingAction">要在缓冲区中记录并执行的操作.
        /// An action to be recorded in the buffer and executed</param>
        public void RecordAction(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(
                    "ActionManager.RecordAction: the action argument is null");
            }
            // make sure we're not inside an Undo or Redo operation
            CheckNotRunningBeforeRecording(action);

            // if we don't want to record actions, just run and forget it
            if (ExecuteImmediatelyWithoutRecording && action.CanExecute())
            {
                action.Execute();
                return;
            }

            // Check if we're inside a transaction that is being recorded
            Transaction currentTransaction = RecordingTransaction;
            if (currentTransaction != null)
            {
                // if we're inside a transaction, just add the action to the transaction's list
                currentTransaction.Add(action);
                if (!currentTransaction.IsDelayed)
                {
                    action.Execute();
                }
            }
            else
            {
                RunActionDirectly(action);
            }
        }

        void CheckNotRunningBeforeRecording(IAction candidate)
        {
            if (CurrentAction != null)
            {
                string candidateActionName = candidate != null ? candidate.ToString() : "";
                throw new InvalidOperationException
                (
                    string.Format
                    (
                          "ActionManager.RecordActionDirectly: the ActionManager is currently running "
                        + "or undoing an action ({0}), and this action (while being executed) attempted "
                        + "to recursively record another action ({1}), which is not allowed. "
                        + "You can examine the stack trace of this exception to see what the "
                        + "executing action did wrong and change this action not to influence the "
                        + "Undo stack during its execution. Checking if ActionManager.ActionIsExecuting == true "
                        + "before launching another transaction might help to avoid the problem. Thanks and sorry for the inconvenience.",
                        CurrentAction.ToString(),
                        candidateActionName
                    )
                );
            }
        }

        /// <summary>
        /// 将操作添加到缓冲区并运行它
        /// Adds the action to the buffer and runs it
        /// </summary>
        void RunActionDirectly(IAction actionToRun)
        {
            CheckNotRunningBeforeRecording(actionToRun);

            CurrentAction = actionToRun;
            try
            {
                if (History.AppendAction(actionToRun))
                {
                    History.MoveForward();
                }
            }
            finally
            {
                CurrentAction = null;
            }
        }

        #endregion

        #region Transactions

        public Transaction CreateTransaction()
        {
            return Transaction.Create(this);
        }

        public Transaction CreateTransaction(bool delayed)
        {
            return Transaction.Create(this, delayed);
        }

        private Stack<Transaction> mTransactionStack = new Stack<Transaction>();
        public Stack<Transaction> TransactionStack
        {
            get
            {
                return mTransactionStack;
            }
        }

        public Transaction RecordingTransaction
        {
            get
            {
                if (TransactionStack.Count > 0)
                {
                    return TransactionStack.Peek();
                }
                return null;
            }
        }

        public void OpenTransaction(Transaction t)
        {
            TransactionStack.Push(t);
        }

        public void CommitTransaction()
        {
            if (TransactionStack.Count == 0)
            {
                throw new InvalidOperationException(
                    "ActionManager.CommitTransaction was called"
                    + " when there is no open transaction (TransactionStack is empty)."
                    + " Please examine the stack trace of this exception to find code"
                    + " which called CommitTransaction one time too many."
                    + " Normally you don't call OpenTransaction and CommitTransaction directly,"
                    + " but use using(var t = Transaction.Create(Root)) instead.");
            }

            Transaction committing = TransactionStack.Pop();

            if (committing.HasActions())
            {
                RecordAction(committing);
            }
        }

        public void RollBackTransaction()
        {
            if (TransactionStack.Count != 0)
            {
                var topLevelTransaction = TransactionStack.Peek();
                if (topLevelTransaction != null)
                {
                    topLevelTransaction.UnExecute();
                }

                TransactionStack.Clear();
            }
        }

        #endregion

        #region Undo, Redo

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }
            if (ActionIsExecuting)
            {
                throw new InvalidOperationException(string.Format("ActionManager is currently busy"
                    + " executing a transaction ({0}). This transaction has called Undo()"
                    + " which is not allowed until the transaction ends."
                    + " Please examine the stack trace of this exception to see"
                    + " what part of your code called Undo.", CurrentAction));
            }
            CurrentAction = History.CurrentState.PreviousAction;
            History.MoveBack();
            CurrentAction = null;
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }
            if (ActionIsExecuting)
            {
                throw new InvalidOperationException(string.Format("ActionManager is currently busy"
                    + " executing a transaction ({0}). This transaction has called Redo()"
                    + " which is not allowed until the transaction ends."
                    + " Please examine the stack trace of this exception to see"
                    + " what part of your code called Redo.", CurrentAction));
            }
            CurrentAction = History.CurrentState.NextAction;
            History.MoveForward();
            CurrentAction = null;
        }

        public bool CanUndo
        {
            get
            {
                return History.CanMoveBack;
            }
        }

        public bool CanRedo
        {
            get
            {
                return History.CanMoveForward;
            }
        }

        #endregion

        #region Buffer

        public void Clear()
        {
            History.Clear();
            CurrentAction = null;
        }

        public IEnumerable<IAction> EnumUndoableActions()
        {
            return History.EnumUndoableActions();
        }

        public IEnumerable<IAction> EnumRedoableActions()
        {
            return History.EnumRedoableActions();
        }

        private IActionHistory mHistory;
        internal IActionHistory History
        {
            get
            {
                return mHistory;
            }
            set
            {
                if (mHistory != null)
                {
                    mHistory.CollectionChanged -= RaiseUndoBufferChanged;
                }
                mHistory = value;
                if (mHistory != null)
                {
                    mHistory.CollectionChanged += RaiseUndoBufferChanged;
                }
            }
        }

        #endregion
    }


    /*
      class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Original color");

            SetConsoleColor(ConsoleColor.Green);
            Console.WriteLine("New color");

            actionManager.Undo();
            Console.WriteLine("Old color again");

            using (var ts=Transaction.Create(actionManager))
            {
                SetConsoleColor(ConsoleColor.Red); // you never see Red
                Console.WriteLine("Still didn't change to Red because of lazy evaluation");
                SetConsoleColor(ConsoleColor.Blue);
              
            }
            Console.WriteLine("Changed two colors at once");

            actionManager.Undo();
            Console.WriteLine("Back to original");

            actionManager.Redo();
            Console.WriteLine("Blue again");
            Console.ReadKey();
        }

        static void SetConsoleColor(ConsoleColor color)
        {
            SetConsoleColorAction action = new SetConsoleColorAction(color);
            actionManager.RecordAction(action);
        }

        static ActionManager actionManager = new ActionManager();
    }

    class SetConsoleColorAction : AbstractAction
    {
        public SetConsoleColorAction(ConsoleColor newColor)
        {
            color = newColor;
        }

        ConsoleColor color;
        ConsoleColor oldColor;

        protected override void ExecuteCore()
        {
            oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        protected override void UnExecuteCore()
        {
            Console.ForegroundColor = oldColor;
        }
    }
     */
}
