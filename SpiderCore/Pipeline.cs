using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpiderCore
{
    public class Pipeline : IPipeline
    {
        private readonly List<IPipelineHandle> _pipelineHandleList = new List<IPipelineHandle>();

        public void Push(MessageContext message)
        {
            foreach (var handle in _pipelineHandleList)
            {
                if (Regex.Match(message.Response.Request.Url, handle.FilterPath).Success)
                {
                    handle.Extract(message);
                }
            }
        }

        public void AddPipelineHandle(IPipelineHandle handle)
        {
            _pipelineHandleList.Add(handle);
        }

        public void RemovePipelineHandle(IPipelineHandle handle)
        {
            _pipelineHandleList.Remove(handle);
        }
    }
}
