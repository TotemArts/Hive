using System.Threading.Tasks;

namespace Hive.Endpoints.Server.API.Services
{
    public interface IServerManager
    {
        Task RegisterServerAsync(string Address, int Port);
        Task UnRegisterServerAsync(string Address, int Port);
        Task AddMessageToSendQueueAsync(string Address, int Port, string Message);
        Task<string?> RetrieveSendQueueAsync(string Address, int Port);
    }
}
