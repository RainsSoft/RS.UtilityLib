using System;
using System.Collections.Generic;
using GuiLabs.Undo;
namespace Apos.History {
    /// <summary>
    /// ��ʷ��Ļ��ࡣ�ṩ������������
    /// Base class for History classes. Provides undo, redo.
    /// </summary>
    public class History {
        /// <summary>
        /// ���ع������ʷ��¼��
        /// History that is managed locally.
        /// </summary>
        public History() { }
        /// <summary>
        /// �� null ʱ����ʷ��¼�Ǳ��صġ�����������ʷ��¼����������
        /// The history is local when null. Otherwise it's managed by a HistoryHandler.
        /// </summary>
        public History(HistoryHandler historyHandler) {
            _historyHandler = historyHandler;
        }

        /// <summary>
        /// ���Ϊ false������ʷ��¼���ӳٵ��ֶ����� Commit������
        /// When false, the history is delayed until Commit() is called manually.
        /// </summary>
        public bool AutoCommit { get; set; } = true;

        /// <summary>
        /// ��ȡ������ջ�е�Ԫ������
        /// Gets the number of elements in the undo stack.
        /// </summary>
        public int UndoCount => _undo.Count;
        /// <summary>
        /// ��ȡ������ջ�е�Ԫ������
        /// Gets the number of elements in the redo stack.
        /// </summary>
        public int RedoCount => _redo.Count;
        /// <summary>
        /// ��ȡԪ������������������ + ����������
        /// Gets the number of elements in total. (UndoCount + RedoCount)
        /// </summary>
        public int Count => UndoCount + RedoCount;

        /// <summary>
        /// ���Զ��ύ����Ϊ true ʱ�ύ��
        /// Commits when AutoCommit is set to true.
        /// </summary>
        public void TryCommit() {
            if (AutoCommit) {
                Commit();
            }
        }
        /// <summary>
        /// ������ĳ����������ύ��������������ջ��ִ�������������Զ��ύ����Ϊ false ʱ������Ҫ���ô˹��ܡ�
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
        /// ������ĳ����������ύ��������������ջ������ִ�������������Զ��ύ����Ϊ false ʱ������Ҫ���ô˹��ܡ�
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
        /// �ָ���ǰ��״̬��
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
        /// ��ԭ��һ��״̬��
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
        /// �ӳ�����ջ��ɾ��һϵ��Ԫ�أ�����������Ԫ�ء�
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
        /// �ӳ�����ջ��ɾ��һϵ��Ԫ�أ�����������Ԫ�ء�
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
        /// �����Ϊ null�����������ط�������ʷ��¼��
        /// When not null, the history is managed elsewhere.
        /// </summary>
        protected HistoryHandler _historyHandler;
    }
}
