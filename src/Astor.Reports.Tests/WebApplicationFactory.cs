using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Astor.Reports.Protocol;
using PickPoint.Reports.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Astor.Reports.Tests
{
    public class WebApplicationFactory : WebApplicationFactory<Startup>
    {
        private HttpClient httpClient;

        private void ensureHttpClientCreated()
        {
            if (this.httpClient == null)
            {
                this.httpClient = this.CreateClient();
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(new MongoClient("mongodb://localhost:27017"));

                
            });

        }

        public IServiceProvider ServiceProvider
        {
            get
            {
                this.ensureHttpClientCreated();
                return this.Server.Services;
            }
        }

        public ReportsClient Create()
        {
            this.ensureHttpClientCreated();
            return new TestClient(this.httpClient);
        }

        private class TestClient : ReportsClient
        {
            public TestClient(HttpClient httpClient) : base(httpClient)
            {
            }

            protected override async Task OnResponseReceivedAsync(HttpResponseMessage response)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
        }
    }
}