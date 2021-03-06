using Astor.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using Newtonsoft.Json.Converters;
using PickPoint.Reports.WebApi.Helpers;
using Report = Astor.Reports.Data.Models.Report;

namespace PickPoint.Reports.WebApi
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
            services.AddControllers()
                .AddNewtonsoftJson(json =>
                {
                    json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    json.SerializerSettings.ContractResolver = new ReportsJsonContractResolver();
                    json.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddSingleton(new MongoClient(this.Configuration.GetConnectionString("Mongo")));

            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<MongoClient>();
                return client.GetDatabase("reports");
            });

            services.AddSingleton(sp =>
            {
                var db = sp.GetRequiredService<IMongoDatabase>();
                return new RowsStoresFactory(db);
            });

            services.AddSingleton(sp =>
            {
                var db = sp.GetRequiredService<IMongoDatabase>();
                return db.GetCollection<Report>("reports");
            });

            services.AddSingleton<ReportsStore>();
            services.AddSingleton<IReportsStore, ReportsStore>();
            services.AddSingleton<ReportsCollection>();
            services.AddScoped<IEventsStore, EventsStore>();

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PickPoint.Reports",
                    Version = this.GetType().Assembly.GetName().Version.ToString()
                });
            });

            services.AddScoped<Mapper>();

            MongoConventions.Register(new IConvention[]
            {
                new CamelCaseElementNameConvention(), 
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true)
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestsLogging(l =>
            {
                l.IgnoredPathPatterns.Add("swagger");

                if (!env.IsDevelopment())
                {
                    l.IgnoredPathPatterns.Add("events");
                }
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PickPoint.Reports"); });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    var error = Error.Interpret(exceptionHandlerPathFeature.Error, true);

                    context.Response.StatusCode = (int) error.Code;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(error, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    }));
                });
            });


            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}