using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public class HtmlMessageContext : MessageContext
    {
        /// <summary>
        /// Html文本内容
        /// </summary>
        public string Text { get; set; }

        public HtmlMessageContext()
        {
            Text = String.Empty;
        }
    }
}
