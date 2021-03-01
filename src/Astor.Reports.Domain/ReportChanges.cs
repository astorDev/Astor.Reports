using System;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Domain
{
    public class ReportChanges
    {
        public string ReportId { get; }
        
        public DateTime? LastModificationTime { get; set; }
        
        public ReportStatus? Status { get; set; }

        public ReportChanges(string reportId)
        {
            this.ReportId = reportId;
        }
        

    }
}