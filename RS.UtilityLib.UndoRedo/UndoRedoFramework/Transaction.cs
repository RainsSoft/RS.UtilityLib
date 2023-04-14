using System;
using System.Collections.Generic;
using System.Linq;

namespace GuiLabs.Undo
{
    /// <summary>
    /// Dispose时候如果Aborted==false则自动Commit
    /// </summary>
    public sealed class Transaction : IAction, IDisposable
    {
        readonly List<IAction> Actions;
        readonly ActionManager ActionManager;
        bool Aborted { get; set; }

        public bool AllowToMergeWithPrevious { get; set; }
        public bool IsDelayed { get; set; }

        Transaction(ActionManager actionManager, bool delayed)
        {
            Actions = new List<IAction>();
            ActionManager = actionManager;
            actionManager.OpenTransaction(this);
            IsDelayed = delayed;
        }

        public static Transaction Create(ActionManager actionManager, bool delayed)
        {
            if (actionManager == null)
            {
                throw new ArgumentNullException("actionManager");
            }
            return new Transaction(actionManager, delayed);
        }

        /// <summary>
        /// 默认情况下，操作会延迟，仅在顶级事务提交后执行。
        /// By default, the actions are delayed and executed only after
        /// the top-level transaction commits.
        /// </summary>
        /// <remarks>
        /// 确保在完成(Dispose)后处理事务 - 它实际上会为您调用 Commit
        /// Make sure to dispose of the transaction once you're done - it will actually call Commit for you
        /// </remarks>
        /// <example>
        /// Recommended usage: using (Transaction.Create(actionManager)) { DoStuff(); }
        /// </example>
        public static Transaction Create(ActionManager actionManager)
        {
            return Create(actionManager, true);
        }

        #region IAction implementation

        public void Execute()
        {
            if (!IsDelayed)
            {
                IsDelayed = true;
                return;
            }
            foreach (var action in Actions)
            {
                action.Execute();
            }
        }

        public void UnExecute()
        {
            foreach (var action in LinqHelper.Reverse(Actions))
            {
                action.UnExecute();
            }
        }

        public bool CanExecute()
        {
            foreach (var action in Actions)
            {
                if (!action.CanExecute())
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanUnExecute()
        {
            foreach (var action in LinqHelper.Reverse(Actions))
            {
                if (!action.CanUnExecute())
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryToMerge(IAction followingAction)
        {
            return false;
        }

        #endregion

        public void Commit()
        {
            ActionManager.CommitTransaction();
        }

        public void Rollback()
        {
            ActionManager.RollBackTransaction();
            Aborted = true;
        }

        public void Dispose()
        {
            if (!Aborted)
            {
                Commit();
            }
        }

        public void Add(IAction actionToAppend)
        {
            if (actionToAppend == null)
            {
                throw new ArgumentNullException("actionToAppend");
            }
            Actions.Add(actionToAppend);
        }

        public bool HasActions()
        {
            return Actions.Count != 0;
        }

        public void Remove(IAction actionToCancel)
        {
            if (actionToCancel == null)
            {
                throw new ArgumentNullException("actionToCancel");
            }
            Actions.Remove(actionToCancel);
        }
    }
}
