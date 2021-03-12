using System;
using Astor.Reports.Protocol.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Astor.Reports.Data.Models
{
    public class Report
    {
        public string Id { get; set; }
        
        public string Type { get; set; }
        
        public ReportStatus Status { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationTime { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastModificationTime { get; set; }
        
        public int? EstimatedRowsCount { get; set; }

        public Export[] Exports { get; set; }
    }
}