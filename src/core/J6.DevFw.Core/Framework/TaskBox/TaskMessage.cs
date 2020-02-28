using System;
using System.Collections;

namespace JR.DevFw.Framework.TaskBox
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    [Serializable]
    public class TaskMessage
    {
        /// <summary>
        /// ʧ�ܽ��
        /// </summary>
        public static TaskMessage Fault = new TaskMessage {Result = false, Data = null, Message = null};


        /// <summary>
        /// �ɹ����
        /// </summary>
        public static TaskMessage Ok = new TaskMessage {Result = true, Data = null, Message = null};

        /// <summary>
        /// ���
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// ��Ϣ
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public Hashtable Data { get; set; }

        public override string ToString()
        {
            return TaskMessageParser.ConvertSyncMessageToString(this);
        }
    }
}