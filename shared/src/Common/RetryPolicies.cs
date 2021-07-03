using System;
using GreenPipes;
using GreenPipes.Configurators;

namespace Hive.Shared.Common
{
    public static class RetryPolicies
    {
        public static Action<IRetryConfigurator> None => retryConfigurator => retryConfigurator.None();
        public static Action<IRetryConfigurator> EveryTenSecondsForAMinute => retryConfigurator => retryConfigurator.Interval(6, TimeSpan.FromSeconds(1));
        public static Action<IRetryConfigurator> ExponentialForHundredTimes => retryConfigurator => retryConfigurator.Exponential(100, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1), TimeSpan.FromSeconds(5));
    }
}