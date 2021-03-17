using System;
using Astor.RabbitMq;
using Astor.Reports.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Reports.EventsPublisher
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var rabbitConnectionString = this.Configuration["ConnectionStrings:Rabbit"];
            services.AddRabbit(rabbitConnectionString);

            services.AddHttpClient<ReportsClient>(cl =>
            {
                cl.BaseAddress = new Uri(this.Configuration["ConnectionStrings:Webapi"]);
            });
        }
    }
}