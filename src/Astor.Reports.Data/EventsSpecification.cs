using System;
using System.Linq.Expressions;
using Astor.Reports.Domain;
using Astor.Reports.Protocol;
using MongoDB.Bson;
using SpeciVacation;
using Event = Astor.Reports.Data.Models.Event;

namespace Astor.Reports.Data
{
    public static class EventsSpecification
    {
        public static ISpecification<Event> ToSpecification(this EventsFilter filter)
        {
            var spec = Specification<Event>.All;

            if (filter.Processed != null)
            {
                spec = spec.And(new ProcessedEventSpecification(filter.Processed.Value));
            }

            if (filter.Ids != null)
            {
                spec = spec.And(filter.Ids.ToOrSpecification(id => new IdSpecification(id)));
            }

            return spec;
        }
        
        public class ProcessedEventSpecification : Specification<Event>
        {
            public bool Processed { get; }

            public ProcessedEventSpecification(bool processed)
            {
                this.Processed = processed;
            }
            
            public override Expression<Func<Event, bool>> ToExpression()
            {
                return r => r.Processed == this.Processed;
            }
        }

        public class IdSpecification : Specification<Event>
        {
            public ObjectId Id { get; }

            public IdSpecification(string id)
            {
                this.Id = ObjectId.Parse(id);
            }


            public override Expression<Func<Event, bool>> ToExpression()
            {
                return e => e.Id == this.Id;
            }
        }
    }
}