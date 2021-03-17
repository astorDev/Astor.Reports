using System;
using Astor.Reports.Protocol;
using Astor.Reports.Protocol.Models;
using Astor.Time;

namespace Astor.Reports.Domain
{
    public class EventCandidate
    {
        public string Type { get; set; }
        
        public DateTime Time { get; set; }

        public static EventCandidate CreateFromStatus(ReportStatus status)
        {
            return status switch
            {
                ReportStatus.BeingBuilt => new EventCandidate {Time = Clock.Time, Type = EventNames.BuildStarted},
                ReportStatus.Done => new EventCandidate {Time = Clock.Time, Type = EventNames.Done},
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}