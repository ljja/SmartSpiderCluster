using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiderCore.Cache;
using SpiderCore.PipelineHandle;
using StackExchange.Redis;

namespace SpiderCore.Tests
{
    [TestClass]
    public class LocalSpiderEngineTests
    {
        private readonly LocalSpiderEngine _localSpiderEngine = new LocalSpiderEngine();

        [TestMethod]
        public void StartTest()
        {
            if (RedisContext.RedisDatabase == null)
            {
                RedisContext.RedisDatabase = ConnectionMultiplexer.Connect("cache.spider.mfniu.com").GetDatabase();
            }

            //初始化引擎组件
            _localSpiderEngine.AddPipelineHandle(new LogPipelineHandle());

            //启动抓取引擎，自我循环运行
            _localSpiderEngine.Start();

            var request = new Request
            {
                Encoding = "utf-8",
                Url = "http://www.zycg.gov.cn/",
                Timeout = 30000,
                UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.89 Safari/537.37"
            };
            SchedulerManage.Instance.Push(request);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
