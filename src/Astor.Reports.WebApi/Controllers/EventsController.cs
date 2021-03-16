using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Microsoft.AspNetCore.Mvc;
using PickPoint.Reports.WebApi.Helpers;
using Report = Astor.Reports.Protocol.Models.Report;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route(Uris.Events)]
    public class EventsController
    {
        public IReportsStore ReportsStore { get; }
        public ReportsMapper ReportsMapper { get; }

        public EventsController(IReportsStore reportsStore, ReportsMapper reportsMapper)
        {
            this.ReportsStore = reportsStore;
            this.ReportsMapper = reportsMapper;
        }
        
        [HttpGet]
        public async Task<ReportEventsCollection> GetReportEventsAsync([FromQuery] EventsQuery query)
        {
            if (query.Processed != false)
            {
                throw new NotImplementedException("Only unprocessed events filter is possible");
            }

            var reports = await this.ReportsStore.GetAsync(new ReportsFilter
            {
                AnyUnprocessedEvents = true
            });

            var resultArray = await this.GetUnprocessedEvents(reports).ToArrayAsync();
            return new ReportEventsCollection
            {
                Count = resultArray.Length,
                Events = resultArray
            };
        }

        private async IAsyncEnumerable<ReportEvent> GetUnprocessedEvents(IEnumerable<Astor.Reports.Domain.Report> reports)
        {
            foreach (var report in reports)
            {
                foreach (var @event in report.Events.Where(e => !e.Processed))
                {
                    yield return new ReportEvent
                    {
                        Id = @event.Id,
                        Type = @event.Type,
                        Processed = @event.Processed,
                        Body = new ReportEventBody
                        {
                            Time = @event.Time,
                            Report = await this.ReportsMapper.MapAsync(report)
                        }
                    };
                }
            }
        }
    }
}