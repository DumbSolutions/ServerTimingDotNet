using System;
using System.Web;
using ServerTiming.DotNet.Core;

namespace ServerTiming.DotNet.NetFramework4_7_2
{
    public class TimingModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextOnBeginRequest;
            context.EndRequest += ContextOnEndRequest;
        }

        

        private void ContextOnBeginRequest(object sender, EventArgs e)
        {
            if (!(sender is HttpApplication httpApplication))
                return;
            
            var context = httpApplication.Context;
            var performanceTimer = PerformanceTimer.StartNew();
            context.Items.SetServerResponseTimer(performanceTimer);
        }
        
        private void ContextOnEndRequest(object sender, EventArgs e)
        {
            if (!(sender is HttpApplication httpApplication))
                return;
            var context = httpApplication.Context;
            
            var performanceTimer = context.Items.GetServerResponseTimer();
            
            if (performanceTimer == null) return;
            
            performanceTimer.Stop();

            var selfTotalMilliseconds = performanceTimer.GetSelfTotalMilliseconds();
            HttpResponseWrapper.Wrap(context.Response).WriteServerTimingHeaders(x =>
            {
                foreach (var summary in performanceTimer.GetSummary())
                {
                    x.AddAsOneList(summary.AsTimingHeaderValue());
                }
                x.Add("total",duration: selfTotalMilliseconds);
            });
        }

        public void Dispose()
        {
            
        }
    }
}