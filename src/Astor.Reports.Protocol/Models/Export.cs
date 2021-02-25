namespace Astor.Reports.Protocol.Models
{
    public class Export
     {
         public string Id { get; set; }
         
         public string ReportId { get; set; }
         
         public ExportConditions Conditions { get; set; }
         
         public int ElementsCount { get; set; }
 
         public ExportBucket[] Buckets { get; set; }
     }
     
     public class ExportBucket
     {
         public dynamic Start { get; set; }
 
         public dynamic End { get; set; }
         
         public int ElementsCount { get; set; }
     }
 }