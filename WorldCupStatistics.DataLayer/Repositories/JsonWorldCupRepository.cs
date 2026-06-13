using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Configuration;
using WorldCupStatistics.DataLayer.Dtos;
using WorldCupStatistics.DataLayer.Exceptions;
using WorldCupStatistics.DataLayer.Mapping;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Serialization;

namespace WorldCupStatistics.DataLayer.Repositories
{
    internal sealed class JsonWorldCupRepository : IWorldCupRepository
    {
        private readonly DataSourceConfig _config;

        public JsonWorldCupRepository(IOptions<DataSourceConfig> options) => _config = options.Value;

        public async Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default)
        {
            var dtos = await ReadAsync<List<TeamDto>>(gender, "results.json", ct);
            return dtos.Select(DtoMapper.ToTeam).ToList();
        }

        public async Task<IReadOnlyList<Match>> GetMatchesAsync(Gender gender, CancellationToken ct = default)
        {
            var dtos = await ReadAsync<List<MatchDto>>(gender, "matches.json", ct);
            return dtos.Select(DtoMapper.ToMatch).ToList();
        }

        public async Task<IReadOnlyList<Match>> GetMatchesForCountryAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var all = await GetMatchesAsync(gender, ct);
            return all.Where(m => m.Involves(fifaCode)).ToList();
        }

        private async Task<T> ReadAsync<T>(Gender gender, string fileName, CancellationToken ct)
        {
            var path = Path.Combine(AppContext.BaseDirectory, _config.DataFolder, gender.ToSegment(), fileName);

            if (!File.Exists(path))
                throw new DataLayerException($"Data file not found: {path}");

            try
            {
                await using var stream = File.OpenRead(path);
                var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptionsFactory.Default, ct);
                return result ?? throw new DataLayerException($"File '{fileName}' was empty or invalid.");
            }
            catch (JsonException ex)
            {
                throw new DataLayerException($"Invalid JSON in '{fileName}': {ex.Message}", ex);
            }
        }
    }
}
