using System.Collections.Generic;

namespace Astor.Reports.Domain
{
    public class EventsFilter
    {
        public IEnumerable<string> Ids { get; set; }
        
        public bool? Processed { get; set; }
    }
}