namespace GuiLabs.Undo
{
    /// <summary>
    /// 封装用户操作（实际上是两个操作：执行和撤消） 可以是任何内容。您可以为实现提供能够执行和回滚所需内容所需的任何信息。
    /// Encapsulates a user action (actually two actions: Do and Undo)
    /// Can be anything.
    /// You can give your implementation any information it needs to be able to
    /// execute and rollback what it needs.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// 应用此对象封装的更改。
        /// Apply changes encapsulated by this object.
        /// </summary>
        /// <remarks>
        /// ExecuteCount++
        /// </remarks>
        void Execute();

        /// <summary>
        /// 撤消上一次执行调用所做的更改。
        /// Undo changes made by a previous Execute call.
        /// </summary>
        /// <remarks>
        /// ExecuteCount--
        /// </remarks>
        void UnExecute();

        /// <summary>
        /// 对于大多数操作，当 ExecuteCount = 0（尚未执行）时，CanExecute 为 true，当 ExecuteCount = 1（已执行一次）时为 false。
        /// For most Actions, CanExecute is true when ExecuteCount = 0 (not yet executed)
        /// and false when ExecuteCount = 1 (already executed once)
        /// </summary>
        /// <returns>true if an encapsulated action can be applied</returns>
        bool CanExecute();

        /// <returns>如果操作已执行且可以撤消，则为 true.
        /// true if an action was already executed and can be undone</returns>
        bool CanUnExecute();

        /// <summary>
        /// 尝试执行新的传入操作，而不是将该操作记录为新操作，只需修改当前操作，使其摘要效果是两者的组合。
        /// Attempts to take a new incoming action and instead of recording that one
        /// as a new action, just modify the current one so that it's summary effect is 
        /// a combination of both.
        /// </summary>
        /// <param name="followingAction"></param>
        /// <returns>如果操作同意合并，则为 true，如果我们希望单独跟踪以下内容操作，则为 false。
        /// true if the action agreed to merge, false if we want the followingAction
        /// to be tracked separately</returns>
        bool TryToMerge(IAction followingAction);

        /// <summary>
        /// 定义操作是否可以与撤消缓冲区中的前一个操作合并 这对于相同类型的连续操作的长链很有用，例如拖动某些内容或键入某些文本.
        /// Defines if the action can be merged with the previous one in the Undo buffer
        /// This is useful for long chains of consecutive operations of the same type,
        /// e.g. dragging something or typing some text
        /// </summary>
        bool AllowToMergeWithPrevious { get; set; }
    }
}
