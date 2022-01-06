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
            Address = ipEndpoint.Address.ToString();
            if (ipEndpoint.Address.IsIPv4MappedToIPv6)
                Address = Address.Substring(7);
            Port = ipEndpoint.Port;
        }

        public string Message { get; }
        public string Address { get; }
        public int Port { get; }
    }
}