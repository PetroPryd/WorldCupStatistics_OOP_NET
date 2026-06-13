using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.DataLayer.Repositories
{
    public interface IWorldCupRepository
    {
        Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default);
        Task<IReadOnlyList<Match>> GetMatchesAsync(Gender gender, CancellationToken ct = default);
        Task<IReadOnlyList<Match>> GetMatchesForCountryAsync(Gender gender, string fifaCode, CancellationToken ct = default);
    }
}
