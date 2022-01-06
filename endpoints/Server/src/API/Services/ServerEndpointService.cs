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
        private readonly IServerManager _manager;

        public ServerEndpointService(ILogger<ServerEndpointService> logger, IBus bus, IServerManager manager)
        {
            _logger = logger;
            _bus = bus;
            _manager = manager;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var ipEndpoint = ((IPEndPoint) connection.RemoteEndPoint!);
            var address = ipEndpoint.Address.ToString();
            var port = ipEndpoint.Port;
            if (ipEndpoint.Address.IsIPv4MappedToIPv6)
                address = address.Substring(7);

            _logger.LogInformation($"I am connected to {address} on port number {port}");
            await _manager.RegisterServerAsync(address, port);

            var messageInput = connection.Transport.Input;
            var messageOutput = connection.Transport.Output;

            var password = Environment.GetEnvironmentVariable("GLOBAL_SERVER_PASSWORD");
            await connection.Transport.Output.WriteAsync(Encoding.UTF8.GetBytes($"a{password}\n"));

            var readMessages = GetMessagesAsync(messageInput, ipEndpoint, connection.ConnectionClosed);
            var writeMessages = SendMessagesAsync(messageOutput, ipEndpoint, connection.ConnectionClosed);

            await Task.WhenAny(readMessages, writeMessages);

            await _manager.UnRegisterServerAsync(address, port);
            _logger.LogInformation($"Connected was closed: {address} on port number {port}");
        }

        private async Task GetMessagesAsync(PipeReader messageInput, IPEndPoint ipEndpoint, CancellationToken connectionClosed)
        {
            ReadResult messageResult;
            while (!(messageResult = await messageInput.ReadAsync(connectionClosed)).IsCanceled)
            {
                var incomingMessages = Encoding.UTF8.GetString(messageResult.Buffer);
                _logger.LogInformation("Got Message: {IncomingMessage}", Utf8Decoder.ToLiteral(incomingMessages));
                foreach(var incomingMessage in incomingMessages.Split('\n')) {
                    try
                    {
                        await _bus.Send<IHandleIncomingMessageCommand>(new HandleIncomingMessageCommand(incomingMessage, ipEndpoint));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to send message on bus...");
                    }
                }
                messageInput.AdvanceTo(messageResult.Buffer.End);
            }
        }

        private async Task SendMessagesAsync(PipeWriter messageOutput, IPEndPoint ipEndpoint, CancellationToken connectionClosed)
        {
            var address = ipEndpoint.Address.ToString();
            var port = ipEndpoint.Port;
            if (ipEndpoint.Address.IsIPv4MappedToIPv6)
                address = address.Substring(7);

            while (!connectionClosed.IsCancellationRequested)
            {
                var message = await _manager.RetrieveSendQueueAsync(address, port);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    _logger.LogInformation("Sending Message: {OutgoingMessage}", Utf8Decoder.ToLiteral(message));
                    await messageOutput.WriteAsync(Encoding.UTF8.GetBytes(message), connectionClosed);
                }

                await Task.Delay(50, connectionClosed);
            }
        }
    }
}
