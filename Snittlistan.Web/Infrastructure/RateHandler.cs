﻿namespace Snittlistan.Web.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    public class RateHandler : DelegatingHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly object locker = new object();
        private readonly double rate;
        private readonly double per;
        private readonly int maxTries;
        private readonly double sleepSeconds;
        private double allowance;
        private Stopwatch stopwatch;

        public RateHandler(double rate, double per, int maxTries)
            : base(new HttpClientHandler())
        {
            if (rate < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be >= 1");
            }

            this.rate = rate;
            this.per = per;
            allowance = rate;
            this.maxTries = maxTries;
            sleepSeconds = per / rate / 2;
            stopwatch = Stopwatch.StartNew();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var allowed = false;
            for (var currentTry = 0; currentTry < maxTries; currentTry++)
            {
                lock (locker)
                {
                    var increase = stopwatch.Elapsed.TotalSeconds * (rate / per);
                    Logger.Debug($"Time passed={stopwatch.ElapsedMilliseconds}ms Increasing allowance={allowance} to {allowance + increase}");
                    stopwatch = Stopwatch.StartNew();
                    allowance += increase;

                    if (allowance > rate)
                    {
                        Logger.Debug($"Allowance={allowance} exceeded rate ({rate:F1} / {per:F1})");
                        allowance = rate;
                    }

                    if (allowance >= 1.0)
                    {
                        Logger.Debug($"Allowance={allowance} >= 1, request allowed, decreasing to {allowance - 1}");
                        allowance--;
                        allowed = true;
                        break;
                    }

                    Logger.Debug($"Allowance={allowance} < 1.0, delay={sleepSeconds}");
                }

                // not yet, delay then try again
                await Task.Delay(TimeSpan.FromSeconds(sleepSeconds), cancellationToken);
            }

            if (!allowed)
            {
                throw new Exception($"Failed after {maxTries} tries (elapsed = {sw.Elapsed.TotalSeconds:F1}s)");
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}