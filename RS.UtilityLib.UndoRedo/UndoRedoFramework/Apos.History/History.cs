using System;
using System.Collections.Generic;
using GuiLabs.Undo;
namespace Apos.History {
    /// <summary>
    /// 历史类的基类。提供撤消、重做。
    /// Base class for History classes. Provides undo, redo.
    /// </summary>
    public class History {
        /// <summary>
        /// 本地管理的历史记录。
        /// History that is managed locally.
        /// </summary>
        public History() { }
        /// <summary>
        /// 当 null 时，历史记录是本地的。否则，它由历史记录处理程序管理。
        /// The history is local when null. Otherwise it's managed by a HistoryHandler.
        /// </summary>
        public History(HistoryHandler historyHandler) {
            _historyHandler = historyHandler;
        }

        /// <summary>
        /// 如果为 false，则历史记录将延迟到手动调用 Commit（）。
        /// When false, the history is delayed until Commit() is called manually.
        /// </summary>
        public bool AutoCommit { get; set; } = true;

        /// <summary>
        /// 获取撤消堆栈中的元素数。
        /// Gets the number of elements in the undo stack.
        /// </summary>
        public int UndoCount => _undo.Count;
        /// <summary>
        /// 获取重做堆栈中的元素数。
        /// Gets the number of elements in the redo stack.
        /// </summary>
        public int RedoCount => _redo.Count;
        /// <summary>
        /// 获取元素总数。（撤消计数 + 重做计数）
        /// Gets the number of elements in total. (UndoCount + RedoCount)
        /// </summary>
        public int Count => UndoCount + RedoCount;

        /// <summary>
        /// 当自动提交设置为 true 时提交。
        /// Commits when AutoCommit is set to true.
        /// </summary>
        public void TryCommit() {
            if (AutoCommit) {
                Commit();
            }
        }
        /// <summary>
        /// 将挂起的撤消和重做提交到撤消和重做堆栈并执行重做。仅当自动提交设置为 false 时，才需要调用此功能。
        /// Commits pending undo and redo to the undo and redo stacks and executes redo.
        /// This only needs to be called when AutoCommit is set to false.
        /// </summary>
        public void Commit() {
            if (_pendingUndo.Count > 0 && _pendingRedo.Count > 0) {
                _pendingUndo.Reverse();
                HistorySet hs = new HistorySet(_pendingUndo.ToArray(), _pendingRedo.ToArray());
                _redo.Clear();

                if (_historyHandler != null) {
                    _historyHandler.Add(hs);
                } else {
                    hs.Redo();
                    _undo.AddLast(hs);
                }

                _pendingRedo.Clear();
                _pendingUndo.Clear();
            }
        }
        /// <summary>
        /// 将挂起的撤消和重做提交到撤消和重做堆栈，而不执行重做。仅当自动提交设置为 false 时，才需要调用此功能。
        /// Commits pending undo and redo to the undo and redo stacks without executing redo.
        /// This only needs to be called when AutoCommit is set to false.
        /// </summary>
        public void CommitOnly() {
            if (_pendingUndo.Count > 0 && _pendingRedo.Count > 0) {
                _pendingUndo.Reverse();
                HistorySet hs = new HistorySet(_pendingUndo.ToArray(), _pendingRedo.ToArray());
                _redo.Clear();

                if (_historyHandler != null) {
                    _historyHandler.Add(hs);
                } else {
                    _undo.AddLast(hs);
                }

                _pendingRedo.Clear();
                _pendingUndo.Clear();
            }
        }
        /// <summary>
        /// 恢复以前的状态。
        /// Restores the previous state.
        /// </summary>
        public void Undo() {
            if (_undo.Count > 0) {
                HistorySet hs = _undo.Last.Value;
                _undo.RemoveLast();
                hs.Undo();
                _redo.AddLast(hs);
            }
        }
        /// <summary>
        /// 还原下一个状态。
        /// Restores the next state.
        /// </summary>
        public void Redo() {
            if (_redo.Count > 0) {
                HistorySet hs = _redo.Last.Value;
                _redo.RemoveLast();
                hs.Redo();
                _undo.AddLast(hs);
            }
        }

        /// <summary>
        /// 从撤消堆栈中删除一系列元素，仅保留最新元素。
        /// Removes a range of elements from the undo stack keeping only the newest elements.
        /// </summary>
        /// <param name="count">The number of elements to keep.</param>
        public void Keep(int count) {
            int amount = Math.Max(count, 0);
            while (_undo.Count > amount) {
                _undo.RemoveFirst();
            }
        }
        /// <summary>
        /// 从撤消堆栈中删除一系列元素，仅保留最新元素。
        /// Removes a range of elements from the undo stack keeping only the newest elements.
        /// </summary>
        /// <param name="count">The number of elements to remove.</param>
        public void Remove(int count) {
            int amount = Math.Min(count, _undo.Count);
            for (int i = 0; i < amount; i++) {
                _undo.RemoveFirst();
            }
        }

        /// <summary>
        /// Undo stack.
        /// </summary>
        protected LinkedList<HistorySet> _undo = new LinkedList<HistorySet>();
        /// <summary>
        /// Redo stack.
        /// </summary>
        protected LinkedList<HistorySet> _redo = new LinkedList<HistorySet>();
        /// <summary>
        /// Undo states that haven't been commited yet.
        /// </summary>
        protected List<Action> _pendingUndo = new List<Action>();
        /// <summary>
        /// Redo states that haven't been commited yet.
        /// </summary>
        protected List<Action> _pendingRedo = new List<Action>();
        /// <summary>
        /// 如果不为 null，则在其他地方管理历史记录。
        /// When not null, the history is managed elsewhere.
        /// </summary>
        protected HistoryHandler _historyHandler;
    }
}
