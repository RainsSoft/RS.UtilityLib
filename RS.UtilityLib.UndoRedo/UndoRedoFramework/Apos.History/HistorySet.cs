using System;
using GuiLabs.Undo;
namespace Apos.History {
    /// <summary>
    /// 保存一组操作及其反向操作。主要用于撤消和重做功能。
    /// Holds a set of actions along with their inverse.
    /// Mostly used for undo and redo functionality.
    /// </summary>
    public class HistorySet {
        /// <summary>
        /// 组撤消和重做操作。
        /// Groups undo and redo actions.
        /// </summary>
        public HistorySet(Action[] undos, Action[] redos) {
            _undos = undos;
            _redos = redos;
        }
        /// <summary>
        /// 应用撤消操作。
        /// Applies the undo actions.
        /// </summary>
        public void Undo() {
            foreach (Action a in _undos) {
                a();
            }
        }
        /// <summary>
        /// 应用重做操作。
        /// Applies the redo actions.
        /// </summary>
        public void Redo() {
            foreach (Action a in _redos) {
                a();
            }
        }

        /// <summary>
        /// 应一起执行的撤消操作组。
        /// Group of undo actions that should be executed together.
        /// </summary>
        protected Action[] _undos;
        /// <summary>
        /// 应一起执行的重做操作组。
        /// Group of redo actions that should be executed together.
        /// </summary>
        protected Action[] _redos;
    }
}
