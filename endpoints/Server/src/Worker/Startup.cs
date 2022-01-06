using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Endpoints.Server.Worker.CommandHandlers;
using Hive.Endpoints.Server.Worker.CommandManagers;
using Hive.Shared.Common;

namespace Hive.Endpoints.Server.Worker
{
    public class StartUp
    {
        private readonly IConfiguration _configuration;

        public StartUp(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IGameServerCommandManager, GameServerCommandManager>();

            services.AddMassTransit(_configuration, configurator =>
            {
                configurator.RegisterCommandHandler<HandleIncomingMessageCommandHandler>();

                configurator.SendCommandsTo(DomainInformation.ServiceName, registrar =>
                {
                    registrar.Map<IHandleIncomingMessageCommand>();
                });
            });
        }
    }
}