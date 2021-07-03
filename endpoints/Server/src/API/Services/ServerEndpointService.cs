using System;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading;
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
            var ipEndpoint = ((IPEndPoint) connection.RemoteEndPoint!);
            Console.WriteLine($"I am connected to {ipEndpoint.Address} on port number {ipEndpoint.Port}");
            var messageInput = connection.Transport.Input;

            var readMessages = GetMessagesAsync(messageInput, ipEndpoint, connection.ConnectionClosed);
            var writeMessages = Task.CompletedTask;

            await Task.WhenAll(readMessages, writeMessages);
            Console.WriteLine($"Connected was closed: {ipEndpoint.Address} on port number {ipEndpoint.Port}");
        }

        public async Task GetMessagesAsync(PipeReader messageInput, IPEndPoint ipEndpoint, CancellationToken connectionClosed)
        {
            ReadResult messageResult;
            while (!(messageResult = await messageInput.ReadAsync(connectionClosed)).IsCanceled)
            {
                var incomingMessage = Encoding.UTF8.GetString(messageResult.Buffer);
                messageInput.AdvanceTo(messageResult.Buffer.End);
                await _bus.Send<IHandleIncomingMessageCommand>(new HandleIncomingMessageCommand(incomingMessage, ipEndpoint));
            }
        }
    }
}
