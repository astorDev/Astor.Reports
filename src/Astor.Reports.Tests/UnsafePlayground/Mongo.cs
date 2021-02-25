using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Astor.Reports.Tests.UnsafePlayground
{
    [TestClass]
    [Ignore("For experiments with mongo on real data")]
    public class Mongo
    {
        [TestMethod]
        public async Task StringFilters()
        {
            var mongoClient = new MongoClient("mongodb://10.7.8.178:27017");
            var db = mongoClient.GetDatabase("reports");
            var collection = db.GetCollection<BsonDocument>("PeriodsPlanning_2020-08-07");

            var filterString = @"{ ""InvoiceNumber"" : ""15963812460"" }";
            
            var filter = new JsonFilterDefinition<BsonDocument>(filterString);

            var docs = await collection.Find(filter).ToListAsync();
            Assert.AreEqual(1, docs.Count);
        }

        [TestMethod]
        public async Task AsyncCursor()
        {
            var mongoClient = new MongoClient("mongodb://10.7.8.178:27017");
            var db = mongoClient.GetDatabase("reports");
            var collection = db.GetCollection<BsonDocument>("PeriodsPlanning_2020-08-05");

            var filter = new JsonFilterDefinition<BsonDocument>("{}");
            var cursor = await collection.FindAsync(filter, new FindOptions<BsonDocument>
            {
                Limit = 6000000
            });

            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                Console.WriteLine(batch.Count());
            }
        }

        [TestMethod]
        public async Task CopyFew()
        {
            var mongoClient = new MongoClient("mongodb://10.7.8.178:27017");
            var db = mongoClient.GetDatabase("reports");
            var collection = db.GetCollection<BsonDocument>("PeriodsPlanning_2020-08-05");
            var smallCollection = db.GetCollection<BsonDocument>("PeriodsPlanning_Small");
            
            var filter = new JsonFilterDefinition<BsonDocument>("{}");
            var cursor = await collection.FindAsync(filter, new FindOptions<BsonDocument>
            {
                Limit = 300000
            });

            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                await smallCollection.InsertManyAsync(batch);
            }
        }
    }
}