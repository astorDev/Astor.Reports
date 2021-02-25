namespace Astor.Reports.Protocol.Models
{
    public class RowsQuery
    {
        public string Filter { get; set; }
        
        public string Projection { get; set; }
        
        public int? Limit { get; set; }

        public string AfterId { get; set; }
        
        public string Sorting { get; set; }
    }
}