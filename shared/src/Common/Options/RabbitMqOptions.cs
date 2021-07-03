using System;

namespace Hive.Shared.Common.Options
{
    // Since configuration is written to this class it REQUIRES the setters, if these are not included the services will FAIL to connect to the bus!
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
    public class RabbitMqOptions
    {
        public TimeSpan Heartbeat { get; set; } = TimeSpan.FromSeconds(10);
        public string Password { get; set; } = "guest";
        public string Url { get; set; } = "rabbitmq://rabbitmq";
        public string Username { get; set; } = "guest";
    }
}