using System;
using System.Linq;
using GreenPipes;
using Hive.Shared.Common;
using Hive.Shared.Common.MassTransit.Bus;
using Hive.Shared.Common.Options;
using MassTransit;
using MassTransit.Context;
using MassTransit.ExtensionsDependencyInjectionIntegration.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration, Action<IBusConfigurator>? configurator = null)
        {
            var rabbitMqOptions = ConfigurationToOptions(configuration);

            if (services.Any(d => d.ServiceType == typeof(IBus)))
                throw new Exception("AddMassTransit() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");

            var bus = new ServiceCollectionBusConfigurator(services);
            BusConfigurator config = new BusConfigurator(bus, services);
            configurator?.Invoke(config);

            bus.UsingRabbitMq((context, cfg) =>
            {
                LogContext.ConfigureCurrentLogContext(context.GetService<ILoggerFactory>());

                cfg.Host(new Uri(rabbitMqOptions.Url ?? throw new Exception("MassTransit__Url is not configured properly.")), r =>
                {
                    r.Username(rabbitMqOptions.Username);
                    r.Password(rabbitMqOptions.Password);
                    r.Heartbeat(rabbitMqOptions.Heartbeat);
                });

                if (config.HasConsumers)
                    cfg.ReceiveEndpoint(DomainInformation.ServiceName, ep =>
                    {
                        ep.UseMessageRetry(policy => policy.Interval(10, TimeSpan.FromMilliseconds(333)).Handle<ObjectDisposedException>());
                        config.HandleExceptions(ep);

                        if (config.HasConsumers) ep.ConfigureConsumers(context);
                    });
            });
            
            services.AddMassTransitHostedService();

            return services;
        }
        
        private static RabbitMqOptions ConfigurationToOptions(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("MassTransit");
            var options = new RabbitMqOptions();
            configurationSection.Bind(options);
            return options;
        }
    }
}