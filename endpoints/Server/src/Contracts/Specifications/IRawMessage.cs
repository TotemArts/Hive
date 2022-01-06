namespace Hive.Endpoints.Server.Contracts.Specifications
{
    public interface IRawMessage
    {
        string Message { get; }
        public string Address { get; }
        public int Port { get; }
    }
}
