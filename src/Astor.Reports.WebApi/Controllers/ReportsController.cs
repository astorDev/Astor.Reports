using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Report = Astor.Reports.Protocol.Models.Report;
using ReportsCollection = Astor.Reports.Domain.ReportsCollection;

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
        
        [HttpGet]
        public async Task<Astor.Reports.Protocol.Models.ReportsCollection> GetAsync([FromQuery]ReportsQuery query)
        {
            var reports = await this.ReportsCollection.Store.GetAsync(query);

            return await this.mapAsync(reports);
        }
        
        [HttpPost()]
        public async Task<Report> CreateAsync([FromBody] ReportCandidate candidate)
        {
            var report = await Astor.Reports.Domain.Report.CreateAsync(candidate, this.ReportsCollection.Store);
            return await mapAsync(report);
        }

        private async Task<Astor.Reports.Protocol.Models.ReportsCollection> mapAsync(IEnumerable<Astor.Reports.Domain.Report> reports)
        {
            var resultList = new List<Report>();

            foreach (var domainModel in reports)
            {
                resultList.Add(await this.mapAsync(domainModel));
            }

            return new Astor.Reports.Protocol.Models.ReportsCollection
            {
                Count = resultList.Count,
                Reports = resultList.ToArray()
            };
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