using System;
using System.Collections.Generic;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Domain
{
    public class ReportChanges
    {
        public string ReportId { get; }
        
        public ReportStatus? Status { get; set; }

        public List<EventCandidate> Events { get; set; } = new();
        
        public ReportChanges(string reportId)
        {
            this.ReportId = reportId;
        }
        
        
    }
}