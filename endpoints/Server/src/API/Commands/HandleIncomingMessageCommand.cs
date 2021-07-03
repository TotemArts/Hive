using Hive.Endpoints.Server.Contracts.Commands;

namespace Hive.Endpoints.Server.API.Commands
{
    public class HandleIncomingMessageCommand : IHandleIncomingMessageCommand
    {
        public HandleIncomingMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}