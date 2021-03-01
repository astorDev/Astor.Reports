using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Report = Astor.Reports.Protocol.Models.Report;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route("/")]
    public class ReportsController
    {
        public ReportsCollection ReportsCollection { get; }
        public RowsStoresFactory RowsStoresFactory { get; }

        public ReportsController(ReportsCollection reportsCollection, RowsStoresFactory rowsStoresFactory)
        {
            this.ReportsCollection = reportsCollection;
            this.RowsStoresFactory = rowsStoresFactory;
        }
        
        [HttpGet("{reportId}")]       
        public async Task<Report> GetAsync(string reportId)
        {
            var report = await this.ReportsCollection.GetAsync(reportId);

            return await this.mapAsync(report);
        }

        [HttpPost()]
        public async Task<Report> CreateAsync([FromBody] ReportCandidate candidate)
        {
            var report = await Astor.Reports.Domain.Report.CreateAsync(candidate, this.ReportsCollection.Store);
            return await mapAsync(report);
        }

        private async Task<Report> mapAsync(Astor.Reports.Domain.Report report)
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
    }
}