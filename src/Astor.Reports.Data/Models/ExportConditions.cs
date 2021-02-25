using System;
using System.Text;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Astor.Reports.Data.Models
{
    public class ExportConditions 
    {
        public BsonDocument Filter { get; set; }

        public BsonDocument Sort { get; set; }

        public object FilterDotNetObject => this.Filter == null ? null : BsonTypeMapper.MapToDotNetValue(this.Filter);

        public object SortDotNetObject => BsonTypeMapper.MapToDotNetValue(this.Sort);

        public override string ToString()
        {
            return base64(this.FilterDotNetObject) + "-" + base64(this.SortDotNetObject);
        }

        public static ExportConditions Parse(string id)
        {
            var parts = id.Split("-");
            
            return new ExportConditions
            {
                Filter = doc(parts[0]),
                Sort = doc(parts[1])
            };
        }
        
        private static string base64(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            
            var json = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.Default.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        private static BsonDocument doc(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var json = Encoding.Default.GetString(bytes);

            return String.IsNullOrEmpty(json) ? null : BsonDocument.Parse(json);
        }
        
        
        
    }
}