using System;

namespace Astor.Reports.Protocol.Models
{
    public class Report
    {
        public string Id { get; set; }
        
        public string Type { get; set; }
        
        public ReportStatus Status { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public DateTime LastModificationTime { get; set; }
        
        public int? EstimatedRowsCount { get; set; }
        
        public int RowsCount { get; set; }
    }
}