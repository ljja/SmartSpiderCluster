using System;

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
