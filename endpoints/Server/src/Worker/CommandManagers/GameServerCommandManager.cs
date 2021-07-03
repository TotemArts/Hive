using System;
using System.Threading.Tasks;
using Hive.Endpoints.Server.Contracts.Commands;

namespace Hive.Endpoints.Server.Worker.CommandManagers
{
    public class GameServerCommandManager : IGameServerCommandManager
    {
        public Task HandleIncomingMessageAsync(IHandleIncomingMessageCommand command)
        {
            // todo: Actually process the message to the multiple different possibilities

            /*
// Packet Commands
`define AUTH		"a"		// c->s Auth Request, s->c Auth Confirmed
`define COMMAND		"c"		// c->s ConsoleCommand, s->c ConsoleCommand Completed/End of Response
`define RESPONSE    "r"     // s->c ConsoleCommand Response
`define	SUB			"s"		// c->s Subscribe to Logs, s->c Confirmed
`define UNSUB		"u"		// c->s Unsubscribe from Logs, s->c Confirmed
`define VERSION	    "v"		// s->c RxRcon Protocol Version and RenX Version
`define LOGMSG		"l"		// s->c RxLog message
`define ERROR		"e"		// s->c Error with Message
             */

            throw new NotImplementedException();
        }
    }
}
