using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ServerTiming.DotNet.Core
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PerformanceTimer : IPerformanceTimer
    {
        private readonly Stopwatch _selfTime = new Stopwatch();
        private readonly Dictionary<string, PerformanceTimer> _subTimers = new Dictionary<string, PerformanceTimer>();

        public IDisposable Time()
        {
            return new TimerDisposable(this);
        }

        public void Start()
        {
            _selfTime.Start();
        }

        public void Stop()
        {
            _selfTime.Stop();
        }

        public static PerformanceTimer StartNew()
        {
            var timer = new PerformanceTimer();
            timer.Start();
            return timer;
        }

        public IDisposable Time(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            var timerObject = GetOrCreateSub(key);
            return new TimerDisposable(timerObject);
        }

        private PerformanceTimer GetOrCreateSub(string key)
        {
            PerformanceTimer timerObject;
            if (_subTimers.TryGetValue(key, out var timer))
            {
                timerObject = timer;
            }
            else
            {
                timerObject = new PerformanceTimer();
                _subTimers.Add(key, timerObject);
            }

            return timerObject;
        }

        public IPerformanceTimer Sub(string key)
        {
            var timerObject = GetOrCreateSub(key);
            return timerObject;
        }

        public TimeSpan GetTotalTime()
        {
            var timeSpan = new TimeSpan(0);
            timeSpan = timeSpan.Add(GetSelfTime());
            timeSpan = timeSpan.Add(GetSubTotalTime());
            return timeSpan;
        }

        public long GetTotalMilliseconds()
        {
            return GetSelfTotalMilliseconds() + GetSubTotalMilliseconds();
        }

        public TimeSpan GetSelfTime()
        {
            return _selfTime?.Elapsed ?? TimeSpan.Zero;
        }

        public long GetSelfTotalMilliseconds()
        {
            return _selfTime?.ElapsedMilliseconds ?? 0;
        }

        public TimeSpan GetSubTotalTime()
        {
            return _subTimers.Values.Aggregate(TimeSpan.Zero, (agg, item) => agg.Add(item.GetTotalTime()));
        }

        public long GetSubTotalMilliseconds()
        {
            return _subTimers.Values.Sum(item => item.GetTotalMilliseconds());
        }

        public Dictionary<string, TimerSummary> GetSummary()
        {
            if (_subTimers.Count == 0)
                return new Dictionary<string, TimerSummary>();
            var result = new Dictionary<string, TimerSummary>();
            foreach (var keyValuePair in _subTimers)
            {
                result.Add(keyValuePair.Key, new TimerSummary(keyValuePair.Value));
            }

            return result;
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class TimerSummary
        {
            internal TimerSummary(PerformanceTimer value)
            {
                SelfMilliseconds = value.GetSelfTotalMilliseconds();
                TotalMilliseconds = value.GetTotalMilliseconds();
                SubMilliseconds = value.GetSubTotalMilliseconds();
                SubSummary = value.GetSummary();
            }

            public long TotalMilliseconds { get; }
            public long SelfMilliseconds { get; }
            public long SubMilliseconds { get; }
            public Dictionary<string, TimerSummary> SubSummary { get; }
            public string Description { get; set; }
        }


        private class TimerDisposable : IDisposable
        {
            private readonly PerformanceTimer _timerObject;

            public TimerDisposable(PerformanceTimer timerObject)
            {
                _timerObject = timerObject;
                if (timerObject._selfTime.IsRunning)
                    throw new InvalidOperationException("Stopwatch is already running");
                timerObject._selfTime.Start();
            }

            public void Dispose()
            {
                if (!_timerObject._selfTime.IsRunning)
                    throw new InvalidOperationException("Stopwatch was already stopped");
                _timerObject._selfTime.Stop();
            }
        }

        // ReSharper disable once InconsistentNaming
        private const string TIMER_SRVRESPONSE = "DUMBSOLUTIONS_SERVERTIMING_TIMER_SERVERRESPONSE";

        public static PerformanceTimer GetServerResponseTimerFromDictionary(IDictionary dictionary)
        {
            if (dictionary.Contains(TIMER_SRVRESPONSE))
            {
                return dictionary[TIMER_SRVRESPONSE] as PerformanceTimer;
            }

            return null;
        }

        public static void SetServerResponseTimerInDictionary(IDictionary dictionary, PerformanceTimer timer)
        {
            dictionary[TIMER_SRVRESPONSE] = timer;
        }
    }
}