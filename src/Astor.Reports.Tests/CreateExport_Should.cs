using System;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Astor.Reports.Protocol.Models;
using PickPoint.Reports.WebApi.Helpers;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class CreateExport_Should : Test
    {
        [TestMethod]
        public async Task CreateCorrectMap()
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
                    created = new DateTime(2020, 1, 1),
                },
                new
                {
                    type = "good",
                    created = new DateTime(2020, 1, 10),
                },
                new
                {
                    type = "good",
                    created =  new DateTime(2020, 1, 10)
                },
                new
                {
                    type = "bad",
                    created = new DateTime(2020, 1, 6),
                },
                new
                {
                    type = "good",
                    created = new DateTime(2020, 1, 5)
                },
                new
                {
                    type = "good",
                    created = new DateTime(2020, 1, 6)
                }
            });

            var client = this.Factory.Create();
            var exportConditions = new ExportConditions
            {
                Sort = new {created = -1},
                Filter = new {type = "good"}
            };

            ExportBuckets.MaxCount = 3;
            var export = await client.CreateExport(this.ReportId, exportConditions);
            
            Assert.AreEqual(5, export.ElementsCount);
            Assert.AreEqual(3, export.Buckets.Length, "buckets length");
            Assert.AreEqual(new DateTime(2020, 1, 10), export.Buckets[0].Start);
            Assert.AreEqual(new DateTime(2020, 1, 10), export.Buckets[0].End);
            Assert.AreEqual(2, export.Buckets[0].ElementsCount);
            Assert.AreEqual(new DateTime(2020, 1, 6), export.Buckets[1].Start);
            Assert.AreEqual(new DateTime(2020, 1, 5), export.Buckets[1].End);
            Assert.AreEqual(2, export.Buckets[1].ElementsCount);
            Assert.AreEqual(new DateTime(2020, 1, 1), export.Buckets[2].Start);
            Assert.AreEqual(new DateTime(2020, 1, 1), export.Buckets[2].End);
            Assert.AreEqual(1, export.Buckets[2].ElementsCount);
        }
    }
}