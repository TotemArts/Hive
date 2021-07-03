using System.Net;

namespace Hive.Endpoints.Server.Contracts.Specifications
{
    public interface IRawMessage
    {
        string Message { get; }
        IPEndPoint IpEndpoint { get; }
    }
}
