using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using ExportConditions = Astor.Reports.Protocol.Models.ExportConditions;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class GetExport_Should : Test
    {
        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task ReturnNotFound_WhenExportDoesNotExists()
        {
            var client = this.Factory.Create();

            await client.GetExportAsync("66", "66");
        }
        
        [TestMethod]
        public async Task ReturnValidExport()
        {
            var collectionName = Guid.NewGuid().ToString();

            var storeFactory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            var store = storeFactory.GetRowsStore(collectionName);

            var reportsStore = this.Factory.ServiceProvider.GetRequiredService<ReportsStore>();
            await reportsStore.AddAsync(collectionName);

            await store.AddAsync(new[]
            {
                new
                {
                    id = "1",
                    name = "Alex",
                    age = 26
                },
                new
                {
                    id = "2",
                    name = "Kevin",
                    age = 34
                }
            });

            var client = this.Factory.Create();
            var condition = new ExportConditions
            {
                Filter = new
                {
                    name = "Alex"
                },
                Sort = JObject.Parse("{ '_id': -1 }")
            };
            
            var created = await client.CreateExport(collectionName, condition);

            var export = await client.GetExportAsync(collectionName, created.Id);
            
            Assert.AreEqual(1, export.ElementsCount);
            Assert.AreEqual("1", export.Buckets.Single().Start.ToString());
            Assert.AreEqual("1", export.Buckets.Single().Start.ToString());
        }
    }
}