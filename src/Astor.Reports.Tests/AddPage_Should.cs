using System;
using System.Threading.Tasks;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class AddPage_Should : Test
    {
        [TestMethod]
        public async Task ChangeRowsCounter()
        {
            var client = this.Factory.Create();
            Clock.Time = new DateTime(2020, 1, 1, 10, 0, 0);
            var report = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "AutotestPage",
                EstimatedRowsCount = 5,
                Status = ReportStatus.New
            });
            
            Clock.Time = new DateTime(2020, 1, 1, 11, 0, 0);
            await client.AddPagesAsync(report.Id, new PageCandidate
            {
                Rows = new[]
                {
                    new {Id = "1", Name = "Henry"},
                    new {Id = "2", Name = "Josh"}
                }
            });

            report = await client.GetReportAsync(report.Id);
            
            Assert.AreEqual(2, report.RowsCount);
        }

        [TestMethod]
        public async Task ChangeModificationTime()
        {
            var client = this.Factory.Create();
            Clock.Time = new DateTime(2020, 1, 1, 10, 0, 0);
            var report = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "AutotestPage",
                EstimatedRowsCount = 5,
                Status = ReportStatus.New
            });
            
            Clock.Time = new DateTime(2020, 1, 1, 11, 0, 0);
            await client.AddPagesAsync(report.Id, new PageCandidate
            {
                Rows = new[]
                {
                    new {Id = "1", Name = "Henry"},
                    new {Id = "2", Name = "Josh"}
                }
            });

            report = await client.GetReportAsync(report.Id);
            
            Assert.AreEqual(new DateTime(2020, 1, 1, 11, 0, 0), report.LastModificationTime);
        }
    }
}