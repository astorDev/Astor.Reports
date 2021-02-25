using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Data.Models
{
    public class Export
    {
        public ExportConditions Id { get; set; }
        
        public ExportBucket[] Buckets { get; set; }
        
        public int ElementsCount { get; set; }
    }
}