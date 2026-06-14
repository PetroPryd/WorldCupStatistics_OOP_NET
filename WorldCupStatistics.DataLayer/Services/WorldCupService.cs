using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Repositories;
using WorldCupStatistics.DataLayer.Results;

namespace WorldCupStatistics.DataLayer.Services
{
    internal sealed class WorldCupService : IWorldCupService
    {
        private readonly IWorldCupRepository _repository;

        private readonly ConcurrentDictionary<Gender, IReadOnlyList<Match>> _matchCache = new();
        private readonly ConcurrentDictionary<Gender, IReadOnlyList<Team>> _teamCache = new();

        public WorldCupService(IWorldCupRepository repository) => _repository = repository;

        public async Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default)
        {
            if (_teamCache.TryGetValue(gender, out var cached)) return cached;
            var teams = (await _repository.GetTeamsAsync(gender, ct))
                .OrderBy(t => t.Country, StringComparer.CurrentCulture)
                .ToList();
            _teamCache[gender] = teams;
            return teams;
        }

        public async Task<Team?> GetTeamAsync(Gender gender, string fifaCode, CancellationToken ct = default)
            => (await GetTeamsAsync(gender, ct))
                .FirstOrDefault(t => Same(t.FifaCode, fifaCode));

        public async Task<IReadOnlyList<Player>> GetFirstMatchPlayersAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var matches = await GetMatchesAsync(gender, ct);

            var firstMatch = matches
                .Where(m => m.Involves(fifaCode))
                .OrderBy(m => m.DateTimeUtc)
                .FirstOrDefault();

            if (firstMatch is null) return [];

            var stats = StatsFor(firstMatch, fifaCode);
            return stats is null ? [] : stats.AllPlayers.OrderBy(p => p.ShirtNumber).ToList();
        }

        public async Task<IReadOnlyList<Team>> GetOpponentsAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var matches = await GetMatchesAsync(gender, ct);
            var teams = await GetTeamsAsync(gender, ct);

            var opponentCodes = matches
                .Where(m => m.Involves(fifaCode))
                .Select(m => Same(m.HomeTeam.Code, fifaCode) ? m.AwayTeam.Code : m.HomeTeam.Code)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            return opponentCodes
                .Select(code => teams.FirstOrDefault(t => Same(t.FifaCode, code))
                                ?? new Team { Country = code, FifaCode = code })
                .OrderBy(t => t.Country, StringComparer.CurrentCulture)
                .ToList();
        }

        public async Task<Match?> GetMatchBetweenAsync(Gender gender, string fifaCodeA, string fifaCodeB, CancellationToken ct = default)
        {
            var matches = await GetMatchesAsync(gender, ct);
            return matches.FirstOrDefault(m => m.Involves(fifaCodeA) && m.Involves(fifaCodeB));
        }

        public async Task<IReadOnlyList<PlayerRanking>> GetPlayerRankingsAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var teamMatches = (await GetMatchesAsync(gender, ct))
                .Where(m => m.Involves(fifaCode))
                .ToList();

            var profiles = new Dictionary<string, Player>();
            var appearances = new Dictionary<string, int>();
            var goals = new Dictionary<string, int>();
            var yellows = new Dictionary<string, int>();

            foreach (var match in teamMatches)
            {
                var stats = StatsFor(match, fifaCode);
                var events = EventsFor(match, fifaCode);
                if (stats is null) continue;

                foreach (var player in stats.AllPlayers)
                {
                    profiles.TryAdd(player.Name, player);
                    appearances[player.Name] = appearances.GetValueOrDefault(player.Name) + 1;
                }

                foreach (var ev in events)
                {
                    if (ev.Type.IsGoal())
                        goals[ev.Player] = goals.GetValueOrDefault(ev.Player) + 1;
                    else if (ev.Type == MatchEventType.YellowCard)
                        yellows[ev.Player] = yellows.GetValueOrDefault(ev.Player) + 1;
                }
            }

            return profiles.Values
                .Select(p => new PlayerRanking
                {
                    Name = p.Name,
                    ShirtNumber = p.ShirtNumber,
                    Position = p.Position,
                    IsCaptain = p.IsCaptain,
                    Appearances = appearances.GetValueOrDefault(p.Name),
                    Goals = goals.GetValueOrDefault(p.Name),
                    YellowCards = yellows.GetValueOrDefault(p.Name)
                })
                .OrderByDescending(r => r.Goals)
                .ThenByDescending(r => r.YellowCards)
                .ThenBy(r => r.Name, StringComparer.CurrentCulture)
                .ToList();
        }

        public async Task<IReadOnlyList<MatchAttendance>> GetMatchAttendanceRankingAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var matches = await GetMatchesAsync(gender, ct);
            return matches
                .Where(m => m.Involves(fifaCode))
                .OrderByDescending(m => m.Attendance)
                .Select(m => new MatchAttendance
                {
                    Location = m.Location ?? m.Venue ?? "—",
                    Attendance = m.Attendance,
                    HomeTeam = m.HomeTeam.Country,
                    AwayTeam = m.AwayTeam.Country,
                    DateTimeUtc = m.DateTimeUtc
                })
                .ToList();
        }

        private async Task<IReadOnlyList<Match>> GetMatchesAsync(Gender gender, CancellationToken ct)
        {
            if (_matchCache.TryGetValue(gender, out var cached)) return cached;
            var matches = await _repository.GetMatchesAsync(gender, ct);
            _matchCache[gender] = matches;
            return matches;
        }

        private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        private static TeamStatistics? StatsFor(Match m, string fifaCode) =>
            Same(m.HomeTeam.Code, fifaCode) ? m.HomeTeamStatistics
            : Same(m.AwayTeam.Code, fifaCode) ? m.AwayTeamStatistics
            : null;

        private static IReadOnlyList<MatchEvent> EventsFor(Match m, string fifaCode) =>
            Same(m.HomeTeam.Code, fifaCode) ? m.HomeTeamEvents
            : Same(m.AwayTeam.Code, fifaCode) ? m.AwayTeamEvents
            : [];
    }
}
