using System;
using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests.ReportEvents
{
    [TestClass]
    public class GetUnprocessed_Should : Test
    {
        [TestMethod]
        public async Task ReturnCreatedEventOfReport()
        {
            var client = this.Factory.Create();

            Clock.Time = new DateTime(2020, 1, 1);
            
            var report = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "some"
            });

            await client.UpdateReportAsync(report.Id, new ReportChanges
            {
                Status = ReportStatus.Done
            });
            
            var result = await client.GetReportEventsAsync(new EventsQuery
            {
                Processed = false
            });

            Assert.AreEqual(2, result.Count);
            var createdEvent = result.Events.FirstOrDefault(e => e.Type == EventNames.Created);
            
            Assert.IsNotNull(createdEvent);
            Assert.AreEqual(new DateTime(2020, 1, 1), createdEvent.Body.Time);
            Assert.AreEqual(report.Id, createdEvent.Body.Report.Id);
            
            var doneEvent = result.Events.FirstOrDefault(e => e.Type == EventNames.Done);
            Assert.IsNotNull(doneEvent);
            Assert.AreEqual(ReportStatus.Done, doneEvent.Body.Report.Status);
        }
    }
}