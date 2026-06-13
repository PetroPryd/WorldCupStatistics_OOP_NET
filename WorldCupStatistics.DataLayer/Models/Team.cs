using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record Team
    {
        public required string Country { get; init; }
        public required string FifaCode { get; init; }
        public string? GroupLetter { get; init; }

        public int Wins { get; init; }
        public int Draws { get; init; }
        public int Losses { get; init; }
        public int GamesPlayed { get; init; }
        public int Points { get; init; }
        public int GoalsFor { get; init; }
        public int GoalsAgainst { get; init; }
        public int GoalDifferential { get; init; }

        /// <summary>Display form required by both clients: "NAME (FIFA_CODE)".</summary>
        public string DisplayName => $"{Country} ({FifaCode})";
    }
}
