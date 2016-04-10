using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpiderCore.Util;

namespace SpiderCore
{
    public class Request : ICloneable
    {
        /// <summary>
        /// Url地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// UrlMd5签名
        /// </summary>
        public string UrlHash { get; set; }

        /// <summary>
        /// Header集合
        /// </summary>
        public List<NameValue> Header { get; set; }

        /// <summary>
        /// Cookie集合
        /// </summary>
        public List<NameValue> Cookie { get; set; }

        /// <summary>
        /// Http请求方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// 浏览器头
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 默认超时30秒
        /// </summary>
        [JsonIgnore]
        public int Timeout { get; set; }

        /// <summary>
        /// 默认休眠1000毫秒
        /// </summary>
        [JsonIgnore]
        public int Sleep { get; set; }

        /// <summary>
        /// 忽略外部网站域名
        /// </summary>
        [JsonIgnore]
        public bool IgnoreThirdpartyDomain { get; set; }

        /// <summary>
        /// 是否提取链接，默认true
        /// </summary>
        [JsonIgnore]
        public bool IsExtractLink { get; set; }

        /// <summary>
        /// 抓取完成时间
        /// </summary>
        public DateTime CrawlTime { get; set; }

        /// <summary>
        /// 总下载时间毫秒
        /// </summary>
        public double DownloadTime { get; set; }

        public Request()
        {
            Cookie = new List<NameValue>();
            Header = new List<NameValue>();
            Url = string.Empty;
            Method = "GET";
            Encoding = "utf-8";
            UserAgent = "mfniu-spider";
            ContentType = "text/html";
            Host = String.Empty;
            Referer = String.Empty;
            Timeout = 30000;
            Sleep = 0;
            IgnoreThirdpartyDomain = true;
            IsExtractLink = true;
            UrlHash = String.Empty;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
