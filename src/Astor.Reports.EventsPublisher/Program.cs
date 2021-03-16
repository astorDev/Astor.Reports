using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Astor.Reports.EventsPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostedService, EventsPublisher>();
                    var startup = new Startup(hostContext.Configuration);
                    startup.ConfigureServices(services);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();
                });

            var host = builder.UseConsoleLifetime().Build();
            
            await host.RunAsync();
        }
    }
}