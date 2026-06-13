using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record GroupResult
    {
        public required string Letter { get; init; }
        public IReadOnlyList<Team> OrderedTeams { get; init; } = [];
    }
}
