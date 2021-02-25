using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Astor.Reports.Data;
using Astor.Reports.Protocol.Models;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route("/")]
    public class ReportsController
    {
        public ReportsStore ReportsStore { get; }
        public RowsStoresFactory RowsStoresFactory { get; }

        public ReportsController(ReportsStore reportsStore, RowsStoresFactory rowsStoresFactory)
        {
            this.ReportsStore = reportsStore;
            this.RowsStoresFactory = rowsStoresFactory;
        }
        
        [HttpGet("{reportId}")]       
        public async Task<Report> GetAsync(string reportId)
        {
            var report = await this.ReportsStore.SearchAsync(reportId);
            var rowsStore = this.RowsStoresFactory.GetRowsStore(reportId);
            var count = await rowsStore.CountAsync("{}");

            return new Report
            {
                Id = reportId,
                ElementsCount = count
            };
        }
    }
}