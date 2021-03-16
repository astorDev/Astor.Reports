using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using PickPoint.Reports.WebApi.Helpers;
using Report = Astor.Reports.Protocol.Models.Report;
using ReportChanges = Astor.Reports.Protocol.Models.ReportChanges;
using ReportsCollection = Astor.Reports.Domain.ReportsCollection;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route("/")]
    public class ReportsController
    {
        public ReportsCollection ReportsCollection { get; }
        public RowsStoresFactory RowsStoresFactory { get; }
        public ReportsMapper Mapper { get; }

        public ReportsController(ReportsCollection reportsCollection, RowsStoresFactory rowsStoresFactory, ReportsMapper mapper)
        {
            this.ReportsCollection = reportsCollection;
            this.RowsStoresFactory = rowsStoresFactory;
            this.Mapper = mapper;
        }
        
        [HttpGet("{reportId}")]       
        public async Task<Report> GetAsync(string reportId)
        {
            var report = await this.ReportsCollection.GetAsync(reportId);
            return await this.Mapper.MapAsync(report);
        }
        
        [HttpGet]
        public async Task<Astor.Reports.Protocol.Models.ReportsCollection> GetAsync([FromQuery]ReportsQuery query)
        {
            var reports = await this.ReportsCollection.Store.GetAsync(query);
            return await this.Mapper.MapAsync(reports);
        }
        
        [HttpPost]
        public async Task<Report> CreateAsync([FromBody] ReportCandidate candidate)
        {
            var report = await Astor.Reports.Domain.Report.CreateAsync(candidate, this.ReportsCollection.Store);
            return await this.Mapper.MapAsync(report);
        }

        [HttpPatch("{id}")]
        public async Task<Report> UpdateAsync(string id, [FromBody] ReportChanges changes)
        {
            var report = await this.ReportsCollection.GetAsync(id);
            report = await report.SaveAsync(changes, this.ReportsCollection.Store);
            return await this.Mapper.MapAsync(report);
        }
    }
}