using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Astor.Reports.Protocol.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RowsQuery = Astor.Reports.Data.Queries.RowsQuery;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class RowsQuery_Should : Test
    {
        [TestMethod]
        public async Task CreateValidQuery_WhenSortedByString()
        {
            var reportId = Guid.NewGuid().ToString();
            var rowsStore = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>().GetRowsStoreInternal(reportId);

            await rowsStore.AddAsync(new[]
            {
                new
                {
                    id = "1",
                    name = "Alex"
                }
            });

            var export = new Protocol.Models.Export
            {
                Conditions = new Protocol.Models.ExportConditions
                {
                    Filter = new {name = "Alex"},
                    Sort = new Dictionary<string, object>
                    {
                        {"_id", -1}
                    }
                }
            };

            var bucket = new Protocol.Models.ExportBucket
            {
                Start = "1",
                End = "1"
            };

            var columns = new Dictionary<string, string>
            {
                {"name", "Called"}
            };

            var query = RowsQuery.ForExport(export, bucket, columns);

            var result = rowsStore.GetAsyncEnumerable(query);
            
            Assert.AreEqual(1, await result.CountAsync());
        }
    }
}