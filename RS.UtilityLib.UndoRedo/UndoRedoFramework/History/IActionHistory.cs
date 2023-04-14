using System;
using System.Collections.Generic;

namespace GuiLabs.Undo
{
    /// <summary>
    /// 缓冲区的概念。它不是两个堆栈，而是具有当前状态的状态机。
    /// 它可以将一个状态后移或一个州向前移动。允许非线性缓冲区，您可以在其中选择要重做的多个操作之一。
    /// A notion of the buffer. Instead of two stacks, it's a state machine
    /// with the current state. It can move one state back or one state forward.
    /// Allows for non-linear buffers, where you can choose one of several actions to redo.
    /// </summary>
    internal interface IActionHistory : IEnumerable<IAction>
    {
        /// <summary>
        /// 将操作追加到撤消缓冲区的末尾。
        /// Appends an action to the end of the Undo buffer.
        /// </summary>
        /// <param name="newAction">An action to append.</param>
        /// <returns>false if merged with previous, else true</returns>
        bool AppendAction(IAction newAction);
        void Clear();

        void MoveBack();
        void MoveForward();

        bool CanMoveBack { get; }
        bool CanMoveForward { get; }
        int Length { get; }

        SimpleHistoryNode CurrentState { get; }

        IEnumerable<IAction> EnumUndoableActions();
        IEnumerable<IAction> EnumRedoableActions();

        event EventHandler CollectionChanged;
    }
}
