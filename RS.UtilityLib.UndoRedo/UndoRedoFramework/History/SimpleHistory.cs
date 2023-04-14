using System;
using System.Collections.Generic;

namespace GuiLabs.Undo
{
    /// <summary>
    /// IActionHistory represents a recorded list of actions undertaken by user.
    ///IActionHistory表示用户执行的操作的记录列表。
    ///此类实现通常的线性操作序列。您可以来回移动更改相应文档的状态。当您向前移动时，
    ///您将执行相应的操作，当您向后移动时，您将撤消它（UnExecute）。
    /// This class implements a usual, linear action sequence. You can move back and forth
    /// changing the state of the respective document. When you move forward, you execute
    /// a respective action, when you move backward, you Undo it (UnExecute).
    ///通过 SimpleHistoryNode 对象的双链接列表实现。
    /// Implemented through a double linked-list of SimpleHistoryNode objects.
    /// ====================================================================
    /// </summary>
    internal class SimpleHistory : IActionHistory
    {
        public SimpleHistory()
        {
            Init();
        }

        #region Events

        public event EventHandler CollectionChanged;
        protected void RaiseUndoBufferChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new EventArgs());
            }
        }

        #endregion

        private SimpleHistoryNode mCurrentState = new SimpleHistoryNode();
        /// <summary>
        /// “迭代器”用于在序列中导航，“光标”
        /// "Iterator" to navigate through the sequence, "Cursor"
        /// </summary>
        public SimpleHistoryNode CurrentState
        {
            get
            {
                return mCurrentState;
            }
            set
            {
                if (value != null)
                {
                    mCurrentState = value;
                }
                else
                {
                    throw new ArgumentNullException("CurrentState");
                }
            }
        }

        public SimpleHistoryNode Head { get; set; }

        public IAction LastAction { get; set; }

        /// <summary>
        /// 在当前状态之后向尾部添加新操作。如果在此之后存在更多操作，它们将丢失（垃圾回收）。
        /// 这是此类中唯一实际修改链表的方法。
        /// Adds a new action to the tail after current state. If 
        /// there exist more actions after this, they're lost (Garbage Collected).
        /// This is the only method of this class that actually modifies the linked-list.
        /// </summary>
        /// <param name="newAction">Action to be added.</param>
        /// <returns>如果附加了操作，则为 true，如果它与前一个操作合并，则为 false。
        /// true if action was appended, false if it was merged with the previous one</returns>
        public bool AppendAction(IAction newAction)
        {
            if (CurrentState.PreviousAction != null && CurrentState.PreviousAction.TryToMerge(newAction))
            {
                RaiseUndoBufferChanged();
                return false;
            }
            CurrentState.NextAction = newAction;
            CurrentState.NextNode = new SimpleHistoryNode(newAction, CurrentState);
            return true;
        }

        /// <summary>
        /// 所有现有节点和操作都将被垃圾回收。
        /// All existing Nodes and Actions are garbage collected.
        /// </summary>
        public void Clear()
        {
            Init();
            RaiseUndoBufferChanged();
        }

        private void Init()
        {
            CurrentState = new SimpleHistoryNode();
            Head = CurrentState;
        }

        public IEnumerable<IAction> EnumUndoableActions()
        {
            SimpleHistoryNode current = Head;
            while (current != null && current != CurrentState && current.NextAction != null)
            {
                yield return current.NextAction;
                current = current.NextNode;
            }
        }

        public IEnumerable<IAction> EnumRedoableActions()
        {
            SimpleHistoryNode current = CurrentState;
            while (current != null && current.NextAction != null)
            {
                yield return current.NextAction;
                current = current.NextNode;
            }
        }

        public void MoveForward()
        {
            if (!CanMoveForward)
            {
                throw new InvalidOperationException(
                    "History.MoveForward() cannot execute because"
                    + " CanMoveForward returned false (the current state"
                    + " is the last state in the undo buffer.");
            }
            CurrentState.NextAction.Execute();
            CurrentState = CurrentState.NextNode;
            Length += 1;
            RaiseUndoBufferChanged();
        }

        public void MoveBack()
        {
            if (!CanMoveBack)
            {
                throw new InvalidOperationException(
                    "History.MoveBack() cannot execute because"
                    + " CanMoveBack returned false (the current state"
                    + " is the last state in the undo buffer.");
            }
            CurrentState.PreviousAction.UnExecute();
            CurrentState = CurrentState.PreviousNode;
            Length -= 1;
            RaiseUndoBufferChanged();
        }

        public bool CanMoveForward
        {
            get
            {
                return CurrentState.NextAction != null &&
                    CurrentState.NextNode != null;
            }
        }

        public bool CanMoveBack
        {
            get
            {
                return CurrentState.PreviousAction != null &&
                        CurrentState.PreviousNode != null;
            }
        }

        /// <summary>
        /// The length of Undo buffer (total number of undoable actions)
        /// </summary>
        public int Length { get; set; }

        public IEnumerator<IAction> GetEnumerator()
        {
            return EnumUndoableActions().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
