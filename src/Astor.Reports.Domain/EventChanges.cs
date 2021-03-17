namespace Astor.Reports.Domain
{
    public class EventChanges
    {
        public string Id { get; }

        public bool? Processed { get; set; }
        
        public EventChanges(string id)
        {
            this.Id = id;
        }
    }
}