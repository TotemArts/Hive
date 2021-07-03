using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Hive.Shared.Common;
using Hive.Shared.Common.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Hive.Endpoints.Server.API
{
    public class Startup
    {
        private static readonly string ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new Exception("ServiceName could not be derived from Assembly");
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Disable https requirement since we let traefik handle that.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Uri commandUri = new Uri("queue:Hive.Endpoints.Server.Worker");
            // EndpointConvention.Map<IHandleIncomingMessageCommand>(commandUri);

            // Commands that we will be sending ourself
            Uri selfCommandUri = new Uri($"queue:{ServiceName}");
            // EndpointConvention.Map<IExampleCommand>(selfCommandUri);


            services.AddCors(o => o.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddGrpc();


            services.Configure<RabbitMqOptions>(_configuration.GetSection("RabbitMq"));
            services.AddHiveMassTransit(busConfigure =>
            {
                // busConfigure.AddConsumerWithRetry<ExampleEventHandler>(RetryPolicies.EveryTenSecondsForAMinute);
                // busConfigure.AddConsumerWithRetry<ExampleCommandHandler>(RetryPolicies.ExponentialForHundredTimes);
            },
                (context, configure) => configure.ReceiveEndpoint(ServiceName, ep => ep.ConfigureConsumers(context)));

            // Add any gRPC clients here
            // services.AddGrpcClient<ExampleGrpcService.ExampleGrpcServiceClient>(o => o.Address = new Uri(Environment.GetEnvironmentVariable("Example_URL") ?? "http://host.docker.internal:example_port"));

            // Add any dependencies that need to be injected
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<ExampleService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
