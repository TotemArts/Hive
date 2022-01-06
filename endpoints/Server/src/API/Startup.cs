using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Hive.Endpoints.Server.API.CommandHandlers;
using Hive.Endpoints.Server.API.Services;
using Hive.Endpoints.Server.Contracts.Commands;
using Hive.Shared.Common;
using Microsoft.Extensions.Configuration;

namespace Hive.Endpoints.Server.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Disable https requirement since we let traefik handle that.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // MassTransit
            services.AddMassTransit(_configuration, configurator =>
            {
                configurator.RegisterCommandHandler<CommandHandler>();

                configurator.SendCommandsTo(DomainInformation.WorkerServiceName, registrar =>
                {
                    registrar.Map<IHandleIncomingMessageCommand>();
                });
            }, true);

            // Services
            services.AddSingleton<IServerManager, ServerManager>();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
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
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
