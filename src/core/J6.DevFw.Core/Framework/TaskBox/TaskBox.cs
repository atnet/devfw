using System;
using System.Threading;

namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ��������
    /// </summary>
    public class TaskBox
    {
        /// <summary>
        /// �߳���
        /// </summary>
        private int _threadNum = 5;

        private readonly TaskQueue _taskManager;
        private Thread _serviceThread;

        /// <summary>
        /// �����ύ�Ĵ������
        /// </summary>
        public event TaskPostingHandler OnTaskExecuting;

        /// <summary>
        /// ���񷵻���Ϣ
        /// </summary>
        public event TaskMessageHandler OnNotifing;

        /// <summary>
        /// Ĭ�Ϲ��������
        /// </summary>
        private int _suppend_minseconds = 10000;

        public TaskBox(ITaskBoxStorage storage,
            ITaskLogProvider logProvider,
            int threadNum)
        {
            this.Storage = storage;
            this.Log = logProvider;
            this._threadNum = threadNum;
            this._taskManager = new TaskQueue(this);

            //��¼��־
            //this.TaskStateChanged += this.Log.LogTaskState;
        }

        public TaskBox(ITaskBoxStorage storage)
            : this(storage, null, 5)
        {
        }

        public TaskBox(ITaskBoxStorage storage, int threadNum)
            : this(storage, null, threadNum)
        {
        }


        /// <summary>
        /// �����������ݴ洢
        /// </summary>
        public ITaskBoxStorage Storage { get; private set; }

        /// <summary>
        /// ��־�ṩ��
        /// </summary>
        public ITaskLogProvider Log { get; private set; }

        /// <summary>
        /// ֪ͨs
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Notifing(object source, string message)
        {
            if (this.OnNotifing != null)
            {
                this.OnNotifing(source, message);
            }
        }

        public void StartWork()
        {
            //����box����
            _serviceThread = new Thread(() =>
            {
                try
                {
                    _work();
                }
                catch (Exception exc)
                {
                    this.OnNotifing(this, "[Crash]:" + exc.Message);
                }
            });
            _serviceThread.Start();

            this.Notifing(_serviceThread, "[Start]:Task service is running!");
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        public void StopWork()
        {
            //�ȴ��߳�ִ��������
            //serviceThread.Join();
            if (_serviceThread != null)
            {
                _serviceThread.Abort();
            }
            this.Notifing(_serviceThread, "[Stop]:Task service is stoped!");
        }

        private void _work()
        {
            do
            {
                ITask task = this._taskManager.GetNextTask();
                if (task == null)
                {
                    //��������
                    if (Thread.CurrentThread.ThreadState == ThreadState.Running)
                    {
                        //Thread.Sleep(this._suppend_minseconds);
                        continue;
                    }
                    break;
                }

                if (this.OnTaskExecuting != null)
                {
                    new Thread(() => { this.OnTaskExecuting(task); }).Start();
                }
            } while (true);
        }

        public int TaskCount
        {
            get { return _taskManager.TaskCount; }
        }

        /// <summary>
        /// ���뵽����������
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        public virtual void RegistTask(
            ITask task,
            TaskStateChangedHandler handler)
        {
            this._taskManager.RegistTask(task, handler);
        }


        public virtual void RegistContinuTasks(
            TaskBuildHandler taskBuilder,
            TaskStateChangedHandler handler,
            int seconds)
        {
            this._taskManager.RegistContinuTasks(taskBuilder, handler, seconds);
        }
    }
}