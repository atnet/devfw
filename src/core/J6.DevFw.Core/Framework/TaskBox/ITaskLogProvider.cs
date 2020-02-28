namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ������־��¼�ṩ��
    /// </summary>
    public interface ITaskLogProvider
    {
        /// <summary>
        /// ����洢
        /// </summary>
        ITaskBoxStorage Storage { get; }

        /// <summary>
        /// ��¼����״̬
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        void LogTaskState(ITaskExecuteClient client, ITask task, TaskMessage message);
    }
}