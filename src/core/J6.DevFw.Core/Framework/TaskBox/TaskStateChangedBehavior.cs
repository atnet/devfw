namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ��������״̬�����ı�ʱ
    /// </summary>
    /// <param name="source">�ı�״̬����Դ</param>
    /// <param name="state"></param>
    public delegate void TaskStateChangedHandler(ITaskExecuteClient source, ITask task, TaskMessage result);

    /// <summary>
    /// ���������������������
    /// </summary>
    /// <returns></returns>
    public delegate ITask TaskBuildHandler();

    public delegate void TaskPostingHandler(ITask task);

    /// <summary>
    /// ������Ϣ����
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    public delegate void TaskMessageHandler(object data, string message);

    public delegate void TaskBoxHandler(TaskBox taskBox);
}