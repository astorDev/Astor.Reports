using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using Astor.Reports.Data.Models;
using Astor.Reports.Protocol.Models;
using Export = Astor.Reports.Protocol.Models.Export;
using Report = Astor.Reports.Data.Models.Report;

namespace Astor.Reports.Data
{
    public class ReportsStore
    {
        public IMongoCollection<Models.Report> Collection { get; }

        public ReportsStore(IMongoCollection<Models.Report> collection)
        {
            this.Collection = collection;
        }

        public async Task<Models.Report> SearchAsync(string id)
        {
            return await this.Collection.Find(r => r.Id == id).FirstOrDefaultAsync();
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
    }
}