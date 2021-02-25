using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Astor.Reports.Data;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;

namespace PickPoint.Reports.WebApi.Controllers
{
    [Route("/")]
    public class RowsController
    {
        public RowsStoresFactory StoresFactory { get; }

        public RowsController(RowsStoresFactory storesFactory)
        {
            this.StoresFactory = storesFactory;
        }
        
        [HttpGet("{reportId}" + "/" + Uris.Rows)]
        public async Task<RowsCollection> GetRowsAsync(string reportId, [FromQuery] RowsQuery query)
        {
            var store = this.StoresFactory.GetRowsStore(reportId);
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