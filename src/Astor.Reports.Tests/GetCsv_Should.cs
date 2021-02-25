using System;
using System.IO;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class GetCsv_Should : Test
    {
        [TestMethod]
        public async Task CreateValidStream()
        {
            var collectionName = Guid.NewGuid().ToString();

            var storeFactory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            var store = storeFactory.GetRowsStore(collectionName);

            await store.AddAsync(new[]
            {
                new
                {
                    id = "1",
                    name = "Alex",
                    age = 26
                },
                new
                {
                    id = "2",
                    name = "Kevin",
                    age = 34
                }
            });

            var client = this.Factory.Create();

            var resultFileName = Path.GetTempFileName();
            await using (var resultFile = File.Open(resultFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await using var responseStream = await client.GetCsv(collectionName, new ExportQuery
                {
                    Filter = "{ 'name' : 'Kevin' }",
                    Columns = "{ 'age' : 'How Old', 'name' : 'Called' }"
                });
                using var writer = new StreamReader(responseStream);
                responseStream.CopyTo(resultFile);
            }

            var resultLines = File.ReadAllLines(resultFileName);
            Assert.AreEqual("How Old,Called", resultLines[0]);
            Assert.AreEqual("34,Kevin", resultLines[1]);
        }
    }
}