using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Dtos
{
    /// <summary>
    /// Raw shape of /teams/results (results.json) and teams.json.
    /// teams.json omits the standings fields — they deserialize to 0, which is fine.
    /// </summary>
    internal sealed class TeamDto
    {
        public int Id { get; set; }
        public string Country { get; set; } = "";
        public string? AlternateName { get; set; }
        public string FifaCode { get; set; } = "";
        public int GroupId { get; set; }
        public string GroupLetter { get; set; } = "";
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GamesPlayed { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifferential { get; set; }
    }

    internal sealed class GroupResultDto
    {
        public int Id { get; set; }
        public string Letter { get; set; } = "";
        public List<TeamDto> OrderedTeams { get; set; } = [];
    }
}
