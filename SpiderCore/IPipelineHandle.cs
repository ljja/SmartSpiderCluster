using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public interface IPipelineHandle
    {
        /// <summary>
        /// 过滤路径规则
        /// </summary>
        string FilterPath { get; }

        /// <summary>
        /// 内容提取
        /// </summary>
        /// <param name="message">消息内容</param>
        void Extract(MessageContext message);
    }
}
