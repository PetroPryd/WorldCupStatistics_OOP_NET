using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Results;

namespace WorldCupStatistics.DataLayer.Services
{
    public interface IWorldCupService
    {
        // ComboBox source (WinForms favourite team, WPF team + opponent)
        Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default);

        Task<Team?> GetTeamAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        // Favourite-players pool: starting_eleven ∪ substitutes from the team's FIRST played match
        Task<IReadOnlyList<Player>> GetFirstMatchPlayersAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        // WPF: teams that played against the selected team
        Task<IReadOnlyList<Team>> GetOpponentsAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        // WPF: the match between two teams (for "2 : 1" and the starting elevens / field)
        Task<Match?> GetMatchBetweenAsync(Gender gender, string fifaCodeA, string fifaCodeB, CancellationToken ct = default);

        // WinForms rankings (all relative to the selected team)
        Task<IReadOnlyList<PlayerRanking>> GetPlayerRankingsAsync(Gender gender, string fifaCode, CancellationToken ct = default);
        Task<IReadOnlyList<MatchAttendance>> GetMatchAttendanceRankingAsync(Gender gender, string fifaCode, CancellationToken ct = default);
    }
}
