using System;

namespace Astor.Reports.Domain
{
    public class Event
    {
        public string Id { get; set; }
        
        public bool Processed { get; set; }
        
        public string Type { get; set; }
        
        public DateTime Time { get; set; }
    }
}