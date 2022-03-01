using System;

namespace JR.Stand.Core.Framework.Scheduler
{
    public interface ICronJob
    {
        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="jobData">任务上下文</param>
        void ExecuteJob(Object context);
    }
}
