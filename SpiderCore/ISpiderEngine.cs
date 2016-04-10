using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    /// <summary>
    /// 
    /// 设计理念：http://scrapy-chs.readthedocs.org/zh_CN/latest/topics/architecture.html
    /// 
    /// </summary>
    public interface ISpiderEngine
    {
        /// <summary>
        /// 下载器
        /// </summary>
        IDownloader Downloader
        {
            get;
            set;
        }

        /// <summary>
        /// 蜘蛛爬虫
        /// </summary>
        ISpider Spider
        {
            get;
            set;
        }

        /// <summary>
        /// 消息管道
        /// </summary>
        IPipeline Pipeline
        {
            get;
            set;
        }

        /// <summary>
        /// 开始
        /// </summary>
        void Start();

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 继续
        /// </summary>
        void Continue();

        /// <summary>
        /// 添加管道处理
        /// </summary>
        /// <param name="handle"></param>
        void AddPipelineHandle(IPipelineHandle handle);

        /// <summary>
        /// 移除管道处理
        /// </summary>
        void RemovePipelineHandle(SpiderCore.IPipelineHandle handle);
    }
}
