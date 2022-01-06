using Hive.Endpoints.Server.Contracts.Commands;

namespace Hive.Endpoints.Server.Worker.Commands
{
    public class SendToServerCommand : ISendToServerCommand
    {
        public SendToServerCommand(string address, int port, string message)
        {
            Address = address;
            Port = port;
            Message = message;
        }

        public string Message { get; }
        public string Address { get; }
        public int Port { get; }
    }
}
