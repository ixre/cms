using System;

namespace JR.Stand.Core.Framework.TaskBox.Toolkit
{
    /// <summary>
    /// 任务容器
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
                    throw new Exception("服务未启动!");
                }

                return _box;
            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start(TaskBoxHandler handler, ITaskBoxStorage storage, ITaskLogProvider logProvider)
        {
            if (_isBooted == true)
                throw new Exception("服务已经启动!");

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
                //    throw new ArgumentNullException("请使用RegistServer注册任务服务器信息!");
                //HttpSyncClient client = new HttpSyncClient(_server, _token);

                //if (client.TestConnect())
                //{
                //    //注册事件
                //    _box.OnTaskPosting += client.Post;
                //}
                //else
                //{
                //    throw new Exception("任务服务器连接失败");
                //}
            }

            _isBooted = true;
            _box.StartWork();
        }

        /// <summary>
        /// 开启服务,使用内置的存储及日志记录
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