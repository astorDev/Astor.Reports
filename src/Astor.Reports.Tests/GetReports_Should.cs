using System;
using System.Threading.Tasks;
using Astor.Reports.Data;
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

            var rowsStore = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>().GetRowsStore(reportId);
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
            Assert.AreEqual(2, report.ElementsCount);
        }
    }
}