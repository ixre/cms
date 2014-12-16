using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ops.Cms.Conf;
using Ops.Framework.Tasks;
using Ops.Framework.Tasks.Toolkit;

namespace Ops.Cms.Web.Task
{
    internal class TaskClient:ITaskExecuteClient
    {

        public string ClientName
        {
            get { return "taskClient"; }
        }

        public void Execute(ITask task)
        {
            switch (task.TaskName)
            {
                case "gc_collect":
                    GC_Collect();
                    break;
            }

            task.SetState(this, TaskState.Ok, TaskMessage.Ok);
        }

        private void GC_Collect()
        {
            GC.Collect();
        }
    }

    public static class CmsTask
    {
        private static TaskService service;
        public static void Init()
        {
            return;
           service = new TaskService();
            service.Start(box =>
            {
                TaskClient taskClient = new TaskClient();
                box.OnNotifing += box_OnNotifing;
                box.OnTaskExecuting += taskClient.Execute;
            });

            RegistDropMemoryTask();
        }

        private static void RegistDropMemoryTask()
        {
            if (Settings.Opti_GC_Collect_Interval <= 0) 
                return;

            service.Sington.RegistContinuTasks(() =>
            {
                ITask task = new global::Ops.Framework.Tasks.Task();
                task.TaskName = "gc_collect";
                return task;
            }, (client, task, message) =>
            {

            },Settings.Opti_GC_Collect_Interval);   //2小时
        }

        static void box_OnNotifing(object data, string message)
        {
            if (message.StartsWith("[Crash]"))
            {
                service.Stop();
            }
        }
    }
}
