using System.Collections.Generic;
using System.Linq;
using SpiderCore.Extendsions;

namespace SpiderCore
{
    public class MemoryScheduler : IScheduler
    {
        private readonly List<Request> _requestList = new List<Request>();

        public bool Push(Request request)
        {
            lock (_requestList)
            {
                request.UrlHash = request.Url.ToMd5();

                _requestList.Add(request);
            }

            return true;
        }

        public Request Pop()
        {
            Request item;

            lock (_requestList)
            {
                item = _requestList.Take(1).ToList().FirstOrDefault();

                if (_requestList.Count > 0)
                {
                    _requestList.RemoveAt(0);
                }
            }

            return item;
        }

        public void Switch()
        {
            throw new System.NotImplementedException();
        }
    }
}
