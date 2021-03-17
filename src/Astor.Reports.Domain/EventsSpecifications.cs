using System;
using System.Linq.Expressions;
using SpeciVacation;

namespace Astor.Reports.Domain
{
    public static class EventsSpecifications
    {
        public static ISpecification<Event> ToSpecification(this EventsFilter filter)
        {
            var spec = Specification<Event>.All;

            if (filter.Processed != null)
            {
                spec = spec.And(new ProcessedSpecification(filter.Processed.Value));
            }

            if (filter.Ids != null)
            {
                spec = spec.And(filter.Ids.ToOrSpecification(id => new IdSpecification(id)));
            }

            return spec;
        }

        public class ProcessedSpecification : Specification<Event>
        {
            public bool Processed { get; }

            public ProcessedSpecification(bool processed)
            {
                this.Processed = processed;
            }
            
            public override Expression<Func<Event, bool>> ToExpression()
            {
                return e => e.Processed == this.Processed;
            }
        }

        public class IdSpecification : Specification<Event>
        {
            public string Id { get; }

            public IdSpecification(string id)
            {
                this.Id = id;
            }

            public override Expression<Func<Event, bool>> ToExpression()
            {
                return e => e.Id == this.Id;
            }
        }
    }
}