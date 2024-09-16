using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerTiming.DotNet.Core
{
    public static class ResponseExtensions
    {
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static bool WriteServerTimingHeaders(this IResponseWrapper response, Action<TimingHeaderWriter> writer)
        {
            if (response.HeadersWritten)
                return false;
            var timingHeaderWriter = new TimingHeaderWriter();
            writer(timingHeaderWriter);
            foreach (var list in timingHeaderWriter)
            {
                WriteServerTimingHeader(response, list);
            }

            return true;
        }

        private static void WriteServerTimingHeader(IResponseWrapper response,
            IEnumerable<TimingItem> list)
        {
            var value = list.Select(item => item.ToString()).ToList();

            response.AddHeader("Server-Timing", string.Join(", ", value));
        }

        public static bool WriteServerTimingHeader(this IResponseWrapper response, string name, double? duration = null,
            string description = null)
        {
            if (response.HeadersWritten)
                return false;
            WriteServerTimingHeader(response, new[]
            {
                new TimingItem()
                {
                    Name = name,
                    Description = description,
                    Duration = duration,
                }
            });
            return true;
        }

        private static int GetDepth<T>(T main, Func<T, IEnumerable<T>> subGetter)
        {
            if (main == null)
                return 0;
            var depth = 0;

            foreach (var item in subGetter(main) ?? Array.Empty<T>())
            {
                if (depth == 0)
                    depth = 1;
                var subItems = subGetter(item);
                if (subItems?.Any() == true)
                {
                    depth += GetDepth(item, subGetter);
                }
            }

            return depth;
        }

        public static IEnumerable<TimingItem> AsTimingHeaderValue(
            this KeyValuePair<string, PerformanceTimer.TimerSummary> summaryKeyValuePair)
        {
            var depth = GetDepth(summaryKeyValuePair, x => x.Value.SubSummary);
            if (depth == 0)
            {
                return new[]
                {
                    new TimingItem()
                    {
                        Name = summaryKeyValuePair.Key,
                        Duration = summaryKeyValuePair.Value.TotalMilliseconds,
                        Description = summaryKeyValuePair.Value.Description,
                    },
                };
            }

            if (depth >= 1)
            {
                return new[]
                    {
                        new TimingItem()
                        {
                            Name = summaryKeyValuePair.Key,
                        },
                    }
                    .Concat(summaryKeyValuePair.Value.SelfMilliseconds > 0
                        ? new[]
                        {
                            new TimingItem()
                            {
                                Name = "self",
                                Duration = summaryKeyValuePair.Value.SelfMilliseconds,
                            }
                        }
                        : Array.Empty<TimingItem>())
                    .Concat(summaryKeyValuePair.Value.SubSummary?.Select(sum => new TimingItem()
                    {
                        Name = sum.Key,
                        Duration = sum.Value.TotalMilliseconds,
                        Description = sum.Value.Description,
                    }) ?? Array.Empty<TimingItem>()).ToArray();
            }


            return Array.Empty<TimingItem>();
        }
    }
}