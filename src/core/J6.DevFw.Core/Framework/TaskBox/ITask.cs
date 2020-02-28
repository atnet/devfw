using System;
using System.Collections;
using System.Collections.Generic;

namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ��������
    /// </summary>
    public interface ITask
    {
        /// <summary>
        ///  ״̬�����ı�ʱ������,����֪ͨ������
        /// </summary>
        event TaskStateChangedHandler StateChanged;

        /// <summary>
        /// ��������(�����ʶ���������Ŀ��ʶ)
        /// </summary>
        String TaskName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        IList<Hashtable> Datas { get; }

        /// <summary>
        /// ״̬
        /// </summary>
        TaskState State { get; }

        /// <summary>
        /// ����״̬
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source">�����¼���Դ</param>
        /// <param name="msg"></param>
        void SetState(ITaskExecuteClient source, TaskState state, TaskMessage msg);
    }
}