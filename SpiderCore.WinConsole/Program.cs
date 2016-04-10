using System;
using System.Collections.Generic;
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
            if (RedisContext.RedisDatabase == null)
            {
                RedisContext.RedisDatabase = ConnectionMultiplexer.Connect("cache.spider.com").GetDatabase();
            }

            var localSpiderEngine = new LocalSpiderEngine();

            //初始化引擎组件
            localSpiderEngine.AddPipelineHandle(new LogPipelineHandle());
            localSpiderEngine.AddPipelineHandle(new RedisPipelineHandle());
            
            if (args != null && args.Length == 3)
            {
                var request = GetParamRequest(args);
                if (request != null)
                {
                    SchedulerManage.Instance.Push(request);
                }
            }
            else if (args != null && args.Length == 5)
            {
                int startIndex; int.TryParse(args[3], out startIndex);
                int endIndex; int.TryParse(args[4], out endIndex);

                for (var i = startIndex; i <= endIndex; i++)
                {
                    var param = args.Clone() as string[];
                    if (param != null)
                    {
                        param[0] = String.Format(param[0], i);
                        var request = GetParamRequest(param);
                        if (request != null)
                        {
                            SchedulerManage.Instance.Push(request);
                        }
                    }
                }

                return;
            }

            //启动抓取引擎，自我循环运行
            localSpiderEngine.Start();

            while (true)
            {
                Thread.Sleep(10000);
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
