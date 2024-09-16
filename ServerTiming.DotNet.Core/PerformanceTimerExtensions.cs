using System.Collections;

namespace ServerTiming.DotNet.Core
{
    public static class PerformanceTimerExtensions
    {
        public static PerformanceTimer GetServerResponseTimer(this IDictionary dictionary)
        {
            return PerformanceTimer.GetServerResponseTimerFromDictionary(dictionary);
        }
        
        public static void SetServerResponseTimer(this IDictionary dictionary, PerformanceTimer timer)
        {
            PerformanceTimer.SetServerResponseTimerInDictionary(dictionary, timer);
        }
    }
}