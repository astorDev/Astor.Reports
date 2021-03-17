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
using ReportsCollection = Astor.Reports.Domain.ReportsCollection;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route(Uris.Events)]
    public class EventsController
    {
        public ReportsCollection Reports { get; }
        public Mapper Mapper { get; }
        public IEventsStore EventsStore { get; }

        public EventsController(ReportsCollection reports, Mapper mapper, IEventsStore eventsStore)
        {
            this.Reports = reports;
            this.Mapper = mapper;
            this.EventsStore = eventsStore;
        }
        
        [HttpGet]
        public async Task<ReportEventsCollection> GetReportEventsAsync([FromQuery] EventsQuery query)
        {
            var eventsFilter = new EventsFilter
            {
                Processed = query.Processed
            };
            
            var reports = await this.Reports.Store.GetAsync(new ReportsFilter
            {
                AnyEvent = eventsFilter
            });

            var resultArray = await this.Mapper.MapAsync(reports, eventsFilter).ToArrayAsync();
            return new ReportEventsCollection
            {
                Count = resultArray.Length,
                Events = resultArray
            };
        }

        [HttpPatch("{id}")]
        public async Task<ReportEvent> UpdateEventAsync(string id, [FromBody] ReportEventChanges changes)
        {
            var eventsFilter = new EventsFilter
            {
                Ids = new[] {id}
            };
            
            var report = await this.Reports.GetAsync(new ReportsFilter
            {
                AnyEvent = eventsFilter
            });

            report = await report.UpdateEvent(id, new ReportEventChanges
            {
                Processed = changes.Processed
            }, this.EventsStore, this.Reports.Store);

            return await this.Mapper.MapAsync(report, eventsFilter).SingleAsync();
        }
    }
}