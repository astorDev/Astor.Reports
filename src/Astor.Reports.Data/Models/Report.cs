namespace Astor.Reports.Data.Models
{
    public class Report
    {
        public string Id { get; set; }
        
        public Export[] Exports { get; set; }
    }
}