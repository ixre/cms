using System;
using System.Collections.Specialized;
using System.Reflection;
using JR.Cms.Conf;
using JR.Stand.Core.Framework.TaskBox;
using JR.Stand.Core.Framework.TaskBox.Toolkit;
using Quartz;
using Quartz.Impl;

namespace JR.Cms.Web
{
    internal class TaskClient : ITaskExecuteClient
    {
        public string ClientName => "taskClient";

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

    public static class CmsScheduler
    {
        private static TaskService service;

        public static async void Init()
        {
            return;
            // 构造一个调度器工厂
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
// 得到一个调度器
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();
// 定义作业并将其绑定到HelloJob类
            Assembly.GetCallingAssembly().GetType("");
            IJobDetail job = JobBuilder.Create()
                .WithIdentity("myJob", "group1")
                .Build();
// 触发作业现在运行，然后每40秒运行一次
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithCronSchedule("")
                .Build();
            await sched.ScheduleJob(job, trigger);
        }

        private static void RegisterDropMemoryTask()
        {
            if (Settings.opti_gc_collect_interval <= 0)
                return;

            service.Sington.RegistContinuTasks(() =>
            {
                ITask task = new Task();
                task.TaskName = "gc_collect";
                return task;
            }, (client, task, message) => { }, Settings.opti_gc_collect_interval); //2小时
        }

        private static void BoxOnNotifying(object data, string message)
        {
            if (message.StartsWith("[Crash]")) service.Stop();
        }
    }
}