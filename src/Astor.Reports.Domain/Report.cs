using System;
using System.Threading.Tasks;
using Astor.Reports.Protocol.Models;
using Astor.Time;

namespace Astor.Reports.Domain
{
    public class Report
    {
        public string Id { get; set; }
        
        public string Type { get; set; }
        
        public ReportStatus Status { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public DateTime LastModificationTime { get; set; }
        
        public int? EstimatedRowsCount { get; set; }

        public async Task<int> CountRowsAsync(string filter, IRowsStoreFactory rowsStoreFactory)
        {
            var rowsStore = rowsStoreFactory.GetRowsStore(this.Id);

            return await rowsStore.CountAsync(filter);
        }

        public async Task<Report> AddAsync(PageCandidate pageCandidate, IRowsStoreFactory rowsStoreFactory, IReportsStore reportsStore)
        {
            var rowsStore = rowsStoreFactory.GetRowsStore(this.Id);
            
            await rowsStore.AddAsync(pageCandidate.Rows);
            
            return await reportsStore.SaveAsync(new ReportChanges(this.Id)
            {
                LastModificationTime = Clock.Time,
                Status = this.Status == ReportStatus.New ? (ReportStatus?)null : ReportStatus.New
            });
        }

        public static Task<Report> CreateAsync(ReportCandidate candidate, IReportsStore store)
        {
            var id = $"{candidate.Type}_{Clock.Time:yyyy-MM-ddThh:mm:ss}";
            
            var changes = new ReportCreationChanges
            {
                ReportToAdd = new ReportCreationChanges.Report
                {
                    Id = id,
                    Type = candidate.Type,
                    CreationTime = Clock.Time,
                    LastModificationTime = Clock.Time,
                    EstimatedRowsCount = candidate.EstimatedRowsCount,
                    Status = candidate.Status
                },
                RowsCollectionToCreate = id
            };

            return store.SaveAsync(changes);
        }
    }
}