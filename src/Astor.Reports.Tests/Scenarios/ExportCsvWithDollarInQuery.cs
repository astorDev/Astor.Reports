using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Tests.Scenarios
{
    [TestClass]
    public class ExportCsvWithDollarInQuery : Test
    {
        [TestMethod]
        public async Task SaveWithDollar()
        {
            var reportsStore = this.Factory.ServiceProvider.GetRequiredService<ReportsStore>();
            await reportsStore.AddAsync(this.ReportId);
         
            var store = this.GetStore();
            var timeZoneOffset = DateTimeOffset.Now.Offset;
         
            await store.AddAsync(new[]
            {
                new
                {
                    type = "good",
                    created = new DateTime(2020, 1, 1).Add(timeZoneOffset),
                },
                new
                {
                    type = "bad",
                    created = new DateTime(2020, 1, 6).Add(timeZoneOffset),
                },
                new
                {
                    type = "good",
                    created = new DateTime(2020, 1, 5).Add(timeZoneOffset)
                }
            });
                     
            var client = this.Factory.Create();
            var exportConditions = new ExportConditions
            {
                Sort = new {created = -1},
                Filter = JObject.Parse(@"{ ""type"" : { ""$ne"" : ""bad"" } }")
            };
                     
            var export = await client.CreateExport(this.ReportId, exportConditions);
            Assert.AreEqual(2, export.ElementsCount);
            
            var csv = await client.CreateCsv(this.ReportId, export.Id, new ExportFinalization
            {
                Columns = new Dictionary<string, string>
                {
                    { "type", "Group" }
                }
            });

            var resultLines = File.ReadAllLines(csv.Path);
            
            Assert.AreEqual(3, resultLines.Length);
            Assert.AreEqual("Group", resultLines[0]);
            Assert.AreEqual("good", resultLines[1]);
            Assert.AreEqual("good", resultLines[2]);
        }
    }
}