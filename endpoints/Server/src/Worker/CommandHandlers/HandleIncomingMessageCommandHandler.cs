using System.Threading.Tasks;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Endpoints.Server.Worker.CommandManagers;
using MassTransit;


namespace Hive.Endpoints.Server.Worker.CommandHandlers
{
    public class HandleIncomingMessageCommandHandler : IConsumer<IHandleIncomingMessageCommand>
    {
        private readonly IGameServerCommandManager _gameServerCommandManager;

        public HandleIncomingMessageCommandHandler(IGameServerCommandManager gameServerCommandManager)
        {
            _gameServerCommandManager = gameServerCommandManager;
        }

        public Task Consume(ConsumeContext<IHandleIncomingMessageCommand> context)
        {
            return _gameServerCommandManager.HandleIncomingMessageAsync(context.Message);
        }
    }
}
