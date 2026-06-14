using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Dtos
{
    internal sealed class MatchDto
    {
        public string? Venue { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public string? Time { get; set; }
        public string? FifaId { get; set; }
        public string? Attendance { get; set; }
        public string? StageName { get; set; }
        public string? HomeTeamCountry { get; set; }
        public string? AwayTeamCountry { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime Datetime { get; set; }

        public string? Winner { get; set; }
        public string? WinnerCode { get; set; }

        public MatchTeamDto HomeTeam { get; set; } = new();
        public MatchTeamDto AwayTeam { get; set; } = new();

        public List<MatchEventDto> HomeTeamEvents { get; set; } = [];
        public List<MatchEventDto> AwayTeamEvents { get; set; } = [];

        public TeamStatisticsDto HomeTeamStatistics { get; set; } = new();
        public TeamStatisticsDto AwayTeamStatistics { get; set; } = new();
    }

    internal sealed class MatchTeamDto
    {
        public string Country { get; set; } = "";
        public string Code { get; set; } = "";
        public int Goals { get; set; }
        public int Penalties { get; set; }
    }

    internal sealed class MatchEventDto
    {
        public int Id { get; set; }
        public string? TypeOfEvent { get; set; }
        public string Player { get; set; } = "";
        public string? Time { get; set; }
    }

    internal sealed class TeamStatisticsDto
    {
        public string Country { get; set; } = "";
        public int? YellowCards { get; set; }
        public int? RedCards { get; set; }
        public List<PlayerDto> StartingEleven { get; set; } = [];
        public List<PlayerDto> Substitutes { get; set; } = [];
    }

    internal sealed class PlayerDto
    {
        public string Name { get; set; } = "";
        public bool Captain { get; set; }
        public int ShirtNumber { get; set; }
        public string? Position { get; set; }
    }
}
