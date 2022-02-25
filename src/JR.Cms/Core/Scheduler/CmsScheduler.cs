using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.CacheService;
using JR.Stand.Core.Framework.TaskBox;
using JR.Stand.Core.Framework.TaskBox.Toolkit;
using Quartz;
using Quartz.Impl;

namespace JR.Cms.Core.Scheduler
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
            CheckJob();
            var jobs = LocalService.Instance.JobService.FindAllJob();

            // 构造一个调度器工厂
            NameValueCollection props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
// 得到一个调度器
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            foreach (CmsJobEntity je in jobs)
            {
                // 定义作业并将其绑定到HelloJob类
                Type classType =Assembly.GetExecutingAssembly().GetType(je.JobClass);
                IJobDetail job = JobBuilder.Create()
                    .OfType(classType)
                    .UsingJobData("job_id",je.Id)
                    .WithIdentity(je.JobName, "group1")
                    .Build();
                // 触发作业现在运行，然后每40秒运行一次
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger" + je.Id, "group1")
                    .StartNow()
                    .WithCronSchedule(je.CronExp)
                    .Build();
                await sched.ScheduleJob(job, trigger);

            }
        }

        private static void CheckJob()
        {
            var jobs = LocalService.Instance.JobService.FindAllJob();
            // 初始化默认Job
            IList<CmsJobEntity> initial = new List<CmsJobEntity>
            {
                new CmsJobEntity
                {
                    JobName = "搜索引擎提交任务",
                    CronExp = "0 0 */2 * * ?",
                    Enabled = 0,
                    JobClass = "JR.Cms.Core.Scheduler.Job.SearchEngineSubmitJob",
                    JobDescribe = "每日将新的文档提交到百度等搜索引擎, 每2小时执行一次"
                }
            };
            foreach (var it in initial)
            {
                if (jobs.FirstOrDefault(a => a.JobName == it.JobName) == null)
                {
                    LocalService.Instance.JobService.SaveJob(it);
                }
            }
            
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