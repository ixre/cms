using System.Collections.Generic;
using System.Timers;

namespace JR.Stand.Core.Framework.Scheduler
{
    using System;
    using System.Threading;

        public interface ICronJobItem
        {
            void Execute(DateTime dateTime);
            void Abort();
        }
        
        
        public interface ICronDaemon
        {
            void AddJob(string schedule, ThreadStart action);
            void Start();
            void Stop();
        }


    public class CronJob : ICronJobItem
    {
        private readonly ICronSchedule _cronSchedule;
        private readonly ThreadStart _threadStart;
        private Thread _thread;

        public CronJob(string schedule, ThreadStart threadStart)
        {
            _cronSchedule = new CronSchedule(schedule);
            _threadStart = threadStart;
            _thread = new Thread(threadStart);
        }

        private readonly object _lock = new object();
        public void Execute(DateTime dateTime)
        {
            lock (_lock)
            {
                if (!_cronSchedule.IsTime(dateTime))
                    return;

                if (_thread.ThreadState == ThreadState.Running)
                    return;

                _thread = new Thread(_threadStart);
                _thread.Start();
            }
        }

        public void Abort()
        {
            _thread.Abort();
        }
    }
    
    public class CronDaemon : ICronDaemon
    {
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(30000);
        private readonly List<ICronJobItem> _cronJobs = new List<ICronJobItem>();
        private DateTime _last = DateTime.Now;

        public CronDaemon()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsed;
        }

        public void AddJob(string schedule, ThreadStart action)
        {
            var cj = new CronJob(schedule, action);
            _cronJobs.Add(cj);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();

            foreach (var cronJobItem in _cronJobs)
            {
                var job = (CronJob) cronJobItem;
                job.Abort();
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Minute != _last.Minute)
            {
                _last = DateTime.Now;
                foreach (ICronJobItem job in _cronJobs)
                    job.Execute(DateTime.Now);
            }
        }
    }
}
