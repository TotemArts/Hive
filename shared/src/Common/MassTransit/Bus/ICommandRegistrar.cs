namespace Hive.Shared.Common.MassTransit.Bus
{
    public interface ICommandRegistrar
    {
        ICommandRegistrar Map<T>() where T : class;
    }
}
