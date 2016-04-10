using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json;
using SpiderCore.Cache;
using SpiderCore.Extendsions;

namespace SpiderCore
{
    /// <summary>
    /// 
    /// 
    /// 存储结构：队列
    /// 队列格式：spider:request:domain
    /// 数据示例：
    /// spider:request:www.mfniu.com
    /// spider:request:ma.mfniu.com
    /// 
    /// </summary>
    public class RedisScheduler : IScheduler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(RedisScheduler));

        /// <summary>
        /// 当前网站索引
        /// </summary>
        private int _currentIndex = 0;

        private string _currentRedisKey = "";

        private readonly string _filename = "domain.txt";

        private List<string> _domainList = new List<string>();

        private const string RedisQueueRequestUrlFormat = "spider:request:{0}";

        public RedisScheduler()
        {
            //load domain data
            LoadDomainList();

            SetCurrentRedisKey();
        }

        public bool Push(Request request)
        {
            try
            {
                if (request == null) return false;

                request.UrlHash = request.Url.ToMd5();

                var json = JsonConvert.SerializeObject(request);

                var uri = new Uri(request.Url);

                var redisKey = string.Format(RedisQueueRequestUrlFormat, uri.Host).ToLower();

                RedisContext.RedisDatabase.ListLeftPush(redisKey, json);

                if (_domainList.Contains(uri.Host) == false)
                {
                    _domainList.Add(uri.Host);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public Request Pop()
        {
            try
            {
                if (_domainList.Any() == false) return null;

                var json = RedisContext.RedisDatabase.ListRightPop(_currentRedisKey);

                if (string.IsNullOrEmpty(json)) return null;

                return JsonConvert.DeserializeObject<Request>(json);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public void Switch()
        {
            if (_currentIndex >= _domainList.Count)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex++;
            }

            SetCurrentRedisKey();
        }

        private void SetCurrentRedisKey()
        {
            if (_domainList.Count == 0 || _currentIndex >= _domainList.Count) return;

            var domain = _domainList[_currentIndex];

            _currentRedisKey = string.Format(RedisQueueRequestUrlFormat, domain).ToLower();
        }

        private void LoadDomainList()
        {
            if (File.Exists(_filename) == false)
            {
                File.WriteAllText(_filename, String.Empty, Encoding.UTF8);
            }

            _domainList = File.ReadAllLines(_filename, Encoding.UTF8).Distinct().Select(p => p.ToLower()).ToList();

        }

        ~RedisScheduler()
        {
            using (var sw = new StreamWriter(_filename, false, Encoding.UTF8))
            {
                _domainList.Distinct().ToList().ForEach(p => sw.WriteLine(p));

                sw.Close();
                sw.Dispose();
            }

        }
    }
}
