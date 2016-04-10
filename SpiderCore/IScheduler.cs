using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public interface IScheduler
    {
        /// <summary>
        /// 推送
        /// </summary>
        bool Push(Request request);

        /// <summary>
        /// 弹出
        /// </summary>
        Request Pop();

        /// <summary>
        /// 切换网站
        /// </summary>
        void Switch();
    }
}
