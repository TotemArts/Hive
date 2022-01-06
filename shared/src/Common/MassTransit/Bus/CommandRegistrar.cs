using System;
using MassTransit;

namespace Hive.Shared.Common.MassTransit.Bus
{
    internal class CommandRegistrar : ICommandRegistrar
    {
        private readonly Uri _commandEndpoint;

        public CommandRegistrar(Uri commandEndpoint)
        {
            _commandEndpoint = commandEndpoint;
        }

        public ICommandRegistrar Map<T>() where T : class
        {
            EndpointConvention.Map<T>(_commandEndpoint);
            return this;
        }
    }
}
