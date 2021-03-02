using System;
using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class GetReports_Should : Test
    {
        [TestMethod]
        public async Task ReturnValidReportMeta()
        {
            var reportId = Guid.NewGuid().ToString();

            var rowsStore = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>()
                .GetRowsStoreInternal(reportId);
            var reportsStore = this.Factory.ServiceProvider.GetRequiredService<ReportsStore>();

            await reportsStore.AddAsync(reportId);

            await rowsStore.AddAsync(new[]
            {
                new
                {
                    name = "Alex"
                },
                new
                {
                    name = "Serg"
                }
            });

            var client = this.Factory.Create();

            var report = await client.GetReportAsync(reportId);

            Assert.AreEqual(reportId, report.Id);
            Assert.AreEqual(2, report.RowsCount);
        }

        [TestMethod]
        public async Task FilterOnSinceAndType()
        {
            var client = this.Factory.Create();

            Clock.Time = new DateTime(2000, 1, 15, 10, 0, 0);

            var reportOne = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "Whatever",
                Status = ReportStatus.New
            });

            Clock.Time = new DateTime(2001, 1, 15, 10, 0, 0);

            var reportTwo = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "Whatever",
                Status = ReportStatus.New
            });

            var reportThree = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "Other",
                Status = ReportStatus.New
            });
            
            var result = await client.GetReportsAsync(new ReportsQuery
            {
                ModifiedAfter = new DateTime(2001, 1, 1),
                Type = "Whatever"
            });
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(reportTwo.Id, result.Reports.Single().Id);
        }
    }
}