using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Results
{
    /// <summary>A player of the selected team with stats aggregated across that team's matches.</summary>
    public sealed record PlayerRanking
    {
        public required string Name { get; init; }
        public int ShirtNumber { get; init; }      // image key + display
        public Position Position { get; init; }
        public bool IsCaptain { get; init; }
        public int Appearances { get; init; }       // matches where in starting_eleven or substitutes
        public int Goals { get; init; }
        public int YellowCards { get; init; }
    }

    /// <summary>A single match ranked by attendance, for the selected team.</summary>
    public sealed record MatchAttendance
    {
        public required string Location { get; init; }
        public int Attendance { get; init; }
        public required string HomeTeam { get; init; }
        public required string AwayTeam { get; init; }
        public DateTime DateTimeUtc { get; init; }
    }
}
