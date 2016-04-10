using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public sealed class SchedulerManage
    {
        private readonly static IScheduler Scheduler = new RedisScheduler();

        public static IScheduler Instance
        {
            get
            {
                return Scheduler;
            }
        }
    }
}
