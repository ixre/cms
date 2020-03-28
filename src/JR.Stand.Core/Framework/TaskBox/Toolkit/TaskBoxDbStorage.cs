using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Framework.TaskBox.Toolkit
{
    public class TaskBoxDbStorage : ITaskBoxStorage
    {
        public void AppendSuppendTask(ITask task)
        {
            throw new NotImplementedException();
        }


        public void SaveTaskChangedState(ITask task, TaskMessage message)
        {
            //任务任务状态发生改变时储存状态
            //比如这个任务已经执行成功了，失败了。
        }


        public IList<ITask> GetSyncTaskQueue()
        {
            //
            //TODO:从数据库中获取任务，并标识任务的事件,比如用HttpPost,则在event中定义post
            //

            return new List<ITask>();
        }
    }
}