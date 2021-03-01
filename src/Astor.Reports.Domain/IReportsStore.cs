using System.Threading.Tasks;

namespace Astor.Reports.Domain
{
    public interface IReportsStore
    {
        Task<Report> SaveAsync(ReportCreationChanges creationChanges);

        Task<Report> SaveAsync(ReportChanges changes);

        Task<Report> SearchAsync(string id);
    }
}