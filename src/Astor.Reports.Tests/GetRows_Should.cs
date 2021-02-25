using System;
using System.Threading.Tasks;
using Astor.Reports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class GetRows_Should : Test
    {
        public readonly dynamic[] TestData = new[]
        {
            new
            {
                id = "1",
                type = "good",
                name = "one"
            },
            new
            {
                id = "2",
                type = "bad",
                name = "two"
            },
            new
            {
                id = "3",
                type = "good",
                name = "three"
            },
            new
            {
                id = "4",
                type = "good",
                name = "four"
            },
            new
            {
                id = "5",
                type = "bad",
                name = "five"
            },
            new
            {
                id = "6",
                type = "good",
                name = "six"
            },
            new
            {
                id = "7",
                type = "good",
                name = "seven"
            },
        };
        
        [TestMethod]
        public async Task GetAllRowsWithEmptyFilter()
        {
            var collectionName = Guid.NewGuid().ToString();

            var storeFactory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            var store = storeFactory.GetRowsStore(collectionName);

            await store.AddAsync(this.TestData);

            var client = this.Factory.Create();
            var rowsCollection = await client.GetRows(collectionName, new RowsQuery
            {
                Limit = 10
            });
            
            Assert.AreEqual(7, rowsCollection.Count);
        }
        
        [TestMethod]
        public async Task ReturnRowsInLimitSatisfyingFilterAfterPassedId()
        {
            var collectionName = Guid.NewGuid().ToString();

            var storeFactory = this.Factory.ServiceProvider.GetRequiredService<RowsStoresFactory>();
            var store = storeFactory.GetRowsStore(collectionName);

            await store.AddAsync(this.TestData);

            var client = this.Factory.Create();
            var rowsCollection = await client.GetRows(collectionName, new RowsQuery
            {
                Filter = "{ 'type': 'good' }",
                Projection = "{ 'name' : 1 }",
                AfterId = "1",
                Limit = 3
            });

            Assert.AreEqual(3, rowsCollection.Count);
        }
    }
}