using System;
using GreenPipes.Configurators;
using MassTransit;

namespace Hive.Shared.Common.MassTransit.Bus
{
    public interface IBusConfigurator
    {
        public IBusConfigurator RegisterCommandHandler<TCommandHandler>() where TCommandHandler : class, IConsumer;

        public IBusConfigurator RegisterCommandHandlersInSameNamespaceAs<TNameSpace>();

        public IBusConfigurator RegisterEventHandler<TEventHandler>() where TEventHandler : class, IConsumer;

        public IBusConfigurator RegisterEventHandlersInSameNamespaceAs<TNameSpace>();

        public IBusConfigurator SendCommandsTo(string workerServiceName, Action<ICommandRegistrar> registrar);

        public IBusConfigurator UseRetry(Action<IRetryConfigurator> configuration);
    }
}