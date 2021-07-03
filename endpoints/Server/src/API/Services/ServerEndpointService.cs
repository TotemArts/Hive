using System;
using System.Threading.Tasks;
using Hive.Endpoints.Server.API.Commands;
using Hive.Endpoints.Server.Contracts.Commands;
using MassTransit;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Hive.Endpoints.Server.API.Services
{
    public class ServerEndpointService : ConnectionHandler
    {
        private readonly ILogger<ServerEndpointService> _logger;
        private readonly IBus _bus;

        public ServerEndpointService(ILogger<ServerEndpointService> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            // todo: go from a connection to multiple messages
            var incomingMessage = "Hive";

            await _bus.Send<IHandleIncomingMessageCommand>(new HandleIncomingMessageCommand(incomingMessage));

            throw new NotImplementedException();
        }
    }
}
