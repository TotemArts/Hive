using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Endpoints.Server.Worker.CommandHandlers;
using Hive.Endpoints.Server.Worker.CommandManagers;
using Hive.Shared.Common;
using Hive.Shared.Common.Context;
using Hive.Shared.Common.Options;

namespace Hive.Endpoints.Server.Worker
{
    public class StartUp
    {
        private static readonly string ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new Exception("ServiceName could not be derived from Assembly");
        private readonly IConfiguration _configuration;

        public StartUp(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IGameServerCommandManager, GameServerCommandManager>();


            Uri commandUri = new Uri($"queue:{ServiceName}");
            EndpointConvention.Map<IHandleIncomingMessageCommand>(commandUri);

            services.Configure<RabbitMqOptions>(_configuration.GetSection("RabbitMq"));
            services.AddHiveMassTransit(busConfigure =>
            {
                busConfigure.AddConsumerWithRetry<HandleIncomingMessageCommandHandler>(RetryPolicies.EveryTenSecondsForAMinute);
            },
                (context, configure) => configure.ReceiveEndpoint(ServiceName, ep => ep.ConfigureConsumers(context)));
        }
    }
}