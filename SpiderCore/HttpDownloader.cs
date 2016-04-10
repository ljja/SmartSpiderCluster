using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using log4net;
using SpiderCore.Util;

namespace SpiderCore
{
    public class HttpDownloader : IDownloader
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(HttpDownloader));

        public Response Download(Request request)
        {
            //启动计时器
            var stopwatch = new Stopwatch();
            Response response = null;

            try
            {
                if (request.Sleep > 0)
                {
                    Thread.Sleep(request.Sleep);
                }

                stopwatch.Start();

                //Build Request
                var req = (HttpWebRequest)WebRequest.Create(request.Url);

                req.Timeout = request.Timeout;

                req.Headers.Clear();
                foreach (var header in request.Header)
                {
                    req.Headers.Add(header.Name, header.Value);
                }

                if (string.IsNullOrEmpty(request.ContentType) == false)
                {
                    req.ContentType = request.ContentType;
                }

                if (string.IsNullOrEmpty(request.Host) == false)
                {
                    req.Host = request.Host;
                }

                if (string.IsNullOrEmpty(request.Method) == false)
                {
                    req.Method = request.Method;
                }

                if (string.IsNullOrEmpty(request.Referer) == false)
                {
                    req.Referer = request.Referer;
                }

                if (string.IsNullOrEmpty(request.UserAgent) == false)
                {
                    req.UserAgent = request.UserAgent;
                }

                req.CookieContainer = new CookieContainer();
                foreach (var cookie in request.Cookie)
                {
                    req.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value));
                }

                //GetResponse
                var rep = (HttpWebResponse)req.GetResponse();

                response = GetResponseContext(rep, request.Encoding);

                if (response == null) return null;

                //Header Handle
                for (var i = 0; i < rep.Headers.Count; i++)
                {
                    var key = rep.Headers[i];
                    var value = rep.Headers.Get(key);

                    response.Header.Add(new NameValue(key, value));
                }

                //Cookie Handle
                for (var i = 0; i < rep.Cookies.Count; i++)
                {
                    var key = rep.Cookies[i].Name;
                    var value = rep.Cookies[i].Value;
                    response.Cookie.Add(new NameValue(key, value));
                }

                //停止计时器
                stopwatch.Stop();

                request.DownloadTime = stopwatch.Elapsed.TotalMilliseconds;
                request.CrawlTime = DateTime.Now;

                response.Request = request;

                return response;
            }
            catch (TimeoutException timeoutException)
            {
                _logger.Error(timeoutException);
            }
            catch (Exception ex)
            {
                //抓取错误,切换网站
                SchedulerManage.Instance.Switch();

                _logger.Error(ex);
            }
            finally
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }

                if (response != null && response.GetType() == typeof(HtmlResponse))
                {
                    var htmlResponse = response as HtmlResponse;
                    if (htmlResponse != null)
                    {
                        Console.WriteLine("{0}-{1}-{2}", stopwatch.Elapsed, htmlResponse.Html.Length, request.Url);
                    }
                }
                else
                {
                    Console.WriteLine("{0}-{1}", stopwatch.Elapsed, request.Url);
                }

            }

            return null;
        }

        private Response GetResponseContext(HttpWebResponse httpWebResponse, string encoding)
        {
            //ContentType:text/html
            if (string.IsNullOrEmpty(httpWebResponse.ContentType) || httpWebResponse.ContentType.Contains("text/html"))
            {
                return GetTextHtmlResponse(httpWebResponse, encoding);
            }

            return null;
        }

        /// <summary>
        /// ContentType:text/html
        /// </summary>
        /// <param name="httpWebResponse"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private HtmlResponse GetTextHtmlResponse(HttpWebResponse httpWebResponse, string encoding)
        {
            var response = new HtmlResponse();

            using (var stream = httpWebResponse.GetResponseStream())
            {
                if (stream == null) return response;

                using (var reader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                {
                    response.Html = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                }

                stream.Close();
                stream.Dispose();
            }

            return response;
        }
    }
}
