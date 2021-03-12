using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Astor.Reports.Data
{
    public static class MongoToDynamicMapper
    {
        public static dynamic ToNormalizedDynamic(BsonDocument bsonDocument)
        {
            var dotnetValue = BsonTypeMapper.MapToDotNetValue(bsonDocument);
            
            var json = JsonConvert.SerializeObject(dotnetValue, new JsonSerializerSettings
            {
                ContractResolver = new ReportsJsonContractResolver()
            });

            var jo = JObject.Parse(json);
            jo["id"] = jo["_id"];
            jo.Remove("_id");

            return jo;
        }

        public static string AdaptFilterForMongo(this string filter) => filter.Replace("'id'", "'_id'");
    }
}