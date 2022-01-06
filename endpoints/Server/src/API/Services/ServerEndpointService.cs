using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hive.Endpoints.Server.API.Commands;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Shared.Common.Extensions;
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
            _logger.LogInformation($"I am connected to {ipEndpoint.Address} on port number {ipEndpoint.Port}");
            var messageInput = connection.Transport.Input;
            var messageOutput = connection.Transport.Output;

            var password = Environment.GetEnvironmentVariable("GLOBAL_SERVER_PASSWORD");
            await connection.Transport.Output.WriteAsync(Encoding.UTF8.GetBytes($"a{password}"));

            var readMessages = GetMessagesAsync(messageInput, ipEndpoint, connection.ConnectionClosed);
            var writeMessages = SendMessagesAsync(messageOutput, ipEndpoint, connection.ConnectionClosed);

            await Task.WhenAny(readMessages, writeMessages);

            _logger.LogInformation($"Connected was closed: {ipEndpoint.Address} on port number {ipEndpoint.Port}");
        }

        private async Task GetMessagesAsync(PipeReader messageInput, IPEndPoint ipEndpoint, CancellationToken connectionClosed)
        {
            ReadResult messageResult;
            while (!(messageResult = await messageInput.ReadAsync(connectionClosed)).IsCanceled)
            {
                var incomingMessage = Encoding.UTF8.GetString(messageResult.Buffer);
                _logger.LogInformation("Got Message: {IncomingMessage}", Utf8Decoder.ToLiteral(messageResult.Buffer.ToArray()));
                messageInput.AdvanceTo(messageResult.Buffer.End);
                try
                {
                    await _bus.Send<IHandleIncomingMessageCommand>(new HandleIncomingMessageCommand(incomingMessage, ipEndpoint));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to send message on bus...");
                }
            }
        }

        private async Task SendMessagesAsync(PipeWriter messageOutput, IPEndPoint ipEndpoint, CancellationToken connectionClosed)
        {
            while (!connectionClosed.IsCancellationRequested)
            {
                await Task.Delay(250, connectionClosed);
            }
        }
    }
}
