using System;

namespace Astor.Reports.Protocol.Models
{
    public class ReportsQuery
    {
        public string Type { get; set; }
        
        public DateTime? ModifiedAfter { get; set; }
    }
}