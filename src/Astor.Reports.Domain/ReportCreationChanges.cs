using System;
using Astor.Reports.Protocol.Models;

namespace Astor.Reports.Domain
{
    public class ReportCreationChanges
    {
        public Report ReportToAdd { get; set; }

        public EventCandidate[] Events { get; set; }

        public class Report
        {
            public string Id { get; set; }

            public string Type { get; set; }

            public ReportStatus Status { get; set; }

            public int? EstimatedRowsCount { get; set; }
        }
    }
}