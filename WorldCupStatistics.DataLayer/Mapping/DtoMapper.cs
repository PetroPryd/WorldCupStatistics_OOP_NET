using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Dtos;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Mapping
{
    internal static class DtoMapper
    {
        public static Team ToTeam(TeamDto d) => new()
        {
            Country = d.Country,
            FifaCode = d.FifaCode,
            GroupLetter = string.IsNullOrEmpty(d.GroupLetter) ? null : d.GroupLetter,
            Wins = d.Wins,
            Draws = d.Draws,
            Losses = d.Losses,
            GamesPlayed = d.GamesPlayed,
            Points = d.Points,
            GoalsFor = d.GoalsFor,
            GoalsAgainst = d.GoalsAgainst,
            GoalDifferential = d.GoalDifferential
        };

        public static GroupResult ToGroupResult(GroupResultDto d) => new()
        {
            Letter = d.Letter,
            OrderedTeams = d.OrderedTeams.Select(ToTeam).ToList()
        };

        public static Player ToPlayer(PlayerDto d) => new()
        {
            Name = TextHelpers.Normalize(d.Name),
            ShirtNumber = d.ShirtNumber,
            Position = EnumParsing.ToPosition(d.Position),
            IsCaptain = d.Captain
        };

        public static MatchEvent ToEvent(MatchEventDto d) => new()
        {
            Type = EnumParsing.ToMatchEventType(d.TypeOfEvent),
            Player = TextHelpers.Normalize(d.Player),
            Time = d.Time
        };

        public static TeamStatistics ToStatistics(TeamStatisticsDto d) => new()
        {
            Country = d.Country,
            StartingEleven = d.StartingEleven.Select(ToPlayer).ToList(),
            Substitutes = d.Substitutes.Select(ToPlayer).ToList()
        };

        public static MatchTeam ToMatchTeam(MatchTeamDto d) => new()
        {
            Country = d.Country,
            Code = d.Code,
            Goals = d.Goals,
            Penalties = d.Penalties
        };

        public static Match ToMatch(MatchDto d) => new()
        {
            Venue = d.Venue,
            Location = d.Location,
            StageName = d.StageName,
            DateTimeUtc = d.Datetime,
            Attendance = TextHelpers.ParseAttendance(d.Attendance),
            WinnerCode = d.WinnerCode,
            HomeTeam = ToMatchTeam(d.HomeTeam),
            AwayTeam = ToMatchTeam(d.AwayTeam),
            HomeTeamStatistics = ToStatistics(d.HomeTeamStatistics),
            AwayTeamStatistics = ToStatistics(d.AwayTeamStatistics),
            HomeTeamEvents = d.HomeTeamEvents.Select(ToEvent).ToList(),
            AwayTeamEvents = d.AwayTeamEvents.Select(ToEvent).ToList()
        };
    }
}
