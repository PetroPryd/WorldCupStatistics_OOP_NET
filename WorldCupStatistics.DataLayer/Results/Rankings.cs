using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Results
{
    public sealed record PlayerRanking
    {
        public required string Name { get; init; }
        public int ShirtNumber { get; init; }
        public Position Position { get; init; }
        public bool IsCaptain { get; init; }
        public int Appearances { get; init; }
        public int Goals { get; init; }
        public int YellowCards { get; init; }
    }

    public sealed record MatchAttendance
    {
        public required string Location { get; init; }
        public int Attendance { get; init; }
        public required string HomeTeam { get; init; }
        public required string AwayTeam { get; init; }
        public DateTime DateTimeUtc { get; init; }
    }
}
