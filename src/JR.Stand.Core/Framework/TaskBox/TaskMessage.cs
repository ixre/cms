using System;
using System.Collections;

namespace JR.Stand.Core.Framework.TaskBox
{
    /// <summary>
    /// 任务消息
    /// </summary>
    [Serializable]
    public class TaskMessage
    {
        /// <summary>
        /// 失败结果
        /// </summary>
        public static TaskMessage Fault = new TaskMessage {Result = false, Data = null, Message = null};


        /// <summary>
        /// 成功结果
        /// </summary>
        public static TaskMessage Ok = new TaskMessage {Result = true, Data = null, Message = null};

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public Hashtable Data { get; set; }

        public override string ToString()
        {
            return TaskMessageParser.ConvertSyncMessageToString(this);
        }
    }
}