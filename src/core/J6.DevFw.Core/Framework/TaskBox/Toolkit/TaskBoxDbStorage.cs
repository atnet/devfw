using System;
using System.Collections.Generic;

namespace JR.DevFw.Framework.TaskBox.Toolkit
{
    public class TaskBoxDbStorage : ITaskBoxStorage
    {
        public void AppendSuppendTask(ITask task)
        {
            throw new NotImplementedException();
        }


        public void SaveTaskChangedState(ITask task, TaskMessage message)
        {
            //��������״̬�����ı�ʱ����״̬
            //������������Ѿ�ִ�гɹ��ˣ�ʧ���ˡ�
        }


        public IList<ITask> GetSyncTaskQueue()
        {
            //
            //TODO:�����ݿ��л�ȡ���񣬲���ʶ������¼�,������HttpPost,����event�ж���post
            //

            return new List<ITask>();
        }
    }
}