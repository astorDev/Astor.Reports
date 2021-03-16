using System;
using System.Linq;
using System.Linq.Expressions;
using Astor.Reports.Protocol.Models;
using SpeciVacation;
using Report = Astor.Reports.Data.Models.Report;

namespace Astor.Reports.Data
{
    public static class ReportsSpecification
    {
        public static ISpecification<Report> ToSpecification(this ReportsQuery query)
        {
            var spec = Specification<Report>.All;
            
            if (query.Type != null)
            {
                spec = spec.And(new TypeSpecification(query.Type));
            }

            if (query.ModifiedAfter != null)
            {
                spec = spec.And(new ModifiedAfterSpecification(query.ModifiedAfter.Value));
            }

            return spec;
        }
        
        public class TypeSpecification : Specification<Report>
        {
            public string Type { get; }

            public TypeSpecification(string type)
            {
                this.Type = type;
            }
            
            public override Expression<Func<Report, bool>> ToExpression()
            {
                return r => r.Type == this.Type;
            }
        }

        public class ModifiedAfterSpecification : Specification<Report>
        {
            public DateTime Time { get; }

            public ModifiedAfterSpecification(DateTime time)
            {
                this.Time = time;
            }


            public override Expression<Func<Report, bool>> ToExpression()
            {
                return r => r.Events.Any(e => e.Time > this.Time);
            }
        }

        public class AnyUnprocessedEventSpecification : Specification<Report>
        {
            public override Expression<Func<Report, bool>> ToExpression()
            {
                return r => r.Events.Any(e => !e.Processed);
            }
        }
    }


}