using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hive.Endpoints.Server.API.Services
{
    public class ServerManager : IServerManager
    {
        public ServerManager()
        {
            ManagedServers = new Dictionary<string, Queue<string>>();
        }

        public Dictionary<string, Queue<string>> ManagedServers { get; set; }

        public Task RegisterServerAsync(string Address, int Port)
        {
            ManagedServers.Add($"{Address}:{Port}", new Queue<string>());
            return Task.CompletedTask;
        }

        public Task UnRegisterServerAsync(string Address, int Port)
        {
            ManagedServers.Remove($"{Address}:{Port}");
            return Task.CompletedTask;
        }

        public Task AddMessageToSendQueueAsync(string Address, int Port, string Message)
        {
            if (ManagedServers.TryGetValue($"{Address}:{Port}", out var value))
            {
                value.Enqueue(Message);
            }
            return Task.CompletedTask;
        }

        public Task<string?> RetrieveSendQueueAsync(string Address, int Port)
        {
            if (ManagedServers.TryGetValue($"{Address}:{Port}", out var value))
            {
                if (value.TryDequeue(out var result))
                {
                    return Task.FromResult(result);
                }
            }
            return Task.FromResult(string.Empty);
        }
    }
}
