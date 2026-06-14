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
        Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default);

        Task<Team?> GetTeamAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        Task<IReadOnlyList<Player>> GetFirstMatchPlayersAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        Task<IReadOnlyList<Team>> GetOpponentsAsync(Gender gender, string fifaCode, CancellationToken ct = default);

        Task<Match?> GetMatchBetweenAsync(Gender gender, string fifaCodeA, string fifaCodeB, CancellationToken ct = default);

        Task<IReadOnlyList<PlayerRanking>> GetPlayerRankingsAsync(Gender gender, string fifaCode, CancellationToken ct = default);
        Task<IReadOnlyList<MatchAttendance>> GetMatchAttendanceRankingAsync(Gender gender, string fifaCode, CancellationToken ct = default);
    }
}
