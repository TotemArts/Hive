using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hive.Shared.Common
{
    public static class DomainInformation
    {
        private static readonly List<string> Suffixes = new List<string> { "API", "Worker" };
        public static string ServiceName => Assembly.GetEntryAssembly()?.GetName().Name ?? throw new Exception("Could not gather domain information.");

        public static string DomainName
        {
            get
            {
                var serviceName = ServiceName;
                Suffixes.ForEach(x => serviceName = serviceName.EndsWith(x) ? serviceName.Replace(x, "").TrimEnd('.') : serviceName);
                return serviceName;
            }
        }

        public static string WorkerServiceName = DomainName + ".Worker";
        public static string ApiServiceName = DomainName + ".API";
    }
}