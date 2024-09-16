using System;

namespace ServerTiming.DotNet.Core
{
    public interface IPerformanceTimer
    {
        IDisposable Time();
        IDisposable Time(string key);
        IPerformanceTimer Sub(string key);
    }
}