namespace JR.DevFw.Framework.TaskBox.Toolkit
{
    public class TaskLogProvider : ITaskLogProvider
    {
        public TaskLogProvider(ITaskBoxStorage storage)
        {
            this.Storage = storage;
        }

        public void LogTaskState(ITaskExecuteClient client, ITask task, TaskMessage message)
        {
            //��������Լ�¼��־

            if (this.Storage != null)
                this.Storage.SaveTaskChangedState(task, message);
        }

        public ITaskBoxStorage Storage { get; private set; }
    }
}