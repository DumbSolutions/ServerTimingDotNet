using System.Web;
using ServerTiming.DotNet.Core;

namespace ServerTiming.DotNet.NetFramework4_7_2
{
    public static class HttpContextExtensions
    {
        public static PerformanceTimer GetTimer()
        {
            return GetTimer(HttpContext.Current);
        }

        public static PerformanceTimer GetTimer(HttpContext context)
        {
            return context.Items.GetServerResponseTimer();
        }
    }
}