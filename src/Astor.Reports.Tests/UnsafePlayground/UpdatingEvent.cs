using System;
using System.Threading.Tasks;
using Astor.Reports.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PickPoint.Reports.WebApi.Helpers;

namespace Astor.Reports.Tests.UnsafePlayground
{
    [TestClass]
    [Ignore("Playground")]
    public class UpdatingEvent
    {
        [TestMethod]
        public async Task Works()
        {
            MongoConventions.Register(new IConvention[]
            {
                new CamelCaseElementNameConvention(), 
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true)
            });
            
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("reports");
            var collection = db.GetCollection<Report>("reports");

            var update = new UpdateDefinitionBuilder<Report>()
                .Set("events.$.processed", true);
            

            var oid = ObjectId.Parse("6051ccdff874fdd985966b5d");
            
            var filter = new FilterDefinitionBuilder<Report>()
                .ElemMatch(r => r.Events, e => e.Id == oid);

            // var filter2 =
            //     new JsonFilterDefinition<Report>(
            //         "{ 'events' : { '$elemMatch' : { 'processed' : false } } }");

            var reports = await collection.Find(filter).ToListAsync();

            Assert.AreEqual(1, reports.Count);
            
            var result = await collection.UpdateOneAsync(filter, update);
            Console.WriteLine(result.MatchedCount);
        }
    }
}