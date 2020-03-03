using System;

namespace JR.Stand.Core.Framework.TaskBox
{
    public class TaskMessageParser
    {
        public static string ConvertSyncMessageToString(TaskMessage message)
        {
            return JsonSerializer.SerializeObject<TaskMessage>(message);
        }

        /// <summary>
        /// 从Json字符转为SyncMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TaskMessage ConvertToSyncMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                return JsonSerializer.DeserializeObject<TaskMessage>(message);
            }
            return default(TaskMessage);
        }
    }
}