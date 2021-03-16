using System;

namespace Astor.Reports.Protocol.Models
{
    public class ReportEvent
    {
        public string Id { get; set; }
        
        public bool Processed { get; set; }
        
        public string Type { get; set; }
        
        public ReportEventBody Body { get; set; }
    }

    public class ReportEventBody
    {
        public DateTime Time { get; set; }
        
        public Report Report { get; set; }
    }
}