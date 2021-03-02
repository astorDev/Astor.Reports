using System;
using System.Threading.Tasks;
using Astor.Reports.Protocol.Models;
using Astor.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reports.Tests.Scenarios
{
    [TestClass]
    public class ConsolidatedReport : Test
    {
        [TestMethod]
        public async Task GetIdsOfAllRecordInADay()
        {
            var client = this.Factory.Create();

            Clock.Time = new DateTime(2000, 1, 15, 10, 0, 0);

            var reportOne = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "DailyMetrics",
                Status = ReportStatus.New
            });

            await client.AddPagesAsync(reportOne.Id, new PageCandidate
            {
                Rows = new[]
                {
                    new Person
                    {
                        Id = "5",
                        Name = "John"
                    },
                    new Person
                    {
                        Id = "66",
                        Name = "George"
                    }
                }
            });
            
            Clock.Time = new DateTime(2000, 1, 15, 12, 0, 0);

            var reportTwo = await client.CreateReportAsync(new ReportCandidate
            {
                Type = "DailyMetrics",
                Status = ReportStatus.New
            });

            await client.AddPagesAsync(reportTwo.Id, new PageCandidate
            {
                Rows = new[]
                {
                    new Person
                    {
                        Id = "5",
                        Name = "John"
                    },
                    new Person
                    {
                        Id = "66",
                        Name = "George"
                    }
                }
            });
            
            
            
        }
    }

    public class Person
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}