using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Hive.Shared.Common
{
    public static class HostBuilderExtensions
    {
        private const string ConfigureServicesMethodName = "ConfigureServices";

        public static IHostBuilder AddApplicationInsights(this IHostBuilder hostBuilder)
        {
            _ = hostBuilder.ConfigureLogging((context, logger) =>
              {
                  _ = logger.AddApplicationInsights(context.Configuration.GetSection("Logging").GetSection("ApplicationInsightsKey").Value);
                  _ = logger.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);
                  _ = logger.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
              });

            return hostBuilder;
        }

        public static IHostBuilder UseStartup<TStartup>(this IHostBuilder hostBuilder) where TStartup : class
        {
            _ = hostBuilder.ConfigureServices((ctx, serviceCollection) =>
              {
                  var startup = typeof(TStartup).GetMethod(ConfigureServicesMethodName, new[] { typeof(IServiceCollection) });
                  var startupWithHostEnvironment = typeof(TStartup).GetMethod(ConfigureServicesMethodName, new[] { typeof(IServiceCollection), typeof(IHostEnvironment) });

                  var hasConfigCtor = typeof(TStartup).GetConstructor(new[] { typeof(IConfiguration) }) != null;
                  var startUpObj = hasConfigCtor ? (TStartup?)Activator.CreateInstance(typeof(TStartup), ctx.Configuration) : (TStartup?)Activator.CreateInstance(typeof(TStartup), null);
                  _ = serviceCollection.AddTransient<ResponseValidationInterceptor>();

                  if (startup != null)
                      _ = startup.Invoke(startUpObj, new object[] { serviceCollection });
                  else if (startupWithHostEnvironment != null)
                      _ = startupWithHostEnvironment.Invoke(startUpObj, new object[] { serviceCollection, ctx.HostingEnvironment });
              });

            return hostBuilder;
        }
    }
}