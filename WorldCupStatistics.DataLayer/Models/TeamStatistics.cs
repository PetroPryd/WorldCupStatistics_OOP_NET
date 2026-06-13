using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public sealed record TeamStatistics
    {
        public required string Country { get; init; }
        public IReadOnlyList<Player> StartingEleven { get; init; } = [];
        public IReadOnlyList<Player> Substitutes { get; init; } = [];

        /// <summary>Union used to build the favorite-player pool from the first match.</summary>
        public IEnumerable<Player> AllPlayers => StartingEleven.Concat(Substitutes);
    }
}
