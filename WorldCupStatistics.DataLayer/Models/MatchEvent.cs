using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record MatchEvent
    {
        public MatchEventType Type { get; init; }
        public required string Player { get; init; }
        public string? Time { get; init; }
    }
}
