using System;
using System.Text.RegularExpressions;

namespace SpiderCore.PipelineHandle
{
    public class LogPipelineHandle : IPipelineHandle
    {
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

        public void Extract(MessageContext message)
        {
            var htmlMessage = message as HtmlMessageContext;
            if (htmlMessage != null)
            {
                var titleRegex = "<title>[\\s\\S\\W]+?</title>";
                var title = Regex.Match(htmlMessage.Text, titleRegex, RegexOptions.IgnoreCase).Value.Replace("<title>", "").Replace("</title>", "");

                Console.WriteLine("{0}-{1}-{2}-{3}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    htmlMessage.Text.Length,
                    htmlMessage.Response.Request.Url,
                    title);
            }
        }
    }
}
