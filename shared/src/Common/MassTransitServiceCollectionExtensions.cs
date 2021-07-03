using System;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Context;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hive.Shared.Common.Options;

namespace Hive.Shared.Common
{
    public static class MassTransitServiceCollectionExtensions
    {
        public static IServiceCollection AddHiveMassTransit(this IServiceCollection services,
            Action<IServiceCollectionBusConfigurator>? serviceCollectionBusConfigure = null,
            Action<IRegistration, IRabbitMqBusFactoryConfigurator>? configure = null)
        {
            _ = services.AddMassTransit(bus =>
            {
                serviceCollectionBusConfigure?.Invoke(bus);

                bus.UsingRabbitMq((context, cfg) =>
                {
                    LogContext.ConfigureCurrentLogContext(context.GetService<ILoggerFactory>());

                    var rabbitMqOptions = context.GetService<IOptions<RabbitMqOptions>>()?.Value ?? new RabbitMqOptions();
                    cfg.Host(new Uri(rabbitMqOptions.Url), r =>
                    {
                        r.Username(rabbitMqOptions.Username);
                        r.Password(rabbitMqOptions.Password);
                        r.Heartbeat(rabbitMqOptions.Heartbeat);
                    });

                    configure?.Invoke(context, cfg);
                });
            });

            _ = services.AddMassTransitHostedService();

            return services;
        }

        public static IServiceCollectionBusConfigurator AddConsumerWithRetry<TEventHandler>(this IServiceCollectionBusConfigurator busConfigurator, Action<IRetryConfigurator> retryConfigurator)
            where TEventHandler : class, IConsumer
        {
            busConfigurator.AddConsumer<TEventHandler>(consumerConfigurator => consumerConfigurator.UseRetry(retryConfigurator));
            return busConfigurator;
        }
    }
}