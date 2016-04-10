using System;
using log4net;
using Newtonsoft.Json;
using SpiderCore.Cache;

namespace SpiderCore.PipelineHandle
{
    public class RedisPipelineHandle : IPipelineHandle
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(RedisPipelineHandle));

        /// <summary>
        /// 消息管道
        /// 
        /// 示例取值：
        /// msg:text:html
        /// msg:text:json
        /// msg:text:xml
        /// 
        /// msg:image:png
        /// msg:image:jpeg
        /// 
        /// msg:file:doc
        /// msg:file:excel
        /// msg:file:pdf
        /// msg:file:txt
        /// 
        /// </summary>
        private const string MessageChannel = "msg:text:html";

        /// <summary>
        /// 匹配任意请求
        /// </summary>
        public string FilterPath
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// 发送消息至Redis订阅
        /// </summary>
        /// <param name="message"></param>
        public void Extract(MessageContext message)
        {
            if (message == null) return;

            try
            {
                var json = JsonConvert.SerializeObject(message);

                RedisContext.RedisDatabase.Publish(MessageChannel, json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
