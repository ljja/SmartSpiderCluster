using System;
using System.Diagnostics;

namespace SpiderCore.Util
{
    public class ProcessUtil
    {
        public static void RestartProcess()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName + ".exe");

            Environment.Exit(0);
        }
    }
}
