namespace Astor.Reports.Protocol.Models
{
    public class ReportCandidate
    {
        public string Type { get; set; }
        
        public ReportStatus Status { get; set; }
        
        public int? EstimatedRowsCount { get; set; }
    }
}