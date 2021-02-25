using System;
using Astor.Reports.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace Astor.Reports.Tests
{
    [TestClass]
    public class ExportCondition_Should
    {
        [TestMethod]
        public void CorrectlyLookAsStringAndBeParsable()
        {
            var condition = new ExportConditions
            {
                Filter = BsonDocument.Parse("{ 'name' : 'Alex' }"),
                Sort = BsonDocument.Parse("{ '_id' : -1 }")
            };

            var conditionString = condition.ToString();
            Console.WriteLine($"{conditionString} : {conditionString.Length} chars");

            var parsed = ExportConditions.Parse(conditionString);
            Assert.AreEqual(condition.ToString(), parsed.ToString());
        }

        [TestMethod]
        public void BeOkWhenFilterIsEmpty()
        {
            var condition = new ExportConditions
            {
                Sort = BsonDocument.Parse("{ 'createdAt' : -1 }")
            };

            var id = condition.ToString();

            var parsed = ExportConditions.Parse(id);
            
            Assert.AreEqual(condition.ToString(), parsed.ToString());
        }
    }
}