using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route(Uris.About)]
    public class AboutController
    {
        public IWebHostEnvironment Environment { get; }

        public AboutController(IWebHostEnvironment environment)
        {
            this.Environment = environment;
        }

        [HttpGet]
        public About Get()
        {
            return new About
            {
                Description = "Astor.Reports - API отчётов",
                Environment = this.Environment.EnvironmentName,
                Version = this.GetType().Assembly.GetName().Version.ToString()
            };
        }
    }
}