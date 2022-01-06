using System.Threading.Tasks;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Endpoints.Server.Worker.Commands;
using Hive.Shared.Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Hive.Endpoints.Server.Worker.CommandManagers
{
    public class GameServerCommandManager : IGameServerCommandManager
    {
        private readonly ILogger<GameServerCommandManager> _logger;
        private readonly IBus _bus;

        public GameServerCommandManager(ILogger<GameServerCommandManager> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task HandleIncomingMessageAsync(IHandleIncomingMessageCommand command)
        {
            // todo: Actually process the message to the multiple different possibilities

            /*
Packet Commands
c = me, s = renx_server
`define AUTH		"a"		// c->s Auth Request, s->c Auth Confirmed
`define COMMAND		"c"		// c->s ConsoleCommand, s->c ConsoleCommand Completed/End of Response

`define RESPONSE    "r"     // s->c ConsoleCommand Response

`define	SUB			"s"		// c->s Subscribe to Logs, s->c Confirmed
`define UNSUB		"u"		// c->s Unsubscribe from Logs, s->c Confirmed

`define VERSION	    "v"		// s->c RxRcon Protocol Version and RenX Version
`define LOGMSG		"l"		// s->c RxLog message
`define ERROR		"e"		// s->c Error with Message
             */
            if (command.Message.Length == 0)
            {
                return;
            }

            var debugMessage = Utf8Decoder.ToLiteral(Utf8Decoder.GetBytes(command.Message)[1..]);
            switch (command.Message[0])
            {
                case 'a':
                    _logger.LogInformation("Auth request: {message}", debugMessage);
                    break;
                case 'c':
                    _logger.LogInformation("COMMAND request: {message}", debugMessage);
                    break;
                case 'r':
                    _logger.LogInformation("RESPONSE request: {message}", debugMessage);
                    break;
                case 's':
                    _logger.LogInformation("Subscribe request: {message}", debugMessage);
                    break;
                case 'u':
                    _logger.LogInformation("Unsubscribe request: {message}", debugMessage);
                    break;
                case 'v':
                    _logger.LogInformation("Version request: {message}", debugMessage);
                    await _bus.Publish(new SendToServerCommand(command.Address, command.Port, "s\n"));
                    await _bus.Publish(new SendToServerCommand(command.Address, command.Port, "cserverinfo\n"));

                    break;
                case 'l':
                    _logger.LogInformation("RxLog request: {message}", debugMessage);

                    break;
                case 'e':
                    _logger.LogInformation("Error request: {message}", debugMessage);
                    break;
            };
            return;
        }
    }
}
