using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests.Scenarios
{
    [TestClass]
    public class ProcessingEvents : Test
    {
        [TestMethod]
        public async Task AccomplishedRight()
        {
            var client = this.Factory.Create();

            var report = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "processingEventsTest"
            });

            await client.UpdateReportAsync(report.Id, new ReportChanges
            {
                Status = ReportStatus.Done
            });

            var unprocessed = await client.GetReportEventsAsync(new EventsQuery
            {
                Processed = false
            });
            
            Assert.AreEqual(2, unprocessed.Count);

            var createdEvent = unprocessed.Events.Single(e => e.Type == EventNames.Created);
            await client.UpdateReportEventAsync(createdEvent.Id, new ReportEventChanges
            {
                Processed = true
            });

            unprocessed = await client.GetReportEventsAsync(new EventsQuery
            {
                Processed = false
            });
            
            Assert.AreEqual(1, unprocessed.Count);
        }
    }
}