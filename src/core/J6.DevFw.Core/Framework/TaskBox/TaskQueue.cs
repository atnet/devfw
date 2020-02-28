using System;
using System.Collections.Generic;
using System.Threading;

namespace JR.DevFw.Framework.TaskBox
{
    internal class TaskQueue
    {
        private TaskBox _syncBox;
        private Queue<ITask> tasks = new Queue<ITask>();

        public TaskQueue(TaskBox box)
        {
            this._syncBox = box;

            //���ӵ�һ�δ����ݿ����
            this.upgradeStackFromStorage();
        }

        /// <summary>
        /// ��������
        /// </summary>
        public int TaskCount
        {
            get { return this.tasks.Count; }
        }

        public ITask GetNextTask()
        {
            if (tasks.Count == 0)
                return null;

            while (tasks.Count != 0)
            {
                ITask task = tasks.Dequeue();
                if (task != null) return task;
            }
            return null;
        }

        /// <summary>
        /// �Ӵ洢�и��¶���
        /// </summary>
        private void upgradeStackFromStorage()
        {
            IList<ITask> taskList = null;

            try
            {
                taskList = this._syncBox.Storage.GetSyncTaskQueue();
            }
            catch (Exception exc)
            {
                this._syncBox.Notifing(this._syncBox.Storage, "[Error]:�Ӵ洢�л�ȡ������в�������" + exc.Message);
                return;
            }

            if (taskList != null)
            {
                foreach (ITask task in taskList)
                {
                    this.tasks.Enqueue(task);
                }
            }
        }

        internal void RegistTask(ITask task,
            TaskStateChangedHandler behavior)
        {
            task.StateChanged += behavior;
            task.StateChanged += this._syncBox.Log.LogTaskState;
            this.tasks.Enqueue(task);

            task.SetState(null, TaskState.Created, new TaskMessage
            {
                Result = true,
                Message = "�����Ѿ�����..."
            });
        }


        internal void RegistContinuTasks(
            TaskBuildHandler taskBuilder,
            TaskStateChangedHandler handler,
            int minseconds)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.RegistTask(taskBuilder(), handler);
                    }
                    catch (Exception exc)
                    {
                        this._syncBox.Notifing(taskBuilder, "���񴴽�ʧ��!");
                    }

                    Thread.Sleep(minseconds);
                }
            }).Start();
        }
    }
}