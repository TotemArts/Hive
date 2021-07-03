using System.Threading.Tasks;
using Hive.Endpoints.Server.Contracts.Commands;

namespace Hive.Endpoints.Server.Worker.CommandManagers
{
    public interface IGameServerCommandManager
    {
        Task HandleIncomingMessageAsync(IHandleIncomingMessageCommand command);
    }
}