using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore
{
    public interface IDownloader
    {
        Response Download(Request request);
    }
}
