using System.Collections.Generic;

namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ���������洢(���ڴ洢���񣬴洢״̬��)
    /// </summary>
    public interface ITaskBoxStorage
    {
        /// <summary>
        /// ��ӹ��������
        /// </summary>
        /// <param name="task"></param>
        void AppendSuppendTask(ITask task);

        /// <summary>
        /// ��������״̬�����ı�ʱ����״̬
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        void SaveTaskChangedState(ITask task, TaskMessage message);

        /// <summary>
        /// ��ȡ���е�����
        /// </summary>
        /// <returns></returns>
        IList<ITask> GetSyncTaskQueue();
    }
}