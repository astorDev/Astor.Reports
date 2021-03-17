using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Reports.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Report = Astor.Reports.Data.Models.Report;

namespace Astor.Reports.Data
{
    public class EventsStore : IEventsStore
    {
        public IMongoCollection<Report> Collection { get; }

        public EventsStore(IMongoCollection<Report> collection)
        {
            this.Collection = collection;
        }

        public async Task SaveAsync(EventChanges changes)
        {
            var update = new UpdateDefinitionBuilder<Report>()
                .Combine(getUpdates(changes));
            
            var oid = ObjectId.Parse(changes.Id);
            
            var filter = new FilterDefinitionBuilder<Report>()
                .ElemMatch(r => r.Events, e => e.Id == oid);

            await this.Collection.UpdateOneAsync(filter, update);
        }

        public IEnumerable<UpdateDefinition<Report>> getUpdates(EventChanges changes)
        {
            if (changes.Processed != null)
            {
                yield return new UpdateDefinitionBuilder<Report>()
                    .Set("events.$.processed", true);
            }
        }
    }
}