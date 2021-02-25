using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class PostCsv_Should : Test
    {
        [TestMethod]
        public async Task CreateCsvFile()
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
            var csv = await client.CreateCsv(collectionName, created.Id, new ExportFinalization
            {
                Columns = new Dictionary<string, string>
                {
                    { "age", "Old" },
                    { "name", "Called" }
                }
            });

            var resultLines = File.ReadAllLines(csv.Path);
            
            Assert.AreEqual(2, resultLines.Length);
            Assert.AreEqual("Old,Called", resultLines[0]);
            Assert.AreEqual("26,Alex", resultLines[1]);
        }
    }
}