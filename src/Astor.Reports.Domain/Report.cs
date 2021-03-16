using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;

namespace Astor.Reports.Domain
{
    public class Report
    {
        public string Id { get; set; }
        
        public string Type { get; set; }
        
        public ReportStatus Status { get; set; }

        public DateTime CreationTime => this.Events.Single(e => e.Type == EventNames.Created).Time;

        public DateTime LastModificationTime => this.Events.OrderByDescending(e => e.Time).First().Time;
        
        public int? EstimatedRowsCount { get; set; }
        
        public Event[] Events { get; set; }

        public async Task<int> CountRowsAsync(string filter, IRowsStoreFactory rowsStoreFactory)
        {
            var rowsStore = rowsStoreFactory.GetRowsStore(this.Id);

            return await rowsStore.CountAsync(filter);
        }

        public async Task<Report> SaveAsync(Protocol.Models.ReportChanges inputChanges, IReportsStore reportsStore)
        {
            var changes = new ReportChanges(this.Id);
            
            if (inputChanges.Status != null)
            {
                changes.Status = inputChanges.Status.Value;
                changes.Events.Add(EventCandidate.CreateFromStatus(inputChanges.Status.Value));
            }

            return await reportsStore.SaveAsync(changes);
        }
        
        public async Task<Report> AddAsync(PageCandidate pageCandidate, IRowsStoreFactory rowsStoreFactory, IReportsStore reportsStore)
        {
            var rowsStore = rowsStoreFactory.GetRowsStore(this.Id);
            
            await rowsStore.AddAsync(pageCandidate.Rows);
            
            if (this.Status != ReportStatus.BeingBuilt)
            {
                return await reportsStore.SaveAsync(new ReportChanges(this.Id)
                {
                    Status = ReportStatus.BeingBuilt,
                    Events = new List<EventCandidate>()
                    {
                        EventCandidate.CreateFromStatus(ReportStatus.BeingBuilt),
                    }
                });
            }

            return this;
        }

        public static Task<Report> CreateAsync(ReportCandidate candidate, IReportsStore store)
        {
            var id = $"{candidate.Type}_{Clock.Time:yyyy-MM-ddTHH:mm:ss}";
            
            var changes = new ReportCreationChanges
            {
                ReportToAdd = new ReportCreationChanges.Report
                {
                    Id = id,
                    Type = candidate.Type,
                    EstimatedRowsCount = candidate.EstimatedRowsCount,
                    Status = ReportStatus.Pending
                },
                Events = new []
                {
                    new EventCandidate
                    {
                        Time = Clock.Time,
                        Type = EventNames.Created
                    }
                }
            };

            return store.SaveAsync(changes);
        }
    }
}