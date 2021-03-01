using System;
using System.Threading.Tasks;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class CreateReport_Should : Test
    {
        [TestMethod]
        public async Task SaveReportInNewState()
        {
            var client = this.Factory.Create();
            Clock.Time = new DateTime(2020, 1, 2, 10, 10, 0);

            var report = await client.CreateReportAsync(new ReportCandidate
            {
                EstimatedRowsCount = 5,
                Type = "Autotest",
                Status = ReportStatus.New
            });
            
            Assert.AreEqual("Autotest_2020-01-02T10:10:00", report.Id);
            Assert.AreEqual("Autotest", report.Type);
            Assert.AreEqual(new DateTime(2020, 1, 2, 10, 10, 0), report.CreationTime);
            Assert.AreEqual(new DateTime(2020, 1, 2, 10, 10, 0), report.LastModificationTime);
            Assert.AreEqual(ReportStatus.New, report.Status);
            Assert.AreEqual(0, report.RowsCount);

            report = await client.GetReportAsync("Autotest_2020-01-02T10:10:00");
            
            Assert.AreEqual("Autotest_2020-01-02T10:10:00", report.Id);
            Assert.AreEqual("Autotest", report.Type);
            Assert.AreEqual(new DateTime(2020, 1, 2, 10, 10, 0), report.CreationTime);
            Assert.AreEqual(new DateTime(2020, 1, 2, 10, 10, 0), report.LastModificationTime);
            Assert.AreEqual(ReportStatus.New, report.Status);
            Assert.AreEqual(0, report.RowsCount);
        }
    }
}