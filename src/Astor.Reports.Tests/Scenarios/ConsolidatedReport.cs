using System;
using System.Collections.Generic;
using System.Linq;
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
                        Id = "77",
                        Name = "George"
                    }
                }
            });

            var dailyList = await client.GetReportsAsync(new ReportsQuery
            {
                Type = "DailyMetrics",
                ModifiedAfter = new DateTime(2000, 1, 15, 9, 0, 0)
            });

            var uniqueIds = new List<string>();

            foreach (var report in dailyList.Reports)
            {
                var ids = await client.GetRows(report.Id, new RowsQuery
                {
                    Projection = "{ 'id' : 1 }"
                });

                foreach (var idRow in ids.Rows)
                {
                    string id = idRow["id"];
                    if (!uniqueIds.Contains(id))
                    {
                        uniqueIds.Add(id);
                    }
                }
            }
            
            Assert.AreEqual(3, uniqueIds.Count);
            Assert.IsTrue(uniqueIds.Contains("5"));
            Assert.IsTrue(uniqueIds.Contains("77"));
            Assert.IsTrue(uniqueIds.Contains("66"));
        }
    }

    public class Person
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}