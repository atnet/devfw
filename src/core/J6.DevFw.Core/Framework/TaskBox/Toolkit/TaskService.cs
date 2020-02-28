using System;

namespace JR.DevFw.Framework.TaskBox.Toolkit
{
    /// <summary>
    /// ��������
    /// </summary>
    public class TaskService
    {
        private TaskBox _box;
        //private static string _server;
        //private static string _token;
        private bool _isBooted;

        //public static void RegistServer(string server, string token)
        //{
        //    _server = server;
        //    _token = token;
        //}

        public TaskBox Sington
        {
            get
            {
                if (_box == null)
                {
                    throw new Exception("����δ����!");
                }

                return _box;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void Start(TaskBoxHandler handler, ITaskBoxStorage storage, ITaskLogProvider logProvider)
        {
            if (_isBooted == true)
                throw new Exception("�����Ѿ�����!");

            if (_box == null)
            {
                if (storage == null)
                {
                    storage = new TaskBoxDbStorage();
                }

                if (logProvider == null)
                {
                    logProvider = new TaskLogProvider(storage);
                }


                _box = new TaskBox(storage, logProvider, 3);

                if (handler != null)
                {
                    handler(_box);
                }

                //if (String.IsNullOrEmpty(_server)
                //    || String.IsNullOrEmpty(_token))
                //    throw new ArgumentNullException("��ʹ��RegistServerע�������������Ϣ!");
                //HttpSyncClient client = new HttpSyncClient(_server, _token);

                //if (client.TestConnect())
                //{
                //    //ע���¼�
                //    _box.OnTaskPosting += client.Post;
                //}
                //else
                //{
                //    throw new Exception("�������������ʧ��");
                //}
            }

            _isBooted = true;
            _box.StartWork();
        }

        /// <summary>
        /// ��������,ʹ�����õĴ洢����־��¼
        /// </summary>
        public void Start(TaskBoxHandler handler)
        {
            Start(handler, null, null);
        }


        public void Stop()
        {
            _box.StopWork();
        }
    }
}