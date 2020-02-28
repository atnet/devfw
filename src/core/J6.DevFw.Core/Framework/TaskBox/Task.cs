using System;
using System.Collections;
using System.Collections.Generic;

namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ��������
    /// </summary>
    public class Task : ITask
    {
        public event TaskStateChangedHandler StateChanged;
        private readonly IList<Hashtable> _dataList;
        private TaskState _state = TaskState.Default;

        public Task()
        {
            this._dataList = new List<Hashtable>();
        }

        public Task(string taskName, params Hashtable[] datas)
            : this()
        {
            this.TaskName = taskName;
            if (datas != null)
            {
                foreach (Hashtable data in datas)
                {
                    if (data != null)
                    {
                        this._dataList.Add(data);
                    }
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public String TaskName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public IList<Hashtable> Datas
        {
            get { return this._dataList; }
        }


        /// <summary>
        /// ״̬
        /// </summary>
        public TaskState State
        {
            get { return this._state; }
        }


        public void SetState(ITaskExecuteClient source, TaskState state, TaskMessage message)
        {
            this._state = state;

            if (this.StateChanged != null)
            {
                this.StateChanged(source, this, message);
            }
        }
    }
}