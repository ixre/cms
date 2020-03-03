using System;
using System.Collections;
using System.Collections.Generic;

namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务任务
    /// </summary>
    public interface ITask
    {
        /// <summary>
        ///  状态发生改变时触发的,可以通知到各方
        /// </summary>
        event TaskStateChangedHandler StateChanged;

        /// <summary>
        /// 任务名称(服务端识别的任务项目标识)
        /// </summary>
        String TaskName { get; set; }

        /// <summary>
        /// 任务数据
        /// </summary>
        IList<Hashtable> Datas { get; }

        /// <summary>
        /// 状态
        /// </summary>
        TaskState State { get; }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source">触发事件来源</param>
        /// <param name="msg"></param>
        void SetState(ITaskExecuteClient source, TaskState state, TaskMessage msg);
    }
}