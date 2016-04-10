using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;
using SpiderCore.Cache;
using SpiderCore.Extendsions;

namespace SpiderCore
{
    public class Spider : ISpider
    {
        private readonly CacheContext _cacheContext = new RedisContext();
        private readonly ILog _logger = LogManager.GetLogger(typeof(Spider));

        public MessageContext Extract(Response response)
        {
            return ExtractHtmlResponse(response as HtmlResponse);
        }

        /// <summary>
        /// 判断指定的URL哈希是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool Exist(string url)
        {
            var cacheItem = _cacheContext.Get<string>(url);

            return string.IsNullOrEmpty(cacheItem) == false;
        }

        /// <summary>
        /// 解析Html内容
        /// 
        /// 处理步骤：
        /// 1.解析出全部的A链接href地址
        /// 2.逐个对md5(url)签名,并验证是否已存在
        /// 3.如果不存在，则添加到调度器中
        /// 4.返回 MessageContext 消息
        /// </summary>
        /// <param name="htmlResponse"></param>
        /// <returns></returns>
        private HtmlMessageContext ExtractHtmlResponse(HtmlResponse htmlResponse)
        {
            //判断是否提取文档链接
            if (htmlResponse.Request.IsExtractLink)
            {
                ExtractLink(htmlResponse);
            }

            var message = new HtmlMessageContext
            {
                Response = htmlResponse.Clone() as Response,
                Text = htmlResponse.Html
            };

            return message;
        }

        /// <summary>
        /// 提取文档链接
        /// </summary>
        /// <param name="htmlResponse"></param>
        private void ExtractLink(HtmlResponse htmlResponse)
        {
            //解析出全部Url并去除重复记录
            var urlList = GetHrefUrlList(htmlResponse.Html, htmlResponse.Request.Url).Distinct();

            //过滤第三方网站域名
            if (htmlResponse.Request.IgnoreThirdpartyDomain)
            {
                var uriHost = new Uri(htmlResponse.Request.Url).Host.Replace("www.", "");

                urlList = urlList.Where(p => p.Contains(uriHost)).ToList();
            }

            //循环处理url列表
            foreach (var url in urlList)
            {
                try
                {
                    var urlHash = url.ToMd5();
                    var redisKey = string.Format("spider:url:{0}", urlHash);

                    //是否满足抓取条件
                    if (Exist(redisKey) == false)
                    {
                        var requestUri = new Uri(url);
                        var request = htmlResponse.Request.Clone() as Request;
                        if (request != null && requestUri.IsFile == false)
                        {
                            request.Url = url;
                            request.Referer = htmlResponse.Request.Url;

                            //推送到调度队列
                            SchedulerManage.Instance.Push(request);

                            var redisValue = JsonConvert.SerializeObject(request);

                            //推送到缓存
                            _cacheContext.Set(redisKey, redisValue);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception);
                }
            }
        }

        /// <summary>
        /// 
        /// 链接正则：href="[\s\S\W]+?"
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rootUrl"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetHrefUrlList(string html, string rootUrl)
        {

            if (string.IsNullOrEmpty(html)) return new List<string>();

            var uri = new Uri(rootUrl);
            var result = new List<string>();

            var matchCollection = Regex.Matches(html, "href=\"[\\s\\S\\W]+?\"", RegexOptions.IgnoreCase);
            for (var i = 0; i < matchCollection.Count; i++)
            {
                var href = matchCollection[i].Value;

                if (href.Length < 6) continue;

                href = href.Substring(6, href.Length - 7);

                if (href.StartsWith("#", StringComparison.CurrentCultureIgnoreCase) || href == "/") continue;

                if (href.StartsWith("javascript:", StringComparison.CurrentCultureIgnoreCase)) continue;

                if (href.StartsWith("//", StringComparison.CurrentCultureIgnoreCase))
                {
                    href = String.Format("http:{0}", href);

                    result.Add(href);
                }

                if (href.StartsWith("/", StringComparison.CurrentCultureIgnoreCase) )
                {
                    //补http协议

                    href = String.Format("{0}://{1}/{2}", uri.Scheme, uri.Host, href.TrimStart('/'));

                    result.Add(href);

                    continue;
                }

                if (href.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) ||
                    href.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                {
                    //直接返回

                    result.Add(href);

                    continue;
                }

                //other
            }

            return result;
        }
    }
}
