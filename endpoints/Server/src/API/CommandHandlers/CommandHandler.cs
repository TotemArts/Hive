using System.Threading.Tasks;
using Hive.Endpoints.Server.API.Services;
using Hive.Endpoints.Server.Contracts.Commands;
using MassTransit;

namespace Hive.Endpoints.Server.API.CommandHandlers
{
    public class CommandHandler : 
        IConsumer<ISendToServerCommand>
    {
        private readonly IServerManager _manager;

        public CommandHandler(IServerManager manager)
        {
            _manager = manager;
        }

        public Task Consume(ConsumeContext<ISendToServerCommand> context)
        {
            var command = context.Message;
            return _manager.AddMessageToSendQueueAsync(command.Address, command.Port, command.Message);
        }
    }
}
