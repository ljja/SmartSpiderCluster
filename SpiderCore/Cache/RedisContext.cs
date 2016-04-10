using System;
using System.Diagnostics;
using log4net;
using SpiderCore.Extendsions;
using StackExchange.Redis;

namespace SpiderCore.Cache
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public class RedisContext : CacheContext
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(RedisContext));

        public static IDatabase RedisDatabase;

        public override T Get<T>(string key)
        {
            try
            {
                return RedisDatabase.JsonGet<T>(key);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return null;
            }
        }

        public override bool Set<T>(string key, T t)
        {
            try
            {
                RedisDatabase.JsonSet(key, t);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return false;
            }
        }

        public override bool Remove(string key)
        {
            try
            {
                return RedisDatabase.KeyDelete(key);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return false;
            }
        }

        public override void Dispose()
        {
        }

    }
}
