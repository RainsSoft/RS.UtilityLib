using System;
using GuiLabs.Undo;
namespace Apos.History {
    /// <summary>
    /// 此类使在多个数据结构上提供撤消和重做变得容易。
    /// This class makes it easy to provide undo and redo over multiple data structures.
    /// </summary>
    public class HistoryHandler : History {
        /// <summary>
        /// 本地管理的历史记录。
        /// History that is managed locally.
        /// </summary>
        public HistoryHandler() { }
        /// <summary>
        /// 当 null 时，历史记录是本地的。否则，它由历史记录处理程序管理。
        /// The history is local when null. Otherwise it's managed by a HistoryHandler.
        /// </summary>
        public HistoryHandler(HistoryHandler historyHandler) : base(historyHandler) { }

        /// <summary>
        /// 将历史记录集添加到挂起列表并尝试提交它。
        /// Adds a HistorySet to the pending list and tries to commit it.
        /// </summary>
        public void Add(HistorySet hs) {
            _pendingUndo.Add(hs.Undo);
            _pendingRedo.Add(hs.Redo);

            TryCommit();
        }
        /// <summary>
        /// 将撤消和重做操作添加到挂起列表中，并尝试提交它们。
        /// Adds undo and redo actions to the pending list and tries to commit them.
        /// </summary>
        public void Add(Action undo, Action redo) {
            _pendingUndo.Add(undo);
            _pendingRedo.Add(redo);

            TryCommit();
        }
    }

    /*
     var historyHandler = new HistoryHandler();

int fishCount = 0;
int appleCount = 0;

SaveFishCount(fishCount, 3);
SaveFishCount(fishCount, 4);
SaveFishCount(fishCount, 5);

SaveAppleCount(appleCount, 7);
SaveAppleCount(appleCount, 9);
SaveAppleCount(appleCount, 4);
SaveAppleCount(appleCount, 5);

// Group multiple histories in one batch.
historyHandler.AutoCommit = false;
SaveFishCount(fishCount, 10);
SaveAppleCount(appleCount, 20);
// Call Commit manually.
historyHandler.Commit();
historyHandler.AutoCommit = true;

historyHandler.Undo();
historyHandler.Undo();

historyHandler.Redo();

SaveFishCount(int oldCount, int newCount) {
    historyHandler.Add(() => {
        fishCount = oldCount;
    }, () => {
        fishCount = newCount;
    });
}
SaveAppleCount(int oldCount, int newCount) {
    historyHandler.Add(() => {
        appleCount = oldCount;
    }, () => {
        appleCount = newCount;
    });
}
     */
}
