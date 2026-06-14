using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record MatchTeam
    {
        public required string Country { get; init; }
        public required string Code { get; init; }
        public int Goals { get; init; }
        public int Penalties { get; init; }
    }

    public sealed record Match
    {
        public string? Venue { get; init; }
        public string? Location { get; init; }
        public string? StageName { get; init; }
        public DateTime DateTimeUtc { get; init; }
        public int Attendance { get; init; }
        public string? WinnerCode { get; init; }

        public required MatchTeam HomeTeam { get; init; }
        public required MatchTeam AwayTeam { get; init; }

        public required TeamStatistics HomeTeamStatistics { get; init; }
        public required TeamStatistics AwayTeamStatistics { get; init; }

        public IReadOnlyList<MatchEvent> HomeTeamEvents { get; init; } = [];
        public IReadOnlyList<MatchEvent> AwayTeamEvents { get; init; } = [];

        public bool Involves(string fifaCode) =>
            string.Equals(HomeTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(AwayTeam.Code, fifaCode, StringComparison.OrdinalIgnoreCase);

        public string ScoreLine => $"{HomeTeam.Goals} : {AwayTeam.Goals}";
    }
}
