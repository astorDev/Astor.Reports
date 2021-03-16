using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Astor.Reports.Protocol.Models;

namespace PickPoint.Reports.WebApi.Helpers
{
    public class ReportsMapper
    {
        public RowsStoresFactory RowsStoresFactory { get; }

        public ReportsMapper(RowsStoresFactory rowsStoresFactory)
        {
            this.RowsStoresFactory = rowsStoresFactory;
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