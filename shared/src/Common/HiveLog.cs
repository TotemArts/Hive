using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Hive.Shared.Common
{
    public static class HiveLog
    {
        public static void UseSerilog(Action action, LogEventLevel? level = default)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProcessName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadName()
                .Enrich.WithThreadId()
                //.WriteTo.OpenTracing()
                .WriteTo.Console();

            if (level.HasValue)
            {
                loggerConfiguration = loggerConfiguration.MinimumLevel.Is(level.Value);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            try
            {
                Log.Information("HiveLog: Starting up");
                Log.Information("HiveLog: Logging to: {ServerUrl}", configuration["Serilog:WriteTo:0:Args:serverUrl"]);
                action();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}