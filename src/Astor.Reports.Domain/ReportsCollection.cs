using System.Threading.Tasks;

namespace Astor.Reports.Domain
{
    public class ReportsCollection
    {
        public IReportsStore Store { get; }

        public ReportsCollection(IReportsStore reportsStore)
        {
            this.Store = reportsStore;
        }

        public async Task<Report> GetAsync(string id)
        {
            var report = await this.Store.SearchAsync(id);
            if (report == null)
            {
                throw new ReportNotFoundException();
            }

            return report;
        }

        public async Task<Report> GetAsync(ReportsFilter filter)
        {
            var report = await this.Store.SearchAsync(filter);
            if (report == null)
            {
                throw new ReportNotFoundException();
            }

            return report;
        } 
    }
}