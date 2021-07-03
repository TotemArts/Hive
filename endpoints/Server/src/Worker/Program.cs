using Microsoft.Extensions.Hosting;
using Serilog;
using Hive.Shared.Common;

namespace Hive.Endpoints.Server.Worker
{
    public static class Program
    {
        public static void Main(string[] args) =>
            HiveLog.UseSerilog(() =>
                Host.CreateDefaultBuilder(args)
                    .UseStartup<StartUp>()
                    .AddApplicationInsights()
                    .UseSerilog()
                    .Build()
                    .Run()
            );
    }
}