using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;

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
                    logging.AddFilter("", LogLevel.Error);
                    logging.AddFilter("Astor.Reports", LogLevel.Information);
                    
                    logging.AddConsole(o => o.FormatterName = "Mini")
                        .AddConsoleFormatter<MiniConsoleLogFormatter, ConsoleFormatterOptions>();
                    //logging.AddConsole(o => o.FormatterName = )
                });

            var host = builder.UseConsoleLifetime().Build();
            
            await host.RunAsync();
        }
        
        public class MiniConsoleLogFormatter : ConsoleFormatter
        {
            public MiniConsoleLogFormatter() : base("Mini")
            {
            }

            public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
            {
                if (logEntry.State is IEnumerable<KeyValuePair<string, object>> properties)
                {
                    var (key, value) = properties.FirstOrDefault(p => p.Key == "{OriginalFormat}");
                    if (value != null)
                    {
                        textWriter.WriteLine(value);
                    }
                }
            }
        }
    }
}