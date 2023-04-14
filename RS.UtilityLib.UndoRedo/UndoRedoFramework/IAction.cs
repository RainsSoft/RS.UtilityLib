namespace GuiLabs.Undo
{
    /// <summary>
    /// ��װ�û�������ʵ����������������ִ�кͳ����� �������κ����ݡ�������Ϊʵ���ṩ�ܹ�ִ�кͻع���������������κ���Ϣ��
    /// Encapsulates a user action (actually two actions: Do and Undo)
    /// Can be anything.
    /// You can give your implementation any information it needs to be able to
    /// execute and rollback what it needs.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Ӧ�ô˶����װ�ĸ��ġ�
        /// Apply changes encapsulated by this object.
        /// </summary>
        /// <remarks>
        /// ExecuteCount++
        /// </remarks>
        void Execute();

        /// <summary>
        /// ������һ��ִ�е��������ĸ��ġ�
        /// Undo changes made by a previous Execute call.
        /// </summary>
        /// <remarks>
        /// ExecuteCount--
        /// </remarks>
        void UnExecute();

        /// <summary>
        /// ���ڴ������������ ExecuteCount = 0����δִ�У�ʱ��CanExecute Ϊ true���� ExecuteCount = 1����ִ��һ�Σ�ʱΪ false��
        /// For most Actions, CanExecute is true when ExecuteCount = 0 (not yet executed)
        /// and false when ExecuteCount = 1 (already executed once)
        /// </summary>
        /// <returns>true if an encapsulated action can be applied</returns>
        bool CanExecute();

        /// <returns>���������ִ���ҿ��Գ�������Ϊ true.
        /// true if an action was already executed and can be undone</returns>
        bool CanUnExecute();

        /// <summary>
        /// ����ִ���µĴ�������������ǽ��ò�����¼Ϊ�²�����ֻ���޸ĵ�ǰ������ʹ��ժҪЧ�������ߵ���ϡ�
        /// Attempts to take a new incoming action and instead of recording that one
        /// as a new action, just modify the current one so that it's summary effect is 
        /// a combination of both.
        /// </summary>
        /// <param name="followingAction"></param>
        /// <returns>�������ͬ��ϲ�����Ϊ true���������ϣ�����������������ݲ�������Ϊ false��
        /// true if the action agreed to merge, false if we want the followingAction
        /// to be tracked separately</returns>
        bool TryToMerge(IAction followingAction);

        /// <summary>
        /// ��������Ƿ�����볷���������е�ǰһ�������ϲ� �������ͬ���͵����������ĳ��������ã������϶�ĳЩ���ݻ����ĳЩ�ı�.
        /// Defines if the action can be merged with the previous one in the Undo buffer
        /// This is useful for long chains of consecutive operations of the same type,
        /// e.g. dragging something or typing some text
        /// </summary>
        bool AllowToMergeWithPrevious { get; set; }
    }
}
