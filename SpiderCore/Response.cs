using System;
using System.Collections.Generic;
using SpiderCore.Util;

namespace SpiderCore
{
    public class Response : ICloneable
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// Header集合
        /// </summary>
        public List<NameValue> Header { get; set; }

        /// <summary>
        /// Cookie集合
        /// </summary>
        public List<NameValue> Cookie { get; set; }

        public Response()
        {
            Request = new Request();
            Header = new List<NameValue>();
            Cookie = new List<NameValue>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
