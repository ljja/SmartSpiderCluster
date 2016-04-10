using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace SpiderCore.Extendsions
{
    public static class StackExchangeRedisExtensions
    {
        public static T JsonGet<T>(this IDatabase cache, string key)
        {
            var json = cache.StringGet(key);

            if (string.IsNullOrEmpty(json)) return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void JsonSet<T>(this IDatabase cache, string key, T t)
        {
            var json = JsonConvert.SerializeObject(t);

            cache.StringSet(key, json);
        }

        public static T BinaryGet<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(cache.StringGet(key));
        }

        public static void BinarySet(this IDatabase cache, string key, object value)
        {
            cache.StringSet(key, Serialize(value));
        }

        private static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                var objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(stream))
            {
                var result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
    }
}
