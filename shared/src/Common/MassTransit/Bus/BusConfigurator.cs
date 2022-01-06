using System;
using System.Collections.Generic;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Shared.Common.MassTransit.Bus
{
    internal class BusConfigurator : IBusConfigurator
    {
        private readonly IServiceCollectionBusConfigurator _massTransitBusConfigurator;
        private readonly IServiceCollection _services;
        private readonly List<Action<IReceiveEndpointConfigurator>> _exceptionHandlers;

        public BusConfigurator(IServiceCollectionBusConfigurator massTransitBusConfigurator, IServiceCollection services)
        {
            _massTransitBusConfigurator = massTransitBusConfigurator;
            _services = services;
            _exceptionHandlers = new List<Action<IReceiveEndpointConfigurator>>();
        }

        public void HandleExceptions(IReceiveEndpointConfigurator configurator)
        {
            foreach (var handler in _exceptionHandlers)
            {
                handler.Invoke(configurator);
            }
        }

        public bool HasConsumers { get; private set; } = false;

        public IBusConfigurator RegisterCommandHandler<TCommandHandler>() where TCommandHandler : class, IConsumer
        {
            _massTransitBusConfigurator.AddConsumer<TCommandHandler>();
            HasConsumers = true;
            return this;
        }

        public IBusConfigurator RegisterCommandHandlersInSameNamespaceAs<TNameSpace>()
        {
            _massTransitBusConfigurator.AddConsumersFromNamespaceContaining<TNameSpace>();
            HasConsumers = true;
            return this;
        }

        public IBusConfigurator RegisterEventHandler<TEventHandler>() where TEventHandler : class, IConsumer
        {
            _massTransitBusConfigurator.AddConsumer<TEventHandler>();
            HasConsumers = true;
            return this;
        }

        public IBusConfigurator RegisterEventHandlersInSameNamespaceAs<TNameSpace>()
        {
            _massTransitBusConfigurator.AddConsumersFromNamespaceContaining<TNameSpace>();
            HasConsumers = true;
            return this;
        }

        public IBusConfigurator SendCommandsTo(string workerServiceName, Action<ICommandRegistrar> registrar)
        {
            var commandRegistrar = new CommandRegistrar(new Uri($"queue:{workerServiceName}"));
            registrar.Invoke(commandRegistrar);
            return this;
        }

        public IBusConfigurator UseRetry(Action<IRetryConfigurator> configuration)
        {
            _exceptionHandlers.Add(config =>
            {
                config.UseMessageRetry(configuration);
            });
            return this;
        }
    }
}