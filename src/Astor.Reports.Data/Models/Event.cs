using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Astor.Reports.Data.Models
{
    public class Event
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        
        public string Type { get; set; }
        
        public bool Processed { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
    }
}