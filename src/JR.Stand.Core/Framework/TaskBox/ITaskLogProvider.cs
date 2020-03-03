namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务日志记录提供者
    /// </summary>
    public interface ITaskLogProvider
    {
        /// <summary>
        /// 任务存储
        /// </summary>
        ITaskBoxStorage Storage { get; }

        /// <summary>
        /// 记录任务状态
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        void LogTaskState(ITaskExecuteClient client, ITask task, TaskMessage message);
    }
}