using System;
using System.Collections.Generic;
using System.Threading;

namespace JR.Stand.Core.Framework.TaskBox
{
    internal class TaskQueue
    {
        private TaskBox _syncBox;
        private Queue<ITask> tasks = new Queue<ITask>();

        public TaskQueue(TaskBox box)
        {
            this._syncBox = box;

            //仅从第一次从数据库加载
            this.upgradeStackFromStorage();
        }

        /// <summary>
        /// 任务数量
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
        /// 从存储中更新队列
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
                this._syncBox.Notifing(this._syncBox.Storage, "[Error]:从存储中获取任务队列产生错误：" + exc.Message);
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
                Message = "任务已经创建..."
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
                        this._syncBox.Notifing(taskBuilder, "任务创建失败!");
                    }

                    Thread.Sleep(minseconds);
                }
            }).Start();
        }
    }
}