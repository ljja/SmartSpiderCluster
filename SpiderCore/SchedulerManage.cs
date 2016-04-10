using SpiderCore.Scheduler;

namespace SpiderCore
{
    public sealed class SchedulerManage
    {
        private readonly static IScheduler Scheduler = new ActiveMQScheduler();

        public static IScheduler Instance
        {
            get
            {
                return Scheduler;
            }
        }
    }
}
