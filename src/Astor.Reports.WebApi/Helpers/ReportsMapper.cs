using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Astor.Reports.Protocol.Models;
using Report = Astor.Reports.Protocol.Models.Report;
using ReportsCollection = Astor.Reports.Protocol.Models.ReportsCollection;

namespace PickPoint.Reports.WebApi.Helpers
{
    public class Mapper
    {
        public RowsStoresFactory RowsStoresFactory { get; }

        public Mapper(RowsStoresFactory rowsStoresFactory)
        {
            this.RowsStoresFactory = rowsStoresFactory;
        }

        public async IAsyncEnumerable<ReportEvent> MapAsync(IEnumerable<Astor.Reports.Domain.Report> reports,
            EventsFilter eventsFilter)
        {
            foreach (var report in reports)
            {
                foreach (var row in await this.MapAsync(report, eventsFilter).ToArrayAsync())
                {
                    yield return row;
                }
            }
        }
        
        public async IAsyncEnumerable<ReportEvent> MapAsync(Astor.Reports.Domain.Report report, EventsFilter eventsFilter)
        {
            foreach (var @event in report.GetEvents(eventsFilter))
            {
                yield return new ReportEvent
                {
                    Id = @event.Id,
                    Type = @event.Type,
                    Processed = @event.Processed,
                    Body = new ReportEventBody
                    {
                        Time = @event.Time,
                        Report = await this.MapAsync(report)
                    }
                };
            }
        }
        
        public async Task<Report> MapAsync(Astor.Reports.Domain.Report report)
        {
            return new()
            {
                Id = report.Id,
                Type = report.Type,
                Status = report.Status,
                CreationTime = report.CreationTime,
                LastModificationTime = report.LastModificationTime,
                EstimatedRowsCount = report.EstimatedRowsCount,
                RowsCount = await report.CountRowsAsync("{}", this.RowsStoresFactory)
            };
        }

        public async Task<ReportsCollection> MapAsync(IEnumerable<Astor.Reports.Domain.Report> reports)
        {
            var resultList = new List<Report>();

            foreach (var domainModel in reports)
            {
                resultList.Add(await this.MapAsync(domainModel));
            }

            return new ReportsCollection
            {
                Count = resultList.Count,
                Reports = resultList.ToArray()
            };
        }
    }
}