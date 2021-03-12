using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Data.Queries
{
    public class RowsQuery
    {
        public FilterDefinition<BsonDocument> Filter { get; set; }

        public SortDefinition<BsonDocument> Sort { get; set; }

        public ProjectionDefinition<BsonDocument> Projection { get; set; }
        
        public int? Limit { get; set; }

        public static RowsQuery ForExport(Export export, ExportBucket bucket, Dictionary<string, string> columns)
        {
            return new RowsQuery
            {
                Filter = BuildFilter(export, bucket),
                Projection = BuildProjection(columns),
                Sort = new JsonSortDefinition<BsonDocument>(JObject.FromObject(export.Conditions.Sort).ToString())
            };
        } 
        
        public static FilterDefinition<BsonDocument> BuildFilter(Export export, ExportBucket bucket)
        {
            var sortKeyValue = ((Dictionary<string, object>) export.Conditions.Sort).First();
            var sortingField = sortKeyValue.Key;
            var order = ((int) sortKeyValue.Value);

            var startFilter = new BsonDocument(sortingField, new BsonDocument(order == -1 ? "$lte" : "$gte", bucket.Start));
            var endFilter = new BsonDocument(sortingField, new BsonDocument(order == -1 ? "$gte" : "$lte", bucket.End));
            
            string mainFilterRawString = JObject.FromObject(export.Conditions.Filter ?? new {}).ToString();
            var mainFilterString = mainFilterRawString.Replace("*dollar*", "$").Replace("id", "_id");
            var mainFilter = new JsonFilterDefinition<BsonDocument>(mainFilterString);

            return Builders<BsonDocument>.Filter.And(startFilter, endFilter, mainFilter);
        }
        
        public static ProjectionDefinition<BsonDocument> BuildProjection(Dictionary<string, string> columns)
        {
            var projectionDictionary = columns.ToDictionary(c => c.Key, c => 1);
            var projectionString = JObject.FromObject(projectionDictionary).ToString();
            return new JsonProjectionDefinition<BsonDocument>(projectionString);
        }
    }
}