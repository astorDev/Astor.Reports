using System.Threading;
using System.Threading.Tasks;
using Astor.RabbitMq;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Astor.Reports.EventsPublisher
{
    public class EventsPublisher : IHostedService
    {
        public ILogger<EventsPublisher> Logger { get; }
        public IModel RabbitChannel { get; }
        public IConfiguration Configuration { get; }
        public ReportsClient Client { get; }
        public string ExchangePrefix { get; }

        public EventsPublisher(ILogger<EventsPublisher> logger, IModel rabbitChannel, IConfiguration configuration, ReportsClient client)
        {
            this.Logger = logger;
            this.RabbitChannel = rabbitChannel;
            this.Configuration = configuration;
            this.Client = client;
            this.ExchangePrefix = configuration["ExchangePrefix"];
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("declaring exchanges");
            
            foreach (var eventName in EventNames.GetAll())
            {
                this.RabbitChannel.ExchangeDeclare(this.exchangeName(eventName), "fanout", true);
            }
            
            this.Logger.LogInformation("started");

            while (!cancellationToken.IsCancellationRequested)
            {
                var unprocessed = await this.Client.GetReportEventsAsync(new EventsQuery
                {
                    Processed = false
                });
            
                this.Logger.LogInformation($"processing {unprocessed.Count} events");

                foreach (var @event in unprocessed.Events)
                {
                    this.RabbitChannel.PublishJson(this.exchangeName(@event.Type), @event.Body);
                    await this.Client.UpdateReportEventAsync(@event.Id, new ReportEventChanges
                    {
                        Processed = true
                    });
                }

                this.Logger.LogInformation("processed");
                
                if (unprocessed.Count == 0)
                {
                    await Task.Delay(500, cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string exchangeName(string eventType) => $"{this.ExchangePrefix}.{eventType}";
    }
}