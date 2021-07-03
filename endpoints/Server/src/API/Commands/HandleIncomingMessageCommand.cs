using Hive.Endpoints.Server.Contracts.Commands;
using System.Net;

namespace Hive.Endpoints.Server.API.Commands
{
    public class HandleIncomingMessageCommand : IHandleIncomingMessageCommand
    {
        public HandleIncomingMessageCommand(string message)
        {
            Message = message;
        }

        public HandleIncomingMessageCommand(string message, IPEndPoint ipEndpoint) : this(message)
        {
            IpEndpoint = ipEndpoint;
        }

        public string Message { get; }
        public IPEndPoint IpEndpoint { get; }
    }
}