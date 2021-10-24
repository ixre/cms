using System.Collections.Generic;

namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务容器存储(用于存储任务，存储状态等)
    /// </summary>
    public interface ITaskBoxStorage
    {
        /// <summary>
        /// 添加挂起的任务
        /// </summary>
        /// <param name="task"></param>
        void AppendSuppendTask(ITask task);

        /// <summary>
        /// 任务任务状态发生改变时储存状态
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        void SaveTaskChangedState(ITask task, TaskMessage message);

        /// <summary>
        /// 获取所有的任务
        /// </summary>
        /// <returns></returns>
        IList<ITask> GetSyncTaskQueue();
    }
}