using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public interface ISpider
    {
        /// <summary>
        /// 提取内容
        /// </summary>
        MessageContext Extract(Response response);

        /// <summary>
        /// 是否已存在
        /// </summary>
        bool Exist(string url);
    }
}
