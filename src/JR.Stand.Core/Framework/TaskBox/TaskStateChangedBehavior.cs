namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务任务状态发生改变时
    /// </summary>
    /// <param name="source">改变状态的来源</param>
    /// <param name="state"></param>
    public delegate void TaskStateChangedHandler(ITaskExecuteClient source, ITask task, TaskMessage result);

    /// <summary>
    /// 任务任务生成器处理程序
    /// </summary>
    /// <returns></returns>
    public delegate ITask TaskBuildHandler();

    public delegate void TaskPostingHandler(ITask task);

    /// <summary>
    /// 任务消息处理
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    public delegate void TaskMessageHandler(object data, string message);

    public delegate void TaskBoxHandler(TaskBox taskBox);
}