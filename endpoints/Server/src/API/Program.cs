using Hive.Endpoints.Server.API.Services;
using Hive.Shared.Common;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hive.Endpoints.Server.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HiveLog.UseSerilog(() => CreateHostBuilder(args).Build().Run());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => {
                        options.ListenAnyIP(21337, builder => {
                            builder.UseConnectionHandler<ServerEndpointService>();
                         });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
