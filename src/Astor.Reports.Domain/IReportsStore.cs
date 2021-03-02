using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Domain
{
    public interface IReportsStore
    {
        Task<Report> SaveAsync(ReportCreationChanges creationChanges);

        Task<Report> SaveAsync(ReportChanges changes);

        Task<Report> SearchAsync(string id);

        Task<IEnumerable<Report>> GetAsync(ReportsQuery query);
    }
}