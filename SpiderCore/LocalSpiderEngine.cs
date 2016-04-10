using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpiderCore
{
    public class LocalSpiderEngine : ISpiderEngine
    {
        private readonly Thread _localSpiderEngineThread;

        public IDownloader Downloader { get; set; }

        public ISpider Spider { get; set; }

        public IPipeline Pipeline { get; set; }

        public LocalSpiderEngine()
        {
            _localSpiderEngineThread = new Thread(LoopSchedulerPop)
            {
                Name = GetType().ToString()
            };

            Downloader = new HttpDownloader();
            Spider = new Spider();
            Pipeline = new Pipeline();
        }

        public void Start()
        {
            _localSpiderEngineThread.Start();
        }

        public void Pause()
        {
            _localSpiderEngineThread.Join();
        }

        public void Stop()
        {
            _localSpiderEngineThread.Abort();
        }

        public void Continue()
        {
            _localSpiderEngineThread.Resume();
        }

        public void AddPipelineHandle(IPipelineHandle handle)
        {
            Pipeline.AddPipelineHandle(handle);
        }

        public void RemovePipelineHandle(IPipelineHandle handle)
        {
            Pipeline.RemovePipelineHandle(handle);
        }

        private void LoopSchedulerPop()
        {
            while (true)
            {
                var request = SchedulerManage.Instance.Pop();
                if (request == null)
                {
                    SchedulerManage.Instance.Switch();
                    continue;
                }

                var response = Downloader.Download(request);
                if (response != null)
                {
                    var message = Spider.Extract(response);

                    Pipeline.Push(message);
                }
            }
        }
    }
}
