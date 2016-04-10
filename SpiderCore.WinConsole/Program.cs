using System;
using System.Threading;
using SpiderCore.Cache;
using SpiderCore.PipelineHandle;
using StackExchange.Redis;

namespace SpiderCore.WinConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // host config
            // 127.0.0.1 cache.spider.com
            if (RedisContext.RedisDatabase == null)
            {
                try
                {
                    RedisContext.RedisDatabase = ConnectionMultiplexer.Connect("cache.spider.com").GetDatabase();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return;
                }
            }

            try
            {
                var localSpiderEngine = new LocalSpiderEngine();

                //初始化引擎组件
                localSpiderEngine.AddPipelineHandle(new ActiveMQPipelineHandle());

                //种子请求地址
                if (args != null && args.Length >= 3)
                {
                    var request = GetParamRequest(args);

                    SchedulerManage.Instance.Push(request);
                }

                //启动抓取引擎，自我循环运行
                localSpiderEngine.Start();

                while (true)
                {
                    Thread.Sleep(10000);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static Request GetParamRequest(string[] args)
        {
            if (args == null || args.Length < 2) return null;

            var url = args[0];
            var encoding = args[1];
            int sleep;

            int.TryParse(args[2], out sleep);

            var request = new Request
            {
                Encoding = encoding,
                Url = url,
                Timeout = 10000,
                UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.89 Safari/537.36",
                Sleep = sleep
            };

            return request;
        }
    }
}
