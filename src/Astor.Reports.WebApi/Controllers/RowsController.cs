using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Astor.Reports.Data;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReportsCollection = Astor.Reports.Domain.ReportsCollection;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route("/")]
    public class RowsController
    {
        public RowsStoresFactory StoresFactory { get; }
        public ReportsCollection ReportsCollection { get; }

        public RowsController(RowsStoresFactory storesFactory, ReportsCollection reportsCollection)
        {
            this.StoresFactory = storesFactory;
            this.ReportsCollection = reportsCollection;
        }

        [HttpPost("{reportId}" + "/" + Uris.Pages)]
        public async Task AddAsync(string reportId, [FromBody] PageCandidate pageCandidate)
        {
            var report = await this.ReportsCollection.GetAsync(reportId);
            await report.AddAsync(pageCandidate, this.StoresFactory, this.ReportsCollection.Store);
        }
        
        [HttpGet("{reportId}" + "/" + Uris.Rows)]
        public async Task<RowsCollection> GetRowsAsync(string reportId, [FromQuery] RowsQuery query)
        {
            var store = this.StoresFactory.GetRowsStoreInternal(reportId);
            var rows = await store.GetAsync(query);
            
            return new RowsCollection
            {
                Count = rows.Count(),
                Rows = rows.ToArray()
            };
        }

        public class BsonDocSerialized
        {
            public string Name { get; set; }

            public JToken Value { get; set; }
        }
    }
}