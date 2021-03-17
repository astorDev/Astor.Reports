using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using Astor.Reports.Data.Models;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using SpeciVacation;
using Event = Astor.Reports.Data.Models.Event;
using Export = Astor.Reports.Protocol.Models.Export;
using Report = Astor.Reports.Data.Models.Report;
using ReportChanges = Astor.Reports.Domain.ReportChanges;

namespace Astor.Reports.Data
{
    public class ReportsStore : IReportsStore
    {
        public IMongoCollection<Models.Report> Collection { get; }

        public ReportsStore(IMongoCollection<Models.Report> collection)
        {
            this.Collection = collection;
        }

        public async Task<Domain.Report> SaveAsync(ReportChanges changes)
        {
            var updateDefinition = new UpdateDefinitionBuilder<Report>()
                .Combine(getUpdates(changes));

            await this.Collection.UpdateOneAsync(r => r.Id == changes.ReportId, updateDefinition);
            return await this.SearchAsync(changes.ReportId);
        }

        private IEnumerable<UpdateDefinition<Report>> getUpdates(ReportChanges changes)
        {
            if (changes.Status != null)
            {
                yield return new UpdateDefinitionBuilder<Report>().Set(m => m.Status, changes.Status.Value);
            }

            if (changes.Events != null)
            {
                foreach (var @event in changes.Events)
                {
                    yield return new UpdateDefinitionBuilder<Report>().AddToSet(m => m.Events, new Models.Event
                    {
                        Time = @event.Time,
                        Type = @event.Type
                    });
                }
            }
        }

        public async Task<Domain.Report> SearchAsync(string id)
        {
            var data = await this.Collection.Find(r => r.Id == id).FirstOrDefaultAsync();
            return map(data);
        }

        public async Task<Domain.Report> SearchAsync(ReportsFilter filter)
        {
            var report = await this.finder(filter).FirstOrDefaultAsync();
            return map(report);
        }

        public async Task<IEnumerable<Domain.Report>> GetAsync(ReportsQuery query)
        {
            var data = await this.Collection.Find(query.ToSpecification().ToExpression()).ToListAsync();
            return data.Select(map);
        }

        public async Task<IEnumerable<Domain.Report>> GetAsync(ReportsFilter filter)
        {
            var data = await this.finder(filter).ToListAsync();
            return data.Select(map);
        }

        public IFindFluent<Report, Report> finder(ReportsFilter filter)
        {
            var spec = Specification<Data.Models.Report>.All;

            FilterDefinition<Report> mongoFilter = spec.ToExpression();
            
            if (filter.AnyEvent != null)
            {
                mongoFilter &= new FilterDefinitionBuilder<Report>()
                    .ElemMatch(r => r.Events, filter.AnyEvent.ToSpecification().ToExpression());;
            }

            return this.Collection.Find(mongoFilter);
        }

        public async Task<Models.Report> AddAsync(string id)
        {
            var report = new Models.Report
            {
                Id = id
            };

            await this.Collection.InsertOneAsync(report);
            return report;
        }

        public async Task<Export> AddExportAsync(string reportId, Export export)
        {
            var filterString = ((object)export.Conditions.Filter)?.ToString();
            var sortString = ((object) export.Conditions.Sort)?.ToString();

            filterString = filterString.Replace("$", "*dollar*");
            
            var created = new Models.Export
            {
                Buckets = export.Buckets,
                Id = new Models.ExportConditions
                {
                    Filter = filterString == null ? null : BsonDocument.Parse(filterString),
                    Sort = sortString == null ? null : BsonDocument.Parse(sortString)
                },
                ElementsCount = export.ElementsCount
            };

            var update = Builders<Report>.Update.AddToSet(r => r.Exports, created);
            await this.Collection.UpdateOneAsync(r => r.Id == reportId, update);

            return map(created, reportId);
        }

        public async Task<Export> SearchExportAsync(string reportId, string exportId)
        {
            var report = await this.Collection.Find(r => r.Id == reportId).FirstOrDefaultAsync();

            var data = report?.Exports.SingleOrDefault(e => e.Id.ToString() == exportId);
            return data == null ? null : map(data, reportId);
        }

        private static Export map(Models.Export data, string reportId)
        {
            return new Export
            {
                Id = HttpUtility.UrlEncode(data.Id.ToString()),
                ReportId = reportId,
                ElementsCount = data.ElementsCount,
                Buckets = data.Buckets,
                Conditions = new Protocol.Models.ExportConditions
                {
                    Filter = data.Id.FilterDotNetObject,
                    Sort = data.Id.SortDotNetObject
                }
            };
        }

        public async Task<Domain.Report> SaveAsync(ReportCreationChanges creationChanges)
        {
            var reportData = new Models.Report
            {
                Id = creationChanges.ReportToAdd.Id,
                Type = creationChanges.ReportToAdd.Type,
                EstimatedRowsCount = creationChanges.ReportToAdd.EstimatedRowsCount,
                Status = creationChanges.ReportToAdd.Status,
                Events = creationChanges.Events.Select(e => new Models.Event
                {
                    Type = e.Type,
                    Time = e.Time,
                }).ToArray()
            };

            await this.Collection.InsertOneAsync(reportData);
            return map(reportData);
        }

        private static Domain.Report map(Report data)
        {
            return new Domain.Report
            {
                Id = data.Id,
                Type = data.Type,
                EstimatedRowsCount = data.EstimatedRowsCount,
                Status = data.Status,
                Events = data.Events.Select(e => new Domain.Event
                {
                    Id = e.Id.ToString(),
                    Processed = e.Processed,
                    Time = e.Time,
                    Type = e.Type
                }).ToArray()
            };
        }
    }
}