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
        /// ����ִ�к������ṩʵ��ִ�в������߼�
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
        /// ��д��ѡ�����ṩ�����������߼�
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
        /// ������һ���������������²������ӣ������²���������ӵ�������ջ�У������뵱ǰ���������һ��
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
        /// ��������Ƿ�����볷���������е�ǰһ�������ϲ� �������ͬ���͵����������ĳ��������ã�
        /// �����϶�ĳЩ���ݻ����ĳЩ�ı�
        /// Defines if the action can be merged with the previous one in the Undo buffer
        /// This is useful for long chains of consecutive operations of the same type,
        /// e.g. dragging something or typing some text
        /// </summary>
        public bool AllowToMergeWithPrevious { get; set; }
    }
}
