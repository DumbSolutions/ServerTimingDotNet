using System.Web;
using ServerTiming.DotNet.Core;

namespace ServerTiming.DotNet.NetFramework4_7_2
{
    public static class HttpContextExtensions
    {
        public static PerformanceTimer GetTimer()
        {
            return HttpContext.Current.Items.GetServerResponseTimer();
        }
    }
}