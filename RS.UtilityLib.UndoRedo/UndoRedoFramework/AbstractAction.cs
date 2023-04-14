namespace GuiLabs.Undo
{
    public abstract class AbstractAction : IAction
    {
        protected int ExecuteCount { get; set; }

        public virtual void Execute()
        {
            if (!CanExecute())
            {
                return;
            }
            ExecuteCore();
            ExecuteCount++;
        }

        /// <summary>
        /// 覆盖执行核心以提供实际执行操作的逻辑
        /// Override execute core to provide your logic that actually performs the action
        /// </summary>
        protected abstract void ExecuteCore();

        public virtual void UnExecute()
        {
            if (!CanUnExecute())
            {
                return;
            }
            UnExecuteCore();
            ExecuteCount--;
        }

        /// <summary>
        /// 重写此选项以提供撤消操作的逻辑
        /// Override this to provide the logic that undoes the action
        /// </summary>
        protected abstract void UnExecuteCore();

        public virtual bool CanExecute()
        {
            return ExecuteCount == 0;
        }

        public virtual bool CanUnExecute()
        {
            return !CanExecute();
        }

        /// <summary>
        /// 如果最后一个操作可以与以下操作联接，则以下操作不会添加到撤消堆栈中，而是与当前操作混合在一起。
        /// If the last action can be joined with the followingAction,
        /// the following action isn't added to the Undo stack,
        /// but rather mixed together with the current one.
        /// </summary>
        /// <param name="FollowingAction"></param>
        /// <returns>true if the FollowingAction can be merged with the
        /// last action in the Undo stack</returns>
        public virtual bool TryToMerge(IAction followingAction)
        {
            return false;
        }

        /// <summary>
        /// 定义操作是否可以与撤消缓冲区中的前一个操作合并 这对于相同类型的连续操作的长链很有用，
        /// 例如拖动某些内容或键入某些文本
        /// Defines if the action can be merged with the previous one in the Undo buffer
        /// This is useful for long chains of consecutive operations of the same type,
        /// e.g. dragging something or typing some text
        /// </summary>
        public bool AllowToMergeWithPrevious { get; set; }
    }
}
