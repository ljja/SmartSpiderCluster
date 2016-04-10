using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public interface IPipeline
    {
        void Push(MessageContext message);

        /// <param name="handle">增加管道处理</param>
        void AddPipelineHandle(IPipelineHandle handle);

        /// <param name="handle">移除管道处理</param>
        void RemovePipelineHandle(IPipelineHandle handle);
    }
}
