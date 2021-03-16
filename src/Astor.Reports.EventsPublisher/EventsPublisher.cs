using System.Threading;
using System.Threading.Tasks;
using Astor.Reports.Protocol;
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
        
        public string ExchangeFamily { get; }

        public EventsPublisher(ILogger<EventsPublisher> logger, IModel rabbitChannel, IConfiguration configuration)
        {
            this.Logger = logger;
            this.RabbitChannel = rabbitChannel;
            this.Configuration = configuration;
            this.ExchangeFamily = configuration["ExchangeFamily"];
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("started");

            foreach (var eventName in EventNames.GetAll())
            {
                this.RabbitChannel.ExchangeDeclare(exchangeName(eventName), "fanout", true);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string exchangeName(string eventType) => $"{this.ExchangeFamily}.reports.{eventType}";
    }
}