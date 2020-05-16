using System;
using System.Threading;

namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务容器
    /// </summary>
    public class TaskBox
    {
        /// <summary>
        /// 线程数
        /// </summary>
        private int _threadNum = 5;

        private readonly TaskQueue _taskManager;
        private Thread _serviceThread;

        /// <summary>
        /// 任务提交的处理程序
        /// </summary>
        public event TaskPostingHandler OnTaskExecuting;

        /// <summary>
        /// 任务返回消息
        /// </summary>
        public event TaskMessageHandler OnNotifing;

        /// <summary>
        /// 默认挂起毫秒数
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

            //记录日志
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
        /// 任务容器数据存储
        /// </summary>
        public ITaskBoxStorage Storage { get; private set; }

        /// <summary>
        /// 日志提供者
        /// </summary>
        public ITaskLogProvider Log { get; private set; }

        /// <summary>
        /// 通知s
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
            //开启box进程
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
        /// 停止工作
        /// </summary>
        public void StopWork()
        {
            //等待线程执行完任务
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
                    //继续工作
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
        /// 加入到任务容器中
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