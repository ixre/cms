namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// 刚创建的
        /// </summary>
        Created,

        /// <summary>
        /// 成功
        /// </summary>
        Ok,

        /// <summary>
        /// 由于内部原因造成的失败
        /// </summary>
        Error,

        /// <summary>
        /// 由于网络中断或其他原因挂起
        /// </summary>
        Suppend,
        Default
    }
}